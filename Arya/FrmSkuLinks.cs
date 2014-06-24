using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LinqKit;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using Attribute = Arya.Data.Attribute;
using Sku = Arya.Data.Sku;
using SkuLink = Arya.Data.SkuLink;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;

namespace Arya
{
    public partial class FrmSkuLinks : Form
    {
        #region Fields (13)

        private string _currentStatus = string.Empty;
        private readonly string[] _dontCareValues = new[] { "Not Visible", "N/A" };
        private List<FilteredSkuLink> _filteredSkuLinks;
        readonly Dictionary<TaxonomyInfo, List<Attribute>> _inSchemaAttributes = new Dictionary<TaxonomyInfo, List<Attribute>>();
        private readonly string[] _notDiscernibles = new[] { "Not Discernable", "Not Discernible" };
        readonly Dictionary<Sku, List<string>> _skuGaps = new Dictionary<Sku, List<string>>();
        private Guid _waitkey = Guid.Empty;
        private Thread _workerThread;
        //const string BrandName = "Brand Name";
        private const string ConsolidatedPartNumberAttribute = "Primary Supplier Part Number";
        const string Maybe = "Maybe";
        const string No = "No";
        const string skuGapsFileName = "Sku Gaps.txt";
        const string Yes = "Yes";

        #endregion Fields

        #region Constructors (1)

        public FrmSkuLinks()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        #endregion Constructors

        #region Properties (4)

        protected bool LeftIncludeChildren { get; set; }

        protected List<TaxonomyInfo> LeftTaxonomyFilters { get; set; }

        protected bool RightIncludeChildren { get; set; }

        protected List<TaxonomyInfo> RightTaxonomyFilters { get; set; }

        #endregion Properties

        #region Methods (19)

        // Private Methods (19) 

        void AddSkuGap(Sku sku, string attributeName)
        {
            if (!_skuGaps.ContainsKey(sku))
                _skuGaps.Add(sku, new List<string>());

            _skuGaps[sku].Add(attributeName);
        }

        private void ApplySkuLinkFilters(bool queryDatabase)
        {
            //Cannot use LinkedFrom and LinkedTo
            //Using Sku and Sku1 (repectively) instead

            var waitkey = FrmWaitScreen.ShowMessage("Applying Filter");

            dgvSkuLinks.DataSource = null;
            dgvSkuLinks.Invalidate();
            _filteredSkuLinks = null;

            lblLeftTaxonomy.Text = @"No Filter Applied";
            lblRightTaxonomy.Text = @"No Filter Applied";

            if ((LeftTaxonomyFilters == null || LeftTaxonomyFilters.Count == 0) &&
                (RightTaxonomyFilters == null || RightTaxonomyFilters.Count == 0))
            {
                FrmWaitScreen.HideMessage(waitkey);
                return;
            }

            var skuLinks = AryaTools.Instance.InstanceData.Dc.SkuLinks.Where(sl => sl.Active);

            if (LeftTaxonomyFilters != null && LeftTaxonomyFilters.Count > 0)
            {
                lblLeftTaxonomy.Text = LeftTaxonomyFilters.Count == 1
                                           ? string.Format("Taxonomy is [{0}]", LeftTaxonomyFilters[0])
                                           : string.Format(
                                               "Taxonomy is one of the {0} selected nodes", LeftTaxonomyFilters.Count);

                List<Guid> taxIds = LeftTaxonomyFilters.Select(tf => tf.ID).ToList();
                if (LeftIncludeChildren)
                {
                    LeftTaxonomyFilters.ForEach(tax => taxIds.AddRange(tax.AllChildren.Select(t => t.ID)));
                    lblLeftTaxonomy.Text += @" and its child nodes";
                }

                skuLinks = from sl in skuLinks
                           //let tax = sl.LinkedFrom.Taxonomy.ID
                           let tax = sl.Sku.SkuInfos.Where(si => si.Active).Select(si => si.TaxonomyID).FirstOrDefault()
                           where taxIds.Contains(tax)
                           select sl;
            }

            if (RightTaxonomyFilters != null && RightTaxonomyFilters.Count > 0)
            {
                lblRightTaxonomy.Text = RightTaxonomyFilters.Count == 1
                                         ? string.Format("Taxonomy is [{0}]", RightTaxonomyFilters[0])
                                         : string.Format(
                                             "Taxonomy is one of the {0} selected nodes", RightTaxonomyFilters.Count);

                List<Guid> taxIds = RightTaxonomyFilters.Select(tf => tf.ID).ToList();
                if (RightIncludeChildren)
                {
                    RightTaxonomyFilters.ForEach(tax => taxIds.AddRange(tax.AllChildren.Select(t => t.ID)));
                    lblRightTaxonomy.Text += @" and its child nodes";
                }

                skuLinks = from sl in skuLinks
                           //let tax = sl.LinkedTo.Taxonomy.ID
                           let tax =
                               sl.Sku1.SkuInfos.Where(si => si.Active).Select(si => si.TaxonomyID).FirstOrDefault()
                           where taxIds.Contains(tax)
                           select sl;
            }

            if (!string.IsNullOrEmpty(txtFilterSkuLinkType.Text))
            {
                skuLinks = from sl in skuLinks
                           where sl.LinkType.Equals(txtFilterSkuLinkType.Text)
                           select sl;
            }

            if (queryDatabase)
            {
                _filteredSkuLinks = (from sl in skuLinks
                                     select
                                         new FilteredSkuLink
                                             {
                                                 LeftSku = sl.Sku,
                                                 LeftTaxonomy = sl.Sku.Taxonomy,
                                                 RightSku = sl.Sku1,
                                                 RightTaxonomy = sl.Sku1.Taxonomy
                                             }).ToList();

                dgvSkuLinks.DataSource = _filteredSkuLinks;
                dgvSkuLinks.Invalidate();
            }

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplySkuLinkFilters(true);
        }

