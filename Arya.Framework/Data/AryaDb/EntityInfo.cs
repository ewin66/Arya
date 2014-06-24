namespace Arya.Framework.Data.AryaDb
{
    public partial class EntityInfo : BaseEntity
    {
        public EntityInfo(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public override string ToString() { return EntityDatas.Count + " entries"; }
    }
}