using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbQuotaService : DbBaseService, IQuotaService
    {
        private const string tenants_quota = "tenants_quota";
        internal const string tenants_quotarow = "tenants_quotarow";


        public DbQuotaService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {
        }


        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return GetTenantQuotas(null);
        }

        public TenantQuota GetTenantQuota(int tenant)
        {
            return GetTenantQuotas(Exp.Eq("tenant", tenant))
                .SingleOrDefault();
        }

        private IEnumerable<TenantQuota> GetTenantQuotas(Exp where)
        {
            var q = new SqlQuery(tenants_quota)
                .Select("tenant", "name", "description", "max_file_size", "max_total_size", "active_users", "features", "price", "avangate_id")
                .Where(where);

            return ExecList(q)
                .ConvertAll(r => new TenantQuota(Convert.ToInt32(r[0]))
                {
                    Name = (string)r[1],
                    Desc = (string)r[2],
                    MaxFileSize = GetInBytes(Convert.ToInt64(r[3])),
                    MaxTotalSize = GetInBytes(Convert.ToInt64(r[4])),
                    ActiveUsers = Convert.ToInt32(r[5]) != 0 ? Convert.ToInt32(r[5]) : int.MaxValue,
                    Features = (string)r[6],
                    Price = Convert.ToDecimal(r[7]),
                    AvangateId = (string)r[8],
                });
        }

        
        public TenantQuota SaveTenantQuota(TenantQuota quota)
        {
            if (quota == null) throw new ArgumentNullException("quota");

            var i = Insert(tenants_quota, quota.Id)
                .InColumnValue("name", quota.Name)
                .InColumnValue("description", quota.Desc)
                .InColumnValue("max_file_size", quota.MaxFileSize)
                .InColumnValue("max_total_size", quota.MaxTotalSize)
                .InColumnValue("active_users", quota.ActiveUsers)
                .InColumnValue("features", quota.Features)
                .InColumnValue("price", quota.Price)
                .InColumnValue("avangate_id", quota.AvangateId);

            ExecNonQuery(i);
            return quota;
        }

        public void RemoveTenantQuota(int tenant)
        {
            var d = Delete(tenants_quota, tenant);
            ExecNonQuery(d);
        }


        public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
        {
            if (row == null) throw new ArgumentNullException("row");

            ExecAction(db =>
            {
                var counter = db.ExecScalar<long>(Query(tenants_quotarow, row.Tenant)
                    .Select("counter")
                    .Where("path", row.Path));

                db.ExecNonQuery(Insert(tenants_quotarow, row.Tenant)
                    .InColumnValue("path", row.Path)
                    .InColumnValue("counter", exchange ? counter + row.Counter : row.Counter)
                    .InColumnValue("tag", row.Tag)
                    .InColumnValue("last_modified", DateTime.UtcNow));
            });
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var q = new SqlQuery(tenants_quotarow).Select("tenant", "path", "counter", "tag");
            if (query.Tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where("tenant", query.Tenant);
            }
            if (!string.IsNullOrEmpty(query.Path))
            {
                q.Where("path", query.Path);
            }
            if (query.LastModified != default(DateTime))
            {
                q.Where(Exp.Ge("last_modified", query.LastModified));
            }

            return ExecList(q)
                .ConvertAll(r => new TenantQuotaRow
                {
                    Tenant = Convert.ToInt32(r[0]),
                    Path = (string)r[1],
                    Counter = Convert.ToInt64(r[2]),
                    Tag = (string)r[3],
                });
        }


        private long GetInBytes(long bytes)
        {
            const long MB = 1024 * 1024;
            return bytes < MB ? bytes * MB : bytes;
        }
    }
}