        private void btnFindMatches_Click(object sender, EventArgs e)
        {
            _workerThread = new Thread(FindExactMatches);
            _workerThread.Start();
            tmrMatch.Start();
        }

        private void btnValidateMatched_Click(object sender, EventArgs e)
        {
            _workerThread = new Thread(ValidateMatchedLinks);
            _workerThread.Start();
            tmrMatch.Start();
        }

        private bool CreateSkuLink(Sku leftSku, Sku rightSku, string matchType)
        {
            if (rightSku.ID.Equals(leftSku.ID))
                return false;

            var existingSkuLink =
                AryaTools.Instance.InstanceData.Dc.SkuLinks.Any(
                    sl => sl.Active && sl.Sku.Equals(leftSku) && sl.Sku1.Equals(rightSku));

            if (existingSkuLink)
                return false;

            var newSkuLink = new SkuLink { Sku = leftSku, Sku1 = rightSku, LinkType = txtSkuLinkType.Text + "-" + matchType };
            AryaTools.Instance.InstanceData.Dc.SkuLinks.InsertOnSubmit(newSkuLink);
            return true;
        }

        private void DoMatch(FilteredSkuLink link, out string accuracy, out string reasonCode, out string reasonDetail, out string brandReasonDetail)
        {
            string leftValue, rightValue;

            accuracy = Yes;
            reasonCode = null;
            reasonDetail = null;
            brandReasonDetail = null;

            //First, match on Brand Name
            //bool? isMatch = ValidateMatchedLinks(link, BrandName, out leftValue, out rightValue);
            //if (isMatch == null)
            //{
            //    accuracy = Maybe;
            //}
            //else if (isMatch == false)
            //{
            //    accuracy = No;
            //    //reasonCode = "Brand.";
            //    brandReasonDetail = string.Format("Brand: SKU={0}, Image={1}.", leftValue, rightValue);
            //    //return;
            //}

            //First, match Taxonomy only if the image is in quarantine nodes
            if (link.RightTaxonomy.ToString().ToLower().Contains("quarantine") && !ValidateMatchedLinks(link, out leftValue, out rightValue))
            {
                accuracy = No;
                reasonCode = "Taxonomy doesn't match";
                reasonDetail = string.Format("Taxonomy: SKU={0}, Image={1}.", leftValue, rightValue);
                //return;
            }

            //Second, match on PartNumber
            bool? isMatch = ValidateMatchedLinks(link, ConsolidatedPartNumberAttribute, out leftValue, out rightValue);
            //if (isMatch == null)
            //{
            //    //accuracy = Maybe;
            //    return;
            //}

            if (isMatch == false)
            {
                accuracy = No;
                reasonCode = "Specs don't match";
                reasonDetail = string.Format("{0}: SKU={1}, Image={2}.", ConsolidatedPartNumberAttribute, leftValue, rightValue);
            }

            //Then, match all attributes
            //var schemaTaxonomy = rbUseLeftSchema.Checked ? link.LeftTaxonomy : link.RightTaxonomy;
            //use schema from right taxonomy
            //var schemaTaxonomy = link.RightTaxonomy;
            //var atts = from si in schemaTaxonomy.SchemaInfos
            //           let sd = si.SchemaData
            //           where sd != null && sd.InSchema && !si.Attribute.AttributeName.Equals(BrandName)
            //           select si.Attribute;

            //int reasonCount = 0;
            //foreach (var att in atts)
            //{
            //    isMatch = ValidateMatchedLinks(link, att.AttributeName, out leftValue, out rightValue);
            //    switch (isMatch)
            //    {
            //        case null:
            //            if (accuracy.Equals(Yes))
            //                accuracy = Maybe;
            //            break;
            //        case false:
            //            accuracy = No;
            //            reasonCode = "Specs don't match";
            //            reasonDetail += string.Format("{0}: SKU={1}, Image={2}.", att, leftValue, rightValue);
            //            reasonCount++;
            //            break;
            //    }
            //    if (reasonCount == 3)
            //        break;
            //}

            UpdateLinkStatus(link, accuracy, reasonCode, reasonDetail);
        }

