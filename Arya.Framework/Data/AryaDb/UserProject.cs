namespace Arya.Framework.Data.AryaDb
{
    public partial class UserProject : BaseEntity
    {
        public UserProject(AryaDbDataContext parenteContext, bool initialize = true) : this()
        {
            ParentContext = parenteContext;
            Initialize = initialize;
            InitEntity();

            GroupID = Group.DefaultGroupID;
        }
    }
}