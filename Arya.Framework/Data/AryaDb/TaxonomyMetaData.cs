namespace Arya.Framework.Data.AryaDb
{
    partial class TaxonomyMetaData : BaseEntity
    {
        public TaxonomyMetaData(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #region Methods 

        public override string ToString() { return Value; }

        partial void OnActiveChanged()
        {
            if (Active)
                return;
            var parentContext = ParentContext;
            if (parentContext == null)
                return;
            AryaDbDataContext.DefaultDeletedTableValue(this, parentContext.CurrentUser.ID);
        }

        #endregion Methods
    }
}