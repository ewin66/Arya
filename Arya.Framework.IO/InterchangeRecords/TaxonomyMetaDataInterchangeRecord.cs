using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;
using Arya.Framework.IO.InterchangeRecords;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("TaxonomyMetaData")]
    public class TaxonomyMetaDataInterchangeRecord : InterchangeRecord
    {
        private string _taxonomyMetaAttributeValue;
        private List<LanguageValue> _values;

        [Column(DbType = "NVarChar(4000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false,Name = "MetaAttributeName"), Category(WorkerBase.CaptionRequired)]
        [XmlElement("MetaAttributeName")]
        public string TaxonomyMetaAttributeName { get; set; }

        [Column(DbType = "NVarChar(4000)", CanBeNull = false, Name = "MetaAttributeValue"), Category(WorkerBase.CaptionRequired)]
        [XmlIgnore]
        public string TaxonomyMetaAttributeValue
        {
            get
            {
                if (_taxonomyMetaAttributeValue == null && Values != null)
                    _taxonomyMetaAttributeValue = Values.GetValue();
                return _taxonomyMetaAttributeValue;
            }
            set
            {
                _taxonomyMetaAttributeValue = value;
                Values.AddValue(value);
            }
        }

        [XmlElement("Value")]
        public List<LanguageValue> Values
        {
            get { return _values ?? (_values = new List<LanguageValue>()); }
            set { _values = value; }
        }

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }
        //TODO: Move tostring to base class 
        public override string ToString() { return TaxonomyPath + '\t' + TaxonomyMetaAttributeName + '\t' + TaxonomyMetaAttributeValue; }
    }

    public class TaxonomyMetaDataInterchangeRecordComparer : IEqualityComparer<TaxonomyMetaDataInterchangeRecord>
    {
        #region IEqualityComparer<TaxonomyMetaDataInterchangeRecord> Members

        bool IEqualityComparer<TaxonomyMetaDataInterchangeRecord>.Equals(TaxonomyMetaDataInterchangeRecord x,
                                                                         TaxonomyMetaDataInterchangeRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath)
                   && string.Equals(x.TaxonomyMetaAttributeName, y.TaxonomyMetaAttributeName)
                   && string.Equals(x.TaxonomyMetaAttributeValue, y.TaxonomyMetaAttributeValue);
        }

        int IEqualityComparer<TaxonomyMetaDataInterchangeRecord>.GetHashCode(TaxonomyMetaDataInterchangeRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.TaxonomyMetaAttributeName.GetHashCode()
                        ^ obj.TaxonomyMetaAttributeValue.GetHashCode();
                return h;
            }
        }

        #endregion
    }
}
