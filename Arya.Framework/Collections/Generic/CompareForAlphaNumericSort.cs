using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Arya.Framework.Math;

namespace Arya.Framework.Collections.Generic
{
    internal class CompareForAlphaNumericSort : IComparer<string>
    {
        #region Fields (3)

        private readonly Dictionary<string, int> _lovOrder;
        readonly Regex _rxLeadingNumber = new Regex("^([-]?(?:[0-9]+[-/ 0-9]*|[0-9]+(?:\\.[0-9]+)?))([^0-9].*)$", RegexOptions.Compiled);
        readonly Regex _rxTrailingNumber = new Regex("^(.*?)([-]?(?:[0-9]+[-/ 0-9]*|[0-9]+(?:\\.[0-9]+)?))$", RegexOptions.Compiled);


        static readonly Dictionary<string, Match> LeadingMatches = new Dictionary<string, Match>();
        static readonly Dictionary<string, Match> TrailingMatches = new Dictionary<string, Match>();

        private Match LeadingNumberMatch(string str)
        {
            if (!LeadingMatches.ContainsKey(str))
                LeadingMatches[str] = _rxLeadingNumber.Match(str);

            return LeadingMatches[str];
        }

        private Match TrailingNumberMatch(string str)
        {
            if (!TrailingMatches.ContainsKey(str))
                TrailingMatches[str] = _rxTrailingNumber.Match(str);

            return TrailingMatches[str];
        }

        #endregion Fields

        #region Constructors (2)

        public CompareForAlphaNumericSort(Dictionary<string, int> lovOrder)
        {
            _lovOrder = lovOrder;
        }

        public CompareForAlphaNumericSort()
        {
            _lovOrder = new Dictionary<string, int>();
        }

        #endregion Constructors

        #region Methods (1)

        // Public Methods (1) 

        public int Compare(string x, string y)
        {
            bool xBlank = string.IsNullOrEmpty(x);
            bool yBlank = string.IsNullOrEmpty(y);
            if (xBlank && yBlank)
                return 0;
            if (xBlank)
                return -1;
            if (yBlank)
                return 1;
            if (x.Equals(y))
                return 0;

            string value1, value2;
            bool xStartsWithDelimiter = x.StartsWith(",", StringComparison.Ordinal) || x.StartsWith(";", StringComparison.Ordinal) || x.StartsWith(".", StringComparison.Ordinal);
            bool yStartsWithDelimiter = y.StartsWith(",", StringComparison.Ordinal) || y.StartsWith(";", StringComparison.Ordinal) || y.StartsWith(".", StringComparison.Ordinal);

            if (_lovOrder.ContainsKey(x) && _lovOrder.ContainsKey(y))
            {
                value1 = _lovOrder[x].ToString(CultureInfo.InvariantCulture);
                value2 = _lovOrder[y].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                value1 = x;
                value2 = y;
            }

            double xValue, yValue;
            bool xNumeric = MathUtils.TryConvertToNumber(value1, out xValue);
            bool yNumeric = MathUtils.TryConvertToNumber(value2, out yValue);
            if (xNumeric && yNumeric)
                return xValue.CompareTo(yValue);
            if (xNumeric)
                return -1;
            if (yNumeric)
                return 1;
            if (xStartsWithDelimiter && !yStartsWithDelimiter)
                return 1;
            if (yStartsWithDelimiter && !xStartsWithDelimiter)
                return -1;

            var xPartialNumber = LeadingNumberMatch(x);
            var yPartialNumber = LeadingNumberMatch(y);
            if (xPartialNumber.Success && yPartialNumber.Success &&
                xPartialNumber.Groups[2].Value.Equals(yPartialNumber.Groups[2].Value))
            {
                xNumeric = MathUtils.TryConvertToNumber(xPartialNumber.Groups[1].Value, out xValue);
                yNumeric = MathUtils.TryConvertToNumber(yPartialNumber.Groups[1].Value, out yValue);
                if (xNumeric && yNumeric)
                    return xValue.CompareTo(yValue);
            }

            xPartialNumber = TrailingNumberMatch(x);
            yPartialNumber = TrailingNumberMatch(y);
            if (xPartialNumber.Success && yPartialNumber.Success &&
                xPartialNumber.Groups[1].Value.Equals(yPartialNumber.Groups[1].Value))
            {
                xNumeric = MathUtils.TryConvertToNumber(xPartialNumber.Groups[2].Value, out xValue);
                yNumeric = MathUtils.TryConvertToNumber(yPartialNumber.Groups[2].Value, out yValue);
                if (xNumeric && yNumeric)
                    return xValue.CompareTo(yValue);
            }

            return String.Compare(x, y, StringComparison.Ordinal);
        }

        #endregion Methods
    }
}
