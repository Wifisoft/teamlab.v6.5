using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Web.Community.Product;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.Controls;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Resources;
using WikiManager = ASC.Web.UserControls.Wiki.WikiManager;
using WikiNotifySource = ASC.Web.UserControls.Wiki.WikiNotifySource;

namespace ASC.Web.Community.Wiki
{
    [Flags]
    public enum WikiNavigationActionVisible
    {
        None = 0x000,
        AddNewPage = 0x001,
        UploadFile = 0x002,
        EditThePage = 0x004,
        ShowVersions = 0x008,
        PrintPage = 0x010,
        DeleteThePage = 0x020,
        SubscriptionOnNewPage = 0x040,
        SubscriptionThePage = 0x080,
        SubscriptionOnCategory = 0x100,
        CreateThePage = 0x200
    }


    [AjaxNamespace("MainWikiAjaxMaster")]
    public partial class WikiMaster : System.Web.UI.MasterPage
    {
        public delegate string GetDelUniqIdHandle();
        public event GetDelUniqIdHandle GetDelUniqId;

        public delegate WikiNavigationActionVisible GetNavigateActionsVisibleHandle();
        public event GetNavigateActionsVisibleHandle GetNavigateActionsVisible;

        static IDirectRecipient IAmAsRecipient
        {
            get
            {
                return (IDirectRecipient)WikiNotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType(), this.Page);



            InitActions();
            CheckBreadCrumbs();

            sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
            sideRecentActivity.ProductId = Product.CommunityProduct.ID;
            sideRecentActivity.ModuleId = WikiManager.ModuleId;
        }


        private int TenantId
        {
            get { return TenantProvider.CurrentTenantID; }
        }

        private void CheckBreadCrumbs()
        {
            if (Breadcrumbs.Count >= 1 && !Breadcrumbs[0].Caption.Equals(WikiResource.WikiBreadcrumbsModuleName, StringComparison.InvariantCultureIgnoreCase))
            {
                Breadcrumbs.Insert(0, new BreadCrumb() { Caption = WikiResource.WikiBreadcrumbsModuleName, NavigationUrl = this.ResolveUrlLC("Default.aspx") });
            }
        }

        public void PrintInfoMessage(string info, InfoType infoType)
        {
            MainWikiContainer.Options.InfoMessageText = info;
            MainWikiContainer.Options.InfoType = infoType;
        }

        protected string MasterResolveUrlLC(string url)
        {
            return this.ResolveUrlLC(url);
        }

