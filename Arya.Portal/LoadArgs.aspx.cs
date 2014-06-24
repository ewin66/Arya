using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO;
using Arya.Framework.IO.Imports;
using Arya.Framework.Utility;
using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Arya.Framework.Extensions;

namespace Arya.Portal
{
    public partial class LoadArgs : Page
    {
        private List<string> _allFields;
        private string _email;
        private Dictionary<string, int> dict = new Dictionary<string, int>();

        private string Email
        {
            get
            {
                if (_email == null)
                {
                    var response = Session["FetchResponse"] as FetchResponse;
                    _email = response.GetAttributeValue(WellKnownAttributes.Contact.Email) ?? "N/A";
                }
                return _email;
            }
        }

        private List<string> AllFields
        {
            get
            {
                if (_allFields == null)
                    _allFields = GetPossibleFieldNames();
                return _allFields;
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            string descr = Session["Description"].ToString();
            txtProjectDesc.Text = descr;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            GetAllFieldDelimiters();
            DisplayProjectList();
            LoadExistingImportFields();
            if (!Page.IsPostBack)
            {
               string newDescription = txtProjectDesc.Text;
               PopulateImportOptions();
            }
          
        }

        protected void DisplayProjectList()
        {
            var availableProjects = new List<ListItem>();
            using (var Dc = new AryaDbDataContext())
            {
                var currUser = (from u in Dc.Users
                                where u.EmailAddress == Email
                                select u).Single();
                var currentUserID = currUser.ID;
                var currentUserName = currUser.FullName;
                var userRole = currUser.IsAdmin;
                userId.Value = currentUserID.ToString();
                userName.Value = currentUserName;

                var existingDatabases = Dc.ExecuteQuery<string>("select name from sys.databases").ToList();
                if (!userRole)
                {
                    var projects = (from ug in currUser.UserProjects
                                    where
                                        ug.GroupID == Arya.Framework.Data.AryaDb.Group.ImportAdminGroup
                                        ||
                                        ug.GroupID == Arya.Framework.Data.AryaDb.Group.ImportUserGroup
                                        && existingDatabases.Contains(ug.Project.DatabaseName)
                                    orderby ug.Project.DatabaseName ascending
                                    select ug.Project).Distinct();
                    availableProjects =
                        projects.Select(p => new ListItem {Text = p.ClientDescription + " " + p.SetName}).ToList();
                }
                else
                {
                    var projects = (from p in Dc.Projects
                                    where existingDatabases.Contains(p.DatabaseName)
                                    orderby p.DatabaseName ascending
                                    select p);
                    availableProjects = projects.Select(p => new ListItem { Text = p.ClientDescription + " " + p.SetName }).Distinct().ToList();
                }
                ddlProject.Items.Insert(0, "Select a Project");
                availableProjects.ForEach(ap => ddlProject.Items.Add(ap));
               
            }
        }

        private void GetAllFieldDelimiters()
        {
            var itemNames = Enum.GetNames(typeof (Delimiter));
            ddlDelimiter.DataSource = itemNames;
            ddlDelimiter.DataBind();
        }

        private List<string> GetPossibleFieldNames()
        {
            var allImports = ImportWorkerBase.GetAvailableImports();
            var fields =
                allImports.SelectMany(i => Enumerable.AsEnumerable(i.GetRequiredFields())).Union(
                    allImports.SelectMany(i => Enumerable.AsEnumerable(i.GetOptionalFields())));

            var allFields = fields.Distinct().OrderBy(a => a).ToList();
            allFields.Insert(0, "_ignore_");
            return allFields;
        }

        protected void LoadExistingImportFields()
        {
            string connStr = Arya.Framework.Utility.Util.GetAryaServicesConnectionString();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            string basepath = ConfigurationManager.AppSettings["BasePath"].ToString();
            var id = Request.QueryString["ID"].ToString();
            var projectID = Request.QueryString["ProjectID"].ToString();
            using (var Dc = new AryaDbDataContext())
            {

                var projectName = (from p in Dc.Projects
                                   where p.ID == Guid.Parse(projectID)
                                   select p.ClientDescription + ' ' + p.SetName).Single();
                ddlProject.SelectedItem.Text = projectName;
            }
            projectId.Value = projectID;
            //string description = Session["Description"].ToString();
            //txtProjectDesc.Text = description;
            var cmd1 = new SqlCommand();
            cmd1.CommandText = "Select ArgumentDirectoryPath from AryaTask Where ID= @id";
            cmd1.CommandType = CommandType.Text;
            cmd1.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(id);
           //cmd1.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            sqlConnection.Close();
            cmd1.Connection = sqlConnection;
            sqlConnection.Open();
            var xmlpathValue = cmd1.ExecuteScalar().ToString();
            var path = Path.Combine(basepath, xmlpathValue);
            var ArgumentDirectoryPath = Path.Combine(path, WorkerArguments.ArgumentsFileName);
            ReadDataFromXMLFile(ArgumentDirectoryPath);
            sqlConnection.Close();
          }
     
        private void ReadHeaderFromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var line = reader.ReadLine();
                var parts = line.Split('\t');
                gv_Fields.DataSource = (from p in parts
                                        select new {HeaderColumn = p, MappingFields = AllFields}).ToList();
                gv_Fields.DataBind();
                reader.Close();
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string newDescription = txtProjectDesc.Text;
            if (!inputFileUpload.HasFile)
            {
                lblUploadResult.Text = "Click 'Browse' to select the file to upload.";
                return;
            }
            var fileID = Guid.NewGuid();
            folderGuid.Value = fileID.ToString();
            inputFilename.Value = inputFileUpload.FileName;
            var basepath = ConfigurationManager.AppSettings["BasePath"];
            var path = Path.Combine(basepath, projectId.Value);
            var fileName = inputFileUpload.FileName;
            var strFileExtension = Path.GetExtension(fileName);
            if (strFileExtension == ".txt")
            {
                var directory = Path.Combine(path, fileID.ToString());

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var file = Path.Combine(directory, fileName);
                if (!File.Exists(file))
                {
                    inputFileUpload.SaveAs(file);
                    lblUploadResult.Text = fileName + " has been successfully uploaded.";
                }
                else
                    lblUploadResult.Text = fileName + " already exists on the server!";

                inputFileLocation.Value = file;
                ReadHeaderFromFile(file);
            }
            frmConfirmation.Visible = true;
            foreach (GridViewRow grdRow in gv_Fields.Rows)
            {
                var i = grdRow.RowIndex;
                var drdList = (DropDownList) (gv_Fields.Rows[grdRow.RowIndex].Cells[1].FindControl("DropDownList1"));
                drdList.DataSource = GetPossibleFieldNames();
                drdList.DataBind();
            }
            MapFields(dict);
        }

