namespace Arya.Framework.GUI.UserControls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class DataGridViewTextAndImageCell : DataGridViewTextBoxCell
    {
        private Image imageValue;
        public Size ImageSize;

        public override object Clone()
        {
            var c = (DataGridViewTextAndImageCell)base.Clone();
            c.imageValue = this.imageValue;
            c.ImageSize = this.ImageSize;
            return c;
        }

        public Image Image
        {
            get
            {
                if (this.OwningColumn == null || this.OwningTextAndImageColumn == null)
                {
                    return this.imageValue;
                }
                return this.imageValue ?? this.OwningTextAndImageColumn.Image;
            }
            set
            {
                if (this.imageValue != value)
                {
                    try
                    {
                        this.imageValue = value;
                        this.ImageSize = value.Size;

                        Padding inheritedPadding = this.InheritedStyle.Padding;
                        this.Style.Padding = new Padding(inheritedPadding.Left,
                        inheritedPadding.Top, this.ImageSize.Width,
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

            if (this.Image != null)
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
                graphics.DrawImage(this.Image, rect);

                //Draw image unscaled, the image size will keep unchanged.
                //graphics.DrawImageUnscaled(this.Image, rect);

                graphics.EndContainer(container);
            }
        }

        private DataGridViewTextAndImageColumn OwningTextAndImageColumn
        {
            get
            {
                return this.OwningColumn as DataGridViewTextAndImageColumn;
            }
        }
    }
}
