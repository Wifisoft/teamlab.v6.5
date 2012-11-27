using System;
using ASC.Bookmarking.Pojo;
using ASC.Web.Controls;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;


namespace ASC.Web.UserControls.Bookmarking
{
	public partial class BookmarkInfoUserControl : BookmarkInfoBase
	{

		public override void InitUserControl()
		{
			var singleBookmarkUserControl = LoadControl(BookmarkUserControlPath.SingleBookmarkUserControlPath) as BookmarkInfoBase;
			singleBookmarkUserControl.Bookmark = Bookmark;
			singleBookmarkUserControl.UserBookmark = UserBookmark;
			BookmarkInfoHolder.Controls.Add(singleBookmarkUserControl);

			var SortControl = new ViewSwitcher();
			
			SortControl.TabItems.Add(new ViewSwitcherTabItem()
			{
				TabName = BookmarkingUCResource.BookmarkedBy,
				DivID = "BookmarkedByPanel",
				IsSelected = ServiceHelper.SelectedTab == 0,
				SkipRender = true
			});

            SortControl.TabItems.Add(new ViewSwitcherTabItem()
			{
				TabName = BookmarkingUCResource.Comments +  String.Format(" ({0})",CommentsCount),
				DivID = "BookmarkCommentsPanel",
				IsSelected = ServiceHelper.SelectedTab == 1,
				SkipRender = true
			});


			BookmarkInfoTabsContainer.Controls.Add(SortControl);


			//Init comments
			using (var c = LoadControl(BookmarkUserControlPath.CommentsUserControlPath) as CommentsUserControl)
			{
				c.BookmarkID = this.Bookmark.ID;
				c.BookmarkComments = ServiceHelper.GetBookmarkComments(this.Bookmark);
				c.InitComments();
				CommentsHolder.Controls.Add(c);
			}
			if (Bookmark != null)
			{
				var userBookmarks = Bookmark.UserBookmarks;
				if (userBookmarks != null && userBookmarks.Count > 0)
				{
					//Init added by list
					AddedByRepeater.DataSource = userBookmarks;
					AddedByRepeater.DataBind();
				}
			}
		}		

		public string GetAddedByTableItem(bool TintFlag, string UserImage, string UserPageLink,
										 string UserBookmarkDescription, string DateAddedAsString, object userID)
		{
			return new BookmarkAddedByUserContorl().GetAddedByTableItem(TintFlag, UserImage, UserPageLink, UserBookmarkDescription, DateAddedAsString, userID);
		}
	}

}