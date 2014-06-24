using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Properties;

namespace Arya.Framework.Data.AryaDb
{
    public partial class Attribute : BaseEntity, IComparable<Attribute>
    {
        #region Fields (3)

        public static readonly string[] NonMetaAttributeTypes = { "Sku", "Global", "Derived", "Flag", "Product" };
        //public static readonly string[] SchemaMetaAttributeTypes = { "Sku", "Global", "Derived", "Flag" };
        public static readonly string[] TaxonomyEnrichmentAttributes = { Resources.TaxonomyEnrichmentImageAttributeName,Resources.TaxonomyEnrichmentCopyAttributeName };
        public static readonly string[] SchemaEnrichmentAttributes = { Resources.SchemaEnrichmentCopyAttributeName, Resources.SchemaEnrichmentImageAttributeName };
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

        #endregion Fields

        #region Construtor

        public Attribute(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #endregion Constructor

        #region Properties

        public string Group
        {
            get
            {
               var attr = AttributeGroups.Select(g => g.Group.Name).Aggregate((text1, text2) => text1 + "," + text2);
               return attr.ToString();
            }
        }
        public AttributeTypeEnum Type
        {
            get
            {
                AttributeTypeEnum currentAttributeType;
                if (Enum.TryParse(AttributeType ?? AttributeTypeEnum.Sku.ToString(), true, out currentAttributeType))
                    return currentAttributeType;
                throw new Exception(String.Format("Attribute Type {0} not defined", AttributeType));
            }
            set { AttributeType = value.ToString(); }
        }

        #endregion

        public int CompareTo(Attribute other)
        {
            return !AttributeName.Equals(other.AttributeName)
                ? String.CompareOrdinal(AttributeName, other.AttributeName)
                : ID.CompareTo(other.ID);
        }

        //public static Attribute GetAttributeFromName(AryaDbDataContext db, string attributeName,
        //    bool createIfNotFound, AttributeTypeEnum attributeType = AttributeTypeEnum.NonMeta)
       public static Attribute GetAttributeFromName(AryaDbDataContext db, string attributeName, bool createIfNotFound, AttributeTypeEnum attributeType = AttributeTypeEnum.NonMeta, bool useChache = true)
        {
            // select cache to use
           //TODO: Very important please ask vivek for making this change.
            if (!useChache)
            {
                NonMetaAttributeCache.Clear();
                SchemaMetaAttributeCache.Clear();
                SchemaMetaMetaAttributeCache.Clear();
                TaxonomyMetaAttributeCache.Clear();
                TaxonomyMetaMetaAttributeCache.Clear();

            }
            Dictionary<string, Attribute> attributeCache;
            switch (attributeType)
            {
                case AttributeTypeEnum.SchemaMeta:
                {
                    attributeCache = SchemaMetaAttributeCache;
                    break;
                }

                case AttributeTypeEnum.SchemaMetaMeta:
                {
                    attributeCache = SchemaMetaMetaAttributeCache;
                    break;
                }

                case AttributeTypeEnum.TaxonomyMeta:
                {
                    attributeCache = TaxonomyMetaAttributeCache;
                    break;
                }

                case AttributeTypeEnum.TaxonomyMetaMeta:
                {
                    attributeCache = TaxonomyMetaMetaAttributeCache;
                    break;
                }

                case AttributeTypeEnum.Workflow:
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
            var attQuery = from attribute in db.Attributes
                where attribute.AttributeName.ToLower().Equals(lowerAttributeName) && attribute.ProjectID == db.CurrentProject.ID
                select attribute;
            attQuery = attributeType == AttributeTypeEnum.NonMeta
                ? attQuery.Where(attr => NonMetaAttributeTypes.Contains(attr.AttributeType))
                : attQuery.Where(attr => attr.AttributeType == attributeType.ToString());

            var att = attQuery.FirstOrDefault();

            // if found, return it
            if (att != null)
                newAttribute = att;
                // if not found and creation is requested, create it
            else if (createIfNotFound)
            {
                newAttribute = new Attribute(db)
                               {
                                   AttributeName = attributeName,
                                   AttributeType =
                                       (attributeType == AttributeTypeEnum.NonMeta
                                           ? AttributeTypeEnum.Sku
                                           : attributeType).ToString()
                               };
                db.CurrentProject.Attributes.Add(newAttribute);
              // db.SubmitChanges();
                
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
        //partial void OnCreated()
        //{
        //    var parentContext = ParentContext;
        //    if (parentContext == null) return;
        //    AryaDbDataContext.DefaultInsertedTableValues(this, parentContext.CurrentUser.ID);
        //} 
    }

    public enum AttributeTypeEnum
    {
        Sku,
        Global,
        Derived,
        Workflow,
        Flag,
        SchemaMeta,
        TaxonomyMeta,
        SchemaMetaMeta,
        TaxonomyMetaMeta,
        AttributeMeta,
        NonMeta
    }
}