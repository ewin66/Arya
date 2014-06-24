using Natalie.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natalie.Data
{
    public class SchemaSpellCheckEntity : ISpell
    {
        private TaxonomyInfo _taxonomy;
        private Attribute _attribute;
        private SchemaAttribute _schematus;
        public SchemaSpellCheckEntity(TaxonomyInfo taxonomy, Attribute attribute, SchemaAttribute schematus)
        {
            _taxonomy = taxonomy;
            _attribute = attribute;
            _schematus = schematus;

        }


        Type ISpell.GetType()
        {
            return this.GetType();
        }

        string ISpell.GetSpellValue()
        {
            object cellValue = SchemaAttribute.GetValue(_taxonomy, _attribute, _schematus);
            return cellValue == null ? "" : cellValue.ToString();
        }

        Guid ISpell.GetId()
        {
            var schemaInfo = _taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(_attribute));
            return schemaInfo.SchemaData.ID;
        }

        void ISpell.SetValue(string value)
        {
            SchemaAttribute.SetValue(_taxonomy, _attribute, true, _schematus, value, true);
        }
        public override string ToString()
        {
            object cellValue = SchemaAttribute.GetValue(_taxonomy, _attribute, _schematus);
            return cellValue == null ? "" : cellValue.ToString();
        }

        void ISpell.AddNew()
        {
            throw new NotImplementedException();
        }


        void ISpell.Deactivate(Guid spellCheckRemarkId)
        {
            throw new NotImplementedException();
        }


        string ISpell.GetLocation()
        {
            throw new NotImplementedException();
        }
    }

}
