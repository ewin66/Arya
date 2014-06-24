using Arya.Framework.GUI.UserControls;

namespace Arya.UserControls
{
    partial class BuildDataGridView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMultiValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.validateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewSKUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.field1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.field2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.field3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.field4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.field5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorkerAutoFillSouceFetch = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bdgv = new CustomDataGridView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvListOfValues = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvMetaAttributes = new System.Windows.Forms.DataGridView();
            this.dgvGlobals = new System.Windows.Forms.DataGridView();
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdgv)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetaAttributes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGlobals)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataToolStripMenuItem,
            this.displayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveChangesToolStripMenuItem,
            this.addMultiValuesToolStripMenuItem,
            this.deleteValuesToolStripMenuItem,
            this.toolStripSeparator1,
            this.validateToolStripMenuItem,
            this.addNewSKUsToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.dataToolStripMenuItem.Text = "&Data";
            // 
            // saveChangesToolStripMenuItem
            // 
            this.saveChangesToolStripMenuItem.CheckOnClick = true;
            this.saveChangesToolStripMenuItem.Name = "saveChangesToolStripMenuItem";
            this.saveChangesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveChangesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.saveChangesToolStripMenuItem.Text = "&Save Changes";
            this.saveChangesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.saveChangesToolStripMenuItem_CheckedChanged);
            this.saveChangesToolStripMenuItem.Click += new System.EventHandler(this.saveChangesToolStripMenuItem_Click);
            // 
            // addMultiValuesToolStripMenuItem
            // 
            this.addMultiValuesToolStripMenuItem.Name = "addMultiValuesToolStripMenuItem";
            this.addMultiValuesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.addMultiValuesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.addMultiValuesToolStripMenuItem.Text = "Add &Multi-Values";
            this.addMultiValuesToolStripMenuItem.Click += new System.EventHandler(this.addMultiValuesToolStripMenuItem_Click);
            // 
            // deleteValuesToolStripMenuItem
            // 
            this.deleteValuesToolStripMenuItem.Name = "deleteValuesToolStripMenuItem";
            this.deleteValuesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteValuesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.deleteValuesToolStripMenuItem.Text = "&Delete Values";
            this.deleteValuesToolStripMenuItem.Click += new System.EventHandler(this.deleteValuesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(224, 6);
            // 
            // validateToolStripMenuItem
            // 
            this.validateToolStripMenuItem.CheckOnClick = true;
            this.validateToolStripMenuItem.Name = "validateToolStripMenuItem";
            this.validateToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.validateToolStripMenuItem.Text = "&Validate Values";
            this.validateToolStripMenuItem.CheckedChanged += new System.EventHandler(this.validateToolStripMenuItem_CheckedChanged);
            // 
            // addNewSKUsToolStripMenuItem
            // 
            this.addNewSKUsToolStripMenuItem.Name = "addNewSKUsToolStripMenuItem";
            this.addNewSKUsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.addNewSKUsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.addNewSKUsToolStripMenuItem.Text = "Add &New SKUs";
            this.addNewSKUsToolStripMenuItem.Click += new System.EventHandler(this.addNewSKUsToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataAttributesToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.displayToolStripMenuItem.Text = "&View";
            // 
            // dataAttributesToolStripMenuItem
            // 
            this.dataAttributesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.field1ToolStripMenuItem,
            this.field2ToolStripMenuItem,
            this.field3ToolStripMenuItem,
            this.field4ToolStripMenuItem,
            this.field5ToolStripMenuItem});
            this.dataAttributesToolStripMenuItem.Name = "dataAttributesToolStripMenuItem";
            this.dataAttributesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.dataAttributesToolStripMenuItem.Text = "&Data Attributes";
            // 
            // field1ToolStripMenuItem
            // 
            this.field1ToolStripMenuItem.CheckOnClick = true;
            this.field1ToolStripMenuItem.Name = "field1ToolStripMenuItem";
            this.field1ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.field1ToolStripMenuItem.Text = "Field&1";
            this.field1ToolStripMenuItem.Visible = false;
            this.field1ToolStripMenuItem.Click += new System.EventHandler(this.field1ToolStripMenuItem_Click);
            // 
            // field2ToolStripMenuItem
            // 
            this.field2ToolStripMenuItem.CheckOnClick = true;
            this.field2ToolStripMenuItem.Name = "field2ToolStripMenuItem";
            this.field2ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.field2ToolStripMenuItem.Text = "Field&2";
            this.field2ToolStripMenuItem.Visible = false;
            this.field2ToolStripMenuItem.Click += new System.EventHandler(this.field2ToolStripMenuItem_Click);
            // 
            // field3ToolStripMenuItem
            // 
            this.field3ToolStripMenuItem.CheckOnClick = true;
            this.field3ToolStripMenuItem.Name = "field3ToolStripMenuItem";
            this.field3ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.field3ToolStripMenuItem.Text = "Field&3";
            this.field3ToolStripMenuItem.Visible = false;
            this.field3ToolStripMenuItem.Click += new System.EventHandler(this.field3ToolStripMenuItem_Click);
            // 
            // field4ToolStripMenuItem
            // 
            this.field4ToolStripMenuItem.CheckOnClick = true;
            this.field4ToolStripMenuItem.Name = "field4ToolStripMenuItem";
            this.field4ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.field4ToolStripMenuItem.Text = "Field&4";
            this.field4ToolStripMenuItem.Visible = false;
            this.field4ToolStripMenuItem.Click += new System.EventHandler(this.field4ToolStripMenuItem_Click);
            // 
            // field5ToolStripMenuItem
            // 
            this.field5ToolStripMenuItem.CheckOnClick = true;
            this.field5ToolStripMenuItem.Name = "field5ToolStripMenuItem";
            this.field5ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.field5ToolStripMenuItem.Text = "Field&5";
            this.field5ToolStripMenuItem.Visible = false;
            this.field5ToolStripMenuItem.Click += new System.EventHandler(this.field5ToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bdgv);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 576);
            this.panel1.TabIndex = 7;
            // 
            // bdgv
            // 
            this.bdgv.AllowDrop = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.bdgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.bdgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bdgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bdgv.LastSelectedRowIndex = 0;
            this.bdgv.Location = new System.Drawing.Point(0, 0);
            this.bdgv.Name = "bdgv";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.bdgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.bdgv.Size = new System.Drawing.Size(800, 323);
            this.bdgv.TabIndex = 0;
            this.bdgv.VirtualMode = true;
            this.bdgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.bdgv_CellClick);
            this.bdgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.bdgv_CellEnter);
            this.bdgv.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.bdgv_CellFormatting);
            this.bdgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.bdgv_CellPainting);
            this.bdgv.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.bdgv_CellValueNeeded);
            this.bdgv.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.bdgv_CellValuePushed);
            this.bdgv.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.bdgv_ColumnWidthChanged);
            this.bdgv.CurrentCellDirtyStateChanged += new System.EventHandler(this.bdgv_CurrentCellDirtyStateChanged);
            this.bdgv.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.bdgv_EditingControlShowing);
            this.bdgv.SelectionChanged += new System.EventHandler(this.bdgv_SelectionChanged);
            // 
            // splitter1
            // 
            this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 323);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(800, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.dgvListOfValues, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvMetaAttributes, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvGlobals, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 326);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 250);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // dgvListOfValues
            // 
            this.dgvListOfValues.AllowUserToAddRows = false;
            this.dgvListOfValues.AllowUserToDeleteRows = false;
            this.dgvListOfValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvListOfValues.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvListOfValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListOfValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dgvListOfValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvListOfValues.Location = new System.Drawing.Point(3, 3);
            this.dgvListOfValues.Name = "dgvListOfValues";
            this.dgvListOfValues.ReadOnly = true;
            this.dgvListOfValues.RowHeadersVisible = false;
            this.dgvListOfValues.Size = new System.Drawing.Size(260, 244);
            this.dgvListOfValues.TabIndex = 0;
            this.dgvListOfValues.DoubleClick += new System.EventHandler(this.dgvListOfValues_DoubleClick);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Value";
            this.Column1.HeaderText = "Valid Values";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // dgvMetaAttributes
            // 
            this.dgvMetaAttributes.AllowUserToAddRows = false;
            this.dgvMetaAttributes.AllowUserToDeleteRows = false;
            this.dgvMetaAttributes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMetaAttributes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMetaAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMetaAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column3,
            this.Column4});
            this.dgvMetaAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMetaAttributes.Location = new System.Drawing.Point(269, 3);
            this.dgvMetaAttributes.Name = "dgvMetaAttributes";
            this.dgvMetaAttributes.RowHeadersVisible = false;
            this.dgvMetaAttributes.Size = new System.Drawing.Size(260, 244);
            this.dgvMetaAttributes.TabIndex = 1;
            this.dgvMetaAttributes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMetaAttributes_CellValueChanged);
            this.dgvMetaAttributes.CurrentCellDirtyStateChanged += new System.EventHandler(this.AutoEndEditing);
            // 
            // dgvGlobals
            // 
            this.dgvGlobals.AllowUserToAddRows = false;
            this.dgvGlobals.AllowUserToDeleteRows = false;
            this.dgvGlobals.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGlobals.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvGlobals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGlobals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.Column6,
            this.Column7});
            this.dgvGlobals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGlobals.Location = new System.Drawing.Point(535, 3);
            this.dgvGlobals.Name = "dgvGlobals";
            this.dgvGlobals.RowHeadersVisible = false;
            this.dgvGlobals.Size = new System.Drawing.Size(262, 244);
            this.dgvGlobals.TabIndex = 2;
            this.dgvGlobals.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGlobals_CellValueChanged);
            this.dgvGlobals.CurrentCellDirtyStateChanged += new System.EventHandler(this.AutoEndEditing);
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.DataPropertyName = "Show";
            this.Column2.FalseValue = false;
            this.Column2.FillWeight = 40F;
            this.Column2.HeaderText = "Show";
            this.Column2.Name = "Column2";
            this.Column2.TrueValue = true;
            this.Column2.Width = 40;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "AttributeName";
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column3.HeaderText = "Meta Attribute";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "Value";
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column4.FillWeight = 150F;
            this.Column4.HeaderText = "Meta Value";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "Show";
            this.Column5.FalseValue = false;
            this.Column5.FillWeight = 40F;
            this.Column5.HeaderText = "Show";
            this.Column5.Name = "Column5";
            this.Column5.TrueValue = true;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "AttributeName";
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column6.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column6.HeaderText = "Global Attribute";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "Value";
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column7.DefaultCellStyle = dataGridViewCellStyle6;
            this.Column7.FillWeight = 150F;
            this.Column7.HeaderText = "Value";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // BuildDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "BuildDataGridView";
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.BuildDataGridView_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bdgv)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvListOfValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetaAttributes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGlobals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMultiValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataAttributesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem field1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem field2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem field3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem field4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem field5ToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAutoFillSouceFetch;
        private System.Windows.Forms.ToolStripMenuItem validateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteValuesToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private CustomDataGridView bdgv;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addNewSKUsToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvListOfValues;
        private System.Windows.Forms.DataGridView dgvMetaAttributes;
        private System.Windows.Forms.DataGridView dgvGlobals;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
    }
}
