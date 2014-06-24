using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Natalie.Framework.Data;
using ColumnAttribute = System.Data.Linq.Mapping.ColumnAttribute;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public abstract  class ImportRecord : ITempTable
    {
        public abstract string GetCreateIndexString(string databaseName, string tableName);

        readonly Regex _rxRemoveNonNumbers = new Regex("[^0-9]");

        private bool? _isValid;

        [NotMapped]
        public bool IsValid
        {
            get
            {
                if (_isValid != null)
                    return (bool) _isValid;

                var properties = this.GetType().GetProperties();
                foreach (var property in properties)
                {
                    foreach (ColumnAttribute attribute in property.GetCustomAttributes(typeof (ColumnAttribute), false))
                    {
                        var dbTypeLengthString = _rxRemoveNonNumbers.Replace(attribute.DbType, string.Empty);
                       // bool isNullable = attribute.CanBeNull ? true : false ;
                        int length;
                        //bool isNullable = true;
                        //bool.TryParse(dbTypeLengthString, out length)
                        if (!int.TryParse(dbTypeLengthString, out length))
                            continue; //dbType does not contain a number
                        //if(property.GetValue(this) == null && )
                        if (property.GetValue(this).ToString().Length > length || (!attribute.CanBeNull && string.IsNullOrWhiteSpace(property.GetValue(this).ToString())))
                        {
                            _isValid = false;
                            return false;
                        }
                    }
                }

                _isValid = true;
                return true;
            }
        }
    }
}
