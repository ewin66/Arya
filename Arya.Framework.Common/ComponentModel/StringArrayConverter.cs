using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Arya.Framework.Common.ComponentModel
{
    public class StringArrayConverter : ArrayConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string) || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            var collection = value;
            if (value is string)
            {
                 collection = SplitString(value);
            }
            if (collection is IEnumerable<string> && destinationType == typeof (string))
            {
                return ((IEnumerable<string>) value).Aggregate(string.Empty,
                    (current, next) => current + Environment.NewLine + next);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static string[] SplitString(object value)
        {
            return
                value.ToString()
                    .Split(new[] {"\t", "\n", "\r", "|"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string ? SplitString(value) : base.ConvertFrom(context, culture, value);
        }
    }
}
