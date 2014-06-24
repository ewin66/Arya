using System.Linq;

namespace Arya.Framework.Data.AryaDb
{
    partial class TaxonomyMetaInfo : BaseEntity
    {
        public TaxonomyMetaInfo(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public TaxonomyMetaData TaxonomyMetaData
        {
            get { return TaxonomyMetaDatas.FirstOrDefault(md => md.Active); }
        }
    }
}