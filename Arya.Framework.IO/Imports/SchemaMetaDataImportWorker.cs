using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using EntityData = Arya.Framework.Data.AryaDb.EntityData;
using EntityInfo = Arya.Framework.Data.AryaDb.EntityInfo;
using AryaDbDataContext = Arya.Framework.Data.AryaDb.AryaDbDataContext;
using Arya.Framework.Properties;

namespace Arya.Framework.IO.Imports
{
    //TODO: Refactor process result methods,Seperate tax, schema info attri missiing in the warning inside sql  add regions how the sqk return results
      [ImportOrder(6)]
    public class SchemaMetaDataImportWorker : ImportWorkerBase
    {
        #region Constants

        private const string TempTablePrefix = "scmd:";
        

        #endregion Constants

        #region Private variables

        //TODO: Might not need this as priviate variable
        private readonly WorkerError _schemaMetaDataImportWorkerError = new WorkerError();
        
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();
        private ImageManager _currentImageManager;

        #endregion Private variables

        #region Constructors

        public SchemaMetaDataImportWorker()
            
        {
            CurrentInterchangeRecordType = typeof (SchemaMetaDataInterchangeRecord);
        }

        #endregion Constructors

        #region Override Methods

        public override List<string> ValidateInput() { throw new NotImplementedException(); }
          public virtual bool IsInputValid() { throw new NotImplementedException(); }

