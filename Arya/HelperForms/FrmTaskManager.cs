using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Natalie.Data;
using Natalie.Framework.Collections;
using Natalie.Framework.Collections.Generic;
using Natalie.Framework.Extensions;
using Natalie.HelperClasses;
using Attribute = Natalie.Data.Attribute;

namespace Natalie.HelperForms
{
    public partial class FrmTaskManager : Form
    {
        private readonly Dictionary<string,SkuDataDbDataContext> _contextPool = new Dictionary<string, SkuDataDbDataContext>();

        private readonly Dictionary<Guid, ILookup<string, Guid>> _taxonomyCache = new Dictionary<Guid, ILookup<string, Guid>>();

        private readonly Dictionary<Guid,List<Attribute>> _attributeCache =
            new Dictionary<Guid, List<Attribute>>();

        private readonly Dictionary<Guid,List<Sku>> _skuCache = new Dictionary<Guid, List<Sku>>();

        private readonly DoubleKeyDictionary<Guid,Role.TaskObjectType,AutoCompleteStringCollection> _autoCompleteCache = new DoubleKeyDictionary<Guid, Role.TaskObjectType, AutoCompleteStringCollection>(); 

        private readonly AutoCompleteStringCollection _uiObjectAutoCompleteCache = new AutoCompleteStringCollection();

        private bool _autoSuggestEnabled;

        public FrmTaskManager()
        {
            InitializeComponent(); 
            DisplayStyle.SetDefaultFont(this);
            dgvRoles.DoubleBuffered(true);

            //load UIObject AutoComplete Cache
            _uiObjectAutoCompleteCache.AddRange(UIObject.GetUIObjectNames());
        }

        public SkuDataDbDataContext GetDatabaseContext(string databaseName)
        {
            var lowerDbName = databaseName.ToLower();
            if (!_contextPool.ContainsKey(lowerDbName))
            {
                _contextPool.Add(lowerDbName, new SkuDataDbDataContext(NatalieTools.Instance.Dc.Connection));
            }

            _contextPool[lowerDbName].Connection.ChangeDatabase(databaseName);
            return _contextPool[lowerDbName];
        }

        private void LoadProjects()
        {
            var existingDatabases =
                NatalieTools.Instance.Dc.Projects.Select(p => p.DatabaseName).Distinct().ToList().Where(
                    p => NatalieTools.CheckDatabaseExists(NatalieTools.Instance.Dc, p)).ToList();

            var existingProjects =
                (NatalieTools.Instance.Dc.Projects.Where(p => existingDatabases.Contains(p.DatabaseName)).Select(
                    p => new { Project = p, ProjectName = string.Format("{0} {1}", p.ClientDescription, p.SetName) })
                    .ToList()
                    .Union(new { Project = new Project { ID = Guid.Empty }, ProjectName = "---  Select Project  ---" })).OrderBy(p => p.ProjectName).ToList();

            cbProjectList.ValueMember = "Project";
            cbProjectList.DisplayMember = "ProjectName";
            cbProjectList.DataSource = existingProjects;
        }

        public Guid SelectedGroupID
        {
            get
            {
                return lbUserGroups.SelectedValue is Guid ? (Guid)lbUserGroups.SelectedValue : Guid.Empty;
            }
        }

        private void FrmTaskManager_Load(object sender, EventArgs e)
        {

            //Load Permission Types
            var t = Role.GetAllPermissionTypes();
            var permissions = Role.GetAllPermissionTypes().Select(p => new { DisplayText = p.GetDisplayText(), PType = p}).ToList();
            cbPermissionTypes.DataSource = permissions;
            cbPermissionTypes.ValueMember = "PType";
            cbPermissionTypes.DisplayMember = "DisplayText";

            cbPermissionTypes.SelectedIndexChanged += cbPermissionTypes_SelectedIndexChanged;

            LoadProjects();

            var ds =
                Role.GetAllTaskObjects().Select(
                    p => new {TaskName = Enum.GetName(typeof (Role.TaskObjectType), p), TaskEnum = p}).ToList();

            cbTaskTypes.DataSource = ds;
            cbTaskTypes.DisplayMember = "TaskName";
            cbTaskTypes.ValueMember = "TaskEnum";

            //add the default context to the pool
            _contextPool.Add(NatalieTools.DefaultDatabaseName.ToLower(),NatalieTools.Instance.Dc);
        }

        private void cbProjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var selectedProjectID = this.cbProjectList.SelectedValue is Guid ? (Guid) this.cbProjectList.SelectedValue : new Guid();

            tbNewRule.Text = string.Empty;

