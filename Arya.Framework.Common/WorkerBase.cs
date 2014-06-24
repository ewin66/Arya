using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using log4net;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework.Utility;
using Polenter.Serialization;

namespace Arya.Framework.Common
{

    #region Enumerations

    public enum Delimiter
    {
        [DisplayTextAndValue("Tab", '\t')]
        Tab,
        [DisplayTextAndValue("Comma", ',')]
        Comma,
        [DisplayTextAndValue("Semi-Colon", ';')]
        SemiColon,
        [DisplayTextAndValue("Pipe", '|')]
        Pipe
    }

    //IMPORTANT: Always order this by Severity
    public enum WorkerState
    {
        New,
        Working,
        Saving,
        Ready,
        Complete,
        CompletedWithWarning,
        Abort,
        AbortedByUser,
        Error
    }

    #endregion Enumerations

    [Serializable]
    public class WorkerArguments
    {
        #region Fields

        public const string ArgumentsFileName = "Arguments.xml";
        public const string ArgumentsFileRootName = "Arguments";

        protected const string CaptionOptional = "Optional";
        protected const string CaptionRequired = "\tRequired";
        protected const int OptionalBaseOrder = 100;
        protected const int RequiredBaseOrder = 10;
        public string PortalUrl;

        private string _hiddenProperties;

        #endregion Fields

        #region Constructors

        public WorkerArguments() { HiddenProperties = "Exceptions"; }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        [Browsable(false)]
        [XmlIgnore]
        public string HiddenProperties
        {
            get { return _hiddenProperties; }
            set { _hiddenProperties = value; }
        }

        [Browsable(false)]
        public Guid Id { get; set; }

        [Category(CaptionRequired)]
        [Description(
            "The name of the export to display on the job status page and in notification e-mails. Use this name to track your export jobs."
            )]
        [DisplayName("Task Description")]
        [PropertyOrder(RequiredBaseOrder + 1)]
        [Browsable(true)]
        public string JobDescription { get; set; }

        [Category(CaptionRequired)]
        [Description("Comma or semi-colon separated list of email addresses.")]
        [DisplayName("Notification Email Addresses")]
        [PropertyOrder(RequiredBaseOrder + 2)]
        [Browsable(true)]
        public string NotificationEmailAddresses { get; set; }

        [Browsable(false)]
        public Guid ProjectId { get; set; }

        [Browsable(false)]
        public Guid UserId { get; set; }

        #endregion Properties

        #region Methods

        public bool ShouldSerializeHiddenProperties() { return false; }

        protected static void SetPropertyAttribute(Type sourceType, string propertyName, string attributeName,
            object value)
        {
            var descriptor = TypeDescriptor.GetProperties(sourceType)[propertyName];
            var attribute = (BrowsableAttribute)descriptor.Attributes[typeof(BrowsableAttribute)];

            var fieldToChange = attribute.GetType()
                .GetField(attributeName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldToChange != null)
                fieldToChange.SetValue(attribute, value);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion Methods
    }

    public abstract class WorkerBase
    {
        #region Fields

        public const string ArgumentFileName = "Arguments.xml";
        public const string CaptionOptional = "Optional";
        public const string CaptionRequired = "\tRequired";
        public const string LogFileName = "log.xml";
        public const int OptionalBaseOrder = 0;
        public const int RequiredBaseOrder = 0;

        protected bool NeedsArgumentFile = true;

        private WorkerSummary _summary;
        private WorkerArguments _arguments;

        #endregion Fields

        #region Constructors

        protected WorkerBase(bool needArgument) { NeedsArgumentFile = needArgument; }

        [Obsolete("Use WorkerBase(string argumentDirectoryPath, Type type) instead")]
        protected WorkerBase(string argumentDirectoryPath)
            : this(argumentDirectoryPath, null)
        {
        }

        protected WorkerBase(string argumentDirectoryPath, Type type)
        {
            if (type != null)
                ArgumentsType = type;

            if (String.IsNullOrEmpty(argumentDirectoryPath))
                return;

            if (!NeedsArgumentFile)
                return;
            //string argFileDirectory = Path.GetDirectoryName(argumentDirectoryPath);
            //TODO: Log this information
            if (!Directory.Exists(argumentDirectoryPath))
                throw new DirectoryNotFoundException(argumentDirectoryPath + " not found");

            //TODO: Log this information
            if (!File.Exists(Path.Combine(argumentDirectoryPath, ArgumentFileName)))
                throw new FileNotFoundException();

            ArgumentDirectoryPath = argumentDirectoryPath;
        }

        #endregion Constructors

        #region Delegates

        public delegate void WorkerStatusChangeHandler(object sender, WorkerStatusUpdateEventArgs data);

        #endregion Delegates

        #region Events

        public event WorkerStatusChangeHandler WorkerStatusChange;

        #endregion Events

        #region Properties

        [Browsable(false)]
        public string ArgumentDirectoryPath { get; private set; }

        public WorkerArguments Arguments
        {
            get
            {
                if (_arguments == null)
                    ReadArgumentsFile();

                return _arguments;
            }
            set { _arguments = value; }
        }

        public Type ArgumentsType { get; private set; }

        [Browsable(false)]
        public ILog CurrentLogWriter { protected get; set; }

        [Browsable(false)]
        public int CurrentProgress { get; set; }

        [Browsable(false)]
        public Guid CurrentProjectId { get; set; }

