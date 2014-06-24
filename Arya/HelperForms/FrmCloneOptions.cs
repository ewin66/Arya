using System;
using System.Windows.Forms;
using Arya.HelperClasses;

namespace Arya.HelperForms
{
    public partial class FrmCloneOptions : Form
    {
        #region Constructors (1)

        public FrmCloneOptions()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            SetCloneOptions();
        }

        #endregion Constructors

        #region Properties (5)

        public bool CurrentSchematusOnly
        {
            get { return rbCurrentSchematusOnly.Checked; }
        }

        public bool NonBlankOnly
        {
            get { return chkCloneToNBOnly.Checked; }
        }

        public bool EmptyRanks
        {
            get { return rbCustomSchemati.Checked && chkPrimarySchemati.Checked && chkExcludeRanks.Checked; }
        }

        public bool ListOfValues
        {
            get { return (rbCustomSchemati.Checked && chkListOfValues.Checked) || rbAllSchemati.Checked; }
        }

        public bool PrimarySchemati
        {
            get { return (rbCustomSchemati.Checked && chkPrimarySchemati.Checked) || rbAllSchemati.Checked; }
        }

        public bool SecondarySchemati
        {
            get { return (rbCustomSchemati.Checked && chkSecondarySchemati.Checked) || rbAllSchemati.Checked; }
        }

        #endregion Properties

        #region Methods (7)

        // Private Methods (7) 

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void chkPrimarySchemati_CheckedChanged(object sender, EventArgs e)
        {
            SetCloneOptions();
        }

        private void rbAllSchemati_CheckedChanged(object sender, EventArgs e)
        {
            SetCloneOptions();
        }

        private void rbCurrentSchematusOnly_CheckedChanged(object sender, EventArgs e)
        {
            SetCloneOptions();
        }

        private void rbCustomSchemati_CheckedChanged(object sender, EventArgs e)
        {
            SetCloneOptions();
        }

        private void SetCloneOptions()
        {
            pnlCustomSchemati.Enabled = rbCustomSchemati.Checked;
            chkExcludeRanks.Enabled = chkPrimarySchemati.Checked;
            chkCloneToNBOnly.Enabled = rbCurrentSchematusOnly.Checked;

            if (rbCurrentSchematusOnly.Checked == false) chkCloneToNBOnly.Checked = false;
            
        }

        #endregion Methods
    }
}