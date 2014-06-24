using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Arya.Framework.Math;

namespace Arya.Framework.Common.ComponentModel
{
    public class CompareForAlphaNumericSort : IComparer<string>
    {
        #region Fields (3) 

        private readonly Dictionary<string, int> _lovOrder;
        readonly Regex _rxLeadingNumber = new Regex("^([-]?(?:[0-9]+[-/ 0-9]*|[0-9]+(?:\\.[0-9]+)?))([^0-9].*)$",RegexOptions.Compiled);
        //readonly Regex _rxTrailingNumber = new Regex("^(.*?)([-]?(?:[0-9]+[-/ 0-9]*|[0-9]+(?:\\.[0-9]+)?))$", RegexOptions.Compiled);
        readonly Regex _rxTrailingNumber = new Regex("^(.*?)((?:[0-9]+[-/ 0-9]*|[0-9]+(?:\\.[0-9]+)?))$", RegexOptions.Compiled);

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

        public const string BlankValue = "<blank>";
        public const string EmptyValue = "<empty value>";
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

            if (x == BlankValue && y == EmptyValue)
                return -1;
            if (x == EmptyValue && y == BlankValue)
                return 1;
            if (x == BlankValue || x == EmptyValue)
                return -1;
            if (y == BlankValue || y == EmptyValue)
                return 1;

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

            var xPartialNumber = _rxLeadingNumber.Match(x);
            var yPartialNumber = _rxLeadingNumber.Match(y);
            if (xPartialNumber.Success && yPartialNumber.Success &&
                xPartialNumber.Groups[2].Value.Equals(yPartialNumber.Groups[2].Value))
            {
                xNumeric = MathUtils.TryConvertToNumber(xPartialNumber.Groups[1].Value, out xValue);
                yNumeric = MathUtils.TryConvertToNumber(yPartialNumber.Groups[1].Value, out yValue);
                if (xNumeric && yNumeric)
                    return xValue.CompareTo(yValue);
            }

            xPartialNumber = _rxTrailingNumber.Match(x);
            yPartialNumber = _rxTrailingNumber.Match(y);
            if (xPartialNumber.Success && yPartialNumber.Success &&
                xPartialNumber.Groups[1].Value.Equals(yPartialNumber.Groups[1].Value))
            {
                xNumeric = MathUtils.TryConvertToNumber(xPartialNumber.Groups[2].Value, out xValue);
                yNumeric = MathUtils.TryConvertToNumber(yPartialNumber.Groups[2].Value, out yValue);
                if (xNumeric && yNumeric)
                    return xValue.CompareTo(yValue);
            }

            return System.String.Compare(x, y, System.StringComparison.Ordinal);
        }

        #endregion Methods 

