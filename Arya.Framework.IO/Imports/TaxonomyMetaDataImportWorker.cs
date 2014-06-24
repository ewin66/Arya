using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.Properties;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using EntityInfo = Arya.Framework.Data.AryaDb.EntityInfo;
using SkuInfo = Arya.Framework.Data.AryaDb.SkuInfo;

namespace Arya.Framework.IO.Imports
{
    //TODO: Rewrite the methods to take icollection instead of list
    /// <summary>
    /// This class assume that there will not be any duplicates in the taxonomy path in the given file 
    /// </summary>
    [ImportOrder(2)]
    public class TaxonomyMetaDataImportWorker : ImportWorkerBase
    {
        #region Constants

        private const string TempTablePrefix = "taxmd:";

        #endregion

        #region Private variables

        private readonly List<string> _noImageFileTaxEnrichementImage = new List<string>();
        
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();
        private ImageManager _currentImageManager;

        #endregion

        #region Constructor

        public TaxonomyMetaDataImportWorker()
        {
            CurrentInterchangeRecordType = typeof (TaxonomyMetaDataInterchangeRecord);
        }

        #endregion

        #region Override Methods

        public override List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        public override void Run()
        {
            //State = WorkerState.Working;
            List<string> ignoredTaxonomyPaths = new List<string>();
            Dictionary<string, TaxonomyPathAndId> existingTaxonomyPathId;
            List<TaxonomyMetaDataInterchangeRecord> allData =  new List<TaxonomyMetaDataInterchangeRecord>();
            List<TaxonomyMetaDataInterchangeRecord> recordWithMissingMetaAttributes = new List<TaxonomyMetaDataInterchangeRecord>();
            string queryResults = string.Empty;
            try
            {
                //initialize the context
                List<TaxonomyMetaDataInterchangeRecord> validImportRecords;
                using (CurrentDbContext =
                    new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    _currentImageManager = new ImageManager(CurrentDbContext, CurrentProjectId);
                    allData = ImportData.TaxonomyMetaDatas;
                    var invalidRecords = allData.GetInvalidRecords();
                    AddToWarning(invalidRecords.ToList(), Properties.Resources.RequiredValueNullWarningMessage);
                    validImportRecords = allData.Except(invalidRecords.ToList()).ToList();
                    validImportRecords = validImportRecords.Distinct(new TaxonomyMetaDataInterchangeRecordComparer()).ToList();
                    List<TaxonomyMetaDataInterchangeRecord> updatableTaxonomyRecord = null;
                    if (validImportRecords.Count() != 0)
                    {//                        

                        existingTaxonomyPathId = CurrentDbContext.ExecuteQuery<TaxonomyPathAndId>(@"SELECT TaxonomyPath, TaxonomyID
                                                                                                                            FROM V_Taxonomy
                                                                                                                            WHERE TaxonomyPath <> ''
                                                                                                                            AND ProjectId = {0}", CurrentProjectId).
                                                                                  ToDictionary(key => key.TaxonomyPath, value => value, StringComparer.OrdinalIgnoreCase);

                        var allImportingTaxonomyPaths = validImportRecords.Select(ad => ad.TaxonomyPath).Distinct().ToList();
                        ignoredTaxonomyPaths = GetIgnoredTaxonomyPaths(allImportingTaxonomyPaths, existingTaxonomyPathId);
                        updatableTaxonomyRecord = validImportRecords.Where(ad => !ignoredTaxonomyPaths.Contains(ad.TaxonomyPath)).ToList();// GetUpdatableTaxonomyRecord(ignoredTaxonomyPaths, allData);
                        var taxonomyPathEnrichmentImages =
                            updatableTaxonomyRecord.Where(ad => ad.TaxonomyMetaAttributeName == Resources.TaxonomyEnrichmentImageAttributeName) 
                                   .ToDictionary(item => item.TaxonomyPath, item => item.TaxonomyMetaAttributeValue);
                        //TODO: Change logic so that instead of passing  existingTaxonomyPathId pass only the list of tax path that is always in the dictionary
                        var insertedTaxonomyImageValueGuid = GetTaxonomyEnrichmentImageGuid(taxonomyPathEnrichmentImages, existingTaxonomyPathId);
                        //RemoveFailedTaxonomyEnrichmentImageRecord(updatableTaxonomyRecord, insertedTaxonomyImageValueGuid);
                        LogNonImageValues(validImportRecords, _noImageFileTaxEnrichementImage);
                        UpdateSuccessfulEnrichmentImages(updatableTaxonomyRecord, insertedTaxonomyImageValueGuid);
                         var sqlTempTableHelper = new SqlHelper(CurrentInterchangeRecordType);

                    var tempTableName = TempTablePrefix + Guid.NewGuid();
                    var createTempTableScript = sqlTempTableHelper.CreateTableScript(tempTableName, "tempdb");
                    var deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempTableName, "tempdb");

                    //create the temp table.
                    CurrentDbContext.ExecuteCommand(createTempTableScript);
                    //bulk insert data into tempdb
                    CurrentDbContext.BulkInsertAll(updatableTaxonomyRecord, tempTableName, "tempdb");
                    var missingAttributesRecordTable = tempTableName + "_missingAttributesRecords";
                    var currentRemarkId = CurrentRemarkId; //GetDefaultImportUtilRemarkId();
                    queryResults =CurrentDbContext.ExecuteQuery<string>(@"DECLARE @UserID AS UNIQUEIDENTIFIER
                                                                    DECLARE @ProjectID AS UNIQUEIDENTIFIER
                                                                    DECLARE @ResultText AS VARCHAR(2000)
                                                                    DECLARE @DefaultRemark AS UNIQUEIDENTIFIER
                                                                    DECLARE @Unchanged AS int
                                                                    SET @UserID = '"+ ImportRequestedBy + @"'
                                                                    SET @ProjectID = '"+ CurrentProjectId + @"'
                                                                    SET @DefaultRemark = '"+ CurrentRemarkId + @"'

                                                                    IF OBJECT_ID('tempdb..#IgnoredTaxonomy') IS NOT NULL
	                                                                    DROP TABLE #IgnoredTaxonomy
                                                                    IF OBJECT_ID('[tempdb]..[" + missingAttributesRecordTable + @"]') IS NOT NULL
	                                                                    DROP TABLE [tempdb]..[" + missingAttributesRecordTable + @"]
                                                                    IF OBJECT_ID('tempdb..#ImportingTaxonomyMetaValue') IS NOT NULL
	                                                                    DROP TABLE #ImportingTaxonomyMetaValue
                                                                    IF OBJECT_ID('tempdb..#NewTaxonomyMetaValue') IS NOT NULL
	                                                                    DROP TABLE #NewTaxonomyMetaValue
                                                                    IF OBJECT_ID('tempdb..#UpdatableTaxonomyMetaValue') IS NOT NULL
	                                                                    DROP TABLE #UpdatableTaxonomyMetaValue

                                                                    IF NOT EXISTS (SELECT ID
			                                                                       FROM
				                                                                       TaxonomyInfo
			                                                                       WHERE
				                                                                       ID = '00000000-0000-0000-0000-000000000000')
                                                                    BEGIN
	                                                                    INSERT INTO TaxonomyInfo
	                                                                    VALUES
		                                                                    ('00000000-0000-0000-0000-000000000000', @ProjectID, 0, NULL)
                                                                    END
                                                                    SELECT tt.TaxonomyPath, tt.MetaAttributeName, tt.MetaAttributeValue, '" + Properties.Resources.MetaAttributeNotFoundWarningMessage+@"' AS ErrorMessage
                                                                    INTO
	                                                                    [tempdb]..[" + missingAttributesRecordTable + @"]
                                                                    FROM
	                                                                    [tempdb]..[" + tempTableName + @"] tt
	                                                                    LEFT JOIN Attribute a ON tt.MetaAttributeName = a.attributename AND a.attributeType = '"+ AttributeTypeEnum.TaxonomyMeta + @"'
                                                                    WHERE
	                                                                    a.Id IS NULL

                                                                    --Delete the data from input table
                                                                    DELETE t1
                                                                    FROM
	                                                                    [tempdb]..["+ tempTableName + @"] t1
	                                                                    JOIN [tempdb]..[" + missingAttributesRecordTable + @"] mar ON mar.TaxonomyPath = t1.TaxonomyPath AND mar.MetaAttributeName = t1.MetaAttributeName

                                                                    --Record to  inserted/updated
                                                                    SELECT vtm.TaxonomyID, a1.ID AS MetaAttributeID, tt.MetaAttributeValue
                                                                    INTO
	                                                                    #ImportingTaxonomyMetaValue
                                                                    FROM
	                                                                    [tempdb]..[" + tempTableName + @"] tt    
                                                                        INNER JOIN V_Taxonomy vtm ON tt.TaxonomyPath = vtm.TaxonomyPath
	                                                                    INNER JOIN Attribute a1 ON a1.AttributeName = tt.MetaAttributeName AND a1.AttributeType =  '"+ AttributeTypeEnum.TaxonomyMeta + @"' AND a1.ProjectID = @ProjectID
                                                                    --SELECT * FROM #ImportingTaxonomyMetaValue AS itmv

                                                                    --New Values
                                                                    SELECT NEWID() AS NewTaxonomyMetaInfoId, itm.TaxonomyID, itm.MetaAttributeID, itm.MetaAttributeValue
                                                                    INTO
	                                                                    #NewTaxonomyMetaValue
                                                                    FROM
	                                                                    #ImportingTaxonomyMetaValue itm
	                                                                    LEFT OUTER JOIN TaxonomyMetaInfo tmi ON itm.TaxonomyID = tmi.TaxonomyID AND itm.MetaAttributeID = tmi.MetaAttributeID
                                                                    WHERE
	                                                                    tmi.TaxonomyID IS NULL

                                                                    SELECT tmi.ID AS ExistingTaxonomyMetaInfoId, tmd.MetaID, itm.TaxonomyID, itm.MetaAttributeID, itm.MetaAttributeValue, tmd.value
                                                                    INTO
	                                                                    #UpdatableTaxonomyMetaValue
                                                                    FROM
	                                                                    #ImportingTaxonomyMetaValue itm
	                                                                    INNER JOIN TaxonomyMetaInfo tmi ON itm.TaxonomyID = tmi.TaxonomyID AND itm.MetaAttributeID = tmi.MetaAttributeID
	                                                                    INNER JOIN TaxonomyMetaData tmd ON tmd.MetaID = tmi.ID
                                                                    WHERE
	                                                                    tmd.value <> itm.MetaAttributeValue
	                                                                    --AND tmd.Active = 1


                                                                    SELECT @ResultText = '" + Properties.Resources.IgnoredRecordCountIdentifierText + @"' + '=' + CAST(COUNT(*) AS VARCHAR(50)) + ';'
                                                                    FROM
	                                                                    #ImportingTaxonomyMetaValue itm
	                                                                    INNER JOIN TaxonomyMetaInfo tmi ON itm.TaxonomyID = tmi.TaxonomyID AND itm.MetaAttributeID = tmi.MetaAttributeID
	                                                                    INNER JOIN TaxonomyMetaData tmd ON tmd.MetaID = tmi.ID
                                                                    WHERE
	                                                                    tmd.value = itm.MetaAttributeValue
	                                                                    AND tmd.Active = 1
                                                                
                                                                    --NewTaxMetaInfo
                                                                    INSERT INTO TaxonomyMetaInfo (ID, TaxonomyID, MetaAttributeID)
                                                                    SELECT NewTaxonomyMetaInfoId, TaxonomyID, MetaAttributeID
                                                                    FROM
	                                                                    #NewTaxonomyMetaValue

                                                                    --NewTaxMetaData
                                                                    INSERT INTO TaxonomyMetaData (ID, MetaID, Value, CreatedOn, CreatedBy, CreatedRemark, Active)
                                                                    SELECT NEWID(), NewTaxonomyMetaInfoId, MetaAttributeValue, GETDATE(), @UserID, @DefaultRemark, 1
                                                                    FROM
	                                                                    #NewTaxonomyMetaValue

                                                                    SET @ResultText += '" + Properties.Resources.NewRecordCountIdentifierText + @"'+ '=' + CAST(@@ROWCOUNT AS VARCHAR(50)) + ';';

                                                                    UPDATE TaxonomyMetaData
                                                                    SET Active = 0, DeletedBy = @UserID, DeletedOn = GETDATE(), DeletedRemark = @DefaultRemark
                                                                    FROM TaxonomyMetaData tmd
	                                                                INNER JOIN #UpdatableTaxonomyMetaValue up ON tmd.MetaID = up.ExistingTaxonomyMetaInfoId
                                                                    WHERE tmd.Active = 1

                                                                    INSERT INTO TaxonomyMetaData (ID, MetaID, VALUE, CreatedOn, CreatedBy, CreatedRemark, Active)
                                                                    SELECT NEWID(), ExistingTaxonomyMetaInfoId, MetaAttributeValue, GETDATE(), @UserID, @DefaultRemark, 1
                                                                    FROM
	                                                                    #UpdatableTaxonomyMetaValue

                                                                    SET @ResultText += '" +Properties.Resources.UpdatedRecordCountIdentifierText+ @"'+ '=' +  CAST(@@ROWCOUNT AS VARCHAR(50));

                                                                    SELECT @ResultText                                                                    
                                                                    DROP TABLE #ImportingTaxonomyMetaValue
                                                                    DROP TABLE #NewTaxonomyMetaValue
                                                                    DROP TABLE #UpdatableTaxonomyMetaValue
			                                                    ").Single();
                    
                    recordWithMissingMetaAttributes =
                        CurrentDbContext.ExecuteQuery<TaxonomyMetaDataInterchangeRecord>(
                            @"SELECT TaxonomyPath, MetaAttributeName,MetaAttributeValue
                                                                                                                FROM [tempdb]..[" + missingAttributesRecordTable + @"]                                                                                                                
                                                                                                                DROP TABLE [tempdb]..[" + missingAttributesRecordTable + @"]")
                            .ToList();
                        CurrentDbContext.ExecuteCommand(deleteTempTableScript);
                        Summary.Details = ProcessQueryResults(queryResults);
                    }
                    else
                    {
                        _warnings.Add(new WorkerWarning { ErrorMessage = "No Valid Record to import" });
                    }
                }
                ProcessAllWarnings(recordWithMissingMetaAttributes, ignoredTaxonomyPaths, validImportRecords);
                if (_warnings.Count != 0)
                {
                    Summary.Warnings = _warnings;
                    Summary.State = WorkerState.CompletedWithWarning;
                    return;
                }
                Summary.State = WorkerState.Complete;
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

        private void ProcessAllWarnings(List<TaxonomyMetaDataInterchangeRecord> recordWithMissingMetaAttributes,
                                        List<string> ignoredTaxonomyPaths,
                                        List<TaxonomyMetaDataInterchangeRecord> validRecords)
        {
            LogTaxonomyWithMissingAttributeRecords(recordWithMissingMetaAttributes);
            LogIgnoredTaxonomies(ignoredTaxonomyPaths, validRecords);
            //LogNonImageValues(validRecords, _noImageFileTaxEnrichementImage);
        }

        private List<CustomKeyValuePair<string, int>> ProcessQueryResults(string queryResults)
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
            return summeryReportDetails;
        }

        private void LogTaxonomyWithMissingAttributeRecords(
            List<TaxonomyMetaDataInterchangeRecord> recordWithMissingMetaAttributes)
        {
            foreach (var recordWithMissingMetaAttribute in recordWithMissingMetaAttributes)
                AddToWarning(recordWithMissingMetaAttribute,Properties.Resources.MetaAttributeNotFoundWarningMessage);
        }

        private void LogNonImageValues(List<TaxonomyMetaDataInterchangeRecord> validRecords,
                                         List<string> failedTaxEnrichementImageValues)
        {
            foreach (var taxImageValues in failedTaxEnrichementImageValues)
            {
                var taxRecord = validRecords.Where(tx => taxImageValues != null && tx.TaxonomyMetaAttributeValue == taxImageValues.ToString());
                var taxonomyMetaDataInterchangeRecords = taxRecord as IList<TaxonomyMetaDataInterchangeRecord> ?? taxRecord.ToList();
                if (taxonomyMetaDataInterchangeRecords.Count() != 0)
                    AddToWarning(taxonomyMetaDataInterchangeRecords, Properties.Resources.EnrichmentImageFileNotPresentWarningMessage);
            }
        }

        private void LogIgnoredTaxonomies(List<string> ignoredTaxonomyPaths,
                                          List<TaxonomyMetaDataInterchangeRecord> validRecords)
        {
            foreach (var taxPath in ignoredTaxonomyPaths)
            {
                if (validRecords.Any(ad => ad.TaxonomyPath == taxPath))
                    AddToWarning(validRecords.Where(ad => ad.TaxonomyPath == taxPath), Properties.Resources.TaxonomyDoesNotExistsWarningMessage);
            }
        }

        private void AddToWarning(IEnumerable<TaxonomyMetaDataInterchangeRecord> enumerable, string message)
        {
            foreach (var record in enumerable)
                AddToWarning(record, message);
        }

        private void AddToWarning(TaxonomyMetaDataInterchangeRecord recordWithMissingMetaAttribute, string message) { _warnings.Add(new WorkerWarning {LineData = recordWithMissingMetaAttribute.ToString(), ErrorMessage = message}); }

        //private void RecurseInnerException(Exception exception)
        //{
        //    _taxonomyMetaDataImportWorkerError.Messages.Add(exception.Message);
        //    if (exception.InnerException != null)
        //        RecurseInnerException(exception.InnerException);
        //}

        private void RemoveFailedTaxonomyEnrichmentImageRecord(
            List<TaxonomyMetaDataInterchangeRecord> updatableTaxonomyRecord,
            Dictionary<string, string> insertedTaxonomyImageValueGuid)
        {
            //TODO: Ask vivek to modify this
            var taxonomyEnrichmentImageRecords =
                updatableTaxonomyRecord.Where(up => up.TaxonomyMetaAttributeName == Resources.TaxonomyEnrichmentImageAttributeName).ToList();
            for (var i = 0; i < taxonomyEnrichmentImageRecords.Count(); i++)
            {
                if (
                    !insertedTaxonomyImageValueGuid.ContainsKey(
                        taxonomyEnrichmentImageRecords.ElementAt(i).TaxonomyMetaAttributeValue))
                {
                    taxonomyEnrichmentImageRecords.RemoveAll(
                        tax => tax.TaxonomyMetaAttributeValue == taxonomyEnrichmentImageRecords.ElementAt(i).TaxonomyMetaAttributeValue);
                }
            }
        }

        private void UpdateSuccessfulEnrichmentImages(List<TaxonomyMetaDataInterchangeRecord> updatableTaxonomyRecord,
                                                      Dictionary<string, string> insertedTaxonomyImageValueGuid)
        {
            foreach (var item in insertedTaxonomyImageValueGuid)
            {
                var updatableTaxonomyRecords = updatableTaxonomyRecord.Where(tr => tr.TaxonomyMetaAttributeName == Resources.TaxonomyEnrichmentImageAttributeName && tr.TaxonomyMetaAttributeValue == item.Key && tr.TaxonomyMetaAttributeName == Resources.TaxonomyEnrichmentImageAttributeName);
                foreach (var taxonomyMetaDataImportRecord in updatableTaxonomyRecords)
                {
                    taxonomyMetaDataImportRecord.TaxonomyMetaAttributeValue = item.Value;
                }
            }
        }


        private static List<string> GetIgnoredTaxonomyPaths(List<string> allImportingTaxonomyPaths,
                                                            Dictionary<string, TaxonomyPathAndId> existingTaxonomies)
        {
            return (from aid in allImportingTaxonomyPaths
                    join et in existingTaxonomies on aid.ToLower() equals et.Key.ToLower() into temp
                    from t in temp.DefaultIfEmpty()
                    where t.Key == null
                    select aid).ToList();
        }

        private Dictionary<string, string> GetTaxonomyEnrichmentImageGuid(
            Dictionary<string, string> taxonomyPathEnrichmentImages,
            Dictionary<string, TaxonomyPathAndId> existingTaxonomyPathId)
        {
            var enrichmentImageValueGuids = new Dictionary<string, string>();
            var distinctEnrichmentImageValue = taxonomyPathEnrichmentImages.Values.Distinct();
            foreach (var enrichmentImageValue in distinctEnrichmentImageValue)
            {
                var success = false;

                //var isUri = Uri.IsWellFormedUriString(enrichmentImageValue, UriKind.Absolute);
                //var imageUri = isUri
                //                   ? new Uri(enrichmentImageValue)
                //                   : new Uri(Path.Combine(BaseImagePath, CurrentProjectId.ToString(), enrichmentImageValue));
                ////Success is for valid uri. If the image name only exist not the real file it will be false and added as warning
                success = _currentImageManager.UploadImage(enrichmentImageValue);
                var enrichmentImageGuid = _currentImageManager.RemoteImageGuid;

                //do necessary steps to 
                var updatableTaxonomyPath =
                    taxonomyPathEnrichmentImages.FirstOrDefault(tpi => tpi.Value == enrichmentImageValue).Key;
                var updatableTaxonomyId = existingTaxonomyPathId[updatableTaxonomyPath].TaxonomyId.ToString();
                var newSku = _currentImageManager.ImageSku;
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
                                                                                                  true,useChache:false),
                                                               Value = updatableTaxonomyId
                                                           }
                                                   }
                                           });
                newSku.SkuInfos.Add(new SkuInfo(CurrentDbContext) {TaxonomyID = new Guid(updatableTaxonomyId)});
                    enrichmentImageValueGuids.Add(enrichmentImageValue, enrichmentImageGuid);

                if (!success)
                    _noImageFileTaxEnrichementImage.Add(enrichmentImageValue);
            }
            SaveDataChanges();
            return enrichmentImageValueGuids;
        }

        #endregion
    }
}