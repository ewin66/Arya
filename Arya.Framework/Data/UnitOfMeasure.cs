namespace Natalie.Framework.Data
{
    public partial class UnitOfMeasure
    {
        public UnitOfMeasure BaseUom
        {
            get { return UnitOfMeasure1; }
            set { UnitOfMeasure1 = value; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", UnitName, UnitAbbreviation);
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
            UnitName = "New Unit";
            UnitAbbreviation = "n.a.";
            FromBaseExpression = "b";
            ToBaseExpression = "n";
        }
    }
}