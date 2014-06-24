using System;
using System.Collections.Generic;
using System.Linq;
using Arya.HelperClasses;

namespace Arya.Data
{
    public partial class Attribute : IComparable
    {
        #region Fields

        // attribute caches
        //public const string NewAttributeName = " <New Attribute> ";
        //public const string TaxonomyEnrichmentCopy = "Taxonomy Enrichment Copy";
        //public const string TaxonomyEnrichmentImage = "Taxonomy Enrichment Image";
        ////public const string SchemaEnrichmentCopy = "Schema Enrichment Copy";
        //public const string SchemaEnrichmentImage = "Schema Enrichment Image";
        private static readonly Dictionary<string, Attribute> NonMetaAttributeCache =
            new Dictionary<string, Attribute>();

        private static readonly Dictionary<string, Attribute> SchemaMetaAttributeCache =
            new Dictionary<string, Attribute>();

        private static readonly Dictionary<string, Attribute> SchemaMetaMetaAttributeCache =
            new Dictionary<string, Attribute>();

        private static readonly Dictionary<string, Attribute> TaxonomyMetaAttributeCache =
            new Dictionary<string, Attribute>();

        private static readonly Dictionary<string, Attribute> TaxonomyMetaMetaAttributeCache =
            new Dictionary<string, Attribute>();

        private static readonly Dictionary<string, Attribute> WorkflowAttributeCache =
            new Dictionary<string, Attribute>();

        #endregion Fields

        #region Properties

        public string Group
        {
            get
            {
                string groupName = null;
                var ag = AttributeGroups.FirstOrDefault();
                if (ag != null)
                    groupName = ag.Group.Name;
                return groupName;
            }

            set
            {
                // fetch or create a group with the specified value
                var group =
                    AryaTools.Instance.InstanceData.Dc.Groups.FirstOrDefault(
                        g => g.GroupType == "Attribute" && g.Name == value)
                    ?? new Group(true) {GroupType = "Attribute", Name = value};

                // check to see if this attribute has a group associated with it
                var ag = AttributeGroups.FirstOrDefault();
                if (ag == null)
                {
                    // no group associated - add attribute group with this group
                    AttributeGroups.Add(new AttributeGroup {Group = group});
                }
                else
                {
                    // assign group to attribute group
                    ag.Group = group;
                }
            }
        }

        public Framework.Data.AryaDb.AttributeTypeEnum Type
        {
            get
            {
                Framework.Data.AryaDb.AttributeTypeEnum currentAttributeType;
                if (Enum.TryParse(AttributeType, true, out currentAttributeType))
                    return currentAttributeType;
                throw new Exception(String.Format("Unknown Attribute Type {0} in Project {1}", AttributeType,
                    Project.ProjectName));
            }
            set { AttributeType = value.ToString(); }
        }

        #endregion

        #region Methods

        public int CompareTo(object obj)
        {
            var thisString = ToString();
            var objString = obj.ToString();
            if (!thisString.Equals(objString))
                return String.CompareOrdinal(thisString, objString);

            var other = (Attribute) obj;
            return ID.CompareTo(other.ID);
        }

        public static Attribute GetAttributeFromName(string attributeName, bool createIfNotFound,
            Framework.Data.AryaDb.AttributeTypeEnum attributeType =
                Framework.Data.AryaDb.AttributeTypeEnum.NonMeta)
        {
            // select cache to use
            Dictionary<string, Attribute> attributeCache;
            switch (attributeType)
            {
                case Framework.Data.AryaDb.AttributeTypeEnum.SchemaMeta:
                {
                    attributeCache = SchemaMetaAttributeCache;
                    break;
                }

                case Framework.Data.AryaDb.AttributeTypeEnum.SchemaMetaMeta:
                {
                    attributeCache = SchemaMetaMetaAttributeCache;
                    break;
                }

                case Framework.Data.AryaDb.AttributeTypeEnum.TaxonomyMeta:
                {
                    attributeCache = TaxonomyMetaAttributeCache;
                    break;
                }

                case Framework.Data.AryaDb.AttributeTypeEnum.TaxonomyMetaMeta:
                {
                    attributeCache = TaxonomyMetaMetaAttributeCache;
                    break;
                }

                case Framework.Data.AryaDb.AttributeTypeEnum.Workflow:
                {
                    attributeCache = WorkflowAttributeCache;
                    break;
                }

                default: //Sku, Global, Derived, Flag
                {
                    attributeCache = NonMetaAttributeCache;
                    break;
                }
            }

            // format attribute name
            attributeName = attributeName.Trim();
            var lowerAttributeName = attributeName.ToLower();

            // try to find attribute in cache and make sure it is correct
            if (attributeCache.ContainsKey(lowerAttributeName))
            {
                var attribute = attributeCache[lowerAttributeName];
                if (attribute != null && !attribute.AttributeName.ToLower().Equals(lowerAttributeName))
                    attributeCache.Remove(lowerAttributeName);
                else
                    return attribute;
            }

            // if attribute is not cached, try to find it in the whole attribute file
            Attribute newAttribute = null;
            var attQuery = from attribute in AryaTools.Instance.InstanceData.Dc.Attributes
                where
                    attribute.AttributeName.Equals(lowerAttributeName)
                    && attribute.ProjectID == AryaTools.Instance.InstanceData.CurrentProject.ID
                select attribute;
            attQuery = attributeType == Framework.Data.AryaDb.AttributeTypeEnum.NonMeta
                ? attQuery.Where(
                    attr => Framework.Data.AryaDb.Attribute.NonMetaAttributeTypes.Contains(attr.AttributeType))
                : attQuery.Where(attr => attr.AttributeType == attributeType.ToString());

            var att = attQuery.FirstOrDefault();

            // if found, return it
            if (att != null)
                newAttribute = att;
                // if not found and creation is requested, create it
            else if (createIfNotFound)
            {
                newAttribute = new Attribute
                               {
                                   AttributeName = attributeName,
                                   AttributeType =
                                       (attributeType
                                        == Framework.Data.AryaDb.AttributeTypeEnum.NonMeta
                                           ? Framework.Data.AryaDb.AttributeTypeEnum.Sku
                                           : attributeType).ToString()
                               };
                AryaTools.Instance.InstanceData.CurrentProject.Attributes.Add(newAttribute);
            }

            // if attribute exists, try to add it to the appropriate cache
            if (newAttribute != null)
            {
                if (!attributeCache.Keys.Contains(lowerAttributeName))
                    attributeCache.Add(lowerAttributeName, newAttribute);
            }

            return newAttribute;
        }

        public override string ToString() { return AttributeName; }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
            ProjectID = AryaTools.Instance.InstanceData.CurrentProject.ID;
            Type = Framework.Data.AryaDb.AttributeTypeEnum.Sku;
        }

        #endregion Methods

     
    }
}