using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using LinqKit;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.IO.Properties;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Imports
{
    [ImportOrder(9)]
    public class SkuAttributeValueImportWorker : ImportWorkerBase
    {
        #region Fields

        private const string TempExistingEntityInfoTablePrefix = "ief:";
        private const string TempSkuKeyTablePrefix = "iak:";
        private const string TempTablePrefix = "iav:";

        private readonly bool _attPlusUom;
        private readonly bool _replaceExistingValues;
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();

        private int _ignoredCountMultiValues;
        private int _successCountForEntityInfo;
        private int _successCountForMultiValues;
        private string _queryResults;

        #endregion Fields

        #region Constructors

        public SkuAttributeValueImportWorker()
        {
            CurrentInterchangeRecordType = typeof (SkuAttributeValueInterchangeRecord);
            _replaceExistingValues = true;
            _attPlusUom = true;
        }

        #endregion Constructors

        #region Methods

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        public override void Run()
        {
            //State = WorkerState.Working;
            try
            {
                if (!CurrentImportOptions.HasFlag(ImportOptions.CreateMissingValues))
                {
                    CurrentLogWriter.Debug(Resources.CreateMissingValuesFlagOffWanringMessage);
                    _warnings.Add(new WorkerWarning
                                  {
                                      LineData = Resources.CreateMissingValuesFlagOffLineDataText,
                                      ErrorMessage = Resources.CreateMissingValuesFlagOffWanringMessage
                                  });
                }

                using (CurrentDbContext = new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    var allData = ImportData.SkuAttributeValues;
                    CurrentLogWriter.DebugFormat("{0}: Total Records: {1}", Arguments.Id, allData.Count);

                    var invalidRecords = allData.GetInvalidRecords();
                    var attributeValueInterchangeRecords = invalidRecords as IList<SkuAttributeValueInterchangeRecord>
                                                           ?? invalidRecords.ToList();
                    CurrentLogWriter.DebugFormat("{0}: Invalid Records: {1}", Arguments.Id,attributeValueInterchangeRecords.Count);
                    attributeValueInterchangeRecords.ToList()
                        .ForEach(
                            ir =>
                                _warnings.Add(new WorkerWarning
                                              {
                                                  LineData = ir.ToString(),
                                                  ErrorMessage =
                                                      Resources.RequiredValueNullWarningMessage
                                              }));
                    var validImportRecords = allData.Except(attributeValueInterchangeRecords.ToList()).ToList();
                    CurrentLogWriter.DebugFormat("{0}: Valid Records: {1}",Arguments.Id, validImportRecords.Count);

                    var recordsWithEntityInfo = validImportRecords.Where(ad => ad.EntityID != null);
                    validImportRecords = validImportRecords.Where(ad => ad.EntityID == null).ToList();
                       // (from row in validImportRecords where !recordsWithEntityInfo.Contains(row) select row).ToList();
                    var skuAttributeValueInterchangeRecords =
                        recordsWithEntityInfo as SkuAttributeValueInterchangeRecord[] ?? recordsWithEntityInfo.ToArray();
                    CurrentLogWriter.DebugFormat("{0}: Records with EntityInfo: {1}",Arguments.Id,
                        skuAttributeValueInterchangeRecords.Length);
                    if (skuAttributeValueInterchangeRecords.Count() != 0)
                        ProcessRecordWithEntityData(skuAttributeValueInterchangeRecords);

                    //TODO:  take out multi value from import data and data bases Start
                    var groupByResults =
                        (from t in validImportRecords
                            let groupItems = GetGroup(t, _attPlusUom)
                            group t by groupItems
                            into grp select grp).ToHashSet();

                    var multiValues =
                        (from grp in groupByResults where grp.Count() > 1 from row in grp select row).ToHashSet();

                    CurrentLogWriter.DebugFormat("{0}: {1} multi-Value-sets",Arguments.Id, multiValues.Count);
                    var distinctValues =
                        groupByResults.Where(r => r.Count() == 1)
                            .Select(
                                r =>
                                    new StringValueWrapperRecord
                                    {
                                        StringValue =
                                            string.Format("{0}|{1}{2}", r.Key.Item1,
                                                r.Key.Item2,
                                                _attPlusUom
                                                    ? string.Format("|{0}", r.Key.Item3)
                                                    : string.Empty)
                                    });
                    //.ToList();}

                    //TODO : Bulk Insert n db and then join with

                    var sqlTempTableHelper = new SqlHelper(typeof (StringValueWrapperRecord));
                    var tempSkuKeyTableName = TempSkuKeyTablePrefix + Guid.NewGuid();
                    var createTempTableScript = sqlTempTableHelper.CreateTableScript(tempSkuKeyTableName, "tempdb");
                    var deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempSkuKeyTableName, "tempdb");

                    //create the temp table.
                    CurrentDbContext.ExecuteCommand(createTempTableScript);

                    CurrentLogWriter.DebugFormat("{0}: Bulk Inserting Distinct Value (non-multi-value) records",Arguments.Id);
                    CurrentDbContext.BulkInsertAll(distinctValues, tempSkuKeyTableName, "tempdb");
                    //End of bulk insert

                    CurrentLogWriter.DebugFormat("{0}: Processing Distinct Value (non-multi-value) records",Arguments.Id);
                    var query = string.Format(@"
                    SELECT skd.StringValue
                    FROM [tempdb]..[{1}]  skd
                    INNER JOIN V_ActiveSkuData vasd
                    ON vasd.ItemID + '|' + vasd.AttributeName {0} = skd.StringValue
                    GROUP BY skd.StringValue Having count(*) > 1
                    ", _attPlusUom ? " + '|' + ISNULL(Uom,'')" : string.Empty, tempSkuKeyTableName);
                    var groupByResultsDb = CurrentDbContext.ExecuteQuery<StringValueWrapperRecord>(query);

                    CurrentDbContext.ExecuteCommand(deleteTempTableScript);

                    var dbResults = (from row in groupByResultsDb
                        let grp = row.StringValue
                        let parts = grp.Split('|')
                        let itemId = parts[0]
                        let attribute = parts[1]
                        let uom = parts.Length > 2 && parts[2] != string.Empty ? parts[2] : null
                        select new Tuple<string, string, string>(itemId, attribute, uom)).ToHashSet();

                    multiValues.AddRange(from grp in groupByResults
                        where dbResults.Contains(grp.Key)
                        from row in grp
                        select row);

                    ProcessMultivalues(multiValues);
                    //End multivalue vandle

                    validImportRecords =
                        (from row in validImportRecords where !multiValues.Contains(row) select row).ToList();

                    sqlTempTableHelper = new SqlHelper(CurrentInterchangeRecordType);

                    var tempTableName = TempTablePrefix + Guid.NewGuid();
                    var warningTableName = tempTableName + "_warning";
                    createTempTableScript = sqlTempTableHelper.CreateTableScript(tempTableName, "tempdb");
                    deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempTableName, "tempdb");

                    //create the temp table.
                    CurrentDbContext.ExecuteCommand(createTempTableScript);

                    CurrentLogWriter.DebugFormat("{0}: Bulk Inserting valid Import Records",Arguments.Id);
                    CurrentDbContext.BulkInsertAll(validImportRecords, tempTableName, "tempdb");

                    CurrentLogWriter.DebugFormat("{0}: Processing valid Import Records",Arguments.Id);
                    CurrentDbContext.ExecuteCommand(@"
                                         --Update entity data, SET active to 0
                                         ALTER TABLE [tempdb]..[" + tempTableName + @"]
                                         ADD SkuId UNIQUEIDENTIFIER
                                        ,AttributeId UNIQUEIDENTIFIER
                                        ,EntityInfoId UNIQUEIDENTIFIER
                                        ,EntityDataId UNIQUEIDENTIFIER
                                        ,DbField1 nvarchar(4000)
                                        ,DbField2 nvarchar(4000)
                                        ,DbField3 nvarchar(4000)
                                        ,DbField4 nvarchar(4000)
                                        ,DbField5 nvarchar(4000)");
                    var createMissingValues = CurrentImportOptions.HasFlag(ImportOptions.CreateMissingValues);
                    var markAsBeforeEntity = CurrentImportOptions.HasFlag(ImportOptions.MarkAsBeforeEntity);
                    var singleValueProcessorQueryString = string.Format(@"Declare @CreateMissingValues as bit
                                        Declare @MarkAsBeforeEntity as bit
                                        DECLARE @UserID AS UNIQUEIDENTIFIER
                                        DECLARE @ProjectID AS UNIQUEIDENTIFIER
                                        DECLARE @ResultText AS VARCHAR(2000)
                                        DECLARE @CurrentRemarkId AS UNIQUEIDENTIFIER
                                        DECLARE @NewDataCount AS int
                                        DECLARE @IgnoredCount AS int
                                        SET @UserID = '" + ImportRequestedBy + @"'
                                        SET @ProjectID = '" + CurrentProjectId + @"'
                                        SET @IgnoredCount = 0
                                        SET @NewDataCount = 0
                                        SET @ResultText = ''
                                        SET @CreateMissingValues = '" + createMissingValues + @"'
                                        SET @MarkAsBeforeEntity = '" + markAsBeforeEntity + @"'
                                        SET @CurrentRemarkId = '" + CurrentRemarkId + @"'

                                        IF OBJECT_ID('tempdb..#NewEntityInfo') IS NOT NULL
                                        DROP TABLE #NewEntityInfo

                                        IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL
                                            DROP TABLE [tempdb]..[" + warningTableName + @"]

                                        CREATE TABLE [tempdb]..[" + warningTableName + @"]
                                        (
                                            ItemID varchar(255),
                                            AttributeName varchar(255),
                                            Uom varchar(255),
                                            Value varchar(4000),
                                            Field1 varchar(255),
                                            Field2 varchar(255),
                                            Field3 varchar(255),
                                            Field4 varchar(255),
                                            Field5 varchar(255),
                                            EntityID varchar(255),
                                            WarningMessage varchar(255)
                                        );

                                        -- Take Out Missiing Skus
                                        INSERT into [tempdb]..[" + warningTableName+ @"](ItemID,AttributeName,Uom,Value,Field1,Field2, Field3, Field4, Field5, EntityID,WarningMessage)
                                        Select  td.ItemID,td.AttributeName,td.Uom,td.Value,td.Field1,td.Field2,td.Field3,td.Field4,td.Field5,td.EntityID, '"+ Resources.ItemDoesNotExistWarningMessage + @"'
                                        FROM  [tempdb]..[" + tempTableName+ @"] td LEFT OUTER JOIN Sku s on s.ItemID = td.ItemID where s.ItemID  is NULL

                                       
                                        -- Take Out Missiing attributes
                                        INSERT into [tempdb]..[" + warningTableName+ @"](ItemID,AttributeName,Uom,Value,Field1,Field2, Field3, Field4, Field5, EntityID   ,WarningMessage)
                                        Select  td.ItemID,td.AttributeName,td.Uom,td.Value,td.Field1,td.Field2,td.Field3,td.Field4,td.Field5,td.EntityID, '"+ Resources.AttributeDoesNotExistWarningMessage+ @"'
                                        FROM [tempdb]..[" + tempTableName + @"] td LEFT OUTER JOIN Attribute a  on LOWER(a.AttributeName) = LOWER(td.AttributeName) AND a.AttributeType IN ('Sku','Global', 'Derived', 'Flag') 
                                        WHERE a.AttributeName  is NULL

                                        -- DELETE all the warning entries from temp table
                                        Delete td from [tempdb]..[" + tempTableName + @"] td inner join [tempdb]..["
                                                                        + warningTableName
                                                                        + @"] w on td.ItemID = w.ItemID and td.AttributeName = w.AttributeName
                                        -- Add a count here for ignored and delete this record
                                        DELETE td
                                        --select *
                                        from [tempdb]..[" + tempTableName
                                                                        + @"] td inner join V_AllSkuData  sd on sd.ItemID = td.ItemID
                                                                                                          AND sd.AttributeName = td.AttributeName
                                                                                                          AND ISNULL(sd.Uom,'') = ISNULL(td.Uom,'')
                                                                                                          AND ISNULL(sd.Value,'') =  ISNULL(td.Value,'')
                                                                                                          AND ISNULL(sd.Field1,'') = ISNULL(td.Field1,'')
                                                                                                          AND ISNULL(sd.Field2,'') = ISNULL(td.Field2,'')
                                                                                                          AND ISNULL(sd.Field3,'') = ISNULL(td.Field3,'')
                                                                                                          AND ISNULL(sd.Field4,'') = ISNULL(td.Field4,'')
                                                                                                          AND ISNULL(sd.Field5,'') = ISNULL(td.Field5,'')
                                                                                                          Where sd.Active = 1

                                       -- SET @IgnoredCount = @@ROWCOUNT
                                        SET @ResultText += '" + Resources.IgnoredRecordCountIdentifierText
                                                                        + @"'+ '=' + CAST(@@ROWCOUNT AS VARCHAR(50)) + ';';

                                        --UPDATE Skuid and attribute id
                                        UPDATE [tempdb]..[" + tempTableName + @"]
                                        SET SkuId = s.ID, AttributeId = a.ID
                                        FROM [tempdb]..[" + tempTableName + @"] td Inner join sku s on s.ItemID = td.ItemID inner join Attribute a on LOWER(a.AttributeName) = LOWER(td.AttributeName) AND a.AttributeType IN ('Sku','Global', 'Derived', 'Flag') 

                                        --Update Entity info and Entity Data
                                        UPDATE [tempdb]..[" + tempTableName + @"]
                                        SET EntityInfoId = vasd.EntityId, EntityDataId = vasd.EntityDataId,
                                        DbField1 = vasd.Field1, DbField2 = vasd.Field2, DbField3 = vasd.Field3, DbField4 = vasd.Field4, DbField5 = vasd.Field5 
                                        FROM [tempdb]..[" + tempTableName+ @"] td inner join V_ActiveSkuData vasd on vasd.SkuID = td.SkuId and vasd.AttributeID = td.AttributeId

                                        IF @MarkAsBeforeEntity = 1
                                        BEGIN
                                        UPDATE EntityData
                                        SET BeforeEntity = 0 FROM EntityData ed inner join  [tempdb]..[" + tempTableName+ @"] td on td.EntityDataId = ed.ID
                                        END

                                        IF @CreateMissingValues = 0
                                        BEGIN
                                            --Delete all the item the are not in the db from temp so that the update will work.
                                            DELETE td from [tempdb]..[" + tempTableName + @"] td where EntityInfoId is null
                                        END

                                        --Update all the existing entitydata set to 0
                                        UPDATE EntityData
                                        SET Active = 0,DeletedBy = @UserID,DeletedOn = GETDATE(),DeletedRemark = @CurrentRemarkId
                                        From EntityData ed inner join [tempdb]..[" + tempTableName+ @"] td on td.EntityDataId = ed.ID
                                        WHERE ed.Active = 1

                                        SET @ResultText += '" + Resources.UpdatedRecordCountIdentifierText
                                                                        + @"'+ '=' +  CAST(@@ROWCOUNT AS VARCHAR(50)) + ';';

                                        --Update entity info in import data table
                                        UPDATE [tempdb]..[" + tempTableName + @"]
                                        SET EntityInfoId = NEWID()
                                        where [tempdb]..[" + tempTableName + @"].EntityDataId is  null

                                        --Insert new entity info
                                        Insert Into EntityInfo(ID, SkuID)
                                        Select td.EntityInfoId, td.SkuId
                                        FROM [tempdb]..[" + tempTableName + @"] td where td.EntityDataId  is null

                                        SET @ResultText += '" + Resources.NewRecordCountIdentifierText
                                                                        + @"'+ '=' + CAST(@@ROWCOUNT AS VARCHAR(50)) ;

                                        IF @MarkAsBeforeEntity = 1
                                        BEGIN
                                            -- Insert all the record in entity data
                                        Insert Into EntityData(ID, [AttributeID],[Value],[Uom],[Field1],[Field2],[Field3],[Field4],[Field5],[CreatedOn],[CreatedBy],[CreatedRemark],[Active],BeforeEntity, EntityID)
                                        select  NEWID(), td.AttributeId, td.Value, td.Uom, ISNULL(td.[Field1], td.DbField1), ISNULL(td.[Field2], td.DbField2), ISNULL(td.[Field3], td.DbField3), 
                                        ISNULL(td.[Field4], td.DbField4), ISNULL(td.[Field5], td.DbField5), GETDATE(), @UserID, @CurrentRemarkId,1,1, td.EntityInfoId
                                        FROM  [tempdb]..[" + tempTableName + @"] td
                                        END
                                        ELSE
                                        BEGIN
                                                -- Insert all the record in entity data
                                                Insert Into EntityData(ID, [AttributeID],[Value],[Uom],[Field1],[Field2],[Field3],[Field4],[Field5],[CreatedOn],[CreatedBy],[CreatedRemark],[Active], EntityID)
                                                select  NEWID(), td.AttributeId, td.Value, td.Uom, ISNULL(td.[Field1], td.DbField1), ISNULL(td.[Field2], td.DbField2), ISNULL(td.[Field3], td.DbField3), 
                                                ISNULL(td.[Field4], td.DbField4), ISNULL(td.[Field5], td.DbField5), GETDATE(), @UserID, @CurrentRemarkId,1, td.EntityInfoId
                                                FROM  [tempdb]..[" + tempTableName + @"] td
                                        END

                                        Select @ResultText
                                        ");

                    _queryResults = CurrentDbContext.ExecuteQuery<string>(singleValueProcessorQueryString).Single();

                    CurrentLogWriter.DebugFormat("{0}: Fetching warnings",Arguments.Id);
                    var warningRecords =
                        CurrentDbContext.ExecuteQuery<string>(
                            @"SELECT  war.ItemID + char(9) + war.AttributeName + char(9) +
                                                                                   ISNULL(war.Uom,'')  + char(9) + ISNULL(war.Value,'')  + char(9) +
                                                                                   ISNULL(war.Field1,'')  + char(9) + ISNULL(war.Field2,'') + char(9) +
                                                                                   ISNULL(war.Field3,'') + char(9) + ISNULL(war.Field4,'') + char(9) +
                                                                                   ISNULL(war.Field5,'') + char(9) +
                                                                                   ISNULL(war.EntityID,'')  + char(9) + war.WarningMessage

                                                                            FROM[tempdb]..[" + warningTableName
                            + @"] war
                                                                            ").ToList();
                    //var warningRecords =
                    //    CurrentDbContext.ExecuteQuery<string>("select * from [tempdb]..[" + warningTableName + @"]").ToList();

                    CurrentDbContext.ExecuteCommand(@"IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL
                                                        DROP TABLE [tempdb]..[" + warningTableName + @"]");

                    CurrentLogWriter.DebugFormat("{0}: {1} warnings",Arguments.Id, warningRecords.Count);
                    foreach (var warningRecord in warningRecords)
                    {
                        _warnings.Add(new WorkerWarning
                                      {
                                          LineData =
                                              warningRecord.Substring(0,
                                                  warningRecord.LastIndexOf('\t')),
                                          ErrorMessage =
                                              warningRecord.Substring(warningRecord.LastIndexOf('\t')
                                                                      + 1)
                                      });
                    }

                    CurrentLogWriter.DebugFormat("{0}: Processing Summary Report",Arguments.Id);
                    ProcessSummaryReport(_queryResults);
                    CurrentDbContext.ExecuteCommand(deleteTempTableScript);
                }
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

        private bool FieldsInFileAndDbFieldsMatches(IEnumerable<EntityData> query,
            SkuAttributeValueInterchangeRecord record)
        {
            var ed = query.First();
            if ((record.Field1 ?? string.Empty) != (ed.Field1 ?? string.Empty)
                || (record.Field2 ?? string.Empty) != (ed.Field2 ?? string.Empty)
                || (record.Field3 ?? string.Empty) != (ed.Field3 ?? string.Empty)
                || (record.Field4 ?? string.Empty) != (ed.Field4 ?? string.Empty)
                || (record.Field5 ?? string.Empty) != (ed.Field5 ?? string.Empty))
                return false;
            return true;
        }

        private Tuple<string, string, string> GetGroup(object dataItem, bool attPlusUom)
        {
            var type = dataItem.GetType();

            return new Tuple<string, string, string>(type.GetProperty("ItemID").GetValue(dataItem) as string,
                type.GetProperty("AttributeName").GetValue(dataItem) as string,
                attPlusUom ? type.GetProperty("Uom").GetValue(dataItem) as string : null);
        }

        //This should be handles in the invalid value seperations
        //private void ReportNullAttributeValues(List<SkuAttributeValueInterchangeRecord> validImportRecords)
        //{
        //    var nullAttributeValueRecords = validImportRecords.Where(ad => ad.Value == null).ToList();
        //    nullAttributeValueRecords.ForEach(vr =>
        //                                          {
        //                                              _warnings.Add(new WorkerWarning
        //                                                                {
        //                                                                    LineData = vr.ToString(),
        //                                                                    ErrorMessage =
        //                                                                        "Attribute Value can not be null."
        //                                                                });
        //                                              validImportRecords.Remove(vr);
        //                                          });
        //}
        private void ProcessMultivalues(IEnumerable<SkuAttributeValueInterchangeRecord> multiValues)
        {
            CurrentLogWriter.DebugFormat("{0}: Processing Multi-Value records", Arguments.Id);
            var existingEntityInfos = new List<EntityInfo>();

            var results = multiValues.GroupBy(mv => new {mv.ItemID, mv.AttributeName},
                (key, group) => new {key.ItemID, key.AttributeName, Values = group.ToList()});
            foreach (var fileGroup in results)
            {
                var line = fileGroup;
                var dbGroup = (from s in CurrentDbContext.Skus
                    where s.ItemID == line.ItemID
                    from ei in s.EntityInfos
                    from ed in ei.EntityDatas
                    where ed.Active && ed.Attribute.AttributeName.ToLower() == line.AttributeName.ToLower()
                    select ed).ToList();

                //if its an exact match on value+Uom
                var exactMatches = from fileLine in fileGroup.Values
                    join dbLine in dbGroup on new {fileLine.Value, fileLine.Uom} equals new {dbLine.Value, dbLine.Uom}
                    let ln = new {fileLine, dbLine}
                    group ln by ln.fileLine
                    into grp
                    select new {FileLine = grp.Key, DbLines = grp.Select(g => g.dbLine)};

                foreach (var exactMatch in exactMatches)
                {
                    //Processing general updates
                    if (!FieldsInFileAndDbFieldsMatches(exactMatch.DbLines, exactMatch.FileLine))
                    {
                        exactMatch.DbLines.ForEach(e => e.Active = false);
                        var ed = exactMatch.DbLines.First();
                        ed.EntityInfo.EntityDatas.Add(new EntityData(CurrentDbContext)
                                                      {
                                                          Attribute = ed.Attribute,
                                                          Value = ed.Value,
                                                          Uom = ed.Uom,
                                                          Field1 = exactMatch.FileLine.Field1,
                                                          Field2 = exactMatch.FileLine.Field2,
                                                          Field3 = exactMatch.FileLine.Field3,
                                                          Field4 = exactMatch.FileLine.Field4,
                                                          Field5 = exactMatch.FileLine.Field5,
                                                          BeforeEntity = false,
                                                          CreatedRemark = CurrentRemarkId
                                                      });
                    }
                    //remove the item from the eds
                    dbGroup.RemoveAll(exactMatch.DbLines);
                    //TODO: remove from file list??
                    _ignoredCountMultiValues++;
                    fileGroup.Values.Remove(exactMatch.FileLine);
                }

                //case 1
                if (_replaceExistingValues)
                {
                    //add to the entity info
                    existingEntityInfos = dbGroup.OrderBy(e => e.Value).Select(e => e.EntityInfo).ToList();
                    foreach (var entityData in dbGroup)
                    {
                        //deactivate all in dbs
                        entityData.Active = false;
                    }
                }
                if (existingEntityInfos.Count == 0 && !CurrentImportOptions.HasFlag(ImportOptions.CreateMissingValues))
                    continue;
                //case 2
                // var currentEntityInfoIndex = 0;
                foreach (var itemFromFile in fileGroup.Values)
                {
                    //create new entity data
                    if(itemFromFile == null)
                        continue;

                    EntityInfo currentEntityInfo;
                    if (existingEntityInfos.Count != 0)
                        currentEntityInfo = existingEntityInfos.First();
                    else
                    {
                        var currentSku = CurrentDbContext.Skus
                                                    .FirstOrDefault(
                                                        s =>
                                                    s.ItemID == itemFromFile.ItemID);
                        if (currentSku == null)
                        {
                            _warnings.Add(new WorkerWarning
                            {
                                LineData = itemFromFile.ToString(),
                                ErrorMessage = Resources.ItemDoesNotExistWarningMessage// Resources.AttributeDoesNotExistWarningMessage
                            });
                            continue;
                        }
                        else
                        {
                            currentEntityInfo = new EntityInfo(CurrentDbContext)
                            {
                                Sku = currentSku
                            };
                            CurrentDbContext.EntityInfos.InsertOnSubmit(currentEntityInfo);
                        }
                       
                        
                    }
                    var attributeFromName = Attribute.GetAttributeFromName(CurrentDbContext, itemFromFile.AttributeName, false, useChache:false);

                    if (attributeFromName == null)
                    {
                        _warnings.Add(new WorkerWarning
                                      {
                                          LineData = itemFromFile.ToString(),
                                          ErrorMessage = Resources.AttributeDoesNotExistWarningMessage
                                      });
                        continue;
                    }

                    var attId = attributeFromName.ID;
                    currentEntityInfo.EntityDatas.Add(new EntityData(CurrentDbContext)
                                                      {
                                                          AttributeID = attId,
                                                          Value = itemFromFile.Value,
                                                          Uom = itemFromFile.Uom,
                                                          Field1 = itemFromFile.Field1,
                                                          Field2 = itemFromFile.Field2,
                                                          Field3 = itemFromFile.Field3,
                                                          Field4 = itemFromFile.Field4,
                                                          Field5 = itemFromFile.Field5,
                                                          BeforeEntity = false,
                                                          CreatedRemark = CurrentRemarkId
                                                      });

                    _successCountForMultiValues++;
                    if (existingEntityInfos.Contains(currentEntityInfo))
                        existingEntityInfos.Remove(currentEntityInfo);
                }
            }
            SaveDataChanges();
        }
       
        private void ProcessRecordWithEntityData(IEnumerable<SkuAttributeValueInterchangeRecord> recordsWithEntityInfo)
        {
            //TODO: IMPORTANT same value in file and db is being treated as different values. that means even if same record exists new entity data is created
            SkuAttributeValueInterchangeRecord[] skuAttributeValueInterchangeRecords = recordsWithEntityInfo as SkuAttributeValueInterchangeRecord[] ?? recordsWithEntityInfo.ToArray();
            //skuAttributeValueInterchangeRecords.RemoveAll(item => skuAttributeValueInterchangeRecords.Any()));
            var duplicateValueEntityIds = skuAttributeValueInterchangeRecords.GroupBy(s => s.EntityID).SelectMany(grp => grp.Skip(1)).Select(s=>s.EntityID).Distinct();
            var valueEntityIds = duplicateValueEntityIds as Guid?[] ?? duplicateValueEntityIds.ToArray();
            foreach (var warningRecord in valueEntityIds)
            {
                _warnings.Add(new WorkerWarning
                {
                    LineData = warningRecord.ToString(),
                    ErrorMessage = Properties.Resources.DuplicateEntityIdWarningMessage

                });
            }

            skuAttributeValueInterchangeRecords = skuAttributeValueInterchangeRecords.Where(sk => !valueEntityIds.Contains(sk.EntityID)).ToArray();
           // var newList = dup.Where(d => skuAttributeValueInterchangeRecords.Contains(d.EntityID))

            var sqlTempTableHelper = new SqlHelper(typeof (SkuAttributeValueInterchangeRecord));
            var tempEntityInfoRecord = TempExistingEntityInfoTablePrefix + Guid.NewGuid();
            var warningTableName = tempEntityInfoRecord + "_warning";
            var createTempTableScript = sqlTempTableHelper.CreateTableScript(tempEntityInfoRecord, "tempdb");
            var deleteTempTableScript = sqlTempTableHelper.DeleteTableScript(tempEntityInfoRecord, "tempdb");
            //create the temp table.
            CurrentDbContext.ExecuteCommand(createTempTableScript);
            var markAsBeforeEntity = CurrentImportOptions.HasFlag(ImportOptions.MarkAsBeforeEntity);

            CurrentLogWriter.DebugFormat("{0}: Inserting Records with EntityInfo",Arguments.Id);
            CurrentDbContext.BulkInsertAll(skuAttributeValueInterchangeRecords, tempEntityInfoRecord, "tempdb");

            CurrentLogWriter.DebugFormat("{0}: Processing Records with EntityInfo",Arguments.Id);
            var entityInfoValueProcessorQueryString = @"DECLARE @UserID AS UNIQUEIDENTIFIER
                                                    DECLARE @ProjectID AS UNIQUEIDENTIFIER
                                                    DECLARE @ResultText AS VARCHAR(2000)
                                                    DECLARE @DefaultRemark AS UNIQUEIDENTIFIER
                                                    DECLARE @NewDataCount AS int
                                                    DECLARE @IgnoredCount AS int
                                                    DECLARE @AttributeUomUnique AS bit
                                                    DECLARE @CurrentRemarkId AS UNIQUEIDENTIFIER
                                                    Declare @MarkAsBeforeEntity as bit
                                                    SET @MarkAsBeforeEntity = '" + markAsBeforeEntity + @"'

                                                    SET @UserID = '" + ImportRequestedBy + @"'
                                                    SET @ProjectID = '" + CurrentProjectId + @"'

                                                    IF OBJECT_ID('tempdb..#NewEntityDatas') IS NOT NULL
                                                    DROP TABLE #NewEntityDatas

                                                    IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL
                                                    DROP TABLE [tempdb]..[" + warningTableName + @"]

                                                    CREATE TABLE [tempdb]..[" + warningTableName + @"]
                                                    (
                                                        ItemID varchar(255),
                                                        AttributeName varchar(255),
                                                        Uom varchar(255),
                                                        Value varchar(255),
                                                        Field1 varchar(255),
                                                        Field2 varchar(255),
                                                        Field3 varchar(255),
                                                        Field4 varchar(255),
                                                        Field5 varchar(255),
                                                        EntityID varchar(255),
                                                        WarningMessage varchar(255)
                                                    );

                                                    SET @IgnoredCount = 0
                                                    SET @NewDataCount = 0
                                                    SET @ResultText = ''
                                                    SET @AttributeUomUnique = 1

                                                    Select
                                                    sde.ItemID
                                                    ,sde.[AttributeName]
                                                    ,sde.[Uom]
                                                    ,sde.[value]
                                                    ,sde.[Field1]
                                                    ,sde.[Field2]
                                                    ,sde.[Field3]
                                                    ,sde.[Field4]
                                                    ,sde.[Field5]
                                                    ,sde.[EntityID]
                                                    ,sd.[EntityID] As ExistingEntityID
                                                    ,sd.ItemID As ExistingItemId
                                                    ,sd.[AttributeName] As ExistingAttributeName
                                                    ,sd.[Uom] As ExistingUom
                                                    ,sd.[value] As ExistingValue
                                                    ,sd.[Field1] As ExistingField1
                                                    ,sd.[Field2] As ExistingField2
                                                    ,sd.[Field3] As ExistingField3
                                                    ,sd.[Field4] As ExistingField4
                                                    ,sd.[Field5] As ExistingField5
                                                    ,a.ID as AttributeID

                                                    INTO #NewEntityDatas
                                                    FROM  [tempdb]..[" + tempEntityInfoRecord + @"] sde
                                                    LEFT OUTER JOIN V_ActiveSkuData sd ON sd.EntityID = sde.[EntityID] LEFT OUTER JOIN Attribute a ON LOWER(a.AttributeName) =  LOWER(sde.AttributeName) AND a.AttributeType IN ('Sku','Global', 'Derived', 'Flag') 

                                                    --select * from #NewEntityDatas

                                                    INSERT into [tempdb]..[" + warningTableName
                                                      + @"](ItemID,AttributeName,Uom,Value,Field1,Field2, Field3, Field4, Field5, EntityID,WarningMessage)
                                                    SELECT  nd.ItemID,nd.AttributeName,nd.Uom,nd.Value,nd.Field1,nd.Field2,nd.Field3,nd.Field4,nd.Field5,nd.EntityID, '"
                                                      + Resources.AttributeDoesNotExistWarningMessage + @"'
                                                    FROM #NewEntityDatas nd where nd.AttributeID is NULL

                                                    INSERT into [tempdb]..[" + warningTableName
                                                      + @"](ItemID,AttributeName,Uom,Value,Field1,Field2, Field3, Field4, Field5,EntityID,WarningMessage)
                                                    SELECT  nd.ItemID,nd.AttributeName,nd.Uom,nd.Value,nd.Field1,nd.Field2,nd.Field3,nd.Field4,nd.Field5,nd.EntityID,'"
                                                      + Resources.EntityInfoIDDoesNotExistWarningMessage + @"'
                                                    FROM #NewEntityDatas nd where nd.ExistingEntityID is NULL

                                                    DELETE nd FROM #NewEntityDatas nd INNER JOIN [tempdb]..["
                                                      + warningTableName + @"] w ON w.EntityID = nd.EntityID

                                                    --SELECT * FROM #NewEntityDatas

                                                    --UPDATE Entity Datas
                                                    UPDATE EntityData
                                                    SET Active = 0,DeletedOn = GETDATE(),DeletedBy = @UserID, DeletedRemark = @CurrentRemarkId
                                                    FROM EntityData ed inner join #NewEntityDatas nd ON nd.EntityID = ed.EntityID
                                                    WHERE ed.Active = 1
                                                    --Insert New Entity DATA

                                                    IF @MarkAsBeforeEntity = 1
                                                    BEGIN
                                                    INSERT INTO EntityData(ID, [AttributeID],[Value],[Uom],[Field1],[Field2],[Field3],[Field4],[Field5],[CreatedOn],[CreatedBy],[CreatedRemark],[Active],[BeforeEntity], EntityID)
                                                    SELECT  NEWID(), td.AttributeId, td.Value, td.Uom,td.Field1, td.[Field2],td.[Field3],td.[Field4],td.[Field5], GETDATE(), @UserID, @CurrentRemarkId,1, 1, td.EntityID
                                                    FROM  #NewEntityDatas td
                                                    END
                                                    ELSE
                                                    BEGIN
                                                    INSERT INTO EntityData(ID, [AttributeID],[Value],[Uom],[Field1],[Field2],[Field3],[Field4],[Field5],[CreatedOn],[CreatedBy],[CreatedRemark],[Active],[BeforeEntity], EntityID)
                                                    SELECT  NEWID(), td.AttributeId, td.Value, td.Uom,td.Field1, td.[Field2],td.[Field3],td.[Field4],td.[Field5], GETDATE(), @UserID, @CurrentRemarkId,1, 0, td.EntityID
                                                    FROM  #NewEntityDatas td
                                                    END
                                                    --SET @ResultText += '" + Resources.NewRecordCountIdentifierText
                                                      + @"'+ '=' + CAST(@@ROWCOUNT AS VARCHAR(50)) ;

                                                    SELECT @@ROWCOUNT
                                                    ";

            var queryResults = CurrentDbContext.ExecuteQuery<int>(entityInfoValueProcessorQueryString).Single();
            _successCountForEntityInfo = queryResults;
            CurrentDbContext.ExecuteCommand(deleteTempTableScript);

            var warningRecords =
                CurrentDbContext.ExecuteQuery<string>(@"SELECT  war.ItemID + char(9) + war.AttributeName + char(9) +
                                                                                   ISNULL(war.Uom,'')  + char(9) + ISNULL(war.Value,'')  + char(9) +
                                                                                   ISNULL(war.Field1,'')  + char(9) + ISNULL(war.Field2,'') + char(9) +
                                                                                   ISNULL(war.Field3,'') + char(9) + ISNULL(war.Field4,'') + char(9) +
                                                                                   ISNULL(war.Field5,'') + char(9) +
                                                                                   ISNULL(war.EntityID,'')  + char(9) + war.WarningMessage

                                                                            FROM [tempdb]..[" + warningTableName
                                                      + @"] war").ToList();

            CurrentDbContext.ExecuteCommand(@"IF OBJECT_ID('[tempdb]..[" + warningTableName + @"]') IS NOT NULL
                                                        DROP TABLE [tempdb]..[" + warningTableName + @"]");

            CurrentLogWriter.DebugFormat("{0}: {1} Warnings", Arguments.Id, warningRecords.Count);
            foreach (var warningRecord in warningRecords)
            {
                _warnings.Add(new WorkerWarning
                              {
                                  LineData = warningRecord.Substring(0, warningRecord.LastIndexOf('\t')),
                                  ErrorMessage =
                                      warningRecord.Substring(warningRecord.LastIndexOf('\t') + 1)
                              });
            }
        }

        private void ProcessSummaryReport(string queryResults)
        {
            var reportItems = queryResults.Split(';');
            var summeryReportDetails = new List<CustomKeyValuePair<string, int>>();
            foreach (var reportItem in reportItems)
            {
                int itemCount = Int32.Parse(reportItem.Substring(reportItem.IndexOf('=') + 1).Trim());
                if (reportItem.Contains(Resources.NewRecordCountIdentifierText))
                {
                    summeryReportDetails.Add(new CustomKeyValuePair<string, int>(
                        Resources.NewRecordCountIdentifierText, itemCount));
                }
                else if (reportItem.Contains(Resources.UpdatedRecordCountIdentifierText))
                {
                    summeryReportDetails.Add(
                        new CustomKeyValuePair<string, int>(Resources.UpdatedRecordCountIdentifierText,
                            itemCount + _successCountForEntityInfo + _successCountForMultiValues));
                }
                else if (reportItem.Contains(Resources.IgnoredRecordCountIdentifierText))
                {
                    summeryReportDetails.Add(
                        new CustomKeyValuePair<string, int>(Resources.IgnoredRecordCountIdentifierText,
                            itemCount + _ignoredCountMultiValues));
                }
            }
            Summary.Details = summeryReportDetails;
            // Summary.TotalLine = allRecordCount;
            if (_warnings.Count != 0)
            {
                Summary.Warnings = _warnings;
                Summary.State = WorkerState.CompletedWithWarning;
                return;
            }
            Summary.State = WorkerState.Complete;
        }

        #endregion Methods
    }
}