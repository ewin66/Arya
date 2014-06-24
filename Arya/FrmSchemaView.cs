using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;
using Arya.UserControls;
using Attribute = Arya.Data.Attribute;

namespace Arya
{
    public partial class FrmSchemaView : Form
    {
		#region Constructors (1) 

        public FrmSchemaView()
        {
            InitializeComponent(); 
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

		#endregion Constructors 

		#region Methods (7) 

		// Public Methods (1) 

        public void CloseCurrentTab()
        {
            TabPage currentTab = mainTabControl.SelectedTab;

            if(currentTab == null) return;

            AryaTools.Instance.Forms.SchemaTabs.Remove(currentTab);
            mainTabControl.TabPages.Remove(currentTab);
        }
		// Private Methods (5) 

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseCurrentTab();
        }

        private void FrmSchemaView_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCurrentTab();
            AryaTools.Instance.Forms.SchemaTabs.Clear();
        }

        private void LoadNewTab(SchemaDataGridView schemaGridView, string tabName, string tooltipText)
        {
            var newTab = new TabPage(tabName) { ToolTipText = tooltipText };

            mainTabControl.TabPages.Add(newTab);
            mainTabControl.SelectTab(newTab);

            newTab.Controls.Add(schemaGridView);
            schemaGridView.Dock = DockStyle.Fill;

            AryaTools.Instance.Forms.SchemaTabs.Add(newTab, schemaGridView);

            UpdateTitleAndStatus();

            AryaTools.Instance.Forms.SchemaForm.Show();
            AryaTools.Instance.Forms.SchemaForm.BringToFront();
            if (AryaTools.Instance.Forms.SchemaForm.WindowState == FormWindowState.Minimized)
                AryaTools.Instance.Forms.SchemaForm.WindowState = FormWindowState.Maximized;
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTitleAndStatus();
        }

        private void UpdateTitleAndStatus()
        {
            if (mainTabControl.TabPages.Count == 0)
                Close();

            mainTabControl.TabPages.Cast<TabPage>().Where(tab => AryaTools.Instance.Forms.SchemaTabs.ContainsKey(tab)).
                ForEach(tab => AryaTools.Instance.Forms.SchemaTabs[tab].ShortcutKeysEnabled = false);

            if (mainTabControl.SelectedTab == null ||
                !AryaTools.Instance.Forms.SchemaTabs.ContainsKey(mainTabControl.SelectedTab))
                return;

            Text = String.Format("Schema View - {0}", mainTabControl.SelectedTab.Text);
            AryaTools.Instance.Forms.SchemaTabs[mainTabControl.SelectedTab].ShortcutKeysEnabled = true;

            btnClose.Visible = mainTabControl.TabCount > 0;
            Rectangle rectangle = mainTabControl.GetTabRect(mainTabControl.SelectedIndex);
            btnClose.Location = rectangle.Location;
        }


        internal void LoadTab(List<object> items, object filter)
        {
            string tabName, tooltipText;
            if(items.Count == 0 || (items.Count > 0 && items[0] == null)) return;
            var type = items[0].GetType();

            if (type == typeof(TaxonomyInfo))
            {
                var nodes = items.Cast<TaxonomyInfo>().ToList();
                if (nodes.Count == 1)
                {
                    foreach (var tab in AryaTools.Instance.Forms.SchemaTabs.Where(tab => tab.Value.CurrentTaxonomies != null && tab.Value.CurrentTaxonomies.Count <= 1 && tab.Value.CurrentTaxonomies[0].Equals(nodes[0])))
                    {
                        AryaTools.Instance.Forms.SchemaForm.mainTabControl.SelectedTab = tab.Key;
                        return;
                    }
                }

                var schemaGridView = new SchemaDataGridView(nodes);

                GetTabName(nodes, nodes, out tabName, out tooltipText);
                LoadNewTab(schemaGridView, "T:" + tabName, String.Format("{0} node(s):{1}{2}", nodes.Count, Environment.NewLine, tooltipText));
                return;
            }

            if (type == typeof(Attribute))
            {
                var atts = items.Cast<Attribute>().ToList();
                if (atts.Count == 1)
                {
                    foreach (var tab in AryaTools.Instance.Forms.SchemaTabs.Where(tab => tab.Value.CurrentAttributes != null && tab.Value.CurrentAttributes.Count <= 1 && tab.Value.CurrentAttributes[0].Equals(atts[0])))
                    {
                        AryaTools.Instance.Forms.SchemaForm.mainTabControl.SelectedTab = tab.Key;
                        return;
                    }
                }
                var filterTax = filter is TaxonomyInfo ? (TaxonomyInfo) filter : null;

                var schemaGridView = new SchemaDataGridView(atts, filterTax);
                GetTabName(atts, out tabName, out tooltipText);
                LoadNewTab(schemaGridView, "A:" + tabName, String.Format("{0} attribute(s):{1}{2}", atts.Count, Environment.NewLine, tooltipText));
            }
        }

