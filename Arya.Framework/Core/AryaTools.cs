using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using LinqKit;
using Microsoft.Win32;
using Natalie.Data;
using Natalie.HelperForms;
using Natalie.Properties;
using Natalie.UserControls;
using Natalie.Framework.IO.Exports;
using Attribute = Natalie.Data.Attribute;

namespace Natalie.HelperClasses
{
    public class NatalieTools
    {
        #region Fields (37)

        public string DisplayName;
        public string AccessID;
        public const string DefaultDatabaseName = "Natalie3";
        public const string DefaultStatus = "Ready";
        public const string RegistryKeyAutoSuggestOption = "AutoSugggestOption";
        public const string RegistryKeyCurrentTaxonomy = "CurrentTaxonomy";
        private const string SkuManagerRegistrySubKey = "SkuManager";
        public const string RegistryKeyCurrentRemarksPosition = "CurrnetRemarksPosition";
        private static NatalieTools _instance;

        private static readonly Regex Fraction = // The regex for fraction will not catch integers
            new Regex(
                "^(?<sign>[+-]?)\\s*((?<integer>\\d+)?\\s*[ -]\\s*)?(?<numerator>\\d+)\\s*/\\s*(?<denominator>\\d+)$",
                RegexOptions.Compiled);

        internal readonly Queue<Guid> LoadQueue = new Queue<Guid>();

        public readonly Dictionary<string, KeyValuePair<Uri, string>> SnippetCache =
            new Dictionary<string, KeyValuePair<Uri, string>>();

        private bool lookForOpenWindows = true;
        public bool LookForOpenWindows
        {
            get { return lookForOpenWindows; }
            set { lookForOpenWindows = value; }
        }

        public bool CloseTreeForm = true;

        public FillRateWorker AllFillRates = new FillRateWorker();
        public bool AutoLogin;
        private string _currentRemark;
        public string CurrentRemark
        {
            get
            {
                if (_currentRemark == null || _currentRemark.Equals(FrmRemark.DefaultRemark))
                    return string.Empty;
                return _currentRemark;
            }

            set
            {
                if (_currentRemark != null && value != null && _currentRemark.Equals(value))
                    return;

                _currentRemark = value;
            }
        }

        internal User CurrentUser;
        public bool InitOk = true;
        public Guid? RemarkID;
        public Dictionary<TabPage, SchemaDataGridView> SchemaTabs;
        internal Dictionary<Guid, List<Guid>> SkuOrders;
        internal Dictionary<TabPage, EntityDataGridView> SkuTabs;
        public Remark StickyRemark;
        public Dictionary<Guid, string> ProjectUsers = new Dictionary<Guid, string>();
        public bool SubmittingChangesToDb;

        private FrmAttributeFarm _attributeFarmView;
        private FrmAttributeView _attributeView;
        private RegistryKey _baseKey;
        private FrmBrowser _browserForm;
        private FrmBuildView _buildForm;
        private FrmCharacterMap _characterMapForm;
        private FrmCheckpoint _checkpointForm;
        private FrmCloneOptions _cloneOptionsForm;
        private FrmColorRules _colorForm;
        private Project _currentProject;
        private SkuDataDbDataContext _dc;
        private frmFilter _filterForm;
        private string _itemImagesTempFile = string.Empty;
        private FrmListOfValues _listofValuesForm;
        private FrmQueryView _queryForm;
        private Dictionary<TaxonomyInfo, string> _rankedAttributeCount;
        private FrmRemark _remarksForm;
        private FrmSchemaView _schemaForm;
        private Dictionary<TaxonomyInfo, int> _skuCounts;
        private FrmSkuView _skuForm;
        private FrmSkuLinks _skuLinksForm;
        private FrmSkuGroups _skuGroupsForm;
        private FrmSkuLinksView _skuLinksView;
        private FrmTree _treeForm;
        private FrmCreateSkuGroup _createSkuGroupsForm;
        private FrmNotes _notesForm;
        private FrmStartup _startupForm;
        private FrmWorkflow _workflowForm;
        private FrmUnitOfMeasure _uomForm;


        #endregion Fields

        #region Constructors (1)

        private NatalieTools()
        {
            Guid waitkey = FrmWaitScreen.ShowMessage("Starting Natalie");
            InitNatalieTools();
            FrmWaitScreen.HideMessage(waitkey);
        }

        ~NatalieTools()
        {
            if (!string.IsNullOrEmpty(_itemImagesTempFile))
            {
                File.Delete(_itemImagesTempFile);
            }
        }

        #endregion Constructors

        #region Properties (20)

        private bool _autoSave;

        public string ItemImagesTempFile
        {
            get
            {
                if (_itemImagesTempFile == string.Empty)
                {
                    _itemImagesTempFile = Path.GetTempFileName();
                }
                return _itemImagesTempFile;
            }
        }

        public bool IsTreeViewOpen
        {
            get { return _treeForm != null && !_treeForm.IsDisposed; }
        }

        public bool IsSkuViewOpen
        {
            get { return _skuForm != null && !_skuForm.IsDisposed; }
        }

        public bool IsSchemaViewOpen
        {
            get { return _schemaForm != null && !_schemaForm.IsDisposed; }
        }

