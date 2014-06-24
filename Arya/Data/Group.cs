using System;
using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;

namespace Arya.Data
{
    public partial class Group
    {

        private readonly bool initialize = true;

        public Group(bool initialize = true) : this()
        {
            this.initialize = initialize;
        }

        //default GroupID in Arya
        public static readonly Guid DefaultGroupID = new Guid("8A37744E-EFFA-4A1A-8EE1-E5BAF09925A4");

        public const string SKU_GROUP_UD = "Sku_UD";
        public const string USER_GROUP_UD = "User_UD";
        public const string USER_GROUP_PD = "User_PD";
        public const string USER_GROUP_WORKFLOW = "Workflow";

        //Predefined UserGroups
        public static readonly Guid RoleManagerGroup = new Guid("C70DCA7A-6077-47A6-85EB-8740B1C6D6AB");
        public static readonly Guid PermissionsManagerGroup = new Guid("D045B1E7-4E64-4340-91FD-BDE8C4706D3C");
        public static readonly Guid ReadOnlyGroup = new Guid("0C529A01-7E75-4F19-8816-278A2941E705");
        public static readonly Guid ImportAdminGroup = new Guid("DF71F6AC-14E2-42E5-8DA5-1906CCA7362F");

        public HashSet<Guid> TaxonomyExlusions
        {
            get
            {
                if(GroupType == SKU_GROUP_UD)
                {
                    throw new InvalidOperationException("Property not valid for Sku Groups");
                }

                return
                    Roles.Where(
                        r => r.Permission == false && r.ObjectType == Role.TaskObjectType.TaxonomyInfo.ToString()).
                        Select(p => p.ObjectID).ToList().ToHashSet();
            }
        }

        public HashSet<Guid> AttributeExlusions
        {
            get
            {
                if (GroupType == SKU_GROUP_UD)
                {
                    throw new InvalidOperationException("Property not valid for Sku Groups");
                }

                return
                    Roles.Where(
                        r => r.Permission == false && r.ObjectType == Role.TaskObjectType.Attribute.ToString()).
                        Select(p => p.ObjectID).ToList().ToHashSet();
            }
        }

        public HashSet<Guid> SkuExlusions
        {
            get
            {
                if (GroupType == SKU_GROUP_UD)
                {
                    throw new InvalidOperationException("Property not valid for Sku Groups");
                }

                return
                    Roles.Where(
                        r => r.Permission == false && r.ObjectType == Role.TaskObjectType.Sku.ToString()).
                        Select(p => p.ObjectID).ToList().ToHashSet();
            }
        }

        public HashSet<Guid> UIExclusions
        {
            get
            {
                if (GroupType == SKU_GROUP_UD)
                {
                    throw new InvalidOperationException("Property not valid for Sku Groups");
                }

                return
                    Roles.Where(
                        r => r.Permission == false && r.ObjectType == Role.TaskObjectType.UIObject.ToString()).
                        Select(p => p.ObjectID).ToList().ToHashSet();
            }
        }

        public List<Sku> this[TaxonomyInfo index]
        {
            get 
            
            {
                return this.SkuGroups.SelectMany(s => s.Sku.SkuInfos.Where(a => a.Active)).Where(t => t.TaxonomyInfo == index).Select(s => s.Sku).ToList();
                           
            }
           
        }

        public List<Sku> Skus 
        {
            get
            {
               return SkuGroups.Where(a=>a.Active).Select(s => s.Sku).ToList();
            }
        
        }

        public List<TaxonomyInfo> Nodes
        {
            get
            {

                return this.SkuGroups.SelectMany(s => s.Sku.SkuInfos.Where(a => a.Active)).Select(t => t.TaxonomyInfo).Distinct().ToList();
            }
        }
    

        public int SkuCount
        { 
            get
            {
                return this.SkuGroups.Count(a => a.Active);
            }
            
        }

        partial void OnCreated()
        {
            if(initialize)
                SkuDataDbDataContext.DefaultTableValues(this);
        }

       
    }
}
