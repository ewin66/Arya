using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arya.Data;
using Attribute = Arya.Data.Attribute;

namespace Arya.HelperClasses
{
    public static class AttributeListSort
    {
        public enum Field
        {
            NavigationOrder,
            DisplayOrder,
            DataType,
            InSchema,
            AttributeName,
            MetaAttribute
        }

        public enum Order
        {
            Ascending,
            Descending

        }    

        private static object GetSortKey(TaxonomyInfo taxonomy, Attribute attribute, Field fieldName, Attribute metaAttribute, double defaultOrder)
        {
            if(fieldName== Field.MetaAttribute)
                //return SchemaAttribute.GetValue(taxonomy, attribute, new SchemaAttribute
                return SchemaAttribute.GetMetaAttributeValue(attribute,metaAttribute,taxonomy);

            if (fieldName==Field.AttributeName)
                return attribute.AttributeName;
            
                double navOrder = defaultOrder;
                double dispOrder = defaultOrder;
                string dataType = string.Empty;
                bool inSchema = false;

                try
                {
                    var si = taxonomy.SchemaInfos.Where(a => a.Attribute == attribute).FirstOrDefault();
                    var sd = si.SchemaDatas.Where(a => a.Active).FirstOrDefault();
                    if (sd != null)
                    {
                        navOrder = (int)sd.NavigationOrder == 0 ? navOrder : (int)sd.NavigationOrder;
                        dispOrder = (int)sd.DisplayOrder == 0 ? dispOrder : (int)sd.DisplayOrder;
                        dataType = sd.DataType;
                        inSchema = sd.InSchema;
                    }
                }
                catch
                {

                }

            switch (fieldName)
             {
                        
                case Field.DisplayOrder:
                return dispOrder;                

                case Field.NavigationOrder:
                return navOrder;
               
                case Field.DataType:
                return dataType;                

                case Field.InSchema:
                return inSchema ? "Yes" : "No";
                

              }

            return null; //Control should never get here!!!
        }

        public static List<Data.Attribute> Sort( this List<Data.Attribute> _attributeList , Field FieldName,Order order, Data.TaxonomyInfo taxonomy, Data.Attribute metaAttribute)
        {

            double defaultOrder = 0;
            if (order == Order.Ascending)
                defaultOrder = double.MaxValue;
            if (order == Order.Descending)
                defaultOrder = double.MinValue;

            var atts = (from att in _attributeList
                        select new
                        {
                            Attribute = att,
                            SortKey = GetSortKey(taxonomy, att, FieldName, metaAttribute, defaultOrder)
                        }).ToArray();

            if (order == Order.Ascending)
                return atts.OrderBy(a => a.SortKey).ThenBy(a => Array.IndexOf(atts, a)).Select(a=>a.Attribute).ToList();
            else
                return atts.OrderByDescending(a => a.SortKey).ThenBy(a => Array.IndexOf(atts, a)).Select(a => a.Attribute).ToList();        

        }    

    }
}
