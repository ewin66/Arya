namespace Arya.HelperForms
{
    partial class FrmRemark
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
            this.comboBoxRemarks = new System.Windows.Forms.ComboBox();
            this.remarkBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tblRemarks = new System.Windows.Forms.TableLayoutPanel();
            this.chkBoxIsCanned = new System.Windows.Forms.CheckBox();
            this.chkBoxStick = new System.Windows.Forms.CheckBox();
            this.chkBoxShowAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.remarkBindingSource)).BeginInit();
            this.tblRemarks.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxRemarks
            // 
            this.comboBoxRemarks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxRemarks.FormattingEnabled = true;
            this.comboBoxRemarks.Location = new System.Drawing.Point(3, 3);
            this.comboBoxRemarks.Name = "comboBoxRemarks";
            this.comboBoxRemarks.Size = new System.Drawing.Size(268, 21);
            this.comboBoxRemarks.TabIndex = 24;
            this.comboBoxRemarks.SelectedIndexChanged += new System.EventHandler(this.comboBoxRemarks_SelectedIndexChanged);
            this.comboBoxRemarks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxRemarks_KeyDown);
            // 
            // remarkBindingSource
            // 
            this.remarkBindingSource.DataSource = typeof(Arya.Data.Remark);
            // 
            // tblRemarks
            // 
            this.tblRemarks.AutoSize = true;
            this.tblRemarks.ColumnCount = 4;
            this.tblRemarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.68501F));
            this.tblRemarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tblRemarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tblRemarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tblRemarks.Controls.Add(this.chkBoxIsCanned, 2, 0);
            this.tblRemarks.Controls.Add(this.chkBoxStick, 1, 0);
            this.tblRemarks.Controls.Add(this.comboBoxRemarks, 0, 0);
            this.tblRemarks.Controls.Add(this.chkBoxShowAll, 3, 0);
            this.tblRemarks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblRemarks.Location = new System.Drawing.Point(0, 0);
            this.tblRemarks.Name = "tblRemarks";
            this.tblRemarks.RowCount = 1;
            this.tblRemarks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblRemarks.Size = new System.Drawing.Size(486, 29);
            this.tblRemarks.TabIndex = 25;
            // 
            // chkBoxIsCanned
            // 
            this.chkBoxIsCanned.AutoSize = true;
            this.chkBoxIsCanned.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkBoxIsCanned.Location = new System.Drawing.Point(345, 3);
            this.chkBoxIsCanned.Name = "chkBoxIsCanned";
            this.chkBoxIsCanned.Size = new System.Drawing.Size(64, 23);
            this.chkBoxIsCanned.TabIndex = 26;
            this.chkBoxIsCanned.Text = "Canned";
            this.chkBoxIsCanned.UseVisualStyleBackColor = true;
            this.chkBoxIsCanned.CheckedChanged += new System.EventHandler(this.chkBoxIsCanned_CheckedChanged);
            // 
            // chkBoxStick
            // 
            this.chkBoxStick.AutoSize = true;
            this.chkBoxStick.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkBoxStick.Location = new System.Drawing.Point(277, 3);
            this.chkBoxStick.Name = "chkBoxStick";
            this.chkBoxStick.Size = new System.Drawing.Size(62, 23);
            this.chkBoxStick.TabIndex = 27;
            this.chkBoxStick.Text = "Stick it";
            this.chkBoxStick.UseVisualStyleBackColor = true;
            this.chkBoxStick.CheckedChanged += new System.EventHandler(this.chkBoxStick_CheckedChanged);
            // 
            // chkBoxShowAll
            // 
            this.chkBoxShowAll.AutoSize = true;
            this.chkBoxShowAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkBoxShowAll.Location = new System.Drawing.Point(415, 3);
            this.chkBoxShowAll.Name = "chkBoxShowAll";
            this.chkBoxShowAll.Size = new System.Drawing.Size(68, 23);
            this.chkBoxShowAll.TabIndex = 28;
            this.chkBoxShowAll.Text = "Show All";
            this.chkBoxShowAll.UseVisualStyleBackColor = true;
            this.chkBoxShowAll.CheckedChanged += new System.EventHandler(this.chkBoxShowAll_CheckedChanged);
            // 
            // FrmRemark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(486, 29);
            this.ControlBox = false;
            this.Controls.Add(this.tblRemarks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = new System.Drawing.Point(100, 0);
            this.Name = "FrmRemark";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Remarks";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.FrmRemark_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmRemark_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.remarkBindingSource)).EndInit();
            this.tblRemarks.ResumeLayout(false);
            this.tblRemarks.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox comboBoxRemarks;
        private System.Windows.Forms.BindingSource remarkBindingSource;
        private System.Windows.Forms.TableLayoutPanel tblRemarks;
        private System.Windows.Forms.CheckBox chkBoxIsCanned;
        private System.Windows.Forms.CheckBox chkBoxStick;
        private System.Windows.Forms.CheckBox chkBoxShowAll;

       
    }
}