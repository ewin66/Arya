namespace Arya
{
    partial class FrmSkuView
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.exportTabFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCellToolTip = new System.Windows.Forms.Label();
            this.cellValueToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Multiline = true;
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.Padding = new System.Drawing.Point(26, 3);
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.ShowToolTips = true;
            this.mainTabControl.Size = new System.Drawing.Size(784, 562);
            this.mainTabControl.TabIndex = 2;
            this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.mainTabControl_SelectedIndexChanged);
            // 
            // exportTabFileDialog
            // 
            this.exportTabFileDialog.DefaultExt = "txt";
            this.exportTabFileDialog.Filter = "Tab delimited text files|*.txt|XML files|*.xml";
            // 
            // btnClose
            // 
            this.btnClose.Image = global::Arya.Properties.Resources.Close;
            this.btnClose.Location = new System.Drawing.Point(75, 41);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(15, 16);
            this.btnClose.TabIndex = 3;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblCellToolTip
            // 
            this.lblCellToolTip.AutoSize = true;
            this.lblCellToolTip.BackColor = System.Drawing.Color.Transparent;
            this.lblCellToolTip.Location = new System.Drawing.Point(75, 81);
            this.lblCellToolTip.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCellToolTip.Name = "lblCellToolTip";
            this.lblCellToolTip.Size = new System.Drawing.Size(87, 13);
            this.lblCellToolTip.TabIndex = 4;
            this.lblCellToolTip.Text = "Label for ToolTip";
            // 
            // cellValueToolTip
            // 
            this.cellValueToolTip.UseAnimation = false;
            this.cellValueToolTip.UseFading = false;
            // 
            // FrmSkuView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.lblCellToolTip);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.mainTabControl);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FrmSkuView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SKU View";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkuManagerMainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SkuManagerMainForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.SaveFileDialog exportTabFileDialog;
        internal System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.Label lblCellToolTip;
        public System.Windows.Forms.ToolTip cellValueToolTip;
    }
}