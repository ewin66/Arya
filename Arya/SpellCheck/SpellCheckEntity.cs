using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    public class SpellCheckEntity
    {

        private List<SpellTerm> _incorrectTerm;
        //private object _incorrectTerm;

        public SpellCheckEntity(SpellCheckIntermediate spellCheckIntermediate, bool correct, List<SpellTerm> incorrectTerms)
        {
            this.ISpellCheckIntermediate = spellCheckIntermediate;
            this.Correct = correct;
            this.InCorrectTerms = incorrectTerms;
        }

       
        public SpellCheckIntermediate ISpellCheckIntermediate { get; private set; }
        public bool Correct { get; private set; }
        public List<SpellTerm> InCorrectTerms 
        { 
            get{
                if (_incorrectTerm == null)
                {
                    _incorrectTerm = new List<SpellTerm>();
                }
                return _incorrectTerm;
            }
            set 
            {
                _incorrectTerm = value;
            }
         }
       
        public bool RemoveInCorrectTerms(SpellTerm word)
        {
            List<SpellTerm> tt = InCorrectTerms.ToList();
            tt.Remove(word);
            InCorrectTerms = tt;
            return true;
        }
    }
}
