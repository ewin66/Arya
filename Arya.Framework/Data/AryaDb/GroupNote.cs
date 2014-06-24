namespace Arya.Framework.Data.AryaDb
{
    partial class GroupNote : BaseEntity
    {
        public GroupNote(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}