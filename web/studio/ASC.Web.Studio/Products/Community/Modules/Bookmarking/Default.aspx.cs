using System.Web.UI.WebControls;
using ASC.Web.Community.Bookmarking.Resources;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.Community.Bookmarking
{
	public partial class Bookmarking : BookmarkingBasePage
	{	
		
		protected override void PageLoad()
		{
			ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.AllBookmarks;

			var pageTitle = BookmarkingResource.PageTitle;

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
	}
}
