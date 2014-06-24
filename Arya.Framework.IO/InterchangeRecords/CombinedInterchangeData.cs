using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlRoot("Interchange")]
    [XmlType("Interchange")]
    public class CombinedInterchangeData
    {
        [XmlArrayItem]
        public List<AttributeInterchangeRecord> Attributes { get; set; }
        [XmlArrayItem]
        public List<DerivedAttributeInterchangeRecord> DerivedAttributes { get; set; }

        [XmlArrayItem]
        public List<TaxonomyInterchangeRecord> Taxonomies { get; set; }
        [XmlArrayItem]
        public List<TaxonomyMetaDataInterchangeRecord> TaxonomyMetaDatas { get; set; }

        [XmlArrayItem]
        public List<SchemaInterchangeRecord> Schemas { get; set; }
        [XmlArrayItem]
        public List<SchemaMetaDataInterchangeRecord> SchemaMetaDatas { get; set; }
        [XmlArrayItem]
        public List<ListOfValuesInterchangeRecord> ListOfValues { get; set; }

        [XmlArrayItem]
        public List<SkuTaxonomyInterchangeRecord> SkuTaxonomies { get; set; }
        [XmlArrayItem]
        public List<SkuAttributeValueInterchangeRecord> SkuAttributeValues { get; set; }
        [XmlArrayItem]
        public List<SkuLinkInterchangeRecord>  SkuLinks { get; set; }

        public void DedupLists()
        {
            Attributes = (from att in Attributes
                          group att by new { att.AttributeName, att.AttributeType }
                              into grp
                              select grp.First()).ToList();

            DerivedAttributes = (from att in DerivedAttributes
                                 group att by
                                     new { att.DerivedAttributeName, TaxonomyPath = att.TaxonomyPath ?? string.Empty }
                                     into grp
                                     select grp.First()).ToList();

            Taxonomies = (from tax in Taxonomies
                          group tax by new { tax.TaxonomyPath }
                              into grp
                              select grp.First()).ToList();

            TaxonomyMetaDatas = (from tax in TaxonomyMetaDatas
                                 group tax by new { tax.TaxonomyPath, MetaAttributeName = tax.TaxonomyMetaAttributeName }
                                     into grp
                                     select grp.First()).ToList();

            Schemas = (from sch in Schemas
                       group sch by new { sch.TaxonomyPath, sch.AttributeName }
                           into grp
                           select grp.First()).ToList();

            SchemaMetaDatas = (from sch in SchemaMetaDatas
                               group sch by new { sch.TaxonomyPath, AttributeName = sch.AttributeName, MetaAttributeName = sch.SchemaMetaAttributeName }
                                   into grp
                                   select grp.First()).ToList();

            ListOfValues = (from lov in ListOfValues
                            group lov by new { lov.TaxonomyPath, lov.AttributeName, Value = lov.Lov }
                                into grp
                                select grp.First()).ToList();

            SkuTaxonomies = (from skt in SkuTaxonomies
                           group skt by new { skt.ItemID, skt.TaxonomyPath }
                               into grp
                               select grp.First()).ToList();

            SkuAttributeValues = (from sav in SkuAttributeValues
                                  group sav by new { sav.ItemID, sav.AttributeName, sav.Value, sav.Uom }
                                      into grp
                                      select grp.First()).ToList();

            SkuLinks = (from sl in SkuLinks
                            group sl by new {sl.FromItemID, sl.LinkType, sl.ToItemID}
                            into grp
                            select grp.First()).ToList();
        }

        public CombinedInterchangeData(){}

        public CombinedInterchangeData(bool autoInit)
        {
            if (!autoInit)
                return;

            Attributes = new List<AttributeInterchangeRecord>();
            DerivedAttributes = new List<DerivedAttributeInterchangeRecord>();
            Taxonomies = new List<TaxonomyInterchangeRecord>();
            TaxonomyMetaDatas = new List<TaxonomyMetaDataInterchangeRecord>();
            Schemas = new List<SchemaInterchangeRecord>();
            SchemaMetaDatas = new List<SchemaMetaDataInterchangeRecord>();
            ListOfValues = new List<ListOfValuesInterchangeRecord>();
            SkuTaxonomies = new List<SkuTaxonomyInterchangeRecord>();
            SkuAttributeValues = new List<SkuAttributeValueInterchangeRecord>();
            SkuLinks = new List<SkuLinkInterchangeRecord>();
        }

        public void AddRecords<T>(List<object> inportData) where T:InterchangeRecord
        {
            //get the proper list type
            var propertyInfos = this.GetType().GetProperties().ToList();
            var propertyToSet = propertyInfos.FirstOrDefault(pi => pi.PropertyType.GetGenericArguments().First().Name == inportData.First().GetType().Name);
            if (propertyToSet != null)
            {

                propertyToSet.SetValue(this, inportData.Cast<T>().ToList());
            }
              
        }
    }
}