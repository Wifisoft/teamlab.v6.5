using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Studio;
using ASC.Web.Controls;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using System.Text;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Masters;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Community.Product;
using ASC.Bookmarking.Common;
using ASC.Web.Studio.Utility;
using ASC.Common.Data;
using System.Web.Configuration;
using ASC.Data.Storage;
using System.IO;

namespace ASC.Web.Community.Bookmarking.Util
{
	public abstract class BookmarkingBasePage : MainPage
	{
		protected Container container;

		protected BookmarkingServiceHelper ServiceHelper;

		/// <summary>
		/// Page_Load of the Page Controller pattern.
		/// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnpatterns/html/ImpPageController.asp
		/// </summary>
		protected void Page_Load(object sender, EventArgs e)
		{
			BookmarkingBusinessConstants.CommunityProductID = CommunityProduct.ID;

            string script = "<link href='"
				+ WebSkin.GetUserSkin().GetAbsoluteWebPath("products/community/modules/bookmarking/app_themes/<theme_folder>/css/bookmarkingstyle.css")
				+ "' rel='stylesheet' type='text/css' />";

			this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), Guid.NewGuid().ToString(), script);


			ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
			container = new Container() { Body = new PlaceHolder(), Header = new PlaceHolder() };

			IStudioMaster master = null;

			if (this.Master is IStudioMaster)
			{
				master = this.Master as IStudioMaster;
				master.ContentHolder.Controls.Add(container);
			}
			BookmarkingNavigationUtil.InitBookmarkingPage(container.Body.Controls);

			PageLoad();
		}

		protected abstract void PageLoad();

		protected virtual void InitBreadcrumbs(string pageTitle)
		{
			container.BreadCrumbs.Add(new BreadCrumb() { Caption = pageTitle, NavigationUrl = BookmarkingRequestConstants.BookmarkingPageName });

			var searchText = ServiceHelper.GetSearchText();
			if (!String.IsNullOrEmpty(searchText))
			{
				ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.SearchBookmarks;
			    var searchResults = HeaderStringHelper.GetHTMLSearchHeader(searchText);

				container.BreadCrumbs.Add(new BreadCrumb { Caption = searchResults, NavigationUrl = ServiceHelper.GetCurrentURL() });
				return;
			}
			
			searchText = ServiceHelper.GetSearchTag();
			if (!String.IsNullOrEmpty(searchText))
			{
				ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.SearchByTag;
				var searchResults = String.Format("{0} {1}", BookmarkingResource.TagBookmarks, searchText);

				container.BreadCrumbs.Add(new BreadCrumb() { Caption = BookmarkingResource.TagPageTitle, NavigationUrl = BookmarkingRequestConstants.TagsPageName });

				container.BreadCrumbs.Add(new BreadCrumb() { Caption = searchResults, NavigationUrl = ServiceHelper.GetCurrentURL() });
			}
		}
	}
}
