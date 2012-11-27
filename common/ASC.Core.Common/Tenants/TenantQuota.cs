using System;
using System.Diagnostics;
using System.Linq;

namespace ASC.Core.Tenants
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class TenantQuota
    {
        public static readonly TenantQuota Default = new TenantQuota(Tenants.Tenant.DEFAULT_TENANT)
        {
            Name = "Default",
            MaxFileSize = 25 * 1024 * 1024, // 25Mb
            MaxTotalSize = long.MaxValue,
            ActiveUsers = int.MaxValue,
        };


        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Desc
        {
            get;
            set;
        }

        public long MaxFileSize
        {
            get;
            set;
        }

        public long MaxTotalSize
        {
            get;
            set;
        }

        public int ActiveUsers
        {
            get;
            set;
        }

        public string Features
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public bool Trial
        {
            get { return GetFeature("trial"); }
            set { SetFeature("trial", value); }
        }

        public bool HasBackup
        {
            get { return GetFeature("backup"); }
            set { SetFeature("backup", value); }
        }

        public bool HasSupport
        {
            get { return GetFeature("support"); }
            set { SetFeature("support", value); }
        }

        public bool HasDomain
        {
            get { return GetFeature("domain"); }
            set { SetFeature("domain", value); }
        }

        public string AvangateId
        {
            get;
            set;
        }


        public TenantQuota(int tenant)
        {
            Id = tenant;
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var q = obj as TenantQuota;
            return q != null && q.Id == Id;
        }


        private bool GetFeature(string feature)
        {
            if (!string.IsNullOrEmpty(Features))
            {
                return Features.Split(' ', ',', ';').Contains(feature);
            }
            return false;
        }

        private void SetFeature(string feature, bool set)
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            if (set && !features.Contains(feature))
            {
                features.Add(feature);
            }
            else if (!set && features.Contains(feature))
            {
                features.Remove(feature);
            }
            Features = string.Join(",", features.ToArray());
        }
    }
}
