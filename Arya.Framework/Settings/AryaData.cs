namespace Arya.Framework.Settings
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class AryaProject
    {
        [XmlAttribute]
        public string ProjectName;

        [XmlArrayItem]
        public AryaTaxonomy[] Taxonomy;
    }

    [Serializable]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class AryaTaxonomy
    {
        [XmlArrayItem]
        public AryaItem[] Items;
        
        [XmlAttribute]
        public string NodeName;

        [XmlArrayItem]
        public AryaSchema[] Schemas;

        [XmlArrayItem]
        public AryaTaxonomy[] Taxonomies;

        [XmlAttribute]
        public string Copy;

        [XmlAttribute]
        public string Image;
    }

    [Serializable]
    [XmlRoot(Namespace = "", IsNullable = true)]
    public class AryaItem
    {
        [XmlArrayItem]
        public AryaItemData[] ItemDatas;
        
        [XmlAttribute]
        public string ItemId;
    }

    [Serializable]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class AryaItemData
    {
        [XmlAttribute]
        public string AttributeName;

        [XmlAttribute]
        public string Uom;
        
        [XmlAttribute]
        public string Value;
    }

    [Serializable]
    [XmlRoot(Namespace = "", IsNullable = true)]
    public class AryaSchema
    {
        [XmlAttribute]
        public string AttributeName;

        [XmlAttribute]
        public string DataType;

        [XmlAttribute]
        public string DisplayOrder;

        [XmlAttribute]
        public string NavigationalOrder;

        [XmlArrayItem]
        public AryaSchemaData[] SchemaDatas;

        [XmlArrayItem]
        public AryaLov[] ListOfValues;
    }

    [Serializable]
    public class AryaLov
    {
        [XmlAttribute]
        public string Value;

        [XmlAttribute]
        public string DisplayOrder;

        [XmlAttribute]
        public string Copy;

        [XmlAttribute]
        public string Image;
    }

    [Serializable]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class AryaSchemaData
    {
        [XmlAttribute]
        public string MetaAttributeName;

        [XmlAttribute]
        public string Value;
    }
}