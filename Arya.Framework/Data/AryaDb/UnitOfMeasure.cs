namespace Arya.Framework.Data.AryaDb
{
    public partial class UnitOfMeasure:BaseEntity
    {
        public UnitOfMeasure(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public UnitOfMeasure BaseUom
        {
            get { return UnitOfMeasure1; }
            set { UnitOfMeasure1 = value; }
        }

        public bool IsBaseUom
        {
            get
            {
                return UnitOfMeasure1 == null;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", UnitName, UnitAbbreviation);
        }

        partial void OnCreated()
        {
            UnitName = "New Unit";
            UnitAbbreviation = "n.a.";
            FromBaseExpression = "b";
            ToBaseExpression = "n";
        }
    }
}