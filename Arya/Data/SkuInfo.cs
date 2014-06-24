namespace Arya.Data
{
    public partial class SkuInfo
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}