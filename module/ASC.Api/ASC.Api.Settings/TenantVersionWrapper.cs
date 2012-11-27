using System.Collections.Generic;
using ASC.Core;

namespace ASC.Api.Settings
{
    public class TenantVersionWrapper
    {
        public int Current { get; set; }
        public IEnumerable<TenantVersion> Versions { get; set; }

        public TenantVersionWrapper(int version, IEnumerable<TenantVersion> tenantVersions)
        {
            Current = version;
            Versions = tenantVersions;
        }
    }
}