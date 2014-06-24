using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using Arya.Framework.IO.InterchangeRecords;

namespace Arya.Framework.IO
{

    public static class Extensions
    {
        public static IEnumerable<T> GetRecordsWithNulls<T>(this CsvReader newCsvReader) where T : InterchangeRecord
        {
            return newCsvReader.GetRecordsWithNulls(typeof(T)) as IEnumerable<T>;
        }

        public static IEnumerable<object> GetRecordsWithNulls(this CsvReader newCsvReader, Type returnType)
        {
            var properties = returnType.GetProperties().Where(a => a.PropertyType == typeof(string)).ToList();
            var records = newCsvReader.GetRecords(returnType).ToList();
            for (int i = 0; i < records.Count(); i++)
            {
                foreach (var property in properties)
                {
                    var value = records[i].GetType().GetProperty(property.Name).GetValue(records[i], null);
                    value = value == null ? string.Empty : value.ToString();
                    if (value.ToString().ToLower().Equals("null") || string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        records[i].GetType().GetProperty(property.Name).SetValue(records[i], null);
                    }
                }
            }
            return records.AsEnumerable();
        }

        public static IEnumerable<T> GetInvalidRecords<T>(this IEnumerable<T> allrecords) where T : InterchangeRecord
        {
            return allrecords.Where(ad => !ad.IsValid());
        }
    }
}
