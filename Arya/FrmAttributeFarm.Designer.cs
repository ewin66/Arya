namespace Arya
{
    partial class FrmAttributeFarm
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
            this.attributeFarmGridView1 = new Arya.UserControls.AttributeFarmGridView();
            this.SuspendLayout();
            // 
            // attributeFarmGridView1
            // 
            this.attributeFarmGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributeFarmGridView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.attributeFarmGridView1.Location = new System.Drawing.Point(0, 0);
            this.attributeFarmGridView1.Name = "attributeFarmGridView1";
            this.attributeFarmGridView1.Size = new System.Drawing.Size(445, 567);
            this.attributeFarmGridView1.TabIndex = 0;
            // 
            // FrmAttributeFarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 567);
            this.Controls.Add(this.attributeFarmGridView1);
            this.KeyPreview = true;
            this.Name = "FrmAttributeFarm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Attribute Farm";
            this.Load += new System.EventHandler(this.FrmAttributeFarm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmAttributeFarm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.AttributeFarmGridView attributeFarmGridView1;

    }
}