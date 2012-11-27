using System;
using System.Linq;
using System.Threading;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Data.Storage
{
    public class TennantQuotaController : IQuotaController
    {
        private readonly int tenant;
        private long currentSize;


        public TennantQuotaController(string tenantName)
        {
            int.TryParse(tenantName, out tenant);
            currentSize = CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(tenant))
                .Where(r => UsedInQuota(r.Tag))
                .Sum(r => r.Counter);
        }

        #region IQuotaController Members

        public void QuotaUsedAdd(string module, string domain, string dataTag, long size)
        {
            size = Math.Abs(size);
            if (UsedInQuota(dataTag))
            {
                var quota = CoreContext.TenantManager.GetTenantQuota(tenant);
                if (quota != null)
                {
                    if (quota.MaxFileSize != 0 && quota.MaxFileSize < size)
                    {
                        throw new TenantQuotaException(string.Format("Exceeds the maximum file size ({0}MB)", BytesToMegabytes(quota.MaxFileSize)));
                    }
                    if (quota.MaxTotalSize != 0 && quota.MaxTotalSize < currentSize + size)
                    {
                        throw new TenantQuotaException(string.Format("Exceeded maximum amount of disk quota ({0}MB)", BytesToMegabytes(quota.MaxTotalSize)));
                    }
                }
                Interlocked.Add(ref currentSize, size);
            }
            SetTenantQuotaRow(module, domain, size, dataTag, true);
        }

        public void QuotaUsedDelete(string module, string domain, string dataTag, long size)
        {
            size = -Math.Abs(size);
            if (UsedInQuota(dataTag))
            {
                Interlocked.Add(ref currentSize, size);
            }
            SetTenantQuotaRow(module, domain, size, dataTag, true);
        }

        public void QuotaUsedSet(string module, string domain, string dataTag, long size)
        {
            size = Math.Max(0, size);
            if (UsedInQuota(dataTag))
            {
                Interlocked.Exchange(ref currentSize, size);
            }
            SetTenantQuotaRow(module, domain, size, dataTag, false);
        }

        public long QuotaUsedGet(string module, string domain)
        {
            var path = string.IsNullOrEmpty(module) ? null : string.Format("/{0}/{1}", module, domain);
            return CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(tenant).WithPath(path))
                .Where(r => UsedInQuota(r.Tag))
                .Sum(r => r.Counter);
        }

        #endregion

        private void SetTenantQuotaRow(string module, string domain, long size, string dataTag, bool exchange)
        {
            CoreContext.TenantManager.SetTenantQuotaRow(
                new TenantQuotaRow { Tenant = tenant, Path = string.Format("/{0}/{1}", module, domain), Counter = size, Tag = dataTag },
                exchange);
        }

        private bool UsedInQuota(string tag)
        {
            return !string.IsNullOrEmpty(tag) && new Guid(tag) != Guid.Empty;
        }

        private double BytesToMegabytes(long bytes)
        {
            return Math.Round(bytes / 1024d / 1024d, 1);
        }
    }
}