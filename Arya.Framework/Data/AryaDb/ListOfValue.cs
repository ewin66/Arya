namespace Arya.Framework.Data.AryaDb
{
    public partial class ListOfValue : BaseEntity
    {
        public ListOfValue(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #region Properties (1)

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
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