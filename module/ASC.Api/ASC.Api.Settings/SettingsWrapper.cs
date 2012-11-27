using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Core.Tenants;

namespace ASC.Api.Settings
{
    [DataContract(Name = "settings", Namespace = "")]
    public class SettingsWrapper
    {
        [DataMember]
        public string Timezone { get; set; }

        [DataMember]
        public List<string> TrustedDomains { get; set; }

        [DataMember]
        public TenantTrustedDomainsType TrustedDomainsType { get; set; }

        [DataMember]
        public string Culture { get; set; }

        [DataMember]
        public TimeSpan UtcOffset { get; set; }


        [DataMember]
        public double UtcHoursOffset { get; set; }

        public static SettingsWrapper GetSample()
        {
            return new SettingsWrapper()
                       {
                           Culture = "en-US",
                           Timezone = TimeZoneInfo.Utc.ToString(),
                           TrustedDomains = new List<string>() {"mydomain.com"},
                           UtcHoursOffset = -8.5,
                           UtcOffset = TimeSpan.FromHours(-8.5)
                       };
        }
    }
}