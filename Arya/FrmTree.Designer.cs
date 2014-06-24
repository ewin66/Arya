using Arya.UserControls;

namespace Arya
{
    using System.Drawing;
    using System.Windows.Forms;
    using Properties;

    partial class FrmTree
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
            this._pnlSpecialButtons = new System.Windows.Forms.Panel();
            this._tableSpecialButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.taxonomyMenuStrip = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadTaxonomyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.skuViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schemaViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.queryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeFarmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.characterMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitsOfMeasureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skuLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.logoutExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToOneTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orderNodesByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphaNumericToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showEnrichmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRemarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skuGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.workflowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.checkpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSpellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tipsAndTricksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectMetaAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectCustomDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taxonomyTree = new Arya.UserControls.TaxonomyTreeView();
            this._pnlSpecialButtons.SuspendLayout();
            this._tableSpecialButtons.SuspendLayout();
            this.taxonomyMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pnlSpecialButtons
            // 
            this._pnlSpecialButtons.Controls.Add(this._tableSpecialButtons);
            this._pnlSpecialButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pnlSpecialButtons.Location = new System.Drawing.Point(0, 414);
            this._pnlSpecialButtons.Name = "_pnlSpecialButtons";
            this._pnlSpecialButtons.Size = new System.Drawing.Size(304, 30);
            this._pnlSpecialButtons.TabIndex = 1;
            this._pnlSpecialButtons.Visible = false;
            // 
            // _tableSpecialButtons
            // 
            this._tableSpecialButtons.ColumnCount = 3;
            this._tableSpecialButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableSpecialButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableSpecialButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableSpecialButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableSpecialButtons.Controls.Add(this.btnAccept, 1, 0);
            this._tableSpecialButtons.Controls.Add(this.btnCancel, 2, 0);
            this._tableSpecialButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableSpecialButtons.Location = new System.Drawing.Point(0, 0);
            this._tableSpecialButtons.Name = "_tableSpecialButtons";
            this._tableSpecialButtons.RowCount = 1;
            this._tableSpecialButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableSpecialButtons.Size = new System.Drawing.Size(304, 30);
            this._tableSpecialButtons.TabIndex = 4;
            // 
            // btnAccept
            // 
            this.btnAccept.AutoSize = true;
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(145, 3);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 27);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "OK";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(226, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // taxonomyMenuStrip
            // 
            this.taxonomyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.saveNowToolStripMenuItem});
            this.taxonomyMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.taxonomyMenuStrip.Name = "taxonomyMenuStrip";
            this.taxonomyMenuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.taxonomyMenuStrip.Size = new System.Drawing.Size(304, 24);
            this.taxonomyMenuStrip.TabIndex = 3;
            this.taxonomyMenuStrip.Text = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadTaxonomyToolStripMenuItem,
            this.toolStripSeparator1,
            this.skuViewToolStripMenuItem,
            this.schemaViewToolStripMenuItem,
            this.attributeViewToolStripMenuItem,
            this.toolStripSeparator5,
            this.queryToolStripMenuItem1,
            this.attributeFarmToolStripMenuItem,
            this.characterMapToolStripMenuItem,
            this.unitsOfMeasureToolStripMenuItem,
            this.skuLinksToolStripMenuItem,
            this.userPreferencesToolStripMenuItem,
            this.toolStripSeparator4,
            this.logoutExitToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // reloadTaxonomyToolStripMenuItem
            // 
            this.reloadTaxonomyToolStripMenuItem.Name = "reloadTaxonomyToolStripMenuItem";
            this.reloadTaxonomyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reloadTaxonomyToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.reloadTaxonomyToolStripMenuItem.Text = "&Reload Taxonomy";
            this.reloadTaxonomyToolStripMenuItem.Click += new System.EventHandler(this.reloadTaxonomyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
            // 
            // skuViewToolStripMenuItem
            // 
            this.skuViewToolStripMenuItem.Name = "skuViewToolStripMenuItem";
            this.skuViewToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.skuViewToolStripMenuItem.Text = "S&ku View";
            this.skuViewToolStripMenuItem.Click += new System.EventHandler(this.skuViewToolStripMenuItem_Click);
            // 
            // schemaViewToolStripMenuItem
            // 
            this.schemaViewToolStripMenuItem.Name = "schemaViewToolStripMenuItem";
            this.schemaViewToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.schemaViewToolStripMenuItem.Text = "S&chema View";
            this.schemaViewToolStripMenuItem.Click += new System.EventHandler(this.schemaViewToolStripMenuItem_Click);
            // 
            // attributeViewToolStripMenuItem
            // 
            this.attributeViewToolStripMenuItem.Name = "attributeViewToolStripMenuItem";
            this.attributeViewToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.attributeViewToolStripMenuItem.Text = "&Attribute View";
            this.attributeViewToolStripMenuItem.Click += new System.EventHandler(this.attributeViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(207, 6);
            // 
            // queryToolStripMenuItem1
            // 
            this.queryToolStripMenuItem1.Name = "queryToolStripMenuItem1";
            this.queryToolStripMenuItem1.Size = new System.Drawing.Size(210, 22);
            this.queryToolStripMenuItem1.Text = "&Query";
            this.queryToolStripMenuItem1.Click += new System.EventHandler(this.queryToolStripMenuItem_Click);
            // 
            // attributeFarmToolStripMenuItem
            // 
            this.attributeFarmToolStripMenuItem.Name = "attributeFarmToolStripMenuItem";
            this.attributeFarmToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.attributeFarmToolStripMenuItem.Text = "Attribute &Farm";
            this.attributeFarmToolStripMenuItem.Click += new System.EventHandler(this.attributeFarmToolStripMenuItem_Click);
            // 
            // characterMapToolStripMenuItem
            // 
            this.characterMapToolStripMenuItem.Name = "characterMapToolStripMenuItem";
            this.characterMapToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.characterMapToolStripMenuItem.Text = "Character &Map";
            this.characterMapToolStripMenuItem.Click += new System.EventHandler(this.characterMapToolStripMenuItem_Click);
            // 
            // unitsOfMeasureToolStripMenuItem
            // 
            this.unitsOfMeasureToolStripMenuItem.Name = "unitsOfMeasureToolStripMenuItem";
            this.unitsOfMeasureToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.unitsOfMeasureToolStripMenuItem.Text = "&Units of Measure";
            this.unitsOfMeasureToolStripMenuItem.Click += new System.EventHandler(this.unitsOfMeasureToolStripMenuItem_Click);
            // 
            // skuLinksToolStripMenuItem
            // 
            this.skuLinksToolStripMenuItem.Name = "skuLinksToolStripMenuItem";
            this.skuLinksToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.skuLinksToolStripMenuItem.Text = "Sku &Links";
            this.skuLinksToolStripMenuItem.Click += new System.EventHandler(this.skuLinksToolStripMenuItem_Click);
            // 
            // userPreferencesToolStripMenuItem
            // 
            this.userPreferencesToolStripMenuItem.Name = "userPreferencesToolStripMenuItem";
            this.userPreferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.userPreferencesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.userPreferencesToolStripMenuItem.Text = "User &Preferences";
            this.userPreferencesToolStripMenuItem.Click += new System.EventHandler(this.UserPreferences_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(207, 6);
            // 
            // logoutExitToolStripMenuItem
            // 
            this.logoutExitToolStripMenuItem.Name = "logoutExitToolStripMenuItem";
            this.logoutExitToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.logoutExitToolStripMenuItem.Text = "E&xit";
            this.logoutExitToolStripMenuItem.Click += new System.EventHandler(this.logoutExitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToOneTabToolStripMenuItem,
            this.includeChildrenToolStripMenuItem,
            this.autoSaveToolStripMenuItem,
            this.orderNodesByToolStripMenuItem,
            this.showEnrichmentsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // loadToOneTabToolStripMenuItem
            // 
            this.loadToOneTabToolStripMenuItem.CheckOnClick = true;
            this.loadToOneTabToolStripMenuItem.Name = "loadToOneTabToolStripMenuItem";
            this.loadToOneTabToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.loadToOneTabToolStripMenuItem.Text = "Load to &one tab";
            this.loadToOneTabToolStripMenuItem.Click += new System.EventHandler(this.loadToOneTabToolStripMenuItem_Click);
            // 
            // includeChildrenToolStripMenuItem
            // 
            this.includeChildrenToolStripMenuItem.CheckOnClick = true;
            this.includeChildrenToolStripMenuItem.Name = "includeChildrenToolStripMenuItem";
            this.includeChildrenToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.includeChildrenToolStripMenuItem.Text = "Include &children";
            this.includeChildrenToolStripMenuItem.Click += new System.EventHandler(this.includeChildrenToolStripMenuItem_Click);
            // 
            // autoSaveToolStripMenuItem
            // 
            this.autoSaveToolStripMenuItem.CheckOnClick = true;
            this.autoSaveToolStripMenuItem.Name = "autoSaveToolStripMenuItem";
            this.autoSaveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.autoSaveToolStripMenuItem.Text = "&Auto save";
            this.autoSaveToolStripMenuItem.Click += new System.EventHandler(this.autoSaveToolStripMenuItem_Click);
            // 
            // orderNodesByToolStripMenuItem
            // 
            this.orderNodesByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nodeTypeToolStripMenuItem,
            this.alphaNumericToolStripMenuItem});
            this.orderNodesByToolStripMenuItem.Name = "orderNodesByToolStripMenuItem";
            this.orderNodesByToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.orderNodesByToolStripMenuItem.Text = "&Order nodes by";
            // 
            // nodeTypeToolStripMenuItem
            // 
            this.nodeTypeToolStripMenuItem.Name = "nodeTypeToolStripMenuItem";
            this.nodeTypeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.nodeTypeToolStripMenuItem.Text = "Node &Type";
            this.nodeTypeToolStripMenuItem.Click += new System.EventHandler(this.nodeTypeToolStripMenuItem_Click);
            // 
            // alphaNumericToolStripMenuItem
            // 
            this.alphaNumericToolStripMenuItem.Name = "alphaNumericToolStripMenuItem";
            this.alphaNumericToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.alphaNumericToolStripMenuItem.Text = "&AlphaNumeric";
            this.alphaNumericToolStripMenuItem.Click += new System.EventHandler(this.alphaNumericToolStripMenuItem_Click);
            // 
            // showEnrichmentsToolStripMenuItem
            // 
            this.showEnrichmentsToolStripMenuItem.CheckOnClick = true;
            this.showEnrichmentsToolStripMenuItem.Name = "showEnrichmentsToolStripMenuItem";
            this.showEnrichmentsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showEnrichmentsToolStripMenuItem.Text = "Show Enrichments";
            this.showEnrichmentsToolStripMenuItem.Click += new System.EventHandler(this.showEnrichmentsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notesToolStripMenuItem,
            this.showRemarksToolStripMenuItem,
            this.skuGroupsToolStripMenuItem,
            this.workflowToolStripMenuItem,
            this.toolStripSeparator2,
            this.checkpointsToolStripMenuItem,
            this.importToolStripMenuItem,            
            this.exportToolStripMenuItem,
            this.tipsAndTricksToolStripMenuItem,
            this.projectMetaAttributesToolStripMenuItem,
            this.projectCustomDictionaryToolStripMenuItem,
            this.checkSpellingToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // notesToolStripMenuItem
            // 
            this.notesToolStripMenuItem.Name = "notesToolStripMenuItem";
            this.notesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemtilde)));
            this.notesToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.notesToolStripMenuItem.Text = "&Notes";
            this.notesToolStripMenuItem.Click += new System.EventHandler(this.notesToolStripMenuItem_Click);
            // 
            // showRemarksToolStripMenuItem
            // 
            this.showRemarksToolStripMenuItem.CheckOnClick = true;
            this.showRemarksToolStripMenuItem.Name = "showRemarksToolStripMenuItem";
            this.showRemarksToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showRemarksToolStripMenuItem.Text = "Show &Remarks";
            this.showRemarksToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showRemarksToolStripMenuItem_CheckedChanged);
            this.showRemarksToolStripMenuItem.Click += new System.EventHandler(this.showRemarksToolStripMenuItem_Click);
            // 
            // skuGroupsToolStripMenuItem
            // 
            this.skuGroupsToolStripMenuItem.Name = "skuGroupsToolStripMenuItem";
            this.skuGroupsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.skuGroupsToolStripMenuItem.Text = "Sku &Groups";
            this.skuGroupsToolStripMenuItem.Click += new System.EventHandler(this.skuGroupsToolStripMenuItem_Click);
            // 
            // workflowToolStripMenuItem
            // 
            this.workflowToolStripMenuItem.Name = "workflowToolStripMenuItem";
            this.workflowToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.workflowToolStripMenuItem.Text = "&Workflow";
            this.workflowToolStripMenuItem.Click += new System.EventHandler(this.workflowToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
            // 
            // checkpointsToolStripMenuItem
            // 
            this.checkpointsToolStripMenuItem.Name = "checkpointsToolStripMenuItem";
            this.checkpointsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.checkpointsToolStripMenuItem.Text = "Check&points";
            this.checkpointsToolStripMenuItem.Click += new System.EventHandler(this.checkpointsToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.importToolStripMenuItem.Text = "&Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // checkSpellingToolStripMenuItem
            // 
            this.checkSpellingToolStripMenuItem.Name = "checkSpellingToolStripMenuItem";
            this.checkSpellingToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.checkSpellingToolStripMenuItem.Text = "&Check Spelling";
            this.checkSpellingToolStripMenuItem.Click += new System.EventHandler(this.checkSpellingToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // tipsAndTricksToolStripMenuItem
            // 
            this.tipsAndTricksToolStripMenuItem.Name = "tipsAndTricksToolStripMenuItem";
            this.tipsAndTricksToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.tipsAndTricksToolStripMenuItem.Text = "Tips && Tricks";
            // 
            // projectMetaAttributesToolStripMenuItem
            // 
            this.projectMetaAttributesToolStripMenuItem.Name = "projectMetaAttributesToolStripMenuItem";
            this.projectMetaAttributesToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.projectMetaAttributesToolStripMenuItem.Text = "Project Meta-Attributes";
            this.projectMetaAttributesToolStripMenuItem.Click += new System.EventHandler(this.projectMetaAttributesToolStripMenuItem_Click);
            //
            // projectCustomDicktionaryToolStripMenuItem
            // 
            this.projectCustomDictionaryToolStripMenuItem.Name = "projectCustomDictionaryToolStripMenuItem";
            this.projectCustomDictionaryToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.projectCustomDictionaryToolStripMenuItem.Text = "Project Custom Dictionary";
            this.projectCustomDictionaryToolStripMenuItem.Click += new System.EventHandler(this.projectCustomDicktionaryToolStripMenuItem_Click);
            // 
            // saveNowToolStripMenuItem
            // 
            this.saveNowToolStripMenuItem.Name = "saveNowToolStripMenuItem";
            this.saveNowToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.saveNowToolStripMenuItem.Text = "&Save Now!";
            this.saveNowToolStripMenuItem.Click += new System.EventHandler(this.saveNowToolStripMenuItem_Click);
            // 
            // taxonomyTree
            // 
            this.taxonomyTree.AutoSize = true;
            this.taxonomyTree.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.taxonomyTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taxonomyTree.EnableCheckBoxes = false;
            this.taxonomyTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.taxonomyTree.Location = new System.Drawing.Point(0, 24);
            this.taxonomyTree.Name = "taxonomyTree";
            this.taxonomyTree.OpenNodeOptionEnabled = true;
            this.taxonomyTree.Padding = new System.Windows.Forms.Padding(3);
            this.taxonomyTree.ShowEnrichments = true;
            this.taxonomyTree.Size = new System.Drawing.Size(304, 390);
            this.taxonomyTree.TabIndex = 0;
            // 
            // FrmTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 444);
            this.Controls.Add(this.taxonomyTree);
            this.Controls.Add(this._pnlSpecialButtons);
            this.Controls.Add(this.taxonomyMenuStrip);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 315);
            this.Name = "FrmTree";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Taxonomy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTree_FormClosing);
            this.Load += new System.EventHandler(this.FrmTree_Load);
            this.LocationChanged += new System.EventHandler(this.FrmTree_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmTree_KeyDown);
            this._pnlSpecialButtons.ResumeLayout(false);
            this._tableSpecialButtons.ResumeLayout(false);
            this._tableSpecialButtons.PerformLayout();
            this.taxonomyMenuStrip.ResumeLayout(false);
            this.taxonomyMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Panel _pnlSpecialButtons;
        internal TaxonomyTreeView taxonomyTree;
        private System.Windows.Forms.TableLayoutPanel _tableSpecialButtons;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private MenuStrip taxonomyMenuStrip;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem skuViewToolStripMenuItem;
        private ToolStripMenuItem attributeViewToolStripMenuItem;
        private ToolStripMenuItem queryToolStripMenuItem1;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem loadToOneTabToolStripMenuItem;
        private ToolStripMenuItem includeChildrenToolStripMenuItem;
        private ToolStripMenuItem autoSaveToolStripMenuItem;
        private ToolStripMenuItem saveNowToolStripMenuItem;
        private ToolStripMenuItem reloadTaxonomyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem schemaViewToolStripMenuItem;
        private ToolStripMenuItem characterMapToolStripMenuItem;
        private ToolStripMenuItem checkpointsToolStripMenuItem;
        private ToolStripMenuItem skuGroupsToolStripMenuItem;
        private ToolStripMenuItem attributeFarmToolStripMenuItem;
        private ToolStripMenuItem skuLinksToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem orderNodesByToolStripMenuItem;
        private ToolStripMenuItem nodeTypeToolStripMenuItem;
        private ToolStripMenuItem alphaNumericToolStripMenuItem;
        private ToolStripMenuItem notesToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem checkSpellingToolStripMenuItem;
        private ToolStripMenuItem logoutExitToolStripMenuItem;
        private ToolStripMenuItem workflowToolStripMenuItem;
        public ToolStripMenuItem showRemarksToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem tipsAndTricksToolStripMenuItem;
        private ToolStripMenuItem userPreferencesToolStripMenuItem;
        private ToolStripMenuItem unitsOfMeasureToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem showEnrichmentsToolStripMenuItem;
        private ToolStripMenuItem projectCustomDictionaryToolStripMenuItem;
        private ToolStripMenuItem projectMetaAttributesToolStripMenuItem;
    }
}