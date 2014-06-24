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
    [XmlType("SchemaMetaData")]
    public class SchemaMetaDataInterchangeRecord : InterchangeRecord
    {
        private string _schemaMetaAttributeValue;
        private List<LanguageValue> _values = new List<LanguageValue>();

        [Column(DbType = "NVarChar(4000)"), Category(WorkerBase.CaptionRequired)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string AttributeName { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false, Name = "MetaAttributeName"), Category(WorkerBase.CaptionRequired)]
        [XmlElement("MetaAttributeName")]
        public string SchemaMetaAttributeName { get; set; }

        [Column(DbType = "NVarChar(4000)", CanBeNull = false, Name = "MetaAttributeValue"), Category(WorkerBase.CaptionRequired)]
        [XmlIgnore]
        public string SchemaMetaAttributeValue
        {
            get
            {
                if (_schemaMetaAttributeValue == null && Values != null)
                    _schemaMetaAttributeValue = Values.GetValue();
                return _schemaMetaAttributeValue;
            }
            set
            {
                _schemaMetaAttributeValue = value;
                Values.AddValue(value);
            }
        }

        [XmlElement("Value")]
        public List<LanguageValue> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }

        public override string ToString() { return TaxonomyPath + '\t' + AttributeName + '\t' + SchemaMetaAttributeName + '\t' + SchemaMetaAttributeValue; }
    }

    public class SchemaMetaDataInterchangeRecordComparer : IEqualityComparer<SchemaMetaDataInterchangeRecord>
    {
        #region IEqualityComparer<SchemaMetaDataInterchangeRecord> Members

        bool IEqualityComparer<SchemaMetaDataInterchangeRecord>.Equals(SchemaMetaDataInterchangeRecord x,
                                                                       SchemaMetaDataInterchangeRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath) && string.Equals(x.AttributeName, y.AttributeName)
                   && string.Equals(x.SchemaMetaAttributeName, y.SchemaMetaAttributeName)
                   && string.Equals(x.SchemaMetaAttributeValue, y.SchemaMetaAttributeValue);
        }

        int IEqualityComparer<SchemaMetaDataInterchangeRecord>.GetHashCode(SchemaMetaDataInterchangeRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.AttributeName.GetHashCode()
                        ^ obj.SchemaMetaAttributeName.GetHashCode() ^ obj.SchemaMetaAttributeValue.GetHashCode();
                return h;
            }
        }

        #endregion
    }
}