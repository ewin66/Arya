using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework4.State;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using Arya.UserControls;
using Arya.SpellCheck;

namespace Arya
{
    public partial class FrmTree : Form
    {
        #region Fields (9)

        private const string RegistryKeyIncludeChildren = "IncludeChildren";
        private const string RegistryKeyLoadMultiple = "LoadMultiple";
        public readonly TaxonomyInfo CancelledTaxonomy = new TaxonomyInfo();
        private Screen _currentScreen;
        private AryaView _currentView = AryaView.SkuView;
        private Action<List<TaxonomyInfo>> _oldOnTaxonomyOpen;
        private Action<TaxonomyInfo> _oldOnTaxonomySelected;
        private bool _oldWorkerTimerState;
        private NodeOrder _orderNodesBy;

        #endregion Fields

        #region Enums (2)

        public enum NodeOrder
        {
            AlphaNumeric,
            NodeType
        }

        private enum AryaView
        {
            SkuView,
            SchemaView,
            AttributeView,
            BuildView,
            AssetView
        }

        #endregion Enums

        #region Constructors (1)

        public FrmTree(TaxonomyInfo defaultTaxonomy)
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
            skuViewToolStripMenuItem.Checked = true;
            taxonomyTree.ShowEnrichments = false;
            taxonomyTree.SelectedTaxonomy = defaultTaxonomy;
            OrderNodesBy = NodeOrder.NodeType;
        }

        #endregion Constructors

        #region Properties (4)