        //public static void DoubleToFraction(double input, out int numerator, out int denominator, bool makeDenominatorBinary)
        //{
        //    /* See http://mathforum.org/dr.math/faq/faq.fractions.html#decfrac
        //     * Section C: Fractions with Small Denominators
        //        If the decimal is a fraction with a small denominator, but the length
        //        of the repeating part is long enough that you cannot see a repeat, you
        //        can still recover the fraction by expanding the decimal as a simple
        //        continued fraction. This is a compound fraction of the form
        //                                1
        //        F = a(0) + -------------------------------
        //                                    1
        //                   a(1) + ------------------------
        //                                        1
        //                          a(2) + -----------------
        //                                            1
        //                                 a(3) + ----------
        //                                        a(4) + ...
        //        where all the a(n)'s are integers, and all but possibly a(0) are
        //        positive. If we start with the value of F, we can compute the
        //        integers a(0), a(1), a(2), and so on, by the following method:
        //        a(0) is the largest integer less than or equal to F. Subtract a(0)
        //        from F and take the reciprocal, 1/[F-a(0)]. a(1) is the largest
        //        integer less than or equal to this quotient. Subtract a(1) from this
        //        and take the reciprocal. a(2) is the largest integer less than or
        //        equal to this new quotient. Continue this way as many steps as you wish.
        //        If the decimal F is an exact fraction, this process will end with an
        //        attempt to take the reciprocal of 0, because one of the quotients will
        //        be an integer. If the decimal F is a close approximation to a fraction,
        //        this process will encounter a step where you are taking the reciprocal
        //        of a very small number, very close to zero. If you stop at this point,
        //        you will have a rational number that is very close to the decimal F.
        //        As an example, let's find the fraction for F = 0.127659574... .
        //        n    Quotient    a(n)  Fraction      Reciprocal      Value So Far
        //                                                           0/1
        //                                   1/0
        //        0   0.127659574   0   0.127659574   7.833333362    0/1 = 0.000000000
        //        1   7.833333362   7   0.833333362   1.199999959    1/7 = 0.142857143
        //        2   1.199999959   1   0.199999959   5.000001034    1/8 = 0.125000000
        //        3   5.000001034   5   0.000001034       Stop      6/47 = 0.127659574
        //        The work at each step is
        //        n      q      int(q)    q-a          1/(q-a)     num=num1+a*num2
        //                       [a]                   [new q]     den=den1+a*den2
        //                                                         num1=num2, den1=den2
        //                                                         num2=num, den2=den
        //        Then
        //                     1           1      6
        //        F = 0 + ----------- = ------- = --.
        //                       1           5    47
        //                7 + -------   7 + ---
        //                         1         6
        //                    1 + ---
        //                         5
        //        Sure enough, 6/47 = 0.127659574468...
        //        The numerators and denominators of the "Value So Far" column can be
        //        computed by starting with 0/1 and 1/0. Multiply the last numerator
        //        by a(n) and add it to the numerator before that to get the new
        //        numerator, and likewise for the denominators. Above, we had 1/7 and
        //        1/8, and a(3) = 5, so the new value-so-far fraction is
        //        (1*5+1)/(8*5+7) = 6/47.
        //        Here we start with (num1,den1) = (0,1) and (num2,den2) = (1,0), and
        //        keep them equal to the last two fractions obtained; at each step,
        //        they are used to find the new fraction.
        //     */
        //    int[] denominators = new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
        //    int num1 = 0, den1 = 1, num2 = 1, den2 = 0, n = 0;

        //    const int maxSteps = 100;
        //    const int maxNumerator = 130;
        //    const int maxDenominator = 100;
        //    double q = input; // the current value being worked on
        //    const double epsilon = 0.001;

        //    numerator = 0; //To satisfy the compiler
        //    denominator = 1; //To satisfy the compiler

        //    do
        //    {
        //        n++;
        //        if (q > int.MaxValue) // prevent overflow
        //            break;

        //        int a = (int)(q);
        //        numerator = num1 + (a * num2);
        //        denominator = den1 + (a * den2);

        //        if (q - a < 0.000001) // prevent divide by zero
        //            break;

        //        q = 1 / (q - a);
        //        num1 = num2;
        //        den1 = den2;
        //        num2 = numerator;
        //        den2 = denominator;

        //        //Make the denominator a binary
        //        if (makeDenominatorBinary)
        //            foreach (int den in denominators)
        //            {
        //                if (den < denominator)
        //                    continue;

        //                denominator = den;
        //                break;
        //            }
        //    } while ((Math.Abs((1.0 * numerator / denominator) - input) > epsilon) // stop when close enough
        //             && (n < maxSteps) // avoid infinite loops
        //             && (numerator < maxNumerator) // stop if too big
        //             && (denominator < maxDenominator)
        //        );

        //    if (makeDenominatorBinary)
        //    {
        //        while (Math.Abs(((1.0 + numerator) / denominator) - input) < (Math.Abs((1.0 * numerator / denominator) - input)))
        //            numerator++;
        //    }
        //}
    }
}