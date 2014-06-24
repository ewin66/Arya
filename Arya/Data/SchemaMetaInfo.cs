using System.Linq;

namespace Arya.Data
{
    partial class SchemaMetaInfo
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

        public SchemaMetaData SchemaMetaData
        {
            get { return SchemaMetaDatas.Where(md => md.Active).FirstOrDefault(); }

        }
    }
}