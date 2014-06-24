
using Arya.Data;
using Arya.Framework.Utility;
using Arya.HelperClasses;
using Arya.Properties;
using Arya.SpellCheck;
using Arya.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Arya.HelperForms
{
    public partial class FrmSpellCheck : Form
    {
        private Remark _spellCheckRemark;
        public Action InvalidateMethod { get; set; }
        private SpellChecker _aryaSpellChecker;
        private IEnumerator<ISpell> _spellEntities;
        private IEnumerator<KeyValuePair<string, string>> _spellValues;
        SpellCheckEntity _entity;
        SpellCheckIntermediate _currentSpellIntermediate;
        Dictionary<string, string> _changeSets;
        private DoubleSpaceChecker _doubleSpaceChecker = new DoubleSpaceChecker();
        //Timer _timer;
        public Remark SpellCheckRemark
        {
            get
            {
                _spellCheckRemark = AryaTools.Instance.InstanceData.Dc.Remarks.FirstOrDefault(rm => rm.Remark1 == "SpellCheck");
                if (_spellCheckRemark == null)
                {
                    _spellCheckRemark = new Remark() { Remark1 = "SpellCheck", IsCanned = false };
                    AryaTools.Instance.InstanceData.Dc.Remarks.InsertOnSubmit(_spellCheckRemark);
                    AryaTools.Instance.SaveChangesIfNecessary(true, false);
                }


                return _spellCheckRemark;
            }
        }
        public FrmSpellCheck()
        {
            InitializeComponent();
            Icon = Resources.AryaLogoIcon;
            DialogResult = DialogResult.Cancel;
            _aryaSpellChecker = new SpellChecker();
            labelCurrentWord.Text = "";
        }

        //private string _entityOldValue; 
        public void UpdateSpellCheckView(IEnumerable<ISpell> toBeCheckedValues, string ignoreAllBtnText)
        {
            if (AryaTools.Instance.RemarkID == null || AryaTools.Instance.RemarkID.ToString() == FrmRemark.DefaultRemarkID)
            {
                AryaTools.Instance.RemarkID = SpellCheckRemark.ID;
            }
            toolTipChangeAll.SetToolTip(this.btnChangeAll, "Change All in " + ignoreAllBtnText);
            toolTipIgnoreAll.SetToolTip(this.btnIgnoreAll, "Ignore All in " + ignoreAllBtnText);
            lblCurrentContext.Text = ignoreAllBtnText;

            _aryaSpellChecker._ignoreAll.Clear();
            _spellEntities = toBeCheckedValues.GetEnumerator();
            _spellValues = null;
            _changeSets = new Dictionary<string, string>();
            GoNext();
        }

        private void GoNext()
        {
            Guid waitKey = Guid.Empty;
            if (_spellValues == null || !_spellValues.MoveNext())
            {
                if (!_spellEntities.MoveNext())
                {
                    _entity = null;
                    waitKey = FrmWaitScreen.ShowMessage("No spelling errors found in the selected context.",true);                    
                    this.Close();
                    return;
                }
                _spellValues = _spellEntities.Current.GetSpellValue().AsEnumerable().GetEnumerator();
                _spellValues.MoveNext();
            }

            _currentSpellIntermediate = new SpellCheckIntermediate(_spellEntities.Current, _spellValues.Current.Value, _spellValues.Current.Key);
            ApplyStoredChanges();
            _entity = _aryaSpellChecker.GetSpellCheckEntity(_currentSpellIntermediate);

            if (!_entity.Correct)
                ShowCurrentItem();   //Show the current item and await user input
            else
                ApplyCurrentChanges();
            
        }

        private void ApplyStoredChanges()
        {
            foreach (var item in _changeSets)
            {
                if (AppendSpaces(_currentSpellIntermediate.GetSpellValue()).Contains(AppendSpaces(item.Key)))
                    _currentSpellIntermediate.SetValue(item.Value);
            }
        }

        bool updatingColor = false;
        private void ShowCurrentItem()
        {

            lblLocation.Text = ProcessLocationString(_spellEntities.Current.GetLocation() + " "+_spellValues.Current.Key);
            updatingColor = true; 
            UpdateRichTextBox(_entity.ISpellCheckIntermediate.GetSpellValue(), _entity.InCorrectTerms);
            labelCurrentWord.Text = _entity.InCorrectTerms.Count() == 0 ? "" : _entity.InCorrectTerms.First().Value;
            PopulateRecommendationListBox();
            updatingColor = false;
        }

        private string ProcessLocationString(string locationSting)
        {
            var regex = new Regex(@".{100}");
            string result = regex.Replace(locationSting, "$&" + Environment.NewLine);
            return result;
        }

        private void PopulateRecommendationListBox()
        {
            listBox.Items.Clear();
            var recommendations = _aryaSpellChecker.GetRecommendation(labelCurrentWord.Text);
            foreach (string recommendation in recommendations)
            //for (int i = recommendations.Count - 1; i >= 0; i-- )
            {
                listBox.Items.Add(recommendation);
            }
        }

        private void UpdateRichTextBox(string value, IEnumerable<SpellTerm> incorrectTerms)
        {
            richTextValue.Text = "";
            richTextValue.Text = value;
            foreach (SpellTerm word in incorrectTerms)
            {
                //labelCurrentWord.Text = word.Value;
                Font currFont = richTextValue.SelectionFont;
                Font boldUnderFont = new Font(currFont, FontStyle.Underline);
                richTextValue.Select(word.StratIndex, word.length);
                richTextValue.SelectionColor = Color.Red;
                richTextValue.SelectionFont = boldUnderFont;
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            //RollBackChanges();
            this.Close();
        }

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            string selectedRecommendedWord = listBox.SelectedItem.ToString();
            ReplaceText(selectedRecommendedWord);
        }

        private void ReplaceText(string selectedRecommendedWord)
        {
            _currentSpellIntermediate.SetValue(richTextValue.Text.Replace(labelCurrentWord.Text, selectedRecommendedWord).Trim());
            _entity = _aryaSpellChecker.GetSpellCheckEntity(_currentSpellIntermediate);
            ShowCurrentItem();
        }

        private string AppendSpaces(string inputString)
        {
            return " " + inputString + " ";
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            ApplyCurrentChanges();
        }

        private void ApplyCurrentChanges()
        {
            if (_spellEntities.Current.Updatable.Key)
            {
                string value;

                bool valuePresent = _spellEntities.Current.GetSpellValue().TryGetValue(_currentSpellIntermediate.GetPropertyName(), out value);
                if (valuePresent && value != _currentSpellIntermediate.GetSpellValue())
                {
                    
                    _spellEntities.Current.SetValue(_currentSpellIntermediate.GetPropertyName(), _currentSpellIntermediate.GetSpellValue());
                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                }
            }
            else 
            {
                MessageBox.Show(_spellEntities.Current.Updatable.Value);
            }
            
            GoNext();
        }


        private void btnChangeAll_Click(object sender, EventArgs e)
        {
            if (!_changeSets.ContainsKey(_spellValues.Current.Value)) 
                _changeSets.Add(_spellValues.Current.Value, _currentSpellIntermediate.GetSpellValue());
            btnChange_Click(sender, e);

        }
        private void btnIgnore_Click(object sender, EventArgs e)
        {
            _aryaSpellChecker.AddToIgnore(labelCurrentWord.Text, _currentSpellIntermediate.GetISpell());
            _entity = _aryaSpellChecker.GetSpellCheckEntity(_currentSpellIntermediate);
            ShowCurrentItem();
        }

        private void btnIgnoreAll_Click(object sender, EventArgs e)
        {
            _aryaSpellChecker._ignoreAll.Add(labelCurrentWord.Text);
            _entity = _aryaSpellChecker.GetSpellCheckEntity(_currentSpellIntermediate);
            ShowCurrentItem();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _aryaSpellChecker.AddToProjectDictinary(labelCurrentWord.Text);
            //_aryaSpellChecker.AddToProjectDictinary(labelCurrentWord.Text.First().ToString().ToUpper() + String.Join("", labelCurrentWord.Text.Skip(1)));
            _entity = _aryaSpellChecker.GetSpellCheckEntity(_currentSpellIntermediate);
            ShowCurrentItem();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            string value;
             bool valuePresent = _spellEntities.Current.GetSpellValue().TryGetValue(_currentSpellIntermediate.GetPropertyName(), out value);
             if (valuePresent && value != _currentSpellIntermediate.GetSpellValue())
             {
                 switch (MessageBox.Show("You’ve made a change"+
                                "\n\nClick <yes> to save the changes" +
                                "\nClick <no> to not save your changes", "Value changed warning", MessageBoxButtons.YesNoCancel))
                 {
                     case DialogResult.Yes:
                         {
                             ApplyCurrentChanges();
                             break;
                         }
                     case DialogResult.No:
                         {
                             GoNext();
                             break;
                         }
                     case DialogResult.Cancel:
                         {
                             break;
                         }
                 }
                  
             }
             else
             {
                 GoNext();
             }
        }

        private void richTextValue_TextChanged(object sender, EventArgs e)
        {
            if (updatingColor) return;
            tmrWait.Stop();
            tmrWait.Start();
        }

        private void tmrWait_Tick(object sender, EventArgs e)
        {
            tmrWait.Stop();
            if (_entity != null)
            {
                int index = richTextValue.SelectionStart;
                //richTextValue.SelectionStart = index;
                _currentSpellIntermediate.SetValue(richTextValue.Text);
                _entity = _aryaSpellChecker.GetSpellCheckEntity(_currentSpellIntermediate);
                ShowCurrentItem();
                richTextValue.Select(index, 0);
            }
        }

        private void FrmSpellCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( AryaTools.Instance.RemarkID == SpellCheckRemark.ID)
                AryaTools.Instance.RemarkID = new Guid(FrmRemark.DefaultRemarkID);
            if (InvalidateMethod != null)
                InvalidateMethod.Invoke();
        }
        private void btnReplace_Click(object sender, EventArgs e)
        {
            string selectedRecommendedWord = listBox.SelectedItem.ToString();
            ReplaceText(selectedRecommendedWord);
        }

        private void listBox_Click(object sender, EventArgs e)
        {
            btnReplace.Enabled = true;
            AryaTools.Instance.RemarkID = new Guid(FrmRemark.DefaultRemarkID);
        }

        private void richTextValue_SelectionChanged(object sender, EventArgs e)
        {
            var position = richTextValue.SelectionStart;
            List<Char> firstPart = new List<char>();
            List<Char> lastPart = new List<char>();
            foreach (char letter in richTextValue.Text.Substring(0, position).ToCharArray().Reverse())
            {
                if (!_aryaSpellChecker.delimiterChars.Any(ch => ch == letter))
                {
                    firstPart.Add(letter);
                    continue;
                }
                break;
            }
            firstPart.Reverse();
            foreach (char letter in richTextValue.Text.Substring(position).ToCharArray())
            {
                if (!_aryaSpellChecker.delimiterChars.Any(ch => ch == letter))
                {
                    lastPart.Add(letter);
                    continue;
                }
                break;
            }
            firstPart.AddRange(lastPart);

            string finalString = String.Concat(firstPart);
            labelCurrentWord.Text = finalString;
            PopulateRecommendationListBox();
        }

        private void richTextValue_Click(object sender, EventArgs e)
        {
            var position = richTextValue.SelectionStart - richTextValue.GetFirstCharIndexOfCurrentLine();
        }

    }
}
