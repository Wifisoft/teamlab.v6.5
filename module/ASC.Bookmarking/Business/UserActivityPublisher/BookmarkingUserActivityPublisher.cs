using System;
using System.Collections.Generic;
using System.Text;
using ASC.Web.Core.Users.Activity;
using ASC.Core;
using ASC.Bookmarking.Pojo;
using ASC.Bookmarking.Resources;
using System.Web;
using ASC.Bookmarking.Common;
using ASC.Bookmarking.Common.Util;

namespace ASC.Bookmarking.Business.UserActivityPublisher
{
	public class BookmarkingUserActivityPublisher : BaseUserActivityPublisher
	{
		private class BookmarkingUserActivity : UserActivity
		{
			public BookmarkingUserActivity()
			{
				this.Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();;
				this.UserID = SecurityContext.CurrentAccount.ID;
				this.ModuleID = BookmarkingBusinessConstants.BookmarkingCommunityModuleId;
				this.ProductID = BookmarkingBusinessConstants.CommunityProductID;
			}			
		}

		public static void BookmarkCreated(Bookmark b)
		{
			ASC.Web.Core.Users.Activity.UserActivityPublisher.Publish<BookmarkingUserActivityPublisher>(
				new BookmarkingUserActivity()
				{
					Title = HttpUtility.HtmlDecode(b.Name),
					ActionType = UserActivityConstants.ContentActionType,
					ContentID = b.ID.ToString(),
					BusinessValue = UserActivityConstants.ImportantContent,
					ActionText = BookmarkingBusinessResources.BookmarkCreatedAction,
					URL = BookmarkingBusinessUtil.GenerateBookmarkInfoUrl(b)
				});
		}

		public static void BookmarkAddedAsFavourite(Bookmark b)
		{
			ASC.Web.Core.Users.Activity.UserActivityPublisher.Publish<BookmarkingUserActivityPublisher>(
				new BookmarkingUserActivity()
				{
					Title = HttpUtility.HtmlDecode(b.Name),
					ActionType = UserActivityConstants.ActivityActionType,
					ContentID = b.ID.ToString(),
					BusinessValue = UserActivityConstants.NormalActivity,
					ActionText = BookmarkingBusinessResources.BookmarkAddedToFavouritesAction,
					URL = BookmarkingBusinessUtil.GenerateBookmarkInfoUrl(b)
				});
		}

		public static void BookmarkRemoved(Bookmark b)
		{
			ASC.Web.Core.Users.Activity.UserActivityPublisher.Publish<BookmarkingUserActivityPublisher>(
				new BookmarkingUserActivity()
				{
					Title = b.Name,
					ActionType = UserActivityConstants.ActivityActionType,
					ContentID = b.ID.ToString(),
					BusinessValue = UserActivityConstants.SmallActivity,
					ActionText = BookmarkingBusinessResources.BookmarkRemovedAction,
					URL = BookmarkingBusinessUtil.GenerateBookmarksUrl(b)
				});
		}

		public static void BookmarkCommentAdded(Comment c, Bookmark b)
		{
			ASC.Web.Core.Users.Activity.UserActivityPublisher.Publish<BookmarkingUserActivityPublisher>(
				new BookmarkingUserActivity()
				{
					Title = b.Name,
					ActionType = UserActivityConstants.ActivityActionType,
					ContentID = c.ID.ToString(),
					BusinessValue = UserActivityConstants.SmallActivity,
					ActionText = BookmarkingBusinessResources.CommentCreatedAction,
					URL = BookmarkingBusinessUtil.GenerateBookmarkInfoUrl(b)
				});
		}

		public static void BookmarkCommentUpdated(Comment c, Bookmark b)
		{
			ASC.Web.Core.Users.Activity.UserActivityPublisher.Publish<BookmarkingUserActivityPublisher>(
				new BookmarkingUserActivity()
				{
					Title = b.Name,
					ActionType = UserActivityConstants.ActivityActionType,
					ContentID = c.ID.ToString(),
					BusinessValue = UserActivityConstants.NormalActivity,
					ActionText = BookmarkingBusinessResources.CommentModifiedAction,
					URL = BookmarkingBusinessUtil.GenerateBookmarkInfoUrl(b)
				});
		}

		public static void BookmarkCommentRemoved(Comment c, Bookmark b)
		{
			ASC.Web.Core.Users.Activity.UserActivityPublisher.Publish<BookmarkingUserActivityPublisher>(
				new BookmarkingUserActivity()
				{
					Title = b.Name,
					ActionType = UserActivityConstants.ActivityActionType,
					ContentID = c.ID.ToString(),
					BusinessValue = UserActivityConstants.SmallActivity,
					ActionText = BookmarkingBusinessResources.CommentRemovedAction,
					URL = BookmarkingBusinessUtil.GenerateBookmarkInfoUrl(b)
				});
		}

	}
}
