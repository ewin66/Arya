using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Arya.Framework.Common.ComponentModel;
using AryaData = Arya.Framework.Data.AryaDb;
using Arya.Framework.Data.AryaDb;
using TaxonomyInfo = Arya.Framework.Data.AryaDb.TaxonomyInfo;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"Review Files")]
    public class ExportWorkerForReviewFiles : ExportWorkerBase
    {
        private readonly DataTable _taxonomyTable = new DataTable("Classification")
        {
            Columns =
            {
                {"Item Id", typeof (String)},
                {"New Taxonomy", typeof (String)},
                {"Node Type", typeof (String)},
                {"Old Taxonomy", typeof (String)}
            }
        };

        private ColumnSetDataTable _attributeTable;
        private ColumnSetDataTable _blankAttributeTable;
        
//        private string[] _attributeGroupExclusions = new string[0];
//        private string[] _attributeGroupInclusions = new string[0];

        private ReviewExportArgs _args;
        private string[] _globalAttributeNames;
        private int[] _globalAttributeCount;
        private readonly Dictionary<Guid, List<Tuple<Attribute, decimal, decimal>>> _taxonomyAttributesCache = new Dictionary<Guid, List<Tuple<Attribute, decimal, decimal>>>();
        private readonly Dictionary<string, string> _baseAttributeNames = new Dictionary<string, string>();
        private readonly Regex _rxParanthesis = new Regex("\\(.*", RegexOptions.Compiled);

        public ExportWorkerForReviewFiles(string argumentFilePath)
            : base(argumentFilePath, typeof (ReviewExportArgs))
        {
        }
        
        public virtual bool IsInputValid()
        {
            throw new NotImplementedException();
        }

        protected override void FetchExportData()
        {
            // set arguments
            _args = (ReviewExportArgs)Arguments;
            _attributeTable = new ColumnSetDataTable("AttributeData");
            if (_args.GenerateFileWithBlankValues)
            {
                _blankAttributeTable = new ColumnSetDataTable("AttributeBlanks");
            }
            InitDataTables();

            // get list of taxonomies to export
            var exportTaxonomyIds = _args.TaxonomyIds;
            var exportTaxonomies = CurrentDb.TaxonomyInfos.Where(p => exportTaxonomyIds.Contains(p.ID)).ToList();
            var allExportTaxonomies = exportTaxonomies.SelectMany(p => p.AllChildren).Union(exportTaxonomies).Distinct().ToList();

            foreach (var taxonomy in allExportTaxonomies)
            {
                ProcessTaxonomy(taxonomy);
            }

            // add tables to export list
            ExportDataTables.Add(_taxonomyTable);
            ExportDataTables.Add(_attributeTable);
            if (_args.GenerateFileWithBlankValues)
                ExportDataTables.Add(_blankAttributeTable);
        }

        private void ProcessTaxonomy(TaxonomyInfo taxonomy)
        {
            // set formatted taxonomy string
            var taxonomyString = taxonomy.ToString(_args.IgnoreT1Taxonomy);

            // get list of SKUs for this taxonomy
            var allSkus = taxonomy.GetSkus(_args.ExportCrossListNodes);
            if (!allSkus.Any())
                return;

            // filter skus based on sku inclusions and exclusions, then include only Product skus.
            var skus = _args.GetFilteredSkuList(allSkus).ToList();

            // process each sku...
            skus.ForEach(sku =>
            {
                // get data on this sku's taxonomy history
                var taxonomies = sku.SkuInfos.Select(
                    si =>
                    new
                        {
                            Taxonomy = si.TaxonomyInfo.OldString(),
                            SkuAssignedOn = si.CreatedOn,
                            SkuAssignedBy = si.User.FullName,
                            SkuAssignRemark = (si.CreatedRemark == null) ? String.Empty : si.Remark.Remark1,
                            TaxonomyCreatedOn = si.TaxonomyInfo.OldTaxonomyData == null
                                ? string.Empty
                                : si.TaxonomyInfo.OldTaxonomyData.CreatedOn.ToString(CultureInfo.InvariantCulture),
                            TaxonomyCreatedBy = si.TaxonomyInfo.OldTaxonomyData == null
                                ? string.Empty
                                : si.TaxonomyInfo.OldTaxonomyData.User.FullName
                        }).Where(d => !string.IsNullOrWhiteSpace(d.Taxonomy));
                
                // write line to taxonomy file
                if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    var firstOrDefault = sku.SkuInfos.FirstOrDefault(a => a.Active);
                    if (firstOrDefault != null)
                    {
                        var originalTaxonomy = firstOrDefault.TaxonomyInfo;
                        WriteTaxonomyRow(sku.ItemID, taxonomyString + TaxonomyInfo.CROSS_PREFIX +
                                                     originalTaxonomy.ToString(), "Cross List", taxonomyString);
                    }
                }
                else
                {
                    var firstOrDefault = taxonomies.FirstOrDefault();
                    if (firstOrDefault != null)
                        WriteTaxonomyRow(sku.ItemID, taxonomyString, TaxonomyInfo.NodeTypeRegular, firstOrDefault.Taxonomy);
                }

                // write line to attribute file
                if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    var firstOrDefault = sku.SkuInfos.FirstOrDefault(a => a.Active);
                    if (firstOrDefault != null)
                    {
                        var originalTaxonomy = firstOrDefault.TaxonomyInfo;
                        var attributes = GetExportAttributes(originalTaxonomy);
                        ProcessAttributes(sku, originalTaxonomy, "Cross List",
                                                taxonomyString + TaxonomyInfo.CROSS_PREFIX +
                                                originalTaxonomy.ToString(), attributes);
                    }
                }
                else
                {
                    var attributes = GetExportAttributes(taxonomy);
                    ProcessAttributes(sku, taxonomy, TaxonomyInfo.NodeTypeRegular, taxonomyString, attributes);
                }
            });
        }

        private void WriteTaxonomyRow(string itemId, string newNode, string nodeType, string oldNode)
        {
            DataRow newRow = _taxonomyTable.NewRow();
            newRow["Item Id"] = itemId;
            newRow["New Taxonomy"] = newNode;
            newRow["Node Type"] = nodeType;
            newRow["Old Taxonomy"] = oldNode;

            _taxonomyTable.Rows.Add(newRow);
        }
        
        private void ProcessAttributes(Sku sku, TaxonomyInfo taxonomy, string nodeType, string taxonomyString,
                                              IEnumerable<Tuple<Attribute, decimal, decimal>> attributeOrders)
        {
            // add static information to output lists
            var valueElements = new List<string>();
            valueElements.Add(sku.ItemID);
            valueElements.Add(taxonomyString);
            valueElements.Add(nodeType);

            var blankElements = new List<string>();
            if (_args.GenerateFileWithBlankValues)
            {
                blankElements.Add(sku.ItemID);
                blankElements.Add(taxonomyString);
                blankElements.Add(nodeType);
            }

            // add global attributes
            for (int i = 0; i < _globalAttributeNames.Count(); i++)
            {
                // get list of sub-attributes in this attribute, then collect their values
                var parts = _globalAttributeNames[i].Split(new[] { ',' });
                var values = new List<EntityData>();
                foreach (var att in parts)
                {
                    values.AddRange(sku.GetValuesForAttribute(CurrentDb, att.Trim()));
                }

                if (_globalAttributeCount[i] == 0)
                {
                    // if there's no specified number of columns for this attribute, string the values together 
                    var stringValue =
                        values.Select(val =>
                            (val.Value + (_args.IncludeUoM ? " " + val.Uom : ""))
                                .Trim())
                                .Distinct()
                                .OrderBy(val => val, new CompareForAlphaNumericSort())
                                .Aggregate(string.Empty, (current, val) => current + ((string.IsNullOrEmpty(current) ? string.Empty : ",") + val));                
                    valueElements.Add(stringValue);
                    if (_args.GenerateFileWithBlankValues)
                        blankElements.Add(stringValue);
                }
                else
                {
                    // if there are multiple columns for this attribute, assign the values separately, limited by the count of collumns
                    var listStringValue =
                        values.Select(val =>
                            (val.Value + (_args.IncludeUoM ? " " + val.Uom : ""))
                                .Trim())
                                .Distinct()
                                .OrderBy(val => val, new CompareForAlphaNumericSort())
                                .Take(_globalAttributeCount[i])
                                .ToList();
                    for (int j = 0; j < _globalAttributeCount[i]; j++)
                    {
                        if (j < listStringValue.Count)
                        {
                            valueElements.Add(listStringValue[j]);
                            if (_args.GenerateFileWithBlankValues)
                                blankElements.Add(listStringValue[j]);
                        }
                        else
                        {
                            valueElements.Add("");
                            if (_args.GenerateFileWithBlankValues)
                                blankElements.Add("");
                        }
                    }
                }
            }
            
            // collect data for each attribute
            bool writeBlankRow = false;
            foreach (var att in attributeOrders)
            {
                //Attribute,NavOrder,DisplayOrder
                var attributeOrder = att;
                var navOrder = att.Item2;
                var dispOrder = att.Item3;

                if (((_args.ExportNavigationAttributes && navOrder > 0) || (_args.ExportDisplayAttributes && dispOrder > 0) ||
                     (_args.ExportUnRankedAttributes && navOrder == 0 && dispOrder == 0)))
                {
                    var entity = sku.GetValuesForAttribute(CurrentDb, attributeOrder.Item1.AttributeName);

                    // concatenate the values into a comma-delimited string
                    var value = entity.Aggregate(string.Empty,
                                                 (current, ed) =>
                                                 current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + ed.Value);

                    //display the uom once if there is one distinct uom, otherwise, output in the same order as the value (optional)
                    string uom = String.Empty;
                    if (_args.IncludeUoM)
                    {
                        var distinctUoms =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Uom))
                                .Select(ed => ed.Uom)
                                .Distinct()
                                .ToList();
                        uom = distinctUoms.Aggregate(String.Empty, (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }
                    
                    // concatenate field 1 into a comma-delimited string
                    string field1 = String.Empty;
                    if (_args.IncludeField1)
                    {
                        var distinctField1 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field1))
                                .Select(ed => ed.Field1)
                                .Distinct()
                                .ToList();
                        field1 = distinctField1.Aggregate(String.Empty, (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 2 into a comma-delimited string
                    string field2 = String.Empty;
                    if (_args.IncludeField2)
                    {
                        var distinctField2 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field2))
                                .Select(ed => ed.Field2)
                                .Distinct()
                                .ToList();
                        field2 = distinctField2.Aggregate(String.Empty, (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 3 into a comma-delimited string
                    string field3 = String.Empty;
                    if (_args.IncludeField3)
                    {
                        var distinctField3 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field3))
                                .Select(ed => ed.Field3)
                                .Distinct()
                                .ToList();
                        field3 = distinctField3.Aggregate(String.Empty, (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 4 into a comma-delimited string
                    string field4 = String.Empty;
                    if (_args.IncludeField4)
                    {
                        var distinctField4 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field4))
                                .Select(ed => ed.Field4)
                                .Distinct()
                                .ToList();
                        field4 = distinctField4.Aggregate(String.Empty, (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }

                    // concatenate field 5 into a comma-delimited string
                    string field5 = String.Empty;
                    if (_args.IncludeField5)
                    {
                        var distinctField5 =
                            entity.Where(ed => !string.IsNullOrWhiteSpace(ed.Field5OrStatus))
                                .Select(ed => ed.Field5OrStatus)
                                .Distinct()
                                .ToList();
                        field5 = distinctField5.Aggregate(String.Empty, (current, next) => current + (string.IsNullOrEmpty(current) ? string.Empty : ", ") + next);
                    }
                    
                    // optionally, get the navigation data
                    var rank = string.Empty;
                    if (navOrder > 0 || dispOrder > 0)
                    {
                        if (_args.ExportNavigationAttributes)
                            rank = _args.ExportSuperSchema && taxonomy.IsLeafNode
                                       ? "S"
                                       : Decimal.Truncate(navOrder).ToString(CultureInfo.InvariantCulture);
                        if (_args.ExportDisplayAttributes)
                        {
                            if (_args.ExportNavigationAttributes)
                                rank += " • ";
                            rank += Decimal.Truncate(dispOrder).ToString(CultureInfo.InvariantCulture);
                        }
                    }

                    // add attribute data to output list
                    // append navigation data
                    if (_args.ExportRanks)
                        valueElements.Add(rank);

                    // append attribute name
                    valueElements.Add(attributeOrder.Item1.AttributeName);

                    if (_args.ExportNewValueColumn)
                        valueElements.Add("");

                    // append value data
                    valueElements.Add(value);

                    // append UoM data
                    if (_args.IncludeUoM)
                        valueElements.Add(uom);

                    // append field 1 data
                    if (_args.IncludeField1)
                        valueElements.Add(field1);

                    // append field 2 data
                    if (_args.IncludeField2)
                        valueElements.Add(field2);

                    // append field 3 data
                    if (_args.IncludeField3)
                        valueElements.Add(field3);

                    // append field 4 data
                    if (_args.IncludeField4)
                        valueElements.Add(field4);

                    // append field 5 data
                    if (_args.IncludeField5)
                        valueElements.Add(field5);

                    if (_args.GenerateFileWithBlankValues && string.IsNullOrWhiteSpace(value))
                    {
                        // signal that there are blank values to be written
                        writeBlankRow = true;
                        
                        // append navigation data
                        if (_args.ExportRanks)
                            blankElements.Add(rank);

                        // append attribute name
                        blankElements.Add(attributeOrder.Item1.AttributeName);

                        if (_args.ExportNewValueColumn)
                            blankElements.Add("");

                        // append value data - should be blank
                        blankElements.Add("");

                        // append UoM data
                        if (_args.IncludeUoM)
                            blankElements.Add(uom);

                        // append field 1 data
                        if (_args.IncludeField1)
                            blankElements.Add(field1);

                        // append field 2 data
                        if (_args.IncludeField2)
                            blankElements.Add(field2);

                        // append field 3 data
                        if (_args.IncludeField3)
                            blankElements.Add(field3);

                        // append field 4 data
                        if (_args.IncludeField4)
                            blankElements.Add(field4);

                        // append field 5 data
                        if (_args.IncludeField5)
                            blankElements.Add(field5);
                    }
                }
            }

            _attributeTable.WriteDataRow(valueElements);
            if (_args.GenerateFileWithBlankValues && writeBlankRow)
            {
                _blankAttributeTable.WriteDataRow(blankElements);
            }
        }

        private IEnumerable<Tuple<Attribute, decimal, decimal>> GetExportAttributes(TaxonomyInfo taxonomy)
        {
            if (_taxonomyAttributesCache.ContainsKey(taxonomy.ID))
                return _taxonomyAttributesCache[taxonomy.ID];
            var schemaInfos = new List<SchemaInfo>();
            schemaInfos.AddRange(taxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());

            if (_args.ExportSuperSchema)
            {
                foreach (var leaf in taxonomy.AllLeafChildren)
                    schemaInfos.AddRange(leaf.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());
            }

            var atts =
                schemaInfos.Where(p => p.SchemaData.InSchema).Select(p => new { p.Attribute, p.SchemaData }).ToList();

            List<Tuple<Attribute, decimal, decimal>> attributes;
            if (_args.Top6OnlyAttributes)
            {
                var attrs = (from att in atts.Where(att => att.SchemaData != null && att.SchemaData.NavigationOrder > 0)
                             let baseAttributeName = GetBaseAttributeName(att.Attribute.AttributeName)
                             group att by baseAttributeName
                                 into grp
                                 let minNavRank = grp.Min(g => g.SchemaData.NavigationOrder)
                                 orderby minNavRank
                                 select new { BaseAttributeName = grp.Key, Attributes = grp.Select(p => p.Attribute) }).Take(6)
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
            else
            {
                if (_args.ExportSuperSchema)
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
                                  let rank = GetRank(att.SchemaData, _args.OrderAttributesBy)
                                  orderby rank, att.Attribute.AttributeName
                                  select
                                      new Tuple<Attribute, decimal, decimal>(att.Attribute,
                                          att.SchemaData == null ? 0 : att.SchemaData.NavigationOrder,
                                          att.SchemaData == null ? 0 : att.SchemaData.DisplayOrder)).ToList();
                }

            }

            //TODO: Check on whether Attribute Groups are needed.
//            if (AttributeGroupExclusions.Length > 0)
//            {
//                //exclude Attribute Groups
//                attributes =
//                    attributes.Where(
//                        p =>
//                        p.Item1.Group == null ||
//                        (!AttributeGroupExclusions.Contains(p.Item1.Group.ToLower()))).ToList();
//            }
//
//            if (AttributeGroupInclusions.Length > 0)
//            {
//                //include Attribute Groups
//                attributes =
//                    attributes.Where(
//                        p =>
//                        p.Item1.Group == null ||
//                        (AttributeGroupInclusions.Contains(p.Item1.Group.ToLower()))).ToList();
//            }
//
//            if ((AttributeGroupExclusions.Length > 0 && AttributeGroupExclusions.Contains("default")) ||
//                (AttributeGroupInclusions.Length > 0 && !AttributeGroupInclusions.Contains("default")))
//                attributes = attributes.Where(p => p.Item1.Group != null).ToList();

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

        private void InitDataTables()
        {
            // add global columns
            InitGlobals();

            var columnSetNames = new List<string>();
            // order of columns in list determines order in output
            if (_args.ExportRanks)
                columnSetNames.Add("Rank ");
            
            columnSetNames.Add("Att ");

            if (_args.ExportNewValueColumn)
                columnSetNames.Add("New Value ");

            columnSetNames.Add("Val ");

            if (_args.IncludeUoM)
                columnSetNames.Add("UoM ");

            if (_args.IncludeField1)
                columnSetNames.Add("Field 1 ");

            if (_args.IncludeField2)
                columnSetNames.Add("Field 2 ");

            if (_args.IncludeField3)
                columnSetNames.Add("Field 3 ");
            
            if (_args.IncludeField4)
                columnSetNames.Add("Field 4 ");

            if (_args.IncludeField5)
                columnSetNames.Add("Field 5 ");

            _attributeTable.InitColumnSet(columnSetNames);
            if (_args.GenerateFileWithBlankValues)
            {
                _blankAttributeTable.InitColumnSet(columnSetNames);
            }
            
        }

        private void InitGlobals()
        {
            // if there are no globals, set properties to empty and return
            if (_args.GlobalAttributes == null)
            {
                _globalAttributeNames = new string[0];
                _globalAttributeCount = new int[0];
                return;
            }

            // count globals
            int globalCount = _args.GlobalAttributes.Count();
            _globalAttributeNames = new string[globalCount];
            _globalAttributeCount = new int[globalCount];
            var globalAttributeHeaders = new string[globalCount];

            // divide attributes listed into name and header
            var iCtr = 0;
            foreach (var global in _args.GlobalAttributes)
            {
                var parts = global.Split(new[] { '=' });
                if (parts.Count() == 1)
                {
                    _globalAttributeNames[iCtr] = parts[0].Trim();
                    globalAttributeHeaders[iCtr] = parts[0].Trim();
                }
                else
                {
                    _globalAttributeNames[iCtr] = parts[0].Trim();
                    globalAttributeHeaders[iCtr] = parts[1].Trim();
                }
                iCtr++;
            }
            
            _globalAttributeCount = _attributeTable.InitGlobals(globalAttributeHeaders);
            if (_args.GenerateFileWithBlankValues)
            {
                _blankAttributeTable.InitGlobals(globalAttributeHeaders);
            }
        }
    }

    [Serializable]
    public class ReviewExportArgs : AdvancedExportArgs
    {
        #region Fields

        #endregion Fields

        #region Properties

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 2)]
        [DefaultValue(false)]
        [DisplayName(@"Separate File with Blank Values"), Description("Generate a separate file containing only blank values")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool GenerateFileWithBlankValues { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 6)]
        [DefaultValue(true)]
        [DisplayName(@"Navigation Attributes"), Description("Export Navigation Attributes")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportNavigationAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 7)]
        [DefaultValue(true)]
        [DisplayName(@"Display Attributes"), Description("Export Display Attributes")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportDisplayAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 8)]
        [DefaultValue(false)]
        [DisplayName(@"Unranked Attributes"), Description("Export Unranked Attributes")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportUnRankedAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 9)]
        [DefaultValue(true)]
        [DisplayName(@"Show Ranks"), Description("Export Ranks")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportRanks { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 10)]
        [DefaultValue(false)]
        [DisplayName(@"Super Schema"), Description("Export Super Schema")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportSuperSchema { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 11)]
        [DefaultValue(false)]
        [DisplayName(@"New Value Column"), Description("Export New Value Column")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportNewValueColumn { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 12)]
        [DefaultValue(false)]
        [DisplayName(@"Top 6 Only Attributes"), Description("Export Top 6 Only Attributes")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool Top6OnlyAttributes { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 13)]
        [DefaultValue(SortOrder.OrderbyNavigationDisplay)]
        [TypeConverter(typeof(CustomEnumConverter))]
        public SortOrder OrderAttributesBy { get; set; }

