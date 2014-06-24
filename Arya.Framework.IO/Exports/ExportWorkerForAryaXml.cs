using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Natalie.Framework.Common;
using Natalie.Framework.Settings;
using Natalie.Framework.ComponentModel;
using Natalie.Framework.Data.NatalieDb;


namespace Natalie.Framework.IO.Exports
{
    [Serializable]
    public class ExportWorkerForNatalieXml : ExportWorkerBase
    {
        public ExportWorkerForNatalieXml(string argumentFilePath, PropertyGrid ownerPropertyGrid)
            : base(argumentFilePath, ownerPropertyGrid)
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

            var selectedNode = new NatalieTaxonomy {NodeName = selectedTaxonomy.NodeName};
            var topNode = GetTopNode(selectedNode, selectedTaxonomy);

            ExportTaxonomyNode(selectedNode, selectedTaxonomy);

            var project = new NatalieProject
                          {
                              ProjectName = selectedTaxonomy.Project.ProjectName,
                              Taxonomy = new[] {topNode}
                          };

            var serializer = new XmlSerializer(typeof (NatalieProject));
            using (TextWriter file = new StreamWriter(ExportFileName))
                serializer.Serialize(file, project);

            StatusMessage = "Done.";
            State = WorkerState.Ready;
        }

        private void ExportTaxonomyNode(NatalieTaxonomy selectedNode, TaxonomyInfo selectedTaxonomy)
        {
            StatusMessage = selectedTaxonomy.ToString();

            //Export Schema
            var inSchemaAttributes =
                selectedTaxonomy.SchemaInfos.Where(si => si.SchemaData != null && si.SchemaData.InSchema).ToList();
            var schemas = new List<NatalieSchema>();
            foreach (var si in inSchemaAttributes)
            {
                var metaDatas = from mi in si.SchemaMetaInfos let md = mi.SchemaMetaData where md != null select md;

                var datas =
                    metaDatas.Select(
                        metaData =>
                            new NatalieSchemaData
                            {
                                MetaAttributeName = metaData.SchemaMetaInfo.Attribute.AttributeName,
                                Value = metaData.Value
                            }).ToArray();

                var lovs = (from lov in si.ListOfValues
                    select
                        new NatalieLov
                        {
                            Value = lov.Value,
                            DisplayOrder = lov.DisplayOrder.ToString(),
                            Copy = lov.EnrichmentCopy,
                            Image = lov.EnrichmentImage
                        }).ToArray();

                var schema = new NatalieSchema
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
            var items = new List<NatalieItem>();
            var allAttributes = inSchemaAttributes.Select(att => att.Attribute.AttributeName).ToList();
            foreach (var sku in selectedTaxonomy.Skus)
            {
                var curentSku = sku;
                var entities = from attribute in allAttributes
                    from value in curentSku.GetValuesForAttribute(attribute, false)
                    select new NatalieItemData {AttributeName = attribute, Value = value.Value, Uom = value.Uom};

                var skuNode = new NatalieItem {ItemId = sku.ItemID, ItemDatas = entities.ToArray()};
                items.Add(skuNode);
            }
            selectedNode.Items = items.ToArray();

            //Children
            var children = new List<NatalieTaxonomy>();
            foreach (var childTaxonomy in selectedTaxonomy.AllChildren)
            {
                var childNode = new NatalieTaxonomy
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

        private static NatalieTaxonomy GetTopNode(NatalieTaxonomy node, TaxonomyInfo ti)
        {
            if (ti.TaxonomyData.ParentTaxonomyInfo == null)
                return node;

            var parentTaxonomy = ti.TaxonomyData.ParentTaxonomyInfo;
            var parent = new NatalieTaxonomy {NodeName = parentTaxonomy.NodeName, Taxonomies = new[] {node}};

            return GetTopNode(parent, parentTaxonomy);
        }
    }
}