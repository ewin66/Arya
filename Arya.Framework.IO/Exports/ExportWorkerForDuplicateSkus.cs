using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using LinqKit;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"Duplicate SKUs identification")]
    public sealed class ExportWorkerForDuplicateSkus : ExportWorkerBase
    {
        #region Fields

        private const string PotentialDuplicateSkusTableName = "PotentialDuplicateSkus";

        private readonly Dictionary<string, int> _itemGroups = new Dictionary<string, int>();

        private ExportArgs _args;
        private DataTable _duplicateSkusTable;

        #endregion Fields

        #region Constructors

        public ExportWorkerForDuplicateSkus(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof(ExportArgs))
        {
        }

        #endregion Constructors

        #region Properties

        private DataTable DuplicateSkusTable
        {
            get { return _duplicateSkusTable ?? (_duplicateSkusTable = InitDuplicateSkusTable()); }
        }

        #endregion Properties

        #region Methods

        protected override void FetchExportData()
        {
            CurrentLogWriter.Info("Fetching SKUs");
            _args = (ExportArgs)Arguments;
            List<Guid> allTaxIds;
            using (var dc = new AryaDbDataContext(_args.ProjectId, _args.UserId))
            {
                var taxIds = (from ti in dc.TaxonomyInfos where _args.TaxonomyIds.Contains(ti.ID) select ti).ToList();
                allTaxIds = taxIds.SelectMany(ti => ti.AllChildren).Select(ti => ti.ID).ToList();
            }
            _iNodeCount = allTaxIds.Count;
            allTaxIds.ForEach(ProcessNode);
            //allTaxIds.AsParallel().ForAll(ProcessNode);
            //_args.ItemIds.AsParallel().ForAll(sourceItemId => ProcessItem(sourceItemId));

            CurrentLogWriter.Info("Saving Results");
            ExportDataTables.Add(DuplicateSkusTable);
        }

        private int _iNodeCount;
        private int _iNodeCtr;
        private void ProcessNode(Guid taxId)
        {
            Interlocked.Increment(ref _iNodeCtr);
            CurrentLogWriter.InfoFormat("Processing Nodes: {0} of {1}", _iNodeCtr, _iNodeCount);
            try
            {
                var itemValues = GetItemValues(taxId);
                foreach (var sourceItem in itemValues)
                {
                    var duplicateItems = (from targetItem in itemValues
                                          where targetItem.ItemId != sourceItem.ItemId
                                          let result = ProcessItemPair(sourceItem, targetItem)
                                          where result != null
                                          orderby result.MatchPercent descending
                                          select result).FirstOrDefault();

                    if (duplicateItems != null)
                        AddRowToTable(duplicateItems);
                }
            }
            catch (Exception exception)
            {
                var message = string.Empty;
                var ex = exception;
                while (ex != null)
                {
                    message += ex.Message + Environment.NewLine;
                    message += ex.StackTrace + Environment.NewLine;
                    ex = ex.InnerException;
                }
                CurrentLogWriter.Warn("There was a problem processing skus in node " + taxId + Environment.NewLine
                                      + message);
            }
        }

        private List<ItemInfo> GetItemValues(Guid taxId)
        {
            using (var dc = new AryaDbDataContext(_args.ProjectId, _args.UserId))
            {
                var taxonomy = dc.TaxonomyInfos.Where(ti => ti.ID == taxId).First();

                var attributes = (from ti in dc.TaxonomyInfos
                                  where ti.ID == taxId
                                  from si in ti.SchemaInfos
                                  from sd in si.SchemaDatas
                                  where sd.Active && sd.InSchema
                                  //let navOrder = sd.NavigationOrder == 0 ? 999 : sd.NavigationOrder
                                  //let dispOrder = sd.DisplayOrder == 0 ? 999 : sd.DisplayOrder
                                  //orderby navOrder, dispOrder
                                  select si.Attribute.AttributeName).Distinct().ToList();

                var values = (from si in dc.SkuInfos
                              where si.Active && si.TaxonomyID == taxId
                              from ed in dc.EntityDatas
                              let sku = ed.EntityInfo.Sku
                              let attributeName = ed.Attribute.AttributeName
                              where ed.Active && sku.ID == si.SkuID && attributes.Contains(attributeName)
                              select new { SkuID = sku.ID, sku.ItemID, AttributeName = attributeName, ed.Value, ed.Uom }).ToList();

                var itemValues = from val in values
                                 group val by val.ItemID
                                     into grp
                                     select
                                         new ItemInfo
                                         {
                                             SkuId = grp.First().SkuID,
                                             ItemId = grp.First().ItemID,
                                             TaxonomyId = taxonomy.ID,
                                             TaxonomyPath = taxonomy.ToString(_args.IgnoreT1Taxonomy),
                                             SchemaValues = (from attVal in grp
                                                             group attVal by attVal.AttributeName
                                                                 into attGrp
                                                                 select
                                                                 new
                                                                 {
                                                                     Attribute = attGrp.Key,
                                                                     Values =
                                                                 grp.Select(val => (val.Value + " " + (val.Uom ?? string.Empty)).Trim())
                                                                 .ToList()
                                                                 }).ToDictionary(av => av.Attribute, av => av.Values)
                                         };

                return itemValues.ToList();
            }
        }

        //int _iCtr;
        //private int ProcessItem(string sourceItemId)
        //{
        //    if (Interlocked.Increment(ref _iCtr) % 100 == 0)
        //        CurrentLogWriter.InfoFormat("Processing SKUs: {0} of {1}", _iCtr, _args.ItemIds.Length);

        //    try
        //    {
        //        var sourceSchemaValues = GetSchemaValues(sourceItemId);
        //        var sourceAttributes = sourceSchemaValues.SchemaValues.Select(ssv => ssv.Key).ToList();

        //        var otherItems = GetSiblings(sourceItemId);
        //        var duplicateItems = (from targetItemId in otherItems
        //                              where targetItemId != sourceItemId
        //                              let targetSchemaValues = GetSchemaValues(targetItemId, sourceAttributes)
        //                              let result = ProcessItemPair(sourceSchemaValues, targetSchemaValues)
        //                              where result != null
        //                              orderby result.MatchPercent descending
        //                              select result).FirstOrDefault();

        //        if (duplicateItems != null)
        //            AddRowToTable(duplicateItems);
        //    }
        //    catch (Exception exception)
        //    {
        //        var message = string.Empty;
        //        var ex = exception;
        //        while (ex != null)
        //        {
        //            message += ex.Message + Environment.NewLine;
        //            message += ex.StackTrace + Environment.NewLine;
        //            ex = ex.InnerException;
        //        }
        //        CurrentLogWriter.Warn("There was a problem processing skus in node." + Environment.NewLine + message);
        //    }
        //    return _iCtr;
        //}

        //private IEnumerable<string> GetSiblings(string sourceItemId)
        //{
        //    using (var dc = new AryaDbDataContext(_args.ProjectId, _args.UserId))
        //    {
        //        var taxId =
        //            (from sku in dc.Skus
        //             where sku.ItemID == sourceItemId
        //             from si in sku.SkuInfos
        //             where si.Active
        //             select si.TaxonomyID).FirstOrDefault();

        //        var otherSkus =
        //            (from otherSku in dc.SkuInfos where otherSku.TaxonomyID == taxId select otherSku.Sku.ItemID).ToList();

        //        return otherSkus;
        //    }
        //}

        private static string ConcatenateAttributeValues(IEnumerable<KeyValuePair<string, List<string>>> schemaValues)
        {
            var svs = schemaValues.Where(sv => sv.Value.Any()).ToList();
            return svs.Any()
                ? svs.Select(ssv => ssv.Key + ": " + ssv.Value.Aggregate((c, n) => c + ", " + n))
                    .Aggregate((c, n) => c + "; " + n)
                : string.Empty;
        }

        private static DataTable InitDuplicateSkusTable()
        {
            var dataTable = new DataTable(PotentialDuplicateSkusTableName);
            dataTable.Columns.AddRange(new[]
                                       {
                                           new DataColumn("Group Number"), 
                                           new DataColumn("Match Percent"),
                                           new DataColumn("Match Score"),
                                           new DataColumn("Match Attribute Count"),
                                           new DataColumn("Common Attribute Values"),
                                           new DataColumn("Item A"),
                                           new DataColumn("A - Attribute Count"),
                                           new DataColumn("A - Value Count"), 
                                           new DataColumn("A - Attribute Values"),
                                           new DataColumn("Item B"),
                                           new DataColumn("B - Attribute Count"),
                                           new DataColumn("B - Value Count"),
                                           new DataColumn("B - Attribute Values")
                                       });
            return dataTable;
        }

        private void AddRowToTable(PotentialDuplicateSkus record)
        {
            var newRow = DuplicateSkusTable.NewRow();

            newRow["Group Number"] = record.GroupNumber;
            newRow["Match Percent"] = record.MatchPercent;
            newRow["Match Score"] = record.MatchScore;
            newRow["Match Attribute Count"] = record.MatchAttributeCount;
            newRow["Common Attribute Values"] = record.CommonAttributeValues;
            newRow["Item A"] = record.ItemA;
            newRow["A - Attribute Count"] = record.AAttributeCount;
            newRow["A - Value Count"] = record.AValueCount;
            newRow["A - Attribute Values"] = record.AAttributeValues;
            newRow["Item B"] = record.ItemB;
            newRow["B - Attribute Count"] = record.BAttributeCount;
            newRow["B - Value Count"] = record.BValueCount;
            newRow["B - Attribute Values"] = record.BAttributeValues;

            DuplicateSkusTable.Rows.Add(newRow);
        }

        private int GetGroupKey(string sourceItemId, string targetItemId)
        {
            var groupKey = String.Compare(sourceItemId, targetItemId, StringComparison.Ordinal) > 0
                ? targetItemId + "|" + sourceItemId
                : sourceItemId + "|" + targetItemId;

            var groupValue = _itemGroups.Count;
            if (_itemGroups.ContainsKey(groupKey))
                groupValue = _itemGroups[groupKey];
            else
                _itemGroups[groupKey] = groupValue;

            return groupValue;
        }

        //private ItemInfo GetSchemaValues(string sourceItemId,
        //    List<string> sourceAttributes = null)
        //{
        //    using (var dc = new AryaDbDataContext(_args.ProjectId, _args.UserId))
        //    {
        //        var sku = dc.Skus.Where(s => s.ItemID == sourceItemId).FirstOrDefault();
        //        if (sku == null)
        //            return null;

        //        var attributes = sourceAttributes ?? (from si in sku.Taxonomy.SchemaInfos
        //                                              from sd in si.SchemaDatas
        //                                              where sd.Active && sd.InSchema
        //                                              let navOrder = sd.NavigationOrder == 0 ? 999 : sd.NavigationOrder
        //                                              let dispOrder = sd.DisplayOrder == 0 ? 999 : sd.DisplayOrder
        //                                              orderby navOrder, dispOrder
        //                                              select si.Attribute.AttributeName).Distinct().ToList();

        //        var schemaValues = from att in attributes
        //                           let eds = sku.GetValuesForAttribute(dc, att, false)
        //                           let vals =
        //                               eds.Select(ed => ed.Value + (ed.Uom ?? string.Empty))
        //                                   .Where(val => !string.IsNullOrWhiteSpace(val))
        //                                   .ToList()
        //                           select new { att, vals };

        //        return new ItemInfo
        //               {
        //                   SkuId = sku.ID,
        //                   ItemId = sourceItemId,
        //                   TaxonomyId = sku.Taxonomy.ID,
        //                   TaxonomyPath = sku.Taxonomy.ToString(_args.IgnoreT1Taxonomy),
        //                   SchemaValues = schemaValues.ToDictionary(kvp => kvp.att, kvp => kvp.vals)
        //               };
        //    }
        //}

        private class ItemInfo
        {
            public string ItemId { get; set; }
            public Guid SkuId { get; set; }
            public Guid TaxonomyId { get; set; }
            public string TaxonomyPath { get; set; }
            public Dictionary<string, List<string>> SchemaValues { get; set; }
        }

        private PotentialDuplicateSkus ProcessItemPair(ItemInfo sourceItem, ItemInfo targetItem)
        {
            var sourceSchemaValues = sourceItem.SchemaValues;
            var targetSchemaValues = targetItem.SchemaValues;

            var attributes = sourceItem.SchemaValues.Keys;
            var attVals = (from att in attributes
                           let commonAttVals = sourceSchemaValues[att].Where(val => targetSchemaValues.ContainsKey(att) && targetSchemaValues[att].Contains(val)).ToList()
                           let localScore = sourceSchemaValues[att].Count(val => targetSchemaValues.ContainsKey(att) && targetSchemaValues[att].Contains(val))
                           let score = localScore == 0 ? 0 : localScore / ((double)sourceSchemaValues[att].Count)
                           select
                               new
                               {
                                   CommonAttVals = commonAttVals.Any()
                                       ? att + ": " + commonAttVals.Aggregate((c, n) => c + ", " + n)
                                       : string.Empty,
                                   IsMatch = score > 0.0 ? 1 : 0,
                                   Score = score
                               }).ToList();

            var totalScore = attVals.Select(av => av.Score).Sum();

            if (totalScore <= 0)
                return null;

            var commAttVals =
                attVals.Where(av => !string.IsNullOrWhiteSpace(av.CommonAttVals))
                    .Select(av => av.CommonAttVals)
                    .ToList();
            var commonAttributeValues = commAttVals.Any()
                ? commAttVals.Aggregate((c, n) => c + "; " + n)
                : string.Empty;

            var avalCount = sourceSchemaValues.Count(ssv => ssv.Value.Count > 0);
            return new PotentialDuplicateSkus
                   {
                       GroupNumber =
                           GetGroupKey(sourceItem.ItemId, targetItem.ItemId).ToString("D"),
                       MatchPercent = (totalScore / avalCount).ToString("F"),
                       MatchScore = totalScore.ToString("F"),
                       MatchAttributeCount =
                           attVals.Select(av => av.IsMatch).Sum().ToString("D"),
                       CommonAttributeValues = commonAttributeValues,
                       ItemA = sourceItem.ItemId,
                       AAttributeCount = sourceSchemaValues.Count.ToString("D"),
                       AValueCount = avalCount.ToString("D"),
                       AAttributeValues = ConcatenateAttributeValues(sourceSchemaValues),
                       ItemB = targetItem.ItemId,
                       BAttributeCount = targetSchemaValues.Count.ToString("D"),
                       BValueCount =
                           targetSchemaValues.Count(tsv => tsv.Value.Count > 0).ToString("D"),
                       BAttributeValues = ConcatenateAttributeValues(targetSchemaValues)
                   };
        }

        #endregion Methods

        #region Nested Types

        private class PotentialDuplicateSkus
        {
            #region Properties

            public string AAttributeCount { get; set; }

            public string AAttributeValues { get; set; }

            public string AValueCount { get; set; }

            public string BAttributeCount { get; set; }

            public string BAttributeValues { get; set; }

            public string BValueCount { get; set; }

            public string CommonAttributeValues { get; set; }

            public string GroupNumber { get; set; }

            public string ItemA { get; set; }

            public string ItemB { get; set; }

            public string MatchAttributeCount { get; set; }

            public string MatchPercent { get; set; }

            public string MatchScore { get; set; }

            #endregion Properties
        }

        #endregion Nested Types
    }
}