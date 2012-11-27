using System.Collections.Generic;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using System;

namespace ASC.Core
{
    public interface ITenantManagerClient
    {
        List<Tenant> GetTenants();

        Tenant GetTenant(int tenantId);

        Tenant GetTenant(string domain);

        Tenant SetTenantVersion(Tenant tenant, int version);

        Tenant SaveTenant(Tenant tenant);

        void RemoveTenant(int tenantId);

        void CheckTenantAddress(string address);

        IEnumerable<TenantVersion> GetTenantVersions();


        Tenant GetCurrentTenant();

        Tenant GetCurrentTenant(bool throwOnError);

        void SetCurrentTenant(Tenant tenant);

        void SetCurrentTenant(int tenantId);

        void SetCurrentTenant(string domain);


        IEnumerable<TenantQuota> GetTenantQuotas();

        TenantQuota GetTenantQuota(int tenant);

        void SetTenantQuotaRow(TenantQuotaRow row, bool exchange);

        List<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query);


        Tariff GetTariff(int tenantId);

        IEnumerable<PaymentInfo> GetTariffPayments(int tenant);

        Uri GetShoppingUri(int tenant, int quotaId);

        void SetTariffCoupon(int tenant, string coupon);
    }
}
