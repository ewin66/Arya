using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.GUI.UserControls;
using Arya.Framework.Properties;
using Arya.HelperClasses;
using Arya.HelperForms;
using Attribute = Arya.Data.Attribute;

namespace Arya.UserControls
{
    public partial class AttributeFarmGridView : UserControl
    {
        private const string AllAttributeGroups = "All Groups";
        private const string AllAttributeTypes = "All Types";
        private const string ColumnAttributeGroup = "Attribute Group";
        private const string ColumnAttributeName = "Attribute Name";
        private const string ColumnAttributeType = "Attribute Type";
        private const string DefaultAttributeGroup = "Default";
        private readonly HashSet<string> _attributeGroups = new HashSet<string>();
        private List<Attribute> _allAttributes;

        private HashSet<Guid> _checkedAttributes = new HashSet<Guid>();
        private bool _sortAscending;
        private string _sortBy;
        private List<Attribute> _visibleAttributes;

        #region Constructors (1)

        public AttributeFarmGridView()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            //Icon = Resources.BMI_Logo;
            dgvAttributes.AutoGenerateColumns = false;

            dgvAttributes.DefaultCellStyle = DisplayStyle.CellStyleDefaultRegularColumn;

            dgvAttributes.DoubleBuffered(true);
            dgvAttributes.RowsDefaultCellStyle.BackColor = DisplayStyle.CellStyleGreyRegular.BackColor;
            dgvAttributes.AlternatingRowsDefaultCellStyle.BackColor = DisplayStyle.CellStyleOddRow.BackColor;
            dgvAttributes.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            dgvAttributes.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            dgvAttributes.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            dgvAttributes.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
        }

        public bool DisableEditing
        {
            set { dgvAttributes.ReadOnly = value; }
        }

        public bool IsPermissionsUIEnabled
        {
            get { return dgvAttributes.Columns[0].Visible; }
            set
            {
                dgvAttributes.Columns[0].Visible = value;
                DisableEditing = value;
            }
        }

        public List<Guid> CheckedAttributes
        {
            get { return _checkedAttributes.ToList(); }
        }

        public void PrepareData()
        {
            tmrUpdateDgv.Stop();
            InitAttributes();
        }

        public void PrepareData(HashSet<Guid> attributeExclusions)
        {
            tmrUpdateDgv.Stop();
            InitAttributes(attributeExclusions);
        }

        #endregion Constructors

        #region Properties (1)

        private ParallelQuery<Attribute> FilteredAttributes
        {
            get
            {
                var atts = _allAttributes.AsParallel();
                var attributeTypeFilter = ddAttributeTypeFilter.Text == "Product" ? "Sku" : ddAttributeTypeFilter.Text;
                if (!string.IsNullOrEmpty(attributeTypeFilter) && !attributeTypeFilter.Equals(AllAttributeTypes))
                {
                    atts = atts.Where(a => a.AttributeType.Equals(attributeTypeFilter));
                }

                var attributeGroupFilter = ddAttributeGroupFilter.Text;
                if (!string.IsNullOrEmpty(attributeGroupFilter) && !attributeGroupFilter.Equals(AllAttributeGroups))
                {
                    atts = attributeGroupFilter.Equals(DefaultAttributeGroup)
                        ? atts.Where(a => a.Group == null)
                        : atts.Where(a => a.Group != null && a.Group.Equals(attributeGroupFilter));
                }

                if (!string.IsNullOrEmpty(txtAttributeNameFilter.Text))
                    atts = atts.Where(a => a.AttributeName.ToLower().Contains(txtAttributeNameFilter.Text.ToLower()));

                if (_filterCheckedAttributes)
                {
                    _filterCheckedAttributes = false;
                    atts = atts.Where(a => _checkedAttributes.Contains(a.ID));
                }

                return atts;
            }
        }

        #endregion Properties

        #region Methods (11)

        // Private Methods (11) 

        private bool _filterCheckedAttributes;

        private readonly string[] _attributeTypesAutoComplete =
        {
            AttributeTypeEnum.Derived.ToString(),
            AttributeTypeEnum.Global.ToString()
        };

