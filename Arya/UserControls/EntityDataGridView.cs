using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using DataGridViewAutoFilter;
using LinqKit;
using Arya.Data;
using Arya.Framework.Collections.Generic;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO;
using Arya.Framework4.Browser.HtmlTemplates;
using Arya.Framework4.State;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.Properties;
using Attribute = Arya.Data.Attribute;
using DerivedAttribute = Arya.Data.DerivedAttribute;
using EntityData = Arya.Data.EntityData;
using EntityInfo = Arya.Data.EntityInfo;
using ProjectUom = Arya.Data.ProjectUom;
using Sku = Arya.Data.Sku;
using SkuInfo = Arya.Data.SkuInfo;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;
using Arya.Framework.Properties;
using SchemaAttribute = Arya.HelperClasses.SchemaAttribute;
using UnitOfMeasure = Arya.Data.UnitOfMeasure;
using Arya.Framework.Utility;
using Arya.SpellCheck;

namespace Arya.UserControls
{
    public partial class EntityDataGridView : UserControl
    {
        #region Fields (72)

        private const string Bullet = " • ";

        public const int DefaultColumnWidth = 100;
        public const string BlankValue = "<blank>";
        public const string EmptyValue = "<empty value>";
        private const string FilterAttribute = "Filter Attribute";
        private const string FilterBlankValues = "Filter <blank> values";
        private const string FilterColor = "Filter Color";
        private const string FilterFieldPrefix = "Filter ";
        private const string FilterTaxonomyFieldname = "---taxonomy---";
        private const string FilterUom = "Filter Uom";
        private const string FilterValue = "Filter Value";
        private const string NotInSchema = " ^ ";
        private const string SortColor = "Sort Color";
        private const string SortFieldPrefix = "Sort ";
        private const string SortUom = "Sort Uom";
        private const string SortValue = "Sort Value";
        private const int MinCellCount = 25;
        private const string DefaultStatus = "Ready";
        private static readonly DataGridViewCell CellTextBoxTemplate = new DataGridViewTextBoxCell();
        private static readonly object lockObj = new object();
        internal readonly TaxonomyInfo CurrentTaxonomy;
        //List<Sku> loadQuerySkus = new List<Sku>();

        private readonly List<ColorRuleAttribute> _colorRuleAttributes = new List<ColorRuleAttribute>
                                                                             {
                                                                                 FrmColorRules.RuleValue,
                                                                                 FrmColorRules.RuleUom,
                                                                                 FrmColorRules.RuleCreatedOn,
                                                                                 FrmColorRules.RuleLastUpdatedOn,
                                                                                 FrmColorRules.RuleSchematus,
                                                                                 FrmColorRules.SpellCheck
                                                                             };

        private readonly List<FilterEntity> _currentFilters = new List<FilterEntity>();
        private readonly Guid _instanceId;
        private readonly IQueryable<Sku> _loadQuery;
        private readonly Dictionary<Attribute, int> _noOfColumns = new Dictionary<Attribute, int>();
        private readonly Stack<ChangeItem> _redoHistory = new Stack<ChangeItem>();

        private readonly StringFormat _rightAlign = new StringFormat
                                                        {
                                                            Alignment = StringAlignment.Far,
                                                            Trimming = StringTrimming.EllipsisCharacter,
                                                            FormatFlags = StringFormatFlags.NoWrap
                                                        };

        private readonly Stack<ChangeItem> _undoHistory = new Stack<ChangeItem>();

        private readonly DoubleKeyDictionary<TaxonomyInfo, Attribute, string[]> _validValues =
            new DoubleKeyDictionary<TaxonomyInfo, Attribute, string[]>();

        private readonly Dictionary<Guid, bool> _validatedData = new Dictionary<Guid, bool>();
        private readonly Thread _wikiThread;

        private readonly Type[] allowedDestinationTypes = new[]
                                                              {
                                                                  typeof (TextBox), typeof (ComboBox),
                                                                  typeof (SplitContainer), typeof (DataGridView)
                                                              };

        private readonly Template assetTemplate;
        private readonly DataGridViewCellStyle boldStyle = new DataGridViewCellStyle { ForeColor = Color.Blue };
        private readonly AssetCache imageCache = new AssetCache();
        private readonly DataGridViewCellStyle normalStyle = new DataGridViewCellStyle { ForeColor = Color.Black };
        private readonly Random rnd = new Random(DateTime.Now.Millisecond);
        private readonly Regex rxValidSearchString = new Regex("^[a-zA-Z -]+$", RegexOptions.Compiled);
        internal bool AbortWorker;
        public Change CurrentChange;
        internal Thread WorkerThread;

        private Dictionary<string, Attribute> _allAttributes;
        private bool _allowCreateEntities;
        private bool _allowDeleteEntities;

        private List<ColorRule> _colorRules = new List<ColorRule>();
        private bool _createNewEntityData;
        private List<TaxonomyInfo> _currentTaxonomies;
        private HashSet<string> _productAttributeNames;
        private FrmFindReplace _findReplaceWindow;
        private HashSet<string> _inSchemaAttributeNames;
        private List<Sku> _loadQueryResult;
        //private string _getImageFrom = string.Empty;

        private Dictionary<object, CustomKeyValuePair<Color, Color>> _objectColors =
            new Dictionary<object, CustomKeyValuePair<Color, Color>>();

        private bool _populatingGridView;
        private bool _populatingProperties;
        private List<AttributeColumn> _possibleAttributeColumnsForSkuView;
        private HashSet<string> _rankedAttributeNames;

        private bool _selectingManyCells;
        private bool _showAttributesWhenSkuThresholdExceeds = true;
        private bool _showAuditTrail;
        private bool _showLinks;
        private bool _showShowHideAttributeWindow = true;
        private bool _showSkuThresholdWindow = true;
        private Dictionary<Guid, Sku> _skus;
        private DateTime _snippetBrowserLastUpdated = DateTime.MinValue;
        private string _snippetCurrentCellValue = String.Empty;
        private DateTime _snippetDocumentLastUpdated = DateTime.MinValue;
        private string _snippetDocumentText;
        private Uri _snippetUrl;
        private bool _snippetValueChanged;
        private List<TaxonomyInfo> _taxonomyNodes;
        private bool _uncheckingEntityNameCheckboxes;
        private DateTime _workerStartTime;
        private WorkerState _workerState = WorkerState.Ready;
        private string _workerStatus = String.Empty;
        private Color defaultBackColor;
        private Color defaultForeColor;
        private string selectedWorkflow;
       

        #endregion Fields

        #region Constructors (3)

        internal EntityDataGridView(IQueryable<Sku> loadQuery, TaxonomyInfo taxonomy = null)
            : this()
        {
            CurrentTaxonomy = taxonomy;
            _loadQuery = loadQuery;

            if (AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls.Count == 1)
                assetTemplate = new ImageUrlTemplate();
            else if (AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls.Count > 1)
                assetTemplate = new ImagesUrlTemplate();

            InitData();

            SetAuditTrailStyles();
        }

        private EntityDataGridView()
        {
            InitializeComponent();
            _instanceId = Guid.NewGuid();
           
            DisplayStyle.SetDefaultFont(this);

            sliceDelimiterToolStripMenuItem.ComboItems.AddRange(new object[] { "Comma", "Tab", "Space" });
            sliceDelimiterToolStripMenuItem.ComboBox.SelectedIndex = 0;

            noOfDecimalPlacesToolStripMenuItem.ComboItems.AddRange(new object[] { "0", "1", "2", "3", "4" });
            noOfDecimalPlacesToolStripMenuItem.ComboBox.SelectedIndex = 2;

            aggregateFunctionToolStripMenuItem.ComboItems.AddRange(new object[] { "Minimum", "Maximum", "Count" });
            aggregateFunctionToolStripMenuItem.ComboBox.SelectedIndex = 2;

            convertUomPrecisionDropDown.ComboItems.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" });
            noOfDecimalPlacesToolStripMenuItem.ComboBox.SelectedIndex = 2;

            (_wikiThread = new Thread(SnippetWorker) { IsBackground = true }).Start();

            var pictureBoxUrl = AryaTools.Instance.InstanceData.CurrentProject.PictureBoxUrl;
            loadAllImagesToolStripMenuItem.Enabled = pictureBoxUrl != null;
            if (AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.ValidateAttributeValues)
            {
                validateToolStripMenuItem.Checked = true;
                edgv.Invalidate();
            }
        }

        ~EntityDataGridView()
        {
            AbortWorker = true;
            if (WorkerThread != null)
                WorkerThread.Join();
            if (_wikiThread != null)
                _wikiThread.Join();
        }

        #endregion Constructors

        #region Properties

        private List<TaxonomyInfo> CurrentTaxonomies
        {
            get
            {
                if (_currentTaxonomies == null)
                {
                    //var queryList = _loadQuery.ToList();
                    _currentTaxonomies = LoadQueryResults.Select(s => s.Taxonomy).Where(t => t != null).Distinct().ToList();
                }
                return _currentTaxonomies;
            }
        }

        private HashSet<string> InSchemaAttributeNames
        {
            get
            {
                if (_inSchemaAttributeNames == null && CurrentTaxonomies != null)
                {
                    //_inSchemaAttributeNames = new HashSet<string>(from si in CurrentTaxonomies.First().SchemaInfos
                    //                                              let sd = si.SchemaData
                    //                                              where sd != null && sd.InSchema
                    //                                              select si.Attribute.AttributeName);
                    _inSchemaAttributeNames =
                        new HashSet<string>(
                            CurrentTaxonomies.SelectMany(tax => tax.SchemaInfos).Select(si => si.SchemaData).Where(
                                sd => sd != null && sd.InSchema).Select(sd => sd.SchemaInfo.Attribute.AttributeName).
                                Distinct());
                }
                return _inSchemaAttributeNames;
            }
        }

        private HashSet<string> RankedAttributeNameNames
        {
            get
            {
                if (_rankedAttributeNames == null)
                {
                    _rankedAttributeNames =
                        new HashSet<string>(
                            CurrentTaxonomies.SelectMany(tax => tax.SchemaInfos).Select(si => si.SchemaData).Where(
                                sd => sd != null && (sd.DisplayOrder > 0 || sd.NavigationOrder > 0)).Select(
                                    sd => sd.SchemaInfo.Attribute.AttributeName).Distinct());
                }
                return _rankedAttributeNames;
            }
        }

        private HashSet<string> ProductAttributeNames
        {
            get
            {
                if (_productAttributeNames == null)
                {
                    var allAttributes = PossibleAttributeColumnsForSkuView.ToDictionary(c => c.AttributeName,
                                                                                        c => c.Attribute);
                    // GetCurrentSkuAttributes();
                    _productAttributeNames =
                        new HashSet<string>(
                            allAttributes.Where(col => col.Value.Type == AttributeTypeEnum.Sku).Select(
                                col => col.Value.AttributeName));
                    //_productAttributeNames = new HashSet<string>(
                    //        allAttributes.Where(
                    //            col =>
                    //            !InSchemaAttributeNames
                    //                 .Union(RankedAttributeNameNames)
                    //                 .Union(AryaTools.Instance.InstanceData.GlobalAttributeNames)
                    //                 .Contains(col.Value.AttributeName)).Select(col => col.Value.AttributeName));
                }
                return _productAttributeNames;
            }
        }

        //TODO: Implement this property so that it carries only the columns that are in datagrid. no check on visible 
        private List<AttributeColumn> PossibleAttributeColumnsForSkuView
        {
            get
            {
                return _possibleAttributeColumnsForSkuView
                       ?? (_possibleAttributeColumnsForSkuView = new List<AttributeColumn>());
            }
            set { _possibleAttributeColumnsForSkuView = value; }
        }

        private bool CreateNewEntityData
        {
            get { return _createNewEntityData; }
            set
            {
                if (value.Equals(_createNewEntityData))
                    return;

                newEntitiesToolStripMenuItem.Checked = value;
                _createNewEntityData = value;

                if (value && CurrentChange != null)
                {
                    int iteration;
                    string fieldName;
                    var headerText = edgv.Columns[edgv.CurrentCellAddress.X].HeaderText;

                    SplitHeaderText(headerText, out fieldName, out iteration);

                    var currentAttribute = Attribute.GetAttributeFromName(fieldName, false);

                    if (currentAttribute != null
                        && currentAttribute.AttributeType == AttributeTypeEnum.Derived.ToString())
                    {
                        var data =
                            edgv.SelectedCells.Cast<DataGridViewCell>().Select(
                                cell =>
                                new { sku = (Sku)edgv[0, cell.RowIndex].Value, skuValue = (EntityData)cell.Value }).
                                ToList();

                        foreach (var currentSku in data)
                        {
                            var newEntityInfo = new EntityInfo();
                            newEntityInfo.EntityDatas.Add(new EntityData(true)
                                                              {
                                                                  Value = currentSku.skuValue.Value,
                                                                  Attribute = currentAttribute,
                                                                  Uom = currentSku.skuValue.Uom
                                                              });

                            currentSku.sku.EntityInfos.Add(newEntityInfo);
                        }
                    }

                    //chkCreateNewEntityDatas.Image = Resources.Accept;
                    if (!tblEntity.Enabled && CurrentChange.GetBlanks().Count > 0)
                    {
                        tblEntity.Enabled = true;
                        if (!fieldName.Equals(Arya.Framework.Properties.Resources.NewAttributeName))
                        {
                            cboAttributeName.Text = fieldName;
                            txtValue.Focus();
                        }
                        else
                            cboAttributeName.Focus();
                    }
                    lblAttributeValue.Image = Arya.Properties.Resources.Add;
                }
                else
                {
                    lblAttributeValue.Image = null;
                    //chkCreateNewEntityDatas.Image = Resources.Add;
                    PopulateProperties();
                }
            }
        }

        private FrmFindReplace FindReplaceWindow
        {
            get
            {
                if (_findReplaceWindow == null || _findReplaceWindow.IsDisposed)
                    _findReplaceWindow = new FrmFindReplace(this);

                return _findReplaceWindow;
            }
        }

        public bool ShortcutKeysEnabled
        {
            set
            {
                Forms.EnableDisableMenuItems(mainMenuStrip.Items, value);
                if (CurrentTaxonomy != null)
                    openInNewTabToolStripMenuItem.Enabled = false;
            }
        }

        private bool ShowAuditTrail
        {
            get { return _showAuditTrail; }
            set
            {
                _showAuditTrail = value;
                auditTrailToolStripMenuItem.Checked = _showAuditTrail;
                btnShowHideAuditTrail.CheckedChanged -= btnShowHideAuditTrail_CheckedChanged;
                btnShowHideAuditTrail.Checked = _showAuditTrail;
                btnShowHideAuditTrail.CheckedChanged += btnShowHideAuditTrail_CheckedChanged;
                btnShowHideAuditTrail.Image = _showAuditTrail ? Arya.Properties.Resources.BookOpened : Arya.Properties.Resources.BookClosed;

                dgvAuditTrail.Visible = value;
                splitterAuditTrail.Visible = value;
                PopulateAuditTrail();
            }
        }

        private bool ShowItemImagesInBrowser { get; set; }

        private bool ShowItemOnClientWebsite { get; set; }

        private bool ShowLinks
        {
            get { return _showLinks; }
            set
            {
                _showLinks = value;
                showHideSKULinksToolStripMenuItem.Checked = value;
                chkShowHideLinks.Checked = value;
                PopulateLinks();
            }
        }

        private bool ShowResearchPages { get; set; }

        private bool ShowUrlPages { get; set; }

        #endregion Properties

        #region Nested type: ChangeItem

        public class ChangeItem
        {
            internal readonly Guid _changeId;
            private readonly ChangeType _changeType;
            private readonly SkuInfo _newSkuInfo;
            internal readonly EntityData _oldEntityData;
            private readonly SkuInfo _oldSkuInfo;
            internal EntityData _newEntityData;

            public ChangeItem(EntityData oldEntityData, EntityData newEntityData, Guid changeId)
            {
                _changeType = ChangeType.EntityData;
                _oldEntityData = oldEntityData;
                _newEntityData = newEntityData;
                _changeId = changeId;
            }

            public ChangeItem(SkuInfo oldSkuInfo, SkuInfo newSkuInfo, Guid changeId)
            {
                _changeType = ChangeType.SkuInfo;
                _oldSkuInfo = oldSkuInfo;
                _newSkuInfo = newSkuInfo;
                _changeId = changeId;
            }

            public Sku Undo(Stack<ChangeItem> undoHistory, Stack<ChangeItem> redoHistory,
                            out EntityData resultantEntityData)
            {
                Sku skuToRefresh = null;
                resultantEntityData = null;
                switch (_changeType)
                {
                    case ChangeType.EntityData:
                        EntityData newEntityData = null;

                        if (_newEntityData != null)
                        {
                            _newEntityData.Active = false;
                            skuToRefresh = _newEntityData.EntityInfo.Sku;

                            if (
                                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(
                                    _newEntityData.Attribute, _newEntityData.EntityInfo.Sku))
                            {
                                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[_newEntityData.Attribute].
                                    Remove(_newEntityData.EntityInfo.Sku);
                            }
                        }

                        if (_oldEntityData != null)
                        {
                            var entityInfo = _oldEntityData.EntityInfo;
                            newEntityData = new EntityData
                                                {
                                                    Attribute = _oldEntityData.Attribute,
                                                    Value = _oldEntityData.Value,
                                                    Uom = _oldEntityData.Uom,
                                                    Field1 = _oldEntityData.Field1,
                                                    Field2 = _oldEntityData.Field2,
                                                    Field3 = _oldEntityData.Field3,
                                                    Field4 = _oldEntityData.Field4 //,
                                                    //Field5orStatus = _oldEntityData.Field5OrStatus,
                                                };
                            entityInfo.EntityDatas.Add(newEntityData);

                            var oldEntityDataCreator =
                                undoHistory.FirstOrDefault(
                                    ci => ci._newEntityData != null && ci._newEntityData.ID == _oldEntityData.ID);
                            if (oldEntityDataCreator != null)
                                oldEntityDataCreator._newEntityData = newEntityData;

                            skuToRefresh = entityInfo.Sku;

                            if (
                                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(
                                    _oldEntityData.Attribute, skuToRefresh))
                                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[_oldEntityData.Attribute].
                                    Remove(skuToRefresh);
                        }

                        if (redoHistory != null)
                            redoHistory.Push(new ChangeItem(_newEntityData, newEntityData, _changeId));

                        resultantEntityData = _oldEntityData ?? _newEntityData;
                        break;

                    case ChangeType.SkuInfo:
                        skuToRefresh = _newSkuInfo.Sku;

                        _newSkuInfo.Active = false;
                        var newSkuInfo = new SkuInfo { TaxonomyInfo = _oldSkuInfo.TaxonomyInfo };
                        _newSkuInfo.Sku.SkuInfos.Add(newSkuInfo);

                        if (redoHistory != null)
                            redoHistory.Push(new ChangeItem(_newSkuInfo, newSkuInfo, _changeId));

                        break;
                }
                return skuToRefresh;
            }

            #region Nested type: ChangeType

            private enum ChangeType
            {
                EntityData,
                SkuInfo
            };

            #endregion
        }

        #endregion

        #region Nested Classes (1)

        public class FilterEntity
        {
            #region Properties (3)

            public string Field { get; set; }

            public List<string> FilterValues { get; set; }

            public string LowerAttributeName { get; set; }

            #endregion Properties
        }

        #endregion Nested Classes

        #region Nested type: InvokeDelegate

        private delegate void InvokeDelegate();

        #endregion

        #region Methods

        private List<Sku> LoadQueryResults
        {
            get
            {
                if (_loadQueryResult == null)
                    _loadQueryResult = _loadQuery.ToList();

                return _loadQueryResult;
            }
            //set;
        }

        private SortOption LastSortOption { get; set; }

        private void SetAuditTrailStyles()
        {
            //Set Alternating colors for AuditTrail
            dgvAuditTrail.DoubleBuffered(true);
            dgvAuditTrail.RowsDefaultCellStyle.BackColor = DisplayStyle.CellStyleGreyRegular.BackColor;
            dgvAuditTrail.AlternatingRowsDefaultCellStyle.BackColor = DisplayStyle.CellStyleOddRow.BackColor;
            dgvAuditTrail.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            dgvAuditTrail.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            dgvAuditTrail.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            dgvAuditTrail.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
        }

        public void DeselectCurrentSelection()
        {
            var oldSelectingManyCells = _selectingManyCells;
            _selectingManyCells = true;
            edgv.SelectedCells.Cast<DataGridViewCell>().ForEach(cell => cell.Selected = false);
            _selectingManyCells = oldSelectingManyCells;
            SelectionChanged();
        }

        public void InvokeInvalidateMethod() { edgv.Invalidate(); }

        public void SaveCurrentChangeAndGetNewChange(bool forceCancelAutoSave)
        {
            CreateNewEntitiesFromChange(CurrentChange, forceCancelAutoSave);
            GetSelectedCells();
            PopulateProperties();
        }

        public void SelectSku(Guid skuID, int columnIndex)
        {
            var rowIndex = AryaTools.Instance.Forms.SkuOrders[_instanceId].IndexOf(skuID);
            DeselectCurrentSelection();

            if (edgv.RowCount == 0 || edgv.ColumnCount == 0)
                return;

            if (edgv.ColumnCount <= columnIndex)
                columnIndex = 0;

            edgv[columnIndex, rowIndex].Selected = true;
        }

        public void UnCheckShowHideLinks() { chkShowHideLinks.Checked = false; }

        public void UnCheckShowItemImagesMenuItem()
        {
            showItemImagesToolStripMenuItem.Checked = false;
            ShowItemImagesInBrowser = false;
        }

        private void _newAttributeDropDown_Leave(object sender, EventArgs e)
        {
            //_newAttributeDropDown.Text = string.Empty;
            HandleEnterAndEscapeForChange(Keys.Enter);
            _newAttributeDropDown.Hide();
        }

        private void _newAttributeDropDown_TextUpdate(object sender, EventArgs e)
        {
            SetCurrentChangeNewAttribute(_newAttributeDropDown.Text);
            // MessageBox.Show("Invalid New Attribute Name " + _newAttributeDropDown.Text);
        }

        private List<Guid> ApplyCurrentFilters()
        {
            return _currentFilters.Aggregate(_skus.Keys.ToList(),
                                             (current, filter) =>
                                             ApplyFilter(current, filter.LowerAttributeName, filter.FilterValues,
                                                         filter.Field));
        }

        private List<Guid> ApplyFilter(IEnumerable<Guid> skuOrder, string fieldName, List<string> selectedItems,
                                       string field)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Applying Filter");
            var blankSelected = false;
            List<string> filterItems;
            if (selectedItems.Contains(BlankValue))
            {
                blankSelected = true;
                filterItems = selectedItems.Where(item => !item.Equals(BlankValue)).ToList();
            }
            else
                filterItems = selectedItems;
            var newSkuOrder = new List<Guid>();
            if (fieldName.Equals(FilterTaxonomyFieldname))
            {
                skuOrder.AsParallel().AsOrdered().ForEach(skuId =>
                                                              {
                                                                  if (
                                                                      filterItems.Contains(
                                                                          _skus[skuId].Taxonomy.ToString()))
                                                                      newSkuOrder.Add(skuId);
                                                              });
            }
            else
            {
                skuOrder.AsParallel().AsOrdered().ForEach(skuId =>
                                                              {
                                                                  IEnumerable<EntityData> skuAttributeValues =
                                                                      _skus[skuId].GetValuesForAttribute(fieldName);

                                                                  if ((blankSelected && !skuAttributeValues.Any())
                                                                      ||
                                                                      skuAttributeValues.Select(
                                                                          ed => GetSortFilterValue(ed, field)).Where(
                                                                              filterItems.Contains).Any())
                                                                      newSkuOrder.Add(skuId);
                                                              });
            }

            FrmWaitScreen.HideMessage(waitkey);
            return newSkuOrder;
        }

        private int ApplyFilter(string fieldName, List<string> selectedItems, bool clearThisFilter, bool clearAllFilters,
                                string field)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Applying Filter");

            List<Guid> newSkuOrder;
            //Clear all filters
            if (clearAllFilters)
            {
                _currentFilters.Clear();
                newSkuOrder = _skus.Keys.ToList();
            }
            //Clear current filter
            else if (clearThisFilter)
            {
                var filterToRemove = _currentFilters.FirstOrDefault(f => f.LowerAttributeName.Equals(fieldName));
                if (filterToRemove != null)
                {
                    //Remove current filter
                    _currentFilters.Remove(filterToRemove);

                    // re-apply all other filters
                    newSkuOrder = ApplyCurrentFilters();
                }
                else
                    newSkuOrder = AryaTools.Instance.Forms.SkuOrders[_instanceId];
            }
            else
            {
                var filterToUpdate = _currentFilters.FirstOrDefault(f => f.LowerAttributeName.Equals(fieldName));
                if (filterToUpdate != null)
                {
                    //existing filter
                    filterToUpdate.FilterValues = selectedItems;

                    //any change to an existing filters warrants re-application of all other filters
                    newSkuOrder = ApplyCurrentFilters();
                }
                else
                {
                    //new filter
                    _currentFilters.Add(new FilterEntity
                                            {
                                                LowerAttributeName = fieldName,
                                                Field = field,
                                                FilterValues = selectedItems
                                            });

                    var skuOrder = AryaTools.Instance.Forms.SkuOrders[_instanceId];
                    newSkuOrder = ApplyFilter(skuOrder, fieldName, selectedItems, field);
                }
            }
            var oldOrder = AryaTools.Instance.Forms.SkuOrders[_instanceId];
            AryaTools.Instance.Forms.SkuOrders[_instanceId] = newSkuOrder.OrderBy(oldOrder.IndexOf).ToList();

            FrmWaitScreen.HideMessage(waitkey);
            return newSkuOrder.Count;
        }

        private static string ApplyFont(string html) { return String.Format("<div style=\"font-family:arial; font-size:8pt;\">{0}</div>", html); }

        private void AttributeOrderVisibility()
        {
            var attrView = new FrmAttributeView(PossibleAttributeColumnsForSkuView, _taxonomyNodes) { Text = "Show/Hide Attributes" }; // { Height = 500 };
            var applyAttributes = attrView.ShowDialog(this);
            var sw = new Stopwatch();
            sw.Start();
            if (applyAttributes == DialogResult.OK)
            {
                //RedrawDataGridView_Test(null);
                PopulateGridView(PossibleAttributeColumnsForSkuView);
                UpdateFilterStatus();
            }
            sw.Stop();
            Diagnostics.WriteMessage("*Redrawing Grid View from Attribute View",
                                     "EnttityDataGridView - AttributeOrderVisibility", sw.Elapsed,
                                     AryaTools.Instance.Forms.SkuOrders[_instanceId].Count, _taxonomyNodes.Count,
                                     PossibleAttributeColumnsForSkuView.Count(v => v.Visible));
            sw.Reset();
        }

        public void AttributePreferenceVisibility()
        {
            var userprefs = new FrmUserPreferences { };
            userprefs.ShowDialog(this);
            var sw = new Stopwatch();
            sw.Start();
            UpdateFilterStatus();
            sw.Stop();
            Diagnostics.WriteMessage("*Redrawing Grid View from Attribute Preferences",
                                     "EntityDataGridView - AttributePreferenceVisibility", sw.Elapsed,
                                     AryaTools.Instance.Forms.SkuOrders[_instanceId].Count);
            sw.Reset();
        }

        private void auditTrailToolStripMenuItem_Click(object sender, EventArgs e) { ShowAuditTrail = auditTrailToolStripMenuItem.Checked; }

        private void btnAttributeOrderVisibility_Click(object sender, EventArgs e) { AttributeOrderVisibility(); }

        private void btnColor_Click(object sender, EventArgs e) { GetColorRules(); }

        private void btnFilter_Click(object sender, EventArgs e) { filterContextMenu.Show(btnFilter, 0, btnFilter.Height); }

        private void btnFind_Click(object sender, EventArgs e) { FindReplace(); }

        private void btnReOrderColumns_Click(object sender, EventArgs e)
        {
            //var attIds = attributeColumns.Select(ac => ac.Attribute.ID).ToHashSet();
            ReorderColumns();
        }

        private void ReorderColumns()
        {
            var attributePrefs = AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences;
            var toAdd = (from att in _allAttributes
                         where
                             ((att.Value.Type == AttributeTypeEnum.Sku || att.Value.Type == AttributeTypeEnum.Derived
                               || att.Value.Type == AttributeTypeEnum.Flag || att.Value.Type == AttributeTypeEnum.Global)
                              && !att.Value.AttributeName.Equals(Arya.Framework.Properties.Resources.NewAttributeName))
                         select
                             new AttributeColumn(att.Value, CurrentTaxonomies, null)
                             {
                                 Attribute = att.Value,
                                 ColumnWidth = EntityDataGridView.DefaultColumnWidth,
                                 Visible = true
                             }).ToList();

            PossibleAttributeColumnsForSkuView = toAdd;


            foreach (var ac in PossibleAttributeColumnsForSkuView)
            {
                //bool treated = false;
                ac.Visible = (ac.Visible ||
                                  (ac.InSchema != CheckState.Unchecked && attributePrefs.InSchemaAttributes) ||
                                  (ac.IsRanked != CheckState.Unchecked && attributePrefs.RankedAttributes) ||
                                  (ac.Attribute.Type == AttributeTypeEnum.Global && attributePrefs.GlobalAttributes) ||
                                  (ac.Attribute.Type == AttributeTypeEnum.Sku && attributePrefs.ProductAttributes) ||
                                  attributePrefs.AttributeGroupInclusions.Contains(ac.AttributeGroup) ||
                                  attributePrefs.AttributeCustomInclusions.Contains(ac.AttributeName)
                                 );
                ////InSchema User Preference
                //if (ac.InSchema != CheckState.Unchecked)
                //{
                //    ac.Visible = (attributePrefs.InSchemaAttributes || ac.Visible);
                //    treated = true;
                //}

                ////IsRanked User Preference
                //if (ac.IsRanked != CheckState.Unchecked)
                //{
                //    ac.Visible = (attributePrefs.RankedAttributes || ac.Visible);
                //    treated = true;
                //}

                ////Global User Preference
                //if (ac.Attribute.Type == AttributeTypeEnum.Global)
                //{
                //    ac.Visible = (attributePrefs.GlobalAttributes || ac.Visible);
                //    treated = true;
                //}

                ////Extended User Preference
                //if (!treated)
                //    ac.Visible = (attributePrefs.ProductAttributes || ac.Visible);

                //if (attributePrefs.AttributeGroupInclusions.Contains(ac.AttributeName))
                //    ac.Visible = true;

                //if (attributePrefs.AttributeCustomInclusions.Contains(ac.AttributeName))
                //    ac.Visible = true;
            }


            PossibleAttributeColumnsForSkuView.Sort();

            for (var i = 0; i < PossibleAttributeColumnsForSkuView.Count; i++)
                PossibleAttributeColumnsForSkuView[i].Position = i;
            RedrawDataGridView(null);
        }

        private void btnShowHideAuditTrail_CheckedChanged(object sender, EventArgs e) { ShowAuditTrail = btnShowHideAuditTrail.Checked; }

        private void btnSort_Click(object sender, EventArgs e) { sortContextMenu.Show(btnSort, 0, btnSort.Height); }

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

        private void calculatedAttributeFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iteration;
            string fieldName;
            var headerText = edgv.Columns[edgv.CurrentCellAddress.X].HeaderText;
            SplitHeaderText(headerText, out fieldName, out iteration);
            var thisAttribute = Attribute.GetAttributeFromName(fieldName, false);

            if (thisAttribute == null)
                return;

            var derivedAttribute = GetDerivedAttribute(thisAttribute, CurrentTaxonomy);

            var expression = derivedAttribute != null ? derivedAttribute.Expression : String.Empty;

            var maxLength = derivedAttribute != null ? derivedAttribute.MaxResultLength : 255;

            var attributes =
                _allAttributes.Where(att => att.Value.AttributeType != AttributeTypeEnum.Derived.ToString()).Select(
                    att => att.Value.AttributeName);

            var allSkus = AryaTools.Instance.Forms.SkuOrders[_instanceId];
            var top10SkuIds = (allSkus.Count > 10 ? allSkus.Take(10) : allSkus);

            var expressionBuilder = new FrmAttributeBuilder(expression, maxLength, attributes,
                                                            _skus.Where(sku => top10SkuIds.Contains(sku.Key)).Select(
                                                                sku => sku.Value),
                                                            (derivedAttribute != null)
                                                                ? derivedAttribute.TaxonomyInfo
                                                                : null,
                                                            !(derivedAttribute == null
                                                              || derivedAttribute.TaxonomyID == CurrentTaxonomy.ID),
                                                            derivedAttribute == null
                                                                ? String.Empty
                                                                : derivedAttribute.Attribute.AttributeName);

            var columnToBeInvalidated = edgv.CurrentCellAddress.X;

            expressionBuilder.Show(this);

            expressionBuilder.FormClosed += (s, e1) =>
                                                {
                                                    var result = expressionBuilder.DialogResult;

                                                    if (result != DialogResult.OK)
                                                        return;

                                                    if (derivedAttribute != null
                                                        &&
                                                        ((derivedAttribute.TaxonomyID == CurrentTaxonomy.ID)
                                                         ||
                                                         (derivedAttribute.TaxonomyID != CurrentTaxonomy.ID
                                                          && expressionBuilder.IsExpressionInherited)))
                                                    {
                                                        if (
                                                            String.IsNullOrWhiteSpace(
                                                                expressionBuilder.AttributeExpression))
                                                            AryaTools.Instance.InstanceData.Dc.DerivedAttributes.
                                                                DeleteOnSubmit(derivedAttribute);
                                                        else
                                                        {
                                                            derivedAttribute.Expression =
                                                                expressionBuilder.AttributeExpression;
                                                            derivedAttribute.MaxResultLength =
                                                                expressionBuilder.MaxLength;
                                                        }
                                                    }
                                                    else if (
                                                        !String.IsNullOrWhiteSpace(
                                                            expressionBuilder.AttributeExpression))
                                                    {
                                                        thisAttribute.DerivedAttributes.Add(new DerivedAttribute
                                                                                                {
                                                                                                    TaxonomyInfo =
                                                                                                        CurrentTaxonomy,
                                                                                                    Expression =
                                                                                                        expressionBuilder
                                                                                                        .
                                                                                                        AttributeExpression,
                                                                                                    MaxResultLength
                                                                                                        =
                                                                                                        expressionBuilder
                                                                                                        .MaxLength
                                                                                                });
                                                    }
                                                    edgv.InvalidateColumn(columnToBeInvalidated);
                                                    SkuViewSaveChangesIfNecessary(false, false);
                                                };
        }

        private void cboAttributeName_Change(object sender, EventArgs e) { SetCurrentChangeNewAttribute(cboAttributeName.Text); }

        private void chkDispOrder_CheckedChanged(object sender, EventArgs e)
        {
            AttributeColumn.SortDisplayOrders = chkDispOrder.Checked;
            UpdateAllHeaderTexts();
        }

        private void chkNavOrder_CheckedChanged(object sender, EventArgs e)
        {
            AttributeColumn.SortNavigationOrders = chkNavOrder.Checked;
            UpdateAllHeaderTexts();
        }

        private void chkShowHideLinks_CheckedChanged(object sender, EventArgs e)
        {
            ShowLinks = chkShowHideLinks.Checked;
            if (chkShowHideLinks.Checked)
                return;
            if (AryaTools.Instance.Forms.SkuTabs.Values.Any(p => p.ShowLinks) == false)
                AryaTools.Instance.Forms.SkuLinksViewForm.Close();
        }

        private void chkSuggestValues_CheckedChanged(object sender, EventArgs e)
        {
            mainToolTip.SetToolTip(chkSuggestValues,
                                   chkSuggestValues.Checked ? "Auto-Suggest is ON" : "Auto-Suggest is OFF");

            txtValue.Focus();
        }

        private void clearAllFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAllFilters();
        }

        private void ClearAllFilters()
        {
            ApplyFilter(FilterTaxonomyFieldname, new List<string>(), false, true, filterByComboToolStripMenuItem.Text);
            RedrawDataGridView(null);
            UpdateFilterStatus();
            if (LastSortOption != null)
                SortGrid(LastSortOption.Column, LastSortOption.SortOrderOption, LastSortOption.SortField);
            SetStatus(DefaultStatus, WorkerState.Ready);
        }

        private void clearThisFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iteration;
            var headerText = edgv.Columns[edgv.CurrentCellAddress.X].HeaderText;
            string attributeName;
            if (edgv.CurrentCellAddress.X == 1)
                attributeName = FilterTaxonomyFieldname;
            else
                SplitHeaderText(headerText, out attributeName, out iteration);
            var newSkuCount = ApplyFilter(attributeName.ToLower(), new List<string>(), true, false,
                                          filterByComboToolStripMenuItem.Text);

            RedrawDataGridView(edgv.CurrentCell.Value);
            UpdateFilterStatus();
            //Restore the last sorting
            if (LastSortOption != null)
                SortGrid(LastSortOption.Column, LastSortOption.SortOrderOption, LastSortOption.SortField);
            SetStatus(DefaultStatus, WorkerState.Ready);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) { AryaTools.Instance.Forms.SkuForm.CloseCurrentTab(); }

        private void Colorize(object value, int columnnIndex, int rowIndex)
        {
            Color back, fore;
            if (rowIndex > -1 && rowIndex % 2 == 0)
            {
                back = DisplayStyle.CellStyleEvenRow.BackColor;
                fore = Color.Black;
            }
            else
            {
                back = DisplayStyle.CellStyleOddRow.BackColor;
                fore = Color.Black;
            }

            if (value == null)
                return;

            if (_objectColors.ContainsKey(value))
                return;

            //if (value.ToString().Equals(string.Empty))
            //{
            //    back = Color.White;
            //    fore = Color.Black;
            //}
            //else if (value is Sku)
            //{
            //    back = back == DisplayStyle.CellStyleOddRow.BackColor ? DisplayStyle.CellStyleOddRow.BackColor : DisplayStyle.CellStyleGreyRegular.BackColor;
            //    fore = DisplayStyle.CellStyleItemIDColumn.ForeColor;
            //}
            if (value is TaxonomyInfo)
            {
                var x = rnd.Next(1, 3);
                var r = rnd.Next(8, 32) * x * x;
                var y = rnd.Next(1, 3);
                var g = rnd.Next(8, 32) * y * y;
                var z = x == y ? (x == 1 ? 2 : 1) : rnd.Next(1, 3);
                var b = rnd.Next(8, 32) * z * z;

                back = Color.FromArgb(r + 128, g + 128, b + 128);
                fore = Color.FromArgb(r / 2, g / 2, b / 2);
            }
            else if (value is EntityData)
            {
                if (((EntityData)value).EntityID != Guid.Empty)
                    GetColorsBasedOnRules((EntityData)value, ref back, ref fore);
            }
            //else
            //{
            //    back = Color.DarkRed;
            //    fore = Color.White;
            //}

            if (back != DisplayStyle.CellStyleOddRow.BackColor && back != DisplayStyle.CellStyleEvenRow.BackColor)
                _objectColors.Add(value, new CustomKeyValuePair<Color, Color>(back, fore));
        }

        private void colorizeToolStripMenuItem_Click(object sender, EventArgs e) { GetColorRules(); }

        private void convertSelectedEntitysToDecimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int noOfDecimals;
            if (!Int32.TryParse(noOfDecimalPlacesToolStripMenuItem.Text, out noOfDecimals) || noOfDecimals < 0
                || noOfDecimals > 6)
            {
                MessageBox.Show(String.Format("Invalid number of decimal places (0-6): {0}",
                                              noOfDecimalPlacesToolStripMenuItem.Text));
                return;
            }
            CurrentChange.NoOfDecimalPlaces = noOfDecimals;
            CurrentChange.RunTool = Change.Tool.ConvertToDecimal;
            SaveCurrentChangeAndGetNewChange(false);
            RedrawDataGridView(null);
        }

        private void CreateNewChange(DataGridViewSelectedCellCollection selectedCells)
        {
            if (selectedCells.Count == 0)
                return;

            var waitkey = new Guid();

            if (selectedCells.Count > MinCellCount)
                waitkey = FrmWaitScreen.ShowMessage("Selecting cells");
            CurrentChange = new Change(selectedCells.Count, _skus.Count);
            int iteration;
            var headerText = selectedCells[0].OwningColumn.HeaderText;
            string attributeName;
            SplitHeaderText(headerText, out attributeName, out iteration);
            CurrentChange.OldValues.AttributeName = attributeName;
            for (var i = 0; i < selectedCells.Count; i++)
            {
                if (selectedCells.Count > MinCellCount && i % MinCellCount == 0)
                    FrmWaitScreen.UpdateMessage(waitkey,
                                                String.Format("Selecting cells ({0} of {1})", i, selectedCells.Count));
                var cellValue = selectedCells[i].Value ?? String.Empty;
                var cellDataType = cellValue.GetType();
                if (cellDataType == typeof(EntityData))
                {
                    if (((EntityData)cellValue).EntityInfo != null && ((EntityData)cellValue).EntityInfo.Sku != null)
                        CurrentChange.Add((EntityData)cellValue);
                    //else
                    //{
                    //    var currentAttribute = Attribute.GetAttributeFromName(attributeName,false,true,Attribute.AttributeTypeCalculated);

                    //    if(currentAttribute != null)
                    //    {
                    //        var currentSku = (edgv[0, selectedCells[i].RowIndex]).Value as Sku;
                    //        CurrentChange.Add((EntityData)cellValue,currentSku);
                    //    }
                    //}
                }
                else if (cellDataType == typeof(Sku))
                    CurrentChange.Add((Sku)cellValue);
                else if (cellValue.Equals(String.Empty))
                    CurrentChange.AddBlank((Sku)edgv.Rows[selectedCells[i].RowIndex].Cells[0].Value);
            }

            lblCurrentChange.Text = selectedCells.Count + " cells selected";
            if (selectedCells.Count > MinCellCount)
                FrmWaitScreen.HideMessage(waitkey);
        }

        private void CreateNewEntitiesFromChange(Change change, bool forceCancelAutoSave)
        {
            if (change == null || !change.HasChange())
                return;
            if ((change.NewValues.AttributeName ?? change.OldValues.AttributeName).Equals(Arya.Framework.Properties.Resources.NewAttributeName))
                return;
            Attribute newAttribute;
            var refreshColumns = false;
            if (change.NewValues.AttributeName == null && change.OldValues.Att != null)
                newAttribute = change.OldValues.Att;
            else if (GetAttributeFromName(change.NewValues.AttributeName ?? change.OldValues.AttributeName,
                                          out newAttribute))
                refreshColumns = true;
            change.NewValues.Att = newAttribute;
            if (change.OldValues.Att != null && _noOfColumns.ContainsKey(change.OldValues.Att))
                _noOfColumns.Remove(change.OldValues.Att);
            if (change.NewValues.Att != null && _noOfColumns.ContainsKey(change.NewValues.Att))
                _noOfColumns.Remove(change.NewValues.Att);
            if (change.Execute(CurrentTaxonomy, _undoHistory, CreateNewEntityData))
                refreshColumns = true;

            var distinctValues = new[] { change.NewValues.Val };

            if (CurrentTaxonomy != null && newAttribute != null)
            {
                if (_validValues.ContainsKeys(CurrentTaxonomy, newAttribute))
                {
                    distinctValues = _validValues[CurrentTaxonomy, newAttribute];

                    if (!distinctValues.Contains(change.NewValues.Val))
                    {
                        Array.Resize(ref distinctValues, distinctValues.Length + 1);
                        distinctValues[distinctValues.Length - 1] = change.NewValues.Val;
                        _validValues.Add(CurrentTaxonomy, newAttribute, distinctValues);
                    }
                }
                else
                    _validValues.Add(CurrentTaxonomy, newAttribute, distinctValues);
            }

            var newAtt = new Attribute();
            change.NewAttributeNames.ForEach(att => GetAttributeFromName(att, out newAtt));
            if (CurrentTaxonomy != null && change.NewValues.Tax != null)
            {
                ApplyFilter(FilterTaxonomyFieldname, new List<string> { CurrentTaxonomy.ToString() }, false, false,
                            filterByComboToolStripMenuItem.Text);
                refreshColumns = true;
            }
            if (refreshColumns)
            {
                var lastEntityDataCreated = CurrentChange.LastEntityDataCreated;
                CurrentChange = null;
                if (change.OldValues.Att != null
                    && _allAttributes.ContainsKey(change.OldValues.Att.AttributeName.ToLower()))
                {
                    var hasData =
                        _skus.Any(sku => sku.Value.GetValuesForAttribute(change.OldValues.Att.AttributeName).Count > 0);
                    //_skus.SelectMany(sku => sku.Value.EntityInfos).SelectMany(
                    //    ei => ei.EntityDatas.Where(ed => ed.Active && ed.Attribute.Equals(change.OldValues.Att))).
                    //    Any();

                    if (!hasData)
                    {
                        var attCol =
                            PossibleAttributeColumnsForSkuView.FirstOrDefault(col => col.Attribute.Equals(change.OldValues.Att));
                        if (attCol != null)
                            PossibleAttributeColumnsForSkuView.Remove(attCol);
                    }
                }

                RedrawDataGridView(lastEntityDataCreated);
                PopulateAttributeDropdownDataSource(cboAttributeName);
                UpdateFilterStatus();
            }
            else
            {
                foreach (var skuRow in
                    change.GetSkus().Union(change.GetBlanks()).Select(
                        currentSku =>
                        edgv.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => row.Cells[0].Value.Equals(currentSku))).
                        Where(skuRow => skuRow != null))
                    edgv.InvalidateRow(skuRow.Index);
            }
            if (!forceCancelAutoSave)
                SkuViewSaveChangesIfNecessary(false, false);
            //AryaTools.Instance.SaveChangesIfNecessary(false, false);
            edgv.Focus();
        }

        private void DeleteSelectedEntities(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you want to delete the selected entities?", @"Delete?",
                                MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            CurrentChange.Delete = true;
            SaveCurrentChangeAndGetNewChange(false);
            //TryHideColumns();
            RedrawDataGridView(null);
        }

        private void DoSlice(bool sliceInPlace)
        {
            if (String.IsNullOrEmpty(sliceDelimiterToolStripMenuItem.Text))
            {
                MessageBox.Show("Please select (or type) a delimiter first.");
                return;
            }
            switch (sliceDelimiterToolStripMenuItem.Text)
            {
                case "Comma":
                    CurrentChange.SliceDelimiter = ",";
                    break;

                case "Semicolon":
                    CurrentChange.SliceDelimiter = ";";
                    break;

                case "Space":
                    CurrentChange.SliceDelimiter = " ";
                    break;

                default:
                    CurrentChange.SliceDelimiter = sliceDelimiterToolStripMenuItem.Text;
                    break;
            }
            CurrentChange.RunTool = sliceInPlace ? Change.Tool.SliceInPlace : Change.Tool.SliceIntoNewAttributes;
            SaveCurrentChangeAndGetNewChange(false);
            //TryHideColumns();
            //RedrawDataGridView(null);
        }

        private void edgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

                if (e.RowIndex > -1 && e.RowIndex % 2 == 0)
                {
                    defaultBackColor = DisplayStyle.CellStyleEvenRow.BackColor;
                    defaultForeColor = Color.Black;
                }
                else
                {
                    defaultBackColor = DisplayStyle.CellStyleOddRow.BackColor;
                    defaultForeColor = Color.Black;
                }
            }

            if (e.Value == null || e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            //because we set e.Handled=true, we need to put this piece here.
            if (edgv.CurrentRow != null && edgv.CurrentRow.Index == e.RowIndex)
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.OutsetDouble;

            if (_objectColors.ContainsKey(e.Value) && e.Value.ToString() != String.Empty)
            {
                var kvp = _objectColors[e.Value];
                e.CellStyle.BackColor = kvp.Key;
                e.CellStyle.ForeColor = kvp.Value;
            }

            else
            {
                e.CellStyle.BackColor = defaultBackColor;
                e.CellStyle.ForeColor = defaultForeColor;
            }

            if (e.Value is Sku)
            {
                e.CellStyle.BackColor = defaultBackColor == DisplayStyle.CellStyleOddRow.BackColor
                                            ? DisplayStyle.CellStyleOddRow.BackColor
                                            : DisplayStyle.CellStyleGreyRegular.BackColor;
                e.CellStyle.ForeColor = DisplayStyle.CellStyleItemIDColumn.ForeColor;
            }

            var entityData = e.Value as EntityData;
            if (entityData != null)
            {
                var ed = entityData;
                if (ed.Attribute != null && ed.Attribute.AttributeType == AttributeTypeEnum.Derived.ToString())
                    e.CellStyle.BackColor = Color.PaleGoldenrod;
                if (validateToolStripMenuItem.Checked && e.ColumnIndex > 1 && !String.IsNullOrEmpty(ed.Value)
                    && ed.AttributeID != Guid.Empty)
                {
                    if (!ValidateDataType(ed))
                        e.CellStyle.ForeColor = Color.Crimson;
                }
            }

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);
            e.Handled = true;
            var value2 = GetCellValue2(e.Value);
            if (String.IsNullOrEmpty(value2))
                return;
            var value1Width =
                (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font, e.CellBounds.Width).Width;
            var halfCellWidth = e.CellBounds.Width / 2;
            if (value1Width > halfCellWidth)
                value1Width = halfCellWidth;
            var value2Bounds = new Rectangle(e.CellBounds.X + value1Width, e.CellBounds.Y,
                                             e.CellBounds.Width - value1Width, e.CellBounds.Height);
            var value2Measure = e.Graphics.MeasureString(value2, e.CellStyle.Font, value2Bounds.Width, _rightAlign);
            e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.CellBounds.Right - value2Measure.Width, e.CellBounds.Top,
                                     value2Measure.Width - 1, e.CellBounds.Height - 1);

            var fieldColor = Brushes.Black;

            //Not used in all the projects.
            //if (chkUom.Checked && !HelperClasses.Validate.ValidateUom(value2))
            //    fieldColor = Brushes.Crimson;
            e.Graphics.DrawString(value2, e.CellStyle.Font, fieldColor, value2Bounds, _rightAlign);
        }

        private void edgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (!AryaTools.Instance.Forms.SkuOrders.ContainsKey(_instanceId))
            {
                e.Value = String.Empty;
                return;
            }
            var skuOrder = AryaTools.Instance.Forms.SkuOrders[_instanceId];

            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.RowIndex >= edgv.RowCount
                || e.ColumnIndex >= edgv.ColumnCount - 1 || e.RowIndex >= skuOrder.Count
                || !_skus.ContainsKey(skuOrder[e.RowIndex]))
            {
                e.Value = String.Empty;

                if (!_populatingGridView)
                    Colorize(e.Value, e.ColumnIndex, e.RowIndex);
                return;
            }
            var skuId = skuOrder[e.RowIndex];

            if (!_skus.ContainsKey(skuId))
            {
                e.Value = String.Empty;
                return;
            }
            var currentSku = _skus[skuId];
            var currentColumIndex = e.ColumnIndex;
            switch (currentColumIndex)
            {
                case 0:
                    e.Value = currentSku;
                    break;
                case 1:
                    e.Value = currentSku.Taxonomy;
                    break;
                default:
                    int iteration;
                    string fieldName;
                    var columnName = edgv.Columns[currentColumIndex].HeaderText;
                    SplitHeaderText(columnName, out fieldName, out iteration);

                    var values = currentSku.GetValuesForAttribute(fieldName);

                    var count = values.Count();
                    switch (count)
                    {
                        case 0:
                            e.Value = String.Empty;
                            break;

                        case 1:
                            if (iteration == 0)
                                e.Value = values.First();
                            else
                                e.Value = String.Empty;
                            break;

                        default:
                            if (values.Count > iteration)
                            {
                                e.Value = values[iteration];

                                if (!_populatingGridView)
                                {
                                    for (var i = 1;
                                         i < values.Count - iteration && currentColumIndex + i < edgv.ColumnCount;
                                         i++)
                                    {
                                        string nextColumnFieldName;
                                        int nextColumnIteration;
                                        columnName = edgv.Columns[currentColumIndex + i].HeaderText;
                                        SplitHeaderText(columnName, out nextColumnFieldName, out nextColumnIteration);
                                        if (fieldName.Equals(nextColumnFieldName))
                                        {
                                            Colorize(e.Value, e.ColumnIndex, e.RowIndex);
                                            continue;
                                        }

                                        _populatingGridView = true;
                                        Attribute att;
                                        GetAttributeFromName(fieldName, out att);
                                        if (_noOfColumns.ContainsKey(att))
                                            //_noOfColumns.Remove(att);
                                            _noOfColumns[att] = values.Count;
                                        else
                                            _noOfColumns.Add(att, values.Count);

                                        var headerText = GetHeaderText(String.Empty, fieldName, iteration + i);
                                        var newColumn = GetNewColumn(headerText, CellTextBoxTemplate, false,
                                                                     DefaultColumnWidth);

                                        newColumn.HeaderCell.Style =
                                            _currentFilters.Any(
                                                f => f.LowerAttributeName.Equals(att.AttributeName.ToLower()))
                                                ? boldStyle
                                                : normalStyle;

                                        edgv.Columns.Insert(currentColumIndex + i, newColumn);

                                        _populatingGridView = false;
                                    }
                                }
                            }
                            else
                                e.Value = String.Empty;
                            break;
                    }
                    break;
            }
            Colorize(e.Value, e.ColumnIndex, e.RowIndex);
        }

        private void edgv_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (edgv.Rows.Count == 0)
            {
                edgv.Columns[e.ColumnIndex].Selected = false;
                return;
            }

            SelectAttributeFromColumn(e.ColumnIndex);
            var headercell = edgv.Columns[e.ColumnIndex].HeaderCell;
            int iteration;
            string fieldName;
            SplitHeaderText(headercell.Value.ToString(), out fieldName, out iteration);
            var lowerFieldName = fieldName.ToLower();
            if (!_allAttributes.ContainsKey(lowerFieldName))
                return;
            var edgvLocation = GetLocation(edgv);
            var columnDisplayRectangle = edgv.GetColumnDisplayRectangle(e.ColumnIndex, true);
            _newAttributeDropDown.Location = new Point(edgvLocation.X + columnDisplayRectangle.Left, edgvLocation.Y);
            //+ ((edgv.ColumnHeadersHeight)));// - _newAttributeDropDown.Height) / 2));
            _newAttributeDropDown.Width = columnDisplayRectangle.Width;
            _newAttributeDropDown.DataSource = cboAttributeName.DataSource;
            for (var i = 0; i < _newAttributeDropDown.Items.Count; i++)
            {
                if (_newAttributeDropDown.Items[i].ToString().Equals(fieldName))
                {
                    _newAttributeDropDown.SelectedIndex = i;
                    break;
                }
            }
            _newAttributeDropDown.Show();
            _newAttributeDropDown.Focus();
        }

        private void edgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            int iteration;
            string fieldName;
            var headerText = e.Column.HeaderText;
            SplitHeaderText(headerText, out fieldName, out iteration);
            var columnProperties = PossibleAttributeColumnsForSkuView.Where(col => col.AttributeName.Equals(fieldName)).ToList();
            if (columnProperties.Any())
                columnProperties.First().ColumnWidth = e.Column.Width;
        }

        private void edgv_DataError(object sender, DataGridViewDataErrorEventArgs e) { MessageBox.Show(e.Exception.Message); }

        private void edgv_Enter(object sender, EventArgs e)
        {
            //_getImageFrom = edgv.Name;
            ValueToolTip();
        }

        private void edgv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (edgv.SelectedCells.Count == 0 || edgv.SelectedCells[0].ColumnIndex == 0 || e.KeyChar < ' '
                || e.KeyChar > '~')
                return;
            if (CurrentChange.GetEntityDatas().Count + CurrentChange.GetBlanks().Count == edgv.RowCount
                &&
                MessageBox.Show("Are you sure you want to change the value for the entire column?",
                                "Changing entire column", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            string fieldName;
            int iteration;
            SplitHeaderText(edgv.CurrentCell.OwningColumn.HeaderText, out fieldName, out iteration);
            if (_allAttributes.ContainsKey(fieldName.ToLower()) && _allAttributes[fieldName.ToLower()].Readonly)
            {
                MessageBox.Show("This is a read-only attribute.");
                return;
            }
            if (!txtValue.Enabled)
                CreateNewEntityData = true;
            txtValue.Focus();
            txtValue.Text = e.KeyChar.ToString();
            txtValue.SelectionStart = 1;
            CurrentChange.NewValues.Val = txtValue.Text;
            e.Handled = true;
        }

        private void edgv_Leave(object sender, EventArgs e)
        {
            var activeControlType = ActiveControl != null ? ActiveControl.GetType() : null;
            if (activeControlType == null || allowedDestinationTypes.Contains(activeControlType))
                return;
            edgv.Focus();
        }

        private void edgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            edgv.Rows.Cast<DataGridViewRow>().ForEach(row => row.HeaderCell.Value = (row.Index + 1).ToString());
            edgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void edgv_SelectionChanged(object sender, EventArgs e)
        {
            if (!_selectingManyCells && !_populatingGridView)
            {
                //   SelectionChanged();
                selectionChangedTimer.Stop();
                selectionChangedTimer.Start();
            }
        }

        private void EntityDataGridView_Layout(object sender, LayoutEventArgs e) { edgv.Focus(); }

        private void entityFields_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleEnterAndEscapeForChange(e.KeyCode))
                e.Handled = true;
        }

        private void exportCurrentViewToolStripMenuItem_Click(object sender, EventArgs e) { ExportThisTabData(); }

        private void ExportThisTabData()
        {
            var rx = new Regex("[^-,() a-zA-Z0-9]+");
            string filename;
            try
            {
                filename =
                    rx.Replace(
                        CurrentTaxonomy != null
                            ? CurrentTaxonomy.TaxonomyData.NodeName
                            : AryaTools.Instance.Forms.SkuForm.Text, "_").Replace("__", "_");
            }
            catch
            {
                filename = "ExportData";
            }
            AryaTools.Instance.Forms.SkuForm.exportTabFileDialog.FileName = filename;
            if (AryaTools.Instance.Forms.SkuForm.exportTabFileDialog.ShowDialog() == DialogResult.OK)
            {
                var saveFilename = AryaTools.Instance.Forms.SkuForm.exportTabFileDialog.FileName;
                SaveCurrentGridToFile(saveFilename);
            }
        }

        private void extractToNewAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (aggregateFunctionToolStripMenuItem.Text.ToLower())
            {
                case "minimum":
                    CurrentChange.RunTool = Change.Tool.ExtractMinimumValue;
                    break;

                case "maximum":
                    CurrentChange.RunTool = Change.Tool.ExtractMaximumValue;
                    break;

                case "count":
                    CurrentChange.RunTool = Change.Tool.ExtractCountOfValues;
                    break;

                default:
                    MessageBox.Show("Unknown function: " + aggregateFunctionToolStripMenuItem.Text);
                    return;
            }
            SaveCurrentChangeAndGetNewChange(false);
            //RedrawDataGridView(null);
        }

        private void FetchSkuAttributeValueCache(Attribute att)
        {
            var cache = AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache;
            if (cache.ContainsKey(att))
                return;

            var skus = LoadQueryResults;

            if (att.AttributeName.Equals(Arya.Framework.Properties.Resources.NewAttributeName))
            {
                foreach (var sku in skus)
                    cache.Add(att, sku, new List<EntityData>());
            }
            else
            {
                //Rohit
                var skuIds = skus.Select(s => s.ID).Distinct().ToList();
                var listOfList = SplitSkuList(skuIds, 1500);

                foreach (var list in listOfList)
                {
                    var values = from sku in AryaTools.Instance.InstanceData.Dc.Skus
                                 where list.Contains(sku.ID)
                                 let eds = from ei in sku.EntityInfos
                                           from ed in ei.EntityDatas
                                           where ed.Active && ed.AttributeID.Equals(att.ID)
                                           select ed
                                 select new { sku, eds };

                    foreach (var val in values)
                    {
                        var vals = EntityData.OrderSkuAttributeValues(val.eds).ToList();
                        cache.Add(att, val.sku, vals);
                    }
                }

                //Rohit

                //var values = from sku in _loadQuery
                //             let eds = from ei in sku.EntityInfos
                //                       from ed in ei.EntityDatas
                //                       where ed.Active && ed.AttributeID.Equals(att.ID)
                //                       select ed
                //             select new { sku, eds };

                //foreach (var val in values)
                //{
                //    List<EntityData> vals = EntityData.OrderSkuAttributeValues(val.eds).ToList();
                //    cache.Add(att, val.sku, vals);
                //}
            }
        }

        private void FetchSkuAttributeValueCache(IEnumerable<Attribute> listOfAttribute)
        {
            var sw = new Stopwatch();
            sw.Start();

            var cache = AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache;

            var notPresentInCache = new List<Attribute>();
            List<Sku> skus = null;

            foreach (var att in listOfAttribute)
            {
                if (!cache.ContainsKey(att))
                {
                    if (!att.AttributeName.Equals(Framework.Properties.Resources.NewAttributeName))
                        notPresentInCache.Add(att);

                    if (skus == null)
                        skus = LoadQueryResults;

                    if (att.AttributeName.Equals(Framework.Properties.Resources.NewAttributeName) && skus.Count > 0)
                    {
                        foreach (var sku in skus)
                            cache.Add(att, sku, new List<EntityData>());
                    }
                }
            }

            if (notPresentInCache.Count == 0)
                return;

            var attList = notPresentInCache.Select(a => a.ID).ToList();

            //No of Attributes will stay constant, the sku list size will be computed based on value threshold which must be less than 1500, else you messed up!

            var skuListSize = GetListSize(attList.Count, skus.Count, 25000);

            var skuIds = skus.Select(s => s.ID).ToList();
            var listOfList = SplitSkuList(skuIds, skuListSize);

            //parallel
            //Parallel.ForEach(listOfList, list =>
            //{
            //    var values = (from sku in AryaTools.Instance.InstanceData.Dc.Skus.AsParallel()
            //                  where list.Contains(sku.ID)
            //                  let eds = from ei in sku.EntityInfos
            //                            from ed in ei.EntityDatas
            //                            where ed.Active && attList.Contains(ed.AttributeID)
            //                            select ed
            //                  select new { sku, eds }).ToList();

            //    foreach (var val in values)
            //    {
            //        foreach (var att in notPresentInCache)
            //        {
            //            List<EntityData> vals = EntityData.OrderSkuAttributeValues(val.eds.Where(a => a.AttributeID == att.ID)).ToList();
            //            cache.Add(att, val.sku, vals);
            //        }
            //    }

            //}
            //);
            //parallel

            //sequential
            //Parallel.ForEach(listOfList, list =>
            foreach (var list in listOfList)
            {
                var list1 = list;
                var values = (from sku in AryaTools.Instance.InstanceData.Dc.Skus
                              where list1.Contains(sku.ID)
                              let eds = from ei in sku.EntityInfos
                                        join ed in AryaTools.Instance.InstanceData.Dc.EntityDatas on ei.ID equals
                                            ed.EntityID
                                        where ed.Active && attList.Contains(ed.AttributeID)
                                        select ed
                              select new { sku, eds }).ToList();

                foreach (var val in values)
                {
                    foreach (var att in notPresentInCache)
                    {
                        var vals =
                            EntityData.OrderSkuAttributeValues(val.eds.Where(a => a.AttributeID == att.ID)).ToList();
                        cache.Add(att, val.sku, vals);
                        _noOfColumns[att] = vals.Count();
                    }
                }
            }
            //});
            //sequential

            sw.Stop();
            Diagnostics.WriteMessage("Fetching attribtue values for SKUs",
                                     "EntityDataGridView - FetchSkuAttributeValueCache", sw.Elapsed,
                                     skus == null ? 0 : skus.Count, null, skus == null ? 0 : attList.Count());
            sw.Reset();
        }

        private void filterAttributeToolStripMenuItem_Click(object sender, EventArgs e) { FilterCurrentAttribute(); }

        private void filterByComboToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e) { UpdateFilterContextMenu(); }

        private void filterContextMenu_Opening(object sender, CancelEventArgs e) { UpdateFilterContextMenu(); }

        private void FilterCurrentAttribute()
        {
            var totalSkuCount = AryaTools.Instance.Forms.SkuOrders[_instanceId].Count;
            var columnIndex = edgv.CurrentCellAddress.X;
            if (columnIndex < 1)
            {
                MessageBox.Show(@"Filter on Item Id is not yet implemented.");
                return;
            }
            edgv.Focus();
            SetStatus("Filter", WorkerState.Working);
            var currentValue = edgv.CurrentCell.Value;
            string attributeName;
            double fillRate;
            // Build a list of all values for this attribute (not for this column)
            var filterItems = GetFilterItems(columnIndex, filterByComboToolStripMenuItem.Text, out attributeName,
                                             out fillRate);
            // Position the filter window
            var bounds = edgv.GetCellDisplayRectangle(columnIndex, -1, false);
            var location = edgv.PointToScreen(new Point(bounds.Left, bounds.Bottom));
            var filterForm = AryaTools.Instance.Forms.FilterForm;
            filterForm.UpdateFilter(location, filterItems, _currentFilters.Count > 0, attributeName, fillRate);
            // Show the filter dialog
            var result = filterForm.ShowDialog();
            if (result != DialogResult.OK)
            {
                SetStatus(DefaultStatus, WorkerState.Ready);
                return;
            }
            var sw = new Stopwatch();
            sw.Start();
            var waitkey = FrmWaitScreen.ShowMessage("Applying Filter");
            ApplyFilter(attributeName.ToLower(), filterForm._selectedItems, filterForm._clearThisFilter,
                        filterForm._clearAllFilters, filterByComboToolStripMenuItem.Text);
            RedrawDataGridView(currentValue);
            UpdateFilterStatus();
            SetStatus(DefaultStatus, WorkerState.Ready);
            FrmWaitScreen.HideMessage(waitkey);
            sw.Stop();
            Diagnostics.WriteMessage("*Applying Filters", "EntityDataGridView - Filter Current Attribute", sw.Elapsed,
                                     totalSkuCount, _taxonomyNodes.Count);
            sw.Reset();
        }

        private void FilterCurrentValue()
        {
            UpdateFilterContextMenu(); //This is to take care of the case when this is invoked using Shortcut keys
            var currentValue = edgv.CurrentCell.Value;
            int iteration;
            var headerText = edgv.Columns[edgv.CurrentCellAddress.X].HeaderText;
            string attributeName;
            var filterValue = filterValueToolStripMenuItem.Text.Equals(FilterBlankValues)
                                  ? BlankValue
                                  : filterValueToolStripMenuItem.Text.Substring(FilterFieldPrefix.Length + 2);
            if (edgv.CurrentCellAddress.X == 1)
                attributeName = FilterTaxonomyFieldname;
            else
                SplitHeaderText(headerText, out attributeName, out iteration);
            var newSkuCount = ApplyFilter(attributeName.ToLower(), new List<string> { filterValue }, false, false,
                                          filterByComboToolStripMenuItem.Text);
            RedrawDataGridView(currentValue);
            UpdateFilterStatus();
            SetStatus(DefaultStatus, WorkerState.Ready);
        }

        private void filterValueToolStripMenuItem_Click(object sender, EventArgs e) { FilterCurrentValue(); }

        private void FindReplace()
        {
            //string findString;
            //try
            //{
            //    findString = edgv.CurrentCell.Value.ToString();
            //}
            //catch
            //{
            //    findString = string.Empty;
            //}
            if (edgv.SelectedCells.Count == 1)
                SelectAttributeFromColumn(edgv.CurrentCell.ColumnIndex);
            FindReplaceWindow.ShowFindReplaceForm();
        }

        private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e) { FindReplace(); }

        private void PopulateAttributeDefinition()
        {
            lblAttributeDefinition.Text = String.Empty;

            if (edgv.CurrentCell.ColumnIndex < 2 || (edgv.CurrentCell.ColumnIndex + 1) == edgv.ColumnCount)
                return;

            var metaAttribute = Attribute.GetAttributeFromName("Definition", true, AttributeTypeEnum.SchemaMeta);

            if (edgv.CurrentRow == null)
                return;

            var headerCell = edgv.CurrentCell.OwningColumn.HeaderCell;

            int iteration;
            string fieldName;
            Attribute attribute;
            SplitHeaderText(headerCell.Value.ToString(), out fieldName, out iteration);
            GetAttributeFromName(fieldName, out attribute);
            if (attribute == null)
                return;

            var taxonomyInfo = edgv[1, edgv.CurrentCell.RowIndex].Value as TaxonomyInfo;
            if (taxonomyInfo == null)
                return;

            var definition = from si in taxonomyInfo.SchemaInfos
                             where si.Attribute == attribute
                             from mi in si.SchemaMetaInfos
                             where mi.Attribute == metaAttribute
                             from md in mi.SchemaMetaDatas
                             where md.Active
                             select attribute.AttributeName + ": " + md.Value;

            lblAttributeDefinition.Text = definition.FirstOrDefault() ?? string.Empty;
        }

        private string GetAttributeOrders(string lowerAttributeName)
        {
            var navOrderChecked = chkNavOrder.Checked;
            var dispOrderChecked = chkDispOrder.Checked;
            bool inSchema;
            decimal navigationalOrder, displayOrder;
            SchemaAttribute.GetAttributeOrders(Attribute.GetAttributeFromName(lowerAttributeName, true), CurrentTaxonomy,
                                               out navigationalOrder, out displayOrder, out inSchema);
            var isInSchema = inSchema ? String.Empty : NotInSchema;
            if (!dispOrderChecked && !navOrderChecked)
                return isInSchema;
            if (navOrderChecked && !dispOrderChecked)
                return String.Format("{0:#.###;(#.###);}{1}", navigationalOrder, isInSchema);
            if ( //dispOrderChecked &&
                !navOrderChecked)
                return String.Format("{0:#.###;(#.###);}{1}", displayOrder, isInSchema);
            // Both are checked
            if (navigationalOrder > 0 || displayOrder > 0)
                return String.Format("{1:0.###} {0} {2:0.###}", inSchema ? Bullet : NotInSchema, navigationalOrder,
                                     displayOrder);
            return isInSchema;
        }

        private string GetCellValue2(object cellValue)
        {
            var value2 = String.Empty;
            if (cellValue != null && cellValue is EntityData)
            {
                var ed = (EntityData)cellValue;
                if (chkUom.Checked)
                    value2 = ed.Uom ?? String.Empty;
                else if (chkField1.Checked)
                    value2 = ed.Field1 ?? String.Empty;
                else if (chkField2.Checked)
                    value2 = ed.Field2 ?? String.Empty;
                else if (chkField3.Checked)
                    value2 = ed.Field3 ?? String.Empty;
                else if (chkField4.Checked)
                    value2 = ed.Field4 ?? String.Empty;
                else if (chkField5.Checked)
                    value2 = ed.Field5OrStatus ?? String.Empty;
            }
            return value2;
        }

        private void GetColorRules()
        {
            AryaTools.Instance.Forms.ColorForm.LoadRules(_colorRuleAttributes, _colorRules);
            AryaTools.Instance.Forms.ColorForm.ShowDialog();
            if (AryaTools.Instance.Forms.ColorForm.DialogResult == DialogResult.OK)
            {
                _colorRules = AryaTools.Instance.Forms.ColorForm.WorkingRules.ToList();
                _objectColors = new Dictionary<object, CustomKeyValuePair<Color, Color>>();
                edgv.Invalidate();
            }
        }

        private void GetColorsBasedOnRules(EntityData ed, ref Color back, ref Color fore)
        {
            foreach (var rule in _colorRules)
            {
                switch (rule.RuleAttribute.AttributeName)
                {
                    case "Value":
                        if (rule.IsValidValue(ed.Value))
                        {
                            back = rule.BackColor;
                            fore = rule.TextColor;
                            return;
                        }
                        break;
                    case "SpellCheck":
                        if (rule.IsValidValue(ed))
                        {
                            back = rule.BackColor;
                            fore = rule.TextColor;
                            return;
                        }
                        break;
                    case "Uom":
                        if (rule.IsValidValue(ed.Uom))
                        {
                            back = rule.BackColor;
                            fore = rule.TextColor;
                            return;
                        }
                        break;

                    case "CreatedOn":
                        if (rule.IsValidValue(ed.EntityInfo.EntityDatas.Select(e => e.CreatedOn).Min()))
                        {
                            back = rule.BackColor;
                            fore = rule.TextColor;
                            return;
                        }
                        break;

                    case "LastUpdatedOn":
                        if (rule.IsValidValue(ed.CreatedOn))
                        {
                            back = rule.BackColor;
                            fore = rule.TextColor;
                            return;
                        }
                        break;

                    case "Schematus":
                        if (rule.IsValidValue(ed))
                        {
                            back = rule.BackColor;
                            fore = rule.TextColor;
                            return;
                        }
                        break;

                    default:
                        back = Color.Empty;
                        fore = Color.Empty;

                        throw new ArgumentOutOfRangeException();
                }
            }
            //back = Color.White;
            //fore = Color.Black;
        }

        private IEnumerable<ListViewItem> GetFilterItems(int columnIndex, string filterField, out string attributeName,
                                                         out double fillRate)
        {
            string fieldName;
            Attribute attribute = null;
            var allValues = new Dictionary<string, ListViewItem>();
            //Taxonomy Filter
            if (columnIndex == 1)
            {
                fieldName = attributeName = FilterTaxonomyFieldname;
                fillRate = 100;

                // First create a list of all values in this load
                foreach (var kvp in _skus)
                {
                    var value = kvp.Value.Taxonomy.ToString();

                    //SubItems[0]
                    if (allValues.ContainsKey(value))
                        allValues[value].SubItems[1].Text = String.Format("{0}",
                                                                          Int32.Parse(allValues[value].SubItems[1].Text)
                                                                          + 1);
                    else
                        allValues.Add(value, new ListViewItem(new[] { value, "1", "0", "" }));
                }

                // Append counts for visible values
                foreach (DataGridViewRow row in edgv.Rows)
                {
                    var value = _skus[((Sku)row.Cells[0].Value).ID].Taxonomy.ToString();

                    //SubItems[1]
                    if (allValues.ContainsKey(value))
                        allValues[value].SubItems[2].Text = String.Format("{0}",
                                                                          Int32.Parse(allValues[value].SubItems[2].Text)
                                                                          + 1);
                    else
                        allValues.Add(value, new ListViewItem(new[] { value, "0", "1", "" }));
                }
            }
            else
            {
                int iteration;
                var headerText = edgv.Columns[columnIndex].HeaderText;
                SplitHeaderText(headerText, out attributeName, out iteration);
                fieldName = attributeName.ToLower();
                attribute = _allAttributes[fieldName];

                // First create a list of all values in this load
                var skuCount = 0;
                foreach (var kvp in _skus)
                {
                    var sku = kvp.Value;
                    var vals =
                        sku.GetValuesForAttribute(fieldName).Select(ed => GetSortFilterValue(ed, filterField)).ToList();

                    if (vals.Any())
                        skuCount++;
                    else
                        vals.Add(BlankValue);

                    vals.ForEach(value =>
                        {
                            if (value == string.Empty)
                                value = EmptyValue;
                            //SubItems[0]
                            if (allValues.ContainsKey(value))
                                allValues[value].SubItems[1].Text = String.Format("{0}",
                                                                                  Int32.Parse(
                                                                                      allValues[value].
                                                                                          SubItems[1].Text)
                                                                                  + 1);
                            else
                                allValues.Add(value, new ListViewItem(new[] { value, "1", "0", "" }));
                        });
                }
                fillRate = 100.0 * skuCount / _skus.Count;

                // Append counts for visible values
                foreach (DataGridViewRow row in edgv.Rows)
                {
                    var vals =
                        _skus[((Sku)row.Cells[0].Value).ID].GetValuesForAttribute(fieldName).Select(
                            ed => GetSortFilterValue(ed, filterField)).ToList();
                    if (vals.Count == 0)
                        vals.Add(BlankValue);

                    vals.ForEach(value =>
                                     {
                                         if (value == string.Empty)
                                             value = EmptyValue;
                                         //SubItems[1]
                                         if (allValues.ContainsKey(value))
                                             allValues[value].SubItems[2].Text = String.Format("{0}",
                                                                                               Int32.Parse(
                                                                                                   allValues[value].
                                                                                                       SubItems[2].Text)
                                                                                               + 1);
                                         else
                                             allValues.Add(value, new ListViewItem(new[] { value, "0", "1", "" }));
                                     });
                }
            }

            // Assign current selections
            var filter = _currentFilters.FirstOrDefault(f => f.LowerAttributeName.Equals(fieldName));
            if (filter != null)
            {
                filter.FilterValues.ForEach(item =>
                                                {
                                                    if (allValues.ContainsKey(item))
                                                        allValues[item].Selected = true;
                                                });
            }

            var values = HelperClasses.Validate.GetLovs(attribute, CurrentTaxonomy);

            if (values != null)
            {
                var lovs = values.ToHashSet();

                foreach (var val in allValues)
                    val.Value.SubItems[3].Text = lovs.Contains(val.Key) ? "✓" : "?";

                lovs.Where(val => !allValues.ContainsKey(val)).ForEach(
                    val => allValues.Add(val, new ListViewItem(new[] { val, "0", "0", "▤" })));

                foreach (var item in allValues)
                {
                    switch (item.Value.SubItems[3].Text)
                    {
                        case "▤":
                            item.Value.ForeColor = Color.DodgerBlue;
                            break;
                        case "✓":
                            item.Value.ForeColor = Color.DarkGreen;
                            break;
                        case "?":
                            item.Value.ForeColor = Color.Crimson;
                            break;
                    }
                }
            }

            return
                allValues.Select(kvp => kvp.Value).OrderBy(item => item.Text, new CompareForAlphaNumericSort()).ToList();
        }

        private static string GetGoogleUrl(string searchString) { return String.Format("http://www.google.com/search?q={0}", searchString); }

        private static string GetHeaderText(string attributeOrders, string attributeName, int attributeIteration)
        {
            var navigationalOrder = attributeIteration > 0 ? String.Empty : attributeOrders;
            var nthIteration = attributeIteration == 0 ? String.Empty : attributeIteration.ToString();
            return String.Format("{0}\n{1}\n{2}", navigationalOrder, attributeName, nthIteration);
        }

        private int GetListSize(int constantAttCount, int skuCount, int valueThreshold)
        {
            const int MaxListOfSkusAllowed = 1500;

            if (skuCount == 0 || constantAttCount == 0)
                return MaxListOfSkusAllowed;

            if (skuCount * constantAttCount > valueThreshold)
            {
                var listSize = valueThreshold / constantAttCount;
                if (listSize > MaxListOfSkusAllowed)
                    return MaxListOfSkusAllowed;
                return listSize;
            }
            return MaxListOfSkusAllowed;
        }

        private static Point GetLocation(Control control)
        {
            if (control == null || control.GetType().BaseType == typeof(Form))
                return new Point(0, 0);
            var controlLocation = control.Location;
            var parentLocation = GetLocation(control.Parent);
            return new Point(parentLocation.X + controlLocation.X, parentLocation.Y + controlLocation.Y);
        }

        private static DataGridViewColumn GetNewColumn(string headerText, DataGridViewCell cellTemplate, bool frozen,
                                                       int width)
        {
            var newColumn = new DataGridViewColumn
                                {
                                    Name = headerText,
                                    HeaderText = headerText,
                                    CellTemplate = cellTemplate,
                                    Frozen = frozen,
                                    Width = width,
                                    FillWeight = 1,
                                    //HeaderCell =
                                    //    {
                                    //        ToolTipText =
                                    //            "Line 1: Attribute Orders\nLine 2: Attribute Name\nLine 3: nth Value for attribute (if any)"
                                    //    }
                                };
            return newColumn;
        }

        private void GetSelectedCells()
        {
            var selectedCells = edgv.SelectedCells;
            if (edgv.CurrentCellAddress.X == -1 || edgv.CurrentCellAddress.Y == -1 || selectedCells.Count == 0)
            {
                CurrentChange = null;
                return;
            }

            while (edgv.SelectedColumns.Count > 0)
                edgv.SelectedColumns[0].Selected = false;
            var columnIndexes =
                edgv.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.ColumnIndex).Distinct().ToList();
            if (columnIndexes.Count() > 1)
            {
                var multipleSelected = false;
                var headers =
                    edgv.Columns.Cast<DataGridViewColumn>().Where(col => columnIndexes.Contains(col.Index)).Select(
                        col => col.HeaderText);

                string previousFieldName = null;
                foreach (var headerText in headers)
                {
                    int iteration;
                    string attributeName;
                    SplitHeaderText(headerText, out attributeName, out iteration);
                    if (previousFieldName == null)
                        previousFieldName = attributeName;
                    else if (!previousFieldName.Equals(attributeName))
                        multipleSelected = true;
                }

                if (multipleSelected)
                {
                    var iFirstColumn = edgv.CurrentCell.ColumnIndex;
                    edgv.SelectedCells.Cast<DataGridViewCell>().ForEach(cell =>
                                                                            {
                                                                                if (cell.ColumnIndex != iFirstColumn)
                                                                                    cell.Selected = false;
                                                                            });
                }
            }

            CreateNewChange(edgv.SelectedCells);
        }

        private string GetSortFilterValue(object value, string field)
        {
            if (field.Equals(SortColor) || field.Equals(FilterColor))
            {
                if (value != null && _objectColors.ContainsKey(value))
                {
                    var backColor = _objectColors[value].Key;
                    return backColor.ToArgb().ToString();
                }
                return Color.Transparent.ToArgb().ToString();
            }
            if (!(value is EntityData))
                return value.ToString();
            var ed = (EntityData)value;
            
            if (field.Equals(SortValue) || field.Equals(FilterValue))
                return ed.Value;
            if (field.Equals(SortUom) || field.Equals(FilterUom))
                return ed.Uom ?? String.Empty;
            if (field.Equals(SortFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField1Name)
                || field.Equals(FilterFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField1Name))
                return ed.Field1 ?? String.Empty;
            if (field.Equals(SortFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField2Name)
                || field.Equals(FilterFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField2Name))
                return ed.Field2 ?? String.Empty;
            if (field.Equals(SortFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField3Name)
                || field.Equals(FilterFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField3Name))
                return ed.Field3 ?? String.Empty;
            if (field.Equals(SortFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField4Name)
                || field.Equals(FilterFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField4Name))
                return ed.Field4 ?? String.Empty;
            if (field.Equals(SortFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField5Name)
                || field.Equals(FilterFieldPrefix + AryaTools.Instance.InstanceData.CurrentProject.EntityField5Name))
                return ed.Field5OrStatus ?? String.Empty;
            return value.ToString();
        }

        private static string GetWikiPageText(WebClient client, Uri url)
        {
            const string NotFoundText = @"Wikipedia does not have an article with this exact name";
            const RegexOptions RxOptions = (RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var rxTables = new Regex("<table.*?/table>", RxOptions);
            var rxFirstParagraph = new Regex("<p>.+?</p>", RxOptions);
            var rxSearchResults = new Regex("<title>.+Search results.+</title>", RxOptions);
            var rxDisambiguation = new Regex("Category:Disambiguation[ _]pages", RxOptions);
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");
            try
            {
                var page = client.DownloadString(url);
                if (page.Contains(NotFoundText) || rxSearchResults.IsMatch(page) || rxDisambiguation.IsMatch(page))
                    return null;

                var removeTables = rxTables.Replace(page, String.Empty);
                var firstParagraph = rxFirstParagraph.Match(removeTables).ToString();
                var snippet = ApplyFont(firstParagraph);

                return snippet;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetWikiUrl(string searchString)
        {
            return String.Format("http://en.wikipedia.org/wiki/Special:Search?search={0}&go=Go",
                                 searchString.Replace(" ", "%20"));
        }

        private void GroupUndo(Stack<ChangeItem> undoHistory, Stack<ChangeItem> redoHistory)
        {
            if (undoHistory.Count == 0)
                return;
            var changeItem = undoHistory.Peek();
            var changeId = changeItem._changeId;
            EntityData entityDataToSelect;
            var refreshSku = undoHistory.Pop().Undo(undoHistory, redoHistory, out entityDataToSelect);
            if (changeItem._oldEntityData != null && _noOfColumns.ContainsKey(changeItem._oldEntityData.Attribute))
                _noOfColumns.Remove(changeItem._oldEntityData.Attribute);
            if (changeItem._newEntityData != null && _noOfColumns.ContainsKey(changeItem._newEntityData.Attribute))
                _noOfColumns.Remove(changeItem._newEntityData.Attribute);
            if (refreshSku != null)
            {
                var skuRow =
                    edgv.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => row.Cells[0].Value.Equals(refreshSku));
                if (skuRow != null)
                {
                    var refreshRowIndex = skuRow.Index;
                    edgv.InvalidateRow(refreshRowIndex);
                }
            }
            if (undoHistory.Count > 0 && undoHistory.Peek()._changeId == changeId)
            {
                GroupUndo(undoHistory, redoHistory);
                return;
            }
            if (entityDataToSelect != null)
                SelectCell(edgv, entityDataToSelect);
            else
                SelectCell(edgv, refreshSku);
            //if (CurrentTaxonomy != null)
            //    CurrentTaxonomy.TryReorderAttributeOrders(false);
            RedrawDataGridView(null);
            //TryHideColumns();
        }

        private bool HandleEnterAndEscapeForChange(Keys keyCode)
        {
            if (CurrentChange == null)
                return false;
            switch (keyCode)
            {
                case Keys.Escape:
                    CurrentChange.NewValues = new Change.AllFields();
                    PopulateProperties();
                    edgv.Focus();
                    return true;

                case Keys.Enter:
                    SaveCurrentChangeAndGetNewChange(false);
                    edgv.Focus();
                    return true;
            }
            return false;
        }

        private void InitData()
        {
            AryaTools.Instance.Forms.SkuForm.lblCellToolTip.Text = String.Empty;
            SetStatus("Init");
            _workerStartTime = DateTime.Now;
            tblAttributeOrders.Visible = CurrentTaxonomy != null;
            tblRibbon.Visible = false;
            ShowAuditTrail = false;
            ShowLinks = false;
            edgv.Rows.Clear();
            edgv.Columns.Clear();
            _workerState = WorkerState.Working;
            statusTimer.Start();
            WorkerThread = new Thread(PrepareData) { IsBackground = true };
            //DataValidaionThread.Start();
            WorkerThread.Start();
            _validatedData.Clear();
        }

        private void InitEntityDataGridView()
        {
            sortByComboToolStripMenuItem.Items.Clear();
            sortByComboToolStripMenuItem.Items.Add(SortValue);
            sortByComboToolStripMenuItem.Items.Add(SortColor);
            sortByComboToolStripMenuItem.Items.Add(SortUom);
            sortByComboToolStripMenuItem.SelectedIndex = 0;
            filterByComboToolStripMenuItem.Items.Clear();
            filterByComboToolStripMenuItem.Items.Add(FilterValue);
            filterByComboToolStripMenuItem.Items.Add(FilterUom);
            filterByComboToolStripMenuItem.SelectedIndex = 0;
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField1Name, chkField1, txtField1,
                           AryaTools.Instance.InstanceData.CurrentProject.EntityField1Readonly);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField2Name, chkField2, txtField2,
                           AryaTools.Instance.InstanceData.CurrentProject.EntityField2Readonly);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField3Name, chkField3, txtField3,
                           AryaTools.Instance.InstanceData.CurrentProject.EntityField3Readonly);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField4Name, chkField4, txtField4,
                           AryaTools.Instance.InstanceData.CurrentProject.EntityField4Readonly);
            SetEntityField(AryaTools.Instance.InstanceData.CurrentProject.EntityField5Name, chkField5, txtField5,
                           AryaTools.Instance.InstanceData.CurrentProject.EntityField5Readonly);
        }

        private bool IsSingleDimensionEntitySet()
        {
            var uomList = new List<UnitOfMeasure>();
            var selectedCells = edgv.SelectedCells.Cast<DataGridViewCell>();

            foreach (var ed in
                selectedCells.Select(c => c.Value).OfType<EntityData>().Select(entityData => (entityData)))
            {
                if (String.IsNullOrEmpty(ed.Value) || String.IsNullOrEmpty(ed.Uom))
                    return false;

                var baseUom = Arya.HelperClasses.BaseUnitConversion.GetBaseUom(ed.Uom);
                if (baseUom == null)
                    return false;

                if (!uomList.Contains(baseUom))
                    uomList.Add(baseUom);

                if (uomList.Count > 1)
                    return false;
            }

            if (uomList.Count != 1)
                return false;

            var baseUomConversion =
                Arya.HelperClasses.BaseUnitConversion.ProjectUoms.FirstOrDefault(u => uomList[0].ID == u.UomID).AsEnumerable().ToList();
            if (baseUomConversion.First() == null)
            {
                MessageBox.Show("The project uoms are not setup correctly.There are uoms in used but not added in the project uom table.  Please contact  project lead.");
                return false;
            }
            if (baseUomConversion.Count() != 1)
                return false;

            var convertTo =
                Arya.HelperClasses.BaseUnitConversion.ProjectUoms.Where(
                    pu => pu.UnitOfMeasure.BaseUom != null && pu.UnitOfMeasure.BaseUom == uomList[0]).Union(
                        baseUomConversion).OrderBy(u => u.UnitOfMeasure.UnitName).ToArray();

            //convertToUomDropDown.ComboBox.DataSource = convertTo;
            //convertToUomDropDown.ComboBox.DisplayMember = "Display";
            //convertToUomDropDown.ComboBox.ValueMember = "Value";
            convertToUomDropDown.ComboBox.Items.Clear();
            convertToUomDropDown.ComboBox.Items.AddRange(convertTo);

            return true;
        }

        private void loadFiltersFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogXml.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                var deserializer = new XmlSerializer(_currentFilters.GetType());
                TextReader file = new StreamReader(openFileDialogXml.FileName);

                var newFilters = (List<FilterEntity>)deserializer.Deserialize(file);
                _currentFilters.Clear();
                _currentFilters.AddRange(newFilters);

                var newSkuOrder = ApplyCurrentFilters();
                var oldOrder = AryaTools.Instance.Forms.SkuOrders[_instanceId];
                AryaTools.Instance.Forms.SkuOrders[_instanceId] = newSkuOrder.OrderBy(oldOrder.IndexOf).ToList();

                RedrawDataGridView(null);
                edgv.RowCount = newSkuOrder.Count;
                edgv.Invalidate();
                UpdateFilterStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load from file: " + ex.Message);
            }
        }

        private bool LoadSkusAndAttributes()
        {
            var result = DialogResult.No;
            var waitkey = FrmWaitScreen.ShowMessage("Loading SKUs");
            _workerStatus = String.Format("Fetching Taxonomy");
            //loadQuerySkus = _loadQuery.ToList();

            _workerStatus = String.Format("Fetching SKUs");
            var createSkuOrder = true;
            _skus = new Dictionary<Guid, Sku>();
            if (!AryaTools.Instance.Forms.SkuOrders.ContainsKey(_instanceId))
                AryaTools.Instance.Forms.SkuOrders.Add(_instanceId, new List<Guid>());
            else
                createSkuOrder = false;
            var skuExclusions = AryaTools.Instance.InstanceData.CurrentUser.SkuExclusions.ToList();

            _skus =
                LoadQueryResults.Where(sku => !skuExclusions.Contains(sku.ID)).ToList().Distinct(
                    new KeyEqualityComparer<Sku>(sku => sku.ID)).ToDictionary(sku => sku.ID, sku => sku);

            //SkuOrder Reresh after SkuMoves
            if (AryaTools.Instance.Forms.SkuOrders[_instanceId].Count != _skus.Count
                && AryaTools.Instance.Forms.SkuOrders[_instanceId].Count > 0)
                createSkuOrder = true;

            if (createSkuOrder)
                AryaTools.Instance.Forms.SkuOrders[_instanceId] = _skus.Select(sku => sku.Key).ToList();

            var bStripAttributes = false; // <-0->...

            if (_skus.Count > AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.SkuThreshold
                && _showSkuThresholdWindow)
            {
                _showSkuThresholdWindow = false;
                //if (CurrentTaxonomies != null && CurrentTaxonomies.Count() == 1 && DoesUserPreferenceHaveAttributeConfiured())// only single taxonomy
                if (CurrentTaxonomy != null && DoesUserPreferenceHaveAttributeConfiured()) // only single taxonomy
                {
                    result = DisplaySkuThresholdExceedWarningModal(true);
                    if (result == DialogResult.No)
                        _showAttributesWhenSkuThresholdExceeds = false;

                    else if (result == DialogResult.Cancel)
                        return false;
                }
                else // multiple taxnomy present or for single taxonomy no user preferene set
                {
                    result = DisplaySkuThresholdExceedWarningModal(false);
                    if (result == DialogResult.No)
                    {
                        FrmWaitScreen.HideMessage(waitkey);
                        return false;
                    }
                    _showAttributesWhenSkuThresholdExceeds = false;
                }
            }


            if (CurrentTaxonomy != null)
                _taxonomyNodes = new List<TaxonomyInfo> { CurrentTaxonomy };
            else
            {
                _taxonomyNodes = new List<TaxonomyInfo>();
                _taxonomyNodes =
                    LoadQueryResults.SelectMany(s => s.SkuInfos.Where(a => a.Active)).Select(t => t.TaxonomyInfo).
                        Distinct().ToList();
            }
            _taxonomyNodes =
                _taxonomyNodes.Where(
                    ti => !AryaTools.Instance.InstanceData.CurrentUser.TaxonomyExclusions.Contains(ti.ID)).ToList();

            //Fetch SKU Attributes
            //if (bStripAttributes == false)
            _workerStatus = String.Format("{0} SKUs, Fetching SKU Attributes", _skus.Count);

            var attributeIDs = (from sku in LoadQueryResults
                                from ei in sku.EntityInfos
                                from ed in ei.EntityDatas
                                where ed.Active
                                select ed.AttributeID).Distinct().ToList();

            var skuAttributes = new List<Attribute>();
            var workflowAttributes = new List<Attribute>(); // <-0->
            //var workflowAttributes =
            //    AryaTools.Instance.InstanceData.Dc.Attributes.Where(a => a.AttributeType == "Workflow").ToList();

            //if (bStripAttributes == false)
            //{
            workflowAttributes =
                AryaTools.Instance.InstanceData.Dc.Attributes.Where(
                    a => a.AttributeType == Framework.Data.AryaDb.AttributeTypeEnum.Workflow.ToString()).ToList();

            var attrs = SplitSkuList(attributeIDs, 1500);
            Parallel.ForEach(attrs, attr =>
                                        {
                                            var attr1 = attr;
                                            var rets =
                                                AryaTools.Instance.InstanceData.Dc.Attributes.Where(
                                                    a =>
                                                    Framework.Data.AryaDb.Attribute.NonMetaAttributeTypes.Contains(
                                                        a.AttributeType) && attr1.Contains(a.ID)).ToList();
                                            skuAttributes.AddRange(rets);
                                        });
            //}

            //Fetch Schema Attributes
            var schemaAttributes = new List<Attribute>();

            //if (bStripAttributes == false)
            //{
            _workerStatus = String.Format("{0} SKUs, Fetching Schema Attributes", _skus.Count);
            schemaAttributes = (from tax in _taxonomyNodes
                                from si in tax.SchemaInfos
                                where si.SchemaDatas.Any(sd => sd != null && sd.Active)
                                group si by si.Attribute
                                    into attGrp
                                    select attGrp.Key).ToList();
            //}
            //Fetch all Calculated attributes and add to the list
            var calculatedAttributes = new List<Attribute>();

            //if (bStripAttributes == false)
            //{
            _workerStatus = String.Format("{0} SKUs, Fetching Calculated Attributes", _skus.Count);
            _taxonomyNodes.ForEach(tax =>
                                       {
                                           var calculatedAtts =
                                               TaxonomyInfo.GetCalculatedAttributes(tax).Where(
                                                   da => !calculatedAttributes.Contains(da));
                                           calculatedAttributes.AddRange(calculatedAtts);
                                       });

            calculatedAttributes.AddRange(TaxonomyInfo.GetCalculatedAttributes(default(Guid)));

            calculatedAttributes = calculatedAttributes.Distinct().ToList();
            // }

            _allAttributes =
                skuAttributes.Union(schemaAttributes).Union(calculatedAttributes).Union(workflowAttributes).Distinct(
                    new KeyEqualityComparer<Attribute>(att => att.AttributeName.ToLower())).ToDictionary(
                        att => att.AttributeName.ToLower(), att => att);

            var allAtts = _allAttributes.Values.ToList();

            _workerStatus = String.Format("{0} SKUs, {1} Attributes", _skus.Count, _allAttributes.Count);
            //_workerStatus = string.Format("Reading {0} of {1} SKUs, {2} Attributes", skuOrder.Count, skuCount, _allAttributes.Count);
            if (!createSkuOrder)
            {
                _workerStatus = String.Format("{0} SKUs, {1} Attributes, Cleaning SKU List", _skus.Count,
                                              _allAttributes.Count);
                var toRemove =
                    AryaTools.Instance.Forms.SkuOrders[_instanceId].Where(skuId => !_skus.ContainsKey(skuId)).ToList();
                toRemove.ForEach(id => AryaTools.Instance.Forms.SkuOrders[_instanceId].Remove(id));
            }

            //var cache = AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache;

            var allSkus = _skus.Values.ToList();

            //clear the cache for the skus and attributes
            foreach (var att in allAtts)
            {
                if (!AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(att))
                    continue;
                var att1 = att;
                foreach (var sku in
                    allSkus.Where(
                        sku => AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[att1].ContainsKey(sku)))
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[att].Remove(sku);
            }

            FrmWaitScreen.HideMessage(waitkey);
            return true;
        }


        private DialogResult DisplaySkuThresholdExceedWarningModal(bool isSingleNodeModal)
        {
            var result = DialogResult.None;
            if (isSingleNodeModal)
            {
                var sModalText =
                    String.Format(
                        "Node: {0} has {1} Skus. This may take some time.Do you still want to open the nodes?\nYes: Preload SKUs and default Attributes, \nNo: Load SKUs, choose Attributes later, \nCancel: Stop loading the tab",
                        CurrentTaxonomy.NodeName, _skus.Count);
                Invoke(
                    (MethodInvoker)
                    delegate { result = MessageBox.Show(this, sModalText, "Alert", MessageBoxButtons.YesNoCancel); });
            }
            else
            {
                var sModalText =
                    String.Format(
                        "These nodes have {0} SKUs. This may take some time. Do you still want to open the nodes?\nYes: Load SKUs and choose attributes,\nNo: Stop loading the tab.",
                        _skus.Count);
                Invoke(
                    (MethodInvoker)
                    delegate { result = MessageBox.Show(this, sModalText, "Alert", MessageBoxButtons.YesNo); });
            }

            return result;
        }


        private bool DoesUserPreferenceHaveAttributeConfiured()
        {
            var currentUserPreference = AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences;
            if (currentUserPreference.GlobalAttributes || currentUserPreference.InSchemaAttributes
                || currentUserPreference.RankedAttributes || currentUserPreference.ProductAttributes
                || currentUserPreference.AttributeGroupInclusions.Count() != 0
                || currentUserPreference.AttributeGroupInclusions.Count() != 0)
                return true;
            return false;
        }

        private void MoveSku(object sender, EventArgs e)
        {
            Enabled = false;
            AryaTools.Instance.Forms.TreeForm.GetTaxonomy("Move Here", false);
            AryaTools.Instance.Forms.TreeForm.TaxonomySelected += SelectMoveSkuTaxonomy;
            AryaTools.Instance.Forms.TreeForm.BringToFront();
        }

        private void SelectMoveSkuTaxonomy(object sender, EventArgs e)
        {
            Enabled = true;

            if (AryaTools.Instance.Forms.TreeForm.DialogResult == DialogResult.OK)
            {
                CurrentChange.NewValues.Tax = AryaTools.Instance.Forms.TreeForm.taxonomyTree.SelectedTaxonomy;
                SaveCurrentChangeAndGetNewChange(false);
                GetSelectedCells();
                PopulateProperties();
            }

            BringToFront();
            edgv.Focus();
        }

        private void newEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //chkCreateNewEntityDatas.Checked = true;
            CreateNewEntityData = newEntitiesToolStripMenuItem.Checked;
        }

        private void openAllNodesInSchemaViewToolStripMenuItem_Click(object sender, EventArgs e) { AryaTools.Instance.Forms.SchemaForm.LoadTab(_taxonomyNodes.Cast<object>().ToList(), null); }

        private void openCurrentNodeInSchemaViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var taxonomy = edgv.CurrentCell.OwningRow.Cells[1].Value;
            AryaTools.Instance.Forms.SchemaForm.LoadTab(new List<object> { taxonomy }, null);
        }

        private static void OpenInNewTab(TaxonomyInfo taxonomy)
        {
            var skuQuery = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
                           let sku = si.Sku
                           where
                               si.Active && sku.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject)
                               && taxonomy.Equals(si.TaxonomyInfo)
                           select sku;
            AryaTools.Instance.Forms.SkuForm.LoadTab(skuQuery, taxonomy, taxonomy.TaxonomyData.NodeName,
                                                        taxonomy.ToString());
        }

        private void openInNewTabToolStripMenuItem_Click(object sender, EventArgs e) { OpenInNewTab((TaxonomyInfo)edgv[1, edgv.CurrentCellAddress.Y].Value); }

        private void PlaceSkuImageBox()
        {
            skuImageBox.Left = this.Right - skuImageBox.Width - 3;
            skuImageBox.Top = tblRibbon.Top + 3;
        }

        private void PopulateAttributeDropdownDataSource(ComboBox attributeDropdown)
        {
            attributeDropdown.DataSource =
                _allAttributes.Values.Select(att => att.AttributeName).OrderBy(att => att).ToList();
        }

        private void PopulateAuditTrail()
        {
            dgvAuditTrail.DataSource = null;
            dgvAuditTrail.Columns.Clear();
            if (edgv.SelectedCells.Count != 1 || !ShowAuditTrail)
                return;
            try
            {
                var cell = edgv.SelectedCells[0];
                var cellDataType = cell.Value.GetType();

                BindingSource bs = null;

                if (cellDataType == typeof(EntityData))
                {
                    bs =
                        new BindingSource(
                            ((EntityData)cell.Value).EntityInfo.EntityDatas.OrderBy(ed => ed.CreatedOn).Select(
                                ed =>
                                new
                                    {
                                        ed.Active,
                                        Before = ed.BeforeEntity,
                                        ed.EntityInfo.Sku.ItemID,
                                        ed.Attribute.AttributeName,
                                        ed.Value,
                                        ed.Uom,
                                        ed.CreatedOn,
                                        CreatedRemark = (ed.CreatedRemark == null) ? String.Empty : ed.Remark.Remark1,
                                        ed.Field1,
                                        ed.DeletedOn,
                                        DeletedRemark = (ed.DeletedRemark == null) ? String.Empty : ed.Remark1.Remark1,
                                        ed.Field2,
                                        ed.Field3,
                                        ed.Field4,
                                        ed.Field5OrStatus,
                                        CreatedBy =
                                    ed.User != null && !String.IsNullOrWhiteSpace(ed.User.FullName)
                                        ? ed.User.FullName
                                        : ed.CreatedBy.ToString(),
                                        DeletedBy =
                                    ed.User1 != null && ed.DeletedBy != null
                                    && !String.IsNullOrWhiteSpace(ed.User1.FullName)
                                        ? ed.User1.FullName
                                        : ed.DeletedBy == null ? String.Empty : ed.DeletedBy.ToString(),
                                        ed.ID,
                                        ed.EntityID,
                                        ed.AttributeID,
                                        ed.EntityInfo.SkuID
                                    }).ToDataTable(), null);
                }
                else if (cellDataType == typeof(Sku))
                {
                    bs =
                        new BindingSource(
                            ((Sku)cell.Value).EntityInfos.SelectMany(ei => ei.EntityDatas).OrderBy(
                                ed =>
                                ed.EntityInfo.EntityDatas.OrderBy(ed1 => ed1.CreatedOn).First().Attribute.AttributeName)
                                .ThenBy(ed => ed.EntityID).ThenBy(ed => ed.CreatedOn).Select(
                                    ed =>
                                    new
                                        {
                                            ed.Active,
                                            Before = ed.BeforeEntity,
                                            ed.EntityInfo.Sku.ItemID,
                                            ed.Attribute.AttributeName,
                                            ed.Value,
                                            ed.Uom,
                                            ed.Field1,
                                            ed.CreatedOn,
                                            CreatedRemark = (ed.CreatedRemark == null) ? String.Empty : ed.Remark.Remark1,
                                            ed.DeletedOn,
                                            DeletedRemark = (ed.DeletedRemark == null) ? String.Empty : ed.Remark1.Remark1,
                                            //DeletedRemark = ed.Remark1.Remark1,
                                            ed.Field2,
                                            ed.Field3,
                                            ed.Field4,
                                            ed.Field5OrStatus,
                                            CreatedBy =
                                        ed.User != null && !String.IsNullOrWhiteSpace(ed.User.FullName)
                                            ? ed.User.FullName
                                            : ed.CreatedBy.ToString(),
                                            DeletedBy =
                                        ed.User1 != null && ed.DeletedBy != null
                                        && !String.IsNullOrWhiteSpace(ed.User1.FullName)
                                            ? ed.User1.FullName
                                            : ed.DeletedBy == null ? String.Empty : ed.DeletedBy.ToString(),
                                            ed.ID,
                                            ed.EntityID,
                                            ed.AttributeID,
                                            ed.EntityInfo.SkuID
                                        }).ToDataTable(), null);
                }
                else if (cellDataType == typeof(TaxonomyInfo))
                {
                    var sku = (Sku)cell.OwningRow.Cells[0].Value;
                    var skuInfos =
                        sku.SkuInfos.Select(
                            si =>
                            new
                                {
                                    Taxonomy = si.TaxonomyInfo.ToString(),
                                    SkuAssignedOn = si.CreatedOn,
                                    SkuAssignedBy = si.User.FullName,
                                    SkuAssignRemark = (si.CreatedRemark == null) ? String.Empty : si.Remark.Remark1,
                                    TaxonomyCreatedOn =
                                si.TaxonomyInfo.TaxonomyData == null
                                    ? string.Empty
                                    : si.TaxonomyInfo.TaxonomyData.CreatedOn.ToString(),
                                    TaxonomyCreatedBy =
                                si.TaxonomyInfo.TaxonomyData == null
                                    ? string.Empty
                                    : si.TaxonomyInfo.TaxonomyData.User.FullName
                                });

                    bs =
                        new BindingSource(
                            skuInfos.OrderBy(st => st.SkuAssignedOn).ThenBy(st => st.TaxonomyCreatedOn).ToDataTable(),
                            null);
                }

                if (bs != null)
                    dgvAuditTrail.DataSource = bs;
            }
            catch (Exception)
            {
            }
        }

        private void PopulateBeforeEntity()
        {
            var showBeforeEntityGroup = false;
            if (edgv.SelectedCells.Count == 1)
            {
                lblBeforeEntity.Text = @"Before Entity";
                lblBeforeAttribute.Text = String.Empty;
                lblBeforeValue.Text = String.Empty;
                lblCreatedRemark.Text = String.Empty;
                //  lblDeletedRemark.Text = string.Empty;
                lblBeforeUom.Text = String.Empty;
                lblBeforeField1.Text = String.Empty;
                lblBeforeField2.Text = String.Empty;
                lblBeforeField3.Text = String.Empty;
                lblBeforeField4.Text = String.Empty;
                lblBeforeField5.Text = String.Empty;
                lblBeforeDate.Text = String.Empty;

                var currentCellValue = edgv.CurrentCell.Value;

                var currentEd = currentCellValue as EntityData;
                if (currentEd != null)
                {
                    if (currentEd.CreatedRemark != null && currentEd.Remark != null)
                    {
                        lblCreatedRemark.Text = String.Format("Created Remark: {0}", currentEd.Remark.Remark1);
                        //lblCreatedRemark.Font = currentEd.Value.Equals(currentEd.Value) ? fontRegular : fontBold;
                    }

                    if (currentEd.BeforeEntity)
                    {
                        showBeforeEntityGroup = true;
                        lblBeforeEntity.Text = @"This is the 'before' value";
                    }
                    else if (currentEd.EntityInfo != null)
                    {
                        var beforeEd = currentEd.EntityInfo.EntityDatas.FirstOrDefault(ed => ed.BeforeEntity);
                        if (beforeEd != null)
                        {
                            showBeforeEntityGroup = true;
                            lblBeforeAttribute.Text = String.Format("Attribute: {0}", beforeEd.Attribute.AttributeName);
                            lblBeforeAttribute.Font = beforeEd.Attribute.Equals(currentEd.Attribute)
                                                          ? DisplayStyle.DefaultRegularFont
                                                          : DisplayStyle.DefaultBoldFont;

                            lblBeforeValue.Text = String.Format("Value: {0}", beforeEd.Value);
                            lblBeforeValue.Font = beforeEd.Value.Equals(currentEd.Value)
                                                      ? DisplayStyle.DefaultRegularFont
                                                      : DisplayStyle.DefaultBoldFont;

                            //if (currentEd.DeletedRemark != null)
                            //lblDeletedRemark.Text = string.Format("Deleted Remark: {0}", currentEd.Remark1.Remark1);

                            if (!String.IsNullOrEmpty(beforeEd.Uom))
                            {
                                lblBeforeUom.Text = String.Format("UoM: {0}", beforeEd.Uom);
                                lblBeforeUom.Font = (beforeEd.Uom).Equals(currentEd.Uom ?? String.Empty)
                                                        ? DisplayStyle.DefaultRegularFont
                                                        : DisplayStyle.DefaultBoldFont;
                            }

                            if (!String.IsNullOrEmpty(beforeEd.Field1))
                            {
                                lblBeforeField1.Text = String.Format("{0}: {1}",
                                                                     AryaTools.Instance.InstanceData.CurrentProject.
                                                                         EntityField1Name, beforeEd.Field1);
                                lblBeforeField1.Font = beforeEd.Field1.Equals(currentEd.Field1 ?? String.Empty)
                                                           ? DisplayStyle.DefaultRegularFont
                                                           : DisplayStyle.DefaultBoldFont;
                            }

                            if (!String.IsNullOrEmpty(beforeEd.Field2))
                            {
                                lblBeforeField2.Text = String.Format("{0}: {1}",
                                                                     AryaTools.Instance.InstanceData.CurrentProject.
                                                                         EntityField2Name, beforeEd.Field2);
                                lblBeforeField2.Font = beforeEd.Field2.Equals(currentEd.Field2 ?? String.Empty)
                                                           ? DisplayStyle.DefaultRegularFont
                                                           : DisplayStyle.DefaultBoldFont;
                            }

                            if (!String.IsNullOrEmpty(beforeEd.Field3))
                            {
                                lblBeforeField3.Text = String.Format("{0}: {1}",
                                                                     AryaTools.Instance.InstanceData.CurrentProject.
                                                                         EntityField3Name, beforeEd.Field3);
                                lblBeforeField3.Font = beforeEd.Field3.Equals(currentEd.Field3 ?? String.Empty)
                                                           ? DisplayStyle.DefaultRegularFont
                                                           : DisplayStyle.DefaultBoldFont;
                            }

                            if (!String.IsNullOrEmpty(beforeEd.Field4))
                            {
                                lblBeforeField4.Text = String.Format("{0}: {1}",
                                                                     AryaTools.Instance.InstanceData.CurrentProject.
                                                                         EntityField4Name, beforeEd.Field4);
                                lblBeforeField4.Font = (beforeEd.Field4).Equals(currentEd.Field4 ?? String.Empty)
                                                           ? DisplayStyle.DefaultRegularFont
                                                           : DisplayStyle.DefaultBoldFont;
                            }

                            if (!String.IsNullOrEmpty(beforeEd.Field5OrStatus))
                            {
                                lblBeforeField5.Text = String.Format("{0}: {1}",
                                                                     AryaTools.Instance.InstanceData.CurrentProject.
                                                                         EntityField5Name, beforeEd.Field5OrStatus);
                                lblBeforeField5.Font =
                                    (beforeEd.Field5OrStatus ?? String.Empty).Equals(currentEd.Field5OrStatus
                                                                                     ?? String.Empty)
                                        ? DisplayStyle.DefaultRegularFont
                                        : DisplayStyle.DefaultBoldFont;
                            }

                            lblBeforeDate.Text = beforeEd.CreatedOn.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            lblBeforeAttribute.Visible = !String.IsNullOrEmpty(lblBeforeAttribute.Text);
            lblBeforeValue.Visible = !String.IsNullOrEmpty(lblBeforeValue.Text);
            lblCreatedRemark.Visible = !String.IsNullOrEmpty(lblCreatedRemark.Text);
            //lblDeletedRemark.Visible = !string.IsNullOrEmpty(lblDeletedRemark.Text);
            lblBeforeUom.Visible = !String.IsNullOrEmpty(lblBeforeUom.Text);
            lblBeforeField1.Visible = !String.IsNullOrEmpty(lblBeforeField1.Text);
            lblBeforeField2.Visible = !String.IsNullOrEmpty(lblBeforeField2.Text);
            lblBeforeField3.Visible = !String.IsNullOrEmpty(lblBeforeField3.Text);
            lblBeforeField4.Visible = !String.IsNullOrEmpty(lblBeforeField4.Text);
            lblBeforeField5.Visible = !String.IsNullOrEmpty(lblBeforeField5.Text);
            lblBeforeDate.Visible = !String.IsNullOrEmpty(lblBeforeDate.Text);

            lblBeforeEntity.Visible = showBeforeEntityGroup;
        }

        private void PopulateGridView()
        {
            var sw = new Stopwatch();
            sw.Start();
            var waitkey = FrmWaitScreen.ShowMessage("Preparing Data");
            _populatingGridView = true;
            var baseStatus = lblStatus.Text + ", ";

            SetStatus(baseStatus + "Refreshing Attribute Column");
            AttributeColumn.RefreshAttributeColumnPositions(PossibleAttributeColumnsForSkuView, _allAttributes, CurrentTaxonomies, false);

            SetStatus(baseStatus + "Drawing Grid View");
            //Init
            edgv.ScrollBars = ScrollBars.None;
            //edgv.Rows.Clear();
            edgv.Columns.Clear();
            //edgv.Refresh();
            edgv.Columns.Add(GetNewColumn(GetHeaderText(String.Empty, "Item ID", 0), CellTextBoxTemplate, true,
                                          DefaultColumnWidth));
            edgv.Columns.Add(GetNewColumn(GetHeaderText(String.Empty, "Taxonomy", 0), CellTextBoxTemplate, false,
                                          DefaultColumnWidth));

            //hide the attribute columns from SkuView
            var cols =
                PossibleAttributeColumnsForSkuView.Where(
                    att =>
                    att.Visible && _allAttributes.ContainsKey(att.AttributeName.ToLower())
                    && !AryaTools.Instance.InstanceData.CurrentUser.AttributeExclusions.Contains(att.Attribute.ID)
                    && !att.Attribute.AttributeName.Equals(Framework.Properties.Resources.NewAttributeName.Trim())).Select(
                        col => new { col.Attribute, col.ColumnWidth }).ToList();

            int currentCol = 0, totalCols = cols.Count();

            //if (EarlyAttributeCachingThread.IsAlive)
            //{
            //    AttributeCacheWorkerWait.Reset();
            //}

            //Rohit
            var x = cols.Select(a => a.Attribute).ToList();
            SetStatus(String.Format("Populating cache. This may take few minutes.."));
            FetchSkuAttributeValueCache(x);
            //Rohit

            cols.ForEach(col =>
                             {
                                 SetStatus(String.Format("{0}Drawing Grid View ({1:0}%)", baseStatus,
                                                         100.0 * currentCol++ / totalCols));

                                 var att = col.Attribute;
                                 var attributeName = col.Attribute.AttributeName;
                                 //FetchSkuAttributeValueCache(att); //Rohit

                                 int noOfColumns;
                                 if (_noOfColumns.ContainsKey(att))
                                     noOfColumns = _noOfColumns[att];
                                 else
                                 {
                                     //noOfColumns = 1;
                                     noOfColumns = _skus.Max(sku => sku.Value.GetValuesForAttribute(att).Count);
                                     _noOfColumns.Add(att, noOfColumns);
                                 }

                                 for (var i = 0; i == 0 || i < noOfColumns; i++)
                                 {
                                     var attributeOrders = i == 0
                                                               ? GetAttributeOrders(att.AttributeName.ToLower())
                                                               : String.Empty;
                                     var headerText = GetHeaderText(attributeOrders, attributeName, i);
                                     var newColumn = GetNewColumn(headerText, CellTextBoxTemplate, false,
                                                                  col.ColumnWidth);

                                     newColumn.HeaderCell.Style =
                                         _currentFilters.Any(
                                             f => f.LowerAttributeName.Equals(att.AttributeName.ToLower()))
                                             ? boldStyle
                                             : normalStyle;

                                     edgv.Columns.Add(newColumn);
                                 }
                             });
            //Also fetch ImageAttribute(s)

            var assetAttributes =
                AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls.Union(
                    AryaTools.Instance.InstanceData.CurrentProject.PictureBoxUrl).Where(
                        p => p != null && !String.IsNullOrEmpty(p.Value) && !String.IsNullOrEmpty(p.AssetAttributeName))
                    .Select(p => p.AssetAttributeName).ToList();

            foreach (var assetAttribute in assetAttributes)
            {
                var imageAttributeName = assetAttribute;
                var imageAttribute = Attribute.GetAttributeFromName(imageAttributeName, false);
                if (imageAttribute != null)
                    FetchSkuAttributeValueCache(imageAttribute);
            }

            SetStatus(baseStatus + "Drawing Grid View");
            edgv.Columns.Add(GetNewColumn(GetHeaderText(String.Empty, Framework.Properties.Resources.NewAttributeName, 0),
                                          CellTextBoxTemplate, false, DefaultColumnWidth));
            FetchSkuAttributeValueCache(Attribute.GetAttributeFromName(Framework.Properties.Resources.NewAttributeName, true));
            edgv.RowCount = AryaTools.Instance.Forms.SkuOrders[_instanceId].Count;
            edgv.ScrollBars = ScrollBars.Both;

            if (_taxonomyNodes == null)
                openAllNodesInSchemaViewToolStripMenuItem.Visible = false;
            else
                openAllNodesInSchemaViewToolStripMenuItem.Visible = _taxonomyNodes.Count > 1;

            edgv.Columns[0].DefaultCellStyle = DisplayStyle.CellStyleItemIDColumn;

            for (var i = 1; i < edgv.ColumnCount; i++)
                edgv.Columns[i].DefaultCellStyle = DisplayStyle.CellStyleItemEvenColumn;

            for (var i = 0; i < edgv.RowCount; i++)
            {
                if (i % 2 != 0)
                    edgv.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;
            }

            _populatingGridView = false;
            GetSelectedCells();
            PopulateProperties();

            FrmWaitScreen.HideMessage(waitkey);
            sw.Stop();
            sw.Reset();
            Diagnostics.WriteMessage(
                "*Populating GridView" + (_loadQuery.ToString().Length < 600 ? String.Empty : " - Query"),
                "EntityDataGridView - PopulateGridView", sw.Elapsed, LoadQueryResults.Count, _taxonomyNodes.Count,
                cols.Count);
        }

        //Created for testing
        private void PopulateGridView(List<AttributeColumn> attrColumns)
        {
            var sw = new Stopwatch();
            sw.Start();

            //Init
            edgv.ScrollBars = ScrollBars.None;
            //edgv.Rows.Clear();
            edgv.Columns.Clear();
            //edgv.Refresh();
            edgv.Columns.Add(GetNewColumn(GetHeaderText(String.Empty, "Item ID", 0), CellTextBoxTemplate, true,
                                          DefaultColumnWidth));
            edgv.Columns.Add(GetNewColumn(GetHeaderText(String.Empty, "Taxonomy", 0), CellTextBoxTemplate, false,
                                          DefaultColumnWidth));

            var waitkey = FrmWaitScreen.ShowMessage("Preparing Data");
            _populatingGridView = true;
            var baseStatus = lblStatus.Text + ", ";

            SetStatus(baseStatus + "Refreshing Attribute Column");
            var totalCols = 0;
            if (PossibleAttributeColumnsForSkuView != null)
            {
                SetStatus(baseStatus + "Drawing Grid View");
                //hide the attribute columns from SkuView
                var cols =
                    PossibleAttributeColumnsForSkuView.Where(
                        att =>
                        att.Visible && _allAttributes.ContainsKey(att.AttributeName.ToLower())
                        &&
                        !AryaTools.Instance.InstanceData.CurrentUser.AttributeExclusions.Contains(att.Attribute.ID)
                        && !att.Attribute.AttributeName.Equals(Framework.Properties.Resources.NewAttributeName.Trim())).Select(
                            col => new { col.Attribute, col.ColumnWidth }).ToList();

                var currentCol = 0;
                totalCols = cols.Count();

                //if (EarlyAttributeCachingThread.IsAlive)
                //{
                //    AttributeCacheWorkerWait.Reset();
                //}

                //Rohit
                var x = cols.Select(a => a.Attribute).ToList();
                SetStatus(string.Format("Populating cache. This may take few minutes.."));
                FetchSkuAttributeValueCache(x);
                //Rohit

                cols.ForEach(col =>
                                 {
                                     SetStatus(string.Format("{0}Drawing Grid View ({1:0}%)", baseStatus,
                                                             100.0 * currentCol++ / totalCols));

                                     var att = col.Attribute;
                                     var attributeName = col.Attribute.AttributeName;
                                     //FetchSkuAttributeValueCache(att); //Rohit

                                     int noOfColumns;
                                     if (_noOfColumns.ContainsKey(att))
                                         noOfColumns = _noOfColumns[att];
                                     else
                                     {
                                         //noOfColumns = 1;
                                         noOfColumns = _skus.Max(sku => sku.Value.GetValuesForAttribute(att).Count);
                                         _noOfColumns.Add(att, noOfColumns);
                                     }

                                     for (var i = 0; i == 0 || i < noOfColumns; i++)
                                     {
                                         var attributeOrders = i == 0
                                                                   ? GetAttributeOrders(att.AttributeName.ToLower())
                                                                   : string.Empty;
                                         var headerText = GetHeaderText(attributeOrders, attributeName, i);
                                         var newColumn = GetNewColumn(headerText, CellTextBoxTemplate, false,
                                                                      col.ColumnWidth);

                                         newColumn.HeaderCell.Style =
                                             _currentFilters.Any(
                                                 f => f.LowerAttributeName.Equals(att.AttributeName.ToLower()))
                                                 ? boldStyle
                                                 : normalStyle;

                                         edgv.Columns.Add(newColumn);
                                     }
                                 });
            }

            //Also fetch ImageAttribute(s)
            var assetAttributes =
                AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls.Union(
                    AryaTools.Instance.InstanceData.CurrentProject.PictureBoxUrl).Where(
                        p => p != null && !String.IsNullOrEmpty(p.Value) && !String.IsNullOrEmpty(p.AssetAttributeName))
                    .Select(p => p.AssetAttributeName).ToList();

            foreach (var assetAttribute in assetAttributes)
            {
                var imageAttributeName = assetAttribute;
                var imageAttribute = Attribute.GetAttributeFromName(imageAttributeName, false);
                if (imageAttribute != null)
                    FetchSkuAttributeValueCache(imageAttribute);
            }

            SetStatus(baseStatus + "Drawing Grid View");
            edgv.Columns.Add(GetNewColumn(GetHeaderText(String.Empty, Framework.Properties.Resources.NewAttributeName, 0),
                                          CellTextBoxTemplate, false, DefaultColumnWidth));
            FetchSkuAttributeValueCache(Attribute.GetAttributeFromName(Framework.Properties.Resources.NewAttributeName, true));
            edgv.RowCount = AryaTools.Instance.Forms.SkuOrders[_instanceId].Count;
            edgv.ScrollBars = ScrollBars.Both;

            if (_taxonomyNodes == null)
                openAllNodesInSchemaViewToolStripMenuItem.Visible = false;
            else
                openAllNodesInSchemaViewToolStripMenuItem.Visible = _taxonomyNodes.Count > 1;

            edgv.Columns[0].DefaultCellStyle = DisplayStyle.CellStyleItemIDColumn;

            for (var i = 1; i < edgv.ColumnCount; i++)
                edgv.Columns[i].DefaultCellStyle = DisplayStyle.CellStyleItemEvenColumn;

            for (var i = 0; i < edgv.RowCount; i++)
            {
                if (i % 2 != 0)
                    edgv.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;
            }

            _populatingGridView = false;
            GetSelectedCells();
            PopulateProperties();

            FrmWaitScreen.HideMessage(waitkey);
            sw.Stop();
            sw.Reset();
            Diagnostics.WriteMessage(
                "*Populating GridView" + (_loadQuery.ToString().Length < 600 ? String.Empty : " - Query"),
                "EntityDataGridView - PopulateGridView", sw.Elapsed, LoadQueryResults.Count,
                _taxonomyNodes != null ? _taxonomyNodes.Count : 0, totalCols);
        }

        private void PopulateLinks()
        {
            if (ShowLinks == false)
            {
                //AryaTools.Instance.SkuLinksView.Close();
                return;
            }
            if (edgv.SelectedCells.Count != 1)
                return;
            AryaTools.Instance.Forms.SkuLinksViewForm.SelectedSku =
                (Sku)edgv.SelectedCells[0].OwningRow.Cells[0].Value;
        }

        private void PopulateProperties()
        {
            if (CurrentChange == null)
            {
                tblEntity.Enabled = false;
                return;
            }
            _populatingProperties = true;
            Attribute attribute = null;
            string value = null;
            string uom = null;
            string field1 = null;
            string field2 = null;
            string field3 = null;
            string field4 = null;
            string field5 = null;
            var uniqueAttribute = true;
            var uniqueValue = true;
            var uniqueUom = true;
            var uniqueField1 = true;
            var uniqueField2 = true;
            var uniqueField3 = true;
            var uniqueField4 = true;
            var uniqueField5 = true;
            var readonlyAttribute = false;
            //Split header and get attribute orders
            string attributeName;
            int attributeIteration;
            decimal navOrder = 0, dispOrder = 0;
            bool inSchema;

            SplitHeaderText(edgv.Columns[edgv.CurrentCellAddress.X].HeaderText, out attributeName,
                            out attributeIteration);
            if (edgv.CurrentCellAddress.X > 1)
                SchemaAttribute.GetAttributeOrders(Attribute.GetAttributeFromName(attributeName, true), CurrentTaxonomy,
                                                   out navOrder, out dispOrder, out inSchema);

            var lowerFieldName = attributeName.ToLower();
            if (_allAttributes.ContainsKey(lowerFieldName))
                VerifyUnique(ref uniqueAttribute, ref attribute, _allAttributes[lowerFieldName]);
            var entityDatas = CurrentChange.GetEntityDatas();
            var skus = CurrentChange.GetSkus();

            var uoms = new HashSet<string>();
            foreach (var data in entityDatas)
            {
                if (data.Uom != null)
                    uoms.Add(data.Uom);

                VerifyUnique(ref uniqueAttribute, ref attribute, data.Attribute);
                VerifyUnique(ref uniqueValue, ref value, data.Value);
                VerifyUnique(ref uniqueUom, ref uom, data.Uom);
                VerifyUnique(ref uniqueField1, ref field1, data.Field1);
                VerifyUnique(ref uniqueField2, ref field2, data.Field2);
                VerifyUnique(ref uniqueField3, ref field3, data.Field3);
                VerifyUnique(ref uniqueField4, ref field4, data.Field4);
                VerifyUnique(ref uniqueField5, ref field5, data.Field5OrStatus);
            }
            //First uncheck the Create Entity checkbox
            //chkCreateNewEntityDatas.Checked = false;
            CreateNewEntityData = false;
            if (uniqueAttribute && attribute != null)
            {
                cboAttributeName.Text = attribute.AttributeName;
                CurrentChange.OldValues.Att = attribute;

                if (attribute.Readonly)
                    readonlyAttribute = true;
            }
            else
                cboAttributeName.Text = String.Empty;

            if (uniqueValue && !String.IsNullOrEmpty(value))
            {
                txtValue.Text = value;
                CurrentChange.OldValues.Val = value;
            }
            else
                txtValue.Text = String.Empty;

            txtUom.Items.Clear();
            txtUom.Items.Add("<delete>");
            txtUom.Items.AddRange(uoms.OrderBy(u => u).ToArray());
            if (uniqueUom && !String.IsNullOrEmpty(uom))
            {
                txtUom.Text = uom;
                CurrentChange.OldValues.Uom = uom;
            }
            else
                txtUom.Text = String.Empty;

            if (uniqueField1 && !String.IsNullOrEmpty(field1))
            {
                txtField1.Text = field1;
                CurrentChange.OldValues.Field1 = field1;
            }
            else
                txtField1.Text = String.Empty;
            if (uniqueField2 && !String.IsNullOrEmpty(field2))
            {
                txtField2.Text = field2;
                CurrentChange.OldValues.Field2 = field2;
            }
            else
                txtField2.Text = String.Empty;
            if (uniqueField3 && !String.IsNullOrEmpty(field3))
            {
                txtField3.Text = field3;
                CurrentChange.OldValues.Field3 = field3;
            }
            else
                txtField3.Text = String.Empty;
            if (uniqueField4 && !String.IsNullOrEmpty(field4))
            {
                txtField4.Text = field4;
                CurrentChange.OldValues.Field4 = field4;
            }
            else
                txtField4.Text = String.Empty;
            if (uniqueField5 && !String.IsNullOrEmpty(field5))
            {
                txtField5.Text = field5;
                CurrentChange.OldValues.Field5 = field5;
            }
            else
                txtField5.Text = String.Empty;

            tblEntity.Enabled = entityDatas.Any() || (CurrentChange.GetBlanks().Any() && CreateNewEntityData);
            moveSKUToolStripMenuItem.Enabled = btnChangeTaxonomy.Enabled = skus.Any();
            chkSuggestValues.Checked = true;
            //enable combo boxes if not readonly
            cboAttributeName.Enabled = !readonlyAttribute;
            txtValue.Enabled = !readonlyAttribute;
            txtUom.Enabled = !readonlyAttribute;
            //show/hide delete and createEntity buttons
            _allowCreateEntities = (CurrentChange.GetBlanks().Count > 0 && !readonlyAttribute)
                                   ||
                                   (attribute != null && attribute.AttributeType == AttributeTypeEnum.Derived.ToString());
            _allowDeleteEntities = CurrentChange.GetEntityDatas().Count > 0 && !readonlyAttribute;
            newEntitiesToolStripMenuItem.Enabled = _allowCreateEntities;
            deleteSelectedEntitiesToolStripMenuItem.Enabled = _allowDeleteEntities;
            //show/hide calculated Function button

            var cellAttribute = Attribute.GetAttributeFromName(attributeName, false);
            calculatedAttributeFunctionToolStripMenuItem.Enabled = CurrentTaxonomy != null && cellAttribute != null
                                                                   &&
                                                                   cellAttribute.AttributeType
                                                                   == AttributeTypeEnum.Derived.ToString();
            //enable disable ranks table
            if (edgv.CurrentCell.ColumnIndex < 2 || (edgv.CurrentCell.ColumnIndex + 1) == edgv.ColumnCount)
                tblAttributeOrders.Enabled = false;
            else
                tblAttributeOrders.Enabled = true;
            txtNavigationOrder.Text = navOrder == 0 ? String.Empty : navOrder.ToString("#.###");
            txtDisplayOrder.Text = dispOrder == 0 ? String.Empty : dispOrder.ToString("#.###");
            PopulateBeforeEntity();

            if (ShowItemImagesInBrowser)
                skuImageBox.Visible = false;
            else
                PopulateSkuImageBox();
            //txtCurrentValue.Text = edgv.SelectedCells.Count > 0
            //? (edgv.SelectedCells[0].Value ?? string.Empty).ToString()
            //: string.Empty;

            if (edgv.SelectedCells.Count == 1 && edgv.SelectedCells[0].ColumnIndex > 1)
            {
                var ed = edgv.SelectedCells[0].Value as EntityData;

                if (ed != null && ed.Attribute != null
                    && ed.Attribute.AttributeType == AttributeTypeEnum.Derived.ToString())
                    lblDerivedAttributeText.Text = @"Manually Overwritten Derived Attribute Value";
                else
                    lblDerivedAttributeText.Text = String.Empty;
            }
            else
                lblDerivedAttributeText.Text = String.Empty;

            PopulateAttributeDefinition();

            _populatingProperties = false;
        }

        private void PopulateSkuImageBox()
        {
            var pictureBoxUrl = AryaTools.Instance.InstanceData.CurrentProject.PictureBoxUrl;

            if (pictureBoxUrl == null)
                return;

            string filename = null;

            var currentSku = edgv.CurrentCell.OwningRow.Cells[0].Value as Sku;

            var assetInfo = imageCache.GetAsset(currentSku, pictureBoxUrl.AssetAttributeName, pictureBoxUrl.Value,
                                                pictureBoxUrl.Type);

            if (assetInfo != null)
            {
                filename = assetInfo.AssetLocation;

                if (!String.IsNullOrEmpty(filename))
                {
                    var success = false;
                    while (!success)
                    {
                        try
                        {
                            skuImageBox.Load(filename);
                            success = true;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (Exception)
                        {
                            success = true;
                        }
                    }
                    PlaceSkuImageBox();
                }
            }
            skuImageBox.Visible = !String.IsNullOrEmpty(filename);
        }

        private void PrepareData(object o)
        {
            var sw = new Stopwatch();
            //bool loadSkuViewCurrentTab = true;
            sw.Start();
            var myLoadId = Guid.NewGuid();
            AryaTools.Instance.Forms.SkuForm.LoadQueue.Enqueue(myLoadId);
            _workerStatus = "Waiting in line.";
            while (AryaTools.Instance.Forms.SkuForm.LoadQueue.Peek() != myLoadId)
            {
                if (AbortWorker)
                    return;
                Thread.Sleep(100);
            }

            //loadSkuViewCurrentTab = LoadSkusAndAttributes();
            if (!LoadSkusAndAttributes())
            {
                _workerState = WorkerState.Abort;
                return;
            }


            _workerStatus = string.Format("{0} SKUs, {1} Attributes, Fetching column positions",
                                          AryaTools.Instance.Forms.SkuOrders[_instanceId].Count, _allAttributes.Count);
            if (PossibleAttributeColumnsForSkuView == null)
                PossibleAttributeColumnsForSkuView = new List<AttributeColumn>();
            _workerStatus = String.Format("{0} SKUs, {1} Attributes, Refreshing attribute column orders",
                                          AryaTools.Instance.Forms.SkuOrders[_instanceId].Count, _allAttributes.Count);
            AttributeColumn.RefreshAttributeColumnPositions(PossibleAttributeColumnsForSkuView, _allAttributes, CurrentTaxonomies, true);
            if (AbortWorker)
                return;
            AryaTools.Instance.Forms.SkuForm.LoadQueue.Dequeue();
            _workerStatus = String.Format("{0} SKUs, {1} Attributes, Load Complete. Waiting for others to finish",
                                          AryaTools.Instance.Forms.SkuOrders[_instanceId].Count, _allAttributes.Count);
            while (AryaTools.Instance.Forms.SkuForm.LoadQueue.Count > 0)
            {
                if (AbortWorker)
                    return;
                Thread.Sleep(500);
            }

            _workerStatus = String.Format("{0} SKUs, {1} Attributes",
                                          AryaTools.Instance.Forms.SkuOrders[_instanceId].Count, _allAttributes.Count);
            _workerState = WorkerState.Ready;
            sw.Stop();
            //  var x = _loadQuery.ToString().Length;
            Diagnostics.WriteMessage(
                "*Fetching Data for SkuView" + (_loadQuery.ToString().Length > 600 ? "- from Query" : String.Empty),
                "EntityDataGridView - PrepareData", sw.Elapsed,
                NoOfSkus: AryaTools.Instance.Forms.SkuOrders[_instanceId].Count, NoOfAttributes: _allAttributes.Count,
                NoOfNodes: _taxonomyNodes.Count);
        }

        private void Redo() { GroupUndo(_redoHistory, _undoHistory); }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) { Redo(); }

        private void RedrawDataGridView(object selectEntitydataOrSku)
        {
            var scrollOffset = edgv.HorizontalScrollingOffset;
            var currentRow = edgv.CurrentCellAddress.Y;
            var currentCol = edgv.CurrentCellAddress.X;
            PopulateGridView(PossibleAttributeColumnsForSkuView);
            edgv.HorizontalScrollingOffset = scrollOffset;
            if (selectEntitydataOrSku != null && selectEntitydataOrSku.GetType() == typeof(EntityData))
                SelectCell(edgv, (EntityData)selectEntitydataOrSku);
            else if (selectEntitydataOrSku != null && selectEntitydataOrSku.GetType() == typeof(Sku))
                SelectCell(edgv, (Sku)selectEntitydataOrSku);
            else if (edgv.ColumnCount > 0 && edgv.RowCount > 0)
            {
                if (currentRow >= edgv.RowCount || currentRow < 0)
                    currentRow = 0;
                if (currentCol >= edgv.ColumnCount || currentCol < 0)
                    currentCol = 0;
                if (currentCol >= 0 && currentRow >= 0)
                    edgv.CurrentCell = edgv[currentCol, currentRow];
            }
            PopulateProperties();
        }

        private void RefreshCurrentTab()
        {
            _loadQueryResult = null;
            //DialogResult changesSaved = AryaTools.Instance.SaveChangesIfNecessary(true, false);
            var changesSaved = SkuViewSaveChangesIfNecessary(true, false);
            if (changesSaved == DialogResult.None || changesSaved == DialogResult.Yes)
            {
                InitData();
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e) { RefreshCurrentTab(); }

        private void reorderColumnsToolStripMenuItem_Click(object sender, EventArgs e) { ReorderColumns(); }

        private void repopulateGridViewTimer_Tick(object sender, EventArgs e)
        {
            ReorderColumns();
            repopulateGridViewTimer.Stop();
        }

        private void saveCurrentFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogXml.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                var serializer = new XmlSerializer(_currentFilters.GetType());
                TextWriter file = new StreamWriter(saveFileDialogXml.FileName);
                serializer.Serialize(file, _currentFilters);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save: " + ex.Message);
            }
        }

        private void SaveCurrentGridToFile(string filename)
        {
            var saveFileExtension = new FileInfo(filename).Extension.ToLower();
            if (!saveFileExtension.Equals(".txt") && !saveFileExtension.Equals(".xml"))
            {
                MessageBox.Show(String.Format("Unknown Export File Format: {0}", saveFileExtension));
                return;
            }
            SetStatus(String.Format("Saving grid to {0}", filename), WorkerState.Working);
            var waitkey = FrmWaitScreen.ShowMessage("Export - Init");
            string tableName = null;
            if (CurrentTaxonomy != null)
                tableName = CurrentTaxonomy.ToString().Trim();
            if (String.IsNullOrEmpty(tableName))
                tableName = "Arya Export";
            ////Scan through the entire grid - to make sure any multiple iterations are inserted into the grid
            //FrmWaitScreen.UpdateMessage(waitkey, "Reading all values");
            //object thisValue;
            //for (int x = 0; x < edgv.Rows.Count; x++)
            //    for (int y = 0; y < edgv.ColumnCount - 1; y++)
            //        thisValue = edgv[y, x].Value;
            FrmWaitScreen.UpdateMessage(waitkey, "Preparing export dataset");
            var dt = new DataTable(tableName);
            var cols = edgv.ColumnCount - 1;
            for (var index = 0; index < cols; index++)
                dt.Columns.Add(edgv.Columns[index].HeaderText);
            for (var x = 0; x < edgv.RowCount; x++)
            {
                var rowData = new string[cols];
                for (var y = 0; y < cols; y++)
                {
                    var value = edgv[y, x].Value;
                    rowData[y] = value == null ? String.Empty : value.ToString();
                }

                dt.Rows.Add(rowData);
            }
            FrmWaitScreen.UpdateMessage(waitkey, "Writing to file");
            if (saveFileExtension.Equals(".txt"))
            {
                using (TextWriter tw = new StreamWriter(filename))
                {
                    var line = String.Empty;

                    // Write Header
                    for (var i = 0; i < dt.Columns.Count; i++)
                        line += (i > 0 ? "\t" : String.Empty) + dt.Columns[i].ColumnName.Replace("\n", "__");
                    tw.WriteLine(line);

                    // Write all rows
                    foreach (DataRow row in dt.Rows)
                    {
                        line = String.Empty;

                        for (var i = 0; i < dt.Columns.Count; i++)
                            line += (i > 0 ? "\t" : String.Empty) + row[i].ToString().Replace("\n", "__");

                        tw.WriteLine(line);
                    }

                    tw.Close();
                }
            }
            else if (saveFileExtension.Equals(".xml"))
                dt.WriteXml(filename);
            FrmWaitScreen.HideMessage(waitkey);
            SetStatus("Saved", WorkerState.Ready);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AryaTools.Instance.SaveChangesIfNecessary(false, true);
            SkuViewSaveChangesIfNecessary(false, true);
        }

        private void SelectAll(Attribute attribute)
        {
            var oldSelectingManyCells = _selectingManyCells;
            _selectingManyCells = true;

            var columnIndexes = edgv.Columns.Cast<DataGridViewColumn>().Where(col =>
                                                                                  {
                                                                                      string attrName;
                                                                                      int iteration;
                                                                                      SplitHeaderText(col.HeaderText,
                                                                                                      out attrName,
                                                                                                      out iteration);
                                                                                      return
                                                                                          attrName.Equals(
                                                                                              attribute.AttributeName);
                                                                                  }).Select(col => col.Index).ToList();
            //if (_skus.Count > SkuManagerTools._skuOrders[_instanceId].Count)
            //    MessageBox.Show(
            //        "One or more filters have been applied. This operation will only be applied to the currently visible SKUs.");
            if (columnIndexes.Count > 0)
            {
                DeselectCurrentSelection();
                var currentRowIndex = edgv.CurrentCellAddress.Y;
                var rowIndex = currentRowIndex >= 0 && currentRowIndex < edgv.RowCount ? currentRowIndex : 0;
                edgv.CurrentCell = edgv[columnIndexes[0], rowIndex];
                foreach (var column in columnIndexes)
                    SelectAll(column, false);
            }

            _selectingManyCells = oldSelectingManyCells;
            SelectionChanged();
        }

        private void SelectAll(int column, bool deselectCurrentSelection)
        {
            var oldSelectingManyCells = _selectingManyCells;
            _selectingManyCells = true;

            //First, deselect all selected cells
            if (deselectCurrentSelection)
            {
                DeselectCurrentSelection();
                edgv.CurrentCell = edgv[column, edgv.CurrentCellAddress.Y];
            }
            foreach (DataGridViewRow row in edgv.Rows)
                edgv.Rows[row.Index].Cells[column].Selected = true;

            _selectingManyCells = oldSelectingManyCells;
            SelectionChanged();
        }

        private void SelectAttributeFromColumn(int columnIndex)
        {
            if (edgv.Rows.Count == 0)
                return;

            int iteration;
            string fieldName;
            var headerText = edgv.Columns[columnIndex].HeaderText;
            SplitHeaderText(headerText, out fieldName, out iteration);
            if (_allAttributes.ContainsKey(fieldName.ToLower()))
                SelectAll(_allAttributes[fieldName.ToLower()]);
            else
                SelectAll(columnIndex, true);
        }

        private static void SelectCell(DataGridView dgv, EntityData entityData)
        {
            try
            {
                var attributeName = entityData.Attribute.AttributeName;
                var firstColumn = dgv.Columns.Cast<DataGridViewColumn>().Where(col =>
                                                                                   {
                                                                                       string attrName;
                                                                                       int iteration;
                                                                                       SplitHeaderText(col.HeaderText,
                                                                                                       out attrName,
                                                                                                       out iteration);
                                                                                       return
                                                                                           attrName.Equals(attributeName);
                                                                                   }).FirstOrDefault();

                var columnIndex = firstColumn == null ? 0 : firstColumn.Index;
                SelectCell(dgv, entityData.EntityInfo.Sku, columnIndex);
            }
            catch //If we can't do this, there's nothing to loose...
            {
            }
        }

        private static void SelectCell(DataGridView dgv, Sku sku, int columnIndex = 0)
        {
            try
            {
                var dgvRow =
                    dgv.Rows.Cast<DataGridViewRow>().Where(row => row.Cells[0].Value.Equals(sku)).FirstOrDefault();
                var rowIndex = dgvRow != null ? dgvRow.Index : 0;

                //First, deselect all selected cells
                dgv.SelectedCells.Cast<DataGridViewCell>().ForEach(cell => cell.Selected = false);

                //If this cell is hidden, select its itemId column
                if (!dgv.Rows[rowIndex].Cells[columnIndex].Visible)
                    columnIndex = 0;

                //Select this cell
                dgv.Rows[rowIndex].Cells[columnIndex].Selected = true;
                dgv.CurrentCell = dgv.Rows[rowIndex].Cells[columnIndex];
            }
            catch
            {
            } //If we can't do this, there's nothing to loose...
        }

        internal void SelectionChanged()
        {
            if (_selectingManyCells)
                return;
            selectionChangedTimer.Stop();

            try
            {
                _snippetCurrentCellValue = edgv.CurrentCell.Value.ToString();
                _snippetValueChanged = true;
                if (edgv.CurrentCellAddress.X == 0 || _snippetCurrentCellValue.StartsWith("http://"))
                    _snippetDocumentLastUpdated = DateTime.Now;
            }
            catch (Exception)
            {
                _snippetCurrentCellValue = String.Empty;
            }
            ValueToolTip();
            SaveCurrentChangeAndGetNewChange(false);
            if (edgv.CurrentCellAddress.X == -1 || edgv.CurrentCellAddress.Y == -1)
                return;
            while (edgv.SelectedColumns.Count > 0)
                edgv.SelectedColumns[0].Selected = false;
            var columnIndexes =
                edgv.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.ColumnIndex).Distinct().ToList();
            if (columnIndexes.Count() > 1)
            {
                var multipleSelected = false;
                var headers =
                    edgv.Columns.Cast<DataGridViewColumn>().Where(col => columnIndexes.Contains(col.Index)).Select(
                        col => col.HeaderText);

                string previousFieldName = null;
                foreach (var headerText in headers)
                {
                    int iteration;
                    string attributeName;
                    SplitHeaderText(headerText, out attributeName, out iteration);
                    if (previousFieldName == null)
                        previousFieldName = attributeName;
                    else if (!previousFieldName.Equals(attributeName))
                        multipleSelected = true;
                }

                if (multipleSelected)
                {
                    var iFirstColumn = edgv.CurrentCell.ColumnIndex;
                    edgv.SelectedCells.Cast<DataGridViewCell>().ForEach(cell =>
                                                                            {
                                                                                if (cell.ColumnIndex != iFirstColumn)
                                                                                    cell.Selected = false;
                                                                            });
                }
            }

            PopulateAuditTrail();
            PopulateLinks();
        }

        private bool SetCurrentChangeNewAttribute(string newAttributeName)
        {
            if (_populatingProperties || CurrentChange == null
                ||
                (CurrentChange.OldValues.Att != null
                 && CurrentChange.OldValues.Att.AttributeName.Equals(newAttributeName)))
                return false;

            if (!String.IsNullOrWhiteSpace(newAttributeName))
            {
                CurrentChange.NewValues.AttributeName = newAttributeName;
                return true;
            }

            return false;
        }

        private void SetEntityField(string fieldName, Control fieldLabel, Control fieldComboBox, bool isReadonly)
        {
            if (String.IsNullOrEmpty(fieldName))
            {
                fieldLabel.Visible = false;
                fieldComboBox.Visible = false;
            }
            else
            {
                fieldLabel.Text = fieldName + @":";
                sortByComboToolStripMenuItem.Items.Add(SortFieldPrefix + fieldName);
                filterByComboToolStripMenuItem.Items.Add(FilterFieldPrefix + fieldName);
            }
            fieldComboBox.Enabled = !isReadonly;
        }

        private void SetStatus(string status)
        {
            if (status.Equals(lblStatus.Text))
                return;
            lblStatus.Text = status;
            statusStrip1.Refresh();
            //AryaTools.Instance.Forms.SkuForm.SetStatus(this, AryaTools.StatusType.Default, status);
        }

        //private void showAllNonglobalattributesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    var visibility = true;
        //    var nonGlobalColumns =
        //        _allAttributeColumns.Where(col => col.Attribute.Type == AttributeTypeEnum.Sku).ToList();

        //    if (nonGlobalColumns.Count(col => col.Visible) == nonGlobalColumns.Count())
        //        visibility = false;

        //    nonGlobalColumns.ForEach(col => col.Visible = visibility);
        //    RedrawDataGridView(null);
        //    UpdateFilterStatus();
        //}

        private void showHideAttributesToolStripMenuItem_Click(object sender, EventArgs e) { AttributeOrderVisibility(); }

        private void showHideSKULinksToolStripMenuItem_Click(object sender, EventArgs e) { ShowLinks = showHideSKULinksToolStripMenuItem.Checked; }

        private void showItemImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowItemImagesInBrowser = showItemImagesToolStripMenuItem.Checked;
            _snippetValueChanged = true;
        }

        private void showItemOnClientWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowItemOnClientWebsite = showItemOnClientWebsiteToolStripMenuItem.Checked;
            _snippetValueChanged = true;
        }

        private void showResearchPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowResearchPages = showResearchPagesToolStripMenuItem.Checked;
            _snippetValueChanged = true;
        }

        private void showURLsFromDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUrlPages = showURLsFromDataToolStripMenuItem.Checked;
            _snippetValueChanged = true;
        }

        private void skuImageBox_Click(object sender, EventArgs e)
        {
            skuImageBox.Width = skuImageBox.Height = skuImageBox.Height == 100 ? skuImageBox.Height = 300 : 100;
            PlaceSkuImageBox();
        }

        private void skuImageBox_DoubleClick(object sender, EventArgs e) { AryaTools.Instance.Forms.BrowserForm.GotoUrl(skuImageBox.ImageLocation); }

        private DialogResult SkuViewSaveChangesIfNecessary(bool mustSaveToContinue, bool userInitiated)
        {
            //AttributeCacheWorkerWait.Reset();
            var dialogResult = AryaTools.Instance.SaveChangesIfNecessary(mustSaveToContinue, userInitiated);
            //AttributeCacheWorkerWait.Set();
            return dialogResult;
        }

        private void sliceSelectedEntitysInPlaceToolStripMenuItem_Click(object sender, EventArgs e) { DoSlice(true); }

        private void sliceSelectedEntitysIntoNewAttributesToolStripMenuItem_Click(object sender, EventArgs e) { DoSlice(false); }

        private void snippetTimer_Tick(object sender, EventArgs e)
        {
            if (_snippetDocumentLastUpdated < _snippetBrowserLastUpdated)
                return;
            _snippetBrowserLastUpdated = DateTime.Now;
            if (ShowItemImagesInBrowser)
            {
                if (CurrentChange == null || assetTemplate == null)
                    return;

                var fileName = assetTemplate.Render(CurrentChange, _instanceId, imageCache,
                                                    edgv.SelectedCells.Count != 0
                                                        ? edgv.SelectedCells[0].ColumnIndex
                                                        : 0);

                AryaTools.Instance.Forms.BrowserForm.GotoUrl(fileName, this, null, false);
                return;
            }

            if (_snippetCurrentCellValue.StartsWith("http://"))
            {
                if (ShowUrlPages)
                    AryaTools.Instance.Forms.BrowserForm.GotoUrl(_snippetCurrentCellValue);
                return;
            }
            if (ShowItemOnClientWebsite && edgv.CurrentRow != null)
            {
                var skuCell = edgv.CurrentRow.Cells[0].Value;
                var cell = skuCell as Sku;
                if (cell != null)
                {
                    var parts = (cell).Project.ProductSearchString.Split('[', ']');

                    var searchAttribute = parts[1];

                    string searchString;
                    if (searchAttribute == "ItemID")
                        searchString = skuCell.ToString();
                    else
                    {
                        Attribute att;
                        GetAttributeFromName(searchAttribute, out att);
                        searchString = att == null
                                           ? String.Empty
                                           : (cell).GetValuesForAttribute(att.AttributeName, true, false).
                                                 SingleOrDefault().Value;
                    }

                    parts[1] = searchString;
                    var searchUrl = String.Join(String.Empty, parts);
                    AryaTools.Instance.Forms.BrowserForm.GotoUrl(searchUrl);
                }
            }
            if (ShowResearchPages)
            {
                var documentText = String.IsNullOrEmpty(_snippetDocumentText)
                                       ? (String.IsNullOrEmpty(_snippetCurrentCellValue)
                                              ? String.Empty
                                              : ApplyFont(
                                                  String.Format(
                                                      "<p><b>{0}</b></p> <p>Search:</p> <ul> <li> <a href=\"{1}\">Wikipedia</a> </li> <li> <a href=\"{2}\">Google</a> </li> </ul>",
                                                      _snippetCurrentCellValue, GetWikiUrl(_snippetCurrentCellValue),
                                                      GetGoogleUrl(_snippetCurrentCellValue))))
                                       : String.Format("{0} <a href=\"{1}\"> [more] </a>", _snippetDocumentText,
                                                       _snippetUrl);
                AryaTools.Instance.Forms.BrowserForm.SetDocumentText(documentText);
            }
        }

        private void SnippetWorker()
        {
            var webClient = new WebClient();
            while (!AbortWorker)
            {
                Thread.Sleep(200);

                var newValue = _snippetCurrentCellValue;
                if (!_snippetValueChanged)
                    continue;

                _snippetValueChanged = false;

                if (ShowItemImagesInBrowser)
                {
                    _snippetDocumentText = String.Empty;
                    _snippetDocumentLastUpdated = DateTime.Now;
                    continue;
                }

                if (!ShowResearchPages)
                    continue;

                if (String.IsNullOrEmpty(newValue))
                {
                    _snippetDocumentText = String.Empty;
                    _snippetDocumentLastUpdated = DateTime.Now;
                    continue;
                }

                if (!rxValidSearchString.IsMatch(newValue))
                {
                    _snippetDocumentText = String.Empty;
                    _snippetDocumentLastUpdated = DateTime.Now;
                    continue;
                }

                if (AryaTools.Instance.Forms.SkuForm.SnippetCache.ContainsKey(newValue))
                {
                    _snippetUrl = AryaTools.Instance.Forms.SkuForm.SnippetCache[newValue].Key;
                    _snippetDocumentText = AryaTools.Instance.Forms.SkuForm.SnippetCache[newValue].Value;
                    _snippetDocumentLastUpdated = DateTime.Now;
                    continue;
                }

                var url = new Uri(GetWikiUrl(newValue));
                var snippet = GetWikiPageText(webClient, url);
                if (String.IsNullOrEmpty(snippet))
                    _snippetDocumentText = String.Empty;
                else
                {
                    _snippetUrl = url;
                    _snippetDocumentText = snippet;
                    AryaTools.Instance.Forms.SkuForm.SnippetCache.Add(newValue,
                                                                         new KeyValuePair<Uri, string>(_snippetUrl,
                                                                                                       _snippetDocumentText));
                }
                _snippetDocumentLastUpdated = DateTime.Now;
            }
        }

        private void SortAscending(bool forceSortValue)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Sorting");
            SortGrid(edgv.CurrentCellAddress.X, SortOrder.Ascending,
                     forceSortValue ? SortValue : sortByComboToolStripMenuItem.Text);
            SaveCurrentChangeAndGetNewChange(false);
            edgv.Refresh();
            edgv.Focus();
            FrmWaitScreen.HideMessage(waitkey);
        }

        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e) { SortAscending(false); }

        private void SortDescending(bool forceSortValue)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Sorting");
            SortGrid(edgv.CurrentCellAddress.X, SortOrder.Descending,
                     forceSortValue ? SortValue : sortByComboToolStripMenuItem.Text);
            SaveCurrentChangeAndGetNewChange(false);
            edgv.Refresh();
            edgv.Focus();
            FrmWaitScreen.HideMessage(waitkey);
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e) { SortDescending(false); }

        private void SortGrid(int column, SortOrder sortOrder, string sortField)
        {
            _populatingGridView = true;
            var waitkey = FrmWaitScreen.ShowMessage("Sorting");
            var skuOrder = AryaTools.Instance.Forms.SkuOrders[_instanceId];
            var datasWithCurrentRow = (from row in edgv.Rows.Cast<DataGridViewRow>()
                                       let cell = row.Cells[column]
                                       let data = GetSortFilterValue(cell.Value, sortField)
                                       let id = ((Sku)row.Cells[0].Value).ID
                                       let currentRow = skuOrder.FindIndex(val => val.Equals(id))
                                       select new { id, data, currentRow }).ToList()
                                       .AsParallel().WithDegreeOfParallelism(4);
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    datasWithCurrentRow = numericSortToolStripMenuItem.Checked
                                              ? datasWithCurrentRow.OrderBy(val => val.data.Length == 0 ? 1 : 0).ThenBy(
                                                  val => val.data, new CompareForAlphaNumericSort()).ThenBy(
                                                      val => val.currentRow)
                                              : datasWithCurrentRow.OrderBy(val => val.data.Length == 0 ? 1 : 0).ThenBy(
                                                  val => val.data).ThenBy(val => val.currentRow);
                    break;
                case SortOrder.Descending:
                    datasWithCurrentRow = numericSortToolStripMenuItem.Checked
                                              ? datasWithCurrentRow.OrderBy(val => val.data.Length == 0 ? 1 : 0).
                                                    ThenByDescending(val => val.data, new CompareForAlphaNumericSort()).
                                                    ThenBy(val => val.currentRow)
                                              : datasWithCurrentRow.OrderBy(val => val.data.Length == 0 ? 1 : 0).
                                                    ThenByDescending(val => val.data).ThenBy(val => val.currentRow);
                    break;
            }
            AryaTools.Instance.Forms.SkuOrders[_instanceId] = datasWithCurrentRow.Select(val => val.id).ToList();
            FrmWaitScreen.HideMessage(waitkey);
            _populatingGridView = false;
            LastSortOption = new SortOption { Column = column, SortOrderOption = sortOrder, SortField = sortField };
        }

        private static void SplitHeaderText(string headerText, out string attributeName, out int attributeIteration)
        {
            var parts = headerText.Split('\n');
            //int.TryParse(parts[0], out navAttOrder);
            attributeName = parts[1];
            Int32.TryParse(parts[2], out attributeIteration);
        }

        private List<List<Guid>> SplitSkuList(List<Guid> skus, int size)
        {
            var listOfList = new List<List<Guid>>();
            for (var i = 0; i < skus.Count(); i += size)
                listOfList.Add(skus.Skip(i).Take(size).ToList());

            return listOfList;
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            SetStatus(_workerStatus, _workerState);

            if (_workerState == WorkerState.Abort)
            {
                statusTimer.Stop();
                AryaTools.Instance.Forms.SkuForm.CloseCurrentTab();
                return;
            } //close this tab

            if (_workerState != WorkerState.Ready)
                return;
            statusTimer.Stop();
            PopulateAttributeDropdownDataSource(cboAttributeName);

            InitEntityDataGridView();
            if (PossibleAttributeColumnsForSkuView != null && PossibleAttributeColumnsForSkuView.Count == 0)
                //handle reload skuview
                PossibleAttributeColumnsForSkuView = GetColumnsByUserPreferences();
            PopulateGridView(PossibleAttributeColumnsForSkuView);

            UpdateFilterStatus();
            tblRibbon.Visible = true;
            tblEntity.Visible = true;
            AryaTools.Instance.Forms.SkuForm.UpdateTitleAndStatus();
            edgv.Focus();

            if (AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences.AlwaysStartSkusWithShowHide
                && AryaTools.Instance.Forms.SkuTabs.Count == 1 && _showShowHideAttributeWindow)
            {
                _showShowHideAttributeWindow = false;
                //show hide attribute is set and there is only one tab in sku view
                BeginInvoke(new InvokeDelegate(AttributeOrderVisibility));
            }
        }

        private List<AttributeColumn> GetColumnsByUserPreferences()
        {
            var attributeColumns = (from att in _allAttributes
                                    where
                                        ((att.Value.Type.Equals(AttributeTypeEnum.Sku)
                                          || att.Value.Type.Equals(AttributeTypeEnum.Derived)
                                          || att.Value.Type.Equals(AttributeTypeEnum.Flag)
                                          || att.Value.Type.Equals(AttributeTypeEnum.Global))
                                         && !att.Value.AttributeName.Equals(Framework.Properties.Resources.NewAttributeName))
                                    select
                                        new AttributeColumn(att.Value, CurrentTaxonomies, null)
                                            {
                                                ColumnWidth = DefaultColumnWidth,
                                                Visible = false
                                                // all attr visibility is turned off coz it will be truned on according to the user preference
                                            }).ToList();

            EnableAttributeVisibilityByUserPreferences(attributeColumns);
            return attributeColumns;
        }

        private void EnableAttributeVisibilityByUserPreferences(List<AttributeColumn> columnProperties)
        {
            if (CurrentTaxonomies != null && CurrentTaxonomies.Count() == 1 && _showAttributesWhenSkuThresholdExceeds)
            {
                var userPrefs = AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences;
                var attributeGroups = userPrefs.AttributeGroupInclusions.ToList();
                var attributeLists = userPrefs.AttributeCustomInclusions.ToList();

                if (userPrefs.ProductAttributes)
                {
                    columnProperties.Where(col => ProductAttributeNames.Contains(col.Attribute.AttributeName)).
                        ForEach(cp => cp.Visible = true);
                }
                if (userPrefs.GlobalAttributes)
                {
                    columnProperties.Where(
                        col =>
                        AryaTools.Instance.InstanceData.GlobalAttributeNames.Contains(col.Attribute.AttributeName)).
                        ForEach(cp => cp.Visible = true);
                }
                if (userPrefs.InSchemaAttributes)
                {
                    columnProperties.Where(col => InSchemaAttributeNames.Contains(col.Attribute.AttributeName)).ForEach(
                        cp => cp.Visible = true);
                }
                if (userPrefs.RankedAttributes)
                {
                    columnProperties.Where(col => RankedAttributeNameNames.Contains(col.Attribute.AttributeName)).
                        ForEach(cp => cp.Visible = true);
                }
                //add attribute from custom list
                attributeGroups.ForEach(
                    ag => columnProperties.Where(col => col.Attribute.Group == ag).ForEach(cp => cp.Visible = true));
                // add attribute from custom group
                attributeLists.ForEach(
                    al =>
                    columnProperties.Where(col => col.Attribute.AttributeName == al).ForEach(cp => cp.Visible = true));
            }
        }

        private void toolsToolStripMenuItem_MouseEnter(object sender, EventArgs e) 
        { convertUomToToolStripMenuItem.Enabled = IsSingleDimensionEntitySet(); }

        private void txtField1_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            if (txtField1.SelectedIndex == 0)
                CurrentChange.NewValues.Field1Delete = true;
            else
                CurrentChange.NewValues.Field1 = txtField1.Text;
        }

        private void txtField2_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            if (txtField2.SelectedIndex == 0)
                CurrentChange.NewValues.Field2Delete = true;
            else
                CurrentChange.NewValues.Field2 = txtField2.Text;
        }

        private void txtField3_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            if (txtField3.SelectedIndex == 0)
                CurrentChange.NewValues.Field3Delete = true;
            else
                CurrentChange.NewValues.Field3 = txtField3.Text;
        }

        private void txtField4_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            if (txtField4.SelectedIndex == 0)
                CurrentChange.NewValues.Field4Delete = true;
            else
                CurrentChange.NewValues.Field4 = txtField4.Text;
        }

        private void txtField5_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            if (txtField5.SelectedIndex == 0)
                CurrentChange.NewValues.Field5Delete = true;
            else
                CurrentChange.NewValues.Field5 = txtField5.Text;
        }

        private void txtUom_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            if (txtUom.SelectedIndex == 0)
                CurrentChange.NewValues.FieldUomDelete = true;
            else
                CurrentChange.NewValues.Uom = txtUom.Text;
        }

        private void txtValue_Change(object sender, EventArgs e)
        {
            if (_populatingProperties || CurrentChange == null)
                return;

            CurrentChange.NewValues.Val = txtValue.Text;
        }

        private void txtNavigationAndDisplayOrder_Leave(object sender, EventArgs e)
        {
            var txtRank = sender as TextBox;
            if (txtRank == null || CurrentTaxonomy == null)
                return;
            decimal rank;
            if (!Decimal.TryParse(txtRank.Text, out rank))
            {
                txtRank.Text = String.Empty;
                return;
            }
            int iteration;
            string fieldName;
            var currentColumn = edgv.Columns[edgv.CurrentCellAddress.X];
            var columnName = currentColumn.HeaderText;
            SplitHeaderText(columnName, out fieldName, out iteration);
            var lowerFieldName = fieldName.ToLower();
            if (!_allAttributes.ContainsKey(lowerFieldName))
            {
                txtRank.Text = String.Empty;
                return;
            }
            var attribute = _allAttributes[lowerFieldName];
            if (txtRank == txtNavigationOrder)
                CurrentTaxonomy.InsertNavigationOrder(attribute, rank, true);
            else if (txtRank == txtDisplayOrder)
                CurrentTaxonomy.InsertDisplayOrder(attribute, rank, true);
            UpdateAllHeaderTexts();
        }

        private void txtValue_Enter(object sender, EventArgs e)
        {
            var currentValue = txtValue.Text;
            if (chkSuggestValues.Checked)
            {
                int iteration;
                string fieldName;
                var headerText = edgv.Columns[edgv.CurrentCellAddress.X].HeaderText;

                var sku = (Sku)edgv[0, edgv.CurrentCellAddress.Y].Value;

                SplitHeaderText(headerText, out fieldName, out iteration);

                var taxonomy = (TaxonomyInfo)edgv[1, edgv.CurrentCellAddress.Y].Value;
                Attribute attribute;
                GetAttributeFromName(fieldName, out attribute);

                string[] distinctValues = null;
                if (_validValues.ContainsKeys(taxonomy, attribute))
                    distinctValues = _validValues[taxonomy, attribute];
                else
                {
                    var schemaInfo = taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(attribute));

                    if (schemaInfo != null)
                    {
                        var sd = schemaInfo.SchemaData;
                        if (sd != null)
                        {
                            if (sd.DataType.ToLower() == "lov")
                                distinctValues = schemaInfo.ActiveListOfValues.Distinct().OrderBy(val => val).ToArray();
                        }
                    }

                    if (distinctValues == null || !distinctValues.Any())
                    {
                        distinctValues =
                            _skus.SelectMany(s => s.Value.GetValuesForAttribute(fieldName)).Select(ed => ed.Value).
                                Distinct().OrderBy(val => val).ToArray();
                    }
                }

                if (distinctValues != null)
                    _validValues.Add(taxonomy, attribute, distinctValues);

                txtValue.DataSource = HelperClasses.Validate.GetLovs(attribute, taxonomy, sku) ?? distinctValues;
                txtValue.SelectedItem = currentValue;
                if (txtValue.SelectedItem == null || !txtValue.SelectedItem.Equals(currentValue))
                {
                    txtValue.SelectedIndex = -1;
                    txtValue.Text = currentValue;
                }
            }
            else
            {
                txtValue.DataSource = null;
                txtValue.Text = currentValue;
            }
            if (_populatingProperties || CurrentChange == null)
                return;
            CurrentChange.NewValues.Val = currentValue;
        }

        private void UncheckEntityNameCheckboxes(object sender, EventArgs e)
        {
            if (_uncheckingEntityNameCheckboxes)
                return;
            _uncheckingEntityNameCheckboxes = true;
            var changedCheckbox = (CheckBox)sender;
            if (!chkUom.Equals(changedCheckbox))
                chkUom.Checked = false;
            if (!chkField1.Equals(changedCheckbox))
                chkField1.Checked = false;
            if (!chkField2.Equals(changedCheckbox))
                chkField2.Checked = false;
            if (!chkField3.Equals(changedCheckbox))
                chkField3.Checked = false;
            if (!chkField4.Equals(changedCheckbox))
                chkField4.Checked = false;
            if (!chkField5.Equals(changedCheckbox))
                chkField5.Checked = false;
            _uncheckingEntityNameCheckboxes = false;
            edgv.Invalidate();
        }

        private void Undo() { GroupUndo(_undoHistory, _redoHistory); }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) { Undo(); }

        private void UpdateAllHeaderTexts()
        {
            foreach (DataGridViewColumn col in edgv.Columns)
            {
                string attributeName;
                int iteration;
                SplitHeaderText(col.HeaderText, out attributeName, out iteration);
                if (iteration > 0)
                    continue;

                var attributeOrders = GetAttributeOrders(attributeName.ToLower());
                col.HeaderText = GetHeaderText(attributeOrders, attributeName, iteration);
            }
        }

        private void UpdateFilterContextMenu()
        {
            filterAttributeToolStripMenuItem.Text = FilterAttribute;
            filterValueToolStripMenuItem.Text = FilterValue;
            var columnIndex = edgv.CurrentCellAddress.X;
            if (columnIndex < 1)
            {
                filterAttributeToolStripMenuItem.Enabled = false;
                filterValueToolStripMenuItem.Enabled = false;
                clearThisFilterToolStripMenuItem.Enabled = false;
                clearAllFiltersToolStripMenuItem.Enabled = _currentFilters.Count > 0;
                return;
            }
            string attributeName, filterKey;
            if (columnIndex == 1)
                filterKey = attributeName = FilterTaxonomyFieldname;
            else
            {
                int iteration;
                var headerText = edgv.Columns[columnIndex].HeaderText;
                SplitHeaderText(headerText, out attributeName, out iteration);
                filterKey = attributeName.ToLower();
            }
            filterAttributeToolStripMenuItem.Text = String.Format("{0}: {1}", FilterAttribute, attributeName);
            var currentCellValue = GetSortFilterValue(edgv.CurrentCell.Value, filterByComboToolStripMenuItem.Text);
            filterValueToolStripMenuItem.Text = currentCellValue.Length > 0
                                                    ? String.Format("{0}: {1}", FilterFieldPrefix, currentCellValue)
                                                    : FilterBlankValues;
            filterAttributeToolStripMenuItem.Enabled = true;
            filterValueToolStripMenuItem.Enabled = true;
            if (_currentFilters.Any(f => f.LowerAttributeName.Equals(filterKey)))
            {
                clearThisFilterToolStripMenuItem.Enabled = true;
                clearAllFiltersToolStripMenuItem.Enabled = true;
            }
            else
            {
                clearThisFilterToolStripMenuItem.Enabled = false;
                clearAllFiltersToolStripMenuItem.Enabled = _currentFilters.Count > 0;
            }
        }

        private void UpdateFilterStatus()
        {
            var newStatus = String.Empty;
            var visibleGlobalAttributeCount =
                PossibleAttributeColumnsForSkuView.Count(
                    column =>
                    column.Visible
                    && AryaTools.Instance.InstanceData.GlobalAttributeNames.Contains(column.AttributeName));
            ;
            var visibleInSchemaAttributeCount =
                PossibleAttributeColumnsForSkuView.Count(
                    column => column.Visible && InSchemaAttributeNames.Contains(column.AttributeName));
            var visibleRankedAttributeCount =
                PossibleAttributeColumnsForSkuView.Count(
                    column => column.Visible && RankedAttributeNameNames.Contains(column.AttributeName));
            var visibleAttributeCount = PossibleAttributeColumnsForSkuView.Count(col => col.Visible);
            var skuOrderCount = AryaTools.Instance.Forms.SkuOrders[_instanceId].Count;
            if (_skus.Count != skuOrderCount)
                newStatus += String.Format("{0} of {1} SKUs. ", skuOrderCount, _skus.Count);
            else
                newStatus += _skus.Count + " SKUs. ";
            newStatus += visibleAttributeCount + " of " + PossibleAttributeColumnsForSkuView.Count + " Attributes. ";
            newStatus += visibleGlobalAttributeCount + " of "
                         + AryaTools.Instance.InstanceData.GlobalAttributeNames.Count + " Global Attributes. ";
            newStatus += visibleInSchemaAttributeCount + " of " + InSchemaAttributeNames.Count
                         + " InSchema Attributes. ";
            newStatus += visibleRankedAttributeCount + " of " + RankedAttributeNameNames.Count + " Ranked Attributes. ";

            if (!_workerStartTime.Equals(DateTime.MinValue))
            {
                var timeTaken = DateTime.Now.Subtract(_workerStartTime);
                newStatus += String.Format("Time taken: {0:00}:{1:00}", timeTaken.TotalMinutes, timeTaken.Seconds);
                _workerStartTime = DateTime.MinValue;
            }
            SetStatus(newStatus);
        }

        private void ValueToolTip()
        {
            try
            {
                var value1 = edgv.CurrentCell.Value;
                var value2 = GetCellValue2(value1);
                var value = value1 != null ? value1.ToString() : String.Empty;
                if (!String.IsNullOrEmpty(value2))
                    value += " • " + value2;
                if (String.IsNullOrEmpty(value) || !edgv.Visible)
                {
                    AryaTools.Instance.Forms.SkuForm.cellValueToolTip.Hide(
                        AryaTools.Instance.Forms.SkuForm.lblCellToolTip);
                    return;
                }

                var cellLocation =
                    edgv.GetCellDisplayRectangle(edgv.CurrentCellAddress.X, edgv.CurrentCellAddress.Y, true).Location;
                var edgvLocation = GetLocation(edgv);
                AryaTools.Instance.Forms.SkuForm.lblCellToolTip.Location = new Point(
                    edgvLocation.X + cellLocation.X, edgvLocation.Y + cellLocation.Y);
                //new Point(
                //    entityTableLayout.Location.X + splitterDataAudit.Location.X + edgv.Location.X + cellLocation.X,
                //    entityTableLayout.Location.Y + splitterDataAudit.Location.Y + edgv.Location.Y + cellLocation.Y);
                AryaTools.Instance.Forms.SkuForm.cellValueToolTip.Show(value,
                                                                          AryaTools.Instance.Forms.SkuForm.
                                                                              lblCellToolTip, 0, 0);
            }
            catch (Exception)
            {
                AryaTools.Instance.Forms.SkuForm.cellValueToolTip.Hide(
                    AryaTools.Instance.Forms.SkuForm.lblCellToolTip);
            }
        }

        private static void VerifyUnique<T>(ref bool uniqueValue, ref T previousValue, T currentValue) where T : class
        {
            if (!uniqueValue)
                return;
            if (previousValue == null)
                previousValue = currentValue;
            if (currentValue == null || !currentValue.Equals(previousValue))
                uniqueValue = false;
        }

        private bool ValidateDataType(EntityData ed)
        {
            try
            {
                var sku = ed.Sku;
                var taxonomy = sku.Taxonomy;
                var si = taxonomy.SchemaInfos.FirstOrDefault(a => a.Attribute == ed.Attribute);
                var schemaData = si.SchemaDatas.FirstOrDefault(a => a.Active);
                // do not change/comment this code!

                if (!_validatedData.Keys.Contains(ed.ID))
                {
                    var isValidDataType = HelperClasses.Validate.IsValidDataType(ed, schemaData);
                    _validatedData.Add(ed.ID, isValidDataType);
                    return isValidDataType;
                }
                else
                    return _validatedData[ed.ID];
            }
            catch
            {
                return true;
            }
        }

        internal void EntityGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control)
                return;
            switch (e.KeyCode)
            {
                case Keys.OemPeriod:
                    SortDescending(e.Shift);
                    break;

                case Keys.Oemcomma:
                    SortAscending(e.Shift);
                    break;

                case Keys.OemQuestion:
                    if (e.Shift)
                        FilterCurrentValue();
                    else
                        FilterCurrentAttribute();
                    //filterContextMenu.Show(btnFilter, 0, btnFilter.Height);
                    break;
                case Keys.End:
                    ClearAllFilters();
                    break;
                case Keys.B:
                    if (e.Shift)
                    {
                        if (AryaTools.Instance.Forms.BrowserForm.Visible)
                            AryaTools.Instance.Forms.BrowserForm.Close();
                        else
                            showItemImagesToolStripMenuItem.PerformClick();
                    }
                    break;
            }
        }

        internal static void FetchSkuAttributeValueCache(Attribute att, IQueryable<Sku> loadQuery)
        {
            try
            {
                lock (lockObj)
                {
                    var cache = AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache;
                    if (cache.ContainsKey(att))
                        return;
                    if (att.AttributeName.Equals(Framework.Properties.Resources.NewAttributeName))
                    {
                        foreach (var sku in loadQuery)
                            cache.Add(att, sku, new List<EntityData>());
                    }
                    else
                    {
                        var values = from sku in loadQuery
                                     let eds = from ei in sku.EntityInfos
                                               from ed in ei.EntityDatas
                                               where ed.Active && ed.AttributeID.Equals(att.ID)
                                               select ed
                                     select new { sku, eds };

                        foreach (var val in values)
                        {
                            var vals = EntityData.OrderSkuAttributeValues(val.eds).ToList();
                            cache.Add(att, val.sku, vals);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Let it crash, it will be cached again if not done here.
            }
        }

        internal bool GetAttributeFromName(string attributeName, out Attribute outputAttribute)
        {
            //Returns TRUE if this is a New Attribute
            attributeName = attributeName.Trim();
            if (String.IsNullOrEmpty(attributeName) || attributeName.Equals("Item ID"))
            {
                outputAttribute = null;
                return false;
            }
            Attribute newAttribute;
            var lowerAttributeName = attributeName.ToLower();

            if (_allAttributes.ContainsKey(lowerAttributeName))
            {
                newAttribute = _allAttributes[lowerAttributeName];
                outputAttribute = newAttribute;
                return false;
            }
            newAttribute = Attribute.GetAttributeFromName(attributeName, true);
            outputAttribute = newAttribute;
            if (
                PossibleAttributeColumnsForSkuView.Count(
                    col => col.Attribute.AttributeName == newAttribute.AttributeName) == 0)
            {
                PossibleAttributeColumnsForSkuView.Add(new AttributeColumn(newAttribute, CurrentTaxonomies, null) { ColumnWidth = DefaultColumnWidth, Visible = true });
            }
            if (!_allAttributes.Keys.Contains(newAttribute.AttributeName.ToLower()))
                _allAttributes.Add(newAttribute.AttributeName.ToLower(), newAttribute);
            return true;
        }

        private void SetStatus(string status, WorkerState workerState)
        {
            _workerState = workerState;
            SetStatus(status);
        }

        private void openInBuildViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //object taxonomy = edgv.CurrentCell.OwningRow.Cells[1].Value;

            var skus = GetSkus(edgv.SelectedCells);
            AryaTools.Instance.Forms.BuildForm.LoadTab(skus, null);
        }

        private List<Sku> GetSkus(DataGridViewSelectedCellCollection selectedCells)
        {
            var skus = new List<Sku>();

            for (var i = 0; i < selectedCells.Count; i++)
                skus.Add((Sku)edgv[0, selectedCells[i].RowIndex].Value);

            return skus;
        }

        private void selectionChangedTimer_Tick(object sender, EventArgs e) { SelectionChanged(); }

        private void edgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (edgv.Rows.Count == 0)
            {
                edgv.Columns[e.ColumnIndex].Selected = false;
                return;
            }
            SelectAttributeFromColumn(e.ColumnIndex);
        }

        private void groupSKUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mCreateSkuGroup.var x = GetSKUs(edgv.SelectedCells);
            var CreateSkuGroupForm = new FrmCreateSkuGroup(GetSkus(edgv.SelectedCells));
            CreateSkuGroupForm.ShowDialog();
            // AryaTools.Instance.CreateSkuGroupForm.Show();
        }

        private void dgvAuditTrail_DataSourceChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn col in dgvAuditTrail.Columns)
                col.HeaderCell = new DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);

            dgvAuditTrail.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgvAuditTrail.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.None);
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var entityDatas = new List<EntityData>();
            for (var i = 0; i < edgv.SelectedCells.Count; i++)
            {
                if (edgv.SelectedCells[i].Value is EntityData
                    && ((EntityData)edgv.SelectedCells[i].Value).ID != Guid.Empty)
                    entityDatas.Add((EntityData)edgv.SelectedCells[i].Value);
            }

            if (entityDatas.Count > 0)
            {
                var notes = new FrmNotes(entityDatas);
                notes.ShowDialog();
            }
        }

        private void validateToolStripMenuItem_CheckedChanged(object sender, EventArgs e) { edgv.Invalidate(); }

        private void addToWorkflowToolStripMenuItem_Click(object sender, EventArgs e) { new FrmAddToWorkflow(GetSkus(edgv.SelectedCells)).ShowDialog(); }

        private void edgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var currentColumnIndex = e.ColumnIndex;
            if (e.ColumnIndex > 1)
            {
                int iteration;
                string fieldName;
                Attribute att;
                var columnName = edgv.Columns[currentColumnIndex].HeaderText;
                SplitHeaderText(columnName, out fieldName, out iteration);
                GetAttributeFromName(fieldName, out att);
                if (att.AttributeType == AttributeTypeEnum.Workflow.ToString())
                {
                    btnWrkflow.Visible = true;
                    selectedWorkflow = att.AttributeName;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) { contextMenuStripWorkflow.Show(btnWrkflow, 0, btnWrkflow.Height); }

        private void approveMenu_Click(object sender, EventArgs e)
        {
            var skus = GetSkus(edgv.SelectedCells);
            var skuStates =
                skus.SelectMany(s => s.SkuStates).Where(
                    st => st.State.Workflow.WorkflowName == selectedWorkflow && st.Active);
            foreach (var skuState in skuStates)
                skuState.UserApproved = true;

            AryaTools.Instance.SaveChangesIfNecessary(true, true);
        }

        private void unapproveMenu_Click(object sender, EventArgs e)
        {
            var skus = GetSkus(edgv.SelectedCells);
            var skuStates =
                skus.SelectMany(s => s.SkuStates).Where(
                    st => st.State.Workflow.WorkflowName == selectedWorkflow && st.Active);
            foreach (var skuState in skuStates)
                skuState.UserApproved = false;

            AryaTools.Instance.SaveChangesIfNecessary(true, true);
        }

        private void loadAllImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pictureBoxUrl = AryaTools.Instance.InstanceData.CurrentProject.PictureBoxUrl;

            if (pictureBoxUrl == null)
                return;
            var msgKey = FrmWaitScreen.ShowMessage("Caching images");
            _skus.Values.ForEach(
                p => imageCache.GetAsset(p, pictureBoxUrl.AssetAttributeName, pictureBoxUrl.Value, pictureBoxUrl.Type));
            FrmWaitScreen.HideMessage(msgKey);
        }

        private void showAttributePreferencesToolStripMenuItem_Click(object sender, EventArgs e) { AttributePreferenceVisibility(); }

        private void showInSchemaAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var inSchemaAttrNames = InSchemaAttributeNames;
            foreach (var columnPropertyName in inSchemaAttrNames)
            {
                if (
                    PossibleAttributeColumnsForSkuView.Any(
                        col => col.AttributeName == columnPropertyName && col.Visible == false))
                    PossibleAttributeColumnsForSkuView.First(col => col.Attribute.AttributeName == columnPropertyName).
                        Visible = true;
            }
            PopulateGridView(PossibleAttributeColumnsForSkuView);
        }

        private void showRankedAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rankedAttrNames = RankedAttributeNameNames;
            foreach (var columnPropertyName in rankedAttrNames)
            {
                if (
                    PossibleAttributeColumnsForSkuView.Any(
                        col => col.AttributeName == columnPropertyName && col.Visible == false))
                    PossibleAttributeColumnsForSkuView.First(col => col.Attribute.AttributeName == columnPropertyName).
                        Visible = true;
            }
            PopulateGridView(PossibleAttributeColumnsForSkuView);
        }

        private void showExtendedAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var extentedAttrNames = ProductAttributeNames;
            foreach (var columnPropertyName in extentedAttrNames)
            {
                if (
                    PossibleAttributeColumnsForSkuView.Any(
                        col => col.AttributeName == columnPropertyName && col.Visible == false))
                    PossibleAttributeColumnsForSkuView.First(col => col.Attribute.AttributeName == columnPropertyName).
                        Visible = true;
            }
            PopulateGridView(PossibleAttributeColumnsForSkuView);
        }

        private void convertUnitOfMeasureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentChange.UnitofMeasure = ((ProjectUom)convertToUomDropDown.SelectedItem).UnitOfMeasure;
            CurrentChange.UnitPrecision = Int32.Parse(convertUomPrecisionDropDown.Text);
            CurrentChange.RunTool = Change.Tool.ConvertToUom;
            SaveCurrentChangeAndGetNewChange(false);
            RedrawDataGridView(null);
        }

        private void convertToUomComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            convertUomPrecisionDropDown.ComboBox.SelectedIndex =
                ((ProjectUom)convertToUomDropDown.SelectedItem).UnitOfMeasure.Precision ?? 4;
        }

        private class SortOption
        {
            public int Column { get; set; }
            //private int column;
            public SortOrder SortOrderOption { get; set; }
            // private SortOrder sortOrder;
            //private string sortField;
            public string SortField { get; set; }
        }

        #endregion Methods

        private void checkSpellingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool doneSpellCheck = false;   
            var selectedCells = edgv.SelectedCells;
            var currentColumnindex = edgv.CurrentCell.ColumnIndex;
            var totalRows = edgv.RowCount;
            DialogResult result;  
            CheckSpellingForCells(selectedCells);
            if (selectedCells.Count < totalRows)
            {
                result = MessageBox.Show(this, "Would you like to check the spelling of the entire column?", "Spell Check", MessageBoxButtons.YesNo);
                 if (result == DialogResult.Yes)
                 {
                     SpellCheckForColumn(currentColumnindex);
                 }
                 else
                 {
                     doneSpellCheck = true;
                 }
            }
            if (!doneSpellCheck)
            {
                result = MessageBox.Show(this, "Would you like to check the spelling of the entire tab?", "Spell Check", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SpellCheckEntireView();
                }
            }
        }
        private void SpellCheckEntireView()
        {
            List<ISpell> values = new List<ISpell>();
            for (int columnIndex = 1; columnIndex < edgv.ColumnCount; columnIndex++)
            {
                for (int rowIndex = 1; rowIndex < edgv.RowCount; rowIndex++)
                {
                    ISpell currentValue = edgv[columnIndex, rowIndex].Value as ISpell;
                    if (currentValue != null && (((EntityData)currentValue).Attribute != null))
                        values.Add(currentValue);
                }
            }
            DisplaySpellCheck(values, Arya.Properties.Resources.SpellCheckContextMessageForEntireView);
        }
        private void SpellCheckForColumn(int currentColumnindex)
        {
            //int columnIndex = edgv.CurrentCell.ColumnIndex;
            List<ISpell> values = new List<ISpell>();
            for (int i = 1; i < edgv.RowCount; i++)
            {
                ISpell currentValue = edgv[currentColumnindex, i].Value as ISpell;
                if (currentValue != null && (((EntityData)currentValue).Attribute != null))
                    values.Add(currentValue);

            }
            DisplaySpellCheck(values, Arya.Properties.Resources.SpellCheckContextMessageForEntireColumn.ToString());
        }

        private void CheckSpellingForCells(DataGridViewSelectedCellCollection selectedCells)
        {
            List<ISpell> values = new List<ISpell>();
            foreach (DataGridViewCell cell in selectedCells)
            {
                if (cell.Value != "" && cell.Value.GetType() == typeof(EntityData))
                {
                    if (((EntityData)cell.Value).Attribute != null)
                        values.Add((ISpell)cell.Value);
                }

            }
            DisplaySpellCheck(values, values.Count + " " + Arya.Properties.Resources.SpellCheckContextMessageForSelectedCells);
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
               // MessageBox.Show("No spelling error in the context.");
            }
        }

        void InvalidateEdgv()
        {
            RefreshCurrentTab();
        }

       
    }
}