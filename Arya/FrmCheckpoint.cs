using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya
{
    public partial class FrmCheckpoint : Form
    {
        #region Constructors (1) 

      private List<CheckPointInfo> _chkPoints = new List<CheckPointInfo>();

        public FrmCheckpoint()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
            dgvCheckpoint.AutoGenerateColumns = false;

            InitData();
        }

        #endregion Constructors 

        #region Methods (3) 

        // Private Methods (3) 

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dlg = MessageBox.Show("Are you sure you want to create a check point?", "Check Point",
                                      MessageBoxButtons.YesNo);

            if (dlg == DialogResult.Yes)
            {
                lblLoadingImg.Visible = true;
                lblLoadingMesssage.Visible = true;

                if (!backgroundWorkerCreateCheckPoint.IsBusy)
                  backgroundWorkerCreateCheckPoint.RunWorkerAsync();
            }

            if (dlg == DialogResult.No)
                return;
        }

        private void FrmCheckpoint_FormClosing(object sender, FormClosingEventArgs e) { AryaTools.Instance.SaveChangesIfNecessary(false, false); }

        private void InitData()
        {
           //  var cps = new BindingList<CheckPointInfo>(AryaTools.Instance.InstanceData.CurrentProject.CheckPointInfos);
            _chkPoints = AryaTools.Instance.InstanceData.Dc.CheckPointInfos.ToList();

            PrepareGridView();
        }

        private void PrepareGridView() { dgvCheckpoint.RowCount = _chkPoints.Count(); }

        #endregion Methods 

        private void dgvCheckpoint_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                if (e.ColumnIndex == 0) // Description
                    e.Value = _chkPoints[e.RowIndex].Description;
                if (e.ColumnIndex == 1) //ProjectName
                    e.Value = _chkPoints[e.RowIndex].Project.ProjectName;
                if (e.ColumnIndex == 2) //Created On
                    e.Value = _chkPoints[e.RowIndex].CreatedOn;
                if (e.ColumnIndex == 3) //Created By
                    e.Value = _chkPoints[e.RowIndex].CreatedBy;
            }
        }

        private void backgroundWorkerCreateCheckPoint_DoWork(object sender, DoWorkEventArgs e)
        {
            AryaTools.Instance.SaveChangesIfNecessary(true, true);
            var dbName = "=" + AryaTools.Instance.InstanceData.Dc.Connection.Database;
            var connString = AryaTools.Instance.InstanceData.Dc.Connection.ConnectionString.Replace("=Arya",
                                                                                                       dbName);
            var conn = new SqlConnection(connString);
            conn.Open();
            var cmd = new SqlCommand("CP.CreateCheckPoint", conn)
                          {CommandTimeout = int.MaxValue, CommandType = CommandType.StoredProcedure};
            cmd.Parameters.AddWithValue("@description", txtDescription.Text);
            cmd.Parameters.AddWithValue("@projectName", AryaTools.Instance.InstanceData.CurrentProject.ProjectName);
            cmd.Parameters.AddWithValue("@createdBy", AryaTools.Instance.InstanceData.CurrentUser.ID.ToString());
            cmd.Parameters.AddWithValue("@dateCreated", dtTimestamp.Value.ToString());
            cmd.ExecuteNonQuery();
        }

        private void backgroundWorkerCreateCheckPoint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblLoadingImg.Visible = false;
            lblLoadingMesssage.Visible = false;
            txtDescription.Text = string.Empty;
            InitData();
        }
    }
}