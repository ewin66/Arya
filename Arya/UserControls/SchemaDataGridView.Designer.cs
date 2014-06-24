using System.Collections.Generic;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.GUI.UserControls;
using Arya.HelperClasses;
using Arya.CustomControls;

namespace Arya.UserControls
{
    partial class SchemaDataGridView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.auditTrailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GreatGreatGrandparentNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GreatGrandparentNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrandparentNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ParentNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAdditionalNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedNodesFromViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showFillRatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFillRatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloneToAllNeighborsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloneCustomOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cloneCurrentValueOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloneAllSchematiAndLovToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unmapSelectedAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.populateSampleValues1 = new System.Windows.Forms.ToolStripMenuItem();
            this.autoorderRanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ranksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculatedAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSpellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSpellingSchemaViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSpellingSelectedRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.flowRibbon = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCurrentSchematus = new System.Windows.Forms.Label();
            this.ddCurrentSchematus = new System.Windows.Forms.ComboBox();
            this.cbAutoSuggestOptions = new System.Windows.Forms.CheckBox();
            this.RefreshGridViewTimer = new System.Windows.Forms.Timer(this.components);
            this.sdgvAuditTrail = new System.Windows.Forms.DataGridView();
            this.splitterAuditTrail = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.sdgv = new Arya.Framework.GUI.UserControls.CustomDataGridView();
            this.mainMenuStrip.SuspendLayout();
            this.flowRibbon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sdgvAuditTrail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sdgv)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.nodeToolStripMenuItem,
            this.dataToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.mainMenuStrip.Size = new System.Drawing.Size(233, 24);
            this.mainMenuStrip.TabIndex = 1;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInNewTabToolStripMenuItem,
            this.auditTrailToolStripMenuItem,
            this.saveAllChangesToolStripMenuItem,
            this.reloadTabToolStripMenuItem,
            this.closeTabToolStripMenuItem,
            this.notesToolStripMenuItem,
            this.userPreferencesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // openInNewTabToolStripMenuItem
            // 
            this.openInNewTabToolStripMenuItem.Name = "openInNewTabToolStripMenuItem";
            this.openInNewTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.openInNewTabToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.openInNewTabToolStripMenuItem.Text = "Open in &new tab";
            this.openInNewTabToolStripMenuItem.Click += new System.EventHandler(this.openInNewTabToolStripMenuItem_Click);
            // 
            // auditTrailToolStripMenuItem
            // 
            this.auditTrailToolStripMenuItem.CheckOnClick = true;
            this.auditTrailToolStripMenuItem.Image = global::Arya.Properties.Resources.BookClosed;
            this.auditTrailToolStripMenuItem.Name = "auditTrailToolStripMenuItem";
            this.auditTrailToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.auditTrailToolStripMenuItem.Text = "&Audit Trail";
            this.auditTrailToolStripMenuItem.Click += new System.EventHandler(this.auditTrailToolStripMenuItem_Click);
            // 
            // saveAllChangesToolStripMenuItem
            // 
            this.saveAllChangesToolStripMenuItem.Image = global::Arya.Properties.Resources.Save;
            this.saveAllChangesToolStripMenuItem.Name = "saveAllChangesToolStripMenuItem";
            this.saveAllChangesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveAllChangesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.saveAllChangesToolStripMenuItem.Text = "&Save all changes";
            this.saveAllChangesToolStripMenuItem.Click += new System.EventHandler(this.saveAllChangesToolStripMenuItem_Click);
            // 
            // reloadTabToolStripMenuItem
            // 
            this.reloadTabToolStripMenuItem.Image = global::Arya.Properties.Resources.Refresh;
            this.reloadTabToolStripMenuItem.Name = "reloadTabToolStripMenuItem";
            this.reloadTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reloadTabToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.reloadTabToolStripMenuItem.Text = "&Reload current tab";
            this.reloadTabToolStripMenuItem.Click += new System.EventHandler(this.reloadTabToolStripMenuItem_Click);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.closeTabToolStripMenuItem.Text = "&Close tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.closeTabToolStripMenuItem_Click);
            // 
            // notesToolStripMenuItem
            // 
            this.notesToolStripMenuItem.Name = "notesToolStripMenuItem";
            this.notesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemtilde)));
            this.notesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.notesToolStripMenuItem.Text = "Notes";
            this.notesToolStripMenuItem.Click += new System.EventHandler(this.notesToolStripMenuItem_Click);
            // 
            // userPreferencesToolStripMenuItem
            // 
            this.userPreferencesToolStripMenuItem.Name = "userPreferencesToolStripMenuItem";
            this.userPreferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.userPreferencesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.userPreferencesToolStripMenuItem.Text = "User Preferences";
            this.userPreferencesToolStripMenuItem.Click += new System.EventHandler(this.attributePreferencesPropertiesToolStripMenuItem_Click);
            // 
            // nodeToolStripMenuItem
            // 
            this.nodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterNodesToolStripMenuItem,
            this.viewAdditionalNodesToolStripMenuItem,
            this.removeSelectedNodesFromViewToolStripMenuItem,
            this.toolStripSeparator1,
            this.showFillRatesToolStripMenuItem,
            this.customFillRatesToolStripMenuItem});
            this.nodeToolStripMenuItem.Name = "nodeToolStripMenuItem";
            this.nodeToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.nodeToolStripMenuItem.Text = "&Node";
            // 
            // filterNodesToolStripMenuItem
            // 
            this.filterNodesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GreatGreatGrandparentNodeToolStripMenuItem,
            this.GreatGrandparentNodeToolStripMenuItem,
            this.GrandparentNodeToolStripMenuItem,
            this.ParentNodeToolStripMenuItem});
            this.filterNodesToolStripMenuItem.Name = "filterNodesToolStripMenuItem";
            this.filterNodesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.filterNodesToolStripMenuItem.Text = "&Filter";
            // 
            // GreatGreatGrandparentNodeToolStripMenuItem
            // 
            this.GreatGreatGrandparentNodeToolStripMenuItem.Name = "GreatGreatGrandparentNodeToolStripMenuItem";
            this.GreatGreatGrandparentNodeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.GreatGreatGrandparentNodeToolStripMenuItem.Text = "GreatGreatGrandparentNode";
            this.GreatGreatGrandparentNodeToolStripMenuItem.Click += new System.EventHandler(this.level1ToolStripMenuItem_Click);
            // 
            // GreatGrandparentNodeToolStripMenuItem
            // 
            this.GreatGrandparentNodeToolStripMenuItem.Name = "GreatGrandparentNodeToolStripMenuItem";
            this.GreatGrandparentNodeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.GreatGrandparentNodeToolStripMenuItem.Text = "GreatGrandparentNode";
            this.GreatGrandparentNodeToolStripMenuItem.Click += new System.EventHandler(this.level2ToolStripMenuItem_Click);
            // 
            // GrandparentNodeToolStripMenuItem
            // 
            this.GrandparentNodeToolStripMenuItem.Name = "GrandparentNodeToolStripMenuItem";
            this.GrandparentNodeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.GrandparentNodeToolStripMenuItem.Text = "GrandparentNode";
            this.GrandparentNodeToolStripMenuItem.Click += new System.EventHandler(this.level3ToolStripMenuItem_Click);
            // 
            // ParentNodeToolStripMenuItem
            // 
            this.ParentNodeToolStripMenuItem.Name = "ParentNodeToolStripMenuItem";
            this.ParentNodeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.ParentNodeToolStripMenuItem.Text = "ParentNode";
            this.ParentNodeToolStripMenuItem.Click += new System.EventHandler(this.levelLeafNodeToolStripMenuItem_Click);
            // 
            // viewAdditionalNodesToolStripMenuItem
            // 
            this.viewAdditionalNodesToolStripMenuItem.Name = "viewAdditionalNodesToolStripMenuItem";
            this.viewAdditionalNodesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.viewAdditionalNodesToolStripMenuItem.Text = "&Add to view";
            this.viewAdditionalNodesToolStripMenuItem.Click += new System.EventHandler(this.viewAdditionalNodesToolStripMenuItem_Click);
            // 
            // removeSelectedNodesFromViewToolStripMenuItem
            // 
            this.removeSelectedNodesFromViewToolStripMenuItem.Name = "removeSelectedNodesFromViewToolStripMenuItem";
            this.removeSelectedNodesFromViewToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.removeSelectedNodesFromViewToolStripMenuItem.Text = "&Remove from view";
            this.removeSelectedNodesFromViewToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedNodesFromViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(170, 6);
            // 
            // showFillRatesToolStripMenuItem
            // 
            this.showFillRatesToolStripMenuItem.CheckOnClick = true;
            this.showFillRatesToolStripMenuItem.Name = "showFillRatesToolStripMenuItem";
            this.showFillRatesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.showFillRatesToolStripMenuItem.Text = "Show Fill &Rates";
            this.showFillRatesToolStripMenuItem.Click += new System.EventHandler(this.calculateFillRatesToolStripMenuItem_Click);
            // 
            // customFillRatesToolStripMenuItem
            // 
            this.customFillRatesToolStripMenuItem.Name = "customFillRatesToolStripMenuItem";
            this.customFillRatesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.customFillRatesToolStripMenuItem.Text = "C&ustom Fill Rates";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cloneToAllNeighborsToolStripMenuItem,
            this.unmapSelectedAttributesToolStripMenuItem,
            this.refreshSchemaToolStripMenuItem,
            this.populateSampleValues1,
            this.autoorderRanksToolStripMenuItem,
            this.ranksToolStripMenuItem,
            this.calculatedAttributeToolStripMenuItem,
            this.checkSpellingToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.dataToolStripMenuItem.Text = "&Data";
            // 
            // cloneToAllNeighborsToolStripMenuItem
            // 
            this.cloneToAllNeighborsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cloneCustomOptionsToolStripMenuItem,
            this.toolStripSeparator2,
            this.cloneCurrentValueOnlyToolStripMenuItem,
            this.cloneAllSchematiAndLovToolStripMenuItem,
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem});
            this.cloneToAllNeighborsToolStripMenuItem.Name = "cloneToAllNeighborsToolStripMenuItem";
            this.cloneToAllNeighborsToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.cloneToAllNeighborsToolStripMenuItem.Text = "&Clone to all neighbors";
            // 
            // cloneCustomOptionsToolStripMenuItem
            // 
            this.cloneCustomOptionsToolStripMenuItem.Name = "cloneCustomOptionsToolStripMenuItem";
            this.cloneCustomOptionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.cloneCustomOptionsToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
            this.cloneCustomOptionsToolStripMenuItem.Text = "Cus&tom...";
            this.cloneCustomOptionsToolStripMenuItem.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(331, 6);
            // 
            // cloneCurrentValueOnlyToolStripMenuItem
            // 
            this.cloneCurrentValueOnlyToolStripMenuItem.Name = "cloneCurrentValueOnlyToolStripMenuItem";
            this.cloneCurrentValueOnlyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.cloneCurrentValueOnlyToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
            this.cloneCurrentValueOnlyToolStripMenuItem.Text = "&Current Meta-attribute Only";
            this.cloneCurrentValueOnlyToolStripMenuItem.Click += new System.EventHandler(this.currentValueOnlyToolStripMenuItem_Click);
            // 
            // cloneAllSchematiAndLovToolStripMenuItem
            // 
            this.cloneAllSchematiAndLovToolStripMenuItem.Name = "cloneAllSchematiAndLovToolStripMenuItem";
            this.cloneAllSchematiAndLovToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.cloneAllSchematiAndLovToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
            this.cloneAllSchematiAndLovToolStripMenuItem.Text = "&All Meta-attributes && Their Values";
            this.cloneAllSchematiAndLovToolStripMenuItem.Click += new System.EventHandler(this.allAttributeSchematiToolStripMenuItem_Click);
            // 
            // cloneAllSchematiWithEmptyranksToolStripMenuItem
            // 
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem.Name = "cloneAllSchematiWithEmptyranksToolStripMenuItem";
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem.Text = "All Meta-attributes && Values Except Ranks";
            this.cloneAllSchematiWithEmptyranksToolStripMenuItem.Click += new System.EventHandler(this.allSchematiWithEmptyranksToolStripMenuItem_Click);
            // 
            // unmapSelectedAttributesToolStripMenuItem
            // 
            this.unmapSelectedAttributesToolStripMenuItem.Name = "unmapSelectedAttributesToolStripMenuItem";
            this.unmapSelectedAttributesToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.unmapSelectedAttributesToolStripMenuItem.Text = "&Unmap Selected Attribute(s)";
            this.unmapSelectedAttributesToolStripMenuItem.Click += new System.EventHandler(this.unmapSelectedAttributesToolStripMenuItem_Click);
            // 
            // refreshSchemaToolStripMenuItem
            // 
            this.refreshSchemaToolStripMenuItem.Name = "refreshSchemaToolStripMenuItem";
            this.refreshSchemaToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.refreshSchemaToolStripMenuItem.Text = "&Refresh Attributes from SKU Data";
            this.refreshSchemaToolStripMenuItem.Click += new System.EventHandler(this.RefreshAttributesFromSkuDataToolStripMenuItem_Click);
            // 
            // populateSampleValues1
            // 
            this.populateSampleValues1.Name = "populateSampleValues1";
            this.populateSampleValues1.Size = new System.Drawing.Size(248, 22);
            this.populateSampleValues1.Text = "Populate Sample Values";
            this.populateSampleValues1.Click += new System.EventHandler(this.populateSampleValues1_Click);
            // 
            // autoorderRanksToolStripMenuItem
            // 
            this.autoorderRanksToolStripMenuItem.CheckOnClick = true;
            this.autoorderRanksToolStripMenuItem.Name = "autoorderRanksToolStripMenuItem";
            this.autoorderRanksToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.autoorderRanksToolStripMenuItem.Text = "&Auto-order Ranks";
            this.autoorderRanksToolStripMenuItem.Click += new System.EventHandler(this.autoorderRanksToolStripMenuItem_Click);
            // 
            // ranksToolStripMenuItem
            // 
            this.ranksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navOrderToolStripMenuItem,
            this.displayOrderToolStripMenuItem});
            this.ranksToolStripMenuItem.Name = "ranksToolStripMenuItem";
            this.ranksToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.ranksToolStripMenuItem.Text = "Order Ranks by";
            // 
            // navOrderToolStripMenuItem
            // 
            this.navOrderToolStripMenuItem.Name = "navOrderToolStripMenuItem";
            this.navOrderToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.navOrderToolStripMenuItem.Text = "Navigation Order";
            this.navOrderToolStripMenuItem.Click += new System.EventHandler(this.navOrderToolStripMenuItem_Click);
            // 
            // displayOrderToolStripMenuItem
            // 
            this.displayOrderToolStripMenuItem.Name = "displayOrderToolStripMenuItem";
            this.displayOrderToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.displayOrderToolStripMenuItem.Text = "Display Order";
            this.displayOrderToolStripMenuItem.Click += new System.EventHandler(this.displayOrderToolStripMenuItem_Click);
            // 
            // calculatedAttributeToolStripMenuItem
            // 
            this.calculatedAttributeToolStripMenuItem.Name = "calculatedAttributeToolStripMenuItem";
            this.calculatedAttributeToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.calculatedAttributeToolStripMenuItem.Text = "&Calculated Attribute Function";
            this.calculatedAttributeToolStripMenuItem.Click += new System.EventHandler(this.calculatedAttributeToolStripMenuItem_Click);
            // 
            // checkSpellingToolStripMenuItem
            // 
            this.checkSpellingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkSpellingSchemaViewToolStripMenuItem,
            this.checkSpellingSelectedRowToolStripMenuItem});
            this.checkSpellingToolStripMenuItem.Name = "checkSpellingToolStripMenuItem";
            this.checkSpellingToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.checkSpellingToolStripMenuItem.Text = "Check Spelling";
            // 
            // checkSpellingSchemaViewToolStripMenuItem
            // 
            this.checkSpellingSchemaViewToolStripMenuItem.Name = "checkSpellingSchemaViewToolStripMenuItem";
            this.checkSpellingSchemaViewToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.checkSpellingSchemaViewToolStripMenuItem.Text = "Check Spelling for Current Tab";
            this.checkSpellingSchemaViewToolStripMenuItem.Click += new System.EventHandler(this.checkSpellingSchemaViewToolStripMenuItem_Click);
            // 
            // checkSpellingSelectedRowToolStripMenuItem
            // 
            this.checkSpellingSelectedRowToolStripMenuItem.Name = "checkSpellingSelectedRowToolStripMenuItem";
            this.checkSpellingSelectedRowToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.checkSpellingSelectedRowToolStripMenuItem.Text = "Check Spelling for Selected Cells";
            this.checkSpellingSelectedRowToolStripMenuItem.Click += new System.EventHandler(this.checkSpellingSelectedRowToolStripMenuItem_Click);
            // 
            // attributeContextMenuStrip
            // 
            this.attributeContextMenuStrip.Name = "attributeContextMenuStrip";
            this.attributeContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // flowRibbon
            // 
            this.flowRibbon.AutoSize = true;
            this.flowRibbon.Controls.Add(this.mainMenuStrip);
            this.flowRibbon.Controls.Add(this.lblCurrentSchematus);
            this.flowRibbon.Controls.Add(this.ddCurrentSchematus);
            this.flowRibbon.Controls.Add(this.cbAutoSuggestOptions);
            this.flowRibbon.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowRibbon.Location = new System.Drawing.Point(0, 0);
            this.flowRibbon.Margin = new System.Windows.Forms.Padding(2);
            this.flowRibbon.Name = "flowRibbon";
            this.flowRibbon.Size = new System.Drawing.Size(614, 25);
            this.flowRibbon.TabIndex = 2;
            // 
            // lblCurrentSchematus
            // 
            this.lblCurrentSchematus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentSchematus.AutoSize = true;
            this.lblCurrentSchematus.Location = new System.Drawing.Point(235, 6);
            this.lblCurrentSchematus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentSchematus.Name = "lblCurrentSchematus";
            this.lblCurrentSchematus.Size = new System.Drawing.Size(112, 13);
            this.lblCurrentSchematus.TabIndex = 0;
            this.lblCurrentSchematus.Text = "Current Meta-attribute:";
            // 
            // ddCurrentSchematus
            // 
            this.ddCurrentSchematus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddCurrentSchematus.FormattingEnabled = true;
            this.ddCurrentSchematus.Location = new System.Drawing.Point(351, 2);
            this.ddCurrentSchematus.Margin = new System.Windows.Forms.Padding(2);
            this.ddCurrentSchematus.Name = "ddCurrentSchematus";
            this.ddCurrentSchematus.Size = new System.Drawing.Size(98, 21);
            this.ddCurrentSchematus.TabIndex = 1;
            this.ddCurrentSchematus.SelectedIndexChanged += new System.EventHandler(this.ddCurrentSchematus_SelectedIndexChanged);
            // 
            // cbAutoSuggestOptions
            // 
            this.cbAutoSuggestOptions.AutoSize = true;
            this.cbAutoSuggestOptions.Checked = true;
            this.cbAutoSuggestOptions.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cbAutoSuggestOptions.Location = new System.Drawing.Point(463, 3);
            this.cbAutoSuggestOptions.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.cbAutoSuggestOptions.Name = "cbAutoSuggestOptions";
            this.cbAutoSuggestOptions.Size = new System.Drawing.Size(133, 17);
            this.cbAutoSuggestOptions.TabIndex = 2;
            this.cbAutoSuggestOptions.Text = "Auto Multiline/Suggest";
            this.cbAutoSuggestOptions.ThreeState = true;
            this.cbAutoSuggestOptions.UseVisualStyleBackColor = true;
            this.cbAutoSuggestOptions.CheckStateChanged += new System.EventHandler(this.cbAutoSuggestOptions_CheckStateChanged);
            // 
            // RefreshGridViewTimer
            // 
            this.RefreshGridViewTimer.Interval = 1000;
            this.RefreshGridViewTimer.Tick += new System.EventHandler(this.RefreshGridViewTimer_Tick);
            // 
            // sdgvAuditTrail
            // 
            this.sdgvAuditTrail.AllowUserToAddRows = false;
            this.sdgvAuditTrail.AllowUserToDeleteRows = false;
            this.sdgvAuditTrail.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sdgvAuditTrail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.sdgvAuditTrail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sdgvAuditTrail.Dock = System.Windows.Forms.DockStyle.Right;
            this.sdgvAuditTrail.Location = new System.Drawing.Point(341, 25);
            this.sdgvAuditTrail.Name = "sdgvAuditTrail";
            this.sdgvAuditTrail.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sdgvAuditTrail.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.sdgvAuditTrail.Size = new System.Drawing.Size(273, 302);
            this.sdgvAuditTrail.TabIndex = 0;
            this.sdgvAuditTrail.Visible = false;
            this.sdgvAuditTrail.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.sdgvAuditTrail_CellPainting);
            // 
            // splitterAuditTrail
            // 
            this.splitterAuditTrail.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitterAuditTrail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitterAuditTrail.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterAuditTrail.Location = new System.Drawing.Point(336, 25);
            this.splitterAuditTrail.MinExtra = 15;
            this.splitterAuditTrail.MinSize = 15;
            this.splitterAuditTrail.Name = "splitterAuditTrail";
            this.splitterAuditTrail.Size = new System.Drawing.Size(5, 302);
            this.splitterAuditTrail.TabIndex = 6;
            this.splitterAuditTrail.TabStop = false;
            this.splitterAuditTrail.Visible = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(336, 0);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // sdgv
            // 
            this.sdgv.AllowUserToAddRows = false;
            this.sdgv.AllowUserToDeleteRows = false;
            this.sdgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sdgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.sdgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sdgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sdgv.EnableHeadersVisualStyles = false;
            this.sdgv.LastSelectedRowIndex = 0;
            this.sdgv.Location = new System.Drawing.Point(0, 25);
            this.sdgv.Margin = new System.Windows.Forms.Padding(2);
            this.sdgv.Name = "sdgv";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sdgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.sdgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.sdgv.RowTemplate.Height = 24;
            this.sdgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.sdgv.ShowEditingIcon = false;
            this.sdgv.Size = new System.Drawing.Size(341, 302);
            this.sdgv.TabIndex = 0;
            this.sdgv.VirtualMode = true;
            this.sdgv.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.sdgv_CellBeginEdit);
            this.sdgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.sdgv_CellEnter);
            this.sdgv.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.sdgv_CellFormatting);
            this.sdgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.sdgv_CellPainting);
            this.sdgv.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.sdgv_CellToolTipTextNeeded);
            this.sdgv.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.sdgv_CellValueNeeded);
            this.sdgv.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.sdgv_CellValuePushed);
            this.sdgv.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.sdgv_ColumnHeaderMouseClick);
            this.sdgv.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.sdgv_ColumnWidthChanged);
            this.sdgv.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.sdgv_EditingControlShowing);
            this.sdgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.sdgv_RowsAdded);
            this.sdgv.SelectionChanged += new System.EventHandler(this.sdgv_SelectionChanged);
            this.sdgv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sdgv_KeyDown);
            // 
            // SchemaDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.splitterAuditTrail);
            this.Controls.Add(this.sdgv);
            this.Controls.Add(this.sdgvAuditTrail);
            this.Controls.Add(this.flowRibbon);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SchemaDataGridView";
            this.Size = new System.Drawing.Size(614, 327);
            this.Load += new System.EventHandler(this.SchemaDataGridView_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.flowRibbon.ResumeLayout(false);
            this.flowRibbon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sdgvAuditTrail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sdgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomDataGridView sdgv;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshSchemaToolStripMenuItem;
        private FlowLayoutPanel flowRibbon;
        private System.Windows.Forms.Label lblCurrentSchematus;
        private System.Windows.Forms.ComboBox ddCurrentSchematus;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem saveAllChangesToolStripMenuItem;
        private ToolStripMenuItem reloadTabToolStripMenuItem;
        private ToolStripMenuItem closeTabToolStripMenuItem;
        private ToolStripMenuItem openInNewTabToolStripMenuItem;
        private ContextMenuStrip attributeContextMenuStrip;
        private ToolStripMenuItem cloneToAllNeighborsToolStripMenuItem;
        private ToolStripMenuItem cloneAllSchematiAndLovToolStripMenuItem;
        private ToolStripMenuItem cloneCurrentValueOnlyToolStripMenuItem;
        private ToolStripMenuItem nodeToolStripMenuItem;
        private ToolStripMenuItem filterNodesToolStripMenuItem;
        private ToolStripMenuItem GreatGreatGrandparentNodeToolStripMenuItem;
        private ToolStripMenuItem GreatGrandparentNodeToolStripMenuItem;
        private ToolStripMenuItem GrandparentNodeToolStripMenuItem;
        private Timer RefreshGridViewTimer;
        private ToolStripMenuItem ParentNodeToolStripMenuItem;
        private ToolStripMenuItem viewAdditionalNodesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem showFillRatesToolStripMenuItem;
        private ToolStripMenuItem removeSelectedNodesFromViewToolStripMenuItem;
        private ToolStripMenuItem unmapSelectedAttributesToolStripMenuItem;
        private ToolStripMenuItem customFillRatesToolStripMenuItem;
        private ToolStripMenuItem cloneCustomOptionsToolStripMenuItem;
        private ToolStripMenuItem cloneAllSchematiWithEmptyranksToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem autoorderRanksToolStripMenuItem;
        private CheckBox cbAutoSuggestOptions;
        private DataGridView sdgvAuditTrail;
        private ToolStripMenuItem auditTrailToolStripMenuItem;
        private Splitter splitterAuditTrail;
        private ToolStripMenuItem ranksToolStripMenuItem;
        private ToolStripMenuItem navOrderToolStripMenuItem;
        private ToolStripMenuItem displayOrderToolStripMenuItem;
        private FlowLayoutPanel flowLayoutPanel1;
        private ToolStripMenuItem notesToolStripMenuItem;
        private ToolStripMenuItem calculatedAttributeToolStripMenuItem;
        private ToolStripMenuItem populateSampleValues1;
        private ToolStripMenuItem userPreferencesToolStripMenuItem;
        private ToolStripMenuItem checkSpellingToolStripMenuItem;
        private ToolStripMenuItem checkSpellingSchemaViewToolStripMenuItem;
        private ToolStripMenuItem checkSpellingSelectedRowToolStripMenuItem;
    }
}
