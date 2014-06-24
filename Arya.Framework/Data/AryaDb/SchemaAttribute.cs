using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FreeHandFilters;
using FreeHandFilters.Filters;
using LinqKit;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using SchemaData = Arya.Framework.Data.AryaDb.SchemaData;
using SchemaInfo = Arya.Framework.Data.AryaDb.SchemaInfo;
using SchemaMetaData = Arya.Framework.Data.AryaDb.SchemaMetaData;
using SchemaMetaInfo = Arya.Framework.Data.AryaDb.SchemaMetaInfo;
using TaxonomyInfo = Arya.Framework.Data.AryaDb.TaxonomyInfo;

namespace Arya.Framework.Data.AryaDb
{
    public class SchemaAttribute
    {
		#region Fields (12) 
        AryaDbDataContext Dc;
        //private static List<SchemaAttribute> _fillRateSchemati;
        private static List<SchemaAttribute> _secondarySchemati;
        //private const string DefaultFillRateFilter = "@FN_\"FillRate\"\nBEGIN\nnofilter\nEND";
        private static readonly string[] LovForDataType = new[]
                                                              {
                                                                  "Text", "LOV", "Number", "Integer", "Fraction", "Boolean"
                                                                  , "Numeric Text"
                                                              };
        public static readonly SchemaAttribute SchemaAttributeAttributeType =
            new SchemaAttribute(SpecialSchematusAttributeType);
        public static readonly SchemaAttribute SchemaAttributeDataType = new SchemaAttribute(
            "Data Type", typeof (string));
        public static readonly SchemaAttribute SchemaAttributeDisplayOrder = new SchemaAttribute(
            "Display Order", typeof (decimal));
        public static readonly SchemaAttribute SchemaAttributeInSchema = new SchemaAttribute("In Schema", typeof (bool));
        public static readonly SchemaAttribute SchemaAttributeIsMapped = new SchemaAttribute(SpecialSchematusIsMapped);
        public static readonly SchemaAttribute SchemaAttributeNavigationOrder = new SchemaAttribute(
            "Navigation Order", typeof (decimal));
        public static readonly SchemaAttribute SchemaAttributeListOfValues = new SchemaAttribute(SpecialSchematusListOfValues);

        public const string SpecialSchematusAttributeType = "Attribute Type";
        public const string SpecialSchematusIsMapped = "Is Mapped";
        public const string SpecialSchematusListOfValues = "List of Values";

		#endregion Fields 

        #region Properties

        private Project Currentproject { get; set; }

        private User CurrentUser { get; set; }

        #endregion Properties

        #region Constructors (4)
        public SchemaAttribute()
        {
            Currentproject = Currentproject;
            CurrentUser = CurrentUser;
            Dc = new AryaDbDataContext();
        }
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
            DataType = typeof (string);
        }

        //private SchemaAttribute(Filter filter)
        //{
        //    FillRateSchemaAttribute = filter;
        //    DataType = typeof (decimal);
        //}

		#endregion Constructors 

		#region Properties (8) 

        public SchemaAttributeType AttributeType
        {
            get
            {
                return !String.IsNullOrEmpty(PrimarySchemaAttribute)
                           ? SchemaAttributeType.Primary
                           : MetaSchemaAttribute != null
                                 ? SchemaAttributeType.Meta
                                 //: SpecialSchemaAttribute != null
                                 //      ? SchemaAttributeType.Special
                                 //      : FillRateSchemaAttribute != null
                                 //            ? SchemaAttributeType.FillRate
                                             : SchemaAttributeType.Unknown;
            }
        }

        public Type DataType { get; private set; }

        //public Filter FillRateSchemaAttribute { get; private set; } 

        //public static List<SchemaAttribute> FillRateSchemati
        //{
        //    get
        //    {
        //        if (_fillRateSchemati == null)
        //        {
        //            var filters =
        //                FilterParser<CustomFilters>.CurrentContext.BuildPredicate(
        //                    AryaTools.Instance.InstanceData.CurrentProject.SchemaFillRateFilters ?? DefaultFillRateFilter).ToList();

        //            _fillRateSchemati = filters.Select(filter => new SchemaAttribute(filter)).ToList();
        //        }
        //        return _fillRateSchemati;
        //    }
        //}

        public Attribute MetaSchemaAttribute { get; private set; }

        public string PrimarySchemaAttribute { get; private set; }