        public static void GetTabName(ICollection<Attribute> attsToLoad, out string tabName, out string toolTipText)
        {
            // Full taxonomy list is a list of all nodes in the selection
            toolTipText = attsToLoad.OrderBy(node => node.ToString()).Aggregate(String.Empty,
                                                                                (current, attribute) =>
                                                                                current +
                                                                                ((current.Length > 0
                                                                                      ? Environment.NewLine
                                                                                      : String.Empty) + attribute));

            // Also, generate a shorter list for to show in limited space
            var attList = String.Empty;
            foreach (var attribute in attsToLoad.OrderBy(att => att.AttributeName.Length))
            {
                if (attList.Length + attribute.AttributeName.Length > 40)
                {
                    if (attList.Length > 0 && !attList.EndsWith(", ..."))
                        attList += ", ...";
                }
                else
                    attList += (attList.Length > 0 ? ", " : String.Empty) + attribute.AttributeName;
            }
            if (attList.Length > 0)
                attList += " - ";
            tabName = attsToLoad.Count() == 1
                          ? attsToLoad.First().AttributeName
                          : attList + attsToLoad.Count + " attributes";
        }

        public static void GetTabName(ICollection<TaxonomyInfo> nodesSelected, ICollection<TaxonomyInfo> nodesToLoad,
                                      out string tabName, out string toolTipText)
        {
                // Full taxonomy list is a list of all nodes in the selection
                toolTipText = nodesToLoad.OrderBy(node => node.ToString()).Aggregate(String.Empty,
                                                                                     (current, taxonomyInfo) =>
                                                                                     current +
                                                                                     ((current.Length > 0
                                                                                           ? Environment.NewLine
                                                                                           : String.Empty) + taxonomyInfo));

                // Also, generate a shorter list for to show in limited space
                var taxList = String.Empty;
                foreach (var taxonomy in nodesSelected.OrderBy(tax => tax.TaxonomyData.NodeName.Length))
                {
                    if (taxList.Length + taxonomy.TaxonomyData.NodeName.Length > 40)
                    {
                        if (taxList.Length > 0 && !taxList.EndsWith(", ..."))
                            taxList += ", ...";
                    }
                    else
                        taxList += (taxList.Length > 0 ? ", " : String.Empty) + taxonomy.TaxonomyData.NodeName;
                }
                if (taxList.Length > 0)
                    taxList += " - ";
                tabName = nodesSelected.Count == 1
                              ? nodesSelected.First().TaxonomyData.NodeName +
                                (nodesToLoad.Count > 1 ? String.Format(" ({0} nodes)", nodesToLoad.Count) : String.Empty)
                              : taxList + nodesSelected.Count + " nodes" +
                                (nodesSelected.Count < nodesToLoad.Count
                                     ? String.Format(" selected ({0} loaded)", nodesToLoad.Count)
                                     : String.Empty);
            }
            }
}
        #endregion Methods 
 
