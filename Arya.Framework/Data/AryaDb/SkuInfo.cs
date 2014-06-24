namespace Arya.Framework.Data.AryaDb
{
    public partial class SkuInfo : BaseEntity
    {
        public SkuInfo(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}