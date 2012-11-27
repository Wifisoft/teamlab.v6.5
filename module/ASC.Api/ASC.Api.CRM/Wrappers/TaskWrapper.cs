#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Core.Tenants;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///  Задача
    /// </summary>
    [DataContract(Name = "task", Namespace = "")]
    public class TaskWrapper : ObjectWrapperBase
    {

        public TaskWrapper(int id): base(id)
        {
            
        }

        public TaskWrapper(Task task): base(task.ID)
        {
            CreateBy = EmployeeWraper.Get(task.CreateBy);
            Created = (ApiDateTime)task.CreateOn;
            Title = task.Title;
            Description = task.Description;
            DeadLine = (ApiDateTime)task.DeadLine;
            Responsible = EmployeeWraper.Get(task.ResponsibleID);
            IsClosed = task.IsClosed;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Contact { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime DeadLine { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskCategory Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EntityWrapper Entity { get; set; }
        
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        public static TaskWrapper GetSample()
        {
            return new TaskWrapper(0)
                       {
                           Created = (ApiDateTime)DateTime.UtcNow,
                           CreateBy = EmployeeWraper.GetSample(),
                           DeadLine = (ApiDateTime)DateTime.UtcNow.AddMonths(1),
                           IsClosed = false,
                           Responsible = EmployeeWraper.GetSample(),
                           CanEdit = true,
                           Title = "Отправить коммерческое предложение"
                       };
        }
    }
}
