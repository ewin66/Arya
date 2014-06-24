using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class SchemaMetaDataImportRecord : ImportRecord
    {
        [Category(WorkerBase.CAPTION_REQUIRED)]
        public string TaxonomyPath { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string AttributeName { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string MetaAttributeName { get; set; }

        [Column(DbType = "NVarChar(4000)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string MetaValue { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
        public override string ToString()
        {
           
            return TaxonomyPath + '\t' + AttributeName + '\t' + MetaAttributeName + '\t' + MetaValue;;
        }
    }

    public class SchemaMetaDataImportRecordComparer : IEqualityComparer<SchemaMetaDataImportRecord>
    {
        bool IEqualityComparer<SchemaMetaDataImportRecord>.Equals(SchemaMetaDataImportRecord x, SchemaMetaDataImportRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath) && string.Equals(x.AttributeName, y.AttributeName) &&
                   string.Equals(x.MetaAttributeName, y.MetaAttributeName) && string.Equals(x.MetaValue, y.MetaValue);
        }

        int IEqualityComparer<SchemaMetaDataImportRecord>.GetHashCode(SchemaMetaDataImportRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.AttributeName.GetHashCode() ^
                        obj.MetaAttributeName.GetHashCode() ^ obj.MetaValue.GetHashCode();
                return h;
            }
        }
    }
}
