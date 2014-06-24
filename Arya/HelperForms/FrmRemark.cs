namespace Arya.HelperForms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using Data;
    using HelperClasses;
    using Properties;

    public partial class FrmRemark : Form
    {
        public const string DefaultRemark = "Default Remark";
        public const string DefaultRemarkID = "00000000-0000-0000-0000-000000000000";
        private List<Remark> _remarkList = new List<Remark>();

        public FrmRemark()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
            InitDefaultRemark();
        }

        private void InitDefaultRemark()
        {
            chkBoxShowAll.Checked = false;
            var defRemark =
                AryaTools.Instance.InstanceData.Dc.Remarks.FirstOrDefault(r => r.Remark1.ToLower() == DefaultRemark.ToLower());

            if (defRemark == null) //Add Default Remark, if it does not exist
            {
                var rm = new Remark { IsCanned = true, Remark1 = DefaultRemark, ID = new Guid( DefaultRemarkID) };
                AryaTools.Instance.InstanceData.Dc.Remarks.InsertOnSubmit(rm);
                AryaTools.Instance.SaveChangesIfNecessary(true, true);
            }
            else if (
                !AryaTools.Instance.InstanceData.Dc.Remarks.Any(
                    r => r.Remark1.ToLower() == DefaultRemark.ToLower() && r.IsCanned == true))
            {
                defRemark.IsCanned = true;
                AryaTools.Instance.SaveChangesIfNecessary(true, true);
            }

            _remarkList = AryaTools.Instance.InstanceData.Dc.Remarks.OrderBy(r => r.Remark1).ToList();
            comboBoxRemarks.DataSource = _remarkList.Where(c => c.IsCanned == true).Select(r => r.Remark1).ToList();
            var defalutRemark = _remarkList.FirstOrDefault(c => c.IsCanned == true && c.Remark1 == DefaultRemark);
            if (defalutRemark != null)
            {
                comboBoxRemarks.SelectedItem = defalutRemark.Remark1;
                chkBoxIsCanned.Checked = defalutRemark.IsCanned ?? false;
            }
        }

        private void comboBoxRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ProcessRemark();
        }

        private void ProcessRemark(bool reassignDataSource = true)
            // because reassigning the data source will trigger selection changed event.
        {
            if (string.IsNullOrEmpty(comboBoxRemarks.Text))
            {
                if (!chkBoxShowAll.Checked)
                {
                    chkBoxShowAll.Checked = false;
                    comboBoxRemarks.SelectedItem = DefaultRemark;
                }
                else
                    chkBoxIsCanned.Checked = true;
                return;
            }

            var remark = _remarkList.FirstOrDefault(a => a.Remark1.ToLower() == comboBoxRemarks.Text.ToLower());
            if (remark == null)
            {
                var rm = new Remark {IsCanned = chkBoxIsCanned.Checked, Remark1 = comboBoxRemarks.Text};
                AryaTools.Instance.InstanceData.Dc.Remarks.InsertOnSubmit(rm);
                _remarkList.Add(rm);
                if (reassignDataSource)
                    SetDataSource(rm.IsCanned ?? false);
                comboBoxRemarks.SelectedItem = rm.Remark1;
                if (chkBoxStick.Checked)
                    AryaTools.Instance.StickyRemark = rm;
            }
            else
            {
                if (remark.IsCanned != chkBoxIsCanned.Checked)
                    remark.IsCanned = chkBoxIsCanned.Checked;
                if (reassignDataSource)
                    SetDataSource(remark.IsCanned ?? false);
                comboBoxRemarks.SelectedItem = remark.Remark1;

                if (chkBoxStick.Checked)
                    AryaTools.Instance.StickyRemark = remark;
            }
        }

        private void SetDataSource(bool isRemarkCanned)
        {
            if (isRemarkCanned)
            {
                chkBoxShowAll.Checked = false;
                comboBoxRemarks.DataSource = _remarkList.Where(c => c.IsCanned == true).Select(r => r.Remark1).ToList();
            }
            else
            {
                chkBoxShowAll.Checked = true;
                comboBoxRemarks.DataSource = _remarkList.Select(r => r.Remark1).ToList();
            }
        }

        private void FrmRemark_Deactivate(object sender, EventArgs e)
        {
            ProcessRemark();
        }

        private void chkBoxStick_CheckedChanged(object sender, EventArgs e)
        {
            ProcessRemark(false);
            if (chkBoxStick.Checked)
            {
                AryaTools.Instance.StickyRemark =
                    _remarkList.FirstOrDefault(
                        r => r.Remark1 == comboBoxRemarks.Text.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                if (comboBoxRemarks.SelectedItem != null &&
                    comboBoxRemarks.SelectedItem.ToString() == AryaTools.Instance.StickyRemark.Remark1)
                    AryaTools.Instance.StickyRemark = null;
            }
        }

        private void chkBoxShowAll_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRemarks.DataSource = chkBoxShowAll.Checked
                                             ? _remarkList.Select(r => r.Remark1).ToList()
                                             : _remarkList.Where(c => c.IsCanned == true).Select(r => r.Remark1).ToList();
        }

        public void CenterRemarksToScreen()
        {
            CenterToScreen();
        }

        private void chkBoxIsCanned_CheckedChanged(object sender, EventArgs e)
        {
            ProcessRemark();
        }

        private void comboBoxRemarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = comboBoxRemarks.SelectedItem;

            if (selectedItem != null)
            {
                if (AryaTools.Instance.StickyRemark != null)
                    chkBoxStick.Checked = (selectedItem.ToString() == AryaTools.Instance.StickyRemark.Remark1);
                else
                    chkBoxStick.Checked = false;

                var isCannedRemark = chkBoxShowAll.Checked
                                         ? _remarkList.Where(r => r.Remark1 == selectedItem.ToString()).Select(
                                             a => a.IsCanned).FirstOrDefault() ?? false
                                         : _remarkList.Where(r => r.Remark1 == selectedItem.ToString()).Where(
                                             r => r.IsCanned == true).Select(a => a.IsCanned).FirstOrDefault() ?? false;

                chkBoxIsCanned.Checked = isCannedRemark;

                AryaTools.Instance.RemarkID =
                    _remarkList.Where(r => r.Remark1 == selectedItem.ToString()).Select(rid => rid.ID).FirstOrDefault();
            }
        }

        private void FrmRemark_FormClosed(object sender, FormClosedEventArgs e)
        {
            AryaTools.Instance.RemarkID = null;
            AryaTools.Instance.CurrentRemark = string.Empty;
        }
    }
}