namespace Arya.HelperForms
{
    partial class FrmCloneOptions
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
            this.pnlCustomSchemati = new System.Windows.Forms.Panel();
            this.chkListOfValues = new System.Windows.Forms.CheckBox();
            this.chkSecondarySchemati = new System.Windows.Forms.CheckBox();
            this.chkExcludeRanks = new System.Windows.Forms.CheckBox();
            this.chkPrimarySchemati = new System.Windows.Forms.CheckBox();
            this.rbCurrentSchematusOnly = new System.Windows.Forms.RadioButton();
            this.rbAllSchemati = new System.Windows.Forms.RadioButton();
            this.rbCustomSchemati = new System.Windows.Forms.RadioButton();
            this.btnClone = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCloneToNBOnly = new System.Windows.Forms.CheckBox();
            this.pnlCustomSchemati.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCustomSchemati
            // 
            this.pnlCustomSchemati.Controls.Add(this.chkListOfValues);
            this.pnlCustomSchemati.Controls.Add(this.chkSecondarySchemati);
            this.pnlCustomSchemati.Controls.Add(this.chkExcludeRanks);
            this.pnlCustomSchemati.Controls.Add(this.chkPrimarySchemati);
            this.pnlCustomSchemati.Location = new System.Drawing.Point(28, 107);
            this.pnlCustomSchemati.Name = "pnlCustomSchemati";
            this.pnlCustomSchemati.Size = new System.Drawing.Size(163, 95);
            this.pnlCustomSchemati.TabIndex = 1;
            // 
            // chkListOfValues
            // 
            this.chkListOfValues.AutoSize = true;
            this.chkListOfValues.Location = new System.Drawing.Point(3, 72);
            this.chkListOfValues.Name = "chkListOfValues";
            this.chkListOfValues.Size = new System.Drawing.Size(89, 17);
            this.chkListOfValues.TabIndex = 3;
            this.chkListOfValues.Text = "List of Values";
            this.chkListOfValues.UseVisualStyleBackColor = true;
            // 
            // chkSecondarySchemati
            // 
            this.chkSecondarySchemati.AutoSize = true;
            this.chkSecondarySchemati.Location = new System.Drawing.Point(3, 49);
            this.chkSecondarySchemati.Name = "chkSecondarySchemati";
            this.chkSecondarySchemati.Size = new System.Drawing.Size(159, 17);
            this.chkSecondarySchemati.TabIndex = 2;
            this.chkSecondarySchemati.Text = "User-defined Meta-attributes";
            this.chkSecondarySchemati.UseVisualStyleBackColor = true;
            // 
            // chkExcludeRanks
            // 
            this.chkExcludeRanks.AutoSize = true;
            this.chkExcludeRanks.Location = new System.Drawing.Point(23, 26);
            this.chkExcludeRanks.Name = "chkExcludeRanks";
            this.chkExcludeRanks.Size = new System.Drawing.Size(106, 17);
            this.chkExcludeRanks.TabIndex = 1;
            this.chkExcludeRanks.Text = "Excluding Ranks";
            this.chkExcludeRanks.UseVisualStyleBackColor = true;
            // 
            // chkPrimarySchemati
            // 
            this.chkPrimarySchemati.AutoSize = true;
            this.chkPrimarySchemati.Location = new System.Drawing.Point(3, 3);
            this.chkPrimarySchemati.Name = "chkPrimarySchemati";
            this.chkPrimarySchemati.Size = new System.Drawing.Size(142, 17);
            this.chkPrimarySchemati.TabIndex = 0;
            this.chkPrimarySchemati.Text = "Required Meta-attributes";
            this.chkPrimarySchemati.UseVisualStyleBackColor = true;
            this.chkPrimarySchemati.CheckedChanged += new System.EventHandler(this.chkPrimarySchemati_CheckedChanged);
            // 
            // rbCurrentSchematusOnly
            // 
            this.rbCurrentSchematusOnly.AutoSize = true;
            this.rbCurrentSchematusOnly.Checked = true;
            this.rbCurrentSchematusOnly.Location = new System.Drawing.Point(12, 12);
            this.rbCurrentSchematusOnly.Name = "rbCurrentSchematusOnly";
            this.rbCurrentSchematusOnly.Size = new System.Drawing.Size(151, 17);
            this.rbCurrentSchematusOnly.TabIndex = 2;
            this.rbCurrentSchematusOnly.TabStop = true;
            this.rbCurrentSchematusOnly.Text = "Current Meta-attribute Only";
            this.rbCurrentSchematusOnly.UseVisualStyleBackColor = true;
            this.rbCurrentSchematusOnly.CheckedChanged += new System.EventHandler(this.rbCurrentSchematusOnly_CheckedChanged);
            // 
            // rbAllSchemati
            // 
            this.rbAllSchemati.AutoSize = true;
            this.rbAllSchemati.Location = new System.Drawing.Point(12, 61);
            this.rbAllSchemati.Name = "rbAllSchemati";
            this.rbAllSchemati.Size = new System.Drawing.Size(180, 17);
            this.rbAllSchemati.TabIndex = 3;
            this.rbAllSchemati.Text = "All Meta-attributes & Their Values";
            this.rbAllSchemati.UseMnemonic = false;
            this.rbAllSchemati.UseVisualStyleBackColor = true;
            this.rbAllSchemati.CheckedChanged += new System.EventHandler(this.rbAllSchemati_CheckedChanged);
            // 
            // rbCustomSchemati
            // 
            this.rbCustomSchemati.AutoSize = true;
            this.rbCustomSchemati.Location = new System.Drawing.Point(12, 84);
            this.rbCustomSchemati.Name = "rbCustomSchemati";
            this.rbCustomSchemati.Size = new System.Drawing.Size(63, 17);
            this.rbCustomSchemati.TabIndex = 4;
            this.rbCustomSchemati.Text = "Custom:";
            this.rbCustomSchemati.UseVisualStyleBackColor = true;
            this.rbCustomSchemati.CheckedChanged += new System.EventHandler(this.rbCustomSchemati_CheckedChanged);
            // 
            // btnClone
            // 
            this.btnClone.Location = new System.Drawing.Point(12, 208);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(75, 23);
            this.btnClone.TabIndex = 5;
            this.btnClone.Text = "Clone";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(93, 208);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCloneToNBOnly
            // 
            this.chkCloneToNBOnly.AutoSize = true;
            this.chkCloneToNBOnly.Location = new System.Drawing.Point(31, 35);
            this.chkCloneToNBOnly.Name = "chkCloneToNBOnly";
            this.chkCloneToNBOnly.Size = new System.Drawing.Size(100, 17);
            this.chkCloneToNBOnly.TabIndex = 7;
            this.chkCloneToNBOnly.Text = "Non-Blank Only";
            this.chkCloneToNBOnly.UseVisualStyleBackColor = true;
            // 
            // FrmCloneOptions
            // 
            this.AcceptButton = this.btnClone;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(203, 243);
            this.ControlBox = false;
            this.Controls.Add(this.chkCloneToNBOnly);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClone);
            this.Controls.Add(this.rbCustomSchemati);
            this.Controls.Add(this.rbAllSchemati);
            this.Controls.Add(this.rbCurrentSchematusOnly);
            this.Controls.Add(this.pnlCustomSchemati);
            this.Name = "FrmCloneOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clone Options";
            this.pnlCustomSchemati.ResumeLayout(false);
            this.pnlCustomSchemati.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlCustomSchemati;
        private System.Windows.Forms.CheckBox chkExcludeRanks;
        private System.Windows.Forms.CheckBox chkPrimarySchemati;
        private System.Windows.Forms.CheckBox chkSecondarySchemati;
        private System.Windows.Forms.RadioButton rbCurrentSchematusOnly;
        private System.Windows.Forms.RadioButton rbAllSchemati;
        private System.Windows.Forms.RadioButton rbCustomSchemati;
        private System.Windows.Forms.Button btnClone;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkListOfValues;
        private System.Windows.Forms.CheckBox chkCloneToNBOnly;
    }
}