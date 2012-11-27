using System;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.Product;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.Community.Wiki
{
    public partial class Search : WikiBasePage
    {
        protected string searchContent
        {
            get { return Request["Search"]; }
        }

        protected bool PageNameOnly
        {
            get { return Request["pn"] != null; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as WikiMaster).GetNavigateActionsVisible += new WikiMaster.GetNavigateActionsVisibleHandle(Search_GetNavigateActionsVisible);

            BreadCrumb.Add(new BreadCrumb { Caption = WikiResource.wikiSearchCaption, NavigationUrl = this.ResolveUrlLC("Search.aspx") });
            if (!string.IsNullOrEmpty(searchContent))
            {
                BreadCrumb.Add(new BreadCrumb { Caption = HeaderStringHelper.GetHTMLSearchHeader(searchContent) });
            }

            if (!IsPostBack)
            {
                BindRepeater();
            }
        }

        WikiNavigationActionVisible Search_GetNavigateActionsVisible()
        {
            return WikiNavigationActionVisible.AddNewPage;
        }

        private void BindRepeater()
        {

            if (PageNameOnly)
            {
                rptPageList.DataSource = Wiki.SearchPagesByName(searchContent);
            }
            else
            {
                rptPageList.DataSource = Wiki.SearchPagesByContent(searchContent);
            }

            rptPageList.DataBind();
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var pageName = ((LinkButton)sender).CommandName;

                var page = Wiki.GetPage(pageName);
                CommunitySecurity.DemandPermissions(new WikiObjectsSecurityObject(page), Common.Constants.Action_RemovePage);

                foreach (var comment in Wiki.GetComments(pageName))
                {
                    CommonControlsConfigurer.FCKUploadsRemoveForItem("wiki_comments", comment.Id.ToString());
                }
                Wiki.RemovePage(pageName);
                BindRepeater();
            }
            catch (Exception err)
            {
                WikiMaster.PrintInfoMessage(err.Message, InfoType.Warning);
            }
        }

        protected new string GetPageName(Page page)
        {
            if (string.IsNullOrEmpty(page.PageName))
                return WikiResource.MainWikiCaption;
            return page.PageName;
        }

        protected new string GetPageViewLink(Page page)
        {
            return ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), page.PageName);
        }

        protected string GetPageEditLink(Page page)
        {
            return ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), page.PageName);
        }

        protected string GetPageInfo(Page page)
        {
            if (page.UserID.Equals(Guid.Empty))
            {
                return string.Empty;
            }

            return ProcessVersionInfo(page.PageName, page.UserID, page.Date, page.Version, false);
        }

        protected string GetAuthor(Page page)
        {
            return CoreContext.UserManager.GetUsers(page.UserID).RenderProfileLink(Product.CommunityProduct.ID);
        }

        protected string GetDate(Page page)
        {
            return string.Format("<span class=\"wikiDateTime\">{0}</span> {1}", page.Date.ToString("t"), page.Date.ToString("d"));
        }
    }
}