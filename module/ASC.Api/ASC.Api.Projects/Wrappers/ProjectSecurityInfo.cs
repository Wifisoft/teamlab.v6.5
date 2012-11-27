using System;
using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Api.Projects.Wrappers
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "project_security", Namespace = "")]
    public class ProjectSecurityInfo
    {
        private ProjectSecurityInfo()
        {
            
        }

        public ProjectSecurityInfo(Project project)
        {
            CanCreateMessage = ProjectSecurity.CanCreateMessage(project);
            CanCreateMilestone = ProjectSecurity.CanCreateMilestone(project);
            CanCreateTask = ProjectSecurity.CanCreateTask(project);
            CanEditTeam = ProjectSecurity.CanEditTeam(project);
            CanReadFiles = ProjectSecurity.CanReadFiles(project);
            CanReadMilestones = ProjectSecurity.CanReadMilestones(project);
            CanReadMessages = ProjectSecurity.CanReadMessages(project);
            CanReadTasks = ProjectSecurity.CanReadTasks(project);
        }


        [DataMember]
        public bool CanCreateMessage { get; set; }

        [DataMember]
        public bool CanCreateMilestone { get; set; }

        [DataMember]
        public bool CanCreateTask { get; set; }

        [DataMember]
        public bool CanEditTeam { get; set; }

        [DataMember]
        public bool CanReadFiles { get; set; }

        [DataMember]
        public bool CanReadMilestones { get; set; }

        [DataMember]
        public bool CanReadMessages { get; set; }

        [DataMember]
        public bool CanReadTasks { get; set; }


        public static ProjectSecurityInfo GetSample()
        {
            return new ProjectSecurityInfo();
        }
    }
}