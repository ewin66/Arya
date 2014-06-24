using Arya.Framework.Common.ComponentModel;

namespace Arya.Framework4.ComponentModel
{
    using System.ComponentModel;
    using System.Linq;
    using Arya.Data;

    [TypeConverter(typeof(TaxonomyConverter))]
    public class ExtendedTaxonomyInfo
    {
        [Browsable(false)]
        public TaxonomyInfo Taxonomy { get; set; }

        [Browsable(false)]
        public bool IsLeafNode
        {
            get { return Taxonomy.IsLeafNode; }
        }

        [Browsable(false)]
        public int SkuCount
        {
            get
            {
                if(!Taxonomy.IsLeafNode)
                {
                    return Taxonomy.AllLeafChildren.Sum(p => p.SkuCount) + Taxonomy.SkuCount;
                }
                return Taxonomy.SkuCount;
            }
        }

        [Browsable(true),DefaultValue(true)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IsSelected { get; set; }

        public ExtendedTaxonomyInfo(TaxonomyInfo taxonomyInfo)
        {
            Taxonomy = taxonomyInfo;
            IsSelected = taxonomyInfo != null;
        }

        public override string ToString()
        {
            return Taxonomy == null ? string.Empty : Taxonomy.ToString();
        }
    }
}
