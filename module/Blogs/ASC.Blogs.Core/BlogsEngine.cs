using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Blogs.Core.Data;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Publisher;
using ASC.Blogs.Core.Security;
using ASC.Blogs.Core.Service;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Notify;
using ASC.Notify;
using ASC.FullTextIndex;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Security.Cryptography;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Community.Product;

namespace ASC.Blogs.Core
{
    public class BlogsEngine
    {
        #region fabric

        public static BlogsEngine GetEngine(int tenant)
        {
            if (HttpContext.Current != null)
            {
                var bs = DisposableHttpContext.Current["blogs"] as BlogsStorage;
                if (bs == null)
                {
                    bs = BlogsStorageFactory.GetStorage(tenant);
                    DisposableHttpContext.Current["blogs"] = bs;
                }
                return new BlogsEngine(tenant, bs);
            }
            else
                return new BlogsEngine(tenant, BlogsStorageFactory.GetStorage(tenant));

        }
        #endregion

        readonly BlogsStorage _storage = null;
        static INotifyClient _notifyClient = null;
        static BlogNotifySource _notifySource = null;
        readonly int _tenant;

        public BlogsEngine(int Tenant, BlogsStorage storage)
        {
            _storage = storage;
            _tenant = Tenant;

            InitNotify();
        }

        #region database

        #region blogs
        public Blog EnsurePersonalBlog(Guid userId)
        {
            Blog blog = null;
            var list = _storage.GetBlogDao()
                            .Select(null, userId, null);
            if (list.Count == 0)
            {
                blog = new Blog();
                blog.UserID = userId;

                _storage.GetBlogDao().SaveBlog(blog);
            }
            else
                blog = list[0];

            return blog;
        }

        #endregion

        #region posts

