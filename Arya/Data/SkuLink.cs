using System;
using Arya.HelperClasses;

namespace Arya.Data
{
    partial class SkuLink
    {
		#region Properties (3) 

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

        public Sku LinkedFrom { get { return Sku; } set { Sku = value; } }

        public Sku LinkedTo { get { return Sku1; } set { Sku1 = value; } }

		#endregion Properties 

		#region Methods (2) 

		// Private Methods (2) 

        partial void OnActiveChanged()
        {
            if (Active)
                return;

            //DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
            DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser; //fixes Object not found error
            DeletedOn = DateTime.Now;
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

		#endregion Methods 
    }
}