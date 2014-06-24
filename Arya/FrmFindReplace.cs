using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.UserControls;

namespace Arya
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    internal partial class FrmFindReplace : Form
    {
		#region Fields (8) 

        private bool _findNext;
        private readonly EntityDataGridView _parent;
        private List<DataGridViewCell> _selection;
        private int _selectionCursor;
        private int _startColumn;
        private int _startRow;
        private const string FindTypeEntireValue = "Entire Value";
        private const string FindTypeEqualTo = "Equal to";

		#endregion Fields 

		#region Constructors (2) 

        public FrmFindReplace(EntityDataGridView parent)
            : this()
        {
            _parent = parent;
        }

        private FrmFindReplace()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Properties.Resources.AryaLogoIcon;

            ddReplaceType.Items.Clear();
            ddReplaceType.Items.AddRange(new[] { "Search String", FindTypeEntireValue });

            ddFindType.Items.Clear();
            ddFindType.Items.AddRange(new[] { "Contains", FindTypeEqualTo, "Starts with", "Ends with" });
            ddFindType.SelectedIndex = 0;
        }

		#endregion Constructors 

		#region Methods (14) 

		// Public Methods (1) 

        public void ShowFindReplaceForm(string findString=null)
        {
            _findNext = false;
            txtFind.Text = GetClipboardText();
            Show();
            txtFind.SelectAll();
            txtFind.Focus();
        }
		// Private Methods (13) 

        private static string GetClipboardText()
        {
            var dataObj = Clipboard.GetDataObject();

            if (dataObj == null || !dataObj.GetDataPresent(DataFormats.Text))
                return string.Empty;

            return dataObj.GetData(DataFormats.Text).ToString();
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            if (!ValidateInputParameters())
                return;

            if (!DoFind())
            {
                lblStatus.Text = "Not Found";
                ReselectSelection();
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (!ValidateInputParameters())
                return;

            if (!DoFind())
            {
                lblStatus.Text = "Not Found";
                ReselectSelection();
                return;
            }
            _parent.CurrentChange.NewValues.Val = ddReplaceType.Text.Equals(FindTypeEntireValue)
                                                        ? txtReplace.Text
                                                        : Replace(_parent.edgv.CurrentCell.Value.ToString(),
                                                                  txtFind.Text, txtReplace.Text, rbMatchCase.Checked);

            _parent.SaveCurrentChangeAndGetNewChange(false);
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            if (!ValidateInputParameters())
                return;

            _findNext = false;
            int changeCount = 0;
            Guid waitKey = FrmWaitScreen.ShowMessage("Replace all");
            while (DoFind())
            {
                string originalValue = _parent.edgv.CurrentCell.Value.ToString();
                _parent.CurrentChange.NewValues.Val = ddReplaceType.Text.Equals(FindTypeEntireValue)
                                                            ? txtReplace.Text
                                                            : Replace(_parent.edgv.CurrentCell.Value.ToString(),
                                                                      txtFind.Text, txtReplace.Text, rbMatchCase.Checked);
                _parent.SaveCurrentChangeAndGetNewChange(true);

                if (!_parent.edgv.CurrentCell.Value.ToString().Equals(originalValue))
                {
                    changeCount++;
                    FrmWaitScreen.UpdateMessage(waitKey, string.Format("Replace all: {0} values updated", changeCount));
                }
            }
            FrmWaitScreen.HideMessage(waitKey);

                AryaTools.Instance.SaveChangesIfNecessary(false, false);
            lblStatus.Text = string.Format("{0} values updated.", changeCount);
            ReselectSelection();
        }

        private void ddFindType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetReplaceMode();
        }

        private bool DoFind()
        {
            int currentRow, currentColumn;
            if (rbSearchSelection.Checked)
            {
                if (_selectionCursor == _selection.Count)
                    _selectionCursor = 0;

                currentRow = _selection[_selectionCursor].RowIndex;
                currentColumn = _selection[_selectionCursor].ColumnIndex;
            }
            else
            {
                currentRow = _parent.edgv.CurrentCellAddress.Y;
                currentColumn = _parent.edgv.CurrentCellAddress.X;
            }

            string searchString = txtFind.Text;
            string currentValue = _findNext ? string.Empty : _parent.edgv[currentColumn, currentRow].Value.ToString();
            if (!rbMatchCase.Checked)
            {
                searchString = searchString.ToLower();
                currentValue = currentValue.ToLower();
            }
            bool isMatch = Match(searchString, currentValue);

            while (!isMatch)
            {
                if (rbSearchSelection.Checked)
                {
                    _selectionCursor++;
                    if (_selectionCursor == _selection.Count)
                        _selectionCursor = 0;

                    currentRow = _selection[_selectionCursor].RowIndex;
                    currentColumn = _selection[_selectionCursor].ColumnIndex;

                    currentValue = _selection[_selectionCursor].Value.ToString();
                }
                else
                {
                    currentColumn++;
                    if (currentColumn == _parent.edgv.ColumnCount)
                    {
                        currentRow++;
                        currentColumn = 0;
                    }

                    if (currentRow == _parent.edgv.RowCount)
                        currentRow = 0;

                    currentValue = _parent.edgv[currentColumn, currentRow].Value.ToString();
                }

                if (currentRow == _startRow && currentColumn == _startColumn)
                    return false;

                if (!rbMatchCase.Checked)
                    currentValue = currentValue.ToLower();
                isMatch = Match(searchString, currentValue);
            }
            _parent.DeselectCurrentSelection();

            _parent.edgv[currentColumn, currentRow].Selected = true;
            _parent.edgv.CurrentCell = _parent.edgv[currentColumn, currentRow];
            _parent.SelectionChanged();

            _findNext = true;
            return true;
        }

        private void FrmFindReplace_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    Close();
                    break;
            }
        }

        private void FrmFindReplace_Load(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;

            bool multipleSelection = _parent.edgv.SelectedCells.Count > 1;
            rbSearchSelection.Checked = multipleSelection;
            rbSearchAllCells.Checked = !multipleSelection;
            tblSelection.Enabled = multipleSelection;

            SetReplaceMode();
            GetCurrentSelection();
        }

        private void GetCurrentSelection()
        {
            _selection = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in _parent.edgv.SelectedCells)
                _selection.Add(cell);

            _selection.Sort(
                (x, y) =>
                x.RowIndex == y.RowIndex ? x.ColumnIndex.CompareTo(y.ColumnIndex) : x.RowIndex.CompareTo(y.RowIndex));

            _selectionCursor = 0;
        }

        private bool Match(string searchString, string currentValue)
        {
            switch (ddFindType.Text)
            {
                case "Contains":
                    return currentValue.Contains(searchString);

                case FindTypeEqualTo:
                    return currentValue.Equals(searchString);

                case "Starts with":
                    return currentValue.StartsWith(searchString);

                case "Ends with":
                    return currentValue.EndsWith(searchString);

                default:
                    return false;
            }
        }

        private static string Replace(string source, string find, string replace, bool matchCase)
        {
            if (string.IsNullOrEmpty(find))
                return source;

            return matchCase
                       ? source.Replace(find, replace)
                       : source.Replace(find, replace, StringComparison.CurrentCultureIgnoreCase);
        }

        private void ReselectSelection()
        {
            if (_selection == null || _selection.Count == 0)
                return;

            _parent.DeselectCurrentSelection();
            foreach (DataGridViewCell cell in _selection)
                cell.Selected = true;
        }

        private void SetReplaceMode()
        {
            bool entireValue = ddFindType.Text.Equals(FindTypeEqualTo);
            ddReplaceType.SelectedIndex = entireValue ? 1 : 0;
            ddReplaceType.Enabled = !entireValue;
        }

        private bool ValidateInputParameters()
        {
            if (string.IsNullOrEmpty(txtFind.Text))
            {
                lblStatus.Text = "Find text cannot be empty.";
                return false;
            }

            //if (rbReplaceEntireValue.Checked && rbPartialMatch.Checked)
            //{
            //    const string blankValue = "<blank>";
            //    string findValue = string.IsNullOrEmpty(txtFind.Text) ? blankValue : txtFind.Text;
            //    string replaceValue = string.IsNullOrEmpty(txtReplace.Text) ? blankValue : txtReplace.Text;

            //    string areYouSureMessage =
            //        string.Format(
            //            "This will replace the entire value if it contains '{0}' with '{1}'. Example: 'xyz{0}123' will be replaced by '{1}'. Are you sure?",
            //            findValue, replaceValue);
            //    DialogResult areYouSure = MessageBox.Show(areYouSureMessage, "Replace entire value?",
            //                                              MessageBoxButtons.YesNo);
            //    if (areYouSure == DialogResult.No)
            //        return false;
            //}

            if (_parent.edgv.SelectedCells.Count > 1)
                GetCurrentSelection();

            if (rbSearchSelection.Checked)
            {
                _startRow = _selection[0].RowIndex;
                _startColumn = _selection[0].ColumnIndex;
            }
            else
            {
                _startRow = _parent.edgv.CurrentCellAddress.Y;
                _startColumn = _parent.edgv.CurrentCellAddress.X;
            }

            return true;
        }

		#endregion Methods 
    }
}