        List<Guid> SearchPostsInternal(string searchText)
        {
            if (FullTextSearch.SupportModule(FullTextSearch.BlogsModule))
            {
                return FullTextSearch
                    .Search(searchText, FullTextSearch.BlogsModule)
                    .GetIdentifiers()
                    .Select(id => new Guid(id))
                    .ToList();
            }
            else
            {
                var keyWords = new List<string>(searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                keyWords.RemoveAll(kw => kw.Length < 3);

                Dictionary<string, int> wordResults = new Dictionary<string, int>(keyWords.Count);
                Dictionary<Guid, int> counts = new Dictionary<Guid, int>();
                int wordsCount = 0;
                foreach (var word in keyWords)
                {
                    if (wordResults.ContainsKey(word)) continue;

                    wordsCount++;
                    var wordResult = _storage.GetPostDao().SearchPostsByWord(word);
                    wordResults.Add(word, wordResult.Count);

                    wordResult.ForEach(
                        pid =>
                        {
                            if (counts.ContainsKey(pid))
                                counts[pid] = counts[pid] + 1;
                            else
                                counts.Add(pid, 1);
                        });
                }

                List<Guid> searchResultIds = new List<Guid>();
                foreach (var kw in counts)
                    if (kw.Value == wordsCount)
                        searchResultIds.Add(kw.Key);

                return searchResultIds;
            }
        }

        public int SearchPostsCount(string searchText)
        {
            if (String.IsNullOrEmpty(searchText))
                throw new ArgumentNullException("searchText");

            var ids = SearchPostsInternal(searchText);
            var forSelectPosts = _storage.GetPostDao().GetPosts(ids, false, false);

            return forSelectPosts.Count;
        }
        public List<Post> SearchPosts(string searchText)
        {
            return SearchPosts(searchText, new PagingQuery());
        }
        public List<Post> SearchPosts(string searchText, PagingQuery paging)
        {
            if (String.IsNullOrEmpty(searchText)) throw new ArgumentNullException("searchText");

            var ids = SearchPostsInternal(searchText);

            var forSelectPosts = _storage.GetPostDao().GetPosts(ids, false, false);
            forSelectPosts.Sort((p1, p2) => DateTime.Compare(p2.Datetime, p1.Datetime));

            var offset = paging.Offset.GetValueOrDefault(0);
            var count = paging.Count.GetValueOrDefault(forSelectPosts.Count);
            if (offset > forSelectPosts.Count) return new List<Post>();
            if (count > forSelectPosts.Count - offset) count = forSelectPosts.Count - offset;

            var result = _storage.GetPostDao().GetPosts(forSelectPosts.GetRange(offset, count).ConvertAll(p => p.ID), true, true);

            result.Sort((p1, p2) => DateTime.Compare(p2.Datetime, p1.Datetime));

            return result;
        }

        public int GetPostsCount(PostsQuery query)
        {
            if (query.SearchText != null)
                return SearchPostsCount(query.SearchText);
            return
                _storage.GetPostDao()
                    .GetCount(
                        null,
                        null,
                        query.UserId,
                        query.Tag);
        }
        public List<Post> SelectPosts(PostsQuery query)
        {
            if (query.SearchText != null)
                return SearchPosts(query.SearchText, new PagingQuery(query));
            else
                return
                    _storage.GetPostDao()
                        .Select(
                            null,
                            null,
                            query.UserId,
                            query.Tag,
                            query.WithContent,
                            false,
                            query.Offset,
                            query.Count,
                            query.WithTags,
                            false);

        }
        public List<Post> SelectPostsInfo(List<Guid> ids)
        {
            return _storage.GetPostDao().GetPosts(ids, false, false);
        }
        public List<Typle<Comment, Post>> SelectLastCommentedPosts(int count)
        {
            return _storage.GetPostDao().GetLastCommentedPosts(count);
        }

        public IEnumerable<Post> GetPosts(DateTime from, DateTime to)
        {
            return _storage.GetPostDao().GetPostsByDate(from, to);
        }

        public IEnumerable<Comment> GetComments(DateTime from, DateTime to,Guid userId)
        {
            return _storage.GetPostDao().GetCommentsByDate(from, to,userId);
        }

        public Post GetPostById(Guid postId)
        {
            return _storage
                    .GetPostDao()
                    .Get(postId, true, false);
        }
        public List<Typle<Post, int, int>> GetPostsCommentsCountAndNew(List<Post> posts, Guid userId)
        {
            List<Typle<Post, int, int>> res = new List<Typle<Post, int, int>>(posts.Count);
            if (posts.Count == 0)
                return res;

            var postIds = posts.ConvertAll(p => p.ID);
            var dic = _storage.GetPostDao()
                            .GetCommentsStatistic(postIds, userId);

            foreach (var post in posts)
            {
                if (dic.ContainsKey(post.ID))
                    res.Add(new Typle<Post, int, int>()
                    {
                        Value1 = post,
                        Value2 = dic[post.ID].Value1,
                        Value3 = dic[post.ID].Value2
                    });
                else
                    res.Add(new Typle<Post, int, int>() { Value1 = post, Value2 = 0, Value3 = 0 });

            }
            return res;
        }
        public List<Typle<Post, int>> GetPostsCommentsCount(List<Post> posts)
        {
            var result = new List<Typle<Post, int>>();
            if (posts.Count == 0)
                return result;

            var postIds = posts.ConvertAll(p => p.ID);
            var counts = _storage.GetPostDao().GetCommentsCount(postIds);


            for (int i = 0; i < counts.Count; i++)
            {

                result.Add(new Typle<Post, int>
                {
                    Value1 = posts[i],
                    Value2 = counts[i]
                });
            }

            return result;
        }
        public int GetPostCountByAuthor(Guid authorId)
        {
            var list = _storage.GetPostDao().GetPostsCountByAuthor(authorId);
            if (list.Count == 0) return 0;
            return list[0].Value2;
        }
        public List<Typle<Guid, int, int, int>> GetAuthorsStatistic()
        {
            var postsCount = _storage.GetPostDao().GetPostsCountByAuthor(null);
            var postsCommentsCount = _storage.GetPostDao().GetCommentsCountByAuthorPosts(null);
            var postsReviewCount = _storage.GetPostDao().GetReviewCountByAuthorPosts(null);

            List<Typle<Guid, int, int, int>> result = new List<Typle<Guid, int, int, int>>(postsCount.Count);
            Typle<Guid, int> empty = new Typle<Guid, int>();
            foreach (var author in postsCount)
            {
                result.Add(
                        new Typle<Guid, int, int, int>
                        {
                            Value1 = author.Value1,
                            Value2 = author.Value2,
                            Value3 = (postsCommentsCount.Find(t => t.Value1 == author.Value1) ?? empty).Value2,
                            Value4 = (postsReviewCount.Find(t => t.Value1 == author.Value1) ?? empty).Value2
                        }
                    );
            }

            return result;
        }

        public void SavePostReview(Post post, Guid userID)
        {
            _storage.GetPostDao()
                .SavePostReview(userID, post.ID, ASC.Core.Tenants.TenantUtil.DateTimeNow());
        }
        public void SavePost(Post post, bool isNew, bool notifyComments)
        {
            if (isNew)
            {
                CommunitySecurity.DemandPermissions(
                    new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(post.UserID)),
                    ASC.Blogs.Core.Constants.Action_AddPost);
            }
            else
            {
                CommunitySecurity.DemandPermissions(
                    new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(post.UserID)),
                    Constants.Action_EditRemovePost);
            }

