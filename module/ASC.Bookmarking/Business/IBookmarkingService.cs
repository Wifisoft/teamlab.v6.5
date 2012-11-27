using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ASC.Bookmarking.Pojo;
using ASC.Core.Users;
using ASC.Web.Core.Subscriptions;
using ASC.Notify.Model;
namespace ASC.Bookmarking.Business
{

	public interface IBookmarkingService
	{

		IList<Bookmark> GetAllBookmarks(int FirstResult, int MaxResults);
		IList<Bookmark> GetAllBookmarks();

		Bookmark AddBookmark(Bookmark b, IList<Tag> tags);

		Bookmark UpdateBookmark(UserBookmark userBookmark, IList<Tag> tags);
		Bookmark UpdateBookmark(Bookmark bookmark, IList<Tag> tags);
		
		UserInfo GetCurrentUser();		
		
		IList<Tag> GetAllTags(string startSymbols, int limit);

		IList<Tag> GetAllTags();

		Bookmark GetBookmarkByUrl(string url);

		Bookmark GetBookmarkByID(long id);


		IList<UserBookmark> GetUserBookmarks(Bookmark b);

		UserBookmark GetCurrentUserBookmark(Bookmark b);

		Bookmark GetBookmarkWithUserBookmarks(string url);



		Bookmark RemoveBookmarkFromFavourite(long bookmarkID);

		IList<Bookmark> GetFavouriteBookmarksSortedByRaiting(int FirstResult, int MaxResults);
		IList<Bookmark> GetFavouriteBookmarksSortedByDate(int FirstResult, int MaxResults);


		IList<Bookmark> GetMostRecentBookmarks(int FirstResult, int MaxResults);
		IList<Bookmark> GetMostRecentBookmarksWithRaiting(int FirstResult, int MaxResults);
		IList<Bookmark> GetTopOfTheDay(int FirstResult, int MaxResults);
		IList<Bookmark> GetTopOfTheWeek(int FirstResult, int MaxResults);
		IList<Bookmark> GetTopOfTheMonth(int FirstResult, int MaxResults);
		IList<Bookmark> GetTopOfTheYear(int FirstResult, int MaxResults);

		

		#region Tags		

		IList<Tag> GetMostRecentTags(int FirstResult, int MaxResults);
		IList<Tag> GetTagsCloud();
		IList<Tag> GetMostPopularTags(int FirstResult, int MaxResults);
		IList<Tag> GetFavouriteTagsCloud();
		IList<Tag> GetRelatedTagsCloud(long BookmarkID);
		IList<Tag> GetTagsSortedByName(int FirstResult, int MaxResults);
		int GetTagsCount();
		IList<Bookmark> GetMostPopularBookmarksByTag(Tag t);
		IList<Bookmark> GetMostPopularBookmarksByTag(IList<Tag> tags);
		IList<Tag> GetBookmarkTags(Bookmark b);
		IList<Tag> GetUserBookmarkTags(UserBookmark b);
		#endregion



		#region Comments
		Comment GetCommentById(Guid commentID);

		void AddComment(Comment comment);

		void UpdateComment(Guid commentID, string text);

		void RemoveComment(Guid commentID);

		long GetCommentsCount(long bookmarkID);

		IList<Comment> GetBookmarkComments(Bookmark b);

		IList<Comment> GetChildComments(Comment c);
		#endregion

		void Subscribe(string objectID, INotifyAction notifyAction);

		bool IsSubscribed(string objectID, INotifyAction notifyAction);

		void UnSubscribe(string objectID, INotifyAction notifyAction);



		#region Search
		IList<Bookmark> SearchBookmarks(IList<string> searchStringList, int FirstResult, int MaxResults);

		IList<Bookmark> SearchAllBookmarks(IList<string> searchStringList);

		IList<Bookmark> SearchBookmarksSortedByRaiting(IList<string> searchStringList, int FirstResult, int MaxResults);

		IList<Bookmark> SearchBookmarksByTag(string searchString, int FirstResult, int MaxResults);

		IList<Bookmark> SearchMostPopularBookmarksByTag(string tagName, int FirstResult, int MaxResults);
		#endregion


		IList<Bookmark> GetBookmarksCreatedByUser(Guid userID, int FirstResult, int MaxResults);

		IList<Bookmark> GetMostPopularBookmarksCreatedByUser(Guid userID, int FirstResult, int MaxResults);

		long GetBookmarksCountCreatedByUser(Guid userID);



		IList<Bookmark> GetFullBookmarksInfo(IList<long> bookmarkIds);

		IList<Bookmark> GetFullBookmarksInfo(IList<Bookmark> bookmarks);

		void SetBookmarkTags(Bookmark b);
	}
}
