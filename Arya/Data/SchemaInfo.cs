using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arya.Data
{
    partial class SchemaInfo : IComparable
    {
        #region Properties (2)

        public IEnumerable<string> ActiveListOfValues
        {
            get
            {
                return from lov in ListOfValues
                       where lov.Active
                       select lov.Value;
            }
            set
            {
                var newLovs = new HashSet<string>(value);
                IEnumerable<ListOfValue> toRemove = from val in ListOfValues
                                                    where val.Active && !newLovs.Contains(val.Value)
                                                    select val;
                toRemove.ForEach(val => val.Active = false);

                var currentValues = new HashSet<string>(ListOfValues.Where(val => val.Active).Select(val => val.Value));
                IEnumerable<string> toAdd = from val in value
                                            where !currentValues.Contains(val)
                                            select val;
                toAdd.ForEach(val => ListOfValues.Add(new ListOfValue { Value = val }));
            }
        }

        public SchemaData SchemaData
        {
            get { return SchemaDatas.FirstOrDefault(sd => sd.Active); }
        }

        public ListOfValue AddLov(string value)
        {
            if (ActiveListOfValues.Any(val => val.Equals(value)))
                return null;

            var listOfValue = new ListOfValue { Value = value };
            ListOfValues.Add(listOfValue);

            return listOfValue;
        }

        public ListOfValue AddLov(string value, string parentValue, string enrichmentImage, string enrichmentCopy, int? displayOrder)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return null;
            if (ActiveListOfValues.Any(val => val.Equals(value)))
                return null;

            var listOfValue = new ListOfValue 
                {
                    Value = value, 
                    ParentValue = parentValue, 
                    EnrichmentImage = enrichmentImage, 
                    EnrichmentCopy = enrichmentCopy,
                    DisplayOrder = displayOrder
                };
            ListOfValues.Add(listOfValue);

            return listOfValue;
        }

        #endregion Properties

        #region Methods (1)

        // Private Methods (1) 

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

        #endregion Methods

        public int CompareTo(object obj)
        {
            var thisString = ToString();
            var objString = obj.ToString();
            if (!thisString.Equals(objString))
                return String.CompareOrdinal(thisString, objString);

            var other = (Attribute)obj;
            return ID.CompareTo(other.ID);
        }

    }
}