            var selectedProject = cbProjectList.SelectedValue as Project;

            if (selectedProject  == null) return;

            //default is exclusions
            this.LoadTasks(Guid.Empty, Role.PermissionType.Exclusions);
            this.LoadGroups(selectedProject);
            this.LoadUIObjects(selectedProject);
        }

        private void LoadUIObjects(Project sourceProject)
        {
            if(sourceProject.ID == Guid.Empty) return;

            //swap to default database
            NatalieTools.Instance.Dc = GetDatabaseContext(NatalieTools.DefaultDatabaseName);

            var uiObjects = NatalieTools.Instance.Dc.UIObjects.ToList();

            //swap context to project's database
            NatalieTools.Instance.Dc = GetDatabaseContext(sourceProject.DatabaseName);

            var currentUIObjects = NatalieTools.Instance.Dc.UIObjects.ToList();

            var missingUIObjects =
                uiObjects.Where(p => currentUIObjects.All(q => q.ID != p.ID)).ToList();

            foreach (var missingUIObject in missingUIObjects)
            {
                NatalieTools.Instance.Dc.UIObjects.InsertOnSubmit(missingUIObject.CloneEntity());
            }

            NatalieTools.Instance.SaveChangesIfNecessary(false, true);
        }

        private void LoadGroups(Project sourceProject)
        {
            if(sourceProject.ID == Guid.Empty)
            {
                lbUserGroups.DataSource = null;
                lbUserGroups.Tag = Guid.Empty;
                return;
            }

            //swap to default database
            NatalieTools.Instance.Dc = GetDatabaseContext(NatalieTools.DefaultDatabaseName);

            var projectGroups = sourceProject.UserProjects.Select(p => p.Group).Distinct().ToDictionary(key => key.ID,
                                                                                                        value => value);
            //swap context to project's database
            NatalieTools.Instance.Dc = GetDatabaseContext(sourceProject.DatabaseName);

            var missingGroupIDs =
                projectGroups.Keys.Except(
                    NatalieTools.Instance.Dc.Groups.Select(
                        p => p.ID).ToList()).ToList();

            foreach (var nEntity in missingGroupIDs.Select(missingGroupID => projectGroups[missingGroupID]).Select(missingGroup => missingGroup.CloneEntity()))
            {
                NatalieTools.Instance.Dc.Groups.InsertOnSubmit(nEntity);
            }

            NatalieTools.Instance.SaveChangesIfNecessary(false, true);

            lbUserGroups.Tag = sourceProject.ID;

            lbUserGroups.DataSource = projectGroups.Values.Where(p => p.ID != Group.DefaultGroupID).Select(p => new { GroupName = p.Name , GroupID =  p.ID}).ToList();
            lbUserGroups.DisplayMember = "GroupName";
            lbUserGroups.ValueMember = "GroupID";

            //lbUserGroups.SelectedIndex = 0;
        }

