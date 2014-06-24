using System;
using System.Windows.Forms;

namespace Natalie.HelperClasses
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Framework.Math;
    using NCalc;

    public static class BaseUnitConversion
    {
        private static List<UnitOfMeasure> _uoms;
        private static List<ProjectUom> _projectUoms;

        public static List<UnitOfMeasure> Uoms
        {
            get { return _uoms ?? (_uoms = NatalieTools.Instance.InstanceData.Dc.UnitOfMeasures.ToList()); }
        }

        public static List<ProjectUom> ProjectUoms
        {
            get
            {
                return _projectUoms ??
                       (_projectUoms =
                        NatalieTools.Instance.InstanceData.Dc.ProjectUoms.Where(
                            p => p.ProjectID == NatalieTools.Instance.InstanceData.CurrentProject.ID).ToList());
            }
        }

        //public static double ConvertValue(this double value, string fromUom, UnitOfMeasure toUom, int precision)
        //{
        //    var fromUnit = (from uom in ProjectUoms
        //                    where uom.Uom == fromUom
        //                    select uom.UnitOfMeasure).FirstOrDefault();

        //    if (fromUnit == null || toUom == null)
        //        return double.MinValue;

        //    var baseUnit = fromUnit.BaseUom ?? fromUnit;
        //    var baseValue = fromUnit.BaseUnit == null
        //                        ? value
        //                        : (value * fromUnit.MultiplicationFactor) + fromUnit.SummationFactor;

        //    var convertedValue = toUom.ID == baseUnit.ID
        //                             ? baseValue
        //                             : (baseValue - toUom.SummationFactor) / toUom.MultiplicationFactor;

        //    return double.Parse(convertedValue.ToString("F" + precision));
        //}

        public static double ConvertValueUsingExpression(this double value, string fromUom, UnitOfMeasure toUnit, int precision)
        {
            try
            {
                UnitOfMeasure baseUnit;
                var baseValue = ConvertToBaseValue(value, fromUom, out baseUnit);

                if (baseUnit == null || toUnit == null)
                    return baseValue;


                if (toUnit.ID == baseUnit.ID)
                    return baseValue;

                var fromBaseExpression = new Expression(toUnit.FromBaseExpression);
                fromBaseExpression.Parameters.Add("b", baseValue);
                var convertedValue = (double)fromBaseExpression.Evaluate();
                return double.Parse(convertedValue.ToString("F" + precision));
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Expression is not valid. Please update and try again.");
                return double.MinValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return double.MinValue;
            }
        }

        public static double ConvertToBaseValue(double value, string fromUom, out UnitOfMeasure baseUnit)
        {
            var fromUnit = (from uom in ProjectUoms where uom.Uom == fromUom select uom.UnitOfMeasure).FirstOrDefault();

            if (fromUnit == null)
            {
                baseUnit = null;
                return double.MinValue;
            }

            baseUnit = fromUnit.BaseUom ?? fromUnit;
            double baseValue;
            if (fromUnit.BaseUnit == null)
                baseValue = value;
            else
            {
                var toBaseExpression = new Expression(fromUnit.ToBaseExpression);
                toBaseExpression.Parameters.Add("n", value);
                baseValue = (double)toBaseExpression.Evaluate();
            }
            return baseValue;
        }

        public static UnitOfMeasure GetBaseUom(string uom)
        {
            if (uom == null || ProjectUoms.Count == 0)
                return null;

            return (from pu in ProjectUoms
                    where pu.Uom == uom
                    let thisUom = pu.UnitOfMeasure
                    let baseUom = thisUom.BaseUom ?? thisUom
                    select baseUom).FirstOrDefault();
        }

        public static bool IsTolerated(this IEnumerable<double> checkSkuValueArray, double nodeSkuValue,
                                       string rawTolerance)
        {
            if (rawTolerance.StartsWith("+"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return checkSkuValueArray.Any(val => val.IsInRange(nodeSkuValue + valueTolerance, nodeSkuValue));
            }

            if (rawTolerance.StartsWith("-"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return checkSkuValueArray.Any(val => val.IsInRange(nodeSkuValue, nodeSkuValue - valueTolerance));
            }

            if (rawTolerance.StartsWith("~"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return
                    checkSkuValueArray.Any(
                        val => val.IsInRange(nodeSkuValue + valueTolerance, nodeSkuValue - valueTolerance));
            }

            if (rawTolerance.Equals("0"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return
                    checkSkuValueArray.Any(
                        val => val.IsInRange(nodeSkuValue + valueTolerance, nodeSkuValue - valueTolerance));
            }
            return false; //Default
        }

        private static double GetValueTolerance(double nodeSkuValue, string rawTolerance)
        {
            double tol;

            if (rawTolerance.EndsWith("%"))
            {
                MathUtils.TryConvertToNumber(rawTolerance.TrimStart('+', '-', '~').TrimEnd('%'), out tol);
                return (tol / 100) * nodeSkuValue;
            }
            MathUtils.TryConvertToNumber(rawTolerance.TrimStart('+', '-', '~'), out tol);
            return tol;
        }

        public static bool IsInRange(this double numberToCheck, double top, double bottom)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        //public static string GetBaseUnitValueAsString(string value, string uom)
        //{
        //    if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(uom) || ProjectUoms == null)
        //        return null;

        //    double val;
        //    var success = MathUtils.TryConvertToNumber(value, out val);
        //    if (success)
        //    {
        //        var uomID = ProjectUoms.Where(u => u.Uom == uom).Select(i => i.UomID).FirstOrDefault();
        //        if (uomID != Guid.Empty)
        //        {
        //            var currentUom = Uoms.FirstOrDefault(i => i.ID == uomID);
        //            if (currentUom != null)
        //            {
        //                val = val*currentUom.MultiplicationFactor + currentUom.SummationFactor;
        //                return val.ToString();
        //            }
        //            return "Base Unit Not Found";
        //        }
        //        return "Base Unit Not Found";
        //    }
        //    return "9999.9999";
        //}
    }
}