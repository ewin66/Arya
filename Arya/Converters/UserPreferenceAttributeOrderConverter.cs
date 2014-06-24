using Arya.HelperClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.Converters
{
    class UserPreferenceAttributeOrderConverter: TypeConverter
    {
        #region Private Variables
        private ICollection _displayOrderCategories;
        #endregion
        

        #region Type Converters Override Methods
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;// display drop
        }

        public override bool
         GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // drop-down vs combo
        }
        public override StandardValuesCollection
           GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(DisplayOrderCategories);
        }
        #endregion
        #region Private Methods

        public ICollection DisplayOrderCategories
        {
            get
            {
                if (_displayOrderCategories == null)
                {
                    _displayOrderCategories = new List<string>() {"Attribute Name", 
                                                                   SchemaAttribute.SchemaAttributeNavigationOrder.PrimarySchemaAttribute,
                                                                   SchemaAttribute.SchemaAttributeDisplayOrder.PrimarySchemaAttribute,
                                                                   SchemaAttribute.SchemaAttributeDataType.PrimarySchemaAttribute};
                }
                return (ICollection) _displayOrderCategories;
            }
        }

        #endregion

    }
}
