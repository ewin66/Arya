using System;
using System.Linq;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;

namespace Arya.Framework.Data.AryaDb
{
    partial class Role : BaseEntity
    {
        public enum PermissionType
        {
            [DisplayTextAndValue("Exclusion(s)", false)] Exclusions,
            [DisplayTextAndValue("Inclusion(s)", true)] Inclusions
        }

        public enum TaskObjectType
        {
            TaxonomyInfo,
            Sku,
            Attribute,
            UIObject
        }

        public Role(AryaDbDataContext parentContext, bool initialize = true) : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        public static PermissionType[] GetAllPermissionTypes()
        {
            return Enum.GetValues(typeof (PermissionType)).Cast<PermissionType>().ToArray();
        }

        public static string[] GetAllTaskObjectNames() { return Enum.GetNames(typeof (TaskObjectType)); }

        public static TaskObjectType[] GetAllTaskObjects()
        {
            return Enum.GetValues(typeof (TaskObjectType)).Cast<TaskObjectType>().ToArray();
        }

        public TaskObjectType GetTaskObjectType()
        {
            TaskObjectType result;
            if (Enum.TryParse(ObjectType, true, out result))
                return result;

            throw new InvalidCastException("Enumeration out of range");
        }
    }
}