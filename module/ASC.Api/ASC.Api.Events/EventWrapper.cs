﻿using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Web.Community.News.Code;

namespace ASC.Api.Events
{
    [DataContract(Name = "event", Namespace = "")]
    public class EventWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated
        { get; set; }

        [DataMember(Order = 4)]
        public FeedType Type { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        public EventWrapper(ASC.Web.Community.News.Code.Feed feed)
        {
            Id=feed.Id;
            Title=feed.Caption;
            Updated = Created=(ApiDateTime) feed.Date;
            Type =feed.FeedType;
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(new Guid(feed.Creator)));
        }

        protected EventWrapper()
        {

        }

        public static EventWrapper GetSample()
        {
            return new EventWrapper()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = 10,
                Type = FeedType.News,
                Title = "Sample news",
                Updated = (ApiDateTime)DateTime.Now
            };
        }
        
    }
}