using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Collections;

namespace ASC.Api.Batch
{
    [DataContract(Name = "batch_response", Namespace = "")]
    public class ApiBatchResponse
    {
        public ApiBatchResponse(ApiBatchRequest apiBatchRequest)
        {
            Name = apiBatchRequest.Name;
        }

        [DataMember(Order = 100)]
        public string Data { get; set; }

        [DataMember(Order = 10,EmitDefaultValue = false)]
        public ItemDictionary<string, string> Headers { get; set; }

        [DataMember(Order = 5)]
        public int Status { get; set; }

        [DataMember(Order = 1,EmitDefaultValue = false)]
        public string Name { get; set; }
    }
}