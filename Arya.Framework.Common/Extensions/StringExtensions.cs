using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Arya.Framework.Extensions
{
    public static class StringExtensions
    {
        static public string Replace(this string source, string oldValue, string newValue, StringComparison comparison)
        {
            var sb = new StringBuilder();

            int previousIndex = 0;
            int index = source.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(source.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = source.IndexOf(oldValue, index, comparison);
            }
            sb.Append(source.Substring(previousIndex));

            return sb.ToString();
        }

        public static string ToCamelCase(this string source)
        {
            return Regex.Replace(source, "([A-Z]+[a-z]*)", " $1", RegexOptions.Compiled).Trim();
        }

        public static string Spacify(this string source)
        {
            const string matchpattern = @"([a-z,.]|\d)([A-Z])";
            const string replacementpattern = @"$1 $2";
            return Regex.Replace(source, matchpattern, replacementpattern);
        }

        public static string HtmlLineBreakify(this string source)
        {
            return source.Replace("\n", "<br />").Replace("\t", " &nbsp;  &nbsp; ").Replace("|", ": &nbsp;  &nbsp; ");
        }
    }
}
