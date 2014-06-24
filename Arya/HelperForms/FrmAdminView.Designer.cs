namespace Arya.HelperForms
{
    partial class FrmAdminView
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
            this.tspSaveChanges = new System.Windows.Forms.ToolStripButton();
            this.tcAdminView = new System.Windows.Forms.TabControl();
            this.tpRoleManager = new System.Windows.Forms.TabPage();
            this.rolesManagerControl1 = new Arya.UserControls.RolesManagerControl();
            this.tpPermissions = new System.Windows.Forms.TabPage();
            this.permissionsManagerControl1 = new Arya.UserControls.PermissionsManagerControl();
            this.tcAdminView.SuspendLayout();
            this.tpRoleManager.SuspendLayout();
            this.tpPermissions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tspSaveChanges
            // 
            this.tspSaveChanges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tspSaveChanges.Image = global::Arya.Properties.Resources.Save;
            this.tspSaveChanges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspSaveChanges.Name = "tspSaveChanges";
            this.tspSaveChanges.Size = new System.Drawing.Size(23, 22);
            this.tspSaveChanges.Text = "SaveChanges";
            // 
            // tcAdminView
            // 
            this.tcAdminView.Controls.Add(this.tpRoleManager);
            this.tcAdminView.Controls.Add(this.tpPermissions);
            this.tcAdminView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcAdminView.Location = new System.Drawing.Point(0, 0);
            this.tcAdminView.Name = "tcAdminView";
            this.tcAdminView.SelectedIndex = 0;
            this.tcAdminView.Size = new System.Drawing.Size(841, 542);
            this.tcAdminView.TabIndex = 0;
            // 
            // tpRoleManager
            // 
            this.tpRoleManager.Controls.Add(this.rolesManagerControl1);
            this.tpRoleManager.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.tpRoleManager.Location = new System.Drawing.Point(4, 22);
            this.tpRoleManager.Name = "tpRoleManager";
            this.tpRoleManager.Padding = new System.Windows.Forms.Padding(3);
            this.tpRoleManager.Size = new System.Drawing.Size(833, 516);
            this.tpRoleManager.TabIndex = 0;
            this.tpRoleManager.Text = "Roles";
            this.tpRoleManager.UseVisualStyleBackColor = true;
            // 
            // rolesManagerControl1
            // 
            this.rolesManagerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rolesManagerControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.rolesManagerControl1.Location = new System.Drawing.Point(3, 3);
            this.rolesManagerControl1.Name = "rolesManagerControl1";
            this.rolesManagerControl1.Size = new System.Drawing.Size(827, 510);
            this.rolesManagerControl1.TabIndex = 0;
            // 
            // tpPermissions
            // 
            this.tpPermissions.Controls.Add(this.permissionsManagerControl1);
            this.tpPermissions.Location = new System.Drawing.Point(4, 22);
            this.tpPermissions.Name = "tpPermissions";
            this.tpPermissions.Padding = new System.Windows.Forms.Padding(3);
            this.tpPermissions.Size = new System.Drawing.Size(833, 516);
            this.tpPermissions.TabIndex = 1;
            this.tpPermissions.Text = "Permissions";
            this.tpPermissions.UseVisualStyleBackColor = true;
            // 
            // permissionsManagerControl1
            // 
            this.permissionsManagerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.permissionsManagerControl1.Location = new System.Drawing.Point(3, 3);
            this.permissionsManagerControl1.Name = "permissionsManagerControl1";
            this.permissionsManagerControl1.Size = new System.Drawing.Size(827, 510);
            this.permissionsManagerControl1.TabIndex = 0;
            // 
            // FrmAdminView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 542);
            this.Controls.Add(this.tcAdminView);
            this.Name = "FrmAdminView";
            this.Text = "Roles & Permissions";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmAdminView_FormClosed);
            this.Load += new System.EventHandler(this.FrmAdminView_Load);
            this.tcAdminView.ResumeLayout(false);
            this.tpRoleManager.ResumeLayout(false);
            this.tpPermissions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripButton tspSaveChanges;
        private System.Windows.Forms.TabControl tcAdminView;
        private System.Windows.Forms.TabPage tpRoleManager;
        private System.Windows.Forms.TabPage tpPermissions;
        private UserControls.RolesManagerControl rolesManagerControl1;
        private UserControls.PermissionsManagerControl permissionsManagerControl1;

    }
}