        public  List<SchemaAttribute> SecondarySchemati
        {
            get
            {
                if (_secondarySchemati == null)
                {
                    var emptyGuid = new Guid();
                    List<Attribute> metas = (from att in Dc.Attributes
                                             where
                                                 att.ProjectID.Equals(Currentproject.ID) &&
                                                 att.AttributeType.Equals(AttributeTypeEnum.SchemaMeta.ToString()) &&
                                                 att.SchemaInfos.Any(si => si.SchemaDatas.Any(sd => sd.InSchema && sd.Active))
                                             let displayRank = (from si in att.SchemaInfos
                                                                where si.TaxonomyID.Equals(emptyGuid)
                                                                from sd in si.SchemaDatas
                                                                where sd.Active
                                                                select sd.DisplayOrder).FirstOrDefault()
                                             orderby displayRank , att.AttributeName
                                             select att).ToList();
                    _secondarySchemati = metas.Select(GetMetaAttributeType).ToList();
                }
                return _secondarySchemati;
            }
            set { _secondarySchemati = value; }
        }


        private  SchemaAttribute GetMetaAttributeType(Attribute attribute)
        {
            var attributeName = attribute.AttributeName.ToLower();
            return attributeName.StartsWith("is ") || attributeName.StartsWith("in ")
                       ? new SchemaAttribute(attribute, typeof (bool))
                       : new SchemaAttribute(attribute, typeof (string));
        }

        public string SpecialSchemaAttribute { get; private set; }

