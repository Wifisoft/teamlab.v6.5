using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "post", Namespace = "")]
    public class ForumTopicPostWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Subject { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        private ApiDateTime _updated;

        [DataMember(Order = 3)]
        public ApiDateTime Updated
        {
            get { return _updated>=Created?_updated:Created; }
            set { _updated = value; }
        }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 100,EmitDefaultValue = false)]
        public List<ForumTopicPostAttachmentWrapper> Attachments { get; set; }

        public ForumTopicPostWrapper(Post post)
        {
            Id = post.ID;
            Text = post.Text;
            Created = (ApiDateTime) post.CreateDate;
            Updated = (ApiDateTime) post.EditDate;
            Subject = post.Subject;
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(post.PosterID));
            Attachments = post.Attachments.Select(x=>new ForumTopicPostAttachmentWrapper(x)).ToList();
        }

        private ForumTopicPostWrapper()
        {
        }

        public static ForumTopicPostWrapper GetSample()
        {
            return new ForumTopicPostWrapper()
                       {
                           Id = 123,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Created = (ApiDateTime) DateTime.Now,
                           Updated = (ApiDateTime) DateTime.Now,
                           Subject = "Sample subject",
                           Text = "Post text",
                           Attachments =
                               new List<ForumTopicPostAttachmentWrapper>() {ForumTopicPostAttachmentWrapper.GetSample()}

                       };
        }
    }
}