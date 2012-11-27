using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Controls;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;


namespace ASC.Web.Community.Bookmarking
{
	public partial class Tags : BookmarkingBasePage
	{

		protected override void PageLoad()
		{
			InitTags();
			var pageTitle = BookmarkingResource.TagPageTitle;
			
			InitBreadcrumbs(pageTitle);
			Title = HeaderStringHelper.GetPageTitle(pageTitle, container.BreadCrumbs);

			//Actions and Navigation
            BookmarkingNavigationUtil.SetBookmarkingActionsAndNavigation(BookmarkingSideHolder.Controls);

            BookmarkingNavigationUtil.SetPopularTagsCloud(BookmarkingSideHolder.Controls);

			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = BookmarkingSettings.ModuleId;
		}

		private void InitTags()
		{
			var c = LoadControl(BookmarkUserControlPath.TagsUserControlPath);
			container.Body.Controls.Add(c);
		}

		protected override void InitBreadcrumbs(string pageTitle)
		{
			container.BreadCrumbs.Add(new BreadCrumb() { Caption = BookmarkingResource.PageTitle, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });
			container.BreadCrumbs.Add(new BreadCrumb() { Caption = pageTitle, NavigationUrl = BookmarkingRequestConstants.TagsPageName });
		}
		
	}
}
