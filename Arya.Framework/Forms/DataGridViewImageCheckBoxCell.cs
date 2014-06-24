using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Natalie.Framework.Forms
{
    public class DataGridViewImageCheckBoxCell : DataGridViewImageCell
    {
        [DefaultValue(false)]
        public bool IsChecked { get; set; }

        public override object Clone()
        {
            var cell = (DataGridViewImageCheckBoxCell)base.Clone();
            cell.IsChecked = IsChecked;
            return cell;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            var parentColumn = OwningColumn as DataGridViewImageCheckBoxColumn;

            if (!(parentColumn == null || parentColumn.CheckedImage == null || parentColumn.UnCheckedImage == null))
            {
                formattedValue = IsChecked ? parentColumn.CheckedImage : parentColumn.UnCheckedImage;
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
            IsChecked = !IsChecked;
            base.OnMouseClick(e);
        }
    }

}
