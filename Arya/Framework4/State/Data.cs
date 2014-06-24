using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using Project = Arya.Data.Project;
using User = Arya.Data.User;
using UserProject = Arya.Data.UserProject;

namespace Arya.Framework4.State
{
    public class InstanceData
    {
        private Project _currentProject;
        private UserProject _currentUserProject;
        private SkuDataDbDataContext _dc;
        private HashSet<string> _globalAttributeNames;
        public User CurrentUser { get; set; }

        public SkuDataDbDataContext Dc
        {
            get
            {
                InitDataContext();
                return _dc;
            }
            set { _dc = value; }
        }

        public UserProjectsPreferences CurrentUserProjectsPreferences
        {
            get
            {
                return CurrentUserProject.UserProjectPreferences;
            }
            set { CurrentUserProject.UserProjectPreferences = value; }
        }

        private UserProject CurrentUserProject
        {
            get
            {
                if (_currentUserProject == null)
                {
                    List<UserProject> ups = (from up in Dc.UserProjects
                                             where up.UserID == CurrentUser.ID && up.ProjectID == CurrentProject.ID
                                             select up).ToList();
                    _currentUserProject = ups.FirstOrDefault(up => up.Preferences != null) ??
                                          ups.OrderBy(up => up.GroupID).FirstOrDefault();
                }
                return _currentUserProject ?? new UserProject();
            }
        }

        public HashSet<string> GlobalAttributeNames
        {
            get
            {
                return _globalAttributeNames ??
                       (_globalAttributeNames = new HashSet<string>(from att in Dc.Attributes
                                                                    where att.AttributeType == AttributeTypeEnum.Global.ToString()
                                                                    select att.AttributeName));
            }
        }

        public Project CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;

               

                if (value.DatabaseName == null)
                    return;

                //Load User's Groups
                CurrentUser.UserGroups =
                    CurrentUser.UserProjects.Where(
                        p =>
                        p.ProjectID == _currentProject.ID &&
                        p.Group.GroupType.StartsWith("USER_", StringComparison.OrdinalIgnoreCase))
                               .Select(p => p.GroupID)
                               .Distinct().ToList();


                Dc.Connection.ChangeDatabase(_currentProject.DatabaseName);

                _currentProject = Dc.Projects.Where(prj => prj.ID.Equals(_currentProject.ID)).First();
                CurrentUser = Dc.Users.Where(usr => usr.ID.Equals(CurrentUser.ID)).First();
                Program.WriteToErrorFile("Project: " + _currentProject.ProjectName + "\nUser: " + CurrentUser.FullName, true);
                List<UserProject> userProjects =
                    //This line has to execute to update the DataContext cache with updated UserPreferences
                    Dc.UserProjects.Where(up => up.ProjectID == _currentProject.ID && up.UserID == CurrentUser.ID).
                       ToList();
                Dc.Refresh(RefreshMode.OverwriteCurrentValues, userProjects);

                //ClearCache(Dc); //Do not use this pattern - it causes Attach Entity errors all over the place!
            }
        }

        public bool InitDataContext(Guid projectID = default(Guid), bool forceInit = false)
        {
            if (_dc == null || forceInit)
            {
                _dc = new SkuDataDbDataContext();

                if (CurrentUser != null)
                    CurrentUser = _dc.Users.Single(usr => usr.ID.Equals(CurrentUser.ID));

                if (projectID != default(Guid))
                    CurrentProject = _dc.Projects.Single(prj => prj.ID == projectID);
                else if (CurrentProject != null)
                    CurrentProject = _dc.Projects.Single(prj => prj.ID.Equals(_currentProject.ID));
            }

            while (_dc.Connection.State != ConnectionState.Open)
            {
                if (_dc.Connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        _dc.Connection.Open();
                    }
                    catch (Exception ex)
                    {
                        DialogResult result = MessageBox.Show(ex.Message + @" Do you want to try again?",
                                                              @"Database connection lost", MessageBoxButtons.YesNo);
                        if (result == DialogResult.No)
                        {
                            Application.Exit();
                            return false;
                        }
                    }
                }

                while (_dc.Connection.State == ConnectionState.Connecting)
                    Thread.Sleep(100);
            }

            return true;
        }


        //Do not use this pattern - it causes Attach Entity errors all over the place!
        //public static void ClearCache(SkuDataDbDataContext context)
        //{
        //    const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        //    var method = context.GetType().GetMethod("ClearCache", FLAGS);
        //    method.Invoke(context, null);
        //}
    }
}