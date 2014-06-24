namespace Arya.UserControls
{
    partial class PermissionsManagerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpPermissionsManager = new System.Windows.Forms.TableLayoutPanel();
            this.lbUserGroups = new System.Windows.Forms.ListBox();
            this.tcAllPermissions = new System.Windows.Forms.TabControl();
            this.tpTaxonomy = new System.Windows.Forms.TabPage();
            this.taxonomyPermissions = new Arya.UserControls.TaxonomyTreeView();
            this.tpAttributes = new System.Windows.Forms.TabPage();
            this.attributeExclusionsControl = new Arya.UserControls.AttributeFarmGridView();
            this.tpUIObjects = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cilbUIObjects = new Arya.CustomControls.CheckedImageListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.tsbtnSave = new System.Windows.Forms.ToolStripButton();
            this.tsbtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tscbProjects = new System.Windows.Forms.ToolStripComboBox();
            this.tscbOptions = new System.Windows.Forms.ToolStripComboBox();
            this.tlpPermissionsManager.SuspendLayout();
            this.tcAllPermissions.SuspendLayout();
            this.tpTaxonomy.SuspendLayout();
            this.tpAttributes.SuspendLayout();
            this.tpUIObjects.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpPermissionsManager
            // 
            this.tlpPermissionsManager.ColumnCount = 2;
            this.tlpPermissionsManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpPermissionsManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPermissionsManager.Controls.Add(this.lbUserGroups, 0, 1);
            this.tlpPermissionsManager.Controls.Add(this.tcAllPermissions, 1, 1);
            this.tlpPermissionsManager.Controls.Add(this.tsMenu, 0, 0);
            this.tlpPermissionsManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPermissionsManager.Location = new System.Drawing.Point(0, 0);
            this.tlpPermissionsManager.Name = "tlpPermissionsManager";
            this.tlpPermissionsManager.RowCount = 2;
            this.tlpPermissionsManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpPermissionsManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPermissionsManager.Size = new System.Drawing.Size(485, 414);
            this.tlpPermissionsManager.TabIndex = 1;
            // 
            // lbUserGroups
            // 
            this.lbUserGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbUserGroups.FormattingEnabled = true;
            this.lbUserGroups.Location = new System.Drawing.Point(3, 33);
            this.lbUserGroups.Name = "lbUserGroups";
            this.lbUserGroups.Size = new System.Drawing.Size(194, 378);
            this.lbUserGroups.TabIndex = 0;
            this.lbUserGroups.SelectedIndexChanged += new System.EventHandler(this.lbUserGroups_SelectedIndexChanged);
            // 
            // tcAllPermissions
            // 
            this.tcAllPermissions.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tcAllPermissions.Controls.Add(this.tpTaxonomy);
            this.tcAllPermissions.Controls.Add(this.tpAttributes);
            this.tcAllPermissions.Controls.Add(this.tpUIObjects);
            this.tcAllPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcAllPermissions.Location = new System.Drawing.Point(203, 33);
            this.tcAllPermissions.Multiline = true;
            this.tcAllPermissions.Name = "tcAllPermissions";
            this.tcAllPermissions.SelectedIndex = 0;
            this.tcAllPermissions.Size = new System.Drawing.Size(279, 378);
            this.tcAllPermissions.TabIndex = 5;
            // 
            // tpTaxonomy
            // 
            this.tpTaxonomy.Controls.Add(this.taxonomyPermissions);
            this.tpTaxonomy.Location = new System.Drawing.Point(4, 4);
            this.tpTaxonomy.Name = "tpTaxonomy";
            this.tpTaxonomy.Padding = new System.Windows.Forms.Padding(3);
            this.tpTaxonomy.Size = new System.Drawing.Size(271, 352);
            this.tpTaxonomy.TabIndex = 0;
            this.tpTaxonomy.Text = "Taxonomies";
            this.tpTaxonomy.UseVisualStyleBackColor = true;
            // 
            // taxonomyPermissions
            // 
            this.taxonomyPermissions.AutoSize = true;
            this.taxonomyPermissions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.taxonomyPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taxonomyPermissions.EnableCheckBoxes = false;
            this.taxonomyPermissions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.taxonomyPermissions.Location = new System.Drawing.Point(3, 3);
            this.taxonomyPermissions.Name = "taxonomyPermissions";
            this.taxonomyPermissions.OpenNodeOptionEnabled = true;
            this.taxonomyPermissions.Size = new System.Drawing.Size(265, 346);
            this.taxonomyPermissions.TabIndex = 0;
            // 
            // tpAttributes
            // 
            this.tpAttributes.Controls.Add(this.attributeExclusionsControl);
            this.tpAttributes.Location = new System.Drawing.Point(4, 4);
            this.tpAttributes.Name = "tpAttributes";
            this.tpAttributes.Padding = new System.Windows.Forms.Padding(3);
            this.tpAttributes.Size = new System.Drawing.Size(271, 352);
            this.tpAttributes.TabIndex = 1;
            this.tpAttributes.Text = "Attributes";
            this.tpAttributes.UseVisualStyleBackColor = true;
            // 
            // attributeExclusionsControl
            // 
            this.attributeExclusionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributeExclusionsControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.attributeExclusionsControl.IsPermissionsUIEnabled = false;
            this.attributeExclusionsControl.Location = new System.Drawing.Point(3, 3);
            this.attributeExclusionsControl.Name = "attributeExclusionsControl";
            this.attributeExclusionsControl.Size = new System.Drawing.Size(265, 346);
            this.attributeExclusionsControl.TabIndex = 0;
            // 
            // tpUIObjects
            // 
            this.tpUIObjects.Controls.Add(this.tableLayoutPanel2);
            this.tpUIObjects.Location = new System.Drawing.Point(4, 4);
            this.tpUIObjects.Name = "tpUIObjects";
            this.tpUIObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tpUIObjects.Size = new System.Drawing.Size(271, 352);
            this.tpUIObjects.TabIndex = 2;
            this.tpUIObjects.Text = "UI Objects";
            this.tpUIObjects.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.cilbUIObjects, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(265, 346);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // cilbUIObjects
            // 
            this.cilbUIObjects.CheckedImage = global::Arya.Properties.Resources.bullet_red;
            this.cilbUIObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cilbUIObjects.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cilbUIObjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cilbUIObjects.FormattingEnabled = true;
            this.cilbUIObjects.Location = new System.Drawing.Point(3, 23);
            this.cilbUIObjects.Name = "cilbUIObjects";
            this.cilbUIObjects.Size = new System.Drawing.Size(259, 320);
            this.cilbUIObjects.TabIndex = 0;
            this.cilbUIObjects.UnCheckedImage = global::Arya.Properties.Resources.bullet_green;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(259, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "UI Object Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsMenu
            // 
            this.tlpPermissionsManager.SetColumnSpan(this.tsMenu, 2);
            this.tsMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnSave,
            this.tsbtnRefresh,
            this.tscbProjects,
            this.tscbOptions});
            this.tsMenu.Location = new System.Drawing.Point(3, 2);
            this.tsMenu.Margin = new System.Windows.Forms.Padding(3, 2, 5, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMenu.Size = new System.Drawing.Size(477, 25);
            this.tsMenu.TabIndex = 7;
            this.tsMenu.Text = "toolStrip1";
            // 
            // tsbtnSave
            // 
            this.tsbtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSave.Image = global::Arya.Properties.Resources.Save;
            this.tsbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSave.Name = "tsbtnSave";
            this.tsbtnSave.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.tsbtnSave.Size = new System.Drawing.Size(23, 22);
            this.tsbtnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsbtnSave.ToolTipText = "Save";
            this.tsbtnSave.Click += new System.EventHandler(this.tsbtnSave_Click);
            // 
            // tsbtnRefresh
            // 
            this.tsbtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRefresh.Image = global::Arya.Properties.Resources.Refresh;
            this.tsbtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRefresh.Name = "tsbtnRefresh";
            this.tsbtnRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbtnRefresh.Text = "toolStripButton1";
            this.tsbtnRefresh.Click += new System.EventHandler(this.tsbtnRefresh_Click);
            // 
            // tscbProjects
            // 
            this.tscbProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbProjects.Name = "tscbProjects";
            this.tscbProjects.Size = new System.Drawing.Size(146, 25);
            this.tscbProjects.ToolTipText = "Select Project";
            this.tscbProjects.SelectedIndexChanged += new System.EventHandler(this.tscbProjects_SelectedIndexChanged);
            // 
            // tscbOptions
            // 
            this.tscbOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tscbOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbOptions.Items.AddRange(new object[] {
            "Select",
            "None",
            "All",
            "Invert",
            "Cancel"});
            this.tscbOptions.Name = "tscbOptions";
            this.tscbOptions.Size = new System.Drawing.Size(75, 25);
            this.tscbOptions.ToolTipText = "Select Actions";
            this.tscbOptions.SelectedIndexChanged += new System.EventHandler(this.tscbOptions_SelectedIndexChanged);
            // 
            // PermissionsManagerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpPermissionsManager);
            this.Name = "PermissionsManagerControl";
            this.Size = new System.Drawing.Size(485, 414);
            this.tlpPermissionsManager.ResumeLayout(false);
            this.tlpPermissionsManager.PerformLayout();
            this.tcAllPermissions.ResumeLayout(false);
            this.tpTaxonomy.ResumeLayout(false);
            this.tpTaxonomy.PerformLayout();
            this.tpAttributes.ResumeLayout(false);
            this.tpUIObjects.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpPermissionsManager;
        private System.Windows.Forms.ListBox lbUserGroups;
        private System.Windows.Forms.TabControl tcAllPermissions;
        private System.Windows.Forms.TabPage tpTaxonomy;
        private TaxonomyTreeView taxonomyPermissions;
        private System.Windows.Forms.TabPage tpAttributes;
        private AttributeFarmGridView attributeExclusionsControl;
        private System.Windows.Forms.TabPage tpUIObjects;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private CustomControls.CheckedImageListBox cilbUIObjects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip tsMenu;
        private System.Windows.Forms.ToolStripButton tsbtnSave;
        private System.Windows.Forms.ToolStripComboBox tscbProjects;
        private System.Windows.Forms.ToolStripButton tsbtnRefresh;
        private System.Windows.Forms.ToolStripComboBox tscbOptions;
    }
}
