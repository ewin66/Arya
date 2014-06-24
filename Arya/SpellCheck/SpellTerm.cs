using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    public class SpellTerm
    {
        public string Value { get; set; }
        public int StratIndex { get; set; }
        public int length { get; set; }


        public SpellTerm(string value, int startIndex, int length)
        {
            this.Value = value;
            this.StratIndex = startIndex;
            this.length = length;
        }
    }
}
