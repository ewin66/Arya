namespace Natalie
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using Data;
    using HelperClasses;
    using Properties;

    public partial class FrmAttributePrefs : Form
    {
        private UserProjectsPreferences _attributePrefs = new UserProjectsPreferences();
        private UserProject _userProject;

        public FrmAttributePrefs()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.BMI_Logo;
            pgAttrPrefs.SelectedObject = _attributePrefs;
        }

        private void FrmAttributePrefs_Load(object sender, EventArgs e)
        {
            var ups = (from up in NatalieTools.Instance.Dc.UserProjects
                       where
                           up.UserID == NatalieTools.Instance.CurrentUser.ID &&
                           up.ProjectID == NatalieTools.Instance.CurrentProject.ID
                       select up).ToList();
            _userProject = ups.FirstOrDefault(up => up.Preferences != null) ?? ups.OrderBy(up => up.GroupID).First();

            pgAttrPrefs.SelectedObject = _userProject.UserProjectPreferences;
            _attributePrefs = _userProject.UserProjectPreferences;

            pgAttrPrefs.Text = @"Attribute User Preferences";
        }

        private void FrmAttributePrefs_FormClosing(object sender, FormClosingEventArgs e)
        {
            _userProject.UserProjectPreferences = _attributePrefs;
            NatalieTools.Instance.SaveChangesIfNecessary(false, false);
        }
    }
}