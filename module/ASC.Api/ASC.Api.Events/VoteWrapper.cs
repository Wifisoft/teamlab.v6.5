using System.Runtime.Serialization;

namespace ASC.Api.Events
{
    [DataContract(Name = "vote", Namespace = "")]
    public class VoteWrapper
    {
        [DataMember(Order = 1, EmitDefaultValue = true)]
        public long Id { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = true)]
        public string Name { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = true)]
        public int Votes { get; set; }

        public static VoteWrapper GetSample()
        {
            return new VoteWrapper()
                       {
                           Votes = 100,
                           Name = "Variant 1",
                           Id = 133
                       };
        }
    }
}