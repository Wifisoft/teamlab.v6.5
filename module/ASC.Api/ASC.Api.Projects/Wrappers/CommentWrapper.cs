using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "comment", Namespace = "")]
    public class CommentWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 12)]
        public Guid ParentId { get; set; }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        public CommentWrapper(Comment comment)
        {
            ParentId = comment.Parent;
            Text=comment.Content;
            Id=comment.ID;
            Updated = Created=(ApiDateTime) comment.CreateOn;
            
            CreatedBy = EmployeeWraper.Get(comment.CreateBy);
        }

        private CommentWrapper()
        {

        }

        public static CommentWrapper GetSample()
        {
            return new CommentWrapper()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                Text = "comment text",
                Updated = (ApiDateTime)DateTime.Now
            };
        }
    }
}