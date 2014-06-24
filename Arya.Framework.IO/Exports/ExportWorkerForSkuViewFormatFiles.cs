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
using Natalie.Framework.Common;
using Natalie.Framework.ComponentModel;
using Natalie.Framework.Extensions;
using Natalie.HelperClasses;
using Attribute = Natalie.Framework.Data.NatalieDb.Attribute;
using ExtendedTaxonomyInfo = Natalie.Framework.ComponentModel.ExtendedTaxonomyInfo;
using Natalie.Framework.Data.NatalieDb;


namespace Natalie.Framework.IO.Exports
{
    [Serializable]
    public class ExportWorkerForSkuViewFormatFiles : ExportWorkerBase
    {
        #region GroupSkusByNodeLevel enum

        public enum GroupSkusByNodeLevel
        {
            Level2,
            Level3,
            LeafNode
        }

        #endregion

        private const string Bullet = " • ";

        private readonly Regex _rxAlphaNumeric = new Regex("[^A-Za-z0-9]");

        private string _baseFileName;
        private NatalieDbDataContext _currentDb;
        private string _delimiter;
        private string[] _globalAttributes = new string[0];
        private Dictionary<string, List<string>> _parsedSkuExclusions;

        private Dictionary<string, List<string>> _parsedSkuInclusions;
        private string _separator;

        private string[] _skuExclusions = new string[0];
        private string[] _skuInclusions = new string[0];
        //private TextWriter _taxonomyFile;

        public Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { _currentProject = value; }

        }

