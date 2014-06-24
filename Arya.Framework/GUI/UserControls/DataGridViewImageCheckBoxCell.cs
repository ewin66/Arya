namespace Arya.Framework.GUI.UserControls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class DataGridViewImageCheckBoxCell : DataGridViewImageCell
    {
        [DefaultValue(false)]
        public bool IsChecked { get; set; }

        public override object Clone()
        {
            var cell = (DataGridViewImageCheckBoxCell)base.Clone();
            cell.IsChecked = this.IsChecked;
            return cell;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            var parentColumn = this.OwningColumn as DataGridViewImageCheckBoxColumn;

            if (!(parentColumn == null || parentColumn.CheckedImage == null || parentColumn.UnCheckedImage == null))
            {
                formattedValue = this.IsChecked ? parentColumn.CheckedImage : parentColumn.UnCheckedImage;
            }

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        //public new object Value
        //{
        //    get
        //    {
        //        //bool val;
        //        //return bool.TryParse(RowIndex == -1 || base.Value == null ? string.Empty : base.Value.ToString(), out val) && val;
        //        return boolValue;
        //    }
        //    set
        //    {
        //        bool val;
        //        boolValue = bool.TryParse(value == null ? string.Empty : base.Value.ToString(), out val) && val;
        //    }
        //}

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            this.IsChecked = !this.IsChecked;
            base.OnMouseClick(e);
        }
    }

}