        private void FindExactMatches()
        {
            if (//LeftTaxonomyFilters == null || LeftTaxonomyFilters.Count <= 0 ||
                RightTaxonomyFilters == null || RightTaxonomyFilters.Count <= 0)
            {
                _currentStatus = "Please select filter for Right (Image) Taxonomy";
                return;
            }

            _currentStatus = "Fetching SKUs";

            //var leftTaxIds = LeftTaxonomyFilters.Select(tf => tf.ID).ToList();
            //if (LeftIncludeChildren)
            //    LeftTaxonomyFilters.ForEach(tax => leftTaxIds.AddRange(tax.AllChildren.Select(t => t.ID)));



            var rightTaxIds = RightTaxonomyFilters.Select(tf => tf.ID).ToList();
            if (RightIncludeChildren)
                RightTaxonomyFilters.ForEach(tax => rightTaxIds.AddRange(tax.AllChildren.Select(t => t.ID)));

            var skus = (from sku in AryaTools.Instance.InstanceData.Dc.Skus
                        where sku.SkuInfos.Any(si => si.Active && rightTaxIds.Contains(si.TaxonomyID))
                        select sku).ToList();

            _currentStatus = string.Format("Deleting Existing Suggested Image Links ... ");

            //delete all the existing links for these skus
            //11/7/2011 - delete only the suggested sku links.
            skus.SelectMany(p => p.SkuLinks1).Where(p => p.Active && p.LinkType.Equals(txtFilterSkuLinkType.Text)).ForEach(act =>
            {
                act.DeletedBy =
                    AryaTools.Instance.InstanceData.CurrentUser.
                        ID;
                act.DeletedOn = DateTime.Now;
                act.Active = false;
            });

            AryaTools.Instance.SaveChangesIfNecessary(true, false);

            int skuCount = 0;
            int skusWithNewLinks = 0, totalNewLinks = 0;
            foreach (var sku in skus)
            {
                _currentStatus = string.Format("Processing SKUs ({0} of {1})", ++skuCount, skus.Count);

                int newLinks = FindExactMatches(sku);

                if (newLinks > 0)
                {
                    skusWithNewLinks++;
                    totalNewLinks += newLinks;
                }
            }

            AryaTools.Instance.SaveChangesIfNecessary(false, false);

            //Summary
            _currentStatus = string.Format(
                "Summary:|No. of SKUs with new links: {0}|Total no. of new links created:{1}", skusWithNewLinks,
                totalNewLinks);
        }

        private int FindExactMatches(Sku sourceSku)
        {
            IQueryable<Sku> skuFilter = from sku in AryaTools.Instance.InstanceData.Dc.Skus
                                        //where sku.SkuInfos.Any(si => si.Active && leftTaxIds.Contains(si.TaxonomyID))
                                        select sku;

            var schemaAttributes = GetInSchemaAttributes(sourceSku.Taxonomy);
            if (schemaAttributes.Count == 0)
                return 0;

            //Must match Brand
            //var brandAttribute = Attribute.GetAttributeFromName(BrandName, true);
            //if (!schemaAttributes.Contains(brandAttribute))
            //    schemaAttributes.Add(brandAttribute);

            bool hasValues = false;
            string matchType = "Exact";
            foreach (var att in schemaAttributes)
            {
                var attribute = att;
                var values = sourceSku.GetValuesForAttribute(attribute.AttributeName, true).Select(ed => ed.Value).ToList();
                if (values.Count == 0 || values.Any(val => _dontCareValues.Contains(val)))
                    continue;

                if (values.Any(val => _notDiscernibles.Contains(val)))
                {
                    matchType = "Potential";
                    continue;
                }

                hasValues = true;

                skuFilter =
                    skuFilter.Where(
                        sku =>
                        sku.EntityInfos.Any(
                            ei =>
                            ei.EntityDatas.Any(
                                ed => ed.Active && ed.AttributeID.Equals(attribute.ID) && values.Contains(ed.Value))));
            }

            if (!hasValues)
                return 0;

            var targetSkus = skuFilter.Distinct().ToList();

            int newLinkCount = 0;
            foreach (var targetSku in targetSkus)
            {
                if (targetSku.Taxonomy.ToString().Contains(">Images>"))
                    continue;

                if (CreateSkuLink(targetSku, sourceSku, matchType))
                    newLinkCount++;
            }

            return newLinkCount;
        }

