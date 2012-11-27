using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "thread", Namespace = "")]
    public class ForumThreadWrapperFull : ForumThreadWrapper
    {
       
        [DataMember(Order = 100)]
        public List<ForumTopicWrapper> Topics { get; set; }

        public ForumThreadWrapperFull(Thread thread, IEnumerable<Topic> topics):base(thread)
        {
            Topics = topics.Where(x=>x.IsApproved).Select(x => new ForumTopicWrapper(x)).ToList();
        }

        protected ForumThreadWrapperFull()
        {
        }

        public static new ForumThreadWrapperFull GetSample()
        {
            return new ForumThreadWrapperFull()
            {
                Created = (ApiDateTime)DateTime.Now,
                Updated = (ApiDateTime)DateTime.Now,
                Description = "Sample thread",
                Id = 10,
                UpdatedBy = EmployeeWraper.GetSample(),
                RecentTopicId = 1234,
                RecentTopicTitle = "Sample topic",
                Title = "The Thread",
                Topics = new List<ForumTopicWrapper>(){ForumTopicWrapper.GetSample()}
            };
        }
    }
}