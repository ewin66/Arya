using System;
using Arya.HelperClasses;
using System.Collections.Generic;
using Arya.SpellCheck;

namespace Arya.Data
{
    partial class SchemaMetaData:ISpell
    {
        private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
       
		#region Properties (1) 

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

		#endregion Properties 

		#region Methods (3) 

		// Public Methods (1) 

        public override string ToString()
        {
            return Value;
        }
		// Private Methods (2) 

        partial void OnActiveChanged()
        {
            if (Active)
                return;

            //DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
            DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser; //fixes Object not found error
            DeletedOn = DateTime.Now;
            DeletedRemark = AryaTools.Instance.RemarkID;
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

		#endregion Methods 
    
        string ISpell.GetType()
        {
            return this.GetType().ToString();
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
             var   _propertyNameValue = new Dictionary<string, string>();
                _propertyNameValue = new Dictionary<string, string>();
                _propertyNameValue.Add(this.SchemaMetaInfo.Attribute.AttributeName, Value);
            return _propertyNameValue;          
        }

        Guid ISpell.GetId()
        {
            return this.SchemaMetaInfo.ID;
        }

        ISpell ISpell.SetValue(string propertyName, string value)
        {
            SchemaMetaData schemaMetaData = null;
            if (propertyName.ToLower() == this.SchemaMetaInfo.Attribute.AttributeName.ToLower())
            {
                this.Active = false;
                schemaMetaData = new SchemaMetaData()
                {
                    SchemaMetaInfo = this.SchemaMetaInfo,
                    Value = value
                };
                AryaTools.Instance.InstanceData.Dc.SchemaMetaDatas.InsertOnSubmit(schemaMetaData);
            }
            
            return schemaMetaData;
        }

        string ISpell.GetLocation()
        {
            return "Attribute Name: "+this.SchemaMetaInfo.SchemaInfo.Attribute.AttributeName + " Meta Attribute Name: "+ this.SchemaMetaInfo.Attribute.AttributeName;
        }


        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                return (KeyValuePair<bool, string>)_updatable;        
            }
        }
    }
}