        protected void gv_Fields_SelectedIndexChanged(Object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv_Fields.Rows)
            {
                var drdList = (DropDownList) row.Cells[1].FindControl("DropDownList1");
                drdList.DataSource = GetPossibleFieldNames();
                drdList.DataBind();
            }
        }

        protected void MapFields(Dictionary<string, int> fieldmapping)
        {
            DropDownList drdList;
            foreach (GridViewRow grdRow in gv_Fields.Rows)
            {
                var i = grdRow.RowIndex;
                drdList = (DropDownList) (gv_Fields.Rows[grdRow.RowIndex].Cells[1].FindControl("DropDownList1"));
                var dictItem = fieldmapping.Where(item => item.Value == i);
                drdList.SelectedItem.Text = (dictItem.Count() == 1) ? dictItem.First().Key : "_ignore_";
            }
        }

        protected void ReadDataFromXMLFile(string fileName)
        {
            var serializer = new SharpSerializer();
            var importArgs = new ImportArgs();
            importArgs = (ImportArgs) serializer.Deserialize(fileName);
            var fieldDelimiter = importArgs.FieldDelimiter.ToString();
            if (fieldDelimiter != null)
                ddlDelimiter.SelectedItem.Text = fieldDelimiter;
            var importOptions = importArgs.CurrentImportOptions.ToString().Spacify();
            var io = importOptions.Split(new[] {','}, StringSplitOptions.None);
            chkBoxImportType.DataSource = io;
            chkBoxImportType.DataBind();
            foreach (var i in io)
            {
                switch (i.Trim())
                {
                    case "CreateMissingSkus":
                         chkBoxImportType.Items.FindByValue(i.ToString()).Selected = true;
                         break;

                    case "CreateMissingTaxonomies":
                         chkBoxImportType.Items.FindByValue(i.ToString()).Selected = true;
                         break;

                    case "CreateMissingAttributes":
                         chkBoxImportType.Items.FindByValue(i.ToString()).Selected = true;
                        break;

                    case "CreateMissingMetaAttributes":
                        chkBoxImportType.Items.FindByValue(i.ToString()).Selected = true;
                        break;

                    case "CreateMissingValues":
                        chkBoxImportType.Items.FindByValue(i.ToString()).Selected = true;
                        break;
                }
            }
           
            dict = importArgs.FieldMappings;
        }

