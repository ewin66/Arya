using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Utility;

namespace Arya.Framework.IO.Exports
{
    [Serializable]
    public class CustomExportArgs : WorkerArguments
    {
        #region Constructors

        public CustomExportArgs() { PortalUrl = "Export.aspx"; }

        #endregion Constructors

        #region Properties

        public string DatabaseNames { get; set; }

        public double DatabaseVersion { get; set; }

        public string Name { get; set; }

        [DefaultValue("\t")]
        public string Delimiter { get; set; }

        public string Description { get; set; }

        [DefaultValue(false)]
        public bool ExportEmptyFiles { get; set; }

        [DefaultValue(false)]
        public bool ExportExcelFiles { get; set; }

        [DefaultValue(false)]
        public bool GenerateQueriesOnly { get; set; }

        public List<Parameter> GlobalParameters { get; set; }

        public List<CustomQuery> Queries { get; set; }


        #endregion Properties
    }

    public class CustomExportWorker : WorkerBase
    {
        #region Constructors

        public CustomExportWorker(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof(CustomExportArgs))
        {
        }

        #endregion Constructors

        #region Properties

        private CustomExportArgs Queries
        {
            get { return (CustomExportArgs)Arguments; }
        }

        #endregion Properties

        #region Methods

        public virtual bool IsInputValid()
        {
            return ValidateParameters(Queries.GlobalParameters)
                   && Queries.Queries.All(query => ValidateParameters(query.Parameters));
        }

        public override void Run()
        {
            State = WorkerState.Working;

            Queries.Queries.ForEach(q => q.Directory = ArgumentDirectoryPath);

            var databases = GetDatabases();
            if (databases.Count == 0)
            {
                Summary.Error = new WorkerError
                                {
                                    ExceptionType =
                                        "No databases that match the requested criteria were found."
                                };
                State = WorkerState.Abort;
                return;
            }

            //We're expecting at least one column with the SQL Query
            //An optional second column may contain the target filename, especially if multiple queries are returned
            Summary.Details = new List<CustomKeyValuePair<string, int>>
                              {
                                  new CustomKeyValuePair<string, int>(
                                      "Number of Top Level Queries",
                                      Queries.Queries.Count)
                              };

            foreach (var query in Queries.Queries)
            {
                if (query.Execute == false)
                    continue;

                foreach (var database in databases)
                {
                    if (!ExecuteQuery(database, query, databases.Count > 1))
                        return;
                }
            }

            State = WorkerState.Complete;
        }

        public virtual List<string> ValidateInput() { throw new NotImplementedException(); }

        private static string GetFilePath(string database, CustomQuery query, bool multipleDatabases, string filename)
        {
            var filePath = "results.txt";
            if (!string.IsNullOrWhiteSpace(filename))
            {
                filePath = multipleDatabases
                    ? string.Format("{0} {1} {2}{3}", database, Path.GetFileNameWithoutExtension(filename),
                        DateTime.Now.ToString("yyyy-MM-dd HH-mm"), Path.GetExtension(filename))
                    : filename;
            }

            return Path.Combine(query.Directory, filePath);
        }

        private static bool ValidateParameters(IEnumerable<Parameter> parameters)
        {
            return parameters.All(parameter => !parameter.Required || !string.IsNullOrWhiteSpace(parameter.Value));
        }

        private bool ExecuteQuery(string database, CustomQuery query, bool multipleDatabases)
        {
            using (var dc = new SqlConnection(Framework.Properties.Settings.Default.AryaDbConnectionString))
            {
                dc.Open();
                dc.ChangeDatabase(database);

                //First execute the query generator sql
                var commandQuery = new SqlCommand(query.QueryGeneratorSql, dc) { CommandTimeout = 0 };
                Queries.GlobalParameters.ForEach(p => commandQuery.Parameters.AddWithValue(p.Name, p.Value));
                query.Parameters.ForEach(p => commandQuery.Parameters.AddWithValue(p.Name, p.Value));

                var logFilePath = GetFilePath(string.Empty, query, false, "outerQuery.sql");
                using (TextWriter file = new StreamWriter(logFilePath, true))
                {
                    file.WriteLine("--{0} {1}", DateTime.Now, query.Name);
                    file.WriteLine("USE [{0}]", database);
                    file.WriteLine(commandQuery.CommandText);
                }

                var sqlTable = new DataTable("SqlQueries");
                SqlDataReader queries;
                try
                {
                    queries = commandQuery.ExecuteReader();
                }
                catch (SqlException ex)
                {
                    var message = ex.Errors.Cast<SqlError>()
                        .Aggregate("Sql Exception: ",
                            (current, error) =>
                                current + string.Format("{0} at line {1}. ", error.Message, error.LineNumber));
                    Summary.SetError(new Exception(message, ex));
                    return false;
                }
                using (queries)
                {
                    if (queries.HasRows)
                        sqlTable.Load(queries);
                    else
                        throw new Exception("No queries were generated by the SqlGenerator");
                }

                using (TextWriter file = new StreamWriter(logFilePath, true))
                    file.WriteLine("--{0} {1} Queries Generated", DateTime.Now, sqlTable.Rows.Count);

                var queryFilePath = GetFilePath(string.Empty, query, false, "innerQueries.sql");
                var iCtr = 0;
                foreach (DataRow sqlRow in sqlTable.Rows)
                {
                    var filePath = GetFilePath(database, query, multipleDatabases,
                        sqlTable.Columns.Count == 1 ? query.Filename : sqlRow[1].ToString());

                    //Write (append) the query to a log/sql file
                    var queryText = sqlRow[0].ToString();

                    using (TextWriter file = new StreamWriter(queryFilePath, true))
                    {
                        file.WriteLine("--{0} {1}", DateTime.Now,
                            query.Name + (sqlTable.Rows.Count == 1 ? string.Empty : "[" + (++iCtr) + "]"));
                        file.WriteLine("USE [{0}]", database);
                        file.WriteLine(queryText);
                    }

                    if (Queries.GenerateQueriesOnly)
                        //Do not execute the inner queries, just generate them
                        continue;

                    using (TextWriter file = new StreamWriter(logFilePath, true))
                        file.WriteLine("--{0} Running Query {1}", DateTime.Now, iCtr);

                    var start = DateTime.Now;
                    if (!ProcessQuery(dc, queryText, filePath))
                        return false;

                    var timeTaken = DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss");

                    using (TextWriter file = new StreamWriter(logFilePath, true))
                    {
                        file.WriteLine("--{0} Time taken: {1}", DateTime.Now, timeTaken);
                        file.WriteLine();
                    }
                }
            }
            return true;
        }

