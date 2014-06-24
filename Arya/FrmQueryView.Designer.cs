namespace Arya
{
    partial class FrmQueryView
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
            this.lnkLimitTaxonomyFromTreeView = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanelTaxonomy = new System.Windows.Forms.TableLayoutPanel();
            this.lnkLimitTaxonomyFromResults = new System.Windows.Forms.LinkLabel();
            this.lnkClearTaxonomyFilter = new System.Windows.Forms.LinkLabel();
            this.grpTaxonomy = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.ddDefaultAction = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpValue = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelValue = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAnd = new System.Windows.Forms.Button();
            this.btnOr = new System.Windows.Forms.Button();
            this.lnkClearValueFilter = new System.Windows.Forms.LinkLabel();
            this.ddFieldName = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.ddValueFilterType = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnCrossList = new System.Windows.Forms.Button();
            this.pnlOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkInSchemaAttributes = new System.Windows.Forms.CheckBox();
            this.chkDisplayAttributes = new System.Windows.Forms.CheckBox();
            this.chkNavigationAttributes = new System.Windows.Forms.CheckBox();
            this.chkGlobalAttributes = new System.Windows.Forms.CheckBox();
            this.grpButtons = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtBoxCrossListNode = new System.Windows.Forms.TextBox();
            this.btnGroupSkus = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.dgvTaxonomyResults = new System.Windows.Forms.DataGridView();
            this.groupBoxCrossList = new System.Windows.Forms.GroupBox();
            this.txtBoxSelection = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelTaxonomy.SuspendLayout();
            this.grpTaxonomy.SuspendLayout();
            this.grpValue.SuspendLayout();
            this.tableLayoutPanelValue.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpButtons.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.grpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaxonomyResults)).BeginInit();
            this.groupBoxCrossList.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnkLimitTaxonomyFromTreeView
            // 
            this.lnkLimitTaxonomyFromTreeView.AutoSize = true;
            this.lnkLimitTaxonomyFromTreeView.LinkColor = System.Drawing.Color.Black;
            this.lnkLimitTaxonomyFromTreeView.Location = new System.Drawing.Point(3, 6);
            this.lnkLimitTaxonomyFromTreeView.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lnkLimitTaxonomyFromTreeView.Name = "lnkLimitTaxonomyFromTreeView";
            this.lnkLimitTaxonomyFromTreeView.Size = new System.Drawing.Size(119, 13);
            this.lnkLimitTaxonomyFromTreeView.TabIndex = 1;
            this.lnkLimitTaxonomyFromTreeView.TabStop = true;
            this.lnkLimitTaxonomyFromTreeView.Text = "Get Selection from Tree";
            this.lnkLimitTaxonomyFromTreeView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLimitTaxonomyFromTreeView_LinkClicked);
            // 
            // tableLayoutPanelTaxonomy
            // 
            this.tableLayoutPanelTaxonomy.AutoSize = true;
            this.tableLayoutPanelTaxonomy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelTaxonomy.ColumnCount = 1;
            this.tableLayoutPanelTaxonomy.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTaxonomy.Controls.Add(this.lnkLimitTaxonomyFromTreeView, 0, 0);
            this.tableLayoutPanelTaxonomy.Controls.Add(this.lnkLimitTaxonomyFromResults, 0, 1);
            this.tableLayoutPanelTaxonomy.Controls.Add(this.lnkClearTaxonomyFilter, 0, 2);
            this.tableLayoutPanelTaxonomy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTaxonomy.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanelTaxonomy.Name = "tableLayoutPanelTaxonomy";
            this.tableLayoutPanelTaxonomy.RowCount = 3;
            this.tableLayoutPanelTaxonomy.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTaxonomy.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTaxonomy.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelTaxonomy.Size = new System.Drawing.Size(254, 63);
            this.tableLayoutPanelTaxonomy.TabIndex = 2;
            // 
            // lnkLimitTaxonomyFromResults
            // 
            this.lnkLimitTaxonomyFromResults.AutoSize = true;
            this.lnkLimitTaxonomyFromResults.LinkColor = System.Drawing.Color.Black;
            this.lnkLimitTaxonomyFromResults.Location = new System.Drawing.Point(3, 25);
            this.lnkLimitTaxonomyFromResults.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lnkLimitTaxonomyFromResults.Name = "lnkLimitTaxonomyFromResults";
            this.lnkLimitTaxonomyFromResults.Size = new System.Drawing.Size(119, 13);
            this.lnkLimitTaxonomyFromResults.TabIndex = 4;
            this.lnkLimitTaxonomyFromResults.TabStop = true;
            this.lnkLimitTaxonomyFromResults.Text = "Get Nodes from Results";
            this.lnkLimitTaxonomyFromResults.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLimitTaxonomyFromSearchResults_LinkClicked);
            // 
            // lnkClearTaxonomyFilter
            // 
            this.lnkClearTaxonomyFilter.AutoSize = true;
            this.lnkClearTaxonomyFilter.LinkColor = System.Drawing.Color.Black;
            this.lnkClearTaxonomyFilter.Location = new System.Drawing.Point(3, 44);
            this.lnkClearTaxonomyFilter.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lnkClearTaxonomyFilter.Name = "lnkClearTaxonomyFilter";
            this.lnkClearTaxonomyFilter.Size = new System.Drawing.Size(31, 13);
            this.lnkClearTaxonomyFilter.TabIndex = 2;
            this.lnkClearTaxonomyFilter.TabStop = true;
            this.lnkClearTaxonomyFilter.Text = "Clear";
            this.lnkClearTaxonomyFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearTaxonomyFilter_LinkClicked);
            // 
            // grpTaxonomy
            // 
            this.grpTaxonomy.Controls.Add(this.tableLayoutPanelTaxonomy);
            this.grpTaxonomy.Location = new System.Drawing.Point(3, 3);
            this.grpTaxonomy.Name = "grpTaxonomy";
            this.grpTaxonomy.Size = new System.Drawing.Size(260, 82);
            this.grpTaxonomy.TabIndex = 5;
            this.grpTaxonomy.TabStop = false;
            this.grpTaxonomy.Text = "Taxonomy";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(3, 29);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(78, 21);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Get Counts";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(3, 58);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(78, 21);
            this.btnOpen.TabIndex = 8;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // ddDefaultAction
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.ddDefaultAction, 2);
            this.ddDefaultAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddDefaultAction.FormattingEnabled = true;
            this.ddDefaultAction.Location = new System.Drawing.Point(87, 58);
            this.ddDefaultAction.Name = "ddDefaultAction";
            this.ddDefaultAction.Size = new System.Drawing.Size(152, 21);
            this.ddDefaultAction.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 429);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 13);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "Ready";
            // 
            // grpValue
            // 
            this.grpValue.Controls.Add(this.tableLayoutPanelValue);
            this.grpValue.Location = new System.Drawing.Point(3, 202);
            this.grpValue.Name = "grpValue";
            this.grpValue.Size = new System.Drawing.Size(260, 120);
            this.grpValue.TabIndex = 7;
            this.grpValue.TabStop = false;
            this.grpValue.Text = "Value";
            // 
            // tableLayoutPanelValue
            // 
            this.tableLayoutPanelValue.AutoSize = true;
            this.tableLayoutPanelValue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelValue.ColumnCount = 2;
            this.tableLayoutPanelValue.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelValue.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelValue.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanelValue.Controls.Add(this.ddFieldName, 0, 0);
            this.tableLayoutPanelValue.Controls.Add(this.txtValue, 0, 1);
            this.tableLayoutPanelValue.Controls.Add(this.ddValueFilterType, 1, 0);
            this.tableLayoutPanelValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelValue.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanelValue.Name = "tableLayoutPanelValue";
            this.tableLayoutPanelValue.RowCount = 3;
            this.tableLayoutPanelValue.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelValue.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelValue.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelValue.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanelValue.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanelValue.Size = new System.Drawing.Size(254, 101);
            this.tableLayoutPanelValue.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelValue.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.btnAnd);
            this.flowLayoutPanel1.Controls.Add(this.btnOr);
            this.flowLayoutPanel1.Controls.Add(this.lnkClearValueFilter);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 70);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(137, 29);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // btnAnd
            // 
            this.btnAnd.Location = new System.Drawing.Point(3, 3);
            this.btnAnd.Name = "btnAnd";
            this.btnAnd.Size = new System.Drawing.Size(44, 23);
            this.btnAnd.TabIndex = 6;
            this.btnAnd.Text = "And";
            this.btnAnd.UseVisualStyleBackColor = true;
            this.btnAnd.Click += new System.EventHandler(this.btnAnd_Click);
            // 
            // btnOr
            // 
            this.btnOr.Location = new System.Drawing.Point(53, 3);
            this.btnOr.Name = "btnOr";
            this.btnOr.Size = new System.Drawing.Size(44, 23);
            this.btnOr.TabIndex = 7;
            this.btnOr.Text = "Or";
            this.btnOr.UseVisualStyleBackColor = true;
            this.btnOr.Click += new System.EventHandler(this.btnOr_Click);
            // 
            // lnkClearValueFilter
            // 
            this.lnkClearValueFilter.AutoSize = true;
            this.lnkClearValueFilter.LinkColor = System.Drawing.Color.Black;
            this.lnkClearValueFilter.Location = new System.Drawing.Point(103, 6);
            this.lnkClearValueFilter.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lnkClearValueFilter.Name = "lnkClearValueFilter";
            this.lnkClearValueFilter.Size = new System.Drawing.Size(31, 13);
            this.lnkClearValueFilter.TabIndex = 9;
            this.lnkClearValueFilter.TabStop = true;
            this.lnkClearValueFilter.Text = "Clear";
            this.lnkClearValueFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearValueFilter_LinkClicked);
            // 
            // ddFieldName
            // 
            this.ddFieldName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddFieldName.DropDownWidth = 200;
            this.ddFieldName.FormattingEnabled = true;
            this.ddFieldName.Location = new System.Drawing.Point(3, 3);
            this.ddFieldName.Name = "ddFieldName";
            this.ddFieldName.Size = new System.Drawing.Size(80, 21);
            this.ddFieldName.TabIndex = 5;
            this.ddFieldName.SelectedIndexChanged += new System.EventHandler(this.ddItemValue_SelectedIndexChanged);
            // 
            // txtValue
            // 
            this.tableLayoutPanelValue.SetColumnSpan(this.txtValue, 2);
            this.txtValue.Location = new System.Drawing.Point(3, 30);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(248, 35);
            this.txtValue.TabIndex = 3;
            // 
            // ddValueFilterType
            // 
            this.ddValueFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddValueFilterType.FormattingEnabled = true;
            this.ddValueFilterType.Location = new System.Drawing.Point(89, 3);
            this.ddValueFilterType.Name = "ddValueFilterType";
            this.ddValueFilterType.Size = new System.Drawing.Size(83, 21);
            this.ddValueFilterType.TabIndex = 3;
            // 
            // btnCrossList
            // 
            this.btnCrossList.Location = new System.Drawing.Point(87, 29);
            this.btnCrossList.Name = "btnCrossList";
            this.btnCrossList.Size = new System.Drawing.Size(78, 21);
            this.btnCrossList.TabIndex = 9;
            this.btnCrossList.Text = "Cross List";
            this.toolTip.SetToolTip(this.btnCrossList, "Cross List");
            this.btnCrossList.UseVisualStyleBackColor = true;
            this.btnCrossList.Click += new System.EventHandler(this.btnCrossList_Click);
            // 
            // pnlOptions
            // 
            this.pnlOptions.Controls.Add(this.grpTaxonomy);
            this.pnlOptions.Controls.Add(this.groupBox1);
            this.pnlOptions.Controls.Add(this.grpValue);
            this.pnlOptions.Controls.Add(this.grpButtons);
            this.pnlOptions.Controls.Add(this.lblStatus);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlOptions.Location = new System.Drawing.Point(0, 0);
            this.pnlOptions.Margin = new System.Windows.Forms.Padding(2);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(269, 562);
            this.pnlOptions.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkInSchemaAttributes);
            this.groupBox1.Controls.Add(this.chkDisplayAttributes);
            this.groupBox1.Controls.Add(this.chkNavigationAttributes);
            this.groupBox1.Controls.Add(this.chkGlobalAttributes);
            this.groupBox1.Location = new System.Drawing.Point(2, 90);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(258, 107);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Attribute Type";
            // 
            // chkInSchemaAttributes
            // 
            this.chkInSchemaAttributes.AutoSize = true;
            this.chkInSchemaAttributes.Location = new System.Drawing.Point(10, 86);
            this.chkInSchemaAttributes.Margin = new System.Windows.Forms.Padding(2);
            this.chkInSchemaAttributes.Name = "chkInSchemaAttributes";
            this.chkInSchemaAttributes.Size = new System.Drawing.Size(154, 17);
            this.chkInSchemaAttributes.TabIndex = 4;
            this.chkInSchemaAttributes.Text = "In-Schema Attribute Values";
            this.chkInSchemaAttributes.UseVisualStyleBackColor = true;
            this.chkInSchemaAttributes.CheckedChanged += new System.EventHandler(this.chkInSchemaAttributes_CheckedChanged);
            // 
            // chkDisplayAttributes
            // 
            this.chkDisplayAttributes.AutoSize = true;
            this.chkDisplayAttributes.Location = new System.Drawing.Point(10, 43);
            this.chkDisplayAttributes.Margin = new System.Windows.Forms.Padding(2);
            this.chkDisplayAttributes.Name = "chkDisplayAttributes";
            this.chkDisplayAttributes.Size = new System.Drawing.Size(137, 17);
            this.chkDisplayAttributes.TabIndex = 3;
            this.chkDisplayAttributes.Text = "Display Attribute Values";
            this.chkDisplayAttributes.UseVisualStyleBackColor = true;
            this.chkDisplayAttributes.CheckedChanged += new System.EventHandler(this.chkDisplayAttributes_CheckedChanged);
            // 
            // chkNavigationAttributes
            // 
            this.chkNavigationAttributes.AutoSize = true;
            this.chkNavigationAttributes.Location = new System.Drawing.Point(10, 21);
            this.chkNavigationAttributes.Margin = new System.Windows.Forms.Padding(2);
            this.chkNavigationAttributes.Name = "chkNavigationAttributes";
            this.chkNavigationAttributes.Size = new System.Drawing.Size(154, 17);
            this.chkNavigationAttributes.TabIndex = 2;
            this.chkNavigationAttributes.Text = "Navigation Attribute Values";
            this.chkNavigationAttributes.UseVisualStyleBackColor = true;
            this.chkNavigationAttributes.CheckedChanged += new System.EventHandler(this.chkNavigationAttributes_CheckedChanged);
            // 
            // chkGlobalAttributes
            // 
            this.chkGlobalAttributes.AutoSize = true;
            this.chkGlobalAttributes.Location = new System.Drawing.Point(10, 65);
            this.chkGlobalAttributes.Margin = new System.Windows.Forms.Padding(2);
            this.chkGlobalAttributes.Name = "chkGlobalAttributes";
            this.chkGlobalAttributes.Size = new System.Drawing.Size(133, 17);
            this.chkGlobalAttributes.TabIndex = 1;
            this.chkGlobalAttributes.Text = "Global Attribute Values";
            this.chkGlobalAttributes.UseVisualStyleBackColor = true;
            this.chkGlobalAttributes.CheckedChanged += new System.EventHandler(this.chkGlobalAttributes_CheckedChanged);
            // 
            // grpButtons
            // 
            this.grpButtons.Controls.Add(this.tableLayoutPanel1);
            this.grpButtons.Location = new System.Drawing.Point(2, 327);
            this.grpButtons.Margin = new System.Windows.Forms.Padding(2);
            this.grpButtons.Name = "grpButtons";
            this.grpButtons.Padding = new System.Windows.Forms.Padding(2);
            this.grpButtons.Size = new System.Drawing.Size(258, 100);
            this.grpButtons.TabIndex = 10;
            this.grpButtons.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.txtBoxCrossListNode, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ddDefaultAction, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCrossList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnOpen, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnGroupSkus, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(254, 83);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // txtBoxCrossListNode
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtBoxCrossListNode, 3);
            this.txtBoxCrossListNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBoxCrossListNode.Location = new System.Drawing.Point(3, 3);
            this.txtBoxCrossListNode.Name = "txtBoxCrossListNode";
            this.txtBoxCrossListNode.ReadOnly = true;
            this.txtBoxCrossListNode.Size = new System.Drawing.Size(248, 20);
            this.txtBoxCrossListNode.TabIndex = 10;
            this.txtBoxCrossListNode.Text = "Cross List Node";
            // 
            // btnGroupSkus
            // 
            this.btnGroupSkus.Location = new System.Drawing.Point(171, 29);
            this.btnGroupSkus.Name = "btnGroupSkus";
            this.btnGroupSkus.Size = new System.Drawing.Size(80, 23);
            this.btnGroupSkus.TabIndex = 11;
            this.btnGroupSkus.Text = "Group Skus";
            this.btnGroupSkus.UseVisualStyleBackColor = true;
            this.btnGroupSkus.Click += new System.EventHandler(this.btnGroupSkus_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.grpResults, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBoxCrossList, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(269, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(485, 562);
            this.tableLayoutPanel3.TabIndex = 9;
            // 
            // grpResults
            // 
            this.grpResults.Controls.Add(this.dgvTaxonomyResults);
            this.grpResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpResults.Location = new System.Drawing.Point(3, 190);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(479, 369);
            this.grpResults.TabIndex = 8;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Results";
            // 
            // dgvTaxonomyResults
            // 
            this.dgvTaxonomyResults.AllowUserToAddRows = false;
            this.dgvTaxonomyResults.AllowUserToDeleteRows = false;
            this.dgvTaxonomyResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTaxonomyResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTaxonomyResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTaxonomyResults.Location = new System.Drawing.Point(3, 16);
            this.dgvTaxonomyResults.Name = "dgvTaxonomyResults";
            this.dgvTaxonomyResults.ReadOnly = true;
            this.dgvTaxonomyResults.Size = new System.Drawing.Size(473, 350);
            this.dgvTaxonomyResults.TabIndex = 1;
            this.dgvTaxonomyResults.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvTaxonomyResults_CellPainting);
            this.dgvTaxonomyResults.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTaxonomyResults_ColumnHeaderMouseClick);
            this.dgvTaxonomyResults.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvTaxonomyResults_RowsAdded);
            this.dgvTaxonomyResults.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvTaxonomyResults_RowsRemoved);
            // 
            // groupBoxCrossList
            // 
            this.groupBoxCrossList.Controls.Add(this.txtBoxSelection);
            this.groupBoxCrossList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCrossList.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCrossList.Name = "groupBoxCrossList";
            this.groupBoxCrossList.Size = new System.Drawing.Size(479, 181);
            this.groupBoxCrossList.TabIndex = 9;
            this.groupBoxCrossList.TabStop = false;
            this.groupBoxCrossList.Text = "Selection";
            // 
            // txtBoxSelection
            // 
            this.txtBoxSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBoxSelection.Location = new System.Drawing.Point(3, 16);
            this.txtBoxSelection.Multiline = true;
            this.txtBoxSelection.Name = "txtBoxSelection";
            this.txtBoxSelection.ReadOnly = true;
            this.txtBoxSelection.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBoxSelection.Size = new System.Drawing.Size(473, 162);
            this.txtBoxSelection.TabIndex = 11;
            // 
            // FrmQueryView
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 562);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.pnlOptions);
            this.Name = "FrmQueryView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Query";
            this.Load += new System.EventHandler(this.FrmQueryView_Load);
            this.tableLayoutPanelTaxonomy.ResumeLayout(false);
            this.tableLayoutPanelTaxonomy.PerformLayout();
            this.grpTaxonomy.ResumeLayout(false);
            this.grpTaxonomy.PerformLayout();
            this.grpValue.ResumeLayout(false);
            this.grpValue.PerformLayout();
            this.tableLayoutPanelValue.ResumeLayout(false);
            this.tableLayoutPanelValue.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpButtons.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.grpResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaxonomyResults)).EndInit();
            this.groupBoxCrossList.ResumeLayout(false);
            this.groupBoxCrossList.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkLimitTaxonomyFromTreeView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTaxonomy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelValue;
        private System.Windows.Forms.ComboBox ddValueFilterType;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ComboBox ddDefaultAction;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.LinkLabel lnkLimitTaxonomyFromResults;
        private System.Windows.Forms.GroupBox grpTaxonomy;
        private System.Windows.Forms.GroupBox grpValue;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ComboBox ddFieldName;
        private System.Windows.Forms.Button btnAnd;
        private System.Windows.Forms.Button btnOr;
        private System.Windows.Forms.LinkLabel lnkClearValueFilter;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel pnlOptions;
        private System.Windows.Forms.GroupBox grpButtons;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkDisplayAttributes;
        private System.Windows.Forms.CheckBox chkNavigationAttributes;
        private System.Windows.Forms.CheckBox chkGlobalAttributes;
        private System.Windows.Forms.Button btnCrossList;
        private System.Windows.Forms.TextBox txtBoxCrossListNode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox txtBoxSelection;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.DataGridView dgvTaxonomyResults;
        private System.Windows.Forms.GroupBox groupBoxCrossList;
        private System.Windows.Forms.LinkLabel lnkClearTaxonomyFilter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnGroupSkus;
        private System.Windows.Forms.CheckBox chkInSchemaAttributes;
    }
}