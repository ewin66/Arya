using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using LinqKit;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;
using Arya.HelperClasses;
using Arya.HelperForms;
using Attribute = Arya.Data.Attribute;
using EntityData = Arya.Data.EntityData;
using EntityInfo = Arya.Data.EntityInfo;
using SchemaAttribute = Arya.HelperClasses.SchemaAttribute;
using SchemaData = Arya.Data.SchemaData;
using SchemaInfo = Arya.Data.SchemaInfo;
using Sku = Arya.Data.Sku;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;

namespace Arya.UserControls
{
    public partial class BuildDataGridView : UserControl
    {
        #region Fields

        private const string AttributeHeader = "Attribute";
        private const string MultiValueSuffix = " • ";
        private const string UomHeader = "UoM";
        private const string ValueHeader = "Value";

        private static readonly List<Attribute> SelectedGlobalAttributes = new List<Attribute>();
        private static bool _refreshGridView;

        //private static Color _columnColor = Color.LightGray;
        private readonly List<String> _attributeHeaders; // = new List<string>();

        //private readonly List<Attribute> _attributes;
        private readonly Dictionary<Attribute, List<string>> _autoCompleteUomSourceDictionary =
            new Dictionary<Attribute, List<string>>();

        private readonly Dictionary<Attribute, List<string>> _autoCompleteValueSourceDictionary =
            new Dictionary<Attribute, List<string>>();

        private readonly Dictionary<string, int> _columnWidth; // = new Dictionary<string, int>();
        private readonly List<Sku> _currentSkus;
        private readonly TaxonomyInfo _currentTaxonomy;
        private readonly List<Attribute> _globalAttributes = new List<Attribute>();
        private readonly List<String> _itemHeaders; // = new List<string>(){"Value","UoM"};
        private readonly List<Arya.HelperClasses.SchemaAttribute> _metaAttributes = new List<Arya.HelperClasses.SchemaAttribute>();
        private readonly Stack<EntityDataGridView.ChangeItem> _undoHistory = new Stack<EntityDataGridView.ChangeItem>();

        private List<Attribute> _allAttributes;
        private Dictionary<Attribute, int> _attRows;

        //private int _attributeHeaders.Count; // = 0;
        private List<string> _autoCompleteAttributeNameSource;
        private List<string> _autoFillSource = new List<string>();
        private List<object> _columns;
        private AttributeFilterEnum _currentAttributeFilter = AttributeFilterEnum.Inschema;
        private List<Attribute> _currentAttributes = new List<Attribute>();
        private Change _currentChange;
        private AttributeListSort.Order _currrentOrder = AttributeListSort.Order.Ascending;
        private string _field1Header = "Field 1";
        private string _field2Header = "Field 2";
        private string _field3Header = "Field 3";
        private string _field4Header = "Field 4";
        private string _field5Header = "Field 5";
        private List<string> _rows;
        private Attribute _selectedAttribute;
        private List<EntityData> _selectedEntityDatas = new List<EntityData>();
        private Sku _selectedSku;
        private List<Sku> _selectedSkus = new List<Sku>();
        private bool _validateGrid;

        #endregion Fields

        #region Constructors

        public BuildDataGridView(TaxonomyInfo taxonomyInfo, List<Sku> skus, DisplayColumns cols)
            : this()
        {
            _currentSkus = skus;
            _currentTaxonomy = taxonomyInfo ?? new TaxonomyInfo();
            _itemHeaders = cols.ItemHeaders;
            _attributeHeaders = cols.AttributeHeaders;
            _columnWidth = cols.ColumnWidth;

            InitData();
            InitMetaAttributes();
            InitControlStates();

            FetchAutoFillSource();
        }

        private BuildDataGridView()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
        }

        #endregion Constructors

        #region Enumerations

        private enum AttributeFilterEnum
        {
            All,
            Navigation,
            Display,
            Inschema,
            Extended
        }

        private enum Headers
        {
            Attribute,
            Sku
        }

        #endregion Enumerations

        #region Properties

        internal int PageIndex { get; set; }

        private static bool AutoOrderRanks { get; set; }

        private bool MoveToNextValue { get; set; }

        private bool MoveToUom { get; set; }

        private Attribute SelectedAttribute
        {
            get { return _selectedAttribute; }
            set
            {
                if (value == _selectedAttribute)
                    return;

                _selectedAttribute = value;
                UpdateMetaAttributes();
                UpdateLovs();
            }
        }

        private Sku SelectedSku
        {
            get { return _selectedSku; }
            set
            {
                if (value == _selectedSku)
                    return;

                _selectedSku = value;
                UpdateGlobalAttributes();
            }
        }

        #endregion Properties

        #region Methods

        private static string GetPushedValue(object e)
        {
            var data = e as EntityData;
            if (data != null)
            {
                if (data.Value == null || data.Value.Trim().ToLower() == "null")
                    return null;
                return data.Value.Trim();
            }
            if (e == null || e.ToString().Trim().ToLower() == "null")
                return null;
            return e.ToString();
        }

        private static bool IsValid(EntityData ed)
        {
            var sku = ed.Sku;
            var taxonomy = sku.Taxonomy;
            var si = taxonomy.SchemaInfos.FirstOrDefault(a => a.Attribute == ed.Attribute);
            // do not change/comment this code!
            if (si == null) // do not change/comment this code!
                return false;

            var schemaData = si.SchemaDatas.FirstOrDefault(a => a.Active); // do not change/comment this code!
            if (schemaData == null)
                return false;

            return HelperClasses.Validate.IsValidDataType(ed, schemaData);
        }

        private void addMultiValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var incomingSuffix = 0;
            var insertIndex = bdgv.SelectedCells.Cast<DataGridViewCell>().Select(r => r.RowIndex).Min();

            if (insertIndex < 2)
                return;
            var att =
                bdgv[_attributeHeaders.Count, insertIndex].Value.ToString().Split(MultiValueSuffix.Trim().ToCharArray());
            if (att.Count() > 1)
                Int32.TryParse(att[1], out incomingSuffix);

