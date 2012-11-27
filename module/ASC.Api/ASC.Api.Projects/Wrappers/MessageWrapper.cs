using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "message", Namespace = "")]
    public class MessageWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 9)]
        public string Title { get; set; }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 51)]
        public EmployeeWraper CreatedBy { get; set; }

        private ApiDateTime _updated;

        [DataMember(Order = 50)]
        public ApiDateTime Updated
        {
            get { return _updated >= Created ? _updated : Created; }
            set { _updated = value; }
        }


        [DataMember(Order = 41)]
        public EmployeeWraper UpdatedBy { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember(Order = 15)]
        public int CommentsCount { get; set; }

        public MessageWrapper(Message message)
        {
            Created = (ApiDateTime)message.CreateOn;
            CreatedBy = EmployeeWraper.Get(message.CreateBy);
            Updated = (ApiDateTime)message.LastModifiedOn;
            if (message.CreateBy != message.LastModifiedBy)
                UpdatedBy = EmployeeWraper.Get(message.LastModifiedBy);
            Id = message.ID;
            Text = message.Content;
            Title = message.Title;
            if (message.Project != null)
            {
                ProjectOwner = new SimpleProjectWrapper(message.Project);
            }
            CanEdit = ProjectSecurity.CanEdit(message);
            CommentsCount = message.CommentsCount;
        }

        private MessageWrapper()
        {

        }

        public static MessageWrapper GetSample()
        {
            return new MessageWrapper()
                       {
                           Created = (ApiDateTime)DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Id = 10,
                           Title = "Sample Title",
                           Updated = (ApiDateTime)DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           ProjectOwner = SimpleProjectWrapper.GetSample(),
                           Text = "Hello, this is sample message",
                           CommentsCount = 5
                       };
        }
    }
}