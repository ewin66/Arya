using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using TaxonomyData = Arya.Framework.Data.AryaDb.TaxonomyData;
using TaxonomyInfo = Arya.Framework.Data.AryaDb.TaxonomyInfo;

namespace Arya.Framework.IO.Imports
{
    [ImportOrder(1)]
    public class TaxonomyImportWorker : ImportWorkerBase
    {
        #region Private variables
        private readonly List<WorkerWarning> _warnings = new List<WorkerWarning>();
        private Dictionary<string, Guid> _existingTaxonomyPathsAndIDs;

        #endregion

        #region Constructor
        public TaxonomyImportWorker()
        {
            CurrentInterchangeRecordType = typeof (TaxonomyInterchangeRecord);
        }
        #endregion

        #region Override methods

        public override List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        public override void Run()
        {
            var missingTaxonomies = new List<TaxonomyInterchangeRecord>();
            //State = WorkerState.Working;
            try
            {
                //initialize the context
                using (
                    CurrentDbContext =
                    new AryaDbDataContext(CurrentProjectId, ImportRequestedBy))
                {
                    Summary.StatusMessage = "Reading input file";
                    //reads all the values into a list<T>, change this as its not very efficient and scalable.
                    List<TaxonomyInterchangeRecord> allImportData = ImportData.Taxonomies;
                    var invalidRecords = allImportData.GetInvalidRecords();
                    //var invalidRecords = ImportData.Taxonomies;
                    var taxonomyInterchangeRecords = invalidRecords as IList<TaxonomyInterchangeRecord> ?? invalidRecords.ToList();
                    taxonomyInterchangeRecords.ToList().ForEach(ir => _warnings.Add(new WorkerWarning
                    {
                        LineData = ir.ToString(),
                        ErrorMessage = Properties.Resources.RequiredValueNullWarningMessage
                    }));
                    var validImportRecords = allImportData.Except(taxonomyInterchangeRecords.ToList()).ToList();
                    validImportRecords = validImportRecords.Distinct(new TaxonomyInterchangeRecordComparer()).ToList();
                    Summary.StatusMessage = "Fetching existing taxonomies";                   
                    _existingTaxonomyPathsAndIDs =
                        CurrentDbContext.ExecuteQuery<TaxonomyPathAndId>(
                            @"SELECT TaxonomyPath, TaxonomyID
                                                FROM V_Taxonomy
                                                WHERE TaxonomyPath <> ''
                                                AND ProjectId = {0}", CurrentProjectId).
                            ToDictionary(key => key.TaxonomyPath.ToLower(), value => value.TaxonomyId,
                                         StringComparer.OrdinalIgnoreCase);

                    missingTaxonomies.AddRange(
                        validImportRecords.Where(
                            importRecord => !_existingTaxonomyPathsAndIDs.ContainsKey(importRecord.TaxonomyPath.ToLower())));

                    missingTaxonomies = missingTaxonomies.Distinct(new TaxonomyInterchangeRecordComparer()).ToList();
                    var newTaxonomyCount = 0;
                    if (missingTaxonomies.Count != 0 && !CurrentImportOptions.HasFlag(ImportOptions.CreateMissingTaxonomies))
                    {
                        _warnings.Add(new WorkerWarning
                                          {
                                              LineData = Properties.Resources.CreateMissingTaxonomiesFlagOffLineDataText,
                                              ErrorMessage = Properties.Resources.CreateMissingTaxonomiesFlagOffWarningMessage
                                          });
                        ProcessSummaryReport(0);
                        return;
                    }

                   // var existingTaxonomyInFile = GetExistingTaxonomyInFile(validImportRecords, _existingTaxonomyPathsAndIDs);
                    Summary.StatusMessage = "Identifying missing taxonomies";
                    foreach (var missingTaxonomy in missingTaxonomies)
                    {
                        if (CreateTaxonomy(missingTaxonomy))
                        {
                            newTaxonomyCount++;
                        }
                        else
                        {
                            _warnings.Add(new WorkerWarning
                                              {
                                                  LineData = missingTaxonomy.TaxonomyPath,
                                                  ErrorMessage = Properties.Resources.CreateNewTaxonomyFailWorningMessage
                                              });
                        }
                    }
                    SaveDataChanges();
                    ProcessSummaryReport( newTaxonomyCount);
                } //end of using
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
        private void ProcessSummaryReport(int newTaxonomyCount)
        {
            //Summary.TotalLine = allImportDataCount;
            
            Summary.Details = new List<CustomKeyValuePair<string, int>>
                                             {
                                                 new CustomKeyValuePair<string, int>(Properties.Resources.NewRecordCountIdentifierText,
                                                                                     newTaxonomyCount),
                                                 
                                             };
            Summary.State = WorkerState.Complete;
            if (_warnings.Count != 0)
            {
                Summary.Warnings = _warnings;
                Summary.State = WorkerState.CompletedWithWarning;
                return;
            }
            Summary.State = WorkerState.Complete;
        }

        private bool CreateTaxonomy(TaxonomyInterchangeRecord currentTaxonomyPath)
        {
            var isTaxonomyNew = false;
            
            //double check to make sure that this taxonomy doesn't already exist - should never be executed...
            if (currentTaxonomyPath == null
                || _existingTaxonomyPathsAndIDs.ContainsKey(currentTaxonomyPath.TaxonomyPath.ToLower()))
                return false;
            // split taxonomy into nodes
            var taxonomyParts = currentTaxonomyPath.TaxonomyPath.Split(new[] {'>'});

            var taxonomyPathPrefix = new StringBuilder();

            Guid? parentTaxonomyId = null;

            foreach (var taxNode in taxonomyParts)
            {
                // if this is the first node, don't add a delimiter 
                if (taxonomyPathPrefix.Length > 0)
                    taxonomyPathPrefix.Append(">");
                taxonomyPathPrefix.Append(taxNode);

                var tempTaxonomyPathPrefix = taxonomyPathPrefix.ToString();

                // check to see if the taxonomy's parent is present - if so, get its id.
                // we keep iterating down the branch until we find a space for the taxonomy.
                // keep going until the entire taxonomy string is accounted for.
                if (_existingTaxonomyPathsAndIDs.ContainsKey(tempTaxonomyPathPrefix.ToLower()))
                    parentTaxonomyId = _existingTaxonomyPathsAndIDs[tempTaxonomyPathPrefix.ToLower()];
                else
                {
                    var newTaxonomyInfo = new TaxonomyInfo(CurrentDbContext)
                                              {ProjectID = CurrentProjectId, ShowInTree = false, NodeType = TaxonomyInfo.NodeTypeRegular};
                    newTaxonomyInfo.TaxonomyDatas.Add(new TaxonomyData(CurrentDbContext)
                                                          {
                                                              TaxonomyID = newTaxonomyInfo.ID,
                                                              ParentTaxonomyID = parentTaxonomyId,
                                                              NodeName = taxNode,
                                                              CreatedBy = ImportRequestedBy,
                                                              CreatedOn = DateTime.Now,
                                                              CreatedRemark = CurrentRemarkId
                                                          });

                    CurrentDbContext.TaxonomyInfos.InsertOnSubmit(newTaxonomyInfo);

                    //add it to the cache for future reuse
                    _existingTaxonomyPathsAndIDs.Add(tempTaxonomyPathPrefix.ToLower(), newTaxonomyInfo.ID);
                    parentTaxonomyId = newTaxonomyInfo.ID;
                    isTaxonomyNew = true;
                }
            }
            return isTaxonomyNew;
        }

        #endregion Private Methods
    }
}