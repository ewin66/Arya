using System.Drawing;
using System.Windows.Forms;

namespace Natalie.Framework.Forms
{
    public sealed class DataGridViewTextAndImageColumn : DataGridViewTextBoxColumn
    {
        private Image imageValue;
        private Size imageSize;
 
        public DataGridViewTextAndImageColumn()
        {
            CellTemplate = new DataGridViewTextAndImageCell();
        }
 
        public override object Clone()
        {
            var c = base.Clone() as DataGridViewTextAndImageCell;
            if (c != null)
            {
                c.Image = imageValue;
                c.ImageSize = imageSize;
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
                            inheritedPadding.Top, imageSize.Width,
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
            get { return imageSize; }
        }
    }
}
