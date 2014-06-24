using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Framework.Extensions
{
    public static class CollectionExtensions
    {
        public static List<T> MakeList<T>(T itemOftype)
        {
            var newList = new List<T>();
            return newList;
        }

        public static List<T> SortList<T>(List<T> dataRows, string sortBy, ref string lastSortedAscendingBy)
        {
            var sortAscending = true;
            if (lastSortedAscendingBy != null && lastSortedAscendingBy.Equals(sortBy))
            {
                sortAscending = false;
                lastSortedAscendingBy = null;
            }
            else
                lastSortedAscendingBy = sortBy;

            var propInfo = typeof(T).GetProperty(sortBy.Replace(" ", String.Empty));

            var rows = (from row in dataRows
                        let currentIndex = dataRows.IndexOf(row)
                        let item = (propInfo.GetValue(row)??string.Empty).ToString()
                        let isBlank = String.IsNullOrWhiteSpace(item)
                        select new {row, currentIndex, item, isBlank}).ToList();

            return sortAscending
                       ? rows.OrderBy(i => i.isBlank).ThenBy(i => i.item).ThenBy(i => i.currentIndex).Select(i => i.row)
                             .ToList()
                       : rows.OrderBy(i => i.isBlank).ThenByDescending(i => i.item).ThenBy(i => i.currentIndex).Select(
                           i => i.row).ToList();
        }
    }
}
