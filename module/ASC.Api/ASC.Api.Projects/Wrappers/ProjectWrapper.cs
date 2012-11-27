using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "project", Namespace = "")]
    public class ProjectWrapper : ObjectWrapperBase
    {


        public ProjectWrapper(Project project)
        {
            Responsible = EmployeeWraper.Get(project.Responsible);
            Id = project.ID;
            Title = project.Title;
            Description = project.Description;
            Status = (int) project.Status;
            CanEdit = ProjectSecurity.CanEdit(project);
            IsPrivate = project.Private;
        }

        private ProjectWrapper()
        {

        }

        public static ProjectWrapper GetSample()
        {
            return new ProjectWrapper()
            {

                Id = 10,
                Title = "Sample Title",
                Description = "Sample description",
                Responsible = EmployeeWraper.GetSample(),
                Status = (int)ProjectStatus.Open,
            };
        }

        [DataMember(Order = 31)]
        public bool CanEdit { get; set; }

        [DataMember(Order = 32)]
        public bool IsPrivate { get; set; }
        
    }
}