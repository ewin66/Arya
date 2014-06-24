using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.Properties;
using System.ComponentModel;

namespace Arya
{
    public partial class FrmUnitOfMeasure : Form
    {
        #region Fields

        private const string ProjectUomColumnHeader1 = "Project Unit of Measure";
        private const string ProjectUomColumnHeader2 = "Global Unit of Measure";

        private readonly DataGridViewCheckBoxCell _defaultdgvCheckBox = new DataGridViewCheckBoxCell();
        //private readonly DataGridViewTextBoxCell _defaultdgvTextCell = new DataGridViewTextBoxCell();
        private DataGridViewTextBoxCell _defaultdgvTextCell = new DataGridViewTextBoxCell();

        private readonly UnitOfMeasure _emptyUom = new UnitOfMeasure
                                                   {
                                                       UnitName = string.Empty,
                                                       UnitAbbreviation = "base"
                                                   };

        private readonly Dictionary<string, string> projectUomColumnHeaderPropertyNameMap =
            new Dictionary<string, string>
            {
                {ProjectUomColumnHeader1, "Uom"},
                {ProjectUomColumnHeader2, "UnitOfMeasure"}
            };

        private List<UnitOfMeasure> _allUoms;
        private List<UnitOfMeasure> _baseUoms;
        private string _lastSortedAscendingBy;
        private List<ProjectUom> _projectUoms;
        private int flag = 0;

        BindingSource data = new BindingSource();

        #endregion Fields

        #region Constructors

        public FrmUnitOfMeasure()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
         }

        #endregion Constructors

        #region Methods

        private static void AddDgvColumn(DataGridView dgv, DataGridViewCell cellType, string headerText)
        {
            dgv.Columns.Add(new DataGridViewColumn(cellType) {HeaderText = headerText});
        }