        private void InitActions()
        {
            niMainPage.Name = WikiResource.menu_MainPage;
            niAllCategories.Name = WikiResource.menu_ListCategories;
            niAllPages.Name = WikiResource.menu_ListPages;
            niNewPages.Name = WikiResource.menu_NewPages;
            niFreshEditsPages.Name = WikiResource.menu_FreshEditPages;
            niAllFiles.Name = WikiResource.menu_ListFiles;
            niHelp.Name = WikiResource.menu_Help;

            string mainPath = this.ResolveUrlLC("Default.aspx");
            niMainPage.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalHomeKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));
            niAllCategories.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalCategoriesKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));
            niAllPages.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalIndexKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));
            niNewPages.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalNewPagesKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));
            niFreshEditsPages.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalRecentlyKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));
            niAllFiles.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalFilesKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));
            niHelp.URL = this.ResolveUrlLC(ActionHelper.GetViewPagePath(mainPath, ASC.Web.UserControls.Wiki.Constants.WikiInternalHelpKey, ASC.Web.UserControls.Wiki.Constants.WikiInternalKeyCaption));

            niaAddNewPage.Name = WikiResource.menu_AddNewPage;
            niaCreateThePage.Name = WikiResource.menu_CreateThePage;
            niaUploadFile.Name = WikiResource.menu_AddNewFile;
            niaEditThePage.Name = WikiResource.menu_EditThePage;
            niaShowVersions.Name = WikiResource.menu_ShowVersions;
            niaPrintPage.Name = WikiResource.menu_PrintThePage;
            niaDeleteThePage.Name = WikiResource.menu_DeleteThePage;

            if (SecurityContext.IsAuthenticated)
            {
                ASC.Web.Studio.Controls.Common.MenuItem item = GetNewPageSubscription();
                if (item == null)
                {
                    niaSubscriptionOnNewPage.Visible = false;
                }
                else
                {
                    NewsActions.Controls.AddAt(NewsActions.Controls.IndexOf(niaSubscriptionOnNewPage), item);
                    NewsActions.Controls.Remove(niaSubscriptionOnNewPage);
                    niaSubscriptionOnNewPage = item;
                }

                item = GetThePageSubscription();
                if (item == null)
                {
                    niaSubscriptionThePage.Visible = false;
                }
                else
                {
                    NewsActions.Controls.AddAt(NewsActions.Controls.IndexOf(niaSubscriptionThePage), item);
                    NewsActions.Controls.Remove(niaSubscriptionThePage);
                    niaSubscriptionThePage = item;
                }

                item = GetTheCategorySubscription();
                if (item == null)
                {
                    niaSubscriptionOnCategory.Visible = false;
                }
                else
                {
                    NewsActions.Controls.AddAt(NewsActions.Controls.IndexOf(niaSubscriptionOnCategory), item);
                    NewsActions.Controls.Remove(niaSubscriptionOnCategory);
                    niaSubscriptionOnCategory = item;
                }
            }
            else
            {
                NewsActions.Visible = false;
            }

            niaAddNewPage.URL = ActionHelper.GetAddPagePath(this.ResolveUrlLC("Default.aspx"));
            niaUploadFile.URL = "javascript:ShowUploadFileBox();";

            try
            {
                niaCreateThePage.URL = ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode((this.Page as WikiBasePage).WikiPage));
            }
            catch (SystemException)
            {
                niaCreateThePage.Visible = false;
            }

            try
            {
                niaEditThePage.URL = ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode((this.Page as WikiBasePage).WikiPage));
            }
            catch (SystemException)
            {
                niaEditThePage.Visible = false;
            }

            try
            {
                niaShowVersions.URL = ActionHelper.GetViewPagePath(this.ResolveUrlLC("PageHistoryList.aspx"), PageNameUtil.Decode((this.Page as WikiBasePage).WikiPage));
            }
            catch (SystemException)
            {
                niaShowVersions.Visible = false;
            }

            LinkButton cmdDelete = new LinkButton();

            niaPrintPage.URL = @"javascript:window.print();";
            string delUniqId = GetDeleteUniqueId();
            if (!string.IsNullOrEmpty(delUniqId))
            {
                niaDeleteThePage.URL = string.Format(@"javascript:if(confirm('{0}')) __doPostBack('{1}', '');", WikiResource.cfmDeletePage, delUniqId);
            }
            else
            {
                niaDeleteThePage.Visible = false;
            }

            cmdPageGoButton.Text = WikiResource.wiki_PageContGoButton;
            cmdPageGoButton.OnClientClick = string.Format(@"javascript: var txt =  document.getElementById('{0}'); if(txt.value.replace(/\s/g, '') == '') {{txt.focus(); return false;}} return true;",
                        txtSearchPage.ClientID);

            SetVisibleActions();

            //NotifyOnNews();
        }


        protected void cmdPageGoButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchPage.Text))
                return;
            /*
            Pages page = PagesProvider.PagesGetByName(txtSearchPage.Text, MapPath("~"));
                        if(page != null)
                        {
                            Response.RedirectLC(ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), page.PageName));
                        }
                        else
                        {
                            Response.RedirectLC(string.Format("Search.aspx?Search={0}", txtSearchPage.Text.Trim()));
                        }*/
            Response.RedirectLC(ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), txtSearchPage.Text), Page);
        }


        private string GetDeleteUniqueId()
        {
            if (GetDelUniqId == null)
                return string.Empty;

            return GetDelUniqId();
        }


        public void UpdateNavigationItems()
        {
            niaAddNewPage.Visible = niaCreateThePage.Visible = niaUploadFile.Visible = niaEditThePage.Visible =
            niaShowVersions.Visible = niaPrintPage.Visible = niaDeleteThePage.Visible = niaSubscriptionOnNewPage.Visible =
            niaSubscriptionThePage.Visible = niaSubscriptionOnCategory.Visible = true;
            InitActions();
        }


        private void SetVisibleActions()
        {
            WikiNavigationActionVisible result = WikiNavigationActionVisible.None;
            if (GetNavigateActionsVisible != null)
                result = GetNavigateActionsVisible();

            niaAddNewPage.Visible = niaAddNewPage.Visible && (result & WikiNavigationActionVisible.AddNewPage) > 0 && CommunitySecurity.CheckPermissions(ASC.Web.Community.Wiki.Common.Constants.Action_AddPage);
            niaCreateThePage.Visible = niaCreateThePage.Visible && (result & WikiNavigationActionVisible.CreateThePage) > 0 && CommunitySecurity.CheckPermissions(ASC.Web.Community.Wiki.Common.Constants.Action_AddPage);
            niaUploadFile.Visible = niaUploadFile.Visible && (result & WikiNavigationActionVisible.UploadFile) > 0 && CommunitySecurity.CheckPermissions(ASC.Web.Community.Wiki.Common.Constants.Action_UploadFile) && !ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);
            niaEditThePage.Visible = niaEditThePage.Visible && (result & WikiNavigationActionVisible.EditThePage) > 0;
            niaShowVersions.Visible = niaShowVersions.Visible && (result & WikiNavigationActionVisible.ShowVersions) > 0;
            niaPrintPage.Visible = niaPrintPage.Visible && (result & WikiNavigationActionVisible.PrintPage) > 0;
            niaDeleteThePage.Visible = niaDeleteThePage.Visible && (result & WikiNavigationActionVisible.DeleteThePage) > 0;
            niaSubscriptionOnNewPage.Visible = niaSubscriptionOnNewPage.Visible && (result & WikiNavigationActionVisible.SubscriptionOnNewPage) > 0;
            niaSubscriptionThePage.Visible = niaSubscriptionThePage.Visible && (result & WikiNavigationActionVisible.SubscriptionThePage) > 0;
            niaSubscriptionOnCategory.Visible = niaSubscriptionOnCategory.Visible && (result & WikiNavigationActionVisible.SubscriptionOnCategory) > 0;

            PanelManage.Visible = niaAddNewPage.Visible || niaCreateThePage.Visible || niaUploadFile.Visible || niaEditThePage.Visible ||
                niaShowVersions.Visible || niaPrintPage.Visible || niaDeleteThePage.Visible || niaSubscriptionOnNewPage.Visible ||
                niaSubscriptionThePage.Visible || niaSubscriptionOnCategory.Visible;

        }


        public List<BreadCrumb> Breadcrumbs
        {
            get
            {
                return MainWikiContainer.BreadCrumbs;
            }

        }


        private ASC.Web.Studio.Controls.Common.MenuItem GetNewPageSubscription()
        {
            if ((this.Page as WikiBasePage).IsFile)
                return null;

            StringBuilder sb = new StringBuilder();
            ISubscriptionProvider subscriptionProvider = WikiNotifySource.Instance.GetSubscriptionProvider();

            List<string> userList = new List<string>();
            if (IAmAsRecipient != null)
            {
                userList = new List<string>(
                    subscriptionProvider.GetSubscriptions(
                    ASC.Web.Community.Wiki.Common.Constants.NewPage,
                    IAmAsRecipient)
                    );
            }

            bool subscribed = userList.Contains(null);

            sb.Append("<div id=\"newwiki_notifies\">");

            sb.Append("<a id=\"notify_newwiki\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"javascript:void(0);\" />" + (!subscribed ? WikiResource.NotifyOnNewPage : WikiResource.UnNotifyOnNewPage) + "</a>");
            sb.Append("<script type=\"text/javascript\">");
            sb.AppendLine("var NotifyNewWikiUploads = " + subscribed.ToString().ToLower(CultureInfo.CurrentCulture) + ";");
            sb.AppendLine("document.getElementById('notify_newwiki').onclick = function()");
            sb.AppendLine("{AjaxPro.onLoading = function(b){if(b){jq('#newwiki_notifies').block();}else{jq('#newwiki_notifies').unblock();}};");
            sb.AppendLine("MainWikiAjaxMaster.SubscribeOnNewPage(NotifyNewWikiUploads, callbackNotifyNewWiki);");
            sb.AppendLine("}");
            sb.AppendLine("function callbackNotifyNewWiki(result){NotifyNewWikiUploads = result.value;");
            sb.AppendLine("if(!NotifyNewWikiUploads){jq('#notify_newwiki').html('" + WikiResource.NotifyOnNewPage.ReplaceSingleQuote() + "');} ");
            sb.AppendLine("else {jq('#notify_newwiki').html('" + WikiResource.UnNotifyOnNewPage.ReplaceSingleQuote() + "');} ");
            sb.AppendLine("}");
            sb.Append("</script>");

            sb.Append("</div>");

            return new HtmlMenuItem(sb.ToString());
        }

        private ASC.Web.Studio.Controls.Common.MenuItem GetThePageSubscription()
        {
            if ((this.Page as WikiBasePage).Action.Equals(ActionOnPage.AddNew) || (this.Page as WikiBasePage).Action.Equals(ActionOnPage.None))
                return null;

            StringBuilder sb = new StringBuilder();
            ISubscriptionProvider subscriptionProvider = WikiNotifySource.Instance.GetSubscriptionProvider();

            List<string> userList = new List<string>();
            if (IAmAsRecipient != null)
            {
                userList = new List<string>(
                    subscriptionProvider.GetSubscriptions(
                    ASC.Web.Community.Wiki.Common.Constants.EditPage,
                    IAmAsRecipient)
                    );
            }

            string pageName = (this.Page as WikiBasePage).WikiPage ?? string.Empty;
            bool subscribed = userList.Exists(s => string.Compare(s, pageName, StringComparison.InvariantCultureIgnoreCase) == 0 || (s == null && string.Empty.Equals(pageName)));

            sb = new StringBuilder();

            sb.Append("<div id=\"editwiki_notifies\">");

            sb.Append("<a id=\"notify_editwiki\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"javascript:void(0);\" />" + (!subscribed ? WikiResource.NotifyOnEditPage : WikiResource.UnNotifyOnEditPage) + "</a>");
            sb.Append("<script type=\"text/javascript\">");
            sb.AppendLine("var NotifyEditWikiUploads = " + subscribed.ToString().ToLower(CultureInfo.CurrentCulture) + ";");
            sb.AppendLine("var NotifyEditWikiId = '" + HttpUtility.HtmlEncode((this.Page as WikiBasePage).WikiPage).EscapeString() + "';");
            sb.AppendLine("document.getElementById('notify_editwiki').onclick = function()");
            sb.AppendLine("{AjaxPro.onLoading = function(b){if(b){jq('#editwiki_notifies').block();}else{jq('#editwiki_notifies').unblock();}};");
            sb.AppendLine("MainWikiAjaxMaster.SubscribeOnEditPage(NotifyEditWikiUploads, NotifyEditWikiId, callbackNotifyEditWiki);");
            sb.AppendLine("}");
            sb.AppendLine("function callbackNotifyEditWiki(result){NotifyEditWikiUploads = result.value;");
            sb.AppendLine("if(!NotifyEditWikiUploads){jq('#notify_editwiki').html('" + WikiResource.NotifyOnEditPage.ReplaceSingleQuote() + "');} ");
            sb.AppendLine("else {jq('#notify_editwiki').html('" + WikiResource.UnNotifyOnEditPage.ReplaceSingleQuote() + "');} ");
            sb.AppendLine("}");
            sb.Append("</script>");

            sb.Append("</div>");

            return new HtmlMenuItem(sb.ToString());
        }

        private ASC.Web.Studio.Controls.Common.MenuItem GetTheCategorySubscription()
        {
            if (!(Page is ListPages || Page is _Default))
                return null;

            string categoryName;
            if (Page is ListPages)
                categoryName = (Page as ListPages).categoryName;
            else
                categoryName = (Page as _Default).GetCategoryName();
            if (string.IsNullOrEmpty(categoryName))
                return null;

            StringBuilder sb = new StringBuilder();
            ISubscriptionProvider subscriptionProvider = WikiNotifySource.Instance.GetSubscriptionProvider();

            List<string> userList = new List<string>();
            if (IAmAsRecipient != null)
            {
                userList = new List<string>(
                    subscriptionProvider.GetSubscriptions(
                    ASC.Web.Community.Wiki.Common.Constants.AddPageToCat,
                    IAmAsRecipient)
                    );
            }

            bool subscribed = userList.Exists(s => s.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));

            sb = new StringBuilder();

            sb.Append("<div id=\"pagetocat_notifies\">");

            sb.Append("<a id=\"notify_pagetocat\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"javascript:void(0);\" />" + (!subscribed ? WikiResource.NotifyOnPageCat : WikiResource.UnNotifyOnPageCat) + "</a>");
            sb.Append("<script type=\"text/javascript\">");
            sb.AppendLine("var NotifyPageToCatUploads = " + subscribed.ToString().ToLower(CultureInfo.CurrentCulture) + ";");
            sb.AppendLine("var NotifyPageToCatId = '" + categoryName + "';");
            sb.AppendLine("document.getElementById('notify_pagetocat').onclick = function()");
            sb.AppendLine("{AjaxPro.onLoading = function(b){if(b){jq('#pagetocat_notifies').block();}else{jq('#pagetocat_notifies').unblock();}};");
            sb.AppendLine("MainWikiAjaxMaster.SubscribeOnPageToCat(NotifyPageToCatUploads, NotifyPageToCatId, callbackNotifyPageToCat);");
            sb.AppendLine("}");
            sb.AppendLine("function callbackNotifyPageToCat(result){NotifyPageToCatUploads = result.value;");
            sb.AppendLine("if(!NotifyPageToCatUploads){jq('#notify_pagetocat').html('" + WikiResource.NotifyOnPageCat + "');} ");
            sb.AppendLine("else {jq('#notify_pagetocat').html('" + WikiResource.UnNotifyOnPageCat + "');} ");
            sb.AppendLine("}");
            sb.Append("</script>");

            sb.Append("</div>");

            return new HtmlMenuItem(sb.ToString());
        }


        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SearchPages(string startPage)
        {
            var pages = new WikiEngine().SearchPagesByStartName(startPage);
            if (pages.Count == 0)
                return string.Empty;

            if (pages.Count > WikiManager.MaxPageSearchResult)
                pages = pages.GetRange(0, WikiManager.MaxPageSearchResult);
            StringBuilder sb = new StringBuilder();

            foreach (Page page in pages)
            {
                sb.Append(@"<div class=""seachHelpItem"" onclick=""javascript:MouseSelectSearchPages(this);"">");
                sb.Append(page.PageName);
                sb.Append(@"</div>");
            }

            return sb.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SubscribeOnNewPage(bool isSubscribe)
        {
            ISubscriptionProvider subscriptionProvider = WikiNotifySource.Instance.GetSubscriptionProvider();
            if (IAmAsRecipient == null)
            {
                return false;
            }
            if (!isSubscribe)
            {

                subscriptionProvider.Subscribe(
                         ASC.Web.Community.Wiki.Common.Constants.NewPage,
                         null,
                         IAmAsRecipient
                    );
                return true;
            }
            else
            {
                subscriptionProvider.UnSubscribe(
                         ASC.Web.Community.Wiki.Common.Constants.NewPage,
                         null,
                         IAmAsRecipient
                    );
                return false;
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SubscribeOnEditPage(bool isSubscribe, string pageName)
        {

            ISubscriptionProvider subscriptionProvider = WikiNotifySource.Instance.GetSubscriptionProvider();
            if (IAmAsRecipient == null)
            {
                return false;
            }
            if (!isSubscribe)
            {

                subscriptionProvider.Subscribe(
                         ASC.Web.Community.Wiki.Common.Constants.EditPage,
                         pageName,
                         IAmAsRecipient
                    );
                return true;
            }
            else
            {
                subscriptionProvider.UnSubscribe(
                         ASC.Web.Community.Wiki.Common.Constants.EditPage,
                         pageName,
                         IAmAsRecipient
                    );
                return false;
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SubscribeOnPageToCat(bool isSubscribe, string catName)
        {

            ISubscriptionProvider subscriptionProvider = WikiNotifySource.Instance.GetSubscriptionProvider();
            if (IAmAsRecipient == null)
            {
                return false;
            }
            if (!isSubscribe)
            {

                subscriptionProvider.Subscribe(
                         ASC.Web.Community.Wiki.Common.Constants.AddPageToCat,
                         catName,
                         IAmAsRecipient
                    );
                return true;
            }
            else
            {
                subscriptionProvider.UnSubscribe(
                         ASC.Web.Community.Wiki.Common.Constants.AddPageToCat,
                         catName,
                         IAmAsRecipient
                    );
                return false;
            }
        }
    }
}