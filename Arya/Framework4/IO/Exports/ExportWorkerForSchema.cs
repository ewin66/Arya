using Arya.Data;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.Properties;
using Arya.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Attribute = Arya.Data.Attribute;
using EnumExtensions = Arya.Framework.Common.Extensions.EnumExtensions;
using ExtendedTaxonomyInfo = Arya.Framework4.ComponentModel.ExtendedTaxonomyInfo;
using SchemaAttribute = Arya.HelperClasses.SchemaAttribute;
using SchemaInfo = Arya.Data.SchemaInfo;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;

namespace Arya.Framework4.IO.Exports
{
    [Serializable]
    public class ExportWorkerForSchema : ExportWorker
    {
        private static readonly string EnrichmentImage = Resources.SchemaEnrichmentImageAttributeName;
        private readonly HashSet<TaxonomyInfo> _processedTaxonomies = new HashSet<TaxonomyInfo>();
        private readonly char[] _taxonomyDelimiters = { '>' };
        private List<Arya.HelperClasses.SchemaAttribute> _allSchemati;
        private string _delimiter;
        private DirectoryInfo _downloadDir;
        private List<Tuple<string, Guid, Guid, decimal>> _fillRates;
        private bool _leafNodesOnly = true;
        private TextWriter _lovFile;
        private List<string> _possibleMissingInSchemaAttributes;
        private TextWriter _schemaFile;
        private TextWriter _taxFile;
        private SkuDataDbDataContext currentDb = new SkuDataDbDataContext();
        private List<Attribute> _taxMetaAttributes = new List<Attribute>();

        public ExportWorkerForSchema(string argumentDirectoryPath, PropertyGrid ownerPropertyGrid)
            : base(argumentDirectoryPath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            LeafNodesOnly = true;
            ExportMetaAttributes = true;
            AllowMultipleTaxonomySelection = true;
            IgnoreT1Taxonomy = false;
            ExportAllSchemaNodes = true;
        }

