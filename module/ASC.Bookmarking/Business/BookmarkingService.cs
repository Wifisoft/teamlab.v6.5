using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Bookmarking.Business.Subscriptions;
using ASC.Bookmarking.Business.UserActivityPublisher;
using ASC.Bookmarking.Common;
using ASC.Bookmarking.Common.Util;
using ASC.Bookmarking.Dao;
using ASC.Bookmarking.Pojo;
using ASC.Core;
using ASC.Core.Common.Notify;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;
using Tag = ASC.Bookmarking.Pojo.Tag;

namespace ASC.Bookmarking.Business
{
    public class BookmarkingService : BookmarkingSessionObject<BookmarkingService>, IBookmarkingService
    {
        #region Fields
        public BookmarkingHibernateDao dao = BookmarkingHibernateDao.GetCurrentInstanse();

        private IList<Tag> TagsList;
        #endregion

        #region Get Bookmarks
        public UserInfo GetCurrentUser()
        {
            return dao.GetCurrentUser();
        }

        public IList<Bookmark> GetAllBookmarks(int FirstResult, int MaxResults)
        {
            return dao.GetAllBookmarks(FirstResult, MaxResults);
        }

        public IList<Bookmark> GetAllBookmarks()
        {
            return dao.GetAllBookmarks();
        }

        public Bookmark GetBookmarkByUrl(string url)
        {
            return dao.GetBookmarkByUrl(url);
        }

        public Bookmark GetBookmarkByID(long id)
        {
            return dao.GetBookmarkByID(id);
        }
        public long GetBookmarksCountCreatedByUser(Guid userID)
        {
            return dao.GetBookmarksCountCreatedByUser(userID);
        }

        public UserBookmark GetCurrentUserBookmark(Bookmark b)
        {
            return dao.GetCurrentUserBookmark(b);
        }

        public IList<Bookmark> GetBookmarksCreatedByUser(Guid userID, int FirstResult, int MaxResults)
        {
            return dao.GetBookmarksCreatedByUser(userID, FirstResult, MaxResults);
        }

        public IList<Bookmark> GetMostPopularBookmarksCreatedByUser(Guid userID, int FirstResult, int MaxResults)
        {
            var bookmarks = dao.GetBookmarksCreatedByUser(userID, 0, int.MaxValue);
            var ids = (from b in bookmarks
                       select b.ID).ToList<long>();
            return dao.GetMostPopularBookmarks(ids, FirstResult, MaxResults);
        }

        public IList<UserBookmark> GetUserBookmarks(Bookmark b)
        {
            return dao.GetUserBookmarks(b);
        }

        public Bookmark GetBookmarkWithUserBookmarks(string url)
        {
            return dao.GetBookmarkWithUserBookmarks(url);
        }
        #endregion

        #region Get Tags
        public IList<Tag> GetAllTags(string startSymbols, int limit)
        {
            return dao.GetAllTags(startSymbols, limit);
        }

        public IList<Tag> GetAllTags()
        {
            TagsList = dao.GetAllTags(null, Int32.MaxValue);
            return TagsList;
        }
        #endregion

        #region Add Bookmark
        public Bookmark AddBookmark(Bookmark b, IList<Tag> tags)
        {
            dao.UpdateBookmark(b, tags);
            SendRecentBookmarkUpdates(b);
            BookmarkingUserActivityPublisher.BookmarkCreated(b);
            SubscribeOnBookmarkComments(b);
            return b;
        }
        #endregion

        #region Update Bookmark
        public Bookmark UpdateBookmark(UserBookmark userBookmark, IList<Tag> tags)
        {
            dao.UpdateBookmark(userBookmark, tags);
            var b = GetBookmarkByID(userBookmark.BookmarkID);
            BookmarkingUserActivityPublisher.BookmarkAddedAsFavourite(b);
            SubscribeOnBookmarkComments(b);
            return b;
        }

