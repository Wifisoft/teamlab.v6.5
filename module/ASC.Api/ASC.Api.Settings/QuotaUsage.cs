using System.Runtime.Serialization;

namespace ASC.Api.Settings
{
    [DataContract(Name = "quota_usage", Namespace = "")]
    public class QuotaUsage
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public long Size { get; set; }

    }
}