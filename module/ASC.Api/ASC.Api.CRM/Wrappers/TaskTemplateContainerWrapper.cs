#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Namespace = "taskTemplateContainer")]
    public class TaskTemplateContainerWrapper : ObjectWrapperBase
    {

        public TaskTemplateContainerWrapper() : 
            base(0)
        {
            
        }
       
        public TaskTemplateContainerWrapper(int id)
            : base(id)
        {
            
        }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public String Title { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public String EntityType { get; set; }
        
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<TaskTemplateWrapper> Items { get; set; }

        public static TaskTemplateContainerWrapper GetSample()
        {
            return new TaskTemplateContainerWrapper
            {
                EntityType = "contact",
                Title = "Birthday greetings",
                Items = new List<TaskTemplateWrapper>()
                            {
                                TaskTemplateWrapper.GetSample()
                            }
            };
        }

    }

    [DataContract(Namespace = "taskTemplate")]
    public class TaskTemplateWrapper : ObjectWrapperBase
    {

        public TaskTemplateWrapper():base(0)
        {
            
        }

        public TaskTemplateWrapper(int id) : 
            base(id)
        {


        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int ContainerID { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TaskCategory Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool isNotify { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public long OffsetTicks { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DeadLineIsFixed { get; set; }

        public static TaskTemplateWrapper GetSample()
        {
            return new TaskTemplateWrapper
                       {
                           Title = "Send an Email",
                           Category = TaskCategory.GetSample(),
                           isNotify= true,
                           Responsible = EmployeeWraper.GetSample(),
                           ContainerID = 12,
                           DeadLineIsFixed = false,
                           OffsetTicks = TimeSpan.FromDays(10).Ticks,
                           Description = ""
                       };
        }

    }
}
