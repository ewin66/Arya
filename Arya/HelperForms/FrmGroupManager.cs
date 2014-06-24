using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya.HelperForms
{
    public partial class FrmGroupManager : Form
    {
        private readonly Project filteredProject;
        private readonly SkuDataDbDataContext dbContext;
        private HashSet<Guid> predefinedGroups;

        public FrmGroupManager(Project filteredProject = null)
        {
            InitializeComponent(); 
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.groupIcon;
            this.filteredProject = filteredProject;
            dbContext = new SkuDataDbDataContext();
        }

        private void LoadGroups()
        {
            bsGroups.CurrentItemChanged -= bsGroups_CurrentItemChanged;
            if (filteredProject == null)
                bsGroups.DataSource = dbContext.Groups.Where(p => p.ID != Group.DefaultGroupID).ToList();
            else
            {
                var projectGroupIDs = filteredProject.UserProjects.Where(p => p.GroupID != Group.DefaultGroupID).Select(p => p.GroupID).Distinct().ToList();
                bsGroups.DataSource =
                    dbContext.Groups.Where(p => projectGroupIDs.Contains(p.ID)).ToList();
            }
            bsGroups.CurrentItemChanged += bsGroups_CurrentItemChanged;
        }

        private void bsGroups_CurrentItemChanged(object sender, EventArgs e)
        {
            var currentGroup = bsGroups.Current as Group;

            if (currentGroup == null || string.IsNullOrWhiteSpace(currentGroup.Name) 
            //  || string.IsNullOrWhiteSpace(currentGroup.ClientName)
                ) return;

            if (dbContext.Groups.Any(p => p.ID == currentGroup.ID)) return;

            bsGroups.CurrentItemChanged -= bsGroups_CurrentItemChanged;

            currentGroup.ID = Guid.NewGuid();
            currentGroup.GroupType = Group.USER_GROUP_UD;
            currentGroup.CreatedOn = DateTime.Now;
            currentGroup.CreatedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
            if (filteredProject != null && !currentGroup.Name.Trim().StartsWith(filteredProject.ProjectDescription))
                currentGroup.Name = filteredProject.ProjectDescription + "_" + currentGroup.Name;
            dbContext.Groups.InsertOnSubmit(currentGroup);
            bsGroups.CurrentItemChanged += bsGroups_CurrentItemChanged;
        }

        private void CheckDeletes()
        {
            var deletedGroupsIDs = 
                                    (from row in
                                         dgvGroups.Rows.Cast<DataGridViewRow>().Where(
                                             p => p.Cells[0].Value != null && (Guid)p.Cells[0].Value != Guid.Empty)
                                     let deleteCheckboxCell = row.Cells[1] as DataGridViewCheckBoxCell
                                     where deleteCheckboxCell != null && deleteCheckboxCell.Value != null && (bool)deleteCheckboxCell.Value
                                     select (Guid)row.Cells[0].Value).ToList();

            if(deletedGroupsIDs.Count == 0) return;

            if(deletedGroupsIDs.Count > 0 && MessageBox.Show("Group(s) will be deleted from all of its dependant Role(s) & Project(s)","Warning - Group Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Cancel)
                return;

            var deletedGroups =
                dbContext.Groups.Where(p => deletedGroupsIDs.Contains(p.ID)).ToList();
            var currentDatabase = dbContext.Connection.Database;
            var otherDbs =
               deletedGroups.SelectMany(p => p.UserProjects).Select(p => p.Project.DatabaseName).Where(p => p != currentDatabase).Distinct().ToList();

            dbContext.Groups.DeleteAllOnSubmit(deletedGroups);
            dbContext.SubmitChanges();
            //AryaTools.Instance.SaveChangesIfNecessary(false, true);

            if (otherDbs.Count == 0) return;

            var waitKey = FrmWaitScreen.ShowMessage("Cascading changes to Dependent Database(s) ...");

            foreach (var otherDb in otherDbs)
            {
                dbContext.Connection.ChangeDatabase(otherDb);
                dbContext.Groups.DeleteAllOnSubmit(dbContext.Groups.Where(p => deletedGroupsIDs.Contains(p.ID)));
            }

            AryaTools.Instance.InstanceData.Dc.Connection.ChangeDatabase(currentDatabase);

            FrmWaitScreen.HideMessage(waitKey);
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {

            if (dgvGroups.CurrentCell != null  && dgvGroups.CurrentCell.IsInEditMode) dgvGroups.EndEdit();

            //AryaTools.Instance.SaveChangesIfNecessary(false, true);
            dbContext.SubmitChanges();

            CheckDeletes();

            LoadGroups();
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            LoadGroups();
        }

        private void FrmGroupManager_Load(object sender, EventArgs e)
        {
            predefinedGroups = dbContext.Groups.Where(p => p.GroupType == Group.USER_GROUP_PD).Select(p => p.ID).ToHashSet();
            LoadGroups();

        }

        private void dgvGroups_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var groupID = dgvGroups[0, e.RowIndex].Value;
            if (groupID != null && predefinedGroups.Contains((Guid)groupID))
            {
                e.Cancel = true;
            }
        }

    }
}
