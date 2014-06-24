using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Attribute = Arya.Data.Attribute;

namespace Arya.UserControls
{
    public partial class MetaAttributeDataGridView : UserControl
    {
        #region Fields

        private const string DisplayOrder = "Display Order";
        private const string Active = "Active";
        private const string ListOfValues = "List Of Values";
        private List<Attribute> _columns;
        private object _lastSortedAscendingBy;
        private Framework.Data.AryaDb.AttributeTypeEnum _metaAttType;
        private Framework.Data.AryaDb.AttributeTypeEnum _metaMetaAttType;
        private MetaTypeEnum _metaType;
        private List<Attribute> _rows;

        #endregion

        #region Properties

        public MetaTypeEnum MetaType
        {
            get { return _metaType; }

            set
            {
                _metaType = value;
                switch (value)
                {
                    case MetaTypeEnum.Schema:
                    {
                        _metaAttType = Framework.Data.AryaDb.AttributeTypeEnum.SchemaMeta;
                        _metaMetaAttType = Framework.Data.AryaDb.AttributeTypeEnum.SchemaMetaMeta;
                        break;
                    }
                    case MetaTypeEnum.Taxonomy:
                    {
                        _metaAttType = Framework.Data.AryaDb.AttributeTypeEnum.TaxonomyMeta;
                        _metaMetaAttType = Framework.Data.AryaDb.AttributeTypeEnum.TaxonomyMetaMeta;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Constructors

        public MetaAttributeDataGridView() { InitializeComponent(); }

        #endregion

        #region Event Handlers

        private void dgvMetas_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex != 0 && e.ColumnIndex != 0 && _columns[e.ColumnIndex - 1].AttributeName == ListOfValues)
            {
                AryaTools.Instance.Forms.ListofValuesForm.SchemaInfo =
                    _rows[e.RowIndex - 1].SchemaInfos.FirstOrDefault();
                AryaTools.Instance.Forms.ListofValuesForm.ShowPopulateButton = false;
                AryaTools.Instance.Forms.ListofValuesForm.ShowDialog();
                e.Cancel = true;
            }
        }


        private void dgvMetas_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == dgvMetas.RowCount - 1)
            {
                e.Value = e.ColumnIndex == 0 ? "*" : null;
                return;
            }

            var rowItem = GetRowItem(e.RowIndex);
            var colItem = GetColumnItem(e.ColumnIndex);
            e.Value = GetValue(colItem, rowItem);
        }

        private void dgvMetas_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            string result;
            var columnIndex = e.ColumnIndex;
            var rowIndex = e.RowIndex;
            var value = e.Value;
            SetValue(columnIndex, rowIndex, value);

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            dgvMetas.InvalidateCell(columnIndex, rowIndex);
            AutoResizeColumns(columnIndex);
        }

        private void dgvMetas_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // finish whatever editing is going on
            dgvMetas.EndEdit();

            // get the current column - force to string if null
            const string header = "Header";
            var colItem = dgvMetas[e.ColumnIndex, 0].Value ?? header;

            // determine whether this is an ascending or descending sort
            var sortAscending = true;
            if (_lastSortedAscendingBy != null && _lastSortedAscendingBy.Equals(colItem))
            {
                sortAscending = false;
                _lastSortedAscendingBy = null;
            }
            else
                _lastSortedAscendingBy = colItem;

            // capture members of row collection and add indexes for sorting
            // if column item is the string, force back to null
            var rows = (from row in _rows
                let currentIndex = _rows.IndexOf(row)
                let item = GetValue(colItem.Equals(header) ? null : (Attribute) colItem, row)
                let isBlank = IsBlankValue(item)
                select new {row, currentIndex, item, isBlank}).ToList();

            // sort the rows
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