        private List<string> GetDatabases()
        {
            if (string.IsNullOrWhiteSpace(Queries.DatabaseNames))
                throw new Exception("No Databases Found!");

            var databases = new List<string>();

            using (var dc = new SqlConnection(Framework.Properties.Settings.Default.AryaDbConnectionString))
            {
                dc.Open();

                var whereClause = Queries.DatabaseVersion > 0.0 ? " WHERE AryaCodeBaseVersion=" + Queries.DatabaseVersion : string.Empty;
                var dbQuery =
                    new SqlCommand(
                        "SELECT name FROM sys.databases WHERE name IN (SELECT DatabaseName FROM Arya..Project" + whereClause + ")", dc);
                var dbTable = new DataTable("SqlQueries");
                using (var dbs = dbQuery.ExecuteReader())
                {
                    if (dbs.HasRows)
                    {
                        dbTable.Load(dbs);
                        databases.AddRange(dbTable.Rows.Cast<DataRow>().Select(row => row[0].ToString()));
                    }
                }
            }

            if (Queries.DatabaseNames != "*")
            {
                var dbs =
                    Queries.DatabaseNames.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(db => db.ToLower().Trim());
                databases = databases.Where(db => dbs.Contains(db.ToLower())).ToList();
            }

            if (databases.Count == 0)
            {
                CurrentLogWriter.Error("No Databases Found!");
                //throw new Exception("No Databases Found!");
            }

            return databases;
        }

        private bool ProcessQuery(SqlConnection dc, string cmdText, string filePath)
        {
            //Filename received from the dataTable gets precedence over the filename specified in the xml
            var tb = new DataTable("Results");

            //Then, execute the actual query to get the data results
            var resultsQuery = new SqlCommand(cmdText, dc) { CommandTimeout = 0 };
            SqlDataReader resultsReader;
            try
            {
                resultsReader = resultsQuery.ExecuteReader();
            }
            catch (SqlException ex)
            {
                var message = ex.Errors.Cast<SqlError>()
                    .Aggregate("Sql Exception: ",
                        (current, error) =>
                            current + string.Format("{0} at line {1}. ", error.Message, error.LineNumber));
                Summary.SetError(new Exception(message, ex));
                return false;
            }
            using (resultsReader)
                tb.Load(resultsReader);

            //If no filename is available, either through the query or through the xml, don't write to file
            if (string.IsNullOrWhiteSpace(filePath))
                return true;

            //If ExportEmptyFiles is false, and there is no data, then do not write a file
            if (tb.Rows.Count == 0 && !Queries.ExportEmptyFiles)
                return true;

            if (Queries.ExportExcelFiles)
                tb.SaveExcelFile(Path.ChangeExtension(filePath, "xlsx"));
            else
                tb.SaveTextFile(filePath, Queries.Delimiter);

            return true;
        }

        #endregion Methods
    }

    [Serializable]
    public class CustomQuery
    {
        #region Properties

        public string Description { get; set; }

        [XmlIgnore]
        public string Directory { get; set; }

        [DefaultValue(true)]
        public bool Execute { get; set; }

        public string Filename { get; set; }

        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; }

        public string QueryGeneratorSql { get; set; }

        #endregion Properties
    }

    [Serializable]
    public class Parameter
    {
        #region Properties

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public bool Hidden { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool NewId { get; set; }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool Required { get; set; }

        [XmlText]
        public string Value { get; set; }

        #endregion Properties
    }
}