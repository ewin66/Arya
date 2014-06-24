using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Arya.Framework.Common;
using Arya.Framework.Data.AryaDb;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Updates
{
    public class DerivedAttributeValueUpdateWorker : WorkerBase
    {
        private int _iSkuCount;
        private int _iValueCount;
        public DerivedAttributeValueUpdateWorker(string argumentDirectoryPath) : base(argumentDirectoryPath, typeof (DerivedAttributeValueUpdateArguments))
        {
        }

        public override void Run()
        {
            try
            {
                //Start the presses...
                UpdateDerivedEntities();
            }
            catch (Exception ex)
            {
                Summary.SetError(ex);
            }
        }

        private void UpdateDerivedEntities()
        {
            State = WorkerState.Working;
            StatusMessage = "Init";

            //Set the current Db - this will only be used for High Level processing
            //Using the same Datacontext for all work is causing OutOfMemory Exception
            //So, I'll have to use a bunch of 'worker' datacontexts
            using (var dc = new AryaDbDataContext(Arguments.ProjectId, Arguments.UserId))
            {
                var skuSets = GetSkuSets(dc);

                foreach (var skuSet in skuSets)
                    ProcessSkuSet(skuSet);
            }
            State = WorkerState.Complete;
        }

        /// <summary>
        /// Create chunks of SKUs so that they all can be processed in separate Data Contexts
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IEnumerable<List<Guid>> GetSkuSets(AryaDbDataContext dc)
        {
            const int chunkCapacity = 1000;

            var allSkus = dc.Skus.Where(sku => sku.SkuType == Sku.ItemType.Product.ToString());
            var taxIds = GetTaxonomyFilters(dc);

            if (taxIds.Count > 0 && taxIds.Count < 2000) //2k is because of Sql Server limit of 2100+
                allSkus = allSkus.Where(sku => taxIds.Contains(sku.SkuInfos.First(si => si.Active).TaxonomyID));
            else
            {
                //Otherwise, simply run it for the whole damn thing, for now...
                Summary.Warnings.Add(new WorkerWarning
                                     {
                                         ErrorMessage = "Too many Taxonomy Parameters.",
                                         ErrorDetails =
                                             "SQL has a limit of 2000 parameters. Ignoring the Taxonomy Filter criteria. Running through all SKUs (!!)"
                                     });
            }

            var chunk = new List<Guid>(chunkCapacity);
            var iCtr = 0;
            foreach (var sku in allSkus.Select(sku => sku.ID))
            {
                chunk.Add(sku);
                if (++iCtr < chunkCapacity)
                    continue;

                yield return chunk;
                chunk = new List<Guid>();
                iCtr = 0;
            }
            yield return chunk;
        }

        private List<Guid> GetTaxonomyFilters(AryaDbDataContext dc)
        {
            //Get the Taxonomy IDs from the Taxonomy Filters, and their children
            var taxIds = new List<Guid>();
            foreach (var ti in ((DerivedAttributeValueUpdateArguments) Arguments).TaxonomyIds)
            {
                var taxId = ti;
                var taxonomy = dc.TaxonomyInfos.FirstOrDefault(tax => tax.ID == taxId);

                if (taxonomy == null)
                    continue;

                taxIds.Add(taxonomy.ID);
                taxIds.AddRange(taxonomy.AllChildren.Select(tax => tax.ID));
            }
            return taxIds;
        }

        /// <summary>
        /// Process each SKU Set separately (using a new Data Context)
        /// </summary>
        /// <param name="skuSet"></param>
        private void ProcessSkuSet(List<Guid> skuSet)
        {
            using (var dc = new AryaDbDataContext(Arguments.ProjectId, Arguments.UserId))
            {
                //Get all Derived attributes
                var derivedAttributes =
                    dc.Attributes.Where(att => att.AttributeType == AttributeTypeEnum.Derived.ToString()).ToList();

                var skus = dc.Skus.Where(sku => skuSet.Contains(sku.ID));

                var iCtr = 0;

                if (Summary.Warnings==null)
                    Summary.Warnings=new List<WorkerWarning>();
                //var dbCommand = dc.GetCommand(skus);
                //Summary.Warnings.Add(new WorkerWarning { ErrorDetails = dbCommand.CommandText });
                //foreach (DbParameter parameter in dbCommand.Parameters)
                //{
                //    Summary.Warnings.Add(new WorkerWarning
                //                         {
                //                             ErrorMessage = parameter.ParameterName,
                //                             ErrorDetails = parameter.Value.ToString()
                //                         });
                //}
                foreach (var sku in skus)
                {
                    ProcessSku(sku, derivedAttributes, dc);

                    if (++iCtr%100 != 0)
                        continue;

                    dc.SubmitChanges();
                    StatusMessage = string.Format("{0} SKUs processed, {1} Values inserted/updated.", _iSkuCount, _iValueCount);
                }

                dc.SubmitChanges();
                StatusMessage = string.Format("{0} SKUs processed, {1} Values inserted/updated.", _iSkuCount, _iValueCount);
            }
        }

        /// <summary>
        /// Update Derived Attribute Values in this SKU
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="derivedAttributes"></param>
        /// <param name="dc"></param>
        private void ProcessSku(Sku sku, IEnumerable<Attribute> derivedAttributes, AryaDbDataContext dc)
        {
            var derivedValues = sku.DerivedEntityDatas.Where(ded => ded.Active).ToDictionary(d => d.AttributeID, d => d);

            foreach (var att in derivedAttributes)
            {
                var value = sku.GetValuesForAttribute(dc, att, false).First().Value;

                if (derivedValues.ContainsKey(att.ID))
                {
                    if (derivedValues[att.ID].Value == value)
                        continue; //No change to the value, nothing to do here...

                    //Deactivate this value; we'll insert a new value later
                    derivedValues[att.ID].Active = false;
                }

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                sku.DerivedEntityDatas.Add(new DerivedEntityData(dc) {Attribute = att, Value = value});
                ++_iValueCount;
            }
            ++_iSkuCount;
        }

        public virtual List<string> ValidateInput() { throw new NotImplementedException(); }
        public virtual bool IsInputValid() { throw new NotImplementedException(); }
    }

    public class DerivedAttributeValueUpdateArguments : WorkerArguments
    {
        public List<Guid> TaxonomyIds { get; set; }
    }
}