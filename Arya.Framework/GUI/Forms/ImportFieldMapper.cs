namespace Arya.Framework.GUI.Forms
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class ImportFieldMapper : Form
    {
        public ImportFieldMapper()
        {
            this.InitializeComponent();
        }

        private void lstMapping_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem item = this.GetMappingItemAtPoint(e);
            if (item == null)
                return;

            item.SubItems[2].Text = e.Data.GetData(DataFormats.Text).ToString();
            this.lstMapping.Focus();
        }

        private void lstMapping_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = this.GetMappingItemAtPoint(e);
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
            if (e.KeyCode == Keys.Delete && this.lstMapping.SelectedItems.Count > 0)
                this.lstMapping.SelectedItems[0].SubItems[2].Text = string.Empty;
        }

        private ListViewItem GetMappingItemAtPoint(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text))
                return null;

            Point point = this.lstMapping.PointToClient(new Point(e.X, e.Y));
            return this.lstMapping.GetItemAt(point.X, point.Y);
        }

        private void lstFileFields_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.lstFileFields.SelectedIndex >= 0 &&
                this.lstFileFields.GetItemRectangle(this.lstFileFields.SelectedIndex).Contains(e.X, e.Y))
                this.DoDragDrop(this.lstFileFields.SelectedItem, DragDropEffects.Link);
        }
    }
}
