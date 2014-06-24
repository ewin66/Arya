namespace Arya
{
    partial class FrmAttributeView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.tblAttributeViewMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvListOfValues = new System.Windows.Forms.DataGridView();
            this.colCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAttributes = new System.Windows.Forms.DataGridView();
            this.colLocalVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnProperty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAttributeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAttributeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMultiValued = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAttributeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNavigationalOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisplayOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsRanked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tblFilters = new System.Windows.Forms.TableLayoutPanel();
            this.ddAttributeTypeFilter = new System.Windows.Forms.ComboBox();
            this.ddAttributeGroupFilter = new System.Windows.Forms.ComboBox();
            this.txtAttributeNameFilter = new System.Windows.Forms.TextBox();
            this.flpLOvOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.chkShowLovs = new System.Windows.Forms.CheckBox();
            this.pnlLovOptions = new System.Windows.Forms.Panel();
            this.rbInschemaLovs = new System.Windows.Forms.RadioButton();
            this.rbAllLovs = new System.Windows.Forms.RadioButton();
            this.flpControls = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExportData = new System.Windows.Forms.Button();
            this.btnAttributePreferences = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSavePrefs = new System.Windows.Forms.Button();
            this.btnCheckSpelling = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.workerThreadTimer = new System.Windows.Forms.Timer(this.components);
            this.sfdAttributeData = new System.Windows.Forms.SaveFileDialog();
            this.toolTipMenuButton = new System.Windows.Forms.ToolTip(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tblAttributeViewMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).BeginInit();
            this.tblFilters.SuspendLayout();
            this.flpLOvOptions.SuspendLayout();
            this.pnlLovOptions.SuspendLayout();
            this.flpControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkAll
            // 
            this.chkAll.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(3, 5);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(37, 17);
            this.chkAll.TabIndex = 5;
            this.chkAll.Text = "All";
            this.chkAll.ThreeState = true;
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.Visible = false;
            this.chkAll.CheckStateChanged += new System.EventHandler(this.chkAll_CheckStateChanged);
            // 
            // tblAttributeViewMain
            // 
            this.tblAttributeViewMain.AllowDrop = true;
            this.tblAttributeViewMain.ColumnCount = 2;
            this.tblAttributeViewMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 375F));
            this.tblAttributeViewMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblAttributeViewMain.Controls.Add(this.dgvListOfValues, 1, 2);
            this.tblAttributeViewMain.Controls.Add(this.dgvAttributes, 0, 2);
            this.tblAttributeViewMain.Controls.Add(this.tblFilters, 0, 1);
            this.tblAttributeViewMain.Controls.Add(this.flpLOvOptions, 1, 1);
            this.tblAttributeViewMain.Controls.Add(this.flpControls, 0, 0);
            this.tblAttributeViewMain.Controls.Add(this.btnApply, 0, 3);
            this.tblAttributeViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAttributeViewMain.Location = new System.Drawing.Point(0, 0);
            this.tblAttributeViewMain.Name = "tblAttributeViewMain";
            this.tblAttributeViewMain.RowCount = 3;
            this.tblAttributeViewMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAttributeViewMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAttributeViewMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblAttributeViewMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblAttributeViewMain.Size = new System.Drawing.Size(887, 536);
            this.tblAttributeViewMain.TabIndex = 0;
            // 
            // dgvListOfValues
            // 
            this.dgvListOfValues.AllowUserToAddRows = false;
            this.dgvListOfValues.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvListOfValues.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvListOfValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListOfValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCount,
            this.colValue,
            this.colUom});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvListOfValues.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvListOfValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvListOfValues.Location = new System.Drawing.Point(378, 65);
            this.dgvListOfValues.Name = "dgvListOfValues";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvListOfValues.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvListOfValues.RowHeadersVisible = false;
            this.dgvListOfValues.RowTemplate.Height = 24;
            this.dgvListOfValues.Size = new System.Drawing.Size(506, 438);
            this.dgvListOfValues.TabIndex = 16;
            this.dgvListOfValues.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvListOfValues_ColumnHeaderMouseClick);
            // 
            // colCount
            // 
            this.colCount.DataPropertyName = "Count";
            this.colCount.HeaderText = "Count";
            this.colCount.Name = "colCount";
            this.colCount.ReadOnly = true;
            this.colCount.Width = 50;
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.DataPropertyName = "Value";
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            // 
            // colUom
            // 
            this.colUom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colUom.DataPropertyName = "Uom";
            this.colUom.HeaderText = "UoM";
            this.colUom.Name = "colUom";
            this.colUom.Width = 55;
            // 
            // dgvAttributes
            // 
            this.dgvAttributes.AllowUserToAddRows = false;
            this.dgvAttributes.AllowUserToDeleteRows = false;
            this.dgvAttributes.AllowUserToOrderColumns = true;
            this.dgvAttributes.AllowUserToResizeColumns = false;
            this.dgvAttributes.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAttributes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLocalVisible,
            this.columnProperty,
            this.colAttributeType,
            this.colAttributeGroup,
            this.colMultiValued,
            this.colAttributeName,
            this.colNavigationalOrder,
            this.colDisplayOrder,
            this.colIsRanked});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAttributes.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttributes.Location = new System.Drawing.Point(3, 65);
            this.dgvAttributes.MultiSelect = false;
            this.dgvAttributes.Name = "dgvAttributes";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAttributes.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvAttributes.RowHeadersVisible = false;
            this.dgvAttributes.RowTemplate.Height = 24;
            this.dgvAttributes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvAttributes.Size = new System.Drawing.Size(369, 438);
            this.dgvAttributes.TabIndex = 4;
            this.dgvAttributes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvAttributes_CellFormatting);
            this.dgvAttributes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAttributes_CellValueChanged);
            this.dgvAttributes.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAttributes_ColumnHeaderMouseClick);
            this.dgvAttributes.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvAttributes_DataError);
            this.dgvAttributes.SelectionChanged += new System.EventHandler(this.dgvAttributes_SelectionChanged);
            // 
            // colLocalVisible
            // 
            this.colLocalVisible.DataPropertyName = "Visible";
            this.colLocalVisible.HeaderText = "Show";
            this.colLocalVisible.Name = "colLocalVisible";
            this.colLocalVisible.Visible = false;
            this.colLocalVisible.Width = 40;
            // 
            // columnProperty
            // 
            this.columnProperty.DataPropertyName = "Column";
            this.columnProperty.HeaderText = "Column Property";
            this.columnProperty.Name = "columnProperty";
            this.columnProperty.Visible = false;
            // 
            // colAttributeType
            // 
            this.colAttributeType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAttributeType.DataPropertyName = "AttributeType";
            this.colAttributeType.HeaderText = "Type";
            this.colAttributeType.Name = "colAttributeType";
            this.colAttributeType.Width = 56;
            // 
            // colAttributeGroup
            // 
            this.colAttributeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAttributeGroup.DataPropertyName = "AttributeGroup";
            this.colAttributeGroup.HeaderText = "Group";
            this.colAttributeGroup.Name = "colAttributeGroup";
            this.colAttributeGroup.Width = 61;
            // 
            // colMultiValued
            // 
            this.colMultiValued.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colMultiValued.DataPropertyName = "IsMultiValued";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colMultiValued.DefaultCellStyle = dataGridViewCellStyle5;
            this.colMultiValued.HeaderText = "MV";
            this.colMultiValued.Name = "colMultiValued";
            this.colMultiValued.ReadOnly = true;
            this.colMultiValued.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colMultiValued.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMultiValued.ToolTipText = "Is MultiValued?";
            this.colMultiValued.Width = 30;
            // 
            // colAttributeName
            // 
            this.colAttributeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAttributeName.DataPropertyName = "AttributeName";
            this.colAttributeName.HeaderText = "Attribute Name";
            this.colAttributeName.Name = "colAttributeName";
            // 
            // colNavigationalOrder
            // 
            this.colNavigationalOrder.DataPropertyName = "NavigationOrder";
            this.colNavigationalOrder.HeaderText = "Nav. Order";
            this.colNavigationalOrder.Name = "colNavigationalOrder";
            this.colNavigationalOrder.ReadOnly = true;
            this.colNavigationalOrder.Width = 50;
            // 
            // colDisplayOrder
            // 
            this.colDisplayOrder.DataPropertyName = "DisplayOrder";
            this.colDisplayOrder.HeaderText = "Disp. Order";
            this.colDisplayOrder.Name = "colDisplayOrder";
            this.colDisplayOrder.ReadOnly = true;
            this.colDisplayOrder.Width = 50;
            // 
            // colIsRanked
            // 
            this.colIsRanked.DataPropertyName = "IsRanked";
            this.colIsRanked.FalseValue = "";
            this.colIsRanked.HeaderText = "Ranked";
            this.colIsRanked.IndeterminateValue = "";
            this.colIsRanked.Name = "colIsRanked";
            this.colIsRanked.ReadOnly = true;
            this.colIsRanked.ThreeState = true;
            this.colIsRanked.TrueValue = "";
            this.colIsRanked.Width = 40;
            // 
            // tblFilters
            // 
            this.tblFilters.AutoSize = true;
            this.tblFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblFilters.ColumnCount = 4;
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFilters.Controls.Add(this.chkAll, 0, 0);
            this.tblFilters.Controls.Add(this.ddAttributeTypeFilter, 1, 0);
            this.tblFilters.Controls.Add(this.ddAttributeGroupFilter, 2, 0);
            this.tblFilters.Controls.Add(this.txtAttributeNameFilter, 3, 0);
            this.tblFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFilters.Location = new System.Drawing.Point(3, 32);
            this.tblFilters.Name = "tblFilters";
            this.tblFilters.RowCount = 1;
            this.tblFilters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFilters.Size = new System.Drawing.Size(369, 27);
            this.tblFilters.TabIndex = 13;
            // 
            // ddAttributeTypeFilter
            // 
            this.ddAttributeTypeFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ddAttributeTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddAttributeTypeFilter.FormattingEnabled = true;
            this.ddAttributeTypeFilter.Location = new System.Drawing.Point(46, 3);
            this.ddAttributeTypeFilter.Name = "ddAttributeTypeFilter";
            this.ddAttributeTypeFilter.Size = new System.Drawing.Size(114, 21);
            this.ddAttributeTypeFilter.TabIndex = 7;
            this.ddAttributeTypeFilter.SelectedIndexChanged += new System.EventHandler(this.FilterUpdated);
            // 
            // ddAttributeGroupFilter
            // 
            this.ddAttributeGroupFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ddAttributeGroupFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddAttributeGroupFilter.FormattingEnabled = true;
            this.ddAttributeGroupFilter.Location = new System.Drawing.Point(166, 3);
            this.ddAttributeGroupFilter.Name = "ddAttributeGroupFilter";
            this.ddAttributeGroupFilter.Size = new System.Drawing.Size(114, 21);
            this.ddAttributeGroupFilter.TabIndex = 12;
            this.ddAttributeGroupFilter.SelectedIndexChanged += new System.EventHandler(this.FilterUpdated);
            // 
            // txtAttributeNameFilter
            // 
            this.txtAttributeNameFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAttributeNameFilter.Location = new System.Drawing.Point(286, 3);
            this.txtAttributeNameFilter.Name = "txtAttributeNameFilter";
            this.txtAttributeNameFilter.Size = new System.Drawing.Size(80, 20);
            this.txtAttributeNameFilter.TabIndex = 13;
            this.txtAttributeNameFilter.TextChanged += new System.EventHandler(this.txtAttributeNameFilter_TextChanged);
            // 
            // flpLOvOptions
            // 
            this.flpLOvOptions.Controls.Add(this.chkShowLovs);
            this.flpLOvOptions.Controls.Add(this.pnlLovOptions);
            this.flpLOvOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpLOvOptions.Location = new System.Drawing.Point(378, 32);
            this.flpLOvOptions.Name = "flpLOvOptions";
            this.flpLOvOptions.Size = new System.Drawing.Size(506, 27);
            this.flpLOvOptions.TabIndex = 17;
            // 
            // chkShowLovs
            // 
            this.chkShowLovs.AutoSize = true;
            this.chkShowLovs.Location = new System.Drawing.Point(3, 8);
            this.chkShowLovs.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.chkShowLovs.Name = "chkShowLovs";
            this.chkShowLovs.Size = new System.Drawing.Size(78, 17);
            this.chkShowLovs.TabIndex = 0;
            this.chkShowLovs.Text = "Lov values";
            this.chkShowLovs.UseVisualStyleBackColor = true;
            this.chkShowLovs.CheckedChanged += new System.EventHandler(this.chkShowLovs_CheckedChanged);
            // 
            // pnlLovOptions
            // 
            this.pnlLovOptions.Controls.Add(this.rbInschemaLovs);
            this.pnlLovOptions.Controls.Add(this.rbAllLovs);
            this.pnlLovOptions.Enabled = false;
            this.pnlLovOptions.Location = new System.Drawing.Point(87, 3);
            this.pnlLovOptions.Name = "pnlLovOptions";
            this.pnlLovOptions.Size = new System.Drawing.Size(140, 24);
            this.pnlLovOptions.TabIndex = 3;
            // 
            // rbInschemaLovs
            // 
            this.rbInschemaLovs.AutoSize = true;
            this.rbInschemaLovs.Location = new System.Drawing.Point(45, 4);
            this.rbInschemaLovs.Name = "rbInschemaLovs";
            this.rbInschemaLovs.Size = new System.Drawing.Size(97, 17);
            this.rbInschemaLovs.TabIndex = 2;
            this.rbInschemaLovs.Text = "InSchema Only";
            this.rbInschemaLovs.UseVisualStyleBackColor = true;
            this.rbInschemaLovs.CheckedChanged += new System.EventHandler(this.rbInschemaLovs_CheckedChanged);
            // 
            // rbAllLovs
            // 
            this.rbAllLovs.AutoSize = true;
            this.rbAllLovs.Checked = true;
            this.rbAllLovs.Location = new System.Drawing.Point(3, 4);
            this.rbAllLovs.Name = "rbAllLovs";
            this.rbAllLovs.Size = new System.Drawing.Size(36, 17);
            this.rbAllLovs.TabIndex = 1;
            this.rbAllLovs.TabStop = true;
            this.rbAllLovs.Text = "All";
            this.rbAllLovs.UseVisualStyleBackColor = true;
            this.rbAllLovs.CheckedChanged += new System.EventHandler(this.rbAllLovs_CheckedChanged);
            // 
            // flpControls
            // 
            this.flpControls.AutoSize = true;
            this.flpControls.Controls.Add(this.btnExportData);
            this.flpControls.Controls.Add(this.btnAttributePreferences);
            this.flpControls.Controls.Add(this.lblStatus);
            this.flpControls.Controls.Add(this.btnSavePrefs);
            this.flpControls.Controls.Add(this.btnCheckSpelling);
            this.flpControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpControls.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flpControls.Location = new System.Drawing.Point(0, 0);
            this.flpControls.Margin = new System.Windows.Forms.Padding(0);
            this.flpControls.Name = "flpControls";
            this.flpControls.Size = new System.Drawing.Size(375, 29);
            this.flpControls.TabIndex = 18;
            // 
            // btnExportData
            // 
            this.btnExportData.FlatAppearance.BorderSize = 0;
            this.btnExportData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportData.Image = global::Arya.Properties.Resources.ExportTab;
            this.btnExportData.Location = new System.Drawing.Point(3, 3);
            this.btnExportData.Name = "btnExportData";
            this.btnExportData.Size = new System.Drawing.Size(25, 17);
            this.btnExportData.TabIndex = 9;
            this.toolTipMenuButton.SetToolTip(this.btnExportData, "Export Attributes");
            this.btnExportData.UseVisualStyleBackColor = true;
            this.btnExportData.Click += new System.EventHandler(this.btnExportData_Click);
            // 
            // btnAttributePreferences
            // 
            this.btnAttributePreferences.FlatAppearance.BorderSize = 0;
            this.btnAttributePreferences.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAttributePreferences.Image = global::Arya.Properties.Resources.CalculatedAttribute;
            this.btnAttributePreferences.Location = new System.Drawing.Point(34, 3);
            this.btnAttributePreferences.Name = "btnAttributePreferences";
            this.btnAttributePreferences.Size = new System.Drawing.Size(25, 17);
            this.btnAttributePreferences.TabIndex = 10;
            this.toolTipMenuButton.SetToolTip(this.btnAttributePreferences, "Show User Preferences");
            this.btnAttributePreferences.UseVisualStyleBackColor = true;
            this.btnAttributePreferences.Click += new System.EventHandler(this.btnAttributePreferences_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(65, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Status";
            this.lblStatus.UseMnemonic = false;
            // 
            // btnSavePrefs
            // 
            this.btnSavePrefs.FlatAppearance.BorderSize = 0;
            this.btnSavePrefs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePrefs.Image = global::Arya.Properties.Resources.Save;
            this.btnSavePrefs.Location = new System.Drawing.Point(108, 3);
            this.btnSavePrefs.Name = "btnSavePrefs";
            this.btnSavePrefs.Size = new System.Drawing.Size(25, 17);
            this.btnSavePrefs.TabIndex = 12;
            this.btnSavePrefs.UseVisualStyleBackColor = true;
            this.btnSavePrefs.Click += new System.EventHandler(this.btnSavePrefs_Click);
            // 
            // btnCheckSpelling
            // 
            this.btnCheckSpelling.Location = new System.Drawing.Point(139, 3);
            this.btnCheckSpelling.Name = "btnCheckSpelling";
            this.btnCheckSpelling.Size = new System.Drawing.Size(113, 23);
            this.btnCheckSpelling.TabIndex = 13;
            this.btnCheckSpelling.Text = "Check Spelling";
            this.btnCheckSpelling.UseVisualStyleBackColor = true;
            this.btnCheckSpelling.Click += new System.EventHandler(this.btnCheckSpelling_Click);
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnApply.ForeColor = System.Drawing.Color.Black;
            this.btnApply.Location = new System.Drawing.Point(297, 509);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 24);
            this.btnApply.TabIndex = 19;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // workerThreadTimer
            // 
            this.workerThreadTimer.Enabled = true;
            this.workerThreadTimer.Interval = 500;
            this.workerThreadTimer.Tick += new System.EventHandler(this.workerThreadTimer_Tick);
            // 
            // sfdAttributeData
            // 
            this.sfdAttributeData.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Count";
            this.dataGridViewTextBoxColumn1.HeaderText = "Count";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Value";
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Uom";
            this.dataGridViewTextBoxColumn3.HeaderText = "UoM";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Column";
            this.dataGridViewTextBoxColumn4.HeaderText = "Column Property";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn5.DataPropertyName = "AttributeType";
            this.dataGridViewTextBoxColumn5.HeaderText = "Type";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn6.DataPropertyName = "AttributeGroup";
            this.dataGridViewTextBoxColumn6.HeaderText = "Group";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn7.DataPropertyName = "IsMultiValued";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn7.HeaderText = "MV";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.ToolTipText = "Is MultiValued?";
            this.dataGridViewTextBoxColumn7.Width = 30;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn8.DataPropertyName = "AttributeName";
            this.dataGridViewTextBoxColumn8.HeaderText = "Attribute Name";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "NavigationOrder";
            this.dataGridViewTextBoxColumn9.HeaderText = "Nav. Order";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            this.dataGridViewTextBoxColumn9.Width = 50;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "DisplayOrder";
            this.dataGridViewTextBoxColumn10.HeaderText = "Disp. Order";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Width = 50;
            // 
            // FrmAttributeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 536);
            this.Controls.Add(this.tblAttributeViewMain);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(364, 527);
            this.Name = "FrmAttributeView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Attribute View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAttributeView_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmAttributeView_KeyDown);
            this.tblAttributeViewMain.ResumeLayout(false);
            this.tblAttributeViewMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).EndInit();
            this.tblFilters.ResumeLayout(false);
            this.tblFilters.PerformLayout();
            this.flpLOvOptions.ResumeLayout(false);
            this.flpLOvOptions.PerformLayout();
            this.pnlLovOptions.ResumeLayout(false);
            this.pnlLovOptions.PerformLayout();
            this.flpControls.ResumeLayout(false);
            this.flpControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.TableLayoutPanel tblAttributeViewMain;
        private System.Windows.Forms.ComboBox ddAttributeTypeFilter;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer workerThreadTimer;
        private System.Windows.Forms.TableLayoutPanel tblFilters;
        private System.Windows.Forms.ComboBox ddAttributeGroupFilter;
        private System.Windows.Forms.TextBox txtAttributeNameFilter;
        private System.Windows.Forms.DataGridView dgvListOfValues;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUom;
        private System.Windows.Forms.FlowLayoutPanel flpLOvOptions;
        private System.Windows.Forms.CheckBox chkShowLovs;
        private System.Windows.Forms.RadioButton rbAllLovs;
        private System.Windows.Forms.RadioButton rbInschemaLovs;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.Panel pnlLovOptions;
        private System.Windows.Forms.FlowLayoutPanel flpControls;
        private System.Windows.Forms.Button btnExportData;
        private System.Windows.Forms.SaveFileDialog sfdAttributeData;
        private System.Windows.Forms.Button btnAttributePreferences;
        private System.Windows.Forms.Button btnSavePrefs;
        private System.Windows.Forms.DataGridView dgvAttributes;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLocalVisible;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnProperty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMultiValued;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNavigationalOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisplayOrder;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsRanked;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ToolTip toolTipMenuButton;
        private System.Windows.Forms.Button btnCheckSpelling;
    }
}