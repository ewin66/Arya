using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Arya.Framework.IO.Bridge
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable RedundantAttributeParentheses
    // ReSharper disable LocalizableElement
    // ReSharper disable UnusedMember.Global
    // ReSharper disable PartialTypeWithSinglePart

    #region Enumerations

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public enum TaxonomyNodeFlag
    {
        /// <remarks />
        Temporary,
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public enum TimestampRecordTypeActionType
    {
        /// <remarks />
        Created,

        /// <remarks />
        Updated,

        /// <remarks />
        Deleted,
    }

    #endregion Enumerations

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"), Serializable, DebuggerStepThrough, DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public class Attribute
    {
        #region Fields

        private string dataTypeField;
        private Guid idField;
        private Guid catalogIdField;
        private List<LangDependentMetaData> langDependentMetaDataField;
        private List<AttributeName> nameField;
        private bool readonlyField;
        private string typeField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string DataType
        {
            get { return dataTypeField; }
            set { dataTypeField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        [XmlAttribute]
        public Guid CatalogId
        {
            get { return catalogIdField; }
            set { catalogIdField = value; }
        }

        /// <remarks />
        [XmlElement("LangDependentMetaData")]
        public List<LangDependentMetaData> LangDependentMetaData
        {
            get { return langDependentMetaDataField; }
            set { langDependentMetaDataField = value; }
        }

        /// <remarks />
        [XmlElement("Name")]
        public List<AttributeName> Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks />
        public bool Readonly
        {
            get { return readonlyField; }
            set { readonlyField = value; }
        }

        /// <remarks />
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"), Serializable, DebuggerStepThrough, DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public class AttributeGroup
    {
        #region Fields

        private List<string> attributeIdField;
        private List<AttributeGroupGroupName> groupNameField;
        private string idField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlElement("AttributeId")]
        public List<string> AttributeId
        {
            get { return attributeIdField; }
            set { attributeIdField = value; }
        }

        /// <remarks />
        [XmlElement("GroupName")]
        public List<AttributeGroupGroupName> GroupName
        {
            get { return groupNameField; }
            set { groupNameField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Id
        {
            get { return idField; }
            set { idField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"), Serializable, DebuggerStepThrough, DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class AttributeGroupGroupName
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
     XmlType(Namespace = "api.empiriSense.com")]
    public class AttributeMetaDatumType
    {
        #region Fields

        private int displayOrderField;
        private Guid idField;
        private string nameField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public int DisplayOrder
        {
            get { return displayOrderField; }
            set { displayOrderField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"), Serializable, DebuggerStepThrough, DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class AttributeName
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
    public class CatalogRef
    {
        #region Fields

        private Guid catalogIdField;
        private bool primaryField;
        private Guid taxonomyNodeIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid CatalogId
        {
            get { return catalogIdField; }
            set { catalogIdField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public bool Primary
        {
            get { return primaryField; }
            set { primaryField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid TaxonomyNodeId
        {
            get { return taxonomyNodeIdField; }
            set { taxonomyNodeIdField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(Namespace = "api.empiriSense.com")]
    public class DerivedAttributeType
    {
        #region Fields

        private Guid attributeIdField;
        private List<DerivedAttributeTypeFormula> formulaField;
        private Guid idField;
        private int maxResultsLengthField;
        private bool maxResultsLengthFieldSpecified;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid AttributeId
        {
            get { return attributeIdField; }
            set { attributeIdField = value; }
        }

        /// <remarks />
        [XmlElement("Formula")]
        public List<DerivedAttributeTypeFormula> Formulas
        {
            get { return formulaField; }
            set { formulaField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public int MaxResultsLength
        {
            get { return maxResultsLengthField; }
            set { maxResultsLengthField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool MaxResultsLengthSpecified
        {
            get { return maxResultsLengthFieldSpecified; }
            set { maxResultsLengthFieldSpecified = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class DerivedAttributeTypeFormula
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
        private List<EnrichmentFileResourceType> enrichmentFileResourceField;
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
        public List<EnrichmentFileResourceType> EnrichmentFileResources
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
        private List<EnrichmentFileResourceTypeProperty> propertyField;
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
        public List<EnrichmentFileResourceTypeProperty> Propertys
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
    public class LangDependentMetaData
    {
        #region Fields

        private string definitionField;
        private Enrichment enrichmentField;
        private string langField;
        private List<LangDependentMetaDataLov> listOfValuesField;
        private List<MetaDatumType> metaDatumField;
        private List<UserCommentsRecord> notesField;
        private string sampleValuesField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string Definition
        {
            get { return definitionField; }
            set { definitionField = value; }
        }

        /// <remarks />
        public Enrichment Enrichment
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks />
        [XmlArrayItem("Lov", IsNullable = false)]
        public List<LangDependentMetaDataLov> ListOfValues
        {
            get { return listOfValuesField; }
            set { listOfValuesField = value; }
        }

        /// <remarks />
        [XmlElement("MetaDatum")]
        public List<MetaDatumType> MetaDatums
        {
            get { return metaDatumField; }
            set { metaDatumField = value; }
        }

        /// <remarks />
        [XmlElement("Notes")]
        public List<UserCommentsRecord> Notess
        {
            get { return notesField; }
            set { notesField = value; }
        }

        /// <remarks />
        public string SampleValues
        {
            get { return sampleValuesField; }
            set { sampleValuesField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class LangDependentMetaDataLov
    {
        #region Fields

        private int displayOrderField;
        private Enrichment enrichmentField;
        private Guid idField;
        private string parentValueField;
        private MeasuredValueType valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public int DisplayOrder
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
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public string ParentValue
        {
            get { return parentValueField; }
            set { parentValueField = value; }
        }

        /// <remarks />
        public MeasuredValueType Value
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
     XmlType(Namespace = "api.empiriSense.com")]
    public class MeasuredValueType
    {
        #region Fields

        private string uoMField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string UoM
        {
            get { return uoMField; }
            set { uoMField = value; }
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
     XmlType(Namespace = "api.empiriSense.com")]
    public class MetaDatumType
    {
        #region Fields

        private Guid idField;
        private string nameField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

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
    public class ProductCatalog
    {
        #region Fields

        private string companyField;
        private Guid idField;
        private String typeField;
        private TimestampRecordType lastUpdatedTimestampField;
        private List<ProductCatalogProductCatalogName> productCatalogNameField;
        private List<ProductCatalogSchemaMetaDataLanguageVersions> schemaMetaDataLanguageVersionsField;
        private List<ProductCatalogTaxonomyMetaDataLanguageVersions> taxonomyMetaDataLanguageVersionsField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public string Company
        {
            get { return companyField; }
            set { companyField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public String Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks />
        public TimestampRecordType LastUpdatedTimestamp
        {
            get { return lastUpdatedTimestampField; }
            set { lastUpdatedTimestampField = value; }
        }

        /// <remarks />
        [XmlElement("ProductCatalogName")]
        public List<ProductCatalogProductCatalogName> ProductCatalogNames
        {
            get { return productCatalogNameField; }
            set { productCatalogNameField = value; }
        }

        /// <remarks />
        [XmlElement("SchemaMetaDataLanguageVersions")]
        public List<ProductCatalogSchemaMetaDataLanguageVersions> SchemaMetaDataLanguageVersionss
        {
            get { return schemaMetaDataLanguageVersionsField; }
            set { schemaMetaDataLanguageVersionsField = value; }
        }

        /// <remarks />
        [XmlElement("TaxonomyMetaDataLanguageVersions")]
        public List<ProductCatalogTaxonomyMetaDataLanguageVersions> TaxonomyMetaDataLanguageVersionss
        {
            get { return taxonomyMetaDataLanguageVersionsField; }
            set { taxonomyMetaDataLanguageVersionsField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public partial class ProductCatalogProductCatalogName
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
    public partial class ProductCatalogSchemaMetaDataLanguageVersions
    {
        #region Fields

        private string langField;
        private List<AttributeMetaDatumType> schemaAttributeMetaDataField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks />
        [XmlElement("SchemaAttributeMetaData")]
        public List<AttributeMetaDatumType> SchemaAttributeMetaDatas
        {
            get { return schemaAttributeMetaDataField; }
            set { schemaAttributeMetaDataField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public partial class ProductCatalogTaxonomyMetaDataLanguageVersions
    {
        #region Fields

        private string langField;
        private List<AttributeMetaDatumType> taxonomyNodeAttributeMetaDataField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks />
        [XmlElement("TaxonomyNodeAttributeMetaData")]
        public List<AttributeMetaDatumType> TaxonomyNodeAttributeMetaDatas
        {
            get { return taxonomyNodeAttributeMetaDataField; }
            set { taxonomyNodeAttributeMetaDataField = value; }
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
    public class SchemaAttribute
    {
        #region Fields

        private Guid attributeIdField;
        private string dataTypeField;
        private int displayOrderField;
        private Guid idField;
        private bool inSchemaField;
        private List<LangDependentMetaData> langDependentMetaDataField;
        private List<string> restrictedUom; 
        private TimestampRecordType lastUpdatedTimestampField;
        private bool multivalueField;
        private bool complexField;
        private int navigationOrderField;
        private bool requiredField;
        private Guid taxonomyNodeIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid AttributeId
        {
            get { return attributeIdField; }
            set { attributeIdField = value; }
        }

        /// <remarks />
        public string DataType
        {
            get { return dataTypeField; }
            set { dataTypeField = value; }
        }

        /// <remarks />
        public int DisplayOrder
        {
            get { return displayOrderField; }
            set { displayOrderField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public bool InSchema
        {
            get { return inSchemaField; }
            set { inSchemaField = value; }
        }

        /// <remarks />
        [XmlElement("LangDependentMetaData")]
        public List<LangDependentMetaData> LangDependentMetaDatas
        {
            get { return langDependentMetaDataField; }
            set { langDependentMetaDataField = value; }
        }

        /// <remarks />
        public TimestampRecordType LastUpdatedTimestamp
        {
            get { return lastUpdatedTimestampField; }
            set { lastUpdatedTimestampField = value; }
        }

        /// <remarks />
        public bool Multivalue
        {
            get { return multivalueField; }
            set { multivalueField = value; }
        }

        /// <remarks />
        public bool Complex
        {
            get { return complexField; }
            set { complexField = value; }
        }

        /// <remarks />
        public int NavigationOrder
        {
            get { return navigationOrderField; }
            set { navigationOrderField = value; }
        }

        /// <remarks />
        public bool Required
        {
            get { return requiredField; }
            set { requiredField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid TaxonomyNodeId
        {
            get { return taxonomyNodeIdField; }
            set { taxonomyNodeIdField = value; }
        }

        public List<string> RestrictedUom
        {
            get { return restrictedUom; }
            set { restrictedUom = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(Namespace = "api.empiriSense.com")]
    public class SelectionCriteriaType
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
    public class Sku
    {
        #region Fields

        private List<CatalogRef> classificationField;
        private List<Enrichment> enrichmentField;
        private Guid idField;
        private TimestampRecordType lastUpdatedTimestampField;
        private SkuSkuAttributes skuAttributesField;
        private List<SkuSkuLink> skuLinkField;
        private string typeField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlArrayItem("CatalogRef", IsNullable = false)]
        public List<CatalogRef> Classification
        {
            get { return classificationField; }
            set { classificationField = value; }
        }

        /// <remarks />
        [XmlElement("Enrichment")]
        public List<Enrichment> Enrichments
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
        public TimestampRecordType LastUpdatedTimestamp
        {
            get { return lastUpdatedTimestampField; }
            set { lastUpdatedTimestampField = value; }
        }

        /// <remarks />
        public SkuSkuAttributes SkuAttributes
        {
            get { return skuAttributesField; }
            set { skuAttributesField = value; }
        }

        /// <remarks />
        [XmlElement("SkuLink")]
        public List<SkuSkuLink> SkuLinks
        {
            get { return skuLinkField; }
            set { skuLinkField = value; }
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public class SkuAttribute
    {
        #region Fields

        private Guid attributeIdField;
        private List<SkuAttributeAttributeValues> attributeValuesField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid AttributeId
        {
            get { return attributeIdField; }
            set { attributeIdField = value; }
        }

        /// <remarks />
        [XmlElement("AttributeValues")]
        public List<SkuAttributeAttributeValues> AttributeValuess
        {
            get { return attributeValuesField; }
            set { attributeValuesField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class SkuAttributeAttributeValues
    {
        #region Fields

        private string langField;
        private List<SkuAttributeAttributeValuesValueElement> valueElementField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks />
        [XmlElement("ValueElement")]
        public List<SkuAttributeAttributeValuesValueElement> ValueElements
        {
            get { return valueElementField; }
            set { valueElementField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class SkuAttributeAttributeValuesValueElement
    {
        #region Fields

        private Guid idField;
        private List<MetaDatumType> metaDatumField;
        private string statusFlagField;
        private MeasuredValueType valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        [XmlElement("MetaDatum")]
        public List<MetaDatumType> MetaDatums
        {
            get { return metaDatumField; }
            set { metaDatumField = value; }
        }

        /// <remarks />
        public string StatusFlag
        {
            get { return statusFlagField; }
            set { statusFlagField = value; }
        }

        /// <remarks />
        public MeasuredValueType Value
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
     XmlType(Namespace = "api.empiriSense.com")]
    public class SkuQueryType
    {
        #region Fields

        private List<SelectionCriteriaType> selectionCriteriaField;
        private SkuQueryTypeSourceNode sourceNodeField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlElement("SelectionCriteria")]
        public List<SelectionCriteriaType> SelectionCriterias
        {
            get { return selectionCriteriaField; }
            set { selectionCriteriaField = value; }
        }

        /// <remarks />
        public SkuQueryTypeSourceNode SourceNode
        {
            get { return sourceNodeField; }
            set { sourceNodeField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class SkuQueryTypeSourceNode
    {
        #region Fields

        private bool includeSubnodesField;
        private bool includeSubnodesFieldSpecified;
        private Guid nodeIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public bool IncludeSubnodes
        {
            get { return includeSubnodesField; }
            set { includeSubnodesField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool IncludeSubnodesSpecified
        {
            get { return includeSubnodesFieldSpecified; }
            set { includeSubnodesFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid NodeId
        {
            get { return nodeIdField; }
            set { nodeIdField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class SkuSkuAttributes
    {
        #region Fields

        private string itemIdField;
        private List<SkuSkuAttributesPrimarySemanticPhrase> primarySemanticPhraseField;
        private List<SkuAttribute> skuAttributeField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string ItemId
        {
            get { return itemIdField; }
            set { itemIdField = value; }
        }

        /// <remarks />
        [XmlElement("PrimarySemanticPhrase")]
        public List<SkuSkuAttributesPrimarySemanticPhrase> PrimarySemanticPhrases
        {
            get { return primarySemanticPhraseField; }
            set { primarySemanticPhraseField = value; }
        }

        /// <remarks />
        [XmlElement("SkuAttribute")]
        public List<SkuAttribute> SkuAttributes
        {
            get { return skuAttributeField; }
            set { skuAttributeField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public partial class SkuSkuAttributesPrimarySemanticPhrase
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
    public class SkuSkuLink
    {
        #region Fields

        private string linkTypeField;
        private Guid linkedSkuIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public Guid LinkedSkuId
        {
            get { return linkedSkuIdField; }
            set { linkedSkuIdField = value; }
        }

        /// <remarks />
        public string LinkType
        {
            get { return linkTypeField; }
            set { linkTypeField = value; }
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
    public partial class TaxonomyNode
    {
        #region Fields

        private Guid catalogIdField;
        private List<TaxonomyNodeDerivedAttributeDefinition> derivedAttributeDefinitionField;
        private SkuQueryType derivedNodeDefinitionField;
        private TaxonomyNodeFlag flagField;
        private bool flagFieldSpecified;
        private Guid idField;
        private bool isRootField;
        private TimestampRecordType lastUpdatedTimestampField;
        private int mappedSkuCountField;
        private bool mappedSkuCountFieldSpecified;
        private int mappedSkuCountWithChildrenField;
        private bool mappedSkuCountWithChildrenFieldSpecified;
        private Guid parentIdField;
        private int skuCountField;
        private bool skuCountFieldSpecified;
        private int skuCountWithChildrenField;
        private bool skuCountWithChildrenFieldSpecified;
        private List<TaxonomyNode> taxonomyNode1Field;
        private List<TaxonomyNodeDescriptor> taxonomyNodeDescriptorField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid CatalogId
        {
            get { return catalogIdField; }
            set { catalogIdField = value; }
        }

        /// <remarks />
        [XmlElement("DerivedAttributeDefinition")]
        public List<TaxonomyNodeDerivedAttributeDefinition> DerivedAttributeDefinitions
        {
            get { return derivedAttributeDefinitionField; }
            set { derivedAttributeDefinitionField = value; }
        }

        /// <remarks />
        [XmlElement("DerivedNodeDefinition")]
        public SkuQueryType DerivedNodeDefinition
        {
            get { return derivedNodeDefinitionField; }
            set { derivedNodeDefinitionField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public TaxonomyNodeFlag Flag
        {
            get { return flagField; }
            set { flagField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool FlagSpecified
        {
            get { return flagFieldSpecified; }
            set { flagFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public bool IsRoot
        {
            get { return isRootField; }
            set { isRootField = value; }
        }

        /// <remarks />
        public TimestampRecordType LastUpdatedTimestamp
        {
            get { return lastUpdatedTimestampField; }
            set { lastUpdatedTimestampField = value; }
        }

        /// <remarks />
        public int MappedSkuCount
        {
            get { return mappedSkuCountField; }
            set { mappedSkuCountField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool MappedSkuCountSpecified
        {
            get { return mappedSkuCountFieldSpecified; }
            set { mappedSkuCountFieldSpecified = value; }
        }

        /// <remarks />
        public int MappedSkuCountWithChildren
        {
            get { return mappedSkuCountWithChildrenField; }
            set { mappedSkuCountWithChildrenField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool MappedSkuCountWithChildrenSpecified
        {
            get { return mappedSkuCountWithChildrenFieldSpecified; }
            set { mappedSkuCountWithChildrenFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid ParentId
        {
            get { return parentIdField; }
            set { parentIdField = value; }
        }

        /// <remarks />
        public int SkuCount
        {
            get { return skuCountField; }
            set { skuCountField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool SkuCountSpecified
        {
            get { return skuCountFieldSpecified; }
            set { skuCountFieldSpecified = value; }
        }

        /// <remarks />
        public int SkuCountWithChildren
        {
            get { return skuCountWithChildrenField; }
            set { skuCountWithChildrenField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool SkuCountWithChildrenSpecified
        {
            get { return skuCountWithChildrenFieldSpecified; }
            set { skuCountWithChildrenFieldSpecified = value; }
        }

        /// <remarks />
        [XmlElement("TaxonomyNode")]
        public List<TaxonomyNode> TaxonomyNode1s
        {
            get { return taxonomyNode1Field; }
            set { taxonomyNode1Field = value; }
        }

        /// <remarks />
        [XmlElement("TaxonomyNodeDescriptor")]
        public List<TaxonomyNodeDescriptor> TaxonomyNodeDescriptors
        {
            get { return taxonomyNodeDescriptorField; }
            set { taxonomyNodeDescriptorField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class TaxonomyNodeDerivedAttributeDefinition : DerivedAttributeType
    {
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot(Namespace = "api.empiriSense.com", IsNullable = false)]
    public partial class TaxonomyNodeDescriptor
    {
        #region Fields

        private Enrichment enrichmentField;
        private string langField;
        private List<MetaDatumType> metaDatumField;
        private string nodeDescriptionField;
        private string nodeNameField;
        private List<UserCommentsRecord> notesField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public Enrichment Enrichment
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks />
        [XmlElement("MetaDatum")]
        public List<MetaDatumType> MetaDatums
        {
            get { return metaDatumField; }
            set { metaDatumField = value; }
        }

        /// <remarks />
        public string NodeDescription
        {
            get { return nodeDescriptionField; }
            set { nodeDescriptionField = value; }
        }

        /// <remarks />
        public string NodeName
        {
            get { return nodeNameField; }
            set { nodeNameField = value; }
        }

        /// <remarks />
        [XmlElement("Notes")]
        public List<UserCommentsRecord> Notess
        {
            get { return notesField; }
            set { notesField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(Namespace = "api.empiriSense.com")]
    public partial class TimestampRecordType
    {
        #region Fields

        private TimestampRecordTypeActionType actionTypeField;
        private TimestampRecordTypeRemark remarkField;
        private DateTime timestampField;
        private User userField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public TimestampRecordTypeActionType ActionType
        {
            get { return actionTypeField; }
            set { actionTypeField = value; }
        }

        /// <remarks />
        public TimestampRecordTypeRemark Remark
        {
            get { return remarkField; }
            set { remarkField = value; }
        }

        /// <remarks />
        public DateTime Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }

        /// <remarks />
        public User User
        {
            get { return userField; }
            set { userField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com")]
    public class TimestampRecordTypeRemark
    {
        #region Fields

        private string langField;
        private string valueField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
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
    public partial class User
    {
        #region Fields

        private string firstNameField;
        private string lastNameField;
        private Guid userIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string FirstName
        {
            get { return firstNameField; }
            set { firstNameField = value; }
        }

        /// <remarks />
        public string LastName
        {
            get { return lastNameField; }
            set { lastNameField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid UserId
        {
            get { return userIdField; }
            set { userIdField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(Namespace = "api.empiriSense.com")]
    public class UserCommentsRecord
    {
        #region Fields

        private string commentsField;
        private DateTime timestampField;
        private User userField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks />
        public DateTime Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }

        /// <remarks />
        public User User
        {
            get { return userField; }
            set { userField = value; }
        }

        #endregion Properties
    }

    // ReSharper restore PartialTypeWithSinglePart
    // ReSharper restore UnusedMember.Global
    // ReSharper restore LocalizableElement
    // ReSharper restore RedundantAttributeParentheses
    // ReSharper restore InconsistentNaming
}