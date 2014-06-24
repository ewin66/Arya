using Arya.Data;
using Arya.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arya
{
    public partial class FrmSpellCheckCustomDictionary : Form
    {
        private List<CustomDictionaryWrapper> _formDataSource;
        private ICollection<String> _dicktionaryWord;
        public List<CustomDictionaryWrapper> FormDataSource 
        {
            get
            {               
                _formDataSource =  new List<CustomDictionaryWrapper>();
                _formDataSource.Add(new CustomDictionaryWrapper(0, new ProjectDictionary() { Word = "Add new value here" }));
                var source = AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.ToList().OrderBy(tt => tt.Word);
                for (int i = 0; i < source.Count(); i++)
                {
                    _formDataSource.Add(new CustomDictionaryWrapper(i + 1, source.ToList()[i]));
                }
                
                return _formDataSource; 
            }
            set 
            {
                _formDataSource = value;
            }           
        }
        public ICollection<String> DictionaryWords 
        {
            get
            {
                if (_dicktionaryWord == null)
                {
                    _dicktionaryWord = AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.Select(pd => pd.Word).ToList();
                }
                return _dicktionaryWord; 
            }
            set
            {
                _dicktionaryWord = value;
            }
        }
        public FrmSpellCheckCustomDictionary( )
        {
            InitializeComponent();
            bindingSource1.DataSource = FormDataSource;
            lblAddWordStatus.Text = "";
              
        }

        private void dgvDicktionary_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!String.IsNullOrEmpty((string)dgvDicktionary.CurrentCell.Value))
            {
                if (DictionaryWords.FirstOrDefault(fds => fds == dgvDicktionary.CurrentCell.Value.ToString()) == null)
                {
                    var projectDictionaryWord = new ProjectDictionary() { Word = dgvDicktionary.CurrentCell.Value.ToString() };
                    AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.InsertOnSubmit(projectDictionaryWord);
                    DictionaryWords.Add(dgvDicktionary.CurrentCell.Value.ToString());
                    AryaTools.Instance.SaveChangesIfNecessary(false, false);
                    lblAddWordStatus.Text = dgvDicktionary.CurrentCell.Value.ToString() + " added to the dictionary";
                }
                else
                {
                    lblAddWordStatus.Text = dgvDicktionary.CurrentCell.Value.ToString() + " already exist in the dictionary";
                }
            }
                     
            bindingSource1.DataSource = FormDataSource;
                  
        }

        private void dgvDicktionary_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //only enable edit to its last cell
            if (dgvDicktionary.CurrentCell.RowIndex == 0 && dgvDicktionary.CurrentCell.ColumnIndex == 1)
            {                
                DataGridViewCell cell = dgvDicktionary[1,0];
                cell.ReadOnly = false;
                dgvDicktionary.CurrentCell = cell;
                dgvDicktionary.BeginEdit(true);
            }
        }

        private void dgvDicktionary_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDicktionary.CurrentCell.RowIndex == 0 && dgvDicktionary.CurrentCell.ColumnIndex == 1)
            {
                string currentValue = dgvDicktionary.CurrentCell.Value.ToString();
                if (!String.IsNullOrEmpty(currentValue) && FormDataSource.FirstOrDefault(fds => fds.Value.ToLower() == currentValue.ToLower()) == null)
                {
                    var projectDictionaryWord = new ProjectDictionary() { Word = dgvDicktionary.CurrentCell.Value.ToString() };
                    AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.InsertOnSubmit(projectDictionaryWord);
                    dgvDicktionary.Invalidate();
                }
            }
        }

        private void dgvDicktionary_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDicktionary.CurrentCell != null)
            {
                btnDelete.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDicktionary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedRows();
            }
        }

        private void DeleteSelectedRows()
        {
            foreach (DataGridViewCell item in dgvDicktionary.SelectedCells)
            {
                ProjectDictionary word = AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.FirstOrDefault(pd => pd.Word == item.Value.ToString());
                if (word != null)
                {
                    AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.DeleteOnSubmit(word);
                }                
            }
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            bindingSource1.DataSource = FormDataSource;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedRows();
        }
    }
    public class CustomDictionaryWrapper 
    {
        private ProjectDictionary _customDictionaryObject;
        private string _value;
        public int Index { get; set; }
        public string Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                //if (!String.IsNullOrEmpty(_value) && AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.FirstOrDefault(fds => fds.Word.ToLower() == _value.ToLower()) == null)
                //{
                    
                //    var projectDictionaryWord = new ProjectDictionary() { Word = _value };
                //    AryaTools.Instance.InstanceData.Dc.ProjectDictionaries.InsertOnSubmit(projectDictionaryWord);
                    
                //}  
            }
        }


        public CustomDictionaryWrapper(int index, ProjectDictionary value)
        {
            Index = index;
            _customDictionaryObject = value;
            Value = _customDictionaryObject.Word;
        }


        
    }
}
