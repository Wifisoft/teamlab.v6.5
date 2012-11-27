using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "milestone", Namespace = "")]
    public class MilestoneWrapper : ObjectWrapperFullBase
    {
        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 20)]
        public bool IsNotify { get; set; }

        [DataMember(Order = 20)]
        public bool IsKey { get; set; }

        [DataMember(Order = 20)]
        public ApiDateTime Deadline { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember(Order = 20)]
        public int ActiveTaskCount { get; set; }

        [DataMember(Order = 20)]
        public int ClosedTaskCount { get; set; }

        public MilestoneWrapper(Milestone milestone)
        {
            Created = (ApiDateTime)milestone.CreateOn;
            CreatedBy = EmployeeWraper.Get(milestone.CreateBy);
            Updated = (ApiDateTime)milestone.LastModifiedOn;
            if (milestone.CreateBy != milestone.LastModifiedBy)
                UpdatedBy = EmployeeWraper.Get(milestone.LastModifiedBy);

            Description = milestone.Description;
            Id = milestone.ID;
            Title = milestone.Title;
            ProjectOwner = new SimpleProjectWrapper(milestone.Project);
            Status = (int)milestone.Status;
            IsNotify = milestone.IsNotify;
            IsKey = milestone.IsKey;
            Deadline = (ApiDateTime)milestone.DeadLine;
            CanEdit = ProjectSecurity.CanEdit(milestone);
            ActiveTaskCount = milestone.ActiveTaskCount;
            ClosedTaskCount = milestone.ClosedTaskCount;

            if (!milestone.Responsible.Equals(Guid.Empty))
                Responsible = EmployeeWraper.Get(milestone.Responsible);
        }

        private MilestoneWrapper()
        {

        }

        public static MilestoneWrapper GetSample()
        {
            return new MilestoneWrapper()
            {
                Created = (ApiDateTime)DateTime.Now,
                CreatedBy = EmployeeWraper.GetSample(),
                Id = 10,
                Title = "Sample Title",
                Updated = (ApiDateTime)DateTime.Now,
                UpdatedBy = EmployeeWraper.GetSample(),
                ProjectOwner = SimpleProjectWrapper.GetSample(),
                Deadline = (ApiDateTime)DateTime.Now,
                Description = "Sample description",
                IsKey = false,
                IsNotify = false,
                Responsible = EmployeeWraper.GetSample(),
                Status = (int) MilestoneStatus.Open,
                ActiveTaskCount = 15,
                ClosedTaskCount = 5,
                CanEdit = true
            };
        }
    }
}