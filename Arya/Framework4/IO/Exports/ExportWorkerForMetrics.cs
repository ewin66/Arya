using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework.IO;
using Arya.HelperClasses;
using ExtendedTaxonomyInfo = Arya.Framework4.ComponentModel.ExtendedTaxonomyInfo;

namespace Arya.Framework4.IO.Exports
{
    [Serializable]
    public class ExportWorkerForMetrics : ExportWorker
    {
        #region MetricsType enum

        public enum MetricsType
        {
            [DisplayTextAndValue("Before Metrics", null)] Before,
            [DisplayTextAndValue("Current Metrics", null)] After
        }

        #endregion

        private TextWriter _attributeFile;
        private SkuDataDbDataContext _currentDb;
        private string _delimiter;
        private string[] _ignoreAttributes = new string[0];
        private MetricsType _metricsReportType;
        private Dictionary<string, List<string>> _parsedSkuExclusions;
        private Dictionary<string, List<string>> _parsedSkuInclusions;
        private TextWriter _skuFile;
        private TextWriter _valueFile;

        public ExportWorkerForMetrics(string argumentDirectoryPath, PropertyGrid ownerPropertyGrid)
            : base(argumentDirectoryPath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            AllowMultipleTaxonomySelection = true;
            MetricsReportType = MetricsType.After;
            IgnoreT1Taxonomy = false;
        }
        
        public ExportWorkerForMetrics(string argumentDirectoryPath,SerializationInfo info, StreamingContext ctxt) : base(argumentDirectoryPath,info, ctxt)
        {
            MetricsReportType = (MetricsType) info.GetValue("MetricsReportType", typeof (MetricsType));
            IgnoreAttributes = (string[]) info.GetValue("IgnoreAttributes", typeof (string[]));
            SkuInclusions = (string[]) info.GetValue("SkuInclusions", typeof (string[]));
            SkuExclusions = (string[]) info.GetValue("SkuExclusions", typeof (string[]));
            IgnoreT1Taxonomy = (bool) info.GetValue("IgnoreT1Taxonomy", typeof (bool));
            NoSchemaIfNoSku = (bool)info.GetValue("NoSchemaIfNoSku", typeof(bool));
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 1)]
        [DefaultValue(false)]
        [DisplayName(@"Ignore Taxonomy T1"), Description("Eliminates the top level node name in the export file")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IgnoreT1Taxonomy { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 1)]
        [DefaultValue(false)]
        [DisplayName(@"No Schema If No Sku"), Description("Igonore Taxonomies that does not have skus")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool NoSchemaIfNoSku { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 2)]
        [DefaultValue(MetricsType.After)]
        [TypeConverter(typeof (CustomEnumConverter))]
        [DisplayName(@"Metrics Type")]
        [RefreshProperties(RefreshProperties.All)]
        public MetricsType MetricsReportType
        {
            get { return _metricsReportType; }
            set
            {
                _metricsReportType = value;

                var isBeforeMetrics = _metricsReportType == MetricsType.Before;
                SetReadOnlyProperty(GetType(), "SkuInclusions", isBeforeMetrics);
                SetReadOnlyProperty(GetType(), "SkuExclusions", isBeforeMetrics);
                if (isBeforeMetrics)
                {
                    SkuInclusions = new string[0];
                    SkuExclusions = new string[0];
                }
            }
        }

