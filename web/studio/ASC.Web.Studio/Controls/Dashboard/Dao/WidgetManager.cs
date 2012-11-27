using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Caching;
using log4net;

namespace ASC.Web.Studio.Controls.Dashboard.Dao
{
    static class WidgetManager
    {
        private const string dbId = "webstudio";
        private static readonly ICache Cache = new AspCache();
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);


        public static int[] GetColumnSchemaPercents(ColumnSchemaType columnSchemaType)
        {
            switch (columnSchemaType)
            {
                case ColumnSchemaType.Schema_25_50_25: return new[] { 25, 50, 25 };
                case ColumnSchemaType.Schema_33_33_33: return new[] { 33, 34, 33 };
                case ColumnSchemaType.Schema_65_35: return new[] { 65, 35, 0 };
                default: throw new ArgumentOutOfRangeException("columnSchemaType");
            }
        }

        public static WidgetContainer GetWidgetContainer(Guid containerID, int tenantID, Guid userID)
        {
            return GetContainers(tenantID).FirstOrDefault(c => c.ContainerID == containerID && c.UserID == userID);
        }

        public static WidgetContainer GetWidgetContainer(Guid id)
        {
            return GetContainers(CoreContext.TenantManager.GetCurrentTenant().TenantId).FirstOrDefault(c => c.ID == id);
        }

        public static bool SaveWidgetContainer(WidgetContainer container)
        {
            try
            {
                ResetContainers(container.TenantID);

                using (var db = GetDb())
                using (var tr = db.BeginTransaction())
                {
                    db.ExecuteNonQuery(new SqlInsert("webstudio_widgetcontainer", true)
                        .InColumnValue("ID", container.ID.ToString())
                        .InColumnValue("ContainerID", container.ContainerID.ToString())
                        .InColumnValue("UserID", container.UserID.ToString())
                        .InColumnValue("TenantID", container.TenantID)
                        .InColumnValue("SchemaID", (int)container.ColumnSchemaType));

                    var columnsCount = GetColumnSchemaPercents(container.ColumnSchemaType).Length;
                    db.ExecuteNonQuery(new SqlDelete("webstudio_widgetstate").Where("WidgetContainerID", container.ID.ToString()));
                    foreach (var s in container.States)
                    {
                        db.ExecuteNonQuery(new SqlInsert("webstudio_widgetstate")
                            .InColumnValue("WidgetID", s.ID.ToString())
                            .InColumnValue("WidgetContainerID", s.ContainerID.ToString())
                            .InColumnValue("ColumnID", s.X < columnsCount ? s.X : 0)
                            .InColumnValue("SortOrderInColumn", s.Y));
                    }
                    tr.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(WidgetManager)).Error(ex);
                return false;
            }
        }


        private static DbManager GetDb()
        {
            return DbManager.FromHttpContext(dbId);
        }

        private static IEnumerable<WidgetContainer> GetContainers(int tenant)
        {
            var containers = Cache.Get(GetKey(tenant)) as List<WidgetContainer>;
            if (containers == null)
            {
                var dic = new Dictionary<Guid, WidgetContainer>();
                var q = new SqlQuery()
                    .From("webstudio_widgetcontainer c")
                    .LeftOuterJoin("webstudio_widgetstate s", Exp.EqColumns("c.ID", "s.WidgetContainerID"))
                    .Select("c.ID", "c.ContainerID", "c.UserID", "c.TenantID", "c.SchemaID", "s.WidgetID", "s.ColumnID", "s.SortOrderInColumn")
                    .Where("c.tenantid", tenant)
                    .OrderBy("c.ID", true);

                using (var db = GetDb())
                {
                    foreach (var r in db.ExecuteList(q))
                    {
                        var id = new Guid((string)r[0]);
                        if (!dic.ContainsKey(id))
                        {
                            dic[id] = new WidgetContainer(id, new Guid((string)r[1]))
                            {
                                UserID = new Guid((string)r[2]),
                                TenantID = Convert.ToInt32(r[3]),
                                ColumnSchemaType = (ColumnSchemaType)Convert.ToInt32(r[4]),
                            };
                        }
                        if (r[5] != null)
                        {
                            dic[id].States.Add(new WidgetState(new Guid((string)r[5]), id) { X = Convert.ToInt32(r[6]), Y = Convert.ToInt32(r[7]) });
                        }
                    }
                }

                containers = dic.Values.ToList();
                Cache.Insert(GetKey(tenant), containers, CacheExpiration);
            }
            return containers;
        }

        private static void ResetContainers(int tenant)
        {
            Cache.Remove(GetKey(tenant));
        }

        private static string GetKey(int tenant)
        {
            return string.Format("{0}/WidgetManager", tenant);
        }
    }
}
