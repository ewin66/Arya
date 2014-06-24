using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Data;
using TaxonomyInfo = Arya.Framework.Data.AryaDb.TaxonomyInfo;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"Invalid Values")]
    public class ExportWorkerForInvalidValues : ExportWorkerBase
    {
        private ColumnSetDataTable _attributeTable;
        private InvalidValuesExportArgs _args;
        private string[] _globalAttributeNames;
        private int[] _globalAttributeCount;
        private readonly Dictionary<Guid, List<KeyValuePair<Attribute, SchemaData>>> _taxonomyAttributesCache = new Dictionary<Guid, List<KeyValuePair<Attribute, SchemaData>>>();

        public ExportWorkerForInvalidValues(string argumentFilePath)
            : base(argumentFilePath, typeof(InvalidValuesExportArgs))
        {
        }

        public virtual bool IsInputValid()
        {
            throw new NotImplementedException();
        }

        protected override void FetchExportData()
        {
            // set arguments
            _args = (InvalidValuesExportArgs)Arguments;
            _attributeTable = new ColumnSetDataTable("AttributeData");
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
            ExportDataTables.Add(_attributeTable);
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
                // write line to attribute file
                if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    var firstOrDefault = sku.SkuInfos.FirstOrDefault(a => a.Active);
                    if (firstOrDefault != null)
                    {
                        var originalTaxonomy = firstOrDefault.TaxonomyInfo;
                        var attributes = GetExportAttributes(originalTaxonomy);
                        ProcessAttributes(sku, "Cross List",
                                                taxonomyString + TaxonomyInfo.CROSS_PREFIX +
                                                originalTaxonomy.ToString(), attributes);
                    }
                }
                else
                {
                    var attributes = GetExportAttributes(taxonomy);
                    ProcessAttributes(sku, TaxonomyInfo.NodeTypeRegular, taxonomyString, attributes);
                }
            });
        }

        private void ProcessAttributes(Sku sku, string nodeType, string taxonomyString, IEnumerable<KeyValuePair<Attribute, SchemaData>> attributeOrders)
        {
            // add static information to output lists
            var valueElements = new List<string> {sku.ItemID, taxonomyString, nodeType};

            // add global attributes
            for (int i = 0; i < _globalAttributeNames.Count(); i++)
            {
                // get list of sub-attributes in this attribute, then collect their values
                var parts = _globalAttributeNames[i].Split(new[] {','});
                var values = new List<EntityData>();
                foreach (var att in parts)
                {
                    values.AddRange(sku.GetValuesForAttribute(CurrentDb, att.Trim()));
                }

                if (_globalAttributeCount[i] == 0)
                {
                    // if there's no specified number of columns for this attribute, string the values together 
                    var stringValue =
                        values.Select(val => val.Value)
                            .Distinct()
                            .OrderBy(val => val, new CompareForAlphaNumericSort())
                            .Aggregate(string.Empty,
                                (current, val) => current + ((string.IsNullOrEmpty(current) ? string.Empty : ",") + val));
                    valueElements.Add(stringValue);
                }
                else
                {
                    // if there are multiple columns for this attribute, assign the values separately, limited by the count of collumns
                    var listStringValue =
                        values.Select(val =>val.Value)
                            .Distinct()
                            .OrderBy(val => val, new CompareForAlphaNumericSort())
                            .Take(_globalAttributeCount[i])
                            .ToList();
                    for (int j = 0; j < _globalAttributeCount[i]; j++)
                    {
                        valueElements.Add(j < listStringValue.Count ? listStringValue[j] : "");
                    }
                }
            }

            // collect data for each attribute
            bool invalidValuesFound = false;
            foreach (var att in attributeOrders)
            {
                Attribute attribute = att.Key;
                SchemaData schemaData = att.Value;

                var entity = sku.GetValuesForAttribute(CurrentDb, attribute.AttributeName);
                var validator = new Validate(CurrentDb);
                IEnumerable<EntityData> invalidEntities = null;
                if (attribute.AttributeType != AttributeTypeEnum.Derived.ToString())
                {
                    invalidEntities = entity.Where(e => !validator.IsValidDataType(e, schemaData));
                }

                if (invalidEntities == null || !invalidEntities.Any())
                    continue;

                invalidValuesFound = true;
                var value = entity.Aggregate(string.Empty,
                    (current, ed) => current + ((string.IsNullOrEmpty(current) ? string.Empty : ", ") + ed.Value));

                //display the uom once if there is one distinct uom, otherwise, output in the same order as the value
                var distinctUoms = entity.Where(ed => ed.Uom != null).Select(ed => ed.Uom).Distinct().ToList();
                string uom;
                switch (distinctUoms.Count)
                {
                    case 0:
                    {
                        uom = string.Empty;
                        break;
                    }
                    case 1:
                    {
                        uom = distinctUoms.First();
                        break;
                    }
                    default:
                    {
                        uom = entity.Aggregate(string.Empty,
                            (current, u) => current + ((string.IsNullOrEmpty(current) ? string.Empty : ", ") + u.Uom));
                        break;
                    }
                }

                var rank = Decimal.Truncate(schemaData.NavigationOrder) + " • " + Decimal.Truncate(schemaData.DisplayOrder);

                // add attribute data to output list
                // append navigation data
                valueElements.Add(rank);

                // append attribute name
                valueElements.Add(attribute.AttributeName);

                // append value data
                valueElements.Add(value);

                // append UoM data
                valueElements.Add(uom);
            }
            if (invalidValuesFound)
            {
                _attributeTable.WriteDataRow(valueElements);
            }
        }

        private IEnumerable<KeyValuePair<Attribute, SchemaData>> GetExportAttributes(TaxonomyInfo taxonomy)
        {
            if (_taxonomyAttributesCache.ContainsKey(taxonomy.ID))
                return _taxonomyAttributesCache[taxonomy.ID];
            var schemaInfos = new List<SchemaInfo>();
            schemaInfos.AddRange(taxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());

            var attributes = (from si in schemaInfos
                              let rank = GetRank(si.SchemaData, SortOrder.OrderbyNavigationDisplay)
                              where si.SchemaData.InSchema
                              orderby rank, si.Attribute.AttributeName
                              select new KeyValuePair<Attribute, SchemaData>(si.Attribute, si.SchemaData)).ToList();

            _taxonomyAttributesCache.Add(taxonomy.ID, attributes);

            return attributes;
        }

        private void InitDataTables()
        {
            // add global columns
            InitGlobals();

            var columnSetNames = new List<string> {"Rank ", "Att ", "Val ", "UoM "};
            _attributeTable.InitColumnSet(columnSetNames);
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
        }
    }

    [Serializable]
    public class InvalidValuesExportArgs : AdvancedExportArgs
    {
        #region Constructor

        public InvalidValuesExportArgs()
        {
            HiddenProperties += "ExportExtendedAttributes" + "MarkAsPublished";
        }

        #endregion
    }
}