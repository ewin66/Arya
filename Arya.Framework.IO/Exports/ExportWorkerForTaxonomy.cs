using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    [DisplayName(@"Taxonomy")]
    public class ExportWorkerForTaxonomy : ExportWorkerBase
    {
        #region Fields

        private const string TaxonomyExportTableName = "TaxonomyExportData";
        private readonly List<string> _taxColumns = new List<string>();

        private Dictionary<string, Attribute> _allTaxMetaAttributes;
        private TaxonomyExportArgs _args;
        private int _maxTaxonomyLength;
        private DataTable _taxonomyExportData;

        #endregion Fields

        #region Constructors

        public ExportWorkerForTaxonomy(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof (TaxonomyExportArgs))
        {
        }

        #endregion Constructors

        #region Properties

        private Dictionary<string, Attribute> AllTaxMetaAttributes
        {
            get { return _allTaxMetaAttributes ?? (_allTaxMetaAttributes = GetAllTaxMetaAttributes()); }
        }

        private TaxonomyExportArgs Args
        {
            get { return _args ?? (_args = (TaxonomyExportArgs) Arguments); }
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

        private DataTable TaxonomyExportData
        {
            get { return _taxonomyExportData ?? (_taxonomyExportData = GetInitialDataTable()); }
        }

        #endregion Properties

        #region Methods

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        protected override void FetchExportData()
        {
            try
            {
                StatusMessage = string.Format("Taxonomy Export worker has started");
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
                ExportDataTables.Add(TaxonomyExportData);
                State = WorkerState.Complete;
                StatusMessage = string.Format("Taxonomy Export worker completed");
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
                var currentTaxonomyInfo = dc.TaxonomyInfos.FirstOrDefault(ti => ti.ID == exportTaxonomyId);
                
                // check this taxonomy against arguments and filter accordingly
                if (currentTaxonomyInfo == null)
                    return;
                if (!Args.ExportCrossListNodes && currentTaxonomyInfo.NodeType == TaxonomyInfo.NodeTypeDerived)
                    return;
                if (Args.LeafNodesOnly && !currentTaxonomyInfo.IsLeafNode)
                    return;
                //if (!(Args.ExportEmptyNodes || currentTaxonomyInfo.HasSkus))
                if ( !currentTaxonomyInfo.GetSkus(Args.ExportCrossListNodes).Any(t => t.SkuType == "Product") && !Args.ExportEmptyNodes) 
                    return;
                
                // fill data row
                var row = TaxonomyExportData.NewRow();
                foreach (DataColumn metaAttribute in TaxonomyExportData.Columns)
                {
                    var cellValue = GetColumnValue(dc, currentTaxonomyInfo, metaAttribute.ColumnName);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        row[metaAttribute] = cellValue;
                        continue;
                    }
                    row[metaAttribute] = string.Empty;
                }
                TaxonomyExportData.Rows.Add(row);
            }
        }

        private Dictionary<string, Attribute> GetAllTaxMetaAttributes()
        {
            var emptyGuid = new Guid();
            var metaAttributes = new List<Attribute>();
            List<Attribute> activeMetaAttributes = new List<Attribute>();
            //var metaAttributes = new List<Attribute>();
            if (Args.ExportEnrichments || Args.ExportMetaAttributes)
            {
                using (var dc = new AryaDbDataContext(Args.ProjectId, Args.UserId))
                {
                    metaAttributes = (from att in dc.Attributes
                        where
                            att.ProjectID.Equals(dc.CurrentProject.ID)
                            && att.AttributeType.Equals(AttributeTypeEnum.TaxonomyMeta.ToString())
                            && att.SchemaInfos.Any(si => si.SchemaDatas.Any(sd => sd.Active))
                        let displayRank = (from si in att.SchemaInfos
                            where si.TaxonomyID.Equals(emptyGuid)
                            from sd in si.SchemaDatas
                            where sd.Active
                            select sd.DisplayOrder).FirstOrDefault()
                        orderby displayRank, att.AttributeName
                        select att).ToList();
                    
                    foreach (var metaAttribute in metaAttributes)
                    {
                        var si = metaAttribute.SchemaInfos.FirstOrDefault(s => s.TaxonomyID.Equals(Guid.Empty));
                        if (si != null && si.SchemaData.InSchema)
                        {
                            activeMetaAttributes.Add(metaAttribute);
                        }
                        
                    }
                    //remove enrichment
                    if (!Args.ExportEnrichments)
                        activeMetaAttributes = activeMetaAttributes.Where(s => !Attribute.TaxonomyEnrichmentAttributes.Contains(s.AttributeName)).ToList();
                    if (!Args.ExportMetaAttributes)
                    {
                        activeMetaAttributes =
                            activeMetaAttributes.Where(
                                s =>
                                    s.AttributeType != AttributeTypeEnum.TaxonomyMeta.ToString()
                                    || Attribute.TaxonomyEnrichmentAttributes.Contains(s.AttributeName)).ToList();
                    }
                    //metaAttributeNames = metaAttributeNames.Select(ta => ta.AttributeName).ToList();
                } //end of using
            }
            return activeMetaAttributes.ToDictionary(ma => ma.AttributeName, ma => ma);
        }

        private string GetColumnValue(AryaDbDataContext dc, TaxonomyInfo taxonomyInfo, string columnName)
        {
            if (_taxColumns.Contains(columnName))
            {
                if (columnName == "Taxonomy")
                {
                    return taxonomyInfo.ToString(Args.IgnoreT1Taxonomy);
                }                
                return GetTn(taxonomyInfo, columnName);
            }
            if (columnName == "NodeDescription")
               return taxonomyInfo.TaxonomyData.NodeDescription;
            return GetTaxMetaAttributeValue(dc, columnName, taxonomyInfo);
        }

        //TODO: make it generic, move it to parent
        private DataTable GetInitialDataTable()
        {
            var dataTable = new DataTable(TaxonomyExportTableName);
            //var taxColumns = new List<string>();
            //get the max value for Tn and insert into _taxColumns
            _taxColumns.Add("Taxonomy");
            for (var i = 1; i <= MaxTaxonomyLength; i++)
            {
                _taxColumns.Add("T" + i);
            }
                
            _taxColumns.ForEach(c => dataTable.Columns.Add(c));
            dataTable.Columns.Add("NodeDescription");
            dataTable.Columns.AddRange(
                AllTaxMetaAttributes.Keys.Select(taxMetaAttName => new DataColumn(taxMetaAttName)).ToArray());
            return dataTable;
        }

        private string GetTaxMetaAttributeValue(AryaDbDataContext dc, string taxMetaAttributeName,
            TaxonomyInfo taxonomy)
        {
            var taxMetaData =
                (dc.TaxonomyMetaDatas.FirstOrDefault(
                    tmd =>
                        tmd.TaxonomyMetaInfo.TaxonomyID.Equals(taxonomy.ID)
                        && tmd.TaxonomyMetaInfo.MetaAttributeID.Equals(AllTaxMetaAttributes[taxMetaAttributeName].ID)
                        && tmd.Active));
            string value = taxMetaData == null ? string.Empty : taxMetaData.Value;
            if (String.Compare(taxMetaAttributeName, Resources.TaxonomyEnrichmentImageAttributeName,
                StringComparison.OrdinalIgnoreCase) == 0)
            {               
                    var imageMgr = new ImageManager(dc, Args.ProjectId, value)
                                   {
                                       LocalDirectory =ArgumentDirectoryPath,
                                       RemoteImageGuid = value
                                   };
                    //if (!Directory.Exists(imageMgr.LocalDirectory))
                    //    Directory.CreateDirectory(imageMgr.LocalDirectory);
                    if (Args.DownloadAssets)
                    {
                    imageMgr.DownloadImage(taxonomy.ID);
                    
                    }
                    value = imageMgr.OriginalFileName;
            }
            return value;
        }

        readonly Regex _rxCol = new Regex("\\d+$");

        private string GetTn(TaxonomyInfo taxonomy, string columnName)
        {
            if (!_rxCol.IsMatch(columnName))
                return string.Empty;

            var col = _rxCol.Match(columnName).Value;
            var resultInt = int.Parse(col);
            var taxParts = taxonomy.ToStringParts().ToList();
            if(Args.IgnoreT1Taxonomy)
                taxParts.RemoveAt(0);
            return taxParts.Count >= resultInt ? taxParts[resultInt - 1] : string.Empty;
        }

        //private string GetTn(TaxonomyInfo taxonomy, string columnName)
        //{
        //    int resultInt;
        //    if (int.TryParse(columnName.Substring(columnName.Length - 1, 1), out resultInt)) //comupute n
        //    {
        //        string[] taxParts;
        //        if (Args.IgnoreT1Taxonomy)
        //        {
        //            taxParts = taxonomy.ToString(true).Split(TaxonomyInfo.DELIMITER[0]);
        //        }
        //        else
        //        {
        //            taxParts = taxonomy.ToString().Split(TaxonomyInfo.DELIMITER[0]);
        //        }
                 
        //        if (taxParts.Count() >= resultInt)
        //        {
        //            var taxNodeName = taxParts[resultInt - 1];
        //            string taxonomyData;
        //            if(taxonomy.NodeType != "Regular")
        //                 taxonomyData =  taxonomy.TaxonomyData.NodeName.ToString();
        //            if (taxNodeName.Contains("(CL)"))
        //                return taxonomy.TaxonomyData.NodeName;
        //            else
        //            {
        //                return taxParts[resultInt - 1];
        //            }
        //        }
                    
        //    }
        //    return string.Empty;
        //}

        #endregion Methods
    }

    [Serializable]
    public class TaxonomyExportArgs : ExportArgs
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
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool DownloadAssets { get; set; }

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Export Enrichments")]
        [Description("If YES, the enrichment information (image names and copy) will be included in the export file. If NO, the enrichment information will not be included. Note that this option exports only the name of the image file, not the image file itself.  (See 'Download Enrichments' for exporting images files.)")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportEnrichments { get; set; }

        [DefaultValue(true)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 4)]
        [DisplayName(@"Export Project Meta-Attributes")]
        [Description("If YES, all the project meta-attributes are included in the export. If NO, only the primary meta-attributes (ranks & data type) will be exported.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportMetaAttributes { get; set; }

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 3)]
        [Description("If YES, all nodes regardless of whether they have SKUs or not will be included in the export file. If NO, nodes without SKUs will not be included in the export.")]
        [DisplayName(@"Export Nodes Without SKUs")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
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