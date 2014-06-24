namespace Arya
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class FrmUnitOfMeasure
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
            this.tcUomTabs = new System.Windows.Forms.TabControl();
            this.tpUomConversion = new System.Windows.Forms.TabPage();
            this.dgvUnitOfMeasure = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tpProjectUom = new System.Windows.Forms.TabPage();
            this.dgvProjectUom = new System.Windows.Forms.DataGridView();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tcUomTabs.SuspendLayout();
            this.tpUomConversion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnitOfMeasure)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tpProjectUom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectUom)).BeginInit();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcUomTabs
            // 
            this.tcUomTabs.Controls.Add(this.tpUomConversion);
            this.tcUomTabs.Controls.Add(this.tpProjectUom);
            this.tcUomTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcUomTabs.Location = new System.Drawing.Point(0, 0);
            this.tcUomTabs.Name = "tcUomTabs";
            this.tcUomTabs.SelectedIndex = 0;
            this.tcUomTabs.Size = new System.Drawing.Size(786, 339);
            this.tcUomTabs.TabIndex = 0;
            tcUomTabs.SelectedIndexChanged += new EventHandler(tcUomTabs_SelectedIndexChanged);
            // 
            // tpUomConversion
            // 
            this.tpUomConversion.Controls.Add(this.dgvUnitOfMeasure);
            this.tpUomConversion.Controls.Add(this.menuStrip1);
            this.tpUomConversion.Location = new System.Drawing.Point(4, 22);
            this.tpUomConversion.Name = "tpUomConversion";
            this.tpUomConversion.Padding = new System.Windows.Forms.Padding(3);
            this.tpUomConversion.Size = new System.Drawing.Size(778, 313);
            this.tpUomConversion.TabIndex = 0;
            this.tpUomConversion.Text = "Global Units of Measure";
            this.tpUomConversion.UseVisualStyleBackColor = true;
            // 
            // dgvUnitOfMeasure
            // 
            this.dgvUnitOfMeasure.AllowUserToAddRows = false;
            this.dgvUnitOfMeasure.AllowUserToDeleteRows = false;
            this.dgvUnitOfMeasure.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvUnitOfMeasure.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnitOfMeasure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUnitOfMeasure.Location = new System.Drawing.Point(3, 27);
            this.dgvUnitOfMeasure.MultiSelect = false;
            this.dgvUnitOfMeasure.Name = "dgvUnitOfMeasure";
            this.dgvUnitOfMeasure.Size = new System.Drawing.Size(772, 283);
            this.dgvUnitOfMeasure.TabIndex = 0;
            this.dgvUnitOfMeasure.VirtualMode = true;
            this.dgvUnitOfMeasure.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvUnitOfMeasure_CellValueNeeded);
            this.dgvUnitOfMeasure.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvUnitOfMeasure_CellValuePushed);
            this.dgvUnitOfMeasure.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvUnitOfMeasure_ColumnHeaderMouseClick);
            this.dgvUnitOfMeasure.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.ErrorHandle);
          //  this.dgvUnitOfMeasure.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUnitOfMeasure_CellValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(3, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(772, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem1});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(235, 22);
            this.deleteToolStripMenuItem1.Text = "&Delete selected UOM";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteGlobalUomToolStripMenuItem_Click);
            // 
            // tpProjectUom
            // 
            this.tpProjectUom.Controls.Add(this.dgvProjectUom);
            this.tpProjectUom.Controls.Add(this.menuStrip2);
            this.tpProjectUom.Location = new System.Drawing.Point(4, 22);
            this.tpProjectUom.Name = "tpProjectUom";
            this.tpProjectUom.Padding = new System.Windows.Forms.Padding(3);
            this.tpProjectUom.Size = new System.Drawing.Size(778, 313);
            this.tpProjectUom.TabIndex = 1;
            this.tpProjectUom.Text = "Project UoMs";
            this.tpProjectUom.UseVisualStyleBackColor = true;
            
            // 
            // dgvProjectUom
            // 
            this.dgvProjectUom.AllowUserToAddRows = false;
            this.dgvProjectUom.AllowUserToDeleteRows = false;
            this.dgvProjectUom.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvProjectUom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProjectUom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProjectUom.Location = new System.Drawing.Point(3, 27);
            this.dgvProjectUom.Name = "dgvProjectUom";
            this.dgvProjectUom.Size = new System.Drawing.Size(772, 283);
            this.dgvProjectUom.TabIndex = 0;
            this.dgvProjectUom.VirtualMode = true;
            this.dgvProjectUom.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvProjectUom_CellValueNeeded);
            this.dgvProjectUom.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvProjectUom_CellValuePushed);
            this.dgvProjectUom.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvProjectUom_ColumnHeaderMouseClick);
           
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem1});
            this.menuStrip2.Location = new System.Drawing.Point(3, 3);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(772, 24);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem1.Text = "&Edit";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "&Delete selected UOM";
            this.deleteToolStripMenuItem.ToolTipText = "Delete Selected Row";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteProjectUomToolStripMenuItem_Click);
            // 
            // FrmUnitOfMeasure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 339);
            this.Controls.Add(this.tcUomTabs);
            this.MainMenuStrip = this.menuStrip2;
            this.Name = "FrmUnitOfMeasure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Units of Measure";
            this.Load += new System.EventHandler(this.FrmUnitOfMeasure_Load);
            this.tcUomTabs.ResumeLayout(false);
            this.tpUomConversion.ResumeLayout(false);
            this.tpUomConversion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnitOfMeasure)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tpProjectUom.ResumeLayout(false);
            this.tpProjectUom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectUom)).EndInit();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcUomTabs;
        private System.Windows.Forms.TabPage tpUomConversion;
        private System.Windows.Forms.TabPage tpProjectUom;
        private System.Windows.Forms.DataGridView dgvUnitOfMeasure;
        private System.Windows.Forms.DataGridView dgvProjectUom;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
    }
}