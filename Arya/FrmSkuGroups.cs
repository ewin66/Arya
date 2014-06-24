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
using Arya.Properties;
using System.IO;
using Arya.HelperForms;

namespace Arya
{
    public partial class FrmSkuGroups : Form
    {
        private IQueryable<Sku> SkuQuery;
        private Group CurrentGroup;
        private Group SelectedGroup;
        List<GroupNote> SkuGroupDataList = new List<GroupNote>();
        List<Group> tagGroups = new List<Group>();
        private Dictionary<Group, Dictionary<TaxonomyInfo, int>> GroupTaxonomyCount = new Dictionary<Group, Dictionary<TaxonomyInfo, int>>();
        private List<TaxonomyInfo> GroupTaxonomyList = new List<TaxonomyInfo>();
        public FrmSkuGroups()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            this.Icon = Resources.AryaLogoIcon;
            GroupTaxonomyCount.Clear();
            initdgvSkuGroups();
            initdgvGroupDetails();
            //initdgvGroupComments();
            timerGroupDetails.Interval = 100;
            timerGroupDetails.Start();
        }

        private void initdgvGroupComments()
        {
           SkuGroupDataList =  CurrentGroup.GroupNotes.OrderByDescending(c => c.CreatedOn).ToList();
           dgvComment.RowCount = SkuGroupDataList.Count+1;
           PrepareCommentGrid();
           dgvComment.Invalidate();
        }

        private void PrepareCommentGrid()
        {
            for (int i = 0; i < dgvComment.RowCount; i++)
            {
                if (i % 2 != 0)
                    dgvComment.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRowSkuGroup;
                else
                    dgvComment.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleGreyRegularSkuGroup;
            }
        }

        private void initdgvGroupDetails()
        {

            if (CurrentGroup == null)
            {
                
                return;

            }

            //new DataGridViewLinkColumn();
            if (GroupTaxonomyCount.Keys.Contains(CurrentGroup))
            {
                GroupTaxonomyList = GroupTaxonomyCount[CurrentGroup].Keys.ToList();
                dgvGroupDetails.RowCount = GroupTaxonomyList.Count;
                PrepareSkuDetailsGrid();
                dgvGroupDetails.Invalidate();
                initdgvGroupComments();
            }
            //dgvGroupDetails.DataSource = CurrentGroup.Nodes.Select(s => new { Taxonomy = s.ToString(), Count = CurrentGroup[s].Count() }).ToList();

            
        }

        private void PrepareSkuDetailsGrid()
        {
            for (int i = 0; i < dgvGroupDetails.RowCount; i++)
            {
                if (i % 2 != 0)
                    dgvGroupDetails.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRowSkuGroup;
                else
                    dgvGroupDetails.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleGreyRegularSkuGroup;
            }
        }

        private string FileDelimitter = "\t";
        private List<Group> groups = new List<Group>();

        private void initdgvSkuGroups()
        {
            Guid waitkey = FrmWaitScreen.ShowMessage("Fetching data, please wait...");
            InitGroupData();
            FrmWaitScreen.HideMessage(waitkey);            
            

         
        }

        private void InitGroupData()
        {
            GroupTaxonomyCount.Clear();
            groups = AryaTools.Instance.InstanceData.Dc.Groups.Where(t => t.GroupType == Group.SKU_GROUP_UD &&( t.Criterion!=null || t.SkuGroups.Any(a => a.Active))).OrderBy(g => g.Name).ToList();
            foreach (var g in groups)
            {
                //GroupTaxonomyCount.Clear();
                Dictionary<TaxonomyInfo, int> nodeCount = new Dictionary<TaxonomyInfo, int>();
                foreach (var n in g.Nodes)
                {
                    nodeCount.Add(n, g[n].Count());
                }
                GroupTaxonomyCount.Add(g, nodeCount);
            }

            dgvSkuGroups.RowCount = groups.Count;

            PreparedgvSkuGroupGrid();
        }

