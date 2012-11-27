#region Usings

using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Resources;
using ASC.Specific;
using ASC.Web.Core.Users.Activity;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "activity", Namespace = "")]
    public class ProjectActivityWrapper
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string ProjectId { get; set; }

        [DataMember(Order = 3)]
        public string ProjectTitle { get; set; }

        [DataMember(Order = 4)]
        public string Title { get; set; }

        [DataMember(Order = 5)]
        public string Url { get; set; }

        [DataMember(Order = 6)]
        public string ActionText { get; set; }

        [DataMember(Order = 7)]
        public ApiDateTime Date { get; set; }

        [DataMember(Order = 8)]
        public EmployeeWraper User { get; set; }

        [DataMember(Order = 9)]
        public string EntityType { get; set; }

        [DataMember(Order = 10)]
        public string EntityTitle { get; set; }


        private ProjectActivityWrapper()
        {
        }

        public ProjectActivityWrapper(UserActivity activity)
        {
            Id = activity.ID;
            ProjectId = activity.ContainerID;
            Title = activity.Title;
            Url = activity.URL;
            ActionText = activity.ActionText;
            Date = (ApiDateTime)activity.Date;
            User = EmployeeWraper.Get(activity.UserID);

            if (string.IsNullOrEmpty(activity.AdditionalData)) return;
            
            var data = activity.AdditionalData.Split('|');
            if (data.Length <= 0) return;

            EntityType = data[0];

            ProjectTitle = data.Length == 3 ? data[2] : "";

            switch (EntityType)
            {
                case "Project":
                    EntityTitle = ProjectsEnumResource.EntityType_Project;
                    break;
                case "Milestone":
                    EntityTitle = ProjectsEnumResource.EntityType_Milestone;
                    break;
                case "Message":
                    EntityTitle = ProjectsEnumResource.EntityType_Message;
                    break;
                case "Team":
                    EntityTitle = ProjectsEnumResource.EntityType_Team;
                    break;
                case "Task":
                    EntityTitle = ProjectsEnumResource.EntityType_Task;
                    break;
                case "SubTask":
                    EntityTitle = ProjectsEnumResource.EntityType_SubTask;
                    break;
                case "TimeSpend":
                    EntityTitle = ProjectsEnumResource.EntityType_TimeSpend;
                    break;  
                case "Comment":
                    EntityTitle = ProjectsEnumResource.EntityType_Comment;
                    break;
            }
        }


        public static ProjectActivityWrapper GetSample()
        {
            return new ProjectActivityWrapper
            {
                Id = 10,
                ProjectId = "123",
                ProjectTitle = "Sample Project Title",
                Title = "Sample Title",
                Url = "/asc/products/projects/projects.aspx?prjID=123",
                ActionText = "Created",
                Date = (ApiDateTime)DateTime.Now,
                User = EmployeeWraper.GetSample(),
                EntityType = "Project",
                EntityTitle = "Project"
            };
        }
    }
}
