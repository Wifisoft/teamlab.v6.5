#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Caching;
using log4net;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class AbstractDao : IDisposable
    {
        protected readonly List<EntityType> _supportedEntityType = new List<EntityType>();
        protected readonly ILog _log = LogManager.GetLogger("ASC.CRM");

        protected readonly Cache _cache = HttpRuntime.Cache;

        protected readonly String _contactCacheKey;
        protected readonly String _dealCacheKey;
        protected readonly String _caseCacheKey;
        protected readonly String _taskCacheKey;

        protected AbstractDao(int tenantID, String storageKey)
        {
            TenantID = tenantID;
            DbManager = new DbManager(storageKey);

            if (!DbRegistry.IsDatabaseRegistered(CRMConstants.DatabaseId))
            {
                DbRegistry.RegisterDatabase(CRMConstants.DatabaseId, WebConfigurationManager.ConnectionStrings[CRMConstants.DatabaseId]);
            }

            _supportedEntityType.Add(EntityType.Company);
            _supportedEntityType.Add(EntityType.Person);
            _supportedEntityType.Add(EntityType.Contact);
            _supportedEntityType.Add(EntityType.Opportunity);
            _supportedEntityType.Add(EntityType.Case);

            _contactCacheKey = String.Concat(TenantID, "/contact");
            _dealCacheKey = String.Concat(TenantID, "/deal");
            _caseCacheKey = String.Concat(TenantID, "/case");
            _taskCacheKey = String.Concat(TenantID, "/task");

        }


        protected DbManager DbManager
        {
            get;
            private set;
        }

        protected int TenantID
        {
            get;
            private set;
        }

        protected List<int> SearchByTags(EntityType entityType, int[] exceptIDs, IEnumerable<String> tags)
        {
            if (tags == null || !tags.Any())
                throw new ArgumentException();

            var tagIDs = new List<int>();

            foreach (var tag in tags)
                tagIDs.Add(DbManager.ExecuteScalar<int>(Query("crm_tag")
                      .Select("id")
                      .Where(Exp.Eq("entity_type", (int)entityType ) & Exp.Eq("title", tag))));
            
            var sqlQuery = new SqlQuery("crm_entity_tag")
                .Select("entity_id")
                .Select("count(*) as count")
              
                .GroupBy("entity_id")
                .Having(Exp.Eq("count", tags.Count()));
               
            if (exceptIDs != null && exceptIDs.Length > 0)
                sqlQuery.Where(Exp.In("entity_id", exceptIDs) & Exp.Eq("entity_type", (int)entityType));
            else
                sqlQuery.Where(Exp.Eq("entity_type", (int)entityType));

            sqlQuery.Where(Exp.In("tag_id", tagIDs));

            return DbManager.ExecuteList(sqlQuery).ConvertAll(row => Convert.ToInt32(row[0]));
        }

        protected Dictionary<int, int[]> GetRelativeToEntity(int[] contactID, EntityType entityType, int[] entityID)
        {
            var sqlQuery = new SqlQuery("crm_entity_contact");

            if (contactID != null && contactID.Length > 0 && (entityID == null || entityID.Length == 0))
                sqlQuery.Select("entity_id", "contact_id").Where(Exp.Eq("entity_type", entityType) & Exp.In("contact_id", contactID));
            else if (entityID != null && entityID.Length > 0 && (contactID == null || contactID.Length == 0))
                sqlQuery.Select("entity_id", "contact_id").Where(Exp.Eq("entity_type", entityType) & Exp.In("entity_id", entityID));

            var sqlResult = DbManager.ExecuteList(sqlQuery);

            return sqlResult.GroupBy(item => item[0])
                   .ToDictionary(item => Convert.ToInt32(item.Key),
                                item => item.Select(x => Convert.ToInt32(x[1])).ToArray());

        }

        protected int[] GetRelativeToEntity(int? contactID, EntityType entityType, int? entityID)
        {

            var sqlQuery = new SqlQuery("crm_entity_contact");

            if (contactID.HasValue && !entityID.HasValue)
                sqlQuery.Select("entity_id").Where(Exp.Eq("entity_type", entityType) & Exp.Eq("contact_id", contactID.Value));
            else if (!contactID.HasValue && entityID.HasValue)
                sqlQuery.Select("contact_id").Where(Exp.Eq("entity_type", entityType) & Exp.Eq("entity_id", entityID.Value));

            return DbManager.ExecuteList(sqlQuery).Select(row => Convert.ToInt32(row[0])).ToArray();

        }

        protected void SetRelative(int[] contactID, EntityType entityType, int entityID)
        {
            using (var tx = DbManager.BeginTransaction())
            {

                var sqlDelete = new SqlDelete("crm_entity_contact");

                if (entityID == 0)
                    throw new ArgumentException();

                sqlDelete.Where(Exp.Eq("entity_type", entityType) & Exp.Eq("entity_id", entityID));

                DbManager.ExecuteNonQuery(sqlDelete);

                if (!(contactID == null || contactID.Length == 0))
                    foreach (var id in contactID)
                        SetRelative(id, entityType, entityID);

                tx.Commit();
            }
        }

        protected void SetRelative(int contactID, EntityType entityType, int entityID)
        {
            DbManager.ExecuteNonQuery(new SqlInsert("crm_entity_contact", true)
                                       .InColumnValue("entity_id", entityID)
                                       .InColumnValue("entity_type", (int)entityType)
                                       .InColumnValue("contact_id", contactID)
                                    );
        }

        protected void RemoveRelative(int[] contactID, EntityType entityType, int[] entityID)
        {

            if (contactID != null && contactID.Length == 0 && entityID != null && entityID.Length == 0)
                throw new ArgumentException();

            var sqlQuery = new SqlDelete("crm_entity_contact");

            if (contactID != null && contactID.Length > 0)
                sqlQuery.Where(Exp.In("contact_id", contactID));

            if (entityID != null && entityID.Length > 0)
                sqlQuery.Where(Exp.In("entity_id", entityID) & Exp.Eq("entity_type", (int)entityType));

            DbManager.ExecuteNonQuery(sqlQuery);
        }

        protected void RemoveRelative(int contactID, EntityType entityType, int entityID)
        {
            int[] contactIDs = null;
            int[] entityIDs = null;


            if (contactID > 0)
                contactIDs = new[] { contactID };

            if (entityID > 0)
                entityIDs = new[] { entityID };


            RemoveRelative(contactIDs, entityType, entityIDs);
        }


        public void Dispose()
        {
            DbManager.Dispose();
        }

        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(GetTenantColumnName(table)).Values(TenantID);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";
            if (!table.Contains(" ")) return tenant;
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;
        }


        protected static Guid ToGuid(object guid)
        {
            var str = guid as string;
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }

        protected Exp BuildLike(string[] columns, string[] keywords)
        {
            return BuildLike(columns, keywords, true);
        }

        protected Exp BuildLike(string[] columns, string[] keywords, bool startWith)
        {
            var like = Exp.Empty;
            foreach (var keyword in keywords)
            {
                var keywordLike = Exp.Empty;
                foreach (string column in columns)
                {
                    keywordLike = keywordLike |
                                  Exp.Like(column, keyword, startWith ? SqlLike.StartWith : SqlLike.EndWith) |
                                  Exp.Like(column, ' ' + keyword);
                }
                like = like & keywordLike;
            }
            return like;
        }

    }
}
