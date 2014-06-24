using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;

namespace Arya.Data
{
    partial class Role
    {
        public enum PermissionType
        {
            [DisplayTextAndValue("Exclusion(s)",false)]
            Exclusions,
            [DisplayTextAndValue("Inclusion(s)",true)]
            Inclusions
        }

        public static PermissionType[] GetAllPermissionTypes()
        {
            return Enum.GetValues(typeof (PermissionType)).Cast<PermissionType>().ToArray();
        }

        public enum TaskObjectType
        {
            TaxonomyInfo,
            Sku,
            Attribute,
            UIObject
        }

        private object taskObject;

        public static string[] GetAllTaskObjectNames()
        {
            return Enum.GetNames(typeof(TaskObjectType));
        }

        public static TaskObjectType[] GetAllTaskObjects()
        {
            return Enum.GetValues(typeof(TaskObjectType)).Cast<TaskObjectType>().ToArray();
        }

        public TaskObjectType GetTaskObjectType()
        {
             TaskObjectType result;
            if (Enum.TryParse(ObjectType, true, out result))
                return result;

            throw new InvalidCastException("Enumeration out of range");
        }

        public object TaskObject
        {
            get
            {
                if (taskObject != null) return taskObject;

                TaskObjectType currentObjectType;

                if(!Enum.TryParse(ObjectType, true, out currentObjectType))
                {
                    throw new InvalidCastException("Enumeration out of range");
                }

                switch (currentObjectType)
                {
                    case TaskObjectType.Attribute:
                        taskObject =
                            AryaTools.Instance.InstanceData.Dc.Attributes.SingleOrDefault(p => p.ID.Equals(ObjectID));
                        break;
                    case TaskObjectType.Sku:
                        taskObject = AryaTools.Instance.InstanceData.Dc.Skus.SingleOrDefault(p => p.ID.Equals(ObjectID));
                        break;
                    case TaskObjectType.TaxonomyInfo:
                        taskObject =
                            AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.SingleOrDefault(p => p.ID.Equals(ObjectID));
                        break;
                    case TaskObjectType.UIObject:
                        taskObject =
                            AryaTools.Instance.InstanceData.Dc.UIObjects.SingleOrDefault(p => p.ID.Equals(ObjectID));
                        break;
                }

                return taskObject;
            }
        }
    }

}
