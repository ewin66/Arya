using System;
using System.ComponentModel;
using Natalie.Framework.ComponentModel;

namespace Natalie.Framework.Data
{
    [Serializable]
    public class UserProjectsPreferences
    {
        const String AttributePreferences = "Attribute Preferences";

        [Category(AttributePreferences), PropertyOrder(1)]
        [DefaultValue(false)]
        [DisplayName(@"Global Attributes"), Description("Global Attributes included in the User Preferences")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool GlobalAttributes { get; set; }

        [Category(AttributePreferences), PropertyOrder(2)]
        [DefaultValue(false)]
        [DisplayName(@"Extended Attributes"), Description("Extended Attributes included in the User Preferences")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExtendedAttributes { get; set; }

        [Category(AttributePreferences), PropertyOrder(3)]
        [DefaultValue(false)]
        [DisplayName(@"Ranked Attributes"), Description("Ranked Attributes included in the User Preferences")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool RankedAttributes { get; set; }

        [Category(AttributePreferences), PropertyOrder(4)]
        [DefaultValue(false)]
        [DisplayName(@"InSchema Attributes"), Description("InSchema Attributes included in the User Preferences")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool InSchemaAttributes { get; set; }

        [Category(AttributePreferences), PropertyOrder(5)]
        [DefaultValue(2000)]
        [DisplayName(@"Sku Count Threshold"), Description("Number of Skus Processed To Warrant Permission")]
        [TypeConverter(typeof(Int16Converter))]
        public int SkuThreshold { get; set; }

        [Category(AttributePreferences), PropertyOrder(6)]
        [DefaultValue(150)]
        [DisplayName(@"LOV Character Display Maximum"), Description("Maximum number of characters to display in LOV Schema View")]
        [TypeConverter(typeof(Int16Converter))]
        public int LovDisplayMax { get; set; }

        private string[] attributeGroupInclusions = new string[0];
        [Category(AttributePreferences), PropertyOrder(6)]
        [DisplayName(@"Attribute Groups"), Description("Chosen Attribute Groups included in the User Preferences")]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] AttributeGroupInclusions
        {
            get { return attributeGroupInclusions; }
            set { attributeGroupInclusions = value; }
        }

        private string[] attributeCustomInclusions = new string[0];
        [Category(AttributePreferences), PropertyOrder(7)]
        [DisplayName(@"Custom Included Attributes"), Description("Chosen Custom Attributes included in the User Preferences")]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] AttributeCustomInclusions
        {
            get { return attributeCustomInclusions; }
            set { attributeCustomInclusions = value; }
        }

        [Category(AttributePreferences), PropertyOrder(8)]
        [DefaultValue(false)]
        [DisplayName(@"Always See Show/Hide Attribute View"), Description("Toggle - Always see Show/Hide Attribute View Upon Opening Sku View")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool AlwaysStartSkusWithShowHide { get; set; }


        public UserProjectsPreferences()
        {
            SkuThreshold = 2000;
            LovDisplayMax = 150;
        }

        public void Run()
        {
        }

    }
}
