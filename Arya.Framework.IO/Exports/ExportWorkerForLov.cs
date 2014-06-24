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
using Arya.Framework.IO.Properties;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"List of Values")]
    public class ExportWorkerForLov : ExportWorkerBase
    {
        #region Fields

        private const string LovTableName = "LovExportData";
        private readonly List<string> _taxColumns = new List<string>();

        private LovExportArgs _args;
        private DataTable _lovExportData;
        private int _maxTaxonomyLength;

        #endregion Fields

        #region Constructors

        public ExportWorkerForLov(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof(LovExportArgs))
        {
        }

        #endregion Constructors

        #region Properties

        private LovExportArgs Args
        {
            get
            {
                if (_args == null)
                    _args = (LovExportArgs)Arguments;
                return _args;
            }
        }

        private DataTable LovExportData
        {
            get { return _lovExportData ?? (_lovExportData = GetInitialDataTable()); }
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

        #endregion Properties

        #region Methods

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        protected override void FetchExportData()
        {
            try
            {
                StatusMessage = string.Format("Lov Export worker has started");
                var exportTaxonomyIds = new List<Guid>();
                using (var dc = new AryaDbDataContext(Args.ProjectId, Args.UserId))
                {
                    var taxs =
                        (from tax in dc.TaxonomyInfos where Args.TaxonomyIds.Contains(tax.ID) select tax).ToList();
                    exportTaxonomyIds.AddRange(taxs.Select(tx => tx.ID));
                    exportTaxonomyIds.AddRange(taxs.SelectMany(ti => ti.AllChildren).Select(tax => tax.ID).ToList());
                }

                foreach (var exportTaxonomyId in exportTaxonomyIds)
                    AddTaxonomyToDataTable(exportTaxonomyId);
                ExportDataTables.Add(LovExportData);
                State = WorkerState.Complete;
                StatusMessage = string.Format("Lov Export worker completed");
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
                    var attributes = GetAttributes(dc, taxonomyInfo);
                    foreach (var attribute in attributes)
                    {
                        var schemaData =
                            attribute.SchemaInfos.Where(p => p.TaxonomyID == taxonomyInfo.ID && p.SchemaData != null)
                                .Select(p => p.SchemaData)
                                .FirstOrDefault();

                        if (schemaData != null)
                        {
                            // var attribute1 = attribute;

                            foreach (var lov in schemaData.SchemaInfo.ListOfValues.Where(lov => lov.Active))
                            {
                                var row = LovExportData.NewRow();
                                foreach (DataColumn lovExportColumn in LovExportData.Columns)
                                {
                                    if (lovExportColumn.ColumnName == "NodeType")
                                    {
                                        row[lovExportColumn] = (taxonomyInfo.NodeType == "Derived")
                                            ? "Cross List"
                                            : TaxonomyInfo.NodeTypeRegular;
                                    }
                                    else if (lovExportColumn.ColumnName == "AttributeName")
                                        row[lovExportColumn] = attribute.AttributeName;
                                    else if (lovExportColumn.ColumnName == "NavigationOrder")
                                    {
                                        row[lovExportColumn] = schemaData.NavigationOrder == 0
                                            ? string.Empty
                                            : string.Format("{0:0.##}", schemaData.NavigationOrder);
                                    }
                                    else if (lovExportColumn.ColumnName == "DisplayOrder")
                                    {
                                        row[lovExportColumn] = schemaData.DisplayOrder == 0
                                            ? string.Empty
                                            : string.Format("{0:0.##}", schemaData.DisplayOrder);
                                    }
                                    else if (lovExportColumn.ColumnName == "Value")
                                        row[lovExportColumn] = lov.Value;
                                    else if (lovExportColumn.ColumnName.StartsWith("T"))
                                        row[lovExportColumn] = GetTn(taxonomyInfo, lovExportColumn.ColumnName);
                                    
                                    else if (lovExportColumn.ColumnName == "EnrichmentImage"
                                             || lovExportColumn.ColumnName == "EnrichmentCopy")
                                    {
                                        if (Args.ExportEnrichments)
                                        {
                                            if (lovExportColumn.ColumnName == "EnrichmentImage")
                                            {
                                                var imageMgr = new ImageManager(dc, Args.ProjectId,
                                                    lov.EnrichmentImage)
                                                               {
                                                                   LocalDirectory = ArgumentDirectoryPath,
                                                                   RemoteImageGuid = lov.EnrichmentImage
                                                               };
                                                //if (!Directory.Exists(imageMgr.LocalDirectory))
                                                //    Directory.CreateDirectory(imageMgr.LocalDirectory);
                                                string lovImage;
                                                if (Args.DownloadAssets)
                                                {
                                                    imageMgr.DownloadImage(taxonomyInfo.ID, attribute.ID, lov.ID);
                                                    //lovImage = lov.EnrichmentImage;
                                                    lovImage = imageMgr.OriginalFileName;
                                                }
                                                else
                                                    lovImage = imageMgr.OriginalFileName;
                                                row["EnrichmentImage"] = lovImage;
                                            }
                                            else if (lovExportColumn.ColumnName == "EnrichmentCopy")
                                                row["EnrichmentCopy"] = lov.EnrichmentCopy;
                                        }
                                        else // this should not happen
                                            throw new Exception("The Lov export column is not configured");
                                    }
                                } //end of lov column  loop
                                LovExportData.Rows.Add(row);
                            } //end of for
                        } //endof if
                    }
                }
            }
        }

        private IEnumerable<Attribute> GetAttributes(AryaDbDataContext dc, TaxonomyInfo currentTaxonomyInfo)
        {
            var attributes = new List<Attribute>();
            if (Args.LeafNodesOnly && !currentTaxonomyInfo.IsLeafNode)
                return attributes;
            if (currentTaxonomyInfo.GetSkus(Args.ExportCrossListNodes).Any(t => t.SkuType == "Product") || Args.ExportEmptyNodes)
            {
                if (!currentTaxonomyInfo.IsLeafNode)
                {
                    var schemaInfos = new List<SchemaInfo>();
                    schemaInfos.AddRange(
                        currentTaxonomyInfo.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());

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
                }
                return attributes;
            }
            
          
            return attributes;
        }

        //TODO: make it generic, move it to parent
        private DataTable GetInitialDataTable()
        {
            var dataTable = new DataTable(LovTableName);
            //var taxColumns = new List<string>();
            //get the max value for Tn and insert into _taxColumns
             _taxColumns.Add("Taxonomy");
            for (var i = 1; i <= MaxTaxonomyLength; i++)
                _taxColumns.Add("T" + i);
            _taxColumns.ForEach(c => dataTable.Columns.Add(c));

            dataTable.Columns.AddRange(
                new List<DataColumn>
                {
                    new DataColumn("NodeType"),
                    new DataColumn("AttributeName"),
                    new DataColumn("NavigationOrder"),
                    new DataColumn("DisplayOrder"),
                    new DataColumn("Value")
                }.ToArray());
            if (Args.ExportEnrichments)
            {
                dataTable.Columns.AddRange(
                    new List<DataColumn> { new DataColumn("EnrichmentImage"), new DataColumn("EnrichmentCopy") }.ToArray());
            }
            return dataTable;
        }


        readonly Regex _rxCol = new Regex("\\d+$");
        private string GetTn(TaxonomyInfo taxonomy, string columnName)
        {
            if (columnName == "Taxonomy")
                return taxonomy.ToString(Args.IgnoreT1Taxonomy);
            if (!_rxCol.IsMatch(columnName))
                return string.Empty;

            var col = _rxCol.Match(columnName).Value;
            var resultInt = int.Parse(col);
            var taxParts = taxonomy.ToStringParts().ToList();
            if (Args.IgnoreT1Taxonomy)
                taxParts.RemoveAt(0);
            return taxParts.Count >= resultInt ? taxParts[resultInt - 1] : string.Empty;
        }

        #endregion Methods
    }

    [Serializable]
    public class LovExportArgs : ExportArgs
    {
        #region Fields

        private bool _leafNodesOnly;

        #endregion Fields

        #region Constructors

        #endregion Constructors

        #region Properties

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 6)]
        [Description(" If YES, the export will produce a zipped folder of the enrichment images associated with the schema, taxonomy or LOV export and the enrichment attributes (file names and copy) will be included in the export file. If NO, the enrichments will not be downloadable.")]
        [DisplayName(@"Download Enrichments")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool DownloadAssets { get; set; }

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Export Enrichments")]
        [Description("If YES, the enrichment information (image names and copy) will be included in the export file. If NO, the enrichment information will not be included. Note that this option exports only the name of the image file, not the image file itself.  (See 'Download Enrichments' for exporting images files.)")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportEnrichments { get; set; }

        //[DefaultValue(false)]
        //[Category(CaptionOptional)]
        //[PropertyOrder(OptionalBaseOrder + 3)]
        //[Description("If YES, all nodes regardless of whether they have SKUs or not will be included in the export file. If NO, nodes without SKUs will not be included in the export.")]
        //[DisplayName(@"Export Nodes Without SKUs")]
        //[TypeConverter(typeof(BooleanToYesNoConverter))]
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
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool LeafNodesOnly
        {
            get { return _leafNodesOnly; }
            set { _leafNodesOnly = value; }
        }

        #endregion Properties
    }
}