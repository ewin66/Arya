using System;
using System.Collections.Generic;
using System.Linq;
using NCalc;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Math;

namespace Arya.Framework.Utility
{
    public class BaseUnitConversion
    {
        #region Fields

        private static List<ProjectUom> _projectUoms;
        private static List<UnitOfMeasure> _uoms;
        private readonly AryaDbDataContext _parentContext;

        #endregion Fields

        #region Constructors

        public BaseUnitConversion(AryaDbDataContext parentContext) { _parentContext = parentContext; }

        #endregion Constructors

        #region Properties

        public List<ProjectUom> ProjectUoms
        {
            get
            {
                return _projectUoms
                       ?? (_projectUoms =
                           _parentContext.ProjectUoms.Where(p => p.ProjectID == _parentContext.CurrentProject.ID)
                               .ToList());
            }
        }

        public List<UnitOfMeasure> Uoms
        {
            get { return _uoms ?? (_uoms = _parentContext.UnitOfMeasures.ToList()); }
        }

        #endregion Properties

        #region Methods

        public bool IsTolerated(IEnumerable<double> checkSkuValueArray, double nodeSkuValue, string rawTolerance)
        {
            if (rawTolerance.StartsWith("+"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return checkSkuValueArray.Any(val => IsInRange(val, nodeSkuValue + valueTolerance, nodeSkuValue));
            }

            if (rawTolerance.StartsWith("-"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return checkSkuValueArray.Any(val => IsInRange(val, nodeSkuValue, nodeSkuValue - valueTolerance));
            }

            if (rawTolerance.StartsWith("~"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return
                    checkSkuValueArray.Any(
                        val => IsInRange(val, nodeSkuValue + valueTolerance, nodeSkuValue - valueTolerance));
            }

            if (rawTolerance.Equals("0"))
            {
                var valueTolerance = GetValueTolerance(nodeSkuValue, rawTolerance);
                return
                    checkSkuValueArray.Any(
                        val => IsInRange(val, nodeSkuValue + valueTolerance, nodeSkuValue - valueTolerance));
            }
            return false; //Default
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
        public double ConvertValueUsingExpression(double value, string fromUom, UnitOfMeasure toUnit, int precision)
        {
            try
            {
                var fromUnit =
                    (from uom in ProjectUoms where uom.Uom == fromUom select uom.UnitOfMeasure).FirstOrDefault();

                if (fromUnit == null || toUnit == null)
                    return double.MinValue;

                var baseUnit = fromUnit.BaseUom ?? fromUnit;
                double baseValue;
                if (fromUnit.BaseUnit == null)
                    baseValue = value;
                else
                {
                    var toBaseExpression = new Expression(fromUnit.ToBaseExpression);
                    toBaseExpression.Parameters.Add("n", value);
                    baseValue = (double) toBaseExpression.Evaluate();
                }

                double convertedValue;
                if (toUnit.ID == baseUnit.ID)
                    convertedValue = baseValue;
                else
                {
                    var fromBaseExpression = new Expression(toUnit.FromBaseExpression);
                    fromBaseExpression.Parameters.Add("b", baseValue);
                    convertedValue = (double) fromBaseExpression.Evaluate();
                }
                return double.Parse(convertedValue.ToString("F" + precision));
            }
            catch (ArgumentException ex)
            {
                throw new Exception("Invalid Expression converting from " + fromUom, ex);
                //MessageBox.Show("Expression is not valid. Please update and try again.");
                //return double.MinValue;
            }
            //catch (Exception ex)
            //{
            //    //MessageBox.Show(ex.Message);
            //    return double.MinValue;
            //}
        }

        public double GetBaseValue(string value, string uom)
        {
            double val;
            if (Double.TryParse(value, out val))
            {
                UnitOfMeasure baseUom;
                return ConvertToBaseValue(val, uom, out baseUom);
            }
            return 0.0;
        }
        
        public double ConvertToBaseValue(double value, string fromUom, out UnitOfMeasure baseUnit)
        {
            if (fromUom == null)
            {
                baseUnit = null;
                return value;
            }
            
            var fromUnit = (from uom in ProjectUoms where uom.Uom == fromUom select uom.UnitOfMeasure).FirstOrDefault();

            if (fromUnit == null)
            {
                baseUnit = null;
                return value;
            }

            baseUnit = fromUnit.BaseUom ?? fromUnit;
            double baseValue;
            if (fromUnit.BaseUnit == null)
                baseValue = value;
            else if (fromUnit.ToBaseExpression == null)
            {
                baseValue = Double.MinValue;
            }
            else
            {
                var toBaseExpression = new Expression(fromUnit.ToBaseExpression);
                toBaseExpression.Parameters.Add("n", value);
                baseValue = (double)toBaseExpression.Evaluate();
            }
            return baseValue;
        }

        public UnitOfMeasure GetBaseUom(string uom)
        {
            if (uom == null || ProjectUoms.Count == 0)
                return null;

            return (from pu in ProjectUoms
                where pu.Uom == uom
                let thisUom = pu.UnitOfMeasure
                let baseUom = thisUom.BaseUom ?? thisUom
                select baseUom).FirstOrDefault();
        }

        private static double GetValueTolerance(double nodeSkuValue, string rawTolerance)
        {
            double tol;

            if (rawTolerance.EndsWith("%"))
            {
                MathUtils.TryConvertToNumber(rawTolerance.TrimStart('+', '-', '~').TrimEnd('%'), out tol);
                return (tol/100)*nodeSkuValue;
            }
            MathUtils.TryConvertToNumber(rawTolerance.TrimStart('+', '-', '~'), out tol);
            return tol;
        }

        private static bool IsInRange(double numberToCheck, double top, double bottom)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        #endregion Methods

        #region Other

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

        #endregion Other
    }
}