namespace Arya.Data
{
    public partial class EntityInfo
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

        public override string ToString()
        {
            return EntityDatas.Count + " entries";
        }
    }
}