        public User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }

        }

  
        public ExportWorkerForSkuViewFormatFiles(string argumentFilePath, PropertyGrid ownerPropertyGrid)
            : base(argumentFilePath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            AllowMultipleTaxonomySelection = true;
            IgnoreT1Taxonomy = false;
        }

        public ExportWorkerForSkuViewFormatFiles(string argumentFilePath, SerializationInfo info, StreamingContext ctxt)
            : base(argumentFilePath, info, ctxt)
        {
            IgnoreT1Taxonomy = (bool) info.GetValue("IgnoreT1Taxonomy", typeof (bool));
            GlobalAttributes = (string[]) info.GetValue("GlobalAttributes", typeof (string[]));
            SkuInclusions = (string[]) info.GetValue("SkuInclusions", typeof (string[]));
            SkuExclusions = (string[]) info.GetValue("SkuExclusions", typeof (string[]));
            InSchemaOnly = (bool) info.GetValue("InSchemaOnly", typeof (bool));
            ExportCrossListNodes = (bool) info.GetValue("ExportCrossListNodes", typeof (bool));
        }

        [Description("The taxonomy node level by which to group SKUs by"), DisplayName(@"Group SKUs by Node Level"),
         Category(CaptionRequired), DefaultValue(GroupSkusByNodeLevel.Level3), PropertyOrder(RequiredBaseOrder + 11)]
        [TypeConverter(typeof (CustomEnumConverter))]
        public GroupSkusByNodeLevel GroupSkusBy { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 1)]
        [DefaultValue(false)]
        [DisplayName(@"Ignore Taxonomy T1"), Description("Eliminates the top level node name in the export file")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IgnoreT1Taxonomy { get; set; }

        [Category(CaptionOptional), Description("Global Attributes included in the Review file"),
         PropertyOrder(OptionalBaseOrder + 2)]
        [DisplayName(@"Global Attributes")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] GlobalAttributes
        {
            get { return _globalAttributes; }
            set { _globalAttributes = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 3)]
        [DisplayName(@"Consider SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuInclusions
        {
            get { return _skuInclusions; }
            set { _skuInclusions = value; }
        }

        [Category(CaptionOptional), Description("<attr>(=(%)<val>(%))"), PropertyOrder(OptionalBaseOrder + 4)]
        [DisplayName(@"Ignore SKUs")]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SkuExclusions
        {
            get { return _skuExclusions; }
            set { _skuExclusions = value; }
        }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 5)]
        [DefaultValue(true)]
        [DisplayName(@"InSchema Only"),
         Description(
             "Export only Attributes that are In Schema. If you set this to false, all attributes will be exported")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool InSchemaOnly { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 6)]
        [DefaultValue(false)]
        [DisplayName(@"Concatenate MultiValues"),
         Description(
             "Concatenate Multi-values into a single delimited value. If you set this to false, multi-values will be exported in separate columns"
             )]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ConcatenateMultiValues { get; set; }

        [Category(CaptionOptional), PropertyOrder(OptionalBaseOrder + 15)]
        [DisplayName(@"Export Cross-List nodes"), Description("Include Cross-List nodes in the export")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ExportCrossListNodes { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IgnoreT1Taxonomy", IgnoreT1Taxonomy);
            info.AddValue("GlobalAttributes", GlobalAttributes);
            info.AddValue("SkuInclusions", SkuInclusions);
            info.AddValue("SkuExclusions", SkuExclusions);
            info.AddValue("InSchemaOnly", InSchemaOnly);
            info.AddValue("ExportCrossListNodes", ExportCrossListNodes);
        }

        public override void Run()
        {
            State = WorkerState.Working;
            _parsedSkuExclusions = ParseSkuInclusionAndExclusions(SkuExclusions);
            _parsedSkuInclusions = ParseSkuInclusionAndExclusions(SkuInclusions);

            _delimiter = FieldDelimiter.GetDbValue().ToString();

            //Create new context
            _currentDb = new NatalieDbDataContext();
            var dlo = new DataLoadOptions();
            dlo.LoadWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas);
            dlo.LoadWith<EntityInfo>(entityInfo => entityInfo.EntityDatas);
            dlo.LoadWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas);

            //dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas.Where(p => p.Active));
            dlo.AssociateWith<EntityInfo>(entityInfo => entityInfo.EntityDatas.Where(p => p.Active));
            dlo.AssociateWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas.Where(p => p.Active));
            //dlo.AssociateWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.SkuInfos.Where(p => p.Active));

            _currentDb.LoadOptions = dlo;
            _currentDb.CommandTimeout = 2000;

            _currentDb.Connection.Open();
            _currentDb.Connection.ChangeDatabase(NatalieTools.Instance.InstanceData.Dc.Connection.Database);

            _separator = NatalieTools.Instance.InstanceData.CurrentProject.ListSeparator;

            StatusMessage = "Init";

            var fi = new FileInfo(ExportFileName);
            _baseFileName = fi.FullName.Replace(fi.Extension, string.Empty);

            //_taxonomyFile = new StreamWriter(_baseFileName + "_Classification.txt", false, Encoding.UTF8);
            //_taxonomyFile.WriteLine("ItemId{0}New Taxonomy{0}Node Type{0}Old Taxonomy", _delimiter);

            var exportTaxonomyIds =
                Taxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).Distinct().ToList();
            var exportTaxonomies = _currentDb.TaxonomyInfos.Where(p => exportTaxonomyIds.Contains(p.ID)).ToList();

            var allExportTaxonomies =
                exportTaxonomies.SelectMany(p => p.AllChildren2).Union(exportTaxonomies).Distinct().ToList();

            var exportGroups = allExportTaxonomies.GroupBy(GetTaxPrefix).ToList();

            CurrentProgress = 0;
            MaximumProgress = exportGroups.Count();

            foreach (var grp in exportGroups)
            {
                WriteTaxonomyToFile(grp.Select(g => g), grp.Key);
                CurrentProgress++;
            }

            //_taxonomyFile.Close();

            StatusMessage = "Done!";
            State = WorkerState.Ready;
        }

        private string GetTaxPrefix(TaxonomyInfo tax)
        {
            var parts =
                tax.ToString().Split(new[] {TaxonomyInfo.Delimiter}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var noOfLevels = GroupSkusBy == GroupSkusByNodeLevel.Level2
                ? 2
                : GroupSkusBy == GroupSkusByNodeLevel.Level2 ? 3 : 99;
            parts = parts.Take(noOfLevels).ToList();
            var taxPrefix = parts.Aggregate((full, current) => full + TaxonomyInfo.Delimiter + current);
            return taxPrefix;
        }

        private void WriteTaxonomyToFile(IEnumerable<TaxonomyInfo> taxonomies, string taxPrefix)
        {
            var allSkus = new List<Sku>();
            foreach (var taxonomy in taxonomies)
            {
                StatusMessage = string.Format("{1}{0}Reading Node", Environment.NewLine, taxonomy);

                IQueryable<Sku> skuQuery;
                if (taxonomy.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    if (!ExportCrossListNodes)
                        return;

                    var cl = Query.FetchCrossListObject(taxonomy);
                    if (cl == null)
                        return;
                    skuQuery =
                        Query.GetFilteredSkus(cl.TaxonomyIDFilter, cl.ValueFilters, cl.AttributeTypeFilters,
                            cl.MatchAllTerms).Distinct();
                }
                else
                {
                    var tax = taxonomy;
                    skuQuery = from sku in _currentDb.Skus
                        where sku.SkuInfos.Any(si => si.Active && si.TaxonomyInfo == tax)
                        select sku;
                }

                StatusMessage = string.Format("{1}{0}Filtering SKUs", Environment.NewLine, taxonomy);
                var skus = GetFilteredSkuList(skuQuery, true, _parsedSkuInclusions, _parsedSkuExclusions).ToList();
                allSkus.AddRange(skus);
            }

            WriteAttributeDataToFile(allSkus, _rxAlphaNumeric.Replace(taxPrefix, "-"));
        }

        private void WriteAttributeDataToFile(List<Sku> skus, string filename)
        {
            var taxonomies = skus.Select(sku => sku.Taxonomy).Distinct();

            //Calculate the number of columns required for each attribute, irrespective of whether it will be used or not
            var allAttributes = new Dictionary<Attribute, int>();
            foreach (var sku in skus)
            {
                var attrs = from ei in sku.EntityInfos
                    from ed in ei.EntityDatas
                    where ed.Active
                    group ed by ed.Attribute
                    into grp select grp;

                foreach (var att in attrs)
                {
                    if (!allAttributes.ContainsKey(att.Key) || allAttributes[att.Key] < att.Count())
                        allAttributes[att.Key] = att.Count();
                }
            }

            //First, list the requested global attributes
            var usedAttributes = (from globalAttr in _globalAttributes
                let parts = globalAttr.Split('=')
                let attributeName = parts[0]
                let attribute =
                    allAttributes.First(att => att.Key.AttributeName.ToLower().Equals(globalAttr.ToLower())).Key
                select new Tuple<Attribute, string, int>(attribute, string.Empty, allAttributes[attribute])).ToList();

            //Find InSchema Attributes
            var atts = from tax in taxonomies
                from si in tax.SchemaInfos
                from sd in si.SchemaDatas
                where sd.Active && sd.InSchema
                let nav = sd.NavigationOrder == 0 ? 999 : sd.NavigationOrder
                let disp = sd.DisplayOrder == 0 ? 999 : sd.DisplayOrder
                orderby nav, disp, sd.SchemaInfo.Attribute.AttributeName
                select new {sd.SchemaInfo.Attribute, sd.NavigationOrder, sd.DisplayOrder};

            //Append InSchema Attributes
            foreach (var att in atts)
            {
                var usedAtt = usedAttributes.FirstOrDefault(ua => ua.Item1.Equals(att));

                //If an InSchema Attribute does not have any data associated with it, it will have to be added to AllAttributes
                if (!allAttributes.ContainsKey(att.Attribute))
                    allAttributes[att.Attribute] = 1;

                if (usedAtt == null)
                {
                    usedAttributes.Add(new Tuple<Attribute, string, int>(att.Attribute,
                        att.NavigationOrder + Bullet + att.DisplayOrder, allAttributes[att.Attribute]));
                }
                else
                {
                    usedAttributes[usedAttributes.IndexOf(usedAtt)] = new Tuple<Attribute, string, int>(att.Attribute,
                        "x" + Bullet + "y", allAttributes[att.Attribute]);
                }
            }

            if (!InSchemaOnly)
            {
                //Find and append remaining (unlisted) attributes
                foreach (var attr in allAttributes)
                {
                    var usedAtt = usedAttributes.FirstOrDefault(ua => ua.Item1.Equals(attr.Key));
                    if (usedAtt == null)
                        usedAttributes.Add(new Tuple<Attribute, string, int>(attr.Key, string.Empty, attr.Value));
                }
            }

            //Generate Attribute Header
            var attributeHeader = string.Format("ItemId{0}Taxonomy", _delimiter);
            foreach (var tuple in usedAttributes)
            {
                var colCount = ConcatenateMultiValues ? 1 : tuple.Item3;
                for (var ctr = 0; ctr < colCount; ctr++)
                {
                    attributeHeader += string.Format("{0}{1}{2}{3}", _delimiter, tuple.Item1.AttributeName, Bullet,
                        ctr == 0 ? tuple.Item2 : string.Format("({0})", ctr));
                }
            }

            //Create new file and write Header
            TextWriter attributeDataFile =
                new StreamWriter(string.Format("{0}_AttributeData_{1}.txt", _baseFileName, filename), false,
                    Encoding.UTF8);
            attributeDataFile.WriteLine(attributeHeader);

            foreach (var sku in skus)
            {
                var thisLine = string.Format("{0}{1}{2}", sku.ItemID, _delimiter, sku.Taxonomy);
                foreach (var attribute in usedAttributes)
                {
                    var values = sku.GetValuesForAttribute(attribute.Item1);
                    for (var ctr = 0; ctr < attribute.Item3; ctr++)
                    {
                        //Delimiter or Separator
                        if (ConcatenateMultiValues && ctr > 0)
                            thisLine += _separator;
                        else
                            thisLine += _delimiter;

                        if (values.Count <= ctr)
                            continue;

                        //Value
                        thisLine += values[ctr];

                        if (string.IsNullOrWhiteSpace(values[ctr].Uom))
                            continue;

                        //UOM
                        thisLine += values[ctr].Uom;
                    }
                }
                attributeDataFile.WriteLine(thisLine);
            }

            attributeDataFile.Close();
        }
    }
}