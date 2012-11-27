using System.Runtime.Serialization;
using ASC.Api.Employee;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Namespace = "")]
    public class ObjectWrapperBase
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 10)]
        public string Title { get; set; }

        [DataMember(Order = 11)]
        public string Description { get; set; }

        [DataMember(Order = 20)]
        public int Status { get; set; }

        [DataMember(Order = 30)]
        public EmployeeWraper Responsible { get; set; }
    }
}