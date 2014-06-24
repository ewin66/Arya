using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Polenter.Serialization.Core;

namespace Arya.Framework.Common.Extensions
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            return att != null ? valueSelector(att) : default(TValue);
        }

        public static object DefaultValue(this PropertyInfo property)
        {
            return
                property.GetCustomAttributes(typeof (DefaultValueAttribute), false)
                    .Cast<DefaultValueAttribute>()
                    .Select(att => att.Value)
                    .FirstOrDefault();
        }

        public static bool IsBrowsable(this PropertyInfo prop)
        {
            return prop.GetCustomAttributes(typeof(BrowsableAttribute), false)
                .Cast<BrowsableAttribute>()
                .Select(ba => ba.Browsable)
                .DefaultIfEmpty(true)
                .First();
        }
    }
}
