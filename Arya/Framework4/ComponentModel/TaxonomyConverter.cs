namespace Arya.Framework4.ComponentModel
{
    using System;
    using System.ComponentModel;

    internal class TaxonomyConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Framework4.ComponentModel.ExtendedTaxonomyInfo)
            {
                var exportTaxonomyInfo = (Framework4.ComponentModel.ExtendedTaxonomyInfo)value;

                return exportTaxonomyInfo.SkuCount + " Sku(s)";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
