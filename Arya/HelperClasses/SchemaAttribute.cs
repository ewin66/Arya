using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FreeHandFilters;
using FreeHandFilters.Filters;
using LinqKit;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.UserControls;
using Attribute = Arya.Data.Attribute;
using SchemaData = Arya.Data.SchemaData;
using SchemaInfo = Arya.Data.SchemaInfo;
using SchemaMetaData = Arya.Data.SchemaMetaData;
using SchemaMetaInfo = Arya.Data.SchemaMetaInfo;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;

namespace Arya.HelperClasses
{
    public class SchemaAttribute
    {
        #region Fields

        public const string SpecialSchematusAttributeType = "Attribute Type";
        public const string SpecialSchematusIsMapped = "Is Mapped";
        public const string SpecialSchematusListOfValues = "List of Values";
        private const string DefaultFillRateFilter = "@FN_\"FillRate\"\nBEGIN\nnofilter\nEND";

        public static readonly SchemaAttribute SchemaAttributeAttributeType =
            new SchemaAttribute(SpecialSchematusAttributeType);

        public static readonly SchemaAttribute SchemaAttributeDataType = new SchemaAttribute("Data Type",
            typeof(string));

        public static readonly SchemaAttribute SchemaAttributeDisplayOrder = new SchemaAttribute("Display Order",
            typeof(decimal));

        public static readonly SchemaAttribute SchemaAttributeInSchema = new SchemaAttribute("In Schema", typeof(bool));
        public static readonly SchemaAttribute SchemaAttributeIsMapped = new SchemaAttribute(SpecialSchematusIsMapped);

        public static readonly SchemaAttribute SchemaAttributeListOfValues =
            new SchemaAttribute(SpecialSchematusListOfValues);

        public static readonly SchemaAttribute SchemaAttributeNavigationOrder = new SchemaAttribute("Navigation Order",
            typeof(decimal));

        public static readonly List<SchemaAttribute> PrimarySchemati = new List<SchemaAttribute>
                                                                       {
                                                                           SchemaAttributeInSchema,
                                                                           SchemaAttributeIsMapped,
                                                                           SchemaAttributeAttributeType,
                                                                           SchemaAttributeDataType,
                                                                           SchemaAttributeNavigationOrder,
                                                                           SchemaAttributeDisplayOrder,
                                                                           SchemaAttributeListOfValues
                                                                       };

        private static readonly string[] LovForDataType =
        {
            "Text", "LOV", "Number", "Integer", "Fraction", "Boolean",
            "Numeric Text"
        };

        private static List<SchemaAttribute> _fillRateSchemati;
        private static List<SchemaAttribute> _secondarySchemati;

        #endregion Fields

        #region Constructors

        public SchemaAttribute(Attribute metaAttribute, Type dataType)
        {
            MetaSchemaAttribute = metaAttribute;
            DataType = dataType;
        }

        private SchemaAttribute(string schemaAttribute, Type dataType)
        {
            PrimarySchemaAttribute = schemaAttribute;
            DataType = dataType;
        }

        private SchemaAttribute(string specialAttribute)
        {
            SpecialSchemaAttribute = specialAttribute;
            DataType = typeof(string);
        }

        private SchemaAttribute(Filter filter)
        {
            FillRateSchemaAttribute = filter;
            DataType = typeof(decimal);
        }

        #endregion Constructors

        #region Enumerations

        public enum SchemaAttributeType
        {
            Primary,
            Meta,
            Special,
            FillRate,
            Unknown
        }

        #endregion Enumerations

        #region Properties

        public static List<SchemaAttribute> FillRateSchemati
        {
            get
            {
                if (_fillRateSchemati == null)
                {
                    var filters =
                        FilterParser<CustomFilters>.CurrentContext.BuildPredicate(
                            AryaTools.Instance.InstanceData.CurrentProject.SchemaFillRateFilters
                            ?? DefaultFillRateFilter).ToList();

                    _fillRateSchemati = filters.Select(filter => new SchemaAttribute(filter)).ToList();
                }
                return _fillRateSchemati;
            }

        }

