using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "topic", Namespace = "")]
    public class ForumTopicWrapperFull:ForumTopicWrapper
    {
        [DataMember(Order = 100)]
        public List<ForumTopicPostWrapper> Posts { get; set; }


        public ForumTopicWrapperFull(Topic topic,IEnumerable<Post> posts) : base(topic)
        {
            if (topic.Type==TopicType.Poll)
            {
                //TODO: Deal with polls
            }
            Posts = posts.Where(x=>x.IsApproved).Select(x => new ForumTopicPostWrapper(x)).ToList();
        }

        private ForumTopicWrapperFull()
        {

        }

        public static new ForumTopicWrapperFull GetSample()
        {
            return new ForumTopicWrapperFull()
            {
                Created = (ApiDateTime)DateTime.Now,
                Updated = (ApiDateTime)DateTime.Now,
                Id = 10,
                UpdatedBy = EmployeeWraper.GetSample(),
                Text = "This is sample post",
                Status = TopicStatus.Sticky,
                Tags = new List<string>() { "Tag1", "Tag2" },
                Title = "Sample topic",
                Type = TopicType.Informational,
                Posts = new List<ForumTopicPostWrapper>() { ForumTopicPostWrapper.GetSample()}
            };
        }
    }
}