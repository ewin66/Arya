using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;

namespace Arya.UserControls
{
    public partial class PermissionsManagerControl : UserControl
    {
        public PermissionsManagerControl()
        {
            InitializeComponent();
        }

        private SkuDataDbDataContext currentDb;
        private Project currentProject;

        public void Run(Guid currentUserID,bool isAdmin ,Guid currentProjectIDValue = default(Guid))
        {
            if (currentDb != null)
            {
                currentDb.Dispose();
            }
            currentDb = new SkuDataDbDataContext();
            Version currentVersion = Program.GetCurrrentCodeVersion();
            var allUserProjects =
                !isAdmin
                    ? currentDb.ExecuteQuery<Project>(
                        @"SELECT *
                    FROM Project 
                    WHERE ID IN ( 
	                    SELECT DISTINCT p.ID
	                    FROM UserProject up
	                    INNER JOIN Project p ON up.ProjectID = p.ID
	                    WHERE up.UserID = {0} and up.GroupID = {1}
	                    AND EXISTS (
		                    SELECT database_id FROM sys.databases sd WHERE sd.Name = p.DatabaseName and p.AryaCodeBaseVersion ={2}
	                    ) 
                    )
                   ",
                        currentUserID, Group.PermissionsManagerGroup, currentVersion.Major + "." + currentVersion.Minor).ToList().Select(
                            p => new {p.ProjectDescription, ProjectID = p.ID}).Distinct().ToList()
                    : currentDb.ExecuteQuery<Project>(@"SELECT * FROM Project p INNER JOIN sys.databases sd ON sd.Name = p.DatabaseName WHERE p.AryaCodeBaseVersion={0}",
                       currentVersion.Major + "." + currentVersion.Minor).ToList().Select(
                            p => new {p.ProjectDescription, ProjectID = p.ID}).Distinct().ToList();

            if (allUserProjects.Count == 0)
            {
                MessageBox.Show("You are not  Permission Manager in any project!!");
                return;
            }

            tscbProjects.SelectedIndexChanged -= tscbProjects_SelectedIndexChanged;

            tscbProjects.ComboBox.DataSource = allUserProjects;


            tscbProjects.ComboBox.DisplayMember = "ProjectDescription";
            tscbProjects.ComboBox.ValueMember = "ProjectID";

            //resizes the combobox to show the longest string
            using (Graphics graphics = CreateGraphics())
            {
                int maxWidth = 0;
                foreach (object obj in tscbProjects.ComboBox.Items)
                {
                    var currentString = ((dynamic)obj).ProjectDescription;
                    var area = graphics.MeasureString(currentString, tscbProjects.ComboBox.Font);
                    maxWidth = Math.Max((int)area.Width, maxWidth);
                }
                tscbProjects.ComboBox.Width = maxWidth + 15;
                if (maxWidth + 50 > tlpPermissionsManager.ColumnStyles[0].Width)
                    tlpPermissionsManager.ColumnStyles[0].Width = maxWidth + 70;
            }

            tscbProjects.SelectedIndex = currentProjectIDValue != default(Guid)
                                                      ? allUserProjects.IndexOf(
                                                          allUserProjects.Single(p => p.ProjectID == currentProjectIDValue))
                                                      :0 ;
//            if (tscbProjects.SelectedIndex == 0)
//            {
//                currentDb.ExecuteQuery<Project>(
//                        @"SELECT *
//                    FROM Project 
//                    WHERE ID IN ( 
//	                    SELECT DISTINCT p.ID
//	                    FROM UserProject up
//	                    INNER JOIN Project p ON up.ProjectID = p.ID
//	                    WHERE up.UserID = {0}
//	                    AND EXISTS (
//		                    SELECT database_id FROM sys.databases sd WHERE sd.Name = p.DatabaseName
//	                    ) 
//                    )
//                   ",
//                        currentUserID)
//            }
            tscbProjects.SelectedIndexChanged += tscbProjects_SelectedIndexChanged;
            tscbProjects_SelectedIndexChanged(tscbProjects, EventArgs.Empty);

            LoadPermissions();
        }

        private bool LoadGroups()
        {
            var projectGroupIDs = currentProject.UserProjects.Select(p => p.GroupID).Distinct().Where(p => p != Group.DefaultGroupID).ToList();

            var projectGroups =
                currentDb.Groups.Where(
                    p =>
                    projectGroupIDs.Contains(p.ID) && p.GroupType == Group.USER_GROUP_UD)
                    .Select(p => new {GroupName = p.Name, Grp = p}).ToList();

            lbUserGroups.SelectedIndexChanged -= lbUserGroups_SelectedIndexChanged;
            lbUserGroups.DataSource = projectGroups;
            lbUserGroups.DisplayMember = "GroupName";
            lbUserGroups.ValueMember = "Grp";
            if (projectGroups.Count > 0)
            {
                lbUserGroups.SelectedIndex = 0;
                tscbOptions.Enabled = true;
                tcAllPermissions.Enabled = true;
            }
            else
            {
                tscbOptions.Enabled = false;
                tcAllPermissions.Enabled = false;
            }
            lbUserGroups.SelectedIndexChanged += lbUserGroups_SelectedIndexChanged;

            return projectGroups.Count > 0;
        }

