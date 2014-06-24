using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Attribute = Arya.Data.Attribute;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using EntityData = Arya.Data.EntityData;
using EntityInfo = Arya.Data.EntityInfo;
using ListOfValue = Arya.Data.ListOfValue;
using SchemaInfo = Arya.Data.SchemaInfo;
using Sku = Arya.Data.Sku;
using SkuInfo = Arya.Data.SkuInfo;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;
using Arya.SpellCheck;

namespace Arya
{
    public partial class FrmListOfValues : Form
    {
        #region Fields (2) 

        private List<ListofValueWrapper> _lovs;
        private SchemaInfo _schemaInfo;
        private bool _advancedColumnsVisible;
        private SortDirection _sortDirection = SortDirection.None;
        private int _sortColumn;
        private const string ValueColumn = "colValue";
        private const string ImageColumn = "colImage";
        private readonly Dictionary<string, ListOfValuesColumn> _columns = new Dictionary<string, ListOfValuesColumn>();

        #endregion Fields 

        #region Constructors (1) 

        public FrmListOfValues()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;

            _columns.Add("Value",
                         new ListOfValuesColumn
                             {
                                 ColumnOrder = 1,
                                 ColumnName = "colValue",
                                 FieldName = "Value",
                                 Default = "",
                                 Visible = true,
                                 ReadOnly = false,
                                 Width = 100,
                                 AutoSize = DataGridViewAutoSizeColumnMode.Fill
                             });
            _columns.Add("Parent Value",
                         new ListOfValuesColumn
                             {
                                 ColumnOrder = 2,
                                 ColumnName = "colParentValue",
                                 FieldName = "ParentValue",
                                 Default = "<N/A>",
                                 Visible = false,
                                 ReadOnly = false,
                                 Width = 100,
                                 AutoSize = DataGridViewAutoSizeColumnMode.Fill                             });
            _columns.Add("Enrichment - Image",
                         new ListOfValuesColumn
                             {
                                 ColumnOrder = 4,
                                 ColumnName = ImageColumn,
                                 FieldName = "EnrichmentImage",
                                 Default = "",
                                 Visible = false,
                                 ReadOnly = true,
                                 Width = 100,
                                 AutoSize = DataGridViewAutoSizeColumnMode.Fill
                             });
            _columns.Add("Enrichment - Copy",
                         new ListOfValuesColumn
                             {
                                 ColumnOrder = 3,
                                 ColumnName = "colCopy",
                                 FieldName = "EnrichmentCopy",
                                 Default = "<N/A>",
                                 Visible = false,
                                 ReadOnly = false,
                                 Width = 100,
                                 AutoSize = DataGridViewAutoSizeColumnMode.Fill
                             });
            _columns.Add("Display Order",
                         new ListOfValuesColumn
                             {
                                 ColumnOrder = 0,
                                 ColumnName = "colDisplayOrder",
                                 FieldName = "DisplayOrder",
                                 Default = "",
                                 Visible = false,
                                 ReadOnly = false,
                                 Width = 50,
                                 AutoSize = DataGridViewAutoSizeColumnMode.None
                             });

            var columns = _columns.OrderBy(o => o.Value.ColumnOrder).ToList();
            foreach (var c in columns)
            {
                var i = dgv.Columns.Add(c.Value.ColumnName, c.Key);
                dgv.Columns[i].ReadOnly = c.Value.ReadOnly;
                dgv.Columns[i].Width = c.Value.Width;
                dgv.Columns[i].AutoSizeMode = c.Value.AutoSize;
            }

            chkAdvancedColumns.Checked = false;
            SetColumns();
        }

        #endregion Constructors 

        #region Properties (1) 

        public SchemaInfo SchemaInfo
        {
            get { return _schemaInfo; }
            set
            {
                if (value == null)
                {
                    _schemaInfo = null;
                    _lovs = new List<ListofValueWrapper>();
                    dgv.RowCount = 1;
                    dgv.Enabled = false;
                    lblTaxonomy.Text = string.Empty;
                }
                else
                {
                    _schemaInfo = value;
                    _lovs = (from lov in _schemaInfo.ListOfValues
                             where lov.Active
                             orderby lov.Value
                             select new ListofValueWrapper(lov)).ToList();
                    dgv.RowCount = _lovs.Count + 1;
                    dgv.Enabled = true;
                    lblTaxonomy.Text = 
                        (String.IsNullOrEmpty(_schemaInfo.TaxonomyInfo.ToString()) 
                            ? ""
                            : _schemaInfo.TaxonomyInfo + "  -  ") 
                        + _schemaInfo.Attribute.AttributeName;
                }
                dgv.DefaultCellStyle = DisplayStyle.CellStyleDefaultRegularColumn;

                dgv.Invalidate();
            }
        }

