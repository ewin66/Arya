using System.Web;

namespace Arya.Portal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml.Serialization;

    using Arya.Framework.Common;
    using Arya.Framework.Common.Extensions;
    using Arya.Framework.Data.AryaDb;
    using Arya.Framework.Data.Services;
    using Arya.Framework.Extensions;
    using Arya.Framework.IO.Exports;
    using Arya.Framework.Utility;

    using Polenter.Serialization;

    using Parameter = Arya.Framework.IO.Exports.Parameter;

    public partial class Export : Page
    {
        #region Fields

        private Type _exportType;
        private int _exportTypeSelectedIndex;

        #endregion Fields

        #region Properties

        private Type ExportType
        {
            get
            {
                if (_exportTypeSelectedIndex == ddlExportType.SelectedIndex)
                    return _exportType;

                _exportTypeSelectedIndex = ddlExportType.SelectedIndex;

                if (ddlExportType.SelectedIndex == 0)
                    return _exportType = null;

                var worker = ExportWorkerBase.GetExports(ddlExportType.SelectedItem.Value).FirstOrDefault();
                return _exportType = (worker == null ? typeof(CustomExportWorker) : worker.GetType());
            }
        }

        #endregion Properties

        #region Methods

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            SelectQueries(false);
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectQueries(true);
        }

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var args = SaveParameters();
            if (args == null)
            {
                LblMessage.Text = "There was a problem saving the parameters.";
                return;
            }

            var task = AryaServices.CreateAryaTask(Guid.NewGuid(), Guid.Parse(ddlModel.SelectedValue), args.JobDescription,
                Path.Combine(ddlModel.SelectedValue, args.Id.ToString()), Guid.Parse(UserId.Value), ExportType);

            AryaServices.SendEmail(task, false, null, ddlModel.SelectedItem.Text, ((SiteMaster)Page.Master).Email, "http://" + Request.Url.Authority + Request.ApplicationPath);
            //Response.Redirect("Status.aspx?ProjectID=" + ddlModel.SelectedValue + "&JobType=" + ExportType);
            Response.Redirect("Status.aspx");
        }

        protected void ddlExportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlCustomExportParameters.Visible = false;
            pnlStandardExportParameters.Visible = false;
            BtnSubmit.Visible = false;

            if (ExportType == null)
                return;

            pnlCustomExportParameters.Visible = ExportType == typeof(CustomExportWorker);
            pnlStandardExportParameters.Visible = (typeof(ExportWorkerBase)).IsAssignableFrom(ExportType);
            BtnSubmit.Visible = true;

            if (ExportType == typeof(CustomExportWorker))
            {
                //Custom Export
                LoadCustomExportParameters();
            }
            else
            {
                //Standard Export
                LoadStandardExportParameters();
            }
            ddlModel.Enabled = false;
            ddlExportType.Enabled = false;

            ddlSourceType.Attributes["onchange"] = string.Format("setSourceItemsLabel(this.value);");
        }

        protected void ddlModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            FetchExportTypes();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LblMessage.Text = string.Empty;

            if (!HttpContext.Current.User.Identity.IsAuthenticated || Session["email"] == null)
            {
                //This should never be called!
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            if (!Page.IsPostBack)
            {
                GetAllSourceTypes();
                FetchProjects();
            }

            TryUpdateCustomExportArgs();
            TryUpdateStandardExportArgs();
        }

        protected void Parameters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var parameter = (Parameter)e.Row.DataItem;

            if (parameter == null)
                return;

            if (parameter.Hidden)
                e.Row.Style.Add(HtmlTextWriterStyle.Display, "None");
        }

        protected void RptQueries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var dataItem = (CustomQuery)e.Row.DataItem;

            var lb = e.Row.FindControl("LblDescription") as Label;
            if (lb != null)
                lb.Text = dataItem.Description ?? string.Empty;

            // Individual Queries
            var cb = e.Row.FindControl("ChkExecute") as CheckBox;
            if (cb != null)
            {
                cb.Text = dataItem.Name;
                cb.Checked = dataItem.Execute;
            }

            // Global Parameters
            var gv = e.Row.FindControl("GrdParameters") as GridView;
            if (gv != null)
            {
                gv.DataSource = dataItem.Parameters;
                gv.DataBind();
            }
        }

        private static string CustomExportPath(Guid selectedProjectId)
        {
            return Path.Combine(ConfigurationManager.AppSettings["CustomExportBasePath"], selectedProjectId.ToString());
        }

        private CustomExportArgs CustomExportArgs(string argFilePath)
        {
            var customQueries = ViewState["customQueries"] as CustomExportArgs;
            if (customQueries == null)
                return null;

            customQueries.ProjectId = Guid.Parse(ddlModel.SelectedValue);
            customQueries.UserId = Guid.Parse(UserId.Value);
            customQueries.NotificationEmailAddresses = ((SiteMaster)Page.Master).Email;
            customQueries.JobDescription = txtExportDescription.Text;
            customQueries.NotificationEmailAddresses = txtEmailAddresses.Text;

            return customQueries;
        }

        private CustomExportArgs DeserializeFile(string customExportFileName)
        {
            try
            {
                CustomExportArgs data;
                var filePath = CustomExportPath(Guid.Parse(ddlModel.SelectedValue));
                if (filePath == null)
                    filePath = CustomExportPath(Guid.Empty);

                using (TextReader file = new StreamReader(Path.Combine(filePath, customExportFileName)))
                {
                    var serializer = new XmlSerializer(typeof(CustomExportArgs));
                    data = serializer.Deserialize(file) as CustomExportArgs;
                }
                return data;
            }
            catch (Exception ex)
            {
                LblMessage.Text = ex.Message;
            }
            return null;
        }

        private void FetchExportTypes()
        {
            ddlExportType.Items.Clear();
            ddlExportType.Items.Add(new ListItem("---Select an Export Type---", "---Select an Export Type---"));

            Guid selectedProjectId;
            if (!Guid.TryParse(ddlModel.SelectedValue, out selectedProjectId))
                return;

            using (var dc = new AryaDbDataContext())
            {
                var standardExports = new List<ListItem>();
                var customExports = new List<ListItem>();
                var globalExports = new List<ListItem>();

                var groups = (from up in dc.UserProjects
                              where up.ProjectID == selectedProjectId && up.User.EmailAddress == ((SiteMaster)Page.Master).Email
                              select new { up.User.IsAdmin, up.GroupID }).ToList();

                // get standard exports
                if (groups.Any(g => g.IsAdmin) || groups.Any(g => g.GroupID == Group.ExportAdminGroup)
                    || groups.Any(g => g.GroupID == Group.StandardExportUserGroup))
                {
                    standardExports = (from export in ExportWorkerBase.GetExports()
                                       let type = export.GetType()
                                       let displayName =
                                           type.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                               .Select(att => ((DisplayNameAttribute)att).DisplayName)
                                               .FirstOrDefault() ?? type.Name.Remove(0, 15).Spacify()
                                       select new ListItem(displayName, type.FullName)).ToList();

                    standardExports = standardExports.OrderBy(li => li.Text.ToLower()).ToList();
                }

                // get custom exports
                if (groups.Any(g => g.IsAdmin) || groups.Any(g => g.GroupID == Group.ExportAdminGroup)
                    || groups.Any(g => g.GroupID == Group.CustomExportUserGroup))
                {
                    var customExportPath = CustomExportPath(selectedProjectId);
                    if (!Directory.Exists(customExportPath))
                        Directory.CreateDirectory(customExportPath);
                    var exports = Directory.EnumerateFiles(customExportPath, "*.xml");

                    customExports =
                       (from export in exports
                        select new ListItem(Path.GetFileNameWithoutExtension(export).Spacify(), export)).ToList();

                    if (!groups.Any(g => g.IsAdmin || g.GroupID == Group.ExportAdminGroup))
                        customExports = customExports.Where(li => !li.Text.ToLower().StartsWith("test")).ToList();

                    customExports = customExports.OrderBy(li => li.Text.ToLower()).ToList();
                }

                // get global exports
                if (groups.Any(g => g.IsAdmin) || groups.Any(g => g.GroupID == Group.ExportAdminGroup)
                    || groups.Any(g => g.GroupID == Group.CustomExportUserGroup))
                {
                    var customExportPath = CustomExportPath(Guid.Empty);
                    if (!Directory.Exists(customExportPath))
                        Directory.CreateDirectory(customExportPath);
                    var exports = Directory.EnumerateFiles(customExportPath, "*.xml");

                    globalExports =
                       (from export in exports
                        select new ListItem(Path.GetFileNameWithoutExtension(export).Spacify(), export)).ToList();

                    if (!groups.Any(g => g.IsAdmin || g.GroupID == Group.ExportAdminGroup))
                        globalExports = globalExports.Where(li => !li.Text.ToLower().StartsWith("test")).ToList();

                    globalExports = globalExports.OrderBy(li => li.Text.ToLower()).ToList();
                }

                var allExportTypes = standardExports.Union(customExports).Union(globalExports).ToArray();
                ddlExportType.Items.AddRange(allExportTypes);

                lblExportType.Visible = ddlExportType.Visible = true;
            }
        }

        private void FetchProjects()
        {
            using (var dc = new AryaDbDataContext())
            {
                var currUser =
                    (from u in dc.Users where u.EmailAddress == ((SiteMaster)Page.Master).Email select u)
                        .FirstOrDefault();
                if (currUser == null)
                    throw new UnauthorizedAccessException();

                UserId.Value = currUser.ID.ToString();

                var p = currUser.IsAdmin
                    ? dc.Projects
                    : currUser.UserProjects.Where(
                        dp =>
                            dp.GroupID == Group.ExportAdminGroup || dp.GroupID == Group.StandardExportUserGroup
                            || dp.GroupID == Group.CustomExportUserGroup).Select(dp => dp.Project);

                var existingDatabases = dc.ExecuteQuery<string>("SELECT LOWER(name) FROM sys.databases").ToList();
                p = p.Where(dp => existingDatabases.Contains(dp.DatabaseName.ToLower())).Select(dp => dp);

                var availableProjects =
                    p.Select(
                        dp =>
                            new ListItem
                            {
                                Text = string.Format("{0} {1}", dp.ClientDescription, dp.SetName),
                                Value = dp.ID.ToString()
                            }).Distinct().OrderBy(li => li.Text).ToList();

                availableProjects.Insert(0, new ListItem("---Select a Model---", "---Select a Model---"));
                ddlModel.DataSource = availableProjects;
                ddlModel.DataBind();

                if (availableProjects.Count == 1)
                {
                    ddlModel.SelectedIndex = 1;
                    FetchExportTypes();
                }
            }
        }

        private void GetAllSourceTypes()
        {
            var itemNames = Enum.GetNames(typeof(ExportWorkerBase.SourceType));
            foreach (var item in itemNames)
                ddlSourceType.Items.Add(new ListItem(item));
        }

        private string GetDatabaseName(Guid projectId)
        {
            string results = String.Empty;
            using (var dc = new AryaDbDataContext())
            {
                var project = dc.Projects.FirstOrDefault(p => p.ID == projectId);
                if (project != null)
                {
                    results = project.DatabaseName;
                }
            }
            return results;
        }

        private void LoadCustomExportParameters()
        {
            LblMessage.Text = string.Empty;

            var customQueries = DeserializeFile(ddlExportType.SelectedValue);
            if (customQueries == null)
            {
                pnlCustomExportParameters.Visible = false;
                return;
            }

            customQueries.Queries.ForEach(query => { query.Execute = true; });

            //Look for default GUIDs
            foreach (var parameter in customQueries.GlobalParameters.Where(parameter => parameter.NewId))
                parameter.Value = Guid.NewGuid().ToString();

            foreach (var parameter in
                customQueries.Queries.SelectMany(query => query.Parameters.Where(parameter => parameter.NewId)))
                parameter.Value = Guid.NewGuid().ToString();

            ViewState["customQueries"] = customQueries;

            lblDescription.Text = customQueries.Description ?? string.Empty;
            txtEmailAddresses.Text = ((SiteMaster)Page.Master).Email;
            ddExportType.SelectedIndex = customQueries.ExportExcelFiles ? 1 : 0;
            chkEmptyFiles.Checked = customQueries.ExportEmptyFiles;
            chkQueriesOnly.Checked = customQueries.GenerateQueriesOnly;
            chkQueriesOnly.Visible = customQueries.GenerateQueriesOnly;
            if (customQueries.DatabaseNames == "*")
                customQueries.DatabaseNames = GetDatabaseName(new Guid(ddlModel.SelectedValue));

            var delimiterItem = ddDelimiter.Items.FindByValue(customQueries.Delimiter);
            if (delimiterItem != null)
                delimiterItem.Selected = true;

            GrdGlobalParameters.DataSource = customQueries.GlobalParameters;
            GrdGlobalParameters.DataBind();

            RptQueries.DataSource = customQueries.Queries;
            RptQueries.DataBind();
        }

        private void LoadStandardExportParameters()
        {
            var exportWorker = (ExportWorkerBase)Activator.CreateInstance(ExportType, new string[] { null });
            var exportArgsType = exportWorker.ArgumentsType;
            var args = (ExportArgs)Activator.CreateInstance(exportArgsType);

            foreach (var property in exportArgsType.GetProperties())
            {
                try
                {
                    var defaultValue = property.DefaultValue();
                    if (defaultValue != null)
                        property.SetValue(args, defaultValue);
                }
                catch (ArgumentException)
                {
                    //Cannot set this property. continue;
                }
            }

            args.ProjectId = Guid.Parse(ddlModel.SelectedValue);
            args.UserId = Guid.Parse(UserId.Value);
            args.NotificationEmailAddresses = ((SiteMaster)Page.Master).Email;
            args.JobDescription = ExportType.Name.Spacify();
            args.BaseFilename = DateTime.Now.ToString("yyyyMMdd-HHmm") + "-" +
                                ddlExportType.SelectedItem.Text;

            exportArgsEditor.SelectedObject = args;
            ViewState["exportArguments"] = args;
        }

        private string PrepareDirectory(Guid fileId)
        {
            var path = Path.Combine(ConfigurationManager.AppSettings["BasePath"],
                Guid.Parse(ddlModel.SelectedValue).ToString(), fileId.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private WorkerArguments SaveParameters()
        {
            var taskId = Guid.NewGuid();
            var argFilePath = PrepareDirectory(taskId);

            WorkerArguments args;
            if (ExportType == typeof(CustomExportWorker))
                args = CustomExportArgs(argFilePath);
            else
                args = StandardExportArgs(argFilePath);

            if (args == null)
                return null;

            args.Id = taskId;
            var eaFileName = Path.Combine(argFilePath, WorkerArguments.ArgumentsFileName);
            var settings = args.GetSharpSerializerXmlSettings(WorkerArguments.ArgumentsFileRootName);
            var serializer = new SharpSerializer(settings);
            serializer.Serialize(args, eaFileName);

            return args;
        }

        private void SelectQueries(bool selectAll)
        {
            var customQueries = ViewState["customQueries"] as CustomExportArgs;
            if (customQueries == null)
                return;

            customQueries.Queries.ForEach(q => q.Execute = selectAll);
            ViewState["customQueries"] = customQueries;

            RptQueries.DataSource = customQueries.Queries;
            RptQueries.DataBind();
        }

        private ExportArgs StandardExportArgs(string argFilePath)
        {
            var args = (ExportArgs)(exportArgsEditor.SelectedObject);

            var sources = txtSourceData.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            if (ddlSourceType.SelectedValue == ExportWorkerBase.SourceType.Taxonomy.ToString())
                args.TaxonomyPaths = sources;
            else //ExportWorkerBase.SourceType.SkuList
                args.ItemIds = sources;

            return args;
        }

        /// <summary>
        /// Update the CustomQueries object in the ViewState to reflect the data in the UI
        /// </summary>
        private void TryUpdateCustomExportArgs()
        {
            var customQueries = ViewState["customQueries"] as CustomExportArgs;

            if (customQueries == null)
                return;

            //Export Type
            customQueries.ExportExcelFiles = ddExportType.SelectedValue == "Excel";

            //Empty Files
            customQueries.ExportEmptyFiles = chkEmptyFiles.Checked;

            //Generate Queries only
            customQueries.GenerateQueriesOnly = chkQueriesOnly.Checked;

            //Delimiter
            customQueries.Delimiter = ddDelimiter.SelectedValue == "\\t" ? "\t" : ddDelimiter.SelectedValue;

            // Global Parameters
            for (var p = 0; p < customQueries.GlobalParameters.Count; p++)
            {
                var param = (TextBox)GrdGlobalParameters.Rows[p].FindControl("TxtValue");
                customQueries.GlobalParameters[p].Value = param.Text;
            }

            // Individual Queries
            for (var i = 0; i < customQueries.Queries.Count; i++)
            {
                var query = customQueries.Queries[i];
                var chk = (CheckBox)RptQueries.Rows[i].FindControl("ChkExecute");
                query.Execute = chk.Checked;

                var grid = (GridView)RptQueries.Rows[i].FindControl("GrdParameters");
                for (var p = 0; p < query.Parameters.Count; p++)
                {
                    var param = (TextBox)grid.Rows[p].FindControl("TxtValue");
                    query.Parameters[p].Value = param.Text;
                }
            }

            ViewState["customQueries"] = customQueries;
        }

        //private ExportArgs _arguments;
        private void TryUpdateStandardExportArgs()
        {
            var exportArgs = ViewState["exportArguments"] as ExportArgs;
            if (exportArgs == null)
                return;

            exportArgsEditor.SelectedObject = exportArgs;
        }

        #endregion Methods
    }
}