        private void deleteGlobalUomToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DeleteFromGlobalUom();
        }

        private void DeleteFromGlobalUom()
        {
            if (dgvUnitOfMeasure.CurrentCell.RowIndex != -1 && dgvUnitOfMeasure.CurrentCell.RowIndex < _allUoms.Count)
            {
                var uom = _allUoms[dgvUnitOfMeasure.CurrentCell.RowIndex];
               
                string message = "Are you sure you want to delete the selected Uom?";
                string caption = "Delete Global Uom";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Please make sure that the Unit of Measure you are deleting is NOT being used as a reference in the BaseUoms List");
                    if ((_projectUoms.Where(u => u.UomID == (uom.ID))).Count() > 0)
                    {
                        MessageBox.Show("This will also delete the corresponding entry from the Project Units of Measure");
                        _projectUoms.Remove(uom.ProjectUoms.Single());
                        AryaTools.Instance.InstanceData.Dc.ProjectUoms.DeleteOnSubmit(uom.ProjectUoms.Single());
                    }

                    data.Remove(uom);
                    _allUoms.Remove(uom);
                    if (_baseUoms.Contains(uom))
                    {
                        _baseUoms.Remove(uom);
                    }
                    AryaTools.Instance.InstanceData.Dc.UnitOfMeasures.DeleteOnSubmit(uom);
                    AryaTools.Instance.SaveChangesIfNecessary(false, false);

                    dgvUnitOfMeasure.RowCount = _allUoms.Count + 1;
                    dgvUnitOfMeasure.Invalidate();
                }
              
            }
            else
            {
                MessageBox.Show("There is no data to delete");
                return;
            }
        }
        private void deleteProjectUomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFromProjectUom();
        }

        private void DeleteFromProjectUom()
        {
            if (dgvProjectUom.CurrentCell.RowIndex != -1 && dgvProjectUom.CurrentCell.RowIndex < _projectUoms.Count)
            {
                var uom = _projectUoms[dgvProjectUom.CurrentCell.RowIndex];
                string message = "Are you sure you want to delete the selected Uom?";
                string caption = "Delete Project Uom";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.Yes)
                {
                    _projectUoms.Remove(uom);
                    AryaTools.Instance.InstanceData.Dc.ProjectUoms.DeleteOnSubmit(uom);
                    AryaTools.Instance.SaveChangesIfNecessary(false, false);

                    dgvProjectUom.RowCount = _projectUoms.Count + 1;
                    dgvProjectUom.Invalidate();
                }
            }

            else
            {
                MessageBox.Show("There is no data to delete");
                return;
            }

        }

        private void dgvProjectUom_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= _projectUoms.Count)
                return;

            var uom = _projectUoms[e.RowIndex];
            
            var headerText = dgvProjectUom.Columns[e.ColumnIndex].HeaderText;
            var pi = uom.GetType().GetProperty(projectUomColumnHeaderPropertyNameMap[headerText]);
            e.Value = pi.GetValue(uom);

          }

        private void dgvProjectUom_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()) || e.RowIndex > _projectUoms.Count)
                return;

            if (e.RowIndex == _projectUoms.Count)
            {
                if (e.ColumnIndex != 0)
                    return; //first, we need to know which Unit Of Measure this is for

                var tempUom = _allUoms.FirstOrDefault();
                if (tempUom == null)
                {
                    MessageBox.Show(
                        "No Global Unit of Measure defined. Please define at least one Global Unit of Measure and try again.");
                    return;
                }

                var newUom = new ProjectUom
                             {
                                 Project = AryaTools.Instance.InstanceData.CurrentProject,
                                 UnitOfMeasure = _allUoms.FirstOrDefault()
                             };

                _projectUoms.Add(newUom);

               
            AryaTools.Instance.InstanceData.Dc.ProjectUoms.InsertOnSubmit(newUom);
            }
            var uom = _projectUoms[e.RowIndex];
            //var headerText = dgvProjectUom.Columns[e.ColumnIndex].HeaderText.Replace(" ", string.Empty);
            //var pi = uom.GetType().GetProperty(headerText);
            var headerText = dgvProjectUom.Columns[e.ColumnIndex].HeaderText;
            var pi = uom.GetType().GetProperty(projectUomColumnHeaderPropertyNameMap[headerText]);

            pi.SetValue(uom, e.Value);

            if (string.IsNullOrWhiteSpace(_projectUoms[e.RowIndex].Uom))
                _projectUoms[e.RowIndex].Uom = _projectUoms[e.RowIndex].UnitOfMeasure.UnitAbbreviation;

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            dgvProjectUom.RowCount = _projectUoms.Count + 1;
            dgvProjectUom.Invalidate();
        }

        private void dgvProjectUom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var headerText = dgvProjectUom.Columns[e.ColumnIndex].HeaderText;
            _projectUoms = CollectionExtensions.SortList(_projectUoms, projectUomColumnHeaderPropertyNameMap[headerText],
                ref _lastSortedAscendingBy);
            dgvProjectUom.Invalidate();
        }

        private void tcUomTabs_SelectedIndexChanged(object sender, EventArgs e )
        {
            if (tcUomTabs.SelectedTab == tpProjectUom)
            {
                flag++;
                if (flag == 1)
                {
                    MessageBox.Show("There could be some Base Units of Measure that are missing from the current tab. Please add them from The Global Units of Measure tab and proceed.");
                }
                _allUoms.Clear();
                _allUoms = (from uom in AryaTools.Instance.InstanceData.Dc.UnitOfMeasures.ToList()
                let baseUom = (uom.BaseUom ?? uom).ToString()
                let hasBase = uom.BaseUom != null
                orderby baseUom, hasBase, uom.UnitName
                            select uom).ToList();

                data.ResetBindings(true);
                data.DataSource = _allUoms.Select(u => new { value = u, display = u.ToString() }).Distinct().OrderBy(u => u.display).ToList();
             
                _projectUoms = (from uom in AryaTools.Instance.InstanceData.CurrentProject.ProjectUoms.ToList()
                                let order = _allUoms.IndexOf(uom.UnitOfMeasure)
                                orderby order
                                select uom).ToList();

                DataGridViewComboBoxCell projectUomdgvComboCell = new DataGridViewComboBoxCell();
                projectUomdgvComboCell.DataSource = data;
                  
                projectUomdgvComboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                projectUomdgvComboCell.ValueMember = "value";
                projectUomdgvComboCell.DisplayMember = "display";
               
            }
        }

        private void dgvUnitOfMeasure_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= _allUoms.Count)
            {
                if (e.ColumnIndex == 0)
                    e.Value = "*";

                if (e.ColumnIndex == 6)
                    e.Value = false;
                return;
            }

            var headerText = dgvUnitOfMeasure.Columns[e.ColumnIndex].HeaderText.Replace(" ", string.Empty);
            var uom = _allUoms[e.RowIndex];
            var pi = uom.GetType().GetProperty(headerText);
            var value = pi.GetValue(uom);
           
            if (headerText == "BaseUom")
            {
                if (value == null)
                    value = _emptyUom;

                if (!_baseUoms.Contains(value))
                {
                    uom.BaseUom = null;
                    value = _emptyUom;
                }
            }
            e.Value = value;
   
        }

        private void dgvUnitOfMeasure_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()) || e.RowIndex > _allUoms.Count)
                return;

            if (e.RowIndex == _allUoms.Count)
            {
                var newUom = new UnitOfMeasure();
                _allUoms.Add(newUom);
                _baseUoms.Add(newUom);

                AryaTools.Instance.InstanceData.Dc.UnitOfMeasures.InsertOnSubmit(newUom);
            }
            var uom = _allUoms[e.RowIndex];
            var headerText = dgvUnitOfMeasure.Columns[e.ColumnIndex].HeaderText.Replace(" ", string.Empty);
            var pi = uom.GetType().GetProperty(headerText);

            if (pi.PropertyType == typeof (Int32?)) //Precision is integer
            {
                int val;
                if (int.TryParse(e.Value.ToString(), out val))
                    pi.SetValue(uom, val);
            }
            else if (headerText == "BaseUom")
            {
                if (e.Value.ToString() == _emptyUom.ToString())
                {
                    //set as base uom
                    pi.SetValue(uom, null);
                    //add to base uom list if not present in there
                    if (!_baseUoms.Contains(uom))
                        _baseUoms.Add(uom);
                }
                else
                {
                    var currentBaseUom = _baseUoms.First(bu => bu.ToString() == e.Value.ToString());
                    pi.SetValue(uom, currentBaseUom);
                    if (_baseUoms.Contains(uom))
                        _baseUoms.Remove(uom);
                }

                dgvUnitOfMeasure.InvalidateRow(e.RowIndex);
            }
            else if (headerText == "IsBaseUom" && Convert.ToBoolean(e.Value))
            {
                pi = uom.GetType().GetProperty("BaseUom");
                pi.SetValue(uom, null);
                //dgvUnitOfMeasure.Rows[e.RowIndex].Cells[3].ReadOnly = true;
                dgvUnitOfMeasure.InvalidateRow(e.RowIndex);
            }
            else if (headerText == "IsBaseUom" && !Convert.ToBoolean(e.Value))
            {
                pi = uom.GetType().GetProperty("BaseUom");
                pi.SetValue(uom, _baseUoms.ElementAt(1));
                //dgvUnitOfMeasure.Rows[e.RowIndex].Cells[3].ReadOnly = false;
                dgvUnitOfMeasure.InvalidateRow(e.RowIndex);
            }
            else
                pi.SetValue(uom, e.Value);

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            dgvUnitOfMeasure.RowCount = _allUoms.Count + 1;
        }

      
        private void dgvUnitOfMeasure_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _allUoms = CollectionExtensions.SortList(_allUoms, dgvUnitOfMeasure.Columns[e.ColumnIndex].HeaderText,
                ref _lastSortedAscendingBy);
            dgvUnitOfMeasure.Invalidate();
        }

        private void ErrorHandle(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.RowIndex + Environment.NewLine + e.ColumnIndex + Environment.NewLine + e.Exception);
        }

        private void FrmUnitOfMeasure_Load(object sender, EventArgs e)
        {
            InitGlobalUom();
            InitProjectUom();
            tpProjectUom.Text = AryaTools.Instance.InstanceData.CurrentProject.ClientDescription + " Project Units";
        }

        private void InitGlobalUom()
        {
            // dgvUnitOfMeasure.ReadOnly = !AryaTools.Instance.InstanceData.CurrentUser.IsAdmin;

            dgvUnitOfMeasure.ReadOnly = !AryaTools.Instance.InstanceData.CurrentUser.IsProjectAdmin;
      
            _allUoms = (from uom in AryaTools.Instance.InstanceData.Dc.UnitOfMeasures.ToList()
                let baseUom = (uom.BaseUom ?? uom).ToString()
                let hasBase = uom.BaseUom != null
                orderby baseUom, hasBase, uom.UnitName
                select uom).ToList();

            data.DataSource = _allUoms;

            _baseUoms = (from u in _allUoms where u.BaseUnit == null select u).ToList();
            _baseUoms.Insert(0, _emptyUom);
            var baseUomdgvComboCell = new DataGridViewComboBoxCell {DataSource = _baseUoms};

            //baseUomdgvComboCell.se.SelectedIndexChanged +=
            //new System.EventHandler(ComboBox1_SelectedIndexChanged);
            dgvUnitOfMeasure.Columns.Clear();
            //AddDgvColumn(dgvUnitOfMeasure, _defaultdgvTextCell, "ID");

            AddDgvColumn(dgvUnitOfMeasure, _defaultdgvTextCell, "Unit Name");
            AddDgvColumn(dgvUnitOfMeasure, _defaultdgvTextCell, "Unit Abbreviation");
            AddDgvColumn(dgvUnitOfMeasure, baseUomdgvComboCell, "Base Uom");
            AddDgvColumn(dgvUnitOfMeasure, _defaultdgvTextCell, "From Base Expression");
            AddDgvColumn(dgvUnitOfMeasure, _defaultdgvTextCell, "To Base Expression");
            AddDgvColumn(dgvUnitOfMeasure, _defaultdgvTextCell, "Precision");
            AddDgvColumn(dgvUnitOfMeasure, _defaultdgvCheckBox, "Is Base Uom");

            dgvUnitOfMeasure.RowCount = _allUoms.Count + 1;
        }

        private void InitProjectUom()
        {
            dgvProjectUom.AllowUserToDeleteRows = true;
            _allUoms.Clear();

            _allUoms = (from uom in AryaTools.Instance.InstanceData.Dc.UnitOfMeasures.ToList()
                        let baseUom = (uom.BaseUom ?? uom).ToString()
                        let hasBase = uom.BaseUom != null
                        orderby baseUom, hasBase, uom.UnitName
                        select uom).ToList();

            data.ResetBindings(true);
            data.DataSource = _allUoms.Select(u => new { value = u, display = u.ToString() }).Distinct().OrderBy(u => u.display).ToList();

            _projectUoms = (from uom in AryaTools.Instance.InstanceData.CurrentProject.ProjectUoms.ToList()
                            let order = _allUoms.IndexOf(uom.UnitOfMeasure)
                            orderby order
                            select uom).ToList();
            
            DataGridViewComboBoxCell projectUomdgvComboCell = new DataGridViewComboBoxCell();
            projectUomdgvComboCell.DataSource = data;

            projectUomdgvComboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            projectUomdgvComboCell.ValueMember = "value";
            projectUomdgvComboCell.DisplayMember = "display";

            dgvProjectUom.Columns.Clear();

          
            //AddDgvColumn(dgvProjectUom, _defaultdgvTextCell, "ID");
            //AddDgvColumn(dgvProjectUom, projectUomdgvComboCell, "Unit Of Measure");
            //AddDgvColumn(dgvProjectUom, _defaultdgvTextCell, "Uom");

            AddDgvColumn(dgvProjectUom, _defaultdgvTextCell, ProjectUomColumnHeader1);
            AddDgvColumn(dgvProjectUom, projectUomdgvComboCell, ProjectUomColumnHeader2);

            dgvProjectUom.RowCount = _projectUoms.Count + 1;
        }

        #endregion Methods
    }
}