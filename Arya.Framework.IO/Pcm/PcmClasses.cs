using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Arya.Framework.IO.Pcm
{
    // ReSharper disable LocalizableElement
    // ReSharper disable PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public partial class AttVal
    {
        #region Fields

        private string attributeGroupField;
        private string attributeNameField;
        private Enrichment enrichmentField;
        private string labelField;
        private Guid nodeIdField;
        private string[] searchPhraseField;
        private AttValSearchWord[] searchWordField;
        private string sortableValueField;
        private string[] synonymField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string AttributeGroup
        {
            get { return attributeGroupField; }
            set { attributeGroupField = value; }
        }

        /// <remarks />
        public string AttributeName
        {
            get { return attributeNameField; }
            set { attributeNameField = value; }
        }

        /// <remarks />
        public Enrichment Enrichment
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        public string Label
        {
            get { return labelField; }
            set { labelField = value; }
        }

        /// <remarks />
        [XmlElement("NodeId")]
        public Guid NodeId
        {
            get { return nodeIdField; }
            set { nodeIdField = value; }
        }

        /// <remarks />
        [XmlElement("SearchPhrase")]
        public string[] SearchPhrase
        {
            get { return searchPhraseField; }
            set { searchPhraseField = value; }
        }

        /// <remarks />
        [XmlElement("SearchWord")]
        public AttValSearchWord[] SearchWord
        {
            get { return searchWordField; }
            set { searchWordField = value; }
        }

        /// <remarks />
        public string SortableValue
        {
            get { return sortableValueField; }
            set { sortableValueField = value; }
        }

        /// <remarks />
        [XmlElement("Synonym")]
        public string[] Synonym
        {
            get { return synonymField; }
            set { synonymField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class AttValSearchWord
    {
        #region Fields

        private int rankField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public int Rank
        {
            get { return rankField; }
            set { rankField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public class Enrichment
    {
        #region Fields

        private string enrichmentCopyField;
        private EnrichmentFileResourceType[] enrichmentFileResourceField;
        private EnrichmentFileResourceType enrichmentPrimaryImageField;
        private string langField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string EnrichmentCopy
        {
            get { return enrichmentCopyField; }
            set { enrichmentCopyField = value; }
        }

        /// <remarks />
        [XmlElement("EnrichmentFileResource")]
        public EnrichmentFileResourceType[] EnrichmentFileResource
        {
            get { return enrichmentFileResourceField; }
            set { enrichmentFileResourceField = value; }
        }

        /// <remarks />
        public EnrichmentFileResourceType EnrichmentPrimaryImage
        {
            get { return enrichmentPrimaryImageField; }
            set { enrichmentPrimaryImageField = value; }
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(Namespace = "api.empiriSense.com")]
    public class EnrichmentFileResourceType
    {
        #region Fields

        private string filenameField;
        private string nameField;
        private EnrichmentFileResourceTypeProperty[] propertyField;
        private string typeField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string Filename
        {
            get { return filenameField; }
            set { filenameField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks />
        [XmlElement("Property")]
        public EnrichmentFileResourceTypeProperty[] Property
        {
            get { return propertyField; }
            set { propertyField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class EnrichmentFileResourceTypeProperty
    {
        #region Fields

        private string nameField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public partial class Item
    {
        #region Fields

        private ItemAlternateDescription[] alternateDescriptionField;
        private ItemAlternatePartNumber[] alternatePartNumberField;
        private ItemAttributeValue[] attributeValueField;
        private ItemCompetitorPartNumber[] competitorPartNumberField;
        private Enrichment enrichmentField;
        private ItemGlobalAttributes globalAttributesField;
        private Guid idField;
        private string itemIdField;
        private ItemNode[] nodeField;
        private string primaryDescriptionField;
        private ItemReference[] referencesField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlElement("AlternateDescription")]
        public ItemAlternateDescription[] AlternateDescription
        {
            get { return alternateDescriptionField; }
            set { alternateDescriptionField = value; }
        }

        /// <remarks />
        [XmlElement("AlternatePartNumber")]
        public ItemAlternatePartNumber[] AlternatePartNumber
        {
            get { return alternatePartNumberField; }
            set { alternatePartNumberField = value; }
        }

        /// <remarks />
        [XmlElement("AttributeValue")]
        public ItemAttributeValue[] AttributeValue
        {
            get { return attributeValueField; }
            set { attributeValueField = value; }
        }

        /// <remarks />
        [XmlElement("CompetitorPartNumber")]
        public ItemCompetitorPartNumber[] CompetitorPartNumber
        {
            get { return competitorPartNumberField; }
            set { competitorPartNumberField = value; }
        }

        /// <remarks />
        public Enrichment Enrichment
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        public ItemGlobalAttributes GlobalAttributes
        {
            get { return globalAttributesField; }
            set { globalAttributesField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public string ItemId
        {
            get { return itemIdField; }
            set { itemIdField = value; }
        }

        /// <remarks />
        [XmlElement("Node")]
        public ItemNode[] Node
        {
            get { return nodeField; }
            set { nodeField = value; }
        }

        /// <remarks />
        public string PrimaryDescription
        {
            get { return primaryDescriptionField; }
            set { primaryDescriptionField = value; }
        }

        /// <remarks />
        [XmlArrayItem("Reference", IsNullable = false)]
        public ItemReference[] References
        {
            get { return referencesField; }
            set { referencesField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class ItemAlternateDescription
    {
        #region Fields

        private string typeField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class ItemAlternatePartNumber
    {
        #region Fields

        private string typeField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class ItemAttributeValue
    {
        #region Fields

        private string groupField;
        private string nameField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlElement]
        public string Group
        {
            get { return groupField; }
            set { groupField = value; }
        }

        /// <remarks />
        [XmlElement]
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks />
        [XmlElement]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class ItemCompetitorPartNumber
    {
        #region Fields

        private string competitorField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string Competitor
        {
            get { return competitorField; }
            set { competitorField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class ItemGlobalAttributes
    {
        #region Fields

        private string brandNameField;
        private decimal listPriceField;
        private string manufacturerNameField;
        private double overallRatingField;
        private string packageQuantityField;
        private decimal salePriceField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string BrandName
        {
            get { return brandNameField; }
            set { brandNameField = value; }
        }

        /// <remarks />
        public decimal ListPrice
        {
            get { return listPriceField; }
            set { listPriceField = value; }
        }

        /// <remarks />
        public string ManufacturerName
        {
            get { return manufacturerNameField; }
            set { manufacturerNameField = value; }
        }

        /// <remarks />
        public double OverallRating
        {
            get { return overallRatingField; }
            set { overallRatingField = value; }
        }

        /// <remarks />
        public string PackageQuantity
        {
            get { return packageQuantityField; }
            set { packageQuantityField = value; }
        }

        /// <remarks />
        public decimal SalePrice
        {
            get { return salePriceField; }
            set { salePriceField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public partial class ItemNode
    {
        #region Fields

        private string breadCrumbField;
        private Guid catalogIdField;
        private Guid idField;
        private bool isPrimaryField;
        private string nodeNameField;
        private string standardsCodeNameField;

        #endregion Fields

        #region Constructors

        public ItemNode()
        {
            isPrimaryField = false;
        }

        #endregion Constructors

        #region Properties

        /// <remarks />
        public string BreadCrumb
        {
            get { return breadCrumbField; }
            set { breadCrumbField = value; }
        }

        /// <remarks />
        public Guid CatalogId
        {
            get { return catalogIdField; }
            set { catalogIdField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        [DefaultValue(false)]
        public bool IsPrimary
        {
            get { return isPrimaryField; }
            set { isPrimaryField = value; }
        }

        /// <remarks />
        public string NodeName
        {
            get { return nodeNameField; }
            set { nodeNameField = value; }
        }

        /// <remarks />
        public string StandardsCodeName
        {
            get { return standardsCodeNameField; }
            set { standardsCodeNameField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class ItemReference
    {
        #region Fields

        private string typeField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public partial class Node
    {
        #region Fields

        private string breadCrumbField;
        private Guid catalogIdField;
        private Enrichment enrichmentField;
        private Guid idField;
        private string nodeNameField;
        private Guid parentIdField;
        private NodeSchemaAttribute[] schemaAttributeField;
        private string[] searchPhraseField;
        private NodeSearchWord[] searchWordField;
        private string standardsCodeNameField;
        private string[] synonymField;
        private string[] taxonomyPathField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string BreadCrumb
        {
            get { return breadCrumbField; }
            set { breadCrumbField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid CatalogId
        {
            get { return catalogIdField; }
            set { catalogIdField = value; }
        }

        /// <remarks />
        public Enrichment Enrichment
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public string NodeName
        {
            get { return nodeNameField; }
            set { nodeNameField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid ParentId
        {
            get { return parentIdField; }
            set { parentIdField = value; }
        }

        /// <remarks />
        [XmlElement("SchemaAttribute")]
        public NodeSchemaAttribute[] SchemaAttribute
        {
            get { return schemaAttributeField; }
            set { schemaAttributeField = value; }
        }

        /// <remarks />
        [XmlElement("SearchPhrase")]
        public string[] SearchPhrase
        {
            get { return searchPhraseField; }
            set { searchPhraseField = value; }
        }

        /// <remarks />
        [XmlElement("SearchWord")]
        public NodeSearchWord[] SearchWord
        {
            get { return searchWordField; }
            set { searchWordField = value; }
        }

        /// <remarks />
        public string StandardsCodeName
        {
            get { return standardsCodeNameField; }
            set { standardsCodeNameField = value; }
        }

        /// <remarks />
        [XmlElement("Synonym")]
        public string[] Synonym
        {
            get { return synonymField; }
            set { synonymField = value; }
        }

        /// <remarks />
        [XmlElement("TaxonomyPath")]
        public string[] TaxonomyPath
        {
            get { return taxonomyPathField; }
            set { taxonomyPathField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public partial class NodeSchemaAttribute
    {
        #region Fields

        private string attributeGroupField;
        private string attributeNameField;
        private string dataTypeField;
        private decimal displayOrderField;
        private Enrichment enrichmentField;
        private decimal facetOrderField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string AttributeGroup
        {
            get { return attributeGroupField; }
            set { attributeGroupField = value; }
        }

        /// <remarks />
        public string AttributeName
        {
            get { return attributeNameField; }
            set { attributeNameField = value; }
        }

        /// <remarks />
        public string DataType
        {
            get { return dataTypeField; }
            set { dataTypeField = value; }
        }

        /// <remarks />
        public decimal DisplayOrder
        {
            get { return displayOrderField; }
            set { displayOrderField = value; }
        }

        /// <remarks />
        public Enrichment Enrichment
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        public decimal FacetOrder
        {
            get { return facetOrderField; }
            set { facetOrderField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class NodeSearchWord
    {
        #region Fields

        private int rankField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public int Rank
        {
            get { return rankField; }
            set { rankField = value; }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        #endregion Properties
    }

    // ReSharper restore LocalizableElement
    // ReSharper restore PartialTypeWithSinglePart
    // ReSharper restore InconsistentNaming
}