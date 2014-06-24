using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using LinqKit;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.Bridge;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using SchemaAttribute = Arya.Framework.IO.Bridge.SchemaAttribute;
using Sku = Arya.Framework.Data.AryaDb.Sku;
using User = Arya.Framework.IO.Bridge.User;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"PCO/Bridge (Internal only)")]
    public sealed class ExportWorkerForBridge : ExportWorkerBase
    {
        #region Fields

        private const string EnUs = "en-US";

        //private const bool ExportAuditTrail = true;
        private readonly ConcurrentDictionary<Guid, Attribute> _allAttributes = new ConcurrentDictionary<Guid, Attribute>();

        private AdvancedExportArgs _args;
        private ProductCatalog _productCatalog;
        private Project _project;

        #endregion Fields

        #region Constructors

        public ExportWorkerForBridge(string argumentDirectoryPath)
            : base(argumentDirectoryPath, typeof(AdvancedExportArgs))
        {
        }

        #endregion Constructors

        #region Methods

        protected override void FetchExportData()
        {
            _args = (AdvancedExportArgs)Arguments;
            ProcessCatalog();
        }

        protected override void SaveExportData()
        {
        }

        private static void ProcessLovs(SchemaInfo schemaInfo, SchemaAttribute bridgeSchemaAttribute)
        {
            var lovs = from lov in schemaInfo.ListOfValues where lov.Active select lov;

            var listOfValues = new List<LangDependentMetaDataLov>();
            bridgeSchemaAttribute.LangDependentMetaDatas.First().ListOfValues = listOfValues;
            var metaDataLovs = from listOfValue in lovs
                               let enrichmentImage = new EnrichmentFileResourceType { Filename = listOfValue.EnrichmentImage }
                               let enrichment =
                                   new Enrichment
                                   {
                                       EnrichmentCopy = listOfValue.EnrichmentCopy,
                                       EnrichmentPrimaryImage = enrichmentImage
                                   }
                               select
                                   new LangDependentMetaDataLov
                                   {
                                       Id = listOfValue.ID,
                                       Value =
                                           new MeasuredValueType { Value = listOfValue.Value },
                                       DisplayOrder =
                                           Convert.ToInt32(listOfValue.DisplayOrder),
                                       Enrichment = enrichment
                                   };
            listOfValues.AddRange(metaDataLovs);
        }

        private string GetSaveFilePath(string type, string id = null)
        {
            var name = type + (string.IsNullOrWhiteSpace(id) ? string.Empty : "-" + id);
            return Path.Combine(ArgumentDirectoryPath, string.Format("{0}{1}.xml", _args.BaseFilename, name));
        }

        private SkuAttribute GetSkuAttribute(IGrouping<Guid, EntityData> group)
        {
            var details = new SkuAttributeAttributeValues
            {
                lang = EnUs,
                ValueElements =
                    new List<SkuAttributeAttributeValuesValueElement>()
            };
            var skuAttr = new SkuAttribute
            {
                //Id = group.First().AttributeID,
                AttributeId = group.Key,
                AttributeValuess = new List<SkuAttributeAttributeValues>(new[] { details })
            };

            foreach (var entityData in group)
            {
                var val = new SkuAttributeAttributeValuesValueElement
                {
                    Id = entityData.EntityID,
                    Value =
                        new MeasuredValueType
                        {
                            Value = entityData.Value,
                            UoM = entityData.Uom ?? string.Empty
                        },
                    MetaDatums = new List<MetaDatumType>()
                };

                if (!string.IsNullOrWhiteSpace(entityData.Field1))
                {
                    val.MetaDatums.Add(new MetaDatumType
                    {
                        Name = _project.EntityField1Name ?? "Field 1",
                        Value = entityData.Field1
                    });
                }
                if (!string.IsNullOrWhiteSpace(entityData.Field2))
                {
                    val.MetaDatums.Add(new MetaDatumType
                    {
                        Name = _project.EntityField2Name ?? "Field 2",
                        Value = entityData.Field2
                    });
                }
                if (!string.IsNullOrWhiteSpace(entityData.Field3))
                {
                    val.MetaDatums.Add(new MetaDatumType
                    {
                        Name = _project.EntityField3Name ?? "Field 3",
                        Value = entityData.Field3
                    });
                }
                if (!string.IsNullOrWhiteSpace(entityData.Field4))
                {
                    val.MetaDatums.Add(new MetaDatumType
                    {
                        Name = _project.EntityField4Name ?? "Field 4",
                        Value = entityData.Field4
                    });
                }

                if (_project.EntityField5IsStatus)
                    val.StatusFlag = entityData.Field5OrStatus;
                else if (!string.IsNullOrWhiteSpace(entityData.Field5))
                {
                    val.MetaDatums.Add(new MetaDatumType
                    {
                        Name = _project.EntityField5Name ?? "Field 5",
                        Value = entityData.Field5OrStatus
                    });
                }

                details.ValueElements.Add(val);
            }

            return skuAttr;
        }

        private void ProcessCatalog()
        {
            _project = (from project in CurrentDb.Projects where project.ID == _args.ProjectId select project).First();
            var aryaUser = CurrentDb.Users.First(u => u.ID == _project.CreatedBy);
            var lastUpdatedTimestamp = TimestampRecordType.FromValues(_project.CreatedOn,
                User.FromAryaUser(aryaUser));

            CurrentLogWriter.Debug("Processing " + _project);
            _productCatalog = new ProductCatalog
            {
                Id = _project.ID,
                Company = _project.ClientDescription,
                Type = "STANDARD",
                ProductCatalogNames =
                    ProductCatalogProductCatalogName.FromName(_project.SetName),
                TaxonomyMetaDataLanguageVersionss =
                    ProductCatalogTaxonomyMetaDataLanguageVersions.FromAryaProject(
                        _project),
                SchemaMetaDataLanguageVersionss =
                    ProductCatalogSchemaMetaDataLanguageVersions.FromAryaProject(
                        _project),
                LastUpdatedTimestamp = lastUpdatedTimestamp
            };

            var taxonomyNode = TaxonomyNode.FromValues(_project.CreatedOn, _project.ID, _project.ToString(),
                    Guid.Empty, _project.ID);
            taxonomyNode.IsRoot = true;
            taxonomyNode.SerializeObject(GetSaveFilePath("Node", _project.ID.ToString()));

            _productCatalog.SerializeObject(GetSaveFilePath("Catalog", _productCatalog.Id.ToString()));

            ProcessTaxonomyNodes();
        }

        private void ProcessSchema(SchemaInfo si)
        {
            var metaDatas = new LangDependentMetaData
            {
                lang = EnUs,
            };
            var sa = new SchemaAttribute
            {
                Id = si.ID,
                TaxonomyNodeId = si.TaxonomyID,
                AttributeId = si.AttributeID,
                DataType = si.SchemaData.DataType,
                DisplayOrder = Convert.ToInt32(si.SchemaData.DisplayOrder),
                NavigationOrder = Convert.ToInt32(si.SchemaData.NavigationOrder),
                InSchema = si.SchemaData.InSchema,
                LangDependentMetaDatas =
                    new List<LangDependentMetaData> { metaDatas }
            };

            var schemaMetaDatas = from smi in si.SchemaMetaInfos
                                  let displayOrder = (from mi in smi.Attribute.SchemaInfos
                                                      from md in mi.SchemaDatas
                                                      where md.Active
                                                      select md.DisplayOrder).Min()
                                  where displayOrder > 0
                                  from smd in smi.SchemaMetaDatas
                                  where smd.Active
                                  select smd;

            foreach (var smd in schemaMetaDatas)
            {
                switch (smd.SchemaMetaInfo.Attribute.AttributeName.ToLower())
                {
                    case "definition":
                        metaDatas.Definition = smd.Value;
                        break;

                    case "sample values":
                        metaDatas.SampleValues = smd.Value;
                        break;

                    case "single or multivalue?":
                    case "single or multivalue":
                        sa.Multivalue = smd.Value.ToLower().Contains("multi");
                        break;

                    case "simple or complex?":
                    case "simple or complex":
                        sa.Complex = smd.Value.ToLower().Contains("complex");
                        break;

                    case "restricted uom":
                        sa.RestrictedUom =
                            smd.Value.Split(new[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(uom => uom.Trim())
                                .ToList();
                        break;

                    case "schema enrichment copy":
                        if (metaDatas.Enrichment == null)
                            metaDatas.Enrichment = new Enrichment { lang = EnUs };
                        metaDatas.Enrichment.EnrichmentCopy = smd.Value;
                        break;

                    case "schema enrichment image":
                        if (metaDatas.Enrichment == null)
                            metaDatas.Enrichment = new Enrichment { lang = EnUs };
                        metaDatas.Enrichment.EnrichmentPrimaryImage = new EnrichmentFileResourceType
                        {
                            Filename =
                                smd.Value
                        };
                        break;

                    default:
                        if (metaDatas.MetaDatums == null)
                            metaDatas.MetaDatums = new List<MetaDatumType>();
                        metaDatas.MetaDatums.Add(new MetaDatumType
                        {
                            Id = smd.MetaID,
                            Name = smd.SchemaMetaInfo.Attribute.AttributeName,
                            Value = smd.Value
                        });
                        break;
                }
            }

            ProcessLovs(si, sa);
            sa.SerializeObject(GetSaveFilePath("SchemaAttribute", sa.Id.ToString()));
        }

        //private void ProcessSchemaTrails(TaxonomyInfo node)
        //{
        //    var schemaTrail = SchemaAuditTrail.FromValues(Guid.NewGuid());
        //    foreach (var si in node.SchemaInfos)
        //    {
        //        PopulatePrimarySchemaMetaDataTrail(si, schemaTrail);
        //        var smds = from smi in si.SchemaMetaInfos
        //                   from smd in smi.SchemaMetaDatas
        //                   select smd;
        //        foreach (var smd in smds)
        //        {
        //            var attribute = smd.SchemaMetaInfo.Attribute.AttributeName;
        //            var record = new SchemaAuditTrailRecord
        //            {
        //                Id = si.AttributeID,
        //                AuditTrailTimestamp =
        //                    TimestampRecordType.FromValues(smd.CreatedOn, User.FromAryaUser(smd.User)),
        //                LangDependentMetaData = new SchemaAuditTrailRecordLangDependentMetaData
        //                {
        //                    lang = EnUs,
        //                    SchemaAttributeName = attribute
        //                }
        //            };
        //            if (attribute == Resources.SchemaEnrichmentCopyAttributeName)
        //            {
        //                record.LangDependentMetaData.Enrichment = new Enrichment
        //                {
        //                    lang = EnUs,
        //                    EnrichmentCopy = smd.Value
        //                };
        //            }
        //            else if (attribute == Resources.SchemaEnrichmentImageAttributeName)
        //            {
        //                record.LangDependentMetaData.Enrichment = new Enrichment
        //                {
        //                    lang = EnUs,
        //                    EnrichmentPrimaryImage = new EnrichmentFileResourceType
        //                    {
        //                        Filename =
        //                            new ImageManager(CurrentDb, _project.ID).OriginalFileUri
        //                    }
        //                };
        //            }
        //            else
        //            {
        //                record.LangDependentMetaData.MetaDatums = new List<MetaDatumType>
        //                {
        //                    new MetaDatumType {Id = smd.MetaID, Name = attribute, Value = smd.Value}
        //                };
        //            }
        //            schemaTrail.SchemaAuditTrailRecords.Add(record);
        //        }
        //    }
        //    schemaTrail.SchemaAuditTrailRecords =
        //        schemaTrail.SchemaAuditTrailRecords
        //            .OrderBy(satr => satr.LangDependentMetaData.SchemaAttributeName)
        //            .ThenBy(satr => satr.AuditTrailTimestamp.Timestamp)
        //            .ToList();
        //    schemaTrail.SerializeObject(GetSaveFilePath("SchemaHistory", node.ID.ToString()));
        //}
        //private static void PopulatePrimarySchemaMetaDataTrail(SchemaInfo si, SchemaAuditTrail schemaTrail)
        //{
        //    var previousSd =
        //        Enumerable.Range(1, 1).Select(sd =>
        //            new
        //            {
        //                DataType = (string)null,
        //                DisplayOrder = (int?)null,
        //                NavigationOrder = (int?)null,
        //                InSchema = (bool?)null,
        //                Required = (bool?)null,
        //                Multivalue = (bool?)null
        //            }).First();
        //    foreach (var sd in si.SchemaDatas.OrderBy(s => s.CreatedOn))
        //    {
        //        var limd = new SchemaAuditTrailRecordLangIndependentMetaData();
        //        if (previousSd.InSchema == null || sd.InSchema != previousSd.InSchema)
        //        {
        //            limd.InSchema = sd.InSchema;
        //            limd.InSchemaSpecified = true;
        //        }
        //        if (previousSd.NavigationOrder == null || sd.NavigationOrder != previousSd.NavigationOrder)
        //        {
        //            limd.NavigationOrder = Convert.ToInt32(sd.NavigationOrder);
        //            limd.NavigationOrderSpecified = true;
        //        }
        //        if (previousSd.DisplayOrder == null || sd.DisplayOrder != previousSd.DisplayOrder)
        //        {
        //            limd.DisplayOrder = Convert.ToInt32(sd.DisplayOrder);
        //            limd.DisplayOrderSpecified = true;
        //        }
        //        if (previousSd.DataType == null || sd.DataType != previousSd.DataType)
        //            limd.DataType = sd.DataType;
        //        var record = new SchemaAuditTrailRecord
        //        {
        //            Id = si.AttributeID,
        //            AuditTrailTimestamp =
        //                TimestampRecordType.FromValues(sd.CreatedOn, User.FromAryaUser(sd.User)),
        //            LangDependentMetaData = new SchemaAuditTrailRecordLangDependentMetaData
        //            {
        //                lang = EnUs,
        //                SchemaAttributeName = si.Attribute.AttributeName
        //            },
        //            LangIndependentMetaData = limd
        //        };
        //        schemaTrail.SchemaAuditTrailRecords.Add(record);
        //        var oldSd = previousSd;
        //        previousSd = Enumerable.Range(1, 1).Select(r => new
        //        {
        //            DataType = limd.DataType ?? oldSd.DataType,
        //            DisplayOrder = limd.DisplayOrderSpecified ? limd.DisplayOrder : oldSd.DisplayOrder,
        //            NavigationOrder = limd.NavigationOrderSpecified ? limd.NavigationOrder : oldSd.NavigationOrder,
        //            InSchema = limd.InSchemaSpecified ? limd.InSchema : oldSd.InSchema,
        //            Required = limd.RequiredSpecified ? limd.Required : oldSd.Required,
        //            Multivalue = limd.MultivalueSpecified ? limd.Multivalue : oldSd.Multivalue
        //        }).First();
        //    }
        //}

        private void ProcessSku(Guid skuId)
        {
            try
            {
                //Must use independent DataContext to conserve memory
                using (var dc = new AryaDbDataContext(Arguments.ProjectId, Arguments.UserId))
                {
                    while (dc.Connection.State != ConnectionState.Open)
                    {
                        switch (dc.Connection.State)
                        {
                            case ConnectionState.Closed:
                                dc.Connection.Open();
                                break;
                            case ConnectionState.Connecting:
                                Thread.Sleep(100);
                                break;
                        }
                    }

                    var item = dc.Skus.First(s => s.ID == skuId);
                    var filename = GetSaveFilePath("Sku", item.ItemID);
                    if (File.Exists(filename))
                        return;

                    var skuAttributes = new List<SkuAttribute>();
                    var psp = (from ei in item.EntityInfos
                               from ed in ei.EntityDatas
                               where
                                   ed.Active
                                   && (ed.Attribute.AttributeName.ToLower().Contains("primary keyword")
                                       || ed.Attribute.AttributeName.ToLower().Contains("psp"))
                               orderby ed.Attribute.AttributeName descending
                               select ed.Value).FirstOrDefault() ?? "Item " + item.ItemID;

                    var sku = new Bridge.Sku
                    {
                        Id = item.ID,
                        Classification = new List<CatalogRef>(),
                        Enrichments = new List<Enrichment>(),
                        SkuAttributes =
                            new SkuSkuAttributes
                            {
                                ItemId = item.ItemID,
                                PrimarySemanticPhrases =
                                    SkuSkuAttributesPrimarySemanticPhrase.FromPsp(psp),
                                SkuAttributes = skuAttributes
                            },
                        LastUpdatedTimestamp =
                            TimestampRecordType.FromValues(item.CreatedOn, User.FromAryaUser(item.User))
                    };

                    var catalogRef = new CatalogRef
                    {
                        CatalogId = _project.ID,
                        Primary = true,
                        TaxonomyNodeId = item.Taxonomy.ID
                    };
                    sku.Classification.Add(catalogRef);

                    var edGroups = (from ei in dc.EntityInfos
                                    where ei.SkuID == item.ID
                                    from ed in ei.EntityDatas
                                    where ed.Active
                                    group ed by ed.Attribute.ID
                                        into grp
                                        select grp).ToList();

                    skuAttributes.AddRange(edGroups.Select(GetSkuAttribute));

                    var derivedAttributes =
                        dc.Attributes
                            .Where(att => att.AttributeType == AttributeTypeEnum.Derived.ToString() && _args.GlobalAttributes.Contains(att.AttributeName))
                            .ToList();

                    var derivedAttributeGroups = (from att in derivedAttributes
                                                  let value = item.GetValuesForAttribute(dc, att, false).First()
                                                  group value by att.ID
                                                      into grp
                                                      select grp).ToList();

                    skuAttributes.AddRange(derivedAttributeGroups.Select(GetSkuAttribute));

                    edGroups.Select(edg => edg.First().Attribute).ForEach(AddAttribute);

                    sku.SerializeObject(filename);
                }
            }
            catch (Exception ex)
            {
                if (Summary.Warnings == null)
                    Summary.Warnings = new List<WorkerWarning>();
                Summary.Warnings.Add(new WorkerWarning
                {
                    ErrorMessage = ex.Message,
                    ErrorDetails = ex.StackTrace,
                    LineData = "SKU " + skuId
                });
            }
        }

        private void AddAttribute(Attribute attribute)
        {
            if (!_allAttributes.TryAdd(attribute.ID, attribute))
                return;

            var att = new Bridge.Attribute
                      {
                          Id = attribute.ID,
                          CatalogId = _project.ID,
                          Type = (attribute.AttributeType == "Sku" ? "Product" : attribute.AttributeType).ToUpper(),
                          Readonly = attribute.Readonly,
                          Name =
                              new List<AttributeName>
                              {
                                  new AttributeName
                                  {
                                      lang = EnUs,
                                      Value =
                                          attribute
                                          .AttributeName
                                  }
                              }
                      };

            att.SerializeObject(GetSaveFilePath("Attribute", att.Id.ToString()));
        }

        private void ProcessTaxonomyChildren(TaxonomyInfo node)
        {
            try
            {
                var children = from td in node.ChildTaxonomyDatas where td.Active select td.TaxonomyInfo;

                foreach (var child in children)
                    ProcessTaxonomyNode(node, child);
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
                CurrentLogWriter.Warn("There was a problem processing children for node." + Environment.NewLine +
                                      message);
            }
        }

        private void ProcessTaxonomyMetaDatas(TaxonomyInfo node, TaxonomyNode bridgeTaxonomy)
        {
            var taxonomyMetaDatas = from tmi in node.TaxonomyMetaInfos
                                    from tmd in tmi.TaxonomyMetaDatas
                                    where tmd.Active
                                    select tmd;

            foreach (var tmd in taxonomyMetaDatas)
            {
                var taxonomyMetaInfo = tmd.TaxonomyMetaInfo;
                switch (taxonomyMetaInfo.Attribute.AttributeName)
                {
                    case "Taxonomy Enrichment Copy":
                        bridgeTaxonomy.Enrichment.EnrichmentCopy = tmd.Value;
                        break;

                    case "Taxonomy Enrichment Image":
                        bridgeTaxonomy.Enrichment.EnrichmentPrimaryImage = new EnrichmentFileResourceType
                        {
                            Filename =
                                tmd
                                    .Value
                        };
                        break;

                    default:
                        var descriptor = bridgeTaxonomy.TaxonomyNodeDescriptors.First();
                        if (descriptor.MetaDatums == null)
                            descriptor.MetaDatums = new List<MetaDatumType>();
                        descriptor.MetaDatums.Add(new MetaDatumType
                        {
                            Id = taxonomyMetaInfo.ID,
                            Name = taxonomyMetaInfo.Attribute.AttributeName,
                            Value = tmd.Value
                        });
                        break;
                }
            }
        }

        private void ProcessTaxonomyNode(TaxonomyInfo parent, TaxonomyInfo node)
        {
            try
            {
                StatusMessage = string.Format("Processing {0}", node);

                var taxonomyNode = TaxonomyNode.FromValues(node.TaxonomyData.CreatedOn, node.ID, node.NodeName,
                    parent != null ? parent.ID : _project.ID, _project.ID);

                if (node.NodeType == TaxonomyInfo.NodeTypeDerived)
                {
                    //TODO: Convert Query from Arya Format to Bridge Format
                    taxonomyNode.DerivedNodeDefinition = new SkuQueryType
                    {
                        SourceNode =
                            new SkuQueryTypeSourceNode
                            {
                                NodeId =
                                    node.ID
                            },
                        SelectionCriterias =
                            new List<SelectionCriteriaType>
                            {
                                new SelectionCriteriaType
                                {
                                    lang
                                        =
                                        EnUs,
                                    Value
                                        =
                                        (node
                                            .DerivedTaxonomies
                                            .FirstOrDefault
                                            ()
                                         ?? new DerivedTaxonomy
                                             ())
                                            .Expression
                                            .Value
                                }
                            }
                    };
                }

                ProcessTaxonomyMetaDatas(node, taxonomyNode);

                //if (ExportAuditTrail)
                //    ProcessTaxonomyNodeTrail(node);

                ProcessTaxonomySchemas(node);
                ProcessTaxonomySkus(node);
                ProcessTaxonomyChildren(node);

                taxonomyNode.SerializeObject(GetSaveFilePath("Node", node.ID.ToString()));
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
                CurrentLogWriter.Warn("There was a problem processing node." + Environment.NewLine + message);
            }
        }

        private void ProcessTaxonomyNodes()
        {
            StatusMessage = "Generating Node List";

            var selectedNodes = (from tax in CurrentDb.TaxonomyInfos
                                 where _args.TaxonomyIds.Contains(tax.ID)
                                 let nodeName = tax.TaxonomyDatas.First(td => td.Active).NodeName
                                 orderby nodeName
                                 select tax).ToList();

            foreach (var node in selectedNodes)
                ProcessTaxonomyNode(null, node);
        }

        //private void ProcessTaxonomyNodeTrail(TaxonomyInfo node)
        //{
        //    var nodeTrail = TaxonomyNodeAuditTrail.FromValue(node.ID);
        //    var nodeHistory = node.TaxonomyDatas;
        //    foreach (var td in nodeHistory)
        //        nodeTrail.TaxonomyNodeAuditTrailRecords.Add(TaxonomyNodeAuditTrailRecord.FromValues(td));
        //    var tmds = from tmi in node.TaxonomyMetaInfos
        //               from tmd in tmi.TaxonomyMetaDatas
        //               select tmd;
        //    foreach (var tmd in tmds)
        //    {
        //        var trailRecord = TaxonomyNodeAuditTrailRecord.FromValues(tmd);
        //        if (trailRecord != null)
        //            nodeTrail.TaxonomyNodeAuditTrailRecords.Add(trailRecord);
        //    }
        //    nodeTrail.TaxonomyNodeAuditTrailRecords.Sort(
        //        (t1, t2) => t1.AuditTrailTimestamp.Timestamp.CompareTo(t2.AuditTrailTimestamp.Timestamp));
        //    nodeTrail.TaxonomyNodeAuditTrailRecords[0].AuditTrailTimestamp.ActionType =
        //        TimestampRecordTypeActionType.Created;
        //    nodeTrail.SerializeObject(GetSaveFilePath("NodeHistory", node.ID.ToString()));
        //}

        private void ProcessTaxonomySchemas(TaxonomyInfo node)
        {
            try
            {
                var schemas = from sch in node.SchemaInfos
                              where sch.SchemaDatas.Any(sd => sd.Active)
                              orderby sch.Attribute.AttributeName
                              select sch;

                schemas.ForEach(ProcessSchema);

                //if (ExportAuditTrail)
                //    ProcessSchemaTrails(node);
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
                CurrentLogWriter.Warn("There was a problem processing schema for node." + Environment.NewLine + message);
            }
        }

        private void ProcessTaxonomySkus(TaxonomyInfo node)
        {
            try
            {
                //Must use independent DataContext to conserve memory
                using (var dc = new AryaDbDataContext(Arguments.ProjectId, Arguments.UserId))
                {
                    var allSkus = from si in dc.SkuInfos
                                  where si.Active && si.TaxonomyID == node.ID
                                  let sku = si.Sku
                                  where sku.SkuType == Sku.ItemType.Product.ToString()
                                  select sku;

                    var skus = ((AdvancedExportArgs)Arguments).GetFilteredSkuList(allSkus).Select(s => s.ID).ToList();

                    skus.AsParallel().ForAll(ProcessSku);
                    //skus.ForEach(ProcessSku);
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
                CurrentLogWriter.Warn("There was a problem processing skus in node." + Environment.NewLine + message);
            }
        }

        #endregion Methods
    }
}