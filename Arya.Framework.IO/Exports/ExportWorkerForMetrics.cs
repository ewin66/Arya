using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"Metrics (Beta)")]
    public class ExportWorkerForMetrics : ExportWorkerBase
    {
        private const string ColTaxonomyPath = "TaxonomyPath";
        private const string ColAttribute = "Attribute";
        private const string ColValue = "Value";
        private const string ColUom = "UOM";
        private const string ColNodeFilteredSkuCount = "Node (Filtered) SKU Count";
        private const string ColAttributeSkuCount = "Attribute SKU Count";
        private const string ColAttributeFillRate = "Attribute Fill Rate";
        private const string ColNavigationOrder = "Navigation Order";
        private const string ColDisplayOrder = "Display Order";
        private const string ColValueSkuCount = "Value SKU Count";
        private const string ColValueFillRate = "Value Fill Rate";
        private const string ColItemId = "Item ID";
        private const string ColNodeAttributeCount = "Node Attribute Count";
        private const string ColNodeNavigationAttributeCount = "Node Navigation Attribute Count";
        private const string ColNodeDisplayAttributeCount = "Node Display Attribute Count";
        private const string ColSkuAttributeCount = "Sku Attribute Count";
        private const string ColSkuNavigationAttributeCount = "Sku Navigation Attribute Count";
        private const string ColSkuDisplayAttributeCount = "Sku Display Attribute Count";
        private const string ColNodeAttributeFillRate = "Node Attribute Fill Rate";
        private const string ColNodeNavigationAttributeFillRate = "Node Navigation Attribute Fill Rate";
        private const string ColNodeDisplayAttributeFillRate = "Node Display Attribute Fill Rate";
        private MetricsExportArgs _args;
        private int _maxDepth;
        private DataTable _attributeMetrics;
        private DataTable _skuMetrics;
        private DataTable _valueMetrics;

        public ExportWorkerForMetrics(string argumentFilePath, PropertyGrid ownerPropertyGrid, Guid userId,
            Guid projectId)
            : base(argumentFilePath, ownerPropertyGrid, userId, projectId)
        {
            throw new NotImplementedException();
        }

        public ExportWorkerForMetrics(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof(MetricsExportArgs))
        {
        }

        public ExportWorkerForMetrics(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        protected override void FetchExportData()
        {
            _args = (MetricsExportArgs)Arguments;
            using (var db = new AryaDbDataContext(_args.ProjectId, _args.UserId))
            {
                var nodes = (from ti in db.TaxonomyInfos where _args.TaxonomyIds.Contains(ti.ID) select ti).ToList();
                var allnodes = (from tax in nodes
                                from node in tax.AllChildren
                                where node.GetSkus(_args.ExportCrossListNodes).Any()
                                select
                                    new
                                    {
                                        TaxId = node.ID,
                                        NoOfLevels =
                                            node.ToString().Split(new[] { TaxonomyInfo.DELIMITER }, StringSplitOptions.None).Length
                                    })
                    .ToList();
                _maxDepth = GetMaxTaxonomyLength(db); //allnodes.Count > 0 ? allnodes.Max(node => node.NoOfLevels) : 0;

                InitDataTables();

                allnodes.ForEach(node => GenerateMetrics(node.TaxId));

                if (_args.IncludeSkuMetrics)
                    ExportDataTables.Add(_skuMetrics);
                if (_args.IncludeAttributeMetrics)
                    ExportDataTables.Add(_attributeMetrics);
                if (_args.IncludeValueMetrics)
                    ExportDataTables.Add(_valueMetrics);
            }
        }

        private void InitDataTables()
        {
            if (_args.IncludeAttributeMetrics)
            {
                _attributeMetrics = new DataTable("AttributeMetrics");
                _attributeMetrics.Columns.Add(ColTaxonomyPath);
                for (var i = 0; i < _maxDepth; i++)
                    _attributeMetrics.Columns.Add("T" + (i + 1));
                _attributeMetrics.Columns.Add(ColAttribute);
                _attributeMetrics.Columns.Add(ColNodeFilteredSkuCount);
                _attributeMetrics.Columns.Add(ColAttributeSkuCount);
                _attributeMetrics.Columns.Add(ColAttributeFillRate);
                _attributeMetrics.Columns.Add(ColNavigationOrder);
                _attributeMetrics.Columns.Add(ColDisplayOrder);
            }

            if (_args.IncludeValueMetrics)
            {
                _valueMetrics = new DataTable("ValueMetrics");
                _valueMetrics.Columns.Add(ColTaxonomyPath);
                for (var i = 0; i < _maxDepth; i++)
                    _valueMetrics.Columns.Add("T" + (i + 1));
                _valueMetrics.Columns.Add(ColAttribute);
                _valueMetrics.Columns.Add(ColValue);
                _valueMetrics.Columns.Add(ColUom);
                _valueMetrics.Columns.Add(ColNodeFilteredSkuCount);
                _valueMetrics.Columns.Add(ColAttributeSkuCount);
                _valueMetrics.Columns.Add(ColAttributeFillRate);
                _valueMetrics.Columns.Add(ColValueSkuCount);
                _valueMetrics.Columns.Add(ColValueFillRate);
                _valueMetrics.Columns.Add(ColNavigationOrder);
                _valueMetrics.Columns.Add(ColDisplayOrder);
            }

            if (_args.IncludeSkuMetrics)
            {
                _skuMetrics = new DataTable("SkuMetrics");
                _skuMetrics.Columns.Add(ColTaxonomyPath);
                for (var i = 0; i < _maxDepth; i++)
                    _skuMetrics.Columns.Add("T" + (i + 1));
                _skuMetrics.Columns.Add(ColItemId);
                _skuMetrics.Columns.Add(ColNodeAttributeCount);
                _skuMetrics.Columns.Add(ColNodeNavigationAttributeCount);
                _skuMetrics.Columns.Add(ColNodeDisplayAttributeCount);
                _skuMetrics.Columns.Add(ColSkuAttributeCount);
                _skuMetrics.Columns.Add(ColSkuNavigationAttributeCount);
                _skuMetrics.Columns.Add(ColSkuDisplayAttributeCount);
                _skuMetrics.Columns.Add(ColNodeAttributeFillRate);
                _skuMetrics.Columns.Add(ColNodeNavigationAttributeFillRate);
                _skuMetrics.Columns.Add(ColNodeDisplayAttributeFillRate);
            }
        }

        private void GenerateMetrics(Guid taxId)
        {
            using (var db = new AryaDbDataContext(_args.ProjectId, _args.UserId))
            {
                var node = db.TaxonomyInfos.First(ti => ti.ID == taxId);
                var nodeParts = node.ToStringParts().ToList();
                if (_args.IgnoreT1Taxonomy)
                    nodeParts.RemoveAt(0);

                // get list of SKUs for this taxonomy
                var allSkus = node.GetSkus(_args.ExportCrossListNodes);
                if (!allSkus.Any())
                    return;

                // filter skus based on sku inclusions and exclusions
                var filteredSkus = _args.GetFilteredSkuList(allSkus).ToList();
                var nodeSkuCount = filteredSkus.Count;

                var inSchemaSchematii = (from si in db.SchemaInfos
                                         where si.TaxonomyID == taxId
                                         let inSchema = (from sd in si.SchemaDatas where sd.Active select sd.InSchema).FirstOrDefault()
                                         where inSchema
                                         select si).ToList();

                if (_args.IncludeAttributeMetrics || _args.IncludeValueMetrics)
                {
                    foreach (var schemaInfo in inSchemaSchematii)
                    {
                        var attributeName = schemaInfo.Attribute.AttributeName;
                        var attributeSkuCount = filteredSkus.Count(sku => sku.HasAttribute(attributeName));
                        if (_args.IncludeAttributeMetrics)
                        {
                            var rowValues = GetDataRow(_attributeMetrics.NewRow(), node, nodeParts, attributeName,
                                nodeSkuCount, attributeSkuCount, string.Empty, string.Empty, 0, string.Empty, 0, 0, 0, 0,
                                0, 0);
                            _attributeMetrics.Rows.Add(rowValues);
                        }

                        if (_args.IncludeValueMetrics)
                        {
                            var uniquevalues = from sku in filteredSkus
                                from ed in sku.GetValuesForAttribute(db, schemaInfo.Attribute)
                                let entity = new {sku.ItemID, ed.Value, ed.Uom}
                                group entity by new {entity.Value, entity.Uom}
                                into grp
                                select
                                    new
                                    {
                                        grp.Key.Value,
                                        grp.Key.Uom,
                                        SkuCount = grp.Select(g => g.ItemID).Distinct().Count()
                                    };

                            foreach (var value in uniquevalues)
                            {
                                var rowValues = GetDataRow(_valueMetrics.NewRow(), node, nodeParts, attributeName,
                                    nodeSkuCount, attributeSkuCount, value.Value, value.Uom, value.SkuCount,
                                    string.Empty, 0, 0, 0, 0, 0, 0);
                                _valueMetrics.Rows.Add(rowValues);
                            }
                        }
                    }
                }

                if (_args.IncludeSkuMetrics)
                {
                    var nodeNavigationAttributeCount = inSchemaSchematii.Count(si => si.SchemaData.NavigationOrder > 0);
                    var nodeDisplayAttributeCount = inSchemaSchematii.Count(si => si.SchemaData.DisplayOrder > 0);

                    foreach (var fs in filteredSkus)
                    {
                        var sku = fs;
                        var skuAttributes =
                            inSchemaSchematii.Where(si => sku.HasAttribute(si.Attribute.AttributeName)).ToList();
                        var skuAttributeCount = skuAttributes.Count();
                        var skuNavigationAttributeCount = skuAttributes.Count(si => si.SchemaData.NavigationOrder > 0);
                        var skuDisplayAttributeCount = skuAttributes.Count(si => si.SchemaData.DisplayOrder > 0);
                        var rowValues = GetDataRow(_skuMetrics.NewRow(), node, nodeParts, string.Empty, nodeSkuCount, 0,
                            string.Empty, string.Empty, 0, sku.ItemID, inSchemaSchematii.Count,
                            nodeNavigationAttributeCount, nodeDisplayAttributeCount, skuAttributeCount,
                            skuNavigationAttributeCount, skuDisplayAttributeCount);
                        _skuMetrics.Rows.Add(rowValues);
                    }
                }
            }
        }

        private DataRow GetDataRow(DataRow dataRow, TaxonomyInfo node,
            IList<string> nodeParts, string attributeName, int nodeSkuCount, int attributeSkuCount, string value,
            string uom, int valueSkuCount, string itemId, int nodeAttributeCount, int nodeNavigationAttributeCount,
            int nodeDisplayAttributeCount, int skuAttributeCount, int skuNavigationAttributeCount,
            int skuDisplayAttributeCount)
        {
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                switch (column.ColumnName)
                {
                    case ColTaxonomyPath:
                        dataRow[column.ColumnName] = node.ToString(_args.IgnoreT1Taxonomy);
                        break;
                    case ColItemId:
                        dataRow[column.ColumnName] = itemId;
                        break;
                    case ColAttribute:
                        dataRow[column.ColumnName] = attributeName;
                        break;
                    case ColValue:
                        dataRow[column.ColumnName] = value;
                        break;
                    case ColUom:
                        dataRow[column.ColumnName] = uom;
                        break;
                    case ColNavigationOrder:
                        dataRow[column.ColumnName] =
                            node.SchemaInfos.First(si => si.Attribute.AttributeName == attributeName)
                                .SchemaData.NavigationOrder;
                        break;
                    case ColDisplayOrder:
                        dataRow[column.ColumnName] =
                            node.SchemaInfos.First(si => si.Attribute.AttributeName == attributeName)
                                .SchemaData.DisplayOrder;
                        break;
                    case ColNodeFilteredSkuCount:
                        dataRow[column.ColumnName] = nodeSkuCount;
                        break;
                    case ColAttributeSkuCount:
                        dataRow[column.ColumnName] = attributeSkuCount;
                        break;
                    case ColValueSkuCount:
                        dataRow[column.ColumnName] = valueSkuCount;
                        break;
                    case ColNodeAttributeCount:
                        dataRow[column.ColumnName] = nodeAttributeCount;
                        break;
                    case ColNodeNavigationAttributeCount:
                        dataRow[column.ColumnName] = nodeNavigationAttributeCount;
                        break;
                    case ColNodeDisplayAttributeCount:
                        dataRow[column.ColumnName] = nodeDisplayAttributeCount;
                        break;
                    case ColSkuAttributeCount:
                        dataRow[column.ColumnName] = skuAttributeCount;
                        break;
                    case ColSkuNavigationAttributeCount:
                        dataRow[column.ColumnName] = skuNavigationAttributeCount;
                        break;
                    case ColSkuDisplayAttributeCount:
                        dataRow[column.ColumnName] = skuDisplayAttributeCount;
                        break;
                    case ColAttributeFillRate:
                        dataRow[column.ColumnName] =string.Format("{0:N2}%",  attributeSkuCount * 100.00 / nodeSkuCount);
                        break;
                    case ColValueFillRate:
                        dataRow[column.ColumnName] =string.Format("{0:N2}%",  valueSkuCount * 100.00 / nodeSkuCount);
                        break;
                    case ColNodeAttributeFillRate:
                        dataRow[column.ColumnName] =string.Format("{0:N2}%",  skuAttributeCount * 100.00 / nodeAttributeCount);
                        break;
                    case ColNodeNavigationAttributeFillRate:
                        dataRow[column.ColumnName] =string.Format("{0:N2}%",  skuNavigationAttributeCount * 100.00 / nodeNavigationAttributeCount);
                        break;
                    case ColNodeDisplayAttributeFillRate:
                        dataRow[column.ColumnName] =string.Format("{0:N2}%",  skuDisplayAttributeCount * 100.00 / nodeDisplayAttributeCount);
                        break;
                    default:
                        try
                        {
                            int level;
                            if (!column.ColumnName.StartsWith("T") || !int.TryParse(column.ColumnName.Substring(1), out level))
                                continue;
                            dataRow[column.ColumnName] = nodeParts.Count >= level ? nodeParts[level - 1] : string.Empty;
                        }
                        catch (Exception)
                        {
                            //Do Nothing - I wonder why control got here...                                            
                        }
                        break;
                }
            }
            return dataRow;
        }
    }
    
    [Serializable]
    public class MetricsExportArgs : AdvancedExportArgs
    {
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 31)]
        [DisplayName(@"Export Attribute Metrics")]
        [Description("If YES, the export will produce an attribute level metric report, which includes fill rates for each attribute per taxonomy node. If NO, the attribute level report will not be produced.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeAttributeMetrics { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 32)]
        [DisplayName(@"Export Value Metrics")]
        [Description("If YES, the export will produce Value level metric report. which includes how often a value appears in each attribute per node. If NO, the Value level report will not be produced.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeValueMetrics { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 33)]
        [DisplayName(@"Export Sku Metrics")]
        [Description("If YES, the export will produce a SKU level metric report, which includes fill rates for each SKU (i.e. the percentage of navigation or display values that are filled in for each SKU.) If NO, the SKU level report will not be produced.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeSkuMetrics { get; set; }

        public MetricsExportArgs()
        {
            HiddenProperties += "MarkAsPublished" + "ExportExtendedAttributes";
        }
    }
}