        public ExportWorkerForSchema(string argumentDirectoryPath, SerializationInfo info, StreamingContext ctxt)
            : base(argumentDirectoryPath, info, ctxt)
        {
            LeafNodesOnly = (bool)info.GetValue("LeafNodesOnly", typeof(bool));
            ExportFillRates = (bool)info.GetValue("ExportFillRates", typeof(bool));
            ExportSuperSchema = (bool)info.GetValue("ExportSuperSchema", typeof(bool));
            IgnoreT1Taxonomy = (bool)info.GetValue("IgnoreT1Taxonomy", typeof(bool));
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 1)]
        [DefaultValue(false)]
        [DisplayName(@"Ignore Taxonomy T1"), Description("Eliminates the top level node name in the export file")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IgnoreT1Taxonomy { get; set; }

        [DefaultValue(true), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 2)]
        [Description("If YES, only leaf nodes (nodes with no immediate child nodes) will be exported. If NO, leaf nodes and parent nodes will be exported. (e.g. taxonomy node enrichments on a parent node.)")]
        [DisplayName(@"Export Leaf nodes only")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool LeafNodesOnly
        {
            get { return _leafNodesOnly; }
            set { _leafNodesOnly = value; }
        }

        [DefaultValue(false), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 2),
         DisplayName(@"Export Fill Rates")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportFillRates { get; set; }

        [DefaultValue(false), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 3),
         DisplayName(@"Export Super Schema")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportSuperSchema { get; set; }

        [DefaultValue(true), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 4),
         DisplayName(@"Export All Schema Nodes")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportAllSchemaNodes { get; set; }

        [DefaultValue(true), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 5),
         DisplayName(@"Export All Meta-Attributes"), Description("When set to Yes Export will contain all the active Meta-Attributes. Set to No, export will not contain any meta attribute.")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportMetaAttributes { get; set; }

        [DefaultValue(false), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 6),
         DisplayName(@"Export Enrichments")]
        [Description("If YES, the enrichment information (image names and copy) will be included in the export file. If NO, the enrichment information will not be included. Note that this option exports only the name of the image file, not the image file itself.  (See 'Download Enrichments' for exporting images files.)")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportEnrichments { get; set; }

        [DefaultValue(false), Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 7),
         DisplayName(@"Download Assets")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool DownloadAssets { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IgnoreT1Taxonomy", IgnoreT1Taxonomy);
            info.AddValue("LeafNodesOnly", LeafNodesOnly);
            info.AddValue("ExportFillRates", ExportFillRates);
            info.AddValue("ExportSuperSchema", ExportSuperSchema);
        }

        public override void Run()
        {
            var currentDb = new SkuDataDbDataContext();

            currentDb.Connection.Open();
            currentDb.Connection.ChangeDatabase(AryaTools.Instance.InstanceData.Dc.Connection.Database);

            var allExportTaxonomyIds =
                Taxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).Distinct().ToList();

            var exportTaxonomies = currentDb.TaxonomyInfos.Where(p => allExportTaxonomyIds.Contains(p.ID)).ToList();
            //  var baseFileName = ExportFileName;
            var fi = new FileInfo(ExportFileName);
            var baseFileName = fi.FullName.Replace(fi.Extension, string.Empty);

            // initialize base file
            _schemaFile = new StreamWriter(baseFileName + ".txt", false, Encoding.UTF8);
            // initialize LOV file
            _lovFile = new StreamWriter(baseFileName + "_Lov.txt", false, Encoding.UTF8);
            // initialize Taxonomy file
            _taxFile = new StreamWriter(baseFileName + "_Tax.txt", false, Encoding.UTF8);

            StatusMessage = "Init";
            State = WorkerState.Working;
            AryaTools.Instance.AllFillRates.UseBackgroundWorker = false;
            _delimiter = EnumExtensions.GetValue(FieldDelimiter).ToString();
            _possibleMissingInSchemaAttributes = new List<string>();
            Init();

            var maxDepth = 0;
            var allChildren = exportTaxonomies.SelectMany(p => p.AllChildren).Distinct().ToList();
            var allLeafChildren = exportTaxonomies.SelectMany(p => p.AllLeafChildren).Distinct().ToList();

            if (allChildren.Count == 0 && allLeafChildren.Count != 0)
            {
                maxDepth =
                    allLeafChildren.Select(
                        child => child.ToString().Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None).Length)
                        .Max();
            }

