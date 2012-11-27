using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Api.Employee;
using ASC.Api.Impl;
using ASC.Core;
using ASC.Feed.Activity;
using ASC.Feed.ActivityProvider;
using ASC.Feed.Utils;
using ASC.Forum;

namespace ASC.Api.Forums.ActivityProviders
{
    public class ForumActivityProvider : IActivityProvider
    {
        public ForumActivityProvider()
        {
            ForumSettings.Configure("community");
        }

        private const string Forums = "forums";

        public string SourceName
        {
            get { return Forums; }
        }

        private int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var activities = new List<Activity>(ForumDataProvider.GetTopics(range.From, range.To, TenantId)
                .Select(x =>new Activity(Forums, new ForumTopicWrapper(x), x.PosterID, x.CreateDate,
                    (range.In(x.CreateDate) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.RecentPostCreateDate) ? ActivityAction.Updated : ActivityAction.Undefined))
                    {ItemType = x.Type == TopicType.Informational ? "topic" : "poll"}));
            activities.AddRange(ForumDataProvider.GetPosts(range.From, range.To, TenantId)
                .Select(x =>new Activity(Forums, new ForumTopicPostWrapper(x), x.PosterID, x.CreateDate,
                    (range.In(x.CreateDate) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.EditDate) ? ActivityAction.Updated : ActivityAction.Undefined) | (x.ParentPostID == 0 ? ActivityAction.Reply : ActivityAction.Commented))
                    {ItemType = "post"}));

            return activities;
        }
    }
}
