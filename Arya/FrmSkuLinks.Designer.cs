namespace Arya
{
    partial class FrmSkuLinks
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
            this.tblFilters = new System.Windows.Forms.TableLayoutPanel();
            this.lnkClearLeftTaxonomy = new System.Windows.Forms.LinkLabel();
            this.lnkGetLeftTaxonomy = new System.Windows.Forms.LinkLabel();
            this.lnkGetRightTaxonomy = new System.Windows.Forms.LinkLabel();
            this.lnkClearRightTaxonomy = new System.Windows.Forms.LinkLabel();
            this.lblLeftTaxonomy = new System.Windows.Forms.Label();
            this.lblRightTaxonomy = new System.Windows.Forms.Label();
            this.lblSkuLinkType = new System.Windows.Forms.Label();
            this.txtFilterSkuLinkType = new System.Windows.Forms.TextBox();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.dgvSkuLinks = new System.Windows.Forms.DataGridView();
            this.colFromItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLeftTaxonomy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colToItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRightTaxonomy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtResultPrefix = new System.Windows.Forms.TextBox();
            this.btnValidateMatched = new System.Windows.Forms.Button();
            this.lblResultPrefix = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSkuLinkType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFindExactMatches = new System.Windows.Forms.Button();
            this.grpFilters = new System.Windows.Forms.GroupBox();
            this.tmrMatch = new System.Windows.Forms.Timer(this.components);
            this.tblFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSkuLinks)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblFilters
            // 
            this.tblFilters.AutoSize = true;
            this.tblFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblFilters.ColumnCount = 3;
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFilters.Controls.Add(this.lnkClearLeftTaxonomy, 1, 0);
            this.tblFilters.Controls.Add(this.lnkGetLeftTaxonomy, 0, 0);
            this.tblFilters.Controls.Add(this.lnkGetRightTaxonomy, 0, 1);
            this.tblFilters.Controls.Add(this.lnkClearRightTaxonomy, 1, 1);
            this.tblFilters.Controls.Add(this.lblLeftTaxonomy, 2, 0);
            this.tblFilters.Controls.Add(this.lblRightTaxonomy, 2, 1);
            this.tblFilters.Controls.Add(this.lblSkuLinkType, 0, 2);
            this.tblFilters.Controls.Add(this.txtFilterSkuLinkType, 1, 2);
            this.tblFilters.Controls.Add(this.btnApplyFilter, 2, 2);
            this.tblFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFilters.Location = new System.Drawing.Point(3, 16);
            this.tblFilters.Name = "tblFilters";
            this.tblFilters.RowCount = 3;
            this.tblFilters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFilters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFilters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFilters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblFilters.Size = new System.Drawing.Size(772, 81);
            this.tblFilters.TabIndex = 0;
            // 
            // lnkClearLeftTaxonomy
            // 
            this.lnkClearLeftTaxonomy.AutoSize = true;
            this.lnkClearLeftTaxonomy.LinkColor = System.Drawing.Color.Black;
            this.lnkClearLeftTaxonomy.Location = new System.Drawing.Point(165, 6);
            this.lnkClearLeftTaxonomy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lnkClearLeftTaxonomy.Name = "lnkClearLeftTaxonomy";
            this.lnkClearLeftTaxonomy.Size = new System.Drawing.Size(31, 13);
            this.lnkClearLeftTaxonomy.TabIndex = 7;
            this.lnkClearLeftTaxonomy.TabStop = true;
            this.lnkClearLeftTaxonomy.Text = "Clear";
            this.lnkClearLeftTaxonomy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearLeftTaxonomy_LinkClicked);
            // 
            // lnkGetLeftTaxonomy
            // 
            this.lnkGetLeftTaxonomy.AutoSize = true;
            this.lnkGetLeftTaxonomy.LinkColor = System.Drawing.Color.Black;
            this.lnkGetLeftTaxonomy.Location = new System.Drawing.Point(3, 6);
            this.lnkGetLeftTaxonomy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lnkGetLeftTaxonomy.Name = "lnkGetLeftTaxonomy";
            this.lnkGetLeftTaxonomy.Size = new System.Drawing.Size(146, 13);
            this.lnkGetLeftTaxonomy.TabIndex = 3;
            this.lnkGetLeftTaxonomy.TabStop = true;
            this.lnkGetLeftTaxonomy.Text = "Get Sku Taxonomy from Tree";
            this.lnkGetLeftTaxonomy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGetLeftTaxonomy_LinkClicked);
            // 
            // lnkGetRightTaxonomy
            // 
            this.lnkGetRightTaxonomy.AutoSize = true;
            this.lnkGetRightTaxonomy.LinkColor = System.Drawing.Color.Black;
            this.lnkGetRightTaxonomy.Location = new System.Drawing.Point(3, 32);
            this.lnkGetRightTaxonomy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lnkGetRightTaxonomy.Name = "lnkGetRightTaxonomy";
            this.lnkGetRightTaxonomy.Size = new System.Drawing.Size(156, 13);
            this.lnkGetRightTaxonomy.TabIndex = 4;
            this.lnkGetRightTaxonomy.TabStop = true;
            this.lnkGetRightTaxonomy.Text = "Get Image Taxonomy from Tree";
            this.lnkGetRightTaxonomy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGetRightTaxonomy_LinkClicked);
            // 
            // lnkClearRightTaxonomy
            // 
            this.lnkClearRightTaxonomy.AutoSize = true;
            this.lnkClearRightTaxonomy.LinkColor = System.Drawing.Color.Black;
            this.lnkClearRightTaxonomy.Location = new System.Drawing.Point(165, 32);
            this.lnkClearRightTaxonomy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lnkClearRightTaxonomy.Name = "lnkClearRightTaxonomy";
            this.lnkClearRightTaxonomy.Size = new System.Drawing.Size(31, 13);
            this.lnkClearRightTaxonomy.TabIndex = 8;
            this.lnkClearRightTaxonomy.TabStop = true;
            this.lnkClearRightTaxonomy.Text = "Clear";
            this.lnkClearRightTaxonomy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearRightTaxonomy_LinkClicked);
            // 
            // lblLeftTaxonomy
            // 
            this.lblLeftTaxonomy.AutoEllipsis = true;
            this.lblLeftTaxonomy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLeftTaxonomy.Location = new System.Drawing.Point(322, 6);
            this.lblLeftTaxonomy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lblLeftTaxonomy.Name = "lblLeftTaxonomy";
            this.lblLeftTaxonomy.Size = new System.Drawing.Size(447, 17);
            this.lblLeftTaxonomy.TabIndex = 5;
            this.lblLeftTaxonomy.Text = "No Filter Applied";
            // 
            // lblRightTaxonomy
            // 
            this.lblRightTaxonomy.AutoEllipsis = true;
            this.lblRightTaxonomy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRightTaxonomy.Location = new System.Drawing.Point(322, 32);
            this.lblRightTaxonomy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lblRightTaxonomy.Name = "lblRightTaxonomy";
            this.lblRightTaxonomy.Size = new System.Drawing.Size(447, 17);
            this.lblRightTaxonomy.TabIndex = 6;
            this.lblRightTaxonomy.Text = "No Filter Applied";
            // 
            // lblSkuLinkType
            // 
            this.lblSkuLinkType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSkuLinkType.AutoSize = true;
            this.lblSkuLinkType.Location = new System.Drawing.Point(80, 58);
            this.lblSkuLinkType.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblSkuLinkType.Name = "lblSkuLinkType";
            this.lblSkuLinkType.Size = new System.Drawing.Size(79, 13);
            this.lblSkuLinkType.TabIndex = 9;
            this.lblSkuLinkType.Text = "Sku Link Type:";
            // 
            // txtFilterSkuLinkType
            // 
            this.txtFilterSkuLinkType.Location = new System.Drawing.Point(165, 55);
            this.txtFilterSkuLinkType.Name = "txtFilterSkuLinkType";
            this.txtFilterSkuLinkType.Size = new System.Drawing.Size(151, 20);
            this.txtFilterSkuLinkType.TabIndex = 10;
            this.txtFilterSkuLinkType.Text = "Sku-to-Image, Client Supplied";
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Location = new System.Drawing.Point(322, 55);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(84, 23);
            this.btnApplyFilter.TabIndex = 11;
            this.btnApplyFilter.Text = "Apply Filter";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            // 
            // dgvSkuLinks
            // 
            this.dgvSkuLinks.AllowUserToAddRows = false;
            this.dgvSkuLinks.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSkuLinks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSkuLinks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSkuLinks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFromItemId,
            this.colLeftTaxonomy,
            this.colToItemId,
            this.colRightTaxonomy});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSkuLinks.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSkuLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSkuLinks.Location = new System.Drawing.Point(3, 222);
            this.dgvSkuLinks.Name = "dgvSkuLinks";
            this.dgvSkuLinks.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSkuLinks.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSkuLinks.Size = new System.Drawing.Size(778, 217);
            this.dgvSkuLinks.TabIndex = 1;
            this.dgvSkuLinks.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvSkuLinks_RowsAdded);
            // 
            // colFromItemId
            // 
            this.colFromItemId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colFromItemId.DataPropertyName = "LeftSku";
            this.colFromItemId.HeaderText = "From SKU";
            this.colFromItemId.Name = "colFromItemId";
            this.colFromItemId.ReadOnly = true;
            this.colFromItemId.Width = 74;
            // 
            // colLeftTaxonomy
            // 
            this.colLeftTaxonomy.DataPropertyName = "LeftTaxonomy";
            this.colLeftTaxonomy.HeaderText = "From (Left) Taxonomy";
            this.colLeftTaxonomy.Name = "colLeftTaxonomy";
            this.colLeftTaxonomy.ReadOnly = true;
            this.colLeftTaxonomy.Width = 250;
            // 
            // colToItemId
            // 
            this.colToItemId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colToItemId.DataPropertyName = "RightSku";
            this.colToItemId.HeaderText = "To SKU";
            this.colToItemId.Name = "colToItemId";
            this.colToItemId.ReadOnly = true;
            this.colToItemId.Width = 65;
            // 
            // colRightTaxonomy
            // 
            this.colRightTaxonomy.DataPropertyName = "RightTaxonomy";
            this.colRightTaxonomy.HeaderText = "To (Right) Taxonomy";
            this.colRightTaxonomy.Name = "colRightTaxonomy";
            this.colRightTaxonomy.ReadOnly = true;
            this.colRightTaxonomy.Width = 250;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dgvSkuLinks, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.grpFilters, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 442);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 109);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(778, 107);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtResultPrefix);
            this.tabPage1.Controls.Add(this.btnValidateMatched);
            this.tabPage1.Controls.Add(this.lblResultPrefix);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(770, 81);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Validate Matched Links";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtResultPrefix
            // 
            this.txtResultPrefix.Location = new System.Drawing.Point(123, 15);
            this.txtResultPrefix.Name = "txtResultPrefix";
            this.txtResultPrefix.Size = new System.Drawing.Size(74, 20);
            this.txtResultPrefix.TabIndex = 3;
            this.txtResultPrefix.Text = "AutoGen_";
            // 
            // btnValidateMatched
            // 
            this.btnValidateMatched.Location = new System.Drawing.Point(9, 51);
            this.btnValidateMatched.Name = "btnValidateMatched";
            this.btnValidateMatched.Size = new System.Drawing.Size(188, 23);
            this.btnValidateMatched.TabIndex = 0;
            this.btnValidateMatched.Text = "Validate Matched Links";
            this.btnValidateMatched.UseVisualStyleBackColor = true;
            this.btnValidateMatched.Click += new System.EventHandler(this.btnValidateMatched_Click);
            // 
            // lblResultPrefix
            // 
            this.lblResultPrefix.AutoSize = true;
            this.lblResultPrefix.Location = new System.Drawing.Point(6, 18);
            this.lblResultPrefix.Name = "lblResultPrefix";
            this.lblResultPrefix.Size = new System.Drawing.Size(111, 13);
            this.lblResultPrefix.TabIndex = 2;
            this.lblResultPrefix.Text = "Result Attribute Prefix:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.txtSkuLinkType);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btnFindExactMatches);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(770, 81);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Find Exact Matches";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(302, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Remember: Left Taxonomy = SKUs, Right Taxonomy = Images";
            // 
            // txtSkuLinkType
            // 
            this.txtSkuLinkType.Location = new System.Drawing.Point(117, 9);
            this.txtSkuLinkType.Name = "txtSkuLinkType";
            this.txtSkuLinkType.Size = new System.Drawing.Size(143, 20);
            this.txtSkuLinkType.TabIndex = 5;
            this.txtSkuLinkType.Text = "Suggested AutoGen Link";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "SKU Link Type Prefix:";
            // 
            // btnFindExactMatches
            // 
            this.btnFindExactMatches.Location = new System.Drawing.Point(9, 52);
            this.btnFindExactMatches.Name = "btnFindExactMatches";
            this.btnFindExactMatches.Size = new System.Drawing.Size(159, 23);
            this.btnFindExactMatches.TabIndex = 1;
            this.btnFindExactMatches.Text = "Find Suggested Matches";
            this.btnFindExactMatches.UseVisualStyleBackColor = true;
            this.btnFindExactMatches.Click += new System.EventHandler(this.btnFindMatches_Click);
            // 
            // grpFilters
            // 
            this.grpFilters.AutoSize = true;
            this.grpFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpFilters.Controls.Add(this.tblFilters);
            this.grpFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFilters.Location = new System.Drawing.Point(3, 3);
            this.grpFilters.Name = "grpFilters";
            this.grpFilters.Size = new System.Drawing.Size(778, 100);
            this.grpFilters.TabIndex = 0;
            this.grpFilters.TabStop = false;
            this.grpFilters.Text = "Filters";
            // 
            // tmrMatch
            // 
            this.tmrMatch.Interval = 500;
            this.tmrMatch.Tick += new System.EventHandler(this.tmrMatch_Tick);
            // 
            // FrmSkuLinks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmSkuLinks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Sku Links";
            this.tblFilters.ResumeLayout(false);
            this.tblFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSkuLinks)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.grpFilters.ResumeLayout(false);
            this.grpFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblFilters;
        private System.Windows.Forms.LinkLabel lnkClearLeftTaxonomy;
        private System.Windows.Forms.LinkLabel lnkGetLeftTaxonomy;
        private System.Windows.Forms.LinkLabel lnkGetRightTaxonomy;
        private System.Windows.Forms.LinkLabel lnkClearRightTaxonomy;
        private System.Windows.Forms.DataGridView dgvSkuLinks;
        private System.Windows.Forms.Label lblLeftTaxonomy;
        private System.Windows.Forms.Label lblRightTaxonomy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpFilters;
        private System.Windows.Forms.Button btnFindExactMatches;
        private System.Windows.Forms.Button btnValidateMatched;
        private System.Windows.Forms.TextBox txtResultPrefix;
        private System.Windows.Forms.Label lblResultPrefix;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFromItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLeftTaxonomy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colToItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRightTaxonomy;
        private System.Windows.Forms.Timer tmrMatch;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtSkuLinkType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSkuLinkType;
        private System.Windows.Forms.TextBox txtFilterSkuLinkType;
        private System.Windows.Forms.Button btnApplyFilter;
    }
}