        [Category(CaptionOptional), Description("Attribute that will be Ignored"),
         PropertyOrder(OptionalBaseOrder + 3)]
        [DisplayName(@"Ignore Attributes")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] IgnoreAttributes
        {
            get { return _ignoreAttributes; }
            set { _ignoreAttributes = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 4)]
        [DisplayName(@"Consider SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        [ReadOnly(false)]
        public string[] SkuInclusions { get; set; }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Ignore SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        [ReadOnly(false)]
        public string[] SkuExclusions { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IgnoreT1Taxonomy", IgnoreT1Taxonomy);
            info.AddValue("MetricsReportType", MetricsReportType);
            info.AddValue("IgnoreAttributes", IgnoreAttributes);
            info.AddValue("SkuInclusions", SkuInclusions);
            info.AddValue("SkuExclusions", SkuExclusions);
            info.AddValue("NoSchemaIfNoSku", SkuExclusions);

        }

       public override void Run()
        {
            State = WorkerState.Working;
            _parsedSkuExclusions = ParseSkuInclusionAndExclusions(SkuExclusions);
            _parsedSkuInclusions = ParseSkuInclusionAndExclusions(SkuInclusions);

            _delimiter = FieldDelimiter.GetValue().ToString();
            _currentDb = new SkuDataDbDataContext();

            var dlo = new DataLoadOptions();
            dlo.LoadWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas);
            dlo.LoadWith<EntityInfo>(entityInfo => entityInfo.EntityDatas);
            dlo.LoadWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas);

            dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas.Where(p => p.Active));
            dlo.AssociateWith<EntityInfo>(entityInfo => entityInfo.EntityDatas.Where(p => p.Active || p.BeforeEntity));
            dlo.AssociateWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas.Where(p => p.Active));
            dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.SkuInfos.Where(p => p.Active));

            _currentDb.LoadOptions = dlo;
            _currentDb.CommandTimeout = 2000;

            _currentDb.Connection.Open();
            _currentDb.Connection.ChangeDatabase(AryaTools.Instance.InstanceData.Dc.Connection.Database);


            var fi = new FileInfo(ExportFileName);
            var baseFileName = fi.FullName.Replace(fi.Extension, string.Empty);
            _attributeFile = new StreamWriter(baseFileName + "_attributes.txt", false, Encoding.UTF8);
            _valueFile = new StreamWriter(baseFileName + "_values.txt", false, Encoding.UTF8);
            _skuFile = new StreamWriter(baseFileName + "_skus.txt", false, Encoding.UTF8);

            StatusMessage = "Init";
            var allExportTaxonomyIds =
                Taxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).Distinct().ToList();
            var exportTaxonomies = _currentDb.TaxonomyInfos.Where(p => allExportTaxonomyIds.Contains(p.ID)).ToList();
            var allChildren = exportTaxonomies.SelectMany(p => p.AllChildren).Distinct().ToList();
            var allLeafChildren = exportTaxonomies.SelectMany(p => p.AllLeafChildren).Distinct().ToList();
            var maxDepth = 0;

            if (allChildren.Count == 0 && allLeafChildren.Count != 0)
                maxDepth =
                    allLeafChildren.Select(
                        child => child.ToString().Split(new[] {TaxonomyInfo.Delimiter}, StringSplitOptions.None).Length)
                        .Max();

            else if (allLeafChildren.Count == 0 && allChildren.Count != 0)
                maxDepth =
                    allChildren.Select(
                        child => child.ToString().Split(new[] {TaxonomyInfo.Delimiter}, StringSplitOptions.None).Length)
                        .Max();

            else if (allLeafChildren.Count != 0 && allChildren.Count != 0)
            {
                if (allLeafChildren.Count >= allChildren.Count)
                    maxDepth =
                        allLeafChildren.Select(
                            child =>
                            child.ToString().Split(new[] {TaxonomyInfo.Delimiter}, StringSplitOptions.None).Length).Max();

                else
                    maxDepth =
                        allChildren.Select(
                            child =>
                            child.ToString().Split(new[] {TaxonomyInfo.Delimiter}, StringSplitOptions.None).Length).Max();
            }
            else
            {
                StatusMessage = "There was no data to export.";
                MaximumProgress = 1;
                CurrentProgress = 1;
                State = WorkerState.Ready;
                
            }

            if (IgnoreT1Taxonomy)
                maxDepth--;

            for (var i = 1; i <= maxDepth; i++)
            {
                _attributeFile.Write("T" + i + "\t");
                _valueFile.Write("T" + i + "\t");
                _skuFile.Write("T" + i + "\t");
            }
            _attributeFile.WriteLine(
                "Attribute{0}Node Sku Count{0}Attr. Sku Count{0}Fill Rate{0}Navigation Order{0}Display Order{0}Global",
                _delimiter);
            _valueFile.WriteLine(
                "Attribute{0}Value{0}Uom{0}Node Sku Count{0}Attr. Value Sku Count{0}Value Fill Rate{0}Attr. Fill Rate{0}Navigation Order{0}Display Order{0}Global",
                _delimiter);
            _skuFile.WriteLine(
                "Item Id{0}Node Attr. Count{0}Node Nav. Attr. Count{0}Node Disp. Attr. Count{0}Sku Attr. Count{0}Sku Nav. Attr. Count{0}Sku Disp. Attr. Count{0}Sku Attr. Fill Rate{0}Sku Nav. Attr. Fill Rate{0}Node Disp. Attr. Fill Rate",
                _delimiter);

            CurrentProgress = 0;

            if (allChildren.Count >= allLeafChildren.Count)
            {
                MaximumProgress = allChildren.Count;
                foreach (var child in allChildren)
                {
                    WriteMetricsToFile(child, maxDepth);
                    CurrentProgress++;
                }
            }
            else if (allChildren.Count < allLeafChildren.Count)
            {
                MaximumProgress = allLeafChildren.Count;

                foreach (var child in allLeafChildren)
                {
                    WriteMetricsToFile(child, maxDepth);
                    CurrentProgress++;
                }
            }

            _attributeFile.Close();
            _valueFile.Close();
            _skuFile.Close();

            StatusMessage = "Done!";
            State = WorkerState.Ready;
            
        }

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        private IEnumerable<EntityData> GetEntityDatas(IEnumerable<Sku> skus)
        {
            if (MetricsReportType == MetricsType.After)
                return skus.SelectMany(sku => sku.EntityInfos).SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active));

            return
                skus.SelectMany(
                    sku =>
                    sku.EntityInfos.SelectMany(
                        ei => ei.EntityDatas.Where(ed => ed.CreatedOn < sku.CreatedOn.AddDays(0.5))));
        }

        private void WriteMetricsToFile(TaxonomyInfo taxonomy, int taxonomyMaxDepth)
        {
           // bool noRowIfNoSchema = true;
            var attributes = (from sci in _currentDb.SchemaInfos
                              join scd in _currentDb.SchemaDatas on sci.ID equals scd.SchemaID
                              join a in _currentDb.Attributes on sci.AttributeID equals a.ID
                              where scd.Active && sci.TaxonomyID == taxonomy.ID && scd.InSchema
                              select a).ToList().Where(p => !IgnoreAttributes.Contains(p.AttributeName)).OrderBy(
                                 p => p.AttributeName).ToList();

            
            var taxonomyString = taxonomy.ToString();
            if (IgnoreT1Taxonomy)
                taxonomyString = taxonomyString.Substring(taxonomy.ToString().IndexOf('>') + 1);
            
            if (string.IsNullOrEmpty(taxonomy.ToString()))
                return;
            var taxParts = taxonomyString.Split(new[] { TaxonomyInfo.Delimiter }, StringSplitOptions.None);
            var taxonomyParts = new string[taxonomyMaxDepth];
            StatusMessage = string.Format("{1}{0}Reading node", Environment.NewLine, taxonomy);

            IEnumerable<Sku> allSkus = (from sku in _currentDb.Skus
                                        join si in _currentDb.SkuInfos on sku.ID equals si.SkuID
                                        where si.TaxonomyID == taxonomy.ID && si.Active
                                        select sku).ToList();

            StatusMessage = string.Format("{1}{0}Filtering SKUs", Environment.NewLine, taxonomy);
            IEnumerable<Sku> filteredSkus =
                GetFilteredSkuList(allSkus, true, _parsedSkuInclusions, _parsedSkuExclusions).ToList();
            var filteredSkuCount = filteredSkus.Count();

            if (!NoSchemaIfNoSku || filteredSkuCount != 0)
            {
               
                for (var i = 0; i < taxonomyMaxDepth; i++)
                {
                    taxonomyParts[i] = i < taxParts.Length ? taxParts[i].Trim() : string.Empty;
                    _attributeFile.Write(taxonomyParts[i] + "\t");
                    // valueFile.Write(taxonomyParts[i] + "\t");
                    //  skuFile.Write(taxonomyParts[i] + "\t");
                }
               
            }



            if (filteredSkuCount <= 0 )
            {
                if(!NoSchemaIfNoSku)
                    _attributeFile.WriteLine();
                //print new line
                
                // valueFile.Write("{0}", Environment.NewLine);
                // skuFile.Write("{0}", Environment.NewLine);

               return;
            }

            // Even though we only export fill rates for filtered SKUs, we export all attributes in this node
            StatusMessage = string.Format("{1}{0}Fetching Attributes", Environment.NewLine, taxonomy);


            StatusMessage = string.Format("{1}{0}Reading values", Environment.NewLine, taxonomy);
            IEnumerable<EntityData> filteredEntityDatas = GetEntityDatas(filteredSkus).ToList();

            if (!attributes.Any())
            {
                //print new line
                //this line should be uncommented
                _attributeFile.WriteLine();
                // valueFile.Write("{0}", Environment.NewLine);
                // skuFile.Write("{0}", Environment.NewLine);
            }
            else // taxonomy has attributes
            {
               
                for (var i = 0; i < attributes.Count(); i++)
                {
                    if (i != 0)
                    {
                        //print taxonomy
                        for (var j = 0; j < taxonomyMaxDepth; j++)
                        {
                            taxonomyParts[j] = j < taxParts.Length ? taxParts[j].Trim() : string.Empty;
                            _attributeFile.Write(taxonomyParts[j] + "\t");
                            // valueFile.Write(taxonomyParts[j] + "\t");
                            // skuFile.Write(taxonomyParts[j] + "\t");
                        }
                    }
                    var currentAttribute = attributes[i];

                    StatusMessage = string.Format("{1}{0}Attribute {2}", Environment.NewLine, taxonomy, currentAttribute);

                    var schemaData =
                        taxonomy.SchemaInfos.Where(si => si.Attribute.Equals(currentAttribute)).Select(
                            si => si.SchemaData).FirstOrDefault();
                    if (schemaData == null || !schemaData.InSchema) // Only export In Schema attributes
                        continue;


                    IEnumerable<EntityData> values =
                        filteredEntityDatas.Where(ed => ed.Attribute.Equals(currentAttribute)).ToList();

                    var attrCount = values.Select(ed => ed.EntityInfo.SkuID).Distinct().Count();

                    _attributeFile.WriteLine("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", _delimiter,
                                            currentAttribute, filteredSkuCount, attrCount,
                                            filteredSkuCount == 0 ? 0 : attrCount*100.00/filteredSkuCount,
                                            schemaData.NavigationOrder,
                                            schemaData.DisplayOrder, currentAttribute.AttributeType);

                    IEnumerable<string> distinctValues =
                        values.Select(ed => ed.Value + _delimiter + (ed.Uom ?? string.Empty)).Distinct().OrderBy(
                            val => val);
                    var totalSkus = 0;


                    if (distinctValues.Count() != 0)
                    {
                        for (var v = 0; v < distinctValues.Count(); v++)
                        {
                            var currentValue = distinctValues.ToList()[v];
                            var valueCount =
                                values.Where(
                                    ed => (ed.Value + _delimiter + (ed.Uom ?? string.Empty)).Equals(currentValue)).
                                    Select(ed => ed.EntityInfo.SkuID).Distinct().Count();
                            totalSkus += valueCount;

                            for (var j = 0; j < taxonomyMaxDepth; j++)
                            {
                                taxonomyParts[j] = j < taxParts.Length ? taxParts[j].Trim() : string.Empty;
                                _valueFile.Write(taxonomyParts[j] + "\t");
                            }

                            _valueFile.WriteLine(
                                "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}",
                                _delimiter, currentAttribute, currentValue, filteredSkuCount, valueCount,
                                filteredSkuCount == 0 ? 0 : valueCount*100.00/filteredSkuCount,
                                filteredSkuCount == 0 ? 0 : attrCount*100.00/filteredSkuCount,
                                schemaData.NavigationOrder,
                                schemaData.DisplayOrder, currentAttribute.AttributeType);
                        } //end of for
                    } //end of if


                    if (totalSkus < filteredSkuCount)
                    {
                        var currentValue = string.Empty + _delimiter + string.Empty;
                        var valueCount = filteredSkuCount - totalSkus;

                        for (var j = 0; j < taxonomyMaxDepth; j++)
                        {
                            taxonomyParts[j] = j < taxParts.Length ? taxParts[j].Trim() : string.Empty;
                            _valueFile.Write(taxonomyParts[j] + "\t");
                        }
                        _valueFile.WriteLine(
                            "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}",
                            _delimiter, currentAttribute, currentValue, filteredSkuCount, valueCount,
                            filteredSkuCount == 0 ? 0 : valueCount*100.00/filteredSkuCount,
                            filteredSkuCount == 0 ? 0 : attrCount*100.00/filteredSkuCount, schemaData.NavigationOrder,
                            schemaData.DisplayOrder, currentAttribute.AttributeType);
                    }
                } //end of for
            } //end of else

            StatusMessage = string.Format("{1}{0}Fetching Navigation and Display attributes", Environment.NewLine,
                                          taxonomy);

            var navigationAttributes = taxonomy.SchemaInfos.Where(si =>
                                                                      {
                                                                          var sd = si.SchemaData;
                                                                          return sd != null &&
                                                                                 sd.NavigationOrder > 0;
                                                                      }).Select(si => si.Attribute).ToDictionary(
                                                                          att => att, att => att.AttributeName);

            var displayAttributes = taxonomy.SchemaInfos.Where(si =>
                                                                   {
                                                                       var sd = si.SchemaData;
                                                                       return sd != null && sd.DisplayOrder > 0;
                                                                   }).Select(si => si.Attribute).ToDictionary(
                                                                       att => att, att => att.AttributeName);

            var ctr = 0;

            for (var i = 0; i < filteredSkus.Count(); i++)
            {
                StatusMessage = string.Format("{1}{0}{2} Skus", Environment.NewLine, taxonomy, ++ctr);
                var currentSku = filteredSkus.ToList()[i];

                IEnumerable<EntityData> skuEntityDatas =
                    currentSku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active)).ToList();

                var skuAttCount =
                    skuEntityDatas.Where(ed => attributes.Contains(ed.Attribute)).Select(ed => ed.AttributeID).
                        Distinct().Count();
                var skuNavAttCount =
                    skuEntityDatas.Where(ed => navigationAttributes.ContainsKey(ed.Attribute)).Select(
                        ed => ed.AttributeID).Distinct().Count();
                var skuDispAttCount =
                    skuEntityDatas.Where(ed => displayAttributes.ContainsKey(ed.Attribute)).Select(
                        ed => ed.AttributeID).Distinct().Count();

                //print taxonomy
                for (var j = 0; j < taxonomyMaxDepth; j++)
                {
                    taxonomyParts[j] = j < taxParts.Length ? taxParts[j].Trim() : string.Empty;
                    _skuFile.Write(taxonomyParts[j] + "\t");
                }

                _skuFile.WriteLine("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}", _delimiter,
                                  currentSku.ItemID, attributes.Count, navigationAttributes.Count,
                                  displayAttributes.Count,
                                  skuAttCount, skuNavAttCount, skuDispAttCount,
                                  attributes.Count == 0 ? 0 : skuAttCount*100.0/attributes.Count,
                                  navigationAttributes.Count == 0 ? 0 : skuNavAttCount*100.0/navigationAttributes.Count,
                                  displayAttributes.Count == 0 ? 0 : skuDispAttCount*100.0/displayAttributes.Count);
            }
        }
    }
}