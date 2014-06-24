namespace Arya.HelperForms
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using Arya.Data;
    using Arya.Framework4.State;
    using Arya.HelperClasses;
    using Arya.Properties;

    public partial class FrmSelectProject : Form
    {
        #region Fields

        private readonly bool _firstTime;

        #endregion Fields

        #region Constructors

        public FrmSelectProject(bool firstTime)
            : this()
        {
            //InitializeComponent();
            DialogResult = DialogResult.Cancel;
            _firstTime = firstTime;
        }

        private FrmSelectProject()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        #endregion Constructors

        #region Methods

        // Private Methods (6)
        private void btnSelectProject_Click(object sender, EventArgs e)
        {
            SelectProject();
        }

        private void GetProjects()
        {
            if (AryaTools.Instance.InstanceData.CurrentUser == null)
            {
                MessageBox.Show(@"Cannot choose a project without logging in!");
                Close();
                return;
            }

            var allUserGroups =
                AryaTools.Instance.InstanceData.CurrentUser.UserProjects.Select(p => p.GroupID).Distinct().ToList();

            lnkAdminView.Visible = AryaTools.Instance.InstanceData.CurrentUser.IsAdmin ||
                                   allUserGroups.Contains(Group.RoleManagerGroup) ||
                                   allUserGroups.Contains(Group.PermissionsManagerGroup);

            //this will be ovverwritten when project is selected with Project specific groups
            AryaTools.Instance.InstanceData.CurrentUser.UserGroups = allUserGroups;

            var availableProjects =
                AryaTools.Instance.InstanceData.Dc.ExecuteQuery<Project>(
                    @"SELECT *
                    FROM Project
                    WHERE ID IN (
                        SELECT DISTINCT p.ID
                        FROM UserProject up
                        INNER JOIN Project p ON up.ProjectID = p.ID
                        WHERE up.UserID = {0}
                        AND EXISTS (
                            SELECT database_id FROM sys.databases sd WHERE sd.Name = p.DatabaseName
                        )
                    ) ORDER BY ClientDescription + ' ' + SetName
                   ", AryaTools.Instance.InstanceData.CurrentUser.ID).ToList();

            switch (availableProjects.Count)
            {
                case 0:
                    MessageBox.Show(@"There are no projects configured for you.", "Arya");
                    break;

                    //case 1:
                    //    AryaTools.Instance.InstanceData.CurrentProject = availableProjects.First();
                    //    if (_firstTime)
                    //    {
                    //        DialogResult = DialogResult.OK;

                    //        ddlSelectProject.DataSource = availableProjects;
                    //        ddlSelectProject.DisplayMember = "ProjectDescription";
                    //        //SelectProject();
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show(
                    //            string.Format(
                    //                "You have access to only one project: {0} {1}",
                    //                AryaTools.Instance.InstanceData.CurrentProject.ClientDescription,
                    //                AryaTools.Instance.InstanceData.CurrentProject.SetName));
                    //        DialogResult = DialogResult.Cancel;
                    //    }
                    //    //Close();
                    //    break;

                default:
                    ddlSelectProject.DataSource = availableProjects;
                    ddlSelectProject.DisplayMember = "ProjectDescription";
                    break;
            }

            if (WindowsRegistry.GetFromRegistry(WindowsRegistry.RegistryKeyProject) != null &&
                availableProjects.Exists(
                    prj => prj.ProjectName.Equals(WindowsRegistry.GetFromRegistry(WindowsRegistry.RegistryKeyProject))))
            {
                ddlSelectProject.SelectedItem =
                    availableProjects.First(
                        prj =>
                            prj.ProjectName.Equals(WindowsRegistry.GetFromRegistry(WindowsRegistry.RegistryKeyProject)));
                //if (AryaTools.Instance.AutoLogin)
                //    SelectProject();
            }
        }

        private void lnkAdminView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new FrmAdminView().ShowDialog();
        }

        private void SelectProject()
        {
            var currentProject = (Project) ddlSelectProject.SelectedItem;
            AryaTools.Instance.InstanceData.CurrentProject = currentProject;
            AryaTools.Instance.AutoSave = chkAutoSave.Checked;

            Program.WriteToErrorFile(
                string.Format(
                    "UserID: {0},UserName: {1}, Project: {2}, Hostname: {3}",
                    AryaTools.Instance.InstanceData.CurrentUser.ID,
                    string.IsNullOrEmpty(AryaTools.Instance.InstanceData.CurrentUser.FullName)
                        ? string.Empty
                        : AryaTools.Instance.InstanceData.CurrentUser.FullName,
                    AryaTools.Instance.InstanceData.CurrentProject.ProjectName, Environment.MachineName), true);

            WindowsRegistry.SaveToRegistry(
                WindowsRegistry.RegistryKeyProject, AryaTools.Instance.InstanceData.CurrentProject.ProjectName);

            //AryaTools.Instance.selectedProject = true;

            DialogResult = DialogResult.OK;

            AryaTools.Instance.Forms.TreeForm.Show();
            Hide();
        }

        private void TryLogin(object sender, EventArgs e)
        {
            ddlSelectProject.DataSource = null;
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                return;

            AryaTools.Instance.InstanceData.CurrentUser = (from user in AryaTools.Instance.InstanceData.Dc.Users
                where user.EmailAddress == txtUsername.Text && user.OpenIdentity == txtPassword.Text
                select user).FirstOrDefault();

            GetProjects();
        }

        #endregion Methods
    }
}