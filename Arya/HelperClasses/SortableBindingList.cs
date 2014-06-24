using System;
using System.Collections.Generic;
using System.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;

namespace Arya.HelperClasses
{
    public class SortableBindingList<T> : BindingList<T> where T : class
    {
        private bool isSorted;
        private ListSortDirection sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor sortProperty;

        public SortableBindingList()
        {
   
        }

        public SortableBindingList(IEnumerable<T> source)
        {
            this.AddRange(source);
        }

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirection; }
        }

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortProperty; }
        }

        /// <summary>
        /// Removes any sort applied with ApplySortCore if sorting is implemented
        /// </summary>
        protected override void RemoveSortCore()
        {
            sortDirection = ListSortDirection.Ascending;
            sortProperty = null;
        }

        /// <summary>
        /// Sorts the items if overridden in a derived class
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="direction"></param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            sortProperty = prop;
            sortDirection = direction;

            var list = Items as List<T>;
            if (list == null) return;

            list.Sort(Compare);

            isSorted = true;
            //fire an event that the list has been changed.
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }


        private int Compare(T lhs, T rhs)
        {
            var result = OnComparison(lhs, rhs);
            //invert if descending
            if (sortDirection == ListSortDirection.Descending)
                result = -result;
            return result;
        }

        private int OnComparison(T lhs, T rhs)
        {
            object lhsValue = lhs == null ? null : sortProperty.GetValue(lhs);
            object rhsValue = rhs == null ? null : sortProperty.GetValue(rhs);
            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0 : -1; //nulls are equal
            }
            if (rhsValue == null)
            {
                return 1; //first has value, second doesn't
            }
            var comparable = lhsValue as IComparable;
            if (comparable != null)
            {
                return (comparable).CompareTo(rhsValue);
            }

            return lhsValue.Equals(rhsValue) ? 0 : lhsValue.ToString().CompareTo(rhsValue.ToString());
            //not comparable, compare ToString
        }
    }
}
