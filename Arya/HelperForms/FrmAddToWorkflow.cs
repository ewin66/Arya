using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya.HelperForms
{
    public partial class FrmAddToWorkflow : Form
    {
        private readonly List<Sku> _skuList;
        private readonly List<Group> _workflowGroups = new List<Group>();
        private Workflow _selectedWorkflow;

        private FrmAddToWorkflow()
        {
            InitializeComponent();
            Icon = Resources.AryaLogoIcon;

            _workflowGroups =
                AryaTools.Instance.InstanceData.Dc.Groups.Where(g => g.GroupType == Group.USER_GROUP_WORKFLOW)
                    .ToList();
        }

        public FrmAddToWorkflow(List<Sku> list) : this()
        {
            _skuList = list;
            lblSelection.Text = _skuList.Count() + " Sku(s) selected";
        }

        private State FirstState
        {
            get { return _selectedWorkflow.States.Single(s => s.IsFirst == true); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ProcessSkuGroup();
            AryaTools.Instance.SaveChangesIfNecessary(false, false);
        }

        private void ProcessSkuGroup()
        {
            if (_workflowGroups.Single(d => d.Name == _selectedWorkflow.WorkflowName).SkuGroups.Any())
            {
                var dlgResult =
                    MessageBox.Show(
                        "Existing Skus will remain as is and new Skus will be added to '"
                        + _selectedWorkflow.WorkflowName
                        + "'. New Skus will be assigned first state, do you want to continue?", "Workflow",
                        MessageBoxButtons.YesNo);
                if (dlgResult == DialogResult.Yes)
                    AddSkusToExistingGroup(_selectedWorkflow.WorkflowName);
            }
            else
                AddSkusToExistingGroup(_selectedWorkflow.WorkflowName);

            MessageBox.Show("Skus have been added to " + _selectedWorkflow.WorkflowName, "Workflow");
            Close();
        }

        private void AddSkusToExistingGroup(string groupName)
        {
            var existingGroup = _workflowGroups.FirstOrDefault(g => g.Name == groupName);
            if (existingGroup != null)
                AddSkusToGroup(existingGroup);
        }

        private void AddSkusToGroup(Group addToGroup)
        {
            foreach (var s in _skuList)
            {
                if (!(addToGroup.SkuGroups.Any(a => a.Active && a.SkuID == s.ID)))
                {
                    addToGroup.SkuGroups.Add(new SkuGroup() {Sku = s, Active = true});
                    addToGroup.SkuStates.Add(new SkuState()
                                             {
                                                 SkuGroupID = addToGroup.ID,
                                                 Sku = s,
                                                 State = FirstState,
                                                 ArrivalDate = DateTime.Now,
                                                 Active = true
                                             });
                }
            }

            // _skuList.ForEach(s => addToGroup.SkuGroups.Add(new SkuGroup() { Sku = s, Active = true }));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedWorkflow = (Workflow) cboWorkflows.SelectedItem;
        }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private void FrmAddToWorkflow_Load(object sender, EventArgs e)
        {
            var projectWorkflows = AryaTools.Instance.InstanceData.Dc.Workflows.ToList();
            workflowBindingSource.DataSource = projectWorkflows;

            if (projectWorkflows.Count > 0)
                _selectedWorkflow = projectWorkflows[0];
            else
            {
                var result = MessageBox.Show("No workflows have been defined. Do you want to create one now?",
                    "No workflows", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    AryaTools.Instance.Forms.WorkflowForm.Show();
                    Close();
                }
            }
        }
    }
}