using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.IO.Properties;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Imports
{
    [ImportOrder(3)]
    public class AttributeImportWorker : ImportWorkerBase
    {
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();

        public AttributeImportWorker() { CurrentInterchangeRecordType = typeof (AttributeInterchangeRecord); }

        public override void Run()
        {
            try
            {
                if (!CurrentImportOptions.HasFlag(ImportOptions.CreateMissingAttributes))
                {
                    State = WorkerState.Complete;
                    StatusMessage = string.Format("Skipped as per ImportOptions");
                    //TODO: Create new summary report
                    _warnings.Add(new WorkerWarning
                                  {
                                      LineData ="",
                                      ErrorMessage =
                                          Resources.CreateMissingAttributesFlagOffWarningMessage
                                  });
                    ProcessSummaryReport();
                    return;
                }
                IEnumerable<AttributeInterchangeRecord> allData = ImportData.Attributes;

                var attributeInterchangeRecords = allData as AttributeInterchangeRecord[] ?? allData.ToArray();
                var totalRecordCount = attributeInterchangeRecords.Count();
                CurrentLogWriter.DebugFormat("{0}: Total Records: {1}", Arguments.Id, totalRecordCount);

                var invalidRecords = attributeInterchangeRecords.GetInvalidRecords();
                var invalidAttributeInterchangeRecords = invalidRecords as IList<AttributeInterchangeRecord>
                                                         ?? invalidRecords.ToList();
                CurrentLogWriter.DebugFormat("{0}: Invalid Records: {1}", Arguments.Id,
                    invalidAttributeInterchangeRecords.Count);
                invalidAttributeInterchangeRecords.ToList()
                    .ForEach(
                        ir =>
                            _warnings.Add(new WorkerWarning
                                          {
                                              LineData = ir.ToString(),
                                              ErrorMessage = Resources.RequiredValueNullWarningMessage
                                          }));
                var validImportRecords =
                    attributeInterchangeRecords.Except(invalidAttributeInterchangeRecords.ToList()).ToList();
                validImportRecords =validImportRecords.Distinct(new AttributeInterchangeRecordComparer()).ToList();// as List<AttributeInterchangeRecord>;
                CurrentLogWriter.DebugFormat("{0}: Valid Records: {1}", Arguments.Id, validImportRecords.Count);
                //Object level operation Start

                //TODOs
                //Report all the not found attributes types
                using (CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    CurrentLogWriter.DebugFormat("{0}: Reporting Warnings", Arguments.Id);
                    var existingNonMetaAttributes =
                        CurrentDbContext.Attributes.Where(
                            at => Attribute.NonMetaAttributeTypes.Contains(at.AttributeType)).ToList();
                    ReportAllWarinings(existingNonMetaAttributes, validImportRecords);

                    //create new attribute 
                    CurrentLogWriter.DebugFormat("{0}: Get New Attributes", Arguments.Id);
                    var newAttributes = GetNewAttributes(existingNonMetaAttributes, validImportRecords).ToList();
                    foreach (var attributeInterchangeRecord in newAttributes)
                        CreateNewAttribute(attributeInterchangeRecord);
                    CurrentLogWriter.DebugFormat("{0}: Saving Changes", Arguments.Id);
                    SaveDataChanges();
                    ProcessSummaryReport(newAttributes.Count());
                }
                //Object level operation End
            }
            catch (IndexOutOfRangeException ex)
            {
                var newException = new Exception(Resources.InvalidRowInInputFileMessage, ex);
                Summary.SetError(newException);
            }
            catch (Exception ex)
            {
                Summary.SetError(ex);
            }
        }

        public override List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        //TODO: Refactor this so that it does not apear in every worker
        private void ProcessSummaryReport(int newAttributeCount = 0)
        {
            Summary.Details = new List<CustomKeyValuePair<string, int>>
                              {
                                  new CustomKeyValuePair<string, int>(
                                      Resources.NewRecordCountIdentifierText,
                                      newAttributeCount),
                                  new CustomKeyValuePair<string, int>(
                                      Resources
                                      .IgnoredRecordCountIdentifierText,
                                      _warnings.Count),
                              };

            if (_warnings.Count != 0)
            {
                Summary.Warnings = _warnings;
                Summary.State = WorkerState.CompletedWithWarning;
                return;
            }
            Summary.State = WorkerState.Complete;
        }

        private void ReportAllWarinings(IEnumerable<Attribute> existingNonMetaAttributes,
            List<AttributeInterchangeRecord> validImportRecords)
        {
            ReportInvalidAttributeTypes(validImportRecords);
            var sameAttributeNames = (from er in existingNonMetaAttributes
                join ad in validImportRecords on er.AttributeName.Trim().ToLower() equals
                    ad.AttributeName.Trim().ToLower()
                select
                    new
                    {
                        er.AttributeName,
                        TypeInDb = er.Type.ToString(),
                        TypeInFile = ad.AttributeType,
                        ad.IsDefaultAttributeType
                    }).ToList();

            //Reporting same attribute name but different types
            var sameAttributeButDiffType =
                sameAttributeNames.Where(
                    sa =>
                        (sa.IsDefaultAttributeType && !Attribute.NonMetaAttributeTypes.Contains(sa.TypeInFile))
                        || (!sa.IsDefaultAttributeType && !string.Equals(sa.TypeInFile, sa.TypeInDb))).ToList();

            foreach (var attDiffTypeRecord in sameAttributeButDiffType)
            {
                _warnings.Add(new WorkerWarning
                              {
                                  LineData =
                                      attDiffTypeRecord.AttributeName + '\t'
                                      + attDiffTypeRecord.TypeInFile,
                                  ErrorMessage =
                                      Resources.AttributeNameSameTypeDifferentWarningMessage
                              });
                validImportRecords.Remove(
                    validImportRecords.FirstOrDefault(
                        ad =>
                            ad.AttributeName == attDiffTypeRecord.AttributeName
                            && ad.AttributeType == attDiffTypeRecord.TypeInFile));
            }

            //report same attribute name and type in the file
            var sameAttributeInfoInFile =
                sameAttributeNames.Where(sa => string.Equals(sa.TypeInFile, sa.TypeInDb)).ToList();
            foreach (var sameAttr in sameAttributeInfoInFile)
            {
                _warnings.Add(new WorkerWarning
                              {
                                  LineData = sameAttr.AttributeName + '\t' + sameAttr.TypeInFile,
                                  ErrorMessage = Resources.SimilarAttributeWarningMessage
                              });
                validImportRecords.Remove(
                    validImportRecords.FirstOrDefault(
                        ad => ad.AttributeName == sameAttr.AttributeName && ad.AttributeType == sameAttr.TypeInFile));
            }
            // validImportRecords.Where(vr=> !sameAttributeInfoInFile.Contains());
        }

        private void CreateNewAttribute(AttributeInterchangeRecord attributeInterchangeRecord)
        {
            AttributeTypeEnum type;
            Enum.TryParse(attributeInterchangeRecord.AttributeType, out type);

            var newAttribute = Attribute.GetAttributeFromName(CurrentDbContext, attributeInterchangeRecord.AttributeName,
                true, type, false);
            //CurrentDbContext.Attributes.InsertOnSubmit(newAttribute);
        }

        private static IEnumerable<AttributeInterchangeRecord> GetNewAttributes(
            IEnumerable<Attribute> existingNonAttributes, IEnumerable<AttributeInterchangeRecord> validImportRecords)
        {
            var newAttributes = from record in validImportRecords
                join attibute in existingNonAttributes on record.AttributeName.ToLower() equals
                    attibute.AttributeName.ToLower() into tempItems
                from item in tempItems.DefaultIfEmpty()
                select new {record.AttributeName, record.AttributeType, Id = (item == null ? Guid.Empty : item.ID)};

            return
                newAttributes.Where(na => na.Id == Guid.Empty)
                    .Select(
                        item =>
                            new AttributeInterchangeRecord
                            {
                                AttributeType = item.AttributeType,
                                AttributeName = item.AttributeName
                            });
        }

        private void ReportInvalidAttributeTypes(List<AttributeInterchangeRecord> validImportRecords)
        {
            for (var i = 0; i < validImportRecords.Count; i++)
            {
                var record = validImportRecords[i];
                if (record.AttributeType != null && !Enum.IsDefined(typeof (AttributeTypeEnum), record.AttributeType))
                {
                    _warnings.Add(new WorkerWarning
                                  {
                                      LineData = record.ToString(),
                                      ErrorMessage = Resources.InvalidAttributeTypeWarningMessage
                                  });
                    validImportRecords.Remove(record);
                }
            }
        }
    }
}