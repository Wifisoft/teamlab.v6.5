#region Usings

using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "participant", Namespace = "")]
    public class ParticipantWrapper : EmployeeWraperFull
    {
        [DataMember]
        public bool CanReadFiles { get; set; }

        [DataMember]
        public bool CanReadMilestones { get; set; }

        [DataMember]
        public bool CanReadMessages { get; set; }

        [DataMember]
        public bool CanReadTasks { get; set; }

        [DataMember]
        public bool IsAdministrator { get; set; }

        public ParticipantWrapper(Participant participant) : base(participant.UserInfo)
        {  
            CanReadFiles = participant.CanReadFiles;
            CanReadMilestones = participant.CanReadMilestones;
            CanReadMessages = participant.CanReadMessages;
            CanReadTasks = participant.CanReadTasks;

            IsAdministrator = ProjectSecurity.IsAdministrator(participant.ID);
        }
    }
}
