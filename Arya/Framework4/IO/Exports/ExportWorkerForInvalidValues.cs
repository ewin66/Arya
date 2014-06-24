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
using Arya.HelperClasses;
using Attribute = Arya.Data.Attribute;
using ExtendedTaxonomyInfo = Arya.Framework4.ComponentModel.ExtendedTaxonomyInfo;

namespace Arya.Framework4.IO.Exports
{
    [Serializable]
    public class ExportWorkerForInvalidValues : ExportWorker
    {
        //private HashSet<string> skuList = new HashSet<string>();
        //[Category(CaptionOptional), Description("Export only these SKUs - all other filters (including Taxonomy) will be ignored"), PropertyOrder(OPTIONAL_BASE_ORDER + 1)]
        //[DisplayName(@"Restricted Sku List")]
        //[TypeConverter(typeof(StringArrayConverter))]
        //public string[] SkuList
        //{
        //    get { return skuList.ToArray(); }
        //    set { skuList = value.ToHashSet(); }
        //}

        private TextWriter attributeDataFile;
        private string[] attributeGroupExclusions = new string[0];
        private string[] attributeGroupInclusions = new string[0];
        private Dictionary<string, string> baseAttributeNames;
        private SkuDataDbDataContext currentDb;
        private string delimiter;
        private string[] globalAttributeHeaders;
        private string[] globalAttributeNames;
        private string[] globalAttributes = new string[0];
        private Dictionary<string, List<string>> parsedSkuExclusions;

        private Dictionary<string, List<string>> parsedSkuInclusions;
        private string projectField1Name;

        private string[] skuExclusions = new string[0];
        private string[] skuInclusions = new string[0];
        private Dictionary<Guid, List<KeyValuePair<Attribute, SchemaData>>> taxonomyAttributesCache;

        public ExportWorkerForInvalidValues(string argumentDirectoryPath, PropertyGrid ownerPropertyGrid)
            : base(argumentDirectoryPath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            AllowMultipleTaxonomySelection = true;
            SetBrowsableProperty(GetType(), "ExportSourceType", true);
        }

        public ExportWorkerForInvalidValues(string argumentDirectoryPath, SerializationInfo info, StreamingContext ctxt)
            : base(argumentDirectoryPath, info, ctxt)
        {
            GlobalAttributes = (string[]) info.GetValue("GlobalAttributes", typeof (string[]));
            SkuInclusions = (string[]) info.GetValue("SkuInclusions", typeof (string[]));
            SkuExclusions = (string[]) info.GetValue("SkuExclusions", typeof (string[]));
            //SkuValueInclusions = (string[])info.GetValue("SkuValueInclusions", typeof(string[]));
            AttributeGroupExclusions = (string[]) info.GetValue("AttributeGroupExclusions", typeof (string[]));
            AttributeGroupInclusions = (string[]) info.GetValue("AttributeGroupInclusions", typeof (string[]));
            ExportCrossListNodes = (bool) info.GetValue("ExportCrossListNodes", typeof (bool));
        }

