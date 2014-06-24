using System;
using Arya.HelperClasses;
using System.Collections.Generic;
using Arya.SpellCheck;

namespace Arya.Data
{
    public partial class TaxonomyData
    {
      
        #region Properties (2)

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

        public TaxonomyInfo ParentTaxonomyInfo
        {
            get { return TaxonomyInfo1; }
            set { TaxonomyInfo1 = value; }
        }

        #endregion Properties

        #region Methods (2)
        
        // Private Methods (2) 

        partial void OnActiveChanged()
        {
            if (Active)
                return;
            DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser; //fixes Object not found error
            DeletedOn = DateTime.Now;
            DeletedRemark = AryaTools.Instance.RemarkID;
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

        #endregion Methods

       
    }
}