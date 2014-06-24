namespace Arya
{
    partial class FrmCharacterMap
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
            this.dgvCharacterMaps = new System.Windows.Forms.DataGridView();
            this.pnlFilters = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBlock = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtSymbol = new System.Windows.Forms.TextBox();
            this.pnlStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lnkBrowserTest = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCharacterMaps)).BeginInit();
            this.pnlFilters.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvCharacterMaps
            // 
            this.dgvCharacterMaps.AllowUserToAddRows = false;
            this.dgvCharacterMaps.AllowUserToDeleteRows = false;
            this.dgvCharacterMaps.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCharacterMaps.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvCharacterMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCharacterMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCharacterMaps.Location = new System.Drawing.Point(0, 39);
            this.dgvCharacterMaps.Margin = new System.Windows.Forms.Padding(2);
            this.dgvCharacterMaps.Name = "dgvCharacterMaps";
            this.dgvCharacterMaps.ReadOnly = true;
            this.dgvCharacterMaps.RowTemplate.Height = 24;
            this.dgvCharacterMaps.Size = new System.Drawing.Size(346, 419);
            this.dgvCharacterMaps.TabIndex = 1;
            this.dgvCharacterMaps.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCharacterMaps_CellClick);
            this.dgvCharacterMaps.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvCharacterMaps_CellPainting);
            // 
            // pnlFilters
            // 
            this.pnlFilters.ColumnCount = 3;
            this.pnlFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.pnlFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.pnlFilters.Controls.Add(this.label1, 0, 0);
            this.pnlFilters.Controls.Add(this.label2, 1, 0);
            this.pnlFilters.Controls.Add(this.label3, 2, 0);
            this.pnlFilters.Controls.Add(this.txtBlock, 0, 1);
            this.pnlFilters.Controls.Add(this.txtDescription, 1, 1);
            this.pnlFilters.Controls.Add(this.txtSymbol, 2, 1);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilters.Location = new System.Drawing.Point(0, 0);
            this.pnlFilters.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.RowCount = 2;
            this.pnlFilters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFilters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlFilters.Size = new System.Drawing.Size(346, 39);
            this.pnlFilters.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Block";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Description";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(278, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Symbol";
            // 
            // txtBlock
            // 
            this.txtBlock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBlock.Location = new System.Drawing.Point(2, 15);
            this.txtBlock.Margin = new System.Windows.Forms.Padding(2);
            this.txtBlock.Name = "txtBlock";
            this.txtBlock.Size = new System.Drawing.Size(99, 20);
            this.txtBlock.TabIndex = 0;
            this.txtBlock.TextChanged += new System.EventHandler(this.txtBlock_TextChanged);
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(105, 15);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(2);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(169, 20);
            this.txtDescription.TabIndex = 1;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // txtSymbol
            // 
            this.txtSymbol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSymbol.Location = new System.Drawing.Point(278, 15);
            this.txtSymbol.Margin = new System.Windows.Forms.Padding(2);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(66, 20);
            this.txtSymbol.TabIndex = 2;
            this.txtSymbol.TextChanged += new System.EventHandler(this.txtSymbol_TextChanged);
            // 
            // pnlStatus
            // 
            this.pnlStatus.AutoSize = true;
            this.pnlStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Controls.Add(this.lnkBrowserTest);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(0, 458);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(346, 25);
            this.pnlStatus.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(6, 6);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(6);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            // 
            // lnkBrowserTest
            // 
            this.lnkBrowserTest.AutoSize = true;
            this.lnkBrowserTest.Location = new System.Drawing.Point(56, 6);
            this.lnkBrowserTest.Margin = new System.Windows.Forms.Padding(6);
            this.lnkBrowserTest.Name = "lnkBrowserTest";
            this.lnkBrowserTest.Size = new System.Drawing.Size(69, 13);
            this.lnkBrowserTest.TabIndex = 1;
            this.lnkBrowserTest.TabStop = true;
            this.lnkBrowserTest.Text = "Browser Test";
            this.lnkBrowserTest.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBrowserTest_LinkClicked);
            // 
            // FrmCharacterMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 483);
            this.Controls.Add(this.dgvCharacterMaps);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlFilters);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmCharacterMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Character Map";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCharacterMaps)).EndInit();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCharacterMaps;
        private System.Windows.Forms.TableLayoutPanel pnlFilters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBlock;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtSymbol;
        private System.Windows.Forms.FlowLayoutPanel pnlStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.LinkLabel lnkBrowserTest;
    }
}