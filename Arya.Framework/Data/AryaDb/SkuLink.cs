namespace Arya.Framework.Data.AryaDb
{
    partial class SkuLink : BaseEntity
    {
        public SkuLink(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #region Properties (3)

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

        public Sku LinkedFrom
        {
            get { return Sku; }
            set { Sku = value; }
        }

        public Sku LinkedTo
        {
            get { return Sku1; }
            set { Sku1 = value; }
        }

        #endregion Properties

        #region Methods (2)

        // Private Methods (2) 

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