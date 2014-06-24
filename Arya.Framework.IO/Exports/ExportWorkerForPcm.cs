using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.Pcm;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"Product Content Manager (Internal only)")]
    public sealed class ExportWorkerForPcm : ExportWorkerBase
    {
        #region Fields

        private const string EnUs = "en-US";

        private readonly ConcurrentDictionary<string, AttVal> _lovs = new ConcurrentDictionary<string, AttVal>();

        //private static readonly Regex RxNonAlphaNumerics = new Regex("[^A-Za-z0-9]");
        private AdvancedExportArgs _args;

        #endregion Fields

        #region Constructors

        public ExportWorkerForPcm(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof (AdvancedExportArgs))
        {
        }

        #endregion Constructors

        #region Methods

        protected override void FetchExportData()
        {
            _args = (AdvancedExportArgs) Arguments;
            ProcessCatalog();
        }

        protected override void SaveExportData()
        {
        }

        private static string GetLovKey(Guid nodeId, string attributeName, string value)
        {
            return nodeId + ":" + attributeName + ":" + value;
        }

        private string GetSaveFilePath(string type, string id = null)
        {
            var name = type + (string.IsNullOrWhiteSpace(id) ? string.Empty : "-" + id);
            return Path.Combine(ArgumentDirectoryPath, string.Format("{0}{1}.xml", _args.BaseFilename, name));
        }

        private void ProcessAttributeValues(Sku dbItem, Item resultItem)
        {
            var attVals = (from ei in dbItem.EntityInfos
                from ed in ei.EntityDatas
                let attributeName = ed.Attribute.AttributeName
                let attributeGroup =
                    ed.Attribute.AttributeGroups.Where(ag => ag.Group.Name.StartsWith("Search_"))
                        .Select(ag => ag.Group.Name.Substring(7))
                        .FirstOrDefault() ?? attributeName
                where ed.Active
                select
                    new ItemAttributeValue
                    {
                        Name = attributeName,
                        Group = attributeGroup,
                        Value = ed.Value + (ed.Uom != null ? " " + ed.Uom : string.Empty)
                    }).ToArray();

            resultItem.AttributeValue = attVals;
            var nodeId = dbItem.Taxonomy.ID;
            foreach (var av in attVals)
            {
                var key = GetLovKey(nodeId, av.Name, av.Value);
                if (_lovs.ContainsKey(key))
                    return;

                var attVal = AttVal.FromValues(nodeId, av.Name, av.Group, av.Value);
                _lovs[key] = attVal;
            }
        }

        private void ProcessCatalog()
        {
            ProcessTaxonomyNodes();
        }

        private void ProcessLovs(SchemaInfo schemaInfo)
        {
            var nodeId = schemaInfo.TaxonomyID;
            var attributeName = schemaInfo.Attribute.AttributeName;
            var attributeGroup =
                schemaInfo.Attribute.AttributeGroups.Where(ag => ag.Group.Name.StartsWith("Search_"))
                    .Select(ag => ag.Group.Name.Substring(7))
                    .FirstOrDefault() ?? attributeName;

            var lovMetaDatas = from lov in schemaInfo.ListOfValues
                where lov.Active
                let enrichmentImage = new EnrichmentFileResourceType {Filename = lov.EnrichmentImage}
                let enrichment =
                    new Enrichment {EnrichmentCopy = lov.EnrichmentCopy, EnrichmentPrimaryImage = enrichmentImage}
                select
                    new
                    {
                        NodeId = nodeId,
                        AttributeName = attributeName,
                        AttributeGroup = attributeGroup,
                        lov.Value,
                        Enrichment = enrichment,
                        lov.DisplayOrder
                    };

            foreach (var lov in lovMetaDatas)
            {
                var key = GetLovKey(lov.NodeId, lov.AttributeName, lov.Value);
                if (!_lovs.ContainsKey(key))
                    _lovs[key] = AttVal.FromValues(lov.NodeId, lov.AttributeName, lov.AttributeGroup, lov.Value);

                var attVal = _lovs[key];
                attVal.Enrichment = lov.Enrichment;

                if (lov.DisplayOrder != null)
                    attVal.SortableValue = string.Format("{0:000}-{1}", lov.DisplayOrder, lov.Value);
            }

            var lovsToSave = _lovs.Where(l => l.Key.StartsWith(GetLovKey(nodeId, attributeName, string.Empty))).ToList();
            foreach (var lov in lovsToSave)
            {
                lov.Value.SerializeObject(GetSaveFilePath("AttVal", Guid.NewGuid().ToString()));
                AttVal val;
                _lovs.TryRemove(lov.Key, out val);
            }
        }

        private NodeSchemaAttribute ProcessSchema(SchemaInfo si)
        {
            var attributeGroup =
                si.Attribute.AttributeGroups.Where(ag => ag.Group.Name.StartsWith("Search_"))
                    .Select(ag => ag.Group.Name.Substring(7))
                    .FirstOrDefault() ?? si.Attribute.AttributeName;
            var attr = NodeSchemaAttribute.FromValues(si.Attribute.AttributeName, attributeGroup, si.SchemaData.DataType,
                si.SchemaData.NavigationOrder, si.SchemaData.DisplayOrder);

            var schemaMetaDatas = from smi in si.SchemaMetaInfos
                from smd in smi.SchemaMetaDatas
                where smd.Active
                select smd;
            foreach (var smd in schemaMetaDatas)
            {
                switch (smd.SchemaMetaInfo.Attribute.AttributeName)
                {
                    case "Schema Enrichment Copy":
                        if (attr.Enrichment == null)
                            attr.Enrichment = new Enrichment {lang = EnUs};
                        attr.Enrichment.EnrichmentCopy = smd.Value;
                        break;
                    case "Schema Enrichment Image":
                        if (attr.Enrichment == null)
                            attr.Enrichment = new Enrichment {lang = EnUs};
                        attr.Enrichment.EnrichmentPrimaryImage = new EnrichmentFileResourceType {Filename = smd.Value};
                        break;
                }
            }

            ProcessLovs(si);
            return attr;
        }

        private void ProcessSku(ItemNode node, Sku sku)
        {
            try
            {
                using (var dc = new AryaDbDataContext(Arguments.ProjectId, Arguments.UserId))
                {
                    var dbItem = dc.Skus.Single(s => sku.ID == s.ID);
                    var psp = (from ei in dbItem.EntityInfos
                        from ed in ei.EntityDatas
                        where
                            ed.Active
                            && (ed.Attribute.AttributeName.ToLower().Contains("primary keyword")
                                || ed.Attribute.AttributeName.ToLower().Contains("psp"))
                        orderby ed.Attribute.AttributeName descending
                        select ed.Value).FirstOrDefault() ?? "Item " + dbItem.ItemID;

                    var resultItem = Item.FromValues(node, dbItem.ID, dbItem.ItemID, psp);

                    ProcessAttributeValues(dbItem, resultItem);

                    resultItem.SerializeObject(GetSaveFilePath("Item", dbItem.ID.ToString()));
                }
            }
            catch (Exception ex)
            {
                var message = Environment.NewLine + "Method: ProcessSku";
                var e = ex;
                while (e != null)
                {
                    message = Environment.NewLine + e.Message;
                    e = e.InnerException;
                }
                CurrentLogWriter.Error(ex.Source + message + Environment.NewLine + ex.StackTrace);
                if (Summary.Warnings == null)
                    Summary.Warnings = new List<WorkerWarning>();
                Summary.Warnings.Add(new WorkerWarning
                {
                    ErrorMessage = ex.Message,
                    ErrorDetails = ex.StackTrace,
                    LineData = sku.ItemID
                });
            }
        }

        private void ProcessTaxonomyChildren(TaxonomyInfo node)
        {
            var children = from td in node.ChildTaxonomyDatas where td.Active select td.TaxonomyInfo;

            foreach (var child in children)
                ProcessTaxonomyNode(child);
        }

        private void ProcessTaxonomyNode(TaxonomyInfo node)
        {
            StatusMessage = string.Format("Processing {0}", node);

            ProcessTaxonomySkus(node); //First SKUs, then Schema and LOV
            ProcessTaxonomySchemas(node);
            ProcessTaxonomyChildren(node);
        }

        private void ProcessTaxonomyNodes()
        {
            StatusMessage = "Generating Node List";

            var selectedNodes = from tax in CurrentDb.TaxonomyInfos
                where _args.TaxonomyIds.Contains(tax.ID)
                let nodeName = tax.TaxonomyDatas.First(td => td.Active).NodeName
                orderby nodeName
                select tax;

            foreach (var node in selectedNodes)
                ProcessTaxonomyNode(node);
        }

        private void ProcessTaxonomySchemas(TaxonomyInfo ti)
        {
            //try
            //{
            var taxonomyPaths = new List<string>();
            var parts = ti.ToStringParts().ToList();
            if (((AdvancedExportArgs) Arguments).IgnoreT1Taxonomy)
                parts.RemoveAt(0);

            for (var level = 1; level <= parts.Count; level++)
            {
                var taxPath = level.ToString();
                for (var path = 0; path < level; path++)
                    taxPath += "/" + parts[path];

                taxonomyPaths.Add(taxPath);
            }

            var node = Node.FromValues(ti.ID, ti.ProjectID, ti.TaxonomyData.ParentTaxonomyID ?? Guid.Empty,
                ti.ToString(((AdvancedExportArgs) Arguments).IgnoreT1Taxonomy), taxonomyPaths.ToArray(), ti.NodeName);

            var taxonomyMetaDatas = from tmi in ti.TaxonomyMetaInfos
                from tmd in tmi.TaxonomyMetaDatas
                where tmd.Active
                select tmd;
            foreach (var tmd in taxonomyMetaDatas)
            {
                switch (tmd.TaxonomyMetaInfo.Attribute.AttributeName)
                {
                    case "Taxonomy Enrichment Copy":
                        if (node.Enrichment == null)
                            node.Enrichment = new Enrichment {lang = EnUs};
                        node.Enrichment.EnrichmentCopy = tmd.Value;
                        break;
                    case "Taxonomy Enrichment Image":
                        if (node.Enrichment == null)
                            node.Enrichment = new Enrichment {lang = EnUs};
                        node.Enrichment.EnrichmentPrimaryImage = new EnrichmentFileResourceType {Filename = tmd.Value};
                        break;
                }
            }

            var schemas = from sch in ti.SchemaInfos
                where sch.SchemaDatas.Any(sd => sd.Active)
                orderby sch.Attribute.AttributeName
                select sch;

            node.SchemaAttribute = schemas.Select(ProcessSchema).ToArray();

            node.SerializeObject(GetSaveFilePath("Node", ti.ID.ToString()));
            //}
            //catch (Exception ex)
            //{
            //    if (Summary.Warnings == null)
            //        Summary.Warnings = new List<WorkerWarning>();
            //    Summary.Warnings.Add(new WorkerWarning
            //    {
            //        ErrorMessage = ex.Message,
            //        ErrorDetails = ex.StackTrace,
            //        LineData = ti.ToString()
            //    });
            //}
        }

        private void ProcessTaxonomySkus(TaxonomyInfo ti)
        {
            //Must use independent DataContext to conserve memory
            using (var dc = new AryaDbDataContext(Arguments.ProjectId, Arguments.UserId))
            {
                var node = ItemNode.FromValues(ti.ID, ti.ProjectID,
                    ti.ToString(((AdvancedExportArgs) Arguments).IgnoreT1Taxonomy), ti.NodeName);

                //var allSkus = from si in dc.SkuInfos
                //              where si.Active && si.TaxonomyID == ti.ID
                //              let sku = si.Sku
                //              where sku.SkuType == Sku.ItemType.Product.ToString()
                //              select sku;

                var allSkus = ti.GetSkus(_args.ExportCrossListNodes);
                var skus = ((AdvancedExportArgs) Arguments).GetFilteredSkuList(allSkus).ToList();

                skus.AsParallel().ForAll(sku => ProcessSku(node, sku));
            }
        }

        #endregion Methods
    }
}