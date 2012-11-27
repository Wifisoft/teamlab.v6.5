using System;
using System.Collections.Generic;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Pojo;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Users;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.UserControls.Bookmarking.Common.Util
{
	public static class BookmarkingConverter
	{
		public static IList<CommentInfo> ConvertCommentList(IList<Comment> commentList)
		{
			var result = new List<CommentInfo>();
			foreach (var comment in commentList)
			{
				var parentID = Guid.Empty;
				try
				{
					parentID = new Guid(comment.Parent);
				}
				catch { }
				if (Guid.Empty.Equals(parentID))
				{
					var c = ConvertComment(comment, commentList);
					result.Add(c);
				}
			}
			return result;
		}

		public static CommentInfo ConvertComment(Comment comment, IList<Comment> commentList)
		{
			var userID = comment.UserID;

			CommentInfo c = new CommentInfo();

			c.CommentID = comment.ID.ToString();
			c.UserID = userID;
			c.TimeStamp = comment.Datetime;
			c.TimeStampStr = comment.Datetime.Ago();
						
			c.Inactive = comment.Inactive;
			c.CommentBody = comment.Content;
			c.UserFullName = DisplayUserSettings.GetFullUserName(userID);
			c.UserAvatar = BookmarkingServiceHelper.GetHTMLUserAvatar(userID);
			c.IsEditPermissions = BookmarkingPermissionsCheck.PermissionCheckEditComment(comment);
			c.IsResponsePermissions = BookmarkingPermissionsCheck.PermissionCheckCreateComment();

			c.UserPost = BookmarkingServiceHelper.GetUserInfo(userID).Title;

			var commentsList = new List<CommentInfo>();

			var childComments = GetChildComments(comment, commentList);
			if (childComments != null)
			{
				foreach (var item in childComments)
				{
					commentsList.Add(ConvertComment(item, commentList));
				}
			}
			c.CommentList = commentsList;
			return c;
		}

		private static IList<Comment> GetChildComments(Comment c, IList<Comment> comments)
		{
			var children = new List<Comment>();
			var commentID = c.ID.ToString();
			foreach (var comment in comments)
			{
				if (commentID.Equals(comment.Parent))
				{
					children.Add(comment);
				}
			}
			return children;
		}

		public static string GetDateAsString(DateTime date)
		{
			try
			{
				return date.ToShortTimeString() + "&nbsp;&nbsp;&nbsp;" + date.ToShortDateString();
			}
			catch
			{
				return string.Empty;
			}
		}

	}


}
