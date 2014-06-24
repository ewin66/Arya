using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Arya.Framework.Common.ComponentModel;
using Arya.HelperClasses;
using Arya.Framework.Data;
using Arya.SpellCheck;
using Arya.Framework.Data.AryaDb;

namespace Arya.Data
{
	public partial class EntityData:ISpell
	{
        private KeyValuePair<bool, string>? _updatable;
       
	    private readonly bool initialize = true;
        public EntityData(bool initialize = true) : this()
        {
            this.initialize = initialize;
           // this._updatable = 
        }

       
		#region Properties (4) 

		public User DeletedByUser
		{
			get { return User1; }
			set { User1 = value; }
		}

		public string Field5OrStatus
		{
			get { return AryaTools.Instance.InstanceData.CurrentProject.EntityField5IsStatus ? Status : Field5; }
		}

		public Sku Sku
		{
			get
			{
				try
				{
					return this.EntityInfo.Sku;
				}
				catch
				{
					return null;
				}
			}
		}

        public static readonly Guid PtUser = new Guid("E50F6C3D-9D11-4376-9C17-D89542C65AD2");

		public string Status
		{
            get
            {
                if (EntityInfo == null)
                    return null;

                if (AryaTools.Instance.InstanceData.CurrentProject.ClientDescription.StartsWith("Allied"))
                {
                    var ptValue =
                        EntityInfo.EntityDatas.Where(ed => ed.CreatedBy == PtUser).OrderByDescending(ed => ed.CreatedOn)
                            .FirstOrDefault();

                    var beforeValue = EntityInfo.EntityDatas.FirstOrDefault(ed => ed.BeforeEntity);

                    string val;

                    if (beforeValue == null)
                        val = ptValue == null ? "BMI Created Data" : "PT Captured Data";
                    else
                        val = ptValue == null ? "Legacy Data" : "PT Overwritten Legacy Data";

                    if ((beforeValue != null && ID != beforeValue.ID)
                        || (ptValue != null && ID != ptValue.ID))
                        val = "Modified " + val;

                    return val;
                }
                else
                {
                    var ptValue =
                        EntityInfo.EntityDatas.Where(ed => ed.CreatedBy == PtUser).OrderByDescending(ed => ed.CreatedOn)
                            .FirstOrDefault();

                    if (ptValue == null)
                        return "Unverified";

                    //keep this in mind, skipping all the ptValues except for the latest one.
                    var beforePtValue =
                        EntityInfo.EntityDatas.Where(
                            ed => ed.CreatedOn <= ptValue.CreatedOn && ed.ID != ptValue.ID && ed.CreatedBy != PtUser).
                            OrderByDescending(ed => ed.CreatedOn).FirstOrDefault();

                    if (beforePtValue == null)
                        return "Populated";

                    return ptValue.Value.Equals(beforePtValue.Value)
                               ? (ptValue.Uom == beforePtValue.Uom ? "Verified" : "Change")
                               : "Change";
                }
            }
        }

		#endregion Properties 

		#region Methods (4) 

		// Public Methods (2) 

		public static List<EntityData> OrderSkuAttributeValues(IEnumerable<EntityData> eds, bool sortDesc=false)
		{
			return sortDesc
					   ? eds.OrderByDescending(ed => ed.Value, new CompareForAlphaNumericSort()).ToList()
					   : eds.OrderBy(ed => ed.Value, new CompareForAlphaNumericSort()).ToList();
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
            if(initialize)
                SkuDataDbDataContext.DefaultTableValues(this);
        }
        public override string ToString()
        {
            return Value.Length == 0 ? "<Empty Value>" : (string.IsNullOrWhiteSpace(Value) ? "<space(s)>" : Value);
        }
		#endregion Methods 
        #region ISpell Implemetation 
        string ISpell.GetType()
        {
            return this.GetType().ToString();
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
             var   _propertyNameValue = new Dictionary<string, string>();
            _propertyNameValue = new Dictionary<string, string>();
            _propertyNameValue.Add("Value", Value);
            return _propertyNameValue;
        }

        Guid ISpell.GetId()
        {
            if (EntityInfo == null)
                return this.ID;

            return EntityInfo.ID;
        }
        ISpell ISpell.SetValue(string propertyName, string value)
        {            
            EntityData newEntityData = null;
            if (propertyName.ToLower() == "value")
            {
                Active = false;
                newEntityData = new EntityData()
                {
                    EntityInfo = this.EntityInfo,
                    EntityDataNotes = this.EntityDataNotes,
                    Attribute = this.Attribute,
                    BaseValue = this.BaseValue,
                    BeforeEntity = this.BeforeEntity,
                    Value = value
                };
                AryaTools.Instance.InstanceData.Dc.EntityDatas.InsertOnSubmit(newEntityData);
            }
            
            return newEntityData;
        }

        string ISpell.GetLocation()
        {
            if (this.Sku != null && this.Attribute != null)
            {
                return "ItemId: " + this.Sku.ItemID + " " + "Attribute Name: " + this.Attribute.AttributeName;
            }
            return "";
            
        }
        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                if (_updatable == null)
                {
                    this._updatable = this.Attribute.Type == AttributeTypeEnum.Derived ? new KeyValuePair<bool, string>(false, "Derived attribute value can't be updated.") : new KeyValuePair<bool, string>(true, "");

                }
                return (KeyValuePair<bool, string>)_updatable;
            }
        }
        #endregion
        



        
    }
}