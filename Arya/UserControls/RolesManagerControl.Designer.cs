namespace Arya.UserControls
{
    partial class RolesManagerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RolesManagerControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            this.userTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.clbGroups = new System.Windows.Forms.CheckedListBox();
            this.clbProjects = new System.Windows.Forms.CheckedListBox();
            this.tsUserManager = new System.Windows.Forms.ToolStrip();
            this.tsbReload = new System.Windows.Forms.ToolStripButton();
            this.tsbRemoveFilter = new System.Windows.Forms.ToolStripButton();
            this.clbUsers = new System.Windows.Forms.CheckedListBox();
            this.dgvUserProjectGroups = new System.Windows.Forms.DataGridView();
            this.UPGID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UserName = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.ProjectName = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.GroupName = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.btnDeleteUPG = new System.Windows.Forms.Button();
            this.btnProjects = new System.Windows.Forms.Button();
            this.btnGroups = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.lblSelectedUsers = new System.Windows.Forms.Label();
            this.lblSelectedProjects = new System.Windows.Forms.Label();
            this.lblSelectedGroups = new System.Windows.Forms.Label();
            this.btnAddUPG = new System.Windows.Forms.Button();
            this.tpAdminView = new System.Windows.Forms.ToolTip(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewAutoFilterTextBoxColumn1 = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dataGridViewAutoFilterTextBoxColumn2 = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dataGridViewAutoFilterTextBoxColumn3 = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.cmsProjects = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userTablePanel.SuspendLayout();
            this.tsUserManager.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserProjectGroups)).BeginInit();
            this.cmsProjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // userTablePanel
            // 
            this.userTablePanel.ColumnCount = 4;
            this.userTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.userTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.userTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.userTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.userTablePanel.Controls.Add(this.clbGroups, 2, 2);
            this.userTablePanel.Controls.Add(this.clbProjects, 1, 2);
            this.userTablePanel.Controls.Add(this.tsUserManager, 0, 0);
            this.userTablePanel.Controls.Add(this.clbUsers, 0, 2);
            this.userTablePanel.Controls.Add(this.dgvUserProjectGroups, 0, 4);
            this.userTablePanel.Controls.Add(this.btnDeleteUPG, 3, 4);
            this.userTablePanel.Controls.Add(this.btnProjects, 1, 1);
            this.userTablePanel.Controls.Add(this.btnGroups, 2, 1);
            this.userTablePanel.Controls.Add(this.btnUsers, 0, 1);
            this.userTablePanel.Controls.Add(this.lblSelectedUsers, 0, 3);
            this.userTablePanel.Controls.Add(this.lblSelectedProjects, 1, 3);
            this.userTablePanel.Controls.Add(this.lblSelectedGroups, 2, 3);
            this.userTablePanel.Controls.Add(this.btnAddUPG, 3, 2);
            this.userTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userTablePanel.Location = new System.Drawing.Point(0, 0);
            this.userTablePanel.Name = "userTablePanel";
            this.userTablePanel.RowCount = 5;
            this.userTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.userTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.userTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.userTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.userTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.userTablePanel.Size = new System.Drawing.Size(523, 462);
            this.userTablePanel.TabIndex = 3;
            // 
            // clbGroups
            // 
            this.clbGroups.CheckOnClick = true;
            this.clbGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbGroups.FormattingEnabled = true;
            this.clbGroups.IntegralHeight = false;
            this.clbGroups.Location = new System.Drawing.Point(332, 76);
            this.clbGroups.Name = "clbGroups";
            this.clbGroups.Size = new System.Drawing.Size(135, 173);
            this.clbGroups.TabIndex = 15;
            this.clbGroups.Tag = "";
            this.clbGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbUsersGroupsProjects_ItemCheck);
            // 
            // clbProjects
            // 
            this.clbProjects.CheckOnClick = true;
            this.clbProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbProjects.FormattingEnabled = true;
            this.clbProjects.IntegralHeight = false;
            this.clbProjects.Location = new System.Drawing.Point(144, 76);
            this.clbProjects.Name = "clbProjects";
            this.clbProjects.Size = new System.Drawing.Size(182, 173);
            this.clbProjects.TabIndex = 10;
            this.clbProjects.Tag = "";
            this.clbProjects.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbUsersGroupsProjects_ItemCheck);
            // 
            // tsUserManager
            // 
            this.userTablePanel.SetColumnSpan(this.tsUserManager, 4);
            this.tsUserManager.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsUserManager.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbReload,
            this.tsbRemoveFilter});
            this.tsUserManager.Location = new System.Drawing.Point(3, 2);
            this.tsUserManager.Margin = new System.Windows.Forms.Padding(3, 2, 5, 0);
            this.tsUserManager.Name = "tsUserManager";
            this.tsUserManager.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsUserManager.Size = new System.Drawing.Size(515, 25);
            this.tsUserManager.TabIndex = 8;
            this.tsUserManager.Text = "ToolStrip";
            // 
            // tsbReload
            // 
            this.tsbReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReload.Image = global::Arya.Properties.Resources.Refresh;
            this.tsbReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReload.Name = "tsbReload";
            this.tsbReload.Size = new System.Drawing.Size(23, 22);
            this.tsbReload.Text = "Refresh";
            this.tsbReload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tsbReload.Click += new System.EventHandler(this.tsbReload_Click);
            // 
            // tsbRemoveFilter
            // 
            this.tsbRemoveFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemoveFilter.Image = global::Arya.Properties.Resources.filter_delete;
            this.tsbRemoveFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveFilter.Name = "tsbRemoveFilter";
            this.tsbRemoveFilter.Size = new System.Drawing.Size(23, 22);
            this.tsbRemoveFilter.Text = "Clear Filter(s)";
            this.tsbRemoveFilter.Click += new System.EventHandler(this.tsbRemoveFilter_Click);
            // 
            // clbUsers
            // 
            this.clbUsers.CheckOnClick = true;
            this.clbUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbUsers.FormattingEnabled = true;
            this.clbUsers.IntegralHeight = false;
            this.clbUsers.Location = new System.Drawing.Point(3, 76);
            this.clbUsers.Name = "clbUsers";
            this.clbUsers.Size = new System.Drawing.Size(135, 173);
            this.clbUsers.TabIndex = 14;
            this.clbUsers.Tag = "";
            this.clbUsers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbUsersGroupsProjects_ItemCheck);
            // 
            // dgvUserProjectGroups
            // 
            this.dgvUserProjectGroups.AllowUserToAddRows = false;
            this.dgvUserProjectGroups.AllowUserToDeleteRows = false;
            this.dgvUserProjectGroups.AllowUserToResizeRows = false;
            this.dgvUserProjectGroups.CausesValidation = false;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUserProjectGroups.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.dgvUserProjectGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUserProjectGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UPGID,
            this.Delete,
            this.UserName,
            this.ProjectName,
            this.GroupName});
            this.userTablePanel.SetColumnSpan(this.dgvUserProjectGroups, 3);
            this.dgvUserProjectGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUserProjectGroups.Location = new System.Drawing.Point(3, 285);
            this.dgvUserProjectGroups.Name = "dgvUserProjectGroups";
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUserProjectGroups.RowHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.dgvUserProjectGroups.RowHeadersVisible = false;
            this.dgvUserProjectGroups.Size = new System.Drawing.Size(464, 174);
            this.dgvUserProjectGroups.TabIndex = 16;
            this.dgvUserProjectGroups.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUserProjectGroups_CellContentClick);
            // 
            // UPGID
            // 
            this.UPGID.DataPropertyName = "ID";
            this.UPGID.HeaderText = "UPGID";
            this.UPGID.Name = "UPGID";
            this.UPGID.ReadOnly = true;
            this.UPGID.Visible = false;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Delete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Delete.Width = 50;
            // 
            // UserName
            // 
            this.UserName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UserName.DataPropertyName = "UserName";
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.Black;
            this.UserName.DefaultCellStyle = dataGridViewCellStyle18;
            this.UserName.HeaderText = "User Name";
            this.UserName.Name = "UserName";
            this.UserName.ReadOnly = true;
            this.UserName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ProjectName
            // 
            this.ProjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ProjectName.DataPropertyName = "ProjectDescription";
            dataGridViewCellStyle19.ForeColor = System.Drawing.Color.Black;
            this.ProjectName.DefaultCellStyle = dataGridViewCellStyle19;
            this.ProjectName.HeaderText = "Project Name";
            this.ProjectName.Name = "ProjectName";
            this.ProjectName.ReadOnly = true;
            this.ProjectName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // GroupName
            // 
            this.GroupName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.GroupName.DataPropertyName = "GroupName";
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.Black;
            this.GroupName.DefaultCellStyle = dataGridViewCellStyle20;
            this.GroupName.HeaderText = "Group Name";
            this.GroupName.Name = "GroupName";
            this.GroupName.ReadOnly = true;
            this.GroupName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // btnDeleteUPG
            // 
            this.btnDeleteUPG.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDeleteUPG.Enabled = false;
            this.btnDeleteUPG.Image = global::Arya.Properties.Resources.Close;
            this.btnDeleteUPG.Location = new System.Drawing.Point(473, 436);
            this.btnDeleteUPG.Name = "btnDeleteUPG";
            this.btnDeleteUPG.Size = new System.Drawing.Size(47, 23);
            this.btnDeleteUPG.TabIndex = 17;
            this.btnDeleteUPG.UseVisualStyleBackColor = true;
            this.btnDeleteUPG.Click += new System.EventHandler(this.btnDeleteUPG_Click);
            // 
            // btnProjects
            // 
            this.btnProjects.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProjects.FlatAppearance.BorderSize = 0;
            this.btnProjects.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnProjects.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnProjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProjects.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnProjects.Image = ((System.Drawing.Image)(resources.GetObject("btnProjects.Image")));
            this.btnProjects.Location = new System.Drawing.Point(144, 30);
            this.btnProjects.Name = "btnProjects";
            this.btnProjects.Size = new System.Drawing.Size(70, 40);
            this.btnProjects.TabIndex = 23;
            this.btnProjects.Text = "Projects";
            this.btnProjects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnProjects.UseVisualStyleBackColor = true;
            this.btnProjects.Click += new System.EventHandler(this.btnProjects_Click);
            // 
            // btnGroups
            // 
            this.btnGroups.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGroups.FlatAppearance.BorderSize = 0;
            this.btnGroups.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnGroups.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnGroups.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroups.Image = ((System.Drawing.Image)(resources.GetObject("btnGroups.Image")));
            this.btnGroups.Location = new System.Drawing.Point(332, 30);
            this.btnGroups.Name = "btnGroups";
            this.btnGroups.Size = new System.Drawing.Size(68, 40);
            this.btnGroups.TabIndex = 24;
            this.btnGroups.Text = "Groups";
            this.btnGroups.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnGroups.UseVisualStyleBackColor = true;
            this.btnGroups.Click += new System.EventHandler(this.btnGroups_Click);
            // 
            // btnUsers
            // 
            this.btnUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnUsers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.Image = global::Arya.Properties.Resources.user1;
            this.btnUsers.Location = new System.Drawing.Point(3, 30);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(54, 40);
            this.btnUsers.TabIndex = 22;
            this.btnUsers.Text = "Users";
            this.btnUsers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUsers.UseVisualStyleBackColor = true;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // lblSelectedUsers
            // 
            this.lblSelectedUsers.AutoEllipsis = true;
            this.lblSelectedUsers.AutoSize = true;
            this.lblSelectedUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectedUsers.Location = new System.Drawing.Point(3, 252);
            this.lblSelectedUsers.Name = "lblSelectedUsers";
            this.lblSelectedUsers.Size = new System.Drawing.Size(135, 30);
            this.lblSelectedUsers.TabIndex = 25;
            this.lblSelectedUsers.Tag = "User(s)";
            this.lblSelectedUsers.Text = "Selected Users(s)";
            // 
            // lblSelectedProjects
            // 
            this.lblSelectedProjects.AutoEllipsis = true;
            this.lblSelectedProjects.AutoSize = true;
            this.lblSelectedProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectedProjects.Location = new System.Drawing.Point(144, 252);
            this.lblSelectedProjects.Name = "lblSelectedProjects";
            this.lblSelectedProjects.Size = new System.Drawing.Size(182, 30);
            this.lblSelectedProjects.TabIndex = 26;
            this.lblSelectedProjects.Tag = "Project(s)";
            this.lblSelectedProjects.Text = "Selected Project(s)";
            // 
            // lblSelectedGroups
            // 
            this.lblSelectedGroups.AutoEllipsis = true;
            this.lblSelectedGroups.AutoSize = true;
            this.lblSelectedGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectedGroups.Location = new System.Drawing.Point(332, 252);
            this.lblSelectedGroups.Name = "lblSelectedGroups";
            this.lblSelectedGroups.Size = new System.Drawing.Size(135, 30);
            this.lblSelectedGroups.TabIndex = 27;
            this.lblSelectedGroups.Tag = "Group(s)";
            this.lblSelectedGroups.Text = "Selected Group(s)";
            // 
            // btnAddUPG
            // 
            this.btnAddUPG.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAddUPG.Enabled = false;
            this.btnAddUPG.Image = global::Arya.Properties.Resources.Add;
            this.btnAddUPG.Location = new System.Drawing.Point(473, 226);
            this.btnAddUPG.Name = "btnAddUPG";
            this.btnAddUPG.Size = new System.Drawing.Size(47, 23);
            this.btnAddUPG.TabIndex = 21;
            this.btnAddUPG.UseVisualStyleBackColor = true;
            this.btnAddUPG.Click += new System.EventHandler(this.btnAddUPG_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ID";
            this.dataGridViewTextBoxColumn1.HeaderText = "UPGID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Visible = false;
            // 
            // dataGridViewAutoFilterTextBoxColumn1
            // 
            this.dataGridViewAutoFilterTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewAutoFilterTextBoxColumn1.DataPropertyName = "UserName";
            dataGridViewCellStyle22.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewAutoFilterTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewAutoFilterTextBoxColumn1.HeaderText = "User Name";
            this.dataGridViewAutoFilterTextBoxColumn1.Name = "dataGridViewAutoFilterTextBoxColumn1";
            this.dataGridViewAutoFilterTextBoxColumn1.ReadOnly = true;
            this.dataGridViewAutoFilterTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataGridViewAutoFilterTextBoxColumn2
            // 
            this.dataGridViewAutoFilterTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewAutoFilterTextBoxColumn2.DataPropertyName = "ProjectDescription";
            dataGridViewCellStyle23.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewAutoFilterTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle23;
            this.dataGridViewAutoFilterTextBoxColumn2.HeaderText = "Project Name";
            this.dataGridViewAutoFilterTextBoxColumn2.Name = "dataGridViewAutoFilterTextBoxColumn2";
            this.dataGridViewAutoFilterTextBoxColumn2.ReadOnly = true;
            this.dataGridViewAutoFilterTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataGridViewAutoFilterTextBoxColumn3
            // 
            this.dataGridViewAutoFilterTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewAutoFilterTextBoxColumn3.DataPropertyName = "GroupName";
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewAutoFilterTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle24;
            this.dataGridViewAutoFilterTextBoxColumn3.HeaderText = "Group Name";
            this.dataGridViewAutoFilterTextBoxColumn3.Name = "dataGridViewAutoFilterTextBoxColumn3";
            this.dataGridViewAutoFilterTextBoxColumn3.ReadOnly = true;
            this.dataGridViewAutoFilterTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // cmsProjects
            // 
            this.cmsProjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editProjectToolStripMenuItem});
            this.cmsProjects.Name = "cmsProjects";
            this.cmsProjects.Size = new System.Drawing.Size(153, 48);
            // 
            // editProjectToolStripMenuItem
            // 
            this.editProjectToolStripMenuItem.Name = "editProjectToolStripMenuItem";
            this.editProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editProjectToolStripMenuItem.Text = "Edit Project";
            this.editProjectToolStripMenuItem.Click += new System.EventHandler(this.editProjectToolStripMenuItem_Click);
            // 
            // RolesManagerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.userTablePanel);
            this.Name = "RolesManagerControl";
            this.Size = new System.Drawing.Size(523, 462);
            this.userTablePanel.ResumeLayout(false);
            this.userTablePanel.PerformLayout();
            this.tsUserManager.ResumeLayout(false);
            this.tsUserManager.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserProjectGroups)).EndInit();
            this.cmsProjects.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel userTablePanel;
        private System.Windows.Forms.CheckedListBox clbGroups;
        private System.Windows.Forms.CheckedListBox clbProjects;
        private System.Windows.Forms.ToolStrip tsUserManager;
        private System.Windows.Forms.ToolStripButton tsbReload;
        private System.Windows.Forms.ToolStripButton tsbRemoveFilter;
        private System.Windows.Forms.CheckedListBox clbUsers;
        private System.Windows.Forms.DataGridView dgvUserProjectGroups;
        private System.Windows.Forms.Button btnDeleteUPG;
        private System.Windows.Forms.Button btnProjects;
        private System.Windows.Forms.Button btnGroups;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Label lblSelectedUsers;
        private System.Windows.Forms.Label lblSelectedProjects;
        private System.Windows.Forms.Label lblSelectedGroups;
        private System.Windows.Forms.Button btnAddUPG;
        private System.Windows.Forms.ToolTip tpAdminView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dataGridViewAutoFilterTextBoxColumn1;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dataGridViewAutoFilterTextBoxColumn2;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dataGridViewAutoFilterTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn UPGID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Delete;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn UserName;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn ProjectName;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn GroupName;
        private System.Windows.Forms.ContextMenuStrip cmsProjects;
        private System.Windows.Forms.ToolStripMenuItem editProjectToolStripMenuItem;
    }
}
