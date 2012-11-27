using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "task", Namespace = "")]
    public class TaskWrapper : ObjectWrapperFullBase
    {
        public TaskWrapper(Task task)
        {
            if (task.SubTasks != null)
                Subtasks = task.SubTasks.Select(x => new SubtaskWrapper(x, task)).ToList();

            CreatedBy = EmployeeWraper.Get(task.CreateBy);
            if (task.CreateBy != task.LastModifiedBy)
                UpdatedBy = EmployeeWraper.Get(task.LastModifiedBy);

            if (task.Responsible!=Guid.Empty)
                Responsible = EmployeeWraper.Get(task.Responsible);

            Id = task.ID;

            Updated = (ApiDateTime)task.LastModifiedOn;
            Created = (ApiDateTime)task.CreateOn;
            Deadline = (task.Deadline == DateTime.MinValue ? null : (ApiDateTime)task.Deadline);
            Priority = task.Priority;
            
            Title = task.Title;
            Status = (int)task.Status;
            Description = task.Description;
            MilestoneId = task.Milestone;
            ProjectOwner = new SimpleProjectWrapper(task.Project);
            CanEdit = ProjectSecurity.CanEdit(task);
            CanWork = ProjectSecurity.CanWork(task);

            if (task.Milestone != 0 && task.MilestoneDesc != null)
                Milestone = new SimpleMilestoneWrapper(task.MilestoneDesc);

            if (task.Responsibles != null)
                Responsibles = task.Responsibles.Select(x => EmployeeWraper.Get(x)).ToList();
        }

        public TaskWrapper(Task task, Milestone milestone)
            : this(task)
        {
            if (task.Milestone != 0)
                Milestone = new SimpleMilestoneWrapper(milestone);
        }

        private TaskWrapper()
        {

        }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public int CanWork { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public ApiDateTime Deadline { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public int MilestoneId { get; set; }

        [DataMember(Order = 12)]
        public TaskPriority Priority { get; set; }

        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }
        
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<SubtaskWrapper> Subtasks { get; set; }

        [DataMember(Order = 53)]
        public List<EmployeeWraper> Responsibles { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public SimpleMilestoneWrapper Milestone { get; set; }

        public static TaskWrapper GetSample()
        {
            return new TaskWrapper()
            {
                Created = (ApiDateTime)DateTime.Now,
                CreatedBy = EmployeeWraper.GetSample(),
                Id = 10,
                Title = "Sample Title",
                Updated = (ApiDateTime)DateTime.Now,
                UpdatedBy = EmployeeWraper.GetSample(),
                Description = "Sample description",
                Responsible = EmployeeWraper.GetSample(),
                Status = (int)MilestoneStatus.Open,
                Deadline = (ApiDateTime)DateTime.Now,
                MilestoneId = 123,
                Priority = TaskPriority.High,
                ProjectOwner = SimpleProjectWrapper.GetSample()
            };
        }
    }
}