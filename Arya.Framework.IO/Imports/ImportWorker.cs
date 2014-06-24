using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.IO.Imports
{
    public sealed class ImportWorker : WorkerBase
    {
        #region Fields

        #endregion Fields

        #region Constructors

        public ImportWorker(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof(ImportArgs))
        {
        }

        #endregion Constructors

        #region Properties

        public CombinedInterchangeData ImportRecords { get; set; }

        #endregion Properties

        #region Methods

        public bool IsInputValid()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            CurrentLogWriter.DebugFormat("{0}: Starting Import Worker",Arguments.Id);
            try
            {
                State = WorkerState.Working;

                var importArgs = (ImportArgs)Arguments;
                var childWorkerSummaries = new List<WorkerSummary>();
                var importFilePath = importArgs.InputFilePath;
                List<ImportWorkerBase> eligibleImports = GetEligibleWorkerAndData(importArgs, importFilePath);
                if (DuplicateTaxonomyIndb(importArgs))
                {
                    var newException = new Exception("Duplicate TaxonomyPath in the database.");
                    Summary.SetError(newException);
                    return;
                }
                foreach (var eligibleImport in eligibleImports)
                {
                    var currentEligibleImport = eligibleImport;
                    var importName = currentEligibleImport.GetType().Name;

                    CurrentLogWriter.DebugFormat("{0}: Starting {1}",Arguments.Id, importName);

                    //this is what should be logged from the external process launcher
                    currentEligibleImport.WorkerStatusChange +=
                        (s, a) =>
                            CurrentLogWriter.DebugFormat("{0}: {1} : State={2}, Message={3}", Arguments.Id, s.GetType(),
                                a.CurrentState, a.StatusMessage);
                    currentEligibleImport.CurrentLogWriter = CurrentLogWriter;
                    StatusMessage = "Running " + importName;

                    currentEligibleImport.Run();

                    childWorkerSummaries.Add(currentEligibleImport.Summary);

                    if (currentEligibleImport.Summary.HasError)
                    {
                        CurrentLogWriter.DebugFormat("{0}: HasError in {1}",Arguments.Id, importName);
                        // State = WorkerState.Error;
                        break;
                    }
                    CurrentLogWriter.DebugFormat("{0}: Finished {1}",Arguments.Id, importName);
                }

                Summary.ChildrenWorkerSummaries = childWorkerSummaries;

                if (eligibleImports.Any())
                    State = childWorkerSummaries.Max(cs => cs.State);
                else
                {
                    State = WorkerState.Error;
                    Summary.SetError(new Exception("No eligible imports found"));
                }
            }
            catch (Exception ex)
            {
                CurrentLogWriter.DebugFormat("{0}: Error: {1}",Arguments.Id, ex.Message);
                Summary.SetError(ex);
            }
            CurrentLogWriter.DebugFormat("{0}: Finished Import Worker", Arguments.Id);
        }

        private bool DuplicateTaxonomyIndb(ImportArgs importArgs)
        {

            using (var CurrentDbContext = new AryaDbDataContext(importArgs.ProjectId, importArgs.UserId))
            {

                return CurrentDbContext.ExecuteQuery<string>(@"SELECT TaxonomyPath
                                                        FROM V_Taxonomy
                                                        WHERE TaxonomyPath <> ''
                                                        AND ProjectId = {0} GROUP BY TaxonomyPath Having  Count(*) > 1", importArgs.ProjectId).Count() > 0;
            
            }
        }

        public List<string> ValidateInput()
        {
            throw new NotImplementedException();
        }

        internal CsvConfiguration GetCurrentConfiguration(ImportArgs importArgs, Type interchangeRecordType)
        {
            var currentConfiguration = new CsvConfiguration
            {
                Delimiter = importArgs.FieldDelimiter.GetValue().ToString(),
                SkipEmptyRecords = true,
                Quote = '█' //,
                //QuoteNoFields = true
            };
            var allProperties = interchangeRecordType.GetProperties();

            //map all the public properties with their indexes.
            foreach (var propertyInfo in allProperties)
            {
                var currentPropertyInfo = propertyInfo;
                var mapping = importArgs.FieldMappings.Keys.SingleOrDefault(p => p == currentPropertyInfo.Name);
                //if the mapping does not exit or equals -1, ignore it.
                if (mapping == null || importArgs.FieldMappings[mapping] == -1)
                    currentConfiguration.PropertyMap(currentPropertyInfo).Ignore();
                else
                {
                    currentConfiguration.Properties.Add(
                        currentConfiguration.PropertyMap(currentPropertyInfo).Index(importArgs.FieldMappings[mapping]));
                }
            }

            return currentConfiguration;
        }

        private List<ImportWorkerBase> GetEligibleWorkerAndData(ImportArgs importArgs,
            string importFilePath)
        {
            CurrentLogWriter.DebugFormat("{0}: Starting Get Eligible Worker And Data", Arguments.Id);
            var eligibleImports = new List<ImportWorkerBase>();
            var availableImports = ImportWorkerBase.GetAvailableImports();

            var fileName = Path.GetFileName(importArgs.InputFilePath);
            if (fileName == null)
                return new List<ImportWorkerBase>();

            foreach (var importWorker in availableImports)
            {
                var currentImportWorker = importWorker;

                currentImportWorker.CurrentFieldMappings = new Dictionary<string, int>();
                var requiredTCurrentImportWorkerFields = currentImportWorker.GetRequiredFields();

                if (fileName.EndsWith(".xml"))
                {
                    if (ImportRecords == null)
                    {
                        CurrentLogWriter.DebugFormat("{0}: Reading XML Data", Arguments.Id);
                        var str = new StreamReader(importArgs.InputFilePath);
                        var xSerializer = new XmlSerializer(typeof(CombinedInterchangeData));
                        ImportRecords = (CombinedInterchangeData)xSerializer.Deserialize(str);
                    }
                }
                else
                {
                    if (ImportRecords == null)
                        ImportRecords = new CombinedInterchangeData(true);
                    //eligible only if all the required field maps are available.
                    if (!requiredTCurrentImportWorkerFields.All(p => importArgs.FieldMappings.Keys.Contains(p)))
                    {
                        Summary.StatusMessage = string.Format("Not Eligible for Import : {0}",
                            currentImportWorker.GetType());
                        continue;
                    }
                    foreach (var requiredCurrentImportWorkerField in requiredTCurrentImportWorkerFields)
                    {
                        currentImportWorker.CurrentFieldMappings.Add(requiredCurrentImportWorkerField,
                            importArgs.FieldMappings[requiredCurrentImportWorkerField]);
                    }
                    //map all the optional fields
                    var optionalImportWorkerFields = currentImportWorker.GetOptionalFields();
                    foreach (
                        var optionalImportWorkerField in
                            optionalImportWorkerFields.Where(importArgs.FieldMappings.ContainsKey))
                    {
                        currentImportWorker.CurrentFieldMappings.Add(optionalImportWorkerField,
                            importArgs.FieldMappings[optionalImportWorkerField]);
                    }

                    var conf = GetCurrentConfiguration(importArgs, currentImportWorker.CurrentInterchangeRecordType);
                    //MOve this to base to somewhere common
                    using (var csvReader = new CsvReader(File.OpenText(importArgs.InputFilePath), conf))
                    {
                        CurrentLogWriter.DebugFormat("{0}: Reading Text Data for {1}", Arguments.Id, currentImportWorker.GetType().Name);
                        var inportData =
                            csvReader.GetRecordsWithNulls(currentImportWorker.CurrentInterchangeRecordType).ToList();

                        var method = ImportRecords.GetType().GetMethod("AddRecords");
                        var generic = method.MakeGenericMethod(currentImportWorker.CurrentInterchangeRecordType);
                        generic.Invoke(ImportRecords, new object[] { inportData });
                    }
                }

                eligibleImports.Add(currentImportWorker);
                currentImportWorker.CurrentImportOptions = importArgs.CurrentImportOptions;
                currentImportWorker.CurrentProjectId = importArgs.ProjectId;
                currentImportWorker.ImportRequestedBy = importArgs.UserId;
                currentImportWorker.InputFilePath = importFilePath;
                currentImportWorker.FieldDelimiter = importArgs.FieldDelimiter;
                currentImportWorker.CurrentFieldMappings = new Dictionary<string, int>();
                currentImportWorker.JobDescription = importArgs.JobDescription;
                currentImportWorker.ImportData = ImportRecords;
            }
            ImportRecords.DedupLists();

            CurrentLogWriter.DebugFormat("{0}: Eligible Workers:",Arguments.Id);
            foreach (var import in eligibleImports)
            {
                CurrentLogWriter.Debug(import.GetType().Name);
            }
            return eligibleImports;
        }

        #endregion Methods
    }
}