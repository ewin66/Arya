namespace Arya.Framework.Data.AryaDb
{
    public partial class SkuGroup : BaseEntity
    {
        public SkuGroup(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}