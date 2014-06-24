using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataGridViewAutoFilter;
using LinqKit;
using Arya.Data;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.HelperForms;

namespace Arya.UserControls
{
    public partial class RolesManagerControl : UserControl
    {
        private bool _userCheckingItem;

        public Guid CurrentUserID
        {
            get;
            private set;
        }

        private SkuDataDbDataContext _currentDB;
        public RolesManagerControl()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            dgvUserProjectGroups.AutoGenerateColumns = false;
        }

        private void ResetSelectionTextBox(Label selectionLabel)
        {
            selectionLabel.Text = string.Format("No {0} Selected", selectionLabel.Tag);
            tpAdminView.SetToolTip(selectionLabel, string.Empty);
        }

        private void LoadUsers()
        {
            ResetSelectionTextBox(lblSelectedUsers);

            var existingUsers =
                _currentDB.Users.Where(u => u.EmailAddress != null && u.Active).ToList().Select(
                    p => new { UserID = p.ID, FullName = p.FullName + " (" + p.SingleSignOnId + ")" }).OrderBy(
                        p => p.FullName).ToList();

            clbUsers.BeginUpdate();
            clbUsers.DataSource = existingUsers;
            clbUsers.DisplayMember = "FullName";
            clbUsers.ValueMember = "UserID";
            clbUsers.CheckedIndices.OfType<int>().ForEach(a => clbUsers.SetItemChecked(a, false));
            clbUsers.EndUpdate();
        }

        private void LoadProjects()
        {
            ResetSelectionTextBox(lblSelectedProjects);

            var allProjects = _currentDB.Projects.AsQueryable();

            //if (CurrentUserID != default(Guid))
            //{
            //    allProjects = allProjects.Where(p => p.UserProjects.Any(up => up.UserID == CurrentUserID && up.GroupID == Group.RoleManagerGroup));
            //}
            List<object> existingProjects;
            Version currentVersion = Program.GetCurrrentCodeVersion();
            if (AryaTools.Instance.InstanceData.CurrentUser.IsAdmin)
            {
                existingProjects = _currentDB.ExecuteQuery<Project>("SELECT * FROM Project p INNER JOIN sys.databases sd ON sd.Name = p.DatabaseName").Where(p => p.AryaCodeBaseVersion == "4.5").Select(
                   p => new { ProjectID = p.ID, ProjectName = p.ClientDescription + " " + p.SetName + " " + "[" + p.AryaCodeBaseVersion + "]" }).OrderBy(
                       p => p.ProjectName).Cast<object>().ToList();
            }
            else
            {
                existingProjects = allProjects.Where(p => p.UserProjects.Any(up => up.UserID == CurrentUserID && up.GroupID == Group.RoleManagerGroup && up.Project.AryaCodeBaseVersion == "4.5")).Select(
                   p => new { ProjectID = p.ID, ProjectName = p.ClientDescription + " " + p.SetName + " " + "[" + p.AryaCodeBaseVersion + "]" }).OrderBy(
                       p => p.ProjectName).Cast<object>().ToList();
            }

            if (!existingProjects.Any())
                return;

            clbProjects.BeginUpdate();
            clbProjects.DataSource = existingProjects;
            clbProjects.DisplayMember = "ProjectName";
            clbProjects.ValueMember = "ProjectID";
            clbProjects.CheckedIndices.OfType<int>().ForEach(a => clbProjects.SetItemChecked(a, false));
            clbProjects.EndUpdate();
        }

        private void LoadGroups()
        {
            ResetSelectionTextBox(lblSelectedGroups);

            var existingGroups =
                _currentDB.Groups.Where(p => p.ID == Group.DefaultGroupID).Select(
                    p => new { GroupName = p.Name, GroupID = p.ID }).ToList().Union(
                        _currentDB.Groups.Where(p => p.ID != Group.DefaultGroupID).Select(
                            p => new { GroupName = p.Name, GroupID = p.ID }).OrderBy(p => p.GroupName)).ToList();

            if (!existingGroups.Any())
                return;

            clbGroups.BeginUpdate();
            clbGroups.DataSource = existingGroups;
            clbGroups.DisplayMember = "GroupName";
            clbGroups.ValueMember = "GroupID";
            clbGroups.CheckedIndices.OfType<int>().ForEach(a => clbGroups.SetItemChecked(a, false));

            clbGroups.SetItemCheckState(0, CheckState.Checked);

            clbGroups.EndUpdate();
        }

        private void LoadUPGs()
        {
            string oldFilter = null;
            if (dgvUserProjectGroups.DataSource != null && dgvUserProjectGroups.DataSource is BindingSource)
            {
                oldFilter = ((BindingSource)dgvUserProjectGroups.DataSource).Filter;
            }
            var dataSource = new BindingSource(_currentDB.UserProjects.ToList().ToDataTable(), null);

            if (!string.IsNullOrEmpty(oldFilter)) dataSource.Filter = oldFilter;

            dgvUserProjectGroups.DataSource = dataSource;
        }

