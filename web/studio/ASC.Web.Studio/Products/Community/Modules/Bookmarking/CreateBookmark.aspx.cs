using System.Web.UI;
using ASC.Bookmarking.Business.Permissions;
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
	public partial class CreateBookmark : BookmarkingBasePage
	{

		protected override void PageLoad()
		{
			if (!BookmarkingPermissionsCheck.PermissionCheckCreateBookmark())
			{
				Response.Redirect(BookmarkingRequestConstants.BookmarkingPageName);
			}			

			ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark;
				

			var c = LoadControl(BookmarkUserControlPath.CreateBookmarkUserControlPath) as CreateBookmarkUserControl;
			c.IsNewBookmark = true;
			container.Body.Controls.Add(c);

			var url = Request.QueryString[BookmarkingRequestConstants.UrlGetRequest];
			var s = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				s = string.Format(" getBookmarkUrlInput().val(\"{0}\"); getBookmarkByUrlButtonClick(); ", url);
			}

			string script = string.Format("jq(document).ready(function() {{ showAddBookmarkPanel(); {0} }} )", s);

			Page.ClientScript.RegisterClientScriptBlock(typeof(string), "createBookmarkScript", script, true);

			Title = HeaderStringHelper.GetPageTitle(BookmarkingResource.AddBookmarkLink, container.BreadCrumbs);
			InitBreadcrumbs(BookmarkingResource.AddBookmarkLink);

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
