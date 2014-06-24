namespace Arya.Framework4.Collections
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using ExtendedTaxonomyInfo = ComponentModel.ExtendedTaxonomyInfo;
    using TaxonomyCollectionPropertyDescriptor = ComponentModel.TaxonomyCollectionPropertyDescriptor;

    public class TaxonomyCollection : CollectionBase, ICustomTypeDescriptor
    {
        public event EventHandler ItemChanged;

        public bool MultipleNodes;

        public void Add(ExtendedTaxonomyInfo taxonomyInfo)
        {
            if(!MultipleNodes)
            {
                List.Clear();
            }
            List.Add(taxonomyInfo);
            if (ItemChanged != null)
            {
                ItemChanged(this, new EventArgs());
            }
        }

        public void Remove(ExtendedTaxonomyInfo taxonomyInfo)
        {
            List.Remove(taxonomyInfo);

            if (ItemChanged != null)
            {
                ItemChanged(this, new EventArgs());
            }
        }

        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            if (ItemChanged != null)
            {
                ItemChanged(this, new EventArgs());
            }
        }

        public ExtendedTaxonomyInfo this[int index]
        {
            get
            {
                if(List.Count - 1 >= index)
                    return (ExtendedTaxonomyInfo)List[index];
                //dummy holder to remove the item
                return new ExtendedTaxonomyInfo(null);
            }
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a collection object to hold property descriptors
            var pds = new PropertyDescriptorCollection(null);

            // Iterate the list of Taxomonies
            for (int i = 0; i < List.Count; i++)
            {
                // Create a property descriptor for the Taxonomy item and add to the property descriptor collection
                var pd = new TaxonomyCollectionPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
}
