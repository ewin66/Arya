using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using LinqKit;
using Arya.Framework.Collections.Generic;
using Arya.Framework.Common;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using EntityData = Arya.Framework.Data.AryaDb.EntityData;
using EntityInfo = Arya.Framework.Data.AryaDb.EntityInfo;
using ListOfValue = Arya.Framework.Data.AryaDb.ListOfValue;
using AryaDbDataContext = Arya.Framework.Data.AryaDb.AryaDbDataContext;
using SkuInfo = Arya.Framework.Data.AryaDb.SkuInfo;

namespace Arya.Framework.IO.Imports
{
    [ImportOrder(7)]
    public class ListOfValuesImportWorker : ImportWorkerBase
    {
        #region Private Fields
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();
        #endregion

        #region  Constructor
        public ListOfValuesImportWorker()
        {
            CurrentInterchangeRecordType = typeof(ListOfValuesInterchangeRecord);
        }
        #endregion

        #region Override Methods
        public override List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        public override void Run()
        {
            //State = WorkerState.Working;
            try
            {
                //initialize the context
                using (CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    var imageMgr = new ImageManager(CurrentDbContext, CurrentProjectId);

                    List<ListOfValuesInterchangeRecord> allData = ImportData.ListOfValues;
                    //read all the data into a list<T>, change this as its not very efficient and scalable.
                    var invalidRecords = allData.GetInvalidRecords();
                    var listOfValuesInterchangeRecords = invalidRecords as IList<ListOfValuesInterchangeRecord> ?? invalidRecords.ToList();
                    listOfValuesInterchangeRecords.ToList().ForEach(ir => _warnings.Add(new WorkerWarning
                    {
                        LineData = ir.ToString(),
                        ErrorMessage = Properties.Resources.RequiredValueNullWarningMessage
                    }));
                    var validImportRecords = allData.Except(listOfValuesInterchangeRecords.ToList()).ToList();
                    var newValueCount = 0;
                    int ignoredCount = 0;
                    int updatedCount = 0;                    
                    // load a dictionary with taxonomy string / id pairs (taken from TaxonomyImport)
                    var taxDict =
                        CurrentDbContext.ExecuteQuery<TaxonomyPathAndId>(@"SELECT TaxonomyPath, TaxonomyId
                                                FROM V_Taxonomy
                                                WHERE TaxonomyPath <> ''
                                                AND ProjectId = {0}", CurrentProjectId).ToDictionary(
                                                                             key => key.TaxonomyPath, value => value.TaxonomyId,
                                                                             StringComparer.OrdinalIgnoreCase);

                    // load attribute and schema dictionaries
                    var attDict =
                        CurrentDbContext.Attributes.Where(
                            a => a.AttributeType == AttributeTypeEnum.Sku.ToString())
                            .Select(a => a)
                            .ToDictionary(key => key.AttributeName, value => value);
                    var schDict = new DoubleKeyDictionary<Guid, Guid, SchemaInfo>();
                    CurrentDbContext.SchemaInfos.ForEach(si => schDict.Add(si.TaxonomyID, si.AttributeID, si));
                   
                    // iterate through the input records.
                    foreach (var csvRecord in validImportRecords)
                    {
                        var currentRecord = csvRecord;
                        // check for taxonomy - if it doesn't exist, give up.
                        if (!taxDict.ContainsKey(currentRecord.TaxonomyPath))
                        {
                            _warnings.Add(new WorkerWarning() { LineData = currentRecord.ToString(), ErrorMessage = Properties.Resources.TaxonomyDoesNotExistsWarningMessage });
                            continue;
                        }
                        var taxId = taxDict[currentRecord.TaxonomyPath];
                        var taxonomy = CurrentDbContext.TaxonomyInfos.First(ti => ti.ID == taxId);

                        // if attribute exists, get it, otherwise give up.
                        if (!attDict.ContainsKey(currentRecord.AttributeName))
                        {
                            _warnings.Add(new WorkerWarning() { LineData = currentRecord.ToString(), ErrorMessage = Properties.Resources.AttributeDoesNotExistWarningMessage });
                            continue;
                        }
                        var att = attDict[currentRecord.AttributeName];

                        // if schema info exists, get it, otherwise create both it and schema data
                        SchemaInfo sch;
                        if (schDict.ContainsKeys(taxId, att.ID))
                            sch = schDict[taxId, att.ID];
                        else
                        {
                            sch = new SchemaInfo(CurrentDbContext)
                            {
                                TaxonomyID = taxId,
                                Attribute = att,
                                SchemaDatas =
                                      {
                                          new SchemaData(CurrentDbContext)
                                              {InSchema = true, NavigationOrder = 0, DisplayOrder = 0}
                                      }
                            };
                            att.SchemaInfos.Add(sch);
                            schDict.Add(taxId, att.ID, sch);
                        }
                        var lov = sch.ListOfValues.FirstOrDefault(v => v.Value.ToLower() == currentRecord.Lov.ToLower() && v.Active);
                        string enrichmentImageGuid = null;
                        int displayOrder;
                        if (lov != null || (lov == null && CurrentImportOptions.HasFlag(ImportOptions.CreateMissingLOVs)))
                        {
                            // if image url exists try to upload the image - "try" is used in case the url is badly formed.
                            // string enrichmentImage = null;
                           
                            var success = false;
                            if (currentRecord.EnrichmentImage != null)
                            {
                                //Success is for valid uri. If the image name only exist not the real file it will be false and added as warning
                                success = imageMgr.UploadImage(currentRecord.EnrichmentImage);
                                enrichmentImageGuid = imageMgr.RemoteImageGuid;
                                var newSku = imageMgr.ImageSku;

                                newSku.EntityInfos.Add(new EntityInfo(CurrentDbContext)
                                {
                                    EntityDatas =
                                            {
                                                new EntityData(CurrentDbContext)
                                                    {
                                                        Attribute =
                                                            Attribute.GetAttributeFromName(CurrentDbContext,
                                                                                             Framework.Properties.Resources.LovIdAttributeName, true,AttributeTypeEnum.Sku,false),
                                                        Value = imageMgr.RemoteImageGuid
                                                    }
                                            }
                                });

                                newSku.EntityInfos.Add(new EntityInfo(CurrentDbContext)
                                {
                                    EntityDatas =
                                            {
                                                new EntityData(CurrentDbContext)
                                                    {
                                                        Attribute =
                                                            Attribute.GetAttributeFromName(CurrentDbContext,
                                                                                            Framework.Properties.Resources.AttributeIdAttributeName,
                                                                                            true,AttributeTypeEnum.Sku,false),
                                                        Value = sch.AttributeID.ToString()
                                                    }
                                            }
                                });

                                newSku.EntityInfos.Add(new EntityInfo(CurrentDbContext)
                                {
                                    EntityDatas =
                                            {
                                                new EntityData(CurrentDbContext)
                                                    {
                                                        Attribute =
                                                            Attribute.GetAttributeFromName(CurrentDbContext,Framework.Properties.Resources.TaxonomyIdAttributeName,
                                                                                            true,AttributeTypeEnum.Sku,false),
                                                        Value = sch.TaxonomyID.ToString()
                                                    }
                                            }
                                });

                                taxonomy.SkuInfos.Add(new SkuInfo(CurrentDbContext) { Sku = newSku });
                                if (!success)
                                {
                                    _warnings.Add(new WorkerWarning
                                    {
                                        LineData = currentRecord.ToString(),
                                        ErrorMessage = Properties.Resources.EnrichmentImageFileNotPresentWarningMessage
                                    });
                                }
                            }
                           
                            if (!string.IsNullOrEmpty(currentRecord.DisplayOrder) && !int.TryParse(currentRecord.DisplayOrder, out displayOrder))
                            {
                                _warnings.Add(new WorkerWarning() { LineData = currentRecord.ToString(), ErrorMessage = Properties.Resources.DisplayOrderNotValidNumberWarningMessage });
                                continue;
                            }
                        }
                        
                        
                        // if specific value record exists in this schema, get it, otherwise create it.
                        //var lov = sch.ListOfValues.FirstOrDefault(v => v.Value.ToLower() == currentRecord.Lov.ToLower() && v.Active);

                        if (lov == null)
                        {
                            if (CurrentImportOptions.HasFlag(ImportOptions.CreateMissingLOVs))
                            {
                                sch.ListOfValues.Add(entity: new ListOfValue(CurrentDbContext)
                                {
                                    Value = currentRecord.Lov,
                                    EnrichmentImage = enrichmentImageGuid,
                                    EnrichmentCopy = currentRecord.EnrichmentCopy,
                                    DisplayOrder = int.TryParse(currentRecord.DisplayOrder, out displayOrder) ? displayOrder : (int?)null,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = ImportRequestedBy

                                });
                                newValueCount++;
                            }
                            else 
                            {
                                _warnings.Add(new WorkerWarning
                                {
                                    LineData = currentRecord.ToString(),
                                    ErrorMessage = Properties.Resources.CreateLovFlagOffWarningMessage
                                });
                                ProcessSummaryReport(0);
                            }
                           
                        }
                        else
                        {
                            if (lov.Value == currentRecord.Lov && lov.EnrichmentCopy == currentRecord.EnrichmentCopy &&
                                lov.EnrichmentImage == currentRecord.EnrichmentImage && lov.DisplayOrder.ToString() == currentRecord.DisplayOrder)
                            {
                                ignoredCount++;
                                continue;
                            }
                            var updatedFlag = false;
                            if (!string.IsNullOrEmpty(enrichmentImageGuid))
                            {
                                lov.EnrichmentImage = enrichmentImageGuid;
                                updatedFlag = true;
                            }
                            if (!string.IsNullOrEmpty(currentRecord.EnrichmentCopy))
                            {
                                lov.EnrichmentCopy = currentRecord.EnrichmentCopy;
                                updatedFlag = true;
                            }
                            if (!string.IsNullOrEmpty(currentRecord.DisplayOrder))
                            {
                                lov.DisplayOrder = int.TryParse(currentRecord.DisplayOrder, out displayOrder)
                                                       ? displayOrder
                                                       : (int?) null;
                                updatedFlag = true;
                            }
                            if (updatedFlag)
                            {
                                lov.CreatedBy = ImportRequestedBy;
                                updatedCount++;
                                lov.CreatedOn = DateTime.Now;
                            }
                            else
                            {
                                ignoredCount++;
                            }
                            
                        }
                    }
                    SaveDataChanges();
                    ProcessSummaryReport(newValueCount, updatedCount, ignoredCount);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                var newException = new Exception(Properties.Resources.InvalidRowInInputFileMessage, ex);
                Summary.SetError(newException);
            }
            catch (Exception ex)
            {
                Summary.SetError(ex);
            }
        }
        #endregion

        #region Private Methods
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