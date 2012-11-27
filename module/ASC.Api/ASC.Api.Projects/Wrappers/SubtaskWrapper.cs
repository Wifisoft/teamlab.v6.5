using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "subtask", Namespace = "")]
    public class SubtaskWrapper : ObjectWrapperFullBase
    {
        public SubtaskWrapper(Subtask subtask, Task task)
        {
            Id = subtask.ID;

            if (subtask.Responsible != Guid.Empty)
                Responsible = EmployeeWraper.Get(subtask.Responsible);

            Title = subtask.Title;
            Status = (int)subtask.Status;
            CanEdit = ProjectSecurity.CanEdit(task, subtask);

            CreatedBy = EmployeeWraper.Get(subtask.CreateBy);

            if (subtask.CreateBy != subtask.LastModifiedBy)
                UpdatedBy = EmployeeWraper.Get(subtask.LastModifiedBy);

            Updated = (ApiDateTime)subtask.LastModifiedOn;
            Created = (ApiDateTime)subtask.CreateOn;

        }

        private SubtaskWrapper()
        {}

        public static SubtaskWrapper GetSample()
        {
            return new SubtaskWrapper()
                       {
                           Id = 1233,
                           Responsible = Employee.EmployeeWraper.GetSample(),
                           Title = "Sample subtask",
                           Status = (int) TaskStatus.Open,
                           Created = (ApiDateTime)DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime)DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                       };
        }
        [DataMember]
        public bool CanEdit { get; set; }
    }
}