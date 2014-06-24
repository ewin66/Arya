using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using LinqKit;

namespace Arya.Framework.GUI.UserControls
{
	public sealed class CustomDataGridView : DataGridView
	{
	    private int lastSelectedRowIndex;

	    public int LastSelectedRowIndex
	    {
	        get { return lastSelectedRowIndex; }
            set { lastSelectedRowIndex = value; }
	    }

		public CustomDataGridView()
		{
			DoubleBuffered = true;
		    lastSelectedRowIndex = 0;
		}

		protected override void OnCursorChanged(EventArgs e)
		{
		    base.OnCursorChanged(e);
		    //DoubleBuffered = Cursor == Cursors.Default;
		}

		#region Methods (3) 

		// Protected Methods (3) 

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessDataGridViewKey(KeyEventArgs e)
		{
            if (SelectedCells.Count != 0)
            {
                if (e.KeyCode == Keys.Down && e.Control && CurrentCell != null)
                {
                    ProcessNewCtrlDown(e.Shift);
                    return true;
                }

                if (e.KeyCode == Keys.Up && e.Control && CurrentCell != null)
                {
                    ProcessNewCtrlUp(e.Shift);
                    return true;
                }
            }

		    return base.ProcessDataGridViewKey(e);
		}

        protected override void OnSelectionChanged(EventArgs e)
        {
            if (SelectedCells.Count > 0 && lastSelectedRowIndex != SelectedCells[0].RowIndex)
            {
                var currentRowIndex = SelectedCells[0].RowIndex;
                if (lastSelectedRowIndex > Rows.Count - 1)
                {
                    lastSelectedRowIndex = currentRowIndex;
                }
                InvalidateRow(lastSelectedRowIndex);
                lastSelectedRowIndex = currentRowIndex;
                InvalidateRow(currentRowIndex);
            }
            base.OnSelectionChanged(e);
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (CurrentRow != null && e.ColumnIndex > -1 && CurrentRow.Index == e.RowIndex)
            {
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
            }

            base.OnCellPainting(e);
        }

