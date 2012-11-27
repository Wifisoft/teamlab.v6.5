using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Exceptions;
using ASC.Api.Impl;
using ASC.Api.Utils;
using ASC.Common.Data;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;

namespace ASC.Api.Events
{
    ///<summary>
    /// Events access 
    ///</summary>
    public class EventApi : Interfaces.IApiEntryPoint
    {
        private readonly ApiContext _context;

        public string Name
        {
            get { return "event"; }
        }

        private IFeedStorage _storage;
        private IFeedStorage FeedStorage
        {
            get
            {
                if (_storage == null)
                {
                    _storage = FeedStorageFactory.Create();
                }
                return _storage;
            }
        }

        public EventApi(ApiContext context)
        {
            _context = context;
        }

		  ///<summary>
		  ///Returns the list of all events on the portal with the event titles, date of creation and update, event text and author
		  ///</summary>
		  ///<short>
		  ///All events
		  ///</short>
		  ///<returns>list of events</returns>
        [Read("")]
        public IEnumerable<EventWrapper> GetEvents()
        {
            var feeds = FeedStorage.GetFeeds(FeedType.All, Guid.Empty, (int)_context.Count, (int)_context.StartIndex);
            _context.SetDataPaginated();
            return feeds.Select(x => new EventWrapper(x)).ToSmartList();
        }


		  ///<summary>
		  ///Create event
		  ///</summary>
		  ///<short>
		  ///Create event
		  ///</short>
		  ///<returns>list of events</returns>
        [Create("")]
        public EventWrapperFull CreateEvent(string content, string title, FeedType type)
        {
            var feed = new ASC.Web.Community.News.Code.Feed
                           {
                               Caption = title,
                               Text = content,
                               Creator = SecurityContext.CurrentAccount.ID.ToString(),
                               Date = DateTime.UtcNow
                           };
            FeedStorage.SaveFeed(feed, false, type);
            return new EventWrapperFull(feed);
        }


