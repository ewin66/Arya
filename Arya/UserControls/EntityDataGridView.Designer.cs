using System.Collections.Generic;
using System.Windows.Forms;
using Arya.CustomControls;
using Arya.Data;
using Arya.Framework.GUI.UserControls;
using Arya.HelperClasses;

namespace Arya.UserControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    partial class EntityDataGridView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityDataGridView));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusTimer = new System.Windows.Forms.Timer(this.components);
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkNavOrder = new System.Windows.Forms.CheckBox();
            this.chkDispOrder = new System.Windows.Forms.CheckBox();
            this.btnWrkflow = new System.Windows.Forms.CheckBox();
            this.chkSuggestValues = new System.Windows.Forms.CheckBox();
            this.btnChangeTaxonomy = new System.Windows.Forms.Button();
            this.btnSort = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnShowHideAttributes = new System.Windows.Forms.Button();
            this.btnReOrderColumns = new System.Windows.Forms.Button();
            this.chkShowHideLinks = new System.Windows.Forms.CheckBox();
            this.btnColor = new System.Windows.Forms.Button();
            this.btnShowHideAuditTrail = new System.Windows.Forms.CheckBox();
            this.sortContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sortAscendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortDescendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useShiftToForceSortByValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.numericSortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByComboToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.filterByComboToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
            this.filterAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.clearThisFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.saveCurrentFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFiltersFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlBeforeEntity = new System.Windows.Forms.FlowLayoutPanel();
            this.lblBeforeEntity = new System.Windows.Forms.Label();
            this.lblBeforeAttribute = new System.Windows.Forms.Label();
            this.lblBeforeValue = new System.Windows.Forms.Label();
            this.lblCreatedRemark = new System.Windows.Forms.Label();
            this.lblBeforeUom = new System.Windows.Forms.Label();
            this.lblBeforeField1 = new System.Windows.Forms.Label();
            this.lblBeforeField2 = new System.Windows.Forms.Label();
            this.lblBeforeField3 = new System.Windows.Forms.Label();
            this.lblBeforeField4 = new System.Windows.Forms.Label();
            this.lblBeforeField5 = new System.Windows.Forms.Label();
            this.lblBeforeDate = new System.Windows.Forms.Label();
            this.lblAttributeDefinition = new System.Windows.Forms.Label();
            this.lblDerivedAttributeText = new System.Windows.Forms.Label();
            this.tblEntity = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblNormalName = new System.Windows.Forms.Label();
            this.cboAttributeName = new System.Windows.Forms.ComboBox();
            this.chkField1 = new System.Windows.Forms.CheckBox();
            this.txtField3 = new System.Windows.Forms.ComboBox();
            this.chkField2 = new System.Windows.Forms.CheckBox();
            this.txtField1 = new System.Windows.Forms.ComboBox();
            this.txtField2 = new System.Windows.Forms.ComboBox();
            this.chkField3 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.txtField5 = new System.Windows.Forms.ComboBox();
            this.txtField4 = new System.Windows.Forms.ComboBox();
            this.chkUom = new System.Windows.Forms.CheckBox();
            this.txtUom = new System.Windows.Forms.ComboBox();
            this.chkField4 = new System.Windows.Forms.CheckBox();
            this.txtValue = new System.Windows.Forms.ComboBox();
            this.chkField5 = new System.Windows.Forms.CheckBox();
            this.lblAttributeValue = new System.Windows.Forms.Label();
            this.tblTools = new System.Windows.Forms.TableLayoutPanel();
            this.tblAttributeOrders = new System.Windows.Forms.TableLayoutPanel();
            this.txtDisplayOrder = new System.Windows.Forms.TextBox();
            this.lblAttributeOrders = new System.Windows.Forms.Label();
            this.txtNavigationOrder = new System.Windows.Forms.TextBox();
            this.lblCurrentChange = new System.Windows.Forms.Label();
            this.pnlDgvs = new System.Windows.Forms.Panel();
            this.edgv = new Arya.Framework.GUI.UserControls.CustomDataGridView();
            this.splitterAuditTrail = new System.Windows.Forms.Splitter();
            this.dgvAuditTrail = new System.Windows.Forms.DataGridView();
            this._newAttributeDropDown = new System.Windows.Forms.ComboBox();
            this.repopulateGridViewTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInSchemaViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCurrentNodeInSchemaViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAllNodesInSchemaViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInBuildViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aryaBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showItemImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showItemOnClientWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showURLsFromDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showResearchPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportCurrentViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showHideSKULinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHideAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHideAttributesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showProductAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInSchemaAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRankedAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAllImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.attributePreferencesPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.findReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveSKUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupSKUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToWorkflowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.newEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.auditTrailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculatedAttributeFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSpellingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sliceEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sliceDelimiterToolStripMenuItem = new Arya.CustomControls.LabelWithComboToolStripMenuItem();
            this.sliceSelectedEntitysInPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToDecimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noOfDecimalPlacesToolStripMenuItem = new Arya.CustomControls.LabelWithComboToolStripMenuItem();
            this.convertSelectedEntitysToDecimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aggregatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aggregateFunctionToolStripMenuItem = new Arya.CustomControls.LabelWithComboToolStripMenuItem();
            this.extractToNewAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertUomToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToUomDropDown = new System.Windows.Forms.ToolStripComboBox();
            this.convertUomPrecisionDropDown = new Arya.CustomControls.LabelWithComboToolStripMenuItem();
            this.convertUnitOfMeasureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.snippetTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialogXml = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogXml = new System.Windows.Forms.OpenFileDialog();
            this.selectionChangedTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripWorkflow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.approveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.unapproveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.skuImageBox = new System.Windows.Forms.PictureBox();
            this.tblRibbon = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.sortContextMenu.SuspendLayout();
            this.filterContextMenu.SuspendLayout();
            this.pnlBeforeEntity.SuspendLayout();
            this.tblEntity.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tblTools.SuspendLayout();
            this.tblAttributeOrders.SuspendLayout();
            this.pnlDgvs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditTrail)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.contextMenuStripWorkflow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.skuImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblRibbon)).BeginInit();
            this.tblRibbon.Panel1.SuspendLayout();
            this.tblRibbon.Panel2.SuspendLayout();
            this.tblRibbon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusTimer
            // 
            this.statusTimer.Interval = 200;
            this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
            // 
            // mainToolTip
            // 
            this.mainToolTip.AutoPopDelay = 5000;
            this.mainToolTip.InitialDelay = 100;
            this.mainToolTip.ReshowDelay = 0;
            // 
            // chkNavOrder
            // 
            this.chkNavOrder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkNavOrder.AutoSize = true;
            this.chkNavOrder.Checked = true;
            this.chkNavOrder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNavOrder.Location = new System.Drawing.Point(3, 26);
            this.chkNavOrder.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkNavOrder.Name = "chkNavOrder";
            this.chkNavOrder.Size = new System.Drawing.Size(49, 17);
            this.chkNavOrder.TabIndex = 0;
            this.chkNavOrder.Text = "Nav.";
            this.mainToolTip.SetToolTip(this.chkNavOrder, "Show Navigation Order in column header");
            this.chkNavOrder.UseVisualStyleBackColor = true;
            this.chkNavOrder.CheckedChanged += new System.EventHandler(this.chkNavOrder_CheckedChanged);
            // 
            // chkDispOrder
            // 
            this.chkDispOrder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkDispOrder.AutoSize = true;
            this.chkDispOrder.Checked = true;
            this.chkDispOrder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDispOrder.Location = new System.Drawing.Point(3, 50);
            this.chkDispOrder.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkDispOrder.Name = "chkDispOrder";
            this.chkDispOrder.Size = new System.Drawing.Size(50, 17);
            this.chkDispOrder.TabIndex = 2;
            this.chkDispOrder.Text = "Disp.";
            this.mainToolTip.SetToolTip(this.chkDispOrder, "Show Display Order in column header");
            this.chkDispOrder.UseVisualStyleBackColor = true;
            this.chkDispOrder.CheckedChanged += new System.EventHandler(this.chkDispOrder_CheckedChanged);
            // 
            // btnWrkflow
            // 
            this.btnWrkflow.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnWrkflow.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnWrkflow.Image = ((System.Drawing.Image)(resources.GetObject("btnWrkflow.Image")));
            this.btnWrkflow.Location = new System.Drawing.Point(3, 3);
            this.btnWrkflow.Name = "btnWrkflow";
            this.btnWrkflow.Size = new System.Drawing.Size(24, 23);
            this.btnWrkflow.TabIndex = 20;
            this.mainToolTip.SetToolTip(this.btnWrkflow, "Show/Hide SKU Links");
            this.btnWrkflow.UseVisualStyleBackColor = true;
            this.btnWrkflow.Visible = false;
            this.btnWrkflow.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkSuggestValues
            // 
            this.chkSuggestValues.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkSuggestValues.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkSuggestValues.Image = ((System.Drawing.Image)(resources.GetObject("chkSuggestValues.Image")));
            this.chkSuggestValues.Location = new System.Drawing.Point(233, 3);
            this.chkSuggestValues.Name = "chkSuggestValues";
            this.chkSuggestValues.Size = new System.Drawing.Size(21, 21);
            this.chkSuggestValues.TabIndex = 20;
            this.mainToolTip.SetToolTip(this.chkSuggestValues, "Auto-Suggest is ON");
            this.chkSuggestValues.UseVisualStyleBackColor = true;
            this.chkSuggestValues.CheckedChanged += new System.EventHandler(this.chkSuggestValues_CheckedChanged);
            // 
            // btnChangeTaxonomy
            // 
            this.btnChangeTaxonomy.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnChangeTaxonomy.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeTaxonomy.Image")));
            this.btnChangeTaxonomy.Location = new System.Drawing.Point(35, 2);
            this.btnChangeTaxonomy.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnChangeTaxonomy.Name = "btnChangeTaxonomy";
            this.btnChangeTaxonomy.Size = new System.Drawing.Size(26, 26);
            this.btnChangeTaxonomy.TabIndex = 9;
            this.mainToolTip.SetToolTip(this.btnChangeTaxonomy, "Move SKU(s)");
            this.btnChangeTaxonomy.UseVisualStyleBackColor = true;
            this.btnChangeTaxonomy.Click += new System.EventHandler(this.MoveSku);
            // 
            // btnSort
            // 
            this.btnSort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSort.Image = ((System.Drawing.Image)(resources.GetObject("btnSort.Image")));
            this.btnSort.Location = new System.Drawing.Point(3, 64);
            this.btnSort.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(26, 26);
            this.btnSort.TabIndex = 6;
            this.mainToolTip.SetToolTip(this.btnSort, "Sort");
            this.btnSort.UseVisualStyleBackColor = true;
            this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.Location = new System.Drawing.Point(3, 2);
            this.btnFind.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(26, 26);
            this.btnFind.TabIndex = 15;
            this.mainToolTip.SetToolTip(this.btnFind, "Find and Replace (Ctrl + F)");
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnFilter.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter.Image")));
            this.btnFilter.Location = new System.Drawing.Point(3, 33);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(26, 26);
            this.btnFilter.TabIndex = 8;
            this.mainToolTip.SetToolTip(this.btnFilter, "Filter (Ctrl + ?)");
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnShowHideAttributes
            // 
            this.btnShowHideAttributes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnShowHideAttributes.Image = ((System.Drawing.Image)(resources.GetObject("btnShowHideAttributes.Image")));
            this.btnShowHideAttributes.Location = new System.Drawing.Point(67, 2);
            this.btnShowHideAttributes.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnShowHideAttributes.Name = "btnShowHideAttributes";
            this.btnShowHideAttributes.Size = new System.Drawing.Size(26, 26);
            this.btnShowHideAttributes.TabIndex = 12;
            this.mainToolTip.SetToolTip(this.btnShowHideAttributes, "Show/Hide Attributes (Ctrl + H)");
            this.btnShowHideAttributes.UseVisualStyleBackColor = true;
            this.btnShowHideAttributes.Click += new System.EventHandler(this.btnAttributeOrderVisibility_Click);
            // 
            // btnReOrderColumns
            // 
            this.btnReOrderColumns.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnReOrderColumns.Image = ((System.Drawing.Image)(resources.GetObject("btnReOrderColumns.Image")));
            this.btnReOrderColumns.Location = new System.Drawing.Point(67, 33);
            this.btnReOrderColumns.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnReOrderColumns.Name = "btnReOrderColumns";
            this.btnReOrderColumns.Size = new System.Drawing.Size(26, 26);
            this.btnReOrderColumns.TabIndex = 14;
            this.mainToolTip.SetToolTip(this.btnReOrderColumns, "Re-order Columns (Ctrl + O)");
            this.btnReOrderColumns.UseVisualStyleBackColor = true;
            this.btnReOrderColumns.Click += new System.EventHandler(this.btnReOrderColumns_Click);
            // 
            // chkShowHideLinks
            // 
            this.chkShowHideLinks.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowHideLinks.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkShowHideLinks.Image = ((System.Drawing.Image)(resources.GetObject("chkShowHideLinks.Image")));
            this.chkShowHideLinks.Location = new System.Drawing.Point(35, 34);
            this.chkShowHideLinks.Name = "chkShowHideLinks";
            this.chkShowHideLinks.Size = new System.Drawing.Size(26, 25);
            this.chkShowHideLinks.TabIndex = 19;
            this.mainToolTip.SetToolTip(this.chkShowHideLinks, "Show/Hide SKU Links");
            this.chkShowHideLinks.UseVisualStyleBackColor = true;
            this.chkShowHideLinks.CheckedChanged += new System.EventHandler(this.chkShowHideLinks_CheckedChanged);
            // 
            // btnColor
            // 
            this.btnColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnColor.Image = ((System.Drawing.Image)(resources.GetObject("btnColor.Image")));
            this.btnColor.Location = new System.Drawing.Point(35, 64);
            this.btnColor.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(26, 26);
            this.btnColor.TabIndex = 18;
            this.mainToolTip.SetToolTip(this.btnColor, "Colorize");
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnShowHideAuditTrail
            // 
            this.btnShowHideAuditTrail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnShowHideAuditTrail.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnShowHideAuditTrail.Image = ((System.Drawing.Image)(resources.GetObject("btnShowHideAuditTrail.Image")));
            this.btnShowHideAuditTrail.Location = new System.Drawing.Point(67, 64);
            this.btnShowHideAuditTrail.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnShowHideAuditTrail.Name = "btnShowHideAuditTrail";
            this.btnShowHideAuditTrail.Size = new System.Drawing.Size(26, 26);
            this.btnShowHideAuditTrail.TabIndex = 17;
            this.mainToolTip.SetToolTip(this.btnShowHideAuditTrail, "Show/Hide Audit Trail");
            this.btnShowHideAuditTrail.UseVisualStyleBackColor = true;
            this.btnShowHideAuditTrail.CheckedChanged += new System.EventHandler(this.btnShowHideAuditTrail_CheckedChanged);
            // 
            // sortContextMenu
            // 
            this.sortContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortAscendingToolStripMenuItem,
            this.sortDescendingToolStripMenuItem,
            this.useShiftToForceSortByValueToolStripMenuItem,
            this.toolStripSeparator1,
            this.numericSortToolStripMenuItem,
            this.sortByComboToolStripMenuItem});
            this.sortContextMenu.Name = "sortContextMenu";
            this.sortContextMenu.OwnerItem = this.sortToolStripMenuItem;
            this.sortContextMenu.Size = new System.Drawing.Size(236, 125);
            // 
            // sortAscendingToolStripMenuItem
            // 
            this.sortAscendingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sortAscendingToolStripMenuItem.Image")));
            this.sortAscendingToolStripMenuItem.Name = "sortAscendingToolStripMenuItem";
            this.sortAscendingToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+<";
            this.sortAscendingToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.sortAscendingToolStripMenuItem.Text = "Sort &Ascending";
            this.sortAscendingToolStripMenuItem.Click += new System.EventHandler(this.sortAscendingToolStripMenuItem_Click);
            // 
            // sortDescendingToolStripMenuItem
            // 
            this.sortDescendingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sortDescendingToolStripMenuItem.Image")));
            this.sortDescendingToolStripMenuItem.Name = "sortDescendingToolStripMenuItem";
            this.sortDescendingToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+>";
            this.sortDescendingToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.sortDescendingToolStripMenuItem.Text = "Sort &Descending";
            this.sortDescendingToolStripMenuItem.Click += new System.EventHandler(this.sortDescendingToolStripMenuItem_Click);
            // 
            // useShiftToForceSortByValueToolStripMenuItem
            // 
            this.useShiftToForceSortByValueToolStripMenuItem.Name = "useShiftToForceSortByValueToolStripMenuItem";
            this.useShiftToForceSortByValueToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.useShiftToForceSortByValueToolStripMenuItem.Text = "(Use Shift key to sort by Value)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(232, 6);
            // 
            // numericSortToolStripMenuItem
            // 
            this.numericSortToolStripMenuItem.Checked = true;
            this.numericSortToolStripMenuItem.CheckOnClick = true;
            this.numericSortToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.numericSortToolStripMenuItem.Name = "numericSortToolStripMenuItem";
            this.numericSortToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.numericSortToolStripMenuItem.Text = "&Numeric Sort";
            // 
            // sortByComboToolStripMenuItem
            // 
            this.sortByComboToolStripMenuItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortByComboToolStripMenuItem.Name = "sortByComboToolStripMenuItem";
            this.sortByComboToolStripMenuItem.Size = new System.Drawing.Size(175, 23);
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.DropDown = this.sortContextMenu;
            this.sortToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sortToolStripMenuItem.Image")));
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.sortToolStripMenuItem.Text = "&Sort";
            // 
            // filterContextMenu
            // 
            this.filterContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterByComboToolStripMenuItem,
            this.filterAttributeToolStripMenuItem,
            this.filterValueToolStripMenuItem,
            this.toolStripSeparator5,
            this.clearThisFilterToolStripMenuItem,
            this.clearAllFiltersToolStripMenuItem,
            this.toolStripSeparator8,
            this.saveCurrentFiltersToolStripMenuItem,
            this.loadFiltersFromFileToolStripMenuItem});
            this.filterContextMenu.Name = "filterContextMenu";
            this.filterContextMenu.OwnerItem = this.filterToolStripMenuItem;
            this.filterContextMenu.Size = new System.Drawing.Size(236, 175);
            this.filterContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.filterContextMenu_Opening);
            // 
            // filterByComboToolStripMenuItem
            // 
            this.filterByComboToolStripMenuItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterByComboToolStripMenuItem.Name = "filterByComboToolStripMenuItem";
            this.filterByComboToolStripMenuItem.Size = new System.Drawing.Size(175, 23);
            this.filterByComboToolStripMenuItem.SelectedIndexChanged += new System.EventHandler(this.filterByComboToolStripMenuItem_SelectedIndexChanged);
            // 
            // filterAttributeToolStripMenuItem
            // 
            this.filterAttributeToolStripMenuItem.Name = "filterAttributeToolStripMenuItem";
            this.filterAttributeToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+/";
            this.filterAttributeToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.filterAttributeToolStripMenuItem.Text = "Filter &Attribute";
            this.filterAttributeToolStripMenuItem.Click += new System.EventHandler(this.filterAttributeToolStripMenuItem_Click);
            // 
            // filterValueToolStripMenuItem
            // 
            this.filterValueToolStripMenuItem.Name = "filterValueToolStripMenuItem";
            this.filterValueToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+?";
            this.filterValueToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.filterValueToolStripMenuItem.Text = "Filter &Value";
            this.filterValueToolStripMenuItem.Click += new System.EventHandler(this.filterValueToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(232, 6);
            // 
            // clearThisFilterToolStripMenuItem
            // 
            this.clearThisFilterToolStripMenuItem.Name = "clearThisFilterToolStripMenuItem";
            this.clearThisFilterToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.clearThisFilterToolStripMenuItem.Text = "Clear this &filter";
            this.clearThisFilterToolStripMenuItem.Click += new System.EventHandler(this.clearThisFilterToolStripMenuItem_Click);
            // 
            // clearAllFiltersToolStripMenuItem
            // 
            this.clearAllFiltersToolStripMenuItem.Name = "clearAllFiltersToolStripMenuItem";
            this.clearAllFiltersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End)));
            this.clearAllFiltersToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.clearAllFiltersToolStripMenuItem.Text = "&Clear all filters";
            this.clearAllFiltersToolStripMenuItem.Click += new System.EventHandler(this.clearAllFiltersToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(232, 6);
            // 
            // saveCurrentFiltersToolStripMenuItem
            // 
            this.saveCurrentFiltersToolStripMenuItem.Name = "saveCurrentFiltersToolStripMenuItem";
            this.saveCurrentFiltersToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.saveCurrentFiltersToolStripMenuItem.Text = "&Save current filter(s)";
            this.saveCurrentFiltersToolStripMenuItem.Click += new System.EventHandler(this.saveCurrentFiltersToolStripMenuItem_Click);
            // 
            // loadFiltersFromFileToolStripMenuItem
            // 
            this.loadFiltersFromFileToolStripMenuItem.Name = "loadFiltersFromFileToolStripMenuItem";
            this.loadFiltersFromFileToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.loadFiltersFromFileToolStripMenuItem.Text = "&Load filter(s) from file";
            this.loadFiltersFromFileToolStripMenuItem.Click += new System.EventHandler(this.loadFiltersFromFileToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDown = this.filterContextMenu;
            this.filterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filterToolStripMenuItem.Image")));
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.filterToolStripMenuItem.Text = "Fil&ter";
            // 
            // pnlBeforeEntity
            // 
            this.pnlBeforeEntity.AutoScroll = true;
            this.pnlBeforeEntity.Controls.Add(this.btnWrkflow);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeEntity);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeAttribute);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeValue);
            this.pnlBeforeEntity.Controls.Add(this.lblCreatedRemark);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeUom);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeField1);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeField2);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeField3);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeField4);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeField5);
            this.pnlBeforeEntity.Controls.Add(this.lblBeforeDate);
            this.pnlBeforeEntity.Controls.Add(this.lblAttributeDefinition);
            this.pnlBeforeEntity.Controls.Add(this.lblDerivedAttributeText);
            this.pnlBeforeEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBeforeEntity.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlBeforeEntity.Location = new System.Drawing.Point(3, 3);
            this.pnlBeforeEntity.Name = "pnlBeforeEntity";
            this.pnlBeforeEntity.Size = new System.Drawing.Size(178, 102);
            this.pnlBeforeEntity.TabIndex = 0;
            this.pnlBeforeEntity.WrapContents = false;
            // 
            // lblBeforeEntity
            // 
            this.lblBeforeEntity.AutoSize = true;
            this.lblBeforeEntity.Location = new System.Drawing.Point(3, 29);
            this.lblBeforeEntity.Name = "lblBeforeEntity";
            this.lblBeforeEntity.Size = new System.Drawing.Size(64, 13);
            this.lblBeforeEntity.TabIndex = 9;
            this.lblBeforeEntity.Text = "BeforeEntity";
            // 
            // lblBeforeAttribute
            // 
            this.lblBeforeAttribute.AutoSize = true;
            this.lblBeforeAttribute.Location = new System.Drawing.Point(3, 42);
            this.lblBeforeAttribute.Name = "lblBeforeAttribute";
            this.lblBeforeAttribute.Size = new System.Drawing.Size(80, 13);
            this.lblBeforeAttribute.TabIndex = 0;
            this.lblBeforeAttribute.Text = "Before Attribute";
            // 
            // lblBeforeValue
            // 
            this.lblBeforeValue.AutoSize = true;
            this.lblBeforeValue.Location = new System.Drawing.Point(3, 55);
            this.lblBeforeValue.Name = "lblBeforeValue";
            this.lblBeforeValue.Size = new System.Drawing.Size(68, 13);
            this.lblBeforeValue.TabIndex = 1;
            this.lblBeforeValue.Text = "Before Value";
            // 
            // lblCreatedRemark
            // 
            this.lblCreatedRemark.AutoSize = true;
            this.lblCreatedRemark.Location = new System.Drawing.Point(3, 68);
            this.lblCreatedRemark.Name = "lblCreatedRemark";
            this.lblCreatedRemark.Size = new System.Drawing.Size(84, 13);
            this.lblCreatedRemark.TabIndex = 11;
            this.lblCreatedRemark.Text = "Created Remark";
            // 
            // lblBeforeUom
            // 
            this.lblBeforeUom.AutoSize = true;
            this.lblBeforeUom.Location = new System.Drawing.Point(3, 81);
            this.lblBeforeUom.Name = "lblBeforeUom";
            this.lblBeforeUom.Size = new System.Drawing.Size(64, 13);
            this.lblBeforeUom.TabIndex = 2;
            this.lblBeforeUom.Text = "Before UoM";
            // 
            // lblBeforeField1
            // 
            this.lblBeforeField1.AutoSize = true;
            this.lblBeforeField1.Location = new System.Drawing.Point(3, 94);
            this.lblBeforeField1.Name = "lblBeforeField1";
            this.lblBeforeField1.Size = new System.Drawing.Size(69, 13);
            this.lblBeforeField1.TabIndex = 3;
            this.lblBeforeField1.Text = "Before Field1";
            // 
            // lblBeforeField2
            // 
            this.lblBeforeField2.AutoSize = true;
            this.lblBeforeField2.Location = new System.Drawing.Point(3, 107);
            this.lblBeforeField2.Name = "lblBeforeField2";
            this.lblBeforeField2.Size = new System.Drawing.Size(69, 13);
            this.lblBeforeField2.TabIndex = 4;
            this.lblBeforeField2.Text = "Before Field2";
            // 
            // lblBeforeField3
            // 
            this.lblBeforeField3.AutoSize = true;
            this.lblBeforeField3.Location = new System.Drawing.Point(3, 120);
            this.lblBeforeField3.Name = "lblBeforeField3";
            this.lblBeforeField3.Size = new System.Drawing.Size(69, 13);
            this.lblBeforeField3.TabIndex = 5;
            this.lblBeforeField3.Text = "Before Field3";
            // 
            // lblBeforeField4
            // 
            this.lblBeforeField4.AutoSize = true;
            this.lblBeforeField4.Location = new System.Drawing.Point(3, 133);
            this.lblBeforeField4.Name = "lblBeforeField4";
            this.lblBeforeField4.Size = new System.Drawing.Size(69, 13);
            this.lblBeforeField4.TabIndex = 6;
            this.lblBeforeField4.Text = "Before Field4";
            // 
            // lblBeforeField5
            // 
            this.lblBeforeField5.AutoSize = true;
            this.lblBeforeField5.Location = new System.Drawing.Point(3, 146);
            this.lblBeforeField5.Name = "lblBeforeField5";
            this.lblBeforeField5.Size = new System.Drawing.Size(69, 13);
            this.lblBeforeField5.TabIndex = 7;
            this.lblBeforeField5.Text = "Before Field5";
            // 
            // lblBeforeDate
            // 
            this.lblBeforeDate.AutoSize = true;
            this.lblBeforeDate.Location = new System.Drawing.Point(3, 159);
            this.lblBeforeDate.Name = "lblBeforeDate";
            this.lblBeforeDate.Size = new System.Drawing.Size(64, 13);
            this.lblBeforeDate.TabIndex = 8;
            this.lblBeforeDate.Text = "Before Date";
            // 
            // lblAttributeDefinition
            // 
            this.lblAttributeDefinition.AutoSize = true;
            this.lblAttributeDefinition.Location = new System.Drawing.Point(3, 172);
            this.lblAttributeDefinition.Margin = new System.Windows.Forms.Padding(3, 0, 15, 0);
            this.lblAttributeDefinition.Name = "lblAttributeDefinition";
            this.lblAttributeDefinition.Size = new System.Drawing.Size(96, 13);
            this.lblAttributeDefinition.TabIndex = 10;
            this.lblAttributeDefinition.Text = "Attribute Definition:";
            // 
            // lblDerivedAttributeText
            // 
            this.lblDerivedAttributeText.AutoSize = true;
            this.lblDerivedAttributeText.Location = new System.Drawing.Point(3, 185);
            this.lblDerivedAttributeText.Name = "lblDerivedAttributeText";
            this.lblDerivedAttributeText.Size = new System.Drawing.Size(110, 13);
            this.lblDerivedAttributeText.TabIndex = 21;
            this.lblDerivedAttributeText.Text = "Derived Attribute Text";
            // 
            // tblEntity
            // 
            this.tblEntity.AutoSize = true;
            this.tblEntity.ColumnCount = 2;
            this.tblEntity.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblEntity.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblEntity.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tblEntity.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tblEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblEntity.Location = new System.Drawing.Point(0, 0);
            this.tblEntity.Name = "tblEntity";
            this.tblEntity.RowCount = 1;
            this.tblEntity.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblEntity.Size = new System.Drawing.Size(525, 108);
            this.tblEntity.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblNormalName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cboAttributeName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkField1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtField3, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.chkField2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtField1, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtField2, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkField3, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(256, 102);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblNormalName
            // 
            this.lblNormalName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNormalName.AutoSize = true;
            this.lblNormalName.Location = new System.Drawing.Point(35, 6);
            this.lblNormalName.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblNormalName.Name = "lblNormalName";
            this.lblNormalName.Size = new System.Drawing.Size(49, 13);
            this.lblNormalName.TabIndex = 0;
            this.lblNormalName.Text = "Attribute:";
            this.lblNormalName.UseMnemonic = false;
            // 
            // cboAttributeName
            // 
            this.cboAttributeName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboAttributeName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboAttributeName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboAttributeName.DropDownHeight = 300;
            this.cboAttributeName.DropDownWidth = 200;
            this.cboAttributeName.FormattingEnabled = true;
            this.cboAttributeName.IntegralHeight = false;
            this.cboAttributeName.Location = new System.Drawing.Point(86, 2);
            this.cboAttributeName.Margin = new System.Windows.Forms.Padding(2);
            this.cboAttributeName.MaxDropDownItems = 50;
            this.cboAttributeName.Name = "cboAttributeName";
            this.cboAttributeName.Size = new System.Drawing.Size(168, 21);
            this.cboAttributeName.TabIndex = 1;
            this.cboAttributeName.SelectedIndexChanged += new System.EventHandler(this.cboAttributeName_Change);
            this.cboAttributeName.TextUpdate += new System.EventHandler(this.cboAttributeName_Change);
            this.cboAttributeName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // chkField1
            // 
            this.chkField1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkField1.AutoSize = true;
            this.chkField1.Location = new System.Drawing.Point(3, 29);
            this.chkField1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkField1.Name = "chkField1";
            this.chkField1.Size = new System.Drawing.Size(81, 17);
            this.chkField1.TabIndex = 4;
            this.chkField1.Text = "Field Value:";
            this.chkField1.UseMnemonic = false;
            this.chkField1.CheckedChanged += new System.EventHandler(this.UncheckEntityNameCheckboxes);
            // 
            // txtField3
            // 
            this.txtField3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtField3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtField3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtField3.Items.AddRange(new object[] {
            "<delete>"});
            this.txtField3.Location = new System.Drawing.Point(86, 77);
            this.txtField3.Margin = new System.Windows.Forms.Padding(2);
            this.txtField3.Name = "txtField3";
            this.txtField3.Size = new System.Drawing.Size(168, 21);
            this.txtField3.TabIndex = 13;
            this.txtField3.TextChanged += new System.EventHandler(this.txtField3_Change);
            this.txtField3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // chkField2
            // 
            this.chkField2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkField2.AutoSize = true;
            this.chkField2.Location = new System.Drawing.Point(3, 54);
            this.chkField2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkField2.Name = "chkField2";
            this.chkField2.Size = new System.Drawing.Size(81, 17);
            this.chkField2.TabIndex = 8;
            this.chkField2.Text = "Field Value:";
            this.chkField2.UseMnemonic = false;
            this.chkField2.CheckedChanged += new System.EventHandler(this.UncheckEntityNameCheckboxes);
            // 
            // txtField1
            // 
            this.txtField1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtField1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtField1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtField1.Items.AddRange(new object[] {
            "<delete>"});
            this.txtField1.Location = new System.Drawing.Point(86, 27);
            this.txtField1.Margin = new System.Windows.Forms.Padding(2);
            this.txtField1.Name = "txtField1";
            this.txtField1.Size = new System.Drawing.Size(168, 21);
            this.txtField1.TabIndex = 5;
            this.txtField1.TextChanged += new System.EventHandler(this.txtField1_Change);
            this.txtField1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // txtField2
            // 
            this.txtField2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtField2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtField2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtField2.Items.AddRange(new object[] {
            "<delete>"});
            this.txtField2.Location = new System.Drawing.Point(86, 52);
            this.txtField2.Margin = new System.Windows.Forms.Padding(2);
            this.txtField2.Name = "txtField2";
            this.txtField2.Size = new System.Drawing.Size(168, 21);
            this.txtField2.TabIndex = 9;
            this.txtField2.TextChanged += new System.EventHandler(this.txtField2_Change);
            this.txtField2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // chkField3
            // 
            this.chkField3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkField3.AutoSize = true;
            this.chkField3.Location = new System.Drawing.Point(3, 80);
            this.chkField3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkField3.Name = "chkField3";
            this.chkField3.Size = new System.Drawing.Size(81, 17);
            this.chkField3.TabIndex = 12;
            this.chkField3.Text = "Field Value:";
            this.chkField3.UseMnemonic = false;
            this.chkField3.CheckedChanged += new System.EventHandler(this.UncheckEntityNameCheckboxes);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.txtField5, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtField4, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkUom, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtUom, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkField4, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtValue, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkField5, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.lblAttributeValue, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkSuggestValues, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(265, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(257, 102);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // txtField5
            // 
            this.txtField5.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtField5.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtField5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtField5.Items.AddRange(new object[] {
            "<delete>"});
            this.txtField5.Location = new System.Drawing.Point(86, 79);
            this.txtField5.Margin = new System.Windows.Forms.Padding(2);
            this.txtField5.Name = "txtField5";
            this.txtField5.Size = new System.Drawing.Size(142, 21);
            this.txtField5.TabIndex = 15;
            this.txtField5.TextChanged += new System.EventHandler(this.txtField5_Change);
            this.txtField5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // txtField4
            // 
            this.txtField4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtField4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtField4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtField4.Items.AddRange(new object[] {
            "<delete>"});
            this.txtField4.Location = new System.Drawing.Point(86, 54);
            this.txtField4.Margin = new System.Windows.Forms.Padding(2);
            this.txtField4.Name = "txtField4";
            this.txtField4.Size = new System.Drawing.Size(142, 21);
            this.txtField4.TabIndex = 11;
            this.txtField4.TextChanged += new System.EventHandler(this.txtField4_Change);
            this.txtField4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // chkUom
            // 
            this.chkUom.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkUom.AutoSize = true;
            this.chkUom.Checked = true;
            this.chkUom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUom.Location = new System.Drawing.Point(24, 31);
            this.chkUom.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkUom.Name = "chkUom";
            this.chkUom.Size = new System.Drawing.Size(60, 17);
            this.chkUom.TabIndex = 6;
            this.chkUom.Text = "U O M:";
            this.chkUom.UseMnemonic = false;
            this.chkUom.CheckedChanged += new System.EventHandler(this.UncheckEntityNameCheckboxes);
            // 
            // txtUom
            // 
            this.txtUom.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtUom.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtUom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUom.Location = new System.Drawing.Point(86, 29);
            this.txtUom.Margin = new System.Windows.Forms.Padding(2);
            this.txtUom.Name = "txtUom";
            this.txtUom.Size = new System.Drawing.Size(142, 21);
            this.txtUom.TabIndex = 7;
            this.txtUom.TextChanged += new System.EventHandler(this.txtUom_Change);
            this.txtUom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // chkField4
            // 
            this.chkField4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkField4.AutoSize = true;
            this.chkField4.Location = new System.Drawing.Point(3, 56);
            this.chkField4.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkField4.Name = "chkField4";
            this.chkField4.Size = new System.Drawing.Size(81, 17);
            this.chkField4.TabIndex = 10;
            this.chkField4.Text = "Field Value:";
            this.chkField4.UseMnemonic = false;
            this.chkField4.CheckedChanged += new System.EventHandler(this.UncheckEntityNameCheckboxes);
            // 
            // txtValue
            // 
            this.txtValue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtValue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.txtValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtValue.DropDownHeight = 300;
            this.txtValue.DropDownWidth = 200;
            this.txtValue.FormattingEnabled = true;
            this.txtValue.IntegralHeight = false;
            this.txtValue.Location = new System.Drawing.Point(86, 2);
            this.txtValue.Margin = new System.Windows.Forms.Padding(2);
            this.txtValue.MaxDropDownItems = 50;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(142, 21);
            this.txtValue.TabIndex = 3;
            this.txtValue.SelectedIndexChanged += new System.EventHandler(this.txtValue_Change);
            this.txtValue.TextUpdate += new System.EventHandler(this.txtValue_Change);
            this.txtValue.Enter += new System.EventHandler(this.txtValue_Enter);
            this.txtValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            // 
            // chkField5
            // 
            this.chkField5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkField5.AutoSize = true;
            this.chkField5.Location = new System.Drawing.Point(3, 81);
            this.chkField5.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkField5.Name = "chkField5";
            this.chkField5.Size = new System.Drawing.Size(81, 17);
            this.chkField5.TabIndex = 14;
            this.chkField5.Text = "Field Value:";
            this.chkField5.UseMnemonic = false;
            this.chkField5.CheckedChanged += new System.EventHandler(this.UncheckEntityNameCheckboxes);
            // 
            // lblAttributeValue
            // 
            this.lblAttributeValue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAttributeValue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAttributeValue.Location = new System.Drawing.Point(29, 3);
            this.lblAttributeValue.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblAttributeValue.Name = "lblAttributeValue";
            this.lblAttributeValue.Size = new System.Drawing.Size(55, 21);
            this.lblAttributeValue.TabIndex = 2;
            this.lblAttributeValue.Text = "Value:";
            this.lblAttributeValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAttributeValue.UseMnemonic = false;
            // 
            // tblTools
            // 
            this.tblTools.AutoSize = true;
            this.tblTools.ColumnCount = 3;
            this.tblTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTools.Controls.Add(this.btnChangeTaxonomy, 1, 0);
            this.tblTools.Controls.Add(this.btnSort, 0, 2);
            this.tblTools.Controls.Add(this.btnFind, 0, 0);
            this.tblTools.Controls.Add(this.btnFilter, 0, 1);
            this.tblTools.Controls.Add(this.btnShowHideAttributes, 2, 0);
            this.tblTools.Controls.Add(this.btnReOrderColumns, 2, 1);
            this.tblTools.Controls.Add(this.chkShowHideLinks, 1, 1);
            this.tblTools.Controls.Add(this.btnColor, 1, 2);
            this.tblTools.Controls.Add(this.btnShowHideAuditTrail, 2, 2);
            this.tblTools.Location = new System.Drawing.Point(3, 3);
            this.tblTools.Name = "tblTools";
            this.tblTools.RowCount = 3;
            this.tblTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblTools.Size = new System.Drawing.Size(96, 93);
            this.tblTools.TabIndex = 2;
            // 
            // tblAttributeOrders
            // 
            this.tblAttributeOrders.AutoSize = true;
            this.tblAttributeOrders.ColumnCount = 2;
            this.tblAttributeOrders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblAttributeOrders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblAttributeOrders.Controls.Add(this.txtDisplayOrder, 1, 2);
            this.tblAttributeOrders.Controls.Add(this.chkNavOrder, 0, 1);
            this.tblAttributeOrders.Controls.Add(this.chkDispOrder, 0, 2);
            this.tblAttributeOrders.Controls.Add(this.lblAttributeOrders, 0, 0);
            this.tblAttributeOrders.Controls.Add(this.txtNavigationOrder, 1, 1);
            this.tblAttributeOrders.Controls.Add(this.lblCurrentChange, 0, 3);
            this.tblAttributeOrders.Enabled = false;
            this.tblAttributeOrders.Location = new System.Drawing.Point(105, 3);
            this.tblAttributeOrders.Name = "tblAttributeOrders";
            this.tblAttributeOrders.RowCount = 4;
            this.tblAttributeOrders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAttributeOrders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblAttributeOrders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblAttributeOrders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblAttributeOrders.Size = new System.Drawing.Size(90, 102);
            this.tblAttributeOrders.TabIndex = 2;
            this.tblAttributeOrders.Visible = false;
            // 
            // txtDisplayOrder
            // 
            this.txtDisplayOrder.Location = new System.Drawing.Point(56, 50);
            this.txtDisplayOrder.Name = "txtDisplayOrder";
            this.txtDisplayOrder.Size = new System.Drawing.Size(31, 20);
            this.txtDisplayOrder.TabIndex = 19;
            this.txtDisplayOrder.Leave += new System.EventHandler(this.txtNavigationAndDisplayOrder_Leave);
            // 
            // lblAttributeOrders
            // 
            this.lblAttributeOrders.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblAttributeOrders.AutoSize = true;
            this.tblAttributeOrders.SetColumnSpan(this.lblAttributeOrders, 2);
            this.lblAttributeOrders.Location = new System.Drawing.Point(26, 3);
            this.lblAttributeOrders.Margin = new System.Windows.Forms.Padding(0, 3, 0, 7);
            this.lblAttributeOrders.Name = "lblAttributeOrders";
            this.lblAttributeOrders.Size = new System.Drawing.Size(38, 13);
            this.lblAttributeOrders.TabIndex = 17;
            this.lblAttributeOrders.Text = "Ranks";
            this.lblAttributeOrders.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtNavigationOrder
            // 
            this.txtNavigationOrder.Location = new System.Drawing.Point(56, 26);
            this.txtNavigationOrder.Name = "txtNavigationOrder";
            this.txtNavigationOrder.Size = new System.Drawing.Size(31, 20);
            this.txtNavigationOrder.TabIndex = 18;
            this.txtNavigationOrder.Leave += new System.EventHandler(this.txtNavigationAndDisplayOrder_Leave);
            // 
            // lblCurrentChange
            // 
            this.lblCurrentChange.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentChange.AutoSize = true;
            this.tblAttributeOrders.SetColumnSpan(this.lblCurrentChange, 2);
            this.lblCurrentChange.Location = new System.Drawing.Point(3, 80);
            this.lblCurrentChange.Name = "lblCurrentChange";
            this.lblCurrentChange.Size = new System.Drawing.Size(81, 13);
            this.lblCurrentChange.TabIndex = 20;
            this.lblCurrentChange.Text = "Current Change";
            // 
            // pnlDgvs
            // 
            this.pnlDgvs.Controls.Add(this.edgv);
            this.pnlDgvs.Controls.Add(this.splitterAuditTrail);
            this.pnlDgvs.Controls.Add(this.dgvAuditTrail);
            this.pnlDgvs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDgvs.Location = new System.Drawing.Point(0, 136);
            this.pnlDgvs.Name = "pnlDgvs";
            this.pnlDgvs.Size = new System.Drawing.Size(1024, 442);
            this.pnlDgvs.TabIndex = 2;
            // 
            // edgv
            // 
            this.edgv.AllowDrop = true;
            this.edgv.AllowUserToAddRows = false;
            this.edgv.AllowUserToDeleteRows = false;
            this.edgv.AllowUserToOrderColumns = true;
            this.edgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.edgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.edgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.edgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edgv.EnableHeadersVisualStyles = false;
            this.edgv.LastSelectedRowIndex = 0;
            this.edgv.Location = new System.Drawing.Point(0, 0);
            this.edgv.Name = "edgv";
            this.edgv.ReadOnly = true;
            this.edgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.edgv.RowTemplate.Height = 24;
            this.edgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.edgv.ShowCellToolTips = false;
            this.edgv.Size = new System.Drawing.Size(820, 442);
            this.edgv.StandardTab = true;
            this.edgv.TabIndex = 0;
            this.edgv.VirtualMode = true;
            this.edgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.edgv_CellClick);
            this.edgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.edgv_CellPainting);
            this.edgv.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.edgv_CellValueNeeded);
            this.edgv.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.edgv_ColumnHeaderMouseClick);
            this.edgv.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.edgv_ColumnHeaderMouseDoubleClick);
            this.edgv.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.edgv_ColumnWidthChanged);
            this.edgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.edgv_DataError);
            this.edgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.edgv_RowsAdded);
            this.edgv.SelectionChanged += new System.EventHandler(this.edgv_SelectionChanged);
            this.edgv.Enter += new System.EventHandler(this.edgv_Enter);
            this.edgv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.edgv_KeyPress);
            this.edgv.Leave += new System.EventHandler(this.edgv_Leave);
            // 
            // splitterAuditTrail
            // 
            this.splitterAuditTrail.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitterAuditTrail.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterAuditTrail.Location = new System.Drawing.Point(820, 0);
            this.splitterAuditTrail.Name = "splitterAuditTrail";
            this.splitterAuditTrail.Size = new System.Drawing.Size(5, 442);
            this.splitterAuditTrail.TabIndex = 5;
            this.splitterAuditTrail.TabStop = false;
            // 
            // dgvAuditTrail
            // 
            this.dgvAuditTrail.AllowUserToAddRows = false;
            this.dgvAuditTrail.AllowUserToDeleteRows = false;
            this.dgvAuditTrail.AllowUserToOrderColumns = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAuditTrail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAuditTrail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuditTrail.Dock = System.Windows.Forms.DockStyle.Right;
            this.dgvAuditTrail.Location = new System.Drawing.Point(825, 0);
            this.dgvAuditTrail.Name = "dgvAuditTrail";
            this.dgvAuditTrail.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAuditTrail.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAuditTrail.RowTemplate.Height = 24;
            this.dgvAuditTrail.Size = new System.Drawing.Size(199, 442);
            this.dgvAuditTrail.TabIndex = 4;
            this.dgvAuditTrail.DataSourceChanged += new System.EventHandler(this.dgvAuditTrail_DataSourceChanged);
            // 
            // _newAttributeDropDown
            // 
            this._newAttributeDropDown.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this._newAttributeDropDown.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this._newAttributeDropDown.DropDownHeight = 200;
            this._newAttributeDropDown.DropDownWidth = 150;
            this._newAttributeDropDown.FormattingEnabled = true;
            this._newAttributeDropDown.IntegralHeight = false;
            this._newAttributeDropDown.Location = new System.Drawing.Point(255, 160);
            this._newAttributeDropDown.MaxDropDownItems = 20;
            this._newAttributeDropDown.Name = "_newAttributeDropDown";
            this._newAttributeDropDown.Size = new System.Drawing.Size(121, 21);
            this._newAttributeDropDown.TabIndex = 1;
            this._newAttributeDropDown.Visible = false;
            this._newAttributeDropDown.TextUpdate += new System.EventHandler(this._newAttributeDropDown_TextUpdate);
            this._newAttributeDropDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityFields_KeyDown);
            this._newAttributeDropDown.Leave += new System.EventHandler(this._newAttributeDropDown_Leave);
            // 
            // repopulateGridViewTimer
            // 
            this.repopulateGridViewTimer.Interval = 500;
            this.repopulateGridViewTimer.Tick += new System.EventHandler(this.repopulateGridViewTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 578);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.editToolStripMenuItem,
            this.dataToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.mainMenuStrip.Size = new System.Drawing.Size(1024, 24);
            this.mainMenuStrip.TabIndex = 4;
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInNewTabToolStripMenuItem,
            this.openInSchemaViewToolStripMenuItem,
            this.openInBuildViewToolStripMenuItem,
            this.aryaBrowserToolStripMenuItem,
            this.toolStripSeparator9,
            this.saveToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportCurrentViewToolStripMenuItem,
            this.toolStripSeparator3,
            this.showHideSKULinksToolStripMenuItem,
            this.showHideAttributesToolStripMenuItem,
            this.reorderColumnsToolStripMenuItem,
            this.loadAllImagesToolStripMenuItem,
            this.notesToolStripMenuItem,
            this.toolStripSeparator10,
            this.attributePreferencesPropertiesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // openInNewTabToolStripMenuItem
            // 
            this.openInNewTabToolStripMenuItem.Enabled = false;
            this.openInNewTabToolStripMenuItem.Name = "openInNewTabToolStripMenuItem";
            this.openInNewTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.openInNewTabToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.openInNewTabToolStripMenuItem.Text = "&Open node in new tab";
            this.openInNewTabToolStripMenuItem.Click += new System.EventHandler(this.openInNewTabToolStripMenuItem_Click);
            // 
            // openInSchemaViewToolStripMenuItem
            // 
            this.openInSchemaViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCurrentNodeInSchemaViewToolStripMenuItem,
            this.openAllNodesInSchemaViewToolStripMenuItem});
            this.openInSchemaViewToolStripMenuItem.Name = "openInSchemaViewToolStripMenuItem";
            this.openInSchemaViewToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.openInSchemaViewToolStripMenuItem.Text = "Open in S&chema View";
            // 
            // openCurrentNodeInSchemaViewToolStripMenuItem
            // 
            this.openCurrentNodeInSchemaViewToolStripMenuItem.Name = "openCurrentNodeInSchemaViewToolStripMenuItem";
            this.openCurrentNodeInSchemaViewToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openCurrentNodeInSchemaViewToolStripMenuItem.Text = "Open Current Node in Schema View";
            this.openCurrentNodeInSchemaViewToolStripMenuItem.Click += new System.EventHandler(this.openCurrentNodeInSchemaViewToolStripMenuItem_Click);
            // 
            // openAllNodesInSchemaViewToolStripMenuItem
            // 
            this.openAllNodesInSchemaViewToolStripMenuItem.Name = "openAllNodesInSchemaViewToolStripMenuItem";
            this.openAllNodesInSchemaViewToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openAllNodesInSchemaViewToolStripMenuItem.Text = "Open &All Nodes in Schema View";
            this.openAllNodesInSchemaViewToolStripMenuItem.Click += new System.EventHandler(this.openAllNodesInSchemaViewToolStripMenuItem_Click);
            // 
            // openInBuildViewToolStripMenuItem
            // 
            this.openInBuildViewToolStripMenuItem.Name = "openInBuildViewToolStripMenuItem";
            this.openInBuildViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.openInBuildViewToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.openInBuildViewToolStripMenuItem.Text = "Open in Build View";
            this.openInBuildViewToolStripMenuItem.Click += new System.EventHandler(this.openInBuildViewToolStripMenuItem_Click);
            // 
            // aryaBrowserToolStripMenuItem
            // 
            this.aryaBrowserToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showItemImagesToolStripMenuItem,
            this.showItemOnClientWebsiteToolStripMenuItem,
            this.showURLsFromDataToolStripMenuItem,
            this.showResearchPagesToolStripMenuItem});
            this.aryaBrowserToolStripMenuItem.Name = "aryaBrowserToolStripMenuItem";
            this.aryaBrowserToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.aryaBrowserToolStripMenuItem.Text = "Arya &Browser";
            // 
            // showItemImagesToolStripMenuItem
            // 
            this.showItemImagesToolStripMenuItem.CheckOnClick = true;
            this.showItemImagesToolStripMenuItem.Name = "showItemImagesToolStripMenuItem";
            this.showItemImagesToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+B";
            this.showItemImagesToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showItemImagesToolStripMenuItem.Text = "Show Item &Image(s)";
            this.showItemImagesToolStripMenuItem.Click += new System.EventHandler(this.showItemImagesToolStripMenuItem_Click);
            // 
            // showItemOnClientWebsiteToolStripMenuItem
            // 
            this.showItemOnClientWebsiteToolStripMenuItem.CheckOnClick = true;
            this.showItemOnClientWebsiteToolStripMenuItem.Name = "showItemOnClientWebsiteToolStripMenuItem";
            this.showItemOnClientWebsiteToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showItemOnClientWebsiteToolStripMenuItem.Text = "Show Item on &Client Website";
            this.showItemOnClientWebsiteToolStripMenuItem.Click += new System.EventHandler(this.showItemOnClientWebsiteToolStripMenuItem_Click);
            // 
            // showURLsFromDataToolStripMenuItem
            // 
            this.showURLsFromDataToolStripMenuItem.CheckOnClick = true;
            this.showURLsFromDataToolStripMenuItem.Name = "showURLsFromDataToolStripMenuItem";
            this.showURLsFromDataToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showURLsFromDataToolStripMenuItem.Text = "Show &URLs from data";
            this.showURLsFromDataToolStripMenuItem.Click += new System.EventHandler(this.showURLsFromDataToolStripMenuItem_Click);
            // 
            // showResearchPagesToolStripMenuItem
            // 
            this.showResearchPagesToolStripMenuItem.CheckOnClick = true;
            this.showResearchPagesToolStripMenuItem.Name = "showResearchPagesToolStripMenuItem";
            this.showResearchPagesToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showResearchPagesToolStripMenuItem.Text = "Show &Research Pages";
            this.showResearchPagesToolStripMenuItem.Click += new System.EventHandler(this.showResearchPagesToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(251, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.saveToolStripMenuItem.Text = "&Save all changes";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolStripMenuItem.Image")));
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.reloadToolStripMenuItem.Text = "&Reload tab";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.closeToolStripMenuItem.Text = "&Close tab";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(251, 6);
            // 
            // exportCurrentViewToolStripMenuItem
            // 
            this.exportCurrentViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportCurrentViewToolStripMenuItem.Image")));
            this.exportCurrentViewToolStripMenuItem.Name = "exportCurrentViewToolStripMenuItem";
            this.exportCurrentViewToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.exportCurrentViewToolStripMenuItem.Text = "&Export current view";
            this.exportCurrentViewToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(251, 6);
            // 
            // showHideSKULinksToolStripMenuItem
            // 
            this.showHideSKULinksToolStripMenuItem.CheckOnClick = true;
            this.showHideSKULinksToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showHideSKULinksToolStripMenuItem.Image")));
            this.showHideSKULinksToolStripMenuItem.Name = "showHideSKULinksToolStripMenuItem";
            this.showHideSKULinksToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.showHideSKULinksToolStripMenuItem.Text = "Show/Hide SKU links";
            this.showHideSKULinksToolStripMenuItem.Click += new System.EventHandler(this.showHideSKULinksToolStripMenuItem_Click);
            // 
            // showHideAttributesToolStripMenuItem
            // 
            this.showHideAttributesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHideAttributesToolStripMenuItem1,
            this.showProductAttributesToolStripMenuItem,
            this.showInSchemaAttributesToolStripMenuItem,
            this.showRankedAttributesToolStripMenuItem});
            this.showHideAttributesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showHideAttributesToolStripMenuItem.Image")));
            this.showHideAttributesToolStripMenuItem.Name = "showHideAttributesToolStripMenuItem";
            this.showHideAttributesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.showHideAttributesToolStripMenuItem.Text = "Show/&Hide Attributes";
            // 
            // showHideAttributesToolStripMenuItem1
            // 
            this.showHideAttributesToolStripMenuItem1.Name = "showHideAttributesToolStripMenuItem1";
            this.showHideAttributesToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.showHideAttributesToolStripMenuItem1.Size = new System.Drawing.Size(283, 22);
            this.showHideAttributesToolStripMenuItem1.Text = "Show/&Hide Attributes Window";
            this.showHideAttributesToolStripMenuItem1.Click += new System.EventHandler(this.showHideAttributesToolStripMenuItem_Click);
            // 
            // showProductAttributesToolStripMenuItem
            // 
            this.showProductAttributesToolStripMenuItem.Name = "showProductAttributesToolStripMenuItem";
            this.showProductAttributesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.H)));
            this.showProductAttributesToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.showProductAttributesToolStripMenuItem.Text = "Show Product Attributes";
            this.showProductAttributesToolStripMenuItem.Click += new System.EventHandler(this.showExtendedAttributesToolStripMenuItem_Click);
            // 
            // showInSchemaAttributesToolStripMenuItem
            // 
            this.showInSchemaAttributesToolStripMenuItem.Name = "showInSchemaAttributesToolStripMenuItem";
            this.showInSchemaAttributesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.J)));
            this.showInSchemaAttributesToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.showInSchemaAttributesToolStripMenuItem.Text = "Show InSchema Attributes";
            this.showInSchemaAttributesToolStripMenuItem.Click += new System.EventHandler(this.showInSchemaAttributesToolStripMenuItem_Click);
            // 
            // showRankedAttributesToolStripMenuItem
            // 
            this.showRankedAttributesToolStripMenuItem.Name = "showRankedAttributesToolStripMenuItem";
            this.showRankedAttributesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.K)));
            this.showRankedAttributesToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.showRankedAttributesToolStripMenuItem.Text = "Show Ranked Attributes";
            this.showRankedAttributesToolStripMenuItem.Click += new System.EventHandler(this.showRankedAttributesToolStripMenuItem_Click);
            // 
            // reorderColumnsToolStripMenuItem
            // 
            this.reorderColumnsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reorderColumnsToolStripMenuItem.Image")));
            this.reorderColumnsToolStripMenuItem.Name = "reorderColumnsToolStripMenuItem";
            this.reorderColumnsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.reorderColumnsToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.reorderColumnsToolStripMenuItem.Text = "Re-order columns";
            this.reorderColumnsToolStripMenuItem.Click += new System.EventHandler(this.reorderColumnsToolStripMenuItem_Click);
            // 
            // loadAllImagesToolStripMenuItem
            // 
            this.loadAllImagesToolStripMenuItem.Name = "loadAllImagesToolStripMenuItem";
            this.loadAllImagesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.loadAllImagesToolStripMenuItem.Text = "Load All Images";
            this.loadAllImagesToolStripMenuItem.Click += new System.EventHandler(this.loadAllImagesToolStripMenuItem_Click);
            // 
            // notesToolStripMenuItem
            // 
            this.notesToolStripMenuItem.Name = "notesToolStripMenuItem";
            this.notesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemtilde)));
            this.notesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.notesToolStripMenuItem.Text = "Notes";
            this.notesToolStripMenuItem.Click += new System.EventHandler(this.notesToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(251, 6);
            // 
            // attributePreferencesPropertiesToolStripMenuItem
            // 
            this.attributePreferencesPropertiesToolStripMenuItem.Name = "attributePreferencesPropertiesToolStripMenuItem";
            this.attributePreferencesPropertiesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.attributePreferencesPropertiesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.attributePreferencesPropertiesToolStripMenuItem.Text = "SKU View User &Preferences";
            this.attributePreferencesPropertiesToolStripMenuItem.Click += new System.EventHandler(this.showAttributePreferencesToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator4,
            this.findReplaceToolStripMenuItem,
            this.colorizeToolStripMenuItem,
            this.sortToolStripMenuItem,
            this.filterToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoToolStripMenuItem.Image")));
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoToolStripMenuItem.Image")));
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(201, 6);
            // 
            // findReplaceToolStripMenuItem
            // 
            this.findReplaceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("findReplaceToolStripMenuItem.Image")));
            this.findReplaceToolStripMenuItem.Name = "findReplaceToolStripMenuItem";
            this.findReplaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findReplaceToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.findReplaceToolStripMenuItem.Text = "&Find and Replace";
            this.findReplaceToolStripMenuItem.Click += new System.EventHandler(this.findReplaceToolStripMenuItem_Click);
            // 
            // colorizeToolStripMenuItem
            // 
            this.colorizeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("colorizeToolStripMenuItem.Image")));
            this.colorizeToolStripMenuItem.Name = "colorizeToolStripMenuItem";
            this.colorizeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.colorizeToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.colorizeToolStripMenuItem.Text = "&Colorize";
            this.colorizeToolStripMenuItem.Click += new System.EventHandler(this.colorizeToolStripMenuItem_Click);
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveSKUToolStripMenuItem,
            this.groupSKUsToolStripMenuItem,
            this.addToWorkflowToolStripMenuItem,
            this.toolStripSeparator6,
            this.newEntitiesToolStripMenuItem,
            this.deleteSelectedEntitiesToolStripMenuItem,
            this.toolStripSeparator7,
            this.auditTrailToolStripMenuItem,
            this.calculatedAttributeFunctionToolStripMenuItem,
            this.validateToolStripMenuItem,
            this.checkSpellingToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.dataToolStripMenuItem.Text = "&Data";
            // 
            // moveSKUToolStripMenuItem
            // 
            this.moveSKUToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveSKUToolStripMenuItem.Image")));
            this.moveSKUToolStripMenuItem.Name = "moveSKUToolStripMenuItem";
            this.moveSKUToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.moveSKUToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.moveSKUToolStripMenuItem.Text = "&Move SKU";
            this.moveSKUToolStripMenuItem.Click += new System.EventHandler(this.MoveSku);
            // 
            // groupSKUsToolStripMenuItem
            // 
            this.groupSKUsToolStripMenuItem.Name = "groupSKUsToolStripMenuItem";
            this.groupSKUsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.groupSKUsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.groupSKUsToolStripMenuItem.Text = "Group SKUs";
            this.groupSKUsToolStripMenuItem.Click += new System.EventHandler(this.groupSKUsToolStripMenuItem_Click);
            // 
            // addToWorkflowToolStripMenuItem
            // 
            this.addToWorkflowToolStripMenuItem.Name = "addToWorkflowToolStripMenuItem";
            this.addToWorkflowToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.addToWorkflowToolStripMenuItem.Text = "Add to Workflow";
            this.addToWorkflowToolStripMenuItem.Click += new System.EventHandler(this.addToWorkflowToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(247, 6);
            // 
            // newEntitiesToolStripMenuItem
            // 
            this.newEntitiesToolStripMenuItem.CheckOnClick = true;
            this.newEntitiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newEntitiesToolStripMenuItem.Image")));
            this.newEntitiesToolStripMenuItem.Name = "newEntitiesToolStripMenuItem";
            this.newEntitiesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Insert)));
            this.newEntitiesToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.newEntitiesToolStripMenuItem.Text = "&New entity(s)";
            this.newEntitiesToolStripMenuItem.Click += new System.EventHandler(this.newEntitiesToolStripMenuItem_Click);
            // 
            // deleteSelectedEntitiesToolStripMenuItem
            // 
            this.deleteSelectedEntitiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteSelectedEntitiesToolStripMenuItem.Image")));
            this.deleteSelectedEntitiesToolStripMenuItem.Name = "deleteSelectedEntitiesToolStripMenuItem";
            this.deleteSelectedEntitiesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteSelectedEntitiesToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.deleteSelectedEntitiesToolStripMenuItem.Text = "&Delete selected entity(s)";
            this.deleteSelectedEntitiesToolStripMenuItem.Click += new System.EventHandler(this.DeleteSelectedEntities);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(247, 6);
            // 
            // auditTrailToolStripMenuItem
            // 
            this.auditTrailToolStripMenuItem.CheckOnClick = true;
            this.auditTrailToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("auditTrailToolStripMenuItem.Image")));
            this.auditTrailToolStripMenuItem.Name = "auditTrailToolStripMenuItem";
            this.auditTrailToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.auditTrailToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.auditTrailToolStripMenuItem.Text = "&Audit Trail";
            this.auditTrailToolStripMenuItem.Click += new System.EventHandler(this.auditTrailToolStripMenuItem_Click);
            // 
            // calculatedAttributeFunctionToolStripMenuItem
            // 
            this.calculatedAttributeFunctionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("calculatedAttributeFunctionToolStripMenuItem.Image")));
            this.calculatedAttributeFunctionToolStripMenuItem.Name = "calculatedAttributeFunctionToolStripMenuItem";
            this.calculatedAttributeFunctionToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.calculatedAttributeFunctionToolStripMenuItem.Text = "&Calculated Attribute Function";
            this.calculatedAttributeFunctionToolStripMenuItem.Click += new System.EventHandler(this.calculatedAttributeFunctionToolStripMenuItem_Click);
            // 
            // validateToolStripMenuItem
            // 
            this.validateToolStripMenuItem.CheckOnClick = true;
            this.validateToolStripMenuItem.Name = "validateToolStripMenuItem";
            this.validateToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.validateToolStripMenuItem.Text = "Validate";
            this.validateToolStripMenuItem.CheckedChanged += new System.EventHandler(this.validateToolStripMenuItem_CheckedChanged);
            // 
            // checkSpellingToolStripMenuItem
            // 
            this.checkSpellingToolStripMenuItem.Name = "checkSpellingToolStripMenuItem";
            this.checkSpellingToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.checkSpellingToolStripMenuItem.Text = "Check Spelling";
            this.checkSpellingToolStripMenuItem.Click += new System.EventHandler(this.checkSpellingToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sliceEntitiesToolStripMenuItem,
            this.convertToDecimalToolStripMenuItem,
            this.aggregatesToolStripMenuItem,
            this.convertUomToToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.MouseEnter += new System.EventHandler(this.toolsToolStripMenuItem_MouseEnter);
            // 
            // sliceEntitiesToolStripMenuItem
            // 
            this.sliceEntitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sliceDelimiterToolStripMenuItem,
            this.sliceSelectedEntitysInPlaceToolStripMenuItem,
            this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem});
            this.sliceEntitiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sliceEntitiesToolStripMenuItem.Image")));
            this.sliceEntitiesToolStripMenuItem.Name = "sliceEntitiesToolStripMenuItem";
            this.sliceEntitiesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.sliceEntitiesToolStripMenuItem.Text = "&Slice entity(s)";
            // 
            // sliceDelimiterToolStripMenuItem
            // 
            this.sliceDelimiterToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.sliceDelimiterToolStripMenuItem.LabelText = "Delimiter:";
            this.sliceDelimiterToolStripMenuItem.Name = "sliceDelimiterToolStripMenuItem";
            this.sliceDelimiterToolStripMenuItem.Size = new System.Drawing.Size(171, 21);
            // 
            // sliceSelectedEntitysInPlaceToolStripMenuItem
            // 
            this.sliceSelectedEntitysInPlaceToolStripMenuItem.Name = "sliceSelectedEntitysInPlaceToolStripMenuItem";
            this.sliceSelectedEntitysInPlaceToolStripMenuItem.Size = new System.Drawing.Size(292, 22);
            this.sliceSelectedEntitysInPlaceToolStripMenuItem.Text = "Slice selected entity(s) in place";
            this.sliceSelectedEntitysInPlaceToolStripMenuItem.Click += new System.EventHandler(this.sliceSelectedEntitysInPlaceToolStripMenuItem_Click);
            // 
            // sliceSelectedEntitysIntoNewAttributesToolStripMenuItem
            // 
            this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem.Name = "sliceSelectedEntitysIntoNewAttributesToolStripMenuItem";
            this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem.Size = new System.Drawing.Size(292, 22);
            this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem.Text = "Slice selected entity(s) into new attributes";
            this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem.Click += new System.EventHandler(this.sliceSelectedEntitysIntoNewAttributesToolStripMenuItem_Click);
            // 
            // convertToDecimalToolStripMenuItem
            // 
            this.convertToDecimalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noOfDecimalPlacesToolStripMenuItem,
            this.convertSelectedEntitysToDecimalToolStripMenuItem});
            this.convertToDecimalToolStripMenuItem.Name = "convertToDecimalToolStripMenuItem";
            this.convertToDecimalToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.convertToDecimalToolStripMenuItem.Text = "Convert to &Decimal";
            // 
            // noOfDecimalPlacesToolStripMenuItem
            // 
            this.noOfDecimalPlacesToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.noOfDecimalPlacesToolStripMenuItem.LabelText = "No. of Decimal Places:";
            this.noOfDecimalPlacesToolStripMenuItem.Name = "noOfDecimalPlacesToolStripMenuItem";
            this.noOfDecimalPlacesToolStripMenuItem.Size = new System.Drawing.Size(236, 21);
            // 
            // convertSelectedEntitysToDecimalToolStripMenuItem
            // 
            this.convertSelectedEntitysToDecimalToolStripMenuItem.Name = "convertSelectedEntitysToDecimalToolStripMenuItem";
            this.convertSelectedEntitysToDecimalToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.convertSelectedEntitysToDecimalToolStripMenuItem.Text = "&Convert selected entity(s) to Decimal";
            this.convertSelectedEntitysToDecimalToolStripMenuItem.Click += new System.EventHandler(this.convertSelectedEntitysToDecimalToolStripMenuItem_Click);
            // 
            // aggregatesToolStripMenuItem
            // 
            this.aggregatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aggregateFunctionToolStripMenuItem,
            this.extractToNewAttributeToolStripMenuItem});
            this.aggregatesToolStripMenuItem.Name = "aggregatesToolStripMenuItem";
            this.aggregatesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.aggregatesToolStripMenuItem.Text = "&Aggregates";
            // 
            // aggregateFunctionToolStripMenuItem
            // 
            this.aggregateFunctionToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.aggregateFunctionToolStripMenuItem.LabelText = "Function:";
            this.aggregateFunctionToolStripMenuItem.Name = "aggregateFunctionToolStripMenuItem";
            this.aggregateFunctionToolStripMenuItem.Size = new System.Drawing.Size(172, 21);
            // 
            // extractToNewAttributeToolStripMenuItem
            // 
            this.extractToNewAttributeToolStripMenuItem.Name = "extractToNewAttributeToolStripMenuItem";
            this.extractToNewAttributeToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.extractToNewAttributeToolStripMenuItem.Text = "&Extract to new attribute";
            this.extractToNewAttributeToolStripMenuItem.Click += new System.EventHandler(this.extractToNewAttributeToolStripMenuItem_Click);
            // 
            // convertUomToToolStripMenuItem
            // 
            this.convertUomToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertToUomDropDown,
            this.convertUomPrecisionDropDown,
            this.convertUnitOfMeasureToolStripMenuItem});
            this.convertUomToToolStripMenuItem.Enabled = false;
            this.convertUomToToolStripMenuItem.Name = "convertUomToToolStripMenuItem";
            this.convertUomToToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.convertUomToToolStripMenuItem.Text = "Convert Uom to";
            // 
            // convertToUomDropDown
            // 
            this.convertToUomDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.convertToUomDropDown.Name = "convertToUomDropDown";
            this.convertToUomDropDown.Size = new System.Drawing.Size(121, 23);
            this.convertToUomDropDown.SelectedIndexChanged += new System.EventHandler(this.convertToUomComboBox_SelectedIndexChanged);
            // 
            // convertUomPrecisionDropDown
            // 
            this.convertUomPrecisionDropDown.BackColor = System.Drawing.Color.Transparent;
            this.convertUomPrecisionDropDown.LabelText = "&Precision:";
            this.convertUomPrecisionDropDown.Name = "convertUomPrecisionDropDown";
            this.convertUomPrecisionDropDown.Size = new System.Drawing.Size(179, 23);
            // 
            // convertUnitOfMeasureToolStripMenuItem
            // 
            this.convertUnitOfMeasureToolStripMenuItem.Name = "convertUnitOfMeasureToolStripMenuItem";
            this.convertUnitOfMeasureToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.convertUnitOfMeasureToolStripMenuItem.Text = "Convert Unit of Measure";
            this.convertUnitOfMeasureToolStripMenuItem.Click += new System.EventHandler(this.convertUnitOfMeasureToolStripMenuItem_Click);
            // 
            // snippetTimer
            // 
            this.snippetTimer.Enabled = true;
            this.snippetTimer.Interval = 500;
            this.snippetTimer.Tick += new System.EventHandler(this.snippetTimer_Tick);
            // 
            // saveFileDialogXml
            // 
            this.saveFileDialogXml.DefaultExt = "xml";
            this.saveFileDialogXml.Filter = "XML files|*.xml|All files|*.*";
            // 
            // openFileDialogXml
            // 
            this.openFileDialogXml.DefaultExt = "xml";
            this.openFileDialogXml.Filter = "XML files|*.xml|All files|*.*";
            // 
            // selectionChangedTimer
            // 
            this.selectionChangedTimer.Interval = 200;
            this.selectionChangedTimer.Tick += new System.EventHandler(this.selectionChangedTimer_Tick);
            // 
            // contextMenuStripWorkflow
            // 
            this.contextMenuStripWorkflow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.approveMenu,
            this.unapproveMenu});
            this.contextMenuStripWorkflow.Name = "contextMenuStripWorkflow";
            this.contextMenuStripWorkflow.Size = new System.Drawing.Size(140, 48);
            // 
            // approveMenu
            // 
            this.approveMenu.Name = "approveMenu";
            this.approveMenu.Size = new System.Drawing.Size(139, 22);
            this.approveMenu.Text = "Approved";
            this.approveMenu.Click += new System.EventHandler(this.approveMenu_Click);
            // 
            // unapproveMenu
            // 
            this.unapproveMenu.Name = "unapproveMenu";
            this.unapproveMenu.Size = new System.Drawing.Size(139, 22);
            this.unapproveMenu.Text = "Unapproved";
            this.unapproveMenu.Click += new System.EventHandler(this.unapproveMenu_Click);
            // 
            // skuImageBox
            // 
            this.skuImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.skuImageBox.Location = new System.Drawing.Point(300, 200);
            this.skuImageBox.MaximumSize = new System.Drawing.Size(300, 300);
            this.skuImageBox.MinimumSize = new System.Drawing.Size(10, 100);
            this.skuImageBox.Name = "skuImageBox";
            this.skuImageBox.Size = new System.Drawing.Size(100, 100);
            this.skuImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.skuImageBox.TabIndex = 4;
            this.skuImageBox.TabStop = false;
            this.skuImageBox.Visible = false;
            this.skuImageBox.Click += new System.EventHandler(this.skuImageBox_Click);
            this.skuImageBox.DoubleClick += new System.EventHandler(this.skuImageBox_DoubleClick);
            // 
            // tblRibbon
            // 
            this.tblRibbon.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tblRibbon.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblRibbon.Location = new System.Drawing.Point(0, 24);
            this.tblRibbon.Name = "tblRibbon";
            // 
            // tblRibbon.Panel1
            // 
            this.tblRibbon.Panel1.Controls.Add(this.splitContainer1);
            // 
            // tblRibbon.Panel2
            // 
            this.tblRibbon.Panel2.Controls.Add(this.tableLayoutPanel4);
            this.tblRibbon.Size = new System.Drawing.Size(1024, 112);
            this.tblRibbon.SplitterDistance = 732;
            this.tblRibbon.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tblEntity);
            this.splitContainer1.Size = new System.Drawing.Size(732, 112);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.tblAttributeOrders, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.tblTools, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(198, 108);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel4.Controls.Add(this.pnlBeforeEntity, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(284, 108);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // EntityDataGridView
            // 
            this.Controls.Add(this.skuImageBox);
            this.Controls.Add(this._newAttributeDropDown);
            this.Controls.Add(this.pnlDgvs);
            this.Controls.Add(this.tblRibbon);
            this.Controls.Add(this.mainMenuStrip);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(640, 320);
            this.Name = "EntityDataGridView";
            this.Size = new System.Drawing.Size(1024, 600);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EntityGridView_KeyDown);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.EntityDataGridView_Layout);
            this.sortContextMenu.ResumeLayout(false);
            this.filterContextMenu.ResumeLayout(false);
            this.pnlBeforeEntity.ResumeLayout(false);
            this.pnlBeforeEntity.PerformLayout();
            this.tblEntity.ResumeLayout(false);
            this.tblEntity.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tblTools.ResumeLayout(false);
            this.tblAttributeOrders.ResumeLayout(false);
            this.tblAttributeOrders.PerformLayout();
            this.pnlDgvs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.edgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditTrail)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.contextMenuStripWorkflow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.skuImageBox)).EndInit();
            this.tblRibbon.Panel1.ResumeLayout(false);
            this.tblRibbon.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tblRibbon)).EndInit();
            this.tblRibbon.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal CustomDataGridView edgv;
        private ComboBox _newAttributeDropDown;
        private ContextMenuStrip sortContextMenu;
        private DataGridView dgvAuditTrail;
        private StatusStrip statusStrip1;
        private Timer repopulateGridViewTimer;
        private Timer statusTimer;
        private ToolStripComboBox sortByComboToolStripMenuItem;
        private ToolStripMenuItem numericSortToolStripMenuItem;
        private ToolStripMenuItem sortAscendingToolStripMenuItem;
        private ToolStripMenuItem sortDescendingToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripStatusLabel lblStatus;
        private ToolTip mainToolTip;
        private TableLayoutPanel tblAttributeOrders;
        private CheckBox chkNavOrder;
        private ComboBox txtField5;
        private Label lblNormalName;
        private ComboBox txtField4;
        private CheckBox chkField1;
        private ComboBox txtUom;
        private ComboBox txtField3;
        private ComboBox txtValue;
        private CheckBox chkField2;
        private ComboBox cboAttributeName;
        private CheckBox chkField3;
        private ComboBox txtField2;
        private Label lblAttributeValue;
        private ComboBox txtField1;
        private TableLayoutPanel tblTools;
        private Button btnReOrderColumns;
        private Button btnFilter;
        private Button btnShowHideAttributes;
        private Button btnChangeTaxonomy;
        private CheckBox btnShowHideAuditTrail;
        private Button btnSort;
        private Button btnFind;
        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem reloadToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exportCurrentViewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem findReplaceToolStripMenuItem;
        private ToolStripMenuItem sortToolStripMenuItem;
        private ToolStripMenuItem filterToolStripMenuItem;
        private ContextMenuStrip filterContextMenu;
        private ToolStripMenuItem filterAttributeToolStripMenuItem;
        private ToolStripMenuItem filterValueToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem clearThisFilterToolStripMenuItem;
        private ToolStripMenuItem clearAllFiltersToolStripMenuItem;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem moveSKUToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem newEntitiesToolStripMenuItem;
        private ToolStripMenuItem deleteSelectedEntitiesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem calculatedAttributeFunctionToolStripMenuItem;
        private ToolStripMenuItem showHideAttributesToolStripMenuItem;
        private ToolStripMenuItem reorderColumnsToolStripMenuItem;
        private ToolStripMenuItem auditTrailToolStripMenuItem;
        private Timer snippetTimer;
        private ToolStripMenuItem aryaBrowserToolStripMenuItem;
        private ToolStripMenuItem showItemOnClientWebsiteToolStripMenuItem;
        private ToolStripMenuItem showURLsFromDataToolStripMenuItem;
        private ToolStripMenuItem showResearchPagesToolStripMenuItem;
        private Button btnColor;
        private ToolStripMenuItem colorizeToolStripMenuItem;
        private ToolStripMenuItem openInNewTabToolStripMenuItem;
        private ToolStripMenuItem sliceEntitiesToolStripMenuItem;
        private LabelWithComboToolStripMenuItem sliceDelimiterToolStripMenuItem;
        private ToolStripMenuItem sliceSelectedEntitysInPlaceToolStripMenuItem;
        private ToolStripComboBox filterByComboToolStripMenuItem;
        private ToolStripMenuItem useShiftToForceSortByValueToolStripMenuItem;
        private FlowLayoutPanel pnlBeforeEntity;
        private Label lblBeforeAttribute;
        private Label lblBeforeValue;
        private Label lblBeforeUom;
        private Label lblBeforeField1;
        private Label lblBeforeField2;
        private Label lblBeforeField3;
        private Label lblBeforeField4;
        private Label lblBeforeField5;
        private Label lblBeforeDate;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem convertToDecimalToolStripMenuItem;
        private ToolStripMenuItem convertSelectedEntitysToDecimalToolStripMenuItem;
        private LabelWithComboToolStripMenuItem noOfDecimalPlacesToolStripMenuItem;
        private ToolStripMenuItem sliceSelectedEntitysIntoNewAttributesToolStripMenuItem;
        private ToolStripMenuItem aggregatesToolStripMenuItem;
        private ToolStripMenuItem extractToNewAttributeToolStripMenuItem;
        private LabelWithComboToolStripMenuItem aggregateFunctionToolStripMenuItem;
        private ToolStripMenuItem openInSchemaViewToolStripMenuItem;
        private ToolStripMenuItem openCurrentNodeInSchemaViewToolStripMenuItem;
        private ToolStripMenuItem openAllNodesInSchemaViewToolStripMenuItem;
        private PictureBox skuImageBox;
        private TableLayoutPanel tblEntity;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label lblBeforeEntity;
        private CheckBox chkShowHideLinks;
        private ToolStripMenuItem showHideSKULinksToolStripMenuItem;
        private Panel pnlDgvs;
        private Splitter splitterAuditTrail;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem saveCurrentFiltersToolStripMenuItem;
        private ToolStripMenuItem loadFiltersFromFileToolStripMenuItem;
        private SaveFileDialog saveFileDialogXml;
        private OpenFileDialog openFileDialogXml;
        private Label lblAttributeOrders;
        private CheckBox chkDispOrder;
        private TextBox txtDisplayOrder;
        private TextBox txtNavigationOrder;
        private CheckBox chkSuggestValues;
        private CheckBox chkUom;
        private CheckBox chkField4;
        private CheckBox chkField5;
        private ToolStripMenuItem showItemImagesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private Label lblAttributeDefinition;
        private ToolStripMenuItem convertUomToToolStripMenuItem;
        private ToolStripComboBox convertToUomDropDown;
        private Label lblCreatedRemark;
        private ToolStripMenuItem openInBuildViewToolStripMenuItem;
        private Timer selectionChangedTimer;
        private Label lblCurrentChange;
        private ToolStripMenuItem groupSKUsToolStripMenuItem;
        private ToolStripMenuItem notesToolStripMenuItem;
        private ToolStripMenuItem addToWorkflowToolStripMenuItem;
        private ContextMenuStrip contextMenuStripWorkflow;
        private ToolStripMenuItem approveMenu;
        private ToolStripMenuItem unapproveMenu;
        private CheckBox btnWrkflow;
        private ToolStripMenuItem validateToolStripMenuItem;
        private Label lblDerivedAttributeText;
        private ToolStripMenuItem loadAllImagesToolStripMenuItem;
        private LabelWithComboToolStripMenuItem convertUomPrecisionDropDown;
        private ToolStripMenuItem convertUnitOfMeasureToolStripMenuItem;
        private ToolStripMenuItem showHideAttributesToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem showProductAttributesToolStripMenuItem;
        private ToolStripMenuItem showInSchemaAttributesToolStripMenuItem;
        private ToolStripMenuItem showRankedAttributesToolStripMenuItem;
        private ToolStripMenuItem attributePreferencesPropertiesToolStripMenuItem;
        private SplitContainer tblRibbon;
        private TableLayoutPanel tableLayoutPanel4;
        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel5;
        private ToolStripMenuItem checkSpellingToolStripMenuItem;
    }
}