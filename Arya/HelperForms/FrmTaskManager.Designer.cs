namespace Natalie.HelperForms
{
    partial class FrmTaskManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTaskManager));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbUserGroups = new System.Windows.Forms.ListBox();
            this.tsTaskManager = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnAutoSuggest = new System.Windows.Forms.ToolStripButton();
            this.dgvRoles = new System.Windows.Forms.DataGridView();
            this.RoleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaskID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteRole = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TaskType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaskName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjectID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbProjectList = new System.Windows.Forms.ComboBox();
            this.tlpTaskManager = new System.Windows.Forms.TableLayoutPanel();
            this.btnDeleteTask = new System.Windows.Forms.Button();
            this.cbTaskTypes = new System.Windows.Forms.ComboBox();
            this.tbNewRule = new System.Windows.Forms.TextBox();
            this.btnAddTask = new System.Windows.Forms.Button();
            this.cbPermissionTypes = new System.Windows.Forms.ComboBox();
            this.tsTaskManager.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoles)).BeginInit();
            this.tlpTaskManager.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbUserGroups
            // 
            this.lbUserGroups.FormattingEnabled = true;
            this.lbUserGroups.IntegralHeight = false;
            this.lbUserGroups.Location = new System.Drawing.Point(3, 55);
            this.lbUserGroups.Name = "lbUserGroups";
            this.tlpTaskManager.SetRowSpan(this.lbUserGroups, 4);
            this.lbUserGroups.Size = new System.Drawing.Size(186, 393);
            this.lbUserGroups.TabIndex = 12;
            this.lbUserGroups.SelectedIndexChanged += new System.EventHandler(this.lbUserGroups_SelectedIndexChanged);
            // 
            // tsTaskManager
            // 
            this.tlpTaskManager.SetColumnSpan(this.tsTaskManager, 4);
            this.tsTaskManager.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTaskManager.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.tsbtnRefresh,
            this.btnAutoSuggest});
            this.tsTaskManager.Location = new System.Drawing.Point(0, 0);
            this.tsTaskManager.Name = "tsTaskManager";
            this.tsTaskManager.Size = new System.Drawing.Size(736, 25);
            this.tsTaskManager.TabIndex = 13;
            this.tsTaskManager.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnRefresh
            // 
            this.tsbtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRefresh.Image = global::Natalie.Properties.Resources.Refresh;
            this.tsbtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRefresh.Name = "tsbtnRefresh";
            this.tsbtnRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbtnRefresh.Text = "Refresh";
            this.tsbtnRefresh.Click += new System.EventHandler(this.tsbtnRefresh_Click);
            // 
            // btnAutoSuggest
            // 
            this.btnAutoSuggest.CheckOnClick = true;
            this.btnAutoSuggest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAutoSuggest.Image = ((System.Drawing.Image)(resources.GetObject("btnAutoSuggest.Image")));
            this.btnAutoSuggest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoSuggest.Name = "btnAutoSuggest";
            this.btnAutoSuggest.Size = new System.Drawing.Size(82, 22);
            this.btnAutoSuggest.Text = "Auto Suggest";
            this.btnAutoSuggest.Click += new System.EventHandler(this.btnAutoSuggest_Click);
            // 
            // dgvRoles
            // 
            this.dgvRoles.AllowUserToAddRows = false;
            this.dgvRoles.AllowUserToDeleteRows = false;
            this.dgvRoles.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRoles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRoles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRoles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RoleID,
            this.TaskID,
            this.DeleteRole,
            this.TaskType,
            this.TaskName,
            this.ObjectID});
            this.tlpTaskManager.SetColumnSpan(this.dgvRoles, 2);
            this.dgvRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRoles.Location = new System.Drawing.Point(195, 55);
            this.dgvRoles.Name = "dgvRoles";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRoles.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRoles.RowHeadersVisible = false;
            this.dgvRoles.Size = new System.Drawing.Size(444, 314);
            this.dgvRoles.TabIndex = 9;
            this.dgvRoles.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRoles_CellContentClick);
            // 
            // RoleID
            // 
            this.RoleID.DataPropertyName = "RoleID";
            this.RoleID.HeaderText = "RoleID";
            this.RoleID.Name = "RoleID";
            this.RoleID.Visible = false;
            // 
            // TaskID
            // 
            this.TaskID.DataPropertyName = "TaskID";
            this.TaskID.HeaderText = "TaskID";
            this.TaskID.Name = "TaskID";
            this.TaskID.Visible = false;
            // 
            // DeleteRole
            // 
            this.DeleteRole.HeaderText = "Delete";
            this.DeleteRole.Name = "DeleteRole";
            this.DeleteRole.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.DeleteRole.Width = 50;
            // 
            // TaskType
            // 
            this.TaskType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TaskType.DataPropertyName = "TaskType";
            this.TaskType.FillWeight = 142.8571F;
            this.TaskType.HeaderText = "Type";
            this.TaskType.Name = "TaskType";
            // 
            // TaskName
            // 
            this.TaskName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TaskName.DataPropertyName = "TaskName";
            this.TaskName.FillWeight = 78.57143F;
            this.TaskName.HeaderText = "Name";
            this.TaskName.Name = "TaskName";
            // 
            // ObjectID
            // 
            this.ObjectID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ObjectID.DataPropertyName = "ObjectID";
            this.ObjectID.FillWeight = 78.57143F;
            this.ObjectID.HeaderText = "Object ID";
            this.ObjectID.Name = "ObjectID";
            // 
            // cbProjectList
            // 
            this.cbProjectList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbProjectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProjectList.FormattingEnabled = true;
            this.cbProjectList.Location = new System.Drawing.Point(3, 28);
            this.cbProjectList.Name = "cbProjectList";
            this.cbProjectList.Size = new System.Drawing.Size(186, 21);
            this.cbProjectList.TabIndex = 11;
            this.cbProjectList.SelectedIndexChanged += new System.EventHandler(this.cbProjectList_SelectedIndexChanged);
            // 
            // tlpTaskManager
            // 
            this.tlpTaskManager.ColumnCount = 4;
            this.tlpTaskManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpTaskManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.96914F));
            this.tlpTaskManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.09259F));
            this.tlpTaskManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tlpTaskManager.Controls.Add(this.cbProjectList, 0, 1);
            this.tlpTaskManager.Controls.Add(this.dgvRoles, 1, 2);
            this.tlpTaskManager.Controls.Add(this.lbUserGroups, 0, 2);
            this.tlpTaskManager.Controls.Add(this.tsTaskManager, 0, 0);
            this.tlpTaskManager.Controls.Add(this.btnDeleteTask, 2, 2);
            this.tlpTaskManager.Controls.Add(this.cbTaskTypes, 1, 4);
            this.tlpTaskManager.Controls.Add(this.tbNewRule, 2, 4);
            this.tlpTaskManager.Controls.Add(this.btnAddTask, 3, 4);
            this.tlpTaskManager.Controls.Add(this.cbPermissionTypes, 1, 1);
            this.tlpTaskManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTaskManager.Location = new System.Drawing.Point(0, 0);
            this.tlpTaskManager.Name = "tlpTaskManager";
            this.tlpTaskManager.RowCount = 6;
            this.tlpTaskManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpTaskManager.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTaskManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTaskManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpTaskManager.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTaskManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpTaskManager.Size = new System.Drawing.Size(736, 451);
            this.tlpTaskManager.TabIndex = 15;
            // 
            // btnDeleteTask
            // 
            this.btnDeleteTask.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDeleteTask.Enabled = false;
            this.btnDeleteTask.Image = global::Natalie.Properties.Resources.Close;
            this.btnDeleteTask.Location = new System.Drawing.Point(645, 346);
            this.btnDeleteTask.Name = "btnDeleteTask";
            this.btnDeleteTask.Size = new System.Drawing.Size(88, 23);
            this.btnDeleteTask.TabIndex = 18;
            this.btnDeleteTask.UseVisualStyleBackColor = true;
            this.btnDeleteTask.Click += new System.EventHandler(this.btnDeleteTask_Click);
            // 
            // cbTaskTypes
            // 
            this.cbTaskTypes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbTaskTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTaskTypes.FormattingEnabled = true;
            this.cbTaskTypes.Location = new System.Drawing.Point(195, 402);
            this.cbTaskTypes.Name = "cbTaskTypes";
            this.cbTaskTypes.Size = new System.Drawing.Size(90, 21);
            this.cbTaskTypes.TabIndex = 19;
            // 
            // tbNewRule
            // 
            this.tbNewRule.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbNewRule.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbNewRule.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbNewRule.Location = new System.Drawing.Point(291, 403);
            this.tbNewRule.Name = "tbNewRule";
            this.tbNewRule.Size = new System.Drawing.Size(348, 20);
            this.tbNewRule.TabIndex = 15;
            this.tbNewRule.TextChanged += new System.EventHandler(this.tbNewRule_TextChanged);
            this.tbNewRule.Enter += new System.EventHandler(this.tbNewRule_Enter);
            // 
            // btnAddTask
            // 
            this.btnAddTask.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAddTask.Enabled = false;
            this.btnAddTask.Image = global::Natalie.Properties.Resources.Add;
            this.btnAddTask.Location = new System.Drawing.Point(645, 400);
            this.btnAddTask.Name = "btnAddTask";
            this.btnAddTask.Size = new System.Drawing.Size(88, 23);
            this.btnAddTask.TabIndex = 22;
            this.btnAddTask.UseVisualStyleBackColor = true;
            this.btnAddTask.Click += new System.EventHandler(this.btnAddTask_Click);
            // 
            // cbPermissionTypes
            // 
            this.cbPermissionTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPermissionTypes.FormattingEnabled = true;
            this.cbPermissionTypes.Location = new System.Drawing.Point(195, 28);
            this.cbPermissionTypes.Name = "cbPermissionTypes";
            this.cbPermissionTypes.Size = new System.Drawing.Size(90, 21);
            this.cbPermissionTypes.TabIndex = 23;
            // 
            // FrmTaskManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 451);
            this.Controls.Add(this.tlpTaskManager);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmTaskManager";
            this.Text = "Task Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTaskManager_FormClosing);
            this.Load += new System.EventHandler(this.FrmTaskManager_Load);
            this.tsTaskManager.ResumeLayout(false);
            this.tsTaskManager.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoles)).EndInit();
            this.tlpTaskManager.ResumeLayout(false);
            this.tlpTaskManager.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbUserGroups;
        private System.Windows.Forms.DataGridView dgvRoles;
        private System.Windows.Forms.ToolStrip tsTaskManager;
        private System.Windows.Forms.ToolStripButton tsbtnRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel tlpTaskManager;
        private System.Windows.Forms.ComboBox cbProjectList;
        private System.Windows.Forms.TextBox tbNewRule;
        private System.Windows.Forms.Button btnDeleteTask;
        private System.Windows.Forms.ComboBox cbTaskTypes;
        private System.Windows.Forms.Button btnAddTask;
        private System.Windows.Forms.DataGridViewTextBoxColumn RoleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DeleteRole;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskType;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectID;
        private System.Windows.Forms.ToolStripButton btnAutoSuggest;
        private System.Windows.Forms.ComboBox cbPermissionTypes;
    }
}