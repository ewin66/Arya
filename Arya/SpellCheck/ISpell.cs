using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    public interface ISpell
    {
        string GetType();
        Dictionary<string, string> GetSpellValue();
        KeyValuePair<bool, string> Updatable
        {
            get;
        }
        //string GetSpellValue(string propertyName, string value);
        Guid GetId();
        ISpell SetValue(string propertyName,string value);
        string GetLocation();
    }
}
