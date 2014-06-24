using System.Collections.Generic;
using Arya.Data;

namespace Arya.HelperForms
{
    partial class FrmNewSkus
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
            this.btnTaxonomy = new System.Windows.Forms.Button();
            this.lblSelectedTaxonomy = new System.Windows.Forms.Label();
            this.txtItems = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvItemStatus = new System.Windows.Forms.DataGridView();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tmrReadItems = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.numberOfPotentialMatches = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemStatus)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfPotentialMatches)).BeginInit();
            this.SuspendLayout();
            // 
            // btnTaxonomy
            // 
            this.btnTaxonomy.AutoSize = true;
            this.btnTaxonomy.Location = new System.Drawing.Point(5, 5);
            this.btnTaxonomy.Margin = new System.Windows.Forms.Padding(5);
            this.btnTaxonomy.Name = "btnTaxonomy";
            this.btnTaxonomy.Size = new System.Drawing.Size(99, 23);
            this.btnTaxonomy.TabIndex = 1;
            this.btnTaxonomy.Text = "Select Taxonomy";
            this.btnTaxonomy.UseVisualStyleBackColor = true;
            this.btnTaxonomy.Click += new System.EventHandler(this.btnTaxonomy_Click);
            // 
            // lblSelectedTaxonomy
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lblSelectedTaxonomy, 2);
            this.lblSelectedTaxonomy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectedTaxonomy.Location = new System.Drawing.Point(138, 5);
            this.lblSelectedTaxonomy.Margin = new System.Windows.Forms.Padding(5);
            this.lblSelectedTaxonomy.Name = "lblSelectedTaxonomy";
            this.lblSelectedTaxonomy.Size = new System.Drawing.Size(481, 23);
            this.lblSelectedTaxonomy.TabIndex = 2;
            this.lblSelectedTaxonomy.Text = "SelectedTaxonomy";
            this.lblSelectedTaxonomy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtItems
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtItems, 2);
            this.txtItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtItems.Location = new System.Drawing.Point(138, 38);
            this.txtItems.Margin = new System.Windows.Forms.Padding(5);
            this.txtItems.Name = "txtItems";
            this.txtItems.Size = new System.Drawing.Size(481, 20);
            this.txtItems.TabIndex = 3;
            this.txtItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItems_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Item ID(s):";
            // 
            // dgvItemStatus
            // 
            this.dgvItemStatus.AllowUserToAddRows = false;
            this.dgvItemStatus.AllowUserToDeleteRows = false;
            this.dgvItemStatus.AllowUserToOrderColumns = true;
            this.dgvItemStatus.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvItemStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel1.SetColumnSpan(this.dgvItemStatus, 3);
            this.dgvItemStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItemStatus.Location = new System.Drawing.Point(5, 68);
            this.dgvItemStatus.Margin = new System.Windows.Forms.Padding(5);
            this.dgvItemStatus.Name = "dgvItemStatus";
            this.dgvItemStatus.ReadOnly = true;
            this.dgvItemStatus.Size = new System.Drawing.Size(614, 309);
            this.dgvItemStatus.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(459, 413);
            this.btnOk.Margin = new System.Windows.Forms.Padding(5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Add";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(544, 413);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tmrReadItems
            // 
            this.tmrReadItems.Interval = 1000;
            this.tmrReadItems.Tick += new System.EventHandler(this.tmrReadItems_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnTaxonomy, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvItemStatus, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblSelectedTaxonomy, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtItems, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numberOfPotentialMatches, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 441);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 388);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "No. of Potential Matches:";
            // 
            // numberOfPotentialMatches
            // 
            this.numberOfPotentialMatches.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numberOfPotentialMatches.Location = new System.Drawing.Point(136, 385);
            this.numberOfPotentialMatches.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numberOfPotentialMatches.Name = "numberOfPotentialMatches";
            this.numberOfPotentialMatches.Size = new System.Drawing.Size(51, 20);
            this.numberOfPotentialMatches.TabIndex = 9;
            this.numberOfPotentialMatches.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // FrmNewSkus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmNewSkus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "New SKU(s)";
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemStatus)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfPotentialMatches)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTaxonomy;
        private System.Windows.Forms.Label lblSelectedTaxonomy;
        private System.Windows.Forms.TextBox txtItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvItemStatus;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer tmrReadItems;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numberOfPotentialMatches;
    }
}