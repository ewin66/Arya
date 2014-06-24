using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using Arya.UserControls;
using System.Diagnostics;

namespace Arya
{
    using System.Collections.Generic;

    public partial class FrmSkuView : Form
    {
        #region Fields (1) 

        private Guid? _skuLoadWaitKey;

        #endregion Fields 
        internal readonly Queue<Guid> LoadQueue = new Queue<Guid>();
        public readonly Dictionary<string, KeyValuePair<Uri, string>> SnippetCache =
        new Dictionary<string, KeyValuePair<Uri, string>>();

        #region Constructors (1) 

        public FrmSkuView()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        #endregion Constructors 

        #region Methods (7) 

        // Public Methods (1) 

        public void UpdateTitleAndStatus()
        {
            if (mainTabControl.TabPages.Count == 0)
            {
                Close();
                return;
            }

            foreach (TabPage tab in mainTabControl.TabPages)
                if (AryaTools.Instance.Forms.SkuTabs.ContainsKey(tab))
                    AryaTools.Instance.Forms.SkuTabs[tab].ShortcutKeysEnabled = false;

            Text = @"SKU View";
            if (mainTabControl.SelectedTab != null && AryaTools.Instance.Forms.SkuTabs.ContainsKey(mainTabControl.SelectedTab))
            {
                Text += @" - " + mainTabControl.SelectedTab.Text;
                AryaTools.Instance.Forms.SkuTabs[mainTabControl.SelectedTab].ShortcutKeysEnabled = true;

                btnClose.Visible = mainTabControl.TabCount > 0;
                Rectangle rectangle = mainTabControl.GetTabRect(mainTabControl.SelectedIndex);
                btnClose.Location = rectangle.Location;
            }

            //Show wait screen if loading skus
            int tabsToLoad = AryaTools.Instance.Forms.SkuForm.LoadQueue.Count;
            if (tabsToLoad > 0)
            {
                if (_skuLoadWaitKey == null)
                    _skuLoadWaitKey = FrmWaitScreen.ShowMessage("Loading SKUs");
                else
                    FrmWaitScreen.UpdateMessage(
                        (Guid) _skuLoadWaitKey,
                        string.Format("Loading SKUs: {0} tab{1}", tabsToLoad, tabsToLoad > 1 ? "s" : ""));
            }

              else
            {
                if (_skuLoadWaitKey != null)
                {
                    FrmWaitScreen.HideMessage((Guid)_skuLoadWaitKey);
                    _skuLoadWaitKey = null;
                }
            }

        }

        // Private Methods (4) 

      

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseCurrentTab();
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTitleAndStatus();
        }

        private void SkuManagerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_skuLoadWaitKey != null)
                FrmWaitScreen.UpdateMessage((Guid) _skuLoadWaitKey, "Waiting for worker threads to finish/abort");

            AryaTools.Instance.Forms.SkuTabs.ForEach(tab => { tab.Value.AbortWorker = true; });
            AryaTools.Instance.Forms.SkuTabs.ForEach(
                tab =>
                {
                    if (tab.Value.WorkerThread != null)
                        tab.Value.WorkerThread.Join();
                    
                });

            if (_skuLoadWaitKey != null)
            {
                FrmWaitScreen.HideMessage((Guid) _skuLoadWaitKey);
                _skuLoadWaitKey = null;
            }

            AryaTools.Instance.Forms.SkuTabs.Clear();
            AryaTools.Instance.Forms.SkuForm.LoadQueue.Clear();
            AryaTools.Instance.Forms.SkuLinksViewForm.Close();
            AryaTools.Instance.Forms.SkuLinksViewForm.Dispose();
        }

        private void SkuManagerMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainTabControl.SelectedTab != null)
                AryaTools.Instance.Forms.SkuTabs[mainTabControl.SelectedTab].EntityGridView_KeyDown(sender, e);
        }

        // Internal Methods (2) 

        internal void CloseCurrentTab()
        {
            TabPage currentTab = mainTabControl.SelectedTab;
            AryaTools.Instance.Forms.SkuTabs.Remove(currentTab);
            mainTabControl.TabPages.Remove(currentTab);
        }

        internal void LoadTab(IQueryable<Sku> loadQuery, TaxonomyInfo taxonomy, string tabName, string toolTipText)
        {
            var sw = new Stopwatch();
            sw.Start();
            if (taxonomy!=null)
                foreach (var tab in AryaTools.Instance.Forms.SkuTabs.Where(tab => tab.Value.CurrentTaxonomy != null && tab.Value.CurrentTaxonomy == taxonomy))
                {
                    AryaTools.Instance.Forms.SkuForm.mainTabControl.SelectedTab = tab.Key;
                    return;
                }

            var newTab = new TabPage(tabName) {ToolTipText = toolTipText};

            mainTabControl.TabPages.Add(newTab);
            mainTabControl.SelectTab(newTab);

            EntityDataGridView entityGridView = taxonomy == null
                                                    ? new EntityDataGridView(loadQuery)
                                                    : new EntityDataGridView(loadQuery, taxonomy);

            
            newTab.Controls.Add(entityGridView);
            entityGridView.Dock = DockStyle.Fill;
            
            UpdateTitleAndStatus();

            AryaTools.Instance.Forms.SkuTabs.Add(newTab, entityGridView);
            AryaTools.Instance.Forms.SkuForm.Show();
            AryaTools.Instance.Forms.SkuForm.BringToFront();
            if (AryaTools.Instance.Forms.SkuForm.WindowState == FormWindowState.Minimized)
                AryaTools.Instance.Forms.SkuForm.WindowState = FormWindowState.Maximized;

            sw.Stop();
            Diagnostics.WriteMessage("Loading - " + tabName.ToString(), "FrmSkuView - LoadTab()", sw.Elapsed);
            sw.Reset();
        }
  
        #endregion Methods 
    }
}

            
          