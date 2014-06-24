using System.Windows.Forms;
using Arya.CustomControls;
using Arya.Framework.GUI.UserControls;

namespace Arya.UserControls
{
    partial class TaxonomyTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaxonomyTreeView));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.txtSearchBox = new System.Windows.Forms.TextBox();
            this.taxonomyTree = new Arya.Framework.GUI.UserControls.MultiSelectTreeView();
            this.txtNodeDefinition = new System.Windows.Forms.TextBox();
            this.pnlEnrichments = new System.Windows.Forms.Panel();
            this.txtEnrichmentCopy = new System.Windows.Forms.TextBox();
            this.txtEnrichmentImage = new System.Windows.Forms.TextBox();
            this.treeNodeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.customActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInSkuViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regularToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crosslistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteThisNodeOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteThisNodeAndAllChildNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.jumpToParentNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crossListNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCrossListRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideEmptyChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshSkuCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToWorkflowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateNodeTimer = new System.Windows.Forms.Timer(this.components);
            this.tmrSearch = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlEnrichments.SuspendLayout();
            this.treeNodeContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lstResults, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtSearchBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.taxonomyTree, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtNodeDefinition, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pnlEnrichments, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(239, 552);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lstResults
            // 
            this.lstResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstResults.FormattingEnabled = true;
            this.lstResults.HorizontalScrollbar = true;
            this.lstResults.IntegralHeight = false;
            this.lstResults.Location = new System.Drawing.Point(3, 29);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(233, 94);
            this.lstResults.TabIndex = 2;
            this.lstResults.Visible = false;
            this.lstResults.SelectedIndexChanged += new System.EventHandler(this.lstResults_SelectedIndexChanged);
            this.lstResults.VisibleChanged += new System.EventHandler(this.lstResults_VisibleChanged);
            // 
            // txtSearchBox
            // 
            this.txtSearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchBox.Location = new System.Drawing.Point(3, 3);
            this.txtSearchBox.Name = "txtSearchBox";
            this.txtSearchBox.Size = new System.Drawing.Size(233, 20);
            this.txtSearchBox.TabIndex = 1;
            this.txtSearchBox.TextChanged += new System.EventHandler(this.txtSearchBox_TextChanged);
            this.txtSearchBox.Enter += new System.EventHandler(this.txtSearchBox_Enter);
            this.txtSearchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchBox_KeyDown);
            this.txtSearchBox.Leave += new System.EventHandler(this.txtSearchBox_Leave);
            // 
            // taxonomyTree
            // 
            this.taxonomyTree.BackColor = System.Drawing.SystemColors.Window;
            this.taxonomyTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taxonomyTree.DrawBoldSelectedNode = false;
            this.taxonomyTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.taxonomyTree.Location = new System.Drawing.Point(3, 349);
            this.taxonomyTree.Name = "taxonomyTree";
            this.taxonomyTree.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("taxonomyTree.SelectedNodes")));
            this.taxonomyTree.Size = new System.Drawing.Size(233, 200);
            this.taxonomyTree.TabIndex = 0;
            this.taxonomyTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.taxonomyTree_AfterExpand);
            this.taxonomyTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.taxonomyTree_BeforeSelect);
            this.taxonomyTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.taxonomyTreeView_AfterSelect);
            this.taxonomyTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.taxonomyTreeView_NodeMouseClick);
            this.taxonomyTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.taxonomyTreeView_NodeMouseDoubleClick);
            this.taxonomyTree.Enter += new System.EventHandler(this.taxonomyTree_Enter);
            this.taxonomyTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.taxonomyTree_KeyDown);
            // 
            // txtNodeDefinition
            // 
            this.txtNodeDefinition.AcceptsReturn = true;
            this.txtNodeDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNodeDefinition.Location = new System.Drawing.Point(3, 129);
            this.txtNodeDefinition.Multiline = true;
            this.txtNodeDefinition.Name = "txtNodeDefinition";
            this.txtNodeDefinition.ReadOnly = true;
            this.txtNodeDefinition.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNodeDefinition.Size = new System.Drawing.Size(233, 94);
            this.txtNodeDefinition.TabIndex = 3;
            this.txtNodeDefinition.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBoxNodeDefinition_MouseClick);
            this.txtNodeDefinition.ReadOnlyChanged += new System.EventHandler(this.textBoxNodeDefinition_ReadOnlyChanged);
            this.txtNodeDefinition.Leave += new System.EventHandler(this.textBoxNodeDefinition_Leave);
            // 
            // pnlEnrichments
            // 
            this.pnlEnrichments.BackColor = System.Drawing.SystemColors.Control;
            this.pnlEnrichments.Controls.Add(this.txtEnrichmentCopy);
            this.pnlEnrichments.Controls.Add(this.txtEnrichmentImage);
            this.pnlEnrichments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEnrichments.Location = new System.Drawing.Point(3, 229);
            this.pnlEnrichments.Name = "pnlEnrichments";
            this.pnlEnrichments.Size = new System.Drawing.Size(233, 114);
            this.pnlEnrichments.TabIndex = 6;
            this.pnlEnrichments.Visible = false;
            // 
            // txtEnrichmentCopy
            // 
            this.txtEnrichmentCopy.AcceptsReturn = true;
            this.txtEnrichmentCopy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEnrichmentCopy.Location = new System.Drawing.Point(0, 20);
            this.txtEnrichmentCopy.Multiline = true;
            this.txtEnrichmentCopy.Name = "txtEnrichmentCopy";
            this.txtEnrichmentCopy.ReadOnly = true;
            this.txtEnrichmentCopy.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEnrichmentCopy.Size = new System.Drawing.Size(233, 94);
            this.txtEnrichmentCopy.TabIndex = 10;
            this.txtEnrichmentCopy.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtEnrichmentCopy_MouseClick);
            this.txtEnrichmentCopy.ReadOnlyChanged += new System.EventHandler(this.txtEnrichmentCopy_ReadOnlyChanged);
            this.txtEnrichmentCopy.Leave += new System.EventHandler(this.txtEnrichmentCopy_Leave);
            // 
            // txtEnrichmentImage
            // 
            this.txtEnrichmentImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtEnrichmentImage.Location = new System.Drawing.Point(0, 0);
            this.txtEnrichmentImage.Name = "txtEnrichmentImage";
            this.txtEnrichmentImage.ReadOnly = true;
            this.txtEnrichmentImage.Size = new System.Drawing.Size(233, 20);
            this.txtEnrichmentImage.TabIndex = 9;
            this.txtEnrichmentImage.DoubleClick += new System.EventHandler(this.txtEnrichmentImage_DoubleClick);
            this.txtEnrichmentImage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEnrichmentImage_KeyDown);
            // 
            // treeNodeContextMenuStrip
            // 
            this.treeNodeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customActionToolStripMenuItem,
            this.openInSkuViewMenuItem,
            this.toolStripSeparator1,
            this.moveNodeToolStripMenuItem,
            this.renameNodeToolStripMenuItem,
            this.addChildNodeToolStripMenuItem,
            this.deleteNodeToolStripMenuItem,
            this.toolStripSeparator4,
            this.jumpToParentNodeToolStripMenuItem,
            this.crossListNodeToolStripMenuItem,
            this.editCrossListRuleToolStripMenuItem,
            this.toolStripSeparator2,
            this.expandAllChildrenToolStripMenuItem,
            this.collapseAllChildrenToolStripMenuItem,
            this.hideEmptyChildrenToolStripMenuItem,
            this.showAllNodesToolStripMenuItem,
            this.toolStripSeparator3,
            this.refreshSkuCountToolStripMenuItem,
            this.exportTreeToolStripMenuItem,
            this.addToWorkflowToolStripMenuItem});
            this.treeNodeContextMenuStrip.Name = "treeNodeContextMenuStrip";
            this.treeNodeContextMenuStrip.ShowImageMargin = false;
            this.treeNodeContextMenuStrip.Size = new System.Drawing.Size(172, 380);
            // 
            // customActionToolStripMenuItem
            // 
            this.customActionToolStripMenuItem.Name = "customActionToolStripMenuItem";
            this.customActionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.customActionToolStripMenuItem.Text = "Custom Action";
            this.customActionToolStripMenuItem.Click += new System.EventHandler(this.customActionToolStripMenuItem_Click);
            // 
            // openInSkuViewMenuItem
            // 
            this.openInSkuViewMenuItem.Name = "openInSkuViewMenuItem";
            this.openInSkuViewMenuItem.Size = new System.Drawing.Size(171, 22);
            this.openInSkuViewMenuItem.Text = "Open";
            this.openInSkuViewMenuItem.Click += new System.EventHandler(this.openInSkuViewMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
            // 
            // moveNodeToolStripMenuItem
            // 
            this.moveNodeToolStripMenuItem.Name = "moveNodeToolStripMenuItem";
            this.moveNodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.moveNodeToolStripMenuItem.Text = "Move node";
            this.moveNodeToolStripMenuItem.Click += new System.EventHandler(this.moveNodeToolStripMenuItem_Click);
            // 
            // renameNodeToolStripMenuItem
            // 
            this.renameNodeToolStripMenuItem.Name = "renameNodeToolStripMenuItem";
            this.renameNodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.renameNodeToolStripMenuItem.Text = "Rename node";
            this.renameNodeToolStripMenuItem.Click += new System.EventHandler(this.renameNodeToolStripMenuItem_Click);
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regularToolStripMenuItem,
            this.crosslistToolStripMenuItem});
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.addChildNodeToolStripMenuItem.Text = "Add child node";
            // 
            // regularToolStripMenuItem
            // 
            this.regularToolStripMenuItem.Name = "regularToolStripMenuItem";
            this.regularToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.regularToolStripMenuItem.Text = "Regular";
            this.regularToolStripMenuItem.Click += new System.EventHandler(this.regularToolStripMenuItem_Click);
            // 
            // crosslistToolStripMenuItem
            // 
            this.crosslistToolStripMenuItem.Name = "crosslistToolStripMenuItem";
            this.crosslistToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.crosslistToolStripMenuItem.Text = "Cross-list";
            this.crosslistToolStripMenuItem.Click += new System.EventHandler(this.crosslistToolStripMenuItem_Click);
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteThisNodeOnlyToolStripMenuItem,
            this.deleteThisNodeAndAllChildNodesToolStripMenuItem});
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.deleteNodeToolStripMenuItem.Text = "Delete node";
            // 
            // deleteThisNodeOnlyToolStripMenuItem
            // 
            this.deleteThisNodeOnlyToolStripMenuItem.Name = "deleteThisNodeOnlyToolStripMenuItem";
            this.deleteThisNodeOnlyToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.deleteThisNodeOnlyToolStripMenuItem.Text = "Delete this node only - roll child nodes up";
            this.deleteThisNodeOnlyToolStripMenuItem.Click += new System.EventHandler(this.deleteThisNodeOnlyToolStripMenuItem_Click);
            // 
            // deleteThisNodeAndAllChildNodesToolStripMenuItem
            // 
            this.deleteThisNodeAndAllChildNodesToolStripMenuItem.Name = "deleteThisNodeAndAllChildNodesToolStripMenuItem";
            this.deleteThisNodeAndAllChildNodesToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.deleteThisNodeAndAllChildNodesToolStripMenuItem.Text = "Delete this node and all child nodes";
            this.deleteThisNodeAndAllChildNodesToolStripMenuItem.Click += new System.EventHandler(this.deleteThisNodeAndAllChildNodesToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(168, 6);
            // 
            // jumpToParentNodeToolStripMenuItem
            // 
            this.jumpToParentNodeToolStripMenuItem.Name = "jumpToParentNodeToolStripMenuItem";
            this.jumpToParentNodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.jumpToParentNodeToolStripMenuItem.Text = "Jump to parent node";
            this.jumpToParentNodeToolStripMenuItem.Click += new System.EventHandler(this.jumpToParentNodeToolStripMenuItem_Click);
            // 
            // crossListNodeToolStripMenuItem
            // 
            this.crossListNodeToolStripMenuItem.Name = "crossListNodeToolStripMenuItem";
            this.crossListNodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.crossListNodeToolStripMenuItem.Text = "Cross-List Node";
            this.crossListNodeToolStripMenuItem.Click += new System.EventHandler(this.crossListNodeToolStripMenuItem_Click);
            // 
            // editCrossListRuleToolStripMenuItem
            // 
            this.editCrossListRuleToolStripMenuItem.Name = "editCrossListRuleToolStripMenuItem";
            this.editCrossListRuleToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.editCrossListRuleToolStripMenuItem.Text = "Edit cross list definition";
            this.editCrossListRuleToolStripMenuItem.Click += new System.EventHandler(this.addCrossReferenceRuleToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
            // 
            // expandAllChildrenToolStripMenuItem
            // 
            this.expandAllChildrenToolStripMenuItem.Name = "expandAllChildrenToolStripMenuItem";
            this.expandAllChildrenToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.expandAllChildrenToolStripMenuItem.Text = "Expand all children";
            this.expandAllChildrenToolStripMenuItem.Click += new System.EventHandler(this.expandAllChildrenToolStripMenuItem_Click);
            // 
            // collapseAllChildrenToolStripMenuItem
            // 
            this.collapseAllChildrenToolStripMenuItem.Name = "collapseAllChildrenToolStripMenuItem";
            this.collapseAllChildrenToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.collapseAllChildrenToolStripMenuItem.Text = "Collapse all children";
            this.collapseAllChildrenToolStripMenuItem.Click += new System.EventHandler(this.collapseAllChildrenToolStripMenuItem_Click);
            // 
            // hideEmptyChildrenToolStripMenuItem
            // 
            this.hideEmptyChildrenToolStripMenuItem.Name = "hideEmptyChildrenToolStripMenuItem";
            this.hideEmptyChildrenToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.hideEmptyChildrenToolStripMenuItem.Text = "Hide empty children";
            this.hideEmptyChildrenToolStripMenuItem.Click += new System.EventHandler(this.hideEmptyChildrenToolStripMenuItem_Click);
            // 
            // showAllNodesToolStripMenuItem
            // 
            this.showAllNodesToolStripMenuItem.Name = "showAllNodesToolStripMenuItem";
            this.showAllNodesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.showAllNodesToolStripMenuItem.Text = "Show all nodes";
            this.showAllNodesToolStripMenuItem.Visible = false;
            this.showAllNodesToolStripMenuItem.Click += new System.EventHandler(this.showAllNodesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(168, 6);
            // 
            // refreshSkuCountToolStripMenuItem
            // 
            this.refreshSkuCountToolStripMenuItem.Name = "refreshSkuCountToolStripMenuItem";
            this.refreshSkuCountToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.refreshSkuCountToolStripMenuItem.Text = "Refresh sku count";
            this.refreshSkuCountToolStripMenuItem.Click += new System.EventHandler(this.upateCountsToolStripMenuItem_Click);
            // 
            // exportTreeToolStripMenuItem
            // 
            this.exportTreeToolStripMenuItem.Name = "exportTreeToolStripMenuItem";
            this.exportTreeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.exportTreeToolStripMenuItem.Text = "Export tree from here";
            this.exportTreeToolStripMenuItem.Click += new System.EventHandler(this.exportTreeToolStripMenuItem_Click);
            // 
            // addToWorkflowToolStripMenuItem
            // 
            this.addToWorkflowToolStripMenuItem.Name = "addToWorkflowToolStripMenuItem";
            this.addToWorkflowToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            // 
            // updateNodeTimer
            // 
            this.updateNodeTimer.Interval = 1000;
            this.updateNodeTimer.Tick += new System.EventHandler(this.updateNodeTimer_Tick);
            // 
            // tmrSearch
            // 
            this.tmrSearch.Interval = 1500;
            this.tmrSearch.Tick += new System.EventHandler(this.tmrSearch_Tick);
            // 
            // TaxonomyTreeView
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TaxonomyTreeView";
            this.Size = new System.Drawing.Size(239, 552);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlEnrichments.ResumeLayout(false);
            this.pnlEnrichments.PerformLayout();
            this.treeNodeContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip treeNodeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem renameNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChildNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllChildrenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInSkuViewMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem moveNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem customActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
        private ToolStripMenuItem collapseAllChildrenToolStripMenuItem;
        internal Timer updateNodeTimer;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem hideEmptyChildrenToolStripMenuItem;
        private ToolStripMenuItem showAllNodesToolStripMenuItem;
        private ToolStripMenuItem editCrossListRuleToolStripMenuItem;
        private ToolStripMenuItem refreshSkuCountToolStripMenuItem;
        private ToolStripMenuItem regularToolStripMenuItem;
        private ToolStripMenuItem crosslistToolStripMenuItem;
        private ToolStripMenuItem deleteThisNodeOnlyToolStripMenuItem;
        private ToolStripMenuItem deleteThisNodeAndAllChildNodesToolStripMenuItem;
        private ToolStripMenuItem crossListNodeToolStripMenuItem;
        private ToolStripMenuItem jumpToParentNodeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private Timer tmrSearch;
        private ToolStripMenuItem addToWorkflowToolStripMenuItem;
        private ListBox lstResults;
        private TextBox txtSearchBox;
        private MultiSelectTreeView taxonomyTree;
        public TextBox txtNodeDefinition;
        private Panel pnlEnrichments;
        public TextBox txtEnrichmentCopy;
        private TextBox txtEnrichmentImage;
    }
}