//        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 14)]
//        [DisplayName(@"Group Exclusions"), Description("Attribute Groups to be excluded")]
//        [TypeConverter(typeof(StringArrayConverter))]
//        public string[] AttributeGroupExclusions
//        {
//            get { return _attributeGroupExclusions; }
//            set { _attributeGroupExclusions = value; }
//        }
//
//        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 15)]
//        [DisplayName(@"Group Inclusions"), Description("Attribute Groups to be excluded")]
//        [TypeConverter(typeof(StringArrayConverter))]
//        public string[] AttributeGroupInclusions
//        {
//            get { return _attributeGroupInclusions; }
//            set { _attributeGroupInclusions = value; }
//        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 17)]
        [DisplayName(@"Include UoM"), Description("Include Units of Measure in the export")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeUoM { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 18)]
        [DisplayName(@"Include Field 1"), Description("Include Field 1 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeField1 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 19)]
        [DisplayName(@"Include Field 2"), Description("Include Field 2 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeField2 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 20)]
        [DisplayName(@"Include Field 3"), Description("Include Field 3 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeField3 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 21)]
        [DisplayName(@"Include Field 4"), Description("Include Field 4 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeField4 { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 22)]
        [DisplayName(@"Include Field 5"), Description("Include Field 5 in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeField5 { get; set; }

        #endregion Properties

        #region Constructor

        public ReviewExportArgs()
        {
            HiddenProperties += "ExportExtendedAttributes" + "MarkAsPublished";
        }

        #endregion
    }
}