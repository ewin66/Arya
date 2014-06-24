using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Arya.Framework.Collections.Generic;
using Arya.Framework.Utility;

namespace Arya.Framework.Data.AryaDb
{
    public partial class AryaDbDataContext
    {
        #region Fields

        private static readonly DoubleKeyDictionary<Guid, Guid, DateTime> AuthorizedUsers =
            new DoubleKeyDictionary<Guid, Guid, DateTime>();

        private static readonly ConcurrentDictionary<Type, List<Action<object>>> CreatePropertyCache =
            new ConcurrentDictionary<Type, List<Action<object>>>(4, 50);

        private static readonly ConcurrentDictionary<Type, List<Action<object>>> DeletePropertyCache =
            new ConcurrentDictionary<Type, List<Action<object>>>(4, 50);

        //public readonly Dictionary<string, Attribute> AttributeCache = new Dictionary<string, Attribute>();

        public readonly DoubleKeyDictionary<Attribute, Sku, List<EntityData>> SkuAttributeValueCache =
            new DoubleKeyDictionary<Attribute, Sku, List<EntityData>>();

        #endregion Fields

        #region Constructors

        public AryaDbDataContext(Guid projectID, Guid userID)
            : this(
                projectID, userID,
                Util.GetAryaDbConnectionString(projectID))
        {
        }

        public AryaDbDataContext(Guid projectID, Guid userID, string connectionString)
            : base(connectionString, mappingSource)
        {
            OnCreated();
            InitDataLoadOptions();
            ChangeProject(projectID);
            SetCurrentUser(userID, projectID);
            CommandTimeout = 10000;
        }

        #endregion Constructors

        #region Properties

        public Project CurrentProject { get; private set; }

        public User CurrentUser { get; private set; }

        #endregion Properties

        #region Methods

        //http://msdn.microsoft.com/en-us/library/dd287757.aspx
        public static void DefaultDeletedTableValue(object o, Guid userID)
        {
            var currentType = o.GetType();

            List<Action<object>> actions = null;

            //Cache the PropertySet delegates
            if (!DeletePropertyCache.ContainsKey(currentType))
            {
                actions = new List<Action<object>>();

                var createdBy = currentType.GetProperty("DeletedBy");
                if (createdBy != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("DeletedBy"))(p, userID));

                var createdOn = currentType.GetProperty("DeletedOn");
                if (createdOn != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("DeletedOn"))(p, DateTime.Now));

                var actions1 = actions;
                DeletePropertyCache.AddOrUpdate(currentType, actions, (key, oldValue) => actions1);
            }

            if (actions == null)
                actions = DeletePropertyCache[currentType];

