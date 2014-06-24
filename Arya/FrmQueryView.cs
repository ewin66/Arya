using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;
using Arya.HelperForms;
using System.Diagnostics;

namespace Arya
{
    public partial class FrmQueryView : Form
    {
        #region Fields (9)

        private readonly HashSet<string> _attributeTypeFilters = new HashSet<string>();

        private readonly object[] _defaultFilterTypes = new object[] { "is equal to", "contains", "begins with", "ends with" };

        private readonly object[] _itemIdFilterTypes = new object[] { "is in" };
        private bool _includeChildren;
        private string _lastSortedAscendingBy;
        private bool _matchAllTerms = true;
        private TaxonomyInfo _selectedCrossListNodeFromTreeView;
        private List<SearchResult> _taxResults;
        private string _valueSelection;
        public Group SelectedSkuGroupForQueryView;

        #endregion Fields

        #region Constructors (1)

        public FrmQueryView()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;

            ddDefaultAction.Items.AddRange(new object[] { "in SKU View", "in Attribute View" });
            ddDefaultAction.SelectedIndex = 0;

            ddFieldName.Items.AddRange(new object[] { "Item Id", "Value", "Attribute Name", "UoM" });
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField1Name);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField2Name);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField3Name);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField4Name);
            if (!AryaTools.Instance.InstanceData.CurrentProject.EntityField5IsStatus)
                SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField5Name);
            ddFieldName.SelectedIndex = 0;

            UpdateFilterTypes();
        }

        #endregion Constructors

        #region Properties (3)

        private IQueryable<Sku> CurrentQuery
        {
            get
            {
                return Query.GetFilteredSkus(
                    TaxonomyFilters.GetTaxonomyIDs(_includeChildren), ValueFilters, _attributeTypeFilters,
                    _matchAllTerms);
            }
        }

        private List<TaxonomyInfo> TaxonomyFilters { get; set; }

        private List<ValueFilter> ValueFilters { get; set; }

        #endregion Properties

        #region Methods (28)

        // Private Methods (27) 

        public enum QueryFormType
        {
            QueryView,
            CrossListDefinition,
            SkuGroup
        }

        private void AddValueFilter()
        {
            if (string.IsNullOrEmpty(txtValue.Text))
                return;

            if (txtValue.Text == "(2k items max)")
            {
                ValueFilters = null;
                return;
            }

            ProcessValueFilter();
        }

        private void ProcessValueFilter()
        {
            if (ValueFilters == null)
                ValueFilters = new List<ValueFilter>();

            if (txtValue.Text != "(2k items max)")
                ValueFilters.Add(
                    new ValueFilter { Field = ddFieldName.Text, FilterType = ddValueFilterType.Text, Value = txtValue.Text });

            string valueFilterList = string.Empty;
            ValueFilters.ForEach(
                filter =>
                {
                    valueFilterList += (string.IsNullOrEmpty(valueFilterList)
                                            ? string.Empty
                                            : (_matchAllTerms ? " And" : " Or") + Environment.NewLine) +
                                       string.Format("{0} {1} [{2}]", filter.Field, filter.FilterType, filter.Value);
                });

            _valueSelection = valueFilterList;
            //populateSelection(TaxonomyFilters, valueSelection, _includeChildren);
            //lblValue.Text = valueFilterList;
            txtValue.Text = string.Empty;
            txtValue.Refresh();
        }

        private void PopulateSelection(List<TaxonomyInfo> taxSelection, string valSelection, bool includeChildren)
        {
            string displayText = null;
            if (taxSelection != null && taxSelection.Any())
            {
                displayText = taxSelection.Aggregate("TAXONOMY\r\n", (current, t) => current + (t.ToString() + "\r\n"));

                if (includeChildren)
                    displayText += "and child nodes";
            }

            if (!string.IsNullOrEmpty(valSelection))
            {
                displayText += "\r\n\r\nFIELDS\r\n" + valSelection;
            }

            txtBoxSelection.Text = displayText;
        }

        private void btnAnd_Click(object sender, EventArgs e)
        {
            _matchAllTerms = true;
            btnOr.Visible = false;
            AddValueFilter();
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);
        }

        private void btnCrossList_Click(object sender, EventArgs e)
        {
            AddValueFilter();

            //Adding an object to cross list- Include Children is Set false to prevent storing multiple IDs in the object
            var cl = new CrossListCriteria(
                TaxonomyFilters.GetTaxonomyIDs(false), ValueFilters, _attributeTypeFilters, _matchAllTerms,
                _includeChildren);

            var derivedTaxonomy = _selectedCrossListNodeFromTreeView.DerivedTaxonomies.FirstOrDefault();
            if (derivedTaxonomy != null)
                derivedTaxonomy.Expression = cl.SerializeToXElement();
            else
                _selectedCrossListNodeFromTreeView.DerivedTaxonomies.Add(
                    new DerivedTaxonomy { Expression = cl.SerializeToXElement() });

            AryaTools.Instance.SaveChangesIfNecessary(false, false);

            //var derivedTaxonomyExists = AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.Any(t => t.TaxonomyID == SelectedCrossListNodeFromTreeView.ID);
            //if (derivedTaxonomyExists)
            //{
            //    //Update the Rule
            //    DerivedTaxonomy ExistingDerivedTaxonomy = AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.Single(t => t.TaxonomyID == SelectedCrossListNodeFromTreeView.ID);
            //    ExistingDerivedTaxonomy.Expression = cl.SerializeToXElement();              
            //}
            //else
            //{
            //    //Insert New Rule
            //    DerivedTaxonomy newDerivedTaxonomy = new DerivedTaxonomy { TaxonomyID = SelectedCrossListNodeFromTreeView.ID, Expression = cl.SerializeToXElement() };
            //    AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.InsertOnSubmit(newDerivedTaxonomy);
            //}
            //AryaTools.Instance.InstanceData.Dc.SubmitChanges();

            MessageBox.Show("Expression Added to Cross List", "Arya");
            AryaTools.Instance.Forms.TreeForm.UpdateCurrentNodeSkuCount();

            //AryaTools.Instance.Forms.SkuForm.LoadTab(GetCrossListedSkus(cl), null, "Query", "Query"); //Sku View
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            AddValueFilter();

            if ((TaxonomyFilters == null || TaxonomyFilters.Count == 0) &&
                MessageBox.Show("Do you want to query the entire project?", "ID-10T Warning", MessageBoxButtons.YesNo) ==
                DialogResult.No)
                return;

            IQueryable<Sku> query = CurrentQuery;

            if (query == null)
            {
                MessageBox.Show("Nothing to search!");
                return;
            }

            if (ddDefaultAction.SelectedIndex == 0)
                AryaTools.Instance.Forms.SkuForm.LoadTab(query, null, "Query", "Query"); //Sku View
            else
                AryaTools.Instance.Forms.AttributeView.LoadQuery(query, "Query"); //Attribute View
        }

        private void btnOr_Click(object sender, EventArgs e)
        {
            _matchAllTerms = false;
            btnAnd.Visible = false;
            AddValueFilter();
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            AddValueFilter();
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);

            if ((TaxonomyFilters == null || TaxonomyFilters.Count == 0) &&
                MessageBox.Show(
                    "You forgot to tell me which taxonomy you wanted to query!\nLove, Arya\nDo you want to query the entire project?",
                    "Search everywhere?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            IQueryable<Sku> query = CurrentQuery;
            if (query == null)
            {
                MessageBox.Show("Nothing to search!");
                return;
            }

            lblStatus.Text = "Searching...";
            lblStatus.Refresh();
            DateTime startTime = DateTime.Now;

            IQueryable<SearchResult> taxQuery = from sku in query
                                                let skuInfo = sku.SkuInfos.First(si => si.Active)
                                                group skuInfo by new { skuInfo.TaxonomyID, skuInfo.SkuID }
                                                    into taxSkuGrp
                                                    group taxSkuGrp by taxSkuGrp.Key.TaxonomyID
                                                        into taxGrp
                                                        join tax in AryaTools.Instance.InstanceData.Dc.TaxonomyInfos on taxGrp.Key equals
                                                            tax.ID
                                                        select new SearchResult { Taxonomy = tax, SkuCount = taxGrp.Count() };

            _taxResults = taxQuery.ToList();
            dgvTaxonomyResults.DataSource = _taxResults;

            for (int i = 0; i < dgvTaxonomyResults.RowCount; i++)
            {
                if (i % 2 != 0)
                    dgvTaxonomyResults.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;
                else
                    dgvTaxonomyResults.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleGreyRegular;

            }

            int skuCount = _taxResults.Sum(tax => tax.SkuCount);
            TimeSpan timeTaken = DateTime.Now.Subtract(startTime);

            lblStatus.Text = string.Format(
                "{0} SKUs found. Time taken: {1:00}:{2:00}", skuCount, timeTaken.TotalMinutes, timeTaken.Seconds);
            sw.Stop();
            Diagnostics.WriteMessage("Query", "FrmQueryView - btnSearch_Click", sw.Elapsed, skuCount);
            sw.Reset();
        }

        private void chkDisplayAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDisplayAttributes.Checked)
                _attributeTypeFilters.Add("Disp");
            else
                _attributeTypeFilters.Remove("Disp");
        }

        private void chkGlobalAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGlobalAttributes.Checked)
                _attributeTypeFilters.Add("Global");
            else
                _attributeTypeFilters.Remove("Global");
        }

        private void chkNavigationAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNavigationAttributes.Checked)
                _attributeTypeFilters.Add("Nav");
            else
                _attributeTypeFilters.Remove("Nav");
        }

        private void chkInSchemaAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInSchemaAttributes.Checked)
                _attributeTypeFilters.Add("InSchema");
            else
                _attributeTypeFilters.Remove("InSchema");
        }

        private void ClearValueFilters()
        {
            ValueFilters = null;
            _valueSelection = null;
            //lblValue.Text = "Any Value";
            btnAnd.Visible = true;
            btnOr.Visible = true;
        }

        private void ddItemValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFilterTypes();
        }

        private void dgvTaxonomyResults_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string propertyName = dgvTaxonomyResults.Columns[e.ColumnIndex].DataPropertyName;
            bool sortAscending = true;
            if (!string.IsNullOrEmpty(_lastSortedAscendingBy) && _lastSortedAscendingBy.Equals(propertyName))
            {
                sortAscending = false;
                _lastSortedAscendingBy = null;
            }
            else
                _lastSortedAscendingBy = propertyName;

            PropertyInfo property = (typeof(SearchResult)).GetProperty(propertyName);

            _taxResults =
                (sortAscending
                     ? _taxResults.OrderBy(col => property.GetValue(col, null))
                     : _taxResults.OrderByDescending(col => property.GetValue(col, null))).ToList();
            dgvTaxonomyResults.DataSource = _taxResults;
        }

        private void dgvTaxonomyResults_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ShowHideLimitTaxonomyFromResults();
        }

        private void dgvTaxonomyResults_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            ShowHideLimitTaxonomyFromResults();
        }

        private void FrmQueryView_Load(object sender, EventArgs e)
        {
            ShowHideLimitTaxonomyFromResults();
        }

        private IQueryable<Sku> GetCrossListedSkus(CrossListCriteria cl)
        {
            return Query.GetFilteredSkus(
                cl.TaxonomyIDFilter, cl.ValueFilters, cl.AttributeTypeFilters, cl.MatchAllTerms);
        }

        private void lnkClearTaxonomyFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TaxonomyFilters = null;
            //taxonomySelection = null;            
            PopulateSelection(null, _valueSelection, _includeChildren);
        }

        private void lnkClearValueFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearValueFilters();
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);
        }

        private void lnkLimitTaxonomyFromSearchResults_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var taxFilter = new List<TaxonomyInfo>();
            foreach (DataGridViewCell cell in dgvTaxonomyResults.SelectedCells)
            {
                TaxonomyInfo tax = ((SearchResult)cell.OwningRow.DataBoundItem).Taxonomy;
                if (!taxFilter.Contains(tax))
                    taxFilter.Add(tax);
            }

            TaxonomyFilters = taxFilter;
            _includeChildren = false;
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);
        }

        private void lnkLimitTaxonomyFromTreeView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //First toggle the _includeChildren to correctly update the current selection label
            _includeChildren = AryaTools.Instance.Forms.TreeForm.IncludeChildren;
            TaxonomyFilters = AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomies;
            //populateSelection(TaxonomyFilters, valSelection, _includeChildren);
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);
        }

        private void PopulateQueryView(CrossListCriteria cl)
        {
            ClearValueFilters();
            TaxonomyFilters =
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.Where(t => cl.TaxonomyIDFilter.Contains(t.ID)).ToList();
            AryaTools.Instance.Forms.TreeForm.IncludeChildren = cl.IncludeChildren;
            _includeChildren = cl.IncludeChildren;
            ValueFilters = cl.ValueFilters;
            ProcessValueFilter();
            PopulateSelection(TaxonomyFilters, _valueSelection, _includeChildren);
        }

        private void SetEntityField(string fieldName)
        {
            if (!string.IsNullOrEmpty(fieldName))
                ddFieldName.Items.Add(fieldName);
        }

        private void ShowHideLimitTaxonomyFromResults()
        {
            lnkLimitTaxonomyFromResults.Visible = dgvTaxonomyResults.RowCount > 0;
        }

        private void UpdateFilterTypes()
        {
            switch (ddFieldName.Text)
            {
                case "Item Id":
                    ddValueFilterType.Items.Clear();
                    ddValueFilterType.Items.AddRange(_itemIdFilterTypes);
                    ddValueFilterType.SelectedIndex = 0;
                    txtValue.Multiline = true;
                    txtValue.AcceptsReturn = true;
                    txtValue.Height = 35;
                    txtValue.Text = @"(2k items max)";
                    break;

                default:
                    ddValueFilterType.Items.Clear();
                    ddValueFilterType.Items.AddRange(_defaultFilterTypes);
                    ddValueFilterType.SelectedIndex = 0;
                    txtValue.Multiline = false;
                    txtValue.AcceptsReturn = false;
                    txtValue.Text = string.Empty;
                    break;
            }
        }

        public void SetFormType(QueryFormType qft, TaxonomyInfo crossListNode = null)
        {
            _selectedCrossListNodeFromTreeView = crossListNode;

            switch (qft)
            {
                case QueryFormType.QueryView:
                    Text = "Query View";
                    btnCrossList.Visible = false;
                    txtBoxCrossListNode.Visible = false;
                    break;

                case QueryFormType.CrossListDefinition:
                    Text = "Cross List Definition";
                    btnCrossList.Visible = true;
                    txtBoxCrossListNode.Visible = true;
                    txtBoxCrossListNode.Text = _selectedCrossListNodeFromTreeView.ToString();

                    var derivedTaxonomy = _selectedCrossListNodeFromTreeView.DerivedTaxonomies.FirstOrDefault();
                    if (derivedTaxonomy != null)
                    {
                        PopulateQueryView(derivedTaxonomy.Expression.DeSerializeXElement());
                    }
                    //if (
                    //    AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.Any(
                    //        t => t.TaxonomyID == SelectedCrossListNodeFromTreeView.ID))
                    //{
                    //    var cl =
                    //        AryaTools.Instance.InstanceData.Dc.DerivedTaxonomies.Where(
                    //            t => t.TaxonomyID == SelectedCrossListNodeFromTreeView.ID).Select(ex => ex.Expression).
                    //            Single().DeSerializeXElement();
                    //    PopulateQueryView(cl);
                    //}
                    break;

                case QueryFormType.SkuGroup:
                    Text = "Sku Group Definition";
                    btnCrossList.Visible = true;
                    txtBoxCrossListNode.Visible = true;
                    txtBoxCrossListNode.Text = SelectedSkuGroupForQueryView.Criterion.Name.ToString();

                    //var skuGroup = _selectedCrossListNodeFromTreeView.DerivedTaxonomies.FirstOrDefault();
                    if (SelectedSkuGroupForQueryView.Criterion != null)
                    {
                        PopulateQueryView(SelectedSkuGroupForQueryView.Criterion.DeSerializeXElement());
                        SelectedSkuGroupForQueryView = null;
                    }

                    break;
            }
        }

        #endregion Methods

        private void dgvTaxonomyResults_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }
        }

        private void btnGroupSkus_Click(object sender, EventArgs e)
        {
            AddValueFilter();

            //Adding an object to cross list- Include Children is Set false to prevent storing multiple IDs in the object
            var cl = new CrossListCriteria(
                TaxonomyFilters.GetTaxonomyIDs(false), ValueFilters, _attributeTypeFilters, _matchAllTerms,
                _includeChildren);

            var xml = cl.SerializeToXElement();

            FrmCreateSkuGroup CreateSkuGroupForm = new FrmCreateSkuGroup(xml);
            CreateSkuGroupForm.ShowDialog();
        }
    }
}