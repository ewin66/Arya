using System;
using System.ComponentModel;
using System.Reflection;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;

namespace Arya.Framework.Common.ComponentModel
{
    public class CustomEnumConverter : EnumConverter
    {
        protected Type MyVal;

        /// <summary>
        /// Gets Enum Value's Description Attribute
        /// </summary>
        /// <param name="value">The value you want the description attribute for</param>
        /// <returns>The description, if any, else it's .ToString()</returns>
        public static string GetEnumDisplayText(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes =
              (DisplayTextAndValue[])fi.GetCustomAttributes(
              typeof(DisplayTextAndValue), false);
            return (attributes.Length > 0) ? attributes[0].DisplayText : value.ToString();
        }

        /// <summary>
        /// Gets the description for certaing named value in an Enumeration
        /// </summary>
        /// <param name="value">The type of the Enumeration</param>
        /// <param name="name">The name of the Enumeration value</param>
        /// <returns>The description, if any, else the passed name</returns>
        public static string GetEnumDisplayText(Type value, string name)
        {
            var fi = value.GetField(name);
            var attributes =
              (DisplayTextAndValue[])fi.GetCustomAttributes(
              typeof(DisplayTextAndValue), false);
            return (attributes.Length > 0) ? attributes[0].DisplayText : name;
        }

        /// <summary>
        /// Gets the value of an Enum, based on it's Description Attribute or named value
        /// </summary>
        /// <param name="value">The Enum type</param>
        /// <param name="description">The description or name of the element</param>
        /// <returns>The value, or the passed in description, if it was not found</returns>
        public static object GetEnumValue(Type value, string description)
        {
            var fis = value.GetFields();
            foreach (FieldInfo fi in fis)
            {
                var attributes =
                  (DisplayTextAndValue[])fi.GetCustomAttributes(
                  typeof(DisplayTextAndValue), false);
                if (attributes.Length > 0)
                {
                    if (attributes[0].DisplayText == description)
                    {
                        return fi.GetValue(fi.Name);
                    }
                }
                if (fi.Name == description)
                {
                    return fi.GetValue(fi.Name);
                }
            }
            return description;
        }

        public CustomEnumConverter(Type type)
            : base(type)
        {
            MyVal = type;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is Enum && destinationType == typeof(string))
            {
                return GetEnumDisplayText((Enum)value);
            }
            if (value is string && destinationType == typeof(string))
            {
                return GetEnumDisplayText(MyVal, (string)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return GetEnumValue(MyVal, (string)value);
            }
            if (value is Enum)
            {
                return GetEnumDisplayText((Enum)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
