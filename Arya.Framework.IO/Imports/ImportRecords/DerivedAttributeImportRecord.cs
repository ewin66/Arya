using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class DerivedAttributeImportRecord : ImportRecord
    {
        [Column(DbType = "VarChar(2303)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string Taxonomy { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string DerivedAttributeName { get; set; }

        [Column(DbType = "VarChar(8000)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string DerivedAttributeFormula { get; set; }

        [Column(DbType = "Int"), Category(WorkerBase.CAPTION_REQUIRED)]
        public int Length { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
    }
}
