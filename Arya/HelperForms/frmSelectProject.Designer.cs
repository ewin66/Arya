namespace Arya.HelperForms
{
    partial class FrmSelectProject
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
            this.lblSelectProject = new System.Windows.Forms.Label();
            this.ddlSelectProject = new System.Windows.Forms.ComboBox();
            this.btnSelectProject = new System.Windows.Forms.Button();
            this.lnkAdminView = new System.Windows.Forms.LinkLabel();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblSelectProject
            // 
            this.lblSelectProject.AutoSize = true;
            this.lblSelectProject.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectProject.ForeColor = System.Drawing.Color.Black;
            this.lblSelectProject.Location = new System.Drawing.Point(8, 72);
            this.lblSelectProject.Name = "lblSelectProject";
            this.lblSelectProject.Size = new System.Drawing.Size(43, 13);
            this.lblSelectProject.TabIndex = 8;
            this.lblSelectProject.Text = "Project:";
            // 
            // ddlSelectProject
            // 
            this.ddlSelectProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlSelectProject.FormattingEnabled = true;
            this.ddlSelectProject.Location = new System.Drawing.Point(72, 69);
            this.ddlSelectProject.Name = "ddlSelectProject";
            this.ddlSelectProject.Size = new System.Drawing.Size(322, 21);
            this.ddlSelectProject.TabIndex = 2;
            // 
            // btnSelectProject
            // 
            this.btnSelectProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectProject.AutoSize = true;
            this.btnSelectProject.Location = new System.Drawing.Point(319, 96);
            this.btnSelectProject.Name = "btnSelectProject";
            this.btnSelectProject.Size = new System.Drawing.Size(75, 30);
            this.btnSelectProject.TabIndex = 4;
            this.btnSelectProject.Text = "Select";
            this.btnSelectProject.UseVisualStyleBackColor = true;
            this.btnSelectProject.Click += new System.EventHandler(this.btnSelectProject_Click);
            // 
            // lnkAdminView
            // 
            this.lnkAdminView.AutoSize = true;
            this.lnkAdminView.BackColor = System.Drawing.Color.Transparent;
            this.lnkAdminView.ForeColor = System.Drawing.Color.Black;
            this.lnkAdminView.LinkColor = System.Drawing.Color.Black;
            this.lnkAdminView.Location = new System.Drawing.Point(8, 9);
            this.lnkAdminView.Name = "lnkAdminView";
            this.lnkAdminView.Size = new System.Drawing.Size(101, 13);
            this.lnkAdminView.TabIndex = 5;
            this.lnkAdminView.TabStop = true;
            this.lnkAdminView.Text = "Roles && Permissions";
            this.lnkAdminView.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lnkAdminView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAdminView_LinkClicked);
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoSave.Checked = true;
            this.chkAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSave.ForeColor = System.Drawing.Color.Black;
            this.chkAutoSave.Location = new System.Drawing.Point(11, 107);
            this.chkAutoSave.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(112, 17);
            this.chkAutoSave.TabIndex = 3;
            this.chkAutoSave.Text = "Enable Auto-Save";
            this.chkAutoSave.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(8, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(212, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Password:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(72, 43);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(120, 20);
            this.txtUsername.TabIndex = 0;
            this.txtUsername.Leave += new System.EventHandler(this.TryLogin);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(274, 43);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(120, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Leave += new System.EventHandler(this.TryLogin);
            // 
            // FrmSelectProject
            // 
            this.AcceptButton = this.btnSelectProject;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(406, 140);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAutoSave);
            this.Controls.Add(this.lnkAdminView);
            this.Controls.Add(this.btnSelectProject);
            this.Controls.Add(this.ddlSelectProject);
            this.Controls.Add(this.lblSelectProject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSelectProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select a Project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelectProject;
        private System.Windows.Forms.ComboBox ddlSelectProject;
        private System.Windows.Forms.Button btnSelectProject;
        private System.Windows.Forms.LinkLabel lnkAdminView;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
    }
}