        public bool ShowPopulateButton
        {
            set
            {
                btnPopulate.Visible = value;
                chkAdvancedColumns.Visible = value;
            }
        }

        #endregion Properties 

        #region Methods (2) 

        // Private Methods (2) 

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < _lovs.Count)
            {
                string header = dgv.Columns[e.ColumnIndex].HeaderText;
                string fname = _columns[header].FieldName;
                Type type = _lovs[e.RowIndex].GetType();
                PropertyInfo lovProperty = type.GetProperty(fname);
                e.Value = lovProperty.GetValue(_lovs[e.RowIndex]) ?? _columns[header].Default;
            }
        }

        private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_schemaInfo == null)
                return;

            if (e.RowIndex < _lovs.Count)
            {
                // this code is called when an existing value is modified (this includes value deletions)
                // first, figure out the relevant property of the LOV, based on the column.
                string header = dgv.Columns[e.ColumnIndex].HeaderText;
                string fname = _columns[header].FieldName;
                Type lovType = _lovs[e.RowIndex].GetType();
                PropertyInfo lovProperty = lovType.GetProperty(fname);

                // get the string representation of the new value
                string newValue = e.Value == null ? string.Empty : e.Value.ToString().Trim();

                if (lovProperty.PropertyType == typeof(string))
                {
                    // get the old value for this cell/property
                    var currentValue = lovProperty.GetValue(_lovs[e.RowIndex]);
                    // if the current value is null or it doesn't match the new value, 
                    // assign the new value to the property (this fires the corresponding setter)
                    if (currentValue == null || currentValue.ToString() != newValue)
                        lovProperty.SetValue(_lovs[e.RowIndex], newValue);
                }
                else if (lovProperty.PropertyType == typeof(int?))
                {
                    int numValue;
                    if (int.TryParse(newValue, out numValue))
                    {
                        lovProperty.SetValue(_lovs[e.RowIndex], numValue);
                    }
                    else
                    {
                        lovProperty.SetValue(_lovs[e.RowIndex], null);
                    }
                }

                if (!_lovs[e.RowIndex].Active)
                    _lovs.RemoveAt(e.RowIndex);
            }
            else
            {
                // this code runs if the focus is on the new row - we're adding a value.
                // note that the setter is never called
                string value = e.Value == null ? string.Empty : e.Value.ToString().Trim();
                if (dgv.Columns[e.ColumnIndex].Name == ValueColumn && !String.IsNullOrEmpty(value))
                {
                    var lov = SchemaInfo.AddLov(value);
                    if (lov != null)
                        _lovs.Add(new ListofValueWrapper(lov));
                }
            }
            AryaTools.Instance.SaveChangesIfNecessary(false, false);

            // match the number of rows in the grid to the number of LOVs
            dgv.RowCount = _lovs.Count + 1;
        }

        private void PopulateLov(bool overwrite)
        {
            // determine location
            Attribute attribute = SchemaInfo.Attribute;
            TaxonomyInfo taxonomy = SchemaInfo.TaxonomyInfo;

            Guid waitkey =
                FrmWaitScreen.ShowMessage(string.Format("Populating LOVs - {0} - {1}", taxonomy.NodeName,
                                                        attribute.AttributeName));

            // get values from database
            var listOfValues = GetLovs(attribute, taxonomy);

            SchemaInfo schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));
            if (schemaInfo != null)
            {
                if (overwrite)
                {
                    // assign database values to list of values
                    schemaInfo.ActiveListOfValues = listOfValues;
                }
                else
                {
                    // compare current list of values to database and add 
                    // missing database values to list of values
                    var currentLov = schemaInfo.ActiveListOfValues.ToList();
                    foreach (var lov in listOfValues)
                    {
                        if (!currentLov.Contains(lov))
                        {
                            currentLov.Add(lov);
                        }
                    }
                    schemaInfo.ActiveListOfValues = currentLov;
                }
            }

            // save new list of values
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            FrmWaitScreen.HideMessage(waitkey);

            // refresh list of values form
            AryaTools.Instance.Forms.ListofValuesForm.SchemaInfo = schemaInfo;
        }

        private IEnumerable<string> GetLovs(Attribute attribute, TaxonomyInfo taxonomy)
        {
            return AryaTools.Instance.InstanceData.Dc.ExecuteQuery<string>(
                @"SELECT distinct ed.Value
                                    FROM EntityData ed
                                    INNER JOIN EntityInfo ei ON ed.Active = 1 AND ed.EntityID = ei.ID AND ed.AttributeID = {0}
                                    INNER JOIN SkuInfo si ON si.Active = 1 AND ei.SkuID = si.SkuID AND si.TaxonomyID = {1}",
                attribute.ID, taxonomy.ID).OrderBy(p => p, new CompareForAlphaNumericSort()).ToList();
        }

        #endregion Methods 

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Columns[ImageColumn] == null || e.ColumnIndex != dgv.Columns[ImageColumn].Index)
            {
                return;
            }

            using (
                var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                    AryaTools.Instance.InstanceData.CurrentUser.ID))
            {
                var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID);
                if (!imageMgr.UploadImage())
                    return;

                imageMgr.AddAttributeValue("LovId", _lovs[e.RowIndex].Id.ToString());
                imageMgr.AddAttributeValue("TaxonomyId", SchemaInfo.TaxonomyID.ToString());
                imageMgr.AddAttributeValue("AttributeId", SchemaInfo.AttributeID.ToString());

                SchemaInfo.TaxonomyInfo.SkuInfos.Add(new SkuInfo {SkuID = imageMgr.ImageSku.ID});

                AryaTools.Instance.SaveChangesIfNecessary(false, false);

                dgv.Rows[e.RowIndex].Cells[ImageColumn].Value = imageMgr.RemoteImageGuid;
                if (AryaTools.Instance.Forms.TreeForm.ShowEnrichments)
                {
                    AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                }
            }
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
                if (e.RowIndex % 2 != 0)
                {
                    e.CellStyle.BackColor = DisplayStyle.CellStyleOddRow.BackColor;

                }

            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            Close();
            Dispose(true);
        }

        private void btnPopulate_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Do you want to overwrite current list of values?" +
                                    "\n\nClick <yes> to overwrite current list of values" +
                                    "\nClick <no> to append values to current list" +
                                    "\nClick <cancel> to close with no changes", "Question",
                                    MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    {
                        PopulateLov(true);
                        break;
                    }
                case DialogResult.No:
                    {
                        PopulateLov(false);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        break;
                    }
            }
        }

        private void FrmListOfValues_Load(object sender, EventArgs e)
        {
            if (dgv.Columns[ValueColumn] != null)
                dgv.CurrentCell = dgv[dgv.Columns[ValueColumn].Index, dgv.Rows.Count - 1];
        }

        private void dgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Columns[ImageColumn] == null
                || !AryaTools.Instance.Forms.TreeForm.ShowEnrichments)
            {
                return;
            }

