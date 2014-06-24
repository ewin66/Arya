using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arya.HelperClasses;
using Arya.Data;

namespace Arya.HelperForms
{
    public partial class FrmCreateSkuGroup : Form
    {
        private List<Sku> skuList;
        private List<Group> skuGroups;
        private System.Xml.Linq.XElement _xml;


        public FrmCreateSkuGroup()
        {
            InitializeComponent();
        }

        public FrmCreateSkuGroup(List<Sku> list)
            : this()
        {
            this.skuList = list;
            ActivateConflicts(list.Any(s => s.SkuGroups.Any(a => a.Active)));
            skuGroups = AryaTools.Instance.InstanceData.Dc.Groups.Where(g => g.GroupType == Group.SKU_GROUP_UD).ToList();
        }

        public FrmCreateSkuGroup(System.Xml.Linq.XElement xml)
            : this()
        {
            _xml = xml;
            ActivateConflicts(false);
            skuGroups = AryaTools.Instance.InstanceData.Dc.Groups.Where(g => g.GroupType == Group.SKU_GROUP_UD).ToList();
        }

        private void ActivateConflicts(bool HasConflicts)
        {
            btnShowConflicts.Visible = HasConflicts;
            lblConflict.Visible = HasConflicts;
            checkBoxOverwrite.Visible = HasConflicts;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxGroupName.Text) || string.IsNullOrWhiteSpace(textBoxGroupDescription.Text))
            {
                MessageBox.Show("Please provide Group Name and Desription");
                return;

            }
            ProcessSkuGroup();
            AryaTools.Instance.SaveChangesIfNecessary(true, true);
            Close();


        }

        private void ProcessSkuGroup()
        {


            if (skuGroups.Any(d => d.Name.ToLower().Trim() == textBoxGroupName.Text.ToLower().Trim()))
            {
                var dlgResult = MessageBox.Show("The Group Name already exists, Are you sure you want to append SKUs to the same group?", "Sku Groups", MessageBoxButtons.YesNo);
                if (dlgResult == System.Windows.Forms.DialogResult.Yes)
                {

                    AddSkusToExistingGroup(textBoxGroupName.Text, textBoxGroupDescription.Text, checkBoxOverwrite.Checked);
                }
                return;
            }
            else
            {
                AddSkusToNewGroup(textBoxGroupName.Text, textBoxGroupDescription.Text, checkBoxOverwrite.Checked);
            }
        }

        private void AddSkusToExistingGroup(string GroupName, string GroupDescription, bool OverwriteExistingSkuGroup)
        {
            var existingGroup = skuGroups.Where(g => g.Name.ToLower().Trim() == GroupName.Trim().ToLower()).FirstOrDefault();
            existingGroup.Description = GroupDescription;
            AddSkusToGroup(checkBoxOverwrite.Checked, existingGroup);
        }

        private void AddSkusToNewGroup(string GroupName, string GroupDescription, bool OverwriteExistingSkuGroup)
        {
            Group newGroup = new Group()
                                 {
                                     Name = textBoxGroupName.Text,
                                     Description = textBoxGroupDescription.Text,
//                                     ClientName = AryaTools.Instance.InstanceData.CurrentProject.ProjectName,
                                     GroupType = Group.SKU_GROUP_UD
            };


            AddSkusToGroup(OverwriteExistingSkuGroup, newGroup);
            skuGroups.Add(newGroup);

        }

        private void AddSkusToGroup(bool OverwriteExistingSkuGroup, Group AddToGroup)
        {
            if (skuList == null && _xml != null)
            {

                AddToGroup.Criterion = _xml;
                AryaTools.Instance.InstanceData.Dc.Groups.InsertOnSubmit(AddToGroup);
                return;
            }

            foreach (var sku in skuList)
            {
                if (OverwriteExistingSkuGroup)
                {
                    sku.SkuGroups.Where(a => a.Active).ToList().ForEach(a => a.Active = false);
                }
            }
            skuList.ForEach(s => AddToGroup.SkuGroups.Add(new SkuGroup() { Sku = s, Active = true }));
        }



        private void btnShowConflicts_Click(object sender, EventArgs e)
        {
            string browserHtml = "ItemID\tGroup<br />"; ;
            foreach (var s in skuList)
            {
                if (s.SkuGroups.Any(a => a.Active))
                    browserHtml += s.ItemID + "\t" + string.Join(", ", s.SkuGroups.Where(g => g.Group.Name != null && g.Active).OrderBy(n => n.Group.Name).Select(g => g.Group.Name)) + "<br />";
            }
            AryaTools.Instance.Forms.BrowserForm.SetDocumentText(browserHtml);
            AryaTools.Instance.Forms.BrowserForm.BringToFront();
        }
    }
}
