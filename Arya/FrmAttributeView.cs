using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LinqKit;
using Arya.Data;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using Attribute = Arya.Data.Attribute;
using EntityData = Arya.Data.EntityData;
using EntityInfo = Arya.Data.EntityInfo;
using Sku = Arya.Data.Sku;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;
using Arya.Framework.Properties;
using Resources = Arya.Framework.Properties.Resources;
using Arya.SpellCheck;

namespace Arya
{
    public partial class FrmAttributeView : Form
    {
        #region Fields (18)

        private const string AllAttributeGroups = "All Groups";
        private const int AttributesOnlyWidth = 500;
        //private const string AttributeTypeExtended = "Extended";
        private const string AttributeTypeProduct = "Product";
        private const string AttributeTypeRanked = "Ranked";
        private const string AttributeTypeInSchema = "In Schema";
        private const string AttributeTypeShowAll = "All Types";
        private const string DefaultAttributeGroup = "Default";
        private readonly Dictionary<Attribute, bool> _columnVisiblity = new Dictionary<Attribute, bool>();
        private readonly bool _showMvColumn = true;
        private bool _applyAttributes;
        private Attribute _currentLovAttribute;
        private List<AttributeColumn> _dsAttributes;
        private List<Lov> _dsListOfValues;
        private IQueryable<EntityInfo> _entities;
        private string _lastSortedAscendingBy;
        private string _queryDescription;
        private bool _settingChkAll;
        private IQueryable<Sku> _skuQuery;
        private TaxonomyInfo _taxonomy;
        //private HashSet<Guid> _multiValuedAttributes = new HashSet<Guid>();
        private List<TaxonomyInfo> _taxonomyNodes;
        private Thread _workerThread;

        #endregion Fields

        #region Constructors (2)

        public FrmAttributeView(List<AttributeColumn> columns, List<TaxonomyInfo> taxonomies) : this()
        {
            _showMvColumn = false;
            _columnVisiblity = columns.ToDictionary(c => c.Attribute, c => c.Visible);
            _dsAttributes =
                columns.Where(
                    col => Framework.Data.AryaDb.Attribute.NonMetaAttributeTypes.Contains(col.AttributeType))
                    .ToList();
            _taxonomyNodes = taxonomies;

            _taxonomy = _taxonomyNodes == null || _taxonomyNodes.Count != 1 ? null : _taxonomyNodes[0];

            ShowHideRankColumns();

            chkAll.Visible = true;
            colLocalVisible.Visible = true;
            lblStatus.Visible = false;
            flpLOvOptions.Visible = false;
            dgvListOfValues.Visible = false;
            Width = AttributesOnlyWidth;
            tblAttributeViewMain.ColumnStyles[0].SizeType = SizeType.Percent;
            tblAttributeViewMain.ColumnStyles[0].Width = 100F;

            StartPosition = FormStartPosition.CenterParent;

            Init();

            var attributeColumn = dgvAttributes.Columns["colAttributeName"];
            if (attributeColumn != null)
                attributeColumn.ReadOnly = true;
            //prepare for show hide attribute window
            tblAttributeViewMain.RowCount = 4;
            btnApply.Visible = true;
            btnSavePrefs.Visible = false;
            btnCheckSpelling.Enabled = false;
           
        }

        public FrmAttributeView()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Arya.Properties.Resources.AryaLogoIcon;
            dgvAttributes.AutoGenerateColumns = false;
            dgvListOfValues.AutoGenerateColumns = false;
            lblStatus.Text = string.Empty;

            dgvAttributes.DoubleBuffered(true);
            dgvAttributes.RowsDefaultCellStyle.BackColor = DisplayStyle.CellStyleGreyRegular.BackColor;
            dgvAttributes.AlternatingRowsDefaultCellStyle.BackColor = DisplayStyle.CellStyleOddRow.BackColor;
            dgvAttributes.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            dgvAttributes.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            dgvAttributes.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            dgvAttributes.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
        }

        #endregion Constructors

        #region Properties (1)

        private bool ShowLovs
        {
            get { return chkShowLovs.Checked; }
        }

        #endregion Properties

