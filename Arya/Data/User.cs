using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;

namespace Arya.Data
{
    public partial class User
    {
        #region Fields (6) 

        private HashSet<Guid> attributeExclusions;
        private HashSet<Guid> skuExclusions;
        private HashSet<Guid> taxonomyExclusions;
        private HashSet<Guid> uiExclusions;
        private List<Guid> userGroups;

        #endregion Fields 

        #region Properties (7) 

        public HashSet<Guid> AttributeExclusions
        {
            get
            {
                if (AryaTools.Instance.InstanceData.CurrentProject == null || userGroups == null)
                    return new HashSet<Guid>();

                return attributeExclusions ??
                       (attributeExclusions =
                        AryaTools.Instance.InstanceData.Dc.Groups.Where(p => userGroups.Contains(p.ID)).SelectMany(p => p.Roles).
                            Where(r => r.Permission == false && r.ObjectType == Role.TaskObjectType.Attribute.ToString())
                            .Select(r => r.ObjectID).ToList().ToHashSet());
            }
        }

        public HashSet<Guid> SkuExclusions
        {
            get
            {
                if (AryaTools.Instance.InstanceData.CurrentProject == null || userGroups == null)
                    return new HashSet<Guid>();

                return skuExclusions ??
                       (skuExclusions =
                        AryaTools.Instance.InstanceData.Dc.Groups.Where(p => userGroups.Contains(p.ID)).SelectMany(p => p.Roles).
                            Where(r => r.Permission == false && r.ObjectType == Role.TaskObjectType.Sku.ToString()).
                            Select(r => r.ObjectID).ToList().ToHashSet());
            }
        }

        public HashSet<Guid> TaxonomyExclusions
        {
            get
            {
                if (AryaTools.Instance.InstanceData.CurrentProject == null || userGroups == null)
                    return new HashSet<Guid>();

                if (taxonomyExclusions != null)
                    return taxonomyExclusions;


                return taxonomyExclusions ??
                       (taxonomyExclusions =
                        AryaTools.Instance.InstanceData.Dc.Groups.Where(p => userGroups.Contains(p.ID)).SelectMany(p => p.Roles).
                            Where(
                                r => r.Permission == false && r.ObjectType == Role.TaskObjectType.TaxonomyInfo.ToString())
                            .Select(r => r.ObjectID).ToList().ToHashSet());
            }
        }

        public HashSet<Guid> UIExclusions
        {
            get
            {
                if (AryaTools.Instance.InstanceData.CurrentProject == null || userGroups == null)
                    return new HashSet<Guid>();

                if (uiExclusions != null) return uiExclusions;

                var db = new SkuDataDbDataContext();

                uiExclusions = db.Groups.Where(p => userGroups.Contains(p.ID)).SelectMany(
                    p => p.Roles).
                    Where(
                        r =>
                        r.Permission == false && r.ObjectType == Role.TaskObjectType.UIObject.ToString()).
                    Select(r => r.ObjectID).ToList().Union(
                        AryaTools.Instance.InstanceData.Dc.Groups.Where(p => userGroups.Contains(p.ID)).SelectMany(
                            p => p.Roles).
                            Where(
                                r =>
                                r.Permission == false && r.ObjectType == Role.TaskObjectType.UIObject.ToString()).
                            Select(r => r.ObjectID).ToList()).ToHashSet();

                db.Dispose();

                return uiExclusions;
            }
        }

        public bool IsAryaReadOnly
        {
            get { return UIExclusions.Contains(UIObject.AryaReadOnly); }
        }
        public bool IsProjectAdmin
        {
            get { return AryaTools.Instance.InstanceData.Dc.UserProjects.Any(up=>up.UserID == AryaTools.Instance.InstanceData.CurrentUser.ID && up.GroupID == UIObject.ProjectAdmin); }
        }
        public List<Guid> UserGroups
        {
            get { return userGroups; }
            set { userGroups = value; }
        }

        #endregion Properties 

        #region Methods (2) 

        // Public Methods (1) 

        public override string ToString()
        {
            return FullName + " (" + ID + ")";
        }
        
        #endregion Methods 
    }
}