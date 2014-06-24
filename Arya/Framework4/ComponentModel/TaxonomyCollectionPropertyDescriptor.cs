namespace Arya.Framework4.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using TaxonomyCollection = Collections.TaxonomyCollection;

    public class TaxonomyCollectionPropertyDescriptor : PropertyDescriptor
    {
        private readonly TaxonomyCollection collection;
        private readonly int index = -1;

        public TaxonomyCollectionPropertyDescriptor(TaxonomyCollection taxonomyCollection, int index)
            : base("#" + index, null)
        {
            this.index = index;
            collection = taxonomyCollection;
        }

        public override string Name
        {
            get { return "#" + index; }
        }

        public override string DisplayName
        {
            get
            {
                var exportTaxonomyInfo = collection[index];
                return exportTaxonomyInfo.ToString();
            }
        }

        public override string Description
        {
            get
            {
                Framework4.ComponentModel.ExtendedTaxonomyInfo exportTaxonomyInfo = collection[index];
                var retValue = exportTaxonomyInfo.SkuCount + " Sku(s)";

                if (!exportTaxonomyInfo.IsLeafNode)
                {
                    retValue += Environment.NewLine + exportTaxonomyInfo.Taxonomy.AllLeafChildren.Count() + " Children";
                }


                return retValue;
            }
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return collection[index];
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {

        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return collection.GetType(); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return collection[index].GetType(); }
        }
    }
}
