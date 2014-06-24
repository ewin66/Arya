using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;
using Attribute = Arya.Data.Attribute;

namespace Arya
{
    public partial class FrmWorkflow : Form
    {
        List<Workflow> _dataSource = new List<Workflow>();
        
        public FrmWorkflow()
        {
            InitializeComponent();
            InitWorkflow();
           
        }

        private void InitWorkflow()
        {
            _dataSource = AryaTools.Instance.InstanceData.Dc.Workflows.ToList();
            dataGridView1.DataSource =
                _dataSource.Select(a => new {Name = a.WorkflowName, CreatedBy = a.User.FullName, a.CreatedOn}).ToList();

            var col = new DataGridViewButtonColumn { Text = "Run" };
            col.HeaderText = "Run";
            col.Width = 50;
            dataGridView1.Columns.Add(col);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK )
                return;
            txtBoxXmldump.Clear();
            txtInputFilename.Text = openFileDialog.FileName;
            using (var sr = new StreamReader(txtInputFilename.Text))
            {
                txtBoxXmldump.Text = sr.ReadToEnd();

            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            
            var xl = XElement.Load(openFileDialog.FileName);
            var wf = xl.DeSerializeWorkflow();
            CreateWorkflow(wf);
                

        }

        private void CreateWorkflow(WorkFlow wf)
        {
            if (AryaTools.Instance.InstanceData.Dc.Workflows.Any(w => w.WorkflowName == wf.name))
            {
                var dlg = MessageBox.Show(wf.name + " already exists, do you want to overwrite?", "Workflow",
                                          MessageBoxButtons.YesNo);
                if (dlg == DialogResult.Yes)
                {
                    var existingWf = AryaTools.Instance.InstanceData.Dc.Workflows.SingleOrDefault(w => w.WorkflowName == wf.name);
                    if (existingWf != null) existingWf.Source = wf.SerializeToXElement();
                    AryaTools.Instance.SaveChangesIfNecessary(true, true);
                }
            }
            else
            {
                var workflow = new Workflow
                                   {
                                       ProjectID = AryaTools.Instance.InstanceData.CurrentProject.ID,
                                       WorkflowName = wf.name,
                                       CreatedOn = DateTime.Now,
                                       CreatedBy = AryaTools.Instance.InstanceData.CurrentUser.ID,
                                       Source = wf.SerializeToXElement()
                                   };

                
                var workflowGroup = new Group()
                {
                    Name = wf.name,
                    Description = "Workflow generated group",
//                    ClientName = AryaTools.Instance.InstanceData.CurrentProject.ProjectName,
                    GroupType = Group.USER_GROUP_WORKFLOW
                };

                workflowGroup.Workflows.Add(workflow);

                var workflowAttribute = new Arya.Data.Attribute
                                    { 
                                        ProjectID = AryaTools.Instance.InstanceData.CurrentProject.ID, 
                                        AttributeType = "Workflow",
                                        AttributeName = wf.name,
                                        Group = "Workflow Group", 
                                        Readonly = true,CreatedOn = DateTime.Now, 
                                        CreatedBy = AryaTools.Instance.InstanceData.CurrentUser.ID
                                    };

                //Attribute.GetAttributeFromName(wf.name)

                AryaTools.Instance.InstanceData.Dc.Attributes.InsertOnSubmit(workflowAttribute);
                CreateStates(wf, workflow);
                
                AryaTools.Instance.InstanceData.Dc.Groups.InsertOnSubmit(workflowGroup);
                AryaTools.Instance.SaveChangesIfNecessary(true, true);
                InitWorkflow();
                //dataGridView1.DataSource = AryaTools.Instance.InstanceData.Dc.Workflows.Select(a => new { Name = a.WorkflowName, CreatedBy = a.User.FullName, a.CreatedOn });
                dataGridView1.Invalidate();
            }
        }

        private void CreateStates(WorkFlow wf, Workflow workflow)
        {
            foreach (var n in wf.state)
            {

                var x = new State
                            {
                                Name = n.name,
                                Description = n.state_description,
                                Action = n.action.SerializeToXElement(),
                                Trigger = n.trigger,
                                UserGroupID = n.user_group == "" ? (Guid?) null : new Guid(n.user_group),
                                IsFirst = n.id == "1"?true:false
                            };
                workflow.States.Add(x);

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtInputFilename.Text = saveFileDialog.FileName;
                    using (var outfile = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        outfile.Write(txtBoxXmldump.Text);
                    }
                }
            }
            else
            {

                using (var outfile = new StreamWriter(openFileDialog.FileName, false))
                {
                    outfile.Write(txtBoxXmldump.Text);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtInputFilename.Text = string.Empty;
            txtBoxXmldump.Text = _dataSource[e.RowIndex].Source.ToString();

            //using (var sr = new StreamReader(_dataSource[e.RowIndex].Source.ToString()))
            //{
            //    txtBoxXmldump.Text = sr.ReadToEnd();

            //}
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int colIndex = e.ColumnIndex;
            if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "Schedule")
            {
                var value = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
                var nextValue = GetNextValue((string)value);
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = nextValue;

            }

            if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "Run")
            {
                try
                {
                    var workflowName = _dataSource[e.RowIndex].WorkflowName;
                }
                catch
                {
                    MessageBox.Show("Cannot find background promoter on your machine.");
                }
            }
        }

        private string GetNextValue(string value)
        {
            if (value == "") return "Daily";
            if (value == "Daily") return "Weekly";
            if (value == "Weekly") return "Bi-Weekly";
            if (value == "Bi-Weekly") return "Monthly";
            if (value == "Monthly") return "";

            return "Daily";
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
        }

       



        
      
    }
}
