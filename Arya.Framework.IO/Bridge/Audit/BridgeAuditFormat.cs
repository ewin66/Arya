using System;
using System.Collections.Generic;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Properties;

namespace Arya.Framework.IO.Bridge.Audit
{
    //public partial class SchemaAuditTrail
    //{
    //    #region Methods

    //    public static SchemaAuditTrail FromValues(Guid schemaId)
    //    {
    //        return new SchemaAuditTrail
    //        {
    //            SchemaId = schemaId,
    //            SchemaAuditTrailRecords = new List<SchemaAuditTrailRecord>()
    //        };
    //    }

    //    #endregion Methods
    //}

    //public partial class TaxonomyNodeAuditTrail
    //{
    //    #region Methods

    //    public static TaxonomyNodeAuditTrail FromValue(Guid nodeId)
    //    {
    //        return new TaxonomyNodeAuditTrail
    //               {
    //                   NodeId = nodeId,
    //                   TaxonomyNodeAuditTrailRecords = new List<TaxonomyNodeAuditTrailRecord>()
    //               };
    //    }

    //    #endregion Methods
    //}

    //public partial class TaxonomyNodeAuditTrailRecord
    //{
    //    #region Fields

    //    private const string EnUs = "en-US";

    //    #endregion Fields

    //    #region Methods

    //    public static TaxonomyNodeAuditTrailRecord FromValues(TaxonomyData td)
    //    {
    //        var hierarchy = new TaxonomyNodeAuditTrailRecordHierarchy
    //                        {
    //                            CatalogId = td.TaxonomyInfo.ProjectID,
    //                            ParentId = td.ParentTaxonomyID
    //                        };
    //        return new TaxonomyNodeAuditTrailRecord
    //               {
    //                   AuditTrailTimestamp =
    //                       TimestampRecordType.FromValues(td.CreatedOn,
    //                           User.FromAryaUser(td.User)),
    //                   TaxonomyNodeDescriptor =
    //                       TaxonomyNodeDescriptor.FromName(td.NodeName,
    //                           td.NodeDescription),
    //                   Hierarchy = hierarchy
    //               };
    //    }

    //    public static TaxonomyNodeAuditTrailRecord FromValues(TaxonomyMetaData tmd)
    //    {
    //        string attributeName;
    //        try
    //        {
    //            attributeName = tmd.TaxonomyMetaInfo.Attribute.AttributeName;
    //        }
    //        catch (Exception)
    //        {
    //            return null;
    //        }

    //        var tnd = new TaxonomyNodeDescriptor
    //        {
    //            lang = EnUs
    //        };

    //        if (attributeName == Resources.TaxonomyEnrichmentCopyAttributeName)
    //            tnd.Enrichment = new Enrichment {EnrichmentCopy = tmd.Value};
    //        else if (attributeName == Resources.TaxonomyEnrichmentImageAttributeName)
    //            tnd.Enrichment = new Enrichment {EnrichmentCopy = tmd.Value};
    //        else
    //            tnd.MetaDatums = new List<MetaDatumType>
    //            {
    //                new MetaDatumType
    //                {
    //                    Name = attributeName,
    //                    Value = tmd.Value
    //                }
    //            };

    //        return new TaxonomyNodeAuditTrailRecord
    //        {
    //            AuditTrailTimestamp = TimestampRecordType.FromValues(tmd.CreatedOn, User.FromAryaUser(tmd.User)),
    //            TaxonomyNodeDescriptor = tnd
    //        };
    //    }

    //    #endregion Methods
    //}

    //public partial class TaxonomyNodeDescriptor
    //{
    //    #region Fields

    //    private const string EnUs = "en-US";

    //    #endregion Fields

    //    #region Methods

    //    public static TaxonomyNodeDescriptor FromName(string nodeName, string nodeDescription = null)
    //    {
    //        return new TaxonomyNodeDescriptor {lang = EnUs, NodeName = nodeName, NodeDescription = nodeDescription};
    //    }

    //    public static List<TaxonomyNodeDescriptor> FromNameGetList(string nodeName, string nodeDescription = null)
    //    {
    //        return new List<TaxonomyNodeDescriptor>
    //               {
    //                   new TaxonomyNodeDescriptor
    //                   {
    //                       lang = EnUs,
    //                       NodeName = nodeName,
    //                       NodeDescription = nodeDescription
    //                   }
    //               };
    //    }

    //    #endregion Methods
    //}
}