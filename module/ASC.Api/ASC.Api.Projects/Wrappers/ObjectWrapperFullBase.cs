using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Namespace = "")]
    public class ObjectWrapperFullBase : ObjectWrapperBase,IApiSortableDate
    {
        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 51,EmitDefaultValue = false)]
        public EmployeeWraper CreatedBy { get; set; }

        private ApiDateTime _updated;

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public ApiDateTime Updated
        {
            get
            {
                return _updated < Created ? Created : _updated;
            }
            set { _updated = value; }
        }


        [DataMember(Order = 41, EmitDefaultValue = false)]
        public EmployeeWraper UpdatedBy { get; set; }
    }
}