using System;
using System.Runtime.Serialization;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "DataToImport", Namespace = "")]
    public class DataToImport
    {
        [DataMember(Name = "title", EmitDefaultValue = false, IsRequired = true)]
        public String Title { get; set; }

        [DataMember(Name = "content_link", EmitDefaultValue = false, IsRequired = true)]
        public String ContentLink { get; set; }

        [DataMember(Name = "create_by", EmitDefaultValue = false, IsRequired = false)]
        public string CreateBy { get; set; }

        [DataMember(Name = "create_on", EmitDefaultValue = false, IsRequired = false)]
        public String CreateOn { get; set; }
    }
}