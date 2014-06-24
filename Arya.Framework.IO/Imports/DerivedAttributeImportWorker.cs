using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using AryaDbDataContext = Arya.Framework.Data.AryaDb.AryaDbDataContext;

namespace Arya.Framework.IO.Imports
{
    [ImportOrder(4)]
    public class DerivedAttributeImportWorker : ImportWorkerBase
    {
        #region Private Fields
        private readonly WorkerError _derivedAttributeImportWorkerError = new WorkerError();
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();
        #endregion

        #region Constructor
        public DerivedAttributeImportWorker()
            
        {
            CurrentInterchangeRecordType = typeof(DerivedAttributeInterchangeRecord);
        }
        #endregion

        #region Override Methods
        //TODO: Add try catch
        public override void Run()
        {
            try
            {
                using (CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    List<DerivedAttributeInterchangeRecord> allData = ImportData.DerivedAttributes;
                    //read all the data into a list<T>, change this as its not very efficient and scalable.
                  
                    //CsvConfiguration conf = GetCurrentConfiguration();
                    ////char delimiterChar = (char)FieldDelimiter.GetDisplayTextAndDbValue().DbValue;
                    //using (var csvReader = new CsvReader(File.OpenText(InputFilePath), conf))
                    //{
                    //    allData = csvReader.GetRecordsWithNulls<DerivedAttributeInterchangeRecord>().Distinct(new DerivedAttributeInterchangeRecordComparer()).ToList();
                    //}
                    var invalidRecords = allData.GetInvalidRecords();
                    var derivedAttributeInterchangeRecords = invalidRecords as IList<DerivedAttributeInterchangeRecord> ?? invalidRecords.ToList();
                    derivedAttributeInterchangeRecords.ToList().ForEach(ir => _warnings.Add(new WorkerWarning
                    {
                        LineData = ir.ToString(),
                        ErrorMessage = Properties.Resources.RequiredValueNullWarningMessage
                    }));
                    var validImportRecords = allData.Except(derivedAttributeInterchangeRecords.ToList()).ToList();
                    int insertCount = 0;
                    int updateCount = 0;
                    int ignoreCount = 0;                    
                    // load a dictionary with taxonomy string / id pairs (taken from TaxonomyImport)
                    var taxDict =
                        CurrentDbContext.ExecuteQuery<TaxonomyPathAndId>(
                            @"SELECT TaxonomyPath, TaxonomyID
                                                FROM V_Taxonomy
                                                WHERE TaxonomyPath <> ''
                                                AND ProjectId = {0}", CurrentProjectId).
                            ToDictionary(key => key.TaxonomyPath, value => value.TaxonomyId, StringComparer.OrdinalIgnoreCase);

                    // iterate through the input records.
                    foreach (var csvRecord in validImportRecords)
                    {
                        var currentRecord = csvRecord;
                        // check for AttributeName - pull Id
                        var attIds = from a in CurrentDbContext.Attributes
                                     where a.AttributeName == currentRecord.DerivedAttributeName && a.AttributeType == AttributeTypeEnum.Derived.ToString()  // AttributeTypeEnum.Derived.ToString()
                                     select a.ID;
                        if (!attIds.Any())
                        {
                            _warnings.Add(new WorkerWarning() { LineData = currentRecord.ToString(), ErrorMessage = Properties.Resources.AttributeDoesNotExistWarningMessage });
                            continue;
                        }

                        if (taxDict.ContainsKey(currentRecord.TaxonomyPath))
                        {
                            Guid taxId = taxDict[currentRecord.TaxonomyPath];
                            var dAtt = from d in CurrentDbContext.DerivedAttributes
                                       where d.TaxonomyID == taxId && d.AttributeID == attIds.First()
                                       select d;

                            // if derived attribute exists, update it, otherwise insert a new one.
                            if (dAtt.Any())
                            {
                                var updatedAttribute = dAtt.First();
                                if (SameValue(updatedAttribute, currentRecord))
                                {
                                    ignoreCount++;
                                    continue;
                                }
                                updatedAttribute.Expression = csvRecord.DerivedAttributeExpression.Trim();
                                updatedAttribute.MaxResultLength = csvRecord.MaxResultLength;

                                updateCount++;
                            }
                            else
                            {
                                var newAttribute = new DerivedAttribute
                                {
                                    ID = Guid.NewGuid(),
                                    TaxonomyID = taxId,
                                    AttributeID = attIds.First(),
                                    Expression = csvRecord.DerivedAttributeExpression,
                                    MaxResultLength = csvRecord.MaxResultLength
                                };

                                CurrentDbContext.DerivedAttributes.InsertOnSubmit(newAttribute);
                                insertCount++;
                            }
                        }
                        else//its a global derived attribute record
                        {
                            var existingGlobalDerivedAttribute =
                                CurrentDbContext.DerivedAttributes.FirstOrDefault(da => da.AttributeID == attIds.First() && da.TaxonomyID == null);
                            // if derived attribute exists, update it, otherwise insert a new one.
                            if (existingGlobalDerivedAttribute != null)
                            {
                                // var updatedAttribute = existingGlobalDerivedAttribute.First();
                                if (SameValue(existingGlobalDerivedAttribute, currentRecord))
                                {
                                    ignoreCount++;
                                    continue;
                                }
                                existingGlobalDerivedAttribute.Expression = currentRecord.DerivedAttributeExpression.Trim();
                                existingGlobalDerivedAttribute.MaxResultLength = currentRecord.MaxResultLength;

                                updateCount++;
                            }
                            else
                            {
                                var newAttribute = new DerivedAttribute
                                {
                                    ID = Guid.NewGuid(),
                                    TaxonomyID = null,
                                    AttributeID = attIds.First(),
                                    Expression = csvRecord.DerivedAttributeExpression,
                                    MaxResultLength = csvRecord.MaxResultLength
                                };

                                CurrentDbContext.DerivedAttributes.InsertOnSubmit(newAttribute);
                                insertCount++;
                            }
                        }
                    }
                    SaveDataChanges();
                    ProcessSummaryReport(insertCount, updateCount, ignoreCount);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                var newException = new Exception(Properties.Resources.InvalidRowInInputFileMessage, ex);
                Summary.SetError(newException);
            }
            catch (Exception ex)
            {
                Summary.SetError(ex);//.SummaryError = _derivedAttributeImportWorkerError;
            }
        }
        public virtual bool IsInputValid()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Variables

        private void RecurseInnerException(Exception exception)
        {
            _derivedAttributeImportWorkerError.Messages.Add(exception.Message);
            if (exception.InnerException != null)
                RecurseInnerException(exception.InnerException);
        }
        private bool SameValue(DerivedAttribute existingGlobalDerivedAttribute, DerivedAttributeInterchangeRecord currentRecord)
        {
            return String.Compare(existingGlobalDerivedAttribute.Expression,currentRecord.DerivedAttributeExpression)  == 0 && 
                existingGlobalDerivedAttribute.MaxResultLength== currentRecord.MaxResultLength;
        }

        public override List<string> ValidateInput()
        {
            throw new NotImplementedException();
        }

        private void ProcessSummaryReport(int insertCount = 0,
                                         int updateCount = 0, int ignoreCount = 0)
        {
           
            Summary.Details = new List<CustomKeyValuePair<string, int>>
                                             {
                                                 new CustomKeyValuePair<string, int>(Properties.Resources.NewRecordCountIdentifierText,
                                                                                     insertCount),
                                                 new CustomKeyValuePair<string, int>(Properties.Resources.IgnoredRecordCountIdentifierText,
                                                                                     ignoreCount),
                                                 new CustomKeyValuePair<string, int>(Properties.Resources.UpdatedRecordCountIdentifierText,
                                                                                     updateCount)
                                             };
            if (_warnings.Count != 0)
            {
                Summary.Warnings = _warnings;
                Summary.State = WorkerState.CompletedWithWarning;
                return;
            }
            Summary.State = WorkerState.Complete;
        }
        #endregion
    }
}