        public void LoadPermissions()
        {
            tscbOptions.SelectedIndexChanged -= tscbOptions_SelectedIndexChanged;
            tscbOptions.SelectedIndex = 0;
            tscbOptions.SelectedIndexChanged += tscbOptions_SelectedIndexChanged;

            if (LoadGroups())
            {
                taxonomyPermissions.EnableCheckBoxes = true;
                taxonomyPermissions.LoadProject();
                LoadUIObjects();
                lbUserGroups_SelectedIndexChanged(lbUserGroups, EventArgs.Empty);
            }
        }

        private void LoadUIObjects()
        {
            cilbUIObjects.DataSource = null;
            cilbUIObjects.DataSource = UIObject.GetDataSource();
            cilbUIObjects.DisplayMember = "UIObjectName";
            cilbUIObjects.ValueMember = "ID";
        }

        private void LoadUIExclusions(HashSet<Guid> uiExlusions)
        {
            //Check the current Permissions
            var i = 0;
            foreach (var item in cilbUIObjects.Items.Cast<object>().ToList())
            {
                var currentItem = item;
                var uiObjectID = ((dynamic)currentItem).ID;

                cilbUIObjects.SetItemCheckState(i,
                                                uiExlusions.Contains(uiObjectID)
                                                    ? CheckState.Checked
                                                    : CheckState.Unchecked);
                i++;
            }
        }

        private void cbTaxonomyOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbSource = (ToolStripComboBox)sender;
            switch (cbSource.SelectedIndex)
            {
                //All
                case 1:
                    taxonomyPermissions.CheckAllNodes(); break;
                //None
                case 2:
                    taxonomyPermissions.UnCheckAllNodes(); break;
                //Toggle
                case 3:
                    taxonomyPermissions.ToogleTaxonomyTree(); break;
                //Reset to original
                case 4:
                    taxonomyPermissions.UnCheckAllNodes();
                    var currentGroup = (Group)lbUserGroups.SelectedValue;
                    taxonomyPermissions.CheckExclusions(currentGroup.TaxonomyExlusions); break;
                default: break;
            }
        }

