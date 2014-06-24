using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya.HelperForms
{
    public partial class FrmNewSkus : Form
    {
        private TaxonomyInfo _selectedTaxonomy;

        private FrmNewSkus()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        public FrmNewSkus(TaxonomyInfo defaultTaxonomy) : this() { SelectedTaxonomy = defaultTaxonomy; }

        private TaxonomyInfo SelectedTaxonomy
        {
            get { return _selectedTaxonomy; }
            set
            {
                _selectedTaxonomy = value;
                lblSelectedTaxonomy.Text = _selectedTaxonomy.ToString();
            }
        }

        private List<NewItem> NewItems { get; set; }

        private void btnTaxonomy_Click(object sender, EventArgs e)
        {
            Enabled = false;
            AryaTools.Instance.Forms.TreeForm.GetTaxonomy("Create SKU(s) Here", false);
            AryaTools.Instance.Forms.TreeForm.TaxonomySelected += SelectNewTaxonomy;
            AryaTools.Instance.Forms.TreeForm.BringToFront();
        }

        private void SelectNewTaxonomy(object sender, EventArgs e)
        {
            Enabled = true;
 
            if (AryaTools.Instance.Forms.TreeForm.DialogResult == DialogResult.OK)
            {
                var selectedTaxonomy = AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomy;
                if (selectedTaxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    MessageBox.Show("Selected node is cross-listed and cannot be used");
                }
                else
                {
                    SelectedTaxonomy = selectedTaxonomy;
                }
            }

            BringToFront();
        }

        public event SkusAddedEventHandler SkusAdded;

        private void txtItems_KeyDown(object sender, KeyEventArgs e)
        {
            tmrReadItems.Stop();
            tmrReadItems.Start();
        }

        private void tmrReadItems_Tick(object sender, EventArgs e)
        {
            tmrReadItems.Stop();
            ProcessItemIds();
        }

        private void ProcessItemIds()
        {
            var items =
                txtItems.Text.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim().ToUpper())
                    .ToList();

            if (items.Count > 10)
            {
                MessageBox.Show("You should use the Import Utility to load large ItemSets.");
                return;
            }

            NewItems = new List<NewItem>();

            foreach (var item in items)
            {
                var itemId = item;
                var query = PredicateBuilder.False<Sku>();
                query = query.Or(sku => sku.ItemID == itemId);
                var skus = AryaTools.Instance.InstanceData.Dc.Skus.Where(query).Distinct().ToList();

                if (skus.Count > 0)
                {
                    var existingItems = from sku in skus
                        select
                            new NewItem
                            {
                                Add = false,
                                ItemId = itemId,
                                Status = NewItem.NewItemStatus.ExactMatch,
                                ExistingSku = sku,
                                Location = sku.Taxonomy
                            };
                    NewItems.AddRange(existingItems);
                }
                else
                {
                    query = PredicateBuilder.False<Sku>();
                    query = query.Or(sku => sku.ItemID.Contains(itemId));
                    skus =
                        AryaTools.Instance.InstanceData.Dc.Skus.Where(query)
                            .Take((int) numberOfPotentialMatches.Value)
                            .Distinct()
                            .ToList();

                    if (skus.Count > 0)
                    {
                        var existingItems = from sku in skus
                            select
                                new NewItem
                                {
                                    Add = false,
                                    ItemId = itemId,
                                    Status = NewItem.NewItemStatus.PossibleMatch,
                                    ExistingSku = sku,
                                    Location = sku.Taxonomy
                                };
                        NewItems.AddRange(existingItems);
                    }
                    else
                        NewItems.Add(new NewItem
                                     {
                                         Add = true,
                                         ItemId = itemId,
                                         Status = NewItem.NewItemStatus.NewItem,
                                         Location = SelectedTaxonomy
                                     });
                }
            }

            dgvItemStatus.DataSource = NewItems;
            dgvItemStatus.Invalidate();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CreateNewSkus();
            Close();
        }

        private void CreateNewSkus()
        {
            var newSkus = new List<Sku>();

            foreach (var newItem in NewItems.Where(item => item.Add))
            {
                var sku = new Sku { ItemID = newItem.ItemId, SkuType = Framework.Data.AryaDb.Sku.ItemType.Product.ToString()};
                newItem.Location.SkuInfos.Add(new SkuInfo { Sku = sku });
                newSkus.Add(sku);
            }

            SkusAdded(this, new SkusAddedEventHandlerArgs(newSkus));
        }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private class NewItem
        {
            public enum NewItemStatus
            {
                NewItem,
                ExactMatch,
                PossibleMatch
            }

            public bool Add { get; set; }
            public string ItemId { get; set; }
            public NewItemStatus Status { get; set; }
            public Sku ExistingSku { get; set; }
            public TaxonomyInfo Location { get; set; }
        }
    }

    public delegate void SkusAddedEventHandler(object sender, SkusAddedEventHandlerArgs args);

    public class SkusAddedEventHandlerArgs
    {
        public SkusAddedEventHandlerArgs(List<Sku> newSkus) { NewSkus = newSkus; }
        public List<Sku> NewSkus { get; private set; }
    }
}