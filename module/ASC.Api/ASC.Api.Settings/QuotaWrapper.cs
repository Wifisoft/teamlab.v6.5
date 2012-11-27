using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core.Tenants;

namespace ASC.Api.Settings
{
    [DataContract(Name = "quota", Namespace = "")]
    public class QuotaWrapper
    {
        [DataMember]
        public ulong StorageSize { get; set; }

        [DataMember]
        public ulong MaxFileSize { get; set; }

        [DataMember]
        public ulong UsedSize { get; set; }

        [DataMember]
        public ulong AvailableSize
        {
            get { return Math.Max(0, StorageSize - UsedSize); }
        }

        [DataMember]
        public IList<QuotaUsage> StorageUsage { get; set; }

        private QuotaWrapper()
        {
            
        }

        public QuotaWrapper(TenantQuota quota, IList<TenantQuotaRow> quotaRows)
        {
            StorageSize = (ulong) Math.Max(0, quota.MaxTotalSize);
            MaxFileSize = (ulong) Math.Max(0, quota.MaxFileSize);
            UsedSize = (ulong) Math.Max(0, quotaRows.Sum(r => r.Counter));
            StorageUsage =
                quotaRows.Select(x => new QuotaUsage() {Path = x.Path.TrimStart('/').TrimEnd('/'), Size = x.Counter,}).
                    ToList();
        }

        public static QuotaWrapper GetSample()
        {
            return new QuotaWrapper()
                       {
                           MaxFileSize = 25 * 1024 * 1024, 
                           StorageSize = 1024 * 1024 * 1024,
                           UsedSize = 250 * 1024 * 1024,
                           StorageUsage = new List<QuotaUsage>()
                                              {
                                                  new QuotaUsage(){Size = 100 * 1024 * 1024,Path = "crm"},
                                                  new QuotaUsage(){Size = 150 * 1024 * 1024,Path = "files"}
                                              }
                       };
        }

}
}