namespace Arya.Framework.Data.AryaDb
{
    public partial class AttributeMetaInfo : BaseEntity
    {
        public AttributeMetaInfo(AryaDbDataContext parentContext, bool initialize = true)
            : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { _currentProject = value; }
        }

        public User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        public Attribute MetaAttribute
        {
            get { return Attribute1; }
            set { Attribute1 = value; }
        }

    }
}