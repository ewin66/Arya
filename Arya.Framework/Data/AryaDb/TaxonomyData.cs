namespace Arya.Framework.Data.AryaDb
{
    public partial class TaxonomyData : BaseEntity
    {
        public TaxonomyData(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

        public TaxonomyInfo ParentTaxonomyInfo
        {
            get { return TaxonomyInfo1; }
            set { TaxonomyInfo1 = value; }
        }

        partial void OnActiveChanged()
        {
            if (Active)
                return;
            var parentContext = ParentContext;
            if (parentContext == null)
                return;
            AryaDbDataContext.DefaultDeletedTableValue(this, parentContext.CurrentUser.ID);
        }
    }
}