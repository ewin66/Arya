using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.Properties;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Exports
{
    [Serializable]
    [DisplayName(@"Schema")]
    public class ExportWorkerForSchema : ExportWorkerBase
    {
        #region Fields

        private const string SchemaExportTableName = "SchemaExportData";
        private readonly List<string> _possibleMissingInSchemaAttributes = new List<string>();

        private SchemaExportArgs _args;
        private int _maxTaxonomyLength;
        private DataTable _schemaExportData;
        public List<string> _taxColumns = new List<string>();

        #endregion Fields

        #region Constructors

        public ExportWorkerForSchema(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof (SchemaExportArgs))
        {
        }

        #endregion Constructors

        #region Properties

        private SchemaExportArgs Args
        {
            get
            {
                if (_args == null)
                    _args = (SchemaExportArgs) Arguments;
                return _args;
            }
        }

        private int MaxTaxonomyLength
        {
            get
            {
                if (_maxTaxonomyLength == 0)
                {
                    using (var dc = new AryaDbDataContext(Args.ProjectId, Args.UserId))
                        _maxTaxonomyLength = GetMaxTaxonomyLength(dc);
                   
                }
              
                return _maxTaxonomyLength;
            }
        }

        private DataTable SchemaExportData
        {
            get
            {
                if (_schemaExportData == null)
                    _schemaExportData = GetInitialDataTable();
                return _schemaExportData;
            }
        }

        #endregion Properties

        #region Methods

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        public override List<string> ValidateInput()
        {
            var baseErrors = base.ValidateInput();
            return baseErrors;
        }

        protected override void FetchExportData()
        {
            try
            {
                StatusMessage = string.Format("Schema Export worker has started");
                var exportTaxonomyIds = new List<Guid>();
                using (var dc = new AryaDbDataContext(Args.ProjectId, Args.UserId))
                {
                    StatusMessage = string.Format("Fetching child taxonomies");
                    var taxs =
                        (from tax in dc.TaxonomyInfos where Args.TaxonomyIds.Contains(tax.ID) select tax).ToList();
                    exportTaxonomyIds.AddRange(taxs.Select(tx => tx.ID));
                    exportTaxonomyIds.AddRange(taxs.SelectMany(ti => ti.AllChildren).Select(tax => tax.ID).ToList());
                }
                foreach (var exportTaxonomyId in exportTaxonomyIds)
                    AddTaxonomyToDataTable(exportTaxonomyId);
                ExportDataTables.Add(SchemaExportData);
                State = WorkerState.Complete;
                StatusMessage = string.Format("Schema Export worker completed");
            }
            catch (Exception ex)
            {
                Summary.SetError(ex);
            }
        }

        private void AddTaxonomyToDataTable(Guid exportTaxonomyId)
        {
            using (var dc = new AryaDbDataContext(Args.ProjectId, Args.UserId))
            {
                var taxonomyInfo = dc.TaxonomyInfos.FirstOrDefault(ti => ti.ID == exportTaxonomyId);
                if (taxonomyInfo != null)
                {
                    var taxPath = taxonomyInfo.ToString();
                    if (Args.IgnoreT1Taxonomy)
                        taxPath = taxonomyInfo.ToString(true);// taxPath.Substring(taxPath.IndexOf('>') + 1);

                    if (string.IsNullOrEmpty(taxPath))
                        return;

                    var attributes = GetAttributes(dc, taxonomyInfo);

                    var isLeafNode = taxonomyInfo.IsLeafNode;

                    //if a node has no schema, just skip it.
                    if ((attributes.Count == 0) || (Args.LeafNodesOnly && isLeafNode == false))
                    {
                        //StatusMessage = string.Format("{1}{0}, Fetching Child Nodes", Environment.NewLine, exportTaxonomyId);
                        //var taxonomies =
                        //    taxonomyInfo.ChildTaxonomyDatas.Where(td => td.Active)
                        //                    .Select(td => td.TaxonomyInfo)
                        //                    .OrderBy(tax => tax.ToString());

                        //foreach (var tax in taxonomies)
                        //    AddTaxonomyToDataTable(tax.ID);
                        //CurrentProgress++;
                        return;
                    }

                    foreach (var attribute in attributes)
                    {
                        var schemaInfo = GetSchemaInfo(taxonomyInfo, attribute);
                        if (schemaInfo == null)
                            continue;

                        var row = SchemaExportData.NewRow();
                        foreach (DataColumn metaAttributeColumn in SchemaExportData.Columns)
                        {
                            var cellValue = GetColumnValue(dc, schemaInfo, metaAttributeColumn.ColumnName);
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                row[metaAttributeColumn] = cellValue;
                                continue;
                            }
                            row[metaAttributeColumn] = string.Empty;
                        }
                        SchemaExportData.Rows.Add(row);
                    }
                } //end of if (taxonomyInfo != null)
            } //end of using
        }

        private List<string> GetAllSchemaMetaAttributes()
        {
            // List<SchemaAttribute> allSchemati = new List<SchemaAttribute>();
            var emptyGuid = new Guid();
            var metaAttributes = new List<SchemaAttribute>();
            if (Args.ExportEnrichments || Args.ExportMetaAttributes)
            {
                metaAttributes = (from att in CurrentDb.Attributes
                    where
                        att.ProjectID.Equals(Args.ProjectId)
                        && att.AttributeType.Equals(AttributeTypeEnum.SchemaMeta.ToString())
                        && att.SchemaInfos.Any(si => si.SchemaDatas.Any(sd => sd.InSchema && sd.Active))
                    let displayRank = (from si in att.SchemaInfos
                        where si.TaxonomyID.Equals(emptyGuid)
                        from sd in si.SchemaDatas
                        where sd.Active
                        select sd.DisplayOrder).FirstOrDefault()
                    orderby displayRank, att.AttributeName
                    select att).Select(att => new SchemaAttribute(att, typeof (string))).ToList();
                //remove enrichment
                if (!Args.ExportEnrichments)
                {
                    metaAttributes =
                        metaAttributes.Where(
                            s =>
                                !(s.AttributeType == SchemaAttribute.SchemaAttributeType.Meta
                                  && s.MetaSchemaAttribute.AttributeName.Contains("Enrichment"))).ToList();
                }
                if (!Args.ExportMetaAttributes)
                {
                    metaAttributes =
                        metaAttributes.Where(
                            s =>
                                s.AttributeType != SchemaAttribute.SchemaAttributeType.Meta
                                || s.MetaSchemaAttribute.AttributeName.Contains("Enrichment")).ToList();
                }
                //remove export
            }

            //TODO: Ask Vivek what is the new primary attributes getter
            // var schemati =
            //    SchemaAttribute.PrimarySchemati.Where(
            //        p => p != SchemaAttribute.SchemaAttributeIsMapped && p != SchemaAttribute.SchemaAttributeInSchema);
            //if (ExportFillRates)
            //    schemati = schemati.Union(SchemaAttribute.FillRateSchemati);
            //allSchemati =
            //    schemati.Union(metaAttributes).ToList();
            ////var schemati =
            ////    SchemaDataGridView.SchematiPrimary.Where(
            ////        p => p != SchemaAttribute.SchemaAttributeIsMapped && p != SchemaAttribute.SchemaAttributeInSchema);
            ////if ( Args.ExportFillRates)
            ////    schemati = schemati.Union(SchemaAttribute.FillRateSchemati);
            ////allSchemati =
            ////    schemati.Union(metaAttributes).ToList();
            return metaAttributes.Select(me => me.ToString()).ToList();
        }

        private List<Attribute> GetAttributes(AryaDbDataContext dc, TaxonomyInfo currentTaxonomyInfo)
        {
            var attributes = new List<Attribute>();
            if (Args.LeafNodesOnly && !currentTaxonomyInfo.IsLeafNode)
                return attributes;
            //if (!Args.ExportEmptyNodes && !currentTaxonomyInfo.GetSkus(Args.ExportCrossListNodes).Any())
            //if (!(Args.ExportEmptyNodes || currentTaxonomyInfo.GetSkus(Args.ExportCrossListNodes).Any(t => t.SkuType == "Product")))
            if (currentTaxonomyInfo.GetSkus(Args.ExportCrossListNodes).Any(t => t.SkuType == "Product") || Args.ExportEmptyNodes)
            {
                if (Args.ExportSuperSchema && !currentTaxonomyInfo.IsLeafNode)
                {
                    var schemaInfos = new List<SchemaInfo>();
                    schemaInfos.AddRange(
                        currentTaxonomyInfo.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());
                    if (Args.ExportSuperSchema)
                    {
                        foreach (var leaf in currentTaxonomyInfo.AllLeafChildren)
                            schemaInfos.AddRange(leaf.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());
                    }
                    var schemaAttributes =
                        schemaInfos.Where(p => p.SchemaData.InSchema).Select(p => new { p.Attribute, p.SchemaData }).ToList();

                    attributes = (from att in schemaAttributes
                                  group att by att.Attribute
                                      into grp
                                      let minRank = grp.Min(p => GetRank(p.SchemaData, SortOrder.OrderbyDisplayNavigation))
                                      orderby minRank
                                      select grp.Key).Distinct().ToList();
                }
                else
                {
                    attributes = (from si in dc.SchemaInfos
                                  where si.TaxonomyID == currentTaxonomyInfo.ID
                                  let sd = si.SchemaDatas.FirstOrDefault(sd => sd.Active)
                                  where sd != null
                                  let inSchema = sd.InSchema
                                  where inSchema
                                  let navRank = sd == null || sd.NavigationOrder == 0 ? decimal.MaxValue : sd.NavigationOrder
                                  let dispRank = sd == null || sd.DisplayOrder == 0 ? decimal.MaxValue : sd.DisplayOrder
                                  orderby navRank, dispRank
                                  select si.Attribute).ToList();

                    var currentTaxonomyString = currentTaxonomyInfo.ToString();

                    var possibleMissingAttributes = (from si in CurrentDb.SchemaInfos
                                                     where si.TaxonomyID == currentTaxonomyInfo.ID
                                                     let sd = si.SchemaDatas.FirstOrDefault(sd => sd.Active)
                                                     where sd != null && !sd.InSchema && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                     select si.Attribute.AttributeName).ToList().Select(p => currentTaxonomyString + "\t" + p).ToList();

                    _possibleMissingInSchemaAttributes.AddRange(possibleMissingAttributes);
                }
                return attributes;
            }

            

            return attributes;
        }

        private string GetColumnValue(TaxonomyInfo taxonomy, string columnName)
        {
            if (columnName == "Taxonomy")
            {
                if (Args.IgnoreT1Taxonomy)
                {
                    return taxonomy.ToString(true);
                }
                else
                    return taxonomy.ToString();
            }
            if (columnName == "NodeType")
                return (taxonomy.NodeType == "Derived") ? "Cross List" : TaxonomyInfo.NodeTypeRegular;
            return GetTn(taxonomy, columnName);
        }

        private string GetColumnValue(AryaDbDataContext dc, SchemaInfo schema, string columnName)
        {
            var value = string.Empty;
            if (_taxColumns.Contains(columnName))
                value = GetColumnValue(schema.TaxonomyInfo, columnName);
            else if (String.CompareOrdinal("Attribute", columnName) == 0)
                value = schema.Attribute.AttributeName;
            else
            {
                value = SchemaAttribute.GetMetaAttributeValue(schema.Attribute,
                    Attribute.GetAttributeFromName(CurrentDb, columnName, false, AttributeTypeEnum.SchemaMeta),
                    schema.TaxonomyInfo);

                if (String.Compare(Resources.SchemaEnrichmentImageAttributeName, columnName,
                    StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var imageMgr = new ImageManager(dc, Args.ProjectId, value)
                                   {
                                       LocalDirectory = ArgumentDirectoryPath,
                                       RemoteImageGuid = value
                                   };
                    //if (!Directory.Exists(imageMgr.LocalDirectory))
                    //    Directory.CreateDirectory(imageMgr.LocalDirectory);

                    if (Args.DownloadAssets)
                    {
                        imageMgr.DownloadImage(schema.TaxonomyInfo.ID, schema.Attribute.ID);

                    }
                    
                    value = imageMgr.OriginalFileName;
                }
            }
            return value;
        }

        private DataTable GetInitialDataTable()
        {
            var dataTable = new DataTable(SchemaExportTableName);
            //_taxColumns = new List<string>();
            //get the max value for Tn and insert into _taxColumns
            _taxColumns.Add("Taxonomy");
            
            for (var i = 1; i <= MaxTaxonomyLength; i++)
                _taxColumns.Add("T" + i);
            _taxColumns.Add("NodeType");
            _taxColumns.ForEach(c => dataTable.Columns.Add(c));
            dataTable.Columns.Add("Attribute");
            dataTable.Columns.AddRange(
                GetAllSchemaMetaAttributes().Select(schematus => new DataColumn(schematus.ToString())).ToArray());
            return dataTable;
        }

        private SchemaInfo GetSchemaInfo(TaxonomyInfo currentTaxInfo, Attribute attributeName)
        {
            var schemaInfo =
                currentTaxInfo.SchemaInfos.FirstOrDefault(
                    si => si.Attribute.AttributeName == attributeName.AttributeName);
            if (schemaInfo != null)
                return schemaInfo;

            var parentTaxonomyInfo = currentTaxInfo.TaxonomyData.ParentTaxonomyInfo;
            if (Args.ExportSuperSchema && parentTaxonomyInfo != null)
                return GetSchemaInfo(parentTaxonomyInfo, attributeName);

            return null;
        }
        readonly Regex _rxCol = new Regex("\\d+$");
        private string GetTn(TaxonomyInfo taxonomy, string columnName)
        {
            if (!_rxCol.IsMatch(columnName))
                return string.Empty;

            var col = _rxCol.Match(columnName).Value;
            var resultInt = int.Parse(col);
            var taxParts = taxonomy.ToStringParts().ToList();
            if (Args.IgnoreT1Taxonomy)
                taxParts.RemoveAt(0);
            return taxParts.Count >= resultInt ? taxParts[resultInt - 1] : string.Empty;
            //int resultInt;
            //if (int.TryParse(columnName.Substring(columnName.Length - 1, 1), out resultInt)) //comupute n
            //{
            //    var taxParts = taxonomy.ToString().Split(TaxonomyInfo.DELIMITER[0]);
            //    if (taxParts.Count() >= resultInt)
            //        return taxParts[resultInt - 1];
            //}
            //return string.Empty;
        }

        #endregion Methods
    }

    [Serializable]
    public class SchemaExportArgs : ExportArgs
    {
        #region Fields

        private bool _leafNodesOnly;

        #endregion Fields

        #region Constructors

        #endregion Constructors

        #region Properties

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 7)]
        [DisplayName(@"Download Enrichments")]
        [Description(" If YES, the export will produce a zipped folder of the enrichment images associated with the schema, taxonomy or LOV export and the enrichment attributes (file names and copy) will be included in the export file. If NO, the enrichments will not be downloadable.")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool DownloadAssets { get; set; }

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 6)]
        [DisplayName(@"Export Enrichments")]
        [Description("If YES, the enrichment information (image names and copy) will be included in the export file. If NO, the enrichment information will not be included. Note that this option exports only the name of the image file, not the image file itself.  (See 'Download Enrichments' for exporting images files.)")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportEnrichments { get; set; }

        [DefaultValue(true)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Export Project Meta-Attributes")]
        [Description("If YES, all the project meta-attributes are included in the export. If NO, only the primary meta-attributes (ranks & data type) will be exported.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportMetaAttributes { get; set; }

        //To be added leater
        //[DefaultValue(false), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 2),
        // DisplayName(@"Export Fill Rates")]
        //[TypeConverter(typeof(BooleanToYesNoConverter))]
        //public bool ExportFillRates { get; set; }
        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 3)]
        [DisplayName(@"Export Super Schema")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportSuperSchema { get; set; }

        //[DefaultValue(true), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 4),
        // DisplayName(@"Export All Schema Nodes")]
        //[TypeConverter(typeof(BooleanToYesNoConverter))]
        //public bool ExportAllSchemaNodes { get; set; }
        //[DefaultValue(true)]
        //[Category(CaptionOptional)]
        //[PropertyOrder(OptionalBaseOrder + 4)]
        //[Description("If YES, all nodes regardless of whether they have SKUs or not will be included in the export file. If NO, nodes without SKUs will not be included in the export.")]
        //[DisplayName(@"Export Nodes Without SKUs")]
        //[TypeConverter(typeof (BooleanToYesNoConverter))]
        //public bool IgnoreEmptyNodes { get; set; }


        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 3)]
        [Description("If YES, all nodes regardless of whether they have SKUs or not will be included in the export file. If NO, nodes without SKUs will not be included in the export.")]
        [DisplayName(@"Export Nodes Without SKUs")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportEmptyNodes { get; set; }


        [DefaultValue(true)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 2)]
        [Description("If YES, only leaf nodes (nodes with no immediate child nodes) will be exported. If NO, leaf nodes and parent nodes will be exported. (e.g. taxonomy node enrichments on a parent node.)")]
        [DisplayName(@"Export Leaf Nodes Only")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool LeafNodesOnly
        {
            get { return _leafNodesOnly; }
            set { _leafNodesOnly = value; }
        }

        #endregion Properties
    }
}