        private List<Attribute> GetInSchemaAttributes(TaxonomyInfo taxonomy)
        {
            if (_inSchemaAttributes.ContainsKey(taxonomy))
                return _inSchemaAttributes[taxonomy];

            var atts = (from si in taxonomy.SchemaInfos
                        where si.SchemaDatas.Any(sd => sd.Active && sd.InSchema)
                        select si.Attribute).ToList();
            _inSchemaAttributes.Add(taxonomy, atts);

            return atts;
        }

        private void lnkClearLeftTaxonomy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LeftTaxonomyFilters = null;
            ApplySkuLinkFilters(false);
        }

        private void lnkClearRightTaxonomy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RightTaxonomyFilters = null;
            ApplySkuLinkFilters(false);
        }

        private void lnkGetLeftTaxonomy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LeftIncludeChildren = AryaTools.Instance.Forms.TreeForm.IncludeChildren;
            LeftTaxonomyFilters = AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomies;
            ApplySkuLinkFilters(false);
        }

        private void lnkGetRightTaxonomy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RightIncludeChildren = AryaTools.Instance.Forms.TreeForm.IncludeChildren;
            RightTaxonomyFilters = AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomies;
            ApplySkuLinkFilters(false);
        }

        private void tmrMatch_Tick(object sender, EventArgs e)
        {
            if (_workerThread == null || !_workerThread.IsAlive)
            {
                tmrMatch.Stop();
                MessageBox.Show(_currentStatus.Replace('|', '\n'), "Done!");
                _currentStatus = string.Empty;

                var fi = new FileInfo(skuGapsFileName);
                if (fi.Exists)
                    AryaTools.Instance.Forms.BrowserForm.GotoUrl(fi.FullName);
            }

            if (string.IsNullOrEmpty(_currentStatus))
            {
                if (_waitkey != Guid.Empty)
                {
                    FrmWaitScreen.HideMessage(_waitkey);
                    _waitkey = Guid.Empty;
                }
            }
            else
            {
                if (_waitkey == Guid.Empty)
                    _waitkey = FrmWaitScreen.ShowMessage(_currentStatus);
                else
                {
                    FrmWaitScreen.UpdateMessage(_waitkey, _currentStatus);
                }
            }
        }

        private void UpdateLinkStatus(FilteredSkuLink link, string accuracy, string reasonCode, string reasonDetail, string brandReasonDetail = null)
        {
            //Sku sku = rbResultsInLeftSku.Checked ? link.LeftSku : link.RightSku;
            //results in left Sku
            Sku sku = link.LeftSku;
            var leftLowerTaxonomy = sku.Taxonomy.ToString().ToLower();

            if (!string.IsNullOrEmpty(reasonCode) && !string.IsNullOrEmpty(reasonDetail))
            {

                var rightLowerTaxonomy = link.RightTaxonomy.ToString().ToLower();

                if (leftLowerTaxonomy.Contains(">Representational>".ToLower()))
                {
                    if (rightLowerTaxonomy.Contains(">Line Drawing".ToLower()))
                    {
                        reasonDetail = "Line Drawing";
                        reasonCode = "Does not fit Image Guidelines";
                    }
                    else if (rightLowerTaxonomy.Contains(">Group Shot".ToLower()))
                    {
                        reasonDetail = "Group Shot";
                        reasonCode = "Does not fit Image Guidelines";
                    }
                }
                else if (leftLowerTaxonomy.Contains(">One-One>".ToLower()))
                {
                    if (rightLowerTaxonomy.Contains(">Line Drawing".ToLower()))
                    {
                        reasonDetail = "Line Drawing";
                        reasonCode = "Does not fit Image Guidelines";
                    }
                }
            }

            var status = accuracy == Yes || accuracy == No ? "Completed" : "Sent to MSC for data insufficiency";

            //if (leftLowerTaxonomy.Contains(">One-One>".ToLower()) && (reasonCode == null || !reasonCode.Equals("Does not fit Image Guidelines", StringComparison.OrdinalIgnoreCase)))
            //    return;

            sku.UpsertValue(txtResultPrefix.Text + "Accurate?", accuracy);
            sku.UpsertValue(txtResultPrefix.Text + "Reason Code", reasonCode);
            sku.UpsertValue(txtResultPrefix.Text + "Reason Detail", reasonDetail);
            //sku.UpsertValue(txtResultPrefix.Text + "Brand Reason Detail", brandReasonDetail);
            sku.UpsertValue(txtResultPrefix.Text + "Status", status);

        }

        private void ValidateMatchedLinks()
        {
            var globalAttributesInSkuGapFile = new[]
												   {
													   "AS400Description", "Detail Description", "Brand Name",
													   "Manufacturer's Part Number"
												   };
            if (_filteredSkuLinks == null)
            {
                _currentStatus = "Nothing to validate\nPlease Apply your Filter first.";
                return;
            }

            _currentStatus = "Validating Links";
            var accuracies = new List<string>(_filteredSkuLinks.Count);

            for (int i = 0; i < _filteredSkuLinks.Count; i++)
            {
                var link = _filteredSkuLinks[i];
                string accuracy, reasonCode, reasonDetail, brandReasonDetail;

                DoMatch(link, out accuracy, out reasonCode, out reasonDetail, out brandReasonDetail);
                UpdateLinkStatus(link, accuracy, reasonCode, reasonDetail, brandReasonDetail);
                accuracies.Add(accuracy);

                _currentStatus = string.Format("Validating Links ({0} of {1})", i + 1, _filteredSkuLinks.Count);
            }

            using (TextWriter skuGapsFile = new StreamWriter(skuGapsFileName, false))
            {
                string line = globalAttributesInSkuGapFile.Aggregate(
                    "Item Id", (result, current) => string.Format("{0}\t{1}\t", result, current));
                skuGapsFile.WriteLine(line);
                //fetch the skuGaps from only SkuNode.
                foreach (var g in _skuGaps.Where(p => p.Key.Taxonomy.ToString().Contains(">SKUs>")))
                {
                    var gap = g;
                    line = globalAttributesInSkuGapFile.Aggregate(
                        gap.Key.ItemID, (result, current) =>
                                        {
                                            var values = gap.Key.GetValuesForAttribute(current, true);
                                            var firstValue = values.Count > 0 ? values.First().Value : string.Empty;
                                            return string.Format("{0}\t{1}\t", result, firstValue);
                                        });

                    line = gap.Value.Aggregate(line, (result, current) => string.Format("{0}\t{1}\t", result, current));
                    skuGapsFile.WriteLine(line);
                }
            }

            AryaTools.Instance.SaveChangesIfNecessary(true, false);

            List<Guid> taxIds = LeftTaxonomyFilters.Select(tf => tf.ID).ToList();

            if (LeftIncludeChildren)
            {
                LeftTaxonomyFilters.ForEach(tax => taxIds.AddRange(tax.AllChildren.Select(t => t.ID)));
            }

            var allSkus = AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.Where(ti => taxIds.Contains(ti.ID)).SelectMany(left => left.SkuInfos.Where(si => si.Active).Select(si => si.Sku)).Distinct().ToList();

            //var allSkus =
            //    LeftTaxonomyFilters.SelectMany(left => left.SkuInfos.Where(si => si.Active).Select(si => si.Sku)).Distinct().ToList();

            _currentStatus =
                "Updating Accuracy,ReasonCode & Detail for Skus with NoImage or ImageID:NOIMAGE-11,TotalSkus = " + allSkus.Count;

            foreach (var sku in from sku in allSkus
                                let values = sku.GetValuesForAttribute("Image")
                                let imageValue = values.Count == 0 ? null : values.Distinct().First().Value
                                where string.IsNullOrEmpty(imageValue) || imageValue == "NOIMAGE-11" || imageValue == "N/A"
                                select sku)
            {
                
                sku.UpsertValue(txtResultPrefix.Text + "Accurate?", "No");
                //sku.UpsertValue(txtResultPrefix.Text + "Reason Code", "Does not fit Image Guidelines");
                sku.UpsertValue(txtResultPrefix.Text + "Reason Detail", string.Empty);
                sku.UpsertValue(txtResultPrefix.Text + "Reason Code", "No Image");
                sku.UpsertValue(txtResultPrefix.Text + "Status", "Completed");
            }

            _currentStatus = "Finished Updating.";

            AryaTools.Instance.SaveChangesIfNecessary(false, false);

            //Summary
            var summary = accuracies.GroupBy(v => v).Select(grp => new { Accuracy = grp.Key, Count = grp.Count() });
            _currentStatus = summary.Aggregate(
                "Summary:", (current, detail) => current + "|" + detail.Accuracy + ": " + detail.Count);
        }

        /// <summary>
        /// Match Taxonomy for the specified Link
        /// </summary>
        private static bool ValidateMatchedLinks(FilteredSkuLink link, out string leftTaxonomy, out string rightTaxonomy)
        {
            //Batch>One-One|Rep.>Images|Skus
            const int numberOfTopTaxonomyLevelsToSkip = 3;
            leftTaxonomy = TaxonomyInfo.StripTopLevelNodes(link.LeftTaxonomy.ToString(), numberOfTopTaxonomyLevelsToSkip);
            rightTaxonomy = TaxonomyInfo.StripTopLevelNodes(link.RightTaxonomy.ToString(), numberOfTopTaxonomyLevelsToSkip);

            return leftTaxonomy.Equals(rightTaxonomy);
        }

        /// <summary>
        /// Match Attribute Values for the specified Link
        /// </summary>
        private bool? ValidateMatchedLinks(FilteredSkuLink link, string attributeName, out string leftValue, out string rightValue)
        {
            var leftValues = link.LeftSku.GetValuesForAttribute(attributeName, true).Select(ed => ed.Value).ToList();
            var rightValues = link.RightSku.GetValuesForAttribute(attributeName, true).Select(ed => ed.Value).ToList();

            leftValue = leftValues.Aggregate(
                string.Empty, (current, value) => string.IsNullOrEmpty(current) ? value : current + ", " + value);
            rightValue = rightValues.Aggregate(
                string.Empty, (current, value) => string.IsNullOrEmpty(current) ? value : current + ", " + value);

            if (leftValues.Count == 0)
            {
                AddSkuGap(link.LeftSku, attributeName);
                return null;
            }

            if (rightValues.Count == 0)
            {
                var leftSchemaAttributes =
                    link.LeftTaxonomy.SchemaInfos.Where(si => si.SchemaData != null).Select(si => si.Attribute).ToList();

                var leftDeriviedAttribute =
                    leftSchemaAttributes.Where(
                        a => (a.AttributeName == attributeName + " (Ratio)") && a.AttributeType.Contains(AttributeTypeEnum.Global.ToString())).SelectMany(
                            p => p.DerivedAttributes).FirstOrDefault(p => p.TaxonomyID == link.LeftTaxonomy.ID);

                if (leftDeriviedAttribute != null)
                {
                    var ta = new TaxAttValueCalculator(link.LeftSku);
                    ta.ProcessCalculatedAttribute(leftDeriviedAttribute.Expression, leftDeriviedAttribute.MaxResultLength);
                    var attributes = ta.GetAttributesInTokens();
                    foreach (var attribute in
                        attributes.Where(attribute => link.LeftSku.GetValuesForAttribute(attribute).Count == 0))
                    {
                        AddSkuGap(link.LeftSku, attribute);
                    }
                    return null;
                }

                AddSkuGap(link.RightSku, attributeName);
                return null;
            }

            if (leftValues.Any(lv => _notDiscernibles.Union(_dontCareValues).Contains(lv)) ||
                rightValues.Any(rv => _notDiscernibles.Union(_dontCareValues).Contains(rv)))
                return true;

            return leftValues.Any(lv => rightValues.Any(rv => rv.Equals(lv)));
        }

        #endregion Methods

        #region Nested Classes (1)


        class FilteredSkuLink
        {
            #region Properties (4)

            public Sku LeftSku { get; set; }

            public TaxonomyInfo LeftTaxonomy { get; set; }

            public Sku RightSku { get; set; }

            public TaxonomyInfo RightTaxonomy { get; set; }

            #endregion Properties
        }
        #endregion Nested Classes

        private void dgvSkuLinks_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dgvSkuLinks.Rows.Cast<DataGridViewRow>().ForEach(row => row.HeaderCell.Value = (row.Index + 1).ToString());
            dgvSkuLinks.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
    }
}