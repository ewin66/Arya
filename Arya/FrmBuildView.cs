using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;
using Arya.UserControls;

namespace Arya
{
    public partial class FrmBuildView : Form
    {
        //private readonly Timer _changeMonitor = new Timer();

        private readonly Dictionary<TaxonomyInfo, DisplayColumns> _display =
            new Dictionary<TaxonomyInfo, DisplayColumns>();

        public FrmBuildView()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
            //_changeMonitor.Interval = 500;
            //_changeMonitor.Start();
            //_changeMonitor.Tick += changeMonitor_Tick;
        }

        //private void changeMonitor_Tick(object sender, EventArgs e) { UpdateTabStatus(); }

        //private void UpdateTabStatus()
        //{
        //    foreach (var tabIndex in SaveTabs)
        //    {
        //        if (!mainTabControl.TabPages[tabIndex.Key].Text.EndsWith("*") && tabIndex.Value)
        //            mainTabControl.TabPages[tabIndex.Key].Text += "*";
        //    }
        //}

        //public void SetTabStatus(bool isModified)
        //{
        //    var tabIndexCount = SaveTabs.Count();
        //    for (var i = 0; i < tabIndexCount; i++)
        //    {
        //        mainTabControl.TabPages[i].Text = mainTabControl.TabPages[i].Text.Split('*')[0];
        //        SaveTabs[i] = isModified;
        //    }

        //    SaveAlert = false;
        //    SaveStatus = "Changes Saved";
        //}

        internal void LoadTab(List<Sku> items, object filter)
        {
            if (items.Count == 0 || (items.Count > 0 && items[0] == null))
                return;

            var nodeItems = new Dictionary<TaxonomyInfo, List<Sku>>();

            foreach (var sku in items)
            {
                var tax = sku.SkuInfos.Where(a => a.Active).Select(t => t.TaxonomyInfo).FirstOrDefault();
                if (nodeItems.Keys.Contains(tax))
                {
                    if (!nodeItems[tax].Contains(sku))
                        nodeItems[tax].Add(sku);
                }
                else
                    nodeItems.Add(tax, new List<Sku> {sku});
            }

            foreach (var node in nodeItems)
            {
                DisplayColumns cols;
                if (_display.Keys.Contains(node.Key))
                    cols = _display[node.Key];
                else
                {
                    cols = new DisplayColumns(new List<string>(), new List<string>(), new Dictionary<string, int>());
                    _display.Add(node.Key, cols);
                }
                var buildGridView = new BuildDataGridView(node.Key, node.Value, cols);
                string tabName;
                string tooltipText;
                FrmSchemaView.GetTabName(nodeItems.Keys, nodeItems.Keys, out tabName, out tooltipText);
                LoadNewTab(buildGridView, "T:" + tabName,
                    string.Format("{0} node(s):{1}{2}", nodeItems.Count, Environment.NewLine, tooltipText));
            }

            //List<TaxonomyInfo> nodes = new List<TaxonomyInfo>();

            //foreach (var sku in items)
            //{
            //    var tax = sku.SkuInfos.Where(a => a.Active).Select(t => t.TaxonomyInfo).FirstOrDefault();
            //    if (!nodes.Contains(tax))
            //        nodes.Add(tax);
            //}

            //BuildDataGridView buildGridView = new BuildDataGridView(nodes, items);
            //AryaTools.GetTabName(nodes, nodes, out tabName, out tooltipText);
            //LoadNewTab(buildGridView, "T:" + tabName, string.Format("{0} node(s):{1}{2}", nodes.Count, Environment.NewLine, tooltipText));
        }

        private void LoadNewTab(BuildDataGridView buildGridView, string tabName, string tooltipText)
        {
            var newTab = new TabPage(tabName) {ToolTipText = tooltipText};

            mainTabControl.TabPages.Add(newTab);
            mainTabControl.SelectTab(newTab);

            buildGridView.PageIndex = mainTabControl.TabPages.Count - 1;

            //SaveTabs.Add((mainTabControl.TabPages.Count - 1), false);

            //mainTabControl.TabPages[newTab.N

            newTab.Controls.Add(buildGridView);
            buildGridView.Dock = DockStyle.Fill;

            //AryaTools.Instance.SchemaTabs.Add(newTab, buildGridView);

            //UpdateTitleAndStatus();

            AryaTools.Instance.Forms.BuildForm.Show();
            AryaTools.Instance.Forms.BuildForm.BringToFront();
            if (AryaTools.Instance.Forms.BuildForm.WindowState == FormWindowState.Minimized)
                AryaTools.Instance.Forms.BuildForm.WindowState = FormWindowState.Maximized;
        }
    }

    public class DisplayColumns
    {
        public readonly List<string> AttributeHeaders;
        public readonly Dictionary<string, int> ColumnWidth;
        public readonly List<string> ItemHeaders;

        public DisplayColumns(List<string> attributeHeaders, List<string> itemHeaders,
            Dictionary<string, int> columnWidth)
        {
            if (itemHeaders.Count == 0)
                ItemHeaders = new List<string> {"Value", "UoM"};
            else
                ItemHeaders = itemHeaders;

            AttributeHeaders = attributeHeaders;

            ColumnWidth = columnWidth;
        }
    }
}