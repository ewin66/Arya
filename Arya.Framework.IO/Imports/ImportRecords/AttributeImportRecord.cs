using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class AttributeImportRecord : ImportRecord, IComparable<AttributeImportRecord>
    {
        [Column(DbType = "NVarChar(255)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string AttributeName { get; set; }

        [Column(DbType = "VarChar(50)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string AttributeType { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }

        public int CompareTo(AttributeImportRecord other)
        {
            return string.CompareOrdinal(AttributeName, other.AttributeName);
        }

        public override string ToString()
        {
            string recordToString = AttributeName + '\t' + AttributeType;
            return recordToString;
        }

        
    }

    public class AttributeImportRecordComparer : IEqualityComparer<AttributeImportRecord>
    {
        bool IEqualityComparer<AttributeImportRecord>.Equals(AttributeImportRecord x, AttributeImportRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.AttributeName, y.AttributeName);
        }

        int IEqualityComparer<AttributeImportRecord>.GetHashCode(AttributeImportRecord obj)
        {
            return obj.AttributeName.GetHashCode();
        }
    }
}
