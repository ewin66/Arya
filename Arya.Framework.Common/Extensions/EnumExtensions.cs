using System;

namespace Arya.Framework.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayText(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(DisplayTextAndValue), false) as DisplayTextAndValue[];

            return attribs == null
                       ? null
                       : attribs.Length > 0 ? attribs[0].DisplayText : null;
        }

        public static object GetValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(DisplayTextAndValue), false) as DisplayTextAndValue[];

            return attribs == null
                       ? null
                       : attribs.Length > 0 ? attribs[0].DbValue : null;
        }

        public static DisplayTextAndValue GetDisplayTextAndDbValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(DisplayTextAndValue), false) as DisplayTextAndValue[];

            return attribs == null
                       ? null
                       : attribs.Length > 0 ? attribs[0] : null;
        }
    }

    public class DisplayTextAndValue : Attribute
    {

        public string DisplayText { get; protected set; }
        public object DbValue { get; protected set; }

        public DisplayTextAndValue(string displayText, object dbValue)
        {
            DisplayText = displayText;
            DbValue = dbValue;
        }
    }
}
