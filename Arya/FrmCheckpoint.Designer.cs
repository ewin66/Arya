namespace Arya
{
    partial class FrmCheckpoint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCheckpoint));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlNewCheckpoint = new System.Windows.Forms.TableLayoutPanel();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dtTimestamp = new System.Windows.Forms.DateTimePicker();
            this.lblLoadingMesssage = new System.Windows.Forms.Label();
            this.lblLoadingImg = new System.Windows.Forms.Label();
            this.grpNew = new System.Windows.Forms.GroupBox();
            this.dgvCheckpoint = new System.Windows.Forms.DataGridView();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.projectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpExistingCheckpoints = new System.Windows.Forms.GroupBox();
            this.checkPointInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.backgroundWorkerCreateCheckPoint = new System.ComponentModel.BackgroundWorker();
            this.pnlNewCheckpoint.SuspendLayout();
            this.grpNew.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckpoint)).BeginInit();
            this.grpExistingCheckpoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkPointInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlNewCheckpoint
            // 
            this.pnlNewCheckpoint.ColumnCount = 4;
            this.pnlNewCheckpoint.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlNewCheckpoint.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlNewCheckpoint.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlNewCheckpoint.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlNewCheckpoint.Controls.Add(this.txtDescription, 0, 2);
            this.pnlNewCheckpoint.Controls.Add(this.label1, 0, 1);
            this.pnlNewCheckpoint.Controls.Add(this.btnAdd, 0, 3);
            this.pnlNewCheckpoint.Controls.Add(this.label2, 0, 0);
            this.pnlNewCheckpoint.Controls.Add(this.dtTimestamp, 1, 0);
            this.pnlNewCheckpoint.Controls.Add(this.lblLoadingMesssage, 2, 3);
            this.pnlNewCheckpoint.Controls.Add(this.lblLoadingImg, 3, 3);
            this.pnlNewCheckpoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNewCheckpoint.Location = new System.Drawing.Point(3, 16);
            this.pnlNewCheckpoint.Name = "pnlNewCheckpoint";
            this.pnlNewCheckpoint.Padding = new System.Windows.Forms.Padding(3);
            this.pnlNewCheckpoint.RowCount = 4;
            this.pnlNewCheckpoint.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlNewCheckpoint.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.pnlNewCheckpoint.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlNewCheckpoint.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlNewCheckpoint.Size = new System.Drawing.Size(618, 155);
            this.pnlNewCheckpoint.TabIndex = 0;
            // 
            // txtDescription
            // 
            this.pnlNewCheckpoint.SetColumnSpan(this.txtDescription, 4);
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(6, 56);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(606, 64);
            this.txtDescription.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Description:";
            // 
            // btnAdd
            // 
            this.pnlNewCheckpoint.SetColumnSpan(this.btnAdd, 2);
            this.btnAdd.Location = new System.Drawing.Point(6, 126);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(111, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add Checkpoint";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Timestamp:";
            // 
            // dtTimestamp
            // 
            this.dtTimestamp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dtTimestamp.CustomFormat = "yyyy/MM/dd h:mm:ss tt";
            this.dtTimestamp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTimestamp.Location = new System.Drawing.Point(75, 6);
            this.dtTimestamp.Name = "dtTimestamp";
            this.dtTimestamp.Size = new System.Drawing.Size(169, 20);
            this.dtTimestamp.TabIndex = 1;
            // 
            // lblLoadingMesssage
            // 
            this.lblLoadingMesssage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLoadingMesssage.AutoSize = true;
            this.lblLoadingMesssage.Location = new System.Drawing.Point(250, 131);
            this.lblLoadingMesssage.Name = "lblLoadingMesssage";
            this.lblLoadingMesssage.Size = new System.Drawing.Size(220, 13);
            this.lblLoadingMesssage.TabIndex = 8;
            this.lblLoadingMesssage.Text = "Please wait while the check point is created..";
            this.lblLoadingMesssage.Visible = false;
            // 
            // lblLoadingImg
            // 
            this.lblLoadingImg.AutoSize = true;
            this.lblLoadingImg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLoadingImg.Image = ((System.Drawing.Image)(resources.GetObject("lblLoadingImg.Image")));
            this.lblLoadingImg.Location = new System.Drawing.Point(476, 123);
            this.lblLoadingImg.Name = "lblLoadingImg";
            this.lblLoadingImg.Size = new System.Drawing.Size(136, 29);
            this.lblLoadingImg.TabIndex = 9;
            this.lblLoadingImg.Visible = false;
            // 
            // grpNew
            // 
            this.grpNew.Controls.Add(this.pnlNewCheckpoint);
            this.grpNew.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpNew.Location = new System.Drawing.Point(0, 0);
            this.grpNew.Name = "grpNew";
            this.grpNew.Size = new System.Drawing.Size(624, 174);
            this.grpNew.TabIndex = 0;
            this.grpNew.TabStop = false;
            this.grpNew.Text = "New Checkpoint";
            // 
            // dgvCheckpoint
            // 
            this.dgvCheckpoint.AllowUserToAddRows = false;
            this.dgvCheckpoint.AllowUserToDeleteRows = false;
            this.dgvCheckpoint.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvCheckpoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCheckpoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Description,
            this.projectName,
            this.createdOn,
            this.createdBy});
            this.dgvCheckpoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCheckpoint.Location = new System.Drawing.Point(3, 16);
            this.dgvCheckpoint.Name = "dgvCheckpoint";
            this.dgvCheckpoint.ReadOnly = true;
            this.dgvCheckpoint.RowHeadersVisible = false;
            this.dgvCheckpoint.Size = new System.Drawing.Size(618, 249);
            this.dgvCheckpoint.TabIndex = 1;
            this.dgvCheckpoint.VirtualMode = true;
            this.dgvCheckpoint.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvCheckpoint_CellValueNeeded);
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Description.DefaultCellStyle = dataGridViewCellStyle1;
            this.Description.HeaderText = "Description";
            this.Description.MaxInputLength = 4000;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 300;
            // 
            // projectName
            // 
            this.projectName.HeaderText = "Project Name";
            this.projectName.Name = "projectName";
            this.projectName.ReadOnly = true;
            // 
            // createdOn
            // 
            this.createdOn.HeaderText = "Created On";
            this.createdOn.Name = "createdOn";
            this.createdOn.ReadOnly = true;
            // 
            // createdBy
            // 
            this.createdBy.HeaderText = "Created By";
            this.createdBy.Name = "createdBy";
            this.createdBy.ReadOnly = true;
            // 
            // grpExistingCheckpoints
            // 
            this.grpExistingCheckpoints.Controls.Add(this.dgvCheckpoint);
            this.grpExistingCheckpoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpExistingCheckpoints.Location = new System.Drawing.Point(0, 174);
            this.grpExistingCheckpoints.Name = "grpExistingCheckpoints";
            this.grpExistingCheckpoints.Size = new System.Drawing.Size(624, 268);
            this.grpExistingCheckpoints.TabIndex = 2;
            this.grpExistingCheckpoints.TabStop = false;
            this.grpExistingCheckpoints.Text = "Existing Checkpoints";
            // 
            // checkPointInfoBindingSource
            // 
            this.checkPointInfoBindingSource.DataSource = typeof(Arya.Data.CheckPointInfo);
            // 
            // backgroundWorkerCreateCheckPoint
            // 
            this.backgroundWorkerCreateCheckPoint.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCreateCheckPoint_DoWork);
            this.backgroundWorkerCreateCheckPoint.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerCreateCheckPoint_RunWorkerCompleted);
            // 
            // FrmCheckpoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.grpExistingCheckpoints);
            this.Controls.Add(this.grpNew);
            this.Name = "FrmCheckpoint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Checkpoint";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCheckpoint_FormClosing);
            this.pnlNewCheckpoint.ResumeLayout(false);
            this.pnlNewCheckpoint.PerformLayout();
            this.grpNew.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckpoint)).EndInit();
            this.grpExistingCheckpoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkPointInfoBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlNewCheckpoint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtTimestamp;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox grpNew;
        private System.Windows.Forms.DataGridView dgvCheckpoint;
        private System.Windows.Forms.GroupBox grpExistingCheckpoints;
        private System.Windows.Forms.BindingSource checkPointInfoBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn projectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdBy;
        private System.Windows.Forms.Label lblLoadingMesssage;
        private System.Windows.Forms.Label lblLoadingImg;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCreateCheckPoint;
    }
}