            else if (allLeafChildren.Count == 0 && allChildren.Count != 0)
            {
                maxDepth =
                    allChildren.Select(
                        child => child.ToString().Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None).Length)
                        .Max();
            }

            else if (allLeafChildren.Count != 0 && allChildren.Count != 0)
            {
                if (allLeafChildren.Count >= allChildren.Count)
                {
                    maxDepth =
                        allLeafChildren.Select(
                            child =>
                                child.ToString().Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None).Length)
                            .Max();
                }

                else
                {
                    maxDepth =
                        allChildren.Select(
                            child =>
                                child.ToString().Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None).Length)
                            .Max();
                }
            }
            else
            {
                StatusMessage = "There was no data to export.";
                MaximumProgress = 1;
                CurrentProgress = 1;
                State = WorkerState.Ready;
            }
            if (IgnoreT1Taxonomy && maxDepth > 1)
                maxDepth--;

            _taxFile.Write("Taxonomy{0}", _delimiter);
            _schemaFile.Write("Taxonomy{0}", _delimiter);

            for (var i = 1; i <= maxDepth; i++)
            {
                _schemaFile.Write("T" + i + "\t");
                _lovFile.Write("T" + i + "\t");
                _taxFile.Write("T" + i + "\t");
            }
            // _schemaFile.Write("Taxonomy{0}T1{0}T2{0}T3{0}T4{0}T5{0}T6{0}T7{0}T8{0}NodeType{0}Attribute", _delimiter);

            _schemaFile.Write("NodeType{0}Attribute", _delimiter);

            foreach (var schematus in _allSchemati)
                _schemaFile.Write(_delimiter + schematus);
            _schemaFile.WriteLine();

            // _lovFile.Write("T1{0}T2{0}T3{0}T4{0}T5{0}T6{0}T7{0}T8{0}NodeType{0}AttributeName{0}NavigationOrder{0}DisplayOrder{0}Value",_delimiter);

            _lovFile.Write("NodeType{0}AttributeName{0}NavigationOrder{0}DisplayOrder{0}Value", _delimiter);
            if (ExportEnrichments)
                _lovFile.Write("{0}EnrichmentImage{0}EnrichmentCopy", _delimiter);
            _lovFile.WriteLine();
            foreach (var metaAttribute in _taxMetaAttributes)
            {
                _taxFile.Write(_delimiter + metaAttribute.AttributeName);
            }
            _taxFile.WriteLine();
            //if (ExportEnrichments)
            //    _taxFile.Write("{0}EnrichmentImage{0}EnrichmentCopy", _delimiter);
            //_taxFile.WriteLine();

            // create download folder, if requested
            if (ExportEnrichments && DownloadAssets)
            {
                _downloadDir = new DirectoryInfo(baseFileName + "_Assets");
                _downloadDir.Create();
            }

            var allTaxonomies = Taxonomies.Cast<ExtendedTaxonomyInfo>().ToList();
            MaximumProgress = allTaxonomies.Sum(p => TaxonomyInfo.GetNodeCount(p.Taxonomy)) + 1;
            CurrentProgress = 0;
            // MaximumProgress = allChildren.Count;

            var mscFillRateHelper = new MSCFillRateHelper();
            _fillRates = new List<Tuple<string, Guid, Guid, decimal>>(5000);

            if (ExportFillRates)
            {
                foreach (var exportTaxonomyInfo in allTaxonomies)
                {
                    mscFillRateHelper.GetFillRates(exportTaxonomyInfo.Taxonomy.ID,
                        AryaTools.Instance.InstanceData.CurrentProject.ProjectName);
                }
            }

            foreach (var exportTaxonomyInfo in allTaxonomies)
                WriteTaxonomyToFile(exportTaxonomyInfo.Taxonomy, maxDepth);
            CurrentProgress++;

            _schemaFile.Close();
            _lovFile.Close();

            _lovFile.Dispose();
            _taxFile.Close();

            var exceptionFileName = baseFileName + "_Exceptions.txt";

            File.WriteAllLines(exceptionFileName, _possibleMissingInSchemaAttributes, Encoding.UTF8);

            AryaTools.Instance.AllFillRates.UseBackgroundWorker = true;

            StatusMessage = "Done!";
            State = WorkerState.Ready;
        }

        private void WriteTaxonomyToFile(TaxonomyInfo taxonomy, int taxonomyMaxDepth)
        {
            var ignoreT1taxonomy = taxonomy.ToString();
            if (IgnoreT1Taxonomy)
                ignoreT1taxonomy = taxonomy.ToString().Substring(taxonomy.ToString().IndexOf('>') + 1);

            if (string.IsNullOrEmpty(taxonomy.ToString()))
                return;

            if (_processedTaxonomies.Contains(taxonomy))
                return;
            _processedTaxonomies.Add(taxonomy);

            if (!LeafNodesOnly || taxonomy.HasSkus || ExportAllSchemaNodes)
            {
                StatusMessage = string.Format("{1}{0}Reading Node", Environment.NewLine, taxonomy);

                List<Attribute> atts;

                if (ExportSuperSchema && !taxonomy.IsLeafNode)
                {
                    var schemaInfos = new List<SchemaInfo>();
                    schemaInfos.AddRange(taxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());
                    if (ExportSuperSchema)
                    {
                        foreach (var leaf in taxonomy.AllLeafChildren)
                        {
                            schemaInfos.AddRange(
                                leaf.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());
                        }
                    }
                    var schemaAttributes =
                        schemaInfos.Where(p => p.SchemaData.InSchema)
                            .Select(p => new { p.Attribute, p.SchemaData })
                            .ToList();

                    atts = (from att in schemaAttributes
                            group att by att.Attribute
                                into grp
                                let minRank = grp.Min(p => GetRank(p.SchemaData, SortOrder.OrderbyDisplayNavigation))
                                orderby minRank
                                select grp.Key).Distinct().ToList();
                }
                else
                {
                    atts = (from si in AryaTools.Instance.InstanceData.Dc.SchemaInfos
                            where si.TaxonomyID == taxonomy.ID
                            let sd = si.SchemaDatas.FirstOrDefault(sd => sd.Active)
                            where sd != null
                            let inSchema = sd.InSchema
                            where inSchema
                            let navRank = sd == null || sd.NavigationOrder == 0 ? decimal.MaxValue : sd.NavigationOrder
                            let dispRank = sd == null || sd.DisplayOrder == 0 ? decimal.MaxValue : sd.DisplayOrder
                            orderby navRank, dispRank
                            select si.Attribute).ToList();

                    var currentTaxonomyString = ignoreT1taxonomy;

                    var possibleMissingAttributes = (from si in AryaTools.Instance.InstanceData.Dc.SchemaInfos
                                                     where si.TaxonomyID == taxonomy.ID
                                                     let sd = si.SchemaDatas.FirstOrDefault(sd => sd.Active)
                                                     where sd != null && !sd.InSchema && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                     select si.Attribute.AttributeName).ToList()
                        .Select(p => currentTaxonomyString + "\t" + p)
                        .ToList();

                    _possibleMissingInSchemaAttributes.AddRange(possibleMissingAttributes);
                }

                var isLeafNode = taxonomy.IsLeafNode;

                //if a node has no schema, just skip it.
                if ((atts.Count == 0 && ExportAllSchemaNodes) || (LeafNodesOnly && isLeafNode == false))
                {
                    StatusMessage = string.Format("{1}{0}, Fetching Child Nodes", Environment.NewLine, taxonomy);
                    var taxonomies =
                        taxonomy.ChildTaxonomyDatas.Where(td => td.Active)
                            .Select(td => td.TaxonomyInfo)
                            .OrderBy(tax => tax.ToString());

                    foreach (var tax in taxonomies)
                        WriteTaxonomyToFile(tax, taxonomyMaxDepth);
                    CurrentProgress++;
                    return;
                }

                var taxonomyString = ignoreT1taxonomy;
                var taxParts = ignoreT1taxonomy.Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None);
                var taxonomyParts = new string[taxonomyMaxDepth];

                _taxFile.Write("{1}{0}", _delimiter, taxonomyString);
                for (var i = 0; i < taxonomyMaxDepth; i++)
                {
                    taxonomyParts[i] = i < taxParts.Length ? taxParts[i].Trim() : string.Empty;
                    _taxFile.Write(taxonomyParts[i] + "\t");
                }

                // write to taxonomy file
                // _taxFile.WriteLine("{9}{0}{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}", _delimiter, taxonomyString);

                WriteTaxMetaValue(taxonomy);

                _schemaFile.WriteLine("{1}{0}", _delimiter, taxonomyString);
                foreach (var attribute in atts)
                {
                    _schemaFile.Write("{1}{0}", _delimiter, taxonomyString);
                    for (var i = 0; i < taxonomyMaxDepth; i++)
                    {
                        taxonomyParts[i] = i < taxParts.Length ? taxParts[i].Trim() : string.Empty;
                        _schemaFile.Write(taxonomyParts[i] + "\t");
                    }

                    _schemaFile.Write("{1}{0}{2}", _delimiter,
                        (taxonomy.NodeType == "Derived") ? "Cross List" : TaxonomyInfo.NodeTypeRegular, attribute);

                    foreach (var schematus in _allSchemati)
                    {
                        object value = null;

                        if (schematus.AttributeType == SchemaAttribute.SchemaAttributeType.FillRate)
                        {
                            if (_fillRates.Count > 0)
                            {
                                var data =
                                    _fillRates.FirstOrDefault(
                                        p =>
                                            p.Item1 == schematus.FillRateSchemaAttribute.GetFilterName()
                                            && p.Item2 == taxonomy.ID && p.Item3 == attribute.ID);

                                value = data == null ? "0" : data.Item4.ToString(CultureInfo.InvariantCulture);
                            }
                            else
                                value = SchemaAttribute.GetValue(taxonomy, attribute, schematus);
                        }
                        else
                        {
                            if (ExportSuperSchema && !isLeafNode)
                            {
                                if (schematus.AttributeType == SchemaAttribute.SchemaAttributeType.Meta)
                                {
                                    if (schematus.MetaSchemaAttribute.AttributeName.Equals("Sample Values",
                                        StringComparison.OrdinalIgnoreCase)
                                        && schematus.MetaSchemaAttribute.AttributeName.Equals("Restricted UOM",
                                            StringComparison.OrdinalIgnoreCase))
                                        value = string.Empty;
                                }
                                else
                                {
                                    var allChildren = taxonomy.AllLeafChildren.ToList();
                                    allChildren.Add(taxonomy);

                                    var attribute2 = attribute;
                                    var schematus1 = schematus;
                                    var bestValue = (from child in allChildren
                                                     let childValue = SchemaAttribute.GetValue(child, attribute2, schematus1)
                                                     where childValue != null
                                                     group child by childValue
                                                         into grp
                                                         orderby grp.Count() descending
                                                         select grp.Key).FirstOrDefault();

                                    value = bestValue;
                                }
                            }
                            else
                                value = SchemaAttribute.GetValue(taxonomy, attribute, schematus);
                        }

                        value = value ?? string.Empty;

                        if ((schematus.PrimarySchemaAttribute == "Navigation Order"
                             || schematus.PrimarySchemaAttribute == "Display Order"))
                        {
                            decimal result;
                            if (Decimal.TryParse(value.ToString(), out result) && result > 0)
                                value = string.Format("{0:0.##}", value);
                        }
                        else if (schematus.AttributeType == SchemaAttribute.SchemaAttributeType.Meta
                                 && schematus.MetaSchemaAttribute.AttributeName == Resources.SchemaEnrichmentImageAttributeName
                                 && !String.IsNullOrEmpty(value.ToString()))
                        {
                            using (var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID, AryaTools.Instance.InstanceData.CurrentUser.ID))
                            {
                                var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                                    {
                                        RemoteImageGuid = value.ToString()
                                    };
                                if (DownloadAssets)
                                {
                                    imageMgr.LocalDirectory = _downloadDir.ToString();
                                    imageMgr.DownloadImage(taxonomy.ID, attribute.ID);
                                    value = imageMgr.LocalImageName;
                                }
                                else
                                    value = imageMgr.RemoteImageUrl;
                            }
                        }

                        var value2 = value.ToString();

                        if (value2.Length > 0)
                        {
                            _schemaFile.Write(_delimiter
                                              + value2.Replace("\n", string.Empty).Replace("\r", string.Empty));
                        }
                        else
                            _schemaFile.Write(_delimiter + value2);
                    }
                    _schemaFile.WriteLine();

                    var schemaData =
                        attribute.SchemaInfos.Where(p => p.TaxonomyID == taxonomy.ID && p.SchemaData != null)
                            .Select(p => p.SchemaData)
                            .FirstOrDefault();

                    if (schemaData != null)
                    {
                        var attribute1 = attribute;

                        foreach (var lov in schemaData.SchemaInfo.ListOfValues.Where(lov => lov.Active))
                        {
                            for (var j = 0; j < taxonomyMaxDepth; j++)
                            {
                                taxonomyParts[j] = j < taxParts.Length ? taxParts[j].Trim() : string.Empty;
                                _lovFile.Write(taxonomyParts[j] + "\t");
                            }
                            //_lovFile.WriteLine(
                            //    "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}",
                            //    _delimiter, taxParts[0], taxParts[1], taxParts[2], taxParts[3], taxParts[4], taxParts[5],
                            //    taxParts[6], taxParts[7], (taxonomy.NodeType == "Derived") ? "Cross List" : TaxonomyInfo.NodeTypeRegular,
                            //    attribute1.AttributeName,
                            //    schemaData.NavigationOrder == 0
                            //        ? string.Empty
                            //        : string.Format("{0:0.##}", schemaData.NavigationOrder),
                            //    schemaData.DisplayOrder == 0
                            //        ? string.Empty
                            //        : string.Format("{0:0.##}", schemaData.DisplayOrder), lov.Value);

                            _lovFile.Write("{1}{0}{2}{0}{3}{0}{4}{0}{5}", _delimiter,
                                (taxonomy.NodeType == "Derived") ? "Cross List" : TaxonomyInfo.NodeTypeRegular,
                                attribute1.AttributeName,
                                schemaData.NavigationOrder == 0
                                    ? string.Empty
                                    : string.Format("{0:0.##}", schemaData.NavigationOrder),
                                schemaData.DisplayOrder == 0
                                    ? string.Empty
                                    : string.Format("{0:0.##}", schemaData.DisplayOrder), lov.Value);

                            if (ExportEnrichments)
                            {
                                using (
                                    var dc =
                                        new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                                            AryaTools.Instance.InstanceData.CurrentUser.ID))
                                {
                                    var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                                        {
                                            ImageSku = dc.Skus.FirstOrDefault(s => s.ItemID == lov.EnrichmentImage)
                                        };
                                    string lovImage;
                                    if (DownloadAssets)
                                    {
                                        imageMgr.LocalDirectory = _downloadDir.ToString();
                                        imageMgr.DownloadImage(taxonomy.ID, attribute.ID, lov.ID);
                                        lovImage = imageMgr.LocalImageName;
                                    }
                                    else
                                        lovImage = imageMgr.RemoteImageUrl;

                                    _lovFile.Write("{0}{1}{0}{2}", _delimiter, lovImage, lov.EnrichmentCopy);
                                }
                            }

                            _lovFile.WriteLine();
                        }
                    }
                }
            }

            StatusMessage = string.Format("{1}{0}, Fetching Child Nodes", Environment.NewLine, taxonomy);
            var taxs =
                taxonomy.ChildTaxonomyDatas.Where(td => td.Active)
                    .Select(td => td.TaxonomyInfo)
                    .OrderBy(tax => tax.ToString());

            foreach (var tax in taxs)
                WriteTaxonomyToFile(tax, taxonomyMaxDepth);
            CurrentProgress++;
        }

        private void WriteTaxMetaValue(TaxonomyInfo taxonomy)
        {
            foreach (var taxMetaAttribute in _taxMetaAttributes)
            {
                var rawAttributeValue = GetValue(taxMetaAttribute, taxonomy);
                string finalAttributeValue = rawAttributeValue;

                if (ExportEnrichments)
                {
                    if (taxMetaAttribute.ToString() == Resources.TaxonomyEnrichmentCopyAttributeName)
                    {
                        // replace carriage returns with "|"
                        if (!String.IsNullOrEmpty(rawAttributeValue))
                        {
                            var returnChar =
                                AryaTools.Instance.InstanceData.CurrentProject.ProjectPreferences.ReturnSeparator ?? "|";
                            finalAttributeValue = rawAttributeValue.Replace("\r", String.Empty).Replace("\n", returnChar);
                        }
                    }
                    else if (taxMetaAttribute.ToString() == Resources.TaxonomyEnrichmentImageAttributeName)
                    {
                        using (var dc =
                            new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                                AryaTools.Instance.InstanceData.CurrentUser.ID))
                        {
                            var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                            {
                                ImageSku = dc.Skus.FirstOrDefault(s => s.ItemID == rawAttributeValue)
                            };
                            
                            // retrieve Enrichment Image, if it exists
                            if (DownloadAssets)
                            {
                                imageMgr.LocalDirectory = _downloadDir.ToString();
                                imageMgr.DownloadImage(taxonomy.ID);
                                finalAttributeValue = imageMgr.LocalImageName;
                            }
                            else
                                finalAttributeValue = imageMgr.RemoteImageUrl;
                        }
                    }
                }

                _taxFile.Write("{0}{1}", _delimiter, finalAttributeValue);
            }
            _taxFile.WriteLine();
        }

        private string GetValue(Attribute taxMetaAttribute, TaxonomyInfo taxonomy)
        {
            var taxMetaData = (from tmd in AryaTools.Instance.InstanceData.Dc.TaxonomyMetaDatas
                               where
                                   tmd.TaxonomyMetaInfo.TaxonomyID.Equals(taxonomy.ID) &&
                                   tmd.TaxonomyMetaInfo.MetaAttributeID.Equals(taxMetaAttribute.ID) &&
                                   tmd.Active
                               select tmd).FirstOrDefault();
            return taxMetaData == null ? string.Empty : taxMetaData.Value;
        }

        public override List<string> ValidateInput()
        {
            var baseErrors = base.ValidateInput();
            return baseErrors;
        }

        public virtual bool IsInputValid()
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            ProcessSchemaMetaAttribute();
            ProcessTaxonomyMetaAttribute();
        }

        private void ProcessTaxonomyMetaAttribute()
        {
            var emptyGuid = new Guid();
            //var metaAttributes = new List<Attribute>();
            if (ExportEnrichments || ExportMetaAttributes)
            {
                _taxMetaAttributes = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                                      where
                                          att.ProjectID.Equals(AryaTools.Instance.InstanceData.CurrentProject.ID) &&
                                          att.AttributeType.Equals(AttributeTypeEnum.TaxonomyMeta.ToString()) &&
                                          att.SchemaInfos.Any(si => si.SchemaDatas.Any(sd => sd.Active))
                                      let displayRank = (from si in att.SchemaInfos
                                                         where si.TaxonomyID.Equals(emptyGuid)
                                                         from sd in si.SchemaDatas
                                                         where sd.Active
                                                         select sd.DisplayOrder).FirstOrDefault()
                                      orderby displayRank, att.AttributeName
                                      select att).ToList();
                //remove enrichment
                if (!ExportEnrichments)
                    _taxMetaAttributes =
                        _taxMetaAttributes.Where(s => !(s.AttributeName.Contains("Enrichment"))).ToList();
                if (!ExportMetaAttributes)
                    _taxMetaAttributes =
                        _taxMetaAttributes.Where(
                            s =>
                            s.AttributeType != AttributeTypeEnum.TaxonomyMeta.ToString() ||
                            s.AttributeName.Contains("Enrichment")).ToList();
                //remove export
            }
        }

        private void ProcessSchemaMetaAttribute()
        {
            var emptyGuid = new Guid();
            var metaAttributes = new List<SchemaAttribute>();
            if (ExportEnrichments || ExportMetaAttributes)
            {
                metaAttributes = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                                  where
                                      att.ProjectID.Equals(AryaTools.Instance.InstanceData.CurrentProject.ID) &&
                                      att.AttributeType.Equals(AttributeTypeEnum.SchemaMeta.ToString()) &&
                                      att.SchemaInfos.Any(si => si.SchemaDatas.Any(sd => sd.InSchema && sd.Active))
                                  let displayRank = (from si in att.SchemaInfos
                                                     where si.TaxonomyID.Equals(emptyGuid)
                                                     from sd in si.SchemaDatas
                                                     where sd.Active
                                                     select sd.DisplayOrder).FirstOrDefault()
                                  orderby displayRank, att.AttributeName
                                  select att).Select(att => new SchemaAttribute(att, typeof(string))).ToList();
                //remove enrichment
                if (!ExportEnrichments)
                    metaAttributes =
                        metaAttributes.Where(s => !(s.AttributeType == SchemaAttribute.SchemaAttributeType.Meta
                                                    && s.MetaSchemaAttribute.AttributeName.Contains("Enrichment"))).ToList();
                if (!ExportMetaAttributes)
                    metaAttributes =
                        metaAttributes.Where(
                            s =>
                            s.AttributeType != SchemaAttribute.SchemaAttributeType.Meta ||
                            s.MetaSchemaAttribute.AttributeName.Contains("Enrichment")).ToList();
                //remove export
            }

            var schemati =
                SchemaAttribute.PrimarySchemati.Where(
                    p => p != SchemaAttribute.SchemaAttributeIsMapped && p != SchemaAttribute.SchemaAttributeInSchema);
            if (ExportFillRates)
                schemati = schemati.Union(SchemaAttribute.FillRateSchemati);
            _allSchemati =
                schemati.Union(metaAttributes).ToList();
        }
    }
}