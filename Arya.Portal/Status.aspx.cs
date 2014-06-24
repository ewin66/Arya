using System.Web;

namespace Arya.Portal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Arya.Framework.Common;
    using Arya.Framework.Common.ComponentModel;
    using Arya.Framework.Common.Extensions;
    using Arya.Framework.Data.AryaDb;
    using Arya.Framework.Data.Services;
    using Arya.Framework.IO.Exports;
    using Arya.Framework.IO.Imports;
    using Arya.Framework.Utility;

    public partial class Status : Page
    {
        #region Fields

        private readonly string _connStr = Util.GetAryaServicesConnectionString();

        private string _email;

        #endregion Fields

        #region Properties

        private DropDownList DdlJobType
        {
            get
            {
                try
                {
                    return (DropDownList) GvStatus.HeaderRow.FindControl("DdlJobType");
                }
                catch (Exception)
                {
                    return new DropDownList();
                }
            }
        }

        private DropDownList DdlProjects
        {
            get
            {
                try
                {
                    return (DropDownList) GvStatus.HeaderRow.FindControl("DdlProjects");
                }
                catch (Exception)
                {
                    return new DropDownList();
                }
            }
        }

        private DropDownList DdlStatus
        {
            get
            {
                try
                {
                    return (DropDownList) GvStatus.HeaderRow.FindControl("DdlStatus");
                }
                catch (Exception)
                {
                    return new DropDownList();
                }
            }
        }

        private DropDownList DdlSubmittedBy
        {
            get
            {
                try
                {
                    return (DropDownList) GvStatus.HeaderRow.FindControl("DdlSubmittedBy");
                }
                catch (Exception)
                {
                    return new DropDownList();
                }
            }
        }

        private string Email
        {
            get
            {
                return Session["email"].ToString();
            }
        }

        private string sortExpression
        {
            get { return (ViewState["sortExpression"] ?? string.Empty).ToString(); }
            set { ViewState["sortExpression"] = value; }
        }

        private string sortOrder
        {
            get { return (ViewState["sortOrder"] ?? "asc").ToString(); }
            set { ViewState["sortOrder"] = value; }
        }

        #endregion Properties

        #region Methods

        protected void btnloadImportArgs_Click(object sender, EventArgs e)
        {
            RedirectToPage(sender, "LoadArgs");
        }

        protected void btnloadImportArgs_DataBinding(object sender, EventArgs e)
        {
            SetEnabledForCompletedTasks((WebControl) sender);
        }

        protected void CancelCurrentJob(object sender, CommandEventArgs e)
        {
            var taskId = Guid.Parse(e.CommandArgument.ToString());
            using (var taskDc = new AryaServicesDbDataContext())
            {
                var task = taskDc.AryaTasks.First(t => t.ID == taskId);
                if (task.Status == WorkerState.New.ToString())
                {
                    task.Status = WorkerState.AbortedByUser.ToString();
                    taskDc.SubmitChanges();
                }
                else
                {
                    lblMessage.Text =
                        "Could not cancel Job - it had already started before cancel request was sent to server.";
                }
                BindGridView();
            }
        }

        protected void GvStatus_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            else
            {
                sortOrder = "asc";
                sortExpression = e.SortExpression;
            }

            BindGridView();
        }

        protected void lnkClearAllFilters_Click(object sender, EventArgs e)
        {
            BindGridView(true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated || Session["email"] == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            if (!Page.IsPostBack)
                BindGridView();

            lblMessage.Text = string.Empty;
        }

        protected void UpdateGridView(object sender, EventArgs e)
        {
            BindGridView();
        }

        private static IEnumerable<AryaTask> GetTasks(AryaDbDataContext aryaDc,
            AryaServicesDbDataContext taskDc, User currUser, Guid userGroup, IEnumerable<string> jobTypes,
            bool currentUserTasksOnly)
        {
            var projects =
                (from ug in aryaDc.UserProjects
                    where ug.GroupID == userGroup && ug.UserID == currUser.ID
                    select ug.ProjectID).ToList();
            return projects.Any()
                ? taskDc.AryaTasks.Where(
                    task =>
                        projects.Contains(task.ProjectID) && jobTypes.Contains(task.JobType)
                        && (!currentUserTasksOnly || task.SubmittedBy == currUser.ID))
                : Enumerable.Empty<AryaTask>();
        }

        private void BindGridView(bool clearAllFilters = false)
        {
            using (var aryaDc = new AryaDbDataContext())
            {
                var currUser = (from u in aryaDc.Users where u.EmailAddress == Email select u).FirstOrDefault();
                if (currUser == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                    return;
                }

                using (var taskDc = new AryaServicesDbDataContext())
                {
                    var tasks = Enumerable.Empty<AryaTask>().AsQueryable();
                    if (currUser.IsAdmin)
                        tasks = taskDc.AryaTasks;
                    else
                    {
                        //Import Admin Tasks
                        tasks =
                            tasks.Union(GetTasks(aryaDc, taskDc, currUser, Group.ImportAdminGroup,
                                new[] {typeof (ImportWorker).Name}, false));
                        //Import User Tasks
                        tasks =
                            tasks.Union(GetTasks(aryaDc, taskDc, currUser, Group.ImportUserGroup,
                                new[] {typeof (ImportWorker).Name}, true));

                        var exportJobTypes = ExportWorkerBase.GetExports().Select(e => e.GetType().Name).ToList();
                        //Export Admin Tasks
                        tasks =
                            tasks.Union(GetTasks(aryaDc, taskDc, currUser, Group.ExportAdminGroup,
                                exportJobTypes.Union(typeof (CustomExportWorker).Name), false));
                        //Standard Export User Tasks
                        tasks =
                            tasks.Union(GetTasks(aryaDc, taskDc, currUser, Group.StandardExportUserGroup,
                                exportJobTypes, true));
                        //Custom Export User Tasks
                        tasks =
                            tasks.Union(GetTasks(aryaDc, taskDc, currUser, Group.CustomExportUserGroup,
                                new[] {typeof (CustomExportWorker).Name}, true));
                    }

                    if (!clearAllFilters)
                    {
                        Guid selectedProject, selectedUser;
                        if (Guid.TryParse(DdlProjects.SelectedValue, out selectedProject))
                            tasks = tasks.Where(task => task.ProjectID == selectedProject);

                        if (Guid.TryParse(DdlSubmittedBy.SelectedValue, out selectedUser))
                            tasks = tasks.Where(task => task.SubmittedBy == selectedUser);

                        var selectedJobType = DdlJobType.SelectedValue;
                        if (!string.IsNullOrWhiteSpace(selectedJobType) && selectedJobType != "ALL")
                            tasks = tasks.Where(task => task.JobType == DdlJobType.SelectedValue);

                        var selectedStatus = DdlStatus.SelectedValue;
                        if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "ALL")
                            tasks = tasks.Where(task => task.Status == DdlStatus.SelectedValue);
                    }
                    var allTasks = tasks.OrderByDescending(task => task.SubmittedOn).Take(100).ToList();

                    var distinctUserProjects =
                        allTasks.Select(t => t.SubmittedBy + "|" + t.ProjectID).Distinct().ToList();
                    var userProjects = (from up in aryaDc.UserProjects
                        //from task in tasks.ToList()
                        //where task.SubmittedBy == up.UserID && task.ProjectID == up.ProjectID
                        where distinctUserProjects.Contains(up.UserID + "|" + up.ProjectID)
                        select new {up.User, up.Project}).ToList();

                    var visibleTasks = from task in allTasks
                        from up in userProjects
                        where up.Project.ID == task.ProjectID && up.User.ID == task.SubmittedBy
                        let project = up.Project
                        let user = up.User
                        let state =
                            WorkerBase.GetFriendlyWorkerState(
                                (WorkerState) Enum.Parse(typeof (WorkerState), task.Status))
                        orderby task.SubmittedOn descending
                        select
                            new
                            {
                                task.ID,
                                task.ProjectID,
                                ProjectName = project.DisplayName,
                                Type = task.DisplayJobType,
                                task.JobType,
                                task.Description,
                                task.Status,
                                FriendlyStatus = state,
                                user.FullName,
                                task.SubmittedBy,
                                task.SubmittedOn
                            };

                    if (sortExpression != string.Empty)
                    {
                        visibleTasks = LinqExtensions.ApplyOrder(visibleTasks, sortExpression,
                            sortOrder == "asc" ? "OrderBy" : "OrderByDescending");
                    }

                    var visibleTaskList = visibleTasks.Distinct().ToList();
                    GvStatus.DataSource = visibleTaskList;
                    GvStatus.DataBind();

                    if (visibleTaskList.Count == 0)
                        return;

                    if (!Page.IsPostBack)
                    {
                        var projects =
                            userProjects.Select(
                                up =>
                                    new DropDownListItem
                                    {
                                        Text = up.Project.DisplayName,
                                        Value = up.Project.ID.ToString()
                                    })
                                .Distinct(new KeyEqualityComparer<DropDownListItem>(item => item.Value))
                                .OrderBy(item => item.Text)
                                .ToList();
                        Session["projects"] = projects;

                        var users =
                            userProjects.Select(
                                up => new DropDownListItem {Text = up.User.FullName, Value = up.User.ID.ToString()})
                                .Distinct(new KeyEqualityComparer<DropDownListItem>(item => item.Value))
                                .OrderBy(item => item.Text)
                                .ToList();
                        Session["users"] = users;

                        var jobTypes =
                            allTasks.Select(
                                task => new DropDownListItem {Text = task.DisplayJobType, Value = task.JobType})
                                .Distinct(new KeyEqualityComparer<DropDownListItem>(item => item.Value))
                                .OrderBy(item => item.Text)
                                .ToList();
                        Session["jobTypes"] = jobTypes;

                        var statuses =
                            allTasks.Select(task => new DropDownListItem {Text = task.Status, Value = task.Status})
                                .Distinct(new KeyEqualityComparer<DropDownListItem>(item => item.Value))
                                .OrderBy(item => item.Text)
                                .ToList();
                        Session["statuses"] = statuses;
                    }

                    var prjs = Session["projects"] as List<DropDownListItem> ?? new List<DropDownListItem>();
                    var visibleProjects =
                        prjs.Where(prj => visibleTaskList.Any(task => task.ProjectID.ToString() == prj.Value)).ToList();
                    visibleProjects.Insert(visibleProjects.Count > 1 ? 0 : 1,
                        new DropDownListItem {Text = "ALL", Value = "ALL"});
                    visibleProjects.Add(new DropDownListItem {Text = "-----", Value = "ALL"});
                    visibleProjects.AddRange(prjs.Where(prj => !visibleProjects.Contains(prj)));
                    DdlProjects.DataSource = visibleProjects;
                    DdlProjects.DataBind();

                    var usrs = Session["users"] as List<DropDownListItem> ?? new List<DropDownListItem>();
                    var visibleusers =
                        usrs.Where(usr => visibleTaskList.Any(task => task.SubmittedBy.ToString() == usr.Value))
                            .ToList();
                    visibleusers.Insert(visibleusers.Count > 1 ? 0 : 1,
                        new DropDownListItem {Text = "ALL", Value = "ALL"});
                    visibleusers.Add(new DropDownListItem {Text = "-----", Value = "ALL"});
                    visibleusers.AddRange(usrs.Where(usr => !visibleusers.Contains(usr)));
                    DdlSubmittedBy.DataSource = visibleusers;
                    DdlSubmittedBy.DataBind();

                    var jbts = Session["jobTypes"] as List<DropDownListItem> ?? new List<DropDownListItem>();
                    var visiblejobTypes =
                        jbts.Where(jbt => visibleTaskList.Any(task => task.JobType == jbt.Value)).ToList();
                    visiblejobTypes.Insert(visiblejobTypes.Count > 1 ? 0 : 1,
                        new DropDownListItem {Text = "ALL", Value = "ALL"});
                    visiblejobTypes.Add(new DropDownListItem {Text = "-----", Value = "ALL"});
                    visiblejobTypes.AddRange(jbts.Where(jbt => !visiblejobTypes.Contains(jbt)));
                    DdlJobType.DataSource = visiblejobTypes;
                    DdlJobType.DataBind();

                    var stus = Session["statuses"] as List<DropDownListItem> ?? new List<DropDownListItem>();
                    var visiblestatuses =
                        stus.Where(stu => visibleTaskList.Any(task => task.Status == stu.Value)).ToList();
                    visiblestatuses.Insert(visiblestatuses.Count > 1 ? 0 : 1,
                        new DropDownListItem {Text = "ALL", Value = "ALL"});
                    visiblestatuses.Add(new DropDownListItem {Text = "-----", Value = "ALL"});
                    visiblestatuses.AddRange(stus.Where(stu => !visiblestatuses.Contains(stu)));
                    DdlStatus.DataSource = visiblestatuses;
                    DdlStatus.DataBind();
                }
            }
        }

        private void RedirectToPage(object sender, string pageName)
        {
            var gvr = (GridViewRow) ((Button) sender).NamingContainer;
            var id = ((Label) gvr.FindControl("lblID")).Text;
            Response.Redirect(string.Format("~/{1}.aspx?ID={0}", id, pageName));
        }

        private void SetEnabledForCompletedTasks(WebControl sender)
        {
            sender.Enabled = Eval("Status").ToString().Contains("Complete");
        }

        #endregion Methods

        #region Nested Types

        private class DropDownListItem
        {
            #region Properties

            public string Text
            {
                get; set;
            }

            public string Value
            {
                get; set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}