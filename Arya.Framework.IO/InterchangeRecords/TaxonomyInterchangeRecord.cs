using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("Taxonomy")]
    public class TaxonomyInterchangeRecord : InterchangeRecord
    {
        private List<LanguageValue> _taxonomies = new List<LanguageValue>();
        private string _taxonomyPath;

        [Column(DbType = "NVarChar(4000)",CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        [XmlIgnore]
        public string TaxonomyPath
        {
            get
            {
                if (_taxonomyPath == null && Values != null)
                    _taxonomyPath = Values.GetValue();
                return _taxonomyPath;
            }
            set
            {
                _taxonomyPath = value;
                Values.AddValue(value);
            }
        }

        [XmlElement("Value")]
        public List<LanguageValue> Values
        {
            get { return _taxonomies; }
            set { _taxonomies = value; }
        }

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }
        public override string ToString() { return TaxonomyPath; }
    }

    public class TaxonomyInterchangeRecordComparer : IEqualityComparer<TaxonomyInterchangeRecord>
    {
        #region IEqualityComparer<TaxonomyInterchangeRecord> Members

        bool IEqualityComparer<TaxonomyInterchangeRecord>.Equals(TaxonomyInterchangeRecord x,
                                                                 TaxonomyInterchangeRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath.ToLower(), y.TaxonomyPath.ToLower());
        }

        int IEqualityComparer<TaxonomyInterchangeRecord>.GetHashCode(TaxonomyInterchangeRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.ToLower().GetHashCode() ^ obj.TaxonomyPath.ToLower().GetHashCode() ^ obj.TaxonomyPath.ToLower().GetHashCode();
                return h;
            }
        }

        #endregion
    }
}
