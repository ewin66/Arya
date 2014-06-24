namespace Arya.Framework4.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using Data;
    using HelperForms;
    using LinqKit;
    using UserControls;
    using Arya.Framework.Data.AryaDb;
    using Arya.HelperClasses;

    public class Forms
    {
        private FrmAttributeFarm _attributeFarmView;
        private FrmAttributeView _attributeView;
        private FrmBrowser _browserForm;
        private FrmBuildView _buildForm;
        private FrmCharacterMap _characterMapForm;
        private FrmCheckpoint _checkpointForm;
        private FrmCloneOptions _cloneOptionsForm;
        private FrmColorRules _colorForm;
        private FrmCreateSkuGroup _createSkuGroupsForm;
        private FrmListOfValues _listofValuesForm;
        private FrmNotes _notesForm;
        private FrmQueryView _queryForm;
        private FrmSchemaView _schemaForm;
        private FrmSkuView _skuForm;
        private FrmSkuGroups _skuGroupsForm;
        private FrmSkuLinks _skuLinksForm;
        private FrmSkuLinksView _skuLinksView;
        private FrmSelectProject _startupForm;
        private FrmTree _treeForm;
        private FrmUnitOfMeasure _uomForm;
        private FrmWorkflow _workflowForm;
        private FrmFilter _filterForm;
        private FrmMetaAttributeView _metaAttributeForm;
        private FrmSpellCheck _spellCheckForm;

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


        internal Dictionary<TabPage, SchemaDataGridView> SchemaTabs=new Dictionary<TabPage, SchemaDataGridView>();
        internal Dictionary<Guid, List<Guid>> SkuOrders=new Dictionary<Guid, List<Guid>>();
        internal Dictionary<TabPage, EntityDataGridView> SkuTabs=new Dictionary<TabPage, EntityDataGridView>();
        internal FrmSpellCheckCustomDictionary _customDictioanryForm;
        

        internal FrmFilter FilterForm
        {
            get
            {
                if (_filterForm == null || _filterForm.IsDisposed)
                    _filterForm = new FrmFilter();

                return _filterForm;
            }
        }
        internal FrmSpellCheck SpellCheckForm
        {
            get
            {
                if (_spellCheckForm == null || _spellCheckForm.IsDisposed)
                    _spellCheckForm = new FrmSpellCheck();

                return _spellCheckForm;
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

        internal FrmSelectProject StartupForm
        {
            get
            {
                if (_startupForm == null || _startupForm.IsDisposed)
                    _startupForm = new FrmSelectProject(true);

                return _startupForm;
            }
        }

        internal FrmUnitOfMeasure UomForm
        {
            get
            {
                if (_uomForm == null || _uomForm.IsDisposed)
                {
                    _uomForm = new FrmUnitOfMeasure
                                   {
                                       Left = TreeForm.Right,
                                       Top = TreeForm.Top,
                                       Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                       Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                       WindowState = FormWindowState.Normal
                                   };
                }

                return _uomForm;
            }
        }

        internal FrmRemark RemarksForm { get; set; }

        //{
        //    get
        //    {
        //        if (_remarksForm == null || _remarksForm.IsDisposed)
        //            _remarksForm = new FrmRemark();
        //        return _remarksForm;
        //    }
        //}


        internal FrmAttributeFarm AttributeFarmView
        {
            get
            {
                if (_attributeFarmView == null || _attributeFarmView.IsDisposed)
                {
                    var workingArea = Screen.FromControl(TreeForm).WorkingArea;
                    _attributeFarmView = new FrmAttributeFarm
                                             {
                                                 Left = TreeForm.Right,
                                                 Top = 0,
                                                 Height = workingArea.Height,
                                                 Width = workingArea.Width/3
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
                    var workingArea = Screen.FromControl(TreeForm).WorkingArea;
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
                {
                    _workflowForm = new FrmWorkflow
                                        {
//Left = TreeForm.Right,
                                            //Top = TreeForm.Top,
                                            //Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                            //Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                            //WindowState = FormWindowState.Normal
                                        };
                }
                return _workflowForm;
            }
        }

        internal FrmNotes NotesForm
        {
            get
            {
                if (_notesForm == null || _notesForm.IsDisposed)
                    _notesForm = new FrmNotes {WindowState = FormWindowState.Normal};
                return _notesForm;
            }
        }


        internal FrmCharacterMap CharacterMapForm
        {
            get
            {
                if (_characterMapForm == null || _characterMapForm.IsDisposed)
                    _characterMapForm = new FrmCharacterMap();
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

        public FrmListOfValues ListofValuesForm
        {
            get
            {
                var workingArea = Screen.FromControl(TreeForm).WorkingArea;
                if (_listofValuesForm == null || _listofValuesForm.IsDisposed)
                {
                    _listofValuesForm = new FrmListOfValues
                                            {
                                                Left = workingArea.Right - (workingArea.Width/3),
                                                Top = 0,
                                                Height = workingArea.Height,
                                                Width = workingArea.Width/3
                                            };
                }

                return _listofValuesForm;
            }
        }


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
                {
                    _schemaForm = new FrmSchemaView
                                      {
                                          Left = TreeForm.Right,
                                          Top = TreeForm.Top,
                                          Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                          Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                          WindowState = FormWindowState.Normal
                                      };
                }

                return _schemaForm;
            }
        }


        public FrmBuildView BuildForm
        {
            get
            {
                if (_buildForm == null || _buildForm.IsDisposed)
                {
                    _buildForm = new FrmBuildView
                                     {
                                         Left = TreeForm.Right,
                                         Top = TreeForm.Top,
                                         Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                         Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                         WindowState = FormWindowState.Maximized
                                     };
                }

                return _buildForm;
            }
        }

        internal FrmSkuView SkuForm
        {
            get
            {
                if (_skuForm == null || _skuForm.IsDisposed)
                {
                    _skuForm = new FrmSkuView
                                   {
                                       Left = TreeForm.Right,
                                       Top = TreeForm.Top,
                                       Width = Screen.FromControl(TreeForm).WorkingArea.Width - TreeForm.Width,
                                       Height = Screen.FromControl(TreeForm).WorkingArea.Height,
                                       WindowState = FormWindowState.Maximized
                                   };
                }

                return _skuForm;
            }
        }


        internal FrmCreateSkuGroup CreateSkuGroupForm
        {
            get
            {
                if (_createSkuGroupsForm == null || _createSkuGroupsForm.IsDisposed)
                    _createSkuGroupsForm = new FrmCreateSkuGroup {WindowState = FormWindowState.Normal};
                return _createSkuGroupsForm;
            }
        }


        internal FrmSkuGroups SkuGroupsForm
        {
            get
            {
                if (_skuGroupsForm == null || _skuGroupsForm.IsDisposed)
                    _skuGroupsForm = new FrmSkuGroups {WindowState = FormWindowState.Normal};
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
                {
                    _treeForm = new FrmTree(null)
                                    {
                                        Left = Screen.PrimaryScreen.WorkingArea.Left,
                                        Top = Screen.PrimaryScreen.WorkingArea.Top
                                    };
                }

                return _treeForm;
            }
        }

        internal FrmMetaAttributeView MetaAttributeForm
        {
            get
            {
                if (_metaAttributeForm == null || _metaAttributeForm.IsDisposed)
                { 
                    var workingArea = Screen.FromControl(TreeForm).WorkingArea; 
                    _metaAttributeForm = new FrmMetaAttributeView 
                                            {
                                                Left = TreeForm.Right,
                                                Top = TreeForm.Top,
                                                Height = workingArea.Height,
                                                Width = workingArea.Width/2,
                                                WindowState = FormWindowState.Normal
                                            };
                }
                return _metaAttributeForm;
            }
        }
        internal FrmSpellCheckCustomDictionary CustomDictionaryForm
        {
            get
            {
                if (_customDictioanryForm == null || _customDictioanryForm.IsDisposed)
                {
                    var workingArea = Screen.FromControl(TreeForm).WorkingArea;
                    _customDictioanryForm = new FrmSpellCheckCustomDictionary
                    {
                        Left = TreeForm.Right,
                        Top = TreeForm.Top,
                        Height = workingArea.Height,
                        Width = workingArea.Width / 2,
                        WindowState = FormWindowState.Normal
                    };
                }
                return _customDictioanryForm;
            }
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

        public void CloseAllForms()
        {
            //Close All Forms except Tree
            GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(
                field => field == typeof (Form)).Select(field => field.GetValue(this) as Form).Where(
                    form => form != null && !form.IsDisposed).ForEach(form =>
                                                                          {
                                                                              if (form != _treeForm)
                                                                                  form.Close();
                                                                          });
        }


        public void UpdateNodeCounts(Arya.Data.TaxonomyInfo taxonomy)
        {
            if (_treeForm != null && !_treeForm.IsDisposed)
                _treeForm.taxonomyTree.UpdateNodeCounts(taxonomy, true);
        }
    }
}