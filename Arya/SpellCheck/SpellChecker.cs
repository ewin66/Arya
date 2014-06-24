using Arya.Data;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.HelperClasses;
using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    public class SpellChecker
    {
        private Hunspell _currentSpellChecker;
        public   char[] delimiterChars=(" `~!@#$%^&*()_+-={}|[]\\:\";'<>?,./\t").ToCharArray();
        private DoubleSpaceChecker _doubleSpaceChecker = new DoubleSpaceChecker();
        private ConsecutiveWordChecker _consecutiveWordChecker = new ConsecutiveWordChecker();
        public SpellChecker( AvailableDictionaries dictionaryName = AvailableDictionaries.en_US)
        {
            if (_currentSpellChecker == null)
            {
                _currentSpellChecker = SpellCheckerFactory.GetSpellChecker(dictionaryName);
                var projectDictionaryWords = AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.Select(pd => pd.Word);
                foreach (string word in projectDictionaryWords.ToList())
                {
                    _currentSpellChecker.Add(word);
                }
                
            }
        }

        public bool CorrectWord(string word)
        {
            return _currentSpellChecker.Spell(word); 
        }
        /// <summary>
        /// Tokenize the senence with the delimiter provided.
        /// Determine if a sentence has any misspelled word.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="delimiterChars"></param>
        /// <returns></returns>
        public bool CorrectSentence(string sentence, char[] delimiterChars)
        {
            string[] words = sentence.Split(delimiterChars);
            foreach (string word in words)
            {
                if (!_currentSpellChecker.Spell(word))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Tokenize the senence with the delimiter provided.
        /// Determine if a sentence has any misspelled word.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="delimiterChar"></param>
        /// <returns></returns>
        public bool CorrectSentence(string sentence, char delimiterChar)
        {
            char[] delimiterChars = new char[] { delimiterChar };

            return CorrectSentence(sentence,delimiterChars);
        }
        /// <summary>
        /// Tokenize the senence with the already defined  delimiter in the class.
        /// Determine if a sentence has any misspelled word.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public bool CorrectSentence(string sentence)
        {
            return CorrectSentence(sentence, delimiterChars);
        }

        public char[] GetCurrentDelimiters()
        {
            return delimiterChars;
        }
        public List<string> GetRecommendation(string word)
        {
            return _currentSpellChecker.Suggest(word);            
        }
        public HashSet<string> _ignoreAll = new HashSet<string>();

        public SpellCheckEntity GetSpellCheckEntity(SpellCheckIntermediate spellCheckValue)
        {
            List<SpellTerm> spellTerms = new List<SpellTerm>();
            spellTerms.AddRange(_doubleSpaceChecker.GetValidationResult(spellCheckValue.GetSpellValue()));
            var wordsWithoutSpace = spellCheckValue.GetSpellValue().Split(delimiterChars).ToList().Where(val => val != "").Select(val => val.Trim());
            List<SpellTerm> spelledTermWords = GetWords(spellCheckValue.GetSpellValue());
            spellTerms.AddRange(_consecutiveWordChecker.GetValidationResult(spellCheckValue.GetSpellValue()));
            foreach (SpellTerm word in spelledTermWords)
            {
                if (!IsCorrect(word.Value,spellCheckValue.GetPropertyName(), spellCheckValue.GetISpell()))
                {
                    spellTerms.Add(word);
                }
            }
            var spellCheckEntity = new SpellCheckEntity(spellCheckValue, !spellTerms.Any(), spellTerms);
            return spellCheckEntity;            
        }

        private List<SpellTerm> GetWords(string inputString)
        {
            List<SpellTerm> outPutlist = new List<SpellTerm>();
            char[] allChars = AppendSpaces(inputString).ToCharArray();
            List<char> tempWord = new List<char>();
            int firstIndex = 0;
            for (int i= 0; i<allChars.Count();i++)
            {
                if(delimiterChars.Any(ch=> ch==allChars[i]))
                {
                    if (tempWord.Count != 0)
                    {
                        outPutlist.Add(new SpellTerm(new string(tempWord.ToArray()), firstIndex, tempWord.Count));
                        tempWord.Clear();
                    }
                }
                else
                {
                    if(tempWord.Count <1)
                    {
                        firstIndex = i-1;
                    }
                    tempWord.Add(allChars[i]);
                }               
            }
            return outPutlist;
        }

        private void CheckForConsecutiveWords(List<string> wordsWithoutSpace, List<string> spellTerms)
        {
            for(int i = 0; i<wordsWithoutSpace.Count(); i++)
            {
                if (i + 1 < wordsWithoutSpace.Count() && wordsWithoutSpace[i] != "" && wordsWithoutSpace[i].ToLower() == wordsWithoutSpace[i + 1].ToLower())
                {
                    spellTerms.Add(wordsWithoutSpace[i]); 
                }
            }
        }

        private void CheckDoubleSpace(string inputString, List<string> spellTerms)
        {
            Regex regex = new Regex(@"\s{2,}"); // matches at least 2 whitespaces
            if ( !String.IsNullOrEmpty(inputString) && regex.IsMatch(inputString))
            {
                spellTerms.AddRange(Regex.Matches(inputString, @"\s{2,}").Cast<Match>().Select(m=>m.Value));
            }
           
        }

        private bool IsCorrect(string word, string propertyName, ISpell  spellCheckValue)
        {
            //May be TODo:Add property name check
            if(_currentSpellChecker.Spell(word))
                return true;
            else
            {
                if (_ignoreAll.Contains(word))
                {
                    AddToIgnore(word, spellCheckValue);
                    return true;
                }

                return HasIgnore(spellCheckValue, word);
//                return AryaTools.Instance.InstanceData.Dc.IgnoreWords.Any(iv => iv.Value == word && iv.EntityType == spellCheckValue.GetType().ToString() && iv.EntityID == spellCheckValue.GetId());
            }
            
        }

        internal void AddToIgnore(string wordToIgnore, ISpell ispellEntity)
        {
            if (HasIgnore(ispellEntity, wordToIgnore))
                return;

            Arya.Data.IgnoreWord currntIgnoredWord = new Arya.Data.IgnoreWord
            {
                EntityID = ispellEntity.GetId(),
                EntityType = ispellEntity.GetType().ToString(),
                Value = wordToIgnore
            };

            if (!_ignoreWords.ContainsKey(ispellEntity.GetId()))
                _ignoreWords.Add(ispellEntity.GetId(), new List<Data.IgnoreWord>());
            _ignoreWords[ispellEntity.GetId()].Add(currntIgnoredWord);

             AryaTools.Instance.InstanceData.Dc.IgnoreWords.InsertOnSubmit(currntIgnoredWord);
             AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        Dictionary<Guid, List<Arya.Data.IgnoreWord>> _ignoreWords = new Dictionary<Guid, List<Data.IgnoreWord>>();

        internal bool HasIgnore(ISpell ispellEntity, string wordToCheck)
        {
            if (!_ignoreWords.ContainsKey(ispellEntity.GetId()))
                _ignoreWords.Add(ispellEntity.GetId(), AryaTools.Instance.InstanceData.Dc.IgnoreWords.Where(iv => iv.EntityType == ispellEntity.GetType().ToString() && iv.EntityID == ispellEntity.GetId()).ToList());
                
               return  _ignoreWords[ispellEntity.GetId()].Any(iw => iw.EntityType == ispellEntity.GetType().ToString() && iw.Value == wordToCheck);
        }

        internal void AddToProjectDictinary(string dictionaryWord)
        {
            if (!String.IsNullOrEmpty(dictionaryWord))
            {
                Arya.Data.ProjectDictionary currntWord = new Arya.Data.ProjectDictionary
                {
                    Word = dictionaryWord
                };
                AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.InsertOnSubmit(currntWord);
                AryaTools.Instance.SaveChangesIfNecessary(false, false);
                _currentSpellChecker.Add(dictionaryWord);
            }
           
        }

        private string AppendSpaces(string inputString)
        {
            return " " + inputString + " ";
        }
    }
}
