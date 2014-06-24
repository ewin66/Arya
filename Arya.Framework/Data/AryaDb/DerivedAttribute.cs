namespace Arya.Framework.Data.AryaDb
{
    partial class DerivedAttribute : BaseEntity
    {
        public DerivedAttribute(AryaDbDataContext parentContext, bool initialize = true)
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}