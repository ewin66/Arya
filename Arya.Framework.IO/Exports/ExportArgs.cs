using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.IO.Exports
{
    [Serializable]
    public class AdvancedExportArgs : ExportArgs
    {
        #region Fields

        private bool _includeEnrichments;
        private bool _includeSkuValues;
        private bool _includeSkus;

        #endregion Fields

        #region Properties

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 18)]
        [DisplayName(@"Export Product Attributes")]
        [Description("When True, Extended Attributes will be included in the export.")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportExtendedAttributes { get; set; }

        [Category(CaptionOptional)]
        [Description(
           "A list of attribute names that should be exported along side each unique ItemID in the file. Place one attribute name per line or delimit names with a pipe character. It is recommended the fixed attributes be single value, Global attributes that help describe the item. (e.g. In a review file, the fixed attributes are output in fixed position columns at the start of each row.)" 
            )]
        [PropertyOrder(OptionalBaseOrder + 3)]
        [DisplayName(@"Fixed Item Attribute List")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] GlobalAttributes { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 19)]
        [DisplayName(@"Mark As Published")]
        [Description(
            "If YES, the data that is exported will be flagged as being published, or sent to the client, so that future exports may use the information to alter or limit the export output. [Future enhancement. Leave as YES.]"
            )]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool MarkAsPublished { get; set; }

        [Category(CaptionOptional)]
        [Description(
            "A delimited list of attribute expressions in the form of &lt;attributeName&gt;[=&lt;value&gt;] that remove SKUs from the export. SKUs that match all filter criteria will be eliminated from the export. The Exclude SKU Filter is applied after the Include SKU Filters. Multiple filters may be listed. Place one expression per line or delimit expressions with a pipe character. All filters are connected by a logical AND."
            )]
        [PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Exclude SKU Filter")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuExclusions { get; set; }

        [Category(CaptionOptional)]
        [Description(
            "A delimited list of attribute expressions in the form of &lt;attributeName&gt;[=&lt;value&gt;] that filter which SKUs are exported. Only the SKUs that match all filter criteria will be included in the export. Multiple filters may be listed. Place one expression per line or delimit expressions with a pipe character. All filters are connected by a logical AND. For example, listing an attribute name with no value, such as Color, would export all SKUs that have any value for Color.  Using an attribute with value expression, as in Color=Blue, would export any SKUs where the value for Color is Blue."
            )]
        [PropertyOrder(OptionalBaseOrder + 4)]
        [DisplayName(@"Include SKU Filter")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuInclusions { get; set; }

        #endregion Properties

        #region Methods

        public IEnumerable<Sku> GetFilteredSkuList(IEnumerable<Sku> skus)
        {
            var skuInclusions = ParseSkuInclusionAndExclusions(SkuInclusions);
            foreach (var filter in skuInclusions)
            {
                var attribute = filter.Key;

                skus = filter.Value.Count == 0
                    ? skus.Where(sku => sku.HasAttribute(attribute))
                    : filter.Value.Aggregate(skus,
                        (current, value) => current.Where(sku => sku.HasValue(attribute, value)));
            }

            var skuExclusions = ParseSkuInclusionAndExclusions(SkuExclusions);
            foreach (var exclusion in skuExclusions)
            {
                var attribute = exclusion.Key;

                skus = exclusion.Value.Count == 0
                    ? skus.Where(sku => !sku.HasAttribute(attribute))
                    : exclusion.Value.Aggregate(skus,
                        (current, value) => current.Where(sku => !sku.HasValue(attribute, value)));
            }

            if (ItemIds != null && ItemIds.Length > 0)
                skus = skus.Where(sku => ItemIds.Contains(sku.ItemID));
            return skus;
        }

        protected static Dictionary<string, List<string>> ParseSkuInclusionAndExclusions(
            IEnumerable<string> attributeList)
        {
            var filter = new Dictionary<string, List<string>>();

            if (attributeList == null)
                return filter;

            attributeList = attributeList.Where(p => !String.IsNullOrWhiteSpace(p));

            foreach (var attVal in attributeList)
            {
                var attributeValue = attVal.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                if (!attributeValue.Any())
                    continue;

                var lowerAttributeName = attributeValue[0].ToLower().Trim();
                if (!filter.ContainsKey(lowerAttributeName))
                    filter.Add(lowerAttributeName, new List<string>());

                if (attributeValue.Count() == 1)
                    continue;

                var attValues = filter[lowerAttributeName];
                attValues.Add(attributeValue[1].Trim());
            }
            return filter;
        }

        #endregion Methods
    }

    [Serializable]
    public class ExportArgs : WorkerArguments
    {
        #region Fields

        private readonly Regex _rxFileNameBadCharacters = new Regex("[^a-zA-Z0-9_-]");

        private string _baseFilename;
        private ExportWorkerBase.SourceType _exportSourceType;
        private string[] _itemIds;
        private Guid[] _taxonomyIds;

        #endregion Fields

        #region Constructors

        public ExportArgs() { PortalUrl = "Export.aspx"; }

        #endregion Constructors

        #region Properties


        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 17)]
        [DisplayName(@"Export Cross-listed Nodes")]
        [Description(
            "If YES, any cross-listed nodes in the given taxonomy nodes will also be included in the export. (e.g. a schema for a cross-listed node). If NO, cross-listed nodes will not be part of the export."
            )]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportCrossListNodes { get; set; }

        [Category(CaptionRequired)]
        [Description(
            "The base file name for the exported file. The final file name will be the base name + the export file type (e.g. _Data or _Attribute) + an extension consistent with the Export File Type. (e.g. .txt for Text, .xls for Excel)."
            )]
        [DisplayName(@"Export File Name")]
        [PropertyOrder(RequiredBaseOrder + 5)]
        [Browsable(true)]
        public string BaseFilename
        {
            get { return _baseFilename; }
            set { _baseFilename = _rxFileNameBadCharacters.Replace(value, string.Empty); }
        }

        [Description(
            "The file format the export should produce. TXT is delimited UTF-8 text file, XLS is an MS Excel format, XML is a tagged format."
            )]
        [DisplayName(@"Export File Type")]
        [Category(CaptionRequired)]
        [DefaultValue(ExportWorkerBase.SaveFileType.Text)]
        [PropertyOrder(RequiredBaseOrder + 6)]
        [TypeConverter(typeof (CustomEnumConverter))]
        public ExportWorkerBase.SaveFileType ExportFileType { get; set; }

        [Description(
            "The method used to specify which SKUs to export. If TAXONOMY, the SKUs are selected by listing the taxonomy nodes in which they are classified. If SKULIST, the SKUs are selected by listing the Item IDs."
            )]
        [DisplayName(@"Source Type")]
        [Category(CaptionRequired)]
        [DefaultValue(ExportWorkerBase.SourceType.Taxonomy)]
        [PropertyOrder(RequiredBaseOrder + 9)]
        [TypeConverter(typeof (CustomEnumConverter))]
        [Browsable(false)]
        public ExportWorkerBase.SourceType ExportSourceType
        {
            get { return _exportSourceType; }
            set
            {
                _exportSourceType = value;

                //SetPropertyAttribute(GetType(), "TaxonomyPaths", "browsable",
                //    _exportSourceType == ExportWorkerBase.SourceType.Taxonomy);
                //SetPropertyAttribute(GetType(), "ItemIds", "browsable",
                //    _exportSourceType == ExportWorkerBase.SourceType.SkuList);

                //OnPropertyChanged(new PropertyChangedEventArgs(MethodBase.GetCurrentMethod().Name));
            }
        }

        [Description(
            "The character used to separate each data value in the exported file. This option applies only for Text files."
            )]
        [DisplayName(@"Field Delimiter")]
        [Category(CaptionRequired)]
        [DefaultValue(Delimiter.Tab)]
        [PropertyOrder(RequiredBaseOrder + 7)]
        [TypeConverter(typeof (CustomEnumConverter))]
        public Delimiter FieldDelimiter { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 1)]
        [DefaultValue(true)]
        [DisplayName(@"Ignore Taxonomy T1")]
        [Description(
            "If YES, eliminates the level one taxonomy node name from the output of taxonomy paths. If NO, the level one taxonomy will be part of the taxonomy paths."
            )]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IgnoreT1Taxonomy { get; set; }

        [Browsable(false)]
        [Category(CaptionRequired)]
        [PropertyOrder(RequiredBaseOrder + 11)]
        public string[] ItemIds
        {
            get
            {
                //                if (ExportSourceType!=ExportWorkerBase.SourceType.SkuList)
                //                    return new string[0];

                if (_itemIds != null && _itemIds.Length > 0)
                    return _itemIds;

                if (_taxonomyIds == null || _taxonomyIds.Length == 0)
                    return new string[0];
                try
                {
                    using (var dc = new AryaDbDataContext(ProjectId, UserId))
                    {
                        var parts = TaxonomyIds.Split(2000);
                        var taxNodes = new List<TaxonomyInfo>();

                        foreach (var taxIds in parts)
                        {
                            var nodes =
                                (from tax in dc.TaxonomyInfos 
                                 where taxIds.Contains(tax.ID) 
                                 select tax).ToList();
                            taxNodes.AddRange(nodes);
                        }

                        var selectedNodes = taxNodes.SelectMany(p => p.AllChildren).Union(taxNodes).Distinct().ToList();

                        _itemIds = (from ti in selectedNodes
                            from sku in ti.GetSkus(ExportCrossListNodes)
                            where sku.SkuType == Sku.ItemType.Product.ToString()
                            select sku.ItemID).Distinct().ToArray();
                        //_itemIds =
                        //    (from si in dc.SkuInfos
                        //     where selectedNodes.Contains(si.TaxonomyInfo)
                        //         && si.Active
                        //         && si.Sku.SkuType == Sku.ItemType.Product.ToString()
                        //     select si.Sku.ItemID).ToArray();
                        return _itemIds;
                    }
                }
                catch (Exception)
                {
                    return new string[0];
                }
            }
            set
            {
                if ((_itemIds == null || _itemIds.Length == 0) && (value == null || value.Length == 0))
                    return;
                _itemIds = value;
                ExportSourceType = ExportWorkerBase.SourceType.SkuList;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Category(CaptionRequired)]
        [PropertyOrder(RequiredBaseOrder + 8)]
        public bool SourceSelected
        {
            get
            {
                return (_itemIds != null && _itemIds.Length > 0) || (_taxonomyIds != null && _taxonomyIds.Length > 0);
            }
        }

        [Browsable(false)]
        public Guid[] TaxonomyIds
        {
            get
            {
                //if (ExportSourceType != ExportWorkerBase.SourceType.Taxonomy)
                //    return new Guid[0];

                if (_taxonomyIds != null && _taxonomyIds.Length > 0)
                    return _taxonomyIds;
                if (_itemIds == null || _itemIds.Length == 0)
                    return new Guid[0];
                try
                {
                    using (var dc = new AryaDbDataContext(ProjectId, UserId))
                    {
                        var taxIds = new List<Guid>();

                        var parts = _itemIds.Split(2000);
                        foreach (var items in parts)
                        {
                            var tIds =
                                (from si in dc.SkuInfos
                                    where items.Contains(si.Sku.ItemID) && si.Active
                                    select si.TaxonomyID).Distinct().ToList();
                            taxIds.AddRange(tIds);
                        }

                        _taxonomyIds = taxIds.Distinct().ToArray();
                        return _taxonomyIds;
                    }
                }
                catch (Exception)
                {
                    return new Guid[0];
                }
            }
            set
            {
                if ((_taxonomyIds == null || _taxonomyIds.Length == 0) && (value == null || value.Length == 0))
                    return;
                _taxonomyIds = value;
                ExportSourceType = ExportWorkerBase.SourceType.Taxonomy;
            }
        }

        [Browsable(false)]
        [Category(CaptionRequired)]
        [PropertyOrder(RequiredBaseOrder + 10)]
        [XmlIgnore]
        public string[] TaxonomyPaths
        {
            get
            {
                if (_taxonomyIds == null || _taxonomyIds.Length == 0)
                    return new string[0];

                using (var db = new AryaDbDataContext(ProjectId, UserId))
                {
                    var query = "select V.TaxonomyPath from V_Taxonomy V where V.TaxonomyId in (";
                    for (var i = 0; i < _taxonomyIds.Length; i++)
                    {
                        if (i > 0)
                            query += ", ";
                        query += string.Format("{{{0}}}", i);
                    }
                    query += ")";

                    return db.ExecuteQuery<string>(query, _taxonomyIds.Cast<object>().ToArray()).ToArray();
                }
            }
            set
            {
                if (value == null)
                    return;

                using (var db = new AryaDbDataContext(ProjectId, UserId))
                {
                    var parts = value.Split(2000);
                    var taxIds = new List<Guid>();

                    foreach (var taxPaths in parts)
                    {
                        var tps = taxPaths.ToList();
                        var query =
                            "select T.ID from V_Taxonomy V join TaxonomyInfo T on V.TaxonomyId = T.ID where TaxonomyPath in (";

                        for (var i = 0; i < tps.Count(); i++)
                        {
                            if (i > 0)
                                query += ", ";
                            query += string.Format("{{{0}}}", i);
                        }
                        query += ")";

                        taxIds.AddRange(db.ExecuteQuery<Guid>(query, tps.Cast<object>().ToArray()));
                    }

                    TaxonomyIds = taxIds.Distinct().ToArray();
                }
            }
        }

        #endregion Properties

        #region Methods

        public bool ShouldSerializeItemIds() { return ExportSourceType == ExportWorkerBase.SourceType.SkuList; }

        public bool ShouldSerializeTaxonomyIds() { return ExportSourceType == ExportWorkerBase.SourceType.Taxonomy; }

        #endregion Methods
    }
}