        public Bookmark UpdateBookmark(Bookmark bookmark, IList<Tag> tags)
        {
            dao.UpdateBookmark(bookmark, tags);
            var b = GetBookmarkByID(bookmark.ID);
            BookmarkingUserActivityPublisher.BookmarkAddedAsFavourite(b);
            return b;
        }

        private static IList<UserBookmarkTag> GetUserBookmarkTagsToSave(UserBookmark userBookmark, IList<Tag> tags)
        {
            var userBookmarkTags = new List<UserBookmarkTag>();
            foreach (var tag in tags)
            {
                userBookmarkTags.Add(new UserBookmarkTag() { TagID = tag.TagID, UserBookmarkID = userBookmark.UserBookmarkID });
            }
            return userBookmarkTags;
        }

        private static IList<BookmarkTag> GetBookmarkTagsToSave(Bookmark b, IList<Tag> tags)
        {
            var bookmarkTags = new List<BookmarkTag>();
            foreach (var tag in tags)
            {
                bookmarkTags.Add(new BookmarkTag() { TagID = tag.TagID, BookmarkID = b.ID });
            }
            return bookmarkTags;
        }
        #endregion

        #region Remove Bookmark from Favourite
        public static string DeletedBookmarkUrl { get; set; }

        public Bookmark RemoveBookmarkFromFavourite(long bookmarkID)
        {
            var b = dao.GetBookmarkByID(bookmarkID);
            var comments = GetBookmarkComments(b);
            var result = dao.RemoveBookmarkFromFavourite(bookmarkID);
            if (b != null)
            {
                DeletedBookmarkUrl = b.URL;
                UnSubscribe(b.ID.ToString(), BookmarkingBusinessConstants.NotifyActionNewComment);
                BookmarkingUserActivityPublisher.BookmarkRemoved(b);
            }
            if (result == null)
            {
                foreach (var comment in comments)
                {
                    CommonControlsConfigurer.FCKUploadsRemoveForItem("bookmarking_comments", comment.ID.ToString());
                }
            }
            return result;
        }
        #endregion

        #region Sorting
        public IList<Bookmark> GetFavouriteBookmarksSortedByRaiting(int FirstResult, int MaxResults)
        {
            return dao.GetFavouriteBookmarksSortedByRaiting(FirstResult, MaxResults);
        }
        public IList<Bookmark> GetFavouriteBookmarksSortedByDate(int FirstResult, int MaxResults)
        {
            return dao.GetFavouriteBookmarksSortedByDate(FirstResult, MaxResults);
        }

        public IList<Bookmark> GetMostRecentBookmarks(int FirstResult, int MaxResults)
        {
            return dao.GetMostRecentBookmarks(FirstResult, MaxResults);
        }

        public IList<Bookmark> GetMostRecentBookmarksWithRaiting(int FirstResult, int MaxResults)
        {
            return dao.GetMostRecentBookmarksWithRaiting(FirstResult, MaxResults);
        }

        public IList<Bookmark> GetTopOfTheDay(int FirstResult, int MaxResults)
        {
            var d = GetDateTimeToUtcWithShift(-1);
            return dao.GetBookmarksSortedByRaiting(d, FirstResult, MaxResults);
        }

        public IList<Bookmark> GetTopOfTheWeek(int FirstResult, int MaxResults)
        {
            var d = GetDateTimeToUtcWithShift(-7);
            return dao.GetBookmarksSortedByRaiting(d, FirstResult, MaxResults);
        }

        public IList<Bookmark> GetTopOfTheMonth(int FirstResult, int MaxResults)
        {
            var d = GetDateTimeToUtcWithShift(-31);
            return dao.GetBookmarksSortedByRaiting(d, FirstResult, MaxResults);
        }

        public IList<Bookmark> GetTopOfTheYear(int FirstResult, int MaxResults)
        {
            var d = GetDateTimeToUtcWithShift(-365);
            return dao.GetBookmarksSortedByRaiting(d, FirstResult, MaxResults);
        }

