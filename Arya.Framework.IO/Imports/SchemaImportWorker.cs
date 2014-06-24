using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.IO.Properties;

namespace Arya.Framework.IO.Imports
{
    //TODO: Refactor process result methods,Seperate tax and attri missiing in the warning inside sql  add regions how the sqk return results
    [ImportOrder(5)]
    public class SchemaImportWorker : ImportWorkerBase
    {
        private const string TempTablePrefix = "scd:";
        private readonly WorkerError _schemaImportWorkerError = new WorkerError();

        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();

        public SchemaImportWorker() { CurrentInterchangeRecordType = typeof (SchemaInterchangeRecord); }

        public override void Run()
        {
            string queryResults;
            try
            {
                //initialize the context
                CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy);
                List<SchemaInterchangeRecord> allData;
                allData = ImportData.Schemas;
                //read all the values into a list<T>, change this as its not very efficient and scalable.
                //TODO: Ask vivek how to dedupe the file 

                //CsvConfiguration conf = GetCurrentConfiguration();
                ////char delimiterChar = (char)FieldDelimiter.GetDisplayTextAndDbValue().DbValue;
                //using (var csvReader = new CsvReader(File.OpenText(InputFilePath), conf))
                //{
                //    allData = csvReader.GetRecordsWithNulls<SchemaInterchangeRecord>().Distinct(new SchemaInterchangeRecordComparer()).ToList();
                //}

                var invalidRecords = allData.GetInvalidRecords();
                var schemaInterchangeRecords = invalidRecords as IList<SchemaInterchangeRecord>
                                               ?? invalidRecords.ToList();
                schemaInterchangeRecords.ToList()
                    .ForEach(
                        ir =>
                            _warnings.Add(new WorkerWarning
                                          {
                                              LineData = ir.ToString(),
                                              ErrorMessage = Resources.RequiredValueNullWarningMessage
                                          }));
                var validImportRecords = allData.Except(schemaInterchangeRecords.ToList()).ToList();
                //var newRecords = from item in validImportRecords 
                //    let attName = item.AttributeName.ToLower()
                //                 group item by new { item.TaxonomyPath, attName }
                //                 into rec
                //                 select new {rec.Key.TaxonomyPath, rec.Key.attName, col = rec};//validImportRecords.GroupBy((sir=>sir.TaxonomyPath,sir=>sir.AttributeName, sir=>sir,(key,g)=>new {tax = key} )
                //prepare a table for delete
                var sqlTempTableHelper = new SqlHelper(CurrentInterchangeRecordType);

                var tempTableName = TempTablePrefix + Guid.NewGuid();
                var warningTableName = tempTableName + "_warning";
                var duplicateAttrSameNodeTableName = tempTableName + "_duplicateAttrSameNode";
                var createTempTableScript = sqlTempTableHelper.CreateTableScript(tempTableName, "tempdb");
                var deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempTableName, "tempdb");

                //create the temp table.
                CurrentDbContext.ExecuteCommand(createTempTableScript);

                //bulk insert data into tempdb
                CurrentDbContext.BulkInsertAll(validImportRecords, tempTableName, "tempdb");
                var queryString = @"DECLARE @UserID AS UNIQUEIDENTIFIER
                                                        DECLARE @ProjectID AS UNIQUEIDENTIFIER
                                                        DECLARE @ResultText AS VARCHAR(2000)
                                                        DECLARE @DefaultRemark AS UNIQUEIDENTIFIER
                                                        DECLARE @NewDataCount AS int
                                                        DECLARE @IgnoredCount AS int
                                                        DECLARE @DefaultTaxonomyID AS UNIQUEIDENTIFIER
                                                        DECLARE @DefaultImportUtilRemarkID AS UNIQUEIDENTIFIER
                                                        SET @DefaultImportUtilRemarkID = '" + CurrentRemarkId + @"'
                                                        SET @UserID = '" + ImportRequestedBy + @"'
                                                        SET @ProjectID =  '" + CurrentProjectId + @"'
                                                        SET @IgnoredCount = 0
                                                        SET @NewDataCount = 0
                                                        SET @ResultText = ''
                                                        IF OBJECT_ID('tempdb..#SI_Data') IS NOT NULL 
                                                        DROP TABLE #SI_Data
                                                        IF OBJECT_ID('tempdb..#tempGroup') IS NOT NULL 
                                                        DROP TABLE #tempGroup
                                                        
                                                        IF OBJECT_ID('[tempdb]..[" + warningTableName+ @"]') IS NOT NULL 
                                                        DROP TABLE [tempdb]..[" + warningTableName + @"]