            _storage.GetPostDao().SavePost(post);
            if (isNew)
            {
                var initiatorInterceptor = new InitiatorInterceptor(new DirectRecipient(post.UserID.ToString(), ""));
                try
                {
                    NotifyClient.BeginSingleRecipientEvent("asc_blog");
                    NotifyClient.AddInterceptor(initiatorInterceptor);

                    List<ITagValue> tags = new List<ITagValue>
                                               {
                                                   new TagValue(ASC.Blogs.Core.Constants.TagPostSubject, post.Title),
                                                   new TagValue(ASC.Blogs.Core.Constants.TagPostPreview,
                                                                post.GetPreviewText(500)),
                                                   new TagValue(ASC.Blogs.Core.Constants.TagUserName,
                                                                DisplayUserSettings.GetFullUserName(post.UserID)),
                                                   new TagValue(ASC.Blogs.Core.Constants.TagUserURL,
                                                                CommonLinkUtility.GetFullAbsolutePath(
                                                                    CommonLinkUtility.GetUserProfile(post.UserID,
                                                                                                     CommonLinkUtility.GetProductID()))),
                                                   new TagValue(ASC.Blogs.Core.Constants.TagDate,
                                                                string.Format("{0:d} {0:t}", post.Datetime)),
                                                   new TagValue(ASC.Blogs.Core.Constants.TagURL,
                                                                CommonLinkUtility.GetFullAbsolutePath(
                                                                    ASC.Blogs.Core.Constants.ViewBlogPageUrl +
                                                                    "?blogid=" + post.ID.ToString())),
                                                  GetReplyToTag(Guid.Empty, post)

                                               };

                    NotifyClient.SendNoticeAsync(
                        ASC.Blogs.Core.Constants.NewPost,
                        null,
                        null,
                        tags.ToArray());


                    NotifyClient.SendNoticeAsync(
                        ASC.Blogs.Core.Constants.NewPostByAuthor,
                        post.UserID.ToString(),
                        null,
                        tags.ToArray());

                    NotifyClient.EndSingleRecipientEvent("asc_blog");
                }
                finally
                {
                    NotifyClient.RemoveInterceptor(initiatorInterceptor.Name);
                }

                BlogUserActivityPublisher.AddPost(post); //TODO:
            }
            else
            {
                BlogUserActivityPublisher.EditPost(post, SecurityContext.CurrentAccount.ID);
            }
            if (notifyComments)
            {
                ASC.Notify.Model.ISubscriptionProvider subscriptionProvider = NotifySource.GetSubscriptionProvider();

                subscriptionProvider.Subscribe(
                             ASC.Blogs.Core.Constants.NewComment,
                             post.ID.ToString(),
                             NotifySource.GetRecipientsProvider().
                             GetRecipient(post.UserID.ToString())
                        );

            }

        }
        public void DeletePost(Post post)
        {
            CommunitySecurity.CheckPermissions(post, Constants.Action_EditRemovePost);
            _storage.GetPostDao().DeletePost(post.ID);
            BlogUserActivityPublisher.DeletePost(post, CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID));
            NotifySource.GetSubscriptionProvider().UnSubscribe(
                Constants.NewComment,
                post.ID.ToString()
            );



        }
        #endregion

        #region misc
        public List<TagStat> GetTopTagsList(int count)
        {
            return _storage.GetPostDao().GetTagStat(count);
        }

        public List<string> GetTags(string like, int limit)
        {
            return _storage.GetPostDao().GetTags(like, limit);
        }



        #endregion

        #region comments
        public List<Comment> GetPostComments(Guid postId)
        {
            return _storage.GetPostDao().GetComments(postId);
        }
        public Comment GetCommentById(Guid commentId)
        {
            return _storage.GetPostDao().GetCommentById(commentId);
        }
        public void SaveComment(Comment comment, Post post)
        {
            CommunitySecurity.DemandPermissions(post, ASC.Blogs.Core.Constants.Action_AddComment);
            SaveComment(comment);

            var initiatorInterceptor = new InitiatorInterceptor(new DirectRecipient(comment.UserID.ToString(), ""));
            try
            {
                NotifyClient.BeginSingleRecipientEvent("asc_blog_c");
                NotifyClient.AddInterceptor(initiatorInterceptor);

                List<ITagValue> tags = new List<ITagValue>
                {
                    new TagValue(ASC.Blogs.Core.Constants.TagPostSubject, post.Title),
                    new TagValue(ASC.Blogs.Core.Constants.TagPostPreview, post.GetPreviewText(500)),
                    new TagValue(ASC.Blogs.Core.Constants.TagUserName, DisplayUserSettings.GetFullUserName(comment.UserID)),
                    new TagValue(ASC.Blogs.Core.Constants.TagUserURL, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(comment.UserID,CommonLinkUtility.GetProductID()))),
                    new TagValue(ASC.Blogs.Core.Constants.TagDate, string.Format("{0:d} {0:t}", comment.Datetime)),
                    new TagValue(ASC.Blogs.Core.Constants.TagCommentBody, comment.Content),

                    new TagValue(ASC.Blogs.Core.Constants.TagURL, CommonLinkUtility.GetFullAbsolutePath(ASC.Blogs.Core.Constants.ViewBlogPageUrl + "?blogID=" + post.ID.ToString())),
                    new TagValue(ASC.Blogs.Core.Constants.TagCommentURL, CommonLinkUtility.GetFullAbsolutePath(ASC.Blogs.Core.Constants.ViewBlogPageUrl + "?blogID=" + post.ID.ToString()+"#"+comment.ID.ToString())),
                    GetReplyToTag(comment.ID, post)
                };

                NotifyClient.SendNoticeAsync(
                    ASC.Blogs.Core.Constants.NewComment,
                    post.ID.ToString(),
                    null,
                    tags.ToArray());

                NotifyClient.EndSingleRecipientEvent("asc_blog_c");
            }
            finally
            {
                NotifyClient.RemoveInterceptor(initiatorInterceptor.Name);
            }

            BlogUserActivityPublisher.AddComment(comment, post);

            ASC.Notify.Model.ISubscriptionProvider subscriptionProvider = NotifySource.GetSubscriptionProvider();

            if (!subscriptionProvider.IsUnsubscribe((IDirectRecipient)NotifySource.GetRecipientsProvider().
                         GetRecipient(SecurityContext.CurrentAccount.ID.ToString()), ASC.Blogs.Core.Constants.NewComment, post.ID.ToString()))
            {
                subscriptionProvider.Subscribe(
                             ASC.Blogs.Core.Constants.NewComment,
                             post.ID.ToString(),
                             NotifySource.GetRecipientsProvider().
                             GetRecipient(SecurityContext.CurrentAccount.ID.ToString())
                        );
            }

        }

        private static TagValue GetReplyToTag(Guid commentId, Post post)
        {
            return ReplyToTagProvider.Comment("blog", post.ID.ToString(), commentId.ToString());
        }


        private void SaveComment(Comment comment)
        {
            CommunitySecurity.DemandPermissions(comment, ASC.Blogs.Core.Constants.Action_EditRemoveComment);

            if (String.IsNullOrEmpty(comment.Content))
                throw new ArgumentException("comment");
            if (comment.PostId == Guid.Empty)
                throw new ArgumentException("comment");

            _storage.GetPostDao().SaveComment(comment);
        }

        public void DeleteComment(Guid commentId)
        {

        }
        #endregion


        #endregion

        #region notify
        void InitNotify()
        {
            if (_notifySource == null)
                _notifySource = new BlogNotifySource();
            if (_notifyClient == null)
                _notifyClient = ASC.Core.WorkContext.NotifyContext.NotifyService.RegisterClient(_notifySource);
        }
        public BlogNotifySource NotifySource { get { return _notifySource; } }
        public INotifyClient NotifyClient { get { return _notifyClient; } }

        #endregion

        public void UpdateComment(Comment comment, Post post)
        {
            SaveComment(comment);
            BlogUserActivityPublisher.UpdateComment(comment, post, SecurityContext.CurrentAccount.ID);
        }

        public void RemoveComment(Comment comment, Post post)
        {
            SaveComment(comment);
            BlogUserActivityPublisher.RemoveComment(comment, post, SecurityContext.CurrentAccount.ID);
        }
    }


    public class PostsQuery : PagingQuery<PostsQuery>
    {
        internal bool WithTags = true;
        internal bool WithContent = true;
        internal string Tag;
        internal Guid? UserId;
        internal string SearchText;

        public PostsQuery NoTags()
        {
            WithTags = false; return this;
        }
        public PostsQuery NoContent()
        {
            WithContent = false; return this;
        }
        public PostsQuery SetTag(string tag)
        {
            if (String.IsNullOrEmpty(tag))
                throw new ArgumentException("tag");

            Tag = tag; return this;
        }

        public PostsQuery SetUser(Guid userId)
        {
            UserId = userId; return this;
        }
        public PostsQuery SetSearch(string searchText)
        {
            SearchText = searchText; return this;
        }
    }
    public class PagingQuery : PagingQuery<PagingQuery>
    {
        public PagingQuery() { }
        public PagingQuery(PostsQuery q)
        {
            Count = q.Count;
            Offset = q.Offset;
        }
    }
    public class PagingQuery<T>
        : Query<T>
        where T : class
    {
        internal int? Count;
        internal int? Offset;

        public T SetCount(int count)
        {
            Count = count; return this as T;
        }
        public T SetOffset(int offset)
        {
            Offset = offset; return this as T;
        }
    }

    public class Query<T>
        where T : class
    {
        public Query()
        {
        }

    }

}
