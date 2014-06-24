using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Natalie.Framework.UI.TypeEditors.Forms
{
    public partial class ImportFieldMapper : Form
    {
        public ImportFieldMapper()
        {
            InitializeComponent();
        }

        private void lstMapping_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem item = GetMappingItemAtPoint(e);
            if (item == null)
                return;

            item.SubItems[2].Text = e.Data.GetData(DataFormats.Text).ToString();
            lstMapping.Focus();
        }

        private void lstMapping_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = GetMappingItemAtPoint(e);
            if (item == null)
                e.Effect = DragDropEffects.None;
            else
            {
                item.Selected = true;
                e.Effect = DragDropEffects.Link;
            }
        }

        private void lstMapping_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstMapping.SelectedItems.Count > 0)
                lstMapping.SelectedItems[0].SubItems[2].Text = string.Empty;
        }

        private ListViewItem GetMappingItemAtPoint(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text))
                return null;

            Point point = lstMapping.PointToClient(new Point(e.X, e.Y));
            return lstMapping.GetItemAt(point.X, point.Y);
        }

        private void lstFileFields_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstFileFields.SelectedIndex >= 0 &&
                lstFileFields.GetItemRectangle(lstFileFields.SelectedIndex).Contains(e.X, e.Y))
                DoDragDrop(lstFileFields.SelectedItem, DragDropEffects.Link);
        }
    }
}
