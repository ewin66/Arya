using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Arya.Framework.IO.Bridge.Audit
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
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
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
     XmlType(Namespace = "api.empiriSense.com/audit")]
    public class DerivedAttributeType
    {
        #region Fields

        private Guid attributeIdField;
        private DerivedAttributeTypeFormula formulaField;
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
        public DerivedAttributeTypeFormula Formula
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com"),
     XmlRoot("Enrichment", Namespace = "api.empiriSense.com", IsNullable = false)]
    public class Enrichment1
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class GroupMembership
    {
        #region Fields

        private Guid groupIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid GroupId
        {
            get { return groupIdField; }
            set { groupIdField = value; }
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
        private Enrichment1 enrichmentField;
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
        public Enrichment1 Enrichment
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
        public List<LangDependentMetaDataLov> ListOfValuess
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
        private Enrichment1 enrichmentField;
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
        public Enrichment1 Enrichment
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class ProductCatalogAuditTrail
    {
        #region Fields

        private Guid catalogIdField;
        private List<ProductCatalogAuditTrailRecord> productCatalogAuditTrailRecordField;

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
        [XmlElement("ProductCatalogAuditTrailRecord")]
        public List<ProductCatalogAuditTrailRecord> ProductCatalogAuditTrailRecords
        {
            get { return productCatalogAuditTrailRecordField; }
            set { productCatalogAuditTrailRecordField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class ProductCatalogAuditTrailRecord
    {
        #region Fields

        private TimestampRecordType auditTrailTimestampField;
        private string companyField;
        private List<ProductCatalogAuditTrailRecordProductCatalogName> productCatalogNameField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public TimestampRecordType AuditTrailTimestamp
        {
            get { return auditTrailTimestampField; }
            set { auditTrailTimestampField = value; }
        }

        /// <remarks />
        public string Company
        {
            get { return companyField; }
            set { companyField = value; }
        }

        /// <remarks />
        [XmlElement("ProductCatalogName")]
        public List<ProductCatalogAuditTrailRecordProductCatalogName> ProductCatalogNames
        {
            get { return productCatalogNameField; }
            set { productCatalogNameField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class ProductCatalogAuditTrailRecordProductCatalogName
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class SchemaAttributeAuditTrail
    {
        #region Fields

        private List<SchemaAttributeAuditTrailRecord> schemaAttributeAuditTrailRecordField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlElement("SchemaAttributeAuditTrailRecord")]
        public List<SchemaAttributeAuditTrailRecord> SchemaAttributeAuditTrailRecords
        {
            get { return schemaAttributeAuditTrailRecordField; }
            set { schemaAttributeAuditTrailRecordField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class SchemaAttributeAuditTrailRecord
    {
        #region Fields

        private Guid attributeIdField;
        private TimestampRecordType auditTrailTimestampField;
        private Guid idField;
        private SchemaAttributeAuditTrailRecordLangDependentMetaData langDependentMetaDataField;
        private SchemaAttributeAuditTrailRecordLangIndependentMetaData langIndependentMetaDataField;
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
        public TimestampRecordType AuditTrailTimestamp
        {
            get { return auditTrailTimestampField; }
            set { auditTrailTimestampField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        public SchemaAttributeAuditTrailRecordLangDependentMetaData LangDependentMetaData
        {
            get { return langDependentMetaDataField; }
            set { langDependentMetaDataField = value; }
        }

        /// <remarks />
        public SchemaAttributeAuditTrailRecordLangIndependentMetaData LangIndependentMetaData
        {
            get { return langIndependentMetaDataField; }
            set { langIndependentMetaDataField = value; }
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class SchemaAttributeAuditTrailRecordLangDependentMetaData
    {
        #region Fields

        private string definitionField;
        private Enrichment enrichmentField;
        private string langField;
        private List<SchemaAttributeAuditTrailRecordLangDependentMetaDataLov> listOfValuesField;
        private List<MetaDatumType> metaDatumField;
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
        public List<SchemaAttributeAuditTrailRecordLangDependentMetaDataLov> ListOfValuess
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class SchemaAttributeAuditTrailRecordLangDependentMetaDataLov
    {
        #region Fields

        private int displayOrderField;
        private bool displayOrderFieldSpecified;
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
        [XmlIgnore]
        public bool DisplayOrderSpecified
        {
            get { return displayOrderFieldSpecified; }
            set { displayOrderFieldSpecified = value; }
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class SchemaAttributeAuditTrailRecordLangIndependentMetaData
    {
        #region Fields

        private string dataTypeField;
        private int displayOrderField;
        private bool displayOrderFieldSpecified;
        private bool inSchemaField;
        private bool inSchemaFieldSpecified;
        private bool multivalueField;
        private bool multivalueFieldSpecified;
        private int navigationOrderField;
        private bool navigationOrderFieldSpecified;
        private bool requiredField;
        private bool requiredFieldSpecified;

        #endregion Fields

        #region Properties

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
        [XmlIgnore]
        public bool DisplayOrderSpecified
        {
            get { return displayOrderFieldSpecified; }
            set { displayOrderFieldSpecified = value; }
        }

        /// <remarks />
        public bool InSchema
        {
            get { return inSchemaField; }
            set { inSchemaField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool InSchemaSpecified
        {
            get { return inSchemaFieldSpecified; }
            set { inSchemaFieldSpecified = value; }
        }

        /// <remarks />
        public bool Multivalue
        {
            get { return multivalueField; }
            set { multivalueField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool MultivalueSpecified
        {
            get { return multivalueFieldSpecified; }
            set { multivalueFieldSpecified = value; }
        }

        /// <remarks />
        public int NavigationOrder
        {
            get { return navigationOrderField; }
            set { navigationOrderField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool NavigationOrderSpecified
        {
            get { return navigationOrderFieldSpecified; }
            set { navigationOrderFieldSpecified = value; }
        }

        /// <remarks />
        public bool Required
        {
            get { return requiredField; }
            set { requiredField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public bool RequiredSpecified
        {
            get { return requiredFieldSpecified; }
            set { requiredFieldSpecified = value; }
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class SkuAttribute
    {
        #region Fields

        private SkuAttributeAttributeDetails attributeDetailsField;
        private Guid attributeIdField;
        private Guid idField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public SkuAttributeAttributeDetails AttributeDetails
        {
            get { return attributeDetailsField; }
            set { attributeDetailsField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid AttributeId
        {
            get { return attributeIdField; }
            set { attributeIdField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid Id
        {
            get { return idField; }
            set { idField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class SkuAttributeAttributeDetails
    {
        #region Fields

        private string langField;
        private List<SkuAttributeAttributeDetailsValueElement> valueElementField;

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
        public List<SkuAttributeAttributeDetailsValueElement> ValueElements
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class SkuAttributeAttributeDetailsValueElement
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class SkuAuditTrail
    {
        #region Fields

        private List<SkuAuditTrailRecord> skuAuditTrailRecordField;
        private Guid skuIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlElement("SkuAuditTrailRecord")]
        public List<SkuAuditTrailRecord> SkuAuditTrailRecords
        {
            get { return skuAuditTrailRecordField; }
            set { skuAuditTrailRecordField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public Guid SkuId
        {
            get { return skuIdField; }
            set { skuIdField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class SkuAuditTrailRecord
    {
        #region Fields

        private TimestampRecordType auditTrailTimestampField;
        private List<CatalogRef> catalogRefField;
        private List<Enrichment> enrichmentField;
        private List<SkuAuditTrailRecordPrimarySemanticPhrase> primarySemanticPhraseField;
        private List<SkuAttribute> skuAttributeField;
        private List<SkuLink> skuLinkField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public TimestampRecordType AuditTrailTimestamp
        {
            get { return auditTrailTimestampField; }
            set { auditTrailTimestampField = value; }
        }

        /// <remarks />
        [XmlElement("CatalogRef")]
        public List<CatalogRef> CatalogRefs
        {
            get { return catalogRefField; }
            set { catalogRefField = value; }
        }

        /// <remarks />
        [XmlElement("Enrichment")]
        public List<Enrichment> Enrichments
        {
            get { return enrichmentField; }
            set { enrichmentField = value; }
        }

        /// <remarks />
        [XmlElement("PrimarySemanticPhrase")]
        public List<SkuAuditTrailRecordPrimarySemanticPhrase> PrimarySemanticPhrases
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

        /// <remarks />
        [XmlElement("SkuLink")]
        public List<SkuLink> SkuLinks
        {
            get { return skuLinkField; }
            set { skuLinkField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class SkuAuditTrailRecordPrimarySemanticPhrase
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public class SkuLink
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
     XmlType(Namespace = "api.empiriSense.com/audit")]
    public class SkuQueryType
    {
        #region Fields

        private SelectionCriteriaType selectionCriteriaField;
        private SkuQueryTypeSourceNode sourceNodeField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public SelectionCriteriaType SelectionCriteria
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public partial class TaxonomyNodeAuditTrail
    {
        #region Fields

        private Guid nodeIdField;
        private List<TaxonomyNodeAuditTrailRecord> taxonomyNodeAuditTrailRecordField;

        #endregion Fields

        #region Properties

        /// <remarks />
        [XmlAttribute]
        public Guid NodeId
        {
            get { return nodeIdField; }
            set { nodeIdField = value; }
        }

        /// <remarks />
        [XmlElement("TaxonomyNodeAuditTrailRecord")]
        public List<TaxonomyNodeAuditTrailRecord> TaxonomyNodeAuditTrailRecords
        {
            get { return taxonomyNodeAuditTrailRecordField; }
            set { taxonomyNodeAuditTrailRecordField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public partial class TaxonomyNodeAuditTrailRecord
    {
        #region Fields

        private TimestampRecordType auditTrailTimestampField;
        private DerivedAttributeType derivedAttributeDefinitionField;
        private SkuQueryType derivedNodeDefinitionField;
        private TaxonomyNodeAuditTrailRecordHierarchy hierarchyField;
        private TaxonomyNodeDescriptor taxonomyNodeDescriptorField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public TimestampRecordType AuditTrailTimestamp
        {
            get { return auditTrailTimestampField; }
            set { auditTrailTimestampField = value; }
        }

        /// <remarks />
        public DerivedAttributeType DerivedAttributeDefinition
        {
            get { return derivedAttributeDefinitionField; }
            set { derivedAttributeDefinitionField = value; }
        }

        /// <remarks />
        public SkuQueryType DerivedNodeDefinition
        {
            get { return derivedNodeDefinitionField; }
            set { derivedNodeDefinitionField = value; }
        }

        /// <remarks />
        public TaxonomyNodeAuditTrailRecordHierarchy Hierarchy
        {
            get { return hierarchyField; }
            set { hierarchyField = value; }
        }

        /// <remarks />
        public TaxonomyNodeDescriptor TaxonomyNodeDescriptor
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
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit")]
    public class TaxonomyNodeAuditTrailRecordHierarchy
    {
        #region Fields

        private Guid catalogIdField;
        private Guid parentIdField;

        #endregion Fields

        #region Properties

        /// <remarks />
        public Guid CatalogId
        {
            get { return catalogIdField; }
            set { catalogIdField = value; }
        }

        /// <remarks />
        public Guid ParentId
        {
            get { return parentIdField; }
            set { parentIdField = value; }
        }

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(AnonymousType = true, Namespace = "api.empiriSense.com/audit"),
     XmlRoot(Namespace = "api.empiriSense.com/audit", IsNullable = false)]
    public partial class TaxonomyNodeDescriptor
    {
        #region Fields

        private Enrichment enrichmentField;
        private string langField;
        private List<MetaDatumType> metaDatumField;
        private string nodeDescriptionField;
        private string nodeNameField;

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

        #endregion Properties
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.33440"),
     Serializable,
     DebuggerStepThrough,
     DesignerCategory("code"),
     XmlType(Namespace = "api.empiriSense.com")]
    public class TimestampRecordType
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
    public class User
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