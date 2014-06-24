using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class TaxonomyMetaDataImportRecord: ImportRecord
    {
        [Column(DbType = "NVarChar(4000)",CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string TaxonomyPath { get; set; }
       
        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string MetaAttributeName { get; set; }

        [Column(DbType = "NVarChar(4000)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string MetaAttributeValue { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
        public override string ToString()
        {
            return TaxonomyPath + '\t' + MetaAttributeName + '\t' + MetaAttributeValue;
        }
    }

    public class TaxonomyMetaDataImportRecordComparer : IEqualityComparer<TaxonomyMetaDataImportRecord>
    {
        bool IEqualityComparer<TaxonomyMetaDataImportRecord>.Equals(TaxonomyMetaDataImportRecord x, TaxonomyMetaDataImportRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath)  &&
                   string.Equals(x.MetaAttributeName, y.MetaAttributeName) && string.Equals(x.MetaAttributeValue, y.MetaAttributeValue);
        }

        int IEqualityComparer<TaxonomyMetaDataImportRecord>.GetHashCode(TaxonomyMetaDataImportRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.MetaAttributeName.GetHashCode() ^ obj.MetaAttributeValue.GetHashCode();
                return h;
            }
        }
    }
}
