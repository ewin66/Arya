using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    public class SpellCheckIntermediate
    {
        private ISpell _spellEntity;
        private string _value;
        private string _propertyName;

        public SpellCheckIntermediate(ISpell spellEntity, string value, string propertyName)
        {
            _spellEntity = spellEntity;
            _value = value;
            _propertyName = propertyName;
        }


        public string GetSpellValue()
        {
            return _value;
        }

        public string GetPropertyName()
        {
            return _propertyName;
        }

        public void SetValue(string value)
        {
            _value = value;
        }
        public ISpell GetISpell()
        {
            return _spellEntity;
        }
    }
}
