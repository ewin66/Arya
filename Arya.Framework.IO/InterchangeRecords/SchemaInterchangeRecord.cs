using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("Schema")]
    public class SchemaInterchangeRecord : InterchangeRecord
    {
        [Column(DbType = "NVarChar(4000)",CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string AttributeName { get; set; }

        [Column(DbType = "Decimal(6,3)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public decimal NavigationOrder { get; set; }

        [Column(DbType = "Decimal(6,3)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public decimal DisplayOrder { get; set; }

        [Column(DbType = "VarChar(50)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string DataType { get; set; }

        [Column(DbType = "bit"), Category(WorkerBase.CaptionOptional)]
        public bool InSchema { get; set; }


        public override string ToString()
        {
            return TaxonomyPath + '\t' + AttributeName + '\t' + NavigationOrder + '\t' + DisplayOrder + '\t' + DataType + '\t' + InSchema;
        }
        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
    }

    public class SchemaInterchangeRecordComparer : IEqualityComparer<SchemaInterchangeRecord>
    {
        #region IEqualityComparer<SchemaMetaDataInterchangeRecord> Members

        bool IEqualityComparer<SchemaInterchangeRecord>.Equals(SchemaInterchangeRecord x,
                                                                       SchemaInterchangeRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath) && string.Equals(x.AttributeName, y.AttributeName);
        }

        int IEqualityComparer<SchemaInterchangeRecord>.GetHashCode(SchemaInterchangeRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.AttributeName.GetHashCode();
                return h;
            }
        }

        #endregion
    }
}
