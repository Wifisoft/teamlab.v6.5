using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Statistics
{
    static class TenantStatisticsProvider
    {
        public static DateTime GetCreationDate()
        {
            return CoreContext.TenantManager.GetCurrentTenant().CreatedDateTime;
        }

        public static int GetUsersCount()
        {
            return CoreContext.UserManager.GetUsers(EmployeeStatus.All).Length;
        }

        public static long GetUsedSize()
        {
            return GetQuotaRows().Sum(r => r.Counter);
        }

        private static IEnumerable<TenantQuotaRow> GetQuotaRows()
        {
            return CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(CoreContext.TenantManager.GetCurrentTenant().TenantId))
                .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty);
        }
    }
}