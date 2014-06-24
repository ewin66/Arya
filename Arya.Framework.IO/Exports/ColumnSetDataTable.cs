using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Arya.Framework.IO.Exports
{
    class ColumnSetDataTable : DataTable
    {
        private int _columnSetCount;
        private readonly List<string> _columnSetNames = new List<string>();

        public ColumnSetDataTable(string tableName)
            : base(tableName)
        {
            // add static columns
            Columns.Add("Item Id", typeof(String));
            Columns.Add("Taxonomy", typeof(String));
            Columns.Add("Node Type", typeof(String));
        }

        public int[] InitGlobals(string[] attHeaders)
        {
            var attCounts = new int[attHeaders.Count()];

            // create attribute columns
            for (int i = 0; i < attHeaders.Length; i++)
            {
                var attribute = attHeaders[i];
                var parts = attribute.Split(':');
                int noOfHeaders = 0;
                if (parts.Count() > 1)
                {
                    Int32.TryParse(parts[1].Trim(), out noOfHeaders);
                }

                if (parts.Count() == 2 && noOfHeaders > 0)
                {
                    for (var j = 0; j < noOfHeaders; j++)
                    {
                        Columns.Add(parts[0].Trim() + (j + 1), typeof(string));
                    }
                    attCounts[i] = noOfHeaders;
                }
                else
                {
                    Columns.Add(attribute, typeof(string));
                    attCounts[i] = 0;
                }
            }
            return attCounts;
        }

        public void InitColumnSet(List<string> columnSetNames)
        {
            _columnSetNames.AddRange(columnSetNames);
        }

        private void AddColumnSet()
        {
            _columnSetCount++;
            foreach (var columnName in _columnSetNames)
            {
                Columns.Add(new DataColumn(columnName + _columnSetCount, typeof(string)));
            }
        }

        public void WriteDataRow(List<string> outputElements)
        {
            // if we need more room in the table to accomodate the data, add column sets.
            while (outputElements.Count > Columns.Count)
            {
                AddColumnSet();
            }

            var newRow = NewRow();
            for (int i = 0; i < outputElements.Count; i++)
            {
                newRow[i] = outputElements[i];
            }
            Rows.Add(newRow);
        }
    }
}