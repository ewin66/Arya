namespace Arya.UserControls
{
    partial class MetaAttributeDataGridView
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
            this.dgvMetas = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetas)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMetas
            // 
            this.dgvMetas.AllowUserToAddRows = false;
            this.dgvMetas.AllowUserToDeleteRows = false;
            this.dgvMetas.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMetas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMetas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMetas.Location = new System.Drawing.Point(0, 0);
            this.dgvMetas.Name = "dgvMetas";
            this.dgvMetas.Size = new System.Drawing.Size(684, 467);
            this.dgvMetas.TabIndex = 0;
            this.dgvMetas.VirtualMode = true;
            this.dgvMetas.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvMetas_CellBeginEdit);
            this.dgvMetas.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvMetas_CellFormatting);
            this.dgvMetas.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvMetas_CellValueNeeded);
            this.dgvMetas.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvMetas_CellValuePushed);
            this.dgvMetas.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMetas_ColumnHeaderMouseClick);
            this.dgvMetas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvMetas_KeyDown);
            // 
            // MetaAttributeDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvMetas);
            this.Name = "MetaAttributeDataGridView";
            this.Size = new System.Drawing.Size(684, 467);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMetas;
    }
}
