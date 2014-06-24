using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.IO.Bridge
{
    public partial class ProductCatalogProductCatalogName
    {
        #region Fields

        private const string EnUs = "en-US";

        #endregion Fields

        #region Methods

        public static List<ProductCatalogProductCatalogName> FromName(string nodeName)
        {
            return new List<ProductCatalogProductCatalogName>
                   {
                       new ProductCatalogProductCatalogName
                       {
                           lang = EnUs,
                           Value = nodeName
                       }
                   };
        }

        #endregion Methods
    }

    public partial class ProductCatalogSchemaMetaDataLanguageVersions
    {
        #region Fields

        private const string EnUs = "en-US";

        #endregion Fields

        #region Methods

        public static List<ProductCatalogSchemaMetaDataLanguageVersions> FromAryaProject(Project project)
        {
            var attributes = project.GetMetaAttributes(AttributeTypeEnum.SchemaMeta);
            var metaDatas =
                attributes.Select(
                    (t, i) => new AttributeMetaDatumType {Id = t.ID, Name = t.AttributeName, DisplayOrder = i}).ToList();
            var item = new ProductCatalogSchemaMetaDataLanguageVersions
                       {
                           lang = EnUs,
                           SchemaAttributeMetaDatas = metaDatas
                       };
            return new List<ProductCatalogSchemaMetaDataLanguageVersions> {item};
        }

        #endregion Methods
    }

    public partial class ProductCatalogTaxonomyMetaDataLanguageVersions
    {
        #region Fields

        private const string EnUs = "en-US";

        #endregion Fields

        #region Methods

        public static List<ProductCatalogTaxonomyMetaDataLanguageVersions> FromAryaProject(Project project)
        {
            var attributes = project.GetMetaAttributes(AttributeTypeEnum.TaxonomyMeta);
            var metaDatas =
                attributes.Select(
                    (t, i) => new AttributeMetaDatumType {Id = t.ID, Name = t.AttributeName, DisplayOrder = i}).ToList();
            var item = new ProductCatalogTaxonomyMetaDataLanguageVersions
                       {
                           lang = EnUs,
                           TaxonomyNodeAttributeMetaDatas = metaDatas
                       };
            return new List<ProductCatalogTaxonomyMetaDataLanguageVersions> {item};
        }

        #endregion Methods
    }

    public partial class SkuSkuAttributesPrimarySemanticPhrase
    {
        #region Fields

        private const string EnUs = "en-US";

        #endregion Fields

        #region Methods

        public static List<SkuSkuAttributesPrimarySemanticPhrase> FromPsp(string psp)
        {
            var phrase = new SkuSkuAttributesPrimarySemanticPhrase {lang = EnUs, Value = psp};
            return new List<SkuSkuAttributesPrimarySemanticPhrase> {phrase};
        }

        #endregion Methods
    }

    public partial class TaxonomyNode
    {
        #region Fields

        private const string EnUs = "en-US";

        #endregion Fields

        #region Properties

        public Enrichment Enrichment
        {
            get
            {
                if (TaxonomyNodeDescriptors == null || TaxonomyNodeDescriptors.Count == 0)
                    TaxonomyNodeDescriptors = TaxonomyNodeDescriptor.FromNameGetList(string.Empty);
                var desc = TaxonomyNodeDescriptors.First();
                return desc.Enrichment ?? (desc.Enrichment = new Enrichment {lang = EnUs});
            }
        }

        #endregion Properties

        #region Methods

        public static TaxonomyNode FromValues(DateTime createdOn, Guid id, string nodeName, Guid parentId,
            Guid catalogId)
        {
            var node = new TaxonomyNode
                       {
                           Id = id,
                           TaxonomyNodeDescriptors = TaxonomyNodeDescriptor.FromNameGetList(nodeName),
                           LastUpdatedTimestamp = new TimestampRecordType {Timestamp = createdOn},
                           CatalogId = catalogId
                       };
            if (parentId != Guid.Empty)
                node.ParentId = parentId;

            return node;
        }

        public bool ShouldSerializeCatalogId() { return CatalogId != Guid.Empty; }

        public bool ShouldSerializeParentId() { return ParentId != Guid.Empty; }

        #endregion Methods
    }

    public partial class TaxonomyNodeDescriptor
    {
        #region Fields

        private const string EnUs = "en-US";

        #endregion Fields

        #region Methods

        public static TaxonomyNodeDescriptor FromName(string nodeName, string nodeDescription = null)
        {
            return new TaxonomyNodeDescriptor {lang = EnUs, NodeName = nodeName, NodeDescription = nodeDescription};
        }

        public static List<TaxonomyNodeDescriptor> FromNameGetList(string nodeName, string nodeDescription = null)
        {
            return new List<TaxonomyNodeDescriptor>
                   {
                       new TaxonomyNodeDescriptor
                       {
                           lang = EnUs,
                           NodeName = nodeName,
                           NodeDescription = nodeDescription
                       }
                   };
        }

        #endregion Methods
    }

    public partial class TimestampRecordType
    {
        #region Methods

        public static TimestampRecordType FromValues(DateTime dateTime, User user,
            TimestampRecordTypeActionType actionType = TimestampRecordTypeActionType.Updated)
        {
            return new TimestampRecordType {Timestamp = dateTime, ActionType = actionType, User = user};
        }

        #endregion Methods
    }

    public partial class User
    {
        #region Methods

        public static User FromAryaUser(Data.AryaDb.User aryaUser)
        {
            var userParts = aryaUser.FullName.Split(' ');
            return FromValues(userParts.First(), userParts.Last(), aryaUser.ID);
        }

        public static User FromValues(string first, string last, Guid createdBy)
        {
            return new User {FirstName = first, LastName = last, UserId = createdBy};
        }

        #endregion Methods
    }
}