using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Blogs.Core;
using ASC.Core;
using ASC.Feed.Activity;
using ASC.Feed.Utils;

namespace ASC.Api.Blogs.ActivityProviders
{
    /// <summary>
    /// Blog activity provider
    /// </summary>
    public class BlogActivityProvider:ASC.Feed.ActivityProvider.IActivityProvider
    {
        private BlogsEngine _engine;
        private const string BlogsSource = "blogs";

        /// <summary>
        /// Name
        /// </summary>
        public string SourceName
        {
            get { return BlogsSource; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blogApi"></param>
        public BlogActivityProvider(BlogApi blogApi)
        {
            _engine = blogApi.BlogEngine;
        }

        /// <summary>
        /// Return recent blog activities
        /// </summary>
        /// <param name="range"></param>
        /// <param name="relativeTo"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var activities = new List<Activity>();
            //Get blog posts in range
            activities.AddRange(_engine.GetPosts(range.From, range.To).Select(x => new Activity(BlogsSource, new BlogPostWrapperSummary(x), x.Author.ID, x.Updated, (range.In(x.Updated) ? ActivityAction.Updated : ActivityAction.Undefined) | (range.In(x.Datetime) ? ActivityAction.Created : ActivityAction.Undefined)) { ItemType = "post" }));
            //Get post comments in range
            activities.AddRange(_engine.GetComments(range.From, range.To, SecurityContext.CurrentAccount.ID).Select(x => new Activity(BlogsSource, new BlogPostCommentWrapper(x), x.UserID, x.Datetime, ActivityAction.Commented | (x.Inactive ? ActivityAction.Undefined : ActivityAction.Deleted) | (x.ParentId==Guid.Empty ? ActivityAction.Undefined : ActivityAction.Reply)) { ItemType = "comment", IsNew = !x.IsReaded, RelativeTo = x.PostId}));

            return activities;
        }
    }
}