            // redraw the grid
            dgvMetas.Invalidate();
        }

        #endregion

        #region Methods

        public void Init()
        {
            InitData();
            PrepareGridView();
        }

        private void InitData()
        {
            var emptyGuid = new Guid();

            _rows = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                where
                    att.ProjectID.Equals(AryaTools.Instance.InstanceData.CurrentProject.ID)
                    && att.AttributeType.Equals(_metaAttType.ToString())
                let displayRank = (from si in att.SchemaInfos
                    where si.TaxonomyID.Equals(emptyGuid)
                    from sd in si.SchemaDatas
                    where sd.Active
                    select sd.DisplayOrder).FirstOrDefault()
                orderby displayRank, att.AttributeName
                select att).ToList();

            _columns = (from att in AryaTools.Instance.InstanceData.Dc.Attributes
                where
                    att.ProjectID.Equals(AryaTools.Instance.InstanceData.CurrentProject.ID)
                    && att.AttributeType.Equals(_metaMetaAttType.ToString())
                let displayRank = (from si in att.SchemaInfos
                    where si.TaxonomyID.Equals(emptyGuid)
                    from sd in si.SchemaDatas
                    where sd.Active
                    select sd.DisplayOrder).FirstOrDefault()
                orderby displayRank, att.AttributeName
                select att).ToList();
        }

        private void PrepareGridView()
        {
            //for some reason, this is getting triggered when clearing the columns.
            dgvMetas.CellValueNeeded -= dgvMetas_CellValueNeeded;
            dgvMetas.Columns.Clear();
            dgvMetas.Rows.Clear();
            dgvMetas.CellValueNeeded += dgvMetas_CellValueNeeded;

            SetRowCount();
            SetColumnCount();

            // set cosmetics for columns
            for (var i = 0; i < _columns.Count; i++)
            {
                if (_columns[i].AttributeName.Equals(Active))
                    dgvMetas.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleSchemaBoolean;
                else if (_columns[i].AttributeName.Equals(ListOfValues))
                    dgvMetas.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleGreyLovRegular;
                else if (_columns[i].AttributeName.Equals(DisplayOrder))
                    dgvMetas.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleSchemaDecimal;
                else
                    dgvMetas.Columns[i + 1].DefaultCellStyle = DisplayStyle.CellStyleGreyItalic;
            }

            // set specifics for header row and column
            dgvMetas.Rows[0].Frozen = true;
            dgvMetas.Rows[0].ReadOnly = true;
            dgvMetas.Rows[0].DefaultCellStyle = DisplayStyle.CellStyleFirstRow;

            dgvMetas.Columns[0].Frozen = true;
            dgvMetas.Columns[0].DefaultCellStyle = DisplayStyle.CellStyleAttributeColumn;
            dgvMetas.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            // set cosmetics for rows
            for (var i = 0; i <= _rows.Count; i++)
            {
                if (i % 2 != 0)
                    dgvMetas.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;
            }
            
            AutoResizeColumns();
        }

        private void SetRowCount()
        {
            // set number of rows in the grid
            dgvMetas.RowCount = _rows.Count + 2 + (_rows.Count == 0 ? 1 : 0);

            // redraw the grid
            dgvMetas.Invalidate();
        }

        private void SetColumnCount()
        {
            // determine number of columns to add (must be non-negative)
            var colsToAdd = _columns.Count + 1;

            // add the new columns
            Enumerable.Range(0, colsToAdd)
                .ForEach(
                    v =>
                        dgvMetas.Columns.Add(new DataGridViewColumn
                                             {
                                                 SortMode = DataGridViewColumnSortMode.Automatic,
                                                 CellTemplate = new DataGridViewTextBoxCell()
                                             }));
            dgvMetas.ColumnCount = _columns.Count + 1;

            // redraw the grid
            dgvMetas.Invalidate();
        }

        private Attribute GetRowItem(int rowIndex)
        {
            // map row index to corresponding item in row collection
            return rowIndex > _rows.Count ? null : rowIndex > 0 && rowIndex <= _rows.Count ? _rows[rowIndex - 1] : null;
        }

        private Attribute GetColumnItem(int columnIndex)
        {
            // map column index to corresponding item in column collection
            return columnIndex > 0 ? _columns[columnIndex - 1] : null;
        }

        private object GetValue(Attribute colItem, Attribute rowItem)
        {
            // return headers if on the edges of the grid
            if (colItem == null)
                return rowItem;
            if (rowItem == null)
                return colItem;

            object returnVal = null;
            var attName = colItem.AttributeName;
            switch (attName)
            {
                case DisplayOrder:
                {
                    var si = rowItem.SchemaInfos.FirstOrDefault(s => s.TaxonomyID.Equals(Guid.Empty));
                    returnVal = si == null ? null : (object) si.SchemaData.DisplayOrder;
                    break;
                }

                case Active:
                {
                    var si = rowItem.SchemaInfos.FirstOrDefault(s => s.TaxonomyID.Equals(Guid.Empty));
                    returnVal = si == null ? null : (object) si.SchemaData.InSchema;
                    break;
                }

                case ListOfValues:
                {
                    var schemaInfo = rowItem.SchemaInfos.FirstOrDefault(s => s.TaxonomyID.Equals(Guid.Empty));
                    if (schemaInfo != null)
                    {
                        var listOfValues = schemaInfo.ListOfValues.Where(lov => lov.Active).Select(lov => lov).ToList();
                        if (listOfValues.Any())
                        {
                            var listSep =
                                AryaTools.Instance.InstanceData.CurrentProject.ProjectPreferences.ListSeparator
                                ?? "; ";
                            returnVal =
                                listOfValues.OrderBy(val => val.DisplayOrder)
                                    .ThenBy(val => val.Value)
                                    .Select(val => val.Value)
                                    .Aggregate((current, lov) => current + listSep + lov);
                        }
                    }
                    break;
                }

                default:
                {
                    // get intersection of row and column attributes
                    var ami = rowItem.AttributeMetaInfos.FirstOrDefault(a => a.MetaAttribute == colItem);
                    if (ami != null)
                    {
                        // get associated data, if it exists
                        var amd = ami.AttributeMetaDatas.FirstOrDefault(a => a.Active);
                        if (amd != null)
                            returnVal = amd.Value;
                    }
                    break;
                }
            }

            return returnVal;
        }

        private string SetValue(int colIndex, int rowIndex, object value)
        {
            // if cursor is in final row, do add
            if (rowIndex > _rows.Count)
            {
                if (colIndex == 0)
                    AddMetaAttribute(value);
            }
                // if cursor is in row header, rename meta-attribute
                // if rename fails, don't bother resetting the cache
            else if (colIndex == 0)
            {
                if (!RenameMetaAttribute(rowIndex, value))
                    return null;
            }
            else
                // otherwise we're adding a new value (add/change)
                EditMetaAttribute(colIndex, rowIndex, value);

            // flush the cache
            SchemaAttribute.SecondarySchemati = null;

            return null;
        }

        private bool RenameMetaAttribute(int rowIndex, object value)
        {
            var rowName = GetRowItem(rowIndex).AttributeName;

            // if the newname exactly matches the old name, do nothing
            if (rowName.Equals(value.ToString()))
                return false;

            // if the new name is a case change or completely new, make the change
            if (rowName.ToLower().Equals(value.ToString().ToLower())
                || !_rows.Any(row => row.AttributeName.ToLower().Equals(value.ToString().ToLower())))
            {
                // make the change
                _rows[rowIndex - 1].AttributeName = value.ToString();
            }
            else
            {
                MessageBox.Show("This meta-attribute already exists.");
                return false;
            }
            dgvMetas.Invalidate();

            return true;
        }

        private void EditMetaAttribute(int colIndex, int rowIndex, object value)
        {
            if(colIndex == 0)//Schema meta /tax meta column
                return;
            var rowItem = GetRowItem(rowIndex);
            var colItem = GetColumnItem(colIndex);

            // changing display order
            if (colItem.AttributeName.Equals(DisplayOrder))
            {
                decimal order;
                Decimal.TryParse((string)value, out order);
                //if (Decimal.TryParse((string) value, out order))
                {
                    var smi = rowItem.SchemaInfos.FirstOrDefault(si => si.TaxonomyID == Guid.Empty);
                    if (smi == null)
                    {
                        rowItem.SchemaInfos.Add(new SchemaInfo
                                                {
                                                    TaxonomyID = Guid.Empty,
                                                    Attribute = colItem,
                                                    SchemaDatas = {new SchemaData {DisplayOrder = order}}
                                                });
                    }
                    else
                    {
                        var schemaData = smi.SchemaData;
                        if (schemaData != null)
                            schemaData.Active = false;

                        SchemaData newSchemaData;
                        if (schemaData != null)
                        {
                            newSchemaData = new SchemaData
                                            {
                                                DataType = schemaData.DataType,
                                                DisplayOrder = order,
                                                InSchema = schemaData.InSchema
                                            };
                        }
                        else
                            newSchemaData = new SchemaData {DisplayOrder = order,};

                        smi.SchemaDatas.Add(newSchemaData);
                    }
                }
            }
                // changing active (inschema) bit
            else if (colItem.AttributeName.Equals(Active))
            {
                value = value ?? false;
                string firstCharacter;
                try
                {
                    firstCharacter = value.ToString().Substring(0, 1).ToLower();
                }
                catch (Exception)
                {
                    firstCharacter = "f";
                }

                var active = firstCharacter.Equals("t") || firstCharacter.Equals("y") || firstCharacter.Equals("1");

                var smi = rowItem.SchemaInfos.FirstOrDefault(si => si.TaxonomyID == Guid.Empty);
                if (smi == null)
                {
                    rowItem.SchemaInfos.Add(new SchemaInfo
                        {
                            TaxonomyID = Guid.Empty,
                            Attribute = colItem,
                            SchemaDatas = {new SchemaData {InSchema = active}}
                        });
                }
                else
                {
                    var schemaData = smi.SchemaData;
                    if (schemaData != null)
                        schemaData.Active = false;

                    SchemaData newSchemaData;
                    if (schemaData != null)
                    {
                        newSchemaData = new SchemaData
                            {
                                DataType = schemaData.DataType,
                                DisplayOrder = schemaData.DisplayOrder,
                                InSchema = active
                            };
                    }
                    else
                        newSchemaData = new SchemaData {InSchema = active,};

                    smi.SchemaDatas.Add(newSchemaData);
                }
            }
            else
            {
                // most meta-meta data belongs in the AttributeMeta structure.
                var ami = rowItem.AttributeMetaInfos.FirstOrDefault(a => a.MetaAttribute == colItem);
                if (ami != null)
                {
                    var amd = ami.AttributeMetaDatas.FirstOrDefault(d => d.Active);
                    if (amd != null)
                        amd.Active = false;
                }

                if (value != null && value.ToString() != string.Empty)
                {
                    if (ami == null)
                    {
                        ami = new AttributeMetaInfo
                            {
                                MetaAttribute = colItem
                            };

                        rowItem.AttributeMetaInfos.Add(ami);
                    }

                    ami.AttributeMetaDatas.Add(new AttributeMetaData {Value = value.ToString()});
                }
            }
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            dgvMetas.Invalidate();
        }

        private void AddMetaAttribute(object value)
        {
            if (!_rows.Any(row => row.AttributeName.ToLower().Equals(value.ToString().ToLower())))
            {
                var newAttribute = new Attribute
                                   {
                                       AttributeName = value.ToString(),
                                       AttributeType = _metaAttType.ToString(),
                                       SchemaInfos =
                                       {
                                           new SchemaInfo
                                           {
                                               TaxonomyID = Guid.Empty,
                                               SchemaDatas =
                                               {
                                                   new SchemaData
                                                   {
                                                       InSchema
                                                           =
                                                           true
                                                   }
                                               }
                                           }
                                       }
                                   };
                AryaTools.Instance.InstanceData.Dc.Attributes.InsertOnSubmit(newAttribute);
                _rows.Add(newAttribute);
                SetRowCount();
            }
            else
                MessageBox.Show("This meta-attribute already exists.");
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

        private void AutoResizeColumns(int col = -1)
        {
            var columns = col == -1 ? Enumerable.Range(0, dgvMetas.ColumnCount) : col.AsEnumerable();
            foreach (var i in columns)
            {
                var preferredWidth = dgvMetas.Columns[i].GetPreferredWidth(
                    DataGridViewAutoSizeColumnMode.DisplayedCells, false);
                if (preferredWidth > dgvMetas.Columns[i].Width)
                    dgvMetas.Columns[i].Width = preferredWidth > 250 ? 250 : preferredWidth;
            }
        }
        private void dgvMetas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvMetas.SelectedCells.Count > 0)
            {
                for (int i = 0; i < dgvMetas.SelectedCells.Count; i++)
                {
                    EditMetaAttribute(dgvMetas.SelectedCells[i].ColumnIndex, dgvMetas.SelectedCells[i].RowIndex, string.Empty);
                }

            }
        }
        #endregion

        private void dgvMetas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
                return;
            if (e.CellStyle.FormatProvider is BoolFormatter)
            {
                e.Value =
                    ((ICustomFormatter)e.CellStyle.FormatProvider.GetFormat(typeof(ICustomFormatter))).Format(
                        e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }
        }
    }

    public enum MetaTypeEnum
    {
        Schema,
        Taxonomy
    }
}