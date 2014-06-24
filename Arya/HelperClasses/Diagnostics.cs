using System;
using System.IO;
using System.Windows.Forms;
using Arya.Data;
using System.Threading;
using System.Collections.Concurrent;
using System.Data.SqlClient;

namespace Arya.HelperClasses
{
    public static class Diagnostics
    {

        private static readonly Guid _sessionID = Guid.NewGuid();

        public static Guid SessionID
        {
            get
            {
                return _sessionID;
            }
        }

        internal static void WriteToDb(LogRecord record)
        {
            using (var connection = new SqlConnection("Data Source=dev.empirisense.com;Initial Catalog=AryaDiagnostics;Persist Security Info=True;User ID=aryaUser;Password=arya$tark;MultipleActiveResultSets=True"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO AryaDiagnostics..TimeLog (SessionID,TimeStamp, ProjectName, Operation, Method, TimeTaken,NoOfSkus, NoOfNodes, NoOfAttributes)VALUES (@SessionID,@TimeStamp, @ProjectName, @Operation, @Method, @TimeTaken, @NoOfSkus, @NoOfNodes, @NoOfAttributes)";
                    command.Parameters.AddWithValue("@SessionID", record.SessionID);
                    command.Parameters.AddWithValue("@TimeStamp", record.TimeStamp);
                    command.Parameters.AddWithValue("@ProjectName", record.ProjectName.Trim());
                    command.Parameters.AddWithValue("@Operation", record.Operation);
                    command.Parameters.AddWithValue("@Method", record.Method);
                    command.Parameters.AddWithValue("@TimeTaken", record.TimeTaken);
                    command.Parameters.AddWithValue("@NoOfSkus", record.NoOfSkus ?? 0);
                    command.Parameters.AddWithValue("@NoOfNodes", record.NoOfNodes ?? 0);
                    command.Parameters.AddWithValue("@NoOfAttributes", record.NoOfAttributes ?? 0);
                    command.ExecuteNonQuery();
                }
            }
        }

        internal static void WriteMessage(string Operation, string Method, TimeSpan TimeElapased, int? NoOfSkus = null, int? NoOfNodes = null, int? NoOfAttributes = null)
        {
            try
            {
                var newLogRecord = new LogRecord(SessionID, DateTime.Now,
                                                 AryaTools.Instance.InstanceData.CurrentProject == null
                                                     ? string.Empty
                                                     : AryaTools.Instance.InstanceData.CurrentProject.ProjectName, Operation,
                                                 Method, TimeElapased, NoOfSkus, NoOfNodes, NoOfAttributes);


                //System.Threading.Tasks.Task.Factory.StartNew(() => WriteToDb(newLogRecord));

            }
            catch (Exception) { }
        }

    }

    public class LogRecord
    {
        public Guid SessionID;
        public DateTime TimeStamp;
        public string ProjectName;
        public string Operation;
        public string Method;
        public TimeSpan TimeTaken;
        public int? NoOfSkus;
        public int? NoOfNodes;
        public int? NoOfAttributes;

        public LogRecord(Guid sessionID, DateTime timeStamp, string projectName, string operation, string method, TimeSpan timeTaken, int? noOfSkus, int? noOfNodes, int? noOfAttributes)
        {
            SessionID = sessionID;
            TimeStamp = timeStamp;
            ProjectName = projectName;
            Operation = operation;
            Method = method;
            TimeTaken = timeTaken;
            NoOfSkus = noOfSkus;
            NoOfNodes = noOfNodes;
            NoOfAttributes = noOfAttributes;

        }

    }
}