        #region Methods (20)

        // Private Methods (19) 

        //private Dictionary<Guid, List<Guid>> inSchemaAttributes = new Dictionary<Guid, List<Guid>>();
        //private Dictionary<Guid, int> rankedAttributes = new Dictionary<Guid, int>();
        //private Dictionary<Guid, Tuple<int?, int?>> ranks = new Dictionary<Guid, Tuple<int?, int?>>();

        private void chkAll_CheckStateChanged(object sender, EventArgs e)
        {
            if (_settingChkAll)
                return;

            _settingChkAll = true;
            switch (chkAll.CheckState)
            {
                case CheckState.Unchecked:
                    dgvAttributes.Rows.Cast<DataGridViewRow>()
                        .ForEach(item => item.Cells["colLocalVisible"].Value = false);
                    break;
                case CheckState.Checked:
                    dgvAttributes.Rows.Cast<DataGridViewRow>()
                        .ForEach(item => item.Cells["colLocalVisible"].Value = true);
                    break;
                case CheckState.Indeterminate:
                    chkAll.CheckState = CheckState.Unchecked;
                    dgvAttributes.Rows.Cast<DataGridViewRow>()
                        .ForEach(item => item.Cells["colLocalVisible"].Value = false);
                    break;
            }
            _settingChkAll = false;
        }