//            TaxonomyInfo taxonomy = SchemaInfo.TaxonomyInfo;

            if (dgv[dgv.Columns[ImageColumn].Index, e.RowIndex].Value != null)
            {
                var image = _lovs[e.RowIndex].EnrichmentImageId;
                if (!string.IsNullOrEmpty(image))
                {

                    using (
                        var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                            AryaTools.Instance.InstanceData.CurrentUser.ID))
                    {
                        var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                        {
                            ImageSku = dc.Skus.FirstOrDefault(sku => sku.ItemID == image)
                        };
                        AryaTools.Instance.Forms.BrowserForm.DisplayImage(imageMgr);
                    }
                }
                else
                {
                    BlankImageForm();
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

        private void chkAdvancedColumns_CheckedChanged(object sender, EventArgs e)
        {
            _advancedColumnsVisible = chkAdvancedColumns.Checked;
            SetColumns();
        }

        private void SetColumns()
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].Visible = _advancedColumnsVisible || _columns[dgv.Columns[i].HeaderText].Visible;
            }
        }

        private void dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_sortColumn == e.ColumnIndex)
            {
                _sortDirection = _sortDirection != SortDirection.Ascending
                                     ? SortDirection.Ascending
                                     : SortDirection.Descending;
            }
            else
            {
                _sortColumn = e.ColumnIndex;
                _sortDirection = SortDirection.Ascending;
            }
            
            string header = dgv.Columns[e.ColumnIndex].HeaderText;
            string fname = _columns[header].FieldName;
            PropertyInfo lovProperty = _lovs.GetType().GetGenericArguments()[0].GetProperty(fname);

            _lovs = _sortDirection == 
                SortDirection.Ascending 
                ? _lovs.OrderBy(f => lovProperty.GetValue(f, null)).ThenBy(v => _lovs.IndexOf(v)).ToList()
                : _lovs.OrderByDescending(f => lovProperty.GetValue(f, null)).ThenBy(v => _lovs.IndexOf(v)).ToList();
            dgv.Refresh();
        }

        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete ||  dgv.Columns[ImageColumn] == null)
                return;

            var selectedCell = dgv.SelectedCells[0];
            if (dgv.Columns[selectedCell.ColumnIndex] != dgv.Columns[ImageColumn] || String.IsNullOrEmpty(selectedCell.Value.ToString()))
                return;
            
            DialogResult results = MessageBox.Show("Are you sure that you want to delete this image?", "Warning", MessageBoxButtons.YesNo);
            if (results != DialogResult.Yes)
                return;

            
            selectedCell.Value = String.Empty;
            BlankImageForm();
        }

        private void BlankImageForm()
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

    public class ListofValueWrapper:ISpell
    {
        private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                return (KeyValuePair<bool, string>)_updatable;
            }
        }
        #region Fields (1) 
        private ListOfValue _lov;

        #endregion Fields 

        #region Constructors (1) 

        public ListofValueWrapper(ListOfValue lov)
        {
            _lov = lov;
        }

        #endregion Constructors 

        #region Properties (2)

        public string EnrichmentImageId
        {
            get { return _lov.EnrichmentImage; }
        }

        public bool Active
        {
            get { return _lov != null && _lov.Active; }
        }

        public Guid Id
        {
            get { return _lov.ID; }
        }

        public string ParentValue
        {
            get { return _lov.ParentValue; }

            set
            {
                _lov.Active = false;

                _lov = _lov.SchemaInfo.AddLov(_lov.Value, value, _lov.EnrichmentImage, _lov.EnrichmentCopy,
                                              _lov.DisplayOrder);
            }

        }

        public string Value
        {
            get { return _lov.Value; }
            set
            {
                _lov.Active = false;

                if (string.IsNullOrEmpty(value) || _lov.Value.Equals(value))
                    return;

                _lov = _lov.SchemaInfo.AddLov(value);
            }
        }

        public string EnrichmentImage
        {
            get
            {
                string imageFileName = string.Empty;
                var firstOrDefault = _lov.SchemaInfo.TaxonomyInfo.SkuInfos.FirstOrDefault(si => si.Sku.ItemID == _lov.EnrichmentImage);
                if (firstOrDefault == null)
                    return imageFileName;

                var imageSku = firstOrDefault.Sku;

                using (
                    var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                        AryaTools.Instance.InstanceData.CurrentUser.ID))
                {
                    var imageMgr = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                                   {
                                       ImageSku
                                           =
                                           dc
                                           .Skus
                                           .First
                                           (sku
                                           =>
                                           sku
                                           .ID
                                           == imageSku
                                           .ID)
                                   };
                    imageFileName = Path.GetFileName(imageMgr.OriginalFileUri);
                    return imageFileName;
                }
            }
            set
            {
                _lov.Active = false;

                _lov = _lov.SchemaInfo.AddLov(_lov.Value, _lov.ParentValue, value, _lov.EnrichmentCopy,
                                              _lov.DisplayOrder);
            }
        }

        public string EnrichmentCopy
        {
            get
            {
                return _lov.EnrichmentCopy;
            }
            set
            {
                _lov.Active = false;

                _lov = _lov.SchemaInfo.AddLov(_lov.Value, _lov.ParentValue, _lov.EnrichmentImage, value,
                                              _lov.DisplayOrder);
            }
        }

        public int? DisplayOrder
        {
            get { return _lov.DisplayOrder; }
            set
            {
                _lov.Active = false;

                _lov = _lov.SchemaInfo.AddLov(_lov.Value, _lov.ParentValue, _lov.EnrichmentImage, _lov.EnrichmentCopy,
                                              value);
            }
        }

        #endregion Properties 
    
        string ISpell.GetType()
        {
            return this.GetType().ToString();
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
             //var   _propertyNameValue = new Dictionary<string, string>();
             var _propertyNameValue = new Dictionary<string, string>();
                if (!String.IsNullOrEmpty(this.Value))
                {
                    _propertyNameValue.Add("Value", this.Value);
                }
                if (!String.IsNullOrEmpty(this.EnrichmentCopy))
                {
                    _propertyNameValue.Add("EnrichmentCopy", this.EnrichmentCopy);
                }

            return _propertyNameValue;
        }

        Guid ISpell.GetId()
        {
            return _lov.ID;
        }

        ISpell ISpell.SetValue(string propertyName,string value)
        {            
            if (propertyName.ToLower() == "enrichmentcopy")
            {
                EnrichmentCopy = value;
            }
            else if (propertyName.ToLower() == "value")
            {
                 Value = value;
            }           
            return this;
        }

        string ISpell.GetLocation()
        {
            return "Lov Value: "+ Value;
        }
    }

    public struct ListOfValuesColumn
    {
        #region Properties

        public int ColumnOrder { get; set; }
        public string ColumnName { get; set; }
        public string FieldName { get; set; }
        public string Default { get; set; }
        public bool Visible { get; set; }
        public bool ReadOnly { get; set; }
        public int Width { get; set; }
        public DataGridViewAutoSizeColumnMode AutoSize { get; set; }

        #endregion
    }

    public enum SortDirection
    {
        Ascending, 
        Descending,
        None
    }

}