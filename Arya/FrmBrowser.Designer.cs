namespace Arya
{
    partial class FrmBrowser
    {
		#region Fields (2) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

		#endregion Fields 

		#region Methods (1) 

		// Protected Methods (1) 

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

		#endregion Methods 



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._itemWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // _itemWebBrowser
            // 
            this._itemWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemWebBrowser.Location = new System.Drawing.Point(0, 0);
            this._itemWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._itemWebBrowser.Name = "_itemWebBrowser";
            this._itemWebBrowser.ScriptErrorsSuppressed = true;
            this._itemWebBrowser.Size = new System.Drawing.Size(784, 561);
            this._itemWebBrowser.TabIndex = 0;
            // 
            // FrmBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this._itemWebBrowser);
            this.Name = "FrmBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Item on the Web";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LookupItem_FormClosing);
            this.ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.WebBrowser _itemWebBrowser;

    }
}