        private static DateTime GetDateTimeToUtcWithShift(int days)
        {
            var now = DateTime.UtcNow;
            return now.Date.AddDays(days);
        }

        public IList<Tag> GetMostRecentTags(int FirstResult, int MaxResults)
        {
            return dao.GetMostRecentTags(FirstResult, MaxResults);
        }

        public IList<Tag> GetTagsSortedByName(int FirstResult, int MaxResults)
        {
            return dao.GetTagsSortedByName(FirstResult, MaxResults);
        }

        public int GetTagsCount()
        {
            return dao.GetTagsCount();
        }

        #endregion

        #region Tags
        private static IList<Tag> SortTagsCloud(List<Tag> tags)
        {
            tags.Sort(delegate(Tag t1, Tag t2) { return t1.Name.CompareTo(t2.Name); });
            return tags;
        }

        public IList<Tag> GetTagsCloud()
        {
            var tags = dao.GetTagsSortByPopularity(BookmarkingBusinessConstants.BookmarkingTagsCloudMaxCount) as List<Tag>;
            return SortTagsCloud(tags);
        }

        public IList<Tag> GetMostPopularTags(int FirstResult, int MaxResults)
        {
            return dao.GetMostPopularTags(FirstResult, MaxResults);
        }

        public IList<Tag> GetFavouriteTagsCloud()
        {
            var tags = dao.GetFavouriteTagsSortByPopularity(BookmarkingBusinessConstants.BookmarkingTagsCloudMaxCount) as List<Tag>;
            return SortTagsCloud(tags);
        }

        public IList<Tag> GetRelatedTagsCloud(long BookmarkID)
        {
            var tags = dao.GetRelatedTagsSortedByName(BookmarkID, BookmarkingBusinessConstants.BookmarkingTagsCloudMaxCount) as List<Tag>;
            return tags;
        }

        public IList<Bookmark> GetMostPopularBookmarksByTag(Tag t)
        {
            return dao.GetMostPopularBookmarksByTag(t, BookmarkingBusinessConstants.MostPopularBookmarksByTagLimit);
        }

        public IList<Bookmark> GetMostPopularBookmarksByTag(IList<Tag> tags)
        {
            return dao.SearchMostPopularBookmarksByTag(tags, 0, Int32.MaxValue);
        }

        public IList<Tag> GetBookmarkTags(Bookmark b)
        {
            return dao.GetBookmarkTags(b);
        }

        public IList<Tag> GetUserBookmarkTags(UserBookmark ub)
        {
            return dao.GetUserBookmarkTags(ub);
        }
        #endregion

        #region Comments

        public Comment GetCommentById(Guid commentID)
        {
            return dao.GetCommentById(commentID);
        }

        public void AddComment(Comment comment)
        {
            var c = dao.AddComment(comment);

            if (c == null)
            {
                return;
            }
            var b = GetCommentBookmark(c);

            SendCommentUpdates(c, b);
            BookmarkingUserActivityPublisher.BookmarkCommentAdded(c, b);
            SubscribeOnBookmarkComments(b, true);
        }

        public void UpdateComment(Guid commentID, string text)
        {
            var c = dao.UpdateComment(commentID, text);
            if (c != null)
            {
                var b = GetCommentBookmark(c);
                if (b != null)
                {
                    SendCommentUpdates(c, b);
                    BookmarkingUserActivityPublisher.BookmarkCommentUpdated(c, b);
                }
            }
        }

        public void RemoveComment(Guid commentID)
        {
            var c = dao.RemoveComment(commentID);
            if (c != null)
            {
                var b = GetCommentBookmark(c);
                if (b != null)
                {
                    SendCommentUpdates(c, b);
                    BookmarkingUserActivityPublisher.BookmarkCommentRemoved(c, b);
                }
            }
        }

        public long GetCommentsCount(long bookmarkID)
        {
            return dao.GetCommentsCount(bookmarkID);
        }

