using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "participant_full", Namespace = "")]
    public class ParticipantFullWrapper
    {
        [DataMember(Order = 100)]
        public ProjectWrapper Project { get; set; }

        [DataMember(Order = 200)]
        public EmployeeWraper Participant{ get; set; }

        [DataMember(Order = 201)]
        public DateTime Created { get; set; }

        [DataMember(Order = 202)]
        public DateTime Updated { get; set; }

        [DataMember(Order = 203)]
        public bool Removed { get; set; }

        public ParticipantFullWrapper(ParticipantFull participant)
        {
            Project = new ProjectWrapper(participant.Project);
            Participant = EmployeeWraper.Get(participant.ID);
            Created = participant.Created;
            Updated = participant.Updated;
            Removed = participant.Removed;
        }
    }
}
