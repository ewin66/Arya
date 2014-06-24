using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework4.State;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using Attribute = Arya.Data.Attribute;
using EntityData = Arya.Data.EntityData;
using EntityInfo = Arya.Data.EntityInfo;
using LinqExtensions = Arya.Framework.Common.Extensions.LinqExtensions;
using Sku = Arya.Data.Sku;
using SkuInfo = Arya.Data.SkuInfo;
using TaxonomyData = Arya.Data.TaxonomyData;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;
using TaxonomyMetaData = Arya.Data.TaxonomyMetaData;
using TaxonomyMetaInfo = Arya.Data.TaxonomyMetaInfo;
using Query = Arya.HelperClasses.Query;
using Task = System.Threading.Tasks.Task;

namespace Arya.UserControls
{
    /// <summary>
    /// The taxonomy tree view.
    /// </summary>
    public partial class TaxonomyTreeView : UserControl
    {
        #region Fields (13)

        private const string DefaultDescription = "Click here to enter a Node Description...";
        private const string DefaultSearchString = "Press ? to start searching...";
        private const string NewNodeName = "New Taxonomy Node";

        private static readonly Regex RxSkuCount = new Regex(" \\((\\d+)\\)(\\r\\n)?$",
                                                             RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly Queue<TaxonomyInfo> _taxonomiesToRecalculate = new Queue<TaxonomyInfo>();
        internal int CustomActionMinLevel;
        internal bool ShowAllMenuOptions = true;
        internal Dictionary<TaxonomyInfo, TreeNode> Taxonomies;

        private Action<List<TaxonomyInfo>> _oldOnTaxonomyOpen;
        private Action<TaxonomyInfo> _oldOnTaxonomySelected;
        private TreeNode _rightClickedNode;
        private TaxonomyInfo _selectedTaxonomy;
        private Thread _updateNodeWorker;
        private int maxDepth = 0;
        #endregion Fields

        #region Constructors (1)

        public TaxonomyTreeView()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            taxonomyTree.ImageList = new ImageList();
            taxonomyTree.ImageList.Images.Add(Resources.IntermediateNode);
            taxonomyTree.ImageList.Images.Add(Resources.LeafNode);
            taxonomyTree.ImageList.Images.Add(Resources.DerivedNode);
            taxonomyTree.ImageList.Images.Add(Resources.PlaceholderNode);
            taxonomyTree.TreeViewNodeSorter = new NodeSorter();

            txtSearchBox.Text = DefaultSearchString;
            lstResults.DataSource = _searchDefaultResults;
        }

        #endregion Constructors

        #region Properties (8)

        internal string CustomActionText
        {
            get { return customActionToolStripMenuItem.Text; }
            set { customActionToolStripMenuItem.Text = value; }
        }

        internal Action<TaxonomyInfo> OnCustomAction { private get; set; }

        internal Action<TaxonomyInfo> OnTaxonomyMove { private get; set; }

        internal Action<TaxonomyInfo> OnCrossListNode { private get; set; }

        internal Action<List<TaxonomyInfo>> OnTaxonomyOpen { get; set; }

        internal Action<TaxonomyInfo> OnTaxonomySelected { get; set; }

        public bool OpenNodeOptionEnabled
        {
            get { return openInSkuViewMenuItem.Enabled; }
            set { openInSkuViewMenuItem.Enabled = value; }
        }

        internal List<TaxonomyInfo> SelectedTaxonomies
        {
            get
            {
                return
                    Taxonomies.Where(tax => taxonomyTree.SelectedNodes.Contains(tax.Value)).Select(tax1 => tax1.Key).
                        ToList();
            }
        }

        internal TaxonomyInfo SelectedTaxonomy
        {
            get { return _selectedTaxonomy; }
            set
            {
                _selectedTaxonomy = value;
                if (value != null)
                    taxonomyTree.SelectedNode = Taxonomies[value];
            }
        }

        public bool EnableCheckBoxes
        {
            get { return taxonomyTree.CheckBoxes; }
            set { taxonomyTree.CheckBoxes = value; }
        }

        public bool ShowEnrichments
        {
            get { return pnlEnrichments.Visible; }
            set
            {
                pnlEnrichments.Visible = value;
                if (!value && AryaTools.Instance.Forms.BrowserForm.Visible)
                    AryaTools.Instance.Forms.BrowserForm.Close();
                Invalidate();
            }
        }

        #endregion Properties

        private readonly Dictionary<TaxonomyInfo, string> _rankedAttributeCount = new Dictionary<TaxonomyInfo, string>();
        private readonly List<string> _searchDefaultResults = new List<string> { "Search results will go here..." };

        //TODO: Are you sure this constant is professional?
        private readonly List<string> _searchNoResults = new List<string>
                                                             {
                                                                 "Damn, No Matches!",
                                                                 "Are you sure you're looking for",
                                                                 " the right thing?"
                                                             };

        private readonly Dictionary<TaxonomyInfo, int> _skuCounts = new Dictionary<TaxonomyInfo, int>();
        private string _searchedString = string.Empty;
        public bool includeChildren;

        internal string GetRankedAttributesCount(TaxonomyInfo taxonomy, bool fetchFromDb)
        {
            if (taxonomy == null)
                return null;

            var rankedCount = string.Empty;
            if (_rankedAttributeCount.ContainsKey(taxonomy) && !fetchFromDb)
                rankedCount = _rankedAttributeCount[taxonomy];
            else if (fetchFromDb)
            {
                var schemaDatas = (from si in taxonomy.SchemaInfos
                                   from sd in si.SchemaDatas
                                   where sd.Active && sd.InSchema
                                   select sd).ToList();
                var navCount = schemaDatas.Count(sd => sd.NavigationOrder > 0);
                var dispCount = schemaDatas.Count(sd => sd.DisplayOrder > 0);

                rankedCount = navCount == 0 && dispCount == 0 ? string.Empty : navCount + " • " + dispCount;

                UpdateRankedAttributesCount(taxonomy, rankedCount);
            }
            return rankedCount;
        }

        internal void UpdateRankedAttributesCount(TaxonomyInfo taxonomy, string rankedCount)
        {
            if (taxonomy == null)
                return;

            if (_rankedAttributeCount.ContainsKey(taxonomy))
                _rankedAttributeCount[taxonomy] = rankedCount;
            else
                _rankedAttributeCount.Add(taxonomy, rankedCount);
        }

        internal void ClearSkuCounts()
        {
            _skuCounts.Clear();
            _rankedAttributeCount.Clear();
        }

        private void UpdateSkuCount(TaxonomyInfo taxonomy, int skuCount)
        {
            if (taxonomy == null)
                return;

            if (_skuCounts.ContainsKey(taxonomy))
                _skuCounts[taxonomy] = skuCount;
            else
                _skuCounts.Add(taxonomy, skuCount);
        }

