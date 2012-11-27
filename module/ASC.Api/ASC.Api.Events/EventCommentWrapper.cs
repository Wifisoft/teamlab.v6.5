using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Web.Community.News.Code;

namespace ASC.Api.Events
{
    [DataContract(Name = "comment", Namespace = "")]
    public class EventCommentWrapper : IApiSortableDate
    {
        public EventCommentWrapper(FeedComment comment)
        {
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(new Guid(comment.Creator)));
            Updated = Created = (ApiDateTime)comment.Date;
           
            Id = comment.Id;
            Text = comment.Comment;
            ParentId = comment.ParentId;
        }

        private EventCommentWrapper()
        {
        }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Updated
        {
            get;
            set;
        }

        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public long ParentId { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        public static EventCommentWrapper GetSample()
        {
            return new EventCommentWrapper()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = 10,
                ParentId = 123,
                Text = "comment text",
                Updated = (ApiDateTime)DateTime.Now
            };
        }
    }
}