            var rowCount = _attRows[Attribute.GetAttributeFromName(att[0], false)];
            var insertCount = bdgv.SelectedCells.Count;

            for (var i = 0; i < insertCount; i++)
            {
                _rows.Insert(insertIndex + (rowCount - incomingSuffix - 1) + i - 1,
                    att[0].Trim() + MultiValueSuffix + (rowCount + i));
            }

            _attRows[Attribute.GetAttributeFromName(att[0], false)] = rowCount + insertCount;

            ResetRowColumnCount();
            ApplyStyles();
        }

        private void AddNewAttribute(Attribute attribute)
        {
            var newSi = new SchemaInfo { Attribute = attribute };
            var newSd = new SchemaData { DataType = "Text" };
            newSi.SchemaDatas.Add(newSd);
            _currentTaxonomy.SchemaInfos.Add(newSi);
            _attRows.Add(attribute, 1);
        }

        private void addNewSKUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newSkusForm = new FrmNewSkus(_currentTaxonomy)
                              {
                                  Left = AryaTools.Instance.Forms.TreeForm.Right,
                                  Top = AryaTools.Instance.Forms.BuildForm.Top + 200
                              };
            newSkusForm.Show();
            newSkusForm.SkusAdded += UpdateSkus;
        }

        private void AddToAutoCompleteSource(string incomingValue,
            Dictionary<Attribute, List<string>> autoCompleteDictionary)
        {
            var selectedAttributes = _selectedEntityDatas.Select(a => a.Attribute).Distinct().ToList();
            foreach (var att in selectedAttributes)
            {
                if (!autoCompleteDictionary.Keys.Contains(att))
                    continue;

                var uniqueValues = autoCompleteDictionary[att];
                if (!uniqueValues.Contains(incomingValue))
                    uniqueValues.Add(incomingValue);
            }
        }

        private void ApplyStyles()
        {
            for (var i = 0; i < bdgv.ColumnCount; i++)
            {
                if (i < _attributeHeaders.Count)
                    ApplySchematusStyle(i);
                else if (i > _attributeHeaders.Count && (i - _attributeHeaders.Count - 1) % _itemHeaders.Count == 0)
                    ApplySkuStyle(i);
                else if (i == _attributeHeaders.Count) //No AttributeHeaders
                {
                    bdgv.Columns[i].DefaultCellStyle = DisplayStyle.CellStyleAttributeColumn;
                    bdgv.Columns[i].Frozen = true;
                    bdgv.Columns[i].Width = 200;
                }
            }

            for (var i = 0; i < bdgv.RowCount; i++)
            {
                if (i % 2 != 0)
                    bdgv.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;
            }

            bdgv.Rows[0].DefaultCellStyle = DisplayStyle.CellStyleFirstRowItemIDHeader;
            bdgv.Rows[0].Frozen = true;
            bdgv.Rows[0].ReadOnly = true;
            bdgv.Rows[1].DefaultCellStyle = DisplayStyle.CellStyleSecondRowItemFields;
            bdgv.Rows[1].Frozen = true;
            bdgv.Rows[1].ReadOnly = true;

            bdgv[_attributeHeaders.Count, 1].ReadOnly = false;
            bdgv[_attributeHeaders.Count, 1] = new DataGridViewComboBoxCell
                                               {
                                                   DataSource =
                                                       Enum.GetValues(
                                                           typeof(AttributeFilterEnum))
                                               };
        }

        private void ApplySkuStyle(int column)
        {
            var skuLocation = GetSkuIndex(column) + 1;
            bdgv.Columns[column + 1].Width = 35;

            if (skuLocation % 2 == 0)
            {
                bdgv.Columns[column].DefaultCellStyle = DisplayStyle.CellStyleItemEvenColumn;
                for (var j = 1; j < _itemHeaders.Count; j++)
                    bdgv.Columns[column + j].DefaultCellStyle = DisplayStyle.CellStyleItemEvenColumn;
            }
            else
            {
                bdgv.Columns[column].DefaultCellStyle = DisplayStyle.CellStyleItemOddColumn;
                for (var j = 1; j < _itemHeaders.Count; j++)
                    bdgv.Columns[column + j].DefaultCellStyle = DisplayStyle.CellStyleItemOddColumn;
            }
        }

        private void ApplySchematusStyle(int column)
        {
            bdgv.Columns[column].Width = _columnWidth.Keys.Contains(_attributeHeaders[column])
                ? _columnWidth[_attributeHeaders[column]]
                : 40;

            bdgv.Columns[column].ReadOnly = true;
            bdgv.Columns[column].Frozen = true;

            var schemaAttribute = _metaAttributes.First(ma => ma.ToString() == bdgv[column, 0].Value.ToString());
            switch (schemaAttribute.AttributeType)
            {
                case SchemaAttribute.SchemaAttributeType.Primary:
                    if (schemaAttribute.DataType == typeof(decimal))
                        bdgv.Columns[column].DefaultCellStyle = DisplayStyle.CellStyleSchemaDecimal;
                    else  if (schemaAttribute.DataType == typeof(bool))
                        bdgv.Columns[column].DefaultCellStyle = DisplayStyle.CellStyleSchemaBoolean;
                    break;

                case SchemaAttribute.SchemaAttributeType.Meta:
                    if (schemaAttribute.DataType == typeof(bool))
                        bdgv.Columns[column].DefaultCellStyle = DisplayStyle.CellStyleSchemaMetaBoolean;
                    break;

                default:
                    bdgv.Columns[column].DefaultCellStyle = DisplayStyle.CellStyleGreyItalic;
                    break;
            }
        }

        private void AutoEndEditing(object sender, EventArgs e)
        {
            var dataGridView = (DataGridView)sender;

            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (dataGridView.IsCurrentCellInEditMode)
                dataGridView.EndEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void bdgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 || e.ColumnIndex >= _attributeHeaders.Count + 1)
                return;
            var headerClicked = bdgv[e.ColumnIndex, 0].Value.ToString();

            var primaryMeta = SchemaAttribute.PrimarySchemati.FirstOrDefault(sa=>sa.ToString()==headerClicked);
            if (primaryMeta != null)
                SortBy(
                    (AttributeListSort.Field)
                        Enum.Parse(typeof (AttributeListSort.Field), primaryMeta.ToString().Replace(" ", "")));
            else if (headerClicked == AttributeHeader)
                SortBy(AttributeListSort.Field.AttributeName);
            else if (_attributeHeaders.Contains(headerClicked))
            {
                SortBy(AttributeListSort.Field.MetaAttribute,
                    _metaAttributes.First(a => a.ToString() == headerClicked).MetaSchemaAttribute);
            }
        }

        private void bdgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (bdgv.SelectedCells.Count <= 1 || e.ColumnIndex <= _attributeHeaders.Count || e.RowIndex <= 1
                || bdgv[e.ColumnIndex, 1].Value.ToString() != ValueHeader)
                return;

            var rowIndices = new List<int>();
            for (var i = 0; i < bdgv.SelectedCells.Count; i++)
                rowIndices.Add(bdgv.SelectedCells[i].RowIndex);
            rowIndices = rowIndices.Distinct().ToList();
            if (rowIndices.Count > 1)
                bdgv.CurrentCell.ReadOnly = true;
        }

        private void bdgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= -1 || e.RowIndex <= -1 || e.Value == null)
                return;
            
            if (e.CellStyle.FormatProvider is BoolFormatter)
            {
                e.Value =
                    ((ICustomFormatter)e.CellStyle.FormatProvider.GetFormat(typeof(ICustomFormatter))).Format(
                        e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }

            var cell = bdgv[e.ColumnIndex, e.RowIndex];
            cell.ToolTipText = e.Value.ToString();
        }

        private void bdgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetCellBorders(e);

            if (!_validateGrid || !(e.Value is EntityData))
                return;

            if (!IsValid((EntityData)e.Value))
                e.CellStyle.ForeColor = Color.Crimson;
        }

        //private object GetRowItem(int rowIndex)
        //{
        //    return rowIndex > _rows.Count && CurrentTaxonomy != null
        //               ? _blankAttribute.ToString()
        //               : rowIndex > 0 && rowIndex <= _rows.Count ? _rows[rowIndex - 1] : null;
        //}
        //private object GetColumnItem(int columnIndex)
        //{
        //    if ((columnIndex - _attributeHeaders.Count) > 0)
        //    {
        //        var columnIndexMinusHeaders = columnIndex - _attributeHeaders.Count;
        //        var skuIndex = (columnIndexMinusHeaders/_itemHeaders.Count)
        //                       + ((columnIndexMinusHeaders%_itemHeaders.Count) > 0 ? 1 : 0);
        //        return columnIndexMinusHeaders > 0 ? _columns[skuIndex - 1] : null;
        //    }
        //    return GetAttributeHeader(columnIndex);
        //}
        //private string GetAttributeHeader(int columnIndex)
        //{
        //    switch (columnIndex)
        //    {
        //        case 1:
        //            return _attributeHeaders[0];
        //        case 2:
        //            return _attributeHeaders[1];
        //        case 3:
        //            return _attributeHeaders[2];
        //        default:
        //            return null;
        //    }
        //}
        private void bdgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == _attributeHeaders.Count && e.RowIndex == 1)
                e.Value = _currentAttributeFilter;

            else if (e.RowIndex == 0) // Display ItemIDs
            {
                if (e.ColumnIndex < _attributeHeaders.Count)
                    e.Value = _attributeHeaders[e.ColumnIndex];

                else if (e.ColumnIndex - _attributeHeaders.Count == 0)
                    e.Value = AttributeHeader;

                else if (e.ColumnIndex - _attributeHeaders.Count == 1)
                    e.Value = _columns[0];

                else if (((e.ColumnIndex - 1) - _attributeHeaders.Count) % _itemHeaders.Count == 0)
                    e.Value = _columns[(e.ColumnIndex - _attributeHeaders.Count) / _itemHeaders.Count];
            }

            else if (e.RowIndex == 1) //Display Value, Uom, Field1 - Headers
                e.Value = e.ColumnIndex > _attributeHeaders.Count ? GetItemHeaders(e.ColumnIndex) : null;

            else if (e.ColumnIndex == _attributeHeaders.Count)
            {
                if (e.RowIndex > 1 && (e.RowIndex - 2) < _rows.Count)
                    e.Value = _rows[e.RowIndex - 2];
                else
                {
                    e.Value = null;
                    _autoFillSource = _autoCompleteAttributeNameSource;
                }
            }

            else //Display Actual Values
            {
                int multiValueIndex;
                var sku = GetSku(e.ColumnIndex);

                var attribute = GetAttribute(e.RowIndex, out multiValueIndex);
                var itemHeader = bdgv[e.ColumnIndex, 1];
                var attributeHeader = e.ColumnIndex < _attributeHeaders.Count ? _attributeHeaders[e.ColumnIndex] : null;

                e.Value = GetValue(sku, attribute, multiValueIndex, itemHeader, attributeHeader);
            }
        }

        private void bdgv_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var selectedColumnIndex = e.ColumnIndex;
            var selectedRowIndex = e.RowIndex;
            var incomingValue = GetPushedValue(e.Value);
            if (incomingValue == null)
                return;

            if (selectedColumnIndex == _attributeHeaders.Count && selectedRowIndex == 1)
            {
                AttributeFilterEnum incomingFilter;
                Enum.TryParse(e.Value.ToString(), out incomingFilter);
                if (incomingFilter == _currentAttributeFilter)
                    return;

                _currentAttributeFilter = incomingFilter;
                _rows = GetRows(_currentAttributeFilter);
                PrepareGridView();
                return;
            }

            var itemHeader = bdgv[selectedColumnIndex, 1].Value.ToString();

            if (itemHeader == ValueHeader)
            {
                PrepareChange(true);
                _currentChange.NewValues.Val = incomingValue;
                AddToAutoCompleteSource(incomingValue, _autoCompleteValueSourceDictionary);
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }
            if (itemHeader == UomHeader)
            {
                PrepareChange(false);
                _currentChange.NewValues.Uom = incomingValue;
                AddToAutoCompleteSource(incomingValue, _autoCompleteUomSourceDictionary);
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }
            if (itemHeader == _field1Header)
            {
                PrepareChange(false);
                _currentChange.NewValues.Field1 = incomingValue;
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }
            if (itemHeader == _field2Header)
            {
                PrepareChange(false);
                _currentChange.NewValues.Field2 = incomingValue;
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }
            if (itemHeader == _field3Header)
            {
                PrepareChange(false);
                _currentChange.NewValues.Field3 = incomingValue;
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }
            if (itemHeader == _field4Header)
            {
                PrepareChange(false);
                _currentChange.NewValues.Field4 = incomingValue;
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }
            if (itemHeader == _field5Header)
            {
                PrepareChange(false);
                _currentChange.NewValues.Field5 = incomingValue;
                _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            }

            if (selectedColumnIndex == _attributeHeaders.Count)
            {
                if (selectedRowIndex > (_rows.Count + 1))
                // if the new attribute entered at the last row of the Grid
                {
                    var attribute = Attribute.GetAttributeFromName(incomingValue, false);
                    if (attribute != null)
                    {
                        var si = _currentTaxonomy.SchemaInfos.FirstOrDefault(a => a.Attribute == attribute);
                        if (si == null)
                        {
                            AddNewAttribute(attribute);
                            _rows.Add(attribute.AttributeName);
                            PrepareGridView();
                        }
                    }
                    else
                    {
                        attribute = Attribute.GetAttributeFromName(incomingValue, true);
                        _autoCompleteAttributeNameSource.Add(attribute.AttributeName);
                        AddNewAttribute(attribute);
                        _rows.Add(attribute.AttributeName);
                        PrepareGridView();
                    }
                }
            }

            if (_refreshGridView)
            {
                bdgv.Invalidate();
                _refreshGridView = false;
            }

            _selectedEntityDatas = null;
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void bdgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Index >= _attributeHeaders.Count)
                return;

            var colName = _attributeHeaders[e.Column.Index];
            if (_columnWidth.Keys.Contains(colName))
                _columnWidth[colName] = e.Column.Width;
            else
                _columnWidth.Add(colName, e.Column.Width);
        }

        private void bdgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (bdgv.CurrentCell != null && bdgv.CurrentCell.RowIndex == 1
                && bdgv.CurrentCell.ColumnIndex == _attributeHeaders.Count)
                bdgv.EndEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void bdgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (!(e.Control is TextBox))
                return;

            var txt = e.Control as TextBox;
            txt.Multiline = false; //This is important.
            txt.AutoCompleteCustomSource = new AutoCompleteStringCollection();

            txt.AutoCompleteCustomSource.Clear();
            txt.AutoCompleteCustomSource.AddRange(_autoFillSource == null
                ? new[] { string.Empty }
                : _autoFillSource.ToArray());
            //put attention to the "ac\nc" which contains a '\n'

            txt.AutoCompleteMode = AutoCompleteMode.Suggest;
            txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void bdgv_SelectionChanged(object sender, EventArgs e)
        {
            var selectedSkus = GetSelectedSkus();
            SelectedSku = selectedSkus.Count() == 1 ? selectedSkus[0] : null;

            if (bdgv.SelectedCells.Count == 1)
            {
                if (bdgv.CurrentCell.RowIndex > 1 && bdgv.CurrentCell.ColumnIndex > _attributeHeaders.Count
                    && bdgv[bdgv.CurrentCell.ColumnIndex, 1].Value.ToString() == "Value")
                    bdgv.CurrentCell.ReadOnly = false;

                int multivalueIndex;
                SelectedAttribute = GetAttribute(bdgv.CurrentCell.RowIndex, out multivalueIndex);

                if (bdgv.SelectedCells[0].RowIndex == 1 && bdgv.SelectedCells[0].ColumnIndex == _attributeHeaders.Count)
                    bdgv.BeginEdit(true);
            }
        }

        private void BuildDataGridView_Load(object sender, EventArgs e) { PrepareGridView(); }

        private void deleteValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you want to delete the selected entities?", @"Delete?",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            PrepareChange(false);
            _currentChange.Delete = true;
            _currentChange.ExecuteOnBuildView(_currentTaxonomy, _undoHistory, true);
            bdgv.Invalidate();
        }

        private void dgvGlobals_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1 || e.RowIndex < 0)
                return;

            var attributeValue = (AttributeValue)dgvGlobals.Rows[e.RowIndex].DataBoundItem;
            var global = _globalAttributes.First(att => att.AttributeName == attributeValue.AttributeName);

            if (attributeValue.Show)
                SelectedGlobalAttributes.Add(global);
            else
                SelectedGlobalAttributes.Remove(global);

            _rows = GetRows(_currentAttributeFilter);
            PrepareGridView();
        }

        private void dgvListOfValues_DoubleClick(object sender, EventArgs e)
        {
            bdgv.CurrentCell.Value = dgvListOfValues.CurrentCell.Value;
        }

        private void dgvMetaAttributes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var attributeValue = (AttributeValue)dgvMetaAttributes.Rows[e.RowIndex].DataBoundItem;
            ShowHideHeaders(attributeValue.AttributeName, attributeValue.Show, Headers.Attribute);
        }

        private void FetchAutoFillSource()
        {
            var waitKey = FrmWaitScreen.ShowMessage("Fetching Auto Fill Sources");

            foreach (var attr in _allAttributes)
            {
                var uniqueValues = new List<string>();
                var currentAttribute = attr;

                var schemaInfo =
                    _currentTaxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.ID == currentAttribute.ID);

                if (schemaInfo != null)
                {
                    var sd = schemaInfo.SchemaData;
                    if (sd != null)
                    {
                        if (sd.DataType.ToLower() == "lov")
                            uniqueValues = schemaInfo.ActiveListOfValues.Distinct().OrderBy(val => val).ToList();
                    }
                }

                if (uniqueValues.Count == 0)
                {
                    uniqueValues = (from sku in AryaTools.Instance.InstanceData.Dc.Skus
                                    let taxId = (from si in sku.SkuInfos where si.Active select si.TaxonomyID).First()
                                    where taxId == _currentTaxonomy.ID
                                    from ei in sku.EntityInfos
                                    from ed in ei.EntityDatas
                                    where ed.Active && ed.AttributeID == currentAttribute.ID
                                    select ed.Value).Distinct().ToList();

                    //uniqueValues = cacheDb.ExecuteQuery<string>(@"select distinct Value from EntityData as ed
                    //    join EntityInfo as ei on ei.ID = ed.EntityID and ed.Active = 1
                    //    join SkuInfo as si on si.SkuID = ei.SkuID and si.Active = 1
                    //    where si.TaxonomyID = '" + _currentTaxonomy.ID + "'" + " and ed.AttributeID = '"
                    //    + att.ID + "'").ToList();
                }

                var uniqueUoms = (from sku in AryaTools.Instance.InstanceData.Dc.Skus
                                  let taxId = (from si in sku.SkuInfos where si.Active select si.TaxonomyID).First()
                                  where taxId == _currentTaxonomy.ID
                                  from ei in sku.EntityInfos
                                  from ed in ei.EntityDatas
                                  where
                                      ed.Active && ed.AttributeID == currentAttribute.ID && ((ed.Uom ?? string.Empty) != string.Empty)
                                  select ed.Uom).Distinct().ToList();

                //var uniqueUoms = cacheDb.ExecuteQuery<string>(@"select distinct Uom from EntityData as ed
                //    join EntityInfo as ei on ei.ID = ed.EntityID and ed.Active = 1
                //    join SkuInfo as si on si.SkuID = ei.SkuID and si.Active = 1
                //    where isnull(Uom,'')<>'' and si.TaxonomyID = '" + _currentTaxonomy.ID + "'"
                //    + " and ed.AttributeID = '" + att.ID + "'").ToList();

                _autoCompleteValueSourceDictionary.Add(currentAttribute, uniqueValues);
                _autoCompleteUomSourceDictionary.Add(currentAttribute, uniqueUoms);
            }

            FrmWaitScreen.HideMessage(waitKey);
        }

        private void field1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideHeaders(_field1Header, field1ToolStripMenuItem.Checked, Headers.Sku, 3);
        }

        private void field2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideHeaders(_field2Header, field2ToolStripMenuItem.Checked, Headers.Sku, 4);
        }

        private void field3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideHeaders(_field3Header, field3ToolStripMenuItem.Checked, Headers.Sku, 5);
        }

        private void field4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideHeaders(_field4Header, field4ToolStripMenuItem.Checked, Headers.Sku, 6);
        }

        private void field5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHideHeaders(_field5Header, field5ToolStripMenuItem.Checked, Headers.Sku, 7);
        }

        private Attribute GetAttribute(int rowIndex, out int multiValueIndex)
        {
            multiValueIndex = 0;

            if (rowIndex < 2)
                return null;

            if (rowIndex - 2 >= _rows.Count)
                return null;

            var attributeCellValue = _rows[rowIndex - 2].Split(MultiValueSuffix.Trim().ToCharArray());
            var attributeName = attributeCellValue[0];
            if (attributeCellValue.Count() > 1)
                Int32.TryParse(attributeCellValue[1], out multiValueIndex);

            return Attribute.GetAttributeFromName(attributeName, false);
        }

        private EntityData GetEntityData(int columnIndex, int rowIndex)
        {
            var relativeColumn = columnIndex - _attributeHeaders.Count;

            if (relativeColumn <= 0)
                return null;

            int entityDataColumnIndex;
            if (relativeColumn % _itemHeaders.Count == 0)
            {
                entityDataColumnIndex = columnIndex - _itemHeaders.Count + 1;
                return bdgv[entityDataColumnIndex, rowIndex].Value as EntityData;
            }
            entityDataColumnIndex = columnIndex - (relativeColumn % _itemHeaders.Count) + 1;

            return bdgv[entityDataColumnIndex, rowIndex].Value as EntityData;
        }

        private object GetItemHeaders(int columnIndex)
        {
            if (columnIndex == 0 || columnIndex <= _attributeHeaders.Count)
                return null;

            var headerIndex = (columnIndex - _attributeHeaders.Count) % _itemHeaders.Count;

            if (headerIndex == 0)
                return _itemHeaders[_itemHeaders.Count - 1];

            return _itemHeaders[headerIndex - 1];
        }

        private List<string> GetRows(AttributeFilterEnum filter)
        {
            _currentAttributes.Clear();
            var leafNodes = new List<TaxonomyInfo> { _currentTaxonomy };

            _currentAttributes.AddRange(SelectedGlobalAttributes);

            var taxAtts =
                leafNodes.SelectMany(si => si.SchemaInfos)
                    .SelectMany(sd => sd.SchemaDatas.Where(a => a.Active))
                    .Select(
                        s =>
                            new
                            {
                                schemaInfo = s.SchemaInfo,
                                taxonomyInfo = s.SchemaInfo.TaxonomyInfo,
                                attribute = s.SchemaInfo.Attribute,
                                navigationOrder = s.NavigationOrder,
                                displayOrder = s.DisplayOrder,
                                inSchema = s.InSchema
                            })
                    .ToList();

            switch (filter)
            {
                case AttributeFilterEnum.Navigation:
                    _currentAttributes.AddRange(
                        taxAtts.Where(ta => ta.navigationOrder > 0).Select(a => a.attribute).Distinct().ToList());
                    break;
                case AttributeFilterEnum.Display:
                    _currentAttributes.AddRange(
                        taxAtts.Where(ta => ta.displayOrder > 0).Select(a => a.attribute).Distinct().ToList());
                    break;

                case AttributeFilterEnum.Inschema:
                    _currentAttributes.AddRange(
                        taxAtts.Where(ta => ta.inSchema).Select(a => a.attribute).Distinct().ToList());
                    break;

                case AttributeFilterEnum.Extended:
                    _currentAttributes.AddRange(
                        taxAtts.Where(sd => sd.navigationOrder == 0 && sd.displayOrder == 0)
                            .Select(a => a.attribute)
                            .Distinct()
                            .ToList());
                    break;

                case AttributeFilterEnum.All:
                    _currentAttributes.AddRange(taxAtts.Select(a => a.attribute).Distinct().ToList());
                    break;
            }

            return GetRows(_currentAttributes);
        }

        private List<string> GetRows(IEnumerable<Attribute> atts)
        {
            var rows = new List<string>();
            _attRows = new Dictionary<Attribute, int>();

            foreach (var att in atts.Distinct())
            {
                var rowCount =
                    _currentSkus.Select(sku => sku.GetValuesForAttribute(att).Count()).Concat(new[] { 1 }).Max();
                _attRows.Add(att, rowCount);
            }

            foreach (var attRow in _attRows)
            {
                if (attRow.Value == 1)
                    rows.Add(attRow.Key.ToString());

                if (attRow.Value > 1)
                {
                    for (var i = 0; i < attRow.Value; i++)
                        rows.Add(attRow.Key + (i > 0 ? (MultiValueSuffix + i) : string.Empty));
                }
            }
            return rows;
        }

        private List<EntityData> GetSelectedEntityDatas(bool isValueCell)
        {
            var selectedEntityDatas = new List<EntityData>();
            for (var i = 0; i < bdgv.SelectedCells.Count; i++)
            {
                var cell = bdgv.SelectedCells[i];
                var ed = GetEntityData(cell.ColumnIndex, cell.RowIndex);
                if (ed == null && isValueCell)
                {
                    var sku = GetSku(cell.ColumnIndex);
                    if (sku == null)
                        continue;

                    int multiValueIndex;
                    var attribute = GetAttribute(cell.RowIndex, out multiValueIndex);
                    if (attribute == null)
                        continue;

                    var newEntityData = new EntityData
                                        {
                                            Attribute = attribute,
                                            EntityInfo = new EntityInfo { Sku = sku },
                                            Active = false
                                        };
                    selectedEntityDatas.Add(newEntityData);
                    if (_attRows[attribute] > 0)
                        _refreshGridView = true;
                }
                else if (!selectedEntityDatas.Contains(ed) && ed != null)
                {
                    selectedEntityDatas.Add(ed);
                    if (_attRows.Keys.Contains(ed.Attribute) && _attRows[ed.Attribute] > 0)
                        _refreshGridView = true;
                }
            }
            return selectedEntityDatas;
        }

        private List<Sku> GetSelectedSkus()
        {
            var selectedSkus = new List<Sku>();
            for (var i = 0; i < bdgv.SelectedCells.Count; i++)
            {
                var cell = bdgv.SelectedCells[i];
                var sku = GetSku(cell.ColumnIndex);
                if (!selectedSkus.Contains(sku) && sku != null)
                    selectedSkus.Add(sku);
            }
            return selectedSkus;
        }

        private Sku GetSku(int columnIndex)
        {
            var relativeColumn = columnIndex - _attributeHeaders.Count;

            if (relativeColumn > 0)
            {
                if (relativeColumn % _itemHeaders.Count == 0)
                    return (Sku)_columns[(relativeColumn / _itemHeaders.Count) - 1];
                return (Sku)_columns[relativeColumn / _itemHeaders.Count];
            }
            return null;
        }

        private int GetSkuIndex(int columnIndex)
        {
            var relativeColumn = columnIndex - _attributeHeaders.Count;

            if (relativeColumn > 0)
            {
                if (relativeColumn % _itemHeaders.Count == 0)
                    return (relativeColumn / _itemHeaders.Count) - 1;
                return (relativeColumn / _itemHeaders.Count);
            }
            return 0;
        }

        private object GetValue(Sku sku, Attribute attribute, int multiValueIndex, DataGridViewCell itemHeader,
            string attributeHeader)
        {
            if (sku != null && attribute != null && itemHeader.Value != null && _itemHeaders.Contains(itemHeader.Value))
            //Attribute Values
            {
                var eds = sku.GetValuesForAttribute(attribute);

                SetAutoFillSource(attribute, itemHeader);

                if (eds.Count == 0 || eds.Count - 1 < multiValueIndex)
                    return null;

                var header = itemHeader.Value.ToString();
                return header == ValueHeader
                    ? (object)eds[multiValueIndex]
                    : header == UomHeader
                        ? eds[multiValueIndex].Uom
                        : header == _field1Header
                            ? eds[multiValueIndex].Field1
                            : header == _field2Header
                                ? eds[multiValueIndex].Field2
                                : header == _field3Header
                                    ? eds[multiValueIndex].Field3
                                    : header == _field4Header
                                        ? eds[multiValueIndex].Field4
                                        : header == _field5Header ? eds[multiValueIndex].Field5 : null;
            }

            if (attributeHeader == null || attribute == null)
                return attribute;

            var metaAttribute = _metaAttributes.FirstOrDefault(a => a.ToString() == attributeHeader);

            if (metaAttribute != null)
                return SchemaAttribute.GetValue(_currentTaxonomy, attribute, metaAttribute);

            var sd = (from si in _currentTaxonomy.SchemaInfos
                      where si.AttributeID == attribute.ID
                      from scd in si.SchemaDatas
                      where scd.Active
                      select scd).FirstOrDefault();
            if (sd == null)
                return null;

            //if (attributeHeader == NavigationRankHeader)
            //{
            //    var navOrder = sd.NavigationOrder.ToString().Split('.')[0];
            //    return navOrder == "0" ? string.Empty : navOrder;
            //}

            //if (attributeHeader == DisplayRankHeader)
            //{
            //    var disOrder = sd.DisplayOrder.ToString().Split('.')[0];
            //    return disOrder == "0" ? string.Empty : disOrder;
            //}

            //if (attributeHeader == InSchemaHeader)
            //    return sd.InSchema ? "Yes" : "No";

            //if (attributeHeader == DataTypeHeader)
            //    return sd.DataType;

            return attribute;
        }

        private void InitControlStates()
        {
            var currentProject = AryaTools.Instance.InstanceData.CurrentProject;
            _field1Header = currentProject.EntityField1Name;
            _field2Header = currentProject.EntityField2Name;
            _field3Header = currentProject.EntityField3Name;
            _field4Header = currentProject.EntityField4Name;
            _field5Header = currentProject.EntityField5Name;

            if (_field1Header != null)
            {
                field1ToolStripMenuItem.Visible = true;
                field1ToolStripMenuItem.Text = _field1Header;
            }

            if (_field2Header != null)
            {
                field2ToolStripMenuItem.Visible = true;
                field2ToolStripMenuItem.Text = _field2Header;
            }

            if (_field3Header != null)
            {
                field3ToolStripMenuItem.Visible = true;
                field3ToolStripMenuItem.Text = _field3Header;
            }

            if (_field4Header != null)
            {
                field4ToolStripMenuItem.Visible = true;
                field4ToolStripMenuItem.Text = _field4Header;
            }

            if (_field5Header != null)
            {
                field5ToolStripMenuItem.Visible = true;
                field5ToolStripMenuItem.Text = _field5Header;
            }
        }

        private void InitData()
        {
            //default false
            AutoOrderRanks = false;
            var waitkey = FrmWaitScreen.ShowMessage("Loading Attributes");

            AryaTools.Instance.InstanceData.Dc.Attributes.Where(
                a => a.AttributeType == AttributeTypeEnum.Global.ToString())
                .OrderBy(an => an.AttributeName)
                .ForEach(_globalAttributes.Add);

            if (_autoCompleteAttributeNameSource == null)
            {
                _autoCompleteAttributeNameSource = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                                                    where
                                                        att.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject)
                                                        && Framework.Data.AryaDb.Attribute.NonMetaAttributeTypes.Contains(att.AttributeType)
                                                    orderby att.AttributeName
                                                    select att.AttributeName).ToList();
            }

            FrmWaitScreen.UpdateMessage(waitkey, "Fetching Meta Attributes");
            //var taxId = _currentTaxonomy.ID;
            //var attIds = (_attributes ?? new List<Attribute>()).Select(att => att.ID);

            var schemaInfos = _currentTaxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).ToList();

            _allAttributes =
                schemaInfos.Select(si => si.Attribute)
                    .Distinct(new KeyEqualityComparer<Attribute>(att => att.ID))
                    .OrderBy(att => att.AttributeName)
                    .ToList();
            //_allTaxonomies =
            //    schemaInfos.Select(si => si.TaxonomyInfo)
            //        .Distinct(new KeyEqualityComparer<TaxonomyInfo>(tax => tax.ID))
            //        .OrderBy(tax => tax.ToString())
            //        .Cast<object>()
            //        .ToList();

            InitRowsAndColumns();

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void InitMetaAttributes()
        {
            SchemaAttribute.PrimarySchemati.Union(SchemaAttribute.SecondarySchemati).ForEach(_metaAttributes.Add);
        }

        private void InitRowsAndColumns()
        {
            var waitkey = FrmWaitScreen.ShowMessage("Initializing Grid View");

            _rows = GetRows(_currentAttributeFilter);
            _columns = _currentSkus.Cast<Object>().ToList();

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void PrepareChange(bool isValueCell)
        {
            _selectedEntityDatas = GetSelectedEntityDatas(isValueCell);
            _selectedSkus = GetSelectedSkus();
            _currentChange = new Change(_selectedEntityDatas.Count, _selectedSkus.Count);
            _selectedEntityDatas.ForEach(ed => _currentChange.Add(ed));
        }

        //private void ToggleCellColor()
        //{
        //    if (_columnColor == Color.LightGray)
        //        _columnColor = Color.Thistle;
        //    else
        //        _columnColor = Color.LightGray;
        //}
        private void PrepareGridView()
        {
            var waitkey = FrmWaitScreen.ShowMessage("Drawing Grid View");

            bdgv.Rows.Clear();
            bdgv.Columns.Clear();
            SelectedAttribute = null;
            SelectedSku = null;

            ResetRowColumnCount();
            ApplyStyles();

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void ResetRowColumnCount()
        {
            bdgv.RowCount = _rows.Count() + 2 + (_rows.Count == 0 ? 0 : 1);

            while (bdgv.ColumnCount < _attributeHeaders.Count)
                bdgv.Columns.Add(new DataGridViewColumn { FillWeight = 1, CellTemplate = new DataGridViewTextBoxCell() });

            while (bdgv.ColumnCount - _attributeHeaders.Count < _columns.Count() + 1)
            {
                for (var i = 1; i <= _itemHeaders.Count; i++)
                {
                    bdgv.Columns.Add(new DataGridViewColumn
                                     {
                                         FillWeight = 1,
                                         CellTemplate =
                                             new DataGridViewTextBoxCell { ReadOnly = false }
                                     });
                }
            }

            bdgv.ColumnCount = _attributeHeaders.Count + (_columns.Count() * _itemHeaders.Count) + 1;
            bdgv.Invalidate();
        }

        private void saveChangesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            AryaTools.Instance.SaveChangesIfNecessary(false, true);
        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.SaveChangesIfNecessary(true, true);
            //AryaTools.Instance.Forms.BuildForm.SetTabStatus(false);
        }

        private void SetAutoFillSource(Attribute attribute, DataGridViewCell itemHeader)
        {
            if (itemHeader.Value.ToString() == ValueHeader)
                _autoCompleteValueSourceDictionary.TryGetValue(attribute, out _autoFillSource);
            if (itemHeader.Value.ToString() == UomHeader)
                _autoCompleteUomSourceDictionary.TryGetValue(attribute, out _autoFillSource);
        }

        private void SetCellBorders(DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value is EntityData && e.Value.ToString().Trim() == String.Empty)
            {
                using (var p = new Pen(Color.Red, 4))
                {
                    var rect = e.CellBounds;
                    rect.Width -= 2;
                    rect.Height -= 2;
                    e.Graphics.DrawRectangle(p, rect);
                }
                e.Handled = true;
            }

            if (e.RowIndex > 1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }

            if (e.RowIndex == 0 && e.Value == null)
            {
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            }
            else if (e.RowIndex == 0 && e.Value != null)
            {
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            }

            else if ((e.ColumnIndex - _attributeHeaders.Count) % _itemHeaders.Count == 0)
            {
                e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            }
            else
            {
                e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
            }

            if (e.ColumnIndex - _attributeHeaders.Count > 0 && bdgv[e.ColumnIndex, 1].Value != null
                && bdgv[e.ColumnIndex, 1].Value.ToString() == "UoM")
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;

            if (e.ColumnIndex - _attributeHeaders.Count > 0 && bdgv[e.ColumnIndex, 1].Value != null
                && bdgv[e.ColumnIndex, 1].Value.ToString() == "Value" && e.RowIndex != 1)
                e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;

            if (e.Value != null && e.RowIndex == 1 && e.Value.ToString() == "UoM")
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
        }

        private void ShowHideHeaders(string headerName, bool show, Headers typeOfHeader,
            int defaultPosition = int.MaxValue)
        {
            var headerList = new List<string>();
            switch (typeOfHeader)
            {
                case Headers.Attribute:
                    headerList = _attributeHeaders;
                    break;
                case Headers.Sku:
                    headerList = _itemHeaders;
                    break;
            }

            if (show)
            {
                if (headerList.Contains(headerName))
                    return;

                var count = headerList.Count;
                if (count > defaultPosition - 1)
                    headerList.Insert(defaultPosition - 1, headerName);
                else
                    headerList.Add(headerName);
            }
            else
            {
                if (!headerList.Contains(headerName))
                    return;

                headerList.Remove(headerName);
            }

            PrepareGridView();
        }

        private void SortBy(AttributeListSort.Field fieldName, Attribute metaAttribute = null)
        {
            _currentAttributes = _currentAttributes.Sort(fieldName, _currrentOrder, _currentTaxonomy, metaAttribute);
            ToggleCurrentOrder();
            _rows = GetRows(_currentAttributes);
            PrepareGridView();
        }

        private void ToggleCurrentOrder()
        {
            _currrentOrder = _currrentOrder == AttributeListSort.Order.Ascending
                ? AttributeListSort.Order.Descending
                : AttributeListSort.Order.Ascending;
        }

        private void UpdateGlobalAttributes()
        {
            if (SelectedSku == null)
            {
                //dgvGlobals.DataSource = null;
                return;
            }

            var globals = from att in _globalAttributes
                          let values = SelectedSku.GetValuesForAttribute(att)
                          where values.Count > 0
                          select
                              new AttributeValue
                              {
                                  Show = SelectedGlobalAttributes.Contains(att),
                                  AttributeName = att.AttributeName,
                                  Value =
                                      values.Select(
                                          ed =>
                                      ed.Value + (string.IsNullOrWhiteSpace(ed.Uom) ? string.Empty : " " + ed.Uom))
                                      .Aggregate((current, next) => current + ", " + next)
                              };

            var dataSource = globals.ToList();
            dgvGlobals.DataSource = dataSource;
        }

        private void UpdateLovs()
        {
            if (SelectedAttribute == null)
            {
                //dgvListOfValues.DataSource = null;
                return;
            }

            var lovs = from si in _currentTaxonomy.SchemaInfos
                       where si.Attribute == SelectedAttribute
                       from lov in si.ListOfValues
                       where lov.Active
                       select new { lov.Value };

            var dataSource = lovs.ToList();
            dgvListOfValues.DataSource = dataSource;
        }

        private void UpdateMetaAttributes()
        {
            if (SelectedAttribute == null)
            {
                //dgvMetaAttributes.DataSource = null;
                return;
            }

            var metas = from sa in _metaAttributes
                        let value = SchemaAttribute.GetValue(_currentTaxonomy, SelectedAttribute, sa)
                        let isLov =
                            sa.AttributeType == SchemaAttribute.SchemaAttributeType.Special
                            && sa.SpecialSchemaAttribute == SchemaAttribute.SpecialSchematusListOfValues
                        where value != null && !isLov
                        select
                            new AttributeValue
                            {
                                Show = _attributeHeaders.Contains(sa.ToString()),
                                AttributeName = sa.ToString(),
                                Value = value.ToString()
                            };

            var dataSource = metas.ToList();
            dgvMetaAttributes.DataSource = dataSource;
        }

        private void UpdateSkus(object sender, SkusAddedEventHandlerArgs args)
        {
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            _currentSkus.AddRange(args.NewSkus);
            InitData();
            PrepareGridView();
        }

        private void validateToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (validateToolStripMenuItem.Checked)
            {
                _validateGrid = true;
                bdgv.Invalidate();
            }
            else
                _validateGrid = false;
        }

        #endregion Methods

        #region Nested Types

        private class AttributeValue
        {
            #region Properties

            public string AttributeName { get; set; }

            public bool Show { get; set; }

            public string Value { get; set; }

            #endregion Properties
        }

        #endregion Nested Types
    }
}