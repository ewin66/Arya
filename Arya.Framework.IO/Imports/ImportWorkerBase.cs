using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms.Design;
using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Polenter.Serialization;

namespace Arya.Framework.IO.Imports
{
    public abstract class ImportWorkerBase : WorkerBase
    {
        private const string DefaultImportUtilRemarkText = "ImportUtil";
        //public const string NewRecordCountIdentifierText = "New Record:";
        //public const string UpdatedRecordCountIdentifierText = "Updated Record:";
        //public const string IgnoredRecordCountIdentifierText = "Ignored Record:";
        protected const string BaseImagePath = "http://dev.empirisense.com/AryaAssets";
        private static readonly Regex BadQuotes = new Regex(@"(\"")(\1){0,}", RegexOptions.Compiled);
        public AryaDbDataContext CurrentDbContext;
        public Dictionary<string, int> CurrentFieldMappings;

        public ImportOptions CurrentImportOptions;
        public Type CurrentInterchangeRecordType;
        public Guid ImportId = Guid.NewGuid();

        public Guid ImportRequestedBy;
        [Editor(typeof (FileNameEditor), typeof (UITypeEditor))] 
        public string InputFilePath;
        public int UpdateFrequency;
        public string JobDescription;
        private Guid _currentRemarkId = Guid.Empty;
        public CombinedInterchangeData ImportData;
        private bool _duplicateChecked;
        private bool _duplicateTaxonomyCheckResult;
        //public string  { get; set; }

        protected ImportWorkerBase() : base(false)
        {
            NeedsArgumentFile = false;
            CurrentInterchangeRecordType = typeof (InterchangeRecord);
        }
       
        [Description("Field Delimiter to be used in the exported file"), DisplayName(@"Field Delimiter")]
        [Category(CaptionRequired), DefaultValue(Delimiter.Tab), PropertyOrder(RequiredBaseOrder + 1)]
        [TypeConverter(typeof (CustomEnumConverter))]
        public Delimiter FieldDelimiter { get; set; }

        internal CsvConfiguration GetCurrentConfiguration()
        {
            var currentConfiguration = new CsvConfiguration
                {
                    Delimiter = FieldDelimiter.GetValue().ToString(),
                    SkipEmptyRecords = true,
                    Quote = '█'//,
                    //QuoteNoFields = true
                };
            var allProperties = CurrentInterchangeRecordType.GetProperties();

            //map all the public properties with their indexes.
            foreach (var propertyInfo in allProperties)
            {
                var currentPropertyInfo = propertyInfo;
                var mapping = CurrentFieldMappings.Keys.SingleOrDefault(p => p == currentPropertyInfo.Name);
                //if the mapping does not exit or equals -1, ignore it.
                if (mapping == null || CurrentFieldMappings[mapping] == -1)
                    currentConfiguration.PropertyMap(currentPropertyInfo).Ignore();
                else
                {
                    currentConfiguration.Properties.Add(
                        currentConfiguration.PropertyMap(currentPropertyInfo).Index(CurrentFieldMappings[mapping]));
                }
            }

            return currentConfiguration;
        }
        
        public void SaveDataChanges()
        {
            State = WorkerState.Saving;
            CurrentDbContext.SubmitChanges();
            //CurrentDbContext.GetChangeSet();
            State = WorkerState.Working;
        }

        public virtual string[] GetRequiredFields()
        {
            var requiredImportFields =
                CurrentInterchangeRecordType.GetProperties().Where(
                    p =>
                    p.IsDefined(typeof (CategoryAttribute), true)
                    &&
                    p.GetCustomAttributes(typeof (CategoryAttribute), true).Cast<CategoryAttribute>().Single().Category== CaptionRequired).Select(p => p.Name).ToArray();

            return requiredImportFields;
        }

        public virtual string[] GetOptionalFields()
        {
            var optionalImportFields =
                CurrentInterchangeRecordType.GetProperties().Where(
                    p =>
                    p.IsDefined(typeof (CategoryAttribute), true)
                    &&
                    p.GetCustomAttributes(typeof(CategoryAttribute), true).Cast<CategoryAttribute>().Single().Category == CaptionOptional).Select(p => p.Name).ToArray();

            return optionalImportFields;
        }

        public static string[] GetFieldHeaders(string filePath, Delimiter fieldDelimiter)
        {
            using (var csvReader = new CsvReader(File.OpenText(filePath)))
            {
                csvReader.Configuration.Delimiter = fieldDelimiter.GetValue().ToString();
                return csvReader.FieldHeaders;
            }
        }

        public static List<ImportWorkerBase> GetAvailableImports()
        {
            //all import types sorted according to their order
            var allImports =
                Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof (ImportWorkerBase))).OrderBy(
                    p => p.GetAttributeValue((ImportOrderAttribute o) => o.Order)).Select(
                        p => (ImportWorkerBase) Activator.CreateInstance(p)).ToList();

            return allImports;
        }

        public static List<string> GetAvailableInterchangeRecordFields()
        {
            var allInterchangeRecordFields =
                Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof (InterchangeRecord))).
                    SelectMany(p => p.GetProperties()).Select(p => p.Name).Distinct().ToList();

            return allInterchangeRecordFields;
        }

        public Guid  CurrentRemarkId
        {
            get
            {
                if (_currentRemarkId == Guid.Empty)
                {
                    Remark tempRemark;
                    if (!string.IsNullOrEmpty(JobDescription))
                    {
                         tempRemark = CurrentDbContext.Remarks.FirstOrDefault(rm => rm.Remark1 == JobDescription);
                        if (tempRemark == null)
                        {
                            //create new remark;
                            tempRemark = new Remark {ID = Guid.NewGuid(), Remark1 = JobDescription, IsCanned = false};
                            //add that in the db;
                            CurrentDbContext.Remarks.InsertOnSubmit(tempRemark);
                            CurrentDbContext.SubmitChanges();
                            _currentRemarkId = tempRemark.ID;
                        }
                        else
                            _currentRemarkId = tempRemark.ID;
                    }
                    else
                    {
                        _currentRemarkId = GetDefaultImportUtilRemarkId();
                    }
                }
                return _currentRemarkId;
            }
        }

        public Guid GetDefaultImportUtilRemarkId()
        {
            var defaultRemark = CurrentDbContext.Remarks.FirstOrDefault(rm => rm.Remark1 == DefaultImportUtilRemarkText);
            if (defaultRemark == null)
            {
                //create new remark;
                defaultRemark = new Remark
                                    {ID = Guid.NewGuid(), Remark1 = DefaultImportUtilRemarkText, IsCanned = false};

                //add that in the db;
                CurrentDbContext.Remarks.InsertOnSubmit(defaultRemark);
                CurrentDbContext.SubmitChanges();
            }
            return defaultRemark.ID;
        }

        public virtual List<string> ValidateInput()
        {
            throw new NotImplementedException();
        }

    }
    [Flags]
    public enum ImportOptions
    {
        None = 0,
        CreateMissingAttributes = 1,
        CreateMissingTaxonomies = 2,
        CreateMissingSkus = 4,
        CreateMissingValues = 8,
        CreateMissingMetaAttributes = 16,
        MarkAsBeforeEntity = 64,
        CreateMissingLOVs = 32
       // ReplaceExistingValues = 64
        
    }
    public class TaxonomyPathAndId
    {
        public string TaxonomyPath { get; set; }
        public Guid TaxonomyId { get; set; }
    }
   
}