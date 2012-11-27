using System.Web.UI.WebControls;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Controls;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;


namespace ASC.Web.Community.Bookmarking
{
	public partial class UserBookmarks : BookmarkingBasePage
	{

		protected override void PageLoad()
		{
			ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.BookmarksCreatedByUser;

			var pageTitle = ServiceHelper.GetUserNameByRequestParam();

			var c = LoadControl(BookmarkUserControlPath.BookmarkingUserControlPath);


			container.Body.Controls.Add(c);

			container.Header.Controls.Add(new Label() { Text = pageTitle });

			InitBreadcrumbs(pageTitle);

			Title = HeaderStringHelper.GetPageTitle(pageTitle, container.BreadCrumbs);


			//Actions and Navigation
			BookmarkingNavigationUtil.SetBookmarkingActionsAndNavigation(BookmarkingSideHolder.Controls);
			BookmarkingNavigationUtil.SetPopularTagsCloud(BookmarkingSideHolder.Controls);

			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = BookmarkingSettings.ModuleId;
		}

		protected override void InitBreadcrumbs(string pageTitle)
		{
			container.BreadCrumbs.Add(new BreadCrumb() { Caption = BookmarkingResource.PageTitle, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });

			container.BreadCrumbs.Add(new BreadCrumb() { Caption = pageTitle, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });
		}
	}
}