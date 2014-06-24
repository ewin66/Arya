using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class SchemaImportRecord : ImportRecord
    {
        [Category(WorkerBase.CAPTION_REQUIRED)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "NVarChar(255)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string AttributeName { get; set; }

        [Column(DbType = "Decimal(6,3)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public decimal NavigationOrder { get; set; }

        [Column(DbType = "VarChar(50)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public decimal DisplayOrder { get; set; }

        [Column(DbType = "VarChar(50)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string DataType { get; set; }

        [Category(WorkerBase.CAPTION_OPTIONAL)]
        public bool InSchema { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
    }
}
