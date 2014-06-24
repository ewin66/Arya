using System;
using Arya.HelperClasses;
using System.Collections.Generic;

namespace Arya.Data
{
    public partial class ListOfValue
    {
        
		#region Properties (1) 

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

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
    
        //string ISpell.GetType()
        //{
        //    return this.GetType().ToString();
        //}

        //System.Collections.Generic.Dictionary<string, string> ISpell.GetSpellValue()
        //{
        //    if (_propertyNameValue == null)
        //    {
        //        _propertyNameValue = new Dictionary<string, string>();
        //        if (!String.IsNullOrEmpty(this.Value))
        //        {
        //            _propertyNameValue.Add("Value", this.Value);
        //        }
        //        if (!String.IsNullOrEmpty(this.EnrichmentCopy))
        //        {
        //            _propertyNameValue.Add("EnrichmentCopy", this.EnrichmentCopy);
        //        }

        //    }

        //    return _propertyNameValue;
        //}

        //Guid ISpell.GetId()
        //{
        //    return this.SchemaID;
        //}

        //ISpell ISpell.SetValue(string propertyName, string value)
        //{
        //    ListOfValue lov = null;
        //    if (propertyName.ToLower() == "value")
        //    {
        //        lov = new ListOfValue()
        //        {
        //            NodeName = value,
        //            NodeDescription = this.NodeDescription,
        //            ParentTaxonomyInfo = this.ParentTaxonomyInfo,
        //            TaxonomyInfo = this.TaxonomyInfo
        //        };
        //    }
        //    else if (propertyName.ToLower() == "enrichmentcopy")
        //    {
        //        lov = new ListOfValue()
        //        {
        //            NodeName = this.NodeName,
        //            NodeDescription = value,
        //            ParentTaxonomyInfo = this.ParentTaxonomyInfo,
        //            TaxonomyInfo = this.TaxonomyInfo
        //        };
        //    }
        //    if (taxData != null)
        //    {
        //        this.Active = false;
        //        AryaTools.Instance.InstanceData.Dc.TaxonomyDatas.InsertOnSubmit(taxData);
        //    }            
        //}

        //string ISpell.GetLocation()
        //{
        //    throw new NotImplementedException();
        //}
    }
}