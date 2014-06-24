namespace Arya.Data
{

    public partial class AttributeMetaInfo
    {
        public Attribute MetaAttribute
        {
            get { return Attribute1; }
            set { Attribute1 = value; }
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

    }
}