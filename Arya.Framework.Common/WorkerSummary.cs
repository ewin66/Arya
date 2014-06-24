using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Arya.Framework.Common
{
    [Serializable]
    public struct CustomKeyValuePair<TK, TV>
    {
        #region Constructors

        public CustomKeyValuePair(TK k, TV v)
            : this()
        {
            Key = k;
            Value = v;
        }

        #endregion Constructors

        #region Properties

        public TK Key { get; set; }

        public TV Value { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString() { return string.Format("{0}: {1}", Key, Value); }

        #endregion Methods
    }

    //TODO: ask Vivek why worker error has messages.
    [Serializable]
    public class WorkerError
    {
        #region Fields

        private List<string> _messages;

        #endregion Fields

        #region Properties

        public string ExceptionType { get; set; }

        public List<string> Messages
        {
            get { return _messages ?? (_messages = new List<string>()); }
            // set { _messages = value; }
        }

        public string StackTrace { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return ExceptionType + Environment.NewLine + StackTrace + Environment.NewLine
                   + Messages.Aggregate((current, next) => current + Environment.NewLine + next);
        }

        #endregion Methods
    }

    [Serializable]
    [XmlRoot("WorkerSummary")]
    public class WorkerSummary
    {
        #region Fields

        [IgnoreDataMember]
        private readonly List<CustomKeyValuePair<DateTime, string>> _statusMessage =
            new List<CustomKeyValuePair<DateTime, string>>();

        private List<WorkerWarning> _warnings;

        #endregion Fields

        #region Constructors

        public WorkerSummary(string workerName) : this() { WorkerName = workerName; }

        public WorkerSummary()
        {
            //TotalLine = 0;
            State = WorkerState.Error;
        }

        public WorkerSummary(WorkerSummary summary)
        {
            WorkerName = summary.WorkerName;
            //this.Details = new List<CustomKeyValuePair<string, int>>(summary.Details?? new CustomKeyValuePair<string, int>[]);
            State = summary.State;
            _statusMessage = new List<CustomKeyValuePair<DateTime, string>>(summary._statusMessage);

            Details = summary.Details ?? null;
            Warnings = summary.Warnings ?? null;
            Error = summary.Error;
            //this.Warnings = new List<WorkerWarning>(summary.Warnings);
            if (summary.ChildrenWorkerSummaries != null)
            {
                // this.ChildrenWorkerSummaries = new List<WorkerSummary>(summary.ChildrenWorkerSummaries.Count);
                var clildSummarylist = new List<WorkerSummary>(summary.ChildrenWorkerSummaries.Count);
                clildSummarylist.AddRange(summary.ChildrenWorkerSummaries.Select(cws => new WorkerSummary(cws)));
                ChildrenWorkerSummaries = clildSummarylist;
            }
        }

        #endregion Constructors

        #region Properties

        public List<WorkerSummary> ChildrenWorkerSummaries { get; set; }

        public List<CustomKeyValuePair<string, int>> Details { get; set; }

        public WorkerError Error { get; set; }

        [XmlIgnore]
        public bool HasError
        {
            get { return Error != null; }
        }

        public WorkerState State { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public string StatusMessage
        {
            get { return _statusMessage.Count == 0 ? String.Empty : _statusMessage[_statusMessage.Count - 1].Value; }
            set { _statusMessage.Add(new CustomKeyValuePair<DateTime, string>(DateTime.Now, value)); }
        }

        public List<WorkerWarning> Warnings
        {
            get { return _warnings ?? (_warnings = new List<WorkerWarning>()); }
            set { _warnings = value; }
        }

        [XmlAttribute("WorkerName", DataType = "string")]
        public string WorkerName { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets Status to Abort and copies stack trace and all messages from exception
        /// </summary>
        /// <param name="ex"></param>
        public void SetError(Exception ex)
        {
            State = WorkerState.Abort;

            Error = new WorkerError { ExceptionType = ex.GetType().ToString(), StackTrace = ex.StackTrace };
            var x = ex;
            while (x != null)
            {
                Error.Messages.Add(x.Message + '\n' + x.StackTrace);
                x = x.InnerException;
            }
        }

        #endregion Methods

        #region Other

        //private WorkerError _error;

        #endregion Other
    }

    [Serializable]
    public class WorkerWarning
    {
        #region Properties

        public string ErrorDetails { get; set; }

        public string ErrorMessage { get; set; }

        public string LineData { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return LineData + "|" + ErrorMessage + "|" + ErrorDetails;
        }

        #endregion Methods
    }
}