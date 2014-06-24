using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class TaxonomyImportRecord
    {
         [Column(CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string TaxonomyPath { get; set; }
    }
    public class TaxonomyImportRecordComparer : IEqualityComparer<TaxonomyImportRecord>
    {
        bool IEqualityComparer<TaxonomyImportRecord>.Equals(TaxonomyImportRecord x, TaxonomyImportRecord y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return string.Equals(x.TaxonomyPath, y.TaxonomyPath) &&
                   string.Equals(x.TaxonomyPath, y.TaxonomyPath) && string.Equals(x.TaxonomyPath, y.TaxonomyPath);
        }

        int IEqualityComparer<TaxonomyImportRecord>.GetHashCode(TaxonomyImportRecord obj)
        {
            unchecked
            {
                var h = obj.TaxonomyPath.GetHashCode() ^ obj.TaxonomyPath.GetHashCode() ^ obj.TaxonomyPath.GetHashCode();
                return h;
            }
        }
    }
}