        private void adgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var headerText = dgvAttributes.CurrentCell.OwningColumn.HeaderText;
            switch (headerText)
            {
                case ColumnAttributeGroup:
                    ((TextBox) e.Control).SetAutoComplete(_attributeGroups);
                    break;

                case ColumnAttributeType:
                    ((TextBox) e.Control).SetAutoComplete(_attributeTypesAutoComplete);
                    break;
            }
        }

        private void dgvAllAttributes_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Only allow Product/Sku attribute type to be changed to Global or Derived types
            if (dgvAttributes.Columns[e.ColumnIndex].HeaderText.Equals(ColumnAttributeType)
                && dgvAttributes[e.ColumnIndex, e.RowIndex].Value.ToString() != "Product" )
                e.Cancel = true;
        }

        private void dgvAllAttributes_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= _visibleAttributes.Count)
                return;

            var headerText = dgvAttributes.Columns[e.ColumnIndex].HeaderText;

            if (e.ColumnIndex == 0)
                headerText = "ImageCheckBox";

            switch (headerText)
            {
                case ColumnAttributeName:
                    e.Value = _visibleAttributes[e.RowIndex].AttributeName;
                    return;

                case ColumnAttributeType:
                    if (_visibleAttributes[e.RowIndex].AttributeType == "Sku")
                    {
                        e.Value = "Product";
                        return;
                    }
                    e.Value = _visibleAttributes[e.RowIndex].AttributeType;
                    return;

                case ColumnAttributeGroup:
                    e.Value = _visibleAttributes[e.RowIndex].Group;
                    return;
                case "ImageCheckBox":
                    ((DataGridViewImageCheckBoxCell) dgvAttributes[e.ColumnIndex, e.RowIndex]).IsChecked =
                        _checkedAttributes.Contains(_visibleAttributes[e.RowIndex].ID);
                    return;
            }
        }

        public void ShowOnlyCheckedAttributes()
        {
            if (IsPermissionsUIEnabled)
            {
                _filterCheckedAttributes = true;
                tmrUpdateDgv.Stop();
                tmrUpdateDgv.Start();
            }
        }

        private void dgvAllAttributes_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.Value == null)
                return;

            var value = e.Value.ToString();

            if (string.IsNullOrWhiteSpace(value)
                || string.Equals(value, Resources.NewAttributeName, StringComparison.InvariantCultureIgnoreCase))
                return;

            switch (dgvAttributes.Columns[e.ColumnIndex].HeaderText)
            {
                case ColumnAttributeName:
                    if (e.RowIndex == _visibleAttributes.Count)
                    {
                        var existingAttribute = Attribute.GetAttributeFromName(value, false);
                        if (existingAttribute != null)
                            MessageBox.Show(@"Attribute already exists!");
                        else
                        {
                            var newAttribute = Attribute.GetAttributeFromName(value, true);
                            _allAttributes.Add(newAttribute);
                            _visibleAttributes.Add(newAttribute);
                            dgvAttributes.RowCount++;
                        }
                    }
                    else
                    {
                        if (!_visibleAttributes[e.RowIndex].AttributeName.ToLower().Equals(value.ToLower()))
                            MessageBox.Show(@"Only Capitalization can be fixed in the Attribute Farm");
                        else
                            _visibleAttributes[e.RowIndex].AttributeName = value;
                    }
                    break;

                case ColumnAttributeType:
                    if (e.RowIndex < _visibleAttributes.Count)
                    {
                        int rowIndex = e.RowIndex;
                        UpdateAttributeType(_visibleAttributes[rowIndex], value);
                    }
                    break;

                case ColumnAttributeGroup:
                    if (e.RowIndex < _visibleAttributes.Count)
                    {
                        int rowIndex = e.RowIndex;
                        UpdateAttributeGroup(_visibleAttributes[rowIndex], value);
                    }
                    break;
            }
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void UpdateAttributeGroup(Attribute attribute, string value)
        {
            attribute.Group = value;
            _attributeGroups.Add(value);

            InitAttributeGroups();
        }

        private void UpdateAttributeType(Attribute attribute, string value)
        {
            if (attribute.Type == AttributeTypeEnum.Sku)
            {
                if (AttributeTypeEnum.Global.ToString().ToLower().StartsWith(value.ToLower()))
                {
                    attribute.AttributeType = AttributeTypeEnum.Global.ToString();
                    return;
                }

                if (AttributeTypeEnum.Derived.ToString().ToLower().StartsWith(value.ToLower()))
                {
                    attribute.AttributeType = AttributeTypeEnum.Derived.ToString();
                    var existingEntities = attribute.EntityDatas.Where(ed => ed.Active).ToList();

                    if (existingEntities.Any())
                    {
                        var msg =
                            string.Format(
                                "There are {0} existing values connected to this attribute. Do you want to permanently delete them?",
                                existingEntities.Count);
                        if (existingEntities.Count > 0
                            && MessageBox.Show(msg, @"Delete linked entities?", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                        existingEntities.ForEach(ed => ed.Active = false);
                    }
                }
            }
        }

        private void dgvAllAttributes_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var colItem = dgvAttributes.Columns[e.ColumnIndex].HeaderText;
            if (e.ColumnIndex == 0)
                colItem = "ImageCheckBox";
            if (_sortBy != null && _sortBy.Equals(colItem))
                _sortAscending = !_sortAscending;
            else
            {
                _sortBy = colItem;
                _sortAscending = true;
            }

            InitDgv();
        }

        private void FilterUpdated(object sender, EventArgs e)
        {
            tmrUpdateDgv.Stop();
            tmrUpdateDgv.Start();
        }

        private void InitAttributeGroups()
        {
            if (ddAttributeGroupFilter.Items.Count == _attributeGroups.Count + 2)
                return;

            ddAttributeGroupFilter.Items.Clear();
            ddAttributeGroupFilter.Items.AddRange(new [] {AllAttributeGroups, DefaultAttributeGroup});
            ddAttributeGroupFilter.Items.AddRange(_attributeGroups.OrderBy(t => t).ToArray());
            ddAttributeGroupFilter.SelectedIndex = 0;
        }

        private void InitAttributes(HashSet<Guid> attributeExclusions = null)
        {
            var waitKey = FrmWaitScreen.ShowMessage("Reading Attributes");
            _allAttributes =
                AryaTools.Instance.InstanceData.Dc.Attributes.Where(
                    att => att.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject)).ToList();

            _attributeGroups.Clear();
            var groups = (from att in _allAttributes where !string.IsNullOrEmpty(att.Group) select att.Group).Distinct();
            _attributeGroups.AddRange(groups);

            InitAttributeTypes();
            InitAttributeGroups();
            _checkedAttributes.Clear();

            if (attributeExclusions != null)
                _checkedAttributes = attributeExclusions;

            InitDgv();
            FrmWaitScreen.HideMessage(waitKey);
        }

        private void InitAttributeTypes()
        {
            var items = _allAttributes.Select(a => a.AttributeType).Distinct().OrderBy(t => t).ToArray();
            int index = items.ToList().FindIndex(a => a == "Sku");
            if (index>= 0)
            {
                items[index] = "Product";
                //items = items.Where(att => att != "Sku").ToArray();
                //items.AddRange(new List<string>(){"Product"});
            }
            if (ddAttributeTypeFilter.Items.Count == items.Length + 1)
                return;

            ddAttributeTypeFilter.Items.Clear();
            ddAttributeTypeFilter.Items.Add(AllAttributeTypes);
            ddAttributeTypeFilter.Items.AddRange(items);
            ddAttributeTypeFilter.SelectedIndex = 0;
        }

        private void InitDgv()
        {
            var waitKey = FrmWaitScreen.ShowMessage("Populating Grid View");
            var atts = FilteredAttributes;
            if (_sortBy != null)
            {
                switch (_sortBy)
                {
                    case ColumnAttributeName:
                        atts = _sortAscending
                            ? atts.OrderBy(a => a.AttributeName)
                            : atts.OrderByDescending(a => a.AttributeName);
                        break;

                    case ColumnAttributeType:
                        atts = _sortAscending
                            ? atts.OrderBy(a => a.AttributeType)
                            : atts.OrderByDescending(a => a.AttributeType);
                        break;

                    case ColumnAttributeGroup:
                        atts = _sortAscending ? atts.OrderBy(a => a.Group) : atts.OrderByDescending(a => a.Group);
                        break;

                    case "ImageCheckBox":
                        atts = _sortAscending
                            ? atts.OrderBy(a => a, new AttributeCheckBoxSort(_checkedAttributes))
                            : atts.OrderByDescending(a => a, new AttributeCheckBoxSort(_checkedAttributes));
                        break;
                }
            }
            _visibleAttributes = atts.ToList();
            dgvAttributes.RowCount = _visibleAttributes.Count + 1;
            //AttributeColumn
            dgvAttributes.Columns[1].DefaultCellStyle = DisplayStyle.CellStyleAttributeColumn;

            dgvAttributes.Invalidate();
            FrmWaitScreen.HideMessage(waitKey);
        }

        #endregion Methods

        private void tmrUpdateDgv_Tick(object sender, EventArgs e)
        {
            tmrUpdateDgv.Stop();
            InitDgv();
        }

        private void adgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                var currentCell = dgvAttributes[e.ColumnIndex, e.RowIndex] as DataGridViewImageCheckBoxCell;
                if (currentCell != null)
                {
                    var currentAttributeId = _visibleAttributes[e.RowIndex].ID;
                    if (currentCell.IsChecked)
                        _checkedAttributes.Add(currentAttributeId);
                    else
                        _checkedAttributes.Remove(currentAttributeId);
                }
            }
        }

        public void CheckAllAttributes()
        {
            if (IsPermissionsUIEnabled)
            {
                _checkedAttributes.Clear();
                _checkedAttributes.AddRange(_visibleAttributes.Select(p => p.ID));
                InitDgv();
            }
        }

        public void UnCheckAllAttributes()
        {
            if (IsPermissionsUIEnabled)
            {
                _checkedAttributes.Clear();
                InitDgv();
            }
        }

        public void InvertCheckedAttributes()
        {
            if (IsPermissionsUIEnabled)
            {
                var currentCheckedAttributes = _checkedAttributes;
                var allVisibleAttributes = _visibleAttributes.Select(p => p.ID).ToHashSet();
                _checkedAttributes = allVisibleAttributes.Except(currentCheckedAttributes).ToHashSet();
                InitDgv();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var dresult = sfdAttributeData.ShowDialog();

            if (dresult != DialogResult.OK)
                return;

            var attributeData = new StringBuilder();

            //Copy only the visible Headers
            var headerString =
                dgvAttributes.Columns.Cast<DataGridViewColumn>()
                    .Where(p => p.Visible)
                    .Select(p => p.HeaderText)
                    .Aggregate((a, b) => a + "\t" + b);

            var visibleColumns =
                dgvAttributes.Columns.Cast<DataGridViewColumn>()
                    .Where(p => p.Visible)
                    .Select(p => p.DisplayIndex)
                    .ToList();

            attributeData.AppendLine("\t" + headerString);

            for (var row = 0; row < dgvAttributes.Rows.Count; row++)
            {
                var columnCount = dgvAttributes.Rows[row].Cells.Count;
                var columnValues = new string[columnCount];

                foreach (var visibleColumn in visibleColumns)
                {
                    var currentValue = dgvAttributes.Rows[row].Cells[visibleColumn].Value;
                    columnValues[visibleColumn] = currentValue == null ? string.Empty : currentValue.ToString();
                }

                attributeData.AppendLine(columnValues.Aggregate((a, b) => a + "\t" + b));
            }

            File.WriteAllText(sfdAttributeData.FileName, attributeData.ToString());
        }
    }

    public class AttributeCheckBoxSort : IComparer<Attribute>
    {
        private readonly HashSet<Guid> _checkedAttributes;

        public AttributeCheckBoxSort(HashSet<Guid> checkedAttributes) { _checkedAttributes = checkedAttributes; }

        public int Compare(Attribute x, Attribute y)
        {
            var xPresent = _checkedAttributes.Contains(x.ID);
            var yPresent = _checkedAttributes.Contains(y.ID);
            if (xPresent && yPresent)
                return 0;

            if (xPresent)
                return -1;
            if (yPresent)
                return 1;

            return x.CompareTo(y);
        }
    }
}