        protected void InsertIntoTable()
        {
            var connStr = Arya.Framework.Utility.Util.GetAryaServicesConnectionString();
            var sqlConnection = new SqlConnection(connStr);
            var @status = "New";
            var @submittedBy = Guid.Parse(userId.Value);
            var @submittedOn = DateTime.Now;
            var @ArgumentDirectoryPath = Path.Combine(projectId.Value, folderGuid.Value);
            var @description = txtProjectDesc.Text;
            var @projectID = Guid.Parse(projectId.Value);
            var @ID = Guid.NewGuid();
            var @lastupdateOn = DateTime.Now;
            var @jobType = "ImportWorker";
            var insertStmt =
                "INSERT INTO AryaTask(ID, ProjectID, Description, ArgumentDirectoryPath, Status, SubmittedBy, SubmittedOn, LastUpdateOn, JobType)"
                +
                "VALUES(@ID, @projectID, @description, @ArgumentDirectoryPath, @status, @submittedBy, @submittedOn, @lastupdateOn, @jobType)";
            var command = new SqlCommand(insertStmt, sqlConnection);
            command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = ID;
            command.Parameters.Add("@ProjectID", SqlDbType.UniqueIdentifier).Value = projectID;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            command.Parameters.Add("@SubmittedBy", SqlDbType.UniqueIdentifier).Value = submittedBy;
            command.Parameters.Add("@SubmittedOn", SqlDbType.DateTime).Value = submittedOn;
            command.Parameters.Add("@ArgumentDirectoryPath", SqlDbType.VarChar).Value = ArgumentDirectoryPath;
            command.Parameters.Add("@Status", SqlDbType.VarChar).Value = status;
            command.Parameters.Add("@LastUpdateOn", SqlDbType.DateTime).Value = lastupdateOn;
            command.Parameters.Add("@JobType", SqlDbType.VarChar).Value = jobType;
            sqlConnection.Open();
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }

        private void PopulateImportOptions()
        {
            foreach (ImportOptions io in Enum.GetValues(typeof(ImportOptions)))
            {
                if(io == ImportOptions.None || io == ImportOptions.CreateMissingMetaAttributes)
                    continue;
                chkBoxImportType.Items.Add(new ListItem(io.ToString(), io.ToString()));
            }
        }

        private ImportOptions SelectImportType(ImportOptions io)
        {
            foreach (ListItem chk in chkBoxImportType.Items)
            {
                if (chk.Selected)
                    io = io | (ImportOptions)Enum.Parse(typeof(ImportOptions), chk.Value);
            }
            return io;
        }

        protected Dictionary<string, int> FieldMappings()
        {
            DropDownList drdList;
            var fieldMappings = new Dictionary<string, int>();
            foreach (GridViewRow grdRow in gv_Fields.Rows)
            {
                var i = Int32.Parse(grdRow.RowIndex.ToString());
                drdList = (DropDownList) (gv_Fields.Rows[grdRow.RowIndex].Cells[1].FindControl("DropDownList1"));
                if (!fieldMappings.ContainsKey(drdList.SelectedItem.Text)
                    && (!drdList.SelectedItem.Text.Equals("_ignore_")))
                    fieldMappings.Add(drdList.SelectedItem.Text, i);
            }
            return fieldMappings;
        }

        private void GetAllValues()
        {
            //This method should be called when the Save or Load button is clicked by the user
            var ia = new ImportArgs();
            ia.InputFilePath = inputFileLocation.Value;
            ia.FieldDelimiter = (Delimiter) Enum.Parse(typeof (Delimiter), ddlDelimiter.SelectedValue);
            Session["FieldDelimiter"] = ddlDelimiter.SelectedValue;
            ia.ProjectId = Guid.Parse(projectId.Value);
            ia.UserId = Guid.Parse(userId.Value);
            ia.NotificationEmailAddresses = notificationEmail.Value;
            ia.UpdateFrequency = 4000;
            ia.FieldMappings = FieldMappings();
            ia.JobDescription = txtProjectDesc.Text;
            Session["FieldMappings"] = ia.FieldMappings;
            var io = ImportOptions.None;
            io = SelectImportType(io);
            Session["ImportType"] = io;
            ia.CurrentImportOptions = io;

            var iaFileName = Path.Combine(new FileInfo(inputFileLocation.Value).Directory.FullName,
                                          WorkerArguments.ArgumentsFileName);
            var settings = ia.GetSharpSerializerXmlSettings(ImportArgs.ArgumentsFileRootName);
            var serializer = new SharpSerializer(settings);
            serializer.Serialize(ia, iaFileName);
            importArgs.Value = iaFileName;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            GetAllValues();
            InsertIntoTable();
            Response.Redirect("ImportStatus.aspx?ProjectID=" + projectId.Value, false);
        }

    }
}