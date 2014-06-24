using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("SkuAttributeValue")]
    public class SkuAttributeValueInterchangeRecord : InterchangeRecord
    {
        private string _value;
        private List<LanguageValue> _values = new List<LanguageValue>();

        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string ItemID { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string AttributeName { get; set; }

        [Column(DbType = "NVarChar(255)"), Category(WorkerBase.CaptionOptional)]
        public string Uom { get; set; }

        [Column(DbType = "NVarChar(4000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        [XmlIgnore]
        public string Value
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


        [XmlElement("Value")]
        public List<LanguageValue> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CaptionOptional)]
        public string Field1 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CaptionOptional)]
        public string Field2 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CaptionOptional)]
        public string Field3 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CaptionOptional)]
        public string Field4 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CaptionOptional)]
        public string Field5 { get; set; }


        //TODO: Add this as an option
        //        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CaptionOptional)]
        //        public bool BeforeEntity { get; set; }

        [Column(DbType = "Uniqueidentifier"), Category(WorkerBase.CaptionOptional)]
        public Guid? EntityID { get; set; }

        public override string ToString()
        {
            return ItemID + '\t' + AttributeName + '\t' + Value;
        }

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }
    }
}
