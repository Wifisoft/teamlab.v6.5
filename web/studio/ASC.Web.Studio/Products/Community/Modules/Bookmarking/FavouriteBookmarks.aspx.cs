using System;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Core.Utility;
using ASC.Web.UserControls.Bookmarking.Common;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;

using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.Controls;
using System.Text;

namespace ASC.Web.Community.Bookmarking
{
	public partial class FavouriteBookmarks : BookmarkingBasePage
	{	
		protected override void PageLoad()
		{
			ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.Favourites;

			var c = LoadControl(BookmarkUserControlPath.BookmarkingUserControlPath);

			var pageTitle = BookmarkingResource.FavouritesNavigationItem;

			container.Body.Controls.Add(c);			

			InitBreadcrumbs(pageTitle);
			Title = HeaderStringHelper.GetPageTitle(pageTitle, container.BreadCrumbs);

			//Actions and Navigation
			BookmarkingNavigationUtil.SetBookmarkingActionsAndNavigation(BookmarkingSideHolder.Controls);
			BookmarkingNavigationUtil.SetFavouriteTagsCloud(BookmarkingSideHolder.Controls);

			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = BookmarkingSettings.ModuleId;
		}

		protected override void InitBreadcrumbs(string pageTitle)
		{
			container.BreadCrumbs.Add(new BreadCrumb() { Caption = BookmarkingResource.PageTitle, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });

			container.BreadCrumbs.Add(new BreadCrumb() { Caption = pageTitle, NavigationUrl = BookmarkingRequestConstants.FavouriteBookmarksPageName });
						
			var searchText = ServiceHelper.GetSearchTag();
			if (!String.IsNullOrEmpty(searchText))
			{
				StringBuilder sb = new StringBuilder();
				var searchResults = String.Format("{0} {1}", BookmarkingResource.TagBookmarks, searchText);

				container.BreadCrumbs.Add(new BreadCrumb() { Caption = searchResults, NavigationUrl = ServiceHelper.GetCurrentURL() });
			}
		}
	}
}
