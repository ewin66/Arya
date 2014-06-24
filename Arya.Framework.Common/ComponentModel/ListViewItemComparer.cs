using System;
using System.Collections;
using System.Windows.Forms;

namespace Arya.Framework.Common.ComponentModel
{
    public class ListViewItemComparer : IComparer
    {
        #region Fields (3)

        readonly bool _ascending;
        readonly int _column;
        bool _isNumeric;
        readonly bool _sortSelected;

        #endregion Fields

        #region Constructors (1)

        public ListViewItemComparer(int column, bool ascending, bool isNumeric, bool sortSelected)
        {
            _column = column;
            _sortSelected = sortSelected;
            _ascending = ascending;
            _isNumeric = isNumeric;
        }

        #endregion Constructors



        #region IComparer Members

        public int Compare(object o1, object o2)
        {
            var x = (ListViewItem) o1;
            var y = (ListViewItem) o2;

            if (_sortSelected && x.Selected != y.Selected)
                return y.Selected.CompareTo(x.Selected); //selected on top

            if (_isNumeric)
                try
                {
                    double xValue = double.Parse(x.SubItems[_column].Text);
                    double yValue = double.Parse(y.SubItems[_column].Text);

                    return _ascending ? xValue.CompareTo(yValue) : yValue.CompareTo(xValue);
                }
                catch
                {
                    _isNumeric = false;
                }

            return _ascending
                       ? String.Compare(x.SubItems[_column].Text, y.SubItems[_column].Text)
                       : String.Compare(y.SubItems[_column].Text, x.SubItems[_column].Text);
        }

        #endregion
    }
}