                                                        IF OBJECT_ID('[tempdb]..[" + duplicateAttrSameNodeTableName+ @"]') IS NOT NULL 
                                                        DROP TABLE [tempdb]..[" + duplicateAttrSameNodeTableName + @"]

                                                        SELECT tdata.*, ft.TaxonomyId, a.ID AS AttributeID, ISNULL(sa.SchemaID,si.ID) AS SchemaID, sa.SchemaDataID
                                                        INTO #SI_Data	
                                                        FROM [tempdb]..[" + tempTableName + @"] tdata
                                                        LEFT OUTER JOIN V_Taxonomy ft ON tdata.TaxonomyPath = ft.TaxonomyPath and ft.ProjectId = @ProjectID
                                                        LEFT OUTER  JOIN Attribute a ON LOWER(tdata.AttributeName) = LOWER(a.AttributeName) AND a.AttributeType IN ('Sku','Global', 'Derived', 'Flag') and a.ProjectID = @ProjectID
                                                        LEFT OUTER JOIN V_Schema sa ON ft.TaxonomyId = sa.TaxonomyId AND a.ID = sa.AttributeID
                                                        LEFT OUTER JOIN SchemaInfo si ON si.AttributeID = a.ID and si.TaxonomyID = ft.TaxonomyId
                                                        

                                                        --Seperate Taxonomy and Attribute that does not exist
                                                        select sd.TaxonomyPath, sd.AttributeName, sd.NavigationOrder, sd.DisplayOrder,sd.DataType, sd.InSchema
                                                        INTO [tempdb]..[" + warningTableName + @"]
                                                        FROM #SI_Data as sd 
                                                        WHERE sd.TaxonomyId IS NULL OR sd.AttributeID IS NULL
                                                        
                                                        --Delete record that has taxonomy or attribute does not exits in the db
                                                        DELETE #SI_Data  FROM #SI_Data sd WHERE sd.TaxonomyId IS NULL OR sd.AttributeID IS NULL 

                                                        --Find out the Duplicate attribute on taxonomy
                                                        SELECT sd.TaxonomyId, sd.AttributeID, count(*) as Count 
                                                        INTO #tempGroup 
                                                        FROM #SI_Data as sd GROUP BY sd.TaxonomyId, sd.AttributeID
                                                        
                                                        --Insert All the import data  so that it can read later
                                                          
                                                        SELECT sd.TaxonomyPath, sd.AttributeName, sd.NavigationOrder, sd.DisplayOrder,sd.DataType, sd.InSchema
                                                        INTO [tempdb]..[" + duplicateAttrSameNodeTableName + @"] 
                                                        FROM #tempGroup tg INNER JOIN #SI_Data sd ON sd.TaxonomyId = tg.TaxonomyId AND  sd.AttributeID = tg.AttributeID 
                                                        Where tg.Count >1

                                                        --remove from insert table
                                                        DELETE #SI_Data  FROM #SI_Data si INNER JOIN #tempGroup   gt 
                                                        ON gt.TaxonomyId=si.TaxonomyId AND gt.AttributeID = si.AttributeID   Where gt.Count >1


 
                                                      --Insert Schema Info for those that does  not have entry already
                                                        INSERT INTO SchemaInfo
                                                        SELECT NEWID(), TaxonomyID, AttributeID
                                                        FROM #SI_Data
                                                        WHERE SchemaID IS NULL

                                                        SET @ResultText = '" + Resources.NewRecordCountIdentifierText + @"'+ '='  + CAST(@@ROWCOUNT  AS VARCHAR(50))+ ';'

                                                        UPDATE #SI_Data
                                                        SET SchemaID = si.ID
                                                        FROM #SI_Data sid JOIN SchemaInfo si ON sid.TaxonomyID = si.TaxonomyID AND sid.AttributeID = si.AttributeID
                                                        WHERE SchemaID IS NULL
                                                        
                                                        --Remove similar data in file and db
                                                        DELETE #SI_Data
                                                        FROM #SI_Data sd
                                                        INNER JOIN V_Schema sa ON sd.SchemaID = sa.SchemaID
                                                        WHERE 
                                                        (
	                                                        sd.NavigationOrder = sa.NavigationOrder 
	                                                        AND sd.Displayorder = sa.DisplayOrder
	                                                        AND sd.DataType = sa.DataType
	                                                        AND isnull(sd.InSchema, 1) = sa.InSchema
                                                        )

                                                        SET @ResultText += '" + Resources.IgnoredRecordCountIdentifierText + @"' + '='  + CAST(@@ROWCOUNT  AS VARCHAR(50))+ ';'                                                        

