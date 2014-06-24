namespace Arya.HelperForms
{
    partial class FrmGroupManager
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
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.GroupID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.GroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsGroups = new System.Windows.Forms.BindingSource(this.components);
            this.tsGroups = new System.Windows.Forms.ToolStrip();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.tlpGroups = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsGroups)).BeginInit();
            this.tsGroups.SuspendLayout();
            this.tlpGroups.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvGroups
            // 
            this.dgvGroups.AllowUserToResizeRows = false;
            this.dgvGroups.AutoGenerateColumns = false;
            this.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GroupID,
            this.Delete,
            this.GroupName,
            this.ClientName,
            this.Description});
            this.dgvGroups.DataSource = this.bsGroups;
            this.dgvGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGroups.Location = new System.Drawing.Point(3, 28);
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.RowHeadersVisible = false;
            this.dgvGroups.Size = new System.Drawing.Size(529, 231);
            this.dgvGroups.TabIndex = 0;
            this.dgvGroups.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvGroups_CellBeginEdit);
            // 
            // GroupID
            // 
            this.GroupID.DataPropertyName = "ID";
            this.GroupID.HeaderText = "GroupID";
            this.GroupID.Name = "GroupID";
            this.GroupID.Visible = false;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Width = 45;
            // 
            // GroupName
            // 
            this.GroupName.DataPropertyName = "Name";
            this.GroupName.HeaderText = "Group Name";
            this.GroupName.Name = "GroupName";
            // 
            // ClientName
            // 
            this.ClientName.DataPropertyName = "ClientName";
            this.ClientName.HeaderText = "Client Name";
            this.ClientName.Name = "ClientName";
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Width = 200;
            // 
            // bsGroups
            // 
            this.bsGroups.CurrentChanged += new System.EventHandler(this.bsGroups_CurrentItemChanged);
            // 
            // tsGroups
            // 
            this.tsGroups.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsGroups.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSave,
            this.tsbRefresh});
            this.tsGroups.Location = new System.Drawing.Point(0, 0);
            this.tsGroups.Name = "tsGroups";
            this.tsGroups.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsGroups.Size = new System.Drawing.Size(535, 25);
            this.tsGroups.TabIndex = 1;
            this.tsGroups.Text = "toolStrip1";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::Arya.Properties.Resources.Save;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Save";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRefresh.Image = global::Arya.Properties.Resources.Refresh;
            this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbRefresh.Text = "Refresh";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // tlpGroups
            // 
            this.tlpGroups.ColumnCount = 1;
            this.tlpGroups.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGroups.Controls.Add(this.dgvGroups, 0, 1);
            this.tlpGroups.Controls.Add(this.tsGroups, 0, 0);
            this.tlpGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpGroups.Location = new System.Drawing.Point(0, 0);
            this.tlpGroups.Name = "tlpGroups";
            this.tlpGroups.RowCount = 2;
            this.tlpGroups.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpGroups.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGroups.Size = new System.Drawing.Size(535, 262);
            this.tlpGroups.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ID";
            this.dataGridViewTextBoxColumn1.HeaderText = "GroupID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn2.HeaderText = "Group Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "ClientName";
            this.dataGridViewTextBoxColumn3.HeaderText = "Client Name";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Description";
            this.dataGridViewTextBoxColumn4.HeaderText = "Description";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 200;
            // 
            // FrmGroupManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 262);
            this.Controls.Add(this.tlpGroups);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmGroupManager";
            this.Text = "Groups";
            this.Load += new System.EventHandler(this.FrmGroupManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsGroups)).EndInit();
            this.tsGroups.ResumeLayout(false);
            this.tsGroups.PerformLayout();
            this.tlpGroups.ResumeLayout(false);
            this.tlpGroups.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvGroups;
        private System.Windows.Forms.ToolStrip tsGroups;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.BindingSource bsGroups;
        private System.Windows.Forms.TableLayoutPanel tlpGroups;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    }
}