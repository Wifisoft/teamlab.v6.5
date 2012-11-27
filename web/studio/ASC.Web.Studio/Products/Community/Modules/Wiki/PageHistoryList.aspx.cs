using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Resources;
using WikiActivityPublisher = ASC.Web.UserControls.Wiki.WikiActivityPublisher;

namespace ASC.Web.Community.Wiki
{
    public partial class PageHistoryList : WikiBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as WikiMaster).GetNavigateActionsVisible += new WikiMaster.GetNavigateActionsVisibleHandle(PageHistoryList_GetNavigateActionsVisible);

            var page = Wiki.GetPage(PageNameUtil.Decode(WikiPage));           
            
            if (Request["page"] == null || page == null)
                Response.RedirectLC("Default.aspx", this);

            string PageName = page.PageName.Equals(string.Empty) ? WikiResource.MainWikiCaption : page.PageName;

            BreadCrumb.Add(new ASC.Web.Controls.BreadCrumb() { Caption = PageName, NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), page.PageName) });
            BreadCrumb.Add(new ASC.Web.Controls.BreadCrumb() { Caption = WikiResource.wikiHistoryCaption });

            if (!IsPostBack)
            {
                cmdDiff.Text = WikiResource.cmdDiff;
                cmdDiff_Top.Text = WikiResource.cmdDiff;
                BindHistoryList();
            }
        }

        protected void cmdRevert_Click(object sender, EventArgs e)
        {
            int ver;
            if (int.TryParse((sender as LinkButton).CommandName, out ver))
            {
                var page = Wiki.GetPage(PageNameUtil.Decode(WikiPage), ver);
                if(page != null)
                {
                    page.Date = TenantUtil.DateTimeNow();
                    page.UserID = SecurityContext.CurrentAccount.ID;
                    page.Version = Wiki.GetPageMaxVersion(page.PageName) + 1;

                    Wiki.SavePage(page);
                    Wiki.UpdateCategoriesByPageContent(page);
                    
                    WikiActivityPublisher.RevertPage(page);
                    BindHistoryList();
                }
            }
        }     

        protected new string GetPageViewLink(Page page)
        {
            return ActionHelper.GetViewPagePathWithVersion(this.ResolveUrlLC("Default.aspx"), page.PageName, page.Version);
        }

        protected new string GetPageName(Page page)
        {
            if (string.IsNullOrEmpty(page.PageName))
                return WikiResource.MainWikiCaption;
            return page.PageName;
        }

        protected string GetAuthor(Page page)
        {
            return CoreContext.UserManager.GetUsers(page.UserID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID);
        }

        protected string GetDate(Page page)
        {
            return string.Format("<span class=\"wikiDateTime\">{0}</span> {1}", page.Date.ToString("t"), page.Date.ToString("d"));
        }

        protected void cmdDiff_Click(object sender, EventArgs e)
        {
            RadioButton rbOldDiff, rbNewDiff;
            Literal litDiff;
            int oldVersion = 0, newVersion = 0;

            foreach (RepeaterItem item in rptPageHistory.Items)
            {
                if (oldVersion > 0 && newVersion > 0)
                    break;

                if(item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    rbOldDiff = (RadioButton)item.FindControl("rbOldDiff");
                    rbNewDiff = (RadioButton)item.FindControl("rbNewDiff");
                    litDiff = (Literal)item.FindControl("litDiff");

                    if (oldVersion == 0 && rbOldDiff.Checked)
                    {
                        oldVersion = Convert.ToInt32(litDiff.Text);
                    }

                    if (newVersion == 0 && rbNewDiff.Checked)
                    {
                        newVersion = Convert.ToInt32(litDiff.Text);
                    }
                }
            }

            Response.RedirectLC(string.Format(@"Diff.aspx?page={0}&ov={1}&nv={2}", WikiPage, oldVersion, newVersion), this);
        }


        private void BindHistoryList()
        {
            rptPageHistory.DataSource = Wiki.GetPageHistory(PageNameUtil.Decode(WikiPage));
            rptPageHistory.DataBind();
            cmdDiff.Visible = cmdDiff_Top.Visible = (rptPageHistory.DataSource as List<Page>).Count > 1;
        }

        WikiNavigationActionVisible PageHistoryList_GetNavigateActionsVisible()
        {
            return WikiNavigationActionVisible.AddNewPage;
        }
    }
}
