using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.Math;
using Arya.Framework.Utility;

namespace Arya.Framework.IO
{
    class ListOfValuesExportSorter
    {
        private readonly Dictionary<string, Dictionary<UoMValue, List<LanguageValue>>> _valueMap = new Dictionary<string, Dictionary<UoMValue, List<LanguageValue>>>();
        private readonly AryaDbDataContext _db;

        public ListOfValuesExportSorter(AryaDbDataContext db)
        {
            _db = db;
        }

        public void ClearListOfValues()
        {
            _valueMap.Clear();
        }

        public void MakeListOfValues(SchemaData sd)
        {
            // get list of live data
            var liveData = (from si in sd.SchemaInfo.TaxonomyInfo.SkuInfos
                            where si.Active
                            from ei in si.Sku.EntityInfos
                            from ed in ei.EntityDatas
                            where ed.Active && ed.AttributeID == sd.SchemaInfo.AttributeID
                            select new UoMValue(_db, ed.Value, ed.Uom)).Distinct().ToList();

            string attName = sd.SchemaInfo.Attribute.AttributeName.Trim();
            if (!_valueMap.ContainsKey(attName))
            {
                if (sd.InSchema)
                {
                    // get the active lovs
                    var lovActives = sd.SchemaInfo.ListOfValues.Where(lov => lov.Active).ToList();

                    // separate off the lovs with display orders, sort them, and get their values
                    var lovDisplay =
                        lovActives.Where(lov => lov.DisplayOrder != null)
                            .OrderBy(lov => lov.DisplayOrder)
                            .Select(lov => new UoMValue(_db, lov.Value, null))
                            .ToList();

                    var stringData = new List<UoMValue>();
                    var numData = new List<UoMValue>();
                    // process the live data, ignoring anything in the LOV processed above
                    foreach (var value in liveData.Where(val => !lovDisplay.Contains(val, new UomValueCompareValues())))
                    {
                        double numValue;
                        if (MathUtils.TryConvertToNumber(value.Value, out numValue))
                        {
                            numData.Add(value);
                        }
                        else
                        {
                            stringData.Add(value);
                        }
                    }

                    // sort the lists
                    stringData.Sort(new UomValueCompareValues());
                    numData.Sort();

                    // combine the three sets.
                    var values = lovDisplay.Union(numData).Union(stringData).ToList();

                    var valueDict = new Dictionary<UoMValue, List<LanguageValue>>();
                    int sortNumber = 1000;
                    foreach (var val in values)
                    {
                        var dataPoint = new List<LanguageValue>
                        {
                            new LanguageValue
                            {
                                LanguageCode = LanguageValue.En,
                                Value = val.Value.Trim()
                            },
                            new LanguageValue
                            {
                                LanguageCode = LanguageValue.FacetLanguage,
                                Value = sortNumber + "|" + val.Value.Trim()
                            }

                        };
                        if (!valueDict.ContainsKey(val))
                        {
                            valueDict.Add(val, dataPoint);
                        }
                        sortNumber++;
                    }
                    _valueMap.Add(attName, valueDict);
                }
            }
        }

        public List<LanguageValue> GetLanguageValues(string attName, string enValue, string uom)
        {
            var value = new UoMValue(_db, enValue, uom);
            if (_valueMap.ContainsKey(attName.Trim()) && _valueMap[attName.Trim()].ContainsKey(value))
            {
                return _valueMap[attName.Trim()][value];
            }

            // bulletproofing - if the english value is not found, convert the input accordingly
            return new List<LanguageValue>
                { new LanguageValue
                    {
                        LanguageCode = LanguageValue.En, 
                        Value = enValue
                    }
                };
        }
    }

    public class UoMValue : IComparable<UoMValue>, IEquatable<UoMValue>
    {
        private readonly AryaDbDataContext _db;

        public string Value { get; set; }
        public string UoM { get; set; }

        public double BaseValue
        {
            get
            {
                double value;
                if (MathUtils.TryConvertToNumber(Value, out value))
                {
                    UnitOfMeasure baseUnit;
                    return new BaseUnitConversion(_db).ConvertToBaseValue(value, UoM, out baseUnit);
                }
                return 0.0;
            }
        }

        public UoMValue(AryaDbDataContext db, string value, string uom)
        {
            _db = db;
            Value = value;
            UoM = uom;
        }

        public int CompareTo(UoMValue other)
        {
            if (BaseValue < other.BaseValue)
                return -1;
            if (BaseValue > other.BaseValue)
                return 1;

            return 0;
        }

        public bool Equals(UoMValue other)
        {
            if (other == null) return false;
            if (this == other) return true;
            if (GetType() != other.GetType()) return false;

            return (Value == other.Value) && (UoM == other.UoM);
        }

        public override int GetHashCode()
        {
            if (Value == null)
                if (UoM == null)
                    return 0;
                else
                    return UoM.GetHashCode();
            else
                if (UoM == null)
                    return Value.GetHashCode();
                else
                    return ((ulong)Value.GetHashCode() | ((ulong)UoM.GetHashCode() << 32)).GetHashCode();
        }
    }

    public class UomValueCompareValues : IComparer<UoMValue>, IEqualityComparer<UoMValue>
    {
        public int Compare(UoMValue x, UoMValue y)
        {
            return String.CompareOrdinal(x.Value, y.Value);
        }

        public bool Equals(UoMValue that, UoMValue other)
        {
            if (other == null) return false;
            if (that == other) return true;
            if (that.GetType() != other.GetType()) return false;

            return (that.Value == other.Value);
        }

        public int GetHashCode(UoMValue that)
        {
            return that.Value.GetHashCode();
        }
    }
}