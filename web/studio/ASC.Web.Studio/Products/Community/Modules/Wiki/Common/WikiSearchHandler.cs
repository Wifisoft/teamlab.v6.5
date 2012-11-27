using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Web.UserControls.Wiki.Resources;
using ASC.Web.UserControls.Wiki.UC;

namespace ASC.Web.Community.Wiki.Common
{
    public class WikiSearchHandler : BaseSearchHandlerEx
    {
        private int TenantId
        {
            get { return TenantProvider.CurrentTenantID; }
        }

        #region ISearchHandler Members

        public override SearchResultItem[] Search(string text)
        {
            List<SearchResultItem> list = new List<SearchResultItem>();
            string rootPage = HttpContext.Current.Request.PhysicalApplicationPath.TrimEnd('\\');
            string defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);

            string pageName;


            foreach (Page page in new WikiEngine().SearchPagesByContent(text))
            {
                pageName = page.PageName;
                if (string.IsNullOrEmpty(pageName))
                {
                    pageName = WikiResource.MainWikiCaption;
                }

                list.Add(new SearchResultItem()
                {
                    Name = pageName,
                    Description = HtmlUtility.GetText(
                        EditPage.ConvertWikiToHtml(page.PageName, page.Body, defPageHref,
                            WikiSection.Section.ImageHangler.UrlFormat, TenantId), 120),
                    URL = ActionHelper.GetViewPagePath(defPageHref, page.PageName),
                    Date = page.Date
                });
            }
            return list.ToArray();
        }
       
        #endregion

        public override string AbsoluteSearchURL
        {
            get { return VirtualPathUtility.ToAbsolute(WikiManager.BaseVirtualPath + "/Search.aspx").ToLower(); }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions() { ImageFileName = "WikiLogo16.png", PartID = WikiManager.ModuleId }; }
        }

        public override string SearchName
        {
            get { return WikiManager.SearchDefaultString; }
        }

        public override Guid ModuleID
        {
            get { return WikiManager.ModuleId; }
        }

        public override string PlaceVirtualPath
        {
            get { return WikiManager.BaseVirtualPath; }
        }

        public override Guid ProductID
        {
            get { return CommunityProduct.ID; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
    }
}
