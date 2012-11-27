using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Bookmarking.Common.Util;
using ASC.Bookmarking.Dao;
using ASC.Bookmarking.Pojo;
using ASC.Feed.Utils;
using ASC.Feed.Activity;
using ASC.Feed.ActivityProvider;
using ASC.Bookmarking.Business;
using ASC.Common.Data;
using ASC.Bookmarking.Common;
using System.Web.Configuration;

namespace ASC.Api.Bookmarks.ActivityProviders
{
    public class BookmarksActivityProvider : IActivityProvider
    {
        private BookmarkingService _bookmarkingService;

        private BookmarkingService Service
        {
            get
            {
                if (_bookmarkingService == null)
                {
                    const string dbId = BookmarkingBusinessConstants.BookmarkingDbID;
                    if (!DbRegistry.IsDatabaseRegistered(dbId))
                    {
                        DbRegistry.RegisterDatabase(dbId, WebConfigurationManager.ConnectionStrings[dbId]);
                    }
                    _bookmarkingService = BookmarkingService.GetCurrentInstanse();
                }
                return _bookmarkingService;
            }
        }

        public const string Bookmarks = "bookmarks";

        public string SourceName
        {
            get { return Bookmarks; }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var bookmarks = Service.GetBookmarks(range.From, range.To);
            var comments = Service.GetComments(range.From,range.To);

            var activities = new List<Activity>((bookmarks
                .Select(x => new Activity(Bookmarks, new BookmarkWrapper(x), x.UserCreatorID, x.Date,
                    (range.In(x.Date) ? ActivityAction.Created : ActivityAction.Undefined))
                {ItemType = "bookmark"})));
            activities.AddRange(comments
                .Select(x => new Activity(Bookmarks, new BookmarkCommentWrapper(x), x.UserID, x.Datetime,
                    (range.In(x.Datetime) ? ActivityAction.Created : ActivityAction.Undefined) | (x.Parent == null ? ActivityAction.Commented : ActivityAction.Reply) | (x.Inactive ? ActivityAction.Deleted : ActivityAction.Undefined))
                {ItemType = "comment"}));

            return activities;
        }
    }
}