        public static List<SchemaAttribute> SecondarySchemati
        {
            get
            {
                if (_secondarySchemati == null)
                {
                    var emptyGuid = new Guid();
                    var metas = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                                 where
                                     att.ProjectID.Equals(AryaTools.Instance.InstanceData.CurrentProject.ID)
                                     && att.AttributeType.Equals(AttributeTypeEnum.SchemaMeta.ToString())
                                     && att.SchemaInfos.Any(si => si.SchemaDatas.Any(sd => sd.InSchema && sd.Active))
                                 let displayRank = (from si in att.SchemaInfos
                                                    where si.TaxonomyID.Equals(emptyGuid)
                                                    from sd in si.SchemaDatas
                                                    where sd.Active
                                                    select sd.DisplayOrder).FirstOrDefault()
                                 orderby displayRank, att.AttributeName
                                 select att).ToList();
                    _secondarySchemati = metas.Select(GetMetaAttributeType).ToList();
                }
                return _secondarySchemati;
            }
            set { _secondarySchemati = value; }
        }

        public SchemaAttributeType AttributeType
        {
            get
            {
                return !String.IsNullOrEmpty(PrimarySchemaAttribute)
                    ? SchemaAttributeType.Primary
                    : MetaSchemaAttribute != null
                        ? SchemaAttributeType.Meta
                        : SpecialSchemaAttribute != null
                            ? SchemaAttributeType.Special
                            : FillRateSchemaAttribute != null
                                ? SchemaAttributeType.FillRate
                                : SchemaAttributeType.Unknown;
            }
        }

        //public string MetaAttributeName
        //{
        //    get
        //    {
        //        return !String.IsNullOrEmpty(PrimarySchemaAttribute)
        //            ? PrimarySchemaAttribute
        //            : MetaSchemaAttribute != null
        //                ? MetaSchemaAttribute.AttributeName
        //                : SpecialSchemaAttribute
        //                  ?? (FillRateSchemaAttribute != null ? FillRateSchemaAttribute.GetFilterName() : string.Empty);
        //    }
        //}
        public Type DataType { get; private set; }

        public Filter FillRateSchemaAttribute { get; private set; }

        public Dictionary<string, string> MetaMetaSchemaAttributes
        {
            get
            {
                var metaMetas = new Dictionary<string, string> { { "", ToString() } };

                var amis =
                    MetaSchemaAttribute.AttributeMetaInfos.Where(ami => ami.Attribute1.AttributeType == "SchemaMetaMeta")
                        .OrderBy(a => a.Attribute1.AttributeName);
                foreach (var ami in amis)
                {
                    var amName = ami.MetaAttribute.AttributeName;
                    var amValue = ami.AttributeMetaDatas.FirstOrDefault(a => a.Active);
                    if (amValue != null)
                        metaMetas.Add(amName, amValue.Value);
                }

                return metaMetas;
            }
        }

        public Attribute MetaSchemaAttribute { get; private set; }

        public string PrimarySchemaAttribute { get; private set; }

        public string SpecialSchemaAttribute { get; private set; }

        #endregion Properties

        #region Methods

        public static void AutoOrderRanks(TaxonomyInfo taxonomy, SchemaData schemaData)
        {
            if (taxonomy == null)
                return;

            var sds = (from si in taxonomy.SchemaInfos
                       let sd = si.SchemaData
                       where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                       select sd).ToDictionary(sd => sd, sd => new[] { sd.NavigationOrder, sd.DisplayOrder });

            if (!sds.Any())
                return;

            var baseNavOrder = schemaData != null ? schemaData.NavigationOrder : 0;
            var baseDispOrder = schemaData != null ? schemaData.DisplayOrder : 0;

            var navs = (from sd in sds
                        where
                            sd.Key.NavigationOrder > 0 && sd.Key.NavigationOrder >= baseNavOrder
                            && (schemaData == null || !sd.Key.Equals(schemaData))
                        select sd.Key).ToList();
            foreach (var nav in navs)
                sds[nav][0] = nav.NavigationOrder + 1;

            var iCtr = 0;
            foreach (var nav in
                (from sd in sds where sd.Value[0] > 0 orderby sd.Value[0] select sd.Key).ToList())
                sds[nav][0] = ++iCtr;

            var disps = (from sd in sds
                         where
                             sd.Key.DisplayOrder > 0 && sd.Key.DisplayOrder >= baseDispOrder
                             && (schemaData == null || !sd.Key.Equals(schemaData))
                         select sd.Key).ToList();

            foreach (var disp in disps)
                sds[disp][1] = disp.DisplayOrder + 1;

            iCtr = 0;
            foreach (var disp in
                (from sd in sds where sd.Value[1] > 0 orderby sd.Value[1] select sd.Key).ToList())
                sds[disp][1] = ++iCtr;

            foreach (var kvp in sds)
            {
                var schemaInfo = kvp.Key.SchemaInfo;
                SetValue(schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeNavigationOrder,
                    kvp.Value[0]);
                SetValue(schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeDisplayOrder, kvp.Value[1]);
            }
        }

