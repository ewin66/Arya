namespace Arya
{
    partial class FrmSpellCheckCustomDictionary
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
            this.dgvDicktionary = new System.Windows.Forms.DataGridView();
            this.ItemNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Word = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblAddWordStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDicktionary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDicktionary
            // 
            this.dgvDicktionary.AutoGenerateColumns = false;
            this.dgvDicktionary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDicktionary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemNumber,
            this.Word});
            this.dgvDicktionary.DataSource = this.bindingSource1;
            this.dgvDicktionary.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgvDicktionary.Location = new System.Drawing.Point(0, 27);
            this.dgvDicktionary.Name = "dgvDicktionary";
            this.dgvDicktionary.Size = new System.Drawing.Size(400, 526);
            this.dgvDicktionary.TabIndex = 0;
            this.dgvDicktionary.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDicktionary_CellClick);
            this.dgvDicktionary.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDicktionary_CellEndEdit);
            this.dgvDicktionary.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDicktionary_CellLeave);
            this.dgvDicktionary.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDicktionary_CellMouseDoubleClick);
            this.dgvDicktionary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDicktionary_KeyDown);
            // 
            // ItemNumber
            // 
            this.ItemNumber.DataPropertyName = "Index";
            this.ItemNumber.HeaderText = "   #";
            this.ItemNumber.Name = "ItemNumber";
            this.ItemNumber.ReadOnly = true;
            this.ItemNumber.Width = 50;
            // 
            // Word
            // 
            this.Word.DataPropertyName = "Value";
            this.Word.HeaderText = "Word";
            this.Word.Name = "Word";
            this.Word.ReadOnly = true;
            this.Word.Width = 320;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "#";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 30;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Value";
            this.dataGridViewTextBoxColumn2.HeaderText = "Word";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(218, 563);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(307, 562);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblAddWordStatus
            // 
            this.lblAddWordStatus.AutoSize = true;
            this.lblAddWordStatus.Location = new System.Drawing.Point(13, 8);
            this.lblAddWordStatus.Name = "lblAddWordStatus";
            this.lblAddWordStatus.Size = new System.Drawing.Size(0, 13);
            this.lblAddWordStatus.TabIndex = 3;
            // 
            // FrmSpellCheckCustomDictionary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 595);
            this.Controls.Add(this.lblAddWordStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvDicktionary);
            this.Name = "FrmSpellCheckCustomDictionary";
            this.Text = "Spell Check Custom Dictionary";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDicktionary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDicktionary;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Word;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblAddWordStatus;
    }
}