        private void ToggleAddUPGButton()
        {
            btnAddUPG.Enabled = clbUsers.CheckedItems.Count > 0 && clbProjects.CheckedItems.Count > 0 &&
                                clbGroups.CheckedItems.Count > 0;
        }

        private void ToggleDeleteUPGButton()
        {
            btnDeleteUPG.Enabled = (from row in
                                        dgvUserProjectGroups.Rows.Cast<DataGridViewRow>().Where(
                                            p => p.Cells[0].Value != null)
                                    let deleteCheckboxCell = row.Cells[1] as DataGridViewCheckBoxCell
                                    where
                                        deleteCheckboxCell != null && deleteCheckboxCell.Value != null &&
                                        (bool)deleteCheckboxCell.Value
                                    select row).Any();
        }

        public void Run(Guid currentUserID = default(Guid))
        {
            _currentDB = new SkuDataDbDataContext();
            clbProjects.Tag = lblSelectedProjects;
            clbGroups.Tag = lblSelectedGroups;
            clbUsers.Tag = lblSelectedUsers;
            CurrentUserID = currentUserID;

            //only admin can create new users
            if (currentUserID == default(Guid))
            {
                btnUsers.Enabled = true;
                clbProjects.ContextMenuStrip = cmsProjects;
            }
            else
            {
                btnUsers.Enabled = false;
                clbProjects.ContextMenuStrip = null;
            }

            LoadUsers();
            LoadProjects();
            LoadGroups();
            LoadUPGs();
        }

        private void clbUsersGroupsProjects_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var currentClb = (CheckedListBox)sender;

            if (!_userCheckingItem)
            {
                _userCheckingItem = true;
                currentClb.SetItemCheckState(e.Index, e.NewValue);

                if (currentClb.Name == "clbGroups" && currentClb.CheckedItems.Count == 0)
                {
                    if (e.Index == 0) e.NewValue = CheckState.Checked;
                    clbGroups.SetItemCheckState(0, CheckState.Checked);
                }

                _userCheckingItem = false;
                ToggleAddUPGButton();
            }

            var selectedLabel = (Label)currentClb.Tag;

