namespace Arya.Data
{
    using System.Collections.Generic;

    partial class DerivedAttribute
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }

    public class DerivedAttributeComparer : IEqualityComparer<DerivedAttribute>
    {
        public bool Equals(DerivedAttribute x, DerivedAttribute y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.ID == y.ID;
        }

        public int GetHashCode(DerivedAttribute obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            var hashID = obj.ID.GetHashCode();

            return hashID;
        }
    }
}