                                                        --Deactivte schema data for already existing entries
                                                        Update SchemaData 
                                                        SET Active = 0, DeletedBy = @UserID, DeletedOn = GETDATE(), DeletedRemark = @DefaultImportUtilRemarkID
                                                        FROM SchemaData sd
                                                        INNER JOIN #SI_Data sa ON sd.SchemaID = sa.SchemaID
                                                        WHERE sd.Active = 1

                                                        SET @ResultText += '" + Resources.UpdatedRecordCountIdentifierText + @"'+ '=' + CAST(@@ROWCOUNT  AS VARCHAR(50)) + ';';
                                                        
                                                        --INSERT Schema data for all the infos for new data 
                                                        INSERT INTO SchemaData (ID, SchemaID, InSchema, DataType, NavigationOrder,DisplayOrder, CreatedOn, CreatedBy,CreatedRemark, Active)
                                                        SELECT  NEWID(), si.SchemaID, si.InSchema, si.DataType, si.NavigationOrder, si.DisplayOrder, GETDATE(), @UserID,@DefaultImportUtilRemarkID, 1
                                                        from #SI_Data si

                                                        
                                                        DROP TABLE #SI_Data
                                                        SELECT @ResultText";
                queryResults = CurrentDbContext.ExecuteQuery<string>(queryString).Single();

                //delete the temp table.

                var duplicateAttrSingleTaxonomyRecords =
                    CurrentDbContext.ExecuteQuery<SchemaInterchangeRecord>(
                        @"SELECT w.TaxonomyPath, w.AttributeName, w.NavigationOrder, w.DisplayOrder,w.DataType, w.InSchema
                                                                                                                FROM [tempdb]..["
                        + duplicateAttrSameNodeTableName
                        + @"]  w                                                                                                             
                                                                                                            ").ToList();
                foreach (var itemTaxonomyWarning in duplicateAttrSingleTaxonomyRecords)
                {
                    _warnings.Add(new WorkerWarning
                                  {
                                      LineData = itemTaxonomyWarning.ToString(),
                                      ErrorMessage =
                                          Resources.DuplicateAttributeSingleTaxonomyWarningMessage
                                  });
                }

                var warningsRecords =
                    CurrentDbContext.ExecuteQuery<SchemaInterchangeRecord>(
                        @"SELECT w.TaxonomyPath, w.AttributeName, w.NavigationOrder, w.DisplayOrder,w.DataType, w.InSchema
                                                                                                                FROM [tempdb]..["
                        + warningTableName + @"]  w").ToList();
                //CurrentDbContext.ExecuteCommand(deleteTempTableScript);
                foreach (var itemTaxonomyWarning in warningsRecords)
                {
                    _warnings.Add(new WorkerWarning
                                  {
                                      LineData = itemTaxonomyWarning.ToString(),
                                      ErrorMessage = Resources.TaxonomyOrAttributeMissingWarningMessage
                                  });
                }
                ProcessSummaryReport(queryResults);
                //delete temperorary table
                CurrentDbContext.ExecuteCommand(deleteTempTableScript);
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

        private void ProcessSummaryReport(string queryResults)
        {
            //var successCount = 0;
            var reportItems = queryResults.Split(';');
            //Summary summaryReport = new Summary();
            var summeryReportDetails = new List<CustomKeyValuePair<string, int>>();
            foreach (var reportItem in reportItems)
            {
                //itemCount = Int32.Parse(reportItem.Substring(reportItem.IndexOf('=') + 1).Trim());
                int itemCount;
                if (!Int32.TryParse(reportItem.Substring(reportItem.IndexOf('=') + 1).Trim(), out itemCount))
                    itemCount = 0;
                if (reportItem.Contains(Resources.NewRecordCountIdentifierText))
                {
                    summeryReportDetails.Add(new CustomKeyValuePair<string, int>(
                        Resources.NewRecordCountIdentifierText, itemCount));
                }
                else if (reportItem.Contains(Resources.UpdatedRecordCountIdentifierText))
                {
                    summeryReportDetails.Add(
                        new CustomKeyValuePair<string, int>(Resources.UpdatedRecordCountIdentifierText, itemCount));
                }
                else if (reportItem.Contains(Resources.IgnoredRecordCountIdentifierText))
                {
                    summeryReportDetails.Add(
                        new CustomKeyValuePair<string, int>(Resources.IgnoredRecordCountIdentifierText, itemCount));
                }
                //successCount += itemCount;
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

        public override List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }
    }
}