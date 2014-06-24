using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    internal class DoubleSpaceChecker:IGrammerRule
    {
        public DoubleSpaceChecker()
        { 
        }
       
        private List<SpellTerm> CheckDoubleSpace(string inputString)
        {
            List<SpellTerm> spellTerms = new List<SpellTerm>();
            Regex regex = new Regex(@"\s{2,}"); // matches at least 2 whitespaces
            if (!String.IsNullOrEmpty(inputString) && regex.IsMatch(inputString))
            {
                List<string> spaceTerms = Regex.Matches(inputString, @"\s{2,}").Cast<Match>().Select(m => m.Value.ToString()).ToList();
                foreach (var item in spaceTerms)
                {
                    var indices = inputString.AllIndexesOf(item);
                    foreach(int index in indices)
                    {
                        spellTerms.Add(new SpellTerm(item, index, item.Count()));
                    }
                }
                
            }
            return spellTerms;
        }




        // List<SpellTerm> GetValidationResult(string p)
        //{
        //    return CheckDoubleSpace(p);
        //}

        public List<SpellTerm> GetValidationResult(string value)
        {
            return CheckDoubleSpace(value);
        }
    }
}
