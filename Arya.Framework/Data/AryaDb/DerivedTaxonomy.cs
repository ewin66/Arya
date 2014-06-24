namespace Arya.Framework.Data.AryaDb
{
    partial class DerivedTaxonomy : BaseEntity
    {
        public DerivedTaxonomy(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}