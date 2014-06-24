namespace Natalie.Data
{
    public partial class ColumnProperty
    {
       

        #region Properties (2) 

        public string AttributeName
        {
            get
            {
                return Attribute == null ? string.Empty : Attribute.AttributeName;
            }
        }

        public string AttributeType
        {
            get
            {
                return Attribute == null ? string.Empty : Attribute.AttributeType;
            }
        }

        public string AttributeGroup
        {
            get
            {
                return Attribute == null ? string.Empty : Attribute.AttributeGroup;
            }
        }

		#endregion Properties 

		#region Methods (2) 

		// Public Methods (1) 

        public override string ToString()
        {
            return string.Format("{0} - {1}", Attribute.AttributeName, TaxonomyInfo);
        }
		// Private Methods (1) 

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

		#endregion Methods 
    }
}