            actions.ForEach(act => act(o));
        }

        public static void DefaultInsertedTableValues(object o, Guid userID, Guid? projectID = null)
        {
            var currentType = o.GetType();

            List<Action<object>> actions = null;

            //Cache the PropertySet delegates
            if (!CreatePropertyCache.ContainsKey(currentType))
            {
                actions = new List<Action<object>>();

                var id = currentType.GetProperty("ID");
                if (id != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("ID"))(p, Guid.NewGuid()));

                var createdBy = currentType.GetProperty("CreatedBy");
                if (createdBy != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("CreatedBy"))(p, userID));

                var createdOn = currentType.GetProperty("CreatedOn");
                if (createdOn != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("CreatedOn"))(p, DateTime.Now));

                var active = currentType.GetProperty("Active");
                if (active != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("Active"))(p, true));

                var before = currentType.GetProperty("BeforeEntity");
                if (before != null)
                    actions.Add(p => (currentType.DelegateForSetPropertyValue("BeforeEntity"))(p, false));

                if (projectID != null)
                {
                    var projectId = currentType.GetProperty("ProjectID");
                    if (projectId != null)
                        actions.Add(p => (currentType.DelegateForSetPropertyValue("ProjectID"))(p, projectID));
                }

                var actions1 = actions;
                CreatePropertyCache.AddOrUpdate(currentType, actions, (key, oldValue) => actions1);
            }

            if (actions == null)
                actions = CreatePropertyCache[currentType];

            actions.ForEach(act => act(o));
        }

        public void BulkInsertAll<T>(IEnumerable<T> entities, string tableName, string databaseName,
            int batchSize = 10000)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Value cannot be null or empty or whitespace", "databaseName");

            var cs = Connection.ConnectionString;

            var conn = new SqlConnection(cs);

            conn.Open();
            conn.ChangeDatabase(!string.IsNullOrWhiteSpace(databaseName) ? databaseName : Connection.Database);

            var t = typeof(T);
            var bulkCopy = new SqlBulkCopy(conn) { BulkCopyTimeout = 99999 };

            if (string.IsNullOrWhiteSpace(tableName))
            {
                var tableAttribute = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false).Single();
                bulkCopy.DestinationTableName = tableAttribute.Name;
            }
            else
            {
                tableName = tableName.Trim();

                if (!tableName.StartsWith("["))
                    tableName = "[" + tableName;

                if (!tableName.EndsWith("]"))
                    tableName = tableName + "]";

                bulkCopy.DestinationTableName = tableName;
            }

            var properties = (from p in t.GetProperties()
                              let ca = p.GetCustomAttributes(typeof(ColumnAttribute), false) as ColumnAttribute[]
                              where ca != null && ca.Any()
                              select p).Where(EventTypeFilter).ToArray();

            var i = 1;
            var currentTable = GetDataTable(properties);

            foreach (var entity in entities)
            {
                if (i % batchSize == 0)
                {
                    bulkCopy.WriteToServer(currentTable);
                    currentTable.Rows.Clear();
                }

                var entity1 = entity;
                currentTable.Rows.Add(
                    properties.Select(property => GetPropertyValue(property.GetValue(entity1, null))).ToArray());
                i++;
            }

            if (currentTable.Rows.Count > 0)
            {
                bulkCopy.WriteToServer(currentTable);
                currentTable.Rows.Clear();
            }

            conn.Close();
            conn.Dispose();
        }

        public void BulkInsertAllIntoTempdb<T>(IEnumerable<T> entities, string tableName)
        {
            BulkInsertAll(entities, tableName, "tempdb");
        }

        private static bool EventTypeFilter(PropertyInfo p)
        {
            var attribute =
                System.Attribute.GetCustomAttribute(p, typeof(AssociationAttribute)) as AssociationAttribute;

            if (attribute == null)
                return true;
            return attribute.IsForeignKey == false;
        }

        private static DataTable GetDataTable(IEnumerable<PropertyInfo> properties)
        {
            var tableTemplate = new DataTable();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);

                tableTemplate.Columns.Add(new DataColumn(property.Name, propertyType));
            }

            return tableTemplate;
        }

        private static object GetPropertyValue(object o) { return o ?? DBNull.Value; }

        private void SetCurrentUser(Guid userID, Guid projectID)
        {
            if (!AuthorizedUsers.ContainsKeys(userID, projectID))
            {
                var authorized = UserProjects.Any(up => up.ProjectID == projectID && up.UserID == userID);
                if (authorized)
                    AuthorizedUsers[userID, projectID] = DateTime.Now;
            }
            //set current user
            
            if (AuthorizedUsers.ContainsKeys(userID, projectID))
            {
                CurrentUser = CurrentProject.UserProjects.Where(up => up.UserID == userID).Select(up => up.User).First();

                if (AuthorizedUsers[userID, projectID].AddHours(1) < DateTime.Now)
                    AuthorizedUsers.Remove(userID, projectID);
                return;
            }

            throw new UnauthorizedAccessException();
        }

        //TODO: Give optional database and void connecting to Arya4
        private void ChangeProject(Guid projectID)
        {
            if (!Util.ConnectionStrings.ContainsKey(projectID) || Connection.ConnectionString != Util.ConnectionStrings[projectID])
            {
                var project = Projects.Where(p => p.ID == projectID).FirstOrDefault();
                if (project == null)
                    throw new ArgumentException("Invalid Project ID");

                bool result;

                try
                {
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();
                    result =
                        ExecuteQuery<int>(@"SELECT database_id FROM sys.databases WHERE Name = {0}",
                            project.DatabaseName).SingleOrDefault() > 0;
                }
                catch (Exception)
                {
                    result = false;
                }

                if (!result)
                    throw new ArgumentException("Project database does not exist");

                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                Connection.ChangeDatabase(project.DatabaseName);

                string cs = Util.GetAryaDbConnectionString(Guid.Empty);

                Util.ConnectionStrings[projectID] = cs.Replace("Arya", project.DatabaseName);
            }

            //set current project
            CurrentProject = Projects.Single(p => p.ID == projectID);
        }

        private void InitDataLoadOptions()
        {
            var dlo = new DataLoadOptions();
            dlo.LoadWith<TaxonomyInfo>(taxonomyInfo => taxonomyInfo.TaxonomyDatas);
            dlo.LoadWith<EntityInfo>(entityInfo => entityInfo.EntityDatas);
            dlo.LoadWith<SchemaInfo>(schemaInfo => schemaInfo.SchemaDatas);

            LoadOptions = dlo;
        }

        #endregion Methods
    }
}