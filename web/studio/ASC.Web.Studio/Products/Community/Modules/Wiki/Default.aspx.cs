using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Community.Product;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.Controls;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Web.UserControls.Wiki.Resources;
using ASC.Web.UserControls.Wiki.UC;

namespace ASC.Web.Community.Wiki
{
    [AjaxNamespace("_Default")]
    public partial class _Default : WikiBasePage, IContextInitializer
    {
        private bool isEmptyPage = false;

        protected int Version
        {
            get
            {
                int result;
                if (Request["ver"] == null || !int.TryParse(Request["ver"], out result))
                    return 0;

                return result;
            }
        }

        protected bool m_IsCategory
        {
            get { return Action == ActionOnPage.CategoryView || Action == ActionOnPage.CategoryEdit; }
        }

        private string _categoryName = null;

        protected string m_categoryName
        {
            get
            {
                if (_categoryName == null)
                {
                    _categoryName = string.Empty;
                    if (m_IsCategory)
                    {
                        _categoryName = PageNameUtil.Decode(WikiPage).Split(':')[1].Trim();
                    }
                }

                return _categoryName;
            }
        }

        protected string PrintPageName
        {
            get
            {
                string pageName = PageNameUtil.Decode(WikiPage);
                if (string.IsNullOrEmpty(pageName))
                {
                    pageName = WikiResource.MainWikiCaption;
                }
                return pageName;
            }
        }

        protected string PrintPageNameEncoded
        {
            get { return HttpUtility.HtmlEncode(PrintPageName); }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            wikiViewPage.TenantId = wikiViewFile.TenantId = wikiEditFile.TenantId = wikiEditPage.TenantId = TenantId;
        }

        private void CheckSpetialSymbols()
        {
            var spetialName = PrintPageName;
            if (!spetialName.Contains(":"))
                return;

            var spetial = spetialName.Split(':')[0];
            spetialName = spetialName.Split(':')[1];

            /*if (spetial.Equals(ASC.Web.UserControls.Wiki.Resources.WikiResource.wikiCategoryKeyCaption, StringComparison.InvariantCultureIgnoreCase))
            {
                Response.RedirectLC(string.Format("ListPages.aspx?cat={0}", spetialName.Trim()), this);
            }
            else*/
            if (spetial.Equals(UserControls.Wiki.Constants.WikiInternalKeyCaption, StringComparison.InvariantCultureIgnoreCase))
            {
                spetialName = spetialName.Trim();
                var anchors = spetialName;
                if (spetialName.Contains("#"))
                {
                    spetialName = spetialName.Split('#')[0];
                    anchors = anchors.Remove(0, spetialName.Length).TrimStart('#');
                }
                else
                {
                    anchors = string.Empty;
                }

                if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalIndexKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.RedirectLC("ListPages.aspx", this);
                }
                else if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalCategoriesKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.RedirectLC("ListCategories.aspx", this);
                }
                else if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalFilesKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.RedirectLC("ListFiles.aspx", this);
                }

