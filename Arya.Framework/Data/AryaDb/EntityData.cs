using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Math;

namespace Arya.Framework.Data.AryaDb
{
    public partial class EntityData : BaseEntity
    {
        public EntityData(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #region Properties (4)

        public static readonly Guid PTUser = new Guid("E50F6C3D-9D11-4376-9C17-D89542C65AD2");
        private bool? _entityField5IsStatus;

        public User DeletedByUser
        {
            get { return User1; }
            set { User1 = value; }
        }

        public Sku Sku
        {
            get
            {
                try
                {
                    return EntityInfo.Sku;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Field5OrStatus
        {
            get
            {
                return entityField5IsStatus ? Status : Field5;
            }
        }

        public bool entityField5IsStatus
        {
            get { return (bool) (_entityField5IsStatus ?? (_entityField5IsStatus=EntityInfo.Sku.Project.EntityField5IsStatus)); }
            set { _entityField5IsStatus = value; }
        }

        public string Status
        {
            get
            {
                if (EntityInfo == null)
                    return null;

                //var ptValue =
                //    EntityInfo.EntityDatas.Where(ed => ed.Field5 != null && ed.Field5.Equals("PT", StringComparison.OrdinalIgnoreCase)).OrderByDescending(
                //        ed => ed.CreatedOn).FirstOrDefault();

                //if (ptValue == null)
                //    return EntityInfo.EntityDatas.Any(ed => ed.Field5 != null && ed.Field5.Equals("DA", StringComparison.OrdinalIgnoreCase))
                //               ? "Verified"
                //               : "Unverified";

                //var beforePtValue =
                //    EntityInfo.EntityDatas.Where(ed => ed.CreatedOn <= ptValue.CreatedOn && ed.ID != ptValue.ID).
                //        OrderByDescending(ed => ed.CreatedOn).FirstOrDefault();

                //if (beforePtValue == null)
                //    return "Populated";

                //return ptValue.Value.Equals(beforePtValue.Value) ? "Verified" : "Change";

                var ptValue =
                    EntityInfo.EntityDatas.Where(ed => ed.CreatedBy == PTUser)
                        .OrderByDescending(ed => ed.CreatedOn)
                        .FirstOrDefault();

                if (ptValue == null)
                    return "Unverified";

                //keep this in mind, skipping all the ptValues except for the latest one.
                var beforePtValue =
                    EntityInfo.EntityDatas.Where(
                        ed => ed.CreatedOn <= ptValue.CreatedOn && ed.ID != ptValue.ID && ed.CreatedBy != PTUser)
                        .OrderByDescending(ed => ed.CreatedOn)
                        .FirstOrDefault();

                if (beforePtValue == null)
                    return "Populated";

                return ptValue.Value.Equals(beforePtValue.Value)
                    ? (ptValue.Uom == beforePtValue.Uom ? "Verified" : "Change")
                    : "Change";
            }
        }

        #endregion Properties

        #region Methods (4)

        // Public Methods (2) 

        public static List<EntityData> OrderSkuAttributeValues(IEnumerable<EntityData> eds, bool sortDesc = false)
        {
            return sortDesc
                ? eds.OrderByDescending(ed => ed.Value, new CompareForAlphaNumericSort()).ToList()
                : eds.OrderBy(ed => ed.Value, new CompareForAlphaNumericSort()).ToList();
        }

        public override string ToString()
        {
            return "EntityDataId  " + ID + '\t' + "Createdby " + CreatedBy + '\t' + "CreatedOn "
                   + CreatedOn.ToString(CultureInfo.InvariantCulture) + '\t' + "DeletedBy " + DeletedBy;
            // return Value.Length == 0 ? "<blank>" : (string.IsNullOrWhiteSpace(Value) ? "<space(s)>" : Value);
        }

        // Private Methods (2) 

        partial void OnActiveChanged()
        {
            if (Active)
                return;
            var parentContext = ParentContext;

            if (parentContext == null)
                return;

            AryaDbDataContext.DefaultDeletedTableValue(this, parentContext.CurrentUser.ID);
        }

        #endregion Methods
    }
}