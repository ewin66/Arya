namespace Arya.Data
{
    public partial class AttributeMetaData
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
