using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Arya.Framework.Data
{
    public class SqlHelper
    {
        private List<KeyValuePair<String, Type>> _fieldInfo = new List<KeyValuePair<String, Type>>();
        private string _className = String.Empty;

        private readonly ITempTable _source;

        private static readonly Dictionary<Type, String> DefaultMappings = new Dictionary<Type, string>
                                     {
                                         {typeof (int), "BIGINT"},
                                         {typeof (string), "NVARCHAR(4000)"},
                                         {typeof (bool), "BIT"},
                                         {typeof (DateTime), "DATETIME"},
                                         {typeof (float), "FLOAT"},
                                         {typeof (decimal), "DECIMAL(18,0)"},
                                         {typeof (Guid), "UNIQUEIDENTIFIER"}
                                     };

        private readonly Dictionary<string, string> _currentColumnMappings;

        public List<KeyValuePair<String, Type>> Fields
        {
            get { return _fieldInfo; }
            set { _fieldInfo = value; }
        }

        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        public SqlHelper(Type sourceType)
        {

            var tempObject = Activator.CreateInstance(sourceType) as ITempTable;

            if (tempObject == null)
            {
                throw new NotSupportedException("Should implement ITempTable interface");
            }

            _source = tempObject;
            var t = sourceType;
            _className = t.Name;

            var propMappings = GetCurrentMappings(t);
            _currentColumnMappings = new Dictionary<string, string>();

            var fields = from p in t.GetProperties()
                         let ca = p.GetCustomAttributes(typeof(ColumnAttribute), false) as ColumnAttribute[]
                         where ca != null && ca.Any()
                         select new KeyValuePair<String, Type>(string.IsNullOrWhiteSpace(ca[0].Name) ? p.Name : ca[0].Name, p.PropertyType);

            foreach (var field in fields)
            {
                Fields.Add(field);

                if (propMappings.ContainsKey(field.Key))
                {
                    _currentColumnMappings[field.Key] = propMappings[field.Key];
                }
                else //if(_currentColumnMappings.ContainsKey(field.Key))
                {
                    _currentColumnMappings[field.Key] = DefaultMappings[field.Value];
                }
            }
        }

        public string DeleteTableScript(string tableName, string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("Value cannot be null or empty", "databaseName");
            }

            var script = new StringBuilder();

            script.AppendFormat("IF OBJECT_ID(N'{1}..[{0}]', N'U') IS NOT NULL ", tableName, databaseName);
            script.AppendFormat("DROP TABLE [{1}]..[{0}];", tableName, databaseName);

            return script.ToString();
        }

        public string CreateTableScript(string tableName, string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("Value cannot be null or empty", "databaseName");
            }

            var script = new StringBuilder();

            script.AppendFormat("CREATE TABLE [{0}]..[{1}]", databaseName, string.IsNullOrWhiteSpace(tableName) ? ClassName : tableName);

            script.AppendLine("(");
            for (int i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];

                if (_currentColumnMappings.ContainsKey(field.Key))
                {
                    script.Append("\t " + field.Key + " " + _currentColumnMappings[field.Key] + " NULL");
                }

                if (i != Fields.Count - 1)
                {
                    script.Append(",");
                }

                script.Append(Environment.NewLine);
            }

            script.AppendLine(")");

            script.AppendLine(_source.GetCreateIndexString(databaseName, tableName));

            return script.ToString();
        }

        public static Dictionary<string, string> GetCurrentMappings(Type sourceType)
        {
            var type = sourceType;
            var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyMappings = new Dictionary<string, string>();

            foreach (var property in allProperties)
            {
                var attribs = property.GetCustomAttributes(
                    typeof(ColumnAttribute), false) as ColumnAttribute[];

                if (attribs != null && attribs.Length > 0)
                {
                    var name = string.IsNullOrWhiteSpace(attribs[0].Name) ? property.Name : attribs[0].Name;
                    propertyMappings.Add(name, attribs[0].DbType);
                    //propertyMappings.Add(property.Name, attribs[0].DbType);
                }
            }

            return propertyMappings;
        }
    }
}
