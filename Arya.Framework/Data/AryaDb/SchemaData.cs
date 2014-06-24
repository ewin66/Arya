using System.Globalization;

namespace Arya.Framework.Data.AryaDb
{
    partial class SchemaData : BaseEntity
    {
        public SchemaData(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();

            DataType = "Text";
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

        public override string ToString()
        {
            return "SchemaDataId  " + ID + '\t' + "Createdby " + CreatedBy + '\t' + "CreatedOn "
                   + CreatedOn.ToString(CultureInfo.InvariantCulture) + '\t' + "DeletedBy " + DeletedBy;
        }

        #endregion Methods
    }
}