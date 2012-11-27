using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Threading;
using System.Web;
using ASC.Core.Billing;
using ASC.Core.Tenants;


namespace ASC.Core
{
    class ClientTenantManager : ITenantManagerClient
    {
        private const string CURRENT_TENANT = "CURRENT_TENANT";
        private readonly ITenantService tenantService;
        private readonly IQuotaService quotaService;
        private readonly ITariffService tariffService;
        private readonly List<string> thisCompAddresses = new List<string>();


        public ClientTenantManager(ITenantService tenantService, IQuotaService quotaService, ITariffService tariffService)
        {
            this.tenantService = tenantService;
            this.quotaService = quotaService;
            this.tariffService = tariffService;

            thisCompAddresses.Add("localhost");
            thisCompAddresses.Add(Dns.GetHostName().ToLowerInvariant());
            thisCompAddresses.AddRange(Dns.GetHostAddresses("localhost").Select(a => a.ToString()));
            thisCompAddresses.AddRange(Dns.GetHostAddresses(Dns.GetHostName()).Select(a => a.ToString()));
        }


        public List<Tenant> GetTenants()
        {
            return tenantService.GetTenants(default(DateTime)).ToList();
        }

        public Tenant GetTenant(int tenantId)
        {
            return GetTenants().SingleOrDefault(t => t.TenantId == tenantId);
        }

        public Tenant GetTenant(string domain)
        {
            if (string.IsNullOrEmpty(domain)) return null;

            Tenant t = null;
            if (thisCompAddresses.Contains(domain, StringComparer.InvariantCultureIgnoreCase))
            {
                t = tenantService.GetTenant(0);
            }
            if (t == null)
            {
                var baseUrl = ConfigurationManager.AppSettings["core.base-domain"];
                if (baseUrl != null && domain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    t = tenantService.GetTenant(domain.Substring(0, domain.Length - baseUrl.Length - 1));
                }
            }
            if (t == null)
            {
                t = tenantService.GetTenant(domain);
            }
            if (t == null && CoreContext.Configuration.Standalone)
            {
                t = GetTenants().FirstOrDefault();
            }
            return t;
        }

        public Tenant SetTenantVersion(Tenant tenant, int version)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            if (tenant.Version != version)
            {
                tenant.Version = version;
                SaveTenant(tenant);
            }
            else
            {
                throw new ArgumentException("This is current version already");
            }
            return tenant;
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            var newTenant = tenantService.SaveTenant(tenant);

            var oldTenant = CallContext.GetData(CURRENT_TENANT) as Tenant;
            if (oldTenant != null) SetCurrentTenant(newTenant);

            return newTenant;
        }

        public void RemoveTenant(int tenantId)
        {
            tenantService.RemoveTenant(tenantId);
        }

        public Tenant GetCurrentTenant()
        {
            return GetCurrentTenant(true);
        }

        public Tenant GetCurrentTenant(bool throwIfNotFound)
        {
            var tenant = CallContext.GetData(CURRENT_TENANT) as Tenant;
            if (tenant == null && HttpContext.Current != null)
            {
                tenant = HttpContext.Current.Items[CURRENT_TENANT] as Tenant;
                if (tenant == null && HttpContext.Current.Request != null)
                {
                    tenant = GetTenant(HttpContext.Current.Request.GetUrlRewriter().Host);
                    HttpContext.Current.Items[CURRENT_TENANT] = tenant;
                }
            }
            if (tenant == null && throwIfNotFound)
            {
                throw new Exception("Could not resolve current tenant :-(.");
            }
            return tenant;
        }

        public void SetCurrentTenant(Tenant tenant)
        {
            if (tenant != null)
            {
                CallContext.SetData(CURRENT_TENANT, tenant);
                Thread.CurrentThread.CurrentCulture = tenant.GetCulture();
                Thread.CurrentThread.CurrentUICulture = tenant.GetCulture();
            }
        }

        public void SetCurrentTenant(int tenantId)
        {
            SetCurrentTenant(GetTenant(tenantId));
        }

        public void SetCurrentTenant(string domain)
        {
            SetCurrentTenant(GetTenant(domain));
        }

        public void CheckTenantAddress(string address)
        {
            tenantService.ValidateDomain(address);
        }

        public IEnumerable<TenantVersion> GetTenantVersions()
        {
            return tenantService.GetTenantVersions();
        }


        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return quotaService.GetTenantQuotas().Where(q => q.Id < 0).OrderByDescending(q => q.Id).ToList();
        }

        public TenantQuota GetTenantQuota(int tenant)
        {
            var q = quotaService.GetTenantQuota(tenant) ?? quotaService.GetTenantQuota(Tenant.DEFAULT_TENANT) ?? TenantQuota.Default;
            if (q.Id != tenant && tariffService != null)
            {
                var tariffQuota = quotaService.GetTenantQuota(tariffService.GetTariff(tenant).QuotaId);
                if (tariffQuota != null)
                {
                    return tariffQuota;
                }
            }
            return q;
        }

        public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
        {
            quotaService.SetTenantQuotaRow(row, exchange);
        }

        public List<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query)
        {
            return quotaService.FindTenantQuotaRows(query).ToList();
        }


        public Tariff GetTariff(int tenantId)
        {
            return tariffService != null ? tariffService.GetTariff(tenantId) : Tariff.CreateDefault();
        }

        public IEnumerable<PaymentInfo> GetTariffPayments(int tenant)
        {
            return tariffService != null ? tariffService.GetPayments(tenant) : Enumerable.Empty<PaymentInfo>();
        }

        public Uri GetShoppingUri(int tenant, int quotaId)
        {
            return tariffService != null ? tariffService.GetShoppingUri(tenant, quotaId) : null;
        }

        public void SetTariffCoupon(int tenant, string coupon)
        {
            if (tariffService != null)
            {
                tariffService.SetTariffCoupon(tenant, coupon);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
