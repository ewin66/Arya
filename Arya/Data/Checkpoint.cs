namespace Natalie.Data
{
    partial class Checkpoint
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}