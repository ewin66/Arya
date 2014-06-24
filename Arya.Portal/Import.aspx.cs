namespace Arya.Portal
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

    using Arya.Framework.Common;
    using Arya.Framework.Data.AryaDb;
    using Arya.Framework.Data.Services;
    using Arya.Framework.Extensions;
    using Arya.Framework.IO.Imports;
    using Arya.Framework.Utility;

    using Polenter.Serialization;

    public partial class Import : Page
    {
        #region Fields

        private DropDownList drdList = new DropDownList();
        private Dictionary<int, string> map;
        private List<string> _allFields;
        private Dictionary<int, string> _headerText;

        #endregion Fields

        #region Properties

        public Font Font
        {
            get; set;
        }

        public Dictionary<int, string> HeaderText
        {
            get { return _headerText ?? (_headerText = ReadHeaderFromFile(inputFileLocation.Value)); }
        }

        public List<Tuple<string, string>> RequiredFields
        {
            get; set;
        }

        private List<string> AllFields
        {
            get { return _allFields ?? (_allFields = GetPossibleFieldNames()); }
        }

        #endregion Properties

        #region Methods

        public void BindDelimiters()
        {
            var itemValues = Enum.GetValues(typeof (Delimiter));
            var names = Enum.GetNames(typeof (Delimiter));

            for (var i = 0; i <= names.Length - 1; i++)
            {
                var item = new ListItem(names[i], Convert.ToInt32(itemValues.GetValue(i)).ToString());
                ddlDelimiter.Items.Add(item);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var taskId = GetAllValues();

                var task = AryaServices.CreateAryaTask(taskId, Guid.Parse(projectId.Value), txtProjectDesc.Text,
                    Path.Combine(projectId.Value, folderGuid.Value), Guid.Parse(userId.Value), typeof (ImportWorker));

                AryaServices.SendEmail(task, false, null, ddlProject.SelectedItem.Text,
                    ((SiteMaster) Page.Master).Email, "http://" + Request.Url.Authority + Request.ApplicationPath);

                Response.Redirect("Status.aspx");
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
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
            if (strFileExtension == ".txt" || strFileExtension == ".xml")
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
                if (strFileExtension == ".xml")
                {
                    // pnlImport.Visible = false;
                    gv_Fields.Visible = false;
                    chkBoxImportType.Visible = false;
                    // CustomValidator2.Visible = false;
                    // lblImportType.Visible = false;
                    lblChkboxList.Visible = false;
                    chkboxlist.Visible = false;
                    btnSubmit.Visible = true;
                }

                else
                {
                    gv_Fields.Visible = true;
                    lblChkboxList.Visible = true;
                    chkboxlist.Visible = true;
                    chkBoxImportType.Visible = true;
                    pnlImport.Visible = true;
                    GetAllImportWorkers();
                }
            }
        }

        protected void chkboxlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            var checkedValues = new List<string>();
            RequiredFields = new List<Tuple<string, string>>();

            foreach (ListItem chkbox in chkboxlist.Items)
            {
                if (chkbox.Selected)
                    checkedValues.Add(chkbox.Value);
            }
            GetCheckBoxSelectedItems(checkedValues);
            ReadHeaderFromFile(inputFileLocation.Value);
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            var i = 0;
            foreach (ListItem chkbox in chkboxlist.Items)
            {
                if (chkbox.Selected)
                    i = i + 1;
            }
            if (i >= 1)
                args.IsValid = true;
        }

        protected void ddlDelimiter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var delimiter = ddlDelimiter.SelectedItem.Text;
            Session["FieldDelimiter"] = delimiter;
        }

        protected void DisplayProjectList()
        {
            var availableProjects = new List<ListItem>();
            using (var Dc = new AryaDbDataContext())
            {
                var currUser =
                    (from u in Dc.Users where u.EmailAddress == ((SiteMaster) Page.Master).Email select u)
                        .FirstOrDefault();

                var userID = currUser.ID;
                var userName = currUser.FullName;
                var userRole = currUser.IsAdmin;
                userId.Value = userID.ToString();

                var existingDatabases = Dc.ExecuteQuery<string>("select name from sys.databases").ToList();
                if (!userRole)
                {
                    var projects = (from ug in currUser.UserProjects
                        where
                            ug.GroupID == Group.ImportAdminGroup
                            || ug.GroupID == Group.ImportUserGroup
                            && existingDatabases.Contains(ug.Project.DatabaseName)
                        orderby ug.Project.DatabaseName ascending
                        select ug.Project).Distinct();
                    availableProjects =
                        projects.Select(p => new ListItem {Text = p.ClientDescription + " " + p.SetName}).ToList();
                }
                else
                {
                    var projects =
                        (from p in Dc.Projects
                            where existingDatabases.Contains(p.DatabaseName)
                            orderby p.DatabaseName ascending
                            select p);

                    availableProjects =
                        projects.Select(p => new ListItem {Text = p.ClientDescription + " " + p.SetName})
                            .Distinct()
                            .ToList();
                }
                ddlProject.Items.Insert(0, "Select a Project");
                availableProjects.ForEach(ap => ddlProject.Items.Add(ap));
            }
        }

        protected void GetCheckBoxSelectedItems(List<string> selecteditems)
        {
            lblGrid.Visible = true;
            lblGrid.Text = "<b>Field Mappings </b>" + "  "
                           + "<i>(Please select the mappings for the mandatory fields marked in bold)</i>";

            var allFields = new List<Tuple<string, string>>();
            foreach (var selectedText in selecteditems)
            {
                switch (selectedText)
                {
                    case "Taxonomy":
                        var taxIW = new TaxonomyImportWorker();
                        var requiredTaxFields =
                            (from p in taxIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredTaxFields);
                        var optionalTaxFields =
                            (from p in taxIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var taxImports = (requiredTaxFields).Union(optionalTaxFields).ToList();
                        allFields.AddRange(taxImports);
                        break;

                    case "Taxonomy Meta Data":
                        var taxMdIW = new TaxonomyMetaDataImportWorker();
                        var requiredTaxMetaFields =
                            (from p in taxMdIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredTaxMetaFields);

                        var optionalTaxMetaFields =
                            (from p in taxMdIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();

                        var taxMdImports = (requiredTaxMetaFields).Union(optionalTaxMetaFields).ToList();
                        allFields.AddRange(taxMdImports);
                        break;

                    case "SKUTaxonomy":
                        var skutaxIW = new SKUTaxonomyImportWorker();
                        var requiredSkuTaxFields =
                            (from p in skutaxIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredSkuTaxFields);
                        var optionalskuTaxFields =
                            (from p in skutaxIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var skutaxImports = (requiredSkuTaxFields).Union(optionalskuTaxFields).ToList();
                        allFields.AddRange(skutaxImports);
                        break;

                    case "Attribute":
                        var attrIW = new AttributeImportWorker();
                        var requiredAttrFields =
                            (from p in attrIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredAttrFields);
                        var optionalAttrFields =
                            (from p in attrIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var attrImports = (requiredAttrFields).Union(optionalAttrFields).ToList();
                        allFields.AddRange(attrImports);
                        break;

                    case "Schema":
                        var schemaIW = new SchemaImportWorker();
                        var requiredSchemaValues = new List<string>();
                        var requiredSchemaFields =
                            (from p in schemaIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredSchemaFields);
                        var optionalschemaFields =
                            (from p in schemaIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var schemaImports = (requiredSchemaFields).Union(optionalschemaFields).ToList();
                        allFields.AddRange(schemaImports);
                        break;

                    case "Schema Meta Data":
                        var schemametaIW = new SchemaMetaDataImportWorker();
                        var requiredSchemaMetaFields =
                            (from p in schemametaIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredSchemaMetaFields);
                        var optionalschemaMetaFields =
                            (from p in schemametaIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var schemaMetaImports = (requiredSchemaMetaFields).Union(optionalschemaMetaFields).ToList();
                        allFields.AddRange(schemaMetaImports);
                        break;

                    case "Sku Attribute Value":
                        var skuattrIW = new SkuAttributeValueImportWorker();
                        var requiredSkuAttrFields =
                            (from p in skuattrIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredSkuAttrFields);
                        var optionalskuAttrFields =
                            (from p in skuattrIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var SkuattrImports = (requiredSkuAttrFields).Union(optionalskuAttrFields).ToList();
                        allFields.AddRange(SkuattrImports);
                        break;

                    case "Derived Attribute":
                        var derattrIW = new DerivedAttributeImportWorker();
                        var requiredDerAttrFields =
                            (from p in derattrIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredDerAttrFields);
                        var optionalderAttrFields =
                            (from p in derattrIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        var derattrImports = (requiredDerAttrFields).Union(optionalderAttrFields).ToList();
                        allFields.AddRange(derattrImports);
                        break;

                    case "List Of Values":
                        var lovIW = new ListOfValuesImportWorker();
                        var requiredLovFields =
                            (from p in lovIW.GetRequiredFields() select Tuple.Create(p, "Required")).ToList();
                        RequiredFields.AddRange(requiredLovFields);
                        var optionalLovFields =
                            (from p in lovIW.GetOptionalFields() select Tuple.Create(p, "Optional")).ToList();
                        ;
                        var lovImports = (requiredLovFields).Union(optionalLovFields).ToList();
                        allFields.AddRange(lovImports);
                        break;
                }

                gv_Fields.DataSource =
                    (from p in allFields.OrderByDescending(item => item.Item2) select new {MappingFields = p.Item1})
                        .ToList().Distinct();
                gv_Fields.DataBind();
            }
            PopulateImportOptions();
        }

        protected void gv_Fields_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((from p in RequiredFields select p.Item1).ToList().Contains(e.Row.Cells[0].Text))
                {
                    e.Row.Cells[0].Font.Bold = true;
                    var ddList = (DropDownList) e.Row.FindControl("DropDownList1");
                    ddList.Items[0].Text = "_ignore_";
                    ddList.Items[0].Value = "_ignore_";
                }
            }
        }

        protected Dictionary<string, int> MapFields()
        {
            var fieldMappings = new Dictionary<string, int>();
            var drd = new DropDownList();
            foreach (GridViewRow grdRow in gv_Fields.Rows)
            {
                var i = Int32.Parse(grdRow.RowIndex.ToString());
                var fieldText = gv_Fields.Rows[i].Cells[0].Text;
                drd = (DropDownList) (gv_Fields.Rows[i].Cells[1].FindControl("DropDownList1"));
                if ((!fieldMappings.ContainsKey(fieldText)) && (!drd.SelectedItem.Text.Equals("_ignore_")))
                {
                    var drdValue = Convert.ToInt32(HeaderText.SingleOrDefault(x => x.Value == drd.SelectedItem.Text).Key);
                    fieldMappings.Add(fieldText, drdValue);
                }
            }
            return fieldMappings;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var response = Session["FetchResponse"] as FetchResponse;
            if (response == null)
            {
                //This should never be called!
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            if (!Page.IsPostBack)
            {
                DisplayProjectList();
                BindDelimiters();
            }
            SelectProject();
            notificationEmail.Value = ((SiteMaster) Page.Master).Email;
        }

        protected Dictionary<int, string> ReadHeaderFromFile(string filePath)
        {
            drdList.Items.Insert(0, "_ignore_");
            string[] parts;
            map = new Dictionary<int, string>();
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var line = reader.ReadLine();
                if (line == null)
                    throw new Exception("File is empty");

                parts = line.Split('\t');
                map = parts.Select((part, index) => new {part, index}).ToDictionary(p => p.index, p => p.part);
                reader.Close();
            }

            gv_Fields.DataSource = (from p in parts select new {HeaderColumn = p, MappingFields = AllFields}).ToList();
            foreach (GridViewRow grdRow in gv_Fields.Rows)
            {
                var i = Int32.Parse(grdRow.RowIndex.ToString());
                drdList = (DropDownList) (gv_Fields.Rows[grdRow.RowIndex].Cells[1].FindControl("DropDownList1"));
                drdList.DataSource = map.Values.ToList();
                drdList.DataBind();
            }
            return map;
        }

        protected void SelectProject()
        {
            var currentProject = ddlProject.SelectedItem.Value;
            using (var Dc = new AryaDbDataContext())
            {
                var currentProjectID =
                    (from p in Dc.Projects where p.ClientDescription + ' ' + p.SetName == currentProject select p.ID)
                        .FirstOrDefault();
                projectId.Value = currentProjectID.ToString();
            }
        }

        private void GetAllImportWorkers()
        {
            var allImports =
                ImportWorkerBase.GetAvailableImports()
                    .Select(
                        iw =>
                            iw.ToString()
                                .Substring(iw.ToString().LastIndexOf('.') + 1)
                                .Replace("ImportWorker", " ")
                                .Trim());
            chkboxlist.Items.Clear();
            foreach (var imports in allImports)
            {
                var existingimports = imports.Spacify();
                chkboxlist.Items.Add(existingimports);
                pnlImport.Visible = true;
            }
        }

        private Guid GetAllValues()
        {
            //This method should be called when the Save or Load button is clicked by the user
            var fileName = Path.GetFileName(inputFileLocation.Value);
            if (fileName == null)
                return Guid.Empty;

            ImportArgs ia = null;
            if (fileName.Contains(".xml"))
            {
                //var allImports = ImportWorkerBase.GetAvailableImports();

                //foreach (var import in allImports)
                //{
                //    var className = import.GetType();
                //    Type[] typeArgs = { className };
                //    object o = Activator.CreateInstance(className);
                //}

                ia = new ImportArgs
                     {
                         Id = Guid.NewGuid(),
                         InputFilePath = inputFileLocation.Value,
                         JobDescription = txtProjectDesc.Text,
                         ProjectId = Guid.Parse(projectId.Value),
                         UserId = Guid.Parse(userId.Value),
                         CurrentImportOptions =
                             ImportOptions.CreateMissingMetaAttributes
                             | ImportOptions.CreateMissingAttributes | ImportOptions.CreateMissingSkus
                             | ImportOptions.CreateMissingTaxonomies | ImportOptions.CreateMissingValues
                             | ImportOptions.CreateMissingLOVs,
                         UpdateFrequency = 4000
                     };
            }
            else
            {
                ia = new ImportArgs
                     {
                         Id = Guid.NewGuid(),
                         InputFilePath = inputFileLocation.Value,
                         FieldDelimiter =
                             (Delimiter) Enum.Parse(typeof (Delimiter), ddlDelimiter.SelectedValue),
                         ProjectId = Guid.Parse(projectId.Value),
                         UserId = Guid.Parse(userId.Value),
                         NotificationEmailAddresses = notificationEmail.Value,
                         CurrentImportOptions =
                             ImportOptions.CreateMissingMetaAttributes
                             | ImportOptions.CreateMissingAttributes | ImportOptions.CreateMissingSkus
                             | ImportOptions.CreateMissingTaxonomies | ImportOptions.CreateMissingValues
                             | ImportOptions.CreateMissingLOVs,
                         JobDescription = txtProjectDesc.Text,
                         UpdateFrequency = 4000,
                         FieldMappings = MapFields()
                     };

                Session["FieldMappings"] = ia.FieldMappings;
                var io = ImportOptions.None;
                io = SelectImportType(io);
                Session["ImportType"] = io;
                ia.CurrentImportOptions = io;
            }

            var iaFilePath = new FileInfo(inputFileLocation.Value).Directory.FullName;
            argsPath.Value = iaFilePath;
            var iaFileName = Path.Combine(iaFilePath, WorkerArguments.ArgumentsFileName);
            var settings = ia.GetSharpSerializerXmlSettings(WorkerArguments.ArgumentsFileRootName);
            var serializer = new SharpSerializer(settings);
            serializer.Serialize(ia, iaFileName);
            importArgs.Value = iaFileName;
            return ia.Id;
        }

        private List<string> GetPossibleFieldNames()
        {
            var allImports = ImportWorkerBase.GetAvailableImports();
            var fields =
                allImports.SelectMany(i => i.GetRequiredFields())
                    .AsEnumerable()
                    .Union(allImports.SelectMany(i => i.GetOptionalFields()).AsEnumerable());
            //  var allFields = fields.Distinct().OrderBy(a => a).ToList();
            var allFields = fields.Distinct().ToList();
            allFields.Insert(0, "_ignore_");
            return allFields;
        }

        private void PopulateImportOptions()
        {
            lblImportType.Visible = true;
            btnSubmit.Visible = true;
            chkBoxImportType.Items.Clear();
            foreach (ImportOptions io in Enum.GetValues(typeof (ImportOptions)))
            {
                if (io == ImportOptions.None || io == ImportOptions.CreateMissingMetaAttributes)
                    continue;
                chkBoxImportType.Items.Add(new ListItem(io.ToString().Spacify(), io.ToString()));
            }
        }

        private ImportOptions SelectImportType(ImportOptions io)
        {
            foreach (ListItem chk in chkBoxImportType.Items)
            {
                if (chk.Selected)
                    io = io | (ImportOptions) Enum.Parse(typeof (ImportOptions), chk.Value);
            }
            return io;
        }

        #endregion Methods
    }
}