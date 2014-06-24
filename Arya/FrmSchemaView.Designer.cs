namespace Arya
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Properties;
    using Attribute = Data.Attribute;

    partial class FrmSchemaView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.mainTabControl = new TabControl();
            this.btnClose = new Button();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Dock = DockStyle.Fill;
            this.mainTabControl.Location = new Point(0, 0);
            this.mainTabControl.Margin = new Padding(4);
            this.mainTabControl.Multiline = true;
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.Padding = new Point(26, 3);
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.ShowToolTips = true;
            this.mainTabControl.Size = new Size(517, 366);
            this.mainTabControl.TabIndex = 3;
            this.mainTabControl.SelectedIndexChanged += new EventHandler(this.mainTabControl_SelectedIndexChanged);
            // 
            // btnClose
            // 
            this.btnClose.Image = Resources.Close;
            this.btnClose.Location = new Point(248, 173);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(20, 20);
            this.btnClose.TabIndex = 4;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            // 
            // FrmSchemaView
            // 
            this.AutoScaleDimensions = new SizeF(8F, 16F);
//            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(517, 366);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.mainTabControl);
            this.Name = "FrmSchemaView";
            this.StartPosition = FormStartPosition.Manual;
            this.Text = "FrmSchemaView";
            this.FormClosing += new FormClosingEventHandler(this.FrmSchemaView_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        internal TabControl mainTabControl;
        private Button btnClose;
    }
}