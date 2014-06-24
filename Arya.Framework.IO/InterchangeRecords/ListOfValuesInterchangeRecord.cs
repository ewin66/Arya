using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("RestrictedLov")]
    public class ListOfValuesInterchangeRecord : InterchangeRecord
    {
        private List<LanguageValue> _restrictedLov = new List<LanguageValue>();
        private string _value;

        [Column(DbType = "NVarChar(4000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string AttributeName { get; set; }

        [Column(DbType = "NVarChar(4000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        [XmlIgnore]
        public string Lov
        {
            get
            {
                if (_value == null && Values != null)
                    _value = Values.GetValue();
                return _value;
            }
            set
            {
                _value = value;
                Values.AddValue(value);
            }
        }

        [Category(WorkerBase.CaptionOptional)]
        public string EnrichmentImage { get; set; }

        [Category(WorkerBase.CaptionOptional)]
        public string EnrichmentCopy { get; set; }

        //Had ot make the type string coz other wise it will default it to 0
        [Column(DbType = "int"),Category(WorkerBase.CaptionOptional)]
        public string DisplayOrder { get; set; }

        [XmlElement("Value")]
        public List<LanguageValue> Values
        {
            get { return _restrictedLov; }
            set { _restrictedLov = value; }
        }

        public override string ToString() { return TaxonomyPath + '\t' + AttributeName + '\t' + Lov + '\t' + EnrichmentImage + '\t' + EnrichmentCopy; }
        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }
    }

    public class ListOfValuesInterchangeRecordComparer : IEqualityComparer<ListOfValuesInterchangeRecord>
    {
        #region IEqualityComparer<ListOfValuesInterchangeRecord> Members

        bool IEqualityComparer<ListOfValuesInterchangeRecord>.Equals(ListOfValuesInterchangeRecord x,
                                                                     ListOfValuesInterchangeRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath) && string.Equals(x.AttributeName, y.AttributeName)
                   && string.Equals(x.Lov, y.Lov);
        }

        int IEqualityComparer<ListOfValuesInterchangeRecord>.GetHashCode(ListOfValuesInterchangeRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.AttributeName.GetHashCode() ^ obj.Lov.GetHashCode();
                return h;
            }
        }

        #endregion
    }
}