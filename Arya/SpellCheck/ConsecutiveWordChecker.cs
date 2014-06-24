using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
   internal class ConsecutiveWordChecker:IGrammerRule
    {
        private  char[] delimiterChars = ("`~!@#$%^&*()_+-={}|[]\\:\";'<>?,./\t").ToCharArray();
        public List<SpellTerm> GetValidationResult(string inputString)
        {
            return CheckForConsecutiveWords(inputString);
        }
        private string AppendSpaces(string inputString)
        {
            return " " + inputString + " ";
        }
        private List<SpellTerm> CheckForConsecutiveWords(string inputString)
        {
            List<SpellTerm> outPutlist = new List<SpellTerm>();
            char[] allChars =AppendSpaces( inputString).ToCharArray();
            List<char> tempWord = new List<char>();
            int firstIndex = 0;
            string previousWord = string.Empty;
            string currentWord = string.Empty;
            for (int i = 0; i < allChars.Count(); i++)
            {
                if (delimiterChars.Any(ch => ch == allChars[i]))
                {
                    if (tempWord.Count != 0)
                    {
                        currentWord = new string(tempWord.ToArray());
                        tempWord.Clear();
                        if (currentWord.Equals(previousWord, StringComparison.OrdinalIgnoreCase))
                        {
                            outPutlist.Add(new SpellTerm(currentWord, firstIndex, currentWord.Length));
                        }
                        else
                        {
                            currentWord = string.Empty;
                            previousWord = string.Empty;
                        }
                    }
                    
                }
                else if (allChars[i] == ' ' )
                {
                    if (tempWord.Count != 0)
                    {
                        currentWord = new string(tempWord.ToArray());
                        tempWord.Clear();
                        if (previousWord != " " && currentWord.Equals(previousWord, StringComparison.OrdinalIgnoreCase))
                        {
                            outPutlist.Add(new SpellTerm(currentWord, firstIndex, currentWord.Length));
                        }
                        else
                        {
                            previousWord = currentWord;
                            currentWord = string.Empty;
                            //previousWord = string.Empty;
                        }
                    }
                }
                else
                {
                    if (tempWord.Count < 1)
                    {
                        firstIndex = i-1;
                    }
                    tempWord.Add(allChars[i]);
                }
            }
            return outPutlist;
        }
    }
}