        private void PreparedgvSkuGroupGrid()
        {
            for (int i = 0; i < dgvSkuGroups.RowCount; i++)
            {
                if (i % 2 != 0)
                    dgvSkuGroups.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRowSkuGroup;
                else
                    dgvSkuGroups.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleGreyRegularSkuGroup;
            }
        }

       
        private void dgvSkuGroups_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var selectedRowIndex = e.RowIndex;
            var selectedColumnIndex = e.ColumnIndex;

            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Group")
            {
                e.Value = groups[selectedRowIndex].Name + (groups[selectedRowIndex].Criterion == null? " (" + groups[selectedRowIndex].SkuCount+")":string.Empty);
                return;
            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Group Description")
            {
                e.Value = groups[selectedRowIndex].Description;
                return;
            }

            //if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Sku Count")
            //{
            //    e.Value = groups[selectedRowIndex].SkuCount;
            //    return;
            //}

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Created By")
            {
                e.Value = groups[selectedRowIndex].User.FullName;
                return;
            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Created On")
            {
                e.Value = groups[selectedRowIndex].CreatedOn;
                return;
            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Download")
            {
                e.Value = "Export";
                return;
            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Criterion")
            {
                e.Value = groups[selectedRowIndex].Criterion == null ? string.Empty : "Show Query";

                return;
            }
            


        }

        private void dgvSkuGroups_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;


            var selectedRowIndex = e.RowIndex;
            var selectedColumnIndex = e.ColumnIndex;

            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "X")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                DataGridViewButtonCell bc = dgvSkuGroups[0, e.RowIndex] as DataGridViewButtonCell;
                e.Graphics.DrawImage(Resources.Close, e.CellBounds.Left + 5, e.CellBounds.Top + 3);
                e.Handled = true;
            }
            
        }

