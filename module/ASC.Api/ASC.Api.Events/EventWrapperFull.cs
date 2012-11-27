using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Web.Community.News.Code;

namespace ASC.Api.Events
{
    [DataContract(Name = "event", Namespace = "")]
    public class EventWrapperFull : EventWrapper
    {
        [DataMember(Order = 100)]
        public string Text { get; set; }



        [DataMember(Order = 200, EmitDefaultValue = false)]
        public PollWrapper Poll { get; set; }

        public EventWrapperFull(ASC.Web.Community.News.Code.Feed feed)
            : base(feed)
        {
            if (feed is FeedPoll)
            {
                //Add poll info
                var poll = feed as FeedPoll;
                Poll = new PollWrapper(poll);
            }
            Text = feed.Text;
        }

        private EventWrapperFull()
        {

        }

        public static new EventWrapperFull GetSample()
        {
            return new EventWrapperFull()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = 10,
                Type = FeedType.News,
                Title = "Sample news",
                Updated = (ApiDateTime)DateTime.Now,
                Text = "Text of feed",
                Poll = PollWrapper.GetSample()
            };
        }
    }
}