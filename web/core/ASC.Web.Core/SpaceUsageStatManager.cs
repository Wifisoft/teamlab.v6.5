using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Web.Core
{
    public abstract class SpaceUsageStatManager
    {
        public class UsageSpaceStatItem        
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string ImgUrl { get; set; }
            public long SpaceUsage { get; set; }
        }

        public abstract List<UsageSpaceStatItem> GetStatData();


        protected IEnumerable<TenantQuotaRow> GetQuotaRows()
        {
            return CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(CoreContext.TenantManager.GetCurrentTenant().TenantId))
                .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty);
        }

        protected long GetUsedSize(Guid productOrModuleId)
        {
            var rows = GetQuotaRows();
            return rows.Any(r => new Guid(r.Tag) == productOrModuleId) ? rows.Where(r => new Guid(r.Tag) == productOrModuleId).Sum(r => r.Counter) : -1L;
        }
    }
}
