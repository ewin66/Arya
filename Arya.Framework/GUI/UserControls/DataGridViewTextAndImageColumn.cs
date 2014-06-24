namespace Arya.Framework.GUI.UserControls
{
    using System.Drawing;
    using System.Windows.Forms;

    public sealed class DataGridViewTextAndImageColumn : DataGridViewTextBoxColumn
    {
        private Image imageValue;
        private Size imageSize;
 
        public DataGridViewTextAndImageColumn()
        {
            this.CellTemplate = new DataGridViewTextAndImageCell();
        }
 
        public override object Clone()
        {
            var c = base.Clone() as DataGridViewTextAndImageCell;
            if (c != null)
            {
                c.Image = this.imageValue;
                c.ImageSize = this.imageSize;
            }
            return c;
        }
 
        public Image Image
        {
            get { return this.imageValue; }
            set
            {
                if (this.Image != value)
                {
                    this.imageValue = value;
                    this.imageSize = value.Size;
 
                    if (this.InheritedStyle != null)
                    {
                        Padding inheritedPadding = this.InheritedStyle.Padding;
                        this.DefaultCellStyle.Padding = new Padding(inheritedPadding.Left,
                            inheritedPadding.Top, this.imageSize.Width,
                            inheritedPadding.Bottom);
                    }
                }
            }
        }
 
        private DataGridViewTextAndImageCell TextAndImageCellTemplate
        {
            get { return this.CellTemplate as DataGridViewTextAndImageCell; }
        }
 
        internal Size ImageSize
        {
            get { return this.imageSize; }
        }
    }
}
