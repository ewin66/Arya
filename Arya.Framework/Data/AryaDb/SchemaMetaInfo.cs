using System.Linq;

namespace Arya.Framework.Data.AryaDb
{
    partial class SchemaMetaInfo : BaseEntity
    {
        public SchemaMetaInfo(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public SchemaMetaData SchemaMetaData
        {
            get { return SchemaMetaDatas.FirstOrDefault(md => md.Active); }
        }
    }
}