		private void ProcessNewCtrlDown(bool shiftKey = false)
		{
			DataGridViewCell lastSelectedCell =
				SelectedCells.Cast<DataGridViewCell>().Where(p => p.OwningRow.Frozen == false).OrderByDescending(
					p => p.RowIndex).FirstOrDefault();

			if (lastSelectedCell == null)
				return;
			bool reverse = false;

			if (CurrentCell.RowIndex == lastSelectedCell.RowIndex && SelectedCells.Count > 1)
			{
				lastSelectedCell = SelectedCells.Cast<DataGridViewCell>().OrderBy(p => p.RowIndex).FirstOrDefault();
				if (lastSelectedCell == null)
					return;
				reverse = true;
			}

			int currentColumnIndex = lastSelectedCell.ColumnIndex;
			int currentRowIndex = lastSelectedCell.RowIndex;


			DataGridViewCell nextCell = Rows.Count == lastSelectedCell.RowIndex + 1
											? null
											: Rows[lastSelectedCell.RowIndex + 1].Cells[currentColumnIndex];

			if (nextCell == null)
				return;

			DataGridViewCell targetCell;

			if ((lastSelectedCell.Value != null && nextCell.Value != null) &&
				(lastSelectedCell.Value.ToString() == string.Empty ||
				 (lastSelectedCell.Value.ToString() != string.Empty && nextCell.Value.ToString() == string.Empty)))
			{
				targetCell = nextCell.Value.ToString() == string.Empty
								 ? Rows.Cast<DataGridViewRow>().Where(
									 p =>
									 p.Cells[currentColumnIndex].Value != null && p.Frozen == false &&
									 p.Cells[currentColumnIndex].RowIndex > currentRowIndex &&
									 p.Cells[currentColumnIndex].Value.ToString() != string.Empty).Select(
										 p => p.Cells[currentColumnIndex]).FirstOrDefault()
								 : nextCell;

				//no non blank exists, so goto the last cell in that column
				targetCell = targetCell ?? Rows[Rows.Count - 1].Cells[currentColumnIndex];

				targetCell = targetCell ??
							 Rows.Cast<DataGridViewRow>().OrderByDescending(p => p.Cells[currentColumnIndex].RowIndex).
								 First().Cells[currentColumnIndex];
			}

			else
			{
				targetCell =
					Rows.Cast<DataGridViewRow>().Where(
						p =>
						p.Cells[currentColumnIndex].Value != null && p.Frozen == false &&
						p.Cells[currentColumnIndex].RowIndex > currentRowIndex &&
						p.Cells[currentColumnIndex].Value.ToString() == string.Empty).Select(
							p => p.Cells[currentColumnIndex]).FirstOrDefault();

				targetCell = targetCell == null
								 ? Rows[Rows.Count - 1].Cells[currentColumnIndex]
								 : Rows[targetCell.RowIndex - 1].Cells[currentColumnIndex];
			}

            if (shiftKey)
            {
                int selectedRowIndex = reverse
                                           ? SelectedCells.Cast<DataGridViewCell>().OrderByDescending(p => p.RowIndex).
                                                 First().RowIndex
                                           : SelectedCells.Cast<DataGridViewCell>().OrderBy(p => p.RowIndex).First().
                                                 RowIndex;

                List<DataGridViewCell> toBeSelectedRows = reverse
                                                              ? Rows.Cast<DataGridViewRow>().Where(
                                                                  p =>
                                                                  p.Cells[currentColumnIndex].Value != null &&
                                                                  p.Frozen == false &&
                                                                  p.Cells[currentColumnIndex].RowIndex >=
                                                                  selectedRowIndex &&
                                                                  p.Cells[currentColumnIndex].RowIndex <=
                                                                  targetCell.RowIndex).Select(
                                                                      p => p.Cells[currentColumnIndex]).
                                                                    OrderByDescending(p => p.RowIndex).ToList()
                                                              : Rows.Cast<DataGridViewRow>().Where(
                                                                  p =>
                                                                  p.Cells[currentColumnIndex].Value != null &&
                                                                  p.Frozen == false &&
                                                                  p.Cells[currentColumnIndex].RowIndex >=
                                                                  selectedRowIndex &&
                                                                  p.Cells[currentColumnIndex].RowIndex <=
                                                                  targetCell.RowIndex).Select(
                                                                      p => p.Cells[currentColumnIndex]).OrderBy(
                                                                          p => p.RowIndex).ToList();

                SelectedCells.Cast<DataGridViewCell>().Where(p => p.RowIndex < targetCell.RowIndex).ForEach(
                    p => p.Selected = p.RowIndex > CurrentCell.RowIndex);
                toBeSelectedRows.ForEach(a => a.Selected = CurrentCell.RowIndex <= a.RowIndex);

                targetCell.Selected = true;

                if (FirstDisplayedScrollingRowIndex + DisplayedRowCount(false) <= targetCell.RowIndex)
                {
                    FirstDisplayedScrollingRowIndex = targetCell.RowIndex - DisplayedRowCount(false) + 1;
                }
            }
            else
            {
                CurrentCell = targetCell;
            }
		}