        private int GetSkuCount(TaxonomyInfo taxonomy, bool fetchFromDb)
        {
            if (taxonomy == null)
                return 0;

            var noOfSkus = -1;
            //if (_skuCounts.ContainsKey(taxonomy))
            if (_skuCounts.ContainsKey(taxonomy) && !fetchFromDb)
                noOfSkus = _skuCounts[taxonomy];
            else if (fetchFromDb)
            {
                if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    var cl = Arya.HelperClasses.Query.FetchCrossListObject(taxonomy);
                    //FetchCrossListObject will take care of Include Children Condition
                    if (cl != null)
                    {
                        noOfSkus =
                            Arya.HelperClasses.Query.GetFilteredSkus(cl.TaxonomyIDFilter, cl.ValueFilters, cl.AttributeTypeFilters,
                                                  cl.MatchAllTerms).Select(s => s.ID).Distinct().Count();
                    }
                }
                else if (taxonomy.SkuInfos != null)
                    noOfSkus =
                        taxonomy.SkuInfos.Where(s => s.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.Product.ToString()).Count(
                            si => si.Active);
                UpdateSkuCount(taxonomy, noOfSkus);
            }
            return noOfSkus;
        }

        internal void ResetSkuCount(TaxonomyInfo taxonomy)
        {
            //Clear Count Cache
            if (taxonomy == null)
            {
                ClearSkuCounts();
                return;
            }

            if (_skuCounts.ContainsKey(taxonomy))
                _skuCounts.Remove(taxonomy);
            if (_rankedAttributeCount.ContainsKey(taxonomy))
                _rankedAttributeCount.Remove(taxonomy);

            AryaTools.Instance.Forms.UpdateNodeCounts(taxonomy);
        }

        private void tmrSearch_Tick(object sender, EventArgs e)
        {
            PerformSearch();

            if (!txtSearchBox.Focused)
                tmrSearch.Stop();
        }

        private void PerformSearch()
        {
            var searchString = txtSearchBox.Text;

            if (_searchedString.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                return;

            if (searchString.Equals(DefaultSearchString, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(searchString))
            {
                lstResults.DataSource = _searchDefaultResults;
                return;
            }

            _searchedString = searchString;

            var results =
                Taxonomies.AsParallel().Where(
                    t =>
                    t.Value.TreeView != null
                    && t.Key.NodeName.IndexOf(searchString, 0, StringComparison.OrdinalIgnoreCase) > -1).OrderBy(
                        t => t.Key.ToString()).ToList();

            if (results.Count == 0)
                lstResults.DataSource = _searchNoResults;
            else
            {
                lstResults.DataSource = results;
                lstResults.DisplayMember = "Key";
                taxonomyTree.DrawBoldSelectedNode = true;
            }
        }

        private void lstResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = lstResults.SelectedItem;
                var selectedTaxonomy =
                    (TaxonomyInfo)selectedItem.GetType().GetProperty("Key").GetValue(selectedItem, null);
                taxonomyTree.DrawBoldSelectedNode = true;
                SelectedTaxonomy = selectedTaxonomy;
            }
            catch (Exception)
            {
            }
        }

        private void taxonomyTree_Enter(object sender, EventArgs e)
        {
            lstResults.Visible = false;
            txtNodeDefinition.Visible = true;
        }

        public void ToogleTaxonomyTree()
        {
            /*
            * -1: Unchecked
            *  0: Checked
            *  1: Indeterminate
            */
            Taxonomies.Values.Where(p => p.GetNodeCount(false) == 0).ForEach(a => a.Checked = !a.Checked);
        }

        public void CheckAllNodes() { taxonomyTree.Nodes.Cast<TreeNode>().ForEach(nd => nd.Checked = true); }

        public void UnCheckAllNodes()
        {
            //better than cascading down the changes at this point.
            Taxonomies.Values.Where(p => p.GetNodeCount(false) == 0).ForEach(a => a.Checked = false);
        }

        public void CheckExclusions(HashSet<Guid> taxonomyExclusions)
        {
            UnCheckAllNodes();
            if (taxonomyTree.CheckBoxes)
            {
                foreach (var taxExclusion in taxonomyExclusions)
                {
                    var exclusion = taxExclusion;
                    var taxonomyInfo = Taxonomies.Keys.SingleOrDefault(p => p.ID == exclusion);

                    if (taxonomyInfo == null)
                        continue;

                    var treeNode = Taxonomies[taxonomyInfo];
                    treeNode.Checked = true;
                }
            }
        }

        public List<Guid> GetAllCheckedTaxonomies()
        {
            var rootNode = taxonomyTree.Nodes[0];
            var nodes = GetExcludedNodes(rootNode).ToList();

            return Taxonomies.Where(p => nodes.Contains(p.Value)).Select(p => p.Key.ID).ToList();
        }

        private IEnumerable<TreeNode> GetExcludedNodes(TreeNode rootNode)
        {
            if (rootNode.StateImageIndex == 0)
                yield return rootNode;
            else
            {
                foreach (var treeNode in rootNode.Nodes.Cast<TreeNode>())
                {
                    foreach (var childNode in GetExcludedNodes(treeNode))
                        yield return childNode;
                }
            }
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            //tmrSearch.Start();
        }

        private void addToWorkflowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Sku> skus;
            if (includeChildren)
            {
                var leafTaxonomies = SelectedTaxonomies.SelectMany(t => t.GetLeafTaxnomies).ToList();
                skus = leafTaxonomies.SelectMany(s => s.SkuInfos.Where(a => a.Active)).Select(sk => sk.Sku).ToList();
            }
            else
                skus = SelectedTaxonomies.SelectMany(s => s.SkuInfos.Where(a => a.Active)).Select(sk => sk.Sku).ToList();

