using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Attribute = Arya.Data.Attribute;
using ExtendedTaxonomyInfo = Arya.Framework4.ComponentModel.ExtendedTaxonomyInfo;
using Arya.Framework.Common.Extensions;

namespace Arya.Framework4.IO.Exports
{
    [Serializable]
    public class ExportWorkerForReviewFiles : ExportWorker
    {
        private readonly Regex _rxParanthesis = new Regex("\\(.*", RegexOptions.Compiled);
        private TextWriter _attributeDataFile;
        private string[] _attributeGroupExclusions = new string[0];
        private string[] _attributeGroupInclusions = new string[0];
        private Dictionary<string, string> _baseAttributeNames;
        private TextWriter _blankValuesAttributeDataFile;
        private SkuDataDbDataContext _currentDb;
        private string _delimiter;
        private string[] _globalAttributeHeaders;
        private string[] _globalAttributeNames;
        private string[] _globalAttributes = new string[0];
        private Dictionary<string, List<string>> _parsedSkuExclusions;
        private Dictionary<string, List<string>> _parsedSkuInclusions;
        private string _projectField1Name;
        private string[] _skuExclusions = new string[0];
        private string[] _skuInclusions = new string[0];
        private Dictionary<Guid, List<Tuple<Attribute, decimal, decimal>>> _taxonomyAttributesCache;
        private TextWriter _taxonomyFile;

        public ExportWorkerForReviewFiles(string argumentDirectoryPath, PropertyGrid ownerPropertyGrid)
            : base(argumentDirectoryPath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            ExportNavigationAttributes = true;
            ExportDisplayAttributes = true;
            ExportRanks = true;
            OrderAttributesBy = SortOrder.OrderbyNavigationDisplay;
            AllowMultipleTaxonomySelection = true;
            IgnoreT1Taxonomy = false;
            IncludeUoM = true;
        }

