using System;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya
{
    public partial class FrmUserPreferences : Form
    {
        public FrmUserPreferences()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        private void FrmUserPreferences_Load(object sender, EventArgs e)
        {
            pgAttrPrefs.SelectedObject = AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences;
            pgAttrPrefs.Text = @"Attribute User Preferences";
        }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences =
                (UserProjectsPreferences) pgAttrPrefs.SelectedObject;
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            Close();
        }
    }
}