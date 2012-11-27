using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "project", Namespace = "")]
    public class ProjectWrapperFull : ObjectWrapperFullBase,IApiSortableDate
    {
        public ProjectWrapperFull(Project project) : this(project, 0)
        {
        }

        public ProjectWrapperFull(Project project, object filesRoot)
        {
            CreatedBy = EmployeeWraper.Get(project.CreateBy);
            if (project.CreateBy!=project.LastModifiedBy)
                UpdatedBy = EmployeeWraper.Get(project.LastModifiedBy);
            Responsible = EmployeeWraper.Get(project.Responsible);
            Id = project.ID;
            Created = (ApiDateTime) project.CreateOn;
            Updated = (ApiDateTime) project.LastModifiedOn;
            Title = project.Title;
            Description = project.Description;
            Status = (int) project.Status;
            Security = new ProjectSecurityInfo(project);
            CanEdit = ProjectSecurity.CanEdit(project);
            ProjectFolder = filesRoot;
            IsPrivate = project.Private;
            TaskCount = project.TaskCount;
            MilestoneCount = project.MilestoneCount;
            ParticipantCount = project.ParticipantCount;
        }

        private ProjectWrapperFull()
        {

        }

        public static ProjectWrapperFull GetSample()
        {
            return new ProjectWrapperFull()
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
                Security = ProjectSecurityInfo.GetSample(),
                ProjectFolder = 13234
            };
        }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public ProjectSecurityInfo Security { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object ProjectFolder { get; set; }

        [DataMember(Order = 32)]
        public bool IsPrivate { get; set; }

        [DataMember(Order = 33)]
        public int TaskCount { get; set; }

        [DataMember(Order = 34)]
        public int MilestoneCount { get; set; }

        [DataMember(Order = 35)]
        public int ParticipantCount { get; set; }
    }
}