        public ExportWorkerForReviewFiles(string argumentDirectoryPath, SerializationInfo info, StreamingContext ctxt)
            : base(argumentDirectoryPath, info, ctxt)
        {
            IgnoreT1Taxonomy = (bool) info.GetValue("IgnoreT1Taxonomy", typeof (bool));
            GenerateFileWithBlankValues = (bool) info.GetValue("GenerateFileWithBlankValues", typeof (bool));
            GlobalAttributes = (string[]) info.GetValue("GlobalAttributes", typeof (string[]));
            SkuInclusions = (string[]) info.GetValue("SkuInclusions", typeof (string[]));
            SkuExclusions = (string[]) info.GetValue("SkuExclusions", typeof (string[]));
            //SkuValueInclusions = (string[])info.GetValue("SkuValueInclusions", typeof(string[]));
            ExportNavigationAttributes = (bool) info.GetValue("ExportNavigationAttributes", typeof (bool));
            ExportDisplayAttributes = (bool) info.GetValue("ExportDisplayAttributes", typeof (bool));
            ExportUnRankedAttributes = (bool) info.GetValue("ExportUnRankedAttributes", typeof (bool));
            ExportRanks = (bool) info.GetValue("ExportRanks", typeof (bool));
            ExportSuperSchema = (bool) info.GetValue("ExportSuperSchema", typeof (bool));
            ExportNewValueColumn = (bool) info.GetValue("ExportNewValueColumn", typeof (bool));
            Top6OnlyAttributes = (bool) info.GetValue("Top6OnlyAttributes", typeof (bool));
            OrderAttributesBy = (SortOrder) info.GetValue("OrderAttributesBy", typeof (SortOrder));
            AttributeGroupExclusions = (string[]) info.GetValue("AttributeGroupExclusions", typeof (string[]));
            AttributeGroupInclusions = (string[]) info.GetValue("AttributeGroupInclusions", typeof (string[]));
            ExportCrossListNodes = (bool) info.GetValue("ExportCrossListNodes", typeof (bool));
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 1)]
        [DefaultValue(false)]
        [DisplayName(@"Ignore Taxonomy T1"), Description("Eliminates the top level node name in the export file")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IgnoreT1Taxonomy { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 2)]
        [DefaultValue(false)]
        [DisplayName(@"Separate File with Blank Values"),
         Description("Generate a separate file containing only blank values")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool GenerateFileWithBlankValues { get; set; }

        [Category(CaptionOptional), Description("Global Attributes included in the Review file"),
         PropertyOrder(OptionalBaseOrder + 3)]
        [DisplayName(@"Global Attributes")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] GlobalAttributes
        {
            get { return _globalAttributes; }
            set { _globalAttributes = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 4)]
        [DisplayName(@"Consider SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuInclusions
        {
            get { return _skuInclusions; }
            set { _skuInclusions = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Ignore SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuExclusions
        {
            get { return _skuExclusions; }
            set { _skuExclusions = value; }
        }

        //private string[] skuValueInclusions = new string[0];
        //[Category(CaptionOptional), Description("<field>=<value>"), PropertyOrder(OPTIONAL_BASE_ORDER + 4)]
        //[DisplayName(@"Filter Values")]
        //[TypeConverter(typeof(StringArrayConverter))]
        //public string[] SkuValueInclusions
        //{
        //    get { return skuValueInclusions; }
        //    set { skuValueInclusions = value; }
        //}

        //private Dictionary<string, string> parsedSkuValueInclusions;

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 6)]
        [DefaultValue(true)]
        [DisplayName(@"Navigation Attributes"), Description("Export Navigation Attributes")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportNavigationAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 7)]
        [DefaultValue(true)]
        [DisplayName(@"Display Attributes"), Description("Export Display Attributes")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportDisplayAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 8)]
        [DefaultValue(false)]
        [DisplayName(@"Unranked Attributes"), Description("Export Unranked Attributes")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportUnRankedAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 9)]
        [DefaultValue(true)]
        [DisplayName(@"Show Ranks"), Description("Export Ranks")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportRanks { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 10)]
        [DefaultValue(false)]
        [DisplayName(@"Super Schema"), Description("Export Super Schema")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportSuperSchema { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 11)]
        [DefaultValue(false)]
        [DisplayName(@"New Value Column"), Description("Export New Value Column")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportNewValueColumn { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 12)]
        [DefaultValue(false)]
        [DisplayName(@"Top 6 Only Attributes"), Description("Export Top 6 Only Attributes")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool Top6OnlyAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 13)]
        [DefaultValue(SortOrder.OrderbyNavigationDisplay)]
        [TypeConverter(typeof (CustomEnumConverter))]
        public SortOrder OrderAttributesBy { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 14)]
        [DisplayName(@"Group Exclusions"), Description("Attribute Groups to be excluded")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] AttributeGroupExclusions
        {
            get { return _attributeGroupExclusions; }
            set { _attributeGroupExclusions = value; }
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 15)]
        [DisplayName(@"Group Inclusions"), Description("Attribute Groups to be excluded")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] AttributeGroupInclusions
        {
            get { return _attributeGroupInclusions; }
            set { _attributeGroupInclusions = value; }
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 16)]
        [DisplayName(@"Export Cross-List nodes"), Description("Include Cross-List nodes in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportCrossListNodes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 17)]
        [DisplayName(@"Include UoM"), Description("Include Units of Measure in the export")]
        [DefaultValue(true)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeUoM { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 18)]
        [DisplayName(@"Include Field 1"), Description("Include Field 1 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeField1 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 19)]
        [DisplayName(@"Include Field 2"), Description("Include Field 2 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeField2 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 20)]
        [DisplayName(@"Include Field 3"), Description("Include Field 3 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeField3 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 21)]
        [DisplayName(@"Include Field 4"), Description("Include Field 4 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeField4 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 22)]
        [DisplayName(@"Include Field 5"), Description("Include Field 5 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeField5 { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IgnoreT1Taxonomy", IgnoreT1Taxonomy);
            info.AddValue("GenerateFileWithBlankValues", IgnoreT1Taxonomy);
            info.AddValue("GlobalAttributes", GlobalAttributes);
            info.AddValue("SkuInclusions", SkuInclusions);
            info.AddValue("SkuExclusions", SkuExclusions);
            //info.AddValue("SkuValueInclusions", SkuValueInclusions);
            info.AddValue("ExportNavigationAttributes", ExportNavigationAttributes);
            info.AddValue("ExportDisplayAttributes", ExportDisplayAttributes);
            info.AddValue("ExportUnRankedAttributes", ExportUnRankedAttributes);
            info.AddValue("ExportRanks", ExportRanks);
            info.AddValue("ExportSuperSchema", ExportSuperSchema);
            info.AddValue("ExportNewValueColumn", ExportNewValueColumn);
            info.AddValue("Top6OnlyAttributes", Top6OnlyAttributes);
            info.AddValue("OrderAttributesBy", OrderAttributesBy);
            info.AddValue("AttributeGroupExclusions", AttributeGroupExclusions);
            info.AddValue("AttributeGroupInclusions", AttributeGroupInclusions);
            info.AddValue("ExportCrossListNodes", ExportCrossListNodes);
            info.AddValue("IncludeUoM", IncludeUoM);
            info.AddValue("IncludeField1", IncludeField1);
            info.AddValue("IncludeField2", IncludeField2);
            info.AddValue("IncludeField3", IncludeField3);
            info.AddValue("IncludeField4", IncludeField4);
            info.AddValue("IncludeField5", IncludeField5);
        }

        public override void Run()
        {
            State = WorkerState.Working;
            _parsedSkuExclusions = ParseSkuInclusionAndExclusions(SkuExclusions);
            _parsedSkuInclusions = ParseSkuInclusionAndExclusions(SkuInclusions);
            //parsedSkuValueInclusions = ParseSkuValueInclusions(SkuValueInclusions);

            _taxonomyAttributesCache = new Dictionary<Guid, List<Tuple<Attribute, decimal, decimal>>>();
            _baseAttributeNames = new Dictionary<string, string>();
            _delimiter = FieldDelimiter.GetValue().ToString();

            //Create new context
            _currentDb = new SkuDataDbDataContext();
            var dlo = new DataLoadOptions();
            _projectField1Name = AryaTools.Instance.InstanceData.CurrentProject.EntityField1Name ?? string.Empty;
            dlo.LoadWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas);
            dlo.LoadWith<EntityInfo>(entityInfo => entityInfo.EntityDatas);
            dlo.LoadWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas);

            //dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas.Where(p => p.Active));
            //dlo.AssociateWith<EntityInfo>(entityInfo => entityInfo.EntityDatas.Where(p => p.Active));
            dlo.AssociateWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas.Where(p => p.Active));
            //dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.SkuInfos.Where(p => p.Active));

            _currentDb.LoadOptions = dlo;
            _currentDb.CommandTimeout = 2000;

            _currentDb.Connection.Open();
            _currentDb.Connection.ChangeDatabase(AryaTools.Instance.InstanceData.Dc.Connection.Database);

            InitGlobals(GlobalAttributes);

            StatusMessage = "Init";

            var fi = new FileInfo(ExportFileName);
            var baseFileName = fi.FullName.Replace(fi.Extension, string.Empty);

            _taxonomyFile = new StreamWriter(baseFileName + "_Classification.txt", false, Encoding.UTF8);
            _taxonomyFile.WriteLine("ItemId{0}New Taxonomy{0}Node Type{0}Old Taxonomy", _delimiter);

            _attributeDataFile = new StreamWriter(baseFileName + "_AttributeData.txt", false, Encoding.UTF8);
            if (GenerateFileWithBlankValues)
            {
                _blankValuesAttributeDataFile = new StreamWriter(baseFileName + "_AttributeBlanks.txt", false,
                    Encoding.UTF8);
            }

            _attributeDataFile.Write("ItemId{0}Taxonomy{0}Node Type", _delimiter);
            if (GenerateFileWithBlankValues)
                _blankValuesAttributeDataFile.Write("ItemId{0}Taxonomy{0}Node Type", _delimiter);
            foreach (var attribute in _globalAttributeHeaders)
            {
                string attributeHeader = null;
                var parts = attribute.Split(':');
                var noOfHeaders = 0;
                if (parts.Count() > 1)
                    Int32.TryParse(parts[1].Trim(), out noOfHeaders);
                if (parts.Count() == 2 && noOfHeaders > 0)
                {
                    for (var i = 0; i < noOfHeaders; i++)
                    {
                        if (attributeHeader == null)
                            attributeHeader += parts[0].Trim() + (i + 1);
                        else
                            attributeHeader += "\t" + parts[0].Trim() + (i + 1);
                    }
                }
                else
                    attributeHeader = attribute;
                _attributeDataFile.Write("{0}{1}", _delimiter, attributeHeader);
                if (GenerateFileWithBlankValues)
                    _blankValuesAttributeDataFile.Write("{0}{1}", _delimiter, attributeHeader);
            }
            if (ExportRanks)
            {
                _attributeDataFile.Write("{0}Rank 1", _delimiter);
                if (GenerateFileWithBlankValues)
                    _blankValuesAttributeDataFile.Write("{0}Rank 1", _delimiter);
            }

            _attributeDataFile.Write("{0}Att 1", _delimiter);
            if (GenerateFileWithBlankValues)
                _blankValuesAttributeDataFile.Write("{0}Att 1", _delimiter);

            if (ExportNewValueColumn)
            {
                _attributeDataFile.Write("{0}New Value 1", _delimiter);
                if (GenerateFileWithBlankValues)
                    _blankValuesAttributeDataFile.Write("{0}New Value 1", _delimiter);
            }

            _attributeDataFile.Write("{0}Val 1", _delimiter);
            if (IncludeUoM)
                _attributeDataFile.Write("{0}Uom 1", _delimiter);
            if (IncludeField1)
                _attributeDataFile.Write("{0}Field 1 1", _delimiter);
            if (IncludeField2)
                _attributeDataFile.Write("{0}Field 2 1", _delimiter);
            if (IncludeField3)
                _attributeDataFile.Write("{0}Field 3 1", _delimiter);
            if (IncludeField4)
                _attributeDataFile.Write("{0}Field 4 1", _delimiter);
            if (IncludeField5)
                _attributeDataFile.Write("{0}Field 5 1", _delimiter);
            _attributeDataFile.WriteLine("{0}[...]", _delimiter);

            if (GenerateFileWithBlankValues)
            {
                _blankValuesAttributeDataFile.Write("{0}Val 1", _delimiter);
                if (IncludeUoM)
                    _blankValuesAttributeDataFile.Write("{0}Uom 1", _delimiter);
                if (IncludeField1)
                    _blankValuesAttributeDataFile.Write("{0}Field 1 1", _delimiter);
                if (IncludeField2)
                    _blankValuesAttributeDataFile.Write("{0}Field 2 1", _delimiter);
                if (IncludeField3)
                    _blankValuesAttributeDataFile.Write("{0}Field 3 1", _delimiter);
                if (IncludeField4)
                    _blankValuesAttributeDataFile.Write("{0}Field 4 1", _delimiter);
                if (IncludeField5)
                    _blankValuesAttributeDataFile.Write("{0}Field 5 1", _delimiter);
                _blankValuesAttributeDataFile.WriteLine("{0}[...]", _delimiter);
            }

            var exportTaxonomyIds =
                Taxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).Distinct().ToList();
            var exportTaxonomies = _currentDb.TaxonomyInfos.Where(p => exportTaxonomyIds.Contains(p.ID)).ToList();

            var allExportTaxonomies =
                exportTaxonomies.SelectMany(p => p.AllChildren2).Union(exportTaxonomies).Distinct().ToList();

            CurrentProgress = 0;
            MaximumProgress = allExportTaxonomies.Count;

            foreach (var exportChildTaxonomy in allExportTaxonomies)
            {
                WriteTaxonomyToFile(exportChildTaxonomy);
                CurrentProgress++;
            }

            _taxonomyFile.Close();
            _attributeDataFile.Close();
            if (GenerateFileWithBlankValues)
                _blankValuesAttributeDataFile.Close();

            StatusMessage = "Done!";
            State = WorkerState.Ready;
        }

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        private void WriteTaxonomyToFile(TaxonomyInfo taxonomy)
        {
            StatusMessage = string.Format("{1}{0}Reading Node", Environment.NewLine, taxonomy);
            IQueryable<Sku> allSkus;

            if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
            {
                if (!ExportCrossListNodes)
                    return;

                var cl = Query.FetchCrossListObject(taxonomy);
                if (cl == null)
                    return;
                allSkus =
                    Query.GetFilteredSkus(cl.TaxonomyIDFilter, cl.ValueFilters, cl.AttributeTypeFilters,
                        cl.MatchAllTerms).Distinct();
            }
            else
            {
                allSkus = from sku in _currentDb.Skus
                    where sku.SkuInfos.Any(si => si.Active && si.TaxonomyInfo == taxonomy)
                    select sku;
            }

            StatusMessage = string.Format("{1}{0}Filtering SKUs", Environment.NewLine, taxonomy);
            var skus = GetFilteredSkuList(allSkus, true, _parsedSkuInclusions, _parsedSkuExclusions).ToList();

            var iCtr = 0;
            var noOfSkus = skus.Count;
            var ignoreT1taxonomy = taxonomy.ToString();
            if (IgnoreT1Taxonomy)
                ignoreT1taxonomy = taxonomy.ToString().Substring(taxonomy.ToString().IndexOf('>') + 1);
            skus.ForEach(sku =>
                         {
                             StatusMessage = string.Format("{1}{0}{2} of {3} SKUs", Environment.NewLine, taxonomy,
                                 (++iCtr), noOfSkus);

                             var taxonomies =
                                 sku.SkuInfos.Select(
                                     si =>
                                         new
                                         {
                                             Taxonomy = si.TaxonomyInfo.OldString(),
                                             SkuAssignedOn = si.CreatedOn,
                                             SkuAssignedBy = si.User.FullName,
                                             SkuAssignRemark =
                                                 (si.CreatedRemark == null) ? String.Empty : si.Remark.Remark1,
                                             TaxonomyCreatedOn =
                                                 si.TaxonomyInfo.OldTaxonomyData == null
                                                     ? string.Empty
                                                     : si.TaxonomyInfo.OldTaxonomyData.CreatedOn.ToString(),
                                             TaxonomyCreatedBy =
                                                 si.TaxonomyInfo.OldTaxonomyData == null
                                                     ? string.Empty
                                                     : si.TaxonomyInfo.OldTaxonomyData.User.FullName
                                         })
                                     .Where(d => !string.IsNullOrWhiteSpace(d.Taxonomy));
                             if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                             {
                                 var firstOrDefault = sku.SkuInfos.FirstOrDefault(a => a.Active);
                                 if (firstOrDefault != null)
                                 {
                                     var originalTaxonomy = firstOrDefault.TaxonomyInfo;

                                     _taxonomyFile.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}", _delimiter,
                                         sku.ItemID,
                                         ignoreT1taxonomy + TaxonomyInfo.CrossPrefix + originalTaxonomy.ToString(),
                                         "Cross List", ignoreT1taxonomy));
                                 }
                             }
                             else
                             {
                                 var firstOrDefault = taxonomies.FirstOrDefault();
                                 if (firstOrDefault != null)
                                 {
                                     _taxonomyFile.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}", _delimiter,
                                         sku.ItemID, ignoreT1taxonomy, TaxonomyInfo.NodeTypeRegular,
                                         firstOrDefault.Taxonomy));
                                 }
                             }

                             if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                             {
                                 var firstOrDefault = sku.SkuInfos.FirstOrDefault(a => a.Active);
                                 if (firstOrDefault != null)
                                 {
                                     var originalTaxonomy = firstOrDefault.TaxonomyInfo;
                                     var attributes = GetExportAttributes(originalTaxonomy);
                                     WriteAttributeDataToFile(sku, originalTaxonomy, "Cross List",
                                         ignoreT1taxonomy + TaxonomyInfo.CrossPrefix + originalTaxonomy.ToString(),
                                         attributes);
                                 }
                             }
                             else
                             {
                                 var attributes = GetExportAttributes(taxonomy);
                                 WriteAttributeDataToFile(sku, taxonomy, TaxonomyInfo.NodeTypeRegular, ignoreT1taxonomy,
                                     attributes);
                             }
                         });

            StatusMessage = string.Format("{1}{0}({2} of {3}), Fetching Child Nodes", Environment.NewLine, taxonomy,
                iCtr, noOfSkus);
            //taxonomy.ChildTaxonomyDatas.Where(td => td.Active).Select(td => td.TaxonomyInfo).ForEach(
            //    WriteTaxonomyToFile);
            //CurrentProgress++;
        }

        private void WriteAttributeDataToFile(Sku sku, TaxonomyInfo taxonomy, string nodeType, string taxonomyString,
            IEnumerable<Tuple<Attribute, decimal, decimal>> attributeOrders)
        {
            var itemHeader = string.Format("{1}{0}{2}{0}{3}", _delimiter, sku.ItemID, taxonomyString, nodeType);
            var itemGlobals = string.Empty;
            var itemAttributeValues = string.Empty;
            var itemAttributeBlanks = string.Empty;

            for (var iCtr = 0; iCtr < _globalAttributeNames.Count(); iCtr++)
            {
                var headerCount = _globalAttributeHeaders[iCtr].Split(':').Count() > 1
                    ? _globalAttributeHeaders[iCtr].Split(':')[1].Trim()
                    : string.Empty;

                int noOfHeaders;
                Int32.TryParse(headerCount, out noOfHeaders);

                var globalDelimiter = noOfHeaders == 0 ? ", " : "\t";

                var parts = _globalAttributeNames[iCtr].Split(new[] {','});
                var values = new List<EntityData>();
                foreach (var att in parts)
                    values.AddRange(sku.GetValuesForAttribute(att.Trim()));
                //TODO: do global UoMs need to be made optional?
                var stringValue =
                    values.Select(val => (val.Value + " " + val.Uom).Trim())
                        .Distinct()
                        .OrderBy(val => val, new CompareForAlphaNumericSort())
                        .Aggregate(string.Empty,
                            (current, val) =>
                                current + ((string.IsNullOrEmpty(current) ? string.Empty : globalDelimiter) + val));

                if (noOfHeaders > 0)
                {
                    var valuesWritten = new string[noOfHeaders];
                    var valuespresent = stringValue.Split(new[] {globalDelimiter}, StringSplitOptions.None);

                    for (var i = 0; i < noOfHeaders; i++)
                    {
                        if (i < valuespresent.Count())
                            valuesWritten[i] = valuespresent[i];
                        else
                            valuesWritten[i] = string.Empty;
                    }

                    stringValue = string.Join("\t", valuesWritten);
                }

                itemGlobals += _delimiter + stringValue;
            }

            foreach (var att in attributeOrders)
            {
                //Attribute,NavOrder,DisplayOrder
                var attributeOrder = att;
                var navOrder = att.Item2;
                var dispOrder = att.Item3;

                if (((ExportNavigationAttributes && navOrder > 0) || (ExportDisplayAttributes && dispOrder > 0)
                     || (ExportUnRankedAttributes && navOrder == 0 && dispOrder == 0)))
                {
                    //CuttingTools project wide exception from Rebecca, to sort attributes with (Wire) suffix in descending.
                    var ctSortExcp = taxonomy.Project.ProjectName == "CuttingTools"
                                     && attributeOrder.Item1.AttributeName.Contains("(Wire)");

                    // get the collection of values (in entity data objects) associated with this attribute/sku combination
                    var entity = ctSortExcp
                        ? sku.GetValuesForAttribute(attributeOrder.Item1.AttributeName, true, true)
                        : sku.GetValuesForAttribute(attributeOrder.Item1.AttributeName);

                    //if (!ValueFilterValid(entity, parsedSkuValueInclusions))
                    //    continue;

                    // concatenate the values into a comma-delimited string
                    var value = entity.Aggregate(string.Empty,
                        (current, ed) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + ed.Value);

                    //display the uom once if there is one distinct uom, otherwise, output in the same order as the value (optional)
                    var uom = String.Empty;
                    if (IncludeUoM)
                    {
                        var distinctUoms =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Uom))
                                .Select(ed => ed.Uom)
                                .Distinct()
                                .ToList();
                        uom = distinctUoms.Aggregate(String.Empty,
                            (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 1 into a comma-delimited string
                    var field1 = String.Empty;
                    if (IncludeField1)
                    {
                        var distinctField1 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field1))
                                .Select(ed => ed.Field1)
                                .Distinct()
                                .ToList();
                        field1 = distinctField1.Aggregate(String.Empty,
                            (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 2 into a comma-delimited string
                    var field2 = String.Empty;
                    if (IncludeField2)
                    {
                        var distinctField2 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field2))
                                .Select(ed => ed.Field2)
                                .Distinct()
                                .ToList();
                        field2 = distinctField2.Aggregate(String.Empty,
                            (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 3 into a comma-delimited string
                    var field3 = String.Empty;
                    if (IncludeField3)
                    {
                        var distinctField3 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field3))
                                .Select(ed => ed.Field3)
                                .Distinct()
                                .ToList();
                        field3 = distinctField3.Aggregate(String.Empty,
                            (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 4 into a comma-delimited string
                    var field4 = String.Empty;
                    if (IncludeField4)
                    {
                        var distinctField4 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field4))
                                .Select(ed => ed.Field4)
                                .Distinct()
                                .ToList();
                        field4 = distinctField4.Aggregate(String.Empty,
                            (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 5 into a comma-delimited string
                    var field5 = String.Empty;
                    if (IncludeField5)
                    {
                        var distinctField5 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field5OrStatus))
                                .Select(ed => ed.Field5OrStatus)
                                .Distinct()
                                .ToList();
                        field5 = distinctField5.Aggregate(String.Empty,
                            (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // optionally, get the navigation data
                    var rank = string.Empty;
                    if (navOrder > 0 || dispOrder > 0)
                    {
                        if (ExportNavigationAttributes)
                        {
                            rank = ExportSuperSchema && taxonomy.IsLeafNode
                                ? "S"
                                : Decimal.Truncate(navOrder).ToString();
                        }
                        if (ExportDisplayAttributes)
                        {
                            if (ExportNavigationAttributes)
                                rank += " • ";
                            rank += Decimal.Truncate(dispOrder).ToString();
                        }
                    }

                    // append navigation data
                    if (ExportRanks)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, rank);

                    // append attribute name
                    itemAttributeValues += string.Format("{0}{1}", _delimiter, attributeOrder.Item1.AttributeName);

                    if (ExportNewValueColumn)
                        itemAttributeValues += _delimiter;

                    // append value data
                    itemAttributeValues += string.Format("{0}{1}", _delimiter, value);

                    // append UoM data
                    if (IncludeUoM)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, uom);

                    // append field 1 data
                    if (IncludeField1)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, field1);

                    // append field 2 data
                    if (IncludeField2)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, field2);

                    // append field 3 data
                    if (IncludeField3)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, field3);

                    // append field 4 data
                    if (IncludeField4)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, field4);

                    // append field 5 data
                    if (IncludeField5)
                        itemAttributeValues += string.Format("{0}{1}", _delimiter, field5);

                    if (GenerateFileWithBlankValues && string.IsNullOrWhiteSpace(value))
                    {
                        itemAttributeBlanks += string.Format("{0}{1}{0}{2}{0}{3}", _delimiter, rank,
                            attributeOrder.Item1.AttributeName, value);
                        if (IncludeUoM)
                            itemAttributeBlanks += string.Format("{0}{1}", _delimiter, uom);

                        if (IncludeField1)
                            itemAttributeBlanks += string.Format("{0}{1}", _delimiter, field1);

                        if (IncludeField2)
                            itemAttributeBlanks += string.Format("{0}{1}", _delimiter, field2);

                        if (IncludeField3)
                            itemAttributeBlanks += string.Format("{0}{1}", _delimiter, field3);

                        if (IncludeField4)
                            itemAttributeBlanks += string.Format("{0}{1}", _delimiter, field4);

                        if (IncludeField5)
                            itemAttributeBlanks += string.Format("{0}{1}", _delimiter, field5);
                    }
                }
            }

            _attributeDataFile.WriteLine(itemHeader + itemGlobals + itemAttributeValues);
            if (GenerateFileWithBlankValues)
                _blankValuesAttributeDataFile.WriteLine(itemHeader + itemGlobals + itemAttributeBlanks);
        }

        private bool ValueFilterValid(IEnumerable<EntityData> entity, Dictionary<string, string> valueFilters)
        {
            var isValidEntity = false;
            if (valueFilters == null || valueFilters.Count == 0)
                return true;

            foreach (var filter in valueFilters)
            {
                if (filter.Key == _projectField1Name.ToLower())
                {
                    var filter1 = filter;
                    isValidEntity =
                        entity.Where(en => filter1.Value == (en.Field1 != null ? en.Field1.ToLower() : en.Field1))
                            .Aggregate(isValidEntity, (current, en) => true);
                }
                else
                    return false;
            }
            return isValidEntity;
        }

        //Attribute,NavRank,DispRank
        private IEnumerable<Tuple<Attribute, decimal, decimal>> GetExportAttributes(TaxonomyInfo taxonomy)
        {
            if (_taxonomyAttributesCache.ContainsKey(taxonomy.ID))
                return _taxonomyAttributesCache[taxonomy.ID];
            var schemaInfos = new List<SchemaInfo>();
            schemaInfos.AddRange(taxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());

            if (ExportSuperSchema)
            {
                foreach (var leaf in taxonomy.AllLeafChildren)
                    schemaInfos.AddRange(leaf.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());
            }

            var atts =
                schemaInfos.Where(p => p.SchemaData.InSchema).Select(p => new {p.Attribute, p.SchemaData}).ToList();

            List<Tuple<Attribute, decimal, decimal>> attributes;

            if (Top6OnlyAttributes == false)
            {
                if (ExportSuperSchema)
                {
                    attributes = (from att in atts
                        group att by att.Attribute
                        into grp
                        let minRank = grp.Min(p => GetRank(p.SchemaData, SortOrder.OrderbyDisplayNavigation))
                        orderby minRank
                        select new Tuple<Attribute, decimal, decimal>(grp.Key, 0, minRank)).ToList();
                }
                else
                {
                    attributes = (from att in atts
                        let rank = GetRank(att.SchemaData, OrderAttributesBy)
                        orderby rank, att.Attribute.AttributeName
                        select
                            new Tuple<Attribute, decimal, decimal>(att.Attribute,
                                att.SchemaData == null ? 0 : att.SchemaData.NavigationOrder,
                                att.SchemaData == null ? 0 : att.SchemaData.DisplayOrder)).ToList();
                }
            }
            else
            {
                var attrs = (from att in atts.Where(att => att.SchemaData != null && att.SchemaData.NavigationOrder > 0)
                    let baseAttributeName = GetBaseAttributeName(att.Attribute.AttributeName)
                    group att by baseAttributeName
                    into grp
                    let minNavRank = grp.Min(g => g.SchemaData.NavigationOrder)
                    orderby minNavRank
                    select new {BaseAttributeName = grp.Key, Attributes = grp.Select(p => p.Attribute)}).Take(6)
                    .SelectMany(p => p.Attributes)
                    .Distinct()
                    .ToList();

                attributes = (from attribute1 in attrs
                    let schemaData = taxonomy.SchemaInfos.Single(si => si.AttributeID == attribute1.ID).SchemaData
                    select
                        schemaData == null
                            ? new Tuple<Attribute, decimal, decimal>(attribute1, 0, 0)
                            : new Tuple<Attribute, decimal, decimal>(attribute1, schemaData.NavigationOrder,
                                schemaData.DisplayOrder)).ToList();
            }

            if (AttributeGroupExclusions.Length > 0)
            {
                //exclude Attribute Groups
                attributes =
                    attributes.Where(
                        p => p.Item1.Group == null || (!AttributeGroupExclusions.Contains(p.Item1.Group.ToLower())))
                        .ToList();
            }

            if (AttributeGroupInclusions.Length > 0)
            {
                //include Attribute Groups
                attributes =
                    attributes.Where(
                        p => p.Item1.Group == null || (AttributeGroupInclusions.Contains(p.Item1.Group.ToLower())))
                        .ToList();
            }

            if ((AttributeGroupExclusions.Length > 0 && AttributeGroupExclusions.Contains("default"))
                || (AttributeGroupInclusions.Length > 0 && !AttributeGroupInclusions.Contains("default")))
                attributes = attributes.Where(p => p.Item1.Group != null).ToList();

            _taxonomyAttributesCache.Add(taxonomy.ID, attributes);

            return attributes;
        }

        private string GetBaseAttributeName(string attributeName)
        {
            if (_baseAttributeNames.ContainsKey(attributeName))
                return _baseAttributeNames[attributeName];

            var baseAttributeName = attributeName;
            var result = _rxParanthesis.Replace(baseAttributeName, string.Empty).Replace("Unv_", string.Empty).Trim();

            _baseAttributeNames[attributeName] = result;
            return result;
        }

        public void FillTaxonomyAttributesCacheForImagingGapFill(TaxonomyInfo rootTaxonomy)
        {
            var allChilds = rootTaxonomy.AllLeafChildren.ToList();

            //include the root
            allChilds.Add(rootTaxonomy);

            var maxAttrCount = _currentDb.ExecuteQuery<int>(@"SELECT MAX(A.CT) AS MaxAttrCount
                    FROM 
                    (
	                    SELECT TaxonomyID,COUNT(DISTINCT AttributeName)AS  CT
	                    FROM SchemaAttributes 
	                    WHERE InSchema = 1
	                    AND T2 = 'Representational'
	                    AND T3 = 'Skus'
	                    AND (DisplayOrder <= 49 AND DisplayOrder >= 1)
	                    GROUP BY TaxonomyID 
                    ) A").Single();

            //Data Combination
            var dCSchema =
                allChilds.SelectMany(
                    child =>
                        child.SchemaInfos.Where(
                            si =>
                                si.SchemaDatas.Any(sd => sd.Active && sd.InSchema)
                                && si.Attribute.AttributeName == "Data Combination")).FirstOrDefault();

            var dCSchemaAttribute = dCSchema == null
                ? new Attribute {AttributeName = "Data Combination"}
                : dCSchema.Attribute;

            var blankAttribute = new Attribute {AttributeName = string.Empty};

            foreach (var child in allChilds)
            {
                if (_taxonomyAttributesCache.ContainsKey(child.ID))
                    continue;

                var atts =
                    child.SchemaInfos.Where(p => p.SchemaData != null && p.SchemaData.InSchema)
                        .Select(p => new {p.Attribute, p.SchemaData})
                        .ToList();

                var attributes = (from att in atts
                    let rank = GetRank(att.SchemaData, OrderAttributesBy)
                    orderby rank, att.Attribute.AttributeName
                    select
                        new Tuple<Attribute, decimal, decimal>(att.Attribute,
                            att.SchemaData == null ? 0 : att.SchemaData.NavigationOrder,
                            att.SchemaData == null ? 0 : att.SchemaData.DisplayOrder)).ToList();

                var finalList = new List<Tuple<Attribute, decimal, decimal>>();

                var lessThan50DisplayAttributes = attributes.Where(p => p.Item3 < 50);

                //add all the left attributes
                finalList.AddRange(lessThan50DisplayAttributes);

                if (finalList.Count != maxAttrCount)
                {
                    var misCount = maxAttrCount - finalList.Count;
                    finalList.AddRange(
                        Enumerable.Repeat(new Tuple<Attribute, decimal, decimal>(blankAttribute, 0, int.MaxValue),
                            misCount));
                }

                //add DC attribute
                finalList.Add(new Tuple<Attribute, decimal, decimal>(dCSchemaAttribute, 0, int.MaxValue));

                finalList.AddRange(attributes.Where(p => p.Item3 > 50));
                _taxonomyAttributesCache.Add(child.ID, finalList);
            }
        }

        private void InitGlobals(IEnumerable<string> globals)
        {
            var globalCount = 0;
            if (globals != null)
                globalCount = globals.Count();
            _globalAttributeNames = new string[globalCount];
            _globalAttributeHeaders = new string[globalCount];

            if (globals == null)
                return;

            var iCtr = 0;
            foreach (var global in globals)
            {
                var parts = global.Split(new[] {'='});
                if (parts.Count() == 1)
                {
                    _globalAttributeNames[iCtr] = parts[0].Trim();
                    _globalAttributeHeaders[iCtr] = parts[0].Trim();
                }
                else
                {
                    _globalAttributeNames[iCtr] = parts[0].Trim();
                    _globalAttributeHeaders[iCtr] = parts[1].Trim();
                }
                iCtr++;
            }
        }
    }
}