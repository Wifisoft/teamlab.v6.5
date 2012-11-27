using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface IQuotaService
    {
        IEnumerable<TenantQuota> GetTenantQuotas();

        TenantQuota GetTenantQuota(int tenant);

        TenantQuota SaveTenantQuota(TenantQuota quota);

        void RemoveTenantQuota(int tenant);

        
        IEnumerable<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query);

        void SetTenantQuotaRow(TenantQuotaRow row, bool exchange);
    }
}
