using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Arya.Framework.GUI.UserControls
{
    public sealed class MultiSelectTreeView : TreeView
    {
        #region Fields (3)

        TreeNode firstNode;
        List<TreeNode> selectedNodes;

        private IContainer components;
        private readonly List<TreeNode> indeterminateds = new List<TreeNode>();
        private Graphics graphics;
        private readonly Image imgIndeterminate;
        private ImageList checkBoxImages;
        private bool skipCheckEvents = false;

        #endregion Fields

        #region Constructors (1)

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiSelectTreeView));
            this.checkBoxImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // checkBoxImages
            // 
            this.checkBoxImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("checkBoxImages.ImageStream")));
            this.checkBoxImages.TransparentColor = System.Drawing.Color.Transparent;
            this.checkBoxImages.Images.SetKeyName(0, "bullet_green.png");
            this.checkBoxImages.Images.SetKeyName(1, "bullet_red.png");
            this.checkBoxImages.Images.SetKeyName(2, "bullet_yellow.png");

            // 
            // MultiSelectTreeView
            // 
            this.LineColor = Color.Black;
            this.ResumeLayout(false);

        }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public new ImageList StateImageList
        //{
        //    get { return base.StateImageList; }
        //    set { }
        //}

        public MultiSelectTreeView()
        {
            InitializeComponent();
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            imgIndeterminate = checkBoxImages.Images[2];
            StateImageList = checkBoxImages;
            selectedNodes = new List<TreeNode>();

        }

        #endregion Constructors

        #region Properties (1)

        public List<TreeNode> SelectedNodes
        {
            get { return selectedNodes; }
            set
            {
                RemovePaintFromSelectedNodes();
                selectedNodes.Clear();
                selectedNodes = value;
                AddPaintToSelectedNodes();
            }
        }

        #endregion Properties

        #region Methods (8)

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            bool bControl = (ModifierKeys & Keys.Control) == Keys.Control;
            bool bShift = (ModifierKeys & Keys.Shift) == Keys.Shift;

            if (bShift)
            {
                var myQueue = new Queue<TreeNode>();

                TreeNode uppernode = firstNode;
                TreeNode bottomnode = e.Node;
                // case 1 : begin and end nodes are parent
                bool bParent = IsParent(firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                if (!bParent)
                {
                    bParent = IsParent(bottomnode, uppernode);
                    if (bParent) // swap nodes
                    {
                        TreeNode t = uppernode;
                        uppernode = bottomnode;
                        bottomnode = t;
                    }
                }
                if (bParent)
                {
                    TreeNode n = bottomnode;
                    while (uppernode != null && n != uppernode.Parent)
                    {
                        if (!selectedNodes.Contains(n)) // new node ?
                            myQueue.Enqueue(n);

                        n = n.Parent;
                    }
                }
                // case 2 : nor the begin nor the end node are descendant one another
                else if ((uppernode.Parent == null && bottomnode.Parent == null) ||
                         (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                {
                    int nIndexUpper = uppernode.Index;
                    int nIndexBottom = bottomnode.Index;
                    if (nIndexBottom < nIndexUpper) // reversed?
                    {
                        TreeNode t = uppernode;
                        uppernode = bottomnode;
                        bottomnode = t;
                        nIndexUpper = uppernode.Index;
                        nIndexBottom = bottomnode.Index;
                    }

                    TreeNode n = uppernode;
                    while (nIndexUpper <= nIndexBottom)
                    {
                        if (!selectedNodes.Contains(n)) // new node ?
                            myQueue.Enqueue(n);

                        n = n.NextNode;

                        nIndexUpper++;
                    } // end while
                }
                else
                {
                    if (!selectedNodes.Contains(uppernode))
                        myQueue.Enqueue(uppernode);
                    if (!selectedNodes.Contains(bottomnode))
                        myQueue.Enqueue(bottomnode);
                }

                selectedNodes.AddRange(myQueue);

                firstNode = e.Node; // let us chain several SHIFTs if we like it
            } // end if m_bShift
            else if (bControl)
            {
                if (!selectedNodes.Contains(e.Node)) // new node ?
                    selectedNodes.Add(e.Node);
                else // not new, remove it from the collection
                {
                    RemovePaintFromSelectedNodes();
                    selectedNodes.Remove(e.Node);
                }
            }
            else
            {
                // in the case of a simple click, just add this item
                if (selectedNodes != null && selectedNodes.Count > 0)
                {
                    RemovePaintFromSelectedNodes();
                    selectedNodes.Clear();
                }
                if (selectedNodes != null)
                    selectedNodes.Add(e.Node);
            }
            AddPaintToSelectedNodes();
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            bool bControl = (ModifierKeys & Keys.Control) == Keys.Control;
            bool bShift = ((ModifierKeys & Keys.Shift) == Keys.Shift);

            // selecting twice the node while pressing CTRL ?
            if (bControl && selectedNodes.Contains(e.Node))
            {
                // unselect it (let framework know we don't want selection this time)
                e.Cancel = true;

                // update nodes
                RemovePaintFromSelectedNodes();
                selectedNodes.Remove(e.Node);
                AddPaintToSelectedNodes();
                return;
            }

            if (!bShift)
                firstNode = e.Node; // store begin of shift sequence
        }

        // Private Methods (5) 

        static bool IsParent(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode == childNode)
                return true;

            TreeNode n = childNode;
            bool bFound = false;
            while (!bFound && n != null)
            {
                n = n.Parent;
                bFound = (n == parentNode);
            }
            return bFound;
        }

        void AddPaintToSelectedNodes()
        {
            PaintSelectedNodes(SystemColors.Highlight, SystemColors.HighlightText);
        }

        void RemovePaintFromSelectedNodes()
        {
            PaintSelectedNodes(null, null);
        }

        private void PaintSelectedNodes(Color? backColor, Color? foreColor)
        {
            foreach (var node in selectedNodes.Where(node => node.TreeView != null))
            {
                node.BackColor = backColor ?? node.TreeView.BackColor;
                node.ForeColor = foreColor ?? node.TreeView.ForeColor;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && graphics != null)
            {
                graphics.Dispose();
                graphics = null;
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            graphics = CreateGraphics();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (graphics != null)
            {
                graphics.Dispose();
                graphics = CreateGraphics();
            }
        }

        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            if (skipCheckEvents || !CheckBoxes) return;
            if ((e.Node.StateImageIndex == 0) == e.Node.Checked) return;
            /* suppress redundant 
             BeforeCheck-event, if an already checked node programmatically is set to checked */
            base.OnBeforeCheck(e);
        }

        public void RaiseOnBeforeCheck(TreeNode treeNode)
        {
            OnBeforeCheck(new TreeViewCancelEventArgs(treeNode,false,TreeViewAction.ByMouse));
        }

        public void RaiseOnAfterCheck(TreeNode treeNode)
        {
            OnAfterCheck(new TreeViewEventArgs(treeNode,TreeViewAction.ByMouse));
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            /* Logic: All children of an (un)checked Node inherit its Checkstate
             * Parents recompute their state: if all children of a parent have same state, that one 
             * will be taken over as parents state - otherwise take Indeterminate 
             */
            if (skipCheckEvents || !CheckBoxes) return;/* changing any Treenodes .Checked-Property will raise 
                                           another Before- and After-Check. Skip'em */
            skipCheckEvents = true;
            try
            {
                TreeNode nd = e.Node;
                /* uninitialized Nodes have StateImageIndex -1, so I associate StateImageIndex as follows:
                 * -1: Unchecked
                 *  0: Checked
                 *  1: Indeterminate
                 *  That corresponds to the System.Windows.Forms.Checkstate - enumeration, but 1 less.
                 *  Furthermore I ordered the images in that manner
                 */
                int state = nd.StateImageIndex == 0 ? -1 : 0;      /* this state is already toggled.
                Note: -1 (Unchecked) and 1 (Indeterminate) both toggle to 0, that means: Checked */
                if ((state == 0) != nd.Checked) return;       //suppress redundant AfterCheck-event
                InheritCheckstate(nd, state);         // inherit Checkstate to children
                // Parents recompute their state
                nd = nd.Parent;
                while (nd != null)
                {
                    // At Indeterminate (==1) skip the children-query - every parent becomes Indeterminate
                    if (state != 1)
                    {
                        int state1 = state;
                        if (nd.Nodes.Cast<TreeNode>().Any(ndChild => ndChild.StateImageIndex != state1))
                        {
                            state = 1;
                        }
                    }
                    AssignState(nd, state);
                    nd = nd.Parent;
                }
                base.OnAfterCheck(e);
            }
            finally { skipCheckEvents = false; }
        }

        //public void ToggleNode(TreeNode treeNode)
        //{
        //    /*
        //    * -1: Unchecked
        //    *  0: Checked
        //    *  1: Indeterminate
        //    */

        //    switch (treeNode.StateImageIndex)
        //    {
        //        case -1: treeNode.st
        //    }

        //}

        private void AssignState(TreeNode nd, int state)
        {
            bool ck = state == 0;
            bool stateInvalid = nd.StateImageIndex != state;
            if (stateInvalid) nd.StateImageIndex = state;
            if (nd.Checked != ck)
            {
                nd.Checked = ck;            // changing .Checked-Property raises Invalidating internally
            }
            else if (stateInvalid)
            {
                // in general: the less and small the invalidated area, the less flickering
                // so avoid calling Invalidate() if possible, and only call, if really needed.
                Invalidate(GetCheckRect(nd));
            }
        }

        public CheckState GetState(TreeNode nd)
        {
            // compute the System.Windows.Forms.CheckState from a StateImageIndex is not that complicated
            return (CheckState)nd.StateImageIndex + 1;
        }

        public bool DrawBoldSelectedNode { get; set; }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            // here nothing is drawn. Only collect Indeterminated Nodes, to draw them later (in WndProc())
            // because drawing Treenodes properly (Text, Icon(s) Focus, Selection...) is very complicated
            if (CheckBoxes && e.Node.StateImageIndex == 1) indeterminateds.Add(e.Node);
            e.DrawDefault = true;
            //e.Node.NodeFont = DrawBoldSelectedNode && e.Node.IsSelected
            //                      ? new Font(Font, FontStyle.Bold)
            //                      : new Font(Font, FontStyle.Regular);

            base.OnDrawNode(e);
        }

        private void InheritCheckstate(TreeNode nd, int state)
        {
            AssignState(nd, state);
            foreach (TreeNode ndChild in nd.Nodes)
            {
                InheritCheckstate(ndChild, state);
            }
        }

        const int WM_Paint = 15;
        const int WM_LBUTTONDBLCLK = 0x0203;

        protected override void WndProc(ref Message m)
        {
            if(CheckBoxes)
            {
                if (m.Msg == WM_LBUTTONDBLCLK)
                {
                    var lp = m.LParam.ToInt32();
                    var ht = HitTest(lp & ushort.MaxValue, lp >> 16);
                    if (ht.Location == TreeViewHitTestLocations.StateImage) return;
                }
                base.WndProc(ref m);
                if (m.Msg == WM_Paint)
                {
                    foreach (TreeNode nd in indeterminateds)
                    {
                        graphics.DrawImage(imgIndeterminate, GetCheckRect(nd).Location);
                        //CheckBoxRenderer.DrawCheckBox(_graphics, GetCheckRect(nd).Location, System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal);
                    }
                    indeterminateds.Clear();
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private Rectangle GetCheckRect(TreeNode nd)
        {
            var pt = nd.Bounds.Location;
            pt.X -= ImageList == null ? 16 : 35;
            return new Rectangle(pt.X, pt.Y, 16, 16);
        }

        #endregion Methods
    }
}