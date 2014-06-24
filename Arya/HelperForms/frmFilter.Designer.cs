namespace Arya.HelperForms
{
    partial class FrmFilter
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
            this.lstFilterItems = new System.Windows.Forms.ListView();
            this.colText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkuCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAttCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInSchema = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearThisFilter = new System.Windows.Forms.Button();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this._tblpnlButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnClearAllFilters = new System.Windows.Forms.Button();
            this.tblFind = new System.Windows.Forms.TableLayoutPanel();
            this.lblFind = new System.Windows.Forms.Label();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.btnExportView = new System.Windows.Forms.Button();
            this._tblpnlButtons.SuspendLayout();
            this.tblFind.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstFilterItems
            // 
            this.lstFilterItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colText,
            this.colSkuCount,
            this.colAttCount,
            this.colInSchema});
            this.lstFilterItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFilterItems.FullRowSelect = true;
            this.lstFilterItems.GridLines = true;
            this.lstFilterItems.HideSelection = false;
            this.lstFilterItems.Location = new System.Drawing.Point(0, 34);
            this.lstFilterItems.Name = "lstFilterItems";
            this.lstFilterItems.Size = new System.Drawing.Size(292, 233);
            this.lstFilterItems.TabIndex = 1;
            this.lstFilterItems.UseCompatibleStateImageBehavior = false;
            this.lstFilterItems.View = System.Windows.Forms.View.Details;
            this.lstFilterItems.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstFilterItems_ColumnClick);
            // 
            // colText
            // 
            this.colText.Text = "Value";
            this.colText.Width = 120;
            // 
            // colSkuCount
            // 
            this.colSkuCount.Text = "Total Occurrences";
            this.colSkuCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colSkuCount.Width = 80;
            // 
            // colAttCount
            // 
            this.colAttCount.Text = "Visible Count";
            this.colAttCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colAttCount.Width = 80;
            // 
            // colInSchema
            // 
            this.colInSchema.Text = "In Schema";
            this.colInSchema.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colInSchema.Width = 30;
            // 
            // btnClearThisFilter
            // 
            this.btnClearThisFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearThisFilter.AutoSize = true;
            this.btnClearThisFilter.Location = new System.Drawing.Point(3, 3);
            this.btnClearThisFilter.Name = "btnClearThisFilter";
            this.btnClearThisFilter.Size = new System.Drawing.Size(89, 23);
            this.btnClearThisFilter.TabIndex = 1;
            this.btnClearThisFilter.Text = "Clear This Filter";
            this.btnClearThisFilter.UseVisualStyleBackColor = true;
            this.btnClearThisFilter.Click += new System.EventHandler(this.btnClearThisFilter_Click);
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFilter.AutoSize = true;
            this.btnApplyFilter.Location = new System.Drawing.Point(201, 3);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(88, 23);
            this.btnApplyFilter.TabIndex = 0;
            this.btnApplyFilter.Text = "Apply Filter";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // _tblpnlButtons
            // 
            this._tblpnlButtons.AutoSize = true;
            this._tblpnlButtons.ColumnCount = 4;
            this._tblpnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tblpnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tblpnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tblpnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tblpnlButtons.Controls.Add(this.btnClearAllFilters, 1, 0);
            this._tblpnlButtons.Controls.Add(this.btnClearThisFilter, 0, 0);
            this._tblpnlButtons.Controls.Add(this.btnApplyFilter, 3, 0);
            this._tblpnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._tblpnlButtons.Location = new System.Drawing.Point(0, 267);
            this._tblpnlButtons.Name = "_tblpnlButtons";
            this._tblpnlButtons.RowCount = 1;
            this._tblpnlButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tblpnlButtons.Size = new System.Drawing.Size(292, 29);
            this._tblpnlButtons.TabIndex = 2;
            // 
            // btnClearAllFilters
            // 
            this.btnClearAllFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAllFilters.AutoSize = true;
            this.btnClearAllFilters.Location = new System.Drawing.Point(98, 3);
            this.btnClearAllFilters.Name = "btnClearAllFilters";
            this.btnClearAllFilters.Size = new System.Drawing.Size(85, 23);
            this.btnClearAllFilters.TabIndex = 2;
            this.btnClearAllFilters.Text = "Clear All Filters";
            this.btnClearAllFilters.UseVisualStyleBackColor = true;
            this.btnClearAllFilters.Click += new System.EventHandler(this.btnClearAllFilters_Click);
            // 
            // tblFind
            // 
            this.tblFind.AutoSize = true;
            this.tblFind.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblFind.ColumnCount = 3;
            this.tblFind.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFind.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFind.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblFind.Controls.Add(this.lblFind, 1, 0);
            this.tblFind.Controls.Add(this.txtFind, 2, 0);
            this.tblFind.Controls.Add(this.btnExportView, 0, 0);
            this.tblFind.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblFind.Location = new System.Drawing.Point(0, 0);
            this.tblFind.Margin = new System.Windows.Forms.Padding(2);
            this.tblFind.Name = "tblFind";
            this.tblFind.RowCount = 1;
            this.tblFind.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFind.Size = new System.Drawing.Size(292, 34);
            this.tblFind.TabIndex = 0;
            // 
            // lblFind
            // 
            this.lblFind.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFind.AutoSize = true;
            this.lblFind.Location = new System.Drawing.Point(36, 10);
            this.lblFind.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(32, 13);
            this.lblFind.TabIndex = 0;
            this.lblFind.Text = "Filter:";
            // 
            // txtFind
            // 
            this.txtFind.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtFind.Location = new System.Drawing.Point(72, 7);
            this.txtFind.Margin = new System.Windows.Forms.Padding(2);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(218, 20);
            this.txtFind.TabIndex = 1;
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // btnExportView
            // 
            this.btnExportView.Image = global::Arya.Properties.Resources.ExportTab;
            this.btnExportView.Location = new System.Drawing.Point(3, 3);
            this.btnExportView.Name = "btnExportView";
            this.btnExportView.Size = new System.Drawing.Size(28, 28);
            this.btnExportView.TabIndex = 2;
            this.btnExportView.UseVisualStyleBackColor = true;
            this.btnExportView.Click += new System.EventHandler(this.btnExportView_Click);
            // 
            // frmFilter
            // 
            this.AcceptButton = this.btnApplyFilter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 296);
            this.Controls.Add(this.lstFilterItems);
            this.Controls.Add(this._tblpnlButtons);
            this.Controls.Add(this.tblFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(300, 319);
            this.Name = "FrmFilter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Filter";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFilter_KeyDown);
            this._tblpnlButtons.ResumeLayout(false);
            this._tblpnlButtons.PerformLayout();
            this.tblFind.ResumeLayout(false);
            this.tblFind.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClearThisFilter;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.ListView lstFilterItems;
        private System.Windows.Forms.ColumnHeader colText;
        private System.Windows.Forms.ColumnHeader colSkuCount;
        private System.Windows.Forms.ColumnHeader colAttCount;
        private System.Windows.Forms.TableLayoutPanel _tblpnlButtons;
        private System.Windows.Forms.Button btnClearAllFilters;
        private System.Windows.Forms.TableLayoutPanel tblFind;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnExportView;
        private System.Windows.Forms.ColumnHeader colInSchema;
    }
}