            if (currentClb.CheckedIndices.Count > 0)
            {
                List<string> selectedItems =
                    currentClb.CheckedItems.OfType<object>().Select(currentClb.GetItemText).ToList();

                selectedLabel.Text = selectedItems.
                    Aggregate((a, b) => string.Format("{0}, {1}", a, b));
                tpAdminView.SetToolTip(selectedLabel,
                                       string.Format("Selected {0} : {1}", selectedLabel.Tag, Environment.NewLine) +
                                       selectedItems.Aggregate(
                                           (a, b) => string.Format("{0}{2}{1}", a, b, Environment.NewLine)));
            }
            else
            {
                ResetSelectionTextBox(selectedLabel);
            }
        }

        private void btnGroups_Click(object sender, EventArgs e)
        {
            (new FrmGroupManager()).ShowDialog();
            LoadGroups();
        }

        private void tsbReload_Click(object sender, EventArgs e)
        {
            LoadGroups();
            LoadUsers();
            LoadProjects();
            LoadUPGs();

            ToggleAddUPGButton();
            ToggleDeleteUPGButton();
        }

        private void btnAddUPG_Click(object sender, EventArgs e)
        {
            List<Guid> selectedUsers = (from object currentItem in clbUsers.CheckedItems
                                        select currentItem.GetType().GetProperty("UserID").GetValue(currentItem, null)).
                OfType<Guid>().ToList();
            List<Guid> selectedGroups = (from object currentItem in clbGroups.CheckedItems
                                         select currentItem.GetType().GetProperty("GroupID").GetValue(currentItem, null))
                .OfType<Guid>().ToList();
            List<Guid> selectedProjects = (from object currentItem in clbProjects.CheckedItems
                                           select
                                               currentItem.GetType().GetProperty("ProjectID").GetValue(currentItem, null))
                .OfType<Guid>().ToList();

            var userLists = _currentDB.Users.Where(p => selectedUsers.Contains(p.ID)).ToList();
            var groupList = _currentDB.Groups.Where(p => selectedGroups.Contains(p.ID)).ToList();
            var dbLists = _currentDB.Projects.Where(p => selectedProjects.Contains(p.ID)).Select(d => d.DatabaseName).Distinct().ToList();

            foreach (var db in dbLists)
            {
                using (var admindb = new SkuDataDbDataContext())
                {
                    admindb.Connection.Open();
                    admindb.Connection.ChangeDatabase(db);

                    var missingUsers = selectedUsers.Except(admindb.Users.Select(p => p.ID));

                    foreach (var usr in missingUsers)
                    {
                        var currentMissingUser = usr;
                        admindb.Users.InsertOnSubmit(userLists.Single(p => p.ID == currentMissingUser).CloneEntity());
                    }

                    var missingGroups = selectedGroups.Except(admindb.Groups.Select(p => p.ID));

                    foreach (var grp in missingGroups)
                    {
                        var currentMissingGrp = grp;
                        admindb.Groups.InsertOnSubmit(groupList.Single(p => p.ID == currentMissingGrp).CloneEntity());
                    }

                    admindb.SubmitChanges();
                }
            }

            var newUPGs = selectedUsers.SelectMany(
                selectedUserID =>
                selectedProjects.SelectMany(selectedProjectID => (from selectedGroupID in selectedGroups
                                                                  let userID = selectedUserID
                                                                  let projectID = selectedProjectID
                                                                  let groupID = selectedGroupID
                                                                  let newUPG =
                                                                      _currentDB.UserProjects.SingleOrDefault(p => p.UserID == userID &&
                                                                                                                  p.ProjectID == projectID &&
                                                                                                                  p.GroupID == groupID)
                                                                  where newUPG == null
                                                                  select new UserProject
                                                                             {
                                                                                 ID = Guid.NewGuid(),
                                                                                 ProjectID = selectedProjectID,
                                                                                 UserID = selectedUserID,
                                                                                 GroupID = selectedGroupID
                                                                             }))).ToList();

            foreach (UserProject newUPG in newUPGs)
            {
                _currentDB.UserProjects.InsertOnSubmit(newUPG);
            }

            foreach (var db in dbLists)
            {
                using (var admindb = new SkuDataDbDataContext())
                {
                    admindb.Connection.Open();
                    admindb.Connection.ChangeDatabase(db);
                    var currentDbProjects = admindb.Projects.Select(p => p.ID).ToList();

                    foreach (var newUPG in newUPGs.Where(p => currentDbProjects.Contains(p.ProjectID)))
                    {
                        var currentNewUPG = newUPG;
                        admindb.UserProjects.InsertOnSubmit(currentNewUPG.CloneEntity());
                    }

                    admindb.SubmitChanges();
                }
            }

            //AryaTools.Instance.SaveChangesIfNecessary(false, true);
            SaveCurrentDb();

            LoadUPGs();
        }

        private void btnDeleteUPG_Click(object sender, EventArgs e)
        {
            List<Guid> deletedUPGIDs =
                (from row in
                     dgvUserProjectGroups.Rows.Cast<DataGridViewRow>().Where(
                         p => p.Cells[0].Value != null)
                 let deleteCheckboxCell = row.Cells[1] as DataGridViewCheckBoxCell
                 where deleteCheckboxCell != null && deleteCheckboxCell.Value != null && (bool)deleteCheckboxCell.Value
                 select (Guid)row.Cells[0].Value).ToList();

            _currentDB.UserProjects.DeleteAllOnSubmit(
                _currentDB.UserProjects.Where(p => deletedUPGIDs.Contains(p.ID)));

            var dbList =
                _currentDB.UserProjects.Where(p => deletedUPGIDs.Contains(p.ID)).Select(
                    p => p.Project.DatabaseName).Distinct().ToList();

            foreach (var db in dbList)
            {
                using (var admindb = new SkuDataDbDataContext())
                {
                    admindb.Connection.Open();
                    admindb.Connection.ChangeDatabase(db);
                    admindb.UserProjects.DeleteAllOnSubmit(
                        admindb.UserProjects.Where(p => deletedUPGIDs.Contains(p.ID)));
                    admindb.SubmitChanges();
                }
            }

            //AryaTools.Instance.SaveChangesIfNecessary(false, true);

            SaveCurrentDb();

            LoadUPGs();

            ToggleDeleteUPGButton();
        }

        private void SaveCurrentDb()
        {
            var messageID = FrmWaitScreen.ShowMessage("Saving Changes ....");
            _currentDB.SubmitChanges();
            FrmWaitScreen.HideMessage(messageID);
        }

        private void dgvUserProjectGroups_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1) return;

            if (dgvUserProjectGroups.CurrentCell.IsInEditMode)
            {
                dgvUserProjectGroups.EndEdit();
            }

            ToggleDeleteUPGButton();
        }

        private void tsbRemoveFilter_Click(object sender, EventArgs e)
        {
            DataGridViewAutoFilterColumnHeaderCell.RemoveFilter(dgvUserProjectGroups);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            (new FrmNewUser()).ShowDialog();
            LoadUsers();
            ToggleAddUPGButton();
            ToggleDeleteUPGButton();
        }

        private void btnProjects_Click(object sender, EventArgs e)
        {
            var projectManager = new FrmProjectManager();
            projectManager.Run();
        }

        private void editProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clbProjects.SelectedItem != null)
            {
                var selectedProject = ((dynamic)(clbProjects.SelectedItem)).ProjectID;
                var projectManager = new FrmProjectManager();
                projectManager.Run(selectedProject);
            }

        }
    }
}
