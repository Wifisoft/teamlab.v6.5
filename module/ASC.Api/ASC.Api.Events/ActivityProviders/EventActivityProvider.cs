using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Feed.ActivityProvider;
using ASC.Feed.Activity;
using ASC.Feed.Utils;
using ASC.Web.Community.News.Code.DAO;
using ASC.Core;

namespace ASC.Api.Events.ActivityProviders
{
    public class EventActivityProvider : IActivityProvider
    {
        private IFeedStorage _feedStorage;

        private IFeedStorage FeedStorage
        {
            get
            {
                if (_feedStorage == null)
                {
                    var dbId = FeedStorageFactory.Id;
                    if (!DbRegistry.IsDatabaseRegistered(dbId))
                    {
                        DbRegistry.RegisterDatabase(dbId, WebConfigurationManager.ConnectionStrings[dbId]);
                    }
                    _feedStorage = FeedStorageFactory.Create();
                }
                return _feedStorage;
            }
        }

        private const string Events = "events";

        public string SourceName
        {
            get { return Events; }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var events = FeedStorage.GetFeedByDate(range.From, range.To, SecurityContext.CurrentAccount.ID);
            var comments = FeedStorage.GetCommentsByDate(range.From, range.To);
            
            var activities = new List<Activity>((events
                .Select(x => new Activity(Events, new EventWrapper(x), new Guid(x.Creator), x.Date,
                    (range.In(x.Date) ? ActivityAction.Created : ActivityAction.Undefined))
                {ItemType = "event"})));
            activities.AddRange(comments
                .Select(x => new Activity(Events, new EventCommentWrapper(x), new Guid(x.Creator), x.Date,
                    (range.In(x.Date) ? ActivityAction.Created : ActivityAction.Undefined) | (x.ParentId == 0 ? ActivityAction.Commented : ActivityAction.Reply) | (x.Inactive ? ActivityAction.Deleted : ActivityAction.Undefined))
                {ItemType = "comment"}));

            return activities;
        }
    }
}
