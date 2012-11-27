using System.Runtime.Serialization;
using ASC.Files.Core;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "third_party", Namespace = "")]
    public class ThirdPartyParams
    {
        [DataMember(Name = "auth_data")]
        public AuthData AuthData { get; set; }

        [DataMember(Name = "corporate")]
        public bool Corporate { get; set; }

        [DataMember(Name = "customer_title")]
        public string CustomerTitle { get; set; }

        [DataMember(Name = "provider_id")]
        public string ProviderId { get; set; }

        [DataMember(Name = "provider_name")]
        public string ProviderName { get; set; }
    }
}