                else if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalHomeKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(anchors))
                    {
                        Response.RedirectLC("Default.aspx", this);
                    }
                    else
                    {
                        Response.RedirectLC(string.Format(@"Default.aspx?page=#{0}", anchors), this);
                    }
                }
                else if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalNewPagesKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.RedirectLC("ListPages.aspx?n=", this);
                }
                else if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalRecentlyKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.RedirectLC("ListPages.aspx?f=", this);
                }
                else if (spetialName.Equals(UserControls.Wiki.Constants.WikiInternalHelpKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(anchors))
                    {
                        Response.RedirectLC(string.Format(@"Default.aspx?page={0}", UserControls.Wiki.Resources.WikiUCResource.HelpPageCaption), this);
                    }
                    else
                    {
                        Response.RedirectLC(string.Format(@"Default.aspx?page={0}#{1}", UserControls.Wiki.Resources.WikiUCResource.HelpPageCaption, anchors), this);
                    }
                }
                else
                {
                    return;
                }
            }
        }

        protected void wikiEditPage_SetNewFCKMode(bool isWysiwygDefault)
        {
            WikiModuleSettings.SetIsWysiwygDefault(isWysiwygDefault, SecurityContext.CurrentAccount.ID);
        }

        protected string wikiEditPage_GetUserFriendlySizeFormat(long size)
        {
            return GetFileLengthToString(size);
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(WikiPage) || IsFile) return;
                var pageName = PageNameUtil.Decode(WikiPage);

                var page = Wiki.GetPage(pageName);
                CommunitySecurity.DemandPermissions(new WikiObjectsSecurityObject(page), Common.Constants.Action_RemovePage);

                foreach (var cat in Wiki.GetCategoriesRemovedWithPage(pageName))
                {
                    WikiNotifySource.Instance.GetSubscriptionProvider().UnSubscribe(Common.Constants.AddPageToCat, cat.CategoryName);
                }

                Wiki.RemoveCategories(pageName);

                WikiNotifySource.Instance.GetSubscriptionProvider().UnSubscribe(Common.Constants.EditPage, pageName);

                foreach (var comment in Wiki.GetComments(pageName))
                {
                    CommonControlsConfigurer.FCKUploadsRemoveForItem("wiki_comments", comment.Id.ToString());
                }
                Wiki.RemovePage(pageName);

                Response.RedirectLC("Default.aspx", this);
            }
            catch (Exception err)
            {
                WikiMaster.PrintInfoMessage(err.Message, InfoType.Alert);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as WikiMaster).GetNavigateActionsVisible += new WikiMaster.GetNavigateActionsVisibleHandle(_Default_GetNavigateActionsVisible);
            (Master as WikiMaster).GetDelUniqId += new WikiMaster.GetDelUniqIdHandle(_Default_GetDelUniqId);

            Utility.RegisterTypeForAjax(typeof(_Default), Page);
            LoadViews();

            if (!IsPostBack)
            {

                if (IsFile)
                {
                    Response.RedirectLC(string.Format(WikiSection.Section.ImageHangler.UrlFormat, WikiPage, TenantId), this);
                }

                pCredits.Visible = Action.Equals(ActionOnPage.View) || Action.Equals(ActionOnPage.CategoryView);

                commentList.FCKBasePath = CommonControlsConfigurer.FCKEditorBasePath;

                CheckSpetialSymbols();

                wikiEditPage.mainPath = this.ResolveUrlLC("Default.aspx");
                InitEditsLink();

                var mainStudioCss = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath;

                wikiEditPage.CanUploadFiles = CommunitySecurity.CheckPermissions(Common.Constants.Action_UploadFile) && !Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);
                wikiEditPage.MainCssFile = mainStudioCss;
                wikiEditPage.AjaxProgressLoaderGif = WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif");
                wikiEditPage.PleaseWaitMessage = WikiResource.PleaseWaitMessage;

                if (Action == ActionOnPage.CategoryView)
                {
                    BindPagesByCategory();
                }
            }
        }

        //protected void cmdInternal_Click(object sende, EventArgs e)
        //{
        //    List<Pages> pages = PagesProvider.PagesGetAll(TenantId, true);
        //    foreach (Pages p in pages)
        //    {
        //        PagesProvider.UpdateCategoriesByPageContent(p, TenantId);
        //    }

        //}

        //UpdateEditDeleteVisible(_wikiObjOwner);

        private IWikiObjectOwner _wikiObjOwner = null;

        protected void wikiViewPage_WikiPageLoaded(bool isNew, IWikiObjectOwner owner)
        {
            _wikiObjOwner = owner;
            wikiViewPage.CanEditPage = CommunitySecurity.CheckPermissions(new WikiObjectsSecurityObject(_wikiObjOwner), Common.Constants.Action_EditPage);
            UpdateEditDeleteVisible(owner);
            WikiMaster.UpdateNavigationItems();
        }

        protected void wikiEditPage_WikiPageLoaded(bool isNew, IWikiObjectOwner owner)
        {
            if (!isNew)
            {
                _wikiObjOwner = owner;
            }

            if ((isNew && !CommunitySecurity.CheckPermissions(Common.Constants.Action_AddPage))
                ||
                (!isNew && !(CommunitySecurity.CheckPermissions(new WikiObjectsSecurityObject(owner), Common.Constants.Action_EditPage))))
            {
                Response.RedirectLC("Default.aspx", this);
            }
        }

        protected void wikiEditPage_SaveNewCategoriesAdded(object sender, List<string> categories, string pageName)
        {
            //var authorId = SecurityContext.CurrentAccount.ID.ToString();
            //foreach (var catName in categories)
            //{
            //    WikiNotifyClient.SendNoticeAsync(
            //        authorId,
            //        Common.Constants.AddPageToCat,
            //        catName,
            //        null,
            //        GetListOfTagValForCategoryNotify(catName, pageName));
            //}
        }

        private string _Default_GetDelUniqId()
        {
            return cmdDelete.UniqueID;
        }

        private WikiNavigationActionVisible _Default_GetNavigateActionsVisible()
        {
            var result = WikiNavigationActionVisible.None;
            if ((Action.Equals(ActionOnPage.View) || Action.Equals(ActionOnPage.CategoryView)) && !IsFile)
            {
                result = WikiNavigationActionVisible.AddNewPage |
                         WikiNavigationActionVisible.ShowVersions | WikiNavigationActionVisible.PrintPage |
                         WikiNavigationActionVisible.SubscriptionOnNewPage | WikiNavigationActionVisible.SubscriptionThePage;
                if (_wikiObjOwner != null && CommunitySecurity.CheckPermissions(new WikiObjectsSecurityObject(_wikiObjOwner), Common.Constants.Action_EditPage))
                {
                    result |= WikiNavigationActionVisible.EditThePage;
                }
                if (!string.IsNullOrEmpty(WikiPage) && _wikiObjOwner != null && CommunitySecurity.CheckPermissions(new WikiObjectsSecurityObject(_wikiObjOwner), Common.Constants.Action_RemovePage))
                {
                    result |= WikiNavigationActionVisible.DeleteThePage;
                }

                if (pPageIsNotExists.Visible)
                {
                    result = WikiNavigationActionVisible.CreateThePage;
                }
            }
            else if (Action.Equals(ActionOnPage.AddNew))
            {
                result = WikiNavigationActionVisible.AddNewPage;
            }
            else if ((Action.Equals(ActionOnPage.Edit) || Action.Equals(ActionOnPage.CategoryEdit)) && !IsFile)
            {
                result = WikiNavigationActionVisible.AddNewPage;
            }

            if (Action.Equals(ActionOnPage.CategoryView) || Action.Equals(ActionOnPage.CategoryEdit))
            {
                result |= WikiNavigationActionVisible.SubscriptionOnCategory;
            }

            return result;
        }

        internal string GetCategoryName()
        {
            return m_categoryName;
        }

        protected void OnPageEmpty(object sender, EventArgs e)
        {
            var pageName = PageNameUtil.Decode(WikiPage);

            wikiViewPage.Visible = false;
            wikiEditPage.Visible = false;
            wikiViewFile.Visible = false;
            wikiEditFile.Visible = false;
            pPageIsNotExists.Visible = true;

            if (!(Action.Equals(ActionOnPage.CategoryView) || Action.Equals(ActionOnPage.CategoryEdit)))
            {
                if (IsFile)
                {
                    txtPageEmptyLabel.Text = PrepereEmptyString(WikiResource.MainWikiFileIsNotExists, true, false);
                }
                else
                {
                    if (Wiki.SearchPagesByName(pageName).Count > 0)
                    {
                        txtPageEmptyLabel.Text = PrepereEmptyString(WikiResource.MainWikiPageIsNotExists, false, true);
                    }
                    else
                    {
                        txtPageEmptyLabel.Text = PrepereEmptyString(WikiResource.MainWikiPageIsNotExists, false, false);
                    }
                }

                PageHeaderText = pageName;
            }

            isEmptyPage = true;
            InitEditsLink();
            WikiMaster.UpdateNavigationItems();
        }

        private string PrepereEmptyString(string format, bool isFile, bool isSearchResultExists)
        {
            commentList.Visible = false;
            var mainOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
            var rxLinkCreatePlace = new Regex(@"{0([\s\S]+?)}", mainOptions);
            var rxLinkSearchResult = new Regex(@"{1([\s\S]+?)}", mainOptions);
            var rxSearchResultParth = new Regex(@"\[\[([\s\S]+?)\]\]", mainOptions);
            var result = format;

            foreach (Match match in rxLinkCreatePlace.Matches(format))
            {
                if (isFile)
                {
                    if (CommunitySecurity.CheckPermissions(Common.Constants.Action_UploadFile) && !Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
                    {
                        result = result.Replace(match.Value, string.Format(@"<a href=""{0}"">{1}</a>", ActionHelper.GetEditFilePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)), match.Groups[1].Value));
                    }
                }
                else
                {
                    if (CommunitySecurity.CheckPermissions(Common.Constants.Action_AddPage))
                    {
                        result = result.Replace(match.Value, string.Format(@"<a href=""{0}"">{1}</a>", ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)), match.Groups[1].Value));
                    }
                    else
                    {
                        result = result.Replace(match.Value, match.Groups[1].Value);
                    }
                }
            }

            if (isSearchResultExists && !isFile)
            {
                result = rxSearchResultParth.Replace(result, SearchResultParthMatchEvaluator);

                foreach (Match match in rxLinkSearchResult.Matches(format))
                {
                    result = result.Replace(match.Value, string.Format(@"<a href=""{0}"">{1}</a>", this.ResolveUrlLC(string.Format("Search.aspx?Search={0}&pn=", HttpUtility.UrlEncode(PageNameUtil.Decode(WikiPage)))), match.Groups[1].Value));
                }
            }
            else
            {
                result = rxSearchResultParth.Replace(result, string.Empty);
            }

            return result;
        }

        private string SearchResultParthMatchEvaluator(Match match)
        {
            return match.Groups[1].Value;
        }

        private string GetAbsolutePath(string relative)
        {
            return string.Format(@"{0}://{1}{2}{3}",
                                 Request.GetUrlRewriter().Scheme,
                                 Request.GetUrlRewriter().Host,
                                 (Request.GetUrlRewriter().Port != 80 ? string.Format(":{0}", Request.GetUrlRewriter().Port) : string.Empty),
                                 this.ResolveUrlLC(relative));
        }

        private void LoadViews()
        {

            wikiEditPage.AlaxUploaderPath = GetAbsolutePath("~/js/ajaxupload.3.5.js");
            wikiEditPage.JQPath = GetAbsolutePath("~/js/auto/jquery_full.js");

            wikiEditPage.CurrentUserId = SecurityContext.CurrentAccount.ID;
            wikiViewPage.Visible = false;
            wikiEditPage.Visible = false;
            wikiViewFile.Visible = false;
            wikiEditFile.Visible = false;
            pPageIsNotExists.Visible = false;
            pView.Visible = false;
            PrintHeader.Visible = false;
            phCategoryResult.Visible = Action == ActionOnPage.CategoryView;

            var pageName = PrintPageName;

            switch (Action)
            {
                case ActionOnPage.AddNew:
                    pageName = WikiResource.MainWikiAddNewPage;
                    wikiEditPage.IsWysiwygDefault = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context) ? false : WikiModuleSettings.GetIsWysiwygDefault(SecurityContext.CurrentAccount.ID);
                    wikiEditPage.Visible = true;
                    wikiEditPage.IsNew = true;
                    PageHeaderText = pageName;
                    break;
                case ActionOnPage.AddNewFile:
                    pageName = WikiResource.MainWikiAddNewFile;
                    wikiEditFile.Visible = true;
                    PageHeaderText = pageName;
                    break;
                case ActionOnPage.Edit:
                case ActionOnPage.CategoryEdit:
                    if (IsFile)
                    {
                        wikiEditFile.FileName = WikiPage;
                        wikiEditFile.Visible = true;
                        BreadCrumb.Clear();
                        BreadCrumb.Add(new BreadCrumb() { Caption = pageName, NavigationUrl = ActionHelper.GetViewFilePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)) });
                        BreadCrumb.Add(new BreadCrumb() { Caption = WikiResource.MainWikiEditFile });
                    }
                    else
                    {
                        wikiEditPage.PageName = WikiPage;
                        wikiEditPage.IsWysiwygDefault = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context) ? false : WikiModuleSettings.GetIsWysiwygDefault(SecurityContext.CurrentAccount.ID);
                        //wikiEditPage.IsWysiwygDefault = WikiModuleSettings.GetIsWysiwygDefault(SecurityContext.CurrentAccount.ID);
                        wikiEditPage.Visible = true;
                        if (m_IsCategory)
                            wikiEditPage.IsSpecialName = true;

                        BreadCrumb.Clear();
                        if (m_IsCategory)
                        {
                            BreadCrumb.Add(new BreadCrumb() { Caption = WikiResource.menu_ListCategories, NavigationUrl = this.ResolveUrlLC("ListCategories.aspx") });
                            BreadCrumb.Add(new BreadCrumb() { Caption = string.Format(WikiResource.menu_ListPagesCatFormat, m_categoryName), NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), m_categoryName, ASC.Web.UserControls.Wiki.Constants.WikiCategoryKeyCaption) });
                        }
                        else
                        {
                            BreadCrumb.Add(new BreadCrumb() { Caption = pageName, NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)) });
                        }
                        BreadCrumb.Add(new BreadCrumb() { Caption = WikiResource.MainWikiEditPage });
                        break;
                    }
                    break;
                case ActionOnPage.View:
                case ActionOnPage.CategoryView:
                    pView.Visible = true;
                    if (IsFile)
                    {
                        wikiViewFile.FileName = WikiPage;
                        PageHeaderText = pageName;
                        wikiViewFile.Visible = true;
                    }
                    else
                    {
                        PrintHeader.Visible = true;
                        wikiViewPage.PageName = WikiPage;
                        wikiViewPage.Version = Version;
                        if (Version == 0)
                        {
                            if (m_IsCategory)
                            {
                                BreadCrumb.Add(new BreadCrumb { Caption = WikiResource.menu_ListCategories, NavigationUrl = this.ResolveUrlLC("ListCategories.aspx") });
                                BreadCrumb.Add(new BreadCrumb { Caption = string.Format(WikiResource.menu_ListPagesCatFormat, m_categoryName) });
                            }
                            else
                            {
                                PageHeaderText = pageName;
                            }
                        }
                        else
                        {
                            BreadCrumb.Clear();
                            if (m_IsCategory)
                            {
                                BreadCrumb.Add(new BreadCrumb { Caption = WikiResource.menu_ListCategories, NavigationUrl = this.ResolveUrlLC("ListCategories.aspx") });
                                BreadCrumb.Add(new BreadCrumb { Caption = string.Format(WikiResource.menu_ListPagesCatFormat, m_categoryName), NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), m_categoryName, ASC.Web.UserControls.Wiki.Constants.WikiCategoryKeyCaption) });
                            }
                            else
                            {
                                BreadCrumb.Add(new BreadCrumb { Caption = pageName, NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)) });
                            }

                            BreadCrumb.Add(new BreadCrumb { Caption = WikiResource.wikiHistoryCaption, NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("PageHistoryList.aspx"), PageNameUtil.Decode(WikiPage)) });
                            BreadCrumb.Add(new BreadCrumb { Caption = string.Format("{0}{1}", WikiResource.wikiVersionCaption, Version) });
                        }
                        wikiViewPage.Visible = true;
                    }
                    InitCommentsView();
                    break;
            }
        }

        protected void BindPagesByCategory()
        {
            if (Action != ActionOnPage.CategoryView || string.IsNullOrEmpty(m_categoryName))
                return;

            var result = Wiki.GetPages(m_categoryName);

            result.RemoveAll(pemp => string.IsNullOrEmpty(pemp.PageName));

            string firstLetter;
            var letters = new List<string>(WikiResource.wikiCategoryAlfaList.Split(','));

            string otherSymbol = string.Empty;
            if (letters.Count > 0)
            {
                otherSymbol = letters[0];
                letters.Remove(otherSymbol);
            }

            var dictList = new List<PageDictionary>();
            PageDictionary pageDic;
            foreach (var page in result)
            {
                firstLetter = new string(page.PageName[0], 1);

                if (!letters.Exists(lt => lt.Equals(firstLetter, StringComparison.InvariantCultureIgnoreCase)))
                {
                    firstLetter = otherSymbol;
                }

                if (!dictList.Exists(dl => dl.HeadName.Equals(firstLetter, StringComparison.InvariantCultureIgnoreCase)))
                {
                    pageDic = new PageDictionary { HeadName = firstLetter };
                    pageDic.Pages.Add(page);
                    dictList.Add(pageDic);
                }
                else
                {
                    pageDic = dictList.Find(dl => dl.HeadName.Equals(firstLetter, StringComparison.InvariantCultureIgnoreCase));
                    pageDic.Pages.Add(page);
                }
            }

            dictList.Sort(SortPageDict);

            var countAll = dictList.Count * 3 + result.Count; //1 letter is like 2 links to category
            var perColumn = (int)(Math.Round((decimal)countAll / 3));

            var mainDictList = new List<List<PageDictionary>>();

            int index = 0, lastIndex = 0, count = 0;

            PageDictionary p;
            for (var i = 0; i < dictList.Count; i++)
            {
                p = dictList[i];

                count += 3;
                count += p.Pages.Count;
                index++;
                if (count >= perColumn || i == dictList.Count - 1)
                {
                    count = count - perColumn;
                    mainDictList.Add(dictList.GetRange(lastIndex, index - lastIndex));
                    lastIndex = index;
                }
            }

            rptCategoryPageList.DataSource = mainDictList;
            rptCategoryPageList.DataBind();
        }


        private int SortPageDict(PageDictionary cd1, PageDictionary cd2)
        {
            return cd1.HeadName.CompareTo(cd2.HeadName);
        }

        protected void On_PublishVersionInfo(object sender, VersionEventArgs e)
        {
            if (!e.UserID.Equals(Guid.Empty))
            {
                litAuthorInfo.Text = GetPageInfo(PageNameUtil.Decode(WikiPage), e.UserID, e.Date);
            }
            else
            {
                litAuthorInfo.Text = string.Empty;
            }

            hlVersionPage.Text = string.Format(WikiResource.cmdVersionTemplate, e.Version);
            hlVersionPage.NavigateUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("PageHistoryList.aspx"), PageNameUtil.Decode(WikiPage));
            hlVersionPage.Visible = Action.Equals(ActionOnPage.View) || Action.Equals(ActionOnPage.CategoryView);
            litVersionSeparator.Visible = hlEditPage.Visible;
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            if (!IsFile)
            {
                Response.RedirectLC(ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)), this);
            }
            else
            {
                Response.RedirectLC(ActionHelper.GetViewFilePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)), this);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            SaveResult result;
            string pageName;
            if (IsFile || Action.Equals(ActionOnPage.AddNewFile))
            {
                result = wikiEditFile.Save(SecurityContext.CurrentAccount.ID, out pageName);
            }
            else
            {
                result = wikiEditPage.Save(SecurityContext.CurrentAccount.ID, out pageName);
            }

            PrintResultBySave(result, pageName);
            if (result == SaveResult.OkPageRename)
            {
                //Redirect
                Response.RedirectLC(ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(pageName)), this);
            }
        }

        private void PrintInfoMessage(string info, InfoType type)
        {
            WikiMaster.PrintInfoMessage(info, type);
        }

        private void PrintResultBySave(SaveResult result, string pageName)
        {
            var infoType = InfoType.Info;
            var authorId = SecurityContext.CurrentAccount.ID.ToString();
            if (!result.Equals(SaveResult.Ok) && !result.Equals(SaveResult.NoChanges))
            {
                infoType = InfoType.Alert;
            }

            switch (result)
            {
                case SaveResult.SectionUpdate:
                    //WikiNotifyClient.SendNoticeAsync(
                    //    authorId,
                    //    Common.Constants.EditPage,
                    //    pageName,
                    //    null,
                    //    GetListOfTagValForNotify(pageName, "edit wiki page", null));
                    Response.RedirectLC(ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), pageName), this);
                    break;
                case SaveResult.OkPageRename:
                case SaveResult.Ok:
                    PrintInfoMessage(WikiResource.msgSaveSucess, infoType);
                    if (Action.Equals(ActionOnPage.AddNew))
                    {

                        //WikiNotifyClient.SendNoticeAsync(
                        //    authorId,
                        //    Common.Constants.NewPage,
                        //    null,
                        //    null,
                        //    GetListOfTagValForNotify(pageName));

                        //WikiActivityPublisher.AddPage(Wiki.GetPage(pageName));

                        Response.RedirectLC(ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), pageName), this);

                    }
                    else if (Action.Equals(ActionOnPage.AddNewFile))
                    {
                        //WikiActivityPublisher.AddFile(Wiki.GetFile(pageName));
                        Response.RedirectLC(ActionHelper.GetEditFilePath(this.ResolveUrlLC("Default.aspx"), pageName), this);
                    }
                    //else if (!IsFile)
                    //{
                    //    bool isNewPage = !WikiActivityPublisher.EditPage(Wiki.GetPage(pageName));

                    //    if (isNewPage)
                    //    {
                    //        WikiNotifyClient.SendNoticeAsync(
                    //            authorId,
                    //            Common.Constants.NewPage,
                    //            null,
                    //            null,
                    //            GetListOfTagValForNotify(pageName));
                    //    }
                    //    else
                    //    {
                    //        WikiNotifyClient.SendNoticeAsync(
                    //            authorId,
                    //            Common.Constants.EditPage,
                    //            pageName,
                    //            null,
                    //            GetListOfTagValForNotify(pageName, "edit wiki page", null));
                    //    }

                    //}
                    break;
                case SaveResult.FileEmpty:
                    PrintInfoMessage(WikiResource.msgFileEmpty, infoType);
                    break;
                case SaveResult.FileSizeExceeded:
                    PrintInfoMessage(WikiResource.msgFileSizeExceeded, infoType);
                    break;
                case SaveResult.NoChanges:
                    PrintInfoMessage(WikiResource.msgNoChanges, infoType);
                    break;
                case SaveResult.PageNameIsEmpty:
                    PrintInfoMessage(WikiResource.msgPageNameEmpty, infoType);
                    break;
                case SaveResult.PageNameIsIncorrect:
                    PrintInfoMessage(WikiResource.msgPageNameIncorrect, infoType);
                    break;
                case SaveResult.SamePageExists:
                    PrintInfoMessage(WikiResource.msgSamePageExists, infoType);
                    break;
                case SaveResult.UserIdIsEmpty:
                    PrintInfoMessage(WikiResource.msgInternalError, infoType);
                    break;
                case SaveResult.OldVersion:
                    PrintInfoMessage(WikiResource.msgOldVersion, infoType);
                    break;
                case SaveResult.Error:
                    PrintInfoMessage(WikiResource.msgMarkupError, InfoType.Alert);
                    break;
            }
        }

        //private ITagValue[] GetListOfTagValForNotify(string objectID)
        //{
        //    return GetListOfTagValForNotify(objectID, null, null);
        //}

        //private ITagValue[] GetListOfTagValForCategoryNotify(string objectID, string pageName)
        //{
        //    var values = new List<ITagValue>();

        //    var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

        //    var defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);
        //    var page = Wiki.GetPage(pageName);

        //    if (page != null)
        //    {

        //        values.Add(new TagValue(Common.Constants.TagPageName, page.PageName));
        //        values.Add(new TagValue(Common.Constants.TagURL, ActionHelper.GetViewPagePath(defPageHref, page.PageName)));
        //        values.Add(new TagValue(Common.Constants.TagUserName, user.DisplayUserName()));
        //        values.Add(new TagValue(Common.Constants.TagUserURL, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(user.ID, CommonLinkUtility.GetProductID()))));
        //        values.Add(new TagValue(Common.Constants.TagDate, TenantUtil.DateTimeNow()));
        //        values.Add(new TagValue(Common.Constants.TagPostPreview, HtmlUtility.GetText(EditPage.ConvertWikiToHtml(page.PageName, page.Body, defPageHref,
        //                                                                                                                WikiSection.Section.ImageHangler.UrlFormat, TenantId), 120)));
        //        values.Add(new TagValue(Common.Constants.TagCatName, objectID));
        //    }

        //    return values.ToArray();
        //}

        //private ITagValue[] GetListOfTagValForNotify(string objectID, string patternType, string commentBody)
        //{
        //    var values = new List<ITagValue>();
        //    var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

        //    var defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);

        //    var page = Wiki.GetPage(objectID);

        //    if (page != null)
        //    {

        //        values.Add(new TagValue(Common.Constants.TagPageName, String.IsNullOrEmpty(page.PageName) ? WikiResource.MainWikiCaption : page.PageName));
        //        values.Add(new TagValue(Common.Constants.TagURL, CommonLinkUtility.GetFullAbsolutePath(ActionHelper.GetViewPagePath(defPageHref, page.PageName))));
        //        values.Add(new TagValue(Common.Constants.TagUserName, user.DisplayUserName()));
        //        values.Add(new TagValue(Common.Constants.TagUserURL, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(user.ID, CommonLinkUtility.GetProductID()))));
        //        values.Add(new TagValue(Common.Constants.TagDate, TenantUtil.DateTimeNow()));
        //        values.Add(new TagValue(Common.Constants.TagPostPreview, HtmlUtility.GetText(EditPage.ConvertWikiToHtml(page.PageName, page.Body, defPageHref,
        //                                                                                                                WikiSection.Section.ImageHangler.UrlFormat, TenantId), 120)));
        //        if (!string.IsNullOrEmpty(patternType))
        //        {
        //            values.Add(new TagValue(Common.Constants.TagChangePageType, patternType));
        //        }

        //        if (!string.IsNullOrEmpty(commentBody))
        //        {
        //            values.Add(new TagValue(Common.Constants.TagCommentBody, commentBody));
        //        }
        //    }

        //    return values.ToArray();
        //}

        private void InitEditsLink()
        {
            hlEditPage.Text = WikiResource.cmdEdit;
            cmdSave.Text = WikiResource.cmdPublish;
            hlPreview.Text = WikiResource.cmdPreview;
            hlPreview.Attributes["onclick"] = string.Format("{0}();return false;", wikiEditPage.GetShowPrevFunctionName());
            //hlPreview.NavigateUrl = string.Format("javascript:{0}();", wikiEditPage.GetShowPrevFunctionName());
            hlPreview.NavigateUrl = string.Format("javascript:void(0);");
            cmdCancel.Text = WikiResource.cmdCancel;
            cmdCancel.Attributes["name"] = wikiEditPage.WikiFckClientId;
            cmdDelete.Text = WikiResource.cmdDelete;
            cmdDelete.OnClientClick = string.Format("javascript:return confirm(\"{0}\");", WikiResource.cfmDeletePage);

            hlPreview.Visible = Action.Equals(ActionOnPage.AddNew) || Action.Equals(ActionOnPage.Edit) || Action.Equals(ActionOnPage.CategoryEdit);

            if (IsFile)
            {
                hlEditPage.NavigateUrl = ActionHelper.GetEditFilePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage));
            }
            else
            {
                hlEditPage.NavigateUrl = ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage));
            }

            if (isEmptyPage)
            {
                hlEditPage.Visible = pEditButtons.Visible = false;
                cmdDelete.Visible = false;
                if (Action.Equals(ActionOnPage.CategoryView))
                {
                    hlEditPage.Visible = true;
                }
            }
            else
            {
                UpdateEditDeleteVisible(_wikiObjOwner);
            }

            litVersionSeparatorDel.Visible = cmdDelete.Visible;
        }

        private void UpdateEditDeleteVisible(IWikiObjectOwner obj)
        {
            var canEdit = false;
            var canDelete = false;
            var editVisible = Action.Equals(ActionOnPage.View) || Action.Equals(ActionOnPage.CategoryView);
            if (obj != null)
            {
                var secObj = new WikiObjectsSecurityObject(obj);
                canEdit = CommunitySecurity.CheckPermissions(secObj, Common.Constants.Action_EditPage);
                canDelete = CommunitySecurity.CheckPermissions(secObj, Common.Constants.Action_RemovePage) &&
                            !string.IsNullOrEmpty(obj.GetObjectId().ToString());
            }

            pEditButtons.Visible = !editVisible;
            hlEditPage.Visible = editVisible && canEdit;

            if (Version > 0 && (Action.Equals(ActionOnPage.View) || Action.Equals(ActionOnPage.CategoryView)))
            {
                hlEditPage.Visible = pEditButtons.Visible = false;
            }

            cmdDelete.Visible = editVisible && canDelete;
            litVersionSeparatorDel.Visible = cmdDelete.Visible;
        }

        #region Comments Functions

        private void InitCommentsView()
        {
            if (m_IsCategory) return;

            var totalCount = 0;
            var pageName = PageNameUtil.Decode(WikiPage);
            commentList.Visible = true;

            commentList.Items = GetCommentsList(pageName, out totalCount);
            ConfigureComments(commentList, pageName);
            commentList.TotalCount = totalCount;
            commentList.CommentsCountTitle = commentList.TotalCount.ToString(CultureInfo.CurrentCulture);
        }

        private IList<CommentInfo> GetCommentsList(string pageName, out int totalCount)
        {
            var comments = Wiki.GetComments(pageName);
            totalCount = comments.Count;

            var result = new List<CommentInfo>();
            var dic = new Dictionary<Guid, CommentInfo>();

            foreach (var c in comments.OrderBy(c => c.Date))
            {
                var comment = GetCommentInfo(c);
                dic[c.Id] = comment;
                if (c.ParentId == Guid.Empty)
                {
                    result.Add(comment);
                }
                else
                {
                    CommentInfo parent;
                    if (dic.TryGetValue(c.ParentId, out parent))
                    {
                        if (parent.CommentList == null) parent.CommentList = new List<CommentInfo>();
                        parent.CommentList.Add(comment);
                    }
                }
            }

            return result;
        }

        private void ConfigureComments(CommentsList commentList, string pageName)
        {

            CommonControlsConfigurer.CommentsConfigure(commentList);

            commentList.Simple = false;
            commentList.BehaviorID = "_commentsWikiObj";

            commentList.IsShowAddCommentBtn = CommunitySecurity.CheckPermissions(Common.Constants.Action_AddComment);

            commentList.JavaScriptAddCommentFunctionName = "_Default.AddComment";
            commentList.JavaScriptPreviewCommentFunctionName = "_Default.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "_Default.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "_Default.UpdateComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "_Default.LoadCommentText";
            commentList.FckDomainName = "wiki_comments";

            commentList.ObjectID = pageName;
        }

        public CommentInfo GetPrevHTMLComment(string text, string commentId)
        {
            var comment = !string.IsNullOrEmpty(commentId) ? Wiki.GetComment(new Guid(commentId)) : new Comment();
            comment.Date = TenantUtil.DateTimeNow();
            comment.UserId = SecurityContext.CurrentAccount.ID;
            comment.Body = text;

            var info = GetCommentInfo(comment);
            info.IsEditPermissions = false;
            info.IsResponsePermissions = false;
            return info;
        }

        public CommentInfo GetCommentInfo(Comment comment)
        {
            var info = new CommentInfo
                           {
                               CommentID = comment.Id.ToString(),
                               UserID = comment.UserId,
                               TimeStamp = comment.Date,
                               TimeStampStr = comment.Date.Ago(),
                               IsRead = true,
                               Inactive = comment.Inactive,
                               CommentBody = comment.Body,
                               UserFullName = DisplayUserSettings.GetFullUserName(comment.UserId),
                               UserAvatar = GetHtmlImgUserAvatar(comment.UserId),
                               IsEditPermissions = CommunitySecurity.CheckPermissions(new WikiObjectsSecurityObject(comment), Common.Constants.Action_EditRemoveComment),
                               IsResponsePermissions = true,
                               UserPost = CoreContext.UserManager.GetUsers(comment.UserId).Title
                           };

            return info;
        }


        public static string GetHtmlImgUserAvatar(Guid userId)
        {
            string imgPath = UserPhotoManager.GetSmallPhotoURL(userId);
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" src=\"" + imgPath + "\"/>";

            return "";
        }

        #region Ajax functions for comments management

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string RemoveComment(string commentId, string pid)
        {
            //var comment = Wiki.GetComment(new Guid(commentId));
            //if (comment != null)
            //{
            //    comment.Inactive = true;
            //    Wiki.SaveComment(comment);
            //    WikiActivityPublisher.DeletePageComment(Wiki.GetPage(comment.PageName), comment);
            //}
            Wiki.DeleteComment(new Guid(commentId));
            return commentId;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string GetPreview(string text, string commentID)
        {
            var info = GetPrevHTMLComment(text, commentID);
            var defComment = new CommentsList();
            ConfigureComments(defComment, null);

            return CommentsHelper.GetOneCommentHtmlWithContainer(defComment, info, true, false);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse AddComment(string parentCommentId, string pageName, string text, string pid)
        {
            var resp = new AjaxResponse();
            resp.rs1 = parentCommentId;

            //var newComment = new Comment
            //                     {
            //                         Body = text,
            //                         Date = TenantUtil.DateTimeNow(),
            //                         UserId = SecurityContext.CurrentAccount.ID,
            //                     };

            //if (!string.IsNullOrEmpty(parentCommentId))
            //{
            //    newComment.ParentId = new Guid(parentCommentId);
            //}
            //newComment.PageName = pageName;


            //newComment = Wiki.SaveComment(newComment);

            //WikiActivityPublisher.AddPageComment(Wiki.GetPage(newComment.PageName), newComment);
            var parentIdGuid = String.IsNullOrEmpty(parentCommentId) ? Guid.Empty : new Guid(parentCommentId);
            var newComment = Wiki.CreateComment(new Comment { Body = text, PageName = pageName, ParentId = parentIdGuid });

            var info = GetCommentInfo(newComment);

            var defComment = new CommentsList();
            ConfigureComments(defComment, pageName);

            var visibleCommentsCount = Wiki.GetComments(pageName).Count;

            resp.rs2 = CommentsHelper.GetOneCommentHtmlWithContainer(
                defComment,
                info,
                string.IsNullOrEmpty(parentCommentId),
                visibleCommentsCount % 2 == 1);

            //WikiNotifyClient.SendNoticeAsync(
            //    SecurityContext.CurrentAccount.ID.ToString(),
            //    Common.Constants.EditPage,
            //    pageName,
            //    null,
            //    GetListOfTagValForNotify(pageName, "new wiki page comment", newComment.Body));

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UpdateComment(string commentId, string text, string pid)
        {
            var resp = new AjaxResponse { rs1 = commentId };
            if (text == null) return resp;

            //var comment = Wiki.GetComment(new Guid(commentId));
            //comment.Body = text;
            //comment = Wiki.SaveComment(comment);

            //WikiActivityPublisher.EditPageComment(Wiki.GetPage(comment.PageName), comment);
            Wiki.UpdateComment(new Comment { Id = new Guid(commentId), Body = text });

            resp.rs2 = text;
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string LoadCommentText(string commentId, string pid)
        {
            var comment = Wiki.GetComment(new Guid(commentId));
            return comment != null ? comment.Body : string.Empty;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ConvertWikiToHtml(string pageName, string wikiValue, string appRelativeCurrentExecutionFilePath,
                                        string imageHandlerUrl)
        {
            return EditPage.ConvertWikiToHtml(pageName, wikiValue, appRelativeCurrentExecutionFilePath,
                                              imageHandlerUrl, TenantId);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ConvertWikiToHtmlWysiwyg(string pageName, string wikiValue, string appRelativeCurrentExecutionFilePath,
                                               string imageHandlerUrl)
        {
            return EditPage.ConvertWikiToHtmlWysiwyg(pageName, wikiValue, appRelativeCurrentExecutionFilePath,
                                                     imageHandlerUrl, TenantId);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CreateImageFromWiki(string pageName, string wikiValue, string appRelativeCurrentExecutionFilePath,
                                          string imageHandlerUrl)
        {
            return EditPage.CreateImageFromWiki(pageName, wikiValue, appRelativeCurrentExecutionFilePath,
                                                imageHandlerUrl, TenantId);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string UpdateTempImage(string fileName, string UserId, string tempFileName)
        {
            string outFileName;
            EditFile.MoveContentFromTemp(new Guid(UserId), tempFileName, fileName, ConfigLocation, PageWikiSection, TenantId, HttpContext.Current, RootPath, out outFileName);
            return outFileName;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void CancelUpdateImage(string UserId, string tempFileName)
        {
            EditFile.DeleteTempContent(tempFileName, ConfigLocation, PageWikiSection, TenantId, HttpContext.Current);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SearchPagesByStartName(string pageStartName)
        {
            var pages = Wiki.SearchPagesByStartName(pageStartName);

            if (pages.Count > WikiManager.MaxPageSearchInLinkDlgResult)
                pages = pages.GetRange(0, WikiManager.MaxPageSearchInLinkDlgResult);
            var sb = new StringBuilder();

            foreach (var p in pages)
            {
                sb.Append(@"<div class=""seachHelpItem"" onclick=""javascript:MouseSelectSearchPages(this);"">");
                sb.Append(p.PageName);
                sb.Append(@"</div>");
            }

            return sb.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SearchFilesByStartName(string fileStartName)
        {
            var pages = Wiki.FindFiles(fileStartName);

            if (pages.Count > WikiManager.MaxPageSearchInLinkDlgResult)
                pages = pages.GetRange(0, WikiManager.MaxPageSearchInLinkDlgResult);

            var sb = new StringBuilder();

            foreach (var f in pages)
            {
                sb.Append(@"<div class=""seachHelpItem"" onclick=""javascript:MouseSelectSearchPages(this);"">");
                sb.Append(f.FileName);
                sb.Append(@"</div>");
            }

            return sb.ToString();
        }

        #endregion

        #endregion

        #region IContextInitializer Members

        public void InitializeContext(HttpContext context)
        {
            _rootPath = context.Server.MapPath("~");
            _wikiSection = WikiSection.Section;
        }

        #endregion
    }
}