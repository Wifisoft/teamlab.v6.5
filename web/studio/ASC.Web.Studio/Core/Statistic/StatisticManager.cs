using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Web.Studio.Core.Statistic
{
    class StatisticManager
    {
        private static readonly string dbId = "webstudio";
        private static DateTime lastSave = DateTime.UtcNow;
        private static TimeSpan cacheTime = TimeSpan.FromMinutes(2);
        private static IDictionary<string, UserVisit> cache = new Dictionary<string, UserVisit>();


        public static void SaveUserVisit(int tenantID, Guid userID, Guid productID)
        {
            var now = DateTime.UtcNow;
            var key = string.Format("{0}|{1}|{2}|{3}", tenantID, userID, productID, now.Date);

            lock (cache)
            {
                var visit = cache.ContainsKey(key) ?
                    cache[key] :
                    new UserVisit()
                    {
                        TenantID = tenantID,
                        UserID = userID,
                        ProductID = productID,
                        VisitDate = now
                    };

                visit.VisitCount++;
                visit.LastVisitTime = now;
                cache[key] = visit;
            }

            if (cacheTime < DateTime.UtcNow - lastSave)
            {
                FlushCache();
            }
        }

        public static List<Guid> GetVisitorsToday(int tenantID, Guid productID)
        {
            var users = DbManager
                .ExecuteList(
                    new SqlQuery("webstudio_uservisit")
                    .Select("UserID")
                    .Where("VisitDate", DateTime.UtcNow.Date)
                    .Where("TenantID", tenantID)
                    .Where("ProductID", productID.ToString())
                    .GroupBy(1)
                    .OrderBy("FirstVisitTime", true)
                )
                .ConvertAll(r => new Guid((string)r[0]));
            lock (cache)
            {
                foreach (var visit in cache.Values)
                {
                    if (!users.Contains(visit.UserID) && visit.VisitDate.Date == DateTime.UtcNow.Date)
                    {
                        users.Add(visit.UserID);
                    }
                }
            }
            return users;
        }

        public static List<UserVisit> GetHitsByPeriod(int tenantID, DateTime startDate, DateTime endPeriod)
        {
            return DbManager.ExecuteList(new SqlQuery("webstudio_uservisit")
              .Select("VisitDate")
              .SelectSum("VisitCount")
              .Where(Exp.Between("VisitDate", startDate, endPeriod))
              .Where("TenantID", tenantID)
              .GroupBy("VisitDate")
              .OrderBy("VisitDate", true))
              .ConvertAll(r => new UserVisit { VisitDate = Convert.ToDateTime(r[0]), VisitCount = Convert.ToInt32(r[1]) });
        }

        public static List<UserVisit> GetHostsByPeriod(int tenantID, DateTime startDate, DateTime endPeriod)
        {
            return DbManager.ExecuteList(new SqlQuery("webstudio_uservisit")
              .Select("VisitDate", "UserId")
              .Where(Exp.Between("VisitDate", startDate, endPeriod))
              .Where("TenantID", tenantID)
              .GroupBy("UserId", "VisitDate")
              .OrderBy("VisitDate", true))
              .ConvertAll(r => new UserVisit { VisitDate = Convert.ToDateTime(r[0]), UserID = new Guid(Convert.ToString(r[1])) });
        }

        private static void FlushCache()
        {
            if (cache.Count == 0) return;

            List<UserVisit> visits = null;
            lock (cache)
            {
                visits = new List<UserVisit>(cache.Values);
                cache.Clear();
                lastSave = DateTime.UtcNow;
            }

            var db = DbManager;
            using (var tx = db.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                foreach (var v in visits)
                {
                    var pk = Exp.Eq("TenantID", v.TenantID) & Exp.Eq("UserID", v.UserID.ToString()) & Exp.Eq("ProductID", v.ProductID.ToString()) & Exp.Eq("VisitDate", v.VisitDate.Date);

                    var count = db.ExecuteScalar<long>(new SqlQuery("webstudio_uservisit").SelectCount().Where(pk));
                    if (count == 0)
                    {
                        db.ExecuteNonQuery(
                            new SqlInsert("webstudio_uservisit")
                            .InColumns("TenantID", "ProductID", "UserID", "VisitDate", "FirstVisitTime", "LastVisitTime", "VisitCount")
                            .Values(v.TenantID, v.ProductID.ToString(), v.UserID.ToString(), v.VisitDate.Date, v.VisitDate, v.LastVisitTime, v.VisitCount)
                        );
                    }
                    else
                    {
                        db.ExecuteNonQuery(
                            new SqlUpdate("webstudio_uservisit")
                            .Set("LastVisitTime", v.LastVisitTime)
                            .Set("VisitCount = VisitCount + " + v.VisitCount)
                            .Where(pk)
                        );
                    }
                }
                tx.Commit();
            }
        }

        private static DbManager DbManager
        {
            get { return DbManager.FromHttpContext(dbId); }
        }
    }
}