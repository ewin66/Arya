using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.IO.InterchangeRecords;
using Arya.Framework.Properties;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.IO.Exports
{
    [DisplayName(@"Arya Interchange Format")]
    public class ExportWorkerForAryaInterchangeFormat : ExportWorkerBase
    {
        #region Fields

        private readonly CombinedInterchangeData _data = new CombinedInterchangeData(true);
        private readonly List<Sku> _processedSkus = new List<Sku>();
        private ListOfValuesExportSorter _sorter;

        private InterchangeFormatExportArgs _args;
        private IEnumerable<Attribute> _globals;

        #endregion Fields

        #region Constructors

        public ExportWorkerForAryaInterchangeFormat(string argumentFilePath)
            : base(argumentFilePath, typeof (InterchangeFormatExportArgs))
        {
        }

        #endregion Constructors

        #region Methods

        protected override void FetchExportData()
        {
            _sorter = new ListOfValuesExportSorter(CurrentDb);
            ProcessNodes();
        }

        protected override void SaveExportData()
        {
            _data.DedupLists();
            using (
                TextWriter file = new StreamWriter(Path.Combine(ArgumentDirectoryPath, _args.BaseFilename + "_AryaInterchangeFormat.xml")))
            {
                _data.SerializeObject(file);
            }
        }

        private void DeleteSkuAttributes(IEnumerable<Sku> skus)
        {
            foreach (var sku in skus)
            {
                var actives = (from ei in sku.EntityInfos from ed in ei.EntityDatas where ed.Active select ed).ToList();

                // Get all active attribute names for this sku
                var atts = actives.Select(ed => ed.Attribute.AttributeName).Distinct().ToList();

                // get all published entity datas for this sku
                var pubs = from ei in sku.EntityInfos from ed in ei.EntityDatas where ed.Published select ed;

                foreach (var pub in pubs)
                {
                    if (_args.MarkAsPublished)
                        pub.Published = false;
                    if (!atts.Contains(pub.Attribute.AttributeName))
                    {
                        _data.SkuAttributeValues.Add(new SkuAttributeValueInterchangeRecord
                                                     {
                                                         ItemID = sku.ItemID,
                                                         AttributeName =
                                                             pub.Attribute
                                                             .AttributeName
                                                     });
                    }
                }

                // set published to true for all active data
                if (_args.MarkAsPublished)
                {
                    foreach (var active in actives)
                        active.Published = true;
                }
            }
        }

        private string GetImageFileName(string image)
        {
            if (String.IsNullOrEmpty(image))
                return null;

            string imageFileName = null;
            Guid imageGuid;
            if (Guid.TryParse(image, out imageGuid))
            {
                var imageMgr = new ImageManager(CurrentDb, CurrentProjectId, image);
                return imageMgr.OriginalFileName;
                // convert guid to filename
                //                Sku imageSku = CurrentDb.Skus.FirstOrDefault(s => s.ID == imageGuid);
                //                if (imageSku != null)
                //                {
                //                    imageFileName = imageSku.EntityInfos.SelectMany(ei => ei.EntityDatas)
                //                        .Where(
                //                            ed =>
                //                                ed.Active &&
                //                                ed.Attribute.AttributeName.ToLower() == Framework.Properties.Resources.ImageSkuGuidFileNameNoExtensionAttributeName.ToLower())
                //                        .Select(ed => ed.Value).FirstOrDefault();
                //                }
            }
            imageFileName = image;
            return imageFileName;
        }

        private void ProcessNodes()
        {
            StatusMessage = "Generating Node List";
            _args = (InterchangeFormatExportArgs) Arguments;

            _globals =
                _args.GlobalAttributes.Select(att => Attribute.GetAttributeFromName(CurrentDb, att, false))
                    .Where(att => att != null);

            var nodes = (from tax in CurrentDb.TaxonomyInfos where _args.TaxonomyIds.Contains(tax.ID) select tax).ToList();
            var selectedNodes = nodes.SelectMany(p => p.AllChildren).Union(nodes).Distinct();

            var taxonomyEnrichmentImageAttributeId =
                Attribute.GetAttributeFromName(CurrentDb, Resources.TaxonomyEnrichmentImageAttributeName, false,
                    AttributeTypeEnum.TaxonomyMeta).ID;

            var taxonomyEnrichmentCopyAttributeId =
                Attribute.GetAttributeFromName(CurrentDb, Resources.TaxonomyEnrichmentCopyAttributeName, false,
                    AttributeTypeEnum.TaxonomyMeta).ID;

            foreach (var node in selectedNodes)
            {
                ProcessNode(node, taxonomyEnrichmentImageAttributeId, taxonomyEnrichmentCopyAttributeId);
            }
        }

        private void ProcessNode(TaxonomyInfo node, Guid taxonomyEnrichmentImageAttributeId,
            Guid taxonomyEnrichmentCopyAttributeId)
        { 
            // skip over this node if the taxonomy path is blank
            if (String.IsNullOrEmpty(node.ToString(_args.IgnoreT1Taxonomy)))
                return;

            // skip over this node if it's derived and we aren't exporting them
            if (node.NodeType == TaxonomyInfo.NodeTypeDerived && !_args.ExportCrossListNodes)
                return;

            StatusMessage = string.Format("Processing {0}", node);
            _data.Taxonomies.Add(new TaxonomyInterchangeRecord {TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy)});

            if (_args.IncludeTaxonomyEnrichments)
            {
                var creationFilterDate = _args.CreationFilterDate;
                var taxImageMd = (from tmi in node.TaxonomyMetaInfos
                    where tmi.MetaAttributeID.Equals(taxonomyEnrichmentImageAttributeId)
                    from tmd in tmi.TaxonomyMetaDatas
                    where creationFilterDate <= tmd.CreatedOn
                    select tmd).ToList();

                if (taxImageMd.Any())
                {
                    if (taxImageMd.All(tmd => !tmd.Active))
                    {
                        _data.TaxonomyMetaDatas.Add(new TaxonomyMetaDataInterchangeRecord
                                                    {
                                                        TaxonomyPath =
                                                            node.ToString(
                                                                _args.IgnoreT1Taxonomy),
                                                        TaxonomyMetaAttributeName =
                                                            _args.UseOldFormat
                                                                ? "EnrichmentImage"
                                                                : "Taxonomy Enrichment Image",
//                                                            TaxonomyMetaAttributeValue = ""
                                                    });
                    }
                    else
                    {
                        var taxImageValue = taxImageMd.Where(tmd => tmd.Active).Select(tmd => tmd.Value).FirstOrDefault();
                        var taxImage = GetImageFileName(taxImageValue);
                        if (!String.IsNullOrEmpty(taxImage))
                        {
                            _data.TaxonomyMetaDatas.Add(new TaxonomyMetaDataInterchangeRecord
                                                        {
                                                            TaxonomyPath =
                                                                node.ToString(
                                                                    _args.IgnoreT1Taxonomy),
                                                            TaxonomyMetaAttributeName =
                                                                _args.UseOldFormat
                                                                    ? "EnrichmentImage"
                                                                    : "Taxonomy Enrichment Image",
                                                            TaxonomyMetaAttributeValue =
                                                                taxImage
                                                        });
                        }
                    }
                }

                var taxCopyMd = (from tmi in node.TaxonomyMetaInfos
                    where tmi.MetaAttributeID.Equals(taxonomyEnrichmentCopyAttributeId)
                    from tmd in tmi.TaxonomyMetaDatas
                    where creationFilterDate <= tmd.CreatedOn
                    select tmd).ToList();

                if (taxCopyMd.Any())
                {
                    if (taxCopyMd.All(tmd => !tmd.Active))
                    {
                        _data.TaxonomyMetaDatas.Add(new TaxonomyMetaDataInterchangeRecord
                                                    {
                                                        TaxonomyPath =
                                                            node.ToString(
                                                                _args.IgnoreT1Taxonomy),
                                                        TaxonomyMetaAttributeName =
                                                            _args.UseOldFormat
                                                                ? "EnrichmentCopy"
                                                                : "Taxonomy Enrichment Copy",
//                                                            TaxonomyMetaAttributeValue = ""
                                                    });
                    }
                    else
                    {
                        var taxCopyValue = taxCopyMd.Where(tmd => tmd.Active).Select(tmd => tmd.Value).FirstOrDefault();
                        if (!String.IsNullOrEmpty(taxCopyValue))
                        {
                            _data.TaxonomyMetaDatas.Add(new TaxonomyMetaDataInterchangeRecord
                                                        {
                                                            TaxonomyPath =
                                                                node.ToString(
                                                                    _args.IgnoreT1Taxonomy),
                                                            TaxonomyMetaAttributeName =
                                                                _args.UseOldFormat
                                                                    ? "EnrichmentCopy"
                                                                    : "Taxonomy Enrichment Copy",
                                                            TaxonomyMetaAttributeValue =
                                                                taxCopyValue
                                                        });
                        }
                    }
                }
            }

            ProcessNodeComponents(node);
        }

        private void ProcessNodeComponents(TaxonomyInfo node)
        {
            // get list of SKUs for this taxonomy
            var allSkus = node.GetSkus(_args.ExportCrossListNodes);
            if (!allSkus.Any())
                return;

            // filter skus based on sku inclusions and exclusions, then include only Product skus.
            var skus = _args.GetFilteredSkuList(allSkus).ToList();

            ProcessSchema(node);
            ProcessSkus(node, skus);
        }

        private void ProcessSchema(TaxonomyInfo node)
        {
            var schemata = node.SchemaInfos.Select(si => si.SchemaData).Where(sd => sd != null);
            var creationFilterDate = _args.CreationFilterDate;
            _sorter.ClearListOfValues();

            foreach (var sd in schemata)
            {
                // create list of values for this schema data
                _sorter.MakeListOfValues(sd);

                //If ExportExtendedAttributes is TRUE, ignore attributes not InSchema unless it's a specified global
                if (!_args.ExportExtendedAttributes && !sd.InSchema && _args.GlobalAttributes != null
                    && Array.IndexOf(_args.GlobalAttributes, sd.SchemaInfo.Attribute.AttributeName) == -1)
                    continue;

                //Add Attribute
                var attribute = sd.SchemaInfo.Attribute;
                _data.Attributes.Add(new AttributeInterchangeRecord
                                     {
                                         AttributeName = attribute.AttributeName,
                                         AttributeType = attribute.AttributeType
                                     });

                if (attribute.Type == AttributeTypeEnum.Derived)
                {
                    //Add Derived Attribute Expression for Default Taxonomy and Current Taxonomy
                    var expressions =
                        attribute.DerivedAttributes.Where(de => de.TaxonomyInfo == null || de.TaxonomyInfo == node);

                    var derivedExpressions =
                        expressions.Select(
                            de =>
                                new DerivedAttributeInterchangeRecord
                                {
                                    DerivedAttributeName = attribute.AttributeName,
                                    TaxonomyPath =
                                        de.TaxonomyInfo == null
                                            ? null
                                            : de.TaxonomyInfo.ToString(
                                                _args.IgnoreT1Taxonomy),
                                    DerivedAttributeExpression = de.Expression,
                                    MaxResultLength = de.MaxResultLength
                                });
                    _data.DerivedAttributes.AddRange(derivedExpressions);
                }

                //Add Schema Data
                _data.Schemas.Add(new SchemaInterchangeRecord
                                  {
                                      TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                      AttributeName = attribute.AttributeName,
                                      DataType = sd.DataType,
                                      NavigationOrder = sd.NavigationOrder,
                                      DisplayOrder = sd.DisplayOrder,
                                      InSchema = sd.InSchema
                                  });

                //Add ListOfValues
                foreach (var lov in sd.SchemaInfo.ListOfValues.Where(v => v.Active))
                {
                    if (_args.IncludeLovEnrichments && creationFilterDate <= lov.CreatedOn)
                    {
                        _data.ListOfValues.Add(new ListOfValuesInterchangeRecord
                                               {
                                                   TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                   AttributeName = attribute.AttributeName,
                                                   Values = _sorter.GetLanguageValues(attribute.AttributeName, lov.Value, null),
                                                   EnrichmentCopy = lov.EnrichmentCopy,
                                                   EnrichmentImage = GetImageFileName(lov.EnrichmentImage)
                                               });
                    }
                    else
                    {
                        _data.ListOfValues.Add(new ListOfValuesInterchangeRecord
                                               {
                                                   TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                   AttributeName = attribute.AttributeName,
                                                   Values = _sorter.GetLanguageValues(attribute.AttributeName, lov.Value, null)
                                               });
                    }
                }

                // process deleted enrichments
                if (_args.IncludeSchemaEnrichments)
                {
                    var smdImage = (from smi in sd.SchemaInfo.SchemaMetaInfos
                        where smi.Attribute.AttributeName == "Schema Enrichment Image"
                        from smd in smi.SchemaMetaDatas
                        where creationFilterDate <= smd.CreatedOn
                        select smd).ToList();

                    if (smdImage.Any() && smdImage.All(s => !s.Active))
                    {
                        _data.SchemaMetaDatas.Add(new SchemaMetaDataInterchangeRecord
                                                  {
                                                      TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                      AttributeName = attribute.AttributeName,
                                                      SchemaMetaAttributeName = _args.UseOldFormat ? "EnrichmentImage" : "Schema Enrichment Image",
//                                                      SchemaMetaAttributeValue = ""
                                                  });
                    }

                    var smdCopy = (from smi in sd.SchemaInfo.SchemaMetaInfos
                        where smi.Attribute.AttributeName == "Schema Enrichment Copy"
                        from smd in smi.SchemaMetaDatas
                        where creationFilterDate <= smd.CreatedOn
                        select smd).ToList();

                    if (smdCopy.Any() && smdCopy.All(s => !s.Active))
                    {
                        _data.SchemaMetaDatas.Add(new SchemaMetaDataInterchangeRecord
                                                  {
                                                      TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                      AttributeName = attribute.AttributeName,
                                                      SchemaMetaAttributeName = _args.UseOldFormat ? "EnrichmentCopy" : "Schema Enrichment Copy",
//                                                      SchemaMetaAttributeValue = ""
                                                  });
                    }
                }

                //Get Schema Meta Datas
                var smds = sd.SchemaInfo.SchemaMetaInfos.Select(smi => smi.SchemaMetaData).Where(smd => smd != null);
                foreach (var smd in smds)
                {
                    if (smd.SchemaMetaInfo.Attribute.AttributeName == "Schema Enrichment Image")
                    {
                        if (_args.IncludeSchemaEnrichments && creationFilterDate <= smd.CreatedOn)
                        {
                            _data.SchemaMetaDatas.Add(new SchemaMetaDataInterchangeRecord
                                                      {
                                                          TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                          AttributeName = attribute.AttributeName,
                                                          SchemaMetaAttributeName = _args.UseOldFormat ? "EnrichmentImage" : "Schema Enrichment Image",
                                                          SchemaMetaAttributeValue = GetImageFileName(smd.Value)
                                                      });
                        }
                    }
                    else if (smd.SchemaMetaInfo.Attribute.AttributeName == "Schema Enrichment Copy")
                    {
                        if (_args.IncludeSchemaEnrichments && creationFilterDate <= smd.CreatedOn)
                        {
                            _data.SchemaMetaDatas.Add(new SchemaMetaDataInterchangeRecord
                                                      {
                                                          TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                          AttributeName = attribute.AttributeName,
                                                          SchemaMetaAttributeName = _args.UseOldFormat ? "EnrichmentCopy" : "Schema Enrichment Copy",
                                                          SchemaMetaAttributeValue = smd.Value
                                                      });
                        }
                    }
                    else
                    {
                        if (_args.IncludeSchemaMetadata)
                        {
                            _data.SchemaMetaDatas.Add(new SchemaMetaDataInterchangeRecord
                                                      {
                                                          TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                                          AttributeName = attribute.AttributeName,
                                                          SchemaMetaAttributeName = smd.SchemaMetaInfo.Attribute.AttributeName,
                                                          SchemaMetaAttributeValue = smd.Value
                                                      });
                        }
                    }
                }
            }
        }

        private void ProcessSkus(TaxonomyInfo node, IEnumerable<Sku> skus)
        {
            if (!_args.IncludeSkus)
                return;

            var skuList = skus.ToList();

            // process deleted item/attribute pairs
            DeleteSkuAttributes(skuList);

            // start with listed globals
            var baseAttributes = _globals.ToList();

            // add in-schema attributes, if requested
            if (_args.IncludeInSchemaValues)
            {
                var inSchemaAttributes =
                    node.SchemaInfos.Select(si => si.SchemaData)
                        .Where(sd => sd != null && sd.InSchema)
                        .Select(sd => sd.SchemaInfo.Attribute)
                        .ToList();
                baseAttributes.AddRange(inSchemaAttributes);
            }

            foreach (var sku in skuList)
            {
                //Add to SkuTaxonomy records
                _data.SkuTaxonomies.Add(new SkuTaxonomyInterchangeRecord
                                        {
                                            ItemID = sku.ItemID,
                                            TaxonomyPath = node.ToString(_args.IgnoreT1Taxonomy),
                                            IsPrimary = node.NodeType == TaxonomyInfo.NodeTypeRegular
                                        });
                if (node.NodeType != TaxonomyInfo.NodeTypeRegular)
                {
                    _data.SkuTaxonomies.Add(new SkuTaxonomyInterchangeRecord
                    {
                        ItemID = sku.ItemID,
                        TaxonomyPath = sku.Taxonomy.ToString(_args.IgnoreT1Taxonomy),
                        IsPrimary = sku.Taxonomy.NodeType == TaxonomyInfo.NodeTypeRegular
                    });

                    _data.Taxonomies.Add(new TaxonomyInterchangeRecord
                    {
                        TaxonomyPath = sku.Taxonomy.ToString(_args.IgnoreT1Taxonomy)
                    });
                }
                if (!_args.IncludeSkuValues || _processedSkus.Contains(sku))
                    continue;
                
                _processedSkus.Add(sku);

                var atts = baseAttributes.ToHashSet();
                if (_args.ExportExtendedAttributes)
                {
                    //Export all the extended attributes (that haven't been already exported)
                    var ext = from ei in sku.EntityInfos
                        from ed in ei.EntityDatas
                        where ed.Active && !atts.Contains(ed.Attribute)
                        select ed.Attribute;
                    atts.AddRange(ext.Distinct());
                }

                foreach (var att in atts)
                {
                    // write an attribute record, just for completeness
                    _data.Attributes.Add(new AttributeInterchangeRecord
                    {
                        AttributeName = att.AttributeName,
                        AttributeType = att.AttributeType
                    });

                    // get all values for this attribute
                    var values = sku.GetValuesForAttribute(CurrentDb, att);

                    // if there are no values for this attribute, assume that there may
                    // be inactive records - write a delete record.
                    if (values.Count == 0)
                    {
                        // if there are deleted values, write a blank record
                        _data.SkuAttributeValues.Add(new SkuAttributeValueInterchangeRecord
                        {
                            ItemID = sku.ItemID,
                            AttributeName = att.AttributeName,
                        });
                        continue;
                    }

                    foreach (var ed in values)
                    {
                        if (_args.IgnoreEntityId)
                        {
                            _data.SkuAttributeValues.Add(new SkuAttributeValueInterchangeRecord
                            {
                                ItemID = sku.ItemID,
                                AttributeName = att.AttributeName,
                                Values = _sorter.GetLanguageValues(att.AttributeName, ed.Value, ed.Uom),
                                Uom = ed.Uom,
                                Field1 = ed.Field1,
                                Field2 = ed.Field2,
                                Field3 = ed.Field3,
                                Field4 = ed.Field4,
                                Field5 = ed.Field5
                            });
                        }
                        else
                        {
                            _data.SkuAttributeValues.Add(new SkuAttributeValueInterchangeRecord
                            {
                                ItemID = sku.ItemID,
                                AttributeName = att.AttributeName,
                                Values = _sorter.GetLanguageValues(att.AttributeName, ed.Value, ed.Uom),
                                Uom = ed.Uom,
                                Field1 = ed.Field1,
                                Field2 = ed.Field2,
                                Field3 = ed.Field3,
                                Field4 = ed.Field4,
                                Field5 = ed.Field5,
                                EntityID = ed.EntityID
                            });
                        }
                    }
                }
            }
        }

        #endregion Methods
    }

    [Serializable]
    public class InterchangeFormatExportArgs : AdvancedExportArgs
    {
        #region Fields

        private bool _includeEnrichments;
        private bool _includeSkuValues;
        private bool _includeSkus;

        #endregion Fields

        #region Properties

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 27)]
        [DisplayName(@"Filter Enrichments on Creation Date")]
        [Description("(Example: 2014-06-20 or 2008-09-26 18:32:00) A means to limit the enrichment data exported based on the date the data was created. If a date is specified, only enrichment data that has been created after the given date will be exported.")]
        [TypeConverter(typeof (DateTimeConverter))]
        public DateTime CreationFilterDate { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 22)]
        [DisplayName(@"Export Enrichments")]
        [Description("If YES, the interchange format will include the enrichment data (image names and copy) in the export file. If NO, the enrichment data will not be exported.")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeEnrichments
        {
            get { return _includeEnrichments; }
            set
            {
                _includeEnrichments = value;
                IncludeTaxonomyEnrichments = value;
                IncludeSchemaEnrichments = value;
                IncludeLovEnrichments = value;
            }
        }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 25)]
        [DisplayName(@"Include LOV Enrichments")]
        [Description("When True, LOV Enrichments will be exported")]
        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeLovEnrichments { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 24)]
        [DisplayName(@"Include Schema Enrichments")]
        [Description("When True, Schema Enrichments will be exported")]
        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeSchemaEnrichments { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 26)]
        [DisplayName(@"Export Project Meta-Attributes")]
        [Description("If YES, all the project meta-attributes are included in the export. If NO, only the primary meta-attributes (ranks & data type) will be exported.")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeSchemaMetadata { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 20)]
        [DisplayName(@"Export SKUs")]
        [Description("If YES, the interchange format will include the SKU classification data (SKU to taxonomy relationship) in the export file. If NO, the SKU classification data will not be exported.")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeSkus
        {
            get { return _includeSkus; }
            set
            {
                _includeSkus = value;
                if (!value)
                    IncludeSkuValues = false;
            }
        }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 21)]
        [DisplayName(@"Export SKU Attribute Values")]
        [Description("If YES, the interchange format will include the SKU level attribute data in the export file. If NO, the SKU level attribute data will not be exported.")]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeSkuValues
        {
            get { return _includeSkuValues; }
            set
            {
                _includeSkuValues = value;
                if (value)
                    IncludeSkus = true;
            }
        }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 29)]
        [DisplayName(@"Export In-Schema Attributes Only")]
        [Description("If YES, the interchange format will only include attribute data that are marked as In-Schema. If NO, all attributes on the SKUs will be included in the interchange format file.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IncludeInSchemaValues { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 23)]
        [DisplayName(@"Include Taxonomy Enrichments")]
        [Description("When True, Taxonomy Enrichments will be exported")]
        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool IncludeTaxonomyEnrichments { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 28)]
        [DisplayName(@"Ignore Internal Identifier")]
        [Description("If YES, the interchange format will not include the unique entity IDs for entity data in the export. Use the YES setting if your intention is to turn the exported file around and import it into a new project or database. Use the NO setting if you are updating a database in which the entity IDs match those in the database you’re exporting from.")]
        [Browsable(true)]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool IgnoreEntityId { get; set; }

        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder + 30)]
        [DisplayName(@"Use Old (Motion) Enrichment Format")]
        [Description("If YES, the interchange format will be compatible with the Motion project. If NO, the interchange format will use the latest interchange specification.")]
        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool UseOldFormat { get; set; }

        #endregion Properties

        public InterchangeFormatExportArgs()
        {
            HiddenProperties += "ExportFileType" + "FieldDelimiter";
        }
    }
}