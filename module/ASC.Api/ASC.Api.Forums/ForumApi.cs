using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Impl;
using ASC.Api.Utils;
using ASC.Core;
using ASC.Forum;

namespace ASC.Api.Forums
{
    //TODO: Add html decoding to some fields!!! 

    ///<summary>
    ///Forum access
    ///</summary>
    public class ForumApi : Interfaces.IApiEntryPoint
    {
        private readonly ApiContext _context;

        public string Name
        {
            get { return "forum"; }
        }

        public ForumApi(ApiContext context)
        {
            _context = context;
            ForumSettings.Configure("community");
        }

        private int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        private const int DefaultItemsPerPage = 100;

        private int CurrentPage
        {
            get { return (int)(_context.StartIndex / Count); }
        }

        private int Count
        {
            get { return (int) (_context.Count == 0 ? DefaultItemsPerPage : _context.Count); }
        }

		  ///<summary>
		  ///Returns the list of all forums created on the portal with the topic/thread titles, date of creation and update, post text and author id and display name
		  ///</summary>
		  ///<short>
		  ///Forum list
		  ///</short>
		  ///<returns>list of forums</returns>
        [Read("")]
        public ForumWrapper GetForums()
        {
            List<ThreadCategory> categories;
            List<Thread> threads;
            Forum.ForumDataProvider.GetThreadCategories(TenantId,false,out categories,out threads);
            return new ForumWrapper(categories,threads);
        }

		  ///<summary>
		  ///Returns the list of all thread topics in the forums on the portal with the thread title, date of creation and update, post text and author id and display name
		  ///</summary>
		  ///<short>
		  ///Thread topics
		  ///</short>
		  ///<param name="threadid">id of thread</param>
        ///<returns>list of topics in thread</returns>
        [Read("{threadid}")]
        public ForumThreadWrapperFull GetThreadTopics(int threadid)
        {
            var topicsIds = ForumDataProvider.GetTopicIDs(TenantId, threadid).Skip((int) _context.StartIndex);
            if (_context.Count>0)
            {
                topicsIds = topicsIds.Take((int) _context.Count);
            }
            _context.SetDataPaginated();
            return new ForumThreadWrapperFull(ForumDataProvider.GetThreadByID(TenantId,threadid).NotFoundIfNull(),ForumDataProvider.GetTopicsByIDs(TenantId, topicsIds.ToList(), true));
        }


