using Arya.Framework4.Collections;

namespace Arya.Framework4.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    internal class TaxonomyCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is TaxonomyCollection)
            {
                var taxValue = (TaxonomyCollection)value;
                var totalSkus = taxValue.Cast<Framework4.ComponentModel.ExtendedTaxonomyInfo>().Sum(p => p.SkuCount);

                return "Selected Taxonomies (" + taxValue.Count + ")" + (totalSkus > 0 ? ", " + totalSkus + " Sku(s)" : string.Empty);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
