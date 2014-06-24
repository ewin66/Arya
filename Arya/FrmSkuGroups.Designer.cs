namespace Arya
{
    partial class FrmSkuGroups
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.dgvGroupDetails = new System.Windows.Forms.DataGridView();
            this.colTaxonomy = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSkuGroups = new System.Windows.Forms.DataGridView();
            this.colGroup = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colGroupDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCriterion = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colCreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExport = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.dgvComment = new System.Windows.Forms.DataGridView();
            this.colComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.timerGroupDetails = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroupDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSkuGroups)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComment)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitter3);
            this.panel2.Controls.Add(this.dgvGroupDetails);
            this.panel2.Controls.Add(this.dgvSkuGroups);
            this.panel2.Controls.Add(this.splitter2);
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(784, 538);
            this.panel2.TabIndex = 2;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter3.Location = new System.Drawing.Point(525, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 378);
            this.splitter3.TabIndex = 5;
            this.splitter3.TabStop = false;
            // 
            // dgvGroupDetails
            // 
            this.dgvGroupDetails.AllowUserToAddRows = false;
            this.dgvGroupDetails.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGroupDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvGroupDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroupDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTaxonomy,
            this.colCount});
            this.dgvGroupDetails.Dock = System.Windows.Forms.DockStyle.Right;
            this.dgvGroupDetails.Location = new System.Drawing.Point(528, 0);
            this.dgvGroupDetails.Name = "dgvGroupDetails";
            this.dgvGroupDetails.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGroupDetails.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvGroupDetails.Size = new System.Drawing.Size(256, 378);
            this.dgvGroupDetails.TabIndex = 1;
            this.dgvGroupDetails.VirtualMode = true;
            this.dgvGroupDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGroupDetails_CellContentClick);
            this.dgvGroupDetails.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvGroupDetails_CellPainting);
            this.dgvGroupDetails.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvGroupDetails_CellValueNeeded);
            // 
            // colTaxonomy
            // 
            this.colTaxonomy.HeaderText = "Taxonomy";
            this.colTaxonomy.Name = "colTaxonomy";
            this.colTaxonomy.ReadOnly = true;
            this.colTaxonomy.Width = 440;
            // 
            // colCount
            // 
            this.colCount.HeaderText = "Sku Count";
            this.colCount.Name = "colCount";
            this.colCount.ReadOnly = true;
            this.colCount.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCount.Width = 75;
            // 
            // dgvSkuGroups
            // 
            this.dgvSkuGroups.AllowUserToAddRows = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSkuGroups.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSkuGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSkuGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGroup,
            this.colGroupDescription,
            this.colCriterion,
            this.colCreatedBy,
            this.colCreatedOn,
            this.colExport,
            this.colDelete});
            this.dgvSkuGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSkuGroups.Location = new System.Drawing.Point(0, 0);
            this.dgvSkuGroups.Name = "dgvSkuGroups";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSkuGroups.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSkuGroups.Size = new System.Drawing.Size(784, 378);
            this.dgvSkuGroups.TabIndex = 0;
            this.dgvSkuGroups.VirtualMode = true;
            this.dgvSkuGroups.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSkuGroups_CellContentClick);
            this.dgvSkuGroups.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvSkuGroups_CellPainting);
            this.dgvSkuGroups.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvSkuGroups_CellValueNeeded);
            this.dgvSkuGroups.SelectionChanged += new System.EventHandler(this.dgvSkuGroups_SelectionChanged);
            // 
            // colGroup
            // 
            this.colGroup.HeaderText = "Group";
            this.colGroup.Name = "colGroup";
            this.colGroup.ReadOnly = true;
            this.colGroup.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colGroup.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colGroup.Width = 170;
            // 
            // colGroupDescription
            // 
            this.colGroupDescription.HeaderText = "Group Description";
            this.colGroupDescription.Name = "colGroupDescription";
            this.colGroupDescription.Width = 250;
            // 
            // colCriterion
            // 
            this.colCriterion.HeaderText = "Criterion";
            this.colCriterion.Name = "colCriterion";
            this.colCriterion.ReadOnly = true;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.HeaderText = "Created By";
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.ReadOnly = true;
            this.colCreatedBy.Width = 120;
            // 
            // colCreatedOn
            // 
            this.colCreatedOn.HeaderText = "Created On";
            this.colCreatedOn.Name = "colCreatedOn";
            this.colCreatedOn.ReadOnly = true;
            this.colCreatedOn.Width = 150;
            // 
            // colExport
            // 
            this.colExport.HeaderText = "Download";
            this.colExport.Name = "colExport";
            this.colExport.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colExport.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colExport.Width = 75;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "X";
            this.colDelete.Name = "colDelete";
            this.colDelete.Width = 27;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 378);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(784, 3);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 381);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.10191F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.89809F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 157);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.dgvComment);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(778, 114);
            this.panel1.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(776, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 114);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // dgvComment
            // 
            this.dgvComment.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvComment.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvComment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colComment,
            this.colBy});
            this.dgvComment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvComment.Location = new System.Drawing.Point(0, 0);
            this.dgvComment.Name = "dgvComment";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvComment.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvComment.Size = new System.Drawing.Size(778, 114);
            this.dgvComment.TabIndex = 0;
            this.dgvComment.VirtualMode = true;
            this.dgvComment.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvComment_CellPainting);
            this.dgvComment.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvComment_CellValueNeeded);
            this.dgvComment.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvComment_CellValuePushed);
            // 
            // colComment
            // 
            this.colComment.HeaderText = "Comments";
            this.colComment.Name = "colComment";
            this.colComment.Width = 350;
            // 
            // colBy
            // 
            this.colBy.HeaderText = "Commented By";
            this.colBy.Name = "colBy";
            this.colBy.ReadOnly = true;
            this.colBy.Width = 180;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(778, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "User Comments";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerGroupDetails
            // 
            this.timerGroupDetails.Tick += new System.EventHandler(this.timerGroupDetails_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notesToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.showToolStripMenuItem.Text = "Show";
            // 
            // notesToolStripMenuItem
            // 
            this.notesToolStripMenuItem.Name = "notesToolStripMenuItem";
            this.notesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemtilde)));
            this.notesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.notesToolStripMenuItem.Text = "Notes";
            this.notesToolStripMenuItem.Click += new System.EventHandler(this.notesToolStripMenuItem_Click);
            // 
            // FrmSkuGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmSkuGroups";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sku Group Console";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroupDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSkuGroups)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComment)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvSkuGroups;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerGroupDetails;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridView dgvGroupDetails;
        private System.Windows.Forms.DataGridViewLinkColumn colTaxonomy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCount;
        private System.Windows.Forms.DataGridView dgvComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBy;
        private System.Windows.Forms.DataGridViewLinkColumn colGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupDescription;
        private System.Windows.Forms.DataGridViewLinkColumn colCriterion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreatedOn;
        private System.Windows.Forms.DataGridViewLinkColumn colExport;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notesToolStripMenuItem;
    }
}