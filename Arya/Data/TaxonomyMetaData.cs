using System;
using Arya.HelperClasses;
using System.Collections.Generic;
using Arya.SpellCheck;

namespace Arya.Data
{
    partial class TaxonomyMetaData:ISpell
    {
        private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                return (KeyValuePair<bool, string>)_updatable;
            }
        }
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
        

        partial void OnActiveChanged()
        {
            if (Active)
                return;

            //DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
            DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser; //fixes Object not found error
            DeletedOn = DateTime.Now;
            DeletedRemark = AryaTools.Instance.RemarkID;
        }

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

        string ISpell.GetType()
        {
            return this.GetType().ToString();
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
             var   _propertyNameValue = new Dictionary<string, string>();
                _propertyNameValue = new Dictionary<string, string>();                
                _propertyNameValue.Add(this.TaxonomyMetaInfo.Attribute.AttributeName, Value);

           
            return _propertyNameValue;
        }

        Guid ISpell.GetId()
        {
            return TaxonomyMetaInfo.ID;
        }

        ISpell ISpell.SetValue(string propertyName, string value)
        {
            TaxonomyMetaData taxMetaData = null;
            if (propertyName.ToLower() == this.TaxonomyMetaInfo.Attribute.AttributeName.ToLower())
            {
                this.Active = false;
                taxMetaData = new TaxonomyMetaData()
                {
                    TaxonomyMetaInfo = this.TaxonomyMetaInfo,
                    Value = value
                };
                AryaTools.Instance.InstanceData.Dc.TaxonomyMetaDatas.InsertOnSubmit(taxMetaData);
            }            
            return taxMetaData;
        }

        string ISpell.GetLocation()
        {
            return "";// TaxonomyMetaInfo.Attribute.AttributeName;
        }
    }
}