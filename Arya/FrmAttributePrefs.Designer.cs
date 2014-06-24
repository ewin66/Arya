namespace Natalie
{
    partial class FrmAttributePrefs
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
            this.pgAttrPrefs = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pgAttrPrefs
            // 
            this.pgAttrPrefs.Location = new System.Drawing.Point(12, 12);
            this.pgAttrPrefs.Name = "pgAttrPrefs";
            this.pgAttrPrefs.Size = new System.Drawing.Size(582, 505);
            this.pgAttrPrefs.TabIndex = 0;
            // 
            // FrmAttributePrefs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 518);
            this.Controls.Add(this.pgAttrPrefs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "FrmAttributePrefs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attribute Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAttributePrefs_FormClosing);
            this.Load += new System.EventHandler(this.FrmAttributePrefs_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgAttrPrefs;
    }
}