        public Dictionary<string, string> MetaMetaSchemaAttributes
        {
            get
            {
                var metaMetas = new Dictionary<string, string> {{"", ToString()}};

                var amis = MetaSchemaAttribute.AttributeMetaInfos.Where(ami => ami.Attribute1.AttributeType == "SchemaMetaMeta").OrderBy(a => a.Attribute1.AttributeName);
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

		#endregion Properties 

		#region Methods (6) 

		// Public Methods (5) 

        public void GetAttributeOrders(
            Attribute attribute, TaxonomyInfo taxonomy,
            out decimal navAttOrder, out decimal dispAttOrder, out bool inSchema)
        {
            navAttOrder = 0;
            dispAttOrder = 0;
            inSchema = false;
            
            if (taxonomy == null || attribute==null)
                return;
            
            inSchema = GetValue(taxonomy, attribute, SchemaAttributeInSchema) != null;
            
            object nav = GetValue(taxonomy, attribute, SchemaAttributeNavigationOrder);
            navAttOrder = nav != null ? (decimal)nav : 0;

            object disp = GetValue(taxonomy, attribute, SchemaAttributeDisplayOrder);
            dispAttOrder = disp != null ? (decimal)disp : 0;
        }

        public  void OrderByNavigation(TaxonomyInfo taxonomy)
        {
            if (taxonomy == null)
                return;

            Dictionary<SchemaData, decimal[]> sds = (from si in taxonomy.SchemaInfos
                                                     let sd = si.SchemaData
                                                     where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                     select sd).ToDictionary(
                                                         sd => sd, sd => new[] { sd.NavigationOrder, sd.DisplayOrder });

            if (!sds.Any())
                return;

            int iCtr = 0;
            foreach (SchemaData nav in
                (from sd in sds
                 where sd.Value[0] > 0
                 orderby sd.Value[0]
                 select sd.Key).ToList())
            {
                sds[nav][0] = ++iCtr;

                if (sds[nav][1] > 0) sds[nav][1] = iCtr;
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
                SchemaInfo schemaInfo = kvp.Key.SchemaInfo;
                SetValue(
                    schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeNavigationOrder, kvp.Value[0]);
                SetValue(
                    schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeDisplayOrder, kvp.Value[1]);
            }


        }

        public  void OrderByDisplay(TaxonomyInfo taxonomy)
        {
            if (taxonomy == null)
                return;

            Dictionary<SchemaData, decimal[]> sds = (from si in taxonomy.SchemaInfos
                                                     let sd = si.SchemaData
                                                     where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                     select sd).ToDictionary(
                                                        sd => sd, sd => new[] { sd.NavigationOrder, sd.DisplayOrder });

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
            foreach (SchemaData disp in
                (from sd in sds
                 where sd.Value[1] > 0
                 orderby sd.Value[1]
                 select sd.Key).ToList())
            {
                if(sds[disp][0] > 0)
                {
                    sds[disp][1] = iCtr = sds[disp][0];
                }
                else
                {
                    sds[disp][1] = ++iCtr;
                }
            }
            

            foreach (var kvp in sds)
            {
                SchemaInfo schemaInfo = kvp.Key.SchemaInfo;
                SetValue(
                    schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeNavigationOrder, kvp.Value[0]);
                SetValue(
                    schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeDisplayOrder, kvp.Value[1]);
            }
        }

        public  void AutoOrderRanks(TaxonomyInfo taxonomy, SchemaData schemaData)
        {
            if (taxonomy == null)
                return;

            Dictionary<SchemaData, decimal[]> sds = (from si in taxonomy.SchemaInfos
                                                     let sd = si.SchemaData
                                                     where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                     select sd).ToDictionary(
                                                         sd => sd, sd => new[] {sd.NavigationOrder, sd.DisplayOrder});

            if (!sds.Any())
                return;

            decimal baseNavOrder = schemaData != null ? schemaData.NavigationOrder : 0;
            decimal baseDispOrder = schemaData != null ? schemaData.DisplayOrder : 0;

            List<SchemaData> navs = (from sd in sds
                                     where
                                         sd.Key.NavigationOrder > 0 && sd.Key.NavigationOrder >= baseNavOrder &&
                                         (schemaData == null || !sd.Key.Equals(schemaData))
                                     select sd.Key).ToList();
            foreach (SchemaData nav in navs)
                sds[nav][0] = nav.NavigationOrder + 1;

            int iCtr = 0;
            foreach (SchemaData nav in
                (from sd in sds
                 where sd.Value[0] > 0
                 orderby sd.Value[0]
                 select sd.Key).ToList())
                sds[nav][0] = ++iCtr;

            List<SchemaData> disps = (from sd in sds
                                      where
                                          sd.Key.DisplayOrder > 0 && sd.Key.DisplayOrder >= baseDispOrder &&
                                          (schemaData == null || !sd.Key.Equals(schemaData))
                                      select sd.Key).ToList();

            foreach (SchemaData disp in disps)
                sds[disp][1] = disp.DisplayOrder + 1;

            iCtr = 0;
            foreach (SchemaData disp in
                (from sd in sds
                 where sd.Value[1] > 0
                 orderby sd.Value[1]
                 select sd.Key).ToList())
                sds[disp][1] = ++iCtr;

            foreach (var kvp in sds)
            {
                SchemaInfo schemaInfo = kvp.Key.SchemaInfo;
                SetValue(
                    schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeNavigationOrder, kvp.Value[0]);
                SetValue(
                    schemaInfo.TaxonomyInfo, schemaInfo.Attribute, false, SchemaAttributeDisplayOrder, kvp.Value[1]);
            }
        }

        public object GetValue(TaxonomyInfo taxonomy, Attribute attribute, SchemaAttribute schematus)
        {
            if (taxonomy == null && attribute == null)
                return schematus;

            if (taxonomy == null)
                return attribute;

            if (attribute == null)
                return taxonomy;

            var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute != null && si.Attribute.Equals(attribute));
            if (schematus == null || schemaInfo == null)
                return null;

            switch (schematus.AttributeType)
            {
                case SchemaAttributeType.Primary:
                    SchemaData activeSchemaData = schemaInfo.SchemaDatas.FirstOrDefault(sd => sd.Active);
                    if (activeSchemaData == null)
                        return null;

                    PropertyInfo valueProperty =
                        activeSchemaData.GetType().GetProperty(schematus.PrimarySchemaAttribute.Replace(" ", String.Empty));
                    return valueProperty.GetValue(activeSchemaData, null);

                case SchemaAttributeType.Meta:
                    SchemaMetaData metaSchemaData = (from mi in schemaInfo.SchemaMetaInfos
                                                     where mi.Attribute.Equals(schematus.MetaSchemaAttribute) && mi.SchemaMetaData!=null
                                                     select mi.SchemaMetaData).FirstOrDefault();
                    return metaSchemaData;

                case SchemaAttributeType.Special:
                    if (schematus.SpecialSchemaAttribute.Equals(SpecialSchematusIsMapped))
                    {
                        SchemaData activeSd = schemaInfo.SchemaDatas.FirstOrDefault(sd => sd.Active);
                        if (activeSd != null)
                            return "x";
                    }

                    if (schematus.SpecialSchemaAttribute.Equals(SpecialSchematusAttributeType))
                    {
                        return attribute.AttributeType;
                    }

                    if (schematus.SpecialSchemaAttribute.Equals(SpecialSchematusListOfValues))
                    {
                        var listOfValues = schemaInfo.ListOfValues.Where(lov => lov.Active == true).Select(lov => lov).ToList();
                        if (listOfValues.Any())
                        {
                            string listSep = Currentproject.ProjPreferences.ListSeparator ?? "; ";
                            return listOfValues.OrderBy(val => val.DisplayOrder).ThenBy(val => val.Value).Select(val => val.Value).Aggregate((current, lov) => current + listSep + lov); 
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;

                //case SchemaAttributeType.FillRate:
                //    double? fillRate = AryaTools.Instance.AllFillRates.GetFillRate(
                //        taxonomy, attribute, schematus.FillRateSchemaAttribute);
                //    if (fillRate == Double.MinValue)
                //    {
                //        SchemaDataGridView.RefreshGridView = true;
                //        return "Calculating...";
                //    }

                //    return fillRate;

                default:
                    return null;
            }
        }

        public string SetValue(
            TaxonomyInfo taxonomy, Attribute attribute, bool autoMapSchema, SchemaAttribute schematus, object value, bool suppressAutoMapMessage = false)
        {
            SchemaInfo schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
            bool newSchemaInfo = false;
            if (schemaInfo == null)
            {

                if (suppressAutoMapMessage) return null;
                if (!autoMapSchema &&
                    MessageBox.Show(
                        String.Format(
                            "Attribute [{0}] is not mapped in Node [{1}]. Do you want to map it?", attribute,
                            taxonomy.TaxonomyData.NodeName), @"Add to Schema", MessageBoxButtons.YesNo) ==
                    DialogResult.No)
                    return null;
                schemaInfo = new SchemaInfo {Attribute = attribute};
                taxonomy.SchemaInfos.Add(schemaInfo);
                newSchemaInfo = true;
            }

           

            if (schematus == null)
            {
                //Create/Update Schema Info only
                SchemaData currentSchemaData = schemaInfo.SchemaData;
                var newSchemaData = new SchemaData();
                if (value != null && value is SchemaData)
                {
                    var fromSchemaData = (SchemaData) value;
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

            SchemaData activeSd = schemaInfo.SchemaData;
            switch (schematus.AttributeType)
            {
                case SchemaAttributeType.Primary:
                    bool reallyNewSd = false;
                    bool reorderRanks = false;

                    SchemaData newSchemaData;
                    if (activeSd == null)
                    {
                        reallyNewSd = true;
                        newSchemaData = new SchemaData();
                    }
                    else
                    {
                        //activeSd.DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
                        activeSd.DeletedByUser = CurrentUser; //fixes Object not found error
                        activeSd.DeletedOn = DateTime.Now;
                        newSchemaData = new SchemaData
                                            {
                                                DataType = activeSd.DataType, InSchema = activeSd.InSchema,
                                                NavigationOrder = activeSd.NavigationOrder,
                                                DisplayOrder = activeSd.DisplayOrder
                                            };
                    }

                    string propertyName = schematus.PrimarySchemaAttribute.Replace(" ", String.Empty);

                    PropertyInfo valueProperty = newSchemaData.GetType().GetProperty(propertyName);
                    Type propertyType = valueProperty.PropertyType;


                    if (propertyType == typeof (string))
                    {
                        object stringValue = value ?? String.Empty;
                        if (propertyName == "DataType")
                        {
                            stringValue = ValidateDataType(stringValue);
                        }

                        if (!(valueProperty.GetValue(newSchemaData, null) ?? String.Empty).Equals(stringValue))
                        {
                            reallyNewSd = true;
                            valueProperty.SetValue(newSchemaData, stringValue.ToString(), null);
                        }
                    }
                    else if (propertyType == typeof (bool))
                    {
                        object boolValue = value ?? false;
                        string firstCharacter;
                        try
                        {
                            firstCharacter = boolValue.ToString().Substring(0, 1).ToLower();
                        }
                        catch (Exception)
                        {
                            firstCharacter = "f";
                        }

                        bool newValue = firstCharacter.Equals("t") || firstCharacter.Equals("y") ||
                                        firstCharacter.Equals("1");

                        if (!((bool) valueProperty.GetValue(newSchemaData, null)).Equals(newValue))
                        {
                            reallyNewSd = true;
                            valueProperty.SetValue(newSchemaData, newValue, null);
                        }
                    }
                    //else if (propertyType == typeof (decimal))
                    //{
                    //    object decimalValue = value ?? 0.0;
                    //    decimal newDecimalValue;
                    //    if (Decimal.TryParse(decimalValue.ToString(), out newDecimalValue))
                    //    {
                    //        if (!((decimal) valueProperty.GetValue(newSchemaData, null)).Equals(newDecimalValue))
                    //        {
                    //            reallyNewSd = true;
                    //            valueProperty.SetValue(newSchemaData, newDecimalValue, null);
                    //            reorderRanks = SchemaDataGridView.AutoOrderRanks;
                    //        }
                    //    }
                    //    else
                    //        return String.Format(
                    //            "Invalid value [{0}] for [{1}] - [{2}].{3}Expected a decimal value.", decimalValue,
                    //            taxonomy.NodeName, attribute, Environment.NewLine);
                    //}
                    else
                        return String.Format(
                            "Unknown data type for value [{0}] for [{1}] - [{2}]", value ?? String.Empty,
                            taxonomy.NodeName, attribute);

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
                    bool reallyNewSmd = false;

                    if (schematus.DataType == typeof(bool))
                    {
                        value = value.ToString().StartsWith("y",StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";

                    }
                    //First check if there is an active SchemaData
                    if (activeSd == null)
                    {

                        if (suppressAutoMapMessage) return null;

                        if (!newSchemaInfo && !autoMapSchema &&
                            MessageBox.Show(
                                String.Format(
                                    "Attribute [{0}] is not mapped in Node [{1}]. Do you want to map it?", attribute,
                                    taxonomy.TaxonomyData.NodeName), "Add to Schema", MessageBoxButtons.YesNo) ==
                            DialogResult.No)
                            return null;

                        schemaInfo.SchemaDatas.Add(new SchemaData());
                        reallyNewSmd = true;
                    }

                    SchemaMetaInfo activeSmi = (from mi in schemaInfo.SchemaMetaInfos
                                                where mi.Attribute.Equals(schematus.MetaSchemaAttribute)
                                                select mi).FirstOrDefault();

                    if (activeSmi == null)
                    {
                        activeSmi = new SchemaMetaInfo {Attribute = schematus.MetaSchemaAttribute};
                        schemaInfo.SchemaMetaInfos.Add(activeSmi);
                        reallyNewSmd = true;
                    }

                    SchemaMetaData activeSmd = activeSmi.SchemaMetaData;

                    if (activeSmd == null)
                        reallyNewSmd = true;
                    else if (value == null || !activeSmd.Value.Equals(value))
                    {
                        reallyNewSmd = true;
                        activeSmd.Active = false;
                    }

                    //if (reallyNewSmd && value != null && !String.IsNullOrEmpty(value.ToString()))
                    //{
                    //    activeSmi.SchemaMetaDatas.Add(new SchemaMetaData {Value = value.ToString()});
                    //    SchemaDataGridView.UpdateAutoCompleteSource(schematus.MetaSchemaAttribute, value.ToString());
                    //}

                    return null;

                default:
                    return "Unknown Meta-attribute Type! Weird!!!";
            }
        }

        public override string ToString()
        {
            if (PrimarySchemaAttribute != null)
            {
                string[] parts = PrimarySchemaAttribute.Split('.');
                return parts[parts.Length - 1];
            }

            if (MetaSchemaAttribute != null)
            {
                string[] parts = MetaSchemaAttribute.AttributeName.Split('.');
                return parts[parts.Length - 1];
            }

            if (SpecialSchemaAttribute != null)
            {
                return SpecialSchemaAttribute;
            }

            //if (FillRateSchemaAttribute != null)
            //{
            //    return FillRateSchemaAttribute.GetFilterName();
            //}

            return String.Empty;
        }

        public static void UnmapNodeAttribute(TaxonomyInfo taxonomy, Attribute attribute)
        {
            SchemaInfo schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
            if (schemaInfo == null)
                return;

            schemaInfo.SchemaMetaInfos.SelectMany(smi => smi.SchemaMetaDatas).Where(md => md.Active).ForEach(
                md => md.Active = false);

            if (schemaInfo.SchemaData != null)
                schemaInfo.SchemaData.Active = false;
        }
		// Private Methods (1) 

        private static string ValidateDataType(object stringValue)
        {
            try
            {
                uint x = Convert.ToUInt32(stringValue);
                return x < 15 ? x.ToString() : "Text";
            }
            catch
            {
                return LovForDataType.Contains(stringValue.ToString()) ? stringValue.ToString() : "Text";
            }
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
		#endregion Methods 

        #region SchemaAttributeType enum

        public enum SchemaAttributeType
        {
            Primary,
            Meta,
            Special,
            FillRate,
            Unknown
        }

        #endregion
      
    }
}
