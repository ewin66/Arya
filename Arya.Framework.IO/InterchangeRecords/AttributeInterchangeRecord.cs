using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("Attribute")]
    public class AttributeInterchangeRecord : InterchangeRecord
    {
        private string _attributeType = AttributeTypeEnum.Sku.ToString();
        private bool _isDefaultAttributeType = true;

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string AttributeName { get; set; }

        [Column(DbType = "VarChar(50)"), Category(WorkerBase.CaptionOptional)]
        public string AttributeType
        {
            get { return _attributeType; }
            set
            {
                _attributeType = value;
                _isDefaultAttributeType = false;
            }
        }

        [XmlIgnore]
        public bool IsDefaultAttributeType
        {
            get { return _isDefaultAttributeType; }
        }

        #region IComparable<AttributeInterchangeRecord> Members

        #endregion

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }

        public override string ToString()
        {
            var recordToString = AttributeName + '\t' + AttributeType;
            return recordToString;
        }
    }

    public class AttributeInterchangeRecordComparer : IEqualityComparer<AttributeInterchangeRecord>
    {
        #region IEqualityComparer<TaxonomyMetaDataInterchangeRecord> Members

        bool IEqualityComparer<AttributeInterchangeRecord>.Equals(AttributeInterchangeRecord x,
            AttributeInterchangeRecord y)
        {
            try
            {
                // Check whether the compared objects reference the same data.
                if (ReferenceEquals(x, y))
                    return true;

                // Check whether any of the compared objects is null.
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           

            return string.Equals(x.AttributeName.ToLower(), y.AttributeName.ToLower());
        }

        int IEqualityComparer<AttributeInterchangeRecord>.GetHashCode(AttributeInterchangeRecord obj)
        {
            try
            {
                unchecked
                {
                    if (obj.AttributeName != null)
                    {
                        var type = obj.AttributeType ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(type) || Data.AryaDb.Attribute.NonMetaAttributeTypes.ToString().Contains(type))
                            type = AttributeTypeEnum.NonMeta.ToString();
                        var h = (obj.AttributeName.ToLower() + type).GetHashCode();
                        return h;
                    }
                    return string.Empty.GetHashCode();
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        #endregion
    }
}