        private void lbUserGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTasks(SelectedGroupID, (Role.PermissionType)cbPermissionTypes.SelectedValue);
        }

        private void LoadTasks(Guid sourceGroup, Role.PermissionType permissionType)
        {
            if(sourceGroup == Guid.Empty)
            {
                dgvRoles.DataSource = null;
                return;
            }

            var permissionDbValue = (bool)permissionType.GetDbValue();

            var currentRoles =
            NatalieTools.Instance.Dc.Roles.Where(p => p.GroupID == sourceGroup && p.Permission == permissionDbValue).
                ToList().Select(p => new
                {
                    TaskType = p.GetTaskObjectType(),
                    TaskName = (p.TaskObject ?? "<Deleted or Invalid Object>").ToString(),
                    RoleID = p.ID,
                    p.ObjectID
                }).ToSortableBindingList();

            dgvRoles.DataSource = currentRoles;
        }

        private void ToggleDeleteTaskButton()
        {
            btnDeleteTask.Enabled = (from row in
                                        dgvRoles.Rows.Cast<DataGridViewRow>().Where(
                                            p => p.Cells[3].Value != null)
                                    let deleteCheckboxCell = row.Cells[0] as DataGridViewCheckBoxCell
                                    where
                                        deleteCheckboxCell != null && deleteCheckboxCell.Value != null &&
                                        (bool)deleteCheckboxCell.Value
                                    select row).Any();
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var selectedGroupID = SelectedGroupID;

            var currentSelectedProjectID = lbUserGroups.Tag is Guid ? (Guid) lbUserGroups.Tag : Guid.Empty;

            if (selectedGroupID == Guid.Empty || currentSelectedProjectID == Guid.Empty) return;

            var selectedTaskType = (Role.TaskObjectType)cbTaskTypes.SelectedValue;

            var taskObjectIDs = GetTaskObjectIDs(currentSelectedProjectID, tbNewRule.Text.Trim(),
                                                 selectedTaskType);
            taskObjectIDs =
               taskObjectIDs.Where(
                   taskObjectID =>
                   NatalieTools.Instance.Dc.Roles.Where(p => p.ObjectID == taskObjectID).All(p => p.GroupID != selectedGroupID)).ToList();

            foreach (var newRole in taskObjectIDs.Select(taskObjectID => new Role
                                                                             {
                                                                                 GroupID = selectedGroupID,
                                                                                 ObjectID = taskObjectID,
                                                                                 ObjectType = selectedTaskType.ToString(),
                                                                                 Permission = false,
                                                                                 ID = Guid.NewGuid()
                                                                             }))
            {
                //newRole.Task.ObjectType = selectedTaskType;
                NatalieTools.Instance.Dc.Roles.InsertOnSubmit(newRole);
            }

            NatalieTools.Instance.SaveChangesIfNecessary(false,true);

            tbNewRule.Text = string.Empty;

            //refresh the TaskGrid
            LoadTasks(selectedGroupID, (Role.PermissionType)cbPermissionTypes.SelectedValue);

        }

        private IEnumerable<Guid> GetTaskObjectIDs(Guid projectID,string taskObjectString,Role.TaskObjectType objectType)
        {
            List<Guid> taskObjectIDs = null;

            switch (objectType)
            {
                case Role.TaskObjectType.Attribute :

                    taskObjectIDs =
                        _attributeCache[projectID].Where(p => p.AttributeName == taskObjectString).Select(p => p.ID).
                            ToList();
                    break;
                case Role.TaskObjectType.Sku:
                    taskObjectIDs =
                        _skuCache[projectID].Where(p => p.ItemID == taskObjectString).Select(p => p.ID).ToList();
                    break;
                case Role.TaskObjectType.TaxonomyInfo:
                    if(_taxonomyCache[projectID].Any(p => p.Key == taskObjectString))
                        taskObjectIDs = _taxonomyCache[projectID][taskObjectString].ToList();
                    break;
                case Role.TaskObjectType.UIObject:
                    taskObjectIDs = NatalieTools.Instance.Dc.UIObjects.Where(p => p.Name == taskObjectString).Select(p => p.ID).ToList();
                    break;
            }

            return taskObjectIDs ?? new List<Guid>();
        }

        private void FrmTaskManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            //switch back the database to default database
            NatalieTools.Instance.Dc = GetDatabaseContext(NatalieTools.DefaultDatabaseName);

            //Application.Restart();
        }

        private void tbNewRule_TextChanged(object sender, EventArgs e)
        {
            btnAddTask.Enabled = !string.IsNullOrWhiteSpace(tbNewRule.Text) && tbNewRule.TextLength > 0;
        }

        private void dgvRoles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || dgvRoles.CurrentCell == null) return;

            if (dgvRoles.CurrentCell.IsInEditMode)
            {
                dgvRoles.EndEdit();
            }

            ToggleDeleteTaskButton();
        }

        private void btnDeleteTask_Click(object sender, EventArgs e)
        {
            var deletedRoleTasks = 
               (from row in
                    dgvRoles.Rows.Cast<DataGridViewRow>().Where(
                        p => p.Cells[3].Value != null)
                let deleteCheckboxCell = row.Cells[0] as DataGridViewCheckBoxCell
                where deleteCheckboxCell != null && deleteCheckboxCell.Value != null && (bool)deleteCheckboxCell.Value
                select new { RoleID = (Guid)row.Cells[3].Value, TaskID = (Guid)row.Cells[4].Value }).ToList();

            NatalieTools.Instance.Dc.Roles.DeleteAllOnSubmit(
               NatalieTools.Instance.Dc.Roles.Where(p => deletedRoleTasks.Select(q => q.RoleID).Contains(p.ID)));

            //NatalieTools.Instance.Dc.Tasks.DeleteAllOnSubmit(
            // NatalieTools.Instance.Dc.Tasks.Where(p => deletedRoleTasks.Select(q => q.TaskID).Contains(p.ID)));

            NatalieTools.Instance.SaveChangesIfNecessary(false, true);

            LoadTasks(SelectedGroupID,(Role.PermissionType)cbPermissionTypes.SelectedValue);

            ToggleDeleteTaskButton();
        }

        private void tsbtnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void PopulateAutoSuggest()
        {
            var currentSelectedProjectID = lbUserGroups.Tag is Guid ? (Guid)lbUserGroups.Tag : Guid.Empty;
            tbNewRule.AutoCompleteCustomSource = null;

            if (currentSelectedProjectID == Guid.Empty)
            {
                return;
            }

            switch ((Role.TaskObjectType)cbTaskTypes.SelectedValue)
            {
                case Role.TaskObjectType.TaxonomyInfo:

                    if (!_autoCompleteCache.ContainsKeys(currentSelectedProjectID, Role.TaskObjectType.TaxonomyInfo))
                    {
                        var waitKey = FrmWaitScreen.ShowMessage("Caching Taxonomies for Autosuggest ....");

                        _taxonomyCache.Add(currentSelectedProjectID,
                              NatalieTools.Instance.Dc.TaxonomyInfos.Where(p => p.ProjectID == currentSelectedProjectID && p.TaxonomyDatas.Any(q => q.Active)).
                                  ToLookup(key => key.ToString(), value => value.ID));

                        var acs = new AutoCompleteStringCollection();
                        acs.AddRange(_taxonomyCache[currentSelectedProjectID].Select(p => p.Key).OrderBy(p => p).ToArray());
                        _autoCompleteCache.Add(currentSelectedProjectID, Role.TaskObjectType.TaxonomyInfo, acs);

                        FrmWaitScreen.HideMessage(waitKey);
                    }

                    if (_autoSuggestEnabled)
                        tbNewRule.AutoCompleteCustomSource =
                                _autoCompleteCache[currentSelectedProjectID, Role.TaskObjectType.TaxonomyInfo];
                    break;

                case Role.TaskObjectType.Attribute:

                    if (!_autoCompleteCache.ContainsKeys(currentSelectedProjectID, Role.TaskObjectType.Attribute))
                    {
                        var waitKey = FrmWaitScreen.ShowMessage("Caching Attribute(s) for Autosuggest ....");

                        _attributeCache.Add(currentSelectedProjectID, NatalieTools.Instance.Dc.Attributes.Where(p => p.ProjectID == currentSelectedProjectID && p.Active).ToList());
                        var acs = new AutoCompleteStringCollection();
                        acs.AddRange(_attributeCache[currentSelectedProjectID].Select(p => p.AttributeName).ToArray());
                        _autoCompleteCache.Add(currentSelectedProjectID, Role.TaskObjectType.Attribute, acs);

                        FrmWaitScreen.HideMessage(waitKey);
                    }

                    if (_autoSuggestEnabled)
                        tbNewRule.AutoCompleteCustomSource =
                                _autoCompleteCache[currentSelectedProjectID, Role.TaskObjectType.Attribute];
                    break;

                case Role.TaskObjectType.Sku:

                    if (!_autoCompleteCache.ContainsKeys(currentSelectedProjectID, Role.TaskObjectType.Sku))
                    {
                        var waitKey = FrmWaitScreen.ShowMessage("Caching Sku(s) for Autosuggest ....");

                        _skuCache.Add(currentSelectedProjectID, NatalieTools.Instance.Dc.Skus.Where(p => p.ProjectID == currentSelectedProjectID).ToList());
                        var acs = new AutoCompleteStringCollection();
                        acs.AddRange(_skuCache[currentSelectedProjectID].Select(p => p.ItemID).ToArray());
                        _autoCompleteCache.Add(currentSelectedProjectID, Role.TaskObjectType.Sku, acs);

                        FrmWaitScreen.HideMessage(waitKey);
                    }

                    if (_autoSuggestEnabled)
                        tbNewRule.AutoCompleteCustomSource =
                                _autoCompleteCache[currentSelectedProjectID, Role.TaskObjectType.Sku];
                    break;

                case Role.TaskObjectType.UIObject:
                    tbNewRule.AutoCompleteCustomSource = _uiObjectAutoCompleteCache;
                    break;
            }
        }

        private void tbNewRule_Enter(object sender, EventArgs e)
        {
           PopulateAutoSuggest();
        }

        private void btnAutoSuggest_Click(object sender, EventArgs e)
        {
            _autoSuggestEnabled = !_autoSuggestEnabled;
            if(_autoSuggestEnabled)
            {
                PopulateAutoSuggest();
            }
            else
            {
                tbNewRule.AutoCompleteCustomSource = null;
            }
        }

        private void cbPermissionTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var t = cbPermissionTypes.SelectedValue;
            LoadTasks(SelectedGroupID,(Role.PermissionType)cbPermissionTypes.SelectedValue);
        }

    }
}