        [Browsable(false)]
        public int MaximumProgress { get; set; }

        [Browsable(false)]
        public WorkerState State
        {
            get { return Summary.State; }
            protected set
            {
                Summary.State = value;
                CurrentLogWriter.DebugFormat("{0}: State:{1}. Status:{2}", Arguments.Id, State, StatusMessage);
                OnWorkerStatusChange(this, StatusMessage, value);
            }
        }
        
        public static string GetFriendlyWorkerState(WorkerState state)
        {
            switch (state)
            {
                case WorkerState.New:
                    return "Queued";
              
                case WorkerState.Complete:
                    return "Completed";

                case WorkerState.Abort:
                    return "Aborted";

                case WorkerState.CompletedWithWarning:
                    return "Completed with warning(s)";

                case WorkerState.AbortedByUser:
                    return "Aborted by user";

                default:
                    return state.ToString();
            }
        }

        [Browsable(false)]
        public string StatusMessage
        {
            get { return Summary.StatusMessage; }
            protected set
            {
                Summary.StatusMessage = value;
                CurrentLogWriter.DebugFormat("{0}: State:{1}. Status:{2}", Arguments.Id, State, StatusMessage);
                OnWorkerStatusChange(this, value, Summary.State);
            }
        }

        [Browsable(false)]
        public WorkerSummary Summary
        {
            get { return _summary ?? (_summary = new WorkerSummary(GetType().Name)); }
            //private set { _summary = value; }
        }

        public Guid WorkerId { get; set; }

        #endregion Properties

        #region Methods

        public static string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences,
            int numParagraphs)
        {
            var words = new[]
                        {
                            "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam",
                            "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam",
                            "erat"
                        };

            var rand = new Random(DateTime.Now.Millisecond);
            var numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
            var numWords = rand.Next(maxWords - minWords) + minWords + 1;

            var result = String.Empty;

            for (var p = 0; p < numParagraphs; p++)
            {
                if (numParagraphs > 1)
                    result += "<p>";
                for (var s = 0; s < numSentences; s++)
                {
                    for (var w = 0; w < numWords; w++)
                    {
                        if (w > 0)
                            result += " ";

                        result += words[rand.Next(words.Length)];
                    }
                    if (numSentences > 1)
                        result += ". ";
                }
                if (numParagraphs > 1)
                    result += "</p>";
            }

            return result;
        }

        //[Obsolete("Use void ReadArgumetsFile() instead; and use the Arguments property.")]
        //public T ReadArguments<T>(string argumentDirectoryPath) where T : WorkerArguments
        //{
        //    ArgumentDirectoryPath = argumentDirectoryPath;
        //    ReadArgumentsFile();
        //    return (T)Arguments;
        //}
        public static void SaveArguments<T>(T arguments, string folderPath,
            string fileName = WorkerArguments.ArgumentsFileName) where T : WorkerArguments
        {
            var folderTest = Directory.Exists(folderPath);

            if (folderTest == false)
                throw new ArgumentException("Folder path is not valid", folderPath);

            var settings = typeof(T).GetSharpSerializerXmlSettings(WorkerArguments.ArgumentsFileRootName);
            var serializer = new SharpSerializer(settings);
            serializer.Serialize(arguments, Path.Combine(folderPath, fileName));
        }

        public abstract void Run();

        public void SaveSummary()
        {
            var serializer = new XmlSerializer(typeof(WorkerSummary));
            var outputFilePath = Path.Combine(ArgumentDirectoryPath, LogFileName);
            var fs = new FileStream(outputFilePath, FileMode.Create);

            var newWs = new WorkerSummary(Summary);

            serializer.Serialize(fs, newWs);
            fs.Close();
            // serializer.Serialize(fs, Summary);
        }

        protected static int GetNumberOfColumns(string inputFilePath, char fieldDelimiter)
        {
            using (TextReader file = new StreamReader(inputFilePath))
            {
                var readLine = file.ReadLine();
                return readLine != null ? readLine.Split(fieldDelimiter).Count() : 0;
            }
        }

        private void ReadArgumentsFile()
        {
            if (ArgumentsType == null)
            {
                Arguments = new WorkerArguments();
                return;
            }

            if (CurrentLogWriter != null)
                CurrentLogWriter.DebugFormat("Reading Arguments");

            var settings = ArgumentsType.GetSharpSerializerXmlSettings(WorkerArguments.ArgumentsFileRootName);
            var serializer = new SharpSerializer(settings);

            Arguments = (WorkerArguments)serializer.Deserialize(Path.Combine(ArgumentDirectoryPath, ArgumentFileName));
        }

        private void OnWorkerStatusChange(object sender, string statusMessage, WorkerState currentState)
        {
            var handler = WorkerStatusChange;
            if (handler != null)
                handler((sender ?? this), new WorkerStatusUpdateEventArgs(WorkerId, statusMessage, currentState));
        }

        #endregion Methods
    }

    public class WorkerStatusUpdateEventArgs : EventArgs
    {
        #region Constructors

        public WorkerStatusUpdateEventArgs(Guid workerId, string statusMessage, WorkerState currentState)
        {
            WorkerId = workerId;
            StatusMessage = statusMessage;
            CurrentState = currentState;
        }

        #endregion Constructors

        #region Properties

        public WorkerState CurrentState { get; private set; }

        public string StatusMessage { get; private set; }

        public Guid WorkerId { get; private set; }

        #endregion Properties
    }
}