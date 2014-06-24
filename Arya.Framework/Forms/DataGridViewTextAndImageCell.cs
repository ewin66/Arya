using System;
using System.Drawing;
using System.Windows.Forms;

namespace Natalie.Framework.Forms
{
    public class DataGridViewTextAndImageCell : DataGridViewTextBoxCell
    {
        private Image imageValue;
        public Size ImageSize;

        public override object Clone()
        {
            var c = (DataGridViewTextAndImageCell)base.Clone();
            c.imageValue = imageValue;
            c.ImageSize = ImageSize;
            return c;
        }

        public Image Image
        {
            get
            {
                if (OwningColumn == null || OwningTextAndImageColumn == null)
                {
                    return imageValue;
                }
                return imageValue ?? OwningTextAndImageColumn.Image;
            }
            set
            {
                if (imageValue != value)
                {
                    try
                    {
                        imageValue = value;
                        ImageSize = value.Size;

                        Padding inheritedPadding = InheritedStyle.Padding;
                        Style.Padding = new Padding(inheritedPadding.Left,
                        inheritedPadding.Top, ImageSize.Width,
                        inheritedPadding.Bottom);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds,
            Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
            object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // Paint the base content
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
               value, formattedValue, errorText, cellStyle,
               advancedBorderStyle, paintParts);

            if (Image != null)
            {
                // Draw the image clipped to the cell.
                System.Drawing.Drawing2D.GraphicsContainer container =
                graphics.BeginContainer();

                graphics.SetClip(cellBounds);
                var rect = new Rectangle
                               {
                                   Location =
                                       new Point(cellBounds.Location.X + cellBounds.Width - this.Image.Width - 1,
                                                 cellBounds.Location.Y + 1),
                                   Size = new Size(this.Image.Width, cellBounds.Height - 2)
                               };

                //Draw image scaled, the image will be resized to fit the cell
                graphics.DrawImage(Image, rect);

                //Draw image unscaled, the image size will keep unchanged.
                //graphics.DrawImageUnscaled(this.Image, rect);

                graphics.EndContainer(container);
            }
        }

        private DataGridViewTextAndImageColumn OwningTextAndImageColumn
        {
            get
            {
                return OwningColumn as DataGridViewTextAndImageColumn;
            }
        }
    }
}
