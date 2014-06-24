using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Extensions;

namespace Arya.Portal.SupportingClasses
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.HtmlControls;

    //  ------------------------------------
    //  .css sample
    //
    //.PropertyGrid
    //{
    //}
    //.PropertyNestedGrid
    //{
    //    border: 1px solid #ccc;
    //}
    //.PropertyGridCategory
    //{
    //    /*border: 1px solid #ccc;*/
    //}
    //.CategoryName
    //{
    //    font-weight: bold;
    //}
    //.PropertyGridPropertyRow
    //{
    //}
    //.PropertyName
    //{
    //}
    //.PropertyValue
    //{
    //}
    //.PropertyGridPropertyLink
    //{
    //}
    //  ------------------------------------
    [ToolboxData("<{0}:ObjectWebEditor runat=server></{0}:ObjectWebEditor>"),
    Designer(typeof(ObjectWebEditorDesigner)),
    ParseChildren(true, "PropertyMaps")]
    public sealed class ObjectWebEditor : Control, INamingContainer
    {
        #region Fields

        private readonly ArrayList _catlist = new ArrayList();
        private readonly Hashtable _properties = new Hashtable();
        private readonly ArrayList _proplist = new ArrayList();

        private int _catcounter;
        private int _itemcounter;
        private ArrayList _propertyMaps;
        private object _selectedObject;
        private bool _showCategoryLabels = true;
        private int _subcounter;
        private string _hiddenProperties;

        #endregion Fields

        #region Constructors

        public ObjectWebEditor()
        {
            ReadOnly = false;
            HideReadOnly = false;
            HiddenProperties = null;
        }

        #endregion Constructors

        #region Properties

        [Browsable(false)]
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set { base.EnableViewState = false; }
        }

        [Category("Behavior")]
        public string HiddenCategories { get; set; }

        [Category("Behavior")]
        public string HiddenProperties
        {
            get { return _hiddenProperties; }
            set { _hiddenProperties = value; }
        }

        [DefaultValue(false),
        Category("Behavior")]
        public bool HideReadOnly
        {
            get;
            set;
        }

        [Category("Behavior"),
        Description("PropertyMap collection"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(typeof(PropertyCollectionEditor), typeof(UITypeEditor)),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ArrayList PropertyMaps
        {
            get { return _propertyMaps ?? (_propertyMaps = new ArrayList()); }
        }

        [DefaultValue(false),
        Category("ReadOnly")]
        public bool ReadOnly
        {
            get;
            set;
        }

        [Browsable(false)]
        public object SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject == value)
                    return;
                _selectedObject = value;
                var hiddenPropertiesProperty = _selectedObject.GetType().GetProperty("HiddenProperties", typeof(string));
                if (hiddenPropertiesProperty != null)
                {
                    var hiddenProperties = (string)hiddenPropertiesProperty.GetValue(value) ?? string.Empty;
                    HiddenProperties = hiddenProperties;
                }

                CreateEditor();
            }
        }

        [DefaultValue(true),
        Category("Behavior")]
        public bool ShowCategoryLabels
        {
            get { return _showCategoryLabels; }
            set { _showCategoryLabels = value; }
        }

        #endregion Properties

        #region Methods

        internal string DeTokenizePropertyMap(string url)
        {
            var result = url; //  Ensure a "copy" is made, and not a reference.

            //  Search through the SelectedObject looking for a property Name that has been defined as a replacement token.
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(_selectedObject))
            {
                var token = "{" + propertyDescriptor.Name + "}";

                if (!url.Contains(token))
                    continue;

                var typeConverter = propertyDescriptor.Converter;
                if (typeConverter != null && !typeConverter.CanConvertFrom(typeof(string)))
                {
                    throw new Exception("Sorry, the Value of Property Name for [" + propertyDescriptor.Name +
                                        "] cannot be used as a PropertyMap URL token");
                }

                var value = propertyDescriptor.GetValue(_selectedObject);

                if (value != null)
                    result = result.Replace(token, value.ToString());
            }

            return result;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Visible || (Site != null && Site.DesignMode))
            {
                //writer.Write(@"<table class=""PropertyGrid"">");
                //writer.Write(@"<tr>");
                //writer.Write(@"<td valign=""top"" width=""100%"">");
                RenderChildren(writer);
                //writer.Write(@"</td>");
                //writer.Write(@"</tr>");
                //writer.Write("</table>");
            }
        }

        private void CreateEditor()
        {
            if (_selectedObject == null)
                return;

            Controls.Clear();
            _properties.Clear();
            _proplist.Clear();

            _itemcounter = 0;
            _catcounter = 0;
            _subcounter = 0;

            var cats = new Hashtable();

            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(_selectedObject))
            {
                if (!pd.IsBrowsable)
                    continue;

                var cat = pd.Category??"Default";

                var mems = cats[cat] as Hashtable;

                if (mems == null)
                    cats[cat] = mems = new Hashtable();
                try
                {
                    var pgi = new PropertyGridItem(pd) { ControlId = ID + "_" + _itemcounter++ };

                    _properties[pgi.ControlId] = pgi;

                    var o = _selectedObject;
                    object subo = null;

                    try
                    {
                        subo = pd.GetValue(o);
                    }
                    catch
                    {
                    }

                    if (pd.Converter.GetPropertiesSupported())
                    {
                        foreach (PropertyDescriptor spd in pd.Converter.GetProperties(subo))
                        {
                            if (!spd.IsBrowsable)
                                continue;

                            PropertyGridItem pgsi = new PropertyGridSubItem(spd, pgi);

                            pgsi.ControlId = ID + "_" + _itemcounter++;
                            pgi.Subitems.Add(pgsi);

                            _properties[pgsi.ControlId] = pgsi;
                        }
                    }

                    var orderAttribute = pd.Attributes.OfType<PropertyOrderAttribute>().FirstOrDefault();
                    var order = orderAttribute != null ? orderAttribute.Order.ToString("D3") : "999";

                    mems.Add(order + pd.Name, pgi);
                }
                catch (Exception ex)
                {
                    Page.Response.Write(ex);
                }
            }

            _catlist.Clear();

            var catlist = new ArrayList(cats.Keys);

            catlist.Sort();

            HtmlContainerControl cc = new HtmlGenericControl("div");
            cc.ID = "cats";

            Controls.Add(cc);

            foreach (string category in catlist)
            {
                if (HiddenCategories != null)
                {
                    if (HiddenCategories.Contains(category))
                        continue;
                }

                var pgc = new PropertyGridCategory { CategoryName = category };

                _catlist.Add(pgc);
                cc.Controls.Add(pgc);

                var i = cats[category] as Hashtable;
                var il = new ArrayList(i.Keys);

                il.Sort();

                foreach (string pginame in il)
                {
                    var pgi = i[pginame] as PropertyGridItem;

                    _proplist.Add(pgi);
                    pgc.Controls.Add(pgi);

                    if (pgi.Subitems.Count > 0)
                    {
                        var si = new SubItems();
                        pgi.Controls.Add(si);

                        foreach (PropertyGridItem spgi in pgi.Subitems)
                        {
                            si.Controls.Add(spgi);
                            _proplist.Add(spgi);
                        }
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        internal class ObjectWebEditorDesigner : ControlDesigner
        {
            #region Methods

            public override string GetDesignTimeHtml()
            {
                var output = new StringWriter();
                try
                {
                    var w = new HtmlTextWriter(output);
                    w.Write("<table><tr><td>[" + ID + " design time HTML]</td></tr></table>");
                }
                catch (Exception exception)
                {
                    output.Write(exception.ToString());
                }

                return output.ToString();
            }

            #endregion Methods
        }

        private abstract class GridControl : Control
        {
            #region Fields

            private ObjectWebEditor _parentgrid;

            #endregion Fields

            #region Properties

            protected ObjectWebEditor ParentGrid
            {
                get
                {
                    if (_parentgrid == null)
                    {
                        var parent = Parent;

                        while (!(parent is ObjectWebEditor))
                            parent = parent.Parent;

                        _parentgrid = (ObjectWebEditor)parent;
                    }

                    return _parentgrid;
                }
            }

            #endregion Properties
        }

        private class PropertyGridCategory : GridControl
        {
            #region Fields

            private string _catagoryName;

            #endregion Fields

            #region Properties

            public string CategoryName
            {
                get { return _catagoryName; }
                set { _catagoryName = value; }
            }

            #endregion Properties

            #region Methods

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                ID = "cat" + ParentGrid._catcounter++;
            }

            protected override void Render(HtmlTextWriter writer)
            {
                //writer.Write(@"<table class=""PropertyGridCategory"" width=""100%"">");

                if (ParentGrid.ShowCategoryLabels)
                {
                    //writer.Write(@"<tr align=""left"">");
                    writer.Write(
                        //@"<td class=""CategoryName"" valign=""top"" width=""100%""><b>{0}</b></td>",
                        @"<span class=""CategoryName"">{0}</span><br />",
                        _catagoryName);
                    //writer.Write(@"</tr>");
                }

                //writer.Write(@"<tr align=""left"">");
                //writer.Write(@"<td valign=""top"" width=""100%"">");
                RenderChildren(writer);
                //writer.Write(@"</td>");
                //writer.Write(@"</tr>");
                //writer.Write("</table>");
            }

            #endregion Methods
        }

        private class PropertyGridItem : GridControl
        {
            #region Fields

            internal readonly ArrayList Subitems = new ArrayList();

            internal string ControlId;

            private readonly PropertyDescriptor _propertyDescriptor;

            #endregion Fields

            #region Constructors

            public PropertyGridItem(PropertyDescriptor propertyDescriptor)
            {
                _propertyDescriptor = propertyDescriptor;
            }

            #endregion Constructors

            #region Properties

            public PropertyDescriptor Descriptor
            {
                get { return _propertyDescriptor; }
            }

            public string Name
            {
                get { return _propertyDescriptor.Name; }
            }

            public string DisplayName
            {
                get
                {
                    var dn = _propertyDescriptor.DisplayName;
                    if (string.IsNullOrWhiteSpace(dn))
                        dn = _propertyDescriptor.Name.Spacify();
                    return dn;
                }
            }

            public string Definition
            {
                get
                {
                    var defn = _propertyDescriptor.Description;
                    if (string.IsNullOrWhiteSpace(defn))
                        defn = _propertyDescriptor.Name.Spacify();
                    return defn;
                }
            }

            public virtual object SelectedObject
            {
                get { return ParentGrid.SelectedObject; }
            }

            public object Value
            {
                get { return _propertyDescriptor.GetValue(SelectedObject); }
                set
                {
                    _propertyDescriptor.SetValue(SelectedObject, value);

                    if (IsSubItem)
                    {
                        var parent = ((PropertyGridSubItem)this).ParentItem;
                        parent.Descriptor.SetValue(parent.SelectedObject, SelectedObject);
                    }
                    else
                        ParentGrid.CreateEditor();
                }
            }

            public string ValueString
            {
                get
                {
                    var value = Value;
                    if (value == null)
                        return null;

                    return _propertyDescriptor.Converter.CanConvertTo(typeof(string))
                        ? _propertyDescriptor.Converter.ConvertToString(value)
                        : value.ToString();
                }
            }

            protected virtual bool IsSubItem
            {
                get { return false; }
            }

            private bool HasSubItems
            {
                get { return (Subitems.Count > 0); }
            }

            private bool IsParentItem
            {
                get { return !IsSubItem; }
            }

            #endregion Properties

            #region Methods

            public void SetPropertyValue(string value)
            {
                if (value == null)
                    return;

                _propertyDescriptor.SetValue(SelectedObject, value);
            }

            public void SetValue(string value)
            {
                var propertyValue = _propertyDescriptor.Converter.ConvertFromString(value);
                _propertyDescriptor.SetValue(SelectedObject, propertyValue);
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                if (IsParentItem)
                {
                    if (HasSubItems)
                        ID = "sub" + ParentGrid._subcounter++;
                }

                var formValue = Page.Request[ControlId];

                if (formValue == null)
                    return;

                if (Value == null)
                    SetValue(formValue);
                else
                {
                    if ((!String.IsNullOrEmpty(formValue)) && (formValue != ValueString))
                        SetValue(formValue);
                }
            }

            protected override void Render(HtmlTextWriter writer)
            {
                RenderEditor(writer);

                if (IsParentItem)
                    RenderChildren(writer);
            }

            private void RenderEditor(HtmlTextWriter writer)
            {
                if (_propertyDescriptor.IsReadOnly && ParentGrid.HideReadOnly)
                    return;

                if (ParentGrid.HiddenProperties.Contains(Name))
                    return;

                //writer.Write(@"<table width=""100%"">");
                //writer.Write(@"<tr class=""PropertyGridPropertyRow"" align=""left"">");

                var access = string.Empty;
                var displayedName = DisplayName;
                var propertyUrl = string.Empty;

                if (_propertyDescriptor.IsReadOnly || ParentGrid.ReadOnly)
                    //access = "readonly";
                    access = "disabled";

                //// Try to get a DisplayName for this property
                //var displayNameAttribute =
                //    _propertyDescriptor.Attributes.OfType<DisplayNameAttribute>().Select(att=>att.DisplayName).FirstOrDefault();
                //if (!string.IsNullOrWhiteSpace(displayNameAttribute))
                //    displayedName = displayNameAttribute;

                //  Detect any supplied mappings for this Property Name and honor its settings.
                var propertyMapped = false;
                foreach (PropertyMap propertyMap in ParentGrid.PropertyMaps)
                {
                    if ((Name != propertyMap.PropertyName) || (!propertyMap.Enabled))
                        continue;

                    //  If a PropertyMap exists and there is no URL provided, however a DisplayedName
                    //  is provided...the intent is to simply rename the actual PropertyName to
                    //  the optionally provided DisplayName.  This provides developers a mechanism for
                    //  replacing machine readable PropertyNames with human suitable DisplayNames.
                    displayedName = (String.IsNullOrEmpty(propertyMap.DisplayedName))
                        ? Name
                        : propertyMap.DisplayedName;

                    if (String.IsNullOrEmpty(propertyMap.Url))
                        break;

                    propertyMapped = true;

                    var url = ParentGrid.DeTokenizePropertyMap(propertyMap.Url);
                    propertyUrl =
                        String.Format(@"<a class=""PropertyGridPropertyLink"" href=""{0}"" title=""{1}"">{2}</a>",
                            url, propertyMap.ToolTip, propertyMap.LinkDescription);

                    break;
                }

                writer.WriteLine(
                    //@"<td class=""PropertyName"" valign=""top"" align=""left"" width=""100%"">{0}</td>",
                    @"<span class=""PropertyName"">{0}",
                    displayedName);
                writer.WriteLine(@"<span class=""PropertyDefinition""><br/>{0}</span>", Definition);
                writer.WriteLine(@"</span>");
                //writer.Write(@"<td class=""PropertyValue"" valign=""top"" align=""left"" width=""100%"">");
                writer.WriteLine(@"<span class=""PropertyValue"">");

                if (!String.IsNullOrEmpty(propertyUrl))
                    writer.Write(propertyUrl);

                if (!propertyMapped)
                {
                    var typeConverter = _propertyDescriptor.Converter;

                    if (typeConverter.GetStandardValuesSupported())
                    {
                        writer.Write(@"<select id=""{0}"" name=""{0}"" {1} >", ControlId, access);

                        foreach (var standardValue in typeConverter.GetStandardValues())
                        {
                            var val = typeConverter.ConvertToString(standardValue);

                            writer.Write(
                                val == ValueString
                                    ? @"<option selected=""selected"">{0}</option>"
                                    : @"<option>{0}</option>", val);
                        }

                        writer.Write("</select>");
                    }
                    else
                    {
                        if(typeConverter is StringArrayConverter){
                            writer.Write(@"<textarea id=""{0}"" name=""{0}"" rows=""5"" value=""{1}"" class=""PropertyValue"" {2}></textarea>", ControlId, ValueString, access);
                        }
                        else if (typeConverter.CanConvertFrom(typeof(string)))
                        {
                            writer.Write(@"<input id=""{0}"" name=""{0}"" type=""Text"" value=""{1}"" class=""PropertyValue"" {2} />", ControlId, ValueString, access);
                        }
                        else
                        {
                            writer.Write(@"<div class=""PropertyNestedGrid"">");

                            //  ------------
                            //  WIP:    This needs some work.  Does not handle collections...yet.
                            //          Also, it would be nice to put this in a collapsed div with
                            //          a plus/minus toggler.
                            var nestedEditor = new ObjectWebEditor
                            {
                                ID = ID + "X",
                                HiddenCategories = ParentGrid.HiddenCategories,
                                HiddenProperties = ParentGrid.HiddenProperties,
                                ShowCategoryLabels = ParentGrid.ShowCategoryLabels,
                                HideReadOnly = ParentGrid.HideReadOnly,
                                SelectedObject = Value,
                                Page = Page
                            };

                            nestedEditor.Render(writer);
                            writer.Write(@"</div>");
                            //  ------------
                        }
                    }
                }

                //writer.Write(@"</td>");
                //writer.Write(@"</tr>");
                //writer.Write(@"</table>");
                writer.Write(@"</span><br />");
            }

            #endregion Methods
        }

        private class PropertyGridSubItem : PropertyGridItem
        {
            #region Fields

            private readonly PropertyGridItem _parentitem;

            #endregion Fields

            #region Constructors

            public PropertyGridSubItem(PropertyDescriptor propertyDescriptor, PropertyGridItem parentitem)
                : base(propertyDescriptor)
            {
                _parentitem = parentitem;
            }

            #endregion Constructors

            #region Properties

            public PropertyDescriptor ParentDescriptor
            {
                get { return _parentitem.Descriptor; }
            }

            public PropertyGridItem ParentItem
            {
                get { return _parentitem; }
            }

            public override object SelectedObject
            {
                get { return _parentitem.Descriptor.GetValue(base.SelectedObject); }
            }

            protected override bool IsSubItem
            {
                get { return true; }
            }

            #endregion Properties
        }

        private class SubItems : Control
        {
            #region Methods

            protected override void Render(HtmlTextWriter writer)
            {
                writer.Write(@"<table width=""100%"">");
                writer.Write(@"<tr align=""left"">");
                writer.Write(@"<td valign=""top"" width=""100%"">");
                RenderChildren(writer);
                writer.Write(@"</td>");
                writer.Write(@"</tr>");
                writer.Write("</table>");
            }

            #endregion Methods
        }

        #endregion Nested Types
    }

    public class PropertyCollectionEditor : CollectionEditor
    {
        #region Constructors

        public PropertyCollectionEditor(Type type)
            : base(type)
        {
        }

        #endregion Constructors

        #region Methods

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(PropertyMap);
        }

        #endregion Methods
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertyMap
    {
        #region Constructors

        public PropertyMap()
            : this(null, null, null, true)
        {
        }

        public PropertyMap(string propertyName, string displayedName, string url, bool enabled)
        {
            PropertyName = propertyName;
            DisplayedName = displayedName;
            Url = url;
            Enabled = enabled;
        }

        #endregion Constructors

        #region Properties

        [Category("Behavior"),
        DefaultValue(""),
        Description("Optional css defintion to be applied."),
        NotifyParentProperty(true)]
        public string CssClass
        {
            get;
            set;
        }

        [Category("Behavior"),
        DefaultValue(""),
        Description("(optional) Replaces the Property Name that would normally be displayed"),
        NotifyParentProperty(true)]
        public string DisplayedName
        {
            get;
            set;
        }

        [Category("Behavior"),
        DefaultValue(true),
        Description("When Enabled, replaces Property Value with supplied URL."),
        NotifyParentProperty(true)]
        public bool Enabled
        {
            get;
            set;
        }

        [Category("Behavior"),
        DefaultValue("Details"),
        Description("The text that will be used for the URL link."),
        NotifyParentProperty(true)]
        public string LinkDescription
        {
            get;
            set;
        }

        [Category("Behavior"),
        DefaultValue(""),
        Description("Indicates the Property Name whose Value will be replaced with the supplied URL."),
        NotifyParentProperty(true)]
        public string PropertyName
        {
            get;
            set;
        }

        [Category("Behavior"),
        DefaultValue(""),
        Description("Optional hover over Tool Tip."),
        NotifyParentProperty(true)]
        public string ToolTip
        {
            get;
            set;
        }

        [Category("Behavior"),
        DefaultValue(""),
        Description(
             "The URL used to replace the Property Value.  URL can be tokenized with Property Values like {ID}, {Name}, etc.."
             ),
        NotifyParentProperty(true)]
        public string Url
        {
            get;
            set;
        }

        #endregion Properties
    }
}