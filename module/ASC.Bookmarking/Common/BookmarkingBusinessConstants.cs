using System;
using ASC.Bookmarking.Resources;
using ASC.Common.Module;
using ASC.Common.Security.Authorizing;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using Action = ASC.Common.Security.Authorizing.Action;


namespace ASC.Bookmarking.Common
{
	public static class BookmarkingBusinessConstants
	{


		public static readonly Guid BookmarkingCommunityModuleId = new Guid("28B10049-DD20-4f54-B986-873BC14CCFC7");

		public const string BookmarkingDbID = "community";

		public static Guid CommunityProductID;

		#region Notify Action String Constants

		public const string BookmarkCreatedID = "new bookmark created";
		public const string BookmarkCommentCreatedID = "new bookmark comment created";
		public const string TagBookmarkCreatedID = "new bookmark for current tag created";

		#endregion

		public const string BookmarkingActionPattern = "ASC.Bookmarking.Patterns.action_pattern.xml";


		public static INotifyAction NotifyActionNewBookmark = new NotifyAction(BookmarkCreatedID, "new-bookmark");
		internal static Guid NotifyActionNewBookmarkID = Guid.NewGuid();

		public static INotifyAction NotifyActionNewComment = new NotifyAction(BookmarkCommentCreatedID, "new-bookmark-comment");
		internal static Guid NotifyActionNewCommentID = Guid.NewGuid();

		public static INotifyAction NotifyActionTag = new NotifyAction(TagBookmarkCreatedID, "new-bookmark-by-tag");

		public const string SubscriptionRecentBookmarkID = null;



		public static ITag TagURL = new Tag("URL");

		public static ITag TagUserName = new Tag("UserName"); 
		public static ITag TagUserURL = new Tag("UserURL");
		public static ITag TagDate = new Tag("Date");  

		public static ITag TagPostPreview = new Tag("PostPreview"); 

		public static ITag TagCommentBody = new Tag("CommentBody"); 
		public static ITag TagCommentURL = new Tag("CommentURL"); 



		public const int MostPopularBookmarksByTagLimit = 3;

		public const string BookmarkingBasePath = "~/Products/Community/Modules/Bookmarking";

		public const string UserBookmarksPageName = "~/Products/Community/Modules/Bookmarking/UserBookmarks.aspx";


		#region Check Permissions

		/// <summary>
		/// base
		/// </summary>
		public static readonly Action BookmarkCreateAction = new Action(
                                                       new Guid("{0D1F72A8-63DA-47ea-AE42-0900E4AC72A9}"),
                                                       "Create bookmark"
												   );

		/// <summary>
		/// base
		/// </summary>
		public static readonly Action BookmarkAddToFavouriteAction = new Action(
                                                       new Guid("{FBC37705-A04C-40ad-A68C-CE2F0423F397}"),
                                                       "Add to favorites"
												   );

		/// <summary>
		/// base
		/// </summary>
		public static readonly Action BookmarkRemoveFromFavouriteAction = new Action(
                                                       new Guid("{08D66144-E1C9-4065-9AA1-AA4BBA0A7BC8}"),
                                                       "Remove from favorites"
												   );

		/// <summary>
		/// base
		/// </summary>
		public static readonly Action BookmarkCreateCommentAction = new Action(
                                                       new Guid("{A362FE79-684E-4d43-A599-65BC1F4E167F}"),
                                                       "Add Comment"
												   );

		/// <summary>
		/// base
		/// </summary>
		public static readonly Action BookmarkEditCommentAction = new Action(
                                                       new Guid("{A18480A4-6D18-4c71-84FA-789888791F45}"),
													   "Edit comment"
												   );


		public static readonly AuthCategory BaseAuthCategory = new AuthCategory(
                                                        "Basic bookmark operations",
														new[]{
                                                            BookmarkCreateAction,
                                                            BookmarkAddToFavouriteAction,
                                                            BookmarkRemoveFromFavouriteAction,
                                                            BookmarkCreateCommentAction,
                                                            BookmarkEditCommentAction
                                                        }
													);

		public static readonly AuthCategory[] AuthorizingCategories = new[] {BaseAuthCategory};
		#endregion


		public static int BookmarkingTagsCloudMaxCount = 40;


	}
}
