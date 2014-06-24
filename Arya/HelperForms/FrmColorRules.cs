using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using LinqKit;
using Arya.HelperClasses;
using Arya.Data;
using Arya.Framework.Utility;
using Arya.Framework.Data.AryaDb;
using Arya.SpellCheck;

namespace Arya.HelperForms
{
    public partial class FrmColorRules : Form
    {
       
        #region Fields (5)
        
        private List<ColorRuleAttribute> _colorRuleAttributes;
        public static ColorRuleAttribute RuleCreatedOn = new ColorRuleAttribute
                                                             {
                                                                 AttributeName = "CreatedOn",
                                                                 DataType = ColorRuleAttribute.ColorRuleDataType.Datetime
                                                             };
        public static ColorRuleAttribute RuleLastUpdatedOn = new ColorRuleAttribute
                                                                 {
                                                                     AttributeName = "LastUpdatedOn",
                                                                     DataType = ColorRuleAttribute.ColorRuleDataType.Datetime
                                                                 };
        public static ColorRuleAttribute RuleUom = new ColorRuleAttribute
                                                       {
                                                           AttributeName = "Uom",
                                                           DataType = ColorRuleAttribute.ColorRuleDataType.Text
                                                       };
        public static ColorRuleAttribute RuleValue = new ColorRuleAttribute
                                                         {
                                                             AttributeName = "Value",
                                                             DataType = ColorRuleAttribute.ColorRuleDataType.Text
                                                         };
        public static ColorRuleAttribute RuleSchematus = new ColorRuleAttribute
                                                        {
                                                            AttributeName = "Meta-attribute",
                                                            DataType = ColorRuleAttribute.ColorRuleDataType.Schematus
                                                        };
        public static ColorRuleAttribute SpellCheck = new ColorRuleAttribute
                                                        {
                                                            AttributeName = "SpellCheck",
                                                            DataType = ColorRuleAttribute.ColorRuleDataType.SpellCheck
                                                        };

        #endregion Fields

        #region Constructors (1)

        public FrmColorRules()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            cdgv.AutoGenerateColumns = false;
        }

        #endregion Constructors

        #region Properties (1)

        public BindingList<ColorRule> WorkingRules { get; private set; }
        

        #endregion Properties

        #region Methods (14)

        // Public Methods (1) 

        public void LoadRules(List<ColorRuleAttribute> colorRuleAttributes, IEnumerable<ColorRule> colorRules)
        {
            DialogResult = DialogResult.Cancel;
            var operatorColumn = (DataGridViewComboBoxColumn)cdgv.Columns["colOperator"];
            if (operatorColumn != null)
            {
                ColorRuleAttribute.TextOperators.ForEach(i => operatorColumn.Items.Add(i));
                ColorRuleAttribute.DatetimeOperators.ForEach(i => operatorColumn.Items.Add(i));
                ColorRuleAttribute.SchematusOperators.ForEach(i => operatorColumn.Items.Add(i));
            }

            _colorRuleAttributes = colorRuleAttributes;
            WorkingRules = new BindingList<ColorRule>();
            colorRules.ForEach(WorkingRules.Add);
            cdgv.DataSource = WorkingRules;

            ddRuleAttributes.DataSource = _colorRuleAttributes;
            ddRuleAttributes.SelectedIndex = 0;

            EnableDisableApplyButton();
        }
        // Private Methods (13) 