        // Public Methods (5)
        public static void GetAttributeOrders(Attribute attribute, TaxonomyInfo taxonomy, out decimal navAttOrder,
            out decimal dispAttOrder, out bool inSchema)
        {
            navAttOrder = 0;
            dispAttOrder = 0;
            inSchema = false;

            if (taxonomy == null || attribute == null)
                return;

            inSchema = GetValue(taxonomy, attribute, SchemaAttributeInSchema) != null;

            var nav = GetValue(taxonomy, attribute, SchemaAttributeNavigationOrder);
            navAttOrder = nav != null ? (decimal)nav : 0;

            var disp = GetValue(taxonomy, attribute, SchemaAttributeDisplayOrder);
            dispAttOrder = disp != null ? (decimal)disp : 0;
        }

        public static string GetMetaAttributeValue(Attribute attribute, Attribute metaAttribute, TaxonomyInfo taxonomy)
        {
            try
            {
                var si = taxonomy.SchemaInfos.FirstOrDefault(a => a.Attribute == attribute);
                //var si = Arya.SchemaInfos.Where(t => t.TaxonomyID == Guid.Empty && t.Attribute == attribute ).FirstOrDefault();
                //if (si == null)
                //    return null;

                var smi = si.SchemaMetaInfos.FirstOrDefault(mi => mi.MetaAttributeID == metaAttribute.ID);

                //if (smi == null)
                //    return null;

                var smd = smi.SchemaMetaDatas.FirstOrDefault(a => a.Active);

                //if (smd == null)
                //    return null;
                //else
                return smd.Value;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static object GetValue(TaxonomyInfo taxonomy, Attribute attribute, SchemaAttribute schematus)
        {
            if (taxonomy == null && attribute == null)
                return schematus;

            if (taxonomy == null)
                return attribute;

            if (attribute == null)
                return taxonomy;

            var schemaInfo =
                taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute != null && si.Attribute.Equals(attribute));
            if (schematus == null || schemaInfo == null)
                return null;

            switch (schematus.AttributeType)
            {
                case SchemaAttributeType.Primary:
                    var activeSchemaData = schemaInfo.SchemaDatas.FirstOrDefault(sd => sd.Active);
                    if (activeSchemaData == null)
                        return null;

                    var valueProperty =
                        activeSchemaData.GetType()
                            .GetProperty(schematus.PrimarySchemaAttribute.Replace(" ", String.Empty));
                    return valueProperty.GetValue(activeSchemaData, null);

                case SchemaAttributeType.Meta:
                    var metaSchemaData = (from mi in schemaInfo.SchemaMetaInfos
                                          where mi.Attribute.Equals(schematus.MetaSchemaAttribute) && mi.SchemaMetaData != null
                                          select mi.SchemaMetaData).FirstOrDefault();
                    if (schematus.MetaSchemaAttribute.AttributeName == Framework.Properties.Resources.SchemaEnrichmentImageAttributeName)
                    {
                        if (metaSchemaData == null || string.IsNullOrEmpty(metaSchemaData.Value))
                            return metaSchemaData;

                        Guid guidValue;
                        var isGuid = Guid.TryParse(metaSchemaData.Value, out guidValue);
                        return isGuid ? GetImageName(taxonomy, new Guid(metaSchemaData.Value)) : metaSchemaData.Value;
                        //AryaTools.Instance.InstanceData.Dc.Skus.First(sk => sk.ID.ToString() == metaSchemaData.ToString()).EntityInfos.Where(ei => ei.EntityDatas.Where(ed => ed.Attribute.AttributeName == "Image FileName"));
                        //Sku imageSku = taxonomy.SkuInfos.FirstOrDefault(si => si.Sku.SkuType == Framework.Data.AryaDb.Sku.ItemType.EnrichmentImage.ToString()).Sku;
                    }

                    return metaSchemaData;

                case SchemaAttributeType.Special:
                    if (schematus.SpecialSchemaAttribute.Equals(SpecialSchematusIsMapped))
                    {
                        var activeSd = schemaInfo.SchemaDatas.FirstOrDefault(sd => sd.Active);
                        if (activeSd != null)
                            return "x";
                    }

                    if (schematus.SpecialSchemaAttribute.Equals(SpecialSchematusAttributeType))
                    {
                        if (attribute.AttributeType == "Sku")
                            return "Product";
                        return attribute.Type;
                    }
                       

                    if (schematus.SpecialSchemaAttribute.Equals(SpecialSchematusListOfValues))
                    {
                        var listOfValues = schemaInfo.ListOfValues.Where(lov => lov.Active).Select(lov => lov).ToList();
                        if (listOfValues.Any())
                        {
                            var listSep =
                                AryaTools.Instance.InstanceData.CurrentProject.ProjectPreferences.ListSeparator
                                ?? "; ";
                            return
                                listOfValues.OrderBy(val => val.DisplayOrder)
                                    .ThenBy(val => val.Value)
                                    .Select(val => val.Value)
                                    .Aggregate((current, lov) => current + listSep + lov);
                        }
                        return null;
                    }
                    return null;

                case SchemaAttributeType.FillRate:
                    var fillRate = AryaTools.Instance.AllFillRates.GetFillRate(taxonomy, attribute,
                        schematus.FillRateSchemaAttribute);
                    if (fillRate == Double.MinValue)
                    {
                        SchemaDataGridView.RefreshGridView = true;
                        return "Calculating...";
                    }

                    return fillRate;

                default:
                    return null;
            }
        }

        public static void OrderByDisplay(TaxonomyInfo taxonomy)
        {
            if (taxonomy == null)
                return;

            var sds = (from si in taxonomy.SchemaInfos
                       let sd = si.SchemaData
                       where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                       select sd).ToDictionary(sd => sd, sd => new[] { sd.NavigationOrder, sd.DisplayOrder });

            if (!sds.Any())
                return;

            //int iCtr = 0;
            //foreach (SchemaData nav in
            //    (from sd in sds
            //     where sd.Value[0] > 0
            //     orderby sd.Value[0]
            //     select sd.Key).ToList())
            //{
            //    sds[nav][0] = ++iCtr;

            //    if (sds[nav][1] > 0) sds[nav][1] = iCtr;
            //}

            decimal iCtr = 0;
            foreach (var disp in
                (from sd in sds where sd.Value[1] > 0 orderby sd.Value[1] select sd.Key).ToList())
            {
                if (sds[disp][0] > 0)
                    sds[disp][1] = iCtr = sds[disp][0];
                else
                    sds[disp][1] = ++iCtr;
            }

            foreach (var kvp in sds)
            {
                var schemaInfo = kvp.Key.SchemaInfo;
                SetValue(schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeNavigationOrder,
                    kvp.Value[0]);
                SetValue(schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeDisplayOrder, kvp.Value[1]);
            }
        }

        public static void OrderByNavigation(TaxonomyInfo taxonomy)
        {
            if (taxonomy == null)
                return;

            var sds = (from si in taxonomy.SchemaInfos
                       let sd = si.SchemaData
                       where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                       select sd).ToDictionary(sd => sd, sd => new[] { sd.NavigationOrder, sd.DisplayOrder });

            if (!sds.Any())
                return;

            var iCtr = 0;
            foreach (var nav in
                (from sd in sds where sd.Value[0] > 0 orderby sd.Value[0] select sd.Key).ToList())
            {
                sds[nav][0] = ++iCtr;

                if (sds[nav][1] > 0)
                    sds[nav][1] = iCtr;
            }

            //iCtr = 0;
            //foreach (SchemaData disp in
            //    (from sd in sds
            //     where sd.Value[1] > 0
            //     orderby sd.Value[1]
            //     select sd.Key).ToList())
            //    sds[disp][1] = ++iCtr;

            foreach (var kvp in sds)
            {
                var schemaInfo = kvp.Key.SchemaInfo;
                SetValue(schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeNavigationOrder,
                    kvp.Value[0]);
                SetValue(schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeDisplayOrder, kvp.Value[1]);
            }
        }

        public static string SetValue(TaxonomyInfo taxonomy, Attribute attribute, bool autoMapSchema,
            SchemaAttribute schematus, object value, bool suppressAutoMapMessage = false)
        {
            var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
            var newSchemaInfo = false;
            if (schemaInfo == null)
            {
                if (suppressAutoMapMessage)
                    return null;
                if (!autoMapSchema
                    && MessageBox.Show(
                        String.Format("Attribute [{0}] is not mapped in Node [{1}]. Do you want to map it?", attribute,
                            taxonomy.TaxonomyData.NodeName), @"Add to Schema", MessageBoxButtons.YesNo)
                    == DialogResult.No)
                    return null;
                schemaInfo = new SchemaInfo { Attribute = attribute };
                taxonomy.SchemaInfos.Add(schemaInfo);
                newSchemaInfo = true;
            }

            if (schematus == null)
            {
                //Create/Update Schema Info only
                var currentSchemaData = schemaInfo.SchemaData;
                var newSchemaData = new SchemaData();
                if (value != null && value is SchemaData)
                {
                    var fromSchemaData = (SchemaData)value;
                    if (currentSchemaData != null && currentSchemaData.Equals(fromSchemaData))
                        return null;

                    newSchemaData.DataType = fromSchemaData.DataType;
                    newSchemaData.DisplayOrder = fromSchemaData.DisplayOrder;
                    newSchemaData.NavigationOrder = fromSchemaData.NavigationOrder;
                    newSchemaData.InSchema = fromSchemaData.InSchema;
                }

                if (currentSchemaData != null)
                    currentSchemaData.Active = false;
                schemaInfo.SchemaDatas.Add(newSchemaData);
                return null;
            }

            var activeSd = schemaInfo.SchemaData;
            switch (schematus.AttributeType)
            {
                case SchemaAttributeType.Primary:
                    var reallyNewSd = false;
                    var reorderRanks = false;

                    SchemaData newSchemaData;
                    if (activeSd == null)
                    {
                        reallyNewSd = true;
                        newSchemaData = new SchemaData();
                    }
                    else
                    {
                        //activeSd.DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
                        activeSd.DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser;
                        //fixes Object not found error
                        activeSd.DeletedOn = DateTime.Now;
                        newSchemaData = new SchemaData
                                        {
                                            DataType = activeSd.DataType,
                                            InSchema = activeSd.InSchema,
                                            NavigationOrder = activeSd.NavigationOrder,
                                            DisplayOrder = activeSd.DisplayOrder
                                        };
                    }

                    var propertyName = schematus.PrimarySchemaAttribute.Replace(" ", String.Empty);

                    var valueProperty = newSchemaData.GetType().GetProperty(propertyName);
                    var propertyType = valueProperty.PropertyType;

                    if (propertyType == typeof(string))
                    {
                        var stringValue = value ?? String.Empty;
                        if (propertyName == "DataType")
                            stringValue = ValidateDataType(stringValue);

                        if (!(valueProperty.GetValue(newSchemaData, null) ?? String.Empty).Equals(stringValue))
                        {
                            reallyNewSd = true;
                            valueProperty.SetValue(newSchemaData, stringValue.ToString(), null);
                        }
                    }
                    else if (propertyType == typeof(bool))
                    {
                        var boolValue = value ?? false;
                        string firstCharacter;
                        try
                        {
                            firstCharacter = boolValue.ToString().Substring(0, 1).ToLower();
                        }
                        catch (Exception)
                        {
                            firstCharacter = "f";
                        }

                        var newValue = firstCharacter.Equals("t") || firstCharacter.Equals("y")
                                       || firstCharacter.Equals("1");

                        if (!((bool)valueProperty.GetValue(newSchemaData, null)).Equals(newValue))
                        {
                            reallyNewSd = true;
                            valueProperty.SetValue(newSchemaData, newValue, null);
                        }
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        var decimalValue = value ?? 0.0;
                        decimal newDecimalValue;
                        if (Decimal.TryParse(decimalValue.ToString(), out newDecimalValue))
                        {
                            if (!((decimal)valueProperty.GetValue(newSchemaData, null)).Equals(newDecimalValue))
                            {
                                reallyNewSd = true;
                                valueProperty.SetValue(newSchemaData, newDecimalValue, null);
                                reorderRanks = SchemaDataGridView.AutoOrderRanks;
                            }
                        }
                        else
                        {
                            return String.Format("Invalid value [{0}] for [{1}] - [{2}].{3}Expected a decimal value.",
                                decimalValue, taxonomy.NodeName, attribute, Environment.NewLine);
                        }
                    }
                    else
                    {
                        return String.Format("Unknown data type for value [{0}] for [{1}] - [{2}]",
                            value ?? String.Empty, taxonomy.NodeName, attribute);
                    }

                    if (reallyNewSd)
                    {
                        if (activeSd != null)
                            activeSd.Active = false;
                        schemaInfo.SchemaDatas.Add(newSchemaData);

                        if (reorderRanks)
                            AutoOrderRanks(newSchemaData.SchemaInfo.TaxonomyInfo, newSchemaData);
                    }

                    return null;

                case SchemaAttributeType.Meta:
                    var reallyNewSmd = false;

                    if (schematus.DataType == typeof(bool))
                        value = value.ToString().StartsWith("y", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";
                    //First check if there is an active SchemaData
                    if (activeSd == null)
                    {
                        if (suppressAutoMapMessage)
                            return null;

                        if (!newSchemaInfo && !autoMapSchema
                            && MessageBox.Show(
                                String.Format("Attribute [{0}] is not mapped in Node [{1}]. Do you want to map it?",
                                    attribute, taxonomy.TaxonomyData.NodeName), "Add to Schema", MessageBoxButtons.YesNo)
                            == DialogResult.No)
                            return null;

                        schemaInfo.SchemaDatas.Add(new SchemaData());
                        reallyNewSmd = true;
                    }

                    var activeSmi =
                        (from mi in schemaInfo.SchemaMetaInfos
                         where mi.Attribute.Equals(schematus.MetaSchemaAttribute)
                         select mi).FirstOrDefault();

                    if (activeSmi == null)
                    {
                        activeSmi = new SchemaMetaInfo { Attribute = schematus.MetaSchemaAttribute };
                        schemaInfo.SchemaMetaInfos.Add(activeSmi);
                        reallyNewSmd = true;
                    }

                    var activeSmd = activeSmi.SchemaMetaData;

                    if (activeSmd == null)
                        reallyNewSmd = true;
                    else if (value == null || !activeSmd.Value.Equals(value))
                    {
                        reallyNewSmd = true;
                        activeSmd.Active = false;
                    }

                    if (reallyNewSmd && value != null && !String.IsNullOrEmpty(value.ToString()))
                    {
                        activeSmi.SchemaMetaDatas.Add(new SchemaMetaData { Value = value.ToString() });
                        SchemaDataGridView.UpdateAutoCompleteSource(schematus.MetaSchemaAttribute, value.ToString());
                    }

                    return null;

                default:
                    return "Unknown Meta-attribute Type! Weird!!!";
            }
        }

        public static void UnmapNodeAttribute(TaxonomyInfo taxonomy, Attribute attribute)
        {
            var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
            if (schemaInfo == null)
                return;

            schemaInfo.SchemaMetaInfos.SelectMany(smi => smi.SchemaMetaDatas)
                .Where(md => md.Active)
                .ForEach(md => md.Active = false);

            if (schemaInfo.SchemaData != null)
                schemaInfo.SchemaData.Active = false;
        }

        public override string ToString()
        {
            if (PrimarySchemaAttribute != null)
            {
                var parts = PrimarySchemaAttribute.Split('.');
                return parts[parts.Length - 1];
            }

            if (MetaSchemaAttribute != null)
            {
                var parts = MetaSchemaAttribute.AttributeName.Split('.');
                return parts[parts.Length - 1];
            }

            if (SpecialSchemaAttribute != null)
                return SpecialSchemaAttribute;

            if (FillRateSchemaAttribute != null)
                return FillRateSchemaAttribute.GetFilterName();

            return String.Empty;
        }

        private static string GetImageName(TaxonomyInfo taxonomy,Guid skuId)
        {
            var ImagefileName = string.Empty;
            var imageSku = taxonomy.SkuInfos.Select(si => si.Sku).FirstOrDefault(si => si.ID == skuId);
           // var imageSku =  AryaTools.Instance.InstanceData.Dc.Skus.FirstOrDefault(sk => sk.ID == skuId);
            if (imageSku == null)
                return string.Empty;

            using (
                var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                    AryaTools.Instance.InstanceData.CurrentUser.ID))
            {
               
                var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                               {
                                   ImageSku=dc.Skus.FirstOrDefault(sku=>sku.ID== imageSku.ID)
                               };
                ImagefileName = Path.GetFileName(imageMgr.OriginalFileUri);

                return ImagefileName;
            }
        }

        private static SchemaAttribute GetMetaAttributeType(Attribute attribute)
        {
            var attributeName = attribute.AttributeName.ToLower();
            return attributeName.StartsWith("is ") || attributeName.StartsWith("in ")
                ? new SchemaAttribute(attribute, typeof(bool))
                : new SchemaAttribute(attribute, typeof(string));
        }

        // Private Methods (1)
        private static string ValidateDataType(object stringValue)
        {
            try
            {
                var x = Convert.ToUInt32(stringValue);
                return x < 15 ? x.ToString() : "Text";
            }
            catch
            {
                return LovForDataType.Contains(stringValue.ToString()) ? stringValue.ToString() : "Text";
            }
        }

        #endregion Methods
    }
    
}