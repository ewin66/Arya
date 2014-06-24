using System.Collections.Generic;
using System.Linq;
using LinqKit;

namespace Arya.Framework.Data.AryaDb
{
    partial class SchemaInfo : BaseEntity
    {
        public SchemaInfo(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #region Properties (2)

        public IEnumerable<string> ActiveListOfValues
        {
            get { return from lov in ListOfValues where lov.Active select lov.Value; }
            set
            {
                var newLovs = new HashSet<string>(value);
                var toRemove = from val in ListOfValues where val.Active && !newLovs.Contains(val.Value) select val;
                toRemove.ForEach(val => val.Active = false);

                var currentValues = new HashSet<string>(ListOfValues.Where(val => val.Active).Select(val => val.Value));
                var toAdd = from val in value where !currentValues.Contains(val) select val;
                toAdd.ForEach(val => ListOfValues.Add(new ListOfValue {Value = val}));
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

            var listOfValue = new ListOfValue {Value = value};
            ListOfValues.Add(listOfValue);

            return listOfValue;
        }

        public ListOfValue AddLov(string value, string parentValue)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return null;

            if (ActiveListOfValues.Any(val => val.Equals(value)))
            {
                var lov =
                    ListOfValues.FirstOrDefault(a => a.Active && a.Value.ToLower().Trim() == value.ToLower().Trim());

                if (lov == null)
                    return null;

                if (lov.ParentValue.Trim().ToLower() != parentValue.Trim().ToLower())
                {
                    lov.ParentValue = parentValue;
                    return lov;
                }
                return lov;
            }
            var listOfValue = new ListOfValue {Value = value, ParentValue = parentValue};
            ListOfValues.Add(listOfValue);

            return listOfValue;
        }

        #endregion Properties
    }
}