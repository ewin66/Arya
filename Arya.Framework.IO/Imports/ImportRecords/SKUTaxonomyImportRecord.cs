using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class SKUTaxonomyImportRecord : ImportRecord
    {
        [Category(WorkerBase.CAPTION_OPTIONAL)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string ItemID { get; set; }

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