        private void cbAttributeOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbSource = (ToolStripComboBox)sender;
            switch (cbSource.SelectedIndex)
            {
                case 1: //Check all
                    attributeExclusionsControl.CheckAllAttributes();
                    break;
                case 2: //UnCheck all
                    attributeExclusionsControl.UnCheckAllAttributes();
                    break;
                case 3: //Invert Selection
                    attributeExclusionsControl.InvertCheckedAttributes();
                    break;
                case 4: //Reset AttributeOptions
                    var currentGroup = (Group)lbUserGroups.SelectedValue;
                    attributeExclusionsControl.PrepareData(currentGroup.AttributeExlusions);
                    break;
                case 5:
                    attributeExclusionsControl.ShowOnlyCheckedAttributes();
                    break;
            }
        }

        private void cbUIObjectOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbSource = (ToolStripComboBox)sender;
            var i = cilbUIObjects.Items.Count;
            switch (cbSource.SelectedIndex)
            {
                case 1: //Check all
                    while (--i > -1)
                    {
                        cilbUIObjects.SetItemCheckState(i, CheckState.Checked);
                    }
                    break;
                case 2: //UnCheck all
                    while (--i > -1)
                    {
                        cilbUIObjects.SetItemCheckState(i, CheckState.Unchecked);
                    }
                    break;
                case 3: //Invert Selection
                    var checkedIndexes = cilbUIObjects.CheckedIndices;
                    while (--i > -1)
                    {
                        cilbUIObjects.SetItemCheckState(i,
                                                        checkedIndexes.Contains(i)
                                                            ? CheckState.Unchecked
                                                            : CheckState.Checked);
                    }
                    break;
                case 4: //Reset UI Permissions
                    var currentGroup = (Group)lbUserGroups.SelectedValue;
                    LoadUIObjects();
                    LoadUIExclusions(currentGroup.UIExclusions);
                    break;
            }
        }

        private void lbUserGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentGroup = (Group)lbUserGroups.SelectedValue;

            if (currentGroup == null) return;

            //Load Taxonomy Exclusions
            taxonomyPermissions.CheckExclusions(currentGroup.TaxonomyExlusions);

            //Load Attribute Exlusions
            LoadAttributes(currentGroup.AttributeExlusions);

            //Load UI Exclusions
            LoadUIExclusions(currentGroup.UIExclusions);
        }

        private void LoadAttributes(HashSet<Guid> attributeExclusions)
        {
            attributeExclusionsControl.DisableEditing = true;
            attributeExclusionsControl.IsPermissionsUIEnabled = true;
            attributeExclusionsControl.PrepareData(attributeExclusions);
        }

        private void tsbtnSave_Click(object sender, EventArgs e)
        {
            var currentGroup = (Group)lbUserGroups.SelectedValue;

            if (currentGroup == null)
            {
                MessageBox.Show("The project has no group(s)");
                return;
            }

            if (MessageBox.Show(@"Do you want to save the current changes?", @"Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            //gather taxonomy exclusions
            var changedTaxonomyExclusions = taxonomyPermissions.GetAllCheckedTaxonomies();
            var currentTaxonomyExclusions = currentGroup.TaxonomyExlusions.ToList();

            var toBeDeletedTaxonomyExclusions = currentTaxonomyExclusions.Except(changedTaxonomyExclusions).ToList();
            var toBeAddedTaxonomyExclusions = changedTaxonomyExclusions.Except(currentTaxonomyExclusions).ToList();

            foreach (var addedTaxonomyExclusion in toBeAddedTaxonomyExclusions)
            {
                currentGroup.Roles.Add(new Role { GroupID = currentGroup.ID, ID = Guid.NewGuid(), ObjectID = addedTaxonomyExclusion, ObjectType = Role.TaskObjectType.TaxonomyInfo.ToString(), Permission = false });
            }

            foreach (var deletedTaxonomyExclusion in toBeDeletedTaxonomyExclusions)
            {
                Guid exclusion = deletedTaxonomyExclusion;
                var deletedRole = currentGroup.Roles.Single(p => p.ObjectID == exclusion);
                currentDb.Roles.DeleteOnSubmit(deletedRole);
            }

            //gather attribute exclusions
            var changedAttributeExclusions = attributeExclusionsControl.CheckedAttributes;
            var currentAttributeExclusions = currentGroup.AttributeExlusions.ToList();

            var toBeDeletedAttributeExclusions = currentAttributeExclusions.Except(changedAttributeExclusions).ToList();
            var toBeAddedAttributeExclusions = changedAttributeExclusions.Except(currentAttributeExclusions);

            foreach (var addedAttributeExclusion in toBeAddedAttributeExclusions)
            {
                currentGroup.Roles.Add(new Role { GroupID = currentGroup.ID, ID = Guid.NewGuid(), ObjectID = addedAttributeExclusion, ObjectType = Role.TaskObjectType.Attribute.ToString(), Permission = false });
            }

            foreach (var deletedAttributeExclusion in toBeDeletedAttributeExclusions)
            {
                Guid exclusion = deletedAttributeExclusion;
                var deletedRole = currentGroup.Roles.Single(p => p.ObjectID == exclusion);
                currentDb.Roles.DeleteOnSubmit(deletedRole);
            }

            //gather UI exclusions
            var changedUIExclusions = cilbUIObjects.CheckedItems.Select(p => (Guid)((dynamic)p).ID).ToList();
            var currentUIExclusions = currentGroup.UIExclusions.ToList();

            var toBeDeletedUIExclusions = currentUIExclusions.Except(changedUIExclusions).ToList();
            var toBeAddedUIExclusions = changedUIExclusions.Except(currentUIExclusions).ToList();

            foreach (var addedUIExclusion in toBeAddedUIExclusions)
            {
                currentGroup.Roles.Add(new Role { GroupID = currentGroup.ID, ID = Guid.NewGuid(), ObjectID = addedUIExclusion, ObjectType = Role.TaskObjectType.UIObject.ToString(), Permission = false });
            }

            foreach (var deletedUIExclusion in toBeDeletedUIExclusions)
            {
                Guid exclusion = deletedUIExclusion;
                var deletedRole = currentGroup.Roles.Single(p => p.ObjectID == exclusion);
                currentDb.Roles.DeleteOnSubmit(deletedRole);
            }

            currentDb.SubmitChanges();
        }

        private void tscbProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbProjects.ComboBox.Items.Count == 0)
                return;

            Guid projectID = ((dynamic)tscbProjects.ComboBox.SelectedItem).ProjectID;
            if (currentDb != null) currentDb.Dispose();
            currentDb = new SkuDataDbDataContext();
            var databaseName =
                currentDb.Projects.Single(p => p.ID == projectID).DatabaseName;

            currentProject =
                currentDb.Projects.Single(p => p.ID == projectID);

            if (currentDb.Connection.State == ConnectionState.Closed)
            {
                currentDb.Connection.Open();
            }

            currentDb.Connection.ChangeDatabase(databaseName);
            AryaTools.Instance.InstanceData.InitDataContext(projectID, true);
            LoadPermissions();
        }

        private void tsbtnRefresh_Click(object sender, EventArgs e)
        {
            tscbProjects_SelectedIndexChanged(tscbProjects, EventArgs.Empty);
        }

        private void tscbOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tcAllPermissions.SelectedIndex)
            {
                case 0:
                    cbTaxonomyOptions_SelectedIndexChanged(sender,e);
                    break;
                case 1:
                    cbAttributeOptions_SelectedIndexChanged(sender,e);
                    break;
                case 2 :
                    cbUIObjectOptions_SelectedIndexChanged(sender,e);
                    break;
            }

            if (tscbOptions.SelectedIndex != 0)
            {
                tscbOptions.SelectedIndexChanged -= cbAttributeOptions_SelectedIndexChanged;
                tscbOptions.SelectedIndex = 0;
                tscbOptions.SelectedIndexChanged += cbAttributeOptions_SelectedIndexChanged;
            }
        }
    }
}
