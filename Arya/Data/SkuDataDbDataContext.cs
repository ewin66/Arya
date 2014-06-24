using System.Collections.Concurrent;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using Arya.Framework.Collections.Generic;
using Arya.HelperClasses;
using Fasterflect;

namespace Arya.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Reflection;

    public partial class SkuDataDbDataContext
    {
        #region Fields (1)

        public readonly DoubleKeyDictionary<Attribute, Sku, List<EntityData>> SkuAttributeValueCache =
            new DoubleKeyDictionary<Attribute, Sku, List<EntityData>>();

        #endregion Fields

        #region Properties (1)

        public bool HasChanges
        {
            get
            {
                ChangeSet changeSet = GetChangeSet();
                return changeSet.Deletes.Count > 0 || changeSet.Inserts.Count > 0 || changeSet.Updates.Count > 0;
            }
        }


        public static Guid? DataRemark;
        #endregion Properties

        #region Methods (2)

        // Public Methods (1) 

        private readonly static ConcurrentDictionary<Type, List<Action<object>>> PropertyCache = new ConcurrentDictionary<Type, List<Action<object>>>(4,50);

        public static void DefaultTableValues(object o)
        {
            var currentType = o.GetType();

            List<Action<object>> actions = null;

            //Cache the PropertySet delegates
            if (!PropertyCache.ContainsKey(currentType))
            {
                actions = new List<Action<object>>();

                PropertyInfo id = currentType.GetProperty("ID");
                if (id != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("ID"))(p, Guid.NewGuid()));

                PropertyInfo createdBy = currentType.GetProperty("CreatedBy");
                if (createdBy != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("CreatedBy"))(p, AryaTools.Instance.InstanceData.CurrentUser.ID));

                PropertyInfo createdOn = currentType.GetProperty("CreatedOn");
                if (createdOn != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("CreatedOn"))(p, DateTime.Now));

                PropertyInfo active = currentType.GetProperty("Active");
                if (active != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("Active"))(p, true));

                PropertyInfo before = currentType.GetProperty("BeforeEntity");
                if (before != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("BeforeEntity"))(p, false));

                PropertyInfo isCanned = currentType.GetProperty("IsCanned");
                if (isCanned != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("IsCanned"))(p, true));

                PropertyInfo remark = currentType.GetProperty("CreatedRemark");
                if (remark != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("CreatedRemark"))(p, AryaTools.Instance.RemarkID));

                List<Action<object>> actions1 = actions;
                PropertyCache.AddOrUpdate(currentType, actions, (key, oldValue) => actions1);
            }

            if (actions == null)
            {
                actions = PropertyCache[currentType];
            }

            actions.ForEach(act => act(o));

        }
        // Private Methods (1) 

        partial void OnCreated()
        {
            var dlo = new DataLoadOptions();

            //dlo.AssociateWith<User>(user => user.UserProjects.Where(up => up.Project.Active));
           // dlo.AssociateWith<TaxonomyInfo>(ti => ti.TaxonomyDatas.Where(td => td.Active));

            dlo.LoadWith<Attribute>(att => att.AttributeGroups);
            dlo.LoadWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas);
            //dlo.LoadWith<Sku>(sku => sku.EntityInfos);
            dlo.LoadWith<EntityInfo>(ei => ei.EntityDatas);
         
            LoadOptions = dlo;
            CommandTimeout = 600;
        }

        public void BulkInsertAll<T>(IEnumerable<T> entities)
        {
            entities = entities.ToArray();

            string cs = Connection.ConnectionString;
            var conn = new SqlConnection(cs);
            conn.Open();

            Type t = typeof(T);

            var tableAttribute = (TableAttribute)t.GetCustomAttributes(
                typeof(TableAttribute), false).Single();
            var bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = tableAttribute.Name
            };

            var properties = t.GetProperties().Where(EventTypeFilter).ToArray();
            var table = new DataTable();

            foreach (var property in properties)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }

                table.Columns.Add(new DataColumn(property.Name, propertyType));
            }

            foreach (var entity in entities)
            {
                T entity1 = entity;
                table.Rows.Add(properties.Select(
                  property => GetPropertyValue(
                  property.GetValue(entity1, null))).ToArray());
            }

            bulkCopy.WriteToServer(table);
            conn.Close();
        }

        private bool EventTypeFilter(PropertyInfo p)
        {
            var attribute = System.Attribute.GetCustomAttribute(p,
                typeof(AssociationAttribute)) as AssociationAttribute;

            if (attribute == null) return true;
            if (attribute.IsForeignKey == false) return true;

            return false;
        }

        private object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }

        #endregion Methods
    }
}