        private void btnAddRule_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var ruleAttribute = (ColorRuleAttribute)ddRuleAttributes.SelectedItem;
            WorkingRules.Add(
                new ColorRule { RuleAttribute = ruleAttribute, BackColor = Color.White, TextColor = Color.Black });
            cdgv[0, cdgv.RowCount - 1].Selected = true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnDeleteRules_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IEnumerable<int> selectedRows =
                cdgv.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.RowIndex).Distinct();
            foreach (int rowIndex in selectedRows)
            {
                var rule = (ColorRule)cdgv.Rows[rowIndex].DataBoundItem;
                WorkingRules.Remove(rule);
            }
        }

        private void cdgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            EnableDisableApplyButton(true);
        }

        private void cdgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            TryGetTextAndBackColors(e);
        }

        private void cdgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            EnableDisableApplyButton();

            if (e.RowIndex < 0)
                return;

            DataGridViewColumn operatorColumn = cdgv.Columns["colOperator"];
            if (operatorColumn != null && e.ColumnIndex == operatorColumn.DisplayIndex)
            {
                var currentCell = (DataGridViewComboBoxCell)cdgv["colOperator", e.RowIndex];
                var currentRule = (ColorRule)currentCell.OwningRow.DataBoundItem;
                if (currentCell.Items.Count != currentRule.RuleAttribute.Operators.Count)
                {
                    var currentOperator = currentRule.Operator;
                    currentRule.Operator = null;
                    currentCell.Items.Clear();
                    currentCell.Items.AddRange(currentRule.RuleAttribute.Operators.ToArray());
                    currentRule.Operator = currentOperator;
                }
                cdgv.BeginEdit(true);
                var dd = (DataGridViewComboBoxEditingControl)cdgv.EditingControl;
                if (dd != null)
                    dd.DroppedDown = true;
                return;
            }

            TryGetTextAndBackColors(e);
        }

        private void cdgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewCell currentCell = cdgv[e.ColumnIndex, e.RowIndex];
            var rule = (ColorRule)currentCell.OwningRow.DataBoundItem;

            DataGridViewColumn colorColumn = cdgv.Columns["colColorSample"];
            if (colorColumn != null && e.ColumnIndex == colorColumn.DisplayIndex)
            {
                currentCell.Style.ForeColor = rule.TextColor;
                currentCell.Style.BackColor = rule.BackColor;
                return;
            }
        }

        private void EnableDisableApplyButton(bool editing = false)
        {
            btnApply.Enabled = !editing && WorkingRules.All(rule => rule.IsValidRule());
        }

        private void lnkLoad_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (openFileDialogXml.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                var deserializer = new XmlSerializer(WorkingRules.GetType());
                TextReader file = new StreamReader(openFileDialogXml.FileName);

                var newRules = (BindingList<ColorRule>)deserializer.Deserialize(file);
                WorkingRules.Clear();
                newRules.ForEach(rule => WorkingRules.Add(rule));
            }
            catch (Exception ex)
            {
                FrmWaitScreen.ShowMessage("Unable to load from file: " + ex.Message, true);
            }
        }

        private void lnkMoveRuleDown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var item = (ColorRule)cdgv.CurrentCell.OwningRow.DataBoundItem;
                var currentIndex = WorkingRules.IndexOf(item);
                if (currentIndex < WorkingRules.Count - 1)
                {
                    Point currentCellAddress = cdgv.CurrentCellAddress;
                    WorkingRules.Remove(item);
                    WorkingRules.Insert(currentIndex + 1, item);
                    cdgv.CurrentCell = cdgv[currentCellAddress.X, currentCellAddress.Y + 1];
                }
            }
            catch (Exception ex)
            {
                FrmWaitScreen.ShowMessage("Please select a rule to Move: "+ex.Message, true);
            }
        }

        private void lnkMoveRuleUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var item = (ColorRule)cdgv.CurrentCell.OwningRow.DataBoundItem;
                var currentIndex = WorkingRules.IndexOf(item);
                if (currentIndex > 0)
                {
                    Point currentCellAddress = cdgv.CurrentCellAddress;
                    WorkingRules.Remove(item);
                    WorkingRules.Insert(currentIndex - 1, item);
                    cdgv.CurrentCell = cdgv[currentCellAddress.X, currentCellAddress.Y - 1];
                }
            }
            catch (Exception ex)
            {
                FrmWaitScreen.ShowMessage("Please select a rule to Move: " + ex.Message, true);
            }
        }

        private void lnkSave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialogXml.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                var serializer = new XmlSerializer(WorkingRules.GetType());
                TextWriter file = new StreamWriter(saveFileDialogXml.FileName);
                serializer.Serialize(file, WorkingRules);
                file.Close();
            }
            catch (Exception ex)
            {
                FrmWaitScreen.ShowMessage("Unable to save: " + ex.Message, true);
            }
        }

        private void TryGetTextAndBackColors(DataGridViewCellEventArgs e)
        {
            DataGridViewColumn backColorColumn = cdgv.Columns["colBackColor"];
            if (backColorColumn != null && e.ColumnIndex == backColorColumn.DisplayIndex)
            {
                if (cellColorDialog.ShowDialog() == DialogResult.OK)
                {
                    var currentRule = (ColorRule)cdgv.CurrentCell.OwningRow.DataBoundItem;
                    currentRule.BackColor = cellColorDialog.Color;
                    cdgv.InvalidateRow(e.RowIndex);
                }
                return;
            }

            DataGridViewColumn textColorColumn = cdgv.Columns["colTextColor"];
            if (textColorColumn != null && e.ColumnIndex == textColorColumn.DisplayIndex)
            {
                if (cellColorDialog.ShowDialog() == DialogResult.OK)
                {
                    var currentRule = (ColorRule)cdgv.CurrentCell.OwningRow.DataBoundItem;
                    currentRule.TextColor = cellColorDialog.Color;
                    cdgv.InvalidateRow(e.RowIndex);
                }
                return;
            }
        }

        #endregion Methods
    }

    public enum SpellCheckOperatorsEnum
    {
        IsCorrect,
        IsNotCorrect
    }
    public class ColorRuleAttribute
    {
        #region Fields (3)

        private ColorRuleDataType _dataType;
        internal static readonly List<string> DatetimeOperators = new List<string> { "equals", "after", "before", "between" };
        internal static readonly List<string> TextOperators = new List<string>
                                                                 {
                                                                     "equals", "contains", "starts with", "ends with",
                                                                     "is blank", "has a value"
                                                                 };
        internal static readonly List<string> SchematusOperators = new List<string>
                                                                 {
                                                                     "in schema", "is navigation", "is display", "is ranked",
                                                                     "is extended"
                                                                 };
        internal static readonly List<SpellCheckOperatorsEnum> SpellCheckOperators = new List<SpellCheckOperatorsEnum>
                                                                 {
                                                                     SpellCheckOperatorsEnum.IsCorrect,SpellCheckOperatorsEnum.IsNotCorrect
                                                                 };

        #endregion Fields

        #region Properties (3)

        public string AttributeName { get; set; }

        public ColorRuleDataType DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;
                switch (value)
                {
                    case ColorRuleDataType.Text:
                        Operators = TextOperators;
                        break;
                    case ColorRuleDataType.Datetime:
                        Operators = DatetimeOperators;
                        break;
                    case ColorRuleDataType.Schematus:
                        Operators = SchematusOperators;
                        break;
                    case ColorRuleDataType.SpellCheck:
                        Operators = new List<string>() {SpellCheckOperatorsEnum.IsCorrect.ToString(),SpellCheckOperatorsEnum.IsNotCorrect.ToString() };
                        break;
                    case ColorRuleDataType.Number:
                        throw new ArgumentOutOfRangeException("value");
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        [XmlIgnore]
        public List<string> Operators { get; private set; }

        #endregion Properties

        #region Methods (1)

        // Public Methods (1) 

        public override string ToString()
        {
            return AttributeName;
        }

        #endregion Methods


        #region ColorRuleDataType enum

        public enum ColorRuleDataType
        {
            Text,
            Number,
            Datetime,
            Schematus,
            SpellCheck
        }

        #endregion
    }

    public class ColorRule
    {
        private SpellChecker currentSpellChecker = new SpellChecker();
        #region Properties (6)

        [XmlElement(Type = typeof(XmlColor))]
        public Color BackColor { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        public Color TextColor { get; set; }

        public string Operator { get; set; }

        public ColorRuleAttribute RuleAttribute { get; set; }

        public string SampleText
        {
            get { return "Sample Text"; }
        }

        public string Value { get; set; }

        #endregion Properties

        #region Methods (6)

        // Public Methods (2) 

        public bool IsValidRule()
        {
            switch (RuleAttribute.DataType)
            {
                case ColorRuleAttribute.ColorRuleDataType.Text:
                    return ValidateTextRule();
                case ColorRuleAttribute.ColorRuleDataType.Datetime:
                    return ValidateDateTimeRule();
                case ColorRuleAttribute.ColorRuleDataType.Schematus:
                    return ValidateSchematusRule();
                case ColorRuleAttribute.ColorRuleDataType.SpellCheck:
                    return ValidateSpellCheckRule();
                case ColorRuleAttribute.ColorRuleDataType.Number:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

       

        

        public bool IsValidValue(object testValue)
        {
            switch (RuleAttribute.DataType)
            {
                case ColorRuleAttribute.ColorRuleDataType.Text:
                    return ValidateTextValue((string)testValue);
                case ColorRuleAttribute.ColorRuleDataType.Datetime:
                    return ValidateDateTimeRule((DateTime)testValue);
                case ColorRuleAttribute.ColorRuleDataType.Schematus:
                    return ValidateSchematusRule((Arya.Data.EntityData)testValue);
                case ColorRuleAttribute.ColorRuleDataType.SpellCheck:
                    return ValidateSpellCheckValue((Arya.Data.EntityData)testValue);
                case ColorRuleAttribute.ColorRuleDataType.Number:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

       
       
        // Private Methods (4) 
        private bool ValidateSpellCheckValue(Arya.Data.EntityData entityData)
        {
            if (Operator.Equals(SpellCheckOperatorsEnum.IsNotCorrect.ToString()))
            {
                //return !currentSpellChecker.GetSpellCheckEntity(entityData).Correct;
                return true;
            }
            else if (Operator.Equals(SpellCheckOperatorsEnum.IsCorrect.ToString()))
            {
                //return currentSpellChecker.GetSpellCheckEntity(entityData).Correct;
                return true;
            }
            else
            {
                throw new ArgumentException("Not a valid spell check orerator.");
            }
           
        }
        private bool ValidateSpellCheckRule()
        {
            return (Operator != null);
        }
        private bool ValidateSchematusRule()
        {
            var schemaOperators = new[] { "in schema", "is navigation", "is display", "is ranked", "is extended" };
            return (Operator != null &&
                    ((schemaOperators.Contains(Operator) && string.IsNullOrEmpty(Value))));
        }

        private bool ValidateSchematusRule(Arya.Data.EntityData testValue)
        {
            var attribute = testValue.Attribute;
            if (attribute == null)
                return false;

            var taxonomy = testValue.Sku.SkuInfos.Single(a => a.Active).TaxonomyInfo;
            if (taxonomy == null)
                return false;

            var schemaInfo = taxonomy.SchemaInfos.SingleOrDefault(a => a.Attribute == attribute);
            if (schemaInfo == null)
            {
                return Operator == "is extended";
            }

            var schemaData = schemaInfo.SchemaData;
            if (schemaData == null)
            {
                return Operator == "is extended";
            }

            switch (Operator)
            {
                case "in schema":
                    return schemaData.InSchema;
                case "is navigation":
                    return (schemaData.NavigationOrder > 0);
                case "is display":
                    return (schemaData.DisplayOrder > 0);
                case "is ranked":
                    return (schemaData.NavigationOrder > 0 || schemaData.DisplayOrder > 0);
                case "is extended":
                    return (schemaData.Active == false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
                

        }

        private bool ValidateDateTimeRule()
        {
            if (string.IsNullOrEmpty(Operator) || string.IsNullOrEmpty(Value))
                return false;

            DateTime dt;
            if (Operator.Equals("between"))
            {
                string[] parts = Value.ToLower().Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Count() == 2)
                {
                    DateTime before, after;
                    if (DateTime.TryParse(parts[0].Trim(), out before) && DateTime.TryParse(parts[1].Trim(), out after) && before < after)
                    {
                        Value = before + " and " + after;
                        return true;
                    }
                }
                return false;
            }

            if (DateTime.TryParse(Value, out dt))
            {
                Value = dt.ToString();
                return true;
            }

            return false;
        }

        private bool ValidateDateTimeRule(DateTime testValue)
        {
            DateTime ruleValue;
            DateTime.TryParse(Value, out ruleValue);
            switch (Operator)
            {
                case "equals":
                    return testValue == ruleValue;
                case "after":
                    return testValue >= ruleValue;
                case "before":
                    return testValue <= ruleValue;
                case "between":
                    string[] parts = Value.ToLower().Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime firstPart = DateTime.Parse(parts[0]);
                    DateTime secondPart = DateTime.Parse(parts[1]);
                    return testValue >= firstPart && testValue <= secondPart;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool ValidateTextRule()
        {
            var valueSeekingOperators = new[] { "equals", "contains", "starts with", "ends with" };
            var valueNonseekingOperators = new[] { "has a value", "is blank" };
            return (Operator != null &&
                    ((valueSeekingOperators.Contains(Operator) && !string.IsNullOrEmpty(Value)) ||
                     (valueNonseekingOperators.Contains(Operator) && string.IsNullOrEmpty(Value))));
        }

        private bool ValidateTextValue(string testValue)
        {
            switch (Operator)
            {
                case "equals":
                    return !string.IsNullOrEmpty(testValue) && testValue.ToLower().Equals(Value.ToLower());
                case "contains":
                    return !string.IsNullOrEmpty(testValue) && testValue.ToLower().Contains(Value.ToLower());
                case "starts with":
                    return !string.IsNullOrEmpty(testValue) && testValue.ToLower().StartsWith(Value.ToLower());
                case "ends with":
                    return !string.IsNullOrEmpty(testValue) && testValue.ToLower().EndsWith(Value.ToLower());
                case "has a value":
                    return !string.IsNullOrEmpty(testValue);
                case "is blank":
                    return string.IsNullOrEmpty(testValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Methods
    }
}