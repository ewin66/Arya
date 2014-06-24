using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;

namespace Arya
{
    public partial class FrmSkuLinksView : Form
    {
        private Sku currentSku;

        private Guid loadedSkuID;

        public FrmSkuLinksView()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            bgwPopulateLinks.RunWorkerAsync();
        }

        public Sku SelectedSku
        {
            get { return currentSku; }
            set
            {

                if(value == null || chkPin.Checked) return;

                if( currentSku != null && currentSku == value)
                {
                    this.Show();
                    return;
                }

                lblSkuDescription.Text = currentSku == null ? string.Format("Loading {0} ........ ",value) : string.Format("Current Sku {0}, Loading {1} ........ ", currentSku, value);
                currentSku = value;
                dgvLinkedSku.DataSource = null;
                
                this.Show();
            }
        }


        private void PopulateLinks()
        {
            //dgvLinksTo.DataSource = null;
            //dgvLinkedFrom.DataSource = null;

            if(currentSku == null || (loadedSkuID != Guid.Empty && loadedSkuID == currentSku.ID)) return;
            
            if(loadedSkuID == Guid.Empty || (loadedSkuID != Guid.Empty && loadedSkuID != currentSku.ID))
            {
                loadedSkuID = currentSku.ID;
            }

            var linksToItems =
                currentSku.SkuLinks.Where(sl => sl.Active).Select(
                    sl => new {Sku = sl.Sku1, sl.LinkType, sl.Sku1.Taxonomy }).ToList();

            var linkedFromItems =
                currentSku.SkuLinks1.Where(sl => sl.Active).Select(
                    sl => new {sl.Sku, sl.LinkType, sl.Sku.Taxonomy}).ToList();


            this.InvokeEx(act =>
                              {
                                  lblSkuDescription.Text = currentSku.ItemID;

                                  lblSkuTaxonomy.Text = currentSku.Taxonomy.ToString();

                                  dgvLinksTo.DataSource = linksToItems;
                                  dgvLinkedFrom.DataSource = linkedFromItems;

                                  lblLinksTo.Text = string.Format("References ({0}):", linksToItems.Count);
                                  lblLinkedFrom.Text = string.Format("Referenced by ({0}):", linkedFromItems.Count);

                                  if (linksToItems.Any()) dgvLinksTo.Rows[0].Cells[0].Selected = true;
                                  else if (linkedFromItems.Any()) dgvLinkedFrom.Rows[0].Cells[0].Selected = true;

                              });
        }

        private void FrmSkuLinksView_FormClosing(object sender, FormClosingEventArgs e)
        {
            AryaTools.Instance.Forms.SkuTabs.Values.ForEach(a => a.UnCheckShowHideLinks());
            bgwPopulateLinks.CancelAsync();
        }

        private void dgvLinksFromAndTo_SelectionChanged(object sender, EventArgs e)
        {
            var currentGrid = sender as DataGridView;

            //if (currentGrid == null ||!currentGrid.Focused || currentGrid.SelectedCells.Count != 1) return;

            if (currentGrid == null || currentGrid.SelectedCells.Count != 1) return;

            var linkedSku = currentGrid.SelectedCells[0].OwningRow.Cells[0].Value as Sku;

            if (linkedSku == null) return;

            var prevLinkedSku = dgvLinkedSku.Tag as string;

            if(prevLinkedSku != null && prevLinkedSku.Equals(linkedSku.ItemID) && dgvLinkedSku.DataSource != null) return;

            var linkedTaxonomy = currentGrid.SelectedCells[0].OwningRow.Cells["Taxonomy"].Value as TaxonomyInfo;

            var linkedSkuData = (from ed in linkedSku.EntityInfos.SelectMany(p => p.EntityDatas).Where(p => p.Active)
                      let schemaData =
                          ed.Attribute.SchemaInfos.Where(p => p.TaxonomyID == linkedTaxonomy.ID).Select(
                              p => p.SchemaData).FirstOrDefault()
                      let navOrder = schemaData == null || schemaData.NavigationOrder == 0 ? Decimal.MaxValue : schemaData.NavigationOrder
                      let dispOrder = schemaData == null || schemaData.DisplayOrder == 0 ? Decimal.MaxValue : schemaData.DisplayOrder
                      orderby navOrder , dispOrder , ed.Attribute.AttributeType , ed.Attribute.AttributeName
                                 select new { ed.Attribute.AttributeType, Nav = navOrder == decimal.MaxValue ? string.Empty : string.Format("{0:0.##}", navOrder), Disp = dispOrder == decimal.MaxValue ? string.Empty : string.Format("{0:0.##}", dispOrder), ed.Attribute.AttributeName, ed.Value }).ToList();

            
            dgvLinkedSku.DataSource = linkedSkuData;
            dgvLinkedSku.Tag = linkedSku.ItemID;

        }

        private void bgwPopulateLinks_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if(e.Cancel) break;

                PopulateLinks();

                if (e.Cancel) break;

                Thread.Sleep(500);
            }
        }

        private void dgvLinkedFromAndTo_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (!e.Button.Equals(MouseButtons.Left))
                return;

            var currentGrid = sender as DataGridView;

            if(currentGrid == null) return;
           
            if(e.ColumnIndex < 0 || e.RowIndex < 0) return;

            if (currentGrid[e.ColumnIndex, e.RowIndex] == null)
                return;
            var value = currentGrid[e.ColumnIndex, e.RowIndex].Value;
            if (!(value is TaxonomyInfo))
                return;

            OpenInNewTab((TaxonomyInfo)value);
        }

        static void OpenInNewTab(TaxonomyInfo taxonomy)
        {

            var skuQuery = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
                           let sku = si.Sku
                           where
                               si.Active && sku.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject) &&
                               taxonomy.Equals(si.TaxonomyInfo)
                           select sku;

            AryaTools.Instance.Forms.SkuForm.LoadTab(
                skuQuery, taxonomy, taxonomy.TaxonomyData.NodeName, taxonomy.ToString());
        }

    }
}
