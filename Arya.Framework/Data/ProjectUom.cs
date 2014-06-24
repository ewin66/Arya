namespace Natalie.Framework.Data
{
    public partial class ProjectUom
    {
        public override string ToString()
        {
            return UnitOfMeasure.UnitName + " (" + Uom + ")";
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}