        private void chkShowLovs_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowLovs.Checked)
            {
                if (_taxonomyNodes.Count > 1)
                    pnlLovOptions.Enabled = true;
                TryLoadLovs();
            }
            else
            {
                _currentLovAttribute = null;
                _dsListOfValues = null;
                dgvListOfValues.DataSource = null;
                pnlLovOptions.Enabled = false;
            }
        }

        private void dgvAttributes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //Refresh edited row with updated values
            dgvAttributes.InvalidateRow(e.RowIndex);

            switch (dgvAttributes.Columns[e.ColumnIndex].DataPropertyName)
            {
                case "Visible":
                    PopulateCheckAll();
                    break;
            }
        }

        private void dgvAttributes_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SortByColumn(dgvAttributes, e.ColumnIndex, (List<AttributeColumn>) dgvAttributes.DataSource);
        }

        private void dgvAttributes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void dgvAttributes_SelectionChanged(object sender, EventArgs e) { TryLoadLovs(); }

        private void dgvListOfValues_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SortByColumn(dgvListOfValues, e.ColumnIndex, _dsListOfValues);
        }

        private void FilterUpdated(object sender, EventArgs e) { LoadLists(); }

        private void txtAttributeNameFilter_TextChanged(object sender, EventArgs e)
        {
            if (txtAttributeNameFilter.Text.Length == 0 || !txtAttributeNameFilter.Text.All(char.IsWhiteSpace))
                FilterUpdated(sender, e);
        }

        private void FrmAttributeView_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Revert Column Visibilities
            if (_applyAttributes)
                return;
            if (_dsAttributes != null)
            {
                _dsAttributes.ForEach(col =>
                {
                    if (_columnVisiblity.ContainsKey(col.Attribute))
                        col.Visible = _columnVisiblity[col.Attribute];
                });
            }
            
        }

        private void FrmAttributeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && !dgvAttributes.IsCurrentRowDirty)
                Close();

            if (!e.Control)
                return;

            switch (e.KeyCode)
            {
                case Keys.S:
                    AryaTools.Instance.SaveChangesIfNecessary(false, true);
                    break;
            }
        }

        private void GetColumnProperties()
        {
            var sw = new Stopwatch();
            sw.Start();

            var loaded = false;
            var iteration = 0;
            while (!loaded && iteration++ < 5)
            {
                try
                {
                    //var allAtts = (from ei in _entities
                    //    from ed in ei.EntityDatas
                    //    where ed.Active
                    //    group ed by ed.Attribute.AttributeName.ToLower()
                    //    into grp
                    //    select grp.First().Attribute).ToList();

                    //var attGroups = (from ei in _entities
                    //    let ed = ei.EntityDatas.FirstOrDefault(d => d.Active)
                    //    where ed != null
                    //    group ei by ed.Attribute).ToList();

                    var attIds = (from ei in _entities
                                from ed in ei.EntityDatas
                                where ed.Active
                                select ed.AttributeID
                               ).Distinct().ToList();

                    var atts = from att in AryaTools.Instance.InstanceData.Dc.Attributes
                                where attIds.Contains(att.ID)
                                select att;

                    //_dsAttributes = allAtts.Select(att => new AttributeColumn(att, _taxonomyNodes)).ToList();
                    _dsAttributes =
                        atts.Select(att => new AttributeColumn(att, _taxonomyNodes, GetEntities(_entities, att)))
                            .ToList();

                    AttributeColumn.RefreshAttributeColumnPositions(_dsAttributes,
                                                                    atts.ToDictionary(att => att.AttributeName),
                                                                    _taxonomyNodes,
                                                                    true);

                    //var currentTaxIDs = _taxonomyNodes.Select(p => p.ID).ToList();

                    //_multiValuedAttributes = (from ei in _entities
                    //                          from ed in ei.EntityDatas
                    //                          where ed.Active
                    //                          group ed by new {ed.EntityInfo.SkuID, ed.AttributeID}
                    //                          into attrGrp
                    //                          where attrGrp.Count() > 1
                    //                          select attrGrp.Key.AttributeID).Distinct().ToHashSet();

                    //CacheRanks(currentTaxIDs);

                    loaded = true;
                }
                catch (NullReferenceException)
                {
                    Thread.Sleep(100);
                }
                catch (InvalidCastException)
                {
                    Thread.Sleep(100);
                }
            }

            sw.Stop();
            Diagnostics.WriteMessage("Getting Column Properties", "GetColumnProperties()", sw.Elapsed);
            sw.Reset();
        }

        private IQueryable<EntityInfo> GetEntities(IQueryable<EntityInfo> entities, Attribute att)
        {
            return from ei in entities
                   from ed in ei.EntityDatas
                   where ed.Active && ed.AttributeID == att.ID
                   select ed.EntityInfo;
        }

        private void Init()
        {
            txtAttributeNameFilter.TextAlignChanged -= FilterUpdated;

            PopulateFilterDropdown();
            LoadLists();
            lblStatus.Text = _queryDescription;

            txtAttributeNameFilter.TextAlignChanged += FilterUpdated;
            _currentLovAttribute = null;
        }

        private void LoadLists()
        {
            //Un-Hook the selection changed event, else any change will trigger the top row selection in the gridview
            dgvAttributes.SelectionChanged -= dgvAttributes_SelectionChanged;
            dgvListOfValues.DataSource = null;

            if (_dsAttributes == null)
                return;

            var currentDataSource =
                _dsAttributes.Where(col => col.AttributeName != Resources.NewAttributeName.Trim()).AsParallel();

            if (!string.IsNullOrEmpty(txtAttributeNameFilter.Text))
            {
                currentDataSource =
                    currentDataSource.Where(
                        col => col.AttributeName.ToLower().Contains(txtAttributeNameFilter.Text.ToLower()));
            }

            var attributeTypeFilter = ddAttributeTypeFilter.Text;
            if (!attributeTypeFilter.Equals(AttributeTypeShowAll))
            {
                switch (attributeTypeFilter)
                {
                    case  AttributeTypeProduct:
                        currentDataSource = currentDataSource.Where(col => col.Attribute.Type == AttributeTypeEnum.Sku);
                        break;
                    case AttributeTypeRanked:
                        currentDataSource = currentDataSource.Where(col => col.IsRanked != CheckState.Unchecked);
                        break;
                    case AttributeTypeInSchema:
                        currentDataSource = currentDataSource.Where(col => col.InSchema != CheckState.Unchecked);
                        break;
                    default:
                        currentDataSource =
                            currentDataSource.Where(col => col.Attribute.Type.ToString().Equals(attributeTypeFilter));
                        break;
                }
            }

            var attributeGroupFilter = ddAttributeGroupFilter.Text;
            if (!string.IsNullOrEmpty(attributeGroupFilter) && !attributeGroupFilter.Equals(AllAttributeGroups))
            {
                currentDataSource = attributeGroupFilter.Equals(DefaultAttributeGroup)
                    ? currentDataSource.Where(a => a.Attribute.Group == null)
                    : currentDataSource.Where(
                        a => a.Attribute.Group != null && a.Attribute.Group.Equals(attributeGroupFilter));
            }

            var attributeColumns = currentDataSource.ToList();
            attributeColumns.Sort();
            dgvAttributes.DataSource = attributeColumns;

            dgvAttributes.DefaultCellStyle = DisplayStyle.CellStyleGreyRegular;

            //if (dgvAttributes.ColumnCount > 7)
            //{
            dgvAttributes.Columns[5].DefaultCellStyle = DisplayStyle.CellStyleAttributeColumn;
            dgvAttributes.Columns[6].DefaultCellStyle = DisplayStyle.CellStyleDefaultRankColumn;
            dgvAttributes.Columns[7].DefaultCellStyle = DisplayStyle.CellStyleDefaultRankColumn;
            //}

            PopulateCheckAll();

            //Hook back the SelectionChanged Event
            dgvAttributes.SelectionChanged += dgvAttributes_SelectionChanged;
        }

        private void PopulateCheckAll()
        {
            if (_settingChkAll)
                return;

            _settingChkAll = true;

            var checkedRows = 0;
            dgvAttributes.Rows.Cast<DataGridViewRow>().ForEach(item =>
                                                               {
                                                                   if (item.Cells["colLocalVisible"].Value.Equals(true))
                                                                       checkedRows++;
                                                               });

            if (checkedRows == 0)
                chkAll.CheckState = CheckState.Unchecked;
            else if (checkedRows == dgvAttributes.RowCount)
                chkAll.CheckState = CheckState.Checked;
            else
                chkAll.CheckState = CheckState.Indeterminate;

            _settingChkAll = false;
        }

        private void PopulateFilterDropdown()
        {
            ddAttributeTypeFilter.SelectedIndexChanged -= FilterUpdated;
            ddAttributeGroupFilter.SelectedIndexChanged -= FilterUpdated;

            ddAttributeTypeFilter.Items.Clear();
            ddAttributeTypeFilter.Items.Add(AttributeTypeShowAll);
            ddAttributeTypeFilter.SelectedIndex = 0;

            ddAttributeGroupFilter.Items.Clear();
            ddAttributeGroupFilter.Items.Add(AllAttributeGroups);
            ddAttributeGroupFilter.SelectedIndex = 0;

            if (_dsAttributes == null)
                return;

            ddAttributeTypeFilter.Items.Add(AttributeTypeProduct);
            ddAttributeTypeFilter.Items.Add(AttributeTypeRanked);
            ddAttributeTypeFilter.Items.Add(AttributeTypeInSchema);

            ddAttributeGroupFilter.Items.Add(DefaultAttributeGroup);

            var attributeTypes =
                _dsAttributes.Where(col => col.Attribute.Type != AttributeTypeEnum.Sku)
                    .Select(col => col.Attribute.Type)
                    .Distinct();
            attributeTypes.ForEach(type => ddAttributeTypeFilter.Items.Add(type.ToString()));

            var attributeGroups =
                _dsAttributes.Where(col => col.Attribute.Group != null).Select(col => col.Attribute.Group).Distinct();
            attributeGroups.ForEach(group => ddAttributeGroupFilter.Items.Add(group));

            ddAttributeTypeFilter.SelectedIndexChanged += FilterUpdated;
            ddAttributeGroupFilter.SelectedIndexChanged += FilterUpdated;
        }

        private static int IsABlankValue(object val)
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

        private void SortByColumn<T>(DataGridView dgv, int columnIndex, List<T> dsGridViewSource)
        {
            var propertyName = dgv.Columns[columnIndex].DataPropertyName;
            var sortAscending = true;
            if (!string.IsNullOrEmpty(_lastSortedAscendingBy) && _lastSortedAscendingBy.Equals(propertyName))
            {
                sortAscending = false;
                _lastSortedAscendingBy = null;
            }
            else
                _lastSortedAscendingBy = propertyName;

            var property = (typeof (T)).GetProperty(propertyName);

            dsGridViewSource =
                (sortAscending
                    ? dsGridViewSource.OrderBy(col => IsABlankValue(property.GetValue(col, null)))
                        .ThenBy(col => property.GetValue(col, null))
                    : dsGridViewSource.OrderBy(col => IsABlankValue(property.GetValue(col, null)))
                        .ThenByDescending(col => property.GetValue(col, null))).ToList();
            dgv.DataSource = dsGridViewSource;
        }

        private void TryLoadLovs()
        {
            if (!ShowLovs || dgvAttributes.CurrentCell == null)
                return;

            var currentAttribute = ((AttributeColumn) dgvAttributes.CurrentCell.OwningRow.DataBoundItem).Attribute;

            if (currentAttribute == null || (_currentLovAttribute != null && _currentLovAttribute == currentAttribute))
                return;

            _currentLovAttribute = currentAttribute;

            var waitKey =
                FrmWaitScreen.ShowMessage(string.Format("Generating List of Values for {0}",
                    currentAttribute.AttributeName));

            if (rbAllLovs.Checked)
            {
                // this query must go ToList(), otherwise the results are not case sensitive. 
                var loadQuery = (from ei in _entities
                    join ed in AryaTools.Instance.InstanceData.Dc.EntityDatas on ei.ID equals ed.EntityID
                    where ed.AttributeID == currentAttribute.ID && ed.Active
                    select ed).ToList();

                var temp = (from ed in loadQuery
                    group ed by new {ed.Value, ed.Uom}
                    into grp
                    orderby grp.Key.Value
                    select new Lov(grp.Key.Value, grp.Key.Uom, grp.Select(ed => ed.EntityInfo)));

                _dsListOfValues = temp.ToList();

                dgvListOfValues.DataSource = _dsListOfValues;
            }

            else if (rbInschemaLovs.Checked)
            {
                var inSchemaTaxonomies =
                    _dsAttributes.First(ac => ac.Attribute.Equals(currentAttribute))
                        ._schemaDatas.Where(sd => sd.InSchema)
                        .Select(sd => sd.SchemaInfo.TaxonomyID)
                        .ToList();
                // this query must go ToList(), otherwise the results are not case sensitive. 
                var loadQuery = (from ei in _entities
                    join ed in AryaTools.Instance.InstanceData.Dc.EntityDatas on ei.ID equals ed.EntityID
                    join si in AryaTools.Instance.InstanceData.Dc.SkuInfos on ei.SkuID equals si.SkuID
                    where
                        ed.AttributeID == currentAttribute.ID && ed.Active && si.Active
                        && inSchemaTaxonomies.Contains(si.TaxonomyID)
                    select ed).ToList();

                var temp = (from ed in loadQuery
                    group ed by new {ed.Value, ed.Uom}
                    into grp
                    orderby grp.Key.Value
                    select new Lov(grp.Key.Value, grp.Key.Uom, grp.Select(ed => ed.EntityInfo)));

                _dsListOfValues = temp.ToList();

                dgvListOfValues.DataSource = _dsListOfValues;
            }

            FrmWaitScreen.HideMessage(waitKey);
        }

        private void workerThreadTimer_Tick(object sender, EventArgs e)
        {
            if (_workerThread != null)
            {
                _workerThread.Join(100);
                if (_workerThread.IsAlive)
                    return;
            }

            workerThreadTimer.Stop();
            Init();
        }

        // Internal Methods (1) 

        internal void LoadQuery(IQueryable<Sku> skuQuery, string queryDescription,
            List<TaxonomyInfo> taxonomyNodes = null)
        {
            dgvAttributes.DataSource = null;
            dgvListOfValues.DataSource = null;
            chkShowLovs.Checked = false;
            lblStatus.Text = string.Format("Loading {0}...", queryDescription);

            Show();
            BringToFront();

            _skuQuery = skuQuery;
            _queryDescription = queryDescription;

            //_entities = _skuQuery.SelectMany(sku => sku.EntityInfos);
            _entities = from sku in _skuQuery
                join ei in AryaTools.Instance.InstanceData.Dc.EntityInfos on sku.ID equals ei.SkuID
                select ei;

            if (taxonomyNodes == null)
            {
                _taxonomyNodes = (from sku in _skuQuery
                    join si in AryaTools.Instance.InstanceData.Dc.SkuInfos on sku.ID equals si.SkuID
                    join ti in AryaTools.Instance.InstanceData.Dc.TaxonomyInfos on si.TaxonomyID equals ti.ID
                    where si.Active
                    select ti).Distinct().ToList();
            }
            else
                _taxonomyNodes = taxonomyNodes;

            _taxonomy = _taxonomyNodes.Count == 1 ? _taxonomyNodes[0] : null;
            ShowHideRankColumns();

            _workerThread = new Thread(GetColumnProperties) {IsBackground = true};
            _workerThread.Start();
            workerThreadTimer.Start();
        }

        internal void LoadQuery(string queryDescription, List<TaxonomyInfo> taxonomyNodes)
        {
            dgvAttributes.DataSource = null;
            dgvListOfValues.DataSource = null;
            chkShowLovs.Checked = false;
            lblStatus.Text = string.Format("Loading {0}...", queryDescription);

            Show();
            BringToFront();

            _queryDescription = queryDescription;

            _taxonomyNodes = taxonomyNodes;

            _taxonomy = _taxonomyNodes.Count == 1 ? _taxonomyNodes[0] : null;

            //_entities = from ed in AryaTools.Instance.InstanceData.Dc.EntityDatas
            //            join ei in AryaTools.Instance.InstanceData.Dc.EntityInfos on ed.EntityID equals
            //                ei.ID
            //            join si in AryaTools.Instance.InstanceData.Dc.SkuInfos on ei.SkuID equals si.SkuID
            //            where
            //                ed.Active && si.Active &&
            //                taxonomyNodes.Select(p => p.ID).ToList().Contains(si.TaxonomyID)
            //            select ei;

            _entities = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
                where si.Active && taxonomyNodes.Select(t => t.ID).Contains(si.TaxonomyID)
                from ei in si.Sku.EntityInfos
                where ei.EntityDatas.Any(ed => ed.Active)
                select ei;

            //_entities = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
            //            join ei in AryaTools.Instance.InstanceData.Dc.EntityInfos on si.Sku equals ei.Sku
            //            where
            //                si.Active && taxonomyNodes.Select(tn => tn.ID).Contains(si.TaxonomyID)
            //                && ei.EntityDatas.Any(ed => ed.Active)
            //            select ei;

            ShowHideRankColumns();

            _workerThread = new Thread(GetColumnProperties) {IsBackground = true};
            _workerThread.Start();
            workerThreadTimer.Start();
        }

        private void ShowHideRankColumns()
        {
            var isVisible = _taxonomy != null;
            var navigationColumn = dgvAttributes.Columns["colNavigationalOrder"];
            if (navigationColumn != null)
                navigationColumn.Visible = isVisible;

            var displayColumn = dgvAttributes.Columns["colDisplayOrder"];
            if (displayColumn != null)
                displayColumn.Visible = isVisible;

            var rankedColumn = dgvAttributes.Columns["colIsRanked"];
            if (rankedColumn != null)
                rankedColumn.Visible = !isVisible;

            var mvColumn = dgvAttributes.Columns["colMultiValued"];
            if (mvColumn != null)
                mvColumn.Visible = _showMvColumn;
        }

        #endregion Methods

        #region Nested Classes (2)

        private class Lov:ISpell
        {
            private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
            KeyValuePair<bool, string> ISpell.Updatable
            {
                get
                {
                    return (KeyValuePair<bool, string>)_updatable;
                }
            }
            #region Fields (2)

            private string _uom;
            private string _value;
           

            #endregion Fields

            #region Constructors (1)

            public Lov(string value, string uom, IEnumerable<EntityInfo> entityInfos)
            {
                _value = value;
                _uom = uom;
                EntityInfos = entityInfos.Distinct(new KeyEqualityComparer<EntityInfo>(ei => ei.ID)).ToList();
            }

            #endregion Constructors

            #region Properties (4)

            public int Count
            {
                get { return EntityInfos.Count; }
            }

            private List<EntityInfo> EntityInfos { get; set; }

            public string Uom
            {
                get { return _uom; }
                set
                {
                    //exact duplicate
                    if (_uom == value)
                        return;

                    _uom = value;
                    var waitKey = FrmWaitScreen.ShowMessage("Updating UOMs");

                    //remove this attribute from the cache
                    Attribute toBeRemovedAttribute = null;

                    var iCtr = 0;
                    var lastUpdated = DateTime.UtcNow;

                    foreach (var ei in EntityInfos)
                    {
                        var ed = ei.EntityDatas.OrderByDescending(e => e.CreatedOn).FirstOrDefault(e => e.Active);
                        if (ed == null)
                            continue;

                        ei.EntityDatas.Where(e => e.Active).ForEach(e => e.Active = false);
                        //ed.Active = false;
                        toBeRemovedAttribute = ed.Attribute;

                        ei.EntityDatas.Add(new EntityData
                                           {
                                               Attribute = ed.Attribute,
                                               Field1 = ed.Field1,
                                               Field2 = ed.Field2,
                                               Field3 = ed.Field3,
                                               Field4 = ed.Field4, //Field5orStatus = ed.Field5OrStatus,
                                               Value = ed.Value,
                                               Uom = value
                                           });

                        ++iCtr;
                        if (DateTime.UtcNow.Subtract(lastUpdated).TotalMilliseconds > 500)
                        {
                            FrmWaitScreen.UpdateMessage(waitKey,
                                string.Format("Updating UOMs ({0} of {1})", iCtr, EntityInfos.Count));
                            lastUpdated = DateTime.UtcNow;
                        }
                    }

                    FrmWaitScreen.HideMessage(waitKey);
                    if (toBeRemovedAttribute != null
                        && AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(toBeRemovedAttribute))
                        AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[toBeRemovedAttribute].Clear();

                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                }
            }

            public string Value
            {
                get { return _value; }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        MessageBox.Show(@"Value cannot be empty");
                        return;
                    }

                    //exactly same value, bad edit
                    if (_value == value)
                        return;

                    _value = value;
                    var waitKey = FrmWaitScreen.ShowMessage("Updating Values");

                    //remove this attribute from the cache
                    Attribute toBeRemovedAttribute = null;

                    var iCtr = 0;
                    var lastUpdated = DateTime.UtcNow;

                    foreach (var ei in EntityInfos)
                    {
                        var ed = ei.EntityDatas.OrderByDescending(e => e.CreatedOn).FirstOrDefault(e => e.Active);
                        if (ed == null)
                            continue;

                        ei.EntityDatas.Where(e => e.Active).ForEach(e => e.Active = false);
                        //ed.Active = false;
                        toBeRemovedAttribute = ed.Attribute;

                        ei.EntityDatas.Add(new EntityData
                                           {
                                               Attribute = ed.Attribute,
                                               Field1 = ed.Field1,
                                               Field2 = ed.Field2,
                                               Field3 = ed.Field3,
                                               Field4 = ed.Field4, //Field5orStatus = ed.Field5OrStatus,
                                               Uom = ed.Uom,
                                               Value = value
                                           });

                        ++iCtr;
                        if (DateTime.UtcNow.Subtract(lastUpdated).TotalMilliseconds > 500)
                        {
                            FrmWaitScreen.UpdateMessage(waitKey,
                                string.Format("Updating Values ({0} of {1})", iCtr, EntityInfos.Count));
                            lastUpdated = DateTime.UtcNow;
                        }
                    }

                    FrmWaitScreen.HideMessage(waitKey);
                    if (toBeRemovedAttribute != null
                        && AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(toBeRemovedAttribute))
                        AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[toBeRemovedAttribute].Clear();

                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                }
            }

            #endregion Properties

            string ISpell.GetType()
            {
                return this.GetType().ToString();
            }

            Dictionary<string, string> ISpell.GetSpellValue()
            {
                var   _propertyNameValue = new Dictionary<string, string>();
                    _propertyNameValue = new Dictionary<string, string>();
                    _propertyNameValue.Add("Value", Value);
                return _propertyNameValue;                
            }

            Guid ISpell.GetId()
            {
                return EntityInfos.First().ID;
            }

            ISpell ISpell.SetValue(string propertyName, string value)
            {
                if (propertyName.ToLower() == "value")
                {
                    Value = value;                    
                }
                return this;
            }

            string ISpell.GetLocation()
            {
                return "Test Location for Lov.";
            }
        }

        #endregion Nested Classes

        private void rbAllLovs_CheckedChanged(object sender, EventArgs e)
        {
            _currentLovAttribute = null;
            TryLoadLovs();
        }

        private void rbInschemaLovs_CheckedChanged(object sender, EventArgs e)
        {
            _currentLovAttribute = null;
            TryLoadLovs();
        }

        private void btnExportData_Click(object sender, EventArgs e)
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

            attributeData.AppendLine(headerString);

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

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (Owner != null)
            {
                var currentLocation = Location;
                Location = new Point(currentLocation.X, currentLocation.Y + 10);
            }
        }

        private void btnAttributePreferences_Click(object sender, EventArgs e)
        {
            var attrPrefs = new FrmUserPreferences();
            attrPrefs.ShowDialog(this);
        }

        private void btnSavePrefs_Click(object sender, EventArgs e)
        {
            if (dgvAttributes.IsCurrentCellInEditMode)
                dgvAttributes.EndEdit();

            if (_workerThread != null && _workerThread.IsAlive)
                _workerThread.Abort();
        }

        private void btnApply_Click(object sender, EventArgs e) { _applyAttributes = true; }

        private void dgvAttributes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.CellStyle.FormatProvider is ICustomFormatter)
            {
                e.Value =
                    ((ICustomFormatter) e.CellStyle.FormatProvider.GetFormat(typeof (ICustomFormatter))).Format(
                        e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }
        }

        
        private void DisplaySpellCheck(List<ISpell> values, Action invalidateDsListOfValues, string contextMsg)
        {
            try
            {
                var spellCheckForm = AryaTools.Instance.Forms.SpellCheckForm;
                if (values.Count() > 0)
                {
                    spellCheckForm.UpdateSpellCheckView(values, contextMsg);
                    spellCheckForm.InvalidateMethod = invalidateDsListOfValues;
                    var result = spellCheckForm.ShowDialog();
                }
            }
            catch (ObjectDisposedException ex)
            {
                //do nothing
            }
        }
        
        void InvalidateDsListOfValues()
        {
            dgvListOfValues.Invalidate();
        }
        void InvalidatesDgvAttributes()
        {
            dgvAttributes.Invalidate();
        }

        private void btnCheckSpelling_Click(object sender, EventArgs e)
        {
            if (dgvListOfValues.DataSource != null)
            {
                if (dgvListOfValues.CurrentCell != null && dgvListOfValues.CurrentCell.ColumnIndex == 1)
                {
                    List<ISpell> values = _dsListOfValues.Cast<ISpell>().ToList();
                    DisplaySpellCheck(values, InvalidateDsListOfValues, Arya.Properties.Resources.SpellCheckContextMessageForLov.ToString());
                }
            }
            else
            {
                //IEnumerable<AttributeColumn> currentDataSource = null;
                //var attributeGroupFilter = ddAttributeGroupFilter.Text;
                //if (!string.IsNullOrEmpty(attributeGroupFilter) && !attributeGroupFilter.Equals(AllAttributeGroups))
                //{
                //    currentDataSource = attributeGroupFilter.Equals(DefaultAttributeGroup)
                //        ? _dsAttributes.Where(a => a.Attribute.Group == null)
                //        : _dsAttributes.Where(
                //            a => a.Attribute.Group != null && a.Attribute.Group.Equals(attributeGroupFilter));
                //}
                //dgvAttributes.DataSource.
                List<ISpell> values = ((List<AttributeColumn>)dgvAttributes.DataSource).Cast<ISpell>().ToList();
                DisplaySpellCheck(values, InvalidatesDgvAttributes, Arya.Properties.Resources.SpellCheckContextMessageForAttribute.ToString());

            }
        }
       
    }
}