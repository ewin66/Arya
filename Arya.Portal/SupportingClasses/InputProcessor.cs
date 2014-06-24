using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Linq;

namespace AryaPortal.SupportingClasses
{
    abstract class InputProcessor
    {
        public abstract IEnumerable<string> GetTables();
        public abstract IEnumerable<string> GetHeaderColumns(string tableName);
        public abstract DataTable GetSampleData(string tableName);
    }

    internal class TextProcessor : InputProcessor
    {
        public TextProcessor(FileInfo file)
        {
            Init(file);
        }

        ~TextProcessor()
        {
            Connection.Close();
        }

        private FileInfo FileName { get; set; }
        private OleDbConnection Connection { get; set; }

        private void Init(FileInfo file)
        {
            FileName = file;

            var connectionString = string.Format(
                ConfigurationManager.ConnectionStrings[file.Extension].ConnectionString,
                file.FullName);

            Connection = new OleDbConnection(connectionString);
        }

        public override IEnumerable<string> GetTables()
        {
            yield return FileName.Name;
        }

        public override IEnumerable<string> GetHeaderColumns(string tableName)
        {
            throw new NotImplementedException();
        }

        public override DataTable GetSampleData(string tableName)
        {
            throw new NotImplementedException();
        }
    }

    class ExcelProcessor : InputProcessor
    {
        public ExcelProcessor(FileInfo file)
        {
            Init(file);
        }

        private FileInfo FileName { get; set; }
        private OleDbConnection Connection { get; set; }

        private void Init(FileInfo file)
        {
            FileName = file;

            var connectionString = string.Format(
                ConfigurationManager.ConnectionStrings[file.Extension].ConnectionString,
                file.FullName);

            Connection = new OleDbConnection(connectionString);
            Connection.Open();
        }

        ~ExcelProcessor()
        {
            try
            {
                Connection.Close();
            }
            catch (Exception)
            {
            }
        }

        public override IEnumerable<string> GetTables()
        {
            var tables = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (tables == null)
                yield break;

            foreach (DataRow table in tables.Rows)
                yield return table["TABLE_NAME"].ToString();
        }

        public override IEnumerable<string> GetHeaderColumns(string tableName)
        {
            var columns = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                                                         new object[] { null, null, tableName, null });

            if (columns == null)
                yield break;

            foreach (var col in columns.AsEnumerable().OrderBy(row => row["ORDINAL_POSITION"]))
            {
                yield return col["COLUMN_NAME"].ToString();
            }
        }

        public override DataTable GetSampleData(string tableName)
        {
            var adapter = new OleDbDataAdapter(string.Format("SELECT TOP 10 * FROM [{0}]", tableName), Connection);
            var datatable = new DataTable();

            adapter.Fill(datatable);

            return datatable;
        }
    }
}
