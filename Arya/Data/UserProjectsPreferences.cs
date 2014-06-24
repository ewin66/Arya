using System;
using System.ComponentModel;
using System.Windows.Forms;
using Arya.Converters;
using Arya.Framework.Common.ComponentModel;

namespace Arya.Data
{
    [Serializable]
    public class UserProjectsPreferences
    {
        private const String SkuViewUserPreferences = "SKU View";
        private const String SchemaViewUserPreferences = "Schema View";
        private string[] _attributeCustomInclusions = new string[0];
        private string[] _attributeGroupInclusions = new string[0];
        private string[] _schemaColumnWidths = new string[0];
        private int _tabThreshold = 10;

        public UserProjectsPreferences()
        {
            SkuThreshold = 2000;
            TabsThreshold = 10;
            LovDisplayMax = 150;
            AttributeOrderBy = "Attribute Name";
        }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(1)]
        [DefaultValue(false)]
        [DisplayName(@"Show All Global Attributes")]
        [Description("When a single node is opened in SKU View, all global attributes will be displayed.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool GlobalAttributes { get; set; }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(2)]
        [DefaultValue(false)]
        [DisplayName(@"Show All Product Attributes")]
        [Description("When a single node is opened in SKU View, all Product attributes will be displayed.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ProductAttributes { get; set; }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(3)]
        [DefaultValue(false)]
        [DisplayName(@"Show All Ranked Attributes")]
        [Description(
            "When a single node is opened in SKU View, all attributes with either a navigation order or a display order will be displayed."
            )]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool RankedAttributes { get; set; }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(4)]
        [DefaultValue(false)]
        [DisplayName(@"Show All In Schema Attributes")]
        [Description("When a single node is opened in SKU View, all In Schema attributes will be displayed.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool InSchemaAttributes { get; set; }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(5)]
        [DefaultValue(2000)]
        [DisplayName(@"Warn if the number of SKUs exceeds")]
        [Description(
            "If the number of SKUs in the node(s) being opened exceeds the amount specified, the system will prompt you whether or not you want to continue loading the SKUs or abort the open operation."
            )]
        [TypeConverter(typeof (Int16Converter))]
        public int SkuThreshold { get; set; }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(6)]
        [DefaultValue(10)]
        [DisplayName(@"Warn if the number of tabs exceeds")]
        [Description(
            "If the number of tabs being opened exceeds the amount specified, the system will prompt you whether or not you want to continue loading the tabs or abort the open operation."
            )]
        [TypeConverter(typeof (Int16Converter))]
        public int TabsThreshold
        {
            get { return _tabThreshold; }
            set
            {
                if (value < 1)
                    MessageBox.Show(String.Format("Threshold value cannot be less than 1."));
                else
                    _tabThreshold = value;
            }
        }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(7)]
        [DisplayName(@"Show Attributes in These Attribute Groups")]
        [Description(
            "When a single node is opened in SKU View, all attributes that belong to the listed project level attribute groups will be displayed."
            )]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] AttributeGroupInclusions
        {
            get { return _attributeGroupInclusions; }
            set { _attributeGroupInclusions = value; }
        }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(8)]
        [DisplayName(@"Show Attributes in This List")]
        [Description(
            "When a single node is opened in SKU View, all attributes that belong to your custom list of attributes will be displayed."
            )]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] AttributeCustomInclusions
        {
            get { return _attributeCustomInclusions; }
            set { _attributeCustomInclusions = value; }
        }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(9)]
        [DefaultValue(false)]
        [DisplayName(@"Automatically Display Show/Hide Attributes")]
        [Description(
            "When nodes are opened in SKU View, the Show/Hide Attributes Window will automatically display after the node is loaded."
            )]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool AlwaysStartSkusWithShowHide { get; set; }

        [Category(SkuViewUserPreferences)]
        [PropertyOrder(10)]
        [DefaultValue(false)]
        [DisplayName(@"Validate Attribute Values")]
        [Description("When set to Yes, SKU View is opened with Validation enabled.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool ValidateAttributeValues { get; set; }

        [Category(SchemaViewUserPreferences)]
        [PropertyOrder(11)]
        [DefaultValue(150)]
        [DisplayName(@"LOV Character Display Maximum")]
        [Description("The maximum number of characters to display in the List of Values meta-attribute.")]
        [TypeConverter(typeof (Int16Converter))]
        public int LovDisplayMax { get; set; }

        [Category(SchemaViewUserPreferences)]
        [PropertyOrder(12)]
        [DefaultValue("Attribute Name")]
        [DisplayName(@"Order Attributes By")]
        [Description("Order the attributes in Schema View by the selected meta-attribute.")]
        [TypeConverter(typeof (UserPreferenceAttributeOrderConverter))]
        public String AttributeOrderBy { get; set; }

        [Category(SchemaViewUserPreferences)]
        [PropertyOrder(13)]
        [DisplayName(@"Auto-Order Ranks")]
        [Description("When set to Yes, ranks are automatically ordered when Schema View is opened.")]
        [TypeConverter(typeof (BooleanToYesNoConverter))]
        public bool AutoOrderRanks { get; set; }

        [Category(SchemaViewUserPreferences)]
        [PropertyOrder(14)]
        [DisplayName(@"Schema Column Widths")]
        [Description(
            "Width of each column in Schema View. Each value is pipe delimited - first value is width, second values is Schema Meta Attribute Name"
            )]
        [TypeConverter(typeof (StringArrayConverter))]
        public string[] SchemaColumnWidths
        {
            get { return _schemaColumnWidths; }
            set { _schemaColumnWidths = value; }
        }

        public void Run() { }
    }
}