        private AryaView CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                skuViewToolStripMenuItem.Checked = _currentView == AryaView.SkuView;
                attributeViewToolStripMenuItem.Checked = _currentView == AryaView.AttributeView;
                schemaViewToolStripMenuItem.Checked = _currentView == AryaView.SchemaView;
//                assetViewToolStripMenuItem.Checked = _currentView == AryaView.AssetView;
            }
        }

        public bool ShowEnrichments
        {
            get { return showEnrichmentsToolStripMenuItem.Checked; }
            set { showEnrichmentsToolStripMenuItem.Checked = value; }
        }

        public bool IncludeChildren
        {
            get { return includeChildrenToolStripMenuItem.Checked; }
            set
            {
                includeChildrenToolStripMenuItem.Checked = value;
                taxonomyTree.includeChildren = value;
            }
        }

        private bool LoadToOneTab
        {
            get { return loadToOneTabToolStripMenuItem.Checked; }
            set { loadToOneTabToolStripMenuItem.Checked = value; }
        }

        public NodeOrder OrderNodesBy
        {
            get { return _orderNodesBy; }
            set
            {
                _orderNodesBy = value;
                nodeTypeToolStripMenuItem.Checked = value.Equals(NodeOrder.NodeType);
                alphaNumericToolStripMenuItem.Checked = value.Equals(NodeOrder.AlphaNumeric);

                taxonomyTree.SortNodes();
            }
        }

        #endregion Properties

        #region Delegates

        public delegate void ChangedEventHandler(object sender, EventArgs e);

        #endregion

        // Events (1) 

        public event ChangedEventHandler TaxonomySelected;

        //private void buildViewToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    CurrentView = AryaView.BuildView;
        //    OpenSelection();
        //}

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (taxonomyTree.SelectedTaxonomies.Count > 0)
            {
                var notes = new FrmNotes(taxonomyTree.SelectedTaxonomies);
                notes.ShowDialog();
            }
        }

        private void workflowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.WorkflowForm.Show();
        }

        private void showRemarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AryaTools.Instance.Forms.RemarksForm.CenterRemarksToScreen();
        }

        //private void TipsandtricksToolStripMenuItemClick(object sender, EventArgs e)
        //{
        //    AryaTools.Instance.Forms.BrowserForm.GotoUrl(
        //        "https://sites.google.com/a/empiriSense.com/arya-knowledge-base/latestarticle", "Tips & Tricks");
        //}

        private void logoutExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //BrowserHelper.ClearCache();
            //Close();
            if (
                MessageBox.Show(
                    "Are you sure you want to close Arya?" + Environment.NewLine
                    + AryaTools.Instance.InstanceData.CurrentProject.ProjectDescription, "Arya",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                AryaTools.Instance.Forms.StartupForm.Close();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(@"This will close all the other Open Views, do you wish to continue?", "Import",
                MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
                return;

            AryaTools.Instance.Forms.CloseAllForms();
            new FrmImportData().Show();
        }
        private void checkSpellingToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            var values = new List<ISpell>();
            var spellCheckForm = AryaTools.Instance.Forms.SpellCheckForm;
            IEnumerable<TaxonomyInfo> selectedTaxonomies = taxonomyTree.SelectedTaxonomies;
            SpellCheckForSelectedNode(values, selectedTaxonomies);                
            if (selectedTaxonomies.Any(st => st.AllChildren.Count() > 0))
            {
                SpellCheckForChildren(values, spellCheckForm, selectedTaxonomies);
            }
            reloadTaxonomyToolStripMenuItem_Click(sender,e);
        }

        private void SpellCheckForChildren(List<ISpell> values, FrmSpellCheck spellCheckForm, IEnumerable<TaxonomyInfo> selectedTaxonomies)
        {
            //pop up to show if user want to do spell check
            DialogResult result = MessageBox.Show(this, "Would you like to check the spelling of the child nodes?", "Spell Check", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                values.Clear();
                var childTaxonomies = new List<TaxonomyInfo>();

                selectedTaxonomies.ToList().ForEach(st =>childTaxonomies.AddRange(st.AllChildren.Where(ch=>ch.TaxonomyData.Active == true)));
                foreach (var item in childTaxonomies)
                {
                    values.Add(item);
                    var tmi = item.TaxonomyMetaInfos.FirstOrDefault(
                          t =>   t.Attribute.AttributeName == Arya.Framework.Properties.Resources.TaxonomyEnrichmentCopyAttributeName);
                    if (tmi != null)
                    {
                        var tmd = tmi.TaxonomyMetaDatas.FirstOrDefault(t => t.Active);
                        if (tmd != null)
                            values.Add(tmd);

                    } 
                }
                DisplaySpellCheck(values, Arya.Properties.Resources.SpellCheckContextMessageForAllChildren); 
            }
            else
            {
                //done.
            }
        }

        private void SpellCheckForSelectedNode(List<ISpell> values, IEnumerable<TaxonomyInfo> selectedTaxonomies)
        {
            foreach (var item in selectedTaxonomies)
            {
                values.Add(item);
                var tmi = item.TaxonomyMetaInfos.FirstOrDefault(
                      t => t.Attribute.AttributeName == Arya.Framework.Properties.Resources.TaxonomyEnrichmentCopyAttributeName);
                if (tmi != null)
                {
                    var tmd = tmi.TaxonomyMetaDatas.FirstOrDefault(t => t.Active);
                    if (tmd != null)
                        values.Add(tmd);

                }
            }
            //if (showEnrichmentsToolStripMenuItem.Checked)
            //{
            //    var tmi = taxonomyTree.SelectedTaxonomy.TaxonomyMetaInfos.FirstOrDefault(
            //           t => t.Attribute.AttributeName == Arya.Framework.Properties.Resources.TaxonomyEnrichmentCopyAttributeName);
            //    if (tmi != null)
            //    {
            //        var tmd = tmi.TaxonomyMetaDatas.FirstOrDefault(t => t.Active);
            //        if (tmd != null)
            //            values.Add(tmd);

            //    }
            //}
            DisplaySpellCheck(values, Arya.Properties.Resources.SpellCheckContextMessageForSelectedNode);
        }
        private void DisplaySpellCheck(List<ISpell> values, string contextMsg)
        {
            try
            {
                var spellCheckForm = AryaTools.Instance.Forms.SpellCheckForm;
                if (values.Count() > 0)
                {
                    spellCheckForm.UpdateSpellCheckView(values, contextMsg);
                    spellCheckForm.InvalidateMethod = InvalidateEdgv;
                    var result = spellCheckForm.ShowDialog();
                }

            }
            catch (ObjectDisposedException ex)
            {
                //do nothing
                // MessageBox.Show("No spelling error in the context.");
            }
        }
        void InvalidateEdgv()
        {
            taxonomyTree.Invalidate();
        }
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(@"This will close all the other Open Views, do you wish to continue?", "Export",
                MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
                return;

            AryaTools.Instance.Forms.CloseAllForms();
            new FrmExportDataNew().Show();
        }

        private void showRemarksToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (showRemarksToolStripMenuItem.Checked)
            {
                AryaTools.Instance.Forms.RemarksForm = new FrmRemark();
                AryaTools.Instance.Forms.RemarksForm.Show();
                AryaTools.Instance.Forms.RemarksForm.CenterRemarksToScreen();
            }
            else
            {
                AryaTools.Instance.Forms.RemarksForm.Close();
                AryaTools.Instance.Forms.RemarksForm = null;
            }
        }

        private void UserPreferences_Click(object sender, EventArgs e)
        {
            //var attrPrefs = new FrmUserPreferences() { Height = 500 };
            var attrPrefs = new FrmUserPreferences();
            attrPrefs.ShowDialog(this);
        }

        private void unitsOfMeasureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.UomForm.Show();
        }

        private void showEnrichmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taxonomyTree.ShowEnrichments = showEnrichmentsToolStripMenuItem.Checked;
        }

        private void projectMetaAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.MetaAttributeForm.Show();
        }
        private void projectCustomDicktionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new FrmSpellCheckCustomDictionary();
            frm.Show();
 
        }

        #region Methods (33)

        // Public Methods (1) 

        public void GetTaxonomy(string acceptButtonText, bool allowRootNodeSelection)
        {
            _oldWorkerTimerState = taxonomyTree.updateNodeTimer.Enabled;
            _oldOnTaxonomySelected = taxonomyTree.OnTaxonomySelected;
            _oldOnTaxonomyOpen = taxonomyTree.OnTaxonomyOpen;

            taxonomyTree.updateNodeTimer.Stop();
            taxonomyTree.OnTaxonomySelected = null;
            taxonomyTree.OnTaxonomyOpen = null;
            taxonomyTree.CustomActionMinLevel = allowRootNodeSelection ? 0 : 1;

            taxonomyTree.ShowAllMenuOptions = false;
            taxonomyTree.CustomActionText = acceptButtonText;
            taxonomyTree.OnCustomAction = ReturnTaxonomy;

            btnAccept.Text = acceptButtonText;
            _pnlSpecialButtons.Visible = true;
            taxonomyMenuStrip.Enabled = false;
            ControlBox = false;
            DialogResult = DialogResult.None;
        }

        // Private Methods (31) 

        private void alphaNumericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderNodesBy = NodeOrder.AlphaNumeric;
        }

        private void assetViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentView = AryaView.AssetView;
            OpenSelection();
        }

        private void attributeFarmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.AttributeFarmView.Show();
        }

        private void attributeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentView = AryaView.AttributeView;
            OpenSelection();
        }

        private void autoSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.AutoSave = autoSaveToolStripMenuItem.Checked;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            ReturnTaxonomy(taxonomyTree.SelectedTaxonomy);
            _pnlSpecialButtons.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ReturnTaxonomy(CancelledTaxonomy);
            _pnlSpecialButtons.Visible = false;
        }

        private void characterMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.CharacterMapForm.Show();
        }

        private void checkpointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.CheckpointForm.Show();
        }

        private void FrmTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Forms.MustCloseForm(e))
                return;

            e.Cancel = AryaTools.Instance.SaveChangesIfNecessary(true, false) == DialogResult.Cancel
                       && Forms.MustCloseForm(e)
                       || MessageBox.Show(
                           "Are you sure you want to close Arya?" + Environment.NewLine
                           + AryaTools.Instance.InstanceData.CurrentProject.ProjectDescription, "Arya",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;

            if (!e.Cancel)
                Application.Exit();
            }
            catch (InvalidOperationException ex)//dont do anyting if the collection has been modified erro rpop up
            {
                
                //throw;
            }
            
        }

        private void FrmTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && taxonomyTree.OnTaxonomyOpen != null
                && taxonomyTree.SelectedTaxonomies.Count > 0 && !taxonomyTree.txtNodeDefinition.Focused
                && !taxonomyTree.txtEnrichmentCopy.Focused)
            {
                e.Handled = true;
                //if (ActiveControl is TaxonomyTreeView && taxonomyTree.ActiveControl is TextBox)
                //    taxonomyTree.DoSearch(true);
                //else
                taxonomyTree.OnTaxonomyOpen(taxonomyTree.SelectedTaxonomies);
            }

            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        try
                        {
                            Clipboard.SetText(taxonomyTree.SelectedTaxonomy.ToString());
                            e.Handled = true;
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case Keys.S:
                        AryaTools.Instance.SaveChangesIfNecessary(false, true);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void FrmTree_Load(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            SelectOptionsFromRegistry();

            taxonomyTree.LoadProject();

            //Text = string.Format("Tree View - {0}", AryaTools.Instance.InstanceData.CurrentProject.ProjectDescription);
            // Use Publish Version on Deployed Applications
            Text = string.Format("Tree - {0} {1}{2}", Assembly.GetExecutingAssembly().GetName().Name,
                ApplicationDeployment.IsNetworkDeployed
                    ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                    : "Not Deployed",
                AryaTools.Instance.InstanceData.CurrentUser.IsAryaReadOnly ? " - Read Only" : string.Empty);

            SnapToLeftInNewScreen();

            //taxonomyTree.OnTaxonomySelected = UpdateCurrentVocabularyContext;
            taxonomyTree.OnTaxonomyOpen = OpenTaxonomyNodeForAttributeNormalization;

            taxonomyTree.OnTaxonomyMove = MoveTaxonomyNode;
            taxonomyTree.OnCrossListNode = CrossListHere;

            if (taxonomyTree.SelectedTaxonomy == null)
            {
                var taxId = WindowsRegistry.GetFromRegistry(WindowsRegistry.RegistryKeyCurrentTaxonomy);
                if (taxId != null)
                {
                    try
                    {
                        var taxonomyId = new Guid(taxId);
                        //previous selected Taxonomy
                        taxonomyTree.SelectedTaxonomy =
                            taxonomyTree.Taxonomies.Select(tax => tax.Key)
                                .FirstOrDefault(tax => tax.ID.Equals(taxonomyId));
                    }
                    catch
                    {
                    }
                }
            }

            autoSaveToolStripMenuItem.Checked = AryaTools.Instance.AutoSave;

            taxonomyTree.updateNodeTimer.Start();

            sw.Stop();
            Diagnostics.WriteMessage("*Loading Project Tree", "FrmTree - taxonomyTree.LoadProject()", sw.Elapsed);
            sw.Reset();
            Show();
            BringToFront();
            if (!AryaTools.Instance.InstanceData.CurrentUser.IsProjectAdmin)
            {
                projectMetaAttributesToolStripMenuItem.Enabled = false;
                projectCustomDictionaryToolStripMenuItem.Enabled = false;
            }
            
        }

        private void FrmTree_LocationChanged(object sender, EventArgs e) { SnapToLeftInNewScreen(); }

        private void includeChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowsRegistry.SaveToRegistry(RegistryKeyIncludeChildren, IncludeChildren.ToString());
            //taxonomyTree.includeChildren = IncludeChildren;
        }

        private void LoadTaxonomy(List<TaxonomyInfo> taxonomies)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Generating list of nodes to open");

            var nodesToLoad = new List<TaxonomyInfo>();

            //Vivek - not sure why we need this
            if (taxonomies.Count == 1 && CurrentView==AryaView.SkuView)
            {
                var nodeType = taxonomies.Single().NodeType;
                if (nodeType == "Derived")
                {
                    Query.DisplayCrossListInSkuView(Query.FetchCrossListObject(taxonomies.Single()));
                    FrmWaitScreen.HideMessage(waitkey);
                    return;
                }
            }

            taxonomies.ForEach(
                tax => nodesToLoad.AddRange(tax.GetNodes(IncludeChildren, CurrentView == AryaView.SchemaView)));

            if (nodesToLoad.Count == 0)
            {
                FrmWaitScreen.HideMessage(waitkey);
                MessageBox.Show("There are no SKUs to load.", "Zero SKUs");
                return;
            }
            if (nodesToLoad.Count > 2000)
            {
                FrmWaitScreen.HideMessage(waitkey);
                MessageBox.Show("A database limit does not allow me to load more than 2000 nodes at once.",
                    "Too many nodes");
                return;
            }
            FrmWaitScreen.HideMessage(waitkey);

            // Now generate the name to be shown on the tab
            string tabName, fullTaxList;
            FrmSchemaView.GetTabName(taxonomies, nodesToLoad, out tabName, out fullTaxList);

            Program.WriteToErrorFile(
                string.Format("{0} Opening: {1} in {2} in {3}", DateTime.Now.ToShortTimeString(), tabName,
                    (LoadToOneTab ? "one tab" : "separate tabs"), CurrentView), true);

            if (LoadToOneTab || CurrentView == AryaView.AttributeView)
            {
                TaxonomyInfo currentTaxonomy = null;
                if (nodesToLoad.Count == 1)
                    currentTaxonomy = nodesToLoad.First();

                switch (CurrentView)
                {
                    //case AryaView.SkuView:
                    //    // TODO: Rewrite load function in Sku View and Attribute View to be able to accept multiple taxonomy nodes
                    //    break;

                    case AryaView.SchemaView:
                        AryaTools.Instance.Forms.SchemaForm.LoadTab(nodesToLoad.Cast<object>().ToList(), null);
                        break;

                    //case AryaView.BuildView:
                    //   // MessageBox.Show("Under Construction");
                    //    AryaTools.Instance.BuildForm.LoadTab(nodesToLoad.Cast<object>().ToList(), null);
                    //    break;

                    default:
                        // Get the list of SKUs

                        var nodesToLoadTaxonomIds = nodesToLoad.Select(p => p.ID).ToList();

                        switch (CurrentView)
                        {
                            case AryaView.SkuView:
                                {
                                    var skuQuery = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
                                                   where
                                                       si.Active && nodesToLoadTaxonomIds.Contains(si.TaxonomyID)
                                                       && si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.Product.ToString()
                                                   select si.Sku;

                                    AryaTools.Instance.Forms.SkuForm.LoadTab(skuQuery, currentTaxonomy, tabName,
                                        fullTaxList);

                                    break;
                                }
                            case AryaView.AssetView:
                                {
                                    var skuQuery = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
                                                   where
                                                       si.Active && nodesToLoadTaxonomIds.Contains(si.TaxonomyID)
                                                       && si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.EnrichmentImage.ToString()
                                                   select si.Sku;

                                    AryaTools.Instance.Forms.SkuForm.LoadTab(skuQuery, currentTaxonomy, tabName,
                                        fullTaxList);

                                    break;
                                }

                            case AryaView.AttributeView:

                                //IQueryable<EntityInfo> entityInfoQuery = from ed in AryaTools.Instance.InstanceData.Dc.EntityDatas
                                //                                         join ei in AryaTools.Instance.InstanceData.Dc.EntityInfos on ed.EntityID equals
                                //                                             ei.ID
                                //                                         join si in AryaTools.Instance.InstanceData.Dc.SkuInfos on ei.SkuID equals si.SkuID
                                //                                         where
                                //                                             ed.Active && si.Active &&
                                //                                             nodesToLoadTaxonomIds.Contains(si.TaxonomyID)
                                //                                         select ei;

                                //IQueryable<EntityInfo> entityInfoQuery = from ed in AryaTools.Instance.InstanceData.Dc.EntityDatas
                                //                                         join ei in AryaTools.Instance.InstanceData.Dc.EntityInfos on ed.EntityID equals
                                //                                             ei.ID
                                //                                         join si in AryaTools.Instance.InstanceData.Dc.SkuInfos on ei.SkuID equals si.SkuID
                                //                                         where
                                //                                             ed.Active && si.Active 
                                //                                         select ei;

                                //clone the nodesToList collection to remove dependency
                                AryaTools.Instance.Forms.AttributeView.LoadQuery(tabName, nodesToLoad.ToList());
                                break;
                            case AryaView.SchemaView:
                                //Wonder how control got here!!!?
                                break;
                        }
                        break;
                }
            }
            else // load in separate tabs
            {
                var result = DialogResult.None;
                foreach (var tax in nodesToLoad)
                {
                   var currentTaxonomy = tax;
                   if (nodesToLoad.Count  <= AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.TabsThreshold)
                   {
                       switch (CurrentView)
                       {
                        
                           case AryaView.SchemaView:
                               AryaTools.Instance.Forms.SchemaForm.LoadTab(new List<object> { currentTaxonomy }, null);
                               break;

                           default:
                             
                               switch (CurrentView)
                               {
                                   case AryaView.SkuView:
                                       {
                                           var skuQuery =
                                              AryaTools.Instance.InstanceData.Dc.SkuInfos.Where(
                                               si =>
                                                 si.Active && si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.Product.ToString()
                                                  && si.TaxonomyID == tax.ID).Select(si => si.Sku);

                                           AryaTools.Instance.Forms.SkuForm.LoadTab(skuQuery, tax,
                                          currentTaxonomy.TaxonomyData.NodeName, tax.ToString());
                                           break;
                                       }

                                   case AryaView.AssetView:
                                       {
                                           var skuQuery =
                                               AryaTools.Instance.InstanceData.Dc.SkuInfos.Where(
                                                   si =>
                                                       si.Active && si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.EnrichmentImage.ToString()
                                                       && si.TaxonomyID == currentTaxonomy.ID).Select(si => si.Sku);

                                           AryaTools.Instance.Forms.SkuForm.LoadTab(skuQuery, currentTaxonomy,
                                               currentTaxonomy.TaxonomyData.NodeName, currentTaxonomy.ToString());
                                           break;
                                       }
                                   case AryaView.AttributeView:
                                       {
                                           // TODO: Check attribute view over...
                                           var skuQuery =
                                               AryaTools.Instance.InstanceData.Dc.SkuInfos.Where(
                                                   si =>
                                                       si.Active && si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.Product.ToString()
                                                       && si.TaxonomyID == currentTaxonomy.ID).Select(si => si.Sku);

                                           AryaTools.Instance.Forms.AttributeView.LoadQuery(skuQuery,
                                               currentTaxonomy.TaxonomyData.NodeName);
                                           break;
                                       }

                                   case AryaView.SchemaView:
                                       //Wonder how control got here!!!?
                                       break;
                               }//end of switch inner
                               break; // Load only the first Node
                       }//end of switch outer

                   } //end: foreach node in the list of nodes to load
                   else 
                   {
                       var sModalText =
                                       String.Format(
                                           "You have chosen to open {0} nodes in separate tabs. This may take some time. Do you still want to open these nodes?",
                                           nodesToLoad.Count);
                       Invoke(
                               (MethodInvoker)
                               delegate { result = MessageBox.Show(this, sModalText, "Alert", MessageBoxButtons.YesNo); });
                       if (result == DialogResult.No)
                           break;
                       else
                       foreach (var taxonomy in nodesToLoad)
                       {
                           currentTaxonomy = taxonomy;
                           switch (CurrentView)
                           {
                               case AryaView.SkuView:
                                   {
                                       
                                       var skuQuery =
                                            AryaTools.Instance.InstanceData.Dc.SkuInfos.Where(
                                             si =>
                                               si.Active && si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.Product.ToString()
                                                && si.TaxonomyID == currentTaxonomy.ID).Select(si => si.Sku);
                                       AryaTools.Instance.Forms.SkuForm.LoadTab(skuQuery, currentTaxonomy,currentTaxonomy.TaxonomyData.NodeName, currentTaxonomy.ToString());
                                       break;
                                   }

                               case AryaView.SchemaView:
                                {
                                    AryaTools.Instance.Forms.SchemaForm.LoadTab(new List<object> { currentTaxonomy }, null);
                                    break;
                                }                               
                           }                       
                       }
                       break;
                      }
                    }
                }
                 //end: load in one tab vs. load in separate tabs
            }

        private void loadToOneTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowsRegistry.SaveToRegistry(RegistryKeyLoadMultiple, LoadToOneTab.ToString());
        }

        private void CrossListHere(TaxonomyInfo taxonomyInfo)
        {
            var taxonomyToCrossList = taxonomyInfo;
            GetTaxonomy("Cross List Here", true);
            TaxonomySelected += (s, ea) =>
                                {
                                    if (DialogResult != DialogResult.OK)
                                        return;

                                    var destination = taxonomyTree.SelectedTaxonomy;
                                    if (destination != null
                                        && (destination.Equals(taxonomyToCrossList)
                                            || destination.IsChildOf(taxonomyToCrossList)))
                                    {
                                        MessageBox.Show(@"Cannot Cross-List this node to itself or under its own child!");
                                        return;
                                    }

                                    var newCrossListNode = new TaxonomyInfo
                                                           {
                                                               ShowInTree = true,
                                                               ProjectID = destination.ProjectID,
                                                               NodeType = "Derived"
                                                           };
                                    AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(newCrossListNode);

                                    var previousNode = taxonomyToCrossList.TaxonomyData;
                                    var nodeName = "Cross List from - " + taxonomyToCrossList.ToString();
                                    var nodeDescription = previousNode.NodeDescription;
                                    newCrossListNode.TaxonomyDatas.Add(new TaxonomyData
                                                                       {
                                                                           NodeName = nodeName,
                                                                           ParentTaxonomyInfo =
                                                                               destination,
                                                                           NodeDescription =
                                                                               nodeDescription
                                                                       });

                                    taxonomyTree.AddCrossListNode(destination, newCrossListNode);

                                    //taxonomyTree.UpdateNode(taxonomyToCrossList);
                                    //taxonomyTree.SelectedTaxonomy = newCrossListNode;

                                    var cl = new CrossListCriteria(new List<Guid> {taxonomyToCrossList.ID}, null, null,
                                        false, IncludeChildren);

                                    var derivedTaxonomy = newCrossListNode.DerivedTaxonomies.FirstOrDefault();
                                    if (derivedTaxonomy != null)
                                        derivedTaxonomy.Expression = cl.SerializeToXElement();
                                    else
                                    {
                                        newCrossListNode.DerivedTaxonomies.Add(new DerivedTaxonomy
                                                                               {
                                                                                   Expression =
                                                                                       cl
                                                                                       .SerializeToXElement
                                                                                       ()
                                                                               });
                                    }

                                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                                };
        }

        private void MoveTaxonomyNode(TaxonomyInfo taxonomyInfo)
        {
            var taxonomyToMove = taxonomyInfo;
            GetTaxonomy("Move Here", true);
            TaxonomySelected += (s, ea) =>
                                {
                                    if (DialogResult != DialogResult.OK)
                                        return;

                                    var destination = taxonomyTree.SelectedTaxonomy;
                                    if (destination != null
                                        && (destination.Equals(taxonomyToMove) || destination.IsChildOf(taxonomyToMove)))
                                    {
                                        MessageBox.Show(@"Cannot move this node to itself or under its own child!");
                                        return;
                                    }

                                    var previousNode = taxonomyToMove.TaxonomyData;
                                    var nodeName = previousNode.NodeName;
                                    var nodeDescription = previousNode.NodeDescription;
                                    taxonomyToMove.TaxonomyDatas.Add(new TaxonomyData
                                                                     {
                                                                         NodeName = nodeName,
                                                                         ParentTaxonomyInfo = destination,
                                                                         NodeDescription = nodeDescription
                                                                     });
                                    previousNode.Active = false;

                                    taxonomyTree.UpdateNode(taxonomyToMove);
                                    taxonomyTree.SelectedTaxonomy = taxonomyToMove;

                                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                                };
        }

        private void nodeTypeToolStripMenuItem_Click(object sender, EventArgs e) { OrderNodesBy = NodeOrder.NodeType; }

        private void OpenSelection()
        {
            if (taxonomyTree.OnTaxonomyOpen != null && taxonomyTree.SelectedTaxonomies.Count > 0)
                taxonomyTree.OnTaxonomyOpen(taxonomyTree.SelectedTaxonomies);
        }

        private void OpenTaxonomyNodeForAttributeNormalization(List<TaxonomyInfo> taxonomies)
        {
            if (taxonomies == null)
                return;

            var sw = new Stopwatch();
            sw.Start();
            LoadTaxonomy(taxonomies);
            sw.Stop();
            Diagnostics.WriteMessage("Opening " + CurrentView + " from Tree", "LoadTaxonomy(taxonomies)", sw.Elapsed, -1,
                taxonomies.Count);
        }

        private void queryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.QueryForm.SetFormType(FrmQueryView.QueryFormType.QueryView);
            AryaTools.Instance.Forms.QueryForm.Show();
            AryaTools.Instance.Forms.QueryForm.BringToFront();
        }

        private void reloadTaxonomyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taxonomyTree.ResetSkuCount(null);

            taxonomyTree.updateNodeTimer.Stop();

            _pnlSpecialButtons.SuspendLayout();
            _tableSpecialButtons.SuspendLayout();
            taxonomyMenuStrip.SuspendLayout();
            SuspendLayout();

            Controls.Remove(taxonomyTree);
            Controls.Remove(_pnlSpecialButtons);
            Controls.Remove(taxonomyMenuStrip);
            taxonomyTree = null;

            taxonomyTree = new TaxonomyTreeView
                           {
                               AutoSize = true,
                               AutoSizeMode = AutoSizeMode.GrowAndShrink,
                               Dock = DockStyle.Fill,
                               Font = new Font("Microsoft Sans Serif", 8.75F),
                               Location = new Point(0, 24),
                               Name = "taxonomyTree",
                               OpenNodeOptionEnabled = true,
                               ShowEnrichments = showEnrichmentsToolStripMenuItem.Checked,
                               Padding = new Padding(3),
                               Size = new Size(312, 390),
                               TabIndex = 0
                           };

            Controls.Add(taxonomyTree);
            Controls.Add(_pnlSpecialButtons);
            Controls.Add(taxonomyMenuStrip);

            _pnlSpecialButtons.ResumeLayout(false);
            _tableSpecialButtons.ResumeLayout(false);
            _tableSpecialButtons.PerformLayout();
            taxonomyMenuStrip.ResumeLayout(false);
            taxonomyMenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

            taxonomyTree.LoadProject();

            taxonomyTree.OnTaxonomyOpen = OpenTaxonomyNodeForAttributeNormalization;

            taxonomyTree.OnTaxonomyMove = MoveTaxonomyNode;
            taxonomyTree.OnCrossListNode = CrossListHere;

            if (taxonomyTree.SelectedTaxonomy == null)
            {
                var taxId = WindowsRegistry.GetFromRegistry(WindowsRegistry.RegistryKeyCurrentTaxonomy);
                if (taxId != null)
                {
                    try
                    {
                        var taxonomyId = new Guid(taxId);
                        //previous selected Taxonomy
                        taxonomyTree.SelectedTaxonomy =
                            taxonomyTree.Taxonomies.Select(tax => tax.Key)
                                .FirstOrDefault(tax => tax.ID.Equals(taxonomyId));
                    }
                    catch
                    {
                    }
                }
            }

            taxonomyTree.updateNodeTimer.Start();
        }

        private void ReturnTaxonomy(TaxonomyInfo taxonomyInfo)
        {
            if (_oldWorkerTimerState)
                taxonomyTree.updateNodeTimer.Start();

            _pnlSpecialButtons.Visible = false;
            Refresh();

            taxonomyTree.OnTaxonomySelected = _oldOnTaxonomySelected;
            taxonomyTree.OnTaxonomyOpen = _oldOnTaxonomyOpen;

            taxonomyTree.ShowAllMenuOptions = true;
            taxonomyTree.OnCustomAction = null;

            DialogResult = (taxonomyInfo == null && taxonomyTree.CustomActionMinLevel > 0)
                           || (taxonomyInfo != null && taxonomyInfo.Equals(CancelledTaxonomy))
                ? DialogResult.Cancel
                : DialogResult.OK;
            taxonomyMenuStrip.Enabled = true;
            ControlBox = true;

            TaxonomySelected(this, EventArgs.Empty);
            TaxonomySelected = null;
        }

        private void saveNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.SaveChangesIfNecessary(false, true);
        }

        private void schemaViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentView = AryaView.SchemaView;
            OpenSelection();
        }

        private void SelectOptionsFromRegistry()
        {
            var includeChildrenTaxnNorm = WindowsRegistry.GetFromRegistry(RegistryKeyIncludeChildren);
            if (!string.IsNullOrEmpty(includeChildrenTaxnNorm) && includeChildrenTaxnNorm.Equals(true.ToString()))
                IncludeChildren = true;

            var selectMultipleTaxnNorm = WindowsRegistry.GetFromRegistry(RegistryKeyLoadMultiple);
            if (!string.IsNullOrEmpty(selectMultipleTaxnNorm) && selectMultipleTaxnNorm.Equals(true.ToString()))
                LoadToOneTab = true;
        }

        private void skuLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.SkuLinksForm.Show();
        }

        private void skuGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.SkuGroupsForm.Show();
        }

        private void skuViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentView = AryaView.SkuView;
            OpenSelection();
        }

        private void SnapToLeftInNewScreen()
        {
            var newScreen = Screen.FromControl(this);
            if (_currentScreen != null && newScreen.Equals(_currentScreen))
                return;

            _currentScreen = newScreen;

            Top = 0;
            Height = _currentScreen.WorkingArea.Height;
            Width = _currentScreen.WorkingArea.Width/3;
            if (Width > 400)
                Width = 400;
        }

        // Internal Methods (1) 

        internal void UpdateCurrentNodeSkuCount()
        {
            taxonomyTree.UpdateNodeCounts(taxonomyTree.SelectedTaxonomy, true);
        }

        #endregion Methods
    }
}