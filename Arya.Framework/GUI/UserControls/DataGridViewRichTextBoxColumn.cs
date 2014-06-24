namespace Arya.Framework.GUI.UserControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class DataGridViewRichTextBoxColumn : DataGridViewColumn
    {
        public DataGridViewRichTextBoxColumn() : base(new DataGridViewRichTextBoxCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (!(value is DataGridViewRichTextBoxCell))
                    throw new InvalidCastException("CellTemplate must be a DataGridViewRichTextBoxCell");

                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewRichTextBoxCell : DataGridViewImageCell
    {
        private static readonly RichTextBox EditingControl = new RichTextBox();

        public override Type EditType
        {
            get { return typeof (DataGridViewRichTextBoxEditingControl); }
        }

        public override Type ValueType
        {
            get { return typeof (string); }
            set { base.ValueType = value; }
        }

        public override Type FormattedValueType
        {
            get { return typeof (string); }
        }

        private static void SetRichTextBoxText(RichTextBox ctl, string text)
        {
            try
            {
                ctl.Rtf = text;
            }
            catch (ArgumentException)
            {
                ctl.Text = text;
            }
        }

        private Image GetRtfImage(int rowIndex, object value, bool selected)
        {
            var cellSize = GetSize(rowIndex);

            if (cellSize.Width < 1 || cellSize.Height < 1)
                return null;

            RichTextBox ctl = EditingControl;
            ctl.Size = GetSize(rowIndex);
            SetRichTextBoxText(ctl, Convert.ToString(value));

            if (ctl != null)
            {
                // Print the content of RichTextBox to an image.
                var imgSize = new Size(cellSize.Width - 1, cellSize.Height - 1);
                Image rtfImg;

                if (selected)
                {
                    // Selected cell state
                    ctl.BackColor = DataGridView.DefaultCellStyle.SelectionBackColor;
                    ctl.ForeColor = DataGridView.DefaultCellStyle.SelectionForeColor;

                    // Print image
                    rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width, imgSize.Height);

                    // Restore RichTextBox
                    ctl.BackColor = DataGridView.DefaultCellStyle.BackColor;
                    ctl.ForeColor = DataGridView.DefaultCellStyle.ForeColor;
                }
                else
                    rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width, imgSize.Height);

                return rtfImg;
            }

            return null;
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
                                                      DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var ctl = DataGridView.EditingControl as RichTextBox;

            if (ctl != null)
                SetRichTextBoxText(ctl, Convert.ToString(initialFormattedValue));
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter,
                                                    TypeConverter formattedValueTypeConverter,
                                                    DataGridViewDataErrorContexts context)
        {
            return value;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                      DataGridViewElementStates cellState, object value, object formattedValue,
                                      string errorText, DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle,
                       advancedBorderStyle, paintParts);

            var img = GetRtfImage(rowIndex, value, base.Selected);

            if (img != null)
                graphics.DrawImage(img, cellBounds.Left, cellBounds.Top);
        }

        #region Handlers of edit events, copyied from DataGridViewTextBoxCell

        private byte _flagsState;

        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            base.OnEnter(rowIndex, throughMouseClick);

            if ((DataGridView != null) && throughMouseClick)
                _flagsState = (byte) (_flagsState | 1);
        }

        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            base.OnLeave(rowIndex, throughMouseClick);

            if (DataGridView != null)
                _flagsState = (byte) (_flagsState & -2);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (DataGridView == null)
                return;
            
            var currentCellAddress = DataGridView.CurrentCellAddress;

            if (((currentCellAddress.X == e.ColumnIndex) && (currentCellAddress.Y == e.RowIndex)) &&
                (e.Button == MouseButtons.Left))
            {
                if ((_flagsState & 1) != 0)
                    _flagsState = (byte) (_flagsState & -2);
                else if (DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically)
                    DataGridView.BeginEdit(false);
            }
        }

        public override bool KeyEntersEditMode(KeyEventArgs e)
        {
            return (((((char.IsLetterOrDigit((char) ((ushort) e.KeyCode)) &&
                        ((e.KeyCode < Keys.F1) || (e.KeyCode > Keys.F24))) ||
                       ((e.KeyCode >= Keys.NumPad0) && (e.KeyCode <= Keys.Divide))) ||
                      (((e.KeyCode >= Keys.OemSemicolon) && (e.KeyCode <= Keys.OemBackslash)) ||
                       ((e.KeyCode == Keys.Space) && !e.Shift))) && (!e.Alt && !e.Control)) || base.KeyEntersEditMode(e));
        }

        #endregion
    }

    public class DataGridViewRichTextBoxEditingControl : RichTextBox, IDataGridViewEditingControl
    {
        private bool _valueChanged;

        public DataGridViewRichTextBoxEditingControl()
        {
            BorderStyle = BorderStyle.None;
        }

        #region IDataGridViewEditingControl Members

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
        }

        public DataGridView EditingControlDataGridView { get; set; }

        public object EditingControlFormattedValue
        {
            get { return Rtf; }
            set
            {
                if (value is string)
                    Text = value as string;
            }
        }

        public int EditingControlRowIndex { get; set; }

        public bool EditingControlValueChanged
        {
            get { return _valueChanged; }
            set { _valueChanged = value; }
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch ((keyData & Keys.KeyCode))
            {
                case Keys.Return:
                    if ((((keyData & (Keys.Alt | Keys.Control | Keys.Shift)) == Keys.Shift) && Multiline))
                        return true;
                    break;
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }

            return !dataGridViewWantsInputKey;
        }

        public Cursor EditingPanelCursor
        {
            get { return Cursor; }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Rtf;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        #endregion

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            _valueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            var keys = keyData & Keys.KeyCode;
            return keys == Keys.Return ? Multiline : base.IsInputKey(keyData);
        }
    }
}