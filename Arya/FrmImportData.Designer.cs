namespace Arya
{
    partial class FrmImportData
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lstMapping = new System.Windows.Forms.ListView();
            this.Required = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FieldName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MapTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstFileFields = new System.Windows.Forms.ListBox();
            this.lblAvailableFields = new System.Windows.Forms.Label();
            this.lblMapTo = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.statusTimer = new System.Windows.Forms.Timer(this.components);
            this.btnAbort = new System.Windows.Forms.Button();
            this.tabControlImportData = new System.Windows.Forms.TabControl();
            this.tpProjectProperties = new System.Windows.Forms.TabPage();
            this.chkAttributeNameUomAreOne = new System.Windows.Forms.CheckBox();
            this.chkMarkAsBefore = new System.Windows.Forms.CheckBox();
            this.txtPauseDuration = new System.Windows.Forms.NumericUpDown();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.lblPause = new System.Windows.Forms.Label();
            this.grpImportType = new System.Windows.Forms.GroupBox();
            this.grpUpdateAction = new System.Windows.Forms.GroupBox();
            this.rbAllNewEntities = new System.Windows.Forms.RadioButton();
            this.rbUpsertEntities = new System.Windows.Forms.RadioButton();
            this.rbDeleteExistingEntities = new System.Windows.Forms.RadioButton();
            this.rbInsertUpdate = new System.Windows.Forms.RadioButton();
            this.rbInsertOnly = new System.Windows.Forms.RadioButton();
            this.lblSetName = new System.Windows.Forms.Label();
            this.txtSetLoadName = new System.Windows.Forms.TextBox();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.lblClientName = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.tpFileFieldMapping = new System.Windows.Forms.TabPage();
            this.lblInputFileName = new System.Windows.Forms.Label();
            this.txtInputFilename = new System.Windows.Forms.TextBox();
            this.cboDelimiter = new System.Windows.Forms.ComboBox();
            this.lblDelimiter = new System.Windows.Forms.Label();
            this.btnOpenInputFile = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tpErrorLog = new System.Windows.Forms.TabPage();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.chkBoxNewSkus = new System.Windows.Forms.CheckBox();
            this.tabControlImportData.SuspendLayout();
            this.tpProjectProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPauseDuration)).BeginInit();
            this.grpImportType.SuspendLayout();
            this.grpUpdateAction.SuspendLayout();
            this.tpFileFieldMapping.SuspendLayout();
            this.tpErrorLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Text files|*.txt|All files|*.*";
            // 
            // lstMapping
            // 
            this.lstMapping.AllowDrop = true;
            this.lstMapping.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Required,
            this.FieldName,
            this.MapTo});
            this.lstMapping.FullRowSelect = true;
            this.lstMapping.GridLines = true;
            this.lstMapping.HideSelection = false;
            this.lstMapping.LabelWrap = false;
            this.lstMapping.Location = new System.Drawing.Point(158, 84);
            this.lstMapping.MultiSelect = false;
            this.lstMapping.Name = "lstMapping";
            this.lstMapping.ShowGroups = false;
            this.lstMapping.Size = new System.Drawing.Size(314, 147);
            this.lstMapping.TabIndex = 9;
            this.lstMapping.UseCompatibleStateImageBehavior = false;
            this.lstMapping.View = System.Windows.Forms.View.Details;
            this.lstMapping.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstMapping_DragDrop);
            this.lstMapping.DragOver += new System.Windows.Forms.DragEventHandler(this.lstMapping_DragOver);
            this.lstMapping.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstMapping_KeyUp);
            // 
            // Required
            // 
            this.Required.Text = "Required";
            // 
            // FieldName
            // 
            this.FieldName.Text = "Field Name";
            this.FieldName.Width = 120;
            // 
            // MapTo
            // 
            this.MapTo.Text = "Mapped to";
            this.MapTo.Width = 120;
            // 
            // lstFileFields
            // 
            this.lstFileFields.FormattingEnabled = true;
            this.lstFileFields.Location = new System.Drawing.Point(6, 84);
            this.lstFileFields.Name = "lstFileFields";
            this.lstFileFields.Size = new System.Drawing.Size(146, 147);
            this.lstFileFields.TabIndex = 7;
            this.lstFileFields.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstFileFields_MouseDown);
            // 
            // lblAvailableFields
            // 
            this.lblAvailableFields.AutoSize = true;
            this.lblAvailableFields.Location = new System.Drawing.Point(3, 68);
            this.lblAvailableFields.Name = "lblAvailableFields";
            this.lblAvailableFields.Size = new System.Drawing.Size(125, 13);
            this.lblAvailableFields.TabIndex = 6;
            this.lblAvailableFields.Text = "Available fields in the file:";
            // 
            // lblMapTo
            // 
            this.lblMapTo.AutoSize = true;
            this.lblMapTo.Location = new System.Drawing.Point(155, 68);
            this.lblMapTo.Name = "lblMapTo";
            this.lblMapTo.Size = new System.Drawing.Size(70, 13);
            this.lblMapTo.TabIndex = 8;
            this.lblMapTo.Text = "Map fields to:";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(423, 290);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 16;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(9, 278);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(489, 35);
            this.lblStatus.TabIndex = 18;
            this.lblStatus.Text = "Ready";
            // 
            // statusTimer
            // 
            this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(342, 290);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 17;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // tabControlImportData
            // 
            this.tabControlImportData.Controls.Add(this.tpProjectProperties);
            this.tabControlImportData.Controls.Add(this.tpFileFieldMapping);
            this.tabControlImportData.Controls.Add(this.tpErrorLog);
            this.tabControlImportData.Location = new System.Drawing.Point(12, 12);
            this.tabControlImportData.Name = "tabControlImportData";
            this.tabControlImportData.SelectedIndex = 0;
            this.tabControlImportData.Size = new System.Drawing.Size(486, 263);
            this.tabControlImportData.TabIndex = 19;
            // 
            // tpProjectProperties
            // 
            this.tpProjectProperties.Controls.Add(this.chkBoxNewSkus);
            this.tpProjectProperties.Controls.Add(this.chkAttributeNameUomAreOne);
            this.tpProjectProperties.Controls.Add(this.chkMarkAsBefore);
            this.tpProjectProperties.Controls.Add(this.txtPauseDuration);
            this.tpProjectProperties.Controls.Add(this.lblSeconds);
            this.tpProjectProperties.Controls.Add(this.lblPause);
            this.tpProjectProperties.Controls.Add(this.grpImportType);
            this.tpProjectProperties.Controls.Add(this.lblSetName);
            this.tpProjectProperties.Controls.Add(this.txtSetLoadName);
            this.tpProjectProperties.Controls.Add(this.lblProjectName);
            this.tpProjectProperties.Controls.Add(this.lblClientName);
            this.tpProjectProperties.Controls.Add(this.txtProjectName);
            this.tpProjectProperties.Controls.Add(this.txtClientName);
            this.tpProjectProperties.Location = new System.Drawing.Point(4, 22);
            this.tpProjectProperties.Name = "tpProjectProperties";
            this.tpProjectProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProjectProperties.Size = new System.Drawing.Size(478, 237);
            this.tpProjectProperties.TabIndex = 3;
            this.tpProjectProperties.Text = "Project Properties";
            this.tpProjectProperties.UseVisualStyleBackColor = true;
            // 
            // chkAttributeNameUomAreOne
            // 
            this.chkAttributeNameUomAreOne.AutoSize = true;
            this.chkAttributeNameUomAreOne.Location = new System.Drawing.Point(250, 61);
            this.chkAttributeNameUomAreOne.Name = "chkAttributeNameUomAreOne";
            this.chkAttributeNameUomAreOne.Size = new System.Drawing.Size(124, 17);
            this.chkAttributeNameUomAreOne.TabIndex = 28;
            this.chkAttributeNameUomAreOne.Text = "Use Attribute + UOM";
            this.chkAttributeNameUomAreOne.UseVisualStyleBackColor = true;
            // 
            // chkMarkAsBefore
            // 
            this.chkMarkAsBefore.AutoSize = true;
            this.chkMarkAsBefore.Location = new System.Drawing.Point(250, 44);
            this.chkMarkAsBefore.Name = "chkMarkAsBefore";
            this.chkMarkAsBefore.Size = new System.Drawing.Size(181, 17);
            this.chkMarkAsBefore.TabIndex = 27;
            this.chkMarkAsBefore.Text = "Mark Entities as \'Before\' for Audit";
            this.chkMarkAsBefore.UseVisualStyleBackColor = true;
            // 
            // txtPauseDuration
            // 
            this.txtPauseDuration.Location = new System.Drawing.Point(364, 7);
            this.txtPauseDuration.Name = "txtPauseDuration";
            this.txtPauseDuration.Size = new System.Drawing.Size(45, 20);
            this.txtPauseDuration.TabIndex = 26;
            this.txtPauseDuration.ValueChanged += new System.EventHandler(this.txtPauseDuration_ValueChanged);
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(409, 9);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(47, 13);
            this.lblSeconds.TabIndex = 25;
            this.lblSeconds.Text = "seconds";
            // 
            // lblPause
            // 
            this.lblPause.AutoSize = true;
            this.lblPause.Location = new System.Drawing.Point(247, 9);
            this.lblPause.Name = "lblPause";
            this.lblPause.Size = new System.Drawing.Size(114, 13);
            this.lblPause.TabIndex = 23;
            this.lblPause.Text = "Pause between SKUs:";
            // 
            // grpImportType
            // 
            this.grpImportType.Controls.Add(this.grpUpdateAction);
            this.grpImportType.Controls.Add(this.rbInsertUpdate);
            this.grpImportType.Controls.Add(this.rbInsertOnly);
            this.grpImportType.Location = new System.Drawing.Point(9, 84);
            this.grpImportType.Name = "grpImportType";
            this.grpImportType.Size = new System.Drawing.Size(463, 139);
            this.grpImportType.TabIndex = 22;
            this.grpImportType.TabStop = false;
            this.grpImportType.Text = "Import Type";
            // 
            // grpUpdateAction
            // 
            this.grpUpdateAction.Controls.Add(this.rbAllNewEntities);
            this.grpUpdateAction.Controls.Add(this.rbUpsertEntities);
            this.grpUpdateAction.Controls.Add(this.rbDeleteExistingEntities);
            this.grpUpdateAction.Location = new System.Drawing.Point(6, 42);
            this.grpUpdateAction.Name = "grpUpdateAction";
            this.grpUpdateAction.Size = new System.Drawing.Size(457, 89);
            this.grpUpdateAction.TabIndex = 2;
            this.grpUpdateAction.TabStop = false;
            this.grpUpdateAction.Text = "Update Action";
            // 
            // rbAllNewEntities
            // 
            this.rbAllNewEntities.AutoSize = true;
            this.rbAllNewEntities.Location = new System.Drawing.Point(6, 19);
            this.rbAllNewEntities.Name = "rbAllNewEntities";
            this.rbAllNewEntities.Size = new System.Drawing.Size(283, 17);
            this.rbAllNewEntities.TabIndex = 2;
            this.rbAllNewEntities.Text = "Insert all new values (may create duplicates, but faster)";
            this.rbAllNewEntities.UseVisualStyleBackColor = true;
            // 
            // rbUpsertEntities
            // 
            this.rbUpsertEntities.AutoSize = true;
            this.rbUpsertEntities.Checked = true;
            this.rbUpsertEntities.Location = new System.Drawing.Point(6, 42);
            this.rbUpsertEntities.Name = "rbUpsertEntities";
            this.rbUpsertEntities.Size = new System.Drawing.Size(221, 17);
            this.rbUpsertEntities.TabIndex = 1;
            this.rbUpsertEntities.TabStop = true;
            this.rbUpsertEntities.Text = "Update existing values, Insert new values";
            this.rbUpsertEntities.UseVisualStyleBackColor = true;
            // 
            // rbDeleteExistingEntities
            // 
            this.rbDeleteExistingEntities.AutoSize = true;
            this.rbDeleteExistingEntities.Location = new System.Drawing.Point(6, 65);
            this.rbDeleteExistingEntities.Name = "rbDeleteExistingEntities";
            this.rbDeleteExistingEntities.Size = new System.Drawing.Size(257, 17);
            this.rbDeleteExistingEntities.TabIndex = 0;
            this.rbDeleteExistingEntities.Text = "Delete all existing values; then, Insert new values";
            this.rbDeleteExistingEntities.UseVisualStyleBackColor = true;
            this.rbDeleteExistingEntities.CheckedChanged += new System.EventHandler(this.rbDeleteExistingEntities_CheckedChanged);
            // 
            // rbInsertUpdate
            // 
            this.rbInsertUpdate.AutoSize = true;
            this.rbInsertUpdate.Checked = true;
            this.rbInsertUpdate.Location = new System.Drawing.Point(103, 19);
            this.rbInsertUpdate.Name = "rbInsertUpdate";
            this.rbInsertUpdate.Size = new System.Drawing.Size(260, 17);
            this.rbInsertUpdate.TabIndex = 1;
            this.rbInsertUpdate.TabStop = true;
            this.rbInsertUpdate.Text = "Some SKUs may already exist (default, but slower)";
            this.rbInsertUpdate.UseVisualStyleBackColor = true;
            this.rbInsertUpdate.CheckedChanged += new System.EventHandler(this.rbInsertUpdate_CheckedChanged);
            // 
            // rbInsertOnly
            // 
            this.rbInsertOnly.AutoSize = true;
            this.rbInsertOnly.Location = new System.Drawing.Point(6, 19);
            this.rbInsertOnly.Name = "rbInsertOnly";
            this.rbInsertOnly.Size = new System.Drawing.Size(91, 17);
            this.rbInsertOnly.TabIndex = 0;
            this.rbInsertOnly.Text = "All New SKUs";
            this.rbInsertOnly.UseVisualStyleBackColor = true;
            this.rbInsertOnly.CheckedChanged += new System.EventHandler(this.rbInsertOnly_CheckedChanged);
            // 
            // lblSetName
            // 
            this.lblSetName.AutoSize = true;
            this.lblSetName.Location = new System.Drawing.Point(6, 61);
            this.lblSetName.Name = "lblSetName";
            this.lblSetName.Size = new System.Drawing.Size(57, 13);
            this.lblSetName.TabIndex = 20;
            this.lblSetName.Text = "Set Name:";
            // 
            // txtSetLoadName
            // 
            this.txtSetLoadName.Location = new System.Drawing.Point(91, 58);
            this.txtSetLoadName.Name = "txtSetLoadName";
            this.txtSetLoadName.Size = new System.Drawing.Size(150, 20);
            this.txtSetLoadName.TabIndex = 21;
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(6, 35);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(71, 13);
            this.lblProjectName.TabIndex = 18;
            this.lblProjectName.Text = "Project Code:";
            // 
            // lblClientName
            // 
            this.lblClientName.AutoSize = true;
            this.lblClientName.Location = new System.Drawing.Point(6, 9);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(67, 13);
            this.lblClientName.TabIndex = 16;
            this.lblClientName.Text = "Client Name:";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(91, 32);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(150, 20);
            this.txtProjectName.TabIndex = 19;
            this.txtProjectName.TextChanged += new System.EventHandler(this.txtProjectName_TextChanged);
            // 
            // txtClientName
            // 
            this.txtClientName.Location = new System.Drawing.Point(91, 6);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(150, 20);
            this.txtClientName.TabIndex = 17;
            this.txtClientName.TextChanged += new System.EventHandler(this.txtClientName_TextChanged);
            // 
            // tpFileFieldMapping
            // 
            this.tpFileFieldMapping.Controls.Add(this.lblInputFileName);
            this.tpFileFieldMapping.Controls.Add(this.txtInputFilename);
            this.tpFileFieldMapping.Controls.Add(this.cboDelimiter);
            this.tpFileFieldMapping.Controls.Add(this.lblDelimiter);
            this.tpFileFieldMapping.Controls.Add(this.btnOpenInputFile);
            this.tpFileFieldMapping.Controls.Add(this.btnLoad);
            this.tpFileFieldMapping.Controls.Add(this.lblAvailableFields);
            this.tpFileFieldMapping.Controls.Add(this.lstFileFields);
            this.tpFileFieldMapping.Controls.Add(this.lblMapTo);
            this.tpFileFieldMapping.Controls.Add(this.lstMapping);
            this.tpFileFieldMapping.Location = new System.Drawing.Point(4, 22);
            this.tpFileFieldMapping.Name = "tpFileFieldMapping";
            this.tpFileFieldMapping.Padding = new System.Windows.Forms.Padding(3);
            this.tpFileFieldMapping.Size = new System.Drawing.Size(478, 237);
            this.tpFileFieldMapping.TabIndex = 0;
            this.tpFileFieldMapping.Text = "File and Field Mappings";
            this.tpFileFieldMapping.UseVisualStyleBackColor = true;
            // 
            // lblInputFileName
            // 
            this.lblInputFileName.AutoSize = true;
            this.lblInputFileName.Location = new System.Drawing.Point(3, 9);
            this.lblInputFileName.Name = "lblInputFileName";
            this.lblInputFileName.Size = new System.Drawing.Size(79, 13);
            this.lblInputFileName.TabIndex = 11;
            this.lblInputFileName.Text = "Input Filename:";
            // 
            // txtInputFilename
            // 
            this.txtInputFilename.Location = new System.Drawing.Point(88, 6);
            this.txtInputFilename.Name = "txtInputFilename";
            this.txtInputFilename.Size = new System.Drawing.Size(384, 20);
            this.txtInputFilename.TabIndex = 12;
            this.txtInputFilename.TextChanged += new System.EventHandler(this.txtInputFilename_TextChanged);
            this.txtInputFilename.Enter += new System.EventHandler(this.txtInputFilename_Enter);
            // 
            // cboDelimiter
            // 
            this.cboDelimiter.FormattingEnabled = true;
            this.cboDelimiter.Items.AddRange(new object[] {
            "Tab",
            "Comma",
            "Pipe (|)"});
            this.cboDelimiter.Location = new System.Drawing.Point(88, 32);
            this.cboDelimiter.Name = "cboDelimiter";
            this.cboDelimiter.Size = new System.Drawing.Size(137, 21);
            this.cboDelimiter.TabIndex = 15;
            this.cboDelimiter.SelectedIndexChanged += new System.EventHandler(this.cboDelimiter_SelectedIndexChanged);
            // 
            // lblDelimiter
            // 
            this.lblDelimiter.AutoSize = true;
            this.lblDelimiter.Location = new System.Drawing.Point(3, 35);
            this.lblDelimiter.Name = "lblDelimiter";
            this.lblDelimiter.Size = new System.Drawing.Size(50, 13);
            this.lblDelimiter.TabIndex = 14;
            this.lblDelimiter.Text = "Delimiter:";
            // 
            // btnOpenInputFile
            // 
            this.btnOpenInputFile.Location = new System.Drawing.Point(316, 32);
            this.btnOpenInputFile.Name = "btnOpenInputFile";
            this.btnOpenInputFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenInputFile.TabIndex = 10;
            this.btnOpenInputFile.Text = "Browse";
            this.btnOpenInputFile.UseVisualStyleBackColor = true;
            this.btnOpenInputFile.Click += new System.EventHandler(this.btnOpenInputFile_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(397, 32);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 13;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tpErrorLog
            // 
            this.tpErrorLog.Controls.Add(this.txtErrorLog);
            this.tpErrorLog.Location = new System.Drawing.Point(4, 22);
            this.tpErrorLog.Name = "tpErrorLog";
            this.tpErrorLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpErrorLog.Size = new System.Drawing.Size(478, 237);
            this.tpErrorLog.TabIndex = 4;
            this.tpErrorLog.Text = "Error Log";
            this.tpErrorLog.UseVisualStyleBackColor = true;
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorLog.Location = new System.Drawing.Point(3, 3);
            this.txtErrorLog.Multiline = true;
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorLog.Size = new System.Drawing.Size(472, 231);
            this.txtErrorLog.TabIndex = 0;
            this.txtErrorLog.WordWrap = false;
            // 
            // chkBoxNewSkus
            // 
            this.chkBoxNewSkus.AutoSize = true;
            this.chkBoxNewSkus.Location = new System.Drawing.Point(250, 26);
            this.chkBoxNewSkus.Name = "chkBoxNewSkus";
            this.chkBoxNewSkus.Size = new System.Drawing.Size(75, 17);
            this.chkBoxNewSkus.TabIndex = 29;
            this.chkBoxNewSkus.Text = "New Skus";
            this.chkBoxNewSkus.UseVisualStyleBackColor = true;
            // 
            // FrmImportData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 325);
            this.Controls.Add(this.tabControlImportData);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmImportData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BulkImport_FormClosing);
            this.Load += new System.EventHandler(this.BulkImport_Load);
            this.tabControlImportData.ResumeLayout(false);
            this.tpProjectProperties.ResumeLayout(false);
            this.tpProjectProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPauseDuration)).EndInit();
            this.grpImportType.ResumeLayout(false);
            this.grpImportType.PerformLayout();
            this.grpUpdateAction.ResumeLayout(false);
            this.grpUpdateAction.PerformLayout();
            this.tpFileFieldMapping.ResumeLayout(false);
            this.tpFileFieldMapping.PerformLayout();
            this.tpErrorLog.ResumeLayout(false);
            this.tpErrorLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListView lstMapping;
        private System.Windows.Forms.ListBox lstFileFields;
        private System.Windows.Forms.Label lblAvailableFields;
        private System.Windows.Forms.Label lblMapTo;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.ColumnHeader FieldName;
        private System.Windows.Forms.ColumnHeader Required;
        private System.Windows.Forms.ColumnHeader MapTo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer statusTimer;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.TabControl tabControlImportData;
        private System.Windows.Forms.TabPage tpFileFieldMapping;
        private System.Windows.Forms.Label lblInputFileName;
        private System.Windows.Forms.TextBox txtInputFilename;
        private System.Windows.Forms.ComboBox cboDelimiter;
        private System.Windows.Forms.Label lblDelimiter;
        private System.Windows.Forms.Button btnOpenInputFile;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TabPage tpProjectProperties;
        private System.Windows.Forms.Label lblSetName;
        private System.Windows.Forms.TextBox txtSetLoadName;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.Label lblClientName;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.TabPage tpErrorLog;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.GroupBox grpImportType;
        private System.Windows.Forms.RadioButton rbInsertUpdate;
        private System.Windows.Forms.RadioButton rbInsertOnly;
        private System.Windows.Forms.GroupBox grpUpdateAction;
        private System.Windows.Forms.RadioButton rbUpsertEntities;
        private System.Windows.Forms.RadioButton rbDeleteExistingEntities;
        private System.Windows.Forms.NumericUpDown txtPauseDuration;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label lblPause;
        private System.Windows.Forms.CheckBox chkMarkAsBefore;
        private System.Windows.Forms.RadioButton rbAllNewEntities;
        private System.Windows.Forms.CheckBox chkAttributeNameUomAreOne;
        private System.Windows.Forms.CheckBox chkBoxNewSkus;
    }
}