namespace Arya.Framework.Data.AryaDb
{
    public partial class DerivedEntityData : BaseEntity
    {
        public DerivedEntityData(AryaDbDataContext parentContext, bool initialize = true)
            : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}