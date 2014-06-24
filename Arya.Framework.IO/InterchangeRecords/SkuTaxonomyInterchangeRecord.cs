using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using Arya.Framework.Common;
using Arya.Framework.IO.InterchangeRecords;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    [XmlType("SkuTaxonomy")]
    public class SkuTaxonomyInterchangeRecord : InterchangeRecord
    {
        private bool _isPrimary=true;

        [Column(DbType = "NVarChar(4000)"), Category(WorkerBase.CaptionOptional)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string ItemID { get; set; }

        public bool IsPrimary
        {
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }
        public override string ToString()
        {
            return TaxonomyPath + '\t' + (string.IsNullOrEmpty(ItemID)? string.Empty : ItemID) ; ;
        }
        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return
                string.Format(
                    @"CREATE NONCLUSTERED INDEX [IDX_ItemID] ON [{0}].[dbo].[{1}] 
                        (
	                        [ItemID] ASC
                        )WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]", databaseName, tableName);
        }
    }
}
