using System;
using System.Linq;

namespace Natalie.Framework.Data.Step
{
    public partial class STEPProductInformation
    {
		#region Fields (2) 

        public const string DefaultAttributeGroup = "Product Data";
        public const string DefaultAttributeValidity = "SKU";

		#endregion Fields 

		#region Methods (4) 

		// Public Methods (4) 

        public AttributeList GetAttributeList()
        {
            if (AttributeList == null)
            {
                AttributeList = new AttributeList();
                AttributeGroupList = new AttributeGroupList
                                         {
                                             AttributeGroup =
                                                 new[]
                                                     {
                                                         new AttributeGroup
                                                             {
                                                                 ID = DefaultAttributeGroup,
                                                                 Name = Name.NewName(DefaultAttributeGroup)
                                                             }
                                                     }
                                         };
            }
            return AttributeList;
        }

        public Classifications GetClassifications()
        {
            return Classifications ?? (Classifications = new Classifications());
        }

        public Products GetProducts()
        {
            return Products ?? (Products = new Products());
        }

        public void InitAttributeGroups(AttributeGroup[] attributeGroups)
        {
            AttributeList = new AttributeList();
            AttributeGroupList = new AttributeGroupList {AttributeGroup = attributeGroups};
        }

		#endregion Methods 
    }

    public partial class Attribute
    {
		#region Constructors (1) 

        public Attribute(
            string id, string name, bool isMultiValued, string attributeGroupId, string userTypeId = null)
            : this()
        {
            ID = id;
            Name = Step.Name.NewName(name);
            if (isMultiValued)
                MultiValued = AttributeMultiValued.@true;
            AttributeGroupLink = new[] { new AttributeGroupLink { AttributeGroupID = attributeGroupId } };
            UserTypeLink = new[]
                               {
                                   new UserTypeLink
                                       {UserTypeID = userTypeId ?? STEPProductInformation.DefaultAttributeValidity}
                               };
        }

		#endregion Constructors 
    }

    public partial class Name
    {
		#region Methods (1) 

		// Public Methods (1) 

        public static Name[] NewName(string text)
        {
            return new[] { new Name { Text = new[] { text } } };
        }

		#endregion Methods 
    }

    public partial class Values
    {
		#region Methods (3) 

		// Public Methods (2) 

        public void Add(Value value)
        {
            InitAddObject();
            Item[Item.Length - 1] = value;
        }

        public void Add(MultiValue value)
        {
            InitAddObject();
            Item[Item.Length - 1] = value;
        }
		// Private Methods (1) 

        private void InitAddObject()
        {
            object[] items;
            if (Item == null)
                items = new object[1];
            else
            {
                items = Item;
                Array.Resize(ref items, items.Length + 1);
            }
            Item = items;
        }

		#endregion Methods 
    }

    public partial class Products
    {
		#region Methods (1) 

		// Public Methods (1) 

        public Product AddChildProduct(Product child)
        {
            Product[] products;
            if (Product == null)
                products = new Product[1];
            else
            {
                products = Product;
                Product existingProduct = products.Where(obj => obj.ID.Equals(child.ID)).FirstOrDefault();
                if (existingProduct != null)
                    return existingProduct;

                Array.Resize(ref products, products.Length + 1);
            }

            child.ParentID = "Product hierarchy root";
            products[products.Length - 1] = child;
            Product = products;

            return child;
        }

		#endregion Methods 
    }

    public partial class Product
    {
		#region Fields (1) 

        private Product _parentProduct;

		#endregion Fields 

		#region Constructors (2) 

        public Product(string id, string userTypeId, string name, string overridesProductId = null)
            : this()
        {
            ID = id;
            UserTypeID = userTypeId;
            Name = Step.Name.NewName(name);
            if (!string.IsNullOrEmpty(overridesProductId))
                OverridesProductID = overridesProductId;
        }

        private Product()
        {
        }

		#endregion Constructors 

		#region Properties (1) 

        public bool HasChildProduct
        {
            get
            {

                if (Item == null)
                    return false;

                object[] items = Item;
                return items.Any(obj => obj is Product);
            }
        }

		#endregion Properties 

		#region Methods (4) 

		// Public Methods (4) 

        public Product AddChildProduct(Product child)
        {
            object[] items;
            if (Item == null)
                items = new object[1];
            else
            {
                items = Item;
                object existingProduct =
                    items.Where(obj => obj is Product && ((Product)obj).ID.Equals(child.ID)).FirstOrDefault();
                if (existingProduct != null)
                    return (Product)existingProduct;
                Array.Resize(ref items, items.Length + 1);
            }

            //child.ParentID = ID;
            items[items.Length - 1] = child;
            Item = items;

            child._parentProduct = this;

            return child;
        }

        public void AddOverrideSubProduct(string productId)
        {
            var osp = new OverrideSubProduct { ProductID = productId };

            if (Item == null)
                Item = new object[] { osp };
            else
            {
                object[] items = Item;
                Array.Resize(ref items, items.Length + 1);
                items[items.Length - 1] = osp;
                Item = items;
            }
        }

        public Values GetValues()
        {
            Values values;
            if (Item == null)
                Item = new object[] { values = new Values() };
            else
            {
                values = (Values)Item.Where(obj => obj is Values).FirstOrDefault();
                if (values == null)
                {
                    object[] items = Item;
                    Array.Resize(ref items, items.Length + 1);
                    items[items.Length - 1] = values = new Values();
                }
            }
            return values;
        }

        public void Remove()
        {
            object[] items = _parentProduct.Item;
            _parentProduct.Item = items.Where(obj => !obj.Equals(this)).ToArray();
        }

		#endregion Methods 
    }

    public partial class Classifications
    {
		#region Methods (1) 

		// Public Methods (1) 

        public Classification AddChildClassification(Classification child, string parentId)
        {
            Classification[] items;
            if (Classification == null)
                items = new Classification[1];
            else
            {
                items = Classification;
                Classification existingProduct = items.Where(obj => obj.ID.Equals(child.ID)).FirstOrDefault();
                if (existingProduct != null)
                    return existingProduct;
                Array.Resize(ref items, items.Length + 1);
            }

            child.ParentID = parentId;
            items[items.Length - 1] = child;
            Classification = items;

            return child;
        }

		#endregion Methods 
    }

    public partial class Classification
    {
		#region Fields (1) 

        private Classification _parentClassification;

		#endregion Fields 

		#region Constructors (1) 

        public Classification(string id, string userTypeId, string name)
            : this()
        {
            ID = id;
            UserTypeID = userTypeId;
            Name = Step.Name.NewName(name);
        }

		#endregion Constructors 

		#region Methods (2) 

		// Public Methods (2) 

        public Classification AddChildClassification(Classification child)
        {
            Classification[] items;
            if (Item == null)
                items = new Classification[1];
            else
            {
                items = Item;
                Classification existingProduct = items.Where(obj => obj.ID.Equals(child.ID)).FirstOrDefault();
                if (existingProduct != null)
                    return existingProduct;
                Array.Resize(ref items, items.Length + 1);
            }

            //child.ParentID = ID;
            items[items.Length - 1] = child;
            Item = items;

            child._parentClassification = this;
            return child;
        }

        public void Remove()
        {
            Classification[] items = _parentClassification.Item;
            _parentClassification.Item = items.Where(obj => !obj.Equals(this)).ToArray();
        }

		#endregion Methods 
    }
}