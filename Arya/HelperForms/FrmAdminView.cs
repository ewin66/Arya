using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Windows.Forms;
using DataGridViewAutoFilter;
using LinqKit;
using Arya.Data;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya.HelperForms
{
    public partial class FrmAdminView : Form
    {
        #region Constructors (1)

        public FrmAdminView()
        {
            InitializeComponent();
            Icon = Resources.AryaLogoIcon;
        }

        #endregion Constructors

        private void FrmAdminView_Load(object sender, EventArgs e)
        {
            //if (AryaTools.Instance.InstanceData.CurrentProject != null)
            //{
            //    var showRoleManager = AryaTools.Instance.InstanceData.CurrentUser.IsAdmin ||
            //                          AryaTools.Instance.InstanceData.CurrentUser.UIExclusions.Contains(UIObject.ShowRoleManager);

            //    var showPermissionsManager = AryaTools.Instance.InstanceData.CurrentUser.IsAdmin ||
            //                                 AryaTools.Instance.InstanceData.CurrentUser.UIExclusions.Contains(
            //                                     UIObject.ShowPermissionsManager);
            //    if (showRoleManager)
            //        rolesManagerControl1.Run(AryaTools.Instance.InstanceData.CurrentUser.IsAdmin ? Guid.Empty : AryaTools.Instance.InstanceData.CurrentUser.ID);

            //    if (showPermissionsManager)
            //        permissionsManagerControl1.Run(AryaTools.Instance.InstanceData.CurrentUser.ID, AryaTools.Instance.InstanceData.CurrentProject.ID);
            //}
            //else
            //{
            //    rolesManagerControl1.Run();
            //    permissionsManagerControl1.Enabled = false;
            //    tcAdminView.TabPages.Remove(tpPermissions);
            //}

            var activeDatabases =
                AryaTools.Instance.InstanceData.Dc.ExecuteQuery<string>("select name from sys.databases").ToList();

            var showRoleManager = AryaTools.Instance.InstanceData.CurrentUser.IsAdmin ||
                                  AryaTools.Instance.InstanceData.CurrentUser.UserProjects.Any(
                                             up =>
                                             up.GroupID == Group.RoleManagerGroup &&
                                             activeDatabases.Contains(up.Project.DatabaseName));

            var showPermissionsManager = AryaTools.Instance.InstanceData.CurrentUser.IsAdmin ||
                                         AryaTools.Instance.InstanceData.CurrentUser.UserProjects.Any(
                                             up =>
                                             up.GroupID == Group.PermissionsManagerGroup &&
                                             activeDatabases.Contains(up.Project.DatabaseName));
            if (showRoleManager)
                rolesManagerControl1.Run(AryaTools.Instance.InstanceData.CurrentUser.IsAdmin
                                             ? Guid.Empty
                                             : AryaTools.Instance.InstanceData.CurrentUser.ID);
            else
            {
                rolesManagerControl1.Enabled = false;
                tcAdminView.TabPages.Remove(tpRoleManager);
            }

            if (showPermissionsManager)
                permissionsManagerControl1.Run(AryaTools.Instance.InstanceData.CurrentUser.ID, AryaTools.Instance.InstanceData.CurrentUser.IsAdmin);

            else
            {
                permissionsManagerControl1.Enabled = false;
                tcAdminView.TabPages.Remove(tpPermissions);
            }
            if (tcAdminView.TabPages.Count == 0)
            {
                MessageBox.Show("You are not Role Manager or Project Manager in any project.");
                this.Close();
            }
        }

        private void FrmAdminView_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Pain to deal with when running from an IDE
            if(ApplicationDeployment.IsNetworkDeployed)
                Application.Restart();
        }

        
    }
}