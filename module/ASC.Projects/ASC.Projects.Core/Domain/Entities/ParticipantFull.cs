using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Projects.Core.Domain
{
    public class ParticipantFull : Participant
    {
        public ParticipantFull(Guid userId) : base(userId)
        {
            
        }

        public Project Project { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Removed { get; set; }
    }
}
