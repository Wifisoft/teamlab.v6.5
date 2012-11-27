using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Api.Employee;

namespace ASC.Api.Settings
{
    [DataContract(Name = "security", Namespace = "")]
    public class SecurityWrapper
    {
        [DataMember]
        public string WebItemId { get; set; }

        [DataMember]
        public IEnumerable<EmployeeWraper> Users { get; set; }

        [DataMember]
        public IEnumerable<GroupWrapperSummary> Groups { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public bool IsSubItem { get; set; }
    }
}