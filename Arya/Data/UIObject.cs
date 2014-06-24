using System;
using Arya.Framework.Extensions;

namespace Arya.Data
{
    partial class UIObject : IComparable
    {
        public enum UIAction
        {
            SkuView,
            AttributeView,
            QueryView,
            SchemaView,
            AryaReadOnly,
            PermissionsManager,
            RoleManager
        }

        //***Don't change these at any cost, as these are also stored in the database.
        public static readonly Guid AryaReadOnly = new Guid("5E63D79D-76D7-4D4F-A60A-A8D51371C6C3");
        public static readonly Guid SkuView = new Guid("46E1EFAF-EBE8-4435-A664-81F36A6016A2");
        public static readonly Guid AttributeView = new Guid("630AEFDF-6B51-47EE-88C0-BBA8E89B725C");
        public static readonly Guid QueryView = new Guid("D687EB71-8415-4E0E-A785-54FEA9B4F125");
        public static readonly Guid SchemaView = new Guid("98EE1AA2-33C6-463B-A20E-01B529EBC849");
        public static readonly Guid PermissionsManager = new Guid("B715FC0D-EE02-4886-8961-8796C2869DCC");
        public static readonly Guid RoleManager = new Guid("1C4E8B15-057A-4BA3-913E-E9335074968E");
        public static readonly Guid ProjectAdmin = new Guid("CC7724E9-6653-44D2-BCB2-7F03D4C234FE");

        /// <exception cref="InvalidCastException">Enumeration out of range</exception>
        public UIAction UIActionObject
        {
            get
            {
                UIAction result;
                if (Enum.TryParse(Name, true, out result))
                {
                    return result;
                }
                throw new InvalidCastException("Enumeration out of range");
            }
            set { Name = value.ToString(); }
        }

        public static Guid GetIdentifier(UIAction uiAction)
        {
            var retGuid = Guid.Empty;
            switch (uiAction)
            {
                case UIAction.AryaReadOnly:
                    retGuid = AryaReadOnly; break;
                case UIAction.AttributeView:
                    retGuid = AttributeView; break;
                case UIAction.QueryView:
                    retGuid = QueryView; break;
                case UIAction.SchemaView:
                    retGuid = SchemaView; break;
                case UIAction.SkuView:
                    retGuid = SkuView; break;
                case UIAction.PermissionsManager:
                    retGuid = PermissionsManager;
                    break;
                case UIAction.RoleManager:
                    retGuid = RoleManager;
                    break;
            }

            if (retGuid != Guid.Empty) return retGuid;
            throw new ArgumentOutOfRangeException("uiAction", uiAction, @"Invalid or crazy enum");
        }

        public static string[] GetUIObjectNames()
        {
            return Enum.GetNames(typeof(UIAction));
        }

        public static object GetDataSource()
        {
            var seed = new { UIObjectName = "AryaReadOnly", ID = AryaReadOnly };
            var dataSource = CollectionExtensions.MakeList(seed);
            //dataSource.Add(new { UIObjectName = "AryaReadOnly", ID = AryaReadOnly });
            dataSource.Add(new { UIObjectName = "SkuView", ID = SkuView });
            dataSource.Add(new { UIObjectName = "AttributeView", ID = AttributeView });
            dataSource.Add(new { UIObjectName = "QueryView", ID = QueryView });
            dataSource.Add(new { UIObjectName = "SchemaView", ID = SchemaView });
            //dataSource.Add(new { UIObjectName = "ShowPermissionsManager", ID = ShowPermissionsManager });
            //dataSource.Add(new { UIObjectName = "ShowRoleManager", ID = ShowRoleManager });
            return dataSource;
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            var thisString = ToString();
            var objString = obj.ToString();
            if (!thisString.Equals(objString))
                return thisString.CompareTo(objString);

            var other = (UIObject)obj;
            return ID.CompareTo(other.ID);
        }
    }
}
