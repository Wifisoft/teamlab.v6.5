using System;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;

namespace ASC.Api.Comments
{
    [DataContract(Name = "comment", Namespace = "")]
    public class Comment : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 12)]
        public long? ParentId { get; set; }

        [DataMember(Order = 10)]
        public string Content { get; set; }

        [DataMember(Order = 11)]
        public bool Deleted { get; set; }

        [DataMember(Order = 12)]
        public DateTime? Readed { get; set; }

        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper Author { get; set; }

        public string Key { get; set; }

        public string SecurityId { get; set; }

        public Comment()
        {

        }

        public static Comment GetSample()
        {
            return new Comment()
            {
                Author = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = 1,
                ParentId = 0,
                Content = "comment text",
                Updated = (ApiDateTime)DateTime.Now
            };
        }
    }
}