            new FrmAddToWorkflow(skus).ShowDialog();
        }

        private void lstResults_VisibleChanged(object sender, EventArgs e)
        {
            if (!lstResults.Visible)
                taxonomyTree.DrawBoldSelectedNode = false;
        }

        private void txtEnrichmentCopy_MouseClick(object sender, MouseEventArgs e) { txtEnrichmentCopy.ReadOnly = false; }

        private void txtEnrichmentCopy_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (txtEnrichmentCopy.ReadOnly == false)
            {
                txtEnrichmentCopy.Text = SelectedTaxonomy == null
                                         || string.IsNullOrEmpty(SelectedTaxonomy.EnrichmentCopy)
                                             ? string.Empty
                                             : SelectedTaxonomy.EnrichmentCopy;
            }
        }

        private void txtEnrichmentCopy_Leave(object sender, EventArgs e)
        {
            if (SelectedTaxonomy == null)
            {
                txtEnrichmentCopy.ReadOnly = true;
                return;
            }

            var currentEnrichmentCopy = txtEnrichmentCopy.Text;
            var taxonomyCopy = SelectedTaxonomy.EnrichmentCopy;
            if (taxonomyCopy != currentEnrichmentCopy)
            {
                if (!string.IsNullOrWhiteSpace(taxonomyCopy) && string.IsNullOrWhiteSpace(currentEnrichmentCopy))
                {
                    if (MessageBox.Show("Are you sure you want to erase Enrichment Copy", "Alert",
                                        MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        txtEnrichmentCopy.ReadOnly = true;
                        return;
                    }
                }

                // Look for Taxonomy Meta Info records that have a copy attribute
                var tmi =
                    SelectedTaxonomy.TaxonomyMetaInfos.FirstOrDefault(
                        t => t.Attribute.AttributeName == Framework.Properties.Resources.TaxonomyEnrichmentCopyAttributeName);
                if (tmi == null)
                {
                    // if no such record exists, create new records
                    SelectedTaxonomy.TaxonomyMetaInfos.Add(new TaxonomyMetaInfo
                                                               {
                                                                   Attribute =
                                                                       Attribute.GetAttributeFromName(
                                                                           Framework.Properties.Resources.TaxonomyEnrichmentCopyAttributeName, true,
                                                                           AttributeTypeEnum.TaxonomyMeta),
                                                                   TaxonomyMetaDatas =
                                                                       {
                                                                           new TaxonomyMetaData
                                                                               {Value = currentEnrichmentCopy}
                                                                       }
                                                               });
                }
                else
                {
                    // if one exists, deactivate all associated copy records and add a new copy record
                    foreach (var t in tmi.TaxonomyMetaDatas.Where(t => t.Active))
                        t.Active = false;
                    tmi.TaxonomyMetaDatas.Add(new TaxonomyMetaData { Value = currentEnrichmentCopy });
                }

                AryaTools.Instance.SaveChangesIfNecessary(false, false);
            }

            txtEnrichmentCopy.ReadOnly = true;
        }

        private void txtEnrichmentImage_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedTaxonomy == null)
                return;

            using (
                var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                    AryaTools.Instance.InstanceData.CurrentUser.ID))
            {
                var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID);
                if (!imageMgr.UploadImage())
                    return;

                // Look for Taxonomy Meta Info records that have an image attribute
                var tmi =
                    SelectedTaxonomy.TaxonomyMetaInfos.FirstOrDefault(
                        t => t.Attribute.AttributeName == Framework.Properties.Resources.TaxonomyEnrichmentImageAttributeName);
                if (tmi == null)
                {
                    // if no such record exists, create new records
                    SelectedTaxonomy.TaxonomyMetaInfos.Add(new TaxonomyMetaInfo
                                                           {
                                                               Attribute =
                                                                   Attribute
                                                                   .GetAttributeFromName(
                                                                       Framework.Properties
                                                                   .Resources
                                                                   .TaxonomyEnrichmentImageAttributeName,
                                                                       true,
                                                                       AttributeTypeEnum
                                                                   .TaxonomyMeta),
                                                               TaxonomyMetaDatas =
                                                               {
                                                                   new TaxonomyMetaData
                                                                   {
                                                                       Value
                                                                           =
                                                                           imageMgr
                                                                           .RemoteImageGuid
                                                                   }
                                                               }
                                                           });
                }
                else
                {
                    // if one exists, deactivate all associated image records and add a new image record
                    foreach (var t in tmi.TaxonomyMetaDatas.Where(t => t.Active))
                        t.Active = false;
                    tmi.TaxonomyMetaDatas.Add(new TaxonomyMetaData { Value = imageMgr.RemoteImageGuid });
                }

                imageMgr.AddAttributeValue("TaxonomyId", SelectedTaxonomy.ID.ToString());

                SelectedTaxonomy.SkuInfos.Add(new SkuInfo { SkuID = imageMgr.ImageSku.ID });

                AryaTools.Instance.SaveChangesIfNecessary(false, false);

                txtEnrichmentImage.Text = Path.GetFileName(imageMgr.OriginalFileUri);
                AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
            }
        }

        private void txtEnrichmentImage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete || String.IsNullOrEmpty(txtEnrichmentImage.Text))
                return;

            DialogResult results = MessageBox.Show("Are you sure that you want to delete this image?", "Warning" ,MessageBoxButtons.YesNo);
            if (results != DialogResult.Yes)
                return;
            
            var tmi =
                SelectedTaxonomy.TaxonomyMetaInfos.FirstOrDefault(
                    t => t.Attribute.AttributeName == Framework.Properties.Resources.TaxonomyEnrichmentImageAttributeName);
            if (tmi == null)
                return;

            foreach (var t in tmi.TaxonomyMetaDatas.Where(t => t.Active))
                t.Active = false;
            txtEnrichmentImage.Text = String.Empty;
        }

        #region Methods (57)

        private Dictionary<Guid, TaxonomyData> allNodesDictonary;
        private ILookup<Guid?, Guid> allNodesLookUp;
        private int totalNodeCount;

        /// <summary>
        /// The add new child node.
        /// </summary>
        /// <param name="nodeType">
        /// The node type.
        /// </param>
        public void AddNewChildNode(string nodeType)
        {
            var newTaxonomyInfo = new TaxonomyInfo
                                      {
                                          Project = AryaTools.Instance.InstanceData.CurrentProject,
                                          ShowInTree = true,
                                          NodeType = nodeType
                                      };
            var newTaxonomyData = new TaxonomyData { NodeName = NewNodeName, TaxonomyInfo = newTaxonomyInfo };

            var rightClickedTaxonomy = GetTaxonomyFromTreeNode(_rightClickedNode);
            if (rightClickedTaxonomy != null)
                newTaxonomyData.ParentTaxonomyInfo = rightClickedTaxonomy;

            AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(newTaxonomyInfo);

            var newTreeNode = new TreeNode(NewNodeName);
            _rightClickedNode.Nodes.Add(newTreeNode);
            Taxonomies.Add(newTaxonomyInfo, newTreeNode);

            SetNodeImageIndex(newTreeNode, newTaxonomyInfo, 0);
            SetNodeImageIndex(_rightClickedNode, rightClickedTaxonomy, _rightClickedNode.Nodes.Count);

            taxonomyTree.SelectedNode = newTreeNode;

            //Show rename box for the new node
            _rightClickedNode = newTreeNode;
            ShowRenameTextBox();
        }

        public void LoadProject()
        {
            var sw = new Stopwatch();
            sw.Start();
            LoadTaxonomies();
            sw.Stop();
            Diagnostics.WriteMessage("Loading Tree", "TaxonomyTreeView - LoadTaxonomies()", sw.Elapsed);

            sw.Reset();
            sw.Start();
            PopulateTaxonomyTree();
            sw.Stop();
            Diagnostics.WriteMessage("Loading Tree", "TaxonomyTreeView - PopulateTaxonomyTree()", sw.Elapsed);
        }

        public void SortNodes()
        {
            SuspendLayout();
            //if (taxonomyTree.Nodes.Count > 1)
            taxonomyTree.Sort();
            ResumeLayout();
        }

        public void UpdateNode(TaxonomyInfo taxonomyInfo)
        {
            var treeNode = Taxonomies[taxonomyInfo];
            treeNode.Remove();
            if (taxonomyInfo.TaxonomyData.ParentTaxonomyInfo != null)
                Taxonomies[taxonomyInfo.TaxonomyData.ParentTaxonomyInfo].Nodes.Add(treeNode);
            else
                taxonomyTree.Nodes[0].Nodes.Add(treeNode);
        }

        public void AddCrossListNode(TaxonomyInfo parentTaxonomyInfo, TaxonomyInfo crossListTaxonomyInfo)
        {
            var crossListNode = new TreeNode(crossListTaxonomyInfo.ToString(), 2, 2);
            Taxonomies.Add(crossListTaxonomyInfo, crossListNode);
            Taxonomies[parentTaxonomyInfo].Nodes.Add(crossListNode);
        }

        public int UpdateNodeCounts(TaxonomyInfo taxonomy, bool fetchFromDb)
        {
            if (taxonomy == null || taxonomy.TaxonomyData == null || !Taxonomies.ContainsKey(taxonomy))
                return -1;

            var treeNode = Taxonomies[taxonomy];

            var skuCount = GetSkuCount(taxonomy, false);
            var rankedCount = GetRankedAttributesCount(taxonomy, false);
            if ((string.IsNullOrEmpty(rankedCount) || skuCount == -1) && fetchFromDb)
            {
                lock (_taxonomiesToRecalculate)
                {
                    _taxonomiesToRecalculate.Enqueue(taxonomy);
                }
            }

            var currentNodeText = treeNode.Text;

            var newNodeText = GetNodeName(taxonomy.TaxonomyData.NodeName)
                              + (skuCount > 0 ? " (" + skuCount + ")" : string.Empty);

            if (!string.Equals(currentNodeText, newNodeText, StringComparison.OrdinalIgnoreCase))
                treeNode.Text = newNodeText;

            return skuCount;
        }

        //public async Task<int> UpdateNodeCounts(TaxonomyInfo taxonomy, bool fetchFromDb)
        //{
        //    if (taxonomy == null || taxonomy.TaxonomyData == null || !Taxonomies.ContainsKey(taxonomy))
        //        return -1;

        //    TreeNode treeNode = Taxonomies[taxonomy];

        //    int skuCount = AryaTools.Instance.GetSkuCount(taxonomy, false);
        //    string rankedCount = AryaTools.Instance.GetRankedAttributesCount(taxonomy, false);
        //    if ((string.IsNullOrEmpty(rankedCount) || skuCount == -1) && fetchFromDb)
        //    {
        //        lock (_taxonomiesToRecalculate)
        //        {
        //            _taxonomiesToRecalculate.Enqueue(taxonomy);
        //        }
        //    }

        //    treeNode.Text = GetNodeName(taxonomy.TaxonomyData.NodeName) +
        //                    (skuCount > 0 ? String.Format(" ({0})", skuCount) : string.Empty);

        //    return skuCount;
        //}

        // Private Methods (49) 

        private void addCrossReferenceRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rightClickedTaxonomy = GetTaxonomyFromTreeNode(_rightClickedNode);
            AryaTools.Instance.Forms.QueryForm.SetFormType(FrmQueryView.QueryFormType.CrossListDefinition,
                                                              rightClickedTaxonomy);
            AryaTools.Instance.Forms.QueryForm.Show();
        }

        private void collapseAllChildrenToolStripMenuItem_Click(object sender, EventArgs e) { _rightClickedNode.Collapse(false); }

        private void crosslistToolStripMenuItem_Click(object sender, EventArgs e) { AddNewChildNode(TaxonomyInfo.NodeTypeDerived); }

        private void customActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OnCustomAction == null)
                return;

            OnCustomAction(SelectedTaxonomy);
        }

        private void DeleteTaxonomyNode(TaxonomyInfo taxonomyToDelete, TaxonomyInfo refugeTaxonomy,
                                        bool deleteChildNodes)
        {
            if (taxonomyToDelete == null)
                return;

            var children = taxonomyToDelete.ChildTaxonomyDatas.Where(child => child.Active);

            if (deleteChildNodes)
                children.ForEach(child => DeleteTaxonomyNode(child.TaxonomyInfo, refugeTaxonomy, true));
            else
            {
                children.ForEach(child =>
                                     {
                                         child.Active = false;
                                         child.TaxonomyInfo.TaxonomyDatas.Add(new TaxonomyData
                                                                                  {
                                                                                      NodeName = child.NodeName,
                                                                                      ParentTaxonomyInfo = refugeTaxonomy
                                                                                  });
                                     });
            }

            var skuInfos = taxonomyToDelete.SkuInfos.Where(skuInfo => skuInfo.Active);
            skuInfos.ForEach(skuInfo =>
                                 {
                                     AryaTools.Instance.InstanceData.Dc.SkuInfos.InsertOnSubmit(new SkuInfo
                                                                                                       {
                                                                                                           Sku =
                                                                                                               skuInfo.
                                                                                                               Sku,
                                                                                                           TaxonomyInfo
                                                                                                               =
                                                                                                               refugeTaxonomy
                                                                                                       });
                                     skuInfo.Active = false;
                                 });

            taxonomyToDelete.TaxonomyData.Active = false;

            var nodeToDelete = Taxonomies[taxonomyToDelete];
            var nodeRefuge = Taxonomies[refugeTaxonomy];
            var childNodes = nodeToDelete.Nodes.Cast<TreeNode>().ToList();
            foreach (var childNode in childNodes)
            {
                childNode.Remove();
                nodeRefuge.Nodes.Add(childNode);
            }

            nodeToDelete.Remove();
            ResetSkuCount(taxonomyToDelete);
        }

        private void deleteThisNodeAndAllChildNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parentNode = SelectedTaxonomy.TaxonomyData.ParentTaxonomyInfo;
            if (
                MessageBox.Show(
                    String.Format(
                        "Are you sure you want to delete [{0}] and all its child nodes?\nSKUs (if any) will roll up to [{1}]",
                        SelectedTaxonomy, parentNode), "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            var waitKey =
                FrmWaitScreen.ShowMessage(String.Format("Deleting [{0}] and all child nodes", SelectedTaxonomy));

            DeleteTaxonomyNode(SelectedTaxonomy, parentNode, true);
            SetNodeImageIndex(Taxonomies[parentNode], parentNode, Taxonomies[parentNode].Nodes.Count);

            FrmWaitScreen.HideMessage(waitKey);
            ResetSkuCount(parentNode);

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void deleteThisNodeOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parentNode = SelectedTaxonomy.TaxonomyData.ParentTaxonomyInfo;

            if (
                MessageBox.Show(
                    String.Format(
                        "Are you sure you want to delete [{0}]?\nChild nodes and SKUs (if any) will roll up to [{1}]",
                        SelectedTaxonomy, parentNode), "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            var waitKey = FrmWaitScreen.ShowMessage(String.Format("Deleting [{0}]", SelectedTaxonomy));

            if (SelectedTaxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
            {
                var derivedTaxonomy =
                    AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.SingleOrDefault(
                        t => t.TaxonomyID == SelectedTaxonomy.ID);
                if (derivedTaxonomy != null)
                    AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.DeleteOnSubmit(derivedTaxonomy);
            }

            DeleteTaxonomyNode(SelectedTaxonomy, parentNode, false);

            FrmWaitScreen.HideMessage(waitKey);
            ResetSkuCount(parentNode);

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void DoRename(object sender, EventArgs e)
        {
            OnTaxonomySelected = _oldOnTaxonomySelected;
            OnTaxonomyOpen = _oldOnTaxonomyOpen;

            var textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
            {
                textBox.Focus();
                return;
            }

            if (!textBox.Text.Equals(_rightClickedNode.Text))
            {
                var taxonomyInfo = GetTaxonomyFromTreeNode(_rightClickedNode);
                var oldData = taxonomyInfo.TaxonomyData;
                if (oldData.NodeName.Equals(NewNodeName))
                    oldData.NodeName = textBox.Text;
                else
                {
                    oldData.Active = false;
                    taxonomyInfo.TaxonomyDatas.Add(new TaxonomyData
                                                       {
                                                           NodeName = textBox.Text,
                                                           ParentTaxonomyInfo = oldData.ParentTaxonomyInfo,
                                                           NodeDescription = oldData.NodeDescription
                                                       });
                }
                _rightClickedNode.Text = textBox.Text;
                SetNodeImageIndex(_rightClickedNode, taxonomyInfo, _rightClickedNode.Nodes.Count);
            }

            textBox.Dispose();

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void expandAllChildrenToolStripMenuItem_Click(object sender, EventArgs e) { _rightClickedNode.ExpandAll(); }

        private void exportTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fullTaxonomyHtml = Path.GetTempFileName();
            TextWriter sw = new StreamWriter(fullTaxonomyHtml, false);
            sw.WriteLine(NodeToString(_rightClickedNode, string.Empty));

            sw.Close();

            AryaTools.Instance.Forms.BrowserForm.GotoUrl(new FileInfo(fullTaxonomyHtml).FullName, "Full Taxonomy");
        }

        private ContextMenuStrip GetMenuStrip(int nodeLevel)
        {
            var openVisible = nodeLevel > 0 && OnTaxonomyOpen != null && ShowAllMenuOptions;
            var customActionVisible = OnCustomAction != null && nodeLevel >= CustomActionMinLevel;

            openInSkuViewMenuItem.Visible = openVisible;
            customActionToolStripMenuItem.Visible = customActionVisible;

            var oneNodeSelected = taxonomyTree.SelectedNodes.Count == 1;
            var crossListVisible = nodeLevel > 0 && OnCrossListNode != null && ShowAllMenuOptions && oneNodeSelected;
            var moveVisible = nodeLevel > 0 && OnTaxonomyMove != null && ShowAllMenuOptions && oneNodeSelected;
            var renameVisible = nodeLevel > 0 && ShowAllMenuOptions && oneNodeSelected;
            var deleteVisible = nodeLevel > 1 && ShowAllMenuOptions && oneNodeSelected;
            //bool addVisible = nodeLevel > 0 && ShowAllMenuOptions && oneNodeSelected;  <-0-> 12/5/2012 RAS
            var addVisible = ShowAllMenuOptions && oneNodeSelected;
            var taxonomy = GetTaxonomyFromTreeNode(taxonomyTree.SelectedNode);
            var editCrossListVisible = nodeLevel > 0 && ShowAllMenuOptions && oneNodeSelected
                                       && taxonomy.NodeType != null
                                       && taxonomy.NodeType.Equals(TaxonomyInfo.NodeTypeDerived);

            moveNodeToolStripMenuItem.Visible = moveVisible;
            renameNodeToolStripMenuItem.Visible = renameVisible;
            deleteNodeToolStripMenuItem.Visible = deleteVisible;
            addChildNodeToolStripMenuItem.Visible = addVisible;
            editCrossListRuleToolStripMenuItem.Visible = editCrossListVisible;
            refreshSkuCountToolStripMenuItem.Visible = nodeLevel > 0;

            toolStripSeparator1.Visible = (moveVisible || renameVisible || deleteVisible || addVisible)
                                          && (openVisible || customActionVisible);

            var expandCollapseVisible = oneNodeSelected;
            var exportVisible = ShowAllMenuOptions && oneNodeSelected;

            crosslistToolStripMenuItem.Visible = nodeLevel > 0 && addVisible; //  <-0-> 12/5/2012 RAS
            crossListNodeToolStripMenuItem.Visible = crossListVisible;
            expandAllChildrenToolStripMenuItem.Visible = expandCollapseVisible;
            collapseAllChildrenToolStripMenuItem.Visible = expandCollapseVisible;
            exportTreeToolStripMenuItem.Visible = exportVisible;

            toolStripSeparator2.Visible = (exportVisible || expandCollapseVisible)
                                          && (moveVisible || renameVisible || deleteVisible || addVisible);

            return treeNodeContextMenuStrip;
        }

        private static string GetNodeName(string nodeName) { return String.IsNullOrEmpty(nodeName) ? "<blank>" : nodeName; }

        private TaxonomyInfo GetTaxonomyFromTreeNode(TreeNode treeNode) { return Taxonomies.Where(tax => tax.Value.Equals(treeNode)).Select(tax => tax.Key).FirstOrDefault(); }

        //private Thread _definitionSkuCountWorker;

        private void UpdateDefinitionAndSkuCount(TaxonomyInfo tax)
        {
            if (tax == null)
                return;

            // update definition
            string message;
            try
            {
                var selectedNode = Taxonomies[tax];

                string nodeDescription;
                if (tax.TaxonomyData != null && !string.IsNullOrEmpty(tax.TaxonomyData.NodeDescription))
                    nodeDescription = tax.TaxonomyData.NodeDescription;
                else
                    nodeDescription = DefaultDescription;

                var runningTotal = 0;
                var derivedNodesTotal = 0;
                bool missingValuesForRegularNodes = false, missingValuesForDerivedNodes = false;

                var nodes = new Queue<TreeNode>();
                nodes.Enqueue(selectedNode);
                var leafNodeCount = 0;
                while (nodes.Count > 0)
                {
                    var currentNode = nodes.Dequeue();
                    if (currentNode == null)
                        continue;

                    var taxonomy = GetTaxonomyFromTreeNode(currentNode);
                    if (taxonomy != null)
                    {
                        var skuCount = GetSkuCount(taxonomy, false);
                        if (skuCount == -1)
                            missingValuesForRegularNodes = true;
                        else
                        {
                            switch (taxonomy.NodeType)
                            {
                                case TaxonomyInfo.NodeTypeRegular:
                                    runningTotal += skuCount;
                                    break;
                                case TaxonomyInfo.NodeTypeDerived:
                                    derivedNodesTotal += skuCount;
                                    break;
                            }
                        }
                    }

                    currentNode.Nodes.Cast<TreeNode>().ForEach(nodes.Enqueue);
                    if (currentNode.Nodes.Count == 0)
                        leafNodeCount++;
                }

                message = string.Empty;

                if (runningTotal > 0 || missingValuesForRegularNodes)
                {
                    message += string.Format("L{2}: {0}{1} SKUs. ", runningTotal,
                                             missingValuesForRegularNodes ? "+" : string.Empty, selectedNode.Level);
                }

                if (derivedNodesTotal > 0 || missingValuesForDerivedNodes)
                {
                    message += string.Format("{0}{1} cross listed SKUs. ", derivedNodesTotal,
                                             missingValuesForDerivedNodes ? "+" : string.Empty);
                }

                var attributeCounts = GetRankedAttributesCount(tax, false);
                if (!string.IsNullOrEmpty(attributeCounts))
                    message += string.Format("{0}{1} Ranked In Schema Attributes.", Environment.NewLine, attributeCounts);

                var noOfChildren = tax.ChildTaxonomyDatas.Count(td => td.Active);

                if (noOfChildren > 0 && noOfChildren != leafNodeCount)
                    message += string.Format("{0}{1} Immediate Child Nodes", Environment.NewLine, noOfChildren);
                if (selectedNode.Nodes.Count > 0 && leafNodeCount > 0)
                    message += string.Format("{0}{1} Descendant Leaf Nodes", Environment.NewLine, leafNodeCount);

                message = string.Format("{0}{1}{2}", message, Environment.NewLine, nodeDescription);
            }
            catch (Exception)
            {
                message = string.Empty;
            }

            Task.Run(() => LinqExtensions.InvokeEx(txtNodeDefinition, t => t.Text = message));

            // display Enrichment Image
            if (!ShowEnrichments)
                return;

            // update enrichment copy
            Task.Run(
                () =>
                    LinqExtensions.InvokeEx(txtEnrichmentCopy,
                        t => t.Text = (SelectedTaxonomy ?? new TaxonomyInfo()).EnrichmentCopy));

            // update enrichment image
            Task.Run(
                () =>
                    LinqExtensions.InvokeEx(txtEnrichmentImage,
                        t => t.Text = (SelectedTaxonomy ?? new TaxonomyInfo()).EnrichmentImage));
            
            try
            {
                var imageSku = SelectedTaxonomy.EnrichmentImageSku;
                if (imageSku == null)
                    using (
                        var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                            AryaTools.Instance.InstanceData.CurrentUser.ID))
                    {
                        var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID);
                        AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                    }
                else
                {
                    using (
                        var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                            AryaTools.Instance.InstanceData.CurrentUser.ID))
                    {
                        var iSku = dc.Skus.First(sku => sku.ID == imageSku.ID);
                        var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                                       {
                                           ImageSku
                                               =
                                               iSku
                                       };

                        AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                    }
                }
            }
            catch
            {
                AryaTools.Instance.Forms.BrowserForm.GotoUrl("about:blank");
            }
        }

        private void HideBlankChildren(TreeNode parent)
        {
            var waitKey = FrmWaitScreen.ShowMessage("Hiding empty nodes");

            var nodesToRemove = new List<TreeNode>();
            HideBlankChildren(parent, nodesToRemove);
            nodesToRemove.ForEach(node => node.Remove());

            if (nodesToRemove.Count > 0)
                showAllNodesToolStripMenuItem.Visible = true;

            FrmWaitScreen.HideMessage(waitKey);
        }

        private bool HideBlankChildren(TreeNode node, ICollection<TreeNode> remove)
        {
            var hideThisNode = true;
            foreach (TreeNode child in node.Nodes)
            {
                if (!HideBlankChildren(child, remove))
                    hideThisNode = false;
            }

            var thisTaxonomy = GetTaxonomyFromTreeNode(node);
            var skuCount = UpdateNodeCounts(thisTaxonomy, true);
            if (hideThisNode && thisTaxonomy != null && skuCount == 0)
            {
                remove.Add(node);
                return true;
            }

            return false;
        }

        private void hideEmptyChildrenToolStripMenuItem_Click(object sender, EventArgs e) { HideBlankChildren(_rightClickedNode); }

        //buffers for TreeLoading process

        private void LoadTaxonomies()
        {
            var waitKey = FrmWaitScreen.ShowMessage("Loading Taxonomy");

            var taxs = (from td in AryaTools.Instance.InstanceData.Dc.TaxonomyDatas
                        let ti = td.TaxonomyInfo
                        let skuCount =
                            ti.SkuInfos.Where(si => si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.Product.ToString()).Count(
                                si => si.Active)
                        let schemaDatas = from si in ti.SchemaInfos
                                          from sd in si.SchemaDatas
                                          where sd.Active && sd.InSchema
                                          select sd
                        let navCount = schemaDatas.Count(sd => sd.NavigationOrder > 0)
                        let dispCount = schemaDatas.Count(sd => sd.DisplayOrder > 0)
                        where
                            td.Active
                            && td.TaxonomyInfo.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject)
                        orderby td.CreatedOn descending
                        select
                            new
                                {
                                    TaxonomyData = td,
                                    TaxonomyInfo = ti,
                                    td.NodeName,
                                    SkuCount = skuCount,
                                    NavCount = navCount,
                                    DispCount = dispCount
                                }).ToList();

            Taxonomies = new Dictionary<TaxonomyInfo, TreeNode>(taxs.Count);
            taxs.ForEach(tax =>
                             {
                                 if (Taxonomies.ContainsKey(tax.TaxonomyInfo))
                                     tax.TaxonomyData.Active = false;
                                 else
                                 {
                                     Taxonomies.Add(tax.TaxonomyInfo, new TreeNode(GetNodeName(tax.NodeName)));
                                     if (tax.TaxonomyInfo.NodeType != TaxonomyInfo.NodeTypeDerived)
                                     {
                                         var rankedCount = tax.NavCount == 0 && tax.DispCount == 0
                                                               ? string.Empty
                                                               : tax.NavCount + " • " + tax.DispCount;
                                         UpdateSkuCount(tax.TaxonomyInfo, tax.SkuCount);
                                         UpdateRankedAttributesCount(tax.TaxonomyInfo, rankedCount);
                                     }
                                 }
                             });

            allNodesLookUp = taxs.Select(p => p.TaxonomyData).ToLookup(p => p.ParentTaxonomyID, p => p.TaxonomyID);
            allNodesDictonary = taxs.Select(p => p.TaxonomyData).ToDictionary(key => key.TaxonomyID, value => value);

            FrmWaitScreen.HideMessage(waitKey);
        }

        private void moveNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OnTaxonomyMove == null)
                return;
            OnTaxonomyMove(GetTaxonomyFromTreeNode(_rightClickedNode));

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private string NodeToString(TreeNode node, string indent)
        {
            var skuCount = string.Empty;
            if (RxSkuCount.IsMatch(node.Text))
                skuCount = RxSkuCount.Match(node.Text).Groups[1].Value;

            var tax = GetTaxonomyFromTreeNode(node);

            //var allLeafChildren = tax.SelectMany(p => p.AllLeafChildren).Distinct().ToList();
            if (tax.AllLeafChildren.Count() != 0)
            {
                var tempMaxDepth = tax.AllLeafChildren.Distinct().ToList().Select(child => child.ToString().Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None).Length).Max();
                if (maxDepth < tempMaxDepth)
                    maxDepth = tempMaxDepth;
            }

            var nodeName = node.Text;

            if (tax != null && tax.NodeType == TaxonomyInfo.NodeTypeDerived)
            {
                var cl = Arya.HelperClasses.Query.GetCrossListObject(tax);
                var tabCount = (maxDepth - tax.CurrentNodeLevel) + 1;
                string tabs = new String('\t', tabCount);

                if (cl != null)
                {
                    if (cl.TaxonomyIDFilter.Count == 1)
                    {
                        var taxID = cl.TaxonomyIDFilter.Single();
                        var homeTaxonomy = Taxonomies.Keys.SingleOrDefault(t => t.ID == taxID);
                        if (homeTaxonomy != null)
                        {
                            if (cl.IncludeChildren == false)
                                nodeName = node.Text + tabs + TaxonomyInfo.CrossPrefix + homeTaxonomy;
                            else
                                nodeName = node.Text + tabs + TaxonomyInfo.CrossPrefix + homeTaxonomy + " and child nodes";
                        }

                        else
                            nodeName = node.Text + tabs + " -> Empty CL Node";
                    }
                    else if (cl.TaxonomyIDFilter.Count != 0 || cl.IncludeChildren)
                        nodeName = node.Text + tabs + TaxonomyInfo.CrossPrefix + " More than one node";
                }
                else
                    nodeName = node.Text + tabs + " -> Empty CL Node";
            }

            var text = string.Format("{0}{1}{2}{3}", skuCount, string.IsNullOrEmpty(indent) ? string.Empty : indent,
                                     RxSkuCount.Replace(nodeName, string.Empty), Environment.NewLine);

            return node.Nodes.Cast<TreeNode>().Aggregate(text,
                                                         (current, child) =>
                                                         current
                                                         +
                                                         NodeToString(child,
                                                                      string.Format("{0}\t",
                                                                                    string.IsNullOrEmpty(indent)
                                                                                        ? string.Empty
                                                                                        : indent)));
        }

        private void openInSkuViewMenuItem_Click(object sender, EventArgs e)
        {
            if (OnTaxonomyOpen != null)
                OnTaxonomyOpen(SelectedTaxonomies);
        }

        public void LoadTree(Guid taxonomyID, TreeNode parentNode)
        {
            //Taxonomy Exclusions
            if (!taxonomyTree.CheckBoxes
                && AryaTools.Instance.InstanceData.CurrentUser.TaxonomyExclusions.Contains(taxonomyID))
                return;

            var taxonomyData = allNodesDictonary[taxonomyID];

            var childNode = Taxonomies[taxonomyData.TaxonomyInfo];
            parentNode.Nodes.Add(childNode);
            //Interlocked.Increment(ref totalNodeCount);
            totalNodeCount++;

            var childTaxonomies = allNodesLookUp[taxonomyID];
            var childCount = 0;

            foreach (var childTaxonomy in childTaxonomies)
            {
                LoadTree(childTaxonomy, childNode);
                childCount++;
            }

            SetNodeImageIndex(childNode, taxonomyData.TaxonomyInfo, childCount);
            UpdateNodeCounts(taxonomyData.TaxonomyInfo, false);
        }

        private void PopulateTaxonomyTree()
        {
            var waitKey = FrmWaitScreen.ShowMessage("Building Tree Structure");

            var strTop = AryaTools.Instance.InstanceData.CurrentProject == null
                             ? "Taxonomy Tree"
                             : AryaTools.Instance.InstanceData.CurrentProject.ProjectDescription;

            var topNode = new TreeNode(strTop);

            //prefetch only - because it is being called in parallel later
            var taxExclusions = AryaTools.Instance.InstanceData.CurrentUser.TaxonomyExclusions;

            totalNodeCount = 0;

            //Lightning fast- takes 0.50 seconds to build and load the Tree :)
            allNodesLookUp[null].ForEach(p => LoadTree(p, topNode));
            //Parallel.ForEach(allNodesLookUp[null], new ParallelOptions { MaxDegreeOfParallelism = 4 },node => LoadTree(node, topNode));

            taxonomyTree.Sorted = false;
            try
            {
                taxonomyTree.BeginUpdate();
                taxonomyTree.Nodes.Clear();
                taxonomyTree.Nodes.Add(topNode);
                taxonomyTree.Sort();
            }
            catch (NullReferenceException)
            {
            }
            finally
            {
                taxonomyTree.EndUpdate();
                taxonomyTree.Refresh();
            }

            if (taxonomyTree.SelectedNode == null)
                topNode.Expand();

            FrmWaitScreen.UpdateMessage(waitKey, "Calculating Node Counts");

            //taxonomyTree.SelectedNode = topNode;

            FrmWaitScreen.HideMessage(waitKey);

            allNodesLookUp = null;
            allNodesDictonary.Clear();
            allNodesDictonary = null;

            if (taxonomyTree.GetNodeCount(true) != totalNodeCount + 1)
            {
                var result = MessageBox.Show(@"Tree may not have loaded correctly, Restart Arya?", @"TreeView Alert",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    Application.Restart();
            }
        }

        private void regularToolStripMenuItem_Click(object sender, EventArgs e) { AddNewChildNode(TaxonomyInfo.NodeTypeRegular); }

        private void renameNodeToolStripMenuItem_Click(object sender, EventArgs e) { ShowRenameTextBox(); }

        private void RenameTextboxHandleKeyStrokes(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    OnTaxonomySelected = _oldOnTaxonomySelected;
                    OnTaxonomyOpen = _oldOnTaxonomyOpen;

                    textBox.LostFocus -= DoRename;
                    textBox.Dispose();
                    break;

                case Keys.Enter:
                    e.Handled = true;
                    DoRename(sender, e);
                    break;
            }
        }

        private static void SetNodeImageIndex(TreeNode node, TaxonomyInfo taxonomy, int childCount)
        {
            if (taxonomy == null)
                return;

            if (node.Text.StartsWith("zz", StringComparison.OrdinalIgnoreCase))
                node.ImageIndex = 3;
            else if (childCount > 0)
                node.ImageIndex = 0;
            else
            {
                var nodeType = taxonomy.NodeType;

                if (nodeType == TaxonomyInfo.NodeTypeRegular || !nodeType.Equals(TaxonomyInfo.NodeTypeDerived, StringComparison.Ordinal))
                    node.ImageIndex = 1;
                else
                    node.ImageIndex = 2;
            }

            node.SelectedImageIndex = node.ImageIndex;
        }

        private void showAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PopulateTaxonomyTree();
            ResetSkuCount(null);
            LoadProject();
            showAllNodesToolStripMenuItem.Visible = false;
        }

        private void ShowRenameTextBox()
        {
            _oldOnTaxonomySelected = OnTaxonomySelected;
            _oldOnTaxonomyOpen = OnTaxonomyOpen;
            OnTaxonomySelected = null;
            OnTaxonomyOpen = null;

            var renameTextBox = new TextBox();
            Controls.Add(renameTextBox);

            renameTextBox.Left = taxonomyTree.Left + _rightClickedNode.Bounds.Left;
            renameTextBox.Width = taxonomyTree.Right - renameTextBox.Left - 2;
            renameTextBox.Top = taxonomyTree.Top + _rightClickedNode.Bounds.Top;
            renameTextBox.Height = _rightClickedNode.Bounds.Height;

            renameTextBox.KeyDown += RenameTextboxHandleKeyStrokes;
            renameTextBox.LostFocus += DoRename;

            renameTextBox.Text = GetTaxonomyFromTreeNode(_rightClickedNode).TaxonomyData.NodeName;
            renameTextBox.SelectAll();
            renameTextBox.BringToFront();
            renameTextBox.Focus();
        }

        public void TryStartWorkers()
        {
            if (!(_updateNodeWorker != null && _updateNodeWorker.IsAlive))
            {
                _updateNodeWorker = new Thread(UpdateNodeWorker) { IsBackground = true };
                _updateNodeWorker.Start();
            }

            //if (!(_definitionSkuCountWorker != null && _definitionSkuCountWorker.IsAlive))
            //{
            //    _definitionSkuCountWorker = new Thread(UpdateDefinitionSkuCountWorker) { IsBackground = true };
            //    _definitionSkuCountWorker.Start();
            //}

            updateNodeTimer.Start();
        }

        public void AbortWorkers()
        {
            updateNodeTimer.Stop();

            if (_updateNodeWorker != null && _updateNodeWorker.IsAlive)
                _updateNodeWorker.Abort();

            //if (_definitionSkuCountWorker != null && _definitionSkuCountWorker.IsAlive)
            //{
            //    _definitionSkuCountWorker.Abort();
            //}
        }

        private void taxonomyTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            var nodeCount = e.Node.Nodes.Count;
            if (nodeCount > 0 && nodeCount < 3)
                e.Node.Nodes.Cast<TreeNode>().ForEach(node => node.Expand());
        }

        private void taxonomyTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if ((taxonomyTree.SelectedNode != null && !taxonomyTree.SelectedNode.FullPath.StartsWith(e.Node.FullPath))
                && !e.Node.IsExpanded)
                e.Node.Expand();
        }

        private void taxonomyTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemQuestion)
            {
                if (taxonomyTree.SelectedNode != null)
                    txtSearchBox.Text = RxSkuCount.Replace(taxonomyTree.SelectedNode.Text, mt => string.Empty);
                txtSearchBox.Focus();
            }
        }

        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                lstResults.Focus();
        }

        private void taxonomyTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectedTaxonomy = GetTaxonomyFromTreeNode(e.Node);

            UpdateNodeCounts(SelectedTaxonomy, true);

            if (SelectedTaxonomy != null)
                WindowsRegistry.SaveToRegistry(WindowsRegistry.RegistryKeyCurrentTaxonomy,
                                               SelectedTaxonomy.ID.ToString());

            if (OnTaxonomySelected != null)
                OnTaxonomySelected(SelectedTaxonomy);
        }

        private void taxonomyTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            _rightClickedNode = e.Node;
            SelectedTaxonomy = GetTaxonomyFromTreeNode(_rightClickedNode);
            if (!taxonomyTree.CheckBoxes)
                GetMenuStrip(e.Node.Level).Show(taxonomyTree.PointToScreen(e.Location));
        }

        private void taxonomyTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Bad double click
            if (SystemInformation.DoubleClickTime > 800)
                return;

            if (OnTaxonomyOpen != null)
                OnTaxonomyOpen(SelectedTaxonomies);
        }

        private void textBoxNodeDefinition_Leave(object sender, EventArgs e)
        {
            if (SelectedTaxonomy == null)
            {
                txtNodeDefinition.ReadOnly = true;
                return;
            }

            var taxonomyData = SelectedTaxonomy.TaxonomyData;
            var currentNodeDef = txtNodeDefinition.Text;

            if (taxonomyData.NodeDescription != currentNodeDef)
            {
                if (!string.IsNullOrWhiteSpace(taxonomyData.NodeDescription)
                    && string.IsNullOrWhiteSpace(currentNodeDef))
                {
                    if (MessageBox.Show("Are you sure you want to erase Node Description", "Alert",
                                        MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        txtNodeDefinition.ReadOnly = true;
                        return;
                    }

                    currentNodeDef = null;
                }

                taxonomyData.Active = false;
                taxonomyData.TaxonomyInfo.TaxonomyDatas.Add(new TaxonomyData
                                                                {
                                                                    ParentTaxonomyID = taxonomyData.ParentTaxonomyID,
                                                                    NodeName = taxonomyData.NodeName,
                                                                    NodeDescription = currentNodeDef
                                                                });

                AryaTools.Instance.SaveChangesIfNecessary(false, false);
            }

            txtNodeDefinition.ReadOnly = true;
        }

        private void textBoxNodeDefinition_MouseClick(object sender, MouseEventArgs e) { txtNodeDefinition.ReadOnly = false; }

        private void textBoxNodeDefinition_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (txtNodeDefinition.ReadOnly == false)
            {
                txtNodeDefinition.Text = SelectedTaxonomy == null
                                         || string.IsNullOrEmpty(SelectedTaxonomy.TaxonomyData.NodeDescription)
                                             ? string.Empty
                                             : SelectedTaxonomy.TaxonomyData.NodeDescription;
            }
        }

        private void txtSearchBox_Enter(object sender, EventArgs e)
        {
            if (txtSearchBox.Text.Equals(DefaultSearchString))
                txtSearchBox.Text = string.Empty;

            tmrSearch.Start();
            lstResults.Visible = true;
            txtNodeDefinition.Visible = false;
            taxonomyTree.DrawBoldSelectedNode = true;
        }

        private void txtSearchBox_Leave(object sender, EventArgs e)
        {
            if (txtSearchBox.Text.Equals(string.Empty))
                txtSearchBox.Text = DefaultSearchString;
        }

        private void upateCountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Updating Sku Count");

            var skuCount = GetSkuCount(SelectedTaxonomy, true);
            UpdateSkuCount(SelectedTaxonomy, skuCount);

            var rankedCount = GetRankedAttributesCount(SelectedTaxonomy, true);
            UpdateRankedAttributesCount(SelectedTaxonomy, rankedCount);

            UpdateNodeCounts(SelectedTaxonomy, false);

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void updateNodeTimer_Tick(object sender, EventArgs e) { TryStartWorkers(); }

        private void UpdateNodeWorker()
        {
            var currentTaxonomy = Guid.Empty;
            while (true)
            {
                var found = false;

                do
                {
                    found = false;
                    lock (AryaTools.Instance.UpdateNodeWorkerSyncRoot)
                    {
                        var newTaxonomy = SelectedTaxonomy != null ? SelectedTaxonomy.ID : Guid.Empty;
                        if (txtNodeDefinition.ReadOnly
                            && (!currentTaxonomy.Equals(newTaxonomy) || newTaxonomy == Guid.Empty))
                        {
                            currentTaxonomy = newTaxonomy;
                            UpdateDefinitionAndSkuCount(SelectedTaxonomy);
                            found = true;
                        }
                    }
                } while (found);

                do
                {
                    lock (AryaTools.Instance.UpdateNodeWorkerSyncRoot)
                    {
                        try
                        {
                            TaxonomyInfo tax = null;
                            lock (_taxonomiesToRecalculate)
                            {
                                if (_taxonomiesToRecalculate.Count > 0)
                                    tax = _taxonomiesToRecalculate.Dequeue();
                            }
                            if (tax == null)
                                found = false;
                            else if (tax.NodeType == TaxonomyInfo.NodeTypeRegular)
                            {
                                GetSkuCount(tax, true);
                                GetRankedAttributesCount(tax, true);
                                found = true;
                            }
                        }
                        catch (Exception)
                        {
                            found = false;
                        }
                    }
                } while (found);

                //   new ImageManager { ImageSku = EnrichmentImage ?? null }.DisplayImage();
                Thread.Sleep(500);
            }
        }

        private void crossListNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OnCrossListNode == null)
                return;
            OnCrossListNode(GetTaxonomyFromTreeNode(_rightClickedNode));

            if (AryaTools.Instance.AutoSave)
                AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void jumpToParentNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedTaxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
            {
                if (SelectedTaxonomy.DerivedTaxonomies.Count > 0)
                {
                    var cl = Query.FetchCrossListObject(SelectedTaxonomy);

                    if (cl.TaxonomyIDFilter.Count > 1)
                        return;

                    if (cl.TaxonomyIDFilter.Count == 1)
                    {
                        var clTaxonomy = cl.TaxonomyIDFilter[0];

                        SelectedTaxonomy = Taxonomies.Keys.SingleOrDefault(ti => ti.ID == clTaxonomy);
                    }
                }
            }
        }

        #endregion Methods
    }

    internal class NodeSorter : IComparer
    {
        #region Methods (1)

        // Public Methods (1) 

        public int Compare(object x, object y)
        {
            var xNode = x as TreeNode;
            var yNode = y as TreeNode;

            if (xNode == null || yNode == null)
                return 0;

            if (AryaTools.Instance.Forms.TreeForm.OrderNodesBy.Equals(FrmTree.NodeOrder.AlphaNumeric))
                return string.Compare(xNode.Text, yNode.Text, true);

            var xImageIndex = xNode.ImageIndex;
            var yImageIndex = yNode.ImageIndex;

            return xImageIndex != yImageIndex
                       ? xImageIndex.CompareTo(yImageIndex)
                       : string.Compare(xNode.Text, yNode.Text, true);
        }

        #endregion Methods
    }
}