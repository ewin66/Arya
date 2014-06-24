using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;

//TODO: Create default taxonomyinfo and TaxonomyData with node name string.empty

namespace Arya.Framework.IO.Imports
{
    [ImportOrder(8)]
    public class SKUTaxonomyImportWorker : ImportWorkerBase
    {
        #region Private Fields
        private const string TempTablePrefix = "itemtax:";
        private readonly WorkerError _skuTaxonomyImportWorkerError = new WorkerError();
        
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();
        #endregion
        
        #region Constructor
        public SKUTaxonomyImportWorker()
            
        {
            CurrentInterchangeRecordType = typeof (SkuTaxonomyInterchangeRecord);
        }
#endregion 
       

        public override void Run()
        {
            string queryResults;
            try
            {
                using (
                    CurrentDbContext = CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    List<SkuTaxonomyInterchangeRecord> allData2;
                    allData2 = ImportData.SkuTaxonomies;

                    var invalidRecords = allData2.GetInvalidRecords();
                    var skuTaxonomyInterchangeRecords = invalidRecords as SkuTaxonomyInterchangeRecord[] ?? invalidRecords.ToArray();
                    skuTaxonomyInterchangeRecords.ToList().ForEach(ir => _warnings.Add(new WorkerWarning
                    {
                        LineData = ir.ToString(),
                        ErrorMessage = Properties.Resources.RequiredValueNullWarningMessage
                    }));
                    var validImportRecords = allData2.Except(skuTaxonomyInterchangeRecords.ToList()).ToList();


                    //prepare a table for delete
                    var sqlTempTableHelper = new SqlHelper(CurrentInterchangeRecordType);

                    var tempTableName = TempTablePrefix + Guid.NewGuid();
                    var warningTableName = tempTableName + "_warning";
                    var createTempTableScript = sqlTempTableHelper.CreateTableScript(tempTableName, "tempdb");
                    var deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempTableName, "tempdb");

                    //create the temp table.
                    CurrentDbContext.ExecuteCommand(createTempTableScript);

                    
                    //bulk insert data into tempdb
                    CurrentDbContext.BulkInsertAll(validImportRecords, tempTableName, "tempdb");
                    var createMissingSku = CurrentImportOptions.HasFlag(ImportOptions.CreateMissingSkus);
                    var queryString = @"Declare @CreateMissingSku as bit 
                                                            DECLARE @UserID AS UNIQUEIDENTIFIER
                                                            DECLARE @ProjectID AS UNIQUEIDENTIFIER
                                                            DECLARE @ResultText AS VARCHAR(2000)
                                                            DECLARE @DefaultRemark AS UNIQUEIDENTIFIER
                                                            DECLARE @NewSkuCount AS int
                                                            DECLARE @IgnoredCount AS int
                                                            DECLARE @DefaultTaxonomyID AS UNIQUEIDENTIFIER
                                                            DECLARE @CurrentRemarkId AS UNIQUEIDENTIFIER
                                                            SET @CurrentRemarkId = '" + CurrentRemarkId + @"'


                                                            SET @UserID = '" + ImportRequestedBy + @"'
                                                            SET @ProjectID =  '" + CurrentProjectId + @"'
                                                            SET @CreateMissingSku = '" + createMissingSku + @"'
                                                            SET @IgnoredCount = 0
                                                            SET @NewSkuCount = 0
                                                            SET @ResultText = '';
                                                            IF OBJECT_ID('tempdb..#NewSkus') IS NOT NULL
                                                            DROP TABLE #NewSkus

                                                            IF OBJECT_ID('[tempdb]..[" + warningTableName +@"]') IS NOT NULL
                                                            DROP TABLE [tempdb]..[" + warningTableName + @"]

                                                            CREATE TABLE [tempdb]..[" + warningTableName + @"]
                                                            (
                                                                ItemID varchar(255), 
                                                                TaxonomyPath varchar(1000),
                                                                WarningMessage varchar(255)
                                                            );

                                                            IF OBJECT_ID('tempdb..#UpdatableSkuInfo') IS NOT NULL 
                                                            DROP TABLE #UpdatableSkuInfo

                                                            select @DefaultTaxonomyID = TaxonomyID FROM [TaxonomyData] WHERE [NodeName] = '' 
                                                            

                                                            --Process multiple taxonomy for single item
                                                            SELECT  tt.ItemID 
                                                            INTO #MultipleTaxonomyForSingleItem
                                                            FROM  [tempdb]..[" + tempTableName + @"] tt
                                                            GROUP BY  tt.ItemID HAVING COUNT(*) > 1                                                        
                                                            
                                                            INSERT INTO  [tempdb]..[" + warningTableName +@"] (ItemID,TaxonomyPath,WarningMessage )
                                                            SELECT  ItemID,'','" +Properties.Resources.ItemIdContainsMultipleTaxonomiesWanringMessage + @"' 
                                                            FROM #MultipleTaxonomyForSingleItem
                            
                                                            DELETE [tempdb]..[" + tempTableName + @"] FROM [tempdb]..[" +tempTableName + @"] tf 
                                                            INNER JOIN #MultipleTaxonomyForSingleItem mt ON mt.ItemID = tf.ItemID

                                                            INSERT INTO [tempdb]..[" + warningTableName + @"](ItemID,TaxonomyPath,WarningMessage)
                                                            SELECT tf.ItemID, tf.TaxonomyPath, '" + Properties.Resources.TaxonomyDoesNotExistsWarningMessage + @"'
                                                            FROM [tempdb]..[" + tempTableName + @"] tf LEFT OUTER JOIN V_Taxonomy tp 
                                                            ON  LOWER(tf.TaxonomyPath) = LOWER(tp.TaxonomyPath) 
                                                            WHERE tp.TaxonomyPath IS  NULL and  tf.TaxonomyPath IS NOT  NULL
                                                            --SET @IgnoredCount += @@ROWCOUNT

                                                            INSERT INTO [tempdb]..[" + warningTableName + @"](ItemID,TaxonomyPath,WarningMessage)
                                                            SELECT tf.ItemID, tf.TaxonomyPath, '" + Properties.Resources.SkuOnCrossListedTaxonomyDoesNotExistsWarningMessage + @"'   
                                                            FROM [tempdb]..[" + tempTableName + @"] tf INNER JOIN V_Taxonomy tp 
                                                            ON  LOWER(tf.TaxonomyPath) = LOWER(tp.TaxonomyPath) Where tp.NodeType = '" + TaxonomyInfo.NodeTypeDerived + @"'
                                                            --Remove the items with  tax does not exist in the db
                                                            Delete [tempdb]..[" + tempTableName + @"] FROM [tempdb]..[" + tempTableName + @"] tf Inner JOIN [tempdb]..[" + warningTableName + @"] wr ON tf.ItemID = wr.ItemID 

                                                            --Process New Skus
                                                            --Find New Skus
                                                            select tf.ItemID, tf.TaxonomyPath 
                                                            INTO #NewSkus
                                                            from [tempdb]..[" + tempTableName +@"] tf left OUTER JOIN sku  on sku.ItemID = tf.[ItemID] where sku.ID is null                                                         
                                                            --SET @NewSkuCount = @@ROWCOUNT
                                                            SET @ResultText = '" +Properties.Resources.NewRecordCountIdentifierText +@"'+ '='  + CAST(@@ROWCOUNT  AS VARCHAR(500)) + ';';

                                                            Delete [tempdb]..[" + tempTableName + @"] FROM [tempdb]..[" +tempTableName + @"] tf Inner JOIN #NewSkus wr ON tf.ItemID = wr.ItemID 

                                                            IF @CreateMissingSku = 0
                                                            BEGIN
                                                                INSERT into [tempdb]..[" + warningTableName + @"](ItemID,TaxonomyPath,WarningMessage)
                                                                select ItemID,TaxonomyPath,'" + Properties.Resources.CreateMissingSkuFlagOffWarningMessage + @"' As Message   from #NewSkus
                                                                Delete ns FROM  #NewSkus ns 
                                                                SET @ResultText = '" + Properties.Resources.NewRecordCountIdentifierText + @"'+ '='  + CAST(0  AS VARCHAR(500)) + ';';
                                                                --SET @IgnoredCount = @@ROWCOUNT
                                                            END

                                                            SELECT NEWID() AS SkuID, ns.ItemID, vt.TaxonomyId 
                                                            INTO #taxSku
                                                            FROM #NewSkus ns INNER JOIN V_Taxonomy vt ON vt.TaxonomyPath = ns.TaxonomyPath
                                                            --SELECT * FROM #taxSku ts

                                                            INSERT INTO Sku (ID,ItemID,ProjectID,CreatedOn,CreatedBy,SkuType)
                                                            SELECT ts.SkuID,ItemID,@ProjectID,GETDATE(),@UserID,'Product'
                                                            FROM  #taxSku ts

                                                             Insert Into SkuInfo(ID, SkuID,TaxonomyID, CreatedOn, CreatedBy,CreatedRemark, Active)
                                                            select NEWID(), ts.SkuID,ts.TaxonomyId,GETDATE(),@UserID,@CurrentRemarkId,1
                                                            from #taxSku ts

                                                            DELETE #NewSkus   FROM #NewSkus ns  Inner join #taxSku nt ON nt.ItemID = ns.ItemID
                                                            --SELECT * FROM  #NewSkus  
                                                            --Create new skus
                                                            
                                                            INSERT INTO Sku (ID,ItemID,ProjectID,CreatedOn,CreatedBy,SkuType)
                                                            SELECT NEWID(),ItemID,@ProjectID,GETDATE(),@UserID,'Product'
                                                            FROM #NewSkus   
                                                            

                                                                                                                                  
                                                            SELECT sf.ItemID, sf.TaxonomyPath as newTaxPath,vt1.TaxonomyId as newTaxID, sku.ID AS SkuID,vt.TaxonomyPath as OldTaxonomyPath, vt.TaxonomyId as oldTaxId 
                                                            INTO #UpdatableSkuInfo
                                                            FROM [tempdb]..[" + tempTableName + @"] sf
                                                                Inner join V_Taxonomy vt1 on  vt1.TaxonomyPath = sf.TaxonomyPath 
                                                                Inner join Sku  on sku.ItemID = sf.ItemID 
								                                Inner Join SkuInfo si on si.SkuID = sku.ID and si.Active = 1
								                                Inner join V_Taxonomy vt on vt.TaxonomyId = si.TaxonomyID

                                                            --Remove the items that has same data in import  file and db
                                                            --Select * from #UpdatableSkuInfo
                                                            
                                                            --SELECT @IgnoredCount = COUNT(*) from #UpdatableSkuInfo up WHERE up.newTaxPath = up.OldTaxonomyPath
                                                            DELETE  #UpdatableSkuInfo  FROM  #UpdatableSkuInfo up WHERE up.newTaxPath = up.OldTaxonomyPath
                                                            --SET @IgnoredCount = @@ROWCOUNT 
                                                            SET @ResultText += '" +Properties.Resources.IgnoredRecordCountIdentifierText +@"' + '='  + CAST(@@ROWCOUNT  AS VARCHAR(500))  + ';';

                                                            --Update Sku Info and Create new
                                                            UPDATE SkuInfo
                                                            SET Active = 0
                                                            FROM SkuInfo si
                                                            INNER JOIN #UpdatableSkuInfo usi ON usi.SkuID = si.SkuID
                                                            WHERE si.Active = 1

                                                            INSERT INTO SkuInfo ( ID, SkuID,TaxonomyID, CreatedOn, CreatedBy,CreatedRemark, Active)
                                                            SELECT NEWID(), usi.SkuID,usi.newTaxID,GETDATE(),@UserID,@CurrentRemarkId,1
                                                            FROM #UpdatableSkuInfo usi

                                                            SET @ResultText += '" +Properties.Resources.UpdatedRecordCountIdentifierText +@"'+ '=' + CAST(@@ROWCOUNT  AS VARCHAR(500))
                                                            
                                                            -- Create Sku Info for those that does not have sku info

                                                            Insert Into SkuInfo(ID, SkuID,TaxonomyID, CreatedOn, CreatedBy,CreatedRemark, Active)
                                                            select NEWID(), s.ID,@DefaultTaxonomyID,GETDATE(),@UserID,@CurrentRemarkId,1
                                                            from sku s left outer join SkuInfo on s.ID = SkuInfo.SkuID and SkuInfo.Active = 1 where SkuInfo.SkuID is Null

                                                            SELECT @ResultText                                                                    
                                                            DROP TABLE #NewSkus
                                                            DROP TABLE #UpdatableSkuInfo
                                                            ";
                    
                    queryResults = CurrentDbContext.ExecuteQuery<string>(queryString).Single();

                    var warningsRecords =   CurrentDbContext.ExecuteQuery<ItemTaxonomyWarnings>(@"SELECT ItemID, TaxonomyPath,WarningMessage
                                                                                                                FROM [tempdb]..[" + warningTableName + @"]").ToList();
                    CurrentDbContext.ExecuteCommand(@"IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL 
                                                        DROP TABLE [tempdb]..[" + warningTableName + @"]");

                    foreach (var itemTaxonomyWarning in warningsRecords)
                    {
                    _warnings.Add(new WorkerWarning
                                      {
                                          LineData = itemTaxonomyWarning.ToString(),
                                          ErrorMessage = itemTaxonomyWarning.WarningMessage
                                      });
                    }
                    ProcessSummaryReport(queryResults);

                    //delete temperorary table
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
        public override List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }
    }

    internal class ItemTaxonomyWarnings
    {
        public string ItemId { get; set; }
        public string TaxnonomyPath { get; set; }
        public string WarningMessage { get; set; }

        public override string ToString()
        {
            var recordToString = ItemId + '\t' + TaxnonomyPath;
            return recordToString;
        }
    }
}