        public IList<Comment> GetBookmarkComments(Bookmark b)
        {
            return dao.GetBookmarkComments(b);
        }

        private Bookmark GetCommentBookmark(Comment c)
        {
            return dao.GetCommentBookmark(c);
        }

        public IList<Comment> GetChildComments(Comment c)
        {
            return dao.GetChildComments(c);
        }

        #endregion

        #region Notifications
        private static void SendCommentUpdates(Comment c, Bookmark b)
        {
            if (c == null || b == null)
            {
                return;
            }
            var url = ModifyBookmarkUrl(b);
            var tags = new[]{
					new TagValue(BookmarkSubscriptionConstants.BookmarkTitle, b.Name),
					new TagValue(BookmarkSubscriptionConstants.BookmarkUrl, url),
					new TagValue(BookmarkSubscriptionConstants.UserURL,
													 BookmarkingBusinessUtil.RenderProfileLink(c.UserID)),
					new TagValue(BookmarkSubscriptionConstants.Date, c.Datetime.ToShortString()),
					new TagValue(BookmarkSubscriptionConstants.CommentBody, c.Content),GetReplyToTag(b,c)
							};

            var objectID = b.ID.ToString();

            var action = BookmarkingBusinessConstants.NotifyActionNewComment;

            SendBookmarkNoticeAsync(action, objectID, tags);
        }

        private void SendRecentBookmarkUpdates(Bookmark b)
        {
            var url = ModifyBookmarkUrl(b);
            var tags = new[]{
					new TagValue(BookmarkSubscriptionConstants.BookmarkTitle, b.Name),
					new TagValue(BookmarkSubscriptionConstants.BookmarkUrl, url),
                    new TagValue(BookmarkSubscriptionConstants.UserURL,string.Format("\"{0}\":\"{1}\"",CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(b.UserCreatorID,CommonLinkUtility.GetProductID())),DisplayUserSettings.GetFullUserName(b.UserCreatorID))),//TODO: Rewrite patterns
					new TagValue(BookmarkSubscriptionConstants.Date, b.Date.ToShortString()),
					new TagValue(BookmarkSubscriptionConstants.BookmarkDescription, b.Description),GetReplyToTag(b,null)
							};

            string objectID = BookmarkingBusinessConstants.SubscriptionRecentBookmarkID;

            var action = BookmarkingBusinessConstants.NotifyActionNewBookmark;

            SendBookmarkNoticeAsync(action, objectID, tags);
        }

        private static TagValue GetReplyToTag(Bookmark bookmark, Comment comment)
        {
            return ReplyToTagProvider.Comment("bookmark", bookmark.ID.ToString(), comment != null ? comment.ID.ToString() : null);
        }

        internal static string ModifyBookmarkUrl(Bookmark b)
        {
            var bookmarkUrl = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(b.URL));
            var bookmarkingAbsolutePath = CommonLinkUtility.GetFullAbsolutePath(BookmarkingBusinessConstants.BookmarkingBasePath + "/BookmarkInfo.aspx");

