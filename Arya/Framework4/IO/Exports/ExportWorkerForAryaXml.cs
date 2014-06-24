using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Arya.Data;
using Arya.Framework.Common;
using Arya.Framework.Settings;
using Arya.Framework4.ComponentModel;

namespace Arya.Framework4.IO.Exports
{
    [Serializable]
    public class ExportWorkerForAryaXml : ExportWorker
    {
        public ExportWorkerForAryaXml(string argumentDirectoryPath, PropertyGrid ownerPropertyGrid)
            : base(argumentDirectoryPath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            AllowMultipleTaxonomySelection = false;
            WorkerSupportsSaveOptions = false;
        }

        public override void Run()
        {
            State = WorkerState.Working;
            StatusMessage = "Init";

            StatusMessage = "Generating Child node list ... ";

            var selectedTaxonomy = Taxonomies[0].Taxonomy;
            MaximumProgress =
                Taxonomies.Cast<ExtendedTaxonomyInfo>().SelectMany(p => p.Taxonomy.AllLeafChildren).Distinct().Count();
            CurrentProgress = 0;

            var selectedNode = new AryaTaxonomy {NodeName = selectedTaxonomy.NodeName};
            var topNode = GetTopNode(selectedNode, selectedTaxonomy);

            ExportTaxonomyNode(selectedNode, selectedTaxonomy);

            var project = new AryaProject
                          {
                              ProjectName = selectedTaxonomy.Project.ProjectName,
                              Taxonomy = new[] {topNode}
                          };

            var serializer = new XmlSerializer(typeof (AryaProject));
            using (TextWriter file = new StreamWriter(ExportFileName))
                serializer.Serialize(file, project);

            StatusMessage = "Done.";
            State = WorkerState.Ready;
        }

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        private void ExportTaxonomyNode(AryaTaxonomy selectedNode, TaxonomyInfo selectedTaxonomy)
        {
            StatusMessage = selectedTaxonomy.ToString();

            //Export Schema
            var inSchemaAttributes =
                selectedTaxonomy.SchemaInfos.Where(si => si.SchemaData != null && si.SchemaData.InSchema).ToList();
            var schemas = new List<AryaSchema>();
            foreach (var si in inSchemaAttributes)
            {
                var metaDatas = from mi in si.SchemaMetaInfos let md = mi.SchemaMetaData where md != null select md;

                var datas =
                    metaDatas.Select(
                        metaData =>
                            new AryaSchemaData
                            {
                                MetaAttributeName = metaData.SchemaMetaInfo.Attribute.AttributeName,
                                Value = metaData.Value
                            }).ToArray();

                var lovs = (from lov in si.ListOfValues
                    select
                        new AryaLov
                        {
                            Value = lov.Value,
                            DisplayOrder = lov.DisplayOrder.ToString(),
                            Copy = lov.EnrichmentCopy,
                            Image = lov.EnrichmentImage
                        }).ToArray();

                var schema = new AryaSchema
                             {
                                 AttributeName = si.Attribute.AttributeName,
                                 NavigationalOrder = si.SchemaData.NavigationOrder.ToString(),
                                 DisplayOrder = si.SchemaData.DisplayOrder.ToString(),
                                 DataType = si.SchemaData.DataType,
                                 SchemaDatas = datas,
                                 ListOfValues = lovs
                             };
                schemas.Add(schema);
            }
            selectedNode.Schemas = schemas.ToArray();

            //Export Items
            var items = new List<AryaItem>();
            var allAttributes = inSchemaAttributes.Select(att => att.Attribute.AttributeName).ToList();
            foreach (var sku in selectedTaxonomy.Skus)
            {
                var curentSku = sku;
                var entities = from attribute in allAttributes
                    from value in curentSku.GetValuesForAttribute(attribute, false)
                    select new AryaItemData {AttributeName = attribute, Value = value.Value, Uom = value.Uom};

                var skuNode = new AryaItem {ItemId = sku.ItemID, ItemDatas = entities.ToArray()};
                items.Add(skuNode);
            }
            selectedNode.Items = items.ToArray();

            //Children
            var children = new List<AryaTaxonomy>();
            foreach (var childTaxonomy in selectedTaxonomy.AllChildren)
            {
                var childNode = new AryaTaxonomy
                                {
                                    NodeName = childTaxonomy.NodeName,
                                    Copy = childTaxonomy.EnrichmentCopy,
                                    Image = childTaxonomy.EnrichmentImage
                                };
                children.Add(childNode);

                ExportTaxonomyNode(childNode, childTaxonomy);
            }
            selectedNode.Taxonomies = children.ToArray();

            CurrentProgress++;
        }

        private static AryaTaxonomy GetTopNode(AryaTaxonomy node, TaxonomyInfo ti)
        {
            if (ti.TaxonomyData.ParentTaxonomyInfo == null)
                return node;

            var parentTaxonomy = ti.TaxonomyData.ParentTaxonomyInfo;
            var parent = new AryaTaxonomy {NodeName = parentTaxonomy.NodeName, Taxonomies = new[] {node}};

            return GetTopNode(parent, parentTaxonomy);
        }
    }
}