		  ///<summary>
		  ///Returns the list of all recently updated topics in the forums on the portal with the topic title, date of creation and update, post text and author
		  ///</summary>
		  ///<short>
		  ///Last updated topics
		  ///</short>
		  ///<returns></returns>
        [Read("topic/recent")]
        public IEnumerable<ForumTopicWrapper> GetLastTopics()
        {
            var result = ForumDataProvider.GetLastUpdateTopics(TenantId, (int) _context.StartIndex, (int) _context.Count);
            _context.SetDataPaginated();
            return result.Select(x=>new ForumTopicWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all posts in a selected thread in the forums on the portal with the thread title, date of creation and update, post text and author id and display name
		  ///</summary>
		  ///<short>
		  ///Posts
		  ///</short>
		  ///<param name="topicid">id of topic</param>
        ///<returns>list of posts in topic</returns>
        [Read("topic/{topicid}")]
        public ForumTopicWrapperFull GetTopicPosts(int topicid)
        {
            //TODO: Deal with polls
            var postIds = ForumDataProvider.GetPostIDs(TenantId, topicid).Skip((int) _context.StartIndex);
            if (_context.Count>0)
            {
                postIds = postIds.Take((int) _context.Count);
            }
            _context.SetDataPaginated();
            return new ForumTopicWrapperFull(ForumDataProvider.GetTopicByID(TenantId, topicid).NotFoundIfNull(),
                                             ForumDataProvider.GetPostsByIDs(TenantId, postIds.ToList()));
        }


		  ///<summary>
		  /// Adds a new topic to an existing thread with a subject, content and topic type specified
		  ///</summary>
		  ///<short>
		  /// Add topic to thread
		  ///</short>
		  /// <param name="subject">Topic subject</param>
        /// <param name="threadid">Id of thread to add to</param>
        /// <param name="content">Topic text</param>
        /// <param name="topicType">Type of topic</param>
        ///<returns>Added topic</returns>
        [Create("{threadid}")]
        public ForumTopicWrapperFull AddTopic(int threadid, string subject, string content, TopicType topicType)
        {
            var id = ForumDataProvider.CreateTopic(TenantId, threadid, subject, topicType);
            ForumDataProvider.CreatePost(TenantId, id, 0, subject, content, true,
                                         PostTextFormatter.BBCode);
            return GetTopicPosts(id);
        }

		  ///<summary>
		  /// Updates a topic in an existing thread changing the thread subject, making it sticky or closing it
		  ///</summary>
		  ///<short>
		  /// Update topic in thread
		  ///</short>
		  /// <param name="topicid">id of topic to update</param>
        /// <param name="subject">subject</param>
        /// <param name="sticky">is sticky</param>
        /// <param name="closed">close topic</param>
        ///<returns>Updated topic</returns>
        [Update("topic/{topicid}")]
        public ForumTopicWrapperFull UpdateTopic(int topicid, string subject, bool sticky, bool closed)
        {
            ForumDataProvider.UpdateTopic(TenantId, topicid, subject, sticky, closed);
            return GetTopicPosts(topicid);
        }
		  ///<summary>
		  /// Adds a post to an existing topic with a post subject and content specified
		  ///</summary>
		  ///<short>
		  /// Add post to topic
		  ///</short>
		  ///<param name="topicid">id of topic</param>
        ///<param name="parentPostId">parent post id</param>
        ///<param name="subject">post subject (required)</param>
        ///<param name="content">post text</param>
        ///<returns>New post</returns>
        [Create("topic/{topicid}")]
        public ForumTopicPostWrapper AddTopicPosts(int topicid, int parentPostId, string subject, string content)
        {
            var id = ForumDataProvider.CreatePost(TenantId, topicid, parentPostId, subject, content, true,
                                         PostTextFormatter.BBCode);
            return new ForumTopicPostWrapper(ForumDataProvider.GetPostByID(TenantId,id));
        }

		  ///<summary>
		  /// Updates a post in an existing topic changing the post subject or/and content
		  ///</summary>
		  ///<short>
		  /// Update post in topic
		  ///</short>
		  ///<param name="topicid">id of topic</param>
        ///<param name="postid">id of post to update</param>
        ///<param name="subject">post subject (required)</param>
        ///<param name="content">post text</param>
        ///<returns>Updated post</returns>        
        [Update("topic/{topicid}/{postid}")]
        public ForumTopicPostWrapper UpdateTopicPosts(int topicid, int postid, string subject, string content)
        {
            ForumDataProvider.UpdatePost(TenantId, postid, subject, content, PostTextFormatter.BBCode);
            return new ForumTopicPostWrapper(ForumDataProvider.GetPostByID(TenantId, postid));
        }

		  ///<summary>
		  /// Deletes a selected post in an existing topic
		  ///</summary>
		  ///<short>
		  /// Delete post in topic
		  ///</short>
		  ///<param name="topicid">topic id</param>
        ///<param name="postid">post id</param>
        ///<returns></returns>
        [Delete("topic/{topicid}/{postid}")]
        public ForumTopicPostWrapper DeleteTopicPosts(int topicid, int postid)
        {
            ForumDataProvider.RemovePost(TenantId, postid);
            return null;
        }


		  ///<summary>
		  ///Returns a list of topics matching the search query with the topic title, date of creation and update, post text and author
		  ///</summary>
		  ///<short>
		  ///Search
		  ///</short>
		  ///<param name="query">search query</param>
        ///<returns>list of topics</returns>
        [Read("@search/{query}")]
        public IEnumerable<ForumTopicWrapper> SearchTopics(string query)
        {
            int count;
            var topics = ForumDataProvider.SearchTopicsByText(TenantId, query,0,-1, out count);
            return topics.Select(x => new ForumTopicWrapper(x)).ToSmartList();
        }
    }
}