		private void ProcessNewCtrlUp(bool shiftKey = false)
		{
			DataGridViewCell firstSelectedCell =
				SelectedCells.Cast<DataGridViewCell>().Where(p => p.OwningRow.Frozen == false).OrderBy(p => p.RowIndex).
					FirstOrDefault();

			if (firstSelectedCell == null)
				return;

			bool reverse = false;

			if (CurrentCell.RowIndex == firstSelectedCell.RowIndex && SelectedCells.Count > 1)
			{
				firstSelectedCell =
					SelectedCells.Cast<DataGridViewCell>().Where(p => p.OwningRow.Frozen == false).OrderByDescending(
						p => p.RowIndex).FirstOrDefault();
				if (firstSelectedCell == null)
					return;
				reverse = true;
			}

			int currentColumnIndex = firstSelectedCell.ColumnIndex;
			int currentRowIndex = firstSelectedCell.RowIndex;

			DataGridViewCell beforeCell = firstSelectedCell.RowIndex <= 0
											  ? null
											  : Rows[firstSelectedCell.RowIndex - 1].Cells[currentColumnIndex];
			if (beforeCell == null)
				return;

			DataGridViewCell targetCell;

			if ((firstSelectedCell.Value != null && beforeCell.Value != null) &&
				(firstSelectedCell.Value.ToString() == string.Empty ||
				 (firstSelectedCell.Value.ToString() != string.Empty && beforeCell.Value.ToString() == string.Empty)))
			{
				targetCell = beforeCell.Value.ToString() == string.Empty
								 ? Rows.Cast<DataGridViewRow>().Reverse().Where(
									 p =>
									 p.Cells[currentColumnIndex].Value != null && p.Frozen == false &&
									 p.Cells[currentColumnIndex].RowIndex < currentRowIndex &&
									 p.Cells[currentColumnIndex].Value.ToString() != string.Empty).Select(
										 p => p.Cells[currentColumnIndex]).FirstOrDefault()
								 : beforeCell;

				//no non blank exists, so goto the last cell in that column
				targetCell = targetCell ?? Rows[0].Cells[currentColumnIndex];
			}
			else
			{
				targetCell =
					Rows.Cast<DataGridViewRow>().Reverse().Where(
						p =>
						p.Cells[currentColumnIndex].Value != null && p.Frozen == false &&
						p.Cells[currentColumnIndex].RowIndex < currentRowIndex &&
						p.Cells[currentColumnIndex].Value.ToString() == string.Empty).Select(
							p => p.Cells[currentColumnIndex]).FirstOrDefault();

				//targetCell = targetCell == null ? Rows[0].Cells[currentColumnIndex] : Rows[targetCell.RowIndex + 1].Cells[currentColumnIndex];
				targetCell = targetCell == null
								 ? Rows.Cast<DataGridViewRow>().Where(p => p.Frozen == false).Select(
									 p => p.Cells[currentColumnIndex]).FirstOrDefault()
								 : Rows[targetCell.RowIndex + 1].Cells[currentColumnIndex];
			}


			if(targetCell == null) return;


            if (shiftKey)
            {
                int selectedRowIndex = reverse
                                           ? SelectedCells.Cast<DataGridViewCell>().OrderByDescending(p => p.RowIndex).
                                                 First().RowIndex
                                           : SelectedCells.Cast<DataGridViewCell>().OrderBy(p => p.RowIndex).First().
                                                 RowIndex;

                List<DataGridViewCell> toBeSelectedRows = reverse
                                                              ? Rows.Cast<DataGridViewRow>().Where(
                                                                  p =>
                                                                  p.Frozen == false &&
                                                                  p.Cells[currentColumnIndex].RowIndex <=
                                                                  selectedRowIndex &&
                                                                  p.Cells[currentColumnIndex].RowIndex >=
                                                                  targetCell.RowIndex).Select(
                                                                      p => p.Cells[currentColumnIndex]).OrderBy(
                                                                          p => p.RowIndex).ToList()
                                                              : Rows.Cast<DataGridViewRow>().Where(
                                                                  p =>
                                                                  p.Frozen == false &&
                                                                  p.Cells[currentColumnIndex].RowIndex <=
                                                                  selectedRowIndex &&
                                                                  p.Cells[currentColumnIndex].RowIndex >=
                                                                  targetCell.RowIndex).Select(
                                                                      p => p.Cells[currentColumnIndex]).
                                                                    OrderByDescending(p => p.RowIndex).ToList();

                toBeSelectedRows.ForEach(a => a.Selected = CurrentCell.RowIndex >= a.RowIndex);

                targetCell.Selected = true;

                if (FirstDisplayedScrollingRowIndex > targetCell.RowIndex)
                {
                    FirstDisplayedScrollingRowIndex = targetCell.RowIndex;
                }
            }
            else
            {
                CurrentCell = targetCell;
            }
		}

		#endregion Methods 
    }
}