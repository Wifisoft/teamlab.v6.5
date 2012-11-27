using System.Collections.Generic;
using ASC.Bookmarking.Pojo;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Controls;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;

namespace ASC.Web.Community.Bookmarking
{
	public partial class BookmarkInfo : BookmarkingBasePage
	{
		protected override void PageLoad()
		{
			ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark;

			var c = LoadControl(BookmarkUserControlPath.BookmarkInfoUserControlPath) as BookmarkInfoUserControl;
			InitBookmarkInfoUserControl(c);

			var pageTitle = BookmarkingResource.FavouritesNavigationItem;


			var bookmarks = new List<Bookmark>();
			bookmarks.Add(c.Bookmark);

			var bookmarkingUserControl = LoadControl(BookmarkUserControlPath.BookmarkingUserControlPath) as BookmarkingUserControl;
			bookmarkingUserControl.Bookmarks = bookmarks;

			container.Body.Controls.Add(bookmarkingUserControl);
			container.Body.Controls.Add(c);


			InitBreadcrumbs(pageTitle);
			Title = HeaderStringHelper.GetPageTitle(pageTitle, container.BreadCrumbs);

			//Actions and Navigation
			BookmarkingNavigationUtil.SetBookmarkInfoActionsAndNavigation(BookmarkingSideHolder.Controls);
			BookmarkingNavigationUtil.SetRelatedTagsCloud(BookmarkingSideHolder.Controls);
			//BookmarkingNavigationUtil.SetPopularTagsCloud(BookmarkingSideHolder.Controls);

			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = BookmarkingSettings.ModuleId;
		}

		#region Init Bookmark
		private void InitBookmarkInfoUserControl(BookmarkInfoUserControl c)
		{
			var b = ServiceHelper.GetBookmarkWithUserBookmarks();

			if (b == null)
			{
				var url = Request.QueryString[BookmarkingRequestConstants.UrlGetRequest];

				b = ServiceHelper.GetBookmarkWithUserBookmarks(url, false);

				if (b == null)
				{
					b = ServiceHelper.GetBookmarkWithUserBookmarks(url, true);
				}

				if (b == null)
				{

					var redirectUrl = BookmarkingRequestConstants.CreateBookmarkPageName;
					if (!string.IsNullOrEmpty(url))
					{
						url = BookmarkingServiceHelper.UpdateBookmarkInfoUrl(url);
						redirectUrl += string.Format("?{0}={1}", BookmarkingRequestConstants.UrlGetRequest, url);
					}

					Response.Redirect(redirectUrl);
				}
			}
			c.Bookmark = b;
			c.UserBookmark = ServiceHelper.GetCurrentUserBookmark(b.UserBookmarks);
		}
		
		#endregion

		protected override void InitBreadcrumbs(string pageTitle)
		{
			container.BreadCrumbs.Add(new BreadCrumb() { Caption = BookmarkingResource.BookmarksNavigationItem, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });

			//Get text from the search input
			var bookmarkName = string.Empty;
			if (ServiceHelper.BookmarkToAdd != null)
			{
				bookmarkName = ServiceHelper.BookmarkToAdd.Name;
			}

			container.BreadCrumbs.Add(new BreadCrumb() { Caption = bookmarkName, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });
		}
	}
}