            var url = string.Format("{0}?Url={1}", bookmarkingAbsolutePath, bookmarkUrl);
            return url;
        }

        private static void SendBookmarkNoticeAsync(INotifyAction action, string objectID, TagValue[] tags)
        {
            var initatorInterceptor = new InitiatorInterceptor(GetCurrentRecipient());
            var notifyClient = BookmarkingNotifyClient.NotifyClient;
            try
            {
                notifyClient.AddInterceptor(initatorInterceptor);
                notifyClient.SendNoticeAsync(action, objectID, null, tags);
            }
            finally
            {
                notifyClient.RemoveInterceptor(initatorInterceptor.Name);
            }
        }

        private static IRecipient GetCurrentRecipient()
        {
            return new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name);
        }
        #endregion

        #region Subscriptions
        public void Subscribe(string objectID, INotifyAction notifyAction)
        {
            var provider = BookmarkingNotifySource.Instance.GetSubscriptionProvider();
            provider.Subscribe(notifyAction, objectID, GetCurrentRecipient());
        }

        public bool IsSubscribed(string objectID, INotifyAction notifyAction)
        {
            var provider = BookmarkingNotifySource.Instance.GetSubscriptionProvider();
            var isSubscribed = provider.IsSubscribed(notifyAction, GetCurrentRecipient(), objectID);
            return isSubscribed;
        }

        public void UnSubscribe(string objectID, INotifyAction notifyAction)
        {
            var provider = BookmarkingNotifySource.Instance.GetSubscriptionProvider();
            provider.UnSubscribe(notifyAction, objectID, GetCurrentRecipient());
        }

        private void SubscribeOnBookmarkComments(Bookmark b)
        {
            SubscribeOnBookmarkComments(b, false);
        }

        private void SubscribeOnBookmarkComments(Bookmark b, bool checkIfUnsubscribed)
        {
            if (b == null)
            {
                return;
            }
            var id = b.ID.ToString();
            var provider = BookmarkingNotifySource.Instance.GetSubscriptionProvider();
            var subsribe = true;
            if (checkIfUnsubscribed)
            {
                if (provider.IsUnsubscribe(GetCurrentRecipient() as DirectRecipient, BookmarkingBusinessConstants.NotifyActionNewComment, id))
                {
                    subsribe = false;
                }
            }
            if (subsribe)
            {
                provider.Subscribe(BookmarkingBusinessConstants.NotifyActionNewComment, id, GetCurrentRecipient());
            }
        }
        #endregion

        #region Search

        public IList<Bookmark> SearchBookmarks(IList<string> searchStringList, int FirstResult, int MaxResults)
        {
            return dao.SearchBookmarks(searchStringList, FirstResult, MaxResults);
        }

        public IList<Bookmark> SearchAllBookmarks(IList<string> searchStringList)
        {
            return dao.SearchBookmarks(searchStringList, 0, Int32.MaxValue, true);
        }

        public IList<Bookmark> SearchBookmarksSortedByRaiting(IList<string> searchStringList, int FirstResult, int MaxResults)
        {
            var bookmarks = dao.SearchBookmarks(searchStringList, 0, int.MaxValue);
            var ids = (from b in bookmarks
                       select b.ID).ToList<long>();
            return dao.GetMostPopularBookmarks(ids, FirstResult, MaxResults);
        }

        public IList<Bookmark> SearchBookmarksByTag(string searchString, int FirstResult, int MaxResults)
        {
            return dao.SearchBookmarksByTag(searchString, FirstResult, MaxResults);
        }

        public IList<Bookmark> SearchMostPopularBookmarksByTag(string tagName, int FirstResult, int MaxResults)
        {
            return dao.SearchMostPopularBookmarksByTag(tagName, FirstResult, MaxResults);
        }

        #endregion


        #region IBookmarkingService Members


        public IList<Bookmark> GetFullBookmarksInfo(IList<long> bookmarkIds)
        {
            return dao.GetFullBookmarksInfo(bookmarkIds);
        }

        public IList<Bookmark> GetFullBookmarksInfo(IList<Bookmark> bookmarks)
        {
            if (bookmarks == null || bookmarks.Count == 0)
            {
                return new List<Bookmark>();
            }
            var ids = (from b in bookmarks
                       select b.ID).Distinct<long>().ToList<long>();
            return GetFullBookmarksInfo(ids);
        }

        #endregion

        #region IBookmarkingService Members


        public void SetBookmarkTags(Bookmark b)
        {
            dao.SetBookmarkTags(b);
        }

        #endregion


        public IList<Bookmark> GetBookmarks(DateTime from, DateTime to)
        {
            return dao.GetBookmarksUpdates(from, to);
        }

        public IList<Comment> GetComments(DateTime from, DateTime to)
        {
            return dao.GetCommentsUpdates(from, to);
        }
    }
}
