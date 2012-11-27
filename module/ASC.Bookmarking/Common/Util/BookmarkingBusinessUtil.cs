using System;
using System.Web;
using ASC.Bookmarking.Pojo;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Bookmarking.Common.Util
{
	public static class BookmarkingBusinessUtil
	{
		public static string GenerateBookmarkInfoUrl(Bookmark b)
		{
			return ASC.Bookmarking.Business.BookmarkingService.ModifyBookmarkUrl(b);
		}		

		public static string GenerateBookmarksUrl(Bookmark b)
		{
			return VirtualPathUtility.ToAbsolute(BookmarkingBusinessConstants.BookmarkingBasePath + "/default.aspx");
		}

		public static string RenderProfileLink(Guid userID)
		{
			return CoreContext.UserManager.GetUsers(userID).RenderProfileLink(BookmarkingBusinessConstants.CommunityProductID);
		}
	}
}