        private void dgvSkuGroups_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            var selectedRowIndex = e.RowIndex;
            var selectedColumnIndex = e.ColumnIndex;

            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "X")
            {
                var dlgResult = MessageBox.Show("Are you sure you want to delete " + groups[selectedRowIndex].Name + "?", "Sku Groups", MessageBoxButtons.YesNo);
                if (dlgResult == System.Windows.Forms.DialogResult.Yes)
                {
                    DeleteGroup(groups[selectedRowIndex]);
                }
            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Download")
            {
                saveFileDialog.FileName = groups[selectedRowIndex].Name;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                   WriteToFile(saveFileDialog.FileName,groups[selectedRowIndex].Skus);
                   return;
                }
            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Criterion" && dgvSkuGroups.CurrentCell.Value!= null)
            {
                if (groups[selectedRowIndex].Criterion != null)
                {
                    AryaTools.Instance.Forms.QueryForm.SelectedSkuGroupForQueryView = groups[selectedRowIndex];
                    AryaTools.Instance.Forms.QueryForm.SetFormType(FrmQueryView.QueryFormType.SkuGroup);
                    AryaTools.Instance.Forms.QueryForm.Show();
                }

            }

            if (dgvSkuGroups.Columns[selectedColumnIndex].HeaderText == "Group" && dgvSkuGroups.CurrentCell.Value != null)
            {
                var selectedGroups = new List<Group>();
                for (int i = 0; i < dgvSkuGroups.SelectedCells.Count; i++)
                {
                    if (dgvSkuGroups.Columns[dgvSkuGroups.SelectedCells[i].ColumnIndex].HeaderText == "Group")
                        selectedGroups.Add(groups[dgvSkuGroups.SelectedCells[i].RowIndex]);
                }

                SkuQuery = AryaTools.Instance.InstanceData.Dc.SkuGroups.Where(s => s.Active && selectedGroups.Contains( s.Group)).Select(a => a.Sku);
                //SkuQuery = Natal.Skus.SelectMany(s=> s.SkuInfos.Where(a=>a.Active && a.TaxonomyInfo == GroupTaxonomyList[selectedRowIndex] )).Select(sk=>sk.Sku);
                AryaTools.Instance.Forms.SkuForm.LoadTab(SkuQuery, null,string.Join(", ",selectedGroups.Select(n=>n.Name).ToList()), selectedGroups.Count.ToString()+" Group(s)");

                foreach (var group in selectedGroups.Where(c=>c.Criterion != null))
                {

                    Query.DisplayCrossListInSkuView(Query.FetchCrossListObject(group));
                }

            }


        }

        private void DeleteGroup(Group group)
        {
            var activeSkuGroups = group.SkuGroups.Where(a => a.Active);
            foreach(var sg in activeSkuGroups)
            {
                sg.Active = false;
                
            }

            group.Criterion = null;

            AryaTools.Instance.SaveChangesIfNecessary(true, true);
            initdgvSkuGroups();
            initdgvGroupDetails();
        }

        private void WriteToFile(string FileName, List<Sku> SkuList)
        {
            TextWriter tw = File.CreateText(FileName+".txt");
            
            tw.WriteLine(string.Format("ItemID{0}Taxonomy{0}Group", FileDelimitter));
            foreach (var sku in SkuList)
            {
                tw.WriteLine(sku.ItemID.ToString() + FileDelimitter + sku.Taxonomy.ToString() + FileDelimitter+ string.Join(", ", sku.SkuGroups.Where(a => a.Active).OrderBy(n => n.Group.Name).Select(g => g.Group.Name).ToList()));                
            }

            tw.Close();         
        }

        private void timerGroupDetails_Tick(object sender, EventArgs e)
        {
            if (dgvGroupDetails.IsHandleCreated && SelectedGroup != null && CurrentGroup != SelectedGroup)
            {
                CurrentGroup = SelectedGroup;
                initdgvGroupDetails();
            }
        }

        private void dgvSkuGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSkuGroups.SelectedCells.Count != 1 || dgvSkuGroups.CurrentCell.ColumnIndex < 0 || dgvSkuGroups.CurrentCell.RowIndex < 0)
                SelectedGroup = null;
            else
                SelectedGroup = groups[dgvSkuGroups.CurrentCell.RowIndex];

            tagGroups.Clear();
            for (int i = 0; i < dgvSkuGroups.SelectedCells.Count; i++)
            {
                
                    tagGroups.Add(groups[dgvSkuGroups.SelectedCells[i].RowIndex]);
                    
            }
            tagGroups =  tagGroups.Distinct().ToList();
        }

        private void dgvGroupDetails_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var selectedColumnIndex = e.ColumnIndex;
            var selectedRowIndex = e.RowIndex;

            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;

            if (dgvGroupDetails.Columns[selectedColumnIndex].HeaderText == "Taxonomy")
            {

                e.Value = GroupTaxonomyList.Count == 0? null:GroupTaxonomyList[selectedRowIndex];
                return;

            }

            if (dgvGroupDetails.Columns[selectedColumnIndex].HeaderText == "Sku Count")
            {
                if(GroupTaxonomyList.Count>0)
                    e.Value = GroupTaxonomyCount[CurrentGroup][GroupTaxonomyList[selectedRowIndex]];
                return;

            }
        }

        private void dgvGroupDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedColumnIndex = e.ColumnIndex;
            var selectedRowIndex = e.RowIndex;

            var x = dgvGroupDetails.SelectedCells;
            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;
            if (dgvGroupDetails.Columns[e.ColumnIndex].HeaderText == "Taxonomy")

            {

                var selectedTaxonomies  = new List<TaxonomyInfo>();
                for (int i = 0; i<dgvGroupDetails.SelectedCells.Count; i++)
                {
                    if(dgvGroupDetails.Columns[dgvGroupDetails.SelectedCells[i].ColumnIndex].HeaderText == "Taxonomy")
                        selectedTaxonomies.Add(GroupTaxonomyList[dgvGroupDetails.SelectedCells[i].RowIndex]);
                }
                
                SkuQuery = AryaTools.Instance.InstanceData.Dc.SkuGroups.Where(s => s.Active && s.Group == CurrentGroup).SelectMany(si => si.Sku.SkuInfos.Where(a => a.Active && selectedTaxonomies.Contains( a.TaxonomyInfo))).Select(s => s.Sku);
               //SkuQuery = Natal.Skus.SelectMany(s=> s.SkuInfos.Where(a=>a.Active && a.TaxonomyInfo == GroupTaxonomyList[selectedRowIndex] )).Select(sk=>sk.Sku);
                AryaTools.Instance.Forms.SkuForm.LoadTab(SkuQuery, null, groups[dgvSkuGroups.CurrentCell.RowIndex].Name + " ( " +dgvGroupDetails.CurrentCell.Value.ToString()+" )", "Query"); 
            }
        }

        private void dgvGroupDetails_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

        }

        private void dgvComment_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var selectedColumnIndex = e.ColumnIndex;
            var selectedRowIndex = e.RowIndex;


            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;

            if (selectedColumnIndex == 0 && e.Value != null && !string.IsNullOrWhiteSpace(e.Value.ToString())) // Comment Entered
            {
                var newComment =new GroupNote() { Comment = e.Value.ToString().Trim() } ;
                CurrentGroup.GroupNotes.Add(newComment);
                SkuGroupDataList.Insert(0, newComment);
                AryaTools.Instance.SaveChangesIfNecessary(true, true);
                dgvComment.Invalidate();

            }

        }

        private void dgvComment_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

        }

        private void dgvComment_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var selectedColumnIndex = e.ColumnIndex;
            var selectedRowIndex = e.RowIndex;


            if (selectedColumnIndex < 0 || selectedRowIndex < 0)
                return;

            if (selectedColumnIndex == 0 && selectedRowIndex <SkuGroupDataList.Count) //Comment Column
            {
                e.Value = SkuGroupDataList[selectedRowIndex].Comment;
                return;

            }

            if (selectedColumnIndex == 1 && selectedRowIndex < SkuGroupDataList.Count) //Comment Column
            {
                e.Value = SkuGroupDataList[selectedRowIndex].User.FullName +" on " + SkuGroupDataList[selectedRowIndex].CreatedOn.ToShortDateString();
                return;

            }


        }

        private void treeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmIntersection fm = new FrmIntersection();
            fm.Show();
        }

        private void showCommentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var notes = new FrmNotes(tagGroups);
            notes.ShowDialog();

            //AryaTools.Instance.NotesForm.ShowDialog();
        }

    




       
       
    }


   
}
