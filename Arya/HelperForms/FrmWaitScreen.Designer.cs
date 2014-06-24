using System.Windows.Forms;
using Arya.HelperClasses;

namespace Arya.HelperForms
{
    partial class FrmWaitScreen
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
            this.lblWait = new System.Windows.Forms.Label();
            this.pictBackgroundImage = new System.Windows.Forms.PictureBox();
            this.autoHideTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictBackgroundImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.BackColor = System.Drawing.Color.Transparent;
            this.lblWait.Location = new System.Drawing.Point(12, 23);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(73, 13);
            this.lblWait.TabIndex = 0;
            this.lblWait.Text = "Please Wait...";
            this.lblWait.UseWaitCursor = true;
            // 
            // pictBackgroundImage
            // 
            this.pictBackgroundImage.BackgroundImage = global::Arya.Properties.Resources.StatusBackground;
            this.pictBackgroundImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictBackgroundImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictBackgroundImage.Location = new System.Drawing.Point(0, 0);
            this.pictBackgroundImage.Margin = new System.Windows.Forms.Padding(2);
            this.pictBackgroundImage.Name = "pictBackgroundImage";
            this.pictBackgroundImage.Size = new System.Drawing.Size(640, 59);
            this.pictBackgroundImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictBackgroundImage.TabIndex = 1;
            this.pictBackgroundImage.TabStop = false;
            this.pictBackgroundImage.UseWaitCursor = true;
            // 
            // autoHideTimer
            // 
            this.autoHideTimer.Enabled = true;
            this.autoHideTimer.Interval = 1000;
            this.autoHideTimer.Tick += new System.EventHandler(this.autoHideTimer_Tick);
            // 
            // FrmWaitScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(640, 59);
            this.ControlBox = false;
            this.Controls.Add(this.lblWait);
            this.Controls.Add(this.pictBackgroundImage);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmWaitScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please Wait";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPleaseWait_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictBackgroundImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWait;
        private PictureBox pictBackgroundImage;
        private Timer autoHideTimer;
    }
}