using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya
{
    public partial class FrmNotes : Form
    {
        private List<Data.Group> tagGroups;
        private List<TaxonomyInfo> tagTaxonomyInfos;
        private List<EntityData> tagEntityDatas;
        private NoteEntity noteType;

        public FrmNotes()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        public FrmNotes(List<Data.Group> tagGroups):this()
        {
            // TODO: Complete member initialization
            this.tagGroups = tagGroups;
            noteType = NoteEntity.SkuGroup;
            var sb = new StringBuilder();
            sb.Append("Sku Group/(s)");
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(", ", tagGroups.Select(n => n.Name).ToList()));
            txtBoxTopic.Text = sb.ToString();
            InitData();
        }

        public FrmNotes(List<TaxonomyInfo> tagTaxonomyInfos):this()
        {
            this.tagTaxonomyInfos = tagTaxonomyInfos;
            noteType = NoteEntity.TaxonomyInfo;
            var sb = new StringBuilder();
            sb.Append("Taxonomy/(ies)");
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(", ", tagTaxonomyInfos.Select(n => n.ToString()).ToList()));
            txtBoxTopic.Text = sb.ToString();
            InitData();
        }

        public FrmNotes(List<EntityData> tagEntityDatas): this()
        {
            this.tagEntityDatas = tagEntityDatas;
            noteType = NoteEntity.EntityData;
            var sb = new StringBuilder();
            sb.Append("Entity/(ies)");
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(", ", tagEntityDatas.Select(n => "("+n.Value+", "+n.Uom+")").ToList()));
            txtBoxTopic.Text = sb.ToString();
            InitData();

        }

        private void InitData()
        {
            switch (noteType)
            {

                case NoteEntity.SkuGroup:
                    PopulateSkuGroupNotes();
                    break;
                case NoteEntity.TaxonomyInfo:
                    PopulateTaxonomyNotes();
                    break;
                case NoteEntity.EntityData:
                    PopulateEntityDatas();
                    break;
                default:
                    break;

            }
        }

        private void PopulateEntityDatas()
        {
            List<string> commentDataSource = new List<string>();

            if (tagEntityDatas.Count > 1) //more than one group selected
            {
                List<int> seperators = new List<int>();

                var groupComments = tagEntityDatas.SelectMany(r => r.EntityDataNotes).GroupBy(g => g.EntityData).Select(p => new
                {
                    group = "("+p.Key.Value + ", "+p.Key.Uom+")",
                    comment = p.OrderBy(o => o.CreatedOn)
                        .Select(c => c.Comment).Aggregate(string.Empty, (a, next) => a + "|" + next)
                }).ToList();

                var commentGroups = groupComments.GroupBy(c => c.comment).Select(a => new { comments = a.Key, groups = a.Select(b => b.group).ToList() }).ToList();


                foreach (var commentGroup in commentGroups)
                {
                    var groups = string.Join(", ", commentGroup.groups);
                    var comments = commentGroup.comments.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    seperators.Add(commentDataSource.Count);

                    commentDataSource.Add(groups);
                    commentDataSource.AddRange(comments);
                    commentDataSource.Add(string.Empty);


                }

                dataGridView1.Tag = seperators;
                dataGridView1.DataSource = commentDataSource.Select(a => new { Conversation = a }).ToList();
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Refresh();
            }
            else
            {
                var groupNotes = tagEntityDatas.SelectMany(a => a.EntityDataNotes).OrderBy(d => d.CreatedOn).ToList();
                var y = groupNotes.Select(b => new { DateTime = string.Format("{0:d/M/yyyy HH:mm}", b.CreatedOn), Conversation = b.User.FullName + ": " + b.Comment }).Distinct().ToList();
                dataGridView1.DataSource = y;
                //Applying Grid Style//
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.LightGray;

            }
        }

        private void PopulateTaxonomyNotes()
        {
            List<string> commentDataSource = new List<string>();

            if (tagTaxonomyInfos.Count> 1) //more than one group selected
            {
                List<int> seperators = new List<int>();

                var groupComments = tagTaxonomyInfos.SelectMany(r => r.TaxonomyNotes).GroupBy(g => g.TaxonomyInfo).Select(p => new
                {
                    group = p.Key.ToString(),
                    comment = p.OrderBy(o => o.CreatedOn)
                        .Select(c => c.Comment).Aggregate(string.Empty, (a, next) => a + "|" + next)
                }).ToList();

                var commentGroups = groupComments.GroupBy(c => c.comment).Select(a => new { comments = a.Key, groups = a.Select(b => b.group).ToList() }).ToList();


                foreach (var commentGroup in commentGroups)
                {
                    var groups = string.Join(", ", commentGroup.groups);
                    var comments = commentGroup.comments.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    seperators.Add(commentDataSource.Count);

                    commentDataSource.Add(groups);
                    commentDataSource.AddRange(comments);
                    commentDataSource.Add(string.Empty);


                }

                dataGridView1.Tag = seperators;
                dataGridView1.DataSource = commentDataSource.Select(a => new { Conversation = a }).ToList();
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Refresh();
            }
            else
            {
                var groupNotes = tagTaxonomyInfos.SelectMany(a => a.TaxonomyNotes).OrderBy(d => d.CreatedOn).ToList();
                var y = groupNotes.Select(b => new { DateTime = string.Format("{0:d/M/yyyy HH:mm}", b.CreatedOn), Conversation = b.User.FullName+ ": " + b.Comment }).Distinct().ToList();
                dataGridView1.DataSource = y;
                //Applying Grid Style//
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.LightGray;

            }

        }

        private void PopulateSkuGroupNotes()
        {


            var commentDataSource = new List<string>();

            if (tagGroups.Count > 1) //more than one group selected
            {
                var seperators = new List<int>();

                var groupComments = tagGroups.SelectMany(r => r.GroupNotes).GroupBy(g => g.Group.Name).Select(p => new
                {
                    group = p.Key,
                    comment = p.OrderBy(o => o.CreatedOn)
                        .Select(c => c.Comment).Aggregate(string.Empty, (a, next) => a + "|" + next)
                }).ToList();

                var commentGroups = groupComments.GroupBy(c => c.comment).Select(a => new { comments = a.Key, groups = a.Select(b => b.group).ToList() }).ToList();


                foreach (var commentGroup in commentGroups)
                {
                    var groups = string.Join(", ", commentGroup.groups);
                    var comments = commentGroup.comments.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    seperators.Add(commentDataSource.Count);

                    commentDataSource.Add(groups);
                    commentDataSource.AddRange(comments);
                    commentDataSource.Add(string.Empty);


                }

                dataGridView1.Tag = seperators;
                dataGridView1.DataSource = commentDataSource.Select(a => new { Conversation = a }).ToList();
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGridView1.Refresh();
            }
            else
            {
                var groupNotes = tagGroups.SelectMany(a => a.GroupNotes).OrderBy(d=>d.CreatedOn).ToList();
                var y = groupNotes.Select(b => new { DateTime = string.Format("{0:d/M/yyyy HH:mm}", b.CreatedOn), Conversation = b.User.FullName + ": " + b.Comment }).Distinct().ToList();
                dataGridView1.DataSource = y;
                //Applying Grid Style//
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.LightGray;

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtBoxPost.Text))
            {
                AddComments();             

            }

            txtBoxPost.Text = string.Empty;
            InitData();
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            dataGridView1.Invalidate();
        }

        private void AddComments() 
        {
            if (noteType == NoteEntity.SkuGroup)
            {
                tagGroups.ForEach(g => g.GroupNotes.Add(new GroupNote() { Comment = txtBoxPost.Text.Trim().TrimEnd('\n') }));
            }
            if (noteType == NoteEntity.TaxonomyInfo)
            {
                tagTaxonomyInfos.ForEach(g => g.TaxonomyNotes.Add(new TaxonomyNote() { Comment = txtBoxPost.Text.Trim().TrimEnd('\n'),TaxonomyPath = g.ToString() }));
            }

            if (noteType == NoteEntity.EntityData)
            {
                tagEntityDatas.ForEach(g => g.EntityDataNotes.Add(new EntityDataNote() { Comment = txtBoxPost.Text.Trim().TrimEnd('\n')}));
            }

            AryaTools.Instance.SaveChangesIfNecessary(true, true);
        }

        private void dataGridView1_CellPainting_1(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            
            if(  dataGridView1.Tag  != null &&  ((List<int>)dataGridView1.Tag).Contains(e.RowIndex) )
                 e.CellStyle.BackColor = Color.Gold;

            if ((e.RowIndex>0 && e.ColumnIndex>-1 && (string)dataGridView1[e.ColumnIndex,e.RowIndex-1].Value == string.Empty)||(e.RowIndex == 0 && e.ColumnIndex > -1 && dataGridView1.ColumnCount == 1))
            {
                e.CellStyle.Font = new Font(FontFamily.GenericSansSerif, 9,FontStyle.Bold);
            }
            
        }

    }

    enum NoteEntity
    {
        SkuGroup,
        EntityData,
        TaxonomyInfo,
        SchemaInfo

    }
}
