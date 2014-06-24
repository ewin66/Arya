using System;
using System.ComponentModel;

namespace Arya.Framework.Common.ComponentModel
{
        public class BooleanToYesNoConverter : BooleanConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value is bool && destinationType == typeof(string))
                {
                    return (bool)value ? "Yes" : "No";
                }

                if (value is string && destinationType == typeof(bool))
                {
                    return value.ToString() == "Yes";
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    return value.ToString() == "Yes";
                }

                if (value is bool)
                {
                    return (bool)value ? "Yes" : "No";
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        public class BooleanToYesOrBlankConverter : BooleanConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value is bool && destinationType == typeof(string))
                {
                    return (bool)value ? "Yes" : string.Empty;
                }

                if (value is string && destinationType == typeof(bool))
                {
                    return value.ToString() == "Yes";
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    return value.ToString() == "Yes";
                }

                if (value is bool)
                {
                    return (bool)value ? "Yes" : string.Empty;
                }
                return base.ConvertFrom(context, culture, value);
            }
        }
}
