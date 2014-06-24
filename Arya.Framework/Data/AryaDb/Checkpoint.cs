namespace Arya.Framework.Data.AryaDb
{
    partial class Checkpoint : BaseEntity
    {
        public Checkpoint(AryaDbDataContext parentContext, bool initialize = true)
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }
    }
}