        public override void Run()
        {
            //State = WorkerState.Working;
            try
            {
                using (
                    CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    _currentImageManager = new ImageManager(CurrentDbContext, CurrentProjectId);
                    List<SchemaMetaDataInterchangeRecord> allData;
                    allData = ImportData.SchemaMetaDatas;
                    //read all the values into a list<T>, change this as its not very efficient and scalable.
                    //CsvConfiguration conf = GetCurrentConfiguration();
                    ////char delimiterChar = (char)FieldDelimiter.GetDisplayTextAndDbValue().DbValue;
                    //using (var csvReader = new CsvReader(File.OpenText(InputFilePath), conf))
                    //{
                    //    allData =
                    //       csvReader.GetRecordsWithNulls<SchemaMetaDataInterchangeRecord>().ToList();
                    //}
                    var invalidRecords = allData.GetInvalidRecords();
                    var schemaMetaDataInterchangeRecords = invalidRecords as IList<SchemaMetaDataInterchangeRecord> ?? invalidRecords.ToList();
                    schemaMetaDataInterchangeRecords.ToList().ForEach(ir => _warnings.Add(new WorkerWarning
                    {
                        LineData = ir.ToString(),
                        ErrorMessage = Properties.Resources.RequiredValueNullWarningMessage
                    }));
                    var validImportRecords = allData.Except(schemaMetaDataInterchangeRecords.ToList()).ToList();
                    validImportRecords =
                        validImportRecords.Distinct(new SchemaMetaDataInterchangeRecordComparer()).ToList();
                    // Take the enrichment images out of the list
                    var enrichmentImageRecords =
                        validImportRecords.Where(ad => ad.SchemaMetaAttributeName.ToLower() == Resources.SchemaEnrichmentImageAttributeName.ToLower()).ToList();
                    ProcessEnrichmentImage(enrichmentImageRecords);
                    validImportRecords = validImportRecords.Where(ad => !enrichmentImageRecords.Contains(ad)).ToList();
                    //adding list with updated enrichment guid to the main list
                    validImportRecords.AddRange(enrichmentImageRecords);

                    var sqlTempTableHelper = new SqlHelper(CurrentInterchangeRecordType);

                    var tempTableName = TempTablePrefix + Guid.NewGuid();
                    var warningTableName = tempTableName + "_warning";
                    var createTempTableScript = sqlTempTableHelper.CreateTableScript(tempTableName, "tempdb");
                    var deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempTableName, "tempdb");
                    //create the temp table.
                    CurrentDbContext.ExecuteCommand(createTempTableScript);

                    //bulk insert data into tempdb
                    CurrentDbContext.BulkInsertAll(validImportRecords, tempTableName, "tempdb");

                    var schemaAttributeMetaQuery = @"DECLARE @UserID AS UNIQUEIDENTIFIER
                                            DECLARE @ProjectID AS UNIQUEIDENTIFIER
                                            DECLARE @ResultText AS VARCHAR(2000)
                                            DECLARE @DefaultImportUtilRemarkID AS UNIQUEIDENTIFIER
                                            DECLARE @UpdateCount AS int
                                            SET @DefaultImportUtilRemarkID = '" + CurrentRemarkId + @"'
                                            IF OBJECT_ID('tempdb..#SchemaMetaAttributeAllData') IS NOT NULL 
                                            DROP TABLE #SchemaMetaAttributeAllData
                                            CREATE TABLE #SchemaMetaAttributeAllData
                                            (
                                                TaxonomyPath varchar(4000), 
                                                AttributeName varchar(255),
	                                            MetaAttributeName varchar(255),
	                                            MetaValue varchar(4000),
	                                            TaxonomyId varchar(255),
	                                            AttrributeId varchar(255),
	                                            MetaAttributeId varchar(255),
	                                            SchemaInfoId varchar(255),
	                                            DbValue varchar(4000),
	                                            SchemaMetaInfoId varchar(255),
	                                            SchemaMetaDataId varchar(255)
                                            ); 
                                            
                                            IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL 
                                            DROP TABLE [tempdb]..[" + warningTableName + @"]

                                            SET @UserID = '"
                                                   + ImportRequestedBy + @"'
                                            SET @ProjectID = '"
                                                   + CurrentProjectId + @"'
                                            SET @ResultText = ''

                                            INSERT INTO #SchemaMetaAttributeAllData(TaxonomyPath,AttributeName,MetaAttributeName,MetaValue,TaxonomyId,AttrributeId,MetaAttributeId,SchemaInfoId)
                                            SELECT td.TaxonomyPath, td.AttributeName, td.MetaAttributeName, td.MetaAttributeValue, tx.TaxonomyId, att.ID AS AttrributeId, metaAtt.ID AS MetaAttributeId,si.ID AS SchemaInfoId 
                                            FROM 
                                            [tempdb]..["+ tempTableName + @"] td LEFT OUTER JOIN V_Taxonomy tx
                                            ON td.TaxonomyPath = tx.TaxonomyPath
                                            LEFT OUTER JOIN Attribute att
                                            ON att.AttributeName = td.AttributeName and (att.AttributeType = 'Sku' OR att.AttributeType = 'Global' OR att.AttributeType = 'Derived')
                                            LEFT OUTER JOIN SchemaInfo si
                                            ON si.TaxonomyID = tx.TaxonomyId AND si.AttributeID =  att.ID 
                                            LEFT OUTER JOIN Attribute metaAtt
                                            ON metaAtt.AttributeName = td.MetaAttributeName and metaAtt.AttributeType = 'SchemaMeta'
 

                                           -- Select * from #SchemaMetaAttributeAllData

                                            SELECT sad.TaxonomyPath,  sad.AttributeName, sad.MetaAttributeName, sad.MetaValue, 'TaxonomyPath/AttributeName/MetaAttributeName/SchemaInfo Does Not Exist' AS WarningMessage
                                            INTO  [tempdb]..[" + warningTableName + @"]
                                            FROM #SchemaMetaAttributeAllData sad
                                            WHERE sad.AttrributeId IS NULL OR sad.TaxonomyId IS NULL OR sad.MetaAttributeId IS NULL OR sad.SchemaInfoId IS NULL

                                            DELETE sad 
                                            FROM #SchemaMetaAttributeAllData  sad
                                            INNER JOIN [tempdb]..[" + warningTableName + @"] wn
                                            ON sad.TaxonomyPath = wn.TaxonomyPath AND sad.AttributeName = wn.AttributeName AND sad.MetaAttributeName = wn.MetaAttributeName


                                            UPDATE #SchemaMetaAttributeAllData
                                            SET DbValue = vsmd.SchemaMetaValue, SchemaMetaInfoId = vsmd.SchemaMetaID, SchemaMetaDataId = vsmd.SchemaMetaDataID
                                            FROM #SchemaMetaAttributeAllData sad INNER JOIN V_SchemaMetaData vsmd 
                                            ON vsmd.SchemaID = sad.SchemaInfoId AND vsmd.SchemaMetaAttributeID = sad.MetaAttributeId

                                            --SELECT * FROM #SchemaMetaAttributeAllData

                                            DELETE sad FROM  #SchemaMetaAttributeAllData sad
                                            WHERE sad.MetaValue = sad.DbValue

                                            SET @ResultText += '"
                                                   + Properties.Resources.IgnoredRecordCountIdentifierText
                                                   + @"' + '='  + CAST(@@ROWCOUNT  AS VARCHAR(500)) + ';'
                                           -- SELECT * FROM #SchemaMetaAttributeAllData


                                            UPDATE SchemaMetaData
                                            SET Active = 0, DeletedBy  =@UserID, DeletedOn =  GETDATE(), DeletedRemark = @DefaultImportUtilRemarkID
                                            FROM SchemaMetaData smd  INNER JOIN #SchemaMetaAttributeAllData sad ON sad.SchemaMetaDataId = smd.ID
                                            WHERE smd.Active = 1

                                            SET @UpdateCount = @@ROWCOUNT

                                            UPDATE #SchemaMetaAttributeAllData
                                            SET SchemaMetaInfoId = smi.ID
                                            FROM #SchemaMetaAttributeAllData sad INNER JOIN SchemaMetaInfo smi 
                                            ON sad.SchemaInfoId = smi.SchemaID and sad.MetaAttributeId = smi.MetaAttributeID

                                            INSERT INTO SchemaMetaInfo(ID, SchemaID, MetaAttributeID)
                                            SELECT NEWID(), sad.SchemaInfoId, sad.MetaAttributeId
                                            FROM #SchemaMetaAttributeAllData sad WHERE sad.SchemaMetaInfoId IS NULL

                                            UPDATE #SchemaMetaAttributeAllData
                                            SET SchemaMetaInfoId = smi.ID
                                            FROM #SchemaMetaAttributeAllData sad INNER JOIN SchemaMetaInfo smi 
                                            ON sad.SchemaInfoId = smi.SchemaID and sad.MetaAttributeId = smi.MetaAttributeID

                                            INSERT INTO SchemaMetaData(ID,MetaID, Value,CreatedOn,CreatedBy,CreatedRemark,Active)
                                            SELECT NEWID(), sad.SchemaMetaInfoId, sad.MetaValue, GETDATE(), @UserID,@DefaultImportUtilRemarkID,1
                                            FROM #SchemaMetaAttributeAllData sad 

                                            SET @ResultText += '"
                                                   + Properties.Resources.NewRecordCountIdentifierText
                                                   +
                                                   @"'+ '='  + CAST((@@ROWCOUNT - @UpdateCount)  AS VARCHAR(500)) + ';';
                                            SET @ResultText += '"
                                                   + Properties.Resources.UpdatedRecordCountIdentifierText
                                                   + @"'+ '=' + CAST(@UpdateCount  AS VARCHAR(500)) ;

                                            SELECT @ResultText";
                    var queryResults = CurrentDbContext.ExecuteQuery<string>(schemaAttributeMetaQuery).Single();

                    var warningRecords =
                        CurrentDbContext.ExecuteQuery<string>(
                            @"SELECT  war.TaxonomyPath + char(9) + war.AttributeName + char(9) + 
                                                                                 war.MetaAttributeName + char(9) + war.MetaValue + char(9) + 
                                                                                 war.WarningMessage

                                                                            FROM [tempdb]..[" + warningTableName + @"] war                                                                                                                 
                                                                            ").ToList();

                    CurrentDbContext.ExecuteCommand(@"IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL 
                                                        DROP TABLE [tempdb]..[" + warningTableName + @"]");
                    foreach (var warningRecord in warningRecords)
                    {
                        _warnings.Add(new WorkerWarning
                                          {
                                              LineData = warningRecord.Substring(0, warningRecord.LastIndexOf('\t')),
                                              ErrorMessage = warningRecord.Substring(warningRecord.LastIndexOf('\t') + 1)
                                          });
                    }

                    ProcessSummaryReport(queryResults);
                    CurrentDbContext.ExecuteCommand(deleteTempTableScript);
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

        #endregion Override Methods

        #region Private Methods

        private void ProcessSummaryReport(string queryResults)
        {
            var successCount = 0;
            int itemCount;
            var reportItems = queryResults.Split(';');
            //Summary summaryReport = new Summary();
            var summeryReportDetails = new List<CustomKeyValuePair<string, int>>();
            foreach (var reportItem in reportItems)
            {
                itemCount = Int32.Parse(reportItem.Substring(reportItem.IndexOf('=') + 1).Trim());
                if (reportItem.Contains(Properties.Resources.NewRecordCountIdentifierText))
                    summeryReportDetails.Add(new CustomKeyValuePair<string, int>(Properties.Resources.NewRecordCountIdentifierText, itemCount));
                else if (reportItem.Contains(Properties.Resources.UpdatedRecordCountIdentifierText))
                    summeryReportDetails.Add(new CustomKeyValuePair<string, int>(Properties.Resources.UpdatedRecordCountIdentifierText,
                                                                                 itemCount));
                else if (reportItem.Contains(Properties.Resources.IgnoredRecordCountIdentifierText))
                    summeryReportDetails.Add(new CustomKeyValuePair<string, int>(Properties.Resources.IgnoredRecordCountIdentifierText,
                                                                                 itemCount));
                successCount += itemCount;
            }
            Summary.Details = summeryReportDetails;
            if (_warnings.Count != 0)
            {
                Summary.Warnings = _warnings;
                Summary.State = WorkerState.CompletedWithWarning;
                return;
            }
            Summary.State = WorkerState.Complete;
        }

        private void ProcessEnrichmentImage(List<SchemaMetaDataInterchangeRecord> enrichmentImageRecords)
        {
            bool success;
            // build taxonomy dictionary
            var taxDict =
                CurrentDbContext.ExecuteQuery<TaxonomyPathAndId>(@"SELECT TaxonomyPath, TaxonomyId 
                                                FROM V_Taxonomy
                                                WHERE TaxonomyPath <> ''
                                                AND ProjectId = {0}",
                                                                 CurrentProjectId).ToDictionary(
                                                                     key => key.TaxonomyPath, value => value.TaxonomyId,
                                                                     StringComparer.OrdinalIgnoreCase);

           // var schemaMetaAttributes = CurrentDbContext.Attributes.Where(att => att.AttributeType.Contains("SchemaMeta")).Select(att=>att.AttributeName).ToList();
            var attDict = CurrentDbContext.Attributes.Where(at => Attribute.NonMetaAttributeTypes.Contains(at.AttributeType)).ToDictionary(key => key.AttributeName, value => value);
              

            
            foreach (var enrichmentImageRecord in enrichmentImageRecords)
            {
                
                //var isUri = Uri.IsWellFormedUriString(enrichmentImageRecord.SchemaMetaAttributeValue, UriKind.Absolute);
                //var imageUri = isUri
                //                   ? new Uri(enrichmentImageRecord.SchemaMetaAttributeValue)
                //                   : new Uri(Path.Combine(BaseImagePath,CurrentProjectId.ToString(),enrichmentImageRecord.SchemaMetaAttributeValue));
                //Success is for valid uri. If the image name only exist not the real file it will be false and added as warning
                success = _currentImageManager.UploadImage(enrichmentImageRecord.SchemaMetaAttributeValue);
                var enrichmentImageGuid = _currentImageManager.RemoteImageGuid;
                if (!taxDict.ContainsKey(enrichmentImageRecord.TaxonomyPath))
                {
                    _warnings.Add(new WorkerWarning() { LineData = enrichmentImageRecord.ToString(), ErrorMessage = Properties.Resources.TaxonomyDoesNotExistsWarningMessage });
                    continue;
                }
                var taxonomy =
                    CurrentDbContext.TaxonomyInfos.FirstOrDefault(t => t.ID == taxDict[enrichmentImageRecord.TaxonomyPath]);
                if (taxonomy != null)
                {
                    if (!attDict.ContainsKey(enrichmentImageRecord.AttributeName))
                    {
                        _warnings.Add(new WorkerWarning() { LineData = enrichmentImageRecord.ToString(), ErrorMessage = Properties.Resources.AttributeDoesNotExistWarningMessage });
                        continue;
                    }
                    var attribute = attDict[enrichmentImageRecord.AttributeName];

                    // get sku for the image
                    var newSku = _currentImageManager.ImageSku;

                    newSku.EntityInfos.Add(new EntityInfo(CurrentDbContext)
                    {
                        EntityDatas =
                                    {
                                        new EntityData(CurrentDbContext)
                                            {
                                                Attribute =
                                                    Attribute.GetAttributeFromName(CurrentDbContext,Resources.AttributeIdAttributeName,
                                                                                    true,AttributeTypeEnum.Sku,false),
                                                Value = attribute.ID.ToString()
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
                                                                                                  Framework.Properties.Resources.
                                                                                                      TaxonomyIdAttributeName,
                                                                                                  true,AttributeTypeEnum.Sku,false),
                                                               Value = taxonomy.ID.ToString()
                                                           }
                                                   }
                    });
                    taxonomy.SkuInfos.Add(new SkuInfo(CurrentDbContext) { Sku = newSku });
                    enrichmentImageRecord.SchemaMetaAttributeValue = enrichmentImageGuid;
                    if (!success)
                    {
                        _warnings.Add(new WorkerWarning
                        {
                            LineData = enrichmentImageRecord.ToString(),
                            ErrorMessage = Properties.Resources.EnrichmentImageFileNotPresentWarningMessage
                        });
                    }
                 }//end of if
                else
                {
                    _warnings.Add(new WorkerWarning(){LineData = enrichmentImageRecord.ToString(), ErrorMessage = Properties.Resources.TaxonomyDoesNotExistsWarningMessage});
                }
              }//end of for
               
            SaveDataChanges();
        }

        #endregion
    }
}