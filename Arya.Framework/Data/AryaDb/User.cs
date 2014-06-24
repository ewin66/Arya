using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;

namespace Arya.Framework.Data.AryaDb
{
    public partial class User : BaseEntity
    {
        public User(AryaDbDataContext parentContext, bool initialize = true)
            : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        #region�Properties�(7)

        private HashSet<Guid> _attributeExclusions;
        private Guid _prevFetchedProjectID = default(Guid);
        private HashSet<Guid> _taxonomyExclusions;
        private HashSet<Guid> _uiExclusions;

        public HashSet<Guid> GetAttributeExclusions(Guid projectID)
        {
//            if (_prevFetchedProjectID == projectID)
//                return _attributeExclusions;

            var attributeExclusions =
                UserProjects.Where(
                    p =>
                        p.ProjectID == projectID
                        && p.Group.GroupType.StartsWith("USER_", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(p => p.Group.Roles)
                    .Where(r => r.Permission == false && r.ObjectType == Role.TaskObjectType.Attribute.ToString())
                    .Select(r => r.ObjectID)
                    .ToList()
                    .ToHashSet();

            _prevFetchedProjectID = projectID;
            _attributeExclusions = attributeExclusions;

            return attributeExclusions;
        }

        public HashSet<Guid> GetTaxonomyExclusions(Guid projectID)
        {
            if (_prevFetchedProjectID == projectID)
                return _taxonomyExclusions;

            var taxonomyExclusions =
                UserProjects.Where(
                    p =>
                        p.ProjectID == projectID
                        && p.Group.GroupType.StartsWith("USER_", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(p => p.Group.Roles)
                    .Where(r => r.Permission == false && r.ObjectType == Role.TaskObjectType.Sku.ToString())
                    .Select(r => r.ObjectID)
                    .ToList()
                    .ToHashSet();

            _taxonomyExclusions = taxonomyExclusions;
            _prevFetchedProjectID = projectID;

            return taxonomyExclusions;
        }

        public HashSet<Guid> GetUIExclusions(Guid projectID)
        {
            if (_prevFetchedProjectID == projectID)
                return _uiExclusions;

            var db = new AryaDbDataContext();

            var userGroups =
                db.UserProjects.Where(p => p.UserID == ID && p.ProjectID == projectID)
                    .Select(p => p.GroupID)
                    .Distinct()
                    .ToList();

            var globalUIExclusions =
                db.Groups.Where(p => userGroups.Contains(p.ID))
                    .SelectMany(p => p.Roles)
                    .Where(r => r.Permission == false && r.ObjectType == Role.TaskObjectType.UIObject.ToString())
                    .Select(r => r.ObjectID)
                    .ToList();

            db.Dispose();

            db = new AryaDbDataContext(projectID, ID);

            var projectUIExclusions =
                db.Groups.Where(p => userGroups.Contains(p.ID))
                    .SelectMany(p => p.Roles)
                    .Where(r => r.Permission == false && r.ObjectType == Role.TaskObjectType.UIObject.ToString())
                    .Select(r => r.ObjectID)
                    .ToList();

            _uiExclusions = globalUIExclusions.Union(projectUIExclusions).ToHashSet();
            _prevFetchedProjectID = projectID;

            return _uiExclusions;
        }


        public HashSet<Guid> GetSkuExclusions(Guid projectID)
        {
            //TODO: Add code for skuExclusions
            return new HashSet<Guid>();
        }

        public bool IsAryaReadOnly(Guid projectID)
        {
            return GetUIExclusions(projectID).Contains(UIObject.AryaReadOnly);
        }

        #endregion�Properties

        #region�Methods�(2)

        //�Public�Methods�(1)�

        public override string ToString() { return FullName + " (" + ID + ")"; }

        #endregion�Methods
    }
}