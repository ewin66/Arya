using System;
using System.Globalization;
using Arya.HelperClasses;

namespace Arya.Data
{
    partial class SchemaData
    {
		#region Properties (1) 

        public User DeletedByUser
        {
            get { return User1;  }
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
            DeletedRemark = AryaTools.Instance.RemarkID;
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);

            if (string.IsNullOrEmpty(DataType))
                DataType = "Text";
        }
        public override string ToString()
        {
            return "SchemaDataId  " + ID + '\t' + "Createdby " + CreatedBy + '\t' + "CreatedOn " + CreatedOn.ToString(CultureInfo.InvariantCulture) + '\t' + "DeletedBy " + DeletedBy;
        }
		#endregion Methods 
    }
}