		  ///<summary>
		  ///Updates the selected event changing the event title, content or/and event type specified
		  ///</summary>
		  ///<short>
		  ///Update event
		  ///</short>
		  /// <param name="feedid"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        ///<returns>list of events</returns>
        [Update("{feedid}")]
        public EventWrapperFull UpdateEvent(int feedid, string content, string title, FeedType type)
        {
            var feed = FeedStorage.GetFeed(feedid).NotFoundIfNull();
            feed.Caption = title;
            feed.Text = content;
            feed.Creator = SecurityContext.CurrentAccount.ID.ToString();
            FeedStorage.SaveFeed(feed, true, type);
            return new EventWrapperFull(feed);
        }
		  ///<summary>
		  ///Returns the list of all events for the current user with the event titles, date of creation and update, event text and author
		  ///</summary>
		  ///<short>
		  ///My events
		  ///</short>
		  ///<returns>list of events</returns>
        [Read("@self")]
        public IEnumerable<EventWrapper> GetMyEvents()
        {
            var feeds = FeedStorage.GetFeeds(FeedType.All, SecurityContext.CurrentAccount.ID, (int)_context.Count, (int)_context.StartIndex);
            _context.SetDataPaginated();
            return feeds.Select(x => new EventWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns a list of events matching the search query with the event title, date of creation and update, event type and author
		  ///</summary>
		  ///<short>
		  ///Search
		  ///</short>
		  /// <param name="query">search query</param>
        ///<returns>list of events</returns>
        [Read("@search/{query}")]
        public IEnumerable<EventWrapper> SearchEvents(string query)
        {
            var feeds = FeedStorage.SearchFeeds(query, FeedType.All, Guid.Empty, (int)_context.Count, (int)_context.StartIndex);
            _context.SetDataPaginated();
            return feeds.Select(x => new EventWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the detailed information about the event with the specified ID
		  ///</summary>
		  ///<short>
		  ///Specific event
		  ///</short>
		  ///<param name="feedid">id of event</param>
        ///<returns>event</returns>
        [Read("{feedid}")]
        public EventWrapperFull GetEvent(int feedid)
        {
            var feed = FeedStorage.GetFeed(feedid).NotFoundIfNull();
            return new EventWrapperFull(feed);
        }

		  ///<summary>
		  ///Returns the detailed information about the comments on the event with the specified ID
		  ///</summary>
		  ///<short>
          ///Get comments
		  ///</short>
		  /// <category>Comments</category>
        ///<param name="feedid">event id</param>
        ///<returns>list of comments</returns>
        [Read("{feedid}/comment")]
        public IEnumerable<EventCommentWrapper> GetEventComments(int feedid)
        {
            FeedStorage.GetFeed(feedid).NotFoundIfNull();
            var feedComments = FeedStorage.GetFeedComments(feedid);
            return feedComments.Where(x => !x.Inactive).Select(x => new EventCommentWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Adds a comment to the event with the specified ID. The parent event ID can be also specified if needed.
		  ///</summary>
		  ///<short>
		  ///Add comment
		  ///</short>
		  /// <category>Comments</category>
        ///<param name="feedid">event id</param>
        ///<param name="content">comment content</param>
        ///<param name="parentId">comment parent id</param>
        ///<returns>list of comments</returns>
        /// <example>
        /// <![CDATA[
        /// Sending data in application/json:
        /// 
        /// {
        ///     content:"My comment",
        ///     parentId:"9924256A-739C-462b-AF15-E652A3B1B6EB"
        /// }
        /// 
        /// Sending data in application/x-www-form-urlencoded
        /// content="My comment"&parentId="9924256A-739C-462b-AF15-E652A3B1B6EB"
        /// ]]>
        /// </example>
        /// <remarks>
        /// Send parentId=00000000-0000-0000-0000-000000000000 or don't send it at all if you want your comment to be on the root level
        /// </remarks>
        [Create("{feedid}/comment")]
        public EventCommentWrapper AddEventComments(int feedid, string content, long parentId)
        {
            if (parentId > 0 && FeedStorage.GetFeedComment(parentId) == null) throw new ItemNotFoundException("parent comment not found");

            var comment = new FeedComment(feedid)
                              {
                                  Comment = content,
                                  Creator = SecurityContext.CurrentAccount.ID.ToString(),
                                  FeedId = feedid,
                                  Date = DateTime.UtcNow,
                                  ParentId = parentId
                              };
            FeedStorage.SaveFeedComment(FeedStorage.GetFeed(feedid), comment);
            return new EventCommentWrapper(comment);
        }

		  ///<summary>
		  /// Sends a vote to a certain option in a poll-type event with the ID specified
		  ///</summary>
		  ///<short>
		  /// Vote for event
		  ///</short>
		  /// <category>Voting</category>
        ///<param name="feedid">event id</param>
        ///<param name="variants">variants</param>
        ///<returns>event</returns>
        ///<exception cref="ArgumentException">thrown if not a Poll</exception>
        ///<exception cref="Exception">general error</exception>
        /// <example>
        /// <![CDATA[
        /// Sending data in application/json:
        /// {
        ///     variants:[1,2,3],
        /// }
        ///  Sending data in application/x-www-form-urlencoded
        /// variants=[1,2,3]
        /// ]]>
        /// </example>
        /// <remarks>
        /// If event is not a poll, then you'll get an error
        /// </remarks>
        [Create("{feedid}/vote")]
        public EventWrapperFull VoteForEvent(int feedid, long[] variants)
        {
            var feed = FeedStorage.GetFeed(feedid).NotFoundIfNull();

            if (feed.FeedType != FeedType.Poll) throw new ArgumentException("Feed is not a poll");
            if (((FeedPoll)feed).IsUserVote(SecurityContext.CurrentAccount.ID.ToString())) throw new ArgumentException("User already voted");

            //Voting
            string error;
            PollVoteHandler.VoteForPoll(variants.ToList(), FeedStorage, feedid, out error);//this method is from 
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return new EventWrapperFull(feed);
        }
    }
}
