using System;
using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "milestone", Namespace = "")]
    public class SimpleMilestoneWrapper
    {

        [DataMember(Order = 60)]
        public int Id { get; set; }

        [DataMember(Order = 61)]
        public string Title { get; set; }

        [DataMember(Order = 62)]
        public ApiDateTime Deadline { get; set; }

        ///<summary>
        ///</summary>
        public SimpleMilestoneWrapper()
        {

        }

        ///<summary>
        ///</summary>
        public SimpleMilestoneWrapper(Milestone milestone)
        {
            Id = milestone.ID;
            Title = milestone.Title;
            Deadline = (ApiDateTime)milestone.DeadLine;
        }

        public static SimpleMilestoneWrapper GetSample()
        {
            return new SimpleMilestoneWrapper
                       {                           
                           Id = 123,
                           Title = "Milestone",
                           Deadline = (ApiDateTime)DateTime.Now,
                       };
        }
    }
}
