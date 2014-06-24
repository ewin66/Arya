using System;
using System.ComponentModel;

namespace Arya.Framework.Common.ComponentModel
{
    public class FilterCollectionConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (value == null) return "0 Item(s)";
            // ReSharper restore HeuristicUnreachableCode
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            return ((dynamic)value).Count + " Item(s)";
        }
    }
}
