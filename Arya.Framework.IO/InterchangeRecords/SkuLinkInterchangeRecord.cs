using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("SkuLink")]
    public class SkuLinkInterchangeRecord : InterchangeRecord
    {
        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string FromItemID { get; set; }

        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string ToItemID { get; set; }

        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string LinkType { get; set; }

        public override string ToString()
        {
            return FromItemID + '\t' + LinkType + '\t' + ToItemID;
        }

        public override string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }
    }
}