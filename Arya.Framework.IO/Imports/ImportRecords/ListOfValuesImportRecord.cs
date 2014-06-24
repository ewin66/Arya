using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class ListOfValuesImportRecord : ImportRecord
    {
        [Category(WorkerBase.CAPTION_REQUIRED)]
        public string TaxonomyPath { get; set; }

        [Category(WorkerBase.CAPTION_REQUIRED)]
        public string AttributeName { get; set; }

        [Category(WorkerBase.CAPTION_REQUIRED)]
        public string Value { get; set; }

        [Category(WorkerBase.CAPTION_OPTIONAL)]
        public string EnrichmentImage { get; set; }

        [Category(WorkerBase.CAPTION_OPTIONAL)]
        public string EnrichmentCopy { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
    }
}