        internal FrmStartup StartupForm
        {
            get
            {
                if (_startupForm == null || _startupForm.IsDisposed)
                    _startupForm = new FrmStartup();

                return _startupForm;
            }
        }
        internal FrmUnitOfMeasure UomForm
        {
            get
            {
                if (_uomForm == null || _uomForm.IsDisposed)
                    _uomForm = new FrmUnitOfMeasure
                    {
                        Left = TreeForm.Right,
                        Top = TreeForm.Top,
                        Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                        Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                        WindowState = FormWindowState.Normal
                    };

                return _uomForm;
            }
        }


        internal FrmAttributeFarm AttributeFarmView
        {
            get
            {
                if (_attributeFarmView == null || _attributeFarmView.IsDisposed)
                {
                    Rectangle workingArea = Screen.FromControl(TreeForm).WorkingArea;
                    _attributeFarmView = new FrmAttributeFarm
                                             {
                                                 Left = TreeForm.Right,
                                                 Top = 0,
                                                 Height = workingArea.Height,
                                                 Width = workingArea.Width / 3
                                             };
                }
                return _attributeFarmView;
            }
        }

        internal FrmAttributeView AttributeView
        {
            get
            {
                if (_attributeView == null || _attributeView.IsDisposed)
                {
                    Rectangle workingArea = Screen.FromControl(TreeForm).WorkingArea;
                    _attributeView = new FrmAttributeView
                                         {
                                             Left = TreeForm.Right,
                                             Top = 0,
                                             Height = workingArea.Height,
                                             Width = workingArea.Width - TreeForm.Width
                                         };
                }
                return _attributeView;
            }
        }

        public bool AutoSave
        {
            get { return _autoSave; }
            set
            {
                _autoSave = value;
                if (_remarksForm != null)
                {
                    _remarksForm.Text = "Remarks" + (_autoSave ? string.Empty : " Auto Save Disabled");
                }
            }
        }

        internal FrmBrowser BrowserForm
        {
            get
            {
                if (_browserForm == null || _browserForm.IsDisposed)
                    _browserForm = new FrmBrowser();
                return _browserForm;
            }
        }

        internal FrmWorkflow WorkflowForm
        {
            get
            {
                if (_workflowForm == null || _workflowForm.IsDisposed)
                    _workflowForm = new FrmWorkflow
                    {
                        //Left = TreeForm.Right,
                        //Top = TreeForm.Top,
                        //Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                        //Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                        //WindowState = FormWindowState.Normal
                    };
                return _workflowForm;
            }
        }

        internal FrmNotes NotesForm
        {
            get
            {
                if (_notesForm == null || _notesForm.IsDisposed)
                    _notesForm = new FrmNotes
                    {
                        WindowState = FormWindowState.Normal
                    };
                return _notesForm;
            }
        }


        internal FrmCharacterMap CharacterMapForm
        {
            get
            {
                if (_characterMapForm == null || _characterMapForm.IsDisposed)
                {
                    _characterMapForm = new FrmCharacterMap();
                }
                return _characterMapForm;
            }
        }

        public FrmCheckpoint CheckpointForm
        {
            get
            {
                if (_checkpointForm == null || _checkpointForm.IsDisposed)
                    _checkpointForm = new FrmCheckpoint();

                return _checkpointForm;
            }
        }

        public FrmCloneOptions CloneOptionsForm
        {
            get
            {
                if (_cloneOptionsForm == null || _cloneOptionsForm.IsDisposed)
                    _cloneOptionsForm = new FrmCloneOptions();
                return _cloneOptionsForm;
            }
        }

        internal FrmColorRules ColorForm
        {
            get
            {
                if (_colorForm == null || _colorForm.IsDisposed)
                    _colorForm = new FrmColorRules();

                return _colorForm;
            }
        }

        public Project CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;

                Program.WriteToErrorFile("Project: " + _currentProject.ProjectName, true);

                if (value.DatabaseName == null)
                    return;

                //Load User's Groups
                Instance.CurrentUser.UserGroups =
                    Instance.CurrentUser.UserProjects.Where(p => p.ProjectID == _currentProject.ID && p.Group.GroupType.StartsWith("USER_",StringComparison.OrdinalIgnoreCase)).Select(
                        p => p.GroupID).Distinct().ToList();

                Instance.Dc.Connection.ChangeDatabase(_currentProject.DatabaseName);

                //using ( var userdb = new SkuDataDbDataContext(Dc.Connection.ConnectionString.Replace("Natalie3",_currentProject.DatabaseName)))
                //{
                //    var isUserPresent = userdb.Users.Where(u => u.ID == CurrentUser.ID).FirstOrDefault();
                //    if (isUserPresent == null)
                //    {
                    
                //        User newUser = new User() { ID = CurrentUser.ID, SingleSignOnId = CurrentUser.SingleSignOnId, IsAdmin = CurrentUser.IsAdmin, CreatedOn = DateTime.Now };
                //        userdb.Users.InsertOnSubmit(newUser);
                //        userdb.SubmitChanges();
                //    }
                //}

