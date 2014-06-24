using System;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using Arya.Framework.Data;

namespace Arya.Framework.IO.InterchangeRecords
{
    public abstract class InterchangeRecord : ITempTable
    {
        private readonly Regex _rxRemoveNonNumbers = new Regex("[^0-9]");

        private bool? _isValid;

        public bool IsValid()
        {
            if (_isValid == null)
                SetValid();
            
            return (bool)_isValid;
        }

        private void SetValid()
        {
            _isValid = false;
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                foreach (ColumnAttribute attribute in property.GetCustomAttributes(typeof(ColumnAttribute), false))
                {
                    if (attribute.DbType == null)
                        continue;
                    var dbTypeLengthString = _rxRemoveNonNumbers.Replace(attribute.DbType, string.Empty);
                    int length;
                    if (!int.TryParse(dbTypeLengthString, out length))
                        continue; //dbType does not contain a number

                    //TODO: Ask vivek how to make it efficient
                    try
                    {
                        if ((!attribute.CanBeNull && property.GetValue(this).ToString().Length > length) ||
                            (!attribute.CanBeNull && string.IsNullOrWhiteSpace(property.GetValue(this).ToString())))
                            return;
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }

            _isValid = true;
        }

        #region ITempTable Members

        public abstract string GetCreateIndexString(string databaseName, string tableName);

        #endregion
    }
}