        [Category(CaptionOptional), Description("Global Attributes included in the Review file"),
         PropertyOrder(OptionalBaseOrder + 2)]
        [DisplayName(@"Global Attributes")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] GlobalAttributes
        {
            get { return globalAttributes; }
            set { globalAttributes = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 3)]
        [DisplayName(@"Consider SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuInclusions
        {
            get { return skuInclusions; }
            set { skuInclusions = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 4)]
        [DisplayName(@"Ignore SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuExclusions
        {
            get { return skuExclusions; }
            set { skuExclusions = value; }
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 5)]
        [DisplayName(@"Group Exclusions"), Description("Attribute Groups to be excluded")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] AttributeGroupExclusions
        {
            get { return attributeGroupExclusions; }
            set { attributeGroupExclusions = value; }
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 6)]
        [DisplayName(@"Group Inclusions"), Description("Attribute Groups to be excluded")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] AttributeGroupInclusions
        {
            get { return attributeGroupInclusions; }
            set { attributeGroupInclusions = value; }
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 7)]
        [DisplayName(@"Export Cross-List nodes"), Description("Include Cross-List nodes in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportCrossListNodes { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("GlobalAttributes", GlobalAttributes);
            info.AddValue("SkuInclusions", SkuInclusions);
            info.AddValue("SkuExclusions", SkuExclusions);
            //info.AddValue("SkuValueInclusions", SkuValueInclusions);
            info.AddValue("AttributeGroupExclusions", AttributeGroupExclusions);
            info.AddValue("AttributeGroupInclusions", AttributeGroupInclusions);
            info.AddValue("ExportCrossListNodes", ExportCrossListNodes);
        }

        public override void Run()
        {
            State = WorkerState.Working;
            parsedSkuExclusions = ParseSkuInclusionAndExclusions(SkuExclusions);
            parsedSkuInclusions = ParseSkuInclusionAndExclusions(SkuInclusions);
            //parsedSkuValueInclusions = ParseSkuValueInclusions(SkuValueInclusions);

            taxonomyAttributesCache = new Dictionary<Guid, List<KeyValuePair<Attribute, SchemaData>>>();
            baseAttributeNames = new Dictionary<string, string>();
            delimiter = FieldDelimiter.GetValue().ToString();

            //Create new context
            currentDb = new SkuDataDbDataContext();
            var dlo = new DataLoadOptions();
            projectField1Name = AryaTools.Instance.InstanceData.CurrentProject.EntityField1Name ?? string.Empty;
            dlo.LoadWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas);
            dlo.LoadWith<EntityInfo>(entityInfo => entityInfo.EntityDatas);
            dlo.LoadWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas);

            dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas.Where(p => p.Active));
            dlo.AssociateWith<EntityInfo>(entityInfo => entityInfo.EntityDatas.Where(p => p.Active));
            dlo.AssociateWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas.Where(p => p.Active));
            dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.SkuInfos.Where(p => p.Active));

            currentDb.LoadOptions = dlo;
            currentDb.CommandTimeout = 2000;

            currentDb.Connection.Open();
            currentDb.Connection.ChangeDatabase(AryaTools.Instance.InstanceData.Dc.Connection.Database);

            InitGlobals(GlobalAttributes.ToList());

            StatusMessage = "Init";

            var fi = new FileInfo(ExportFileName);
            var baseFileName = fi.FullName.Replace(fi.Extension, string.Empty);

            attributeDataFile = new StreamWriter(baseFileName + "_AttributeData.txt", false, Encoding.UTF8);

            attributeDataFile.Write("ItemId{0}Taxonomy{0}Node Type", delimiter);
            foreach (var attribute in globalAttributeHeaders)
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
                attributeDataFile.Write("{0}{1}", delimiter, attributeHeader);
            }

            attributeDataFile.WriteLine("{0}Rank 1{0}Att 1{0}Val 1{0}Uom 1{0}[...]", delimiter);

            CurrentProgress = 0;

            if (SkuCollection != null && SkuCollection.Any())
            {
                var taxonomies =
                    (SkuCollection.Split(2000)
                        .SelectMany(skus => currentDb.Skus.Where(s => skus.Contains(s.ItemID)))
                        .GroupBy(s => s.Taxonomy)).ToList();

                MaximumProgress = taxonomies.Count;

                foreach (var st in taxonomies)
                {
                    WriteSkusToFile(st.Key, st.ToList());
                    CurrentProgress++;
                }
            }
            else
            {
                var allExportTaxonomyIds =
                    Taxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).Distinct().ToList();
                var exportTaxonomies = currentDb.TaxonomyInfos.Where(p => allExportTaxonomyIds.Contains(p.ID)).ToList();
                var allExportChildTaxonomies = exportTaxonomies.SelectMany(p => p.AllChildren2).Distinct().ToList();

                MaximumProgress = allExportChildTaxonomies.Count;

                foreach (var exportChildTaxonomy in allExportChildTaxonomies)
                {
                    WriteTaxonomyToFile(exportChildTaxonomy);
                    CurrentProgress++;
                }
            }

            attributeDataFile.Close();

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
                allSkus = from sku in currentDb.Skus
                    where sku.SkuInfos.Any(si => si.Active && si.TaxonomyInfo == taxonomy)
                    select sku;
            }

            StatusMessage = string.Format("{1}{0}Filtering SKUs", Environment.NewLine, taxonomy);
            var skus = GetFilteredSkuList(allSkus, true, parsedSkuInclusions, parsedSkuExclusions).ToList();

            WriteSkusToFile(taxonomy, skus);
        }

        private void WriteSkusToFile(TaxonomyInfo taxonomy, List<Sku> skus)
        {
            var iCtr = 0;
            var noOfSkus = skus.Count;
            skus.ForEach(sku =>
                         {
                             StatusMessage = string.Format("{1}{0}{2} of {3} SKUs", Environment.NewLine, taxonomy,
                                 (++iCtr), noOfSkus);

                             if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                             {
                                 var originalTaxonomy = sku.Taxonomy;
                                 var attributes = GetExportAttributes(originalTaxonomy);
                                 WriteAttributeDataToFile(sku, "Cross List", attributes);
                             }
                             else
                             {
                                 var attributes = GetExportAttributes(taxonomy);
                                 WriteAttributeDataToFile(sku, TaxonomyInfo.NodeTypeRegular, attributes);
                             }
                         });

            StatusMessage = string.Format("{1}{0}({2} of {3})", Environment.NewLine, taxonomy, iCtr, noOfSkus);
        }

        private void WriteAttributeDataToFile(Sku sku, string nodeType,
            IEnumerable<KeyValuePair<Attribute, SchemaData>> attributeOrders)
        {
            var itemHeader = string.Format("{1}{0}{2}{0}{3}", delimiter, sku.ItemID, sku.Taxonomy, nodeType);
            var itemGlobals = string.Empty;
            var itemAttributeValues = string.Empty;

            for (var iCtr = 0; iCtr < globalAttributeNames.Count(); iCtr++)
            {
                var headerCount = globalAttributeHeaders[iCtr].Split(':').Count() > 1
                    ? globalAttributeHeaders[iCtr].Split(':')[1].Trim()
                    : string.Empty;

                int noOfHeaders;
                Int32.TryParse(headerCount, out noOfHeaders);

                var globalDelimiter = noOfHeaders == 0 ? ", " : "\t";

                var parts = globalAttributeNames[iCtr].Split(new[] {','});
                var values = new List<EntityData>();
                foreach (var att in parts)
                    values.AddRange(sku.GetValuesForAttribute(att.Trim()));

                var stringValue =
                    values.Select(val => val.Value)
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

                itemGlobals += delimiter + stringValue;
            }

            foreach (var att in attributeOrders)
            {
                var attribute = att.Key;
                var schemaData = att.Value;
                IEnumerable<EntityData> invalidEntities = null;
                var entity = sku.GetValuesForAttribute(attribute.AttributeName);
                if (attribute.AttributeType != "Derived")
                {
                    invalidEntities = entity.Where(e => !Validate.IsValidDataType(e, schemaData));
                }

                if (invalidEntities == null || !invalidEntities.Any())
                    continue;
                //if (!invalidEntities.Any())
                //    continue;

                //if (!invalidEntities.Any())
                //    continue;

                var value = entity.Aggregate(string.Empty,
                    (current, ed) => current + ((string.IsNullOrEmpty(current) ? string.Empty : ", ") + ed.Value));

                //display the uom once if there is one distinct uom, otherwise, output in the same order as the value
                var distinctUoms = entity.Where(ed => ed.Uom != null).Select(ed => ed.Uom).Distinct().ToList();
                string uom;
                switch (distinctUoms.Count)
                {
                    case 0:
                        uom = string.Empty;
                        break;
                    case 1:
                        uom = distinctUoms.First();
                        break;
                    default:
                        uom = entity.Aggregate(string.Empty,
                            (current, u) => current + ((string.IsNullOrEmpty(current) ? string.Empty : ", ") + u.Uom));
                        break;
                }

                var rank = Decimal.Truncate(schemaData.NavigationOrder) + " • "
                           + Decimal.Truncate(schemaData.DisplayOrder);

                itemAttributeValues += string.Format("{0}{1}{0}{2}{0}{3}{0}{4}", delimiter, rank,
                    attribute.AttributeName, value, uom);
            }

            if (!string.IsNullOrEmpty(itemAttributeValues))
                attributeDataFile.WriteLine(itemHeader + itemGlobals + itemAttributeValues);
        }

        private IEnumerable<KeyValuePair<Attribute, SchemaData>> GetExportAttributes(TaxonomyInfo taxonomy)
        {
            if (taxonomyAttributesCache.ContainsKey(taxonomy.ID))
                return taxonomyAttributesCache[taxonomy.ID];
            var schemaInfos = new List<SchemaInfo>();
            schemaInfos.AddRange(taxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList());

            var attributes = (from si in schemaInfos
                let rank = GetRank(si.SchemaData, SortOrder.OrderbyNavigationDisplay)
                where si.SchemaData.InSchema
                orderby rank, si.Attribute.AttributeName
                select new KeyValuePair<Attribute, SchemaData>(si.Attribute, si.SchemaData)).ToList();

            if (AttributeGroupExclusions.Length > 0)
            {
                //exclude Attribute Groups
                attributes =
                    attributes.Where(
                        p => p.Key.Group == null || (!AttributeGroupExclusions.Contains(p.Key.Group.ToLower())))
                        .ToList();
            }

            if (AttributeGroupInclusions.Length > 0)
            {
                //include Attribute Groups
                attributes =
                    attributes.Where(
                        p => p.Key.Group == null || (AttributeGroupInclusions.Contains(p.Key.Group.ToLower()))).ToList();
            }

            if ((AttributeGroupExclusions.Length > 0 && AttributeGroupExclusions.Contains("default"))
                || (AttributeGroupInclusions.Length > 0 && !AttributeGroupInclusions.Contains("default")))
                attributes = attributes.Where(p => p.Key.Group != null).ToList();

            taxonomyAttributesCache.Add(taxonomy.ID, attributes);

            return attributes;
        }

        private void InitGlobals(List<string> globals)
        {
            var globalCount = 0;
            if (globals != null)
                globalCount = globals.Count();
            globalAttributeNames = new string[globalCount];
            globalAttributeHeaders = new string[globalCount];

            if (globals == null)
                return;

            var iCtr = 0;
            foreach (var global in globals)
            {
                var parts = global.Split(new[] {'='});
                if (parts.Count() == 1)
                {
                    globalAttributeNames[iCtr] = parts[0].Trim();
                    globalAttributeHeaders[iCtr] = parts[0].Trim();
                }
                else
                {
                    globalAttributeNames[iCtr] = parts[0].Trim();
                    globalAttributeHeaders[iCtr] = parts[1].Trim();
                }
                iCtr++;
            }
        }
    }
}