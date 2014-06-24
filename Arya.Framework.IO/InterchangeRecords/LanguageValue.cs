using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Arya.Framework.IO.InterchangeRecords
{
    [Serializable]
    public class LanguageValue
    {
        public const string En = "en";
        public const string Es = "es";
        public const string Fr = "fr";
        public const string FacetLanguage = "00";

        [XmlAttribute]
        public string LanguageCode { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public static class LanguageValueExtensions
    {
        #region LanguageCode enum

        public enum LanguageCode
        {
            En,
            Es,
            Fr,
            Facet
        }

        #endregion

        public static string StringValue(this LanguageCode lc)
        {
            switch (lc)
            {
                case LanguageCode.Facet:
                    return "00";
                    break;
                default:
                    return lc.ToString().ToLower();
            }
        }

        public static void AddValue(this List<LanguageValue> list, string value, LanguageCode languageCode = LanguageCode.En)
        {
            list.Add(new LanguageValue {LanguageCode = languageCode.StringValue(), Value = value});
        }

        public static string GetValue(this List<LanguageValue> list, LanguageCode languageCode = LanguageCode.En)
        {
            return (from t in list
                    where t.LanguageCode == LanguageCode.En.StringValue()
                    select t.Value).FirstOrDefault();
        }
    }
}