using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LinqKit;
using Arya.Framework.Common.ComponentModel;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya.HelperForms
{
    public partial class FrmFilter : Form
    {
		#region Fields (6) 

        internal bool _clearAllFilters;
        internal bool _clearThisFilter;
        private int _currentSortColumn;
        private SortOrder _currentSortOrder = SortOrder.Ascending;
        internal List<string> _selectedItems = new List<string>();
        private string _attributeName;
        private double _fillRate;
        //internal const string BlankValue = " <blank> ";

		#endregion Fields 

		#region Constructors (1) 

        public FrmFilter()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
            DialogResult = DialogResult.Cancel;
        }

		#endregion Constructors 

		#region Methods (10) 

		// Public Methods (1) 

        public void UpdateFilter(
            Point formLocation, IEnumerable<ListViewItem> listViewItems, bool showClearAllButton, string attributeName,
            double fillRate)
        {
            _fillRate = fillRate;
            _attributeName = attributeName;
            _currentSortColumn = 0;
            _currentSortOrder = SortOrder.Ascending;
            _clearThisFilter = false;
            _clearAllFilters = false;
            txtFind.Text = string.Empty;
            lstFilterItems.Items.Clear();
            Text = string.Format("{0} ({1:0.00}%)", attributeName, fillRate);

            listViewItems.ForEach(item => lstFilterItems.Items.Add(item));
            lstFilterItems.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            if (lstFilterItems.Columns[0].Width > 300)
                lstFilterItems.Columns[0].Width = 300;
            if (lstFilterItems.Columns[0].Width < 120)
                lstFilterItems.Columns[0].Width = 120;

            Width = lstFilterItems.Columns.Cast<ColumnHeader>().Sum(header => header.Width) + 40;
            Location = Screen.FromControl(this).WorkingArea.Right > Width + formLocation.X
                           ? formLocation
                           : new Point(Screen.FromControl(this).WorkingArea.Right - Width - 1, formLocation.Y);
            Height = Screen.FromControl(this).WorkingArea.Height - Location.Y - 10;

            btnClearAllFilters.Visible = showClearAllButton;
            btnClearThisFilter.Visible = lstFilterItems.SelectedItems.Count > 0;
        }
		// Private Methods (9) 

        private void ApplyFilter()
        {
            _selectedItems = lstFilterItems.SelectedItems.Cast<ListViewItem>().Select(item => item.Text).ToList();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnClearAllFilters_Click(object sender, EventArgs e)
        {
            _clearAllFilters = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClearThisFilter_Click(object sender, EventArgs e)
        {
            _clearThisFilter = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnExportView_Click(object sender, EventArgs e)
        {
            const string filterFilename = "FilterItems.html";
            using (var file = new StreamWriter(filterFilename))
            {
                file.WriteLine("<html><body align=center>");
                file.WriteLine("<h1>{0}<h1>", _attributeName);
                file.WriteLine("<h3> Fill Rate: {0}<h3>", string.Format("{0:0.00}%", _fillRate));
                file.WriteLine("<table border=1 cellpadding=5>");

                file.WriteLine("<tr>");
                foreach (ColumnHeader column in lstFilterItems.Columns)
                    file.WriteLine("<th>{0}</th>", column.Text);
                file.WriteLine("</tr>");

                foreach (ListViewItem row in lstFilterItems.Items)
                {
                    file.WriteLine("<tr>");
                    foreach (ListViewItem.ListViewSubItem val in row.SubItems)
                        file.WriteLine("<td align=left>{0}</td>", val.Text);
                    file.WriteLine("</tr>");
                }
                file.WriteLine("</table></body></html>");
            }

            AryaTools.Instance.Forms.BrowserForm.GotoUrl(new FileInfo(filterFilename).FullName, Text);
        }

        private void DoFind()
        {
            string searchString = txtFind.Text.ToLower();
            foreach (ListViewItem item in lstFilterItems.Items)
                item.Selected = item.SubItems[0].Text.ToLower().Contains(searchString);

            lstFilterItems.ListViewItemSorter = new ListViewItemComparer(
                _currentSortColumn, _currentSortOrder == SortOrder.Ascending, true, true);
            lstFilterItems.Sort();
        }

        private void frmFilter_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    Close();
                    break;
            }
        }

        private void lstFilterItems_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_currentSortColumn == e.Column)
            {
                _currentSortOrder = _currentSortOrder == SortOrder.Ascending
                                        ? SortOrder.Descending
                                        : SortOrder.Ascending;
            }
            else
            {
                _currentSortColumn = e.Column;
                _currentSortOrder = SortOrder.Ascending;
            }
            lstFilterItems.ListViewItemSorter = new ListViewItemComparer(
                _currentSortColumn, _currentSortOrder == SortOrder.Ascending, true, false);
            lstFilterItems.Sort();
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            DoFind();
        }

		#endregion Methods 
    }
}