using System;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantQuotaRowQuery
    {
        public int Tenant
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }


        public TenantQuotaRowQuery(int tenant)
        {
            Tenant = tenant;
        }


        public TenantQuotaRowQuery WithPath(string path)
        {
            Path = path;
            return this;
        }

        public TenantQuotaRowQuery WithLastModified(DateTime lastModified)
        {
            LastModified = lastModified;
            return this;
        }
    }
}