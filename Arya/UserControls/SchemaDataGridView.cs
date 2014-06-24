using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataGridViewAutoFilter;
using LinqKit;
using Arya.Data;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework.Properties;
using Arya.Framework4.State;
using Arya.HelperClasses;
using Arya.HelperForms;
using Attribute = Arya.Data.Attribute;
using AttributeTypeEnum = Arya.Framework.Data.AryaDb.AttributeTypeEnum;
using AryaDbDataContext = Arya.Framework.Data.AryaDb.AryaDbDataContext;
using Arya.SpellCheck;

namespace Arya.UserControls
{
    
    public partial class SchemaDataGridView : UserControl
    {
        #region Constants

        private const int MaxLineLengthForSingleLine = 30; // used to switch between multiline and single line
        private const string EmptyMeta = "—";

        #endregion

        #region Fields

        private static AutoCompleteStringCollection _autoCompleteAttributeName;
        private static AutoCompleteStringCollection _autoCompleteDataType;

        private static readonly Dictionary<Attribute, AutoCompleteStringCollection> AutoCompleteCache =
            new Dictionary<Attribute, AutoCompleteStringCollection>();

        public static bool RefreshGridView;
        public readonly List<Attribute> CurrentAttributes;
        public readonly List<TaxonomyInfo> CurrentTaxonomies;
        private readonly Attribute _blankAttribute = new Attribute { AttributeName = "*" };

        private readonly List<string> _primarySchemaAttributes = new List<string>
                                                                 {
                                                                     "In Schema",
                                                                     "Is Mapped",
                                                                     "Attribute Type",
                                                                     "Data Type",
                                                                     "Navigation Order",
                                                                     "Display Order",
                                                                     "List of Values"
                                                                 };

        private List<object> _allAttributes;
        private List<SchemaAttribute> _allSchemati;
        private List<object> _allTaxonomies;
        private List<object> _columns;
        private List<TaxonomyInfo> _currentFilteredTaxonomies;
        private SchemaAttribute _currentSchematus;
        private SchemaAttribute _enrichmentImageSchematus;
        private object _lastSortedAscendingBy;
        private List<object> _rows;
        private TaxonomyInfo _taxonomyFilter;
        private bool? _unmapAttributesWithSkus;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// This constructor get called when a single attribute is selected in 
        /// an already opened schema view
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="taxonomyFilter"></param>
        public SchemaDataGridView(List<Attribute> attributes, TaxonomyInfo taxonomyFilter)
            : this()
        {
            CurrentAttributes = attributes;
            TaxonomyFilter = taxonomyFilter;
            InitData();
            PrepareGridView();
        }

        /// <summary>
        /// This constructor get called when schema view opens for one or more 
        /// taxonomy nodes
        /// </summary>
        /// <param name="taxonomies"></param>
        public SchemaDataGridView(List<TaxonomyInfo> taxonomies)
            : this()
        {
            CurrentTaxonomies = taxonomies;
            TaxonomyFilter = null;
            InitData();
            PrepareGridView();
            if (AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.AutoOrderRanks)
            {
                autoorderRanksToolStripMenuItem.Checked = true;
                AutoOrderRanks = autoorderRanksToolStripMenuItem.Checked;
                TryOrderRanks();
            }
            var column =
                GetColumnToSort(AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.AttributeOrderBy);
            SortDataGridColumnItems(column);
        }

        private SchemaDataGridView()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            ShortcutKeysEnabled = false;
        }

        #endregion Constructors

        #region Properties

        public static bool AutoOrderRanks { get; set; }

        public CheckState CurrentAutoSuggestOption
        {
            get { return cbAutoSuggestOptions.CheckState; }
            set { cbAutoSuggestOptions.CheckState = value; }
        }

        private SchemaAttribute CurrentSchematus
        {
            get { return _currentSchematus; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                var previousValue = _currentSchematus;
                _currentSchematus = value;

                if (previousValue != null && previousValue.Equals(value))
                    return;

                ddCurrentSchematus.SelectedItem = value;
                sdgv.DefaultCellStyle = GetDefaultCellStyle(value);
                sdgv.Invalidate();
            }
        }

        public bool ShortcutKeysEnabled
        {
            set { Forms.EnableDisableMenuItems(mainMenuStrip.Items, value); }
        }

        private bool ShowFillRate
        {
            get { return showFillRatesToolStripMenuItem.Checked; }
        }

        public TaxonomyInfo TaxonomyFilter
        {
            get { return _taxonomyFilter; }
            set
            {
                _taxonomyFilter = value;

                if (value == null)
                    return;

                string l1, l2, l3, l4;
                var taxString = _taxonomyFilter.ToString();
                // TaxonomyInfo.SplitTaxonomyString(taxString, out l1, out l2, out l3);
                List<string> taxLevels = TaxonomyInfo.GetTaxonomyLevels(taxString, 4);
                GreatGreatGrandparentNodeToolStripMenuItem.Text = taxLevels[0];
                GreatGrandparentNodeToolStripMenuItem.Text = taxLevels[1];
                GrandparentNodeToolStripMenuItem.Text = taxLevels[2];
                ParentNodeToolStripMenuItem.Text = taxLevels[3];
            }
        }

        #endregion Properties

        #region Event Handlers

        private void cbAutoSuggestOptions_CheckStateChanged(object sender, EventArgs e)
        {
            switch (cbAutoSuggestOptions.CheckState)
            {
                case CheckState.Checked:
                    cbAutoSuggestOptions.Text = "Always Multiline";
                    break;
                case CheckState.Indeterminate:
                    cbAutoSuggestOptions.Text = "Auto Multiline/Suggest";
                    break;
                case CheckState.Unchecked:
                    cbAutoSuggestOptions.Text = "Always Suggest";
                    break;
            }
        }

        private void ddCurrentSchematus_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentSchematus = (SchemaAttribute)ddCurrentSchematus.SelectedItem;
        }

        private void RefreshGridViewTimer_Tick(object sender, EventArgs e)
        {
            if (AryaTools.Instance.AllFillRates.Working)
                return;

            RefreshGridView = false;
            sdgv.Invalidate();
            RefreshGridViewTimer.Stop();
        }

        private void SchemaDataGridView_Load(object sender, EventArgs e)
        {
            //PrepareGridView();
            //object column = GetColumnToSort(AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.AttributeOrderBy);
        }

        private void sdgvAuditTrail_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
                if (e.RowIndex % 2 != 0)
                    e.CellStyle.BackColor = DisplayStyle.CellStyleOddRow.BackColor;
            }
        }

        private void sdgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                e.Cancel = true;
                return;
            }

            var rowItem = GetRowItem(e.RowIndex);
            var colItem = GetColumnItem(e.ColumnIndex);

            TaxonomyInfo taxonomy;
            Attribute attribute;
            SchemaAttribute schematus;
            GetContext(colItem, rowItem, out taxonomy, out attribute, out schematus);

            if (schematus == null)
                return;

            if (attribute == null || schematus.AttributeType == SchemaAttribute.SchemaAttributeType.Special)
            {
                if (schematus.ToString() == SchemaAttribute.SpecialSchematusListOfValues)
                {
                    InitLov();
                    AryaTools.Instance.Forms.ListofValuesForm.ShowPopulateButton = true;
                    AryaTools.Instance.Forms.ListofValuesForm.ShowDialog();
                }
                e.Cancel = true;
                return;
            }
            if (schematus.AttributeType == SchemaAttribute.SchemaAttributeType.Meta)
            {
                if (schematus != _enrichmentImageSchematus)
                    return;

                using (
                    var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                        AryaTools.Instance.InstanceData.CurrentUser.ID))
                {
                    var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID);
                    if (imageMgr.UploadImage())
                    {
                        var newSku = imageMgr.ImageSku;

                        imageMgr.AddAttributeValue("AttributeId", attribute.ID.ToString());
                        imageMgr.AddAttributeValue("TaxonomyId", taxonomy.ID.ToString());

                        //newSku.EntityInfos.Add(new EntityInfo
                        //                       {
                        //                           EntityDatas = { new EntityData { Attribute = Attribute.GetAttributeFromName("AttributeId", true), Value = attribute.ID.ToString() } }
                        //                       });

                        //newSku.EntityInfos.Add(new EntityInfo
                        //                       {
                        //                           EntityDatas = { new EntityData { Attribute = Attribute.GetAttributeFromName("TaxonomyId", true), Value = taxonomy.ID.ToString() } }
                        //                       });

                        taxonomy.SkuInfos.Add(new SkuInfo { SkuID = newSku.ID });

                        AryaTools.Instance.SaveChangesIfNecessary(false, false);

                        sdgv[e.ColumnIndex, e.RowIndex].Value = imageMgr.RemoteImageGuid;

                        if (AryaTools.Instance.Forms.TreeForm.ShowEnrichments)
                            AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                    }
                    e.Cancel = true;
                }
            }
        }

        private void sdgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!AryaTools.Instance.Forms.TreeForm.ShowEnrichments)
                return;

            TaxonomyInfo taxonomy;
            Attribute attribute;
            SchemaAttribute schematus;
            GetContext(GetColumnItem(e.ColumnIndex), GetRowItem(e.RowIndex), out taxonomy, out attribute, out schematus);

            if (schematus == null)
                return;

            var image = SchemaAttribute.GetValue(taxonomy, attribute, _enrichmentImageSchematus);

            if (image != null && taxonomy != null)
            {
                var imageSku = taxonomy.Skus.Where(s => s.ItemID == image.ToString()).FirstOrDefault();
                if (imageSku != null)
                {
                    using (
                   var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                       AryaTools.Instance.InstanceData.CurrentUser.ID))
                    {
                        var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                        {
                            ImageSku
                                =
                                dc.Skus.Where(sku => sku.ID == imageSku.ID).First()
                        };

                        AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                    }
                }

            }

            else
            {
                using (
                    var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                        AryaTools.Instance.InstanceData.CurrentUser.ID))
                {
                    var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID);

                    AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                }
            }
        }

        private void sdgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
                return;
            if (CurrentAttributes != null && e.Value.ToString().Length > MaxLineLengthForSingleLine
                && e.ColumnIndex != 0)
                e.CellStyle.WrapMode = DataGridViewTriState.True; //setting cell to show content in  multiline 
            if (e.Value is TaxonomyInfo)
            {
                e.Value = ((TaxonomyInfo)e.Value).NodeName;
                e.FormattingApplied = true;
            }
            else if (e.CellStyle.FormatProvider is BoolFormatter)
            {
                e.Value =
                    ((ICustomFormatter)e.CellStyle.FormatProvider.GetFormat(typeof(ICustomFormatter))).Format(
                        e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }
            else if (e.CellStyle.FormatProvider is LovFormatter)
            {
                e.Value =
                    ((ICustomFormatter)e.CellStyle.FormatProvider.GetFormat(typeof(ICustomFormatter))).Format(
                        e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }
        }

        private void sdgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > 0 && e.ColumnIndex > -1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }
            if (sdgv.CurrentRow != null && sdgv.CurrentRow.Index == e.RowIndex)
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.OutsetDouble;

            // figure out if I'm sitting on a value for a meta-attribute
            if (e.RowIndex > 0 && e.ColumnIndex > 0)
            {
                TaxonomyInfo taxonomy;
                Attribute attribute;
                SchemaAttribute schematus;
                GetContext(GetColumnItem(e.ColumnIndex), GetRowItem(e.RowIndex), out taxonomy, out attribute,
                    out schematus);

                if (schematus.AttributeType == SchemaAttribute.SchemaAttributeType.Meta
                    && !ValidateMetaAttribute(e.Value, schematus))
                    e.CellStyle.ForeColor = Color.Crimson;
            }
        }

        private void sdgv_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            try
            {
                var gridValue = sdgv[e.ColumnIndex, e.RowIndex].Value;
                if (gridValue is TaxonomyInfo)
                {
                    // display full taxonomy string
                    e.ToolTipText = gridValue.ToString()
                        .Replace(TaxonomyInfo.Delimiter, TaxonomyInfo.Delimiter + Environment.NewLine);
                }
                else if (gridValue is SchemaAttribute)
                {
                    // display meta-Meta-attributes for this meta-attribute
                    var meta = (SchemaAttribute)gridValue;
                    if (meta.AttributeType == SchemaAttribute.SchemaAttributeType.Meta)
                        e.ToolTipText = GetMetaToolTip(meta);
                }
            }
            catch (Exception)
            {
            }
        }

        private void sdgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var rowItem = GetRowItem(e.RowIndex);
            var colItem = GetColumnItem(e.ColumnIndex);
            e.Value = GetValue(colItem, rowItem);
            if (colItem is SchemaAttribute)
            {
                var width = GetColumnWidth(colItem as SchemaAttribute);
                if (width != 0)
                    sdgv.Columns[e.ColumnIndex].Width = width;
            }
            if (RefreshGridView)
                RefreshGridViewTimer.Start();
        }

        private Dictionary<string, int> _columnWidths;

        private void TryFetchColumnWidths()
        {
            if (_columnWidths != null)
                return;

            int width;
            _columnWidths = (from columnWidth in
                                 AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.SchemaColumnWidths
                             let parts = columnWidth.Split(new[] { '|' }, 2)
                             where parts.Length == 2
                             group parts by parts[1]
                                 into grp
                                 select new { grp.Key, Value = int.TryParse(grp.First()[0], out width) ? width : 0 }).ToDictionary(
                    kvp => kvp.Key, kvp => kvp.Value);
        }

        private void TrySaveColumnWidths()
        {
            if (_columnWidths == null)
                return;

            var columnWidths = _columnWidths.OrderBy(cw => cw.Key).Select(cw => cw.Value + "|" + cw.Key).ToArray();
            var prefs = AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences;
            prefs.SchemaColumnWidths = columnWidths;
            AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences = prefs;
        }

        private void SetColumnWidth(SchemaAttribute attribute, int width)
        {
            TryFetchColumnWidths();
            _columnWidths[attribute.ToString()] = width;
            TrySaveColumnWidths();
        }

        private int GetColumnWidth(SchemaAttribute attribute)
        {
            TryFetchColumnWidths();
            var key = attribute.ToString();
            if (_columnWidths.ContainsKey(key))
                return _columnWidths[key];

            return 0;
        }

        private void sdgv_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            string result;
            var columnIndex = e.ColumnIndex;
            var rowIndex = e.RowIndex;
            var value = e.Value;
            if ((result = SetValue(columnIndex, rowIndex, value)) == null)
                AryaTools.Instance.SaveChangesIfNecessary(false, false);
            else
                MessageBox.Show(result);
            sdgv.InvalidateCell(columnIndex, rowIndex);
            AutoResizeColumns(columnIndex);
        }

        private void sdgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            sdgv.EndEdit();

            var colItem = sdgv[e.ColumnIndex, 0].Value;
            var sortAscending = true;
            if (_lastSortedAscendingBy != null && _lastSortedAscendingBy.Equals(colItem))
            {
                sortAscending = false;
                _lastSortedAscendingBy = null;
            }
            else
                _lastSortedAscendingBy = colItem;
            var rows = (from row in _rows
                        let currentIndex = _rows.IndexOf(row)
                        let item = GetValue(colItem, row)
                        let isBlank = IsBlankValue(item)
                        select new { row, currentIndex, item, isBlank }).ToList();
            try
            {
                if (sortAscending)
                {
                    _rows =
                        rows.OrderBy(i => i.isBlank)
                            .ThenBy(i => i.item)
                            .ThenBy(i => i.currentIndex)
                            .Select(i => i.row)
                            .ToList();
                }
                else
                {
                    _rows =
                        rows.OrderBy(i => i.isBlank)
                            .ThenByDescending(i => i.item)
                            .ThenBy(i => i.currentIndex)
                            .Select(i => i.row)
                            .ToList();
                }

                sdgv.Invalidate();
            }
            catch (Exception)
            {
            }
            //var columnToSort = sdgv[e.ColumnIndex, 0].Value;
            //if (columnToSort != null)// Header clicked is not the first cell in the first row.
            //{
            //    if (SchematiPrimary.Any(ap => ap.PrimarySchemaAttribute == columnToSort.ToString()) || columnToSort.ToString().Equals("FillRate"))
            //    {
            //        SortDataGridColumnItems(sdgv[e.ColumnIndex, 0].Value);
            //    }
            //    else if (_currentFilteredTaxonomies != null && _currentFilteredTaxonomies.Count > 1)
            //    {
            //        SortDataGridColumnItems(_currentSchematus);

            //    }
            //}
            //else// Sorting on attribute name
            //{
            //    SortDataGridColumnItems(columnToSort);

            //}

            //SortDataGridColumnItems(null);
        }

        private void sdgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //autoSuggest
            //Always Multi Line -> Checked
            //Always Suggest -> Uncheked
            //Auto Multiline/Suggest -> Indeterminate

            var textBox = (TextBox)e.Control;
            var columnIndex = sdgv.CurrentCell.ColumnIndex;
            if (sdgv.CurrentCell.Value is Attribute)
                textBox.AutoCompleteCustomSource = _autoCompleteAttributeName;
            else
            {
                var rowIndex = sdgv.CurrentCell.RowIndex;
                var rowItem = GetRowItem(rowIndex);
                var colItem = GetColumnItem(columnIndex);

                TaxonomyInfo taxonomy;
                Attribute attribute;
                SchemaAttribute schematus;
                GetContext(colItem, rowItem, out taxonomy, out attribute, out schematus);

                if (schematus.AttributeType.Equals(SchemaAttribute.SchemaAttributeType.Primary)
                    && schematus.PrimarySchemaAttribute.Equals("Data Type"))
                    textBox.AutoCompleteCustomSource = _autoCompleteDataType;
                else if (schematus.AttributeType.Equals(SchemaAttribute.SchemaAttributeType.Meta)
                         && cbAutoSuggestOptions.CheckState != CheckState.Checked)
                {
                    textBox.AutoCompleteCustomSource = GetAutoCompleteSource(schematus.MetaSchemaAttribute,
                        cbAutoSuggestOptions.CheckState);
                }
                else
                    textBox.AutoCompleteCustomSource = null;
            }

            //textBox.Multiline = textBox.AutoCompleteCustomSource == null || textBox.AutoCompleteCustomSource.Count == 0;
            textBox.Multiline = cbAutoSuggestOptions.CheckState == CheckState.Checked
                                || textBox.AutoCompleteCustomSource == null
                                || textBox.AutoCompleteCustomSource.Count == 0;
            textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void sdgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                _unmapAttributesWithSkus = null;
                foreach (DataGridViewCell cell in sdgv.SelectedCells)
                {
                    // Check to see what kind of cell we're on
                    Attribute attribute;
                    TaxonomyInfo taxonomy;
                    SchemaAttribute schematus;
                    GetContext(GetColumnItem(cell.ColumnIndex), GetRowItem(cell.RowIndex), out taxonomy, out attribute,
                        out schematus);

                    // see if we're on an LOV.
                    if (schematus != null && schematus.SpecialSchemaAttribute == SchemaAttribute.SchemaAttributeListOfValues.ToString())
                    {
                        var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
                        if (schemaInfo != null && schemaInfo.ActiveListOfValues.Any())
                        {
                            // if so, confirm that the LOV should be deleted
                            if (
                                MessageBox.Show(
                                    "Do you really want to permanently delete all LOV values for this attribute?\n\nWarning: This cannot be undone.",
                                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                // get the current list of values, assign an empty List to it and save
                                schemaInfo.ActiveListOfValues = new List<string>();
                                AryaTools.Instance.SaveChangesIfNecessary(false, false);
                            }
                        }
                    }
                    else if (schematus != null && schematus == _enrichmentImageSchematus)
                    {
                        if (SchemaAttribute.GetValue(taxonomy, attribute, _enrichmentImageSchematus) != null)
                        {
                            DialogResult results = MessageBox.Show("Are you sure that you want to delete this image?",
                                "Warning", MessageBoxButtons.YesNo);
                            if (results == DialogResult.Yes)
                            {
                                string result;
                                if ((result = SetValue(cell.ColumnIndex, cell.RowIndex, null)) != null)
                                    MessageBox.Show(result);
                            }
                        }
                    }
                    else
                    {
                        // otherwise, delete the contents...
                        string result;
                        if ((result = SetValue(cell.ColumnIndex, cell.RowIndex, null)) != null)
                            MessageBox.Show(result);
                    }
                }
            }
            else
            {
                // check to make sure a cell is selected
                if (sdgv.SelectedCells.Count == 1)
                {
                    var cell = sdgv.SelectedCells[0];
                    Attribute attribute;
                    TaxonomyInfo taxonomy;
                    SchemaAttribute schematus;
                    GetContext(GetColumnItem(cell.ColumnIndex), GetRowItem(cell.RowIndex), out taxonomy, out attribute,
                        out schematus);

                    // if so, see if we're on an LOV.
                    if (schematus != null
                        && schematus.SpecialSchemaAttribute == SchemaAttribute.SchemaAttributeListOfValues.ToString())
                    {
                        // now check the key code - if Ctrl-C, copy the LOV, if CTRL-V, paste an LOV.
                        if (e.Control && e.KeyCode == Keys.C)
                        {
                            var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
                            if (schemaInfo != null && schemaInfo.ActiveListOfValues.Any())
                            {
                                // build List of Values and stash it in a ClipboardObject
                                var lovList = schemaInfo.ActiveListOfValues.ToList();
                                var lovContainer = new SerializableObject<List<string>>(lovList);

                                var data = new DataObject();
                                data.SetText(cell.Value.ToString());
                                data.SetData("ListOfValues", lovContainer);
                                Clipboard.SetDataObject(data, true);
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            if (e.Control && e.KeyCode == Keys.V)
                            {
                                // get list of values container, assuming it is one

                                var lovContainer = Clipboard.GetData("ListOfValues") as SerializableObject<List<string>>;
                                if (lovContainer != null)
                                {
                                    var copiedListOfValues = lovContainer.DataClass;

                                    // get the destination
                                    var destinationSchemaInfo =
                                        taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
                                    if (destinationSchemaInfo != null)
                                    {
                                        DialogResult overwrite;
                                        // check for an existing List of values, if so ask, if not assume overwriting.
                                        if (destinationSchemaInfo.ActiveListOfValues.Any())
                                        {
                                            overwrite =
                                                MessageBox.Show(
                                                    "Do you want to overwrite current list of values?"
                                                    + "\n\nClick <yes> to overwrite current list of values"
                                                    + "\nClick <no> to append  values to current list"
                                                    + "\nClick <cancel> to close with no changes", "Question",
                                                    MessageBoxButtons.YesNoCancel);
                                        }
                                        else
                                            overwrite = DialogResult.Yes;

                                        switch (overwrite)
                                        {
                                            case DialogResult.Yes:
                                                {
                                                    destinationSchemaInfo.ActiveListOfValues = copiedListOfValues;
                                                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                                                    break;
                                                }

                                            case DialogResult.No:
                                                {
                                                    var currentLov = destinationSchemaInfo.ActiveListOfValues.ToList();
                                                    foreach (var lov in copiedListOfValues)
                                                    {
                                                        if (!currentLov.Contains(lov))
                                                            currentLov.Add(lov);
                                                    }
                                                    destinationSchemaInfo.ActiveListOfValues = currentLov;
                                                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                                                    break;
                                                }

                                            case DialogResult.Cancel:
                                                {
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void sdgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            sdgv.Rows.Cast<DataGridViewRow>()
                .ForEach(row => row.HeaderCell.Value = row.Index == 0 ? String.Empty : row.Index.ToString());
            sdgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void sdgv_SelectionChanged(object sender, EventArgs e)
        {
            UpdateOptions();

            if (auditTrailToolStripMenuItem.Checked && CurrentTaxonomies.Count == 1)
            {
                TaxonomyInfo taxonomy;
                Attribute attribute;
                SchemaAttribute schemaAttribute;

                GetContext(GetColumnItem(sdgv.CurrentCell.ColumnIndex), GetRowItem(sdgv.CurrentCell.RowIndex),
                    out taxonomy, out attribute, out schemaAttribute);

                taxonomy = taxonomy ?? CurrentTaxonomies[0];

                if (attribute == null)
                    return;

                var schemaInfo = taxonomy.SchemaInfos.SingleOrDefault(p => p.AttributeID == attribute.ID);

                if (schemaInfo == null)
                {
                    sdgvAuditTrail.DataSource = null;
                    return;
                }

                if (schemaAttribute != null && schemaAttribute.AttributeType == SchemaAttribute.SchemaAttributeType.Meta)
                {
                    sdgvAuditTrail.DataSource = null;
                    sdgvAuditTrail.DataSource =
                        new BindingSource(
                            (from mi in
                                 schemaInfo.SchemaMetaInfos.Where(
                                     p => p.Attribute.Equals(schemaAttribute.MetaSchemaAttribute)).ToList()
                             from md in mi.SchemaMetaDatas
                             select
                                 new
                                 {
                                     mi.Attribute.AttributeName,
                                     md.Value,
                                     md.Active,
                                     md.CreatedOn,
                                     md.CreatedBy,
                                     CreatedRemark = (md.CreatedRemark == null) ? string.Empty : md.Remark.Remark1,
                                     md.DeletedOn,
                                     md.DeletedBy,
                                     DeletedRemark = (md.DeletedRemark == null) ? string.Empty : md.Remark1.Remark1,
                                     md.MetaID,
                                     MetaDataID = md.ID
                                 }).OrderByDescending(p => p.CreatedOn)
                                .ThenByDescending(p => p.Active)
                                .ToDataTable(), null);

                    foreach (DataGridViewColumn col in sdgvAuditTrail.Columns)
                        col.HeaderCell = new DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
                else
                {
                    sdgvAuditTrail.DataSource = null;
                    sdgvAuditTrail.DataSource =
                        new BindingSource(
                            schemaInfo.SchemaDatas.ToList()
                                .Select(
                                    p =>
                                        new
                                        {
                                            p.DataType,
                                            p.InSchema,
                                            p.NavigationOrder,
                                            p.DisplayOrder,
                                            p.Active,
                                            p.CreatedOn,
                                            p.CreatedBy,
                                            CreatedRemark = (p.CreatedRemark == null) ? string.Empty : p.Remark.Remark1,
                                            p.DeletedOn,
                                            p.DeletedBy,
                                            DeletedRemark =
                                                (p.DeletedRemark == null) ? string.Empty : p.Remark1.Remark1
                                        })
                                .OrderByDescending(p => p.CreatedOn)
                                .ThenByDescending(p => p.Active)
                                .ToDataTable(), null);

                    foreach (DataGridViewColumn col in sdgvAuditTrail.Columns)
                        col.HeaderCell = new DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
            }
            else
                sdgvAuditTrail.DataSource = null;

            if (sdgvAuditTrail.DataSource != null)
                sdgvAuditTrail.DefaultCellStyle = DisplayStyle.CellStyleGreyRegular;
        }

        #endregion

        #region Menu Handlers

        private void allAttributeSchematiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clone(false, true, false, true, true);
        }

        private void allSchematiWithEmptyranksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clone(false, true, true, true, true);
        }

        private void attributePreferencesPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var attrPrefs = new FrmUserPreferences();
            attrPrefs.ShowDialog(this);
        }

        private void auditTrailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (auditTrailToolStripMenuItem.Checked)
            {
                splitterAuditTrail.Visible = true;
                sdgvAuditTrail.Visible = true;
            }
            else
            {
                splitterAuditTrail.Visible = false;
                sdgvAuditTrail.Visible = false;
            }
        }

        private void autoorderRanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoOrderRanks = autoorderRanksToolStripMenuItem.Checked;
            TryOrderRanks();
        }

        private void calculatedAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sdgv.SelectedCells.Count != 1)
                return;

            var selectedAttribute =
                sdgv.SelectedCells.Cast<DataGridViewCell>()
                    .Select(cell => cell.OwningRow.Cells[0].Value)
                    .OfType<Attribute>()
                    .First();

            if (selectedAttribute == null)
                return;

            var derivedAttribute = GetDerivedAttribute(selectedAttribute, CurrentTaxonomies[0]);

            var expression = derivedAttribute != null ? derivedAttribute.Expression : string.Empty;

            var maxLength = derivedAttribute != null ? derivedAttribute.MaxResultLength : 255;

            var attributes =
                _allAttributes.OfType<Attribute>()
                    .Where(att => att.AttributeType != AttributeTypeEnum.Derived.ToString())
                    .Select(att => att.AttributeName);

            var top10Skus = CurrentTaxonomies[0].SkuInfos.Where(p => p.Active).Select(p => p.Sku).Take(10).ToList();

            var expressionBuilder = new FrmAttributeBuilder(expression, maxLength, attributes, top10Skus,
                (derivedAttribute != null) ? derivedAttribute.TaxonomyInfo : null,
                !(derivedAttribute == null || derivedAttribute.TaxonomyID == CurrentTaxonomies[0].ID),
                derivedAttribute == null ? string.Empty : derivedAttribute.Attribute.AttributeName);

            expressionBuilder.Show(this);

            expressionBuilder.FormClosed += (s, e1) =>
                                            {
                                                var result = expressionBuilder.DialogResult;
                                                if (result != DialogResult.OK)
                                                    return;

                                                if (derivedAttribute != null
                                                    && ((derivedAttribute.TaxonomyID == CurrentTaxonomies[0].ID)
                                                        || (derivedAttribute.TaxonomyID != CurrentTaxonomies[0].ID
                                                            && expressionBuilder.IsExpressionInherited)))
                                                {
                                                    if (string.IsNullOrWhiteSpace(expressionBuilder.AttributeExpression))
                                                    {
                                                        AryaTools.Instance.InstanceData.Dc.DerivedAttributes
                                                            .DeleteOnSubmit(derivedAttribute);
                                                    }
                                                    else
                                                    {
                                                        derivedAttribute.Expression =
                                                            expressionBuilder.AttributeExpression;
                                                        derivedAttribute.MaxResultLength = expressionBuilder.MaxLength;
                                                    }
                                                }
                                                else if (
                                                    !string.IsNullOrWhiteSpace(expressionBuilder.AttributeExpression))
                                                {
                                                    selectedAttribute.DerivedAttributes.Add(new DerivedAttribute
                                                                                            {
                                                                                                TaxonomyInfo
                                                                                                    =
                                                                                                    CurrentTaxonomies
                                                                                                    [
                                                                                                        0
                                                                                                    ],
                                                                                                Expression
                                                                                                    =
                                                                                                    expressionBuilder
                                                                                                    .AttributeExpression,
                                                                                                MaxResultLength
                                                                                                    =
                                                                                                    expressionBuilder
                                                                                                    .MaxLength
                                                                                            });
                                                }
                                            };
        }

        private void calculateFillRatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitData();
            PrepareGridView();
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.Forms.SchemaForm.CloseCurrentTab();
        }

        private void currentValueOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clone(true, false, false, false, true, false);
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e) { Clone(); }

        private void displayOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTaxonomies == null)
                return;
            CurrentTaxonomies.ForEach(SchemaAttribute.OrderByDisplay);
            sdgv.Invalidate();
        }

        private void level1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GreatGreatGrandparentNodeToolStripMenuItem.Checked = !GreatGreatGrandparentNodeToolStripMenuItem.Checked;
            GreatGrandparentNodeToolStripMenuItem.Checked = false;
            GrandparentNodeToolStripMenuItem.Checked = false;
            ParentNodeToolStripMenuItem.Checked = false;
            InitRowsAndColumns();
            PrepareGridView();
        }

        private void level2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GreatGreatGrandparentNodeToolStripMenuItem.Checked = false;
            GreatGrandparentNodeToolStripMenuItem.Checked = !GreatGrandparentNodeToolStripMenuItem.Checked;
            GrandparentNodeToolStripMenuItem.Checked = false;
            ParentNodeToolStripMenuItem.Checked = false;
            InitRowsAndColumns();
            PrepareGridView();
        }

        private void level3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GreatGreatGrandparentNodeToolStripMenuItem.Checked = false;
            GreatGrandparentNodeToolStripMenuItem.Checked = false;
            GrandparentNodeToolStripMenuItem.Checked = !GrandparentNodeToolStripMenuItem.Checked;
            ParentNodeToolStripMenuItem.Checked = false;
            InitRowsAndColumns();
            PrepareGridView();
        }

        private void levelLeafNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GreatGreatGrandparentNodeToolStripMenuItem.Checked = false;
            GreatGrandparentNodeToolStripMenuItem.Checked = false;
            GrandparentNodeToolStripMenuItem.Checked = false;
            ParentNodeToolStripMenuItem.Checked = !ParentNodeToolStripMenuItem.Checked;
            InitRowsAndColumns();
            PrepareGridView();
        }

        private void navOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTaxonomies == null)
                return;
            CurrentTaxonomies.ForEach(SchemaAttribute.OrderByNavigation);
            sdgv.Invalidate();
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //List<SchemaInfo> selectedSchemaInfos = new List<SchemaInfo>();
            //for (int i = 0; i < sdgv.SelectedCells.Count; i++)
            //{
            //    var rowItem = GetRowItem(sdgv.SelectedCells[i].RowIndex);
            //    var columnItem = GetColumnItem(sdgv.SelectedCells[i].ColumnIndex);

            //    if (rowItem is Attribute && columnItem == null)
            //    {

            //        var sis = ((Attribute)rowItem).SchemaInfos.Where(t => CurrentTaxonomies.Contains(t.TaxonomyInfo)).Select(s=>s);
            //        selectedSchemaInfos.AddRange(sis);
            //    }

            //}

            //selectedSchemaInfos = selectedSchemaInfos.Distinct().ToList();
        }

        private void openInNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var value = sdgv.CurrentCell.Value;
            object filter = null;
            if (value is Attribute)
            {
                //filter = CurrentTaxonomies[0];
                filter = (from ct in CurrentTaxonomies
                          orderby ct.ToString()
                          from si in ct.SchemaInfos
                          where si.SchemaData != null && si.Attribute.Equals(value)
                          select si.TaxonomyInfo).FirstOrDefault();
            }
            AryaTools.Instance.Forms.SchemaForm.LoadTab(new List<object> { value }, filter);
        }

        private void populateSampleValues1_Click(object sender, EventArgs e) { PopulateSampleValues(); }

        private void RefreshAttributesFromSkuDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefresAttributesFromSkuData();
        }

        private void reloadTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.AllFillRates.FillRates.Clear();
            RefreshCurrentTab();
        }

        private void removeSelectedNodesFromViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var nodesToRemove =
                (from DataGridViewCell cell in sdgv.SelectedCells select cell.Value).OfType<TaxonomyInfo>().ToList();

            if (!nodesToRemove.Any())
            {
                MessageBox.Show("Please select cell(s) containing taxonomy to remove it from the current view",
                    "Nothing to remove!");
                return;
            }
            nodesToRemove.ForEach(tax => CurrentTaxonomies.Remove(tax));
            InitRowsAndColumns();
            PrepareGridView();
        }

        private void saveAllChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AryaTools.Instance.SaveChangesIfNecessary(false, true);
        }

        private void unmapSelectedAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get list of Attributes to unmap
            var attributesToUnmap =
                sdgv.SelectedCells.Cast<DataGridViewCell>()
                    .Select(cell => cell.OwningRow.Cells[0].Value)
                    .OfType<Attribute>()
                    .OrderBy(i => i.ToString())
                    .ToList();

            if (attributesToUnmap.Count > 0)
            {
                // extract names of attributes, separated by newlines
                var attNames = attributesToUnmap.Aggregate(String.Empty,
                    (i, j) => i + j.ToString() + Environment.NewLine);

                var result =
                    MessageBox.Show(
                        "The following attributes will be unmapped:\n\n" + attNames
                        + "\nAre you sure you want to do this? This action cannot be undone.", "Warning",
                        MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    // unmap Attributes
                    _unmapAttributesWithSkus = null;
                    attributesToUnmap.ForEach(
                        att => _unmapAttributesWithSkus = UnmapAttribute(att, _unmapAttributesWithSkus));
                }
            }
        }

        private void viewAdditionalNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTaxonomies == null || CurrentTaxonomies.Count <= 1)
                return;
            AryaTools.Instance.Forms.TreeForm.GetTaxonomy("Include in current view", false);
            AryaTools.Instance.Forms.TreeForm.TaxonomySelected += (s, ea) => ViewAdditionalNodes();
        }

        #endregion

        #region Methods

        private void InitData()
        {
            var sw = new Stopwatch();
            sw.Start();

            //default false
            AutoOrderRanks = false;

            var waitkey = FrmWaitScreen.ShowMessage("Loading Attributes");

            // if the Autocomplete collection of the class has not been initialized, do it now.
            if (_autoCompleteAttributeName == null)
            {
                var autoCompleteSource = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                                          where att.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject)
                                          orderby att.AttributeName
                                          select att.AttributeName).ToArray();

                _autoCompleteAttributeName = new AutoCompleteStringCollection();
                _autoCompleteAttributeName.AddRange(autoCompleteSource);

                _autoCompleteDataType = new AutoCompleteStringCollection();
                _autoCompleteDataType.AddRange(new[] { "Text", "LOV", "Number", "Fraction", "Boolean", "Decimal" });

                sw.Stop();
                Diagnostics.WriteMessage("AutoComplete Attribute Source for Schema View", "SchemaGridView - InitData()",
                    sw.Elapsed);
                sw.Reset();
            }

            sw.Start();

            // fill the list of meta-attributes with Primary, Secondary, and (optionally) Fill Rate meta-attributes. 
            _allSchemati = SchemaAttribute.PrimarySchemati;
            if (ShowFillRate)
                _allSchemati = _allSchemati.Union(SchemaAttribute.FillRateSchemati).ToList();
            _allSchemati = _allSchemati.Union(SchemaAttribute.SecondarySchemati).ToList();

            // Get a copy of the Enrichment Image meta-attribute
            _enrichmentImageSchematus =
                _allSchemati.FirstOrDefault(
                    s =>
                        s.AttributeType == SchemaAttribute.SchemaAttributeType.Meta
                        && s.MetaSchemaAttribute.AttributeName == Resources.SchemaEnrichmentImageAttributeName);

            // get list of meta-attributes for autocompletion
            SchemaAttribute.SecondarySchemati.ForEach(schematus => GetAutoCompleteSource(schematus.MetaSchemaAttribute));

            sw.Stop();
            Diagnostics.WriteMessage("AutoComplete Meta-attribute Source for Schema View", "SchemaGridView - InitData()",
                sw.Elapsed);
            sw.Reset();

            // get the SchemaInfos with active SchemaDatas that match the provided Taxonomies or Attributes,
            // in that order of precedence
            FrmWaitScreen.UpdateMessage(waitkey, "Fetching Meta-Attributes");
            sw.Start();
            List<SchemaInfo> schemaInfos;
            var taxIds = (CurrentTaxonomies ?? new List<TaxonomyInfo>()).Select(tax => tax.ID).ToList();
            var attIds = (CurrentAttributes ?? new List<Attribute>()).Select(att => att.ID).ToList();

            if (taxIds.Any())
            {
                schemaInfos = (from si in AryaTools.Instance.InstanceData.Dc.SchemaInfos
                               where
                                   taxIds.Contains(si.TaxonomyID)
                                   && (si.SchemaDatas.Any(sd => sd.Active)
                                       || si.SchemaMetaInfos.SelectMany(mi => mi.SchemaMetaDatas).Any(md => md.Active))
                               select si).ToList();
            }
            else if (attIds.Any())
            {
                schemaInfos = (from si in AryaTools.Instance.InstanceData.Dc.SchemaInfos
                               where
                                   attIds.Contains(si.AttributeID)
                                   && (si.SchemaDatas.Any(sd => sd.Active)
                                       || si.SchemaMetaInfos.SelectMany(mi => mi.SchemaMetaDatas).Any(md => md.Active))
                               select si).ToList();
            }
            else
                schemaInfos = new List<SchemaInfo>();

            // get the list of attributes corresponding to the selected SchemaInfos, in attribute name order
            _allAttributes =
                schemaInfos.Select(si => si.Attribute)
                    .Distinct(new KeyEqualityComparer<Attribute>(att => att.ID))
                    .OrderBy(att => att.AttributeName)
                    .Cast<object>()
                    .ToList();

            // get the list of taxonomy nodes corresponding to the selected SchemaInfos, in taxonomy string order
            _allTaxonomies =
                schemaInfos.Select(si => si.TaxonomyInfo)
                    .Distinct(new KeyEqualityComparer<TaxonomyInfo>(tax => tax.ID))
                    .OrderBy(tax => tax.ToString())
                    .Cast<object>()
                    .ToList();

            sw.Stop();
            Diagnostics.WriteMessage("Fetching Schema", "SchemaGridView - InitData()", sw.Elapsed, null,
                _allTaxonomies.Count, _allAttributes.Count);
            sw.Reset();

            // set up view style (single taxonomy, multi-taxonomy, or attribute)
            sw.Start();
            InitRowsAndColumns();
            sw.Stop();
            Diagnostics.WriteMessage("Initializing Rows and Columns for SchemaGrid",
                "SchemaGridView - InitRowsAndColumns()", sw.Elapsed, null, _allTaxonomies.Count, _allAttributes.Count);
            sw.Reset();

            // if there is more than one Taxonomy or Attribute on tab, meta-attributes are selected by dropdown
            if ((CurrentTaxonomies != null && CurrentTaxonomies.Count > 1)
                || (CurrentAttributes != null && CurrentAttributes.Count > 1))
            {
                ddCurrentSchematus.DataSource = _allSchemati;
                CurrentSchematus = SchemaAttribute.PrimarySchemati[0];
            }
            else
            {
                ddCurrentSchematus.Visible = false;
                lblCurrentSchematus.Visible = false;
            }

            sw.Start();
            UpdateOptions();
            sw.Stop();
            Diagnostics.WriteMessage("Updating Options for SchemaGrid", "SchemaGridView -  UpdateOptions()", sw.Elapsed,
                null, _allTaxonomies.Count, _allAttributes.Count);
            sw.Reset();

            ShortcutKeysEnabled = true;
            FrmWaitScreen.HideMessage(waitkey);
        }

        private void PrepareGridView()
        {
            // This sets up the cell styles for the grid

            var waitkey = FrmWaitScreen.ShowMessage("Drawing Grid View");

            //for some reason, this is getting triggered when clearing the columns.
            //fixes issue #13.
            sdgv.CellValueNeeded -= sdgv_CellValueNeeded;
            sdgv.Columns.Clear();
            sdgv.Rows.Clear();
            sdgv.CellValueNeeded += sdgv_CellValueNeeded;

            ResetRowColumnCount();

            if (CurrentTaxonomies != null)
            {
                for (var i = 0; i < _columns.Count; i++)
                {
                    var colHeader = _columns[i];
                    var schemaAttribute = colHeader as SchemaAttribute;
                    if (schemaAttribute != null)
                    {
                        sdgv.Columns[i + 1].DefaultCellStyle = _primarySchemaAttributes.Contains(colHeader.ToString())
                            ? DisplayStyle.CellStyleGreyItalic
                            : DisplayStyle.CellStyleSchemaMeta;

                        switch (schemaAttribute.AttributeType)
                        {
                            case SchemaAttribute.SchemaAttributeType.Primary:
                                if (schemaAttribute.DataType == typeof(decimal))
                                    sdgv.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleSchemaDecimal;
                                if (schemaAttribute.DataType == typeof(bool))
                                    sdgv.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleSchemaBoolean;
                                break;

                            case SchemaAttribute.SchemaAttributeType.Meta:
                                if (schemaAttribute.DataType == typeof(bool))
                                    sdgv.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleSchemaMetaBoolean;
                                break;

                            // DMS: added case for Restricted List Of Values column
                            case SchemaAttribute.SchemaAttributeType.Special:
                                if (schemaAttribute.SpecialSchemaAttribute
                                    == SchemaAttribute.SpecialSchematusListOfValues)
                                    sdgv.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleGreyLovRegular;
                                break;
                            default: //Handle calculates schema attribute
                                sdgv.Columns[i + 1].ReadOnly = true;
                                break;
                        }
                    }
                }
            }

            sdgv.Rows[0].Frozen = true;
            sdgv.Rows[0].DefaultCellStyle = DisplayStyle.CellStyleFirstRow;

            sdgv.Columns[0].Frozen = true;
            sdgv.Columns[0].DefaultCellStyle = DisplayStyle.CellStyleAttributeColumn;

            for (var i = 0; i < _rows.Count; i++)
            {
                if (i % 2 != 0)
                    sdgv.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;
            }

            if (CurrentAttributes != null)
            {
                for (var i = 0; i < _rows.Count; i++)
                {
                    var schemaAttribute = _rows[i] as SchemaAttribute;
                    if (schemaAttribute != null)
                    {
                        switch (schemaAttribute.AttributeType)
                        {
                            case SchemaAttribute.SchemaAttributeType.Primary:
                                if (schemaAttribute.DataType == typeof(decimal))
                                {
                                    for (var cellIndex = 1; cellIndex < sdgv.Rows[i + 1].Cells.Count; cellIndex++)
                                    {
                                        sdgv.Rows[i + 1].Cells[cellIndex].Style =
                                            DisplayStyle.CellStyleSchemaDecimalAttributeTab;
                                    }
                                }
                                else if (schemaAttribute.DataType == typeof(bool))
                                {
                                    for (var cellIndex = 1; cellIndex < sdgv.Rows[i + 1].Cells.Count; cellIndex++)
                                        sdgv.Rows[i + 1].Cells[cellIndex].Style = DisplayStyle.CellStyleSchemaBoolean;
                                }
                                break;

                            case SchemaAttribute.SchemaAttributeType.Meta:
                                if (schemaAttribute.DataType == typeof(bool))
                                {
                                    for (var cellIndex = 1; cellIndex < sdgv.Rows[i + 1].Cells.Count; cellIndex++)
                                        sdgv.Rows[i + 1].Cells[cellIndex].Style = DisplayStyle.CellStyleSchemaBoolean;
                                }
                                break;

                            // DMS: added case for Restricted List Of Values column
                            case SchemaAttribute.SchemaAttributeType.Special:
                                if (schemaAttribute.SpecialSchemaAttribute
                                    == SchemaAttribute.SpecialSchematusListOfValues)
                                {
                                    for (var cellIndex = 1; cellIndex < sdgv.Rows[i + 1].Cells.Count; cellIndex++)
                                    {
                                        sdgv.Rows[i + 1].Cells[cellIndex].Style =
                                            DisplayStyle.CellStyleGreyLovRegularAttributeTab;
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            autoorderRanksToolStripMenuItem.Checked = AutoOrderRanks;
            TryOrderRanks();

            AutoResizeColumns();
            ShowHideOptions();
            FrmWaitScreen.HideMessage(waitkey);
        }

        private object GetValue(object colItem, object rowItem)
        {
            if (colItem == null)
                return rowItem;

            // determine context
            TaxonomyInfo taxonomy;
            Attribute attribute;
            SchemaAttribute schematus;
            GetContext(colItem, rowItem, out taxonomy, out attribute, out schematus);

            // get meta-attribute - if no value, figure out whether or not it's mapped
            var value = SchemaAttribute.GetValue(taxonomy, attribute, schematus);
            if (value == null)
            {
                var isMapped = SchemaAttribute.GetValue(taxonomy, attribute, SchemaAttribute.SchemaAttributeIsMapped);
                if (isMapped != null)
                    return EmptyMeta;
            }
            return value;
        }

        private object GetRowItem(int rowIndex)
        {
            // map row index to corresponding item in row collection
            return rowIndex > _rows.Count && CurrentTaxonomies != null && CurrentTaxonomies.Count > 0
                ? _blankAttribute
                : rowIndex > 0 && rowIndex <= _rows.Count ? _rows[rowIndex - 1] : null;
        }

        private object GetColumnItem(int columnIndex)
        {
            // map column index to corresponding item in column collection
            return columnIndex > 0 ? _columns[columnIndex - 1] : null;
        }

        private void GetContext(object colItem, object rowItem, out TaxonomyInfo taxonomy, out Attribute attribute,
            out SchemaAttribute schematus)
        {
            // assume nothing
            taxonomy = null;
            attribute = null;
            schematus = null;

            // row is a valid attribute
            if (rowItem is Attribute)
            {
                attribute = (Attribute)rowItem;
                if (colItem != null)
                {
                    taxonomy = colItem is TaxonomyInfo ? (TaxonomyInfo)colItem : CurrentTaxonomies[0];
                    schematus = colItem is SchemaAttribute ? (SchemaAttribute)colItem : CurrentSchematus;
                }
                return;
            }

            // column is a valid taxonomy
            if (colItem is TaxonomyInfo)
            {
                // Row item is definitely not Attribute - because of the above case
                taxonomy = (TaxonomyInfo)colItem;
                if (rowItem != null)
                {
                    attribute = CurrentAttributes[0];
                    schematus = (SchemaAttribute)rowItem;
                }
                return;
            }

            if (rowItem is SchemaAttribute)
                schematus = (SchemaAttribute)rowItem;

            if (colItem is SchemaAttribute)
                schematus = (SchemaAttribute)colItem;
        }

        private string SetValue(int colIndex, int rowIndex, object value)
        {
            var rowHeader = sdgv[0, rowIndex].Value;
            var colHeader = sdgv[colIndex, 0].Value;

            string result;
            SchemaAttribute schematus = null;

            if (rowHeader is Attribute)
            {
                var attribute = (Attribute)rowHeader;
                schematus = colHeader is SchemaAttribute ? (SchemaAttribute)colHeader : CurrentSchematus;
                if (colIndex == 0 && value == null)
                {
                    _unmapAttributesWithSkus = UnmapAttribute(attribute, _unmapAttributesWithSkus);
                    return null;
                }

                var taxonomy = colHeader is TaxonomyInfo
                    ? (TaxonomyInfo)colHeader
                    : CurrentTaxonomies.Count == 1 ? CurrentTaxonomies[0] : null;

                result = schematus == null || taxonomy == null
                    ? RenameAttribute(colIndex, rowIndex, value.ToString())
                    : SchemaAttribute.SetValue(taxonomy, attribute, false, schematus, value);
            }
            else if (colHeader is TaxonomyInfo)
            {
                var taxonomy = (TaxonomyInfo)colHeader;
                var attribute = CurrentAttributes[0];
                schematus = (SchemaAttribute)rowHeader;
                result = SchemaAttribute.SetValue(taxonomy, attribute, false, schematus, value);
            }
            else
                result = string.Format("Invalid value [{0}] for [{1}] - [{2}]", value, rowHeader, colHeader);

            if (schematus != null
                && (schematus.Equals(SchemaAttribute.SchemaAttributeNavigationOrder)
                    || schematus.Equals(SchemaAttribute.SchemaAttributeDisplayOrder)))
                sdgv.InvalidateColumn(colIndex);
            else
                sdgv.InvalidateCell(colIndex, rowIndex);

            return result;
        }

        private void AutoResizeColumns(int col = -1)
        {
            var columns = col == -1 ? Enumerable.Range(0, sdgv.ColumnCount) : col.AsEnumerable();
            foreach (var i in columns)
            {
                var preferredWidth = sdgv.Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells,
                    false);
                if (preferredWidth > sdgv.Columns[i].Width)
                    sdgv.Columns[i].Width = preferredWidth > 250 ? 250 : preferredWidth;
            }
        }

        private void Clone()
        {
            var cloneOptionsForm = AryaTools.Instance.Forms.CloneOptionsForm;
            var result = cloneOptionsForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                Clone(cloneOptionsForm.CurrentSchematusOnly, cloneOptionsForm.PrimarySchemati,
                    cloneOptionsForm.EmptyRanks, cloneOptionsForm.SecondarySchemati, cloneOptionsForm.ListOfValues,
                    cloneOptionsForm.NonBlankOnly);
            }
        }

        private void Clone(bool currentValueOnly, bool primarySchemati, bool emptyRanks, bool secondarySchemati,
            bool listOfValues, bool nonBlankOnly = false)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Cloning");
            var selectedCells = sdgv.SelectedCells;
            foreach (DataGridViewTextBoxCell cell in selectedCells)
            {
                var rowHeaderValue = sdgv[0, cell.RowIndex].Value;
                var columnHeaderValue = sdgv[cell.ColumnIndex, 0].Value;
                CloneAttribute(cell, currentValueOnly, primarySchemati, emptyRanks, secondarySchemati, listOfValues,
                    nonBlankOnly, waitkey, rowHeaderValue, columnHeaderValue);
            }

            FrmWaitScreen.HideMessage(waitkey);
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            sdgv.Invalidate();
        }

        private void CloneAttribute(DataGridViewTextBoxCell currentcell, bool currentValueOnly, bool primarySchemati,
            bool emptyRanks, bool secondarySchemati, bool listOfValues, bool nonBlankOnly, Guid waitkey,
            object rowHeaderValue, object columnHeaderValue)
        {
            var thisAttribute = rowHeaderValue is Attribute
                ? (Attribute)rowHeaderValue
                : CurrentAttributes != null ? CurrentAttributes[0] : null;
            //var thisTaxonomy = columnHeaderValue is TaxonomyInfo ? (TaxonomyInfo)columnHeaderValue : null;

            var thisTaxonomy = columnHeaderValue as TaxonomyInfo;

            //var thisAttribute = ((Attribute)rowHeaderValue);
            //var thisTaxonomy = ((TaxonomyInfo)columnHeaderValue);

            if (thisAttribute == null || thisTaxonomy == null)
                return;

            var taxonomies = CurrentTaxonomies
                             ?? (_columns[0] is TaxonomyInfo
                                 ? _columns.Cast<TaxonomyInfo>().ToList()
                                 : new List<TaxonomyInfo>());

            taxonomies = taxonomies.Where(tax => !tax.Equals(thisTaxonomy)).ToList();

            if (taxonomies.Count == 0)
                return;

            if (currentValueOnly)
            {
                FrmWaitScreen.UpdateMessage(waitkey, "Cloning Schema Meta-attribute");

                var currentSchematus = rowHeaderValue as SchemaAttribute ?? CurrentSchematus;

                var currentValue = currentcell.Value;
                if (currentSchematus == null || currentValue == null || currentValue.ToString().Equals(string.Empty))
                    return;

                if (currentSchematus.AttributeType == SchemaAttribute.SchemaAttributeType.Primary
                    || currentSchematus.AttributeType == SchemaAttribute.SchemaAttributeType.Meta)
                {
                    taxonomies.ForEach(tax =>
                                       {
                                           if (nonBlankOnly)
                                           {
                                               var currVal = SchemaAttribute.GetValue(tax, thisAttribute,
                                                   currentSchematus);
                                               if (currVal != null && !string.IsNullOrEmpty(currVal.ToString().Trim()))
                                               {
                                                   SchemaAttribute.SetValue(tax, thisAttribute, true, currentSchematus,
                                                       currentValue);
                                               }
                                           }
                                           else
                                           {
                                               SchemaAttribute.SetValue(tax, thisAttribute, true, currentSchematus,
                                                   currentValue);
                                           }
                                       });
                }
                else if (currentSchematus.AttributeType == SchemaAttribute.SchemaAttributeType.Special)
                {
                    taxonomies.ForEach(tax =>
                                       {
                                           var schemaInfo =
                                               thisTaxonomy.SchemaInfos.FirstOrDefault(
                                                   si => si.Attribute.Equals(thisAttribute));
                                           if (schemaInfo != null)
                                           {
                                               if (listOfValues)
                                                   CloneLov(thisAttribute, schemaInfo, tax);
                                           }
                                       });
                }
            }
            else
            {
                var schemaInfo = thisTaxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(thisAttribute));
                if (schemaInfo == null)
                {
                    FrmWaitScreen.HideMessage(waitkey);
                    return;
                }

                if (emptyRanks)
                {
                    if (taxonomies.Any(tax =>
                                       {
                                           if (tax.Equals(thisTaxonomy))
                                               return false;

                                           var navOrder = SchemaAttribute.GetValue(tax, thisAttribute,
                                               SchemaAttribute.SchemaAttributeNavigationOrder);
                                           var dispOrder = SchemaAttribute.GetValue(tax, thisAttribute,
                                               SchemaAttribute.SchemaAttributeDisplayOrder);
                                           double nav = 0, disp = 0;
                                           if (navOrder != null)
                                               double.TryParse(navOrder.ToString(), out nav);
                                           if (dispOrder != null)
                                               double.TryParse(dispOrder.ToString(), out disp);

                                           return nav != 0 || disp != 0;
                                       }))
                    {
                        if (
                            MessageBox.Show(
                                @"One or more destination Schema Meta-attributes currently have ranks that will be reset. Are you sure?",
                                @"Reset Ranks", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                }

                var newSchemaData = emptyRanks
                    ? new SchemaData
                      {
                          DataType = schemaInfo.SchemaData.DataType,
                          InSchema = schemaInfo.SchemaData.InSchema
                      }
                    : schemaInfo.SchemaData;

                var taxCount = taxonomies.Count;
                for (var i = 0; i < taxCount; i++)
                {
                    var tax = taxonomies[i];

                    FrmWaitScreen.UpdateMessage(waitkey,
                        string.Format("Cloning Schema Meta-attributes ({0} of {1})", i, taxCount));

                    if (primarySchemati)
                        SchemaAttribute.SetValue(tax, thisAttribute, true, null, newSchemaData);

                    if (secondarySchemati)
                    {
                        SchemaAttribute.SecondarySchemati.ForEach(schematus =>
                                                                  {
                                                                      var newValue =
                                                                          SchemaAttribute.GetValue(thisTaxonomy,
                                                                              thisAttribute, schematus);
                                                                      if (newValue != null)
                                                                      {
                                                                          SchemaAttribute.SetValue(tax, thisAttribute,
                                                                              true, schematus, newValue);
                                                                      }
                                                                  });
                    }

                    if (listOfValues)
                        CloneLov(thisAttribute, schemaInfo, tax);
                }
            }
        }

        private static void CloneLov(Attribute thisAttribute, SchemaInfo schemaInfo, TaxonomyInfo tax)
        {
            var targetSchemaInfo =
                (from si in tax.SchemaInfos where si.Attribute.Equals(thisAttribute) select si).FirstOrDefault();

            if (targetSchemaInfo == null)
                SchemaAttribute.SetValue(tax, thisAttribute, false, null, new SchemaData());

            targetSchemaInfo =
                (from si in tax.SchemaInfos where si.Attribute.Equals(thisAttribute) select si).FirstOrDefault();

            if (targetSchemaInfo != null)
                targetSchemaInfo.ActiveListOfValues = schemaInfo.ActiveListOfValues;
        }

        private static AutoCompleteStringCollection GetAutoCompleteSource(Attribute metaSchemaAttribute,
            CheckState autoSuggestOptions = CheckState.Indeterminate)
        {
            //autoSuggest
            //Always Multi Line -> Checked
            //Always Suggest -> Uncheked
            //Auto Multiline/Suggest -> Indeterminate

            if (AutoCompleteCache.ContainsKey(metaSchemaAttribute) && autoSuggestOptions == CheckState.Unchecked)
                return AutoCompleteCache[metaSchemaAttribute];

            List<string> activeValues = null;
            var schemaInfo = metaSchemaAttribute.SchemaInfos.FirstOrDefault(s => s.TaxonomyID.Equals(Guid.Empty));
            if (schemaInfo != null)
                activeValues = schemaInfo.ListOfValues.Where(lov => lov.Active).Select(lov => lov.Value).ToList();

            if (activeValues == null || activeValues.Count == 0)
            {
                activeValues = (from meta in AryaTools.Instance.InstanceData.Dc.SchemaMetaInfos
                                where meta.Attribute.Equals(metaSchemaAttribute)
                                from smd in meta.SchemaMetaDatas
                                where smd.Active
                                select smd.Value).Distinct().ToList();

                var noOfActiveValues = activeValues.Count();
                var noOflongValues = activeValues.Count(val => val.Length > 50);

                if (noOflongValues > noOfActiveValues / 3 && autoSuggestOptions == CheckState.Indeterminate)
                {
                    if (!AutoCompleteCache.ContainsKey(metaSchemaAttribute))
                        AutoCompleteCache.Add(metaSchemaAttribute, null);
                    else
                        AutoCompleteCache[metaSchemaAttribute] = null;
                    return null;
                }
            }

            var newCollection = new AutoCompleteStringCollection();
            newCollection.AddRange(activeValues.OrderBy(v => v).ToArray());

            if (!AutoCompleteCache.ContainsKey(metaSchemaAttribute))
                AutoCompleteCache.Add(metaSchemaAttribute, newCollection);
            else
                AutoCompleteCache[metaSchemaAttribute] = newCollection;

            return newCollection;
        }

        public static void UpdateAutoCompleteSource(Attribute metaSchemaAttribute, string newValue)
        {
            AutoCompleteStringCollection source;
            if (!AutoCompleteCache.ContainsKey(metaSchemaAttribute)
                || (source = AutoCompleteCache[metaSchemaAttribute]) == null || source.Contains(newValue))
                return;

            source.Add(newValue);
        }

        private string GetMetaToolTip(SchemaAttribute meta)
        {
            var sb = new StringBuilder();

            var metaMetas = meta.MetaMetaSchemaAttributes;
            foreach (var key in metaMetas.Keys)
                sb.AppendFormat("{0}{1}{2}\n", key, (key.Equals(String.Empty) ? "" : ": "), metaMetas[key]);

            return sb.ToString();
        }

        private static DataGridViewCellStyle GetDefaultCellStyle(SchemaAttribute schemaAttribute)
        {
            switch (schemaAttribute.AttributeType)
            {
                case SchemaAttribute.SchemaAttributeType.Primary:
                    if (schemaAttribute.DataType == typeof(decimal))
                        return DisplayStyle.CellStyleSchemaDecimal;
                    if (schemaAttribute.DataType == typeof(bool))
                        return DisplayStyle.CellStyleSchemaBoolean;
                    return DisplayStyle.CellStyleGreyItalic;

                case SchemaAttribute.SchemaAttributeType.Meta:
                    if (schemaAttribute.DataType == typeof(bool))
                        return DisplayStyle.CellStyleSchemaMetaBoolean;
                    return DisplayStyle.CellStyleGreyMetaRegular;

                case SchemaAttribute.SchemaAttributeType.Special:
                    return DisplayStyle.CellStyleSpecialCenter;

                default:
                    return null;
            }
        }

        private void InitRowsAndColumns()
        {
            var waitkey = FrmWaitScreen.ShowMessage("Initializing Grid View");

            // create row collection
            // CurrentAttributes is null: use all attributes in rows
            // One CurrentAttribute: use all meta-attributes in rows
            // More than one CurrentAttribute: Use CurrentAttributes (not implemented)
            _rows = CurrentAttributes == null
                ? _allAttributes.Where(
                    attr =>
                        !AryaTools.Instance.InstanceData.CurrentUser.AttributeExclusions.Contains(
                            ((Attribute)attr).ID)).ToList()
                : (CurrentAttributes.Count == 1
                    ? _allSchemati.Cast<object>()
                    : CurrentAttributes.Where(
                        attr => !AryaTools.Instance.InstanceData.CurrentUser.AttributeExclusions.Contains(attr.ID))
                        .OrderBy(att => att.ToString())).ToList();

            // if Taxonomies are pre-selected, filter on that selection
            var taxFilterString = string.Empty;
            if (TaxonomyFilter != null)
            {
                string level1String, level2String, level3String;
                var taxString = TaxonomyFilter.ToString();
                //TaxonomyInfo.SplitTaxonomyString(taxString, out level1String, out level2String, out level3String);
                var taxLevelFullPaths = TaxonomyInfo.GetTaxonomyLevelFullPaths(taxString, 4);

                if (GreatGreatGrandparentNodeToolStripMenuItem.Checked)
                    taxFilterString = taxLevelFullPaths[0];
                else if (GreatGrandparentNodeToolStripMenuItem.Checked)
                    taxFilterString = taxLevelFullPaths[1];
                else if (GrandparentNodeToolStripMenuItem.Checked)
                    taxFilterString = taxLevelFullPaths[2];
                else if (ParentNodeToolStripMenuItem.Checked)
                    taxFilterString = taxLevelFullPaths[3];
                else

                    taxFilterString = TaxonomyFilter.ToString();

            }
            //levelLeafNodeToolStripMenuItem.Checked = false;//this is a  hack just to open one terminal node that has the attribute
            //if (string.IsNullOrEmpty(taxFilterString))

            var filteredTaxonomies = _allTaxonomies.Where(tax => tax.ToString().StartsWith(taxFilterString)).ToList();

            //Taxonomy Exclusions
            _currentFilteredTaxonomies =
                filteredTaxonomies.Cast<TaxonomyInfo>()
                    .Where(ti => !AryaTools.Instance.InstanceData.CurrentUser.TaxonomyExclusions.Contains(ti.ID))
                    .ToList();

            // create column collection
            // CurrentTaxonomies is null: use preselected taxonomies (attribute in tab)
            // One CurrentTaxonomy: use meta-attributes (single taxonomy in tab)
            // More than one CurrentTaxonomy: use CurrentTaxonomies (multiple taxonomies in tab)
            _columns = CurrentTaxonomies == null
                ? filteredTaxonomies
                : (CurrentTaxonomies.Count == 1
                    ? _allSchemati.Cast<object>()
                    : CurrentTaxonomies.OrderBy(tax => tax.ToString())).ToList();

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void ResetRowColumnCount()
        {
            var cursorInLastRow = sdgv.RowCount > 0 && sdgv.CurrentCellAddress.Y == sdgv.RowCount - 1;

            // set number of rows in the grid
            sdgv.RowCount = _rows.Count() + 1 + (_rows.Count == 0 || _rows.First() is Attribute ? 1 : 0);

            // determine number of columns to add (must be non-negative)
            var colsToAdd = _columns.Count - sdgv.ColumnCount + 1;

            // add the new columns
            Enumerable.Range(0, colsToAdd)
                .ForEach(
                    v =>
                        sdgv.Columns.Add(new DataGridViewColumn
                                         {
                                             FillWeight = 1,
                                             CellTemplate = new DataGridViewTextBoxCell()
                                         }));
            sdgv.ColumnCount = _columns.Count + 1;

            // redraw the grid
            sdgv.Invalidate();
            if (cursorInLastRow)
                sdgv.CurrentCell = sdgv[sdgv.CurrentCellAddress.X, sdgv.RowCount - 1];
        }

        private void InitLov()
        {
            SchemaInfo currentSchemaInfo = null;
            if (sdgv.CurrentCell != null)
            {
                SchemaAttribute schemaAttribute;
                Attribute attribute;
                TaxonomyInfo taxonomy;
                GetContext(GetColumnItem(sdgv.CurrentCell.ColumnIndex), GetRowItem(sdgv.CurrentCell.RowIndex),
                    out taxonomy, out attribute, out schemaAttribute);

                if (attribute == null && CurrentAttributes != null && CurrentAttributes.Count == 1)
                    attribute = CurrentAttributes[0];

                if (taxonomy == null && CurrentTaxonomies != null && CurrentTaxonomies.Count == 1)
                    taxonomy = CurrentTaxonomies[0];

                if (attribute != null && taxonomy != null)
                    currentSchemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
            }

            AryaTools.Instance.Forms.ListofValuesForm.SchemaInfo = currentSchemaInfo;
        }

        private bool InsertAttribute(Attribute newAttribute)
        {
            if (_rows.Contains(newAttribute, new KeyEqualityComparer<object>(att => att.ToString())))
            {
                sdgv.CurrentCell.Selected = false;
                sdgv.Rows.Cast<DataGridViewRow>()
                    .Select(p => p.Cells[0])
                    .First(
                        p =>
                            p.Value != null
                            && p.Value.ToString().Equals(newAttribute.AttributeName, StringComparison.OrdinalIgnoreCase))
                    .Selected = true;
                return false;
            }
            var index = sdgv.CurrentCellAddress.Y - 1;
            if (index < 0)
                index = 0;
            _rows.Insert(index, newAttribute);
            if (CurrentTaxonomies.Count == 1)
                SchemaAttribute.SetValue(CurrentTaxonomies[0], newAttribute, true, null, null);
            else
            {
                if (
                    MessageBox.Show(
                        string.Format("Do you want to map Attribute [{0}] to all Taxonomy nodes in this view?",
                            newAttribute), "Map all nodes?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    CurrentTaxonomies.ForEach(tax => SchemaAttribute.SetValue(tax, newAttribute, true, null, null));
            }
            return true;
        }

        private static int IsBlankValue(object val)
        {
            if (val == null)
                return 2; //Definitely NULL

            var stringValue = val.ToString();
            double dVal;
            return string.IsNullOrEmpty(stringValue) || stringValue.Equals(false.ToString())
                   || (double.TryParse(stringValue, out dVal) && dVal == 0.0)
                ? 1 //Pseudo-NULL
                : 0; // Not NULL
        }

        private void PopulateSampleValues()
        {
            if (sdgv.SelectedCells.Count == 0)
                return;

            var currentSelectedCells = sdgv.SelectedCells.Cast<DataGridViewCell>().ToList();

            if (currentSelectedCells.Any(cell => cell.RowIndex == 0 && cell.ColumnIndex == 0))
                return;

            var waitkey = FrmWaitScreen.ShowMessage("Populating Sample Values");

            foreach (var cell in currentSelectedCells)
            {
                var currentCell = cell;
                Attribute currentAttribute;
                TaxonomyInfo currentTaxonomy;
                SchemaAttribute currentSchematus;

                GetContext(GetColumnItem(currentCell.ColumnIndex), GetRowItem(currentCell.RowIndex), out currentTaxonomy,
                    out currentAttribute, out currentSchematus);

                currentSchematus = currentSchematus ?? CurrentSchematus;

                var currentTaxonomies = currentTaxonomy == null
                    ? CurrentTaxonomies ?? _currentFilteredTaxonomies ?? new List<TaxonomyInfo>()
                    : new List<TaxonomyInfo> { currentTaxonomy };

                var currentAttributes =
                    (currentAttribute == null
                        ? CurrentAttributes ?? _allAttributes.Cast<Attribute>().ToList()
                        : new List<Attribute> { currentAttribute }).Where(p => p.Equals(_blankAttribute) == false)
                        .ToList();

                foreach (var taxonomy in currentTaxonomies)
                {
                    foreach (var att in currentAttributes)
                    {
                        FrmWaitScreen.UpdateMessage(waitkey,
                            string.Format("Populating Sample Values - {0} - {1}", taxonomy.NodeName, att.AttributeName));
                        var attribute = att;
                        var sampleString = GetSampleValues(attribute, taxonomy);
                        var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));

                        if (currentSchematus != null
                            && currentSchematus.AttributeType == SchemaAttribute.SchemaAttributeType.Meta)
                            SchemaAttribute.SetValue(taxonomy, attribute, false, currentSchematus, sampleString, true);
                    }
                }
            }

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            FrmWaitScreen.HideMessage(waitkey);
            sdgv.Invalidate();
        }

        private static string GetSampleValues(Attribute attribute, TaxonomyInfo taxonomy)
        {
            var i = 0;
            var listSep = AryaTools.Instance.InstanceData.CurrentProject.ProjectPreferences.ListSeparator ?? "; ";

            var sampleList = AryaTools.Instance.InstanceData.Dc.ExecuteQuery<string>(@"SELECT TOP 5 ed.Value
                                    FROM EntityData ed
                                    INNER JOIN EntityInfo ei ON ed.Active = 1 AND ed.EntityID = ei.ID AND ed.AttributeID = {0}
                                    INNER JOIN SkuInfo si ON si.Active = 1 AND ei.SkuID = si.SkuID AND si.TaxonomyID = {1}
                                    GROUP BY ed.Value
                                    ORDER BY Count(*) DESC", attribute.ID, taxonomy.ID).ToList();

            string sampleString;
            if (sampleList.Any())
            {
                sampleString =
                    sampleList.OrderBy(p => p, new CompareForAlphaNumericSort())
                        .TakeWhile(p => (i += p.Length) < 4000)
                        .Aggregate((a, b) => string.Format("{0}{1}{2}", a, listSep, b));
            }
            else
                sampleString = "";
            return sampleString;
        }

        private bool ValidateMetaAttribute(object value, SchemaAttribute schematus)
        {
            // if there's no value, don't even bother to validate...
            if (value == null)
                return true;

            var test = value.ToString();
            if (test.Equals(EmptyMeta))
                return true;

            // get the schema info for this meta-attribute
            var schemaInfo = schematus.MetaSchemaAttribute.SchemaInfos.FirstOrDefault(a => a.TaxonomyID == Guid.Empty);
            if (schemaInfo == null)
                return true;

            // get any lovs related to this meta-attribute
            var lovs = schemaInfo.ListOfValues.Where(v => v.Active).Select(v => v.Value).ToList();
            if (lovs.Count == 0)
                return true;

            // return whether the value is in the lov.
            return lovs.Contains(test);
        }

        private void RefreshCurrentTab()
        {
            var changesSaved = AryaTools.Instance.SaveChangesIfNecessary(true, false);
            if (changesSaved == DialogResult.None || changesSaved == DialogResult.Yes)
            {
                InitData();
                PrepareGridView();
            }
        }

        private void RefresAttributesFromSkuData()
        {
            AryaTools.Instance.SaveChangesIfNecessary(true, false);

            AutoCompleteCache.Clear();

            if (CurrentTaxonomies != null)
                CurrentTaxonomies.ForEach(RefreshAttributesFromSkuData);

            if (AryaTools.Instance.SaveChangesIfNecessary(true, true) == DialogResult.No)
                return;

            InitData();
            PrepareGridView();
        }

        private static void RefreshAttributesFromSkuData(TaxonomyInfo taxonomy)
        {
            var waitKey =
                FrmWaitScreen.ShowMessage("Fetching Attributes from SKUs for " + taxonomy.TaxonomyData.NodeName);

            var attributesTobeCopied =
                taxonomy.SkuInfos.Where(si => si.Active)
                    .SelectMany(si => si.Sku.EntityInfos)
                    .SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active))
                    .Select(ed => ed.Attribute)
                    .Except(
                        taxonomy.SchemaInfos.Where(si => si.SchemaDatas.Any(sd => sd.Active)).Select(si => si.Attribute))
                    .ToList();

            FrmWaitScreen.UpdateMessage(waitKey, "Updating Schema for " + taxonomy.TaxonomyData.NodeName);

            foreach (var attribute in attributesTobeCopied)
            {
                var oldSchemaInfo = taxonomy.SchemaInfos.SingleOrDefault(p => p.AttributeID == attribute.ID);
                var si = oldSchemaInfo ?? new SchemaInfo { AttributeID = attribute.ID, TaxonomyID = taxonomy.ID };

                var sd = new SchemaData
                         {
                             NavigationOrder = 0,
                             DisplayOrder = 0,
                             InSchema = false,
                             DataType = "Text",
                             Active = true
                         };

                si.SchemaDatas.Add(sd);

                taxonomy.SchemaInfos.Add(si);
            }

            FrmWaitScreen.HideMessage(waitKey);
        }

        public static bool RenameAttribute(TaxonomyInfo taxonomy, Attribute fromAttribute, Attribute toAttribute)
        {
            var toAttExists = (from si in taxonomy.SkuInfos
                               where si.Active
                               from ei in si.Sku.EntityInfos
                               from ed in ei.EntityDatas
                               where ed.Active && ed.Attribute.Equals(toAttribute)
                               select ed).Any();
            if (toAttExists)
            {
                MessageBox.Show(
                    string.Format(
                        "Attribute [{0}] already exists in [{1}]. Cannot merge in Schema View. Correct in Sku View.",
                        toAttribute, taxonomy.NodeName));
                return false;
            }

            var waitkey =
                FrmWaitScreen.ShowMessage(string.Format("Looking for active entities in [{0}]",
                    taxonomy.TaxonomyData.NodeName));
            var entities = (from si in taxonomy.SkuInfos
                            where si.Active
                            from ei in si.Sku.EntityInfos
                            from ed in ei.EntityDatas
                            where ed.Active && ed.Attribute.Equals(fromAttribute)
                            select ed).ToList();

            FrmWaitScreen.UpdateMessage(waitkey,
                string.Format("Updating {0} active entities in [{1}]", entities.Count, taxonomy.TaxonomyData.NodeName));

            foreach (var entityData in entities)
            {
                var newEntityData = new EntityData
                                    {
                                        Attribute = toAttribute,
                                        Value = entityData.Value,
                                        Uom = entityData.Uom,
                                        Field1 = entityData.Field1,
                                        Field2 = entityData.Field2,
                                        Field3 = entityData.Field3,
                                        Field4 = entityData.Field4 //,
                                        //Field5orStatus = entityData.Field5OrStatus
                                    };
                entityData.Active = false;
                entityData.EntityInfo.EntityDatas.Add(newEntityData);
            }

            FrmWaitScreen.UpdateMessage(waitkey, "Copying Meta-attributes");
            var fromSchemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(fromAttribute));
            if (fromSchemaInfo == null)
            {
                FrmWaitScreen.HideMessage(waitkey);
                return true;
            }

            var newAttDaList = toAttribute.DerivedAttributes.Select(p => p.ID).ToList();
            fromAttribute.DerivedAttributes.Where(p => !newAttDaList.Contains(p.ID)).ToList().ForEach(a =>
                                                                                                      {
                                                                                                          var da =
                                                                                                              new DerivedAttribute
                                                                                                              {
                                                                                                                  AttributeID
                                                                                                                      =
                                                                                                                      toAttribute
                                                                                                                      .ID,
                                                                                                                  Expression
                                                                                                                      =
                                                                                                                      a
                                                                                                                      .Expression,
                                                                                                                  MaxResultLength
                                                                                                                      =
                                                                                                                      a
                                                                                                                      .MaxResultLength,
                                                                                                                  TaxonomyID
                                                                                                                      =
                                                                                                                      a
                                                                                                                      .TaxonomyID
                                                                                                              };
                                                                                                          toAttribute
                                                                                                              .DerivedAttributes
                                                                                                              .Add(da);
                                                                                                      });

            var toBeRemovedDAs = fromAttribute.DerivedAttributes.Where(p => p.TaxonomyID == taxonomy.ID).ToList();

            if (toBeRemovedDAs.Count > 0)
                AryaTools.Instance.InstanceData.Dc.DerivedAttributes.DeleteAllOnSubmit(toBeRemovedDAs);

            var fromSchemaData = fromSchemaInfo.SchemaData;
            SchemaAttribute.SetValue(taxonomy, toAttribute, true, null, fromSchemaData);
            SchemaAttribute.SecondarySchemati.ForEach(schematus =>
                                                      {
                                                          var newValue = SchemaAttribute.GetValue(taxonomy,
                                                              fromAttribute, schematus);
                                                          if (newValue != null)
                                                          {
                                                              SchemaAttribute.SetValue(taxonomy, toAttribute, true,
                                                                  schematus, newValue);
                                                          }
                                                      });

            var toSchemaInfo = taxonomy.SchemaInfos.Single(a => a.AttributeID == toAttribute.ID);
            var oldListofValues = fromSchemaInfo.ListOfValues.Where(a => a.Active).ToList();
            foreach (var lov in oldListofValues)
            {
                lov.Active = false;
                toSchemaInfo.AddLov(lov.Value, lov.ParentValue, null, null, null);
            }

            SchemaAttribute.UnmapNodeAttribute(taxonomy, fromAttribute);

            if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(fromAttribute))
                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[fromAttribute].Clear();

            if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(toAttribute))
                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[toAttribute].Clear();

            FrmWaitScreen.HideMessage(waitkey);
            return true;
        }

        private string RenameAttribute(int columnIndex, int rowIndex, string newAttributeName)
        {
            if (string.IsNullOrEmpty(newAttributeName) || CurrentTaxonomies == null || CurrentTaxonomies.Count == 0)
                return null;

            var oldAttribute = (Attribute)sdgv[columnIndex, rowIndex].Value;
            var newAttribute = Attribute.GetAttributeFromName(newAttributeName, true,
                oldAttribute.Type == AttributeTypeEnum.Sku ? AttributeTypeEnum.NonMeta : oldAttribute.Type);
            if (oldAttribute.Equals(newAttribute))
            {
                if (!newAttributeName.Equals(newAttribute.AttributeName)
                    && MessageBox.Show(
                        @"This will change case for the attribute across the entire project. Are you sure?",
                        @"Change case for attribute name", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    newAttribute.AttributeName = newAttributeName;

                return null;
            }

            var results = oldAttribute.Equals(_blankAttribute)
                ? InsertAttribute(newAttribute).AsEnumerable().ToList()
                : CurrentTaxonomies.Select(tax => RenameAttribute(tax, oldAttribute, newAttribute)).ToList();

            if (!_rows.Contains(newAttribute) && results.Any(r => r))
                _rows.Insert(_rows.IndexOf(oldAttribute), newAttribute);

            if (!oldAttribute.Equals(_blankAttribute) && results.All(r => r))
                _rows.Remove(oldAttribute);

            ResetRowColumnCount();

            return null;
        }

        private void SortDataGridColumnItems(object column)
        {
            try
            {
                sdgv.EndEdit();

                //if(column == null)
                //{
                //    DataGridViewCellCollection headerCells = sdgv.Rows[0].Cells;
                //    for (int i = 1; i < headerCells.Count; i++)
                //    {
                //        var cell = (DataGridViewTextBoxCell)headerCells[i];
                //        if (cell.Value != null && cell.Value.ToString() == "Navigation Order")
                //        {
                //            column = sdgv[i, 0].Value;
                //            break;
                //        }
                //    }
                //}
                //object colItem = sdgv[e.ColumnIndex, 0].Value;
                var sortAscending = true;
                if (_lastSortedAscendingBy != null && _lastSortedAscendingBy.Equals(column))
                {
                    sortAscending = false;
                    _lastSortedAscendingBy = null;
                }
                else
                    _lastSortedAscendingBy = column;
                var rows = (from row in _rows
                            let currentIndex = _rows.IndexOf(row)
                            let item = GetValue(column, row)
                            let isBlank = IsBlankValue(item)
                            select new { row, currentIndex, item, isBlank }).ToList();

                if (sortAscending)
                {
                    _rows =
                        rows.OrderBy(i => i.isBlank)
                            .ThenBy(i => i.item)
                            .ThenBy(i => i.currentIndex)
                            .Select(i => i.row)
                            .ToList();
                }
                else
                {
                    _rows =
                        rows.OrderBy(i => i.isBlank)
                            .ThenByDescending(i => i.item)
                            .ThenBy(i => i.currentIndex)
                            .Select(i => i.row)
                            .ToList();
                }

                sdgv.Invalidate();
            }
            catch (Exception)
            {
            }
        }

        private object GetColumnToSort(string sortCategory)
        {
            var headerCells = sdgv.Rows[0].Cells;
            object column = headerCells[0];
            // column = headerCells.AsEnumerable().Select(c => c).OfType<DataGridViewTextBoxCell>().Where(gc => gc.Value.ToString() == sortCategory);
            //headerCells.AsEnumerable().Select(c => c).OfType<DataGridViewTextBoxCell>().Where(gc => gc.Value.ToString() == sortCategory);
            for (var i = 1; i < headerCells.Count; i++)
            {
                //headerCells.AsEnumerable().Select(c => c).OfType<DataGridViewTextBoxCell>().Where(gc => gc.Value.ToString() == sortCategory);
                var cell = (DataGridViewTextBoxCell)headerCells[i];
                if (cell.Value != null && cell.Value.ToString() == sortCategory)
                {
                    column = sdgv[i, 0].Value;
                    break;
                }
            }

            return column;
        }

        private void ShowHideOptions()
        {
            filterNodesToolStripMenuItem.Visible = TaxonomyFilter != null;
            viewAdditionalNodesToolStripMenuItem.Visible = CurrentTaxonomies != null && CurrentTaxonomies.Count > 1;
            removeSelectedNodesFromViewToolStripMenuItem.Visible = CurrentTaxonomies != null
                                                                   && CurrentTaxonomies.Count > 1;
        }

        private void TryOrderRanks()
        {
            if (!AutoOrderRanks || CurrentTaxonomies == null)
                return;
            CurrentTaxonomies.ForEach(tax => SchemaAttribute.AutoOrderRanks(tax, null));
            sdgv.Invalidate();
            sdgv.BringToFront();
        }

        private bool UnmapAttribute(Attribute attribute, bool? unmapAttributesWithSkus)
        {
            var hasValues = (from tax in AryaTools.Instance.InstanceData.Dc.TaxonomyInfos
                             where CurrentTaxonomies.Contains(tax)
                             from si in tax.SkuInfos
                             where si.Active
                             from ei in si.Sku.EntityInfos
                             from ed in ei.EntityDatas
                             where ed.Active && ed.Attribute.Equals(attribute)
                             select ed).Any();
            //CurrentTaxonomies.SelectMany(tax => tax.SkuInfos.Where(si => si.Active), (tax, si) => si.Sku).SelectMany
            //(sku => sku.EntityInfos).SelectMany(ei => ei.EntityDatas).Where(
            //ed => ed.Active && ed.Attribute.Equals(attribute)).Any();

            if (hasValues)
            {
                if (unmapAttributesWithSkus == null)
                {
                    unmapAttributesWithSkus =
                        MessageBox.Show(
                            @"This attribute has values associated to SKUs. This will only unmap the attribute from Schema. It will NOT delete values. Do you want to continue?",
                            @"Unmap Attribute containing SKU Values", MessageBoxButtons.YesNo) == DialogResult.Yes;
                }
                if (!(bool)unmapAttributesWithSkus)
                    return false;
            }

            CurrentTaxonomies.ForEach(ti => SchemaAttribute.UnmapNodeAttribute(ti, attribute));

            _rows.Remove(attribute);

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            ResetRowColumnCount();
            return true;
        }

        private void UpdateAttributeOptions()
        {
            //attributeToolStripMenuItem.Enabled = CurrentTaxonomies != null;

            //var rowHeaders =
            //    sdgv.SelectedCells.Cast<DataGridViewCell>().Select(cell => sdgv[0, cell.RowIndex].Value).ToList();

            //bool showUnmapAttribute = false;
            //if (rowHeaders.Count == 1 && rowHeaders[0] is Attribute)
            //    showUnmapAttribute = true;

            //unmapSelectedAttributesToolStripMenuItem.Enabled = showUnmapAttribute;

            unmapSelectedAttributesToolStripMenuItem.Enabled = CurrentTaxonomies != null && CurrentAttributes == null;
        }

        private void UpdateCloneOptions(int columnIndex, int rowIndex)
        {
            var primaryCondition = rowIndex > 0 && columnIndex > 0 // && sdgv.SelectedCells.Count == 1 
                                   && ((CurrentTaxonomies != null && CurrentTaxonomies.Count > 1)
                                       || (CurrentAttributes != null && sdgv.ColumnCount > 2))
                                   && sdgv.CurrentCell.Value != null;

            cloneCurrentValueOnlyToolStripMenuItem.Enabled = primaryCondition;

            object rowHeaderValue;
            cloneAllSchematiAndLovToolStripMenuItem.Enabled =
                cloneAllSchematiWithEmptyranksToolStripMenuItem.Enabled =
                    cloneCustomOptionsToolStripMenuItem.Enabled =
                        primaryCondition && (rowHeaderValue = sdgv[0, sdgv.CurrentCellAddress.Y].Value) != null
                        && rowHeaderValue is Attribute;
        }

        private void UpdateOptions()
        {
            var rowIndex = sdgv.CurrentCellAddress.Y;
            var columnIndex = sdgv.CurrentCellAddress.X;

            UpdateAttributeOptions();
            UpdateCloneOptions(columnIndex, rowIndex);

            if (sdgv.SelectedCells.Count == 1 && CurrentTaxonomies != null && CurrentTaxonomies.Count == 1)
            {
                var selectedAttribute =
                    sdgv.SelectedCells.Cast<DataGridViewCell>()
                        .Select(cell => cell.OwningRow.Cells[0].Value)
                        .OfType<Attribute>()
                        .FirstOrDefault();

                if (selectedAttribute != null && selectedAttribute.AttributeType == AttributeTypeEnum.Derived.ToString())
                    calculatedAttributeToolStripMenuItem.Enabled = true;
                else
                    calculatedAttributeToolStripMenuItem.Enabled = false;
            }
            else
                calculatedAttributeToolStripMenuItem.Enabled = false;
        }

        private void ViewAdditionalNodes()
        {
            if (AryaTools.Instance.Forms.TreeForm.DialogResult != DialogResult.OK)
                return;
            var waitkey = FrmWaitScreen.ShowMessage("Generating list of nodes to open");
            var nodesToLoad = new List<TaxonomyInfo>();
            AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomies.ForEach(
                tax => nodesToLoad.AddRange(tax.GetNodes(AryaTools.Instance.Forms.TreeForm.IncludeChildren, true)));
            AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomies.ForEach(tax =>
                                                                                         {
                                                                                             if (
                                                                                                 !CurrentTaxonomies
                                                                                                     .Contains(tax))
                                                                                             {
                                                                                                 CurrentTaxonomies.Add(
                                                                                                     tax);
                                                                                             }
                                                                                         });
            InitRowsAndColumns();
            PrepareGridView();
            FrmWaitScreen.HideMessage(waitkey);
        }

        private DerivedAttribute GetDerivedAttribute(Attribute sourceAttribute, TaxonomyInfo sourceTaxonomyInfo)
        {
            if (sourceTaxonomyInfo == null)
                return null;

            var calculatedAttribute =
                sourceAttribute.DerivedAttributes.FirstOrDefault(
                    da => da.TaxonomyInfo != null && da.TaxonomyID == sourceTaxonomyInfo.ID);

            if (calculatedAttribute == null && sourceAttribute.DerivedAttributes.Any(da => da.TaxonomyInfo == null))
                calculatedAttribute = sourceAttribute.DerivedAttributes.First(da => da.TaxonomyInfo == null);

            return calculatedAttribute
                   ?? GetDerivedAttribute(sourceAttribute, sourceTaxonomyInfo.TaxonomyData.ParentTaxonomyInfo);
        }

        #endregion

        private void sdgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (GetColumnItem(e.Column.Index) is SchemaAttribute)
                SetColumnWidth(GetColumnItem(e.Column.Index) as SchemaAttribute, e.Column.Width);
        }      
       

        private void ProcessSpellCheckForLovEntireColumn()
        {
            throw new NotImplementedException();
        }

        private void SpellCheckEntireView()
        {
            List<ISpell> values = new List<ISpell>();
            for (int columnIndex = 1; columnIndex < sdgv.ColumnCount; columnIndex++)
            {
                for (int rowIndex = 1; rowIndex < sdgv.RowCount; rowIndex++)
                {
                    ISpell currentValue = sdgv[columnIndex, rowIndex].Value as ISpell;
                    if (currentValue != null)
                        values.Add(currentValue);
                }
            }
            List<int> rowIndices = new List<int>();
            for (int i = 1; i < sdgv.RowCount; i++)
            {
                rowIndices.Add(i);
            }
            values.AddRange(GetISpellsForRowIndicesInAttribute(rowIndices));
            //add lov for columns
            values.AddRange(GetLovsInColumn());
            DisplaySpellCheck(values, Arya.Properties.Resources.SpellCheckContextMessageForEntireView);
        }

        private void SpellCheckForColumn(int columnIndex)
        {
            List<ISpell> values = new List<ISpell>();
            var columnHeaderValue = sdgv[columnIndex, 0].Value;
            if ((columnHeaderValue != null && columnHeaderValue.ToString() == SchemaAttribute.SpecialSchematusListOfValues) || (CurrentSchematus != null && CurrentSchematus.ToString() == SchemaAttribute.SpecialSchematusListOfValues))
            {
                values = GetLovsInColumn();
            }
            else if (columnIndex == 0)
            {
                List<int> rowIndices = new List<int>();
                for (int i = 1; i < sdgv.RowCount -1; i++)
                {
                    rowIndices.Add(i);
                }
                values = GetISpellsForRowIndicesInAttribute(rowIndices);
                
            }
            else
            {
                for (int i = 1; i < sdgv.RowCount; i++)
                {
                    ISpell currentValue = sdgv[columnIndex, i].Value as ISpell;
                    if (currentValue != null)
                        values.Add(currentValue);

                }                
            }
            DisplaySpellCheck(values, Arya.Properties.Resources.SpellCheckContextMessageForEntireColumn);
        }
        private int GetLovColumnIndex()
        {
            int lovColumnIndex  = -1;
            for (int i = 0; i < sdgv.ColumnCount; i++)
            {
                var columnHeaderValue = sdgv[i, 0].Value;
                if (columnHeaderValue != null && columnHeaderValue.ToString() == SchemaAttribute.SpecialSchematusListOfValues)
                {
                    lovColumnIndex = i;
                    break;
                }
            }
            return lovColumnIndex;
        }
        private List<ISpell> GetLovsInColumn()
        {
            List<ISpell> values = new List<ISpell>();
           // if(CurrentSchematus)
            if (CurrentSchematus != null && SchemaAttribute.SpecialSchematusListOfValues == CurrentSchematus.ToString())
            {
                for (int columnCount = 1; columnCount < sdgv.ColumnCount; columnCount++)
                {
                    var colItem = GetColumnItem(columnCount);
                    for (int rowCount = 1; rowCount < sdgv.RowCount; rowCount++)
                    {
                        var rowItem = GetRowItem(rowCount);
                        TaxonomyInfo taxonomy;
                        Attribute attribute;
                        SchemaAttribute schematus;
                        GetContext(colItem, rowItem, out taxonomy, out attribute, out schematus);
                        if (taxonomy != null)
                        {
                            var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute != null && si.Attribute.Equals(attribute));
                            if (schemaInfo != null)
                            {
                                var listOfValues = schemaInfo.ListOfValues.Where(lov => lov.Active).Select(lov => lov).ToList();
                                foreach (var itemlov in listOfValues)
                                {
                                    ListofValueWrapper newWrpar = new ListofValueWrapper(itemlov);
                                    values.Add(newWrpar);
                                }
                            }

                        }
                    }
                }

            }
            else 
            {
                var colItem = GetColumnItem(GetLovColumnIndex());//multiple node in one view
                for (int i = 1; i < sdgv.RowCount; i++)
                {
                    var rowItem = GetRowItem(i);
                    TaxonomyInfo taxonomy;
                    Attribute attribute;
                    SchemaAttribute schematus;
                    GetContext(colItem, rowItem, out taxonomy, out attribute, out schematus);
                    if (taxonomy != null)
                    {
                        var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute != null && si.Attribute.Equals(attribute));
                        if (schemaInfo != null)
                        {
                            var listOfValues = schemaInfo.ListOfValues.Where(lov => lov.Active).Select(lov => lov).ToList();
                            foreach (var itemlov in listOfValues)
                            {
                                ListofValueWrapper newWrpar = new ListofValueWrapper(itemlov);
                                values.Add(newWrpar);
                            }
                        }

                    }
                }
            }
            //var test = CurrentSchematus;
           
            return values; 
        }

        private void SpellCheckSelectedCell()
        {
            List<ISpell> values = new List<ISpell>();
            var columnHeaderValue = sdgv[sdgv.CurrentCell.ColumnIndex, 0].Value;
            if (sdgv.CurrentCell.ColumnIndex == GetLovColumnIndex() || (CurrentSchematus != null && CurrentSchematus.ToString() == SchemaAttribute.SpecialSchematusListOfValues))
            {
                values = GetLovsSelectedCells();
            }
            else if (sdgv.CurrentCell.ColumnIndex == 0 )//porcess attribute column
            {
                values = GetAttributesSelectedCells();
            }
            else
            {
                for (int i = 0; i < sdgv.SelectedCells.Count; i++)
                {
                    ISpell currentValue = sdgv.SelectedCells[i].Value as ISpell;
                    if (currentValue != null)
                        values.Add(currentValue);
                }
            }

            DisplaySpellCheck(values, values.Count +" "+ Arya.Properties.Resources.SpellCheckContextMessageForSelectedCells);
        }

        private List<ISpell> GetAttributesSelectedCells()
        {
           
            List<int> rowIndices = new List<int>();
            for (int i = 0; i < sdgv.SelectedCells.Count; i++)
            {
                if (sdgv.SelectedCells[i].RowIndex != sdgv.RowCount - 1)
                {
                    rowIndices.Add(sdgv.SelectedCells[i].RowIndex);
                }
                
            }
            return GetISpellsForRowIndicesInAttribute(rowIndices);
           // return values;
        }
         
        private List<ISpell> GetISpellsForRowIndicesInAttribute(List<int> rowIndices)
        {
            //var rowHeader = sdgv[0, rowIndex].Value;
            var colHeader = sdgv[0, 0].Value;

            List<ISpell> values = new List<ISpell>();
            var colItem = GetColumnItem(sdgv.CurrentCell.ColumnIndex);
            for (int i = 0; i < rowIndices.Count; i++)
            {
                var rowHeader = sdgv[0, rowIndices[i]].Value;
                TaxonomyInfo taxonomy = null;
                Attribute attribute= null;
                if (rowHeader is Attribute)
                {
                    attribute = (Attribute)rowHeader;   
                    taxonomy = colHeader is TaxonomyInfo
                        ? (TaxonomyInfo)colHeader
                        : CurrentTaxonomies.Count == 1 ? CurrentTaxonomies[0] : null;
                }                
                values.Add(new AttributeSpellCheckEntity( taxonomy,attribute));
            }
            return values;
        }

        private List<ISpell> GetLovsSelectedCells()
        {
            List<ISpell> values = new List<ISpell>();
            var colItem = GetColumnItem(sdgv.SelectedCells[0].ColumnIndex);
            for (int i = 0; i < sdgv.SelectedCells.Count; i++)
            {
                var rowItem = GetRowItem(sdgv.SelectedCells[i].RowIndex);
                TaxonomyInfo taxonomy;
                Attribute attribute;
                SchemaAttribute schematus;
                GetContext(colItem, rowItem, out taxonomy, out attribute, out schematus);
                if (taxonomy != null )
                {
                    var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute != null && si.Attribute.Equals(attribute));
                    if (schemaInfo != null)
                    {
                        var listOfValues = schemaInfo.ListOfValues.Where(lov => lov.Active).Select(lov => lov).ToList();
                        foreach (var itemlov in listOfValues)
                        {
                            ListofValueWrapper newWrpar = new ListofValueWrapper(itemlov);
                            values.Add(newWrpar);
                        }
                    }
                    
                }
            } 
            return values;
        }

        private void DisplaySpellCheck(List<ISpell> values, string contextMsg)
        {
            try
            {
                var spellCheckForm = AryaTools.Instance.Forms.SpellCheckForm;
                if (values.Count() > 0)
                {
                    spellCheckForm.UpdateSpellCheckView(values, contextMsg);
                    spellCheckForm.InvalidateMethod = InvalidateEdgv;
                    var result = spellCheckForm.ShowDialog();
                }
            }
            catch (ObjectDisposedException ex)
            {
                //do nothing
            }
        }
        void InvalidateEdgv()
        {
            InitRowsAndColumns();
            PrepareGridView();
        }

        private void checkSpellingSelectedRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentColumnindex = sdgv.CurrentCell.ColumnIndex;
            if (sdgv.SelectedCells != null && sdgv.SelectedCells.Count > 0)
            {
                SpellCheckSelectedCell();
            }
            //GetAllCellsInColumn
            DialogResult result = MessageBox.Show(this, "Would you like to check the spelling of the entire column?", "Spell Check", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SpellCheckForColumn(currentColumnindex);
                result = MessageBox.Show(this, "Would you like to check the spelling of the entire tab?", "Spell Check", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SpellCheckEntireView();
                }

            }
        }

        private void checkSpellingSchemaViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpellCheckEntireView();
        }
    }
    public class AttributeSpellCheckEntity : ISpell
    {
        private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                return (KeyValuePair<bool, string>)_updatable;
            }
        }
        private Attribute Attribute { get; set; }
        private TaxonomyInfo Taxonomy { get; set; }
        public AttributeSpellCheckEntity(TaxonomyInfo taxonomy, Attribute currentAttribute)
        {
            Attribute = currentAttribute;
            Taxonomy = taxonomy;
        }

        string ISpell.GetType()
        {
            return Attribute.GetType().ToString();
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
             var   _propertyNameValue = new Dictionary<string, string>();
                _propertyNameValue = new Dictionary<string, string>();
                if (Attribute != null)
                {
                    _propertyNameValue.Add("AttributeName", Attribute.AttributeName);
                }
                
            
            return _propertyNameValue;
            
        }

        Guid ISpell.GetId()
        {
            Guid id = Guid.Empty;
            if (Attribute != null)
            {
                id =  Attribute.ID;
            }
            return id;
        }

        ISpell ISpell.SetValue(string propertyName, string value)
        {
            if (propertyName.ToLower() == "attributename")
            {
                Attribute newAttr = Attribute.GetAttributeFromName(value,true);
                SchemaDataGridView.RenameAttribute(this.Taxonomy, this.Attribute, newAttr);
            }
            return this;
        }

        string ISpell.GetLocation()
        {
            return "Attribute Name: " + Attribute.AttributeName;
        }
    }
}