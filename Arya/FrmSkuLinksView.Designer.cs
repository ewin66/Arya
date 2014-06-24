namespace Arya
{
    partial class FrmSkuLinksView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSkuLinksView));
            this.tblLinks = new System.Windows.Forms.TableLayoutPanel();
            this.lblLinksTo = new System.Windows.Forms.Label();
            this.lblLinkedFrom = new System.Windows.Forms.Label();
            this.dgvLinksTo = new System.Windows.Forms.DataGridView();
            this.dgvLinkedFrom = new System.Windows.Forms.DataGridView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lblSkuDescription = new System.Windows.Forms.Label();
            this.tblSkuDetails = new System.Windows.Forms.TableLayoutPanel();
            this.lblSkuTaxonomy = new System.Windows.Forms.Label();
            this.dgvLinkedSku = new System.Windows.Forms.DataGridView();
            this.chkPin = new System.Windows.Forms.CheckBox();
            this.bgwPopulateLinks = new System.ComponentModel.BackgroundWorker();
            this.tblLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinksTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinkedFrom)).BeginInit();
            this.tblSkuDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinkedSku)).BeginInit();
            this.SuspendLayout();
            // 
            // tblLinks
            // 
            this.tblLinks.ColumnCount = 1;
            this.tblLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLinks.Controls.Add(this.lblLinksTo, 0, 0);
            this.tblLinks.Controls.Add(this.lblLinkedFrom, 0, 2);
            this.tblLinks.Controls.Add(this.dgvLinksTo, 0, 1);
            this.tblLinks.Controls.Add(this.dgvLinkedFrom, 0, 3);
            this.tblLinks.Dock = System.Windows.Forms.DockStyle.Left;
            this.tblLinks.Location = new System.Drawing.Point(0, 0);
            this.tblLinks.Name = "tblLinks";
            this.tblLinks.RowCount = 4;
            this.tblLinks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.3212F));
            this.tblLinks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.6788F));
            this.tblLinks.Size = new System.Drawing.Size(288, 485);
            this.tblLinks.TabIndex = 2;
            // 
            // lblLinksTo
            // 
            this.lblLinksTo.AutoSize = true;
            this.lblLinksTo.Location = new System.Drawing.Point(3, 0);
            this.lblLinksTo.Name = "lblLinksTo";
            this.lblLinksTo.Size = new System.Drawing.Size(47, 13);
            this.lblLinksTo.TabIndex = 0;
            this.lblLinksTo.Text = "Links to:";
            // 
            // lblLinkedFrom
            // 
            this.lblLinkedFrom.AutoSize = true;
            this.lblLinkedFrom.Location = new System.Drawing.Point(3, 243);
            this.lblLinkedFrom.Name = "lblLinkedFrom";
            this.lblLinkedFrom.Size = new System.Drawing.Size(65, 13);
            this.lblLinkedFrom.TabIndex = 1;
            this.lblLinkedFrom.Text = "Linked from:";
            // 
            // dgvLinksTo
            // 
            this.dgvLinksTo.AllowUserToAddRows = false;
            this.dgvLinksTo.AllowUserToDeleteRows = false;
            this.dgvLinksTo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLinksTo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLinksTo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLinksTo.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLinksTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLinksTo.Location = new System.Drawing.Point(3, 16);
            this.dgvLinksTo.MultiSelect = false;
            this.dgvLinksTo.Name = "dgvLinksTo";
            this.dgvLinksTo.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLinksTo.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLinksTo.RowHeadersVisible = false;
            this.dgvLinksTo.Size = new System.Drawing.Size(282, 224);
            this.dgvLinksTo.TabIndex = 2;
            this.dgvLinksTo.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLinkedFromAndTo_CellMouseDoubleClick);
            this.dgvLinksTo.SelectionChanged += new System.EventHandler(this.dgvLinksFromAndTo_SelectionChanged);
            // 
            // dgvLinkedFrom
            // 
            this.dgvLinkedFrom.AllowUserToAddRows = false;
            this.dgvLinkedFrom.AllowUserToDeleteRows = false;
            this.dgvLinkedFrom.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLinkedFrom.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvLinkedFrom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLinkedFrom.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLinkedFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLinkedFrom.Location = new System.Drawing.Point(3, 259);
            this.dgvLinkedFrom.MultiSelect = false;
            this.dgvLinkedFrom.Name = "dgvLinkedFrom";
            this.dgvLinkedFrom.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLinkedFrom.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLinkedFrom.RowHeadersVisible = false;
            this.dgvLinkedFrom.Size = new System.Drawing.Size(282, 223);
            this.dgvLinkedFrom.TabIndex = 3;
            this.dgvLinkedFrom.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLinkedFromAndTo_CellMouseDoubleClick);
            this.dgvLinkedFrom.SelectionChanged += new System.EventHandler(this.dgvLinksFromAndTo_SelectionChanged);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Location = new System.Drawing.Point(288, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 485);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // lblSkuDescription
            // 
            this.lblSkuDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSkuDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSkuDescription.Location = new System.Drawing.Point(25, 0);
            this.lblSkuDescription.Name = "lblSkuDescription";
            this.lblSkuDescription.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblSkuDescription.Size = new System.Drawing.Size(581, 24);
            this.lblSkuDescription.TabIndex = 5;
            this.lblSkuDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tblSkuDetails
            // 
            this.tblSkuDetails.ColumnCount = 2;
            this.tblSkuDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tblSkuDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSkuDetails.Controls.Add(this.lblSkuTaxonomy, 0, 1);
            this.tblSkuDetails.Controls.Add(this.dgvLinkedSku, 0, 2);
            this.tblSkuDetails.Controls.Add(this.lblSkuDescription, 1, 0);
            this.tblSkuDetails.Controls.Add(this.chkPin, 0, 0);
            this.tblSkuDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSkuDetails.Location = new System.Drawing.Point(293, 0);
            this.tblSkuDetails.Name = "tblSkuDetails";
            this.tblSkuDetails.RowCount = 3;
            this.tblSkuDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblSkuDetails.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSkuDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSkuDetails.Size = new System.Drawing.Size(609, 485);
            this.tblSkuDetails.TabIndex = 6;
            // 
            // lblSkuTaxonomy
            // 
            this.lblSkuTaxonomy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblSkuDetails.SetColumnSpan(this.lblSkuTaxonomy, 2);
            this.lblSkuTaxonomy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSkuTaxonomy.Location = new System.Drawing.Point(3, 24);
            this.lblSkuTaxonomy.Name = "lblSkuTaxonomy";
            this.lblSkuTaxonomy.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lblSkuTaxonomy.Size = new System.Drawing.Size(603, 50);
            this.lblSkuTaxonomy.TabIndex = 6;
            // 
            // dgvLinkedSku
            // 
            this.dgvLinkedSku.AllowUserToAddRows = false;
            this.dgvLinkedSku.AllowUserToDeleteRows = false;
            this.dgvLinkedSku.AllowUserToOrderColumns = true;
            this.dgvLinkedSku.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvLinkedSku.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblSkuDetails.SetColumnSpan(this.dgvLinkedSku, 2);
            this.dgvLinkedSku.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLinkedSku.Location = new System.Drawing.Point(3, 77);
            this.dgvLinkedSku.Name = "dgvLinkedSku";
            this.dgvLinkedSku.ReadOnly = true;
            this.dgvLinkedSku.RowHeadersVisible = false;
            this.dgvLinkedSku.Size = new System.Drawing.Size(603, 405);
            this.dgvLinkedSku.TabIndex = 6;
            // 
            // chkPin
            // 
            this.chkPin.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.chkPin.FlatAppearance.BorderSize = 0;
            this.chkPin.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
            this.chkPin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkPin.ForeColor = System.Drawing.SystemColors.Control;
            this.chkPin.Image = ((System.Drawing.Image)(resources.GetObject("chkPin.Image")));
            this.chkPin.Location = new System.Drawing.Point(3, 3);
            this.chkPin.Name = "chkPin";
            this.chkPin.Size = new System.Drawing.Size(16, 18);
            this.chkPin.TabIndex = 7;
            this.chkPin.UseVisualStyleBackColor = true;
            // 
            // bgwPopulateLinks
            // 
            this.bgwPopulateLinks.WorkerSupportsCancellation = true;
            this.bgwPopulateLinks.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPopulateLinks_DoWork);
            // 
            // FrmSkuLinksView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 485);
            this.Controls.Add(this.tblSkuDetails);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tblLinks);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSkuLinksView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FrmSkuLinksView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSkuLinksView_FormClosing);
            this.tblLinks.ResumeLayout(false);
            this.tblLinks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinksTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinkedFrom)).EndInit();
            this.tblSkuDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinkedSku)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLinks;
        private System.Windows.Forms.Label lblLinksTo;
        private System.Windows.Forms.Label lblLinkedFrom;
        private System.Windows.Forms.DataGridView dgvLinksTo;
        private System.Windows.Forms.DataGridView dgvLinkedFrom;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label lblSkuDescription;
        private System.Windows.Forms.TableLayoutPanel tblSkuDetails;
        private System.Windows.Forms.DataGridView dgvLinkedSku;
        private System.Windows.Forms.Label lblSkuTaxonomy;
        private System.ComponentModel.BackgroundWorker bgwPopulateLinks;
        private System.Windows.Forms.CheckBox chkPin;
    }
}