                _currentProject = Dc.Projects.First(prj => prj.ID.Equals(_currentProject.ID));
                CurrentUser = Dc.Users.First(usr => usr.ID.Equals(CurrentUser.ID));
                PopulateBaseUnitCache(_currentProject.ID);
            }
        }

        private static void PopulateBaseUnitCache(Guid projectId)
        {
        }

        internal SkuDataDbDataContext Dc
        {
            get
            {
                InitDataContext();
                return _dc;
            }
            set { _dc = value; }
        }

        internal frmFilter FilterForm
        {
            get
            {
                if (_filterForm == null || _filterForm.IsDisposed)
                    _filterForm = new frmFilter();

                return _filterForm;
            }
        }

        public static NatalieTools Instance
        {
            get { return _instance ?? (_instance = new NatalieTools()); }
        }

        public FrmListOfValues ListofValuesForm
        {
            get
            {
                Rectangle workingArea = Screen.FromControl(TreeForm).WorkingArea;
                if (_listofValuesForm == null || _listofValuesForm.IsDisposed)
                    _listofValuesForm = new FrmListOfValues
                                            {
                                                Left = workingArea.Right - (workingArea.Width / 3),
                                                Top = 0,
                                                Height = workingArea.Height,
                                                Width = workingArea.Width / 3
                                            };

                return _listofValuesForm;
            }
        }

        [DefaultValue(false)]
        public bool DontShowRemarkForm { get; set; }

        public FrmRemark RemarksForm;

        //public FrmRemark RemarksForm
        //{
        //    get
        //    {
        //        if (_remarksForm == null || _remarksForm.IsDisposed)
        //        {
        //            _remarksForm = new FrmRemark();
        //            //www.leetcode.com/2010/04/how-to-determine-if-point-is-inside.html
        //            int left = 0, top = 0;
        //            try
        //            {
        //                string[] location = Instance.GetFromRegistry(RegistryKeyCurrentRemarksPosition).Split(',');
        //                left = Convert.ToInt32(location[0]);
        //                top = Convert.ToInt32(location[1]);

        //                int left1 = left;
        //                int top1 = top;
        //                if (!Screen.AllScreens.Any(screen => left1 > screen.Bounds.X && top1 < screen.Bounds.Width))
        //                {
        //                    left = TreeForm.Left + 5;
        //                    top = 0;
        //                }
        //            }
        //            catch
        //            {
        //                left = TreeForm.Left + 5;
        //            }
        //            finally
        //            {
        //                _remarksForm.Left = left;
        //                _remarksForm.Top = top;
        //            }
        //        }

        //        return _remarksForm;
        //    }
        //}

        internal FrmQueryView QueryForm
        {
            get
            {
                if (_queryForm == null || _queryForm.IsDisposed)
                {
                    _queryForm = new FrmQueryView
                                     {
                                         Left = TreeForm.Right,
                                         Top = TreeForm.Top,
                                         Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                         Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                         WindowState = FormWindowState.Normal
                                     };
                }

                return _queryForm;
            }
        }

        public FrmSchemaView SchemaForm
        {
            get
            {
                if (_schemaForm == null || _schemaForm.IsDisposed)
                    _schemaForm = new FrmSchemaView
                                      {
                                          Left = TreeForm.Right,
                                          Top = TreeForm.Top,
                                          Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                          Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                          WindowState = FormWindowState.Normal
                                      };

                return _schemaForm;
            }
        }


        public FrmBuildView BuildForm
        {
            get
            {
                if (_buildForm == null || _buildForm.IsDisposed)
                    _buildForm = new FrmBuildView
                                     {
                                         Left = TreeForm.Right,
                                         Top = TreeForm.Top,
                                         Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                         Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                         WindowState = FormWindowState.Normal
                                     };

                return _buildForm;
            }
        }

        internal FrmSkuView SkuForm
        {
            get
            {
                if (_skuForm == null || _skuForm.IsDisposed)
                    _skuForm = new FrmSkuView
                                   {
                                       Left = TreeForm.Right,
                                       Top = TreeForm.Top,
                                       Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                       Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                       WindowState = FormWindowState.Maximized
                                   };

                return _skuForm;
            }
        }


        internal FrmCreateSkuGroup CreateSkuGroupForm
        {

            get
            {
                if (_createSkuGroupsForm == null || _createSkuGroupsForm.IsDisposed)
                {
                    _createSkuGroupsForm = new FrmCreateSkuGroup
                    {


                        WindowState = FormWindowState.Normal
                    };
                }
                return _createSkuGroupsForm;
            }

        }


        internal FrmSkuGroups SkuGroupsForm
        {

            get
            {
                if (_skuGroupsForm == null || _skuGroupsForm.IsDisposed)
                {
                    _skuGroupsForm = new FrmSkuGroups
                    {


                        WindowState = FormWindowState.Normal
                    };
                }
                return _skuGroupsForm;
            }

        }

        internal FrmSkuLinks SkuLinksForm
        {
            get
            {
                if (_skuLinksForm == null || _skuLinksForm.IsDisposed)
                {
                    _skuLinksForm = new FrmSkuLinks
                                        {
                                            Left = TreeForm.Right,
                                            Top = TreeForm.Top,
                                            Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                            Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                            WindowState = FormWindowState.Normal
                                        };
                }
                return _skuLinksForm;
            }
        }

        internal FrmSkuLinksView SkuLinksViewForm
        {
            get
            {
                if (_skuLinksView == null || _skuLinksView.IsDisposed)
                {
                    _skuLinksView = new FrmSkuLinksView
                                        {
                                            Left = TreeForm.Right,
                                            Top = TreeForm.Top,
                                            Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                            Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                            WindowState = FormWindowState.Normal
                                        };
                }

                return _skuLinksView;
            }
        }

        internal FrmTree TreeForm
        {
            get
            {
                if (_treeForm == null || _treeForm.IsDisposed)
                    _treeForm = new FrmTree(null)
                    {
                        Left = Screen.PrimaryScreen.WorkingArea.Left,
                        Top = Screen.PrimaryScreen.WorkingArea.Top
                    };

                return _treeForm;
            }
        }

        #endregion Properties

        #region Methods (25)

        // Public Methods (11) 

        #region Delegates

        public delegate void InvokeDelegate();

        #endregion


        public static bool CheckDatabaseExists(SkuDataDbDataContext db, string databaseName)
        {
            bool result;

            try
            {
                if (db.Connection.State == ConnectionState.Closed)
                {
                    db.Connection.Open();
                }

                result =
                    db.ExecuteQuery<int>(@"SELECT database_id FROM sys.databases WHERE Name = {0}", databaseName).
                        SingleOrDefault() > 0;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public void CloseAllForms()
        {
            if (_skuForm != null && !_skuForm.IsDisposed)
                _skuForm.Close();

            if (_schemaForm != null && !_schemaForm.IsDisposed)
                _schemaForm.Close();

            if (_browserForm != null && !_browserForm.IsDisposed)
            {
                _browserForm.ForceClose = true;
                _browserForm.Close();
            }

            if (_attributeView != null && !_attributeView.IsDisposed)
                _attributeView.Close();

            if (_queryForm != null && !_queryForm.IsDisposed)
                _queryForm.Close();

            if (_characterMapForm != null && !_characterMapForm.IsDisposed)
                _characterMapForm.Close();

            if (_listofValuesForm != null && !_listofValuesForm.IsDisposed)
                _listofValuesForm.Close();

            if (_attributeFarmView != null && !_attributeFarmView.IsDisposed)
                _attributeFarmView.Close();

            if (_skuLinksForm != null && !_skuLinksForm.IsDisposed)
                _skuLinksForm.Close();

            if (_remarksForm != null && !_remarksForm.IsDisposed)
                _remarksForm.Close();

            _dc = null;
            SchemaAttribute.SecondarySchemati = null;
        }

        public static void EmailTechTeam(string subject, string messageBody = null)
        {
            const string Username = "natalie.bytemanagers@gmail.com";
            const string Password = "2010sNewPassword";
            const string FromAddress = "natalie.bytemanagers@gmail.com";
            const string ToAddress = "techteam@g.bytemanagers.com";
            var defaultSubject = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            if (String.IsNullOrEmpty(messageBody))
                messageBody = File.ReadAllText(Program.ErrorFileName);

            var message = new MailMessage
                              {
                                  Subject =
                                      defaultSubject + (!String.IsNullOrEmpty(subject) ? ": " + subject : String.Empty),
                                  Body = messageBody,
                                  From = new MailAddress(FromAddress)
                              };

            message.To.Add(ToAddress);
            //// Allow multiple "To" addresses to be separated by a semi-colon
            //if (toAddress.Trim().Length > 0)
            //{
            //    foreach (string addr in toAddress.Split(';'))
            //    {
            //        message.To.Add(new MailAddress(addr));
            //    }
            //}

            // Set the SMTP server to be used to send the message
            var client = new SmtpClient
                             {
                                 Host = "smtp.gmail.com",
                                 Port = 587,
                                 EnableSsl = true,
                                 DeliveryMethod = SmtpDeliveryMethod.Network,
                                 Credentials = new NetworkCredential(Username, Password)
                             };

            // Send the e-mail message
            client.Send(message);
        }

        public static void EnableDisableMenuItems(ToolStripItemCollection items, bool value)
        {
            foreach (ToolStripItem item in items)
            {
                item.Enabled = value;
                var toolStripMenuItem = item as ToolStripMenuItem;
                if (toolStripMenuItem != null)
                    EnableDisableMenuItems((toolStripMenuItem).DropDownItems, value);
            }
        }

        public static void GetTabName(ICollection<Attribute> attsToLoad, out string tabName, out string toolTipText)
        {
            // Full taxonomy list is a list of all nodes in the selection
            toolTipText = attsToLoad.OrderBy(node => node.ToString()).Aggregate(
                String.Empty,
                (current, attribute) =>
                current + ((current.Length > 0 ? Environment.NewLine : String.Empty) + attribute));

            // Also, generate a shorter list for to show in limited space
            string attList = String.Empty;
            foreach (Attribute attribute in attsToLoad.OrderBy(att => att.AttributeName.Length))
            {
                if (attList.Length + attribute.AttributeName.Length > 40)
                {
                    if (attList.Length > 0 && !attList.EndsWith(", ..."))
                        attList += ", ...";
                }
                else
                    attList += (attList.Length > 0 ? ", " : String.Empty) + attribute.AttributeName;
            }
            if (attList.Length > 0)
                attList += " - ";
            tabName = attsToLoad.Count() == 1
                          ? attsToLoad.First().AttributeName
                          : attList + attsToLoad.Count + " attributes";
        }

        public static void GetTabName(
            ICollection<TaxonomyInfo> nodesSelected, ICollection<TaxonomyInfo> nodesToLoad, out string tabName,
            out string toolTipText)
        {
            // Full taxonomy list is a list of all nodes in the selection
            toolTipText = nodesToLoad.OrderBy(node => node.ToString()).Aggregate(
                String.Empty,
                (current, taxonomyInfo) =>
                current + ((current.Length > 0 ? Environment.NewLine : String.Empty) + taxonomyInfo));

            // Also, generate a shorter list for to show in limited space
            string taxList = String.Empty;
            foreach (TaxonomyInfo taxonomy in nodesSelected.OrderBy(tax => tax.TaxonomyData.NodeName.Length))
            {
                if (taxList.Length + taxonomy.TaxonomyData.NodeName.Length > 40)
                {
                    if (taxList.Length > 0 && !taxList.EndsWith(", ..."))
                        taxList += ", ...";
                }
                else
                    taxList += (taxList.Length > 0 ? ", " : String.Empty) + taxonomy.TaxonomyData.NodeName;
            }
            if (taxList.Length > 0)
                taxList += " - ";
            tabName = nodesSelected.Count == 1
                          ? nodesSelected.First().TaxonomyData.NodeName +
                            (nodesToLoad.Count > 1 ? String.Format(" ({0} nodes)", nodesToLoad.Count) : String.Empty)
                          : taxList + nodesSelected.Count + " nodes" +
                            (nodesSelected.Count < nodesToLoad.Count
                                 ? String.Format(" selected ({0} loaded)", nodesToLoad.Count)
                                 : String.Empty);
        }

        public void InitNatalieTools()
        {
            SkuTabs = new Dictionary<TabPage, EntityDataGridView>();
            SchemaTabs = new Dictionary<TabPage, SchemaDataGridView>();
            SkuOrders = new Dictionary<Guid, List<Guid>>();
            _skuCounts = new Dictionary<TaxonomyInfo, int>();
            _rankedAttributeCount = new Dictionary<TaxonomyInfo, string>();
            SubmittingChangesToDb = false;
            AutoSave = true;

            //Copy the hourglass file - once
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Natalie";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string hourGlassFileName = directory + "\\hourglass.png";
            if (!File.Exists(hourGlassFileName))
            {
                Resources.HourGlass.Save(hourGlassFileName);
            }

            if (_dc != null)
                _dc.Dispose();

            _dc = null;

            if (!InitDataContext())
                InitOk = false;
        }

        public static bool IsFract(string text, out double result)
        {
            Match match = Fraction.Match(text);
            if (match.Success)
            {
                double value = match.Groups["integer"].Success ? Int64.Parse(match.Groups["integer"].Value) : 0;

                if (match.Groups["numerator"].Success && match.Groups["denominator"].Success)
                    value += Double.Parse(match.Groups["numerator"].Value) /
                             Double.Parse(match.Groups["denominator"].Value);

                if (match.Groups["sign"].Success && match.Groups["sign"].Value.Equals("-"))
                    value *= -1;

                result = value;
                return true;
            }

            result = 0.0;
            return false;
        }

        public static bool MustCloseForm(FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.MdiFormClosing:
                case CloseReason.WindowsShutDown:
                case CloseReason.TaskManagerClosing:
                case CloseReason.FormOwnerClosing:
                case CloseReason.ApplicationExitCall:
                    return true;
            }

            return false;
        }

        public static void SetAutoComplete(TextBox textBox, IEnumerable<string> enumerable)
        {
            var autoComplete = new AutoCompleteStringCollection();

            if (enumerable != null)
                foreach (string item in enumerable)
                    autoComplete.Add(item);

            textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox.AutoCompleteCustomSource = autoComplete;
        }

        public static bool TryConvertToNumber(string text, out double result)
        {
            double value;
            bool success = Double.TryParse(text, out value);
            if (success)
            {
                result = value;
                return true;
            }

            if (IsFract(text, out result))
                return true;

            result = 0;
            return false;
        }

        // Private Methods (4) 

        public bool InitDataContext(Guid projectID = default(Guid), bool forceInit = false)
        {
            if (_dc == null || forceInit)
            {
                _dc = new SkuDataDbDataContext();

                if (CurrentUser != null)
                    CurrentUser = _dc.Users.Single(usr => usr.ID.Equals(CurrentUser.ID));

                if(projectID != default(Guid))
                {
                    CurrentProject = _dc.Projects.Single(prj => prj.ID == projectID);
                }
                else if (CurrentProject != null)
                    CurrentProject = _dc.Projects.Single(prj => prj.ID.Equals(_currentProject.ID));
            }

            while (_dc.Connection.State != ConnectionState.Open)
            {
                if (_dc.Connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        _dc.Connection.Open();
                    }
                    catch (Exception ex)
                    {
                        DialogResult result = MessageBox.Show(
                            ex.Message + @" Do you want to try again?", @"Database connection lost",
                            MessageBoxButtons.YesNo);
                        if (result == DialogResult.No)
                        {
                            Application.Exit();
                            return false;
                        }
                    }
                }

                while (_dc.Connection.State == ConnectionState.Connecting)
                    Thread.Sleep(100);
            }

            return true;
        }

        private void InitRegistryBaseKey(bool autoCreate)
        {
            _baseKey = Registry.CurrentUser.OpenSubKey(SkuManagerRegistrySubKey, true);

            if (_baseKey == null && autoCreate)
                _baseKey = Registry.CurrentUser.CreateSubKey(SkuManagerRegistrySubKey);
        }

        public object UpdateNodeWorkerSyncRoot = new object();
        public object UpdateDefinitionWorkerSyncRoot = new object();
        public object UpdateEnrichmentsWorkerSyncRoot = new object();

        private void SaveChangesToDb()
        {
            if (!string.IsNullOrWhiteSpace(Instance.CurrentRemark))
            {
                SkuTabs.Values.ForEach(
                    egv => egv.SetStatus(string.Format("Saving ({0})", Instance.CurrentRemark), WorkerState.Working));
            }
            else
            {
                SkuTabs.Values.ForEach(
                    egv => egv.SetStatus("Saving", WorkerState.Working));
            }

            var waitKey = FrmWaitScreen.ShowMessage("Waiting for workers to finish");

            if (!Monitor.TryEnter(Instance.UpdateNodeWorkerSyncRoot, TimeSpan.FromSeconds(1)) ||
                !Monitor.TryEnter(Instance.UpdateDefinitionWorkerSyncRoot, TimeSpan.FromSeconds(1)) ||
                !Monitor.TryEnter(Instance.UpdateEnrichmentsWorkerSyncRoot, TimeSpan.FromSeconds(1)))
            {
                TreeForm.taxonomyTree.AbortWorkers();
                Monitor.Enter(Instance.UpdateNodeWorkerSyncRoot);
                Monitor.Enter(Instance.UpdateDefinitionWorkerSyncRoot);
                Monitor.Enter(Instance.UpdateEnrichmentsWorkerSyncRoot);
            }
            try
            {
                FrmWaitScreen.UpdateMessage(waitKey, "Saving");
                SubmitAllChanges();
            }
            finally
            {
                Monitor.Exit(Instance.UpdateNodeWorkerSyncRoot);
                Monitor.Exit(Instance.UpdateDefinitionWorkerSyncRoot);
                Monitor.Exit(Instance.UpdateEnrichmentsWorkerSyncRoot);
                TreeForm.taxonomyTree.TryStartWorkers();
            }
            FrmWaitScreen.HideMessage(waitKey);
            SkuTabs.Values.ForEach(egv => egv.SetStatus(DefaultStatus, WorkerState.Ready));
        }

        // Internal Methods (10) 

        internal string GetFromRegistry(string key)
        {
            if (_baseKey == null)
                InitRegistryBaseKey(false);

            if (_baseKey != null)
            {
                object value = _baseKey.GetValue(key);
                if (value != null)
                    return value.ToString();
            }

            return null;
        }

        internal IEnumerable<TaxonomyInfo> GetNodes(TaxonomyInfo parent, bool getChildren, bool getEmptyNodes)
        {
            //var nodes = new List<TaxonomyInfo>();
            //if (getEmptyNodes || GetSkuCount(parent, true) > 0)
            //    nodes.Add(parent);

            //if (getChildren)
            //    parent.ChildTaxonomyDatas.Where(td => td.Active).Select(td => td.TaxonomyInfo).ForEach(
            //        tax => nodes.AddRange(GetNodes(tax, true, getEmptyNodes)));

            //return nodes;

            if (!getChildren && (getEmptyNodes || parent.HasSkus))
            {
                return new List<TaxonomyInfo> { parent };
            }

            return parent.GetLeafTaxonomies(getEmptyNodes);

            //return getEmptyNodes ? parent.GetLeafTaxonomies() : parent.AllLeafChildren.ToList();
        }

        //internal void GetNodes(TaxonomyInfo parent,bool getChildren,b)

        internal void EmptySkuCount()
        {
            _skuCounts.Clear();
        }
        
        internal int GetSkuCount(TaxonomyInfo taxonomy, bool fetchFromDb)
        {
            if (taxonomy == null)
                return 0;

            int noOfSkus = -1;
            //if (_skuCounts.ContainsKey(taxonomy))
            if (_skuCounts.ContainsKey(taxonomy) && !fetchFromDb)
                noOfSkus = _skuCounts[taxonomy];
            else if (fetchFromDb)
            {
                if (taxonomy.NodeType == TaxonomyInfo.DerivedNodeType)
                {
                    CrossListCriteria cl = Query.FetchCrossListObject(taxonomy);
                    //FetchCrossListObject will take care of Include Children Condition
                    if (cl != null)
                        noOfSkus =
                            Query.GetFilteredSkus(
                                cl.TaxonomyIDFilter, cl.ValueFilters, cl.AttributeTypeFilters, cl.MatchAllTerms).Select(
                                    s => s.ID).Distinct().Count();
                }
                else if (taxonomy.SkuInfos != null)
                {
                    noOfSkus = taxonomy.SkuInfos.Where(s => s.Sku.SkuType == TreeForm.CurrentSkuType.ToString()).Count(si => si.Active);
                }
                UpdateSkuCount(taxonomy, noOfSkus);
            }
            return noOfSkus;
        }

        internal string GetRankedAttributesCount(TaxonomyInfo taxonomy, bool fetchFromDb)
        {
            if (taxonomy == null)
                return null;

            string rankedCount = string.Empty;
            if (_rankedAttributeCount.ContainsKey(taxonomy) && !fetchFromDb)
                rankedCount = _rankedAttributeCount[taxonomy];
            else if (fetchFromDb)
            {
                List<SchemaData> schemaDatas = (from si in taxonomy.SchemaInfos
                                                from sd in si.SchemaDatas
                                                where sd.Active && sd.InSchema
                                                select sd).ToList();
                int navCount = schemaDatas.Count(sd => sd.NavigationOrder > 0);
                int dispCount = schemaDatas.Count(sd => sd.DisplayOrder > 0);

                rankedCount = navCount == 0 && dispCount == 0 ? string.Empty : navCount + " • " + dispCount;

                UpdateRankedAttributesCount(taxonomy, rankedCount);
            }
            return rankedCount;
        }

        internal void UpdateRankedAttributesCount(TaxonomyInfo taxonomy, string rankedCount)
        {
            if (taxonomy == null)
                return;

            if (_rankedAttributeCount.ContainsKey(taxonomy))
                _rankedAttributeCount[taxonomy] = rankedCount;
            else
                _rankedAttributeCount.Add(taxonomy, rankedCount);
        }

        internal List<ColumnProperty> RefreshAttributeColumnPositions(
            List<ColumnProperty> columnProperties, Dictionary<string, Attribute> allAttributes, TaxonomyInfo taxonomy,
            bool firstTime)
        {
            Dictionary<Guid, ColumnProperty> properties =
                columnProperties.Distinct(new KeyEqualityComparer<ColumnProperty>(cp => cp.AttributeID)).ToDictionary(
                    cp => cp.AttributeID);

            UserProjectsPreferences _attributePrefs = new UserProjectsPreferences();
            var currentUserID = NatalieTools.Instance.CurrentUser.ID;
            var userProject = (from up in NatalieTools.Instance.CurrentProject.UserProjects
                               where up.UserID == currentUserID
                               select up).FirstOrDefault();
            _attributePrefs = userProject.UserProjPreferences;

            if (firstTime && (taxonomy != null))
            {
                //var InSchema = new HashSet<string>();
                //var Ranked = new HashSet<string>();
                //var Globbal = new HashSet<string>();
                //var Extended = new HashSet<string>();
                var InSchema = new HashSet<string>(from si in taxonomy.SchemaInfos
                                                   let sd = si.SchemaData
                                                   where sd != null && sd.InSchema
                                                   select si.Attribute.AttributeName);
                var allInSchemaCols = columnProperties.Where(col => InSchema.Contains(col.Attribute.AttributeName)).ToList();
                var Ranked = new HashSet<string>(from si in taxonomy.SchemaInfos
                                                 let sd = si.SchemaData
                                                 where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                 select si.Attribute.AttributeName);
                var allRankedCols = columnProperties.Where(col => Ranked.Contains(col.Attribute.AttributeName)).ToList();
                var Globbal = new HashSet<string>(from att in NatalieTools.Instance.Dc.Attributes
                                                  where att.AttributeType != null && att.AttributeType == Attribute.AttributeTypeGlobal
                                                  select att.AttributeName);
                var allGlobbalCols = columnProperties.Where(col => Globbal.Contains(col.Attribute.AttributeName)).ToList();
                var allExtendedCols = columnProperties.Where(col => !InSchema.Union(Ranked).Union(Globbal).Contains(col.Attribute.AttributeName)).ToList();

                var allAttrGroupCols = columnProperties.Where(col => _attributePrefs.AttributeGroupInclusions.Contains(col.Attribute.AttributeGroup));
                var allCustomAttrCols = columnProperties.Where(col => _attributePrefs.AttributeCustomInclusions.Contains(col.Attribute.AttributeName));
                allInSchemaCols.ForEach(col => col.Visible = (_attributePrefs.InSchemaAttributes || col.Visible));
                allRankedCols.ForEach(col => col.Visible = (_attributePrefs.RankedAttributes || col.Visible));
                allGlobbalCols.ForEach(col => col.Visible = (_attributePrefs.GlobalAttributes || col.Visible));
                allExtendedCols.ForEach(col => col.Visible = (_attributePrefs.ExtendedAttributes || col.Visible));
                allAttrGroupCols.ForEach(col => col.Visible = true);
                allCustomAttrCols.ForEach(col => col.Visible = true);
            }

            List<ColumnProperty> toAdd = (from att in allAttributes
                                          where
                                             ((att.Value.AttributeType == null ||
                                               !att.Value.AttributeType.Equals(Attribute.AttributeTypeSchemaMeta)) &&
                                              !properties.ContainsKey(att.Value.ID) &&
                                              !att.Value.AttributeName.Equals(EntityDataGridView.DefaultNewAttributeHeader))
                                          select
                                              new ColumnProperty
                                                  {
                                                      User = CurrentUser,
                                                      TaxonomyInfo =
                                                          (att.Value.AttributeType != null &&
                                                           att.Value.AttributeType.Contains(
                                                               Attribute.AttributeTypeGlobal))
                                                              ? null
                                                              : taxonomy,
                                                      Attribute = att.Value,
                                                      ColumnWidth = EntityDataGridView.DefaultColumnWidth,
                                                      Visible = !firstTime //|| (_attributePrefs.InSchemaAttributes && InSchema.Contains(att.Value.AttributeName))
                                                                           //|| (_attributePrefs.RankedAttributes && Ranked.Contains(att.Value.AttributeName))
                                                                           //|| (_attributePrefs.GlobalAttributes && Globbal.Contains(att.Value.AttributeName))
                                                  }).ToList();

            toAdd.ForEach(cp => properties.Add(cp.AttributeID, cp));

            columnProperties =
                properties.Select(cp => cp.Value).OrderBy(
                    cp => cp, new CompareColumnPropertyForDisplayOrder(taxonomy, allAttributes, true, true)).ToList();

            for (int i = 0; i < columnProperties.Count; i++)
                columnProperties[i].Position = i;

            return columnProperties;
        }

        internal void ResetSkuCount(TaxonomyInfo taxonomy)
        {
            //Clear Count Cache
            if (taxonomy == null)
            {
                _skuCounts = new Dictionary<TaxonomyInfo, int>();
                _rankedAttributeCount = new Dictionary<TaxonomyInfo, string>();
                return;
            }

            if (_skuCounts.ContainsKey(taxonomy))
                _skuCounts.Remove(taxonomy);
            if (_rankedAttributeCount.ContainsKey(taxonomy))
                _rankedAttributeCount.Remove(taxonomy);

            if (_treeForm != null && !_treeForm.IsDisposed)
                _treeForm.taxonomyTree.UpdateNodeCounts(taxonomy, true);
        }

        public void InvokeUpdateRemarkSelectionMethod()
        {
            if(RemarksForm != null)
                RemarksForm.comboBoxRemarks.SelectedItem = StickyRemark.Remark1;
        }

        internal DialogResult SaveChangesIfNecessary(bool mustSaveToContinue, bool userInitiated)
        {
            if (RemarksForm != null && StickyRemark != null)
                RemarksForm.BeginInvoke(new InvokeDelegate(InvokeUpdateRemarkSelectionMethod));

            if (Instance.CurrentUser.IsNatalieReadOnly) // && selectedProject)
            {
                return DialogResult.None;
            }

            //if (Dc.HasChanges)
            {
                DialogResult result = AutoSave || userInitiated
                                          ? DialogResult.Yes
                                          : mustSaveToContinue
                                                ? MessageBox.Show(
                                                    @"All changes must be saved now. Would you like to save your changes?",
                                                    @"Unsaved Changes", MessageBoxButtons.YesNoCancel)
                                                : DialogResult.No;
                if (result == DialogResult.Yes)
                    SaveChangesToDb();

                return result;
            }

            return DialogResult.None;
        }

        internal void SaveToRegistry(string key, string value)
        {
            if (_baseKey == null)
                InitRegistryBaseKey(true);


            if (_baseKey != null)
                _baseKey.SetValue(key, value);
        }

        internal void SubmitAllChanges()
        {
            SubmittingChangesToDb = true;
            try
            {
                Dc.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                const string ConcurrencyErrors = "ConcurrencyErrors.txt";
                TextWriter sw = new StreamWriter(ConcurrencyErrors);

                //Automerge database values for members that client has not modified.
                foreach (ObjectChangeConflict occ in Dc.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges, true);
                    sw.WriteLine(occ.Object.ToString());
                }
                sw.Close();

                Instance.BrowserForm.GotoUrl(new FileInfo(ConcurrencyErrors).FullName, "Concurrency errors");
                MessageBox.Show(
                    String.Format(
                        "{1}{0}Concurrency conflict(s) during save. Your changes win.", Environment.NewLine,
                        ex.Message));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    @"Unable to save to database. You may try to save again after some time. If you are seeing this message too many times, you should restart Natalie." +
                    Environment.NewLine + ex.Message, @"Cannot Save", MessageBoxButtons.OK);
            }
            SubmittingChangesToDb = false;
        }

        internal void UpdateSkuCount(TaxonomyInfo taxonomy, int skuCount)
        {
            if (taxonomy == null)
                return;

            if (_skuCounts.ContainsKey(taxonomy))
                _skuCounts[taxonomy] = skuCount;
            else
                _skuCounts.Add(taxonomy, skuCount);
        }

        internal void UpdateTabPageNames()
        {
            try
            {
                foreach (var keyValuePair in SkuTabs)
                    keyValuePair.Key.Text = keyValuePair.Value.CurrentTaxonomy != null
                                                ? keyValuePair.Value.CurrentTaxonomy.TaxonomyData.NodeName
                                                : String.Format("Query ({0})", keyValuePair.Key.TabIndex);
            }
            catch // Don't bother if this blows up - nothing important going on around here
            {
            }
        }

        #endregion Methods

        #region Nested type: StatusType

        internal enum StatusType
        {
            Default,
            Filter
        }

        #endregion
    }
}