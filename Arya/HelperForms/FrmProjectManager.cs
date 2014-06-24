using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Arya.Data;
using Fasterflect;
using Omu.ValueInjecter;
using LinqKit;
using Arya.Framework.Settings;

namespace Arya.HelperForms
{
    using Framework.Settings;
    using System.Data.SqlClient;

    public partial class FrmProjectManager : Form
    {
        public FrmProjectManager()
        {
            InitializeComponent();
        }

        private SkuDataDbDataContext currentDb;
        private Guid currentProjectID;
        private Project currentProject;
        private bool newProject;
        private ProjectPreferences projPreferences;

        public void Run(Guid projectID = default(Guid))
        {
            currentProjectID = projectID;
            PrepareData();
            ShowDialog();
        }

        private void PrepareData()
        {
            //Load all the projects for the User.
            currentDb = new SkuDataDbDataContext();

            if (currentProjectID != Guid.Empty)
            {
                currentProject = currentDb.Projects.Single(p => p.ID == currentProjectID);
                Text = Text + " : " + currentProject.ProjectDescription;
                projPreferences = currentProject.ProjectPreferences;
            }
            else
            {
                //ID and other fields will be set in the defaultValues method.
                currentProject = new Project();
                currentDb.Projects.InsertOnSubmit(currentProject);
                Text = Text + " : <New Project>";
                newProject = true;
                currentProjectID = currentProject.ID;
                projPreferences = new ProjectPreferences
                    {
                        ListSeparator = "; ", 
                        ReturnSeparator = "|"
                    };
            }

            tbProjectName.Text = currentProject.ProjectName;
            //can edit Project name only for new projects
            tbProjectName.Enabled = newProject;
            tbDatabaseName.Text = currentProject.DatabaseName;
            //can edit database name only for new projects
            tbDatabaseName.Enabled = newProject;

            tbListSeparator.Text = projPreferences.ListSeparator;
            tbReturnSeparator.Text = projPreferences.ReturnSeparator;

            tbClientDescription.Text = currentProject.ClientDescription;
            tbSetName.Text = currentProject.SetName;

            //Field1
            tbEntityField1Name.Text = currentProject.EntityField1Name;
            cbEntityField1ReadOnly.Checked = currentProject.EntityField1Readonly;

            //Field2
            tbEntityField2Name.Text = currentProject.EntityField2Name;
            cbEntityField2ReadOnly.Checked = currentProject.EntityField2Readonly;

            //Field3
            tbEntityField3Name.Text = currentProject.EntityField3Name;
            cbEntityField3ReadOnly.Checked = currentProject.EntityField3Readonly;

            //Field4
            tbEntityField4Name.Text = currentProject.EntityField4Name;
            cbEntityField4ReadOnly.Checked = currentProject.EntityField4Readonly;

            //Field5
            tbEntityField5Name.Text = currentProject.EntityField5Name;
            cbEntityField5ReadOnly.Checked = currentProject.EntityField5Readonly;
            cbEntityField5IsStatus.Checked = currentProject.EntityField5IsStatus;

            //Image URL
            tbProductSearchString.Text = currentProject.ImageUrlString;
            tbSchemaFillRateFilters.Text = currentProject.SchemaFillRateFilters;

            //Hookup all the events
            var allTextBoxControls = GetControls(this).Where(p => p.Name.StartsWith("tb")).ToList();
            foreach (var textBoxControl in allTextBoxControls)
            {
                var method = GetType().GetMethod("tbAllFields_TextChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                var type = typeof(EventHandler);
                var handler = Delegate.CreateDelegate(type, this, method);
                var evt = textBoxControl.GetType().GetEvent("TextChanged");
                evt.AddEventHandler(textBoxControl, handler);
            }

            var allCheckBoxControls = GetControls(this).Where(p => p.Name.StartsWith("cb")).ToList();
            foreach (var checkBoxControl in allCheckBoxControls)
            {
                var method = GetType().GetMethod("cbAllFields_CheckedChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                var type = typeof(EventHandler);
                var handler = Delegate.CreateDelegate(type, this, method);
                var evt = checkBoxControl.GetType().GetEvent("CheckedChanged");
                evt.AddEventHandler(checkBoxControl, handler);
            }
        }

        public static IEnumerable<Control> GetControls(Control form)
        {
            foreach (Control childControl in form.Controls)
            {
                foreach (Control grandChild in GetControls(childControl))
                {
                    yield return grandChild;
                }
                yield return childControl;
            }
        }

        private void FrmProjectManager_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool errors = false;

            var allTextBoxControls = GetControls(this).Where(p => p.Name.StartsWith("tb")).ToList();
            MetaTable projectMeta = currentDb.Mapping.GetTable(typeof(Project));
            var dataMembers = projectMeta.RowType.PersistentDataMembers.Where(p => !p.CanBeNull && p.Type == typeof(string)).ToList();

            foreach (var requiredMember in dataMembers)
            {
                var requiredValue = currentProject.GetPropertyValue(requiredMember.Name);
                if (requiredValue == null || string.IsNullOrWhiteSpace(requiredValue.ToString()))
                {
                    MetaDataMember member = requiredMember;
                    epUser.SetError(allTextBoxControls.Single(p => p.Name == "tb" + member.Name), "Invalid Input");
                    errors = true;
                }
            }

            if (errors) return;

            currentProject.ProjectPreferences = projPreferences;
            currentDb.SubmitChanges();

            if (newProject)
            {
                var backupFileName = @"E:\TemporarySpace\" + Guid.NewGuid();
                BackupDatabase(backupFileName);
                RestoreDatabase(currentProject.DatabaseName, backupFileName);

                using (var targetDb = new SkuDataDbDataContext())
                {
                    targetDb.Connection.Open();
                    targetDb.Connection.ChangeDatabase(currentProject.DatabaseName);
                    //targetDb.Projects.InsertOnSubmit(currentProject.CloneEntity());

                    targetDb.Attributes.ForEach(att => att.ProjectID = currentProject.ID);
                    targetDb.TaxonomyInfos.ForEach(tax => tax.ProjectID = currentProject.ID);
                    targetDb.Projects.DeleteAllOnSubmit(targetDb.Projects.Where(prj => prj.ID != currentProject.ID));
                    targetDb.SubmitChanges();
                }
            }
            else
            {
                using (var targetDb = new SkuDataDbDataContext())
                {
                    targetDb.Connection.Open();
                    targetDb.Connection.ChangeDatabase(currentProject.DatabaseName);
                    var targetProject = targetDb.Projects.Single(p => p.ID == currentProject.ID);
                    targetProject.InjectFrom<ProjectInjection>(currentProject);
                    targetDb.SubmitChanges();
                }
            }

            MessageBox.Show("Project Created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void BackupDatabase(string destinationPath)
        {
           // var connection = new ServerConnection("dev.empirisense.com", "hari", "hari123");
            //var connection = new ServerConnection(Properties.Settings.Default.ServerName, Properties.Settings.Default.UserName, Properties.Settings.Default.Password);
            //string connString = Properties.Settings.Default.AryaConnectionString;
            var connection = new ServerConnection(new SqlConnection(Properties.Settings.Default.AryaDbConnectionString));
            var sqlServer = new Server(connection);

            var bkpDatabase = new Backup { Action = BackupActionType.Database, Database = "Arya" };
            var bkpDevice = new BackupDeviceItem(destinationPath, DeviceType.File);
            bkpDatabase.Devices.Add(bkpDevice);
            bkpDatabase.SqlBackup(sqlServer);
            connection.Disconnect();
        }

        public void RestoreDatabase(String databaseName, String backUpFile)
        {
            //var connection = new ServerConnection("dev.empirisense.com", "hari", "hari123");
            //var connection = new ServerConnection(Properties.Settings.Default.ServerName, Properties.Settings.Default.UserName, Properties.Settings.Default.Password);
            var connection = new ServerConnection(new SqlConnection(Properties.Settings.Default.AryaDbConnectionString));
            var sqlServer = new Server(connection);
            var rstDatabase = new Restore { Action = RestoreActionType.Database, Database = databaseName };
            var bkpDevice = new BackupDeviceItem(backUpFile, DeviceType.File);
            rstDatabase.Devices.Add(bkpDevice);
            var logicalRestoreFiles = rstDatabase.ReadFileList(sqlServer);
            rstDatabase.RelocateFiles.Add(new RelocateFile(logicalRestoreFiles.Rows[0][0].ToString(), @"E:\MSSQL\Data\" + databaseName + ".mdf"));
            rstDatabase.RelocateFiles.Add(new RelocateFile(logicalRestoreFiles.Rows[1][0].ToString(), @"D:\MSSQL\Data\" + databaseName + ".ldf"));
            rstDatabase.ReplaceDatabase = true;
            rstDatabase.SqlRestore(sqlServer);
        }

        private void tbAllFields_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            currentProject.SetPropertyValue(tb.Name.Remove(0, 2), tb.Text);
        }

        private void cbAllFields_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            currentProject.SetPropertyValue(cb.Name.Remove(0, 2), cb.Checked);
        }

        public class ProjectInjection : ConventionInjection
        {
            private readonly string[] allowedProperties = {
                                                     "DatabaseName", "ProjectName", "ClientDescription", "SetName", "AryaCodeBaseVersion",
                                                     "EntityField1Name", "EntityField1Readonly", "EntityField2Name",
                                                     "EntityField2Readonly", "EntityField3Name", "EntityField3Readonly",
                                                     "EntityField4Name", "EntityField4Readonly", "EntityField5Name",
                                                     "EntityField5Readonly", "EntityField5IsStatus", "ProductSearchString",
                                                     "SchemaFillRateFilters"
                                                 };

            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Type == typeof(string) && allowedProperties.Contains(c.SourceProp.Name);
            }
        }

        private void tbListSeparator_TextChanged(object sender, EventArgs e)
        {
            projPreferences.ListSeparator = tbListSeparator.Text;
        }

        private void tbReturnSeparator_TextChanged(object sender, EventArgs e)
        {
            projPreferences.ReturnSeparator = tbReturnSeparator.Text;
        }
    }
}
