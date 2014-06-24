using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("DerivedAttribute")]
    public class DerivedAttributeInterchangeRecord : InterchangeRecord
    {
        [Column(DbType = "VarChar(4000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string DerivedAttributeName { get; set; }

        [Column(DbType = "VarChar(8000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string DerivedAttributeExpression { get; set; }

        [Column(DbType = "Int", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public int MaxResultLength { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }

        public override string ToString()
        {
            return TaxonomyPath + '\t' + DerivedAttributeName + '\t' + DerivedAttributeExpression + '\t'
                   + MaxResultLength;
        }
    }

    public class DerivedAttributeInterchangeRecordComparer : IEqualityComparer<DerivedAttributeInterchangeRecord>
    {
        #region IEqualityComparer<TaxonomyMetaDataInterchangeRecord> Members

        bool IEqualityComparer<DerivedAttributeInterchangeRecord>.Equals(DerivedAttributeInterchangeRecord x,
                                                                         DerivedAttributeInterchangeRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.DerivedAttributeName, y.DerivedAttributeName)
                   && string.Equals(x.TaxonomyPath, y.TaxonomyPath);
        }

        int IEqualityComparer<DerivedAttributeInterchangeRecord>.GetHashCode(DerivedAttributeInterchangeRecord obj)
        {
            unchecked
            {
                var h = obj.DerivedAttributeName.GetHashCode() ^ obj.TaxonomyPath.GetHashCode();
                return h;
            }
        }

        #endregion
    }
}
