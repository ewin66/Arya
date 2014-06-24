using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
     public interface IGrammerRule
    {
         List<SpellTerm> GetValidationResult(string value);
    }
}
