using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Arya.Framework.Math
{
    public class MathUtils
    {
        private static readonly Regex FractionRegex = // The regex for fraction will not catch integers
            new Regex(
                "^(?<sign>[+-]?)\\s*((?<integer>\\d+)?\\s*[ -]\\s*)?(?<numerator>\\d+)\\s*/\\s*(?<denominator>\\d+)$",
                RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<string, double?> StringToDoubles = new ConcurrentDictionary<string, double?>();
        public static bool TryConvertToNumber(string text, out double result)
        {
            if (StringToDoubles.ContainsKey(text))
            {
                var val = StringToDoubles[text];
                if (val == null)
                {
                    result = 0;
                    return false;
                }
                
                result = (double) val;
                return true;
            }

            double value;
            bool success = Double.TryParse(text, out value);
            if (success)
            {
                result = value;
                StringToDoubles[text] = result;
                return true;
            }

            if (IsFract(text, out result))
            {
                StringToDoubles[text] = result;
                return true;
            }

            result = 0.0;
            StringToDoubles[text] = null;
            return false;
        }

        public static bool IsFract(string text, out double result)
        {
            Match match = FractionRegex.Match(text);
            if (match.Success)
            {
                double value = match.Groups["integer"].Success ? Int64.Parse(match.Groups["integer"].Value) : 0;

                if (match.Groups["numerator"].Success && match.Groups["denominator"].Success)
                    value += Double.Parse(match.Groups["numerator"].Value) /
                             Double.Parse(match.Groups["denominator"].Value);

                if (match.Groups["sign"].Success && match.Groups["sign"].Value.Equals("-"))
                    value *= -1;

                result = value;
                return true;
            }

            result = 0.0;
            return false;
        }
    }

}
