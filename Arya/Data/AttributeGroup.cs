namespace Arya.Data
{
    public partial class AttributeGroup
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}