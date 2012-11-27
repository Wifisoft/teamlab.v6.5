using System;
using System.Collections.Generic;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.Controls;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.Community.Wiki
{
    public partial class ListPages : WikiBasePage
    {
        private static readonly int MaxNewResults = 25;

        public string categoryName
        {
            get { return Request["cat"]; }
        }

        protected bool isShowCat
        {
            get { return !isByUser && !string.IsNullOrEmpty(categoryName); }
        }

        protected bool isShowNew
        {
            get { return !isShowCat && Request["n"] != null; }
        }

        protected bool isShowFresh
        {
            get { return !isShowNew && Request["f"] != null; }
        }

        protected bool isByUser
        {
            get { return !byUserID.Equals(Guid.Empty); }
        }

        private Guid? m_byUserID = null;
        protected Guid byUserID
        {
            get
            {
                if (m_byUserID == null)
                {
                    try
                    {
                        m_byUserID = new Guid(Request["uid"]);
                    }
                    catch 
                    {
                        m_byUserID = Guid.Empty;
                    }
                }

                return m_byUserID.Value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as WikiMaster).GetNavigateActionsVisible += new WikiMaster.GetNavigateActionsVisibleHandle(ListPages_GetNavigateActionsVisible);
            if (isByUser)
            {
                PageHeaderText = string.Format(WikiResource.menu_byUserPagesFormat, CoreContext.UserManager.GetUsers(byUserID).DisplayUserName());
            }
            else if (isShowCat)
            {
                BreadCrumb.Add(new BreadCrumb() { Caption = WikiResource.menu_ListCategories, NavigationUrl = this.ResolveUrlLC("ListCategories.aspx") });
                BreadCrumb.Add(new BreadCrumb() { Caption = string.Format(WikiResource.menu_ListPagesCatFormat, categoryName) });
            }
            else if (isShowNew)
            {
                PageHeaderText = WikiResource.menu_NewPages;
            }
            else if (isShowFresh)
            {
                PageHeaderText = WikiResource.menu_FreshEditPages;
            }
            else
            {
                PageHeaderText = WikiResource.menu_ListPages;
            }

            if (!IsPostBack)
            {
                BindRepeater();
            }
        }

        WikiNavigationActionVisible ListPages_GetNavigateActionsVisible()
        {
            WikiNavigationActionVisible result = WikiNavigationActionVisible.AddNewPage;
            if(isShowCat)
            {
                result |= WikiNavigationActionVisible.SubscriptionOnCategory;
            }
            else
            {
                result |= WikiNavigationActionVisible.SubscriptionOnNewPage;
            }

            return result;
        }


        private void BindRepeater()
        {
            phListResult.Visible = phTableResult.Visible = false;
            if (isByUser || isShowNew || isShowFresh)
            {
                phListResult.Visible = true;
                List<Page> dataSource;

                if(isByUser)
                {
                    dataSource = Wiki.GetPages(byUserID);
                }
                else if (isShowNew)
                {                  
                    dataSource = Wiki.GetNewPages(MaxNewResults);
                }
                else
                {
                    dataSource = Wiki.GetRecentEditedPages(MaxNewResults);
                }

                //foreach (Pages p in dataSource)
                //{
                //    p.PageName = HttpUtility.HtmlEncode(p.PageName);
                //}

                rptPageList.DataSource = dataSource;
                rptPageList.DataBind();
            }
            else
            {
                phTableResult.Visible = true;
                List<Page> result;
                if (isShowCat)
                {
                    result = Wiki.GetPages(categoryName);
                }
                else
                {
                    result = Wiki.GetPages();
                }

                result.RemoveAll(pemp => string.IsNullOrEmpty(pemp.PageName));

                string firstLetter;
                List<string> letters = new List<string>(WikiResource.wikiCategoryAlfaList.Split(','));


                string otherSymbol = string.Empty;
                if (letters.Count > 0)
                {
                    otherSymbol = letters[0];
                    letters.Remove(otherSymbol);
                }

                List<PageDictionary> dictList = new List<PageDictionary>();
                PageDictionary pageDic;
                foreach (Page page in result)
                {
                    page.PageName = HttpUtility.HtmlEncode(page.PageName);

                    firstLetter = new string(page.PageName[0], 1);

                    if (!letters.Exists(lt => lt.Equals(firstLetter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        firstLetter = otherSymbol;
                    }

                    if (!dictList.Exists(dl => dl.HeadName.Equals(firstLetter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        pageDic = new PageDictionary();
                        pageDic.HeadName = firstLetter;
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

                int countAll = dictList.Count * 3 + result.Count; //1 letter is like 2 links to category
                int perColumn = (int)(Math.Round((decimal)countAll / 3));

                List<List<PageDictionary>> mainDictList = new List<List<PageDictionary>>();

                int index = 0, lastIndex = 0, count = 0;

                PageDictionary p;
                for (int i = 0; i < dictList.Count; i++)
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

                rptMainPageList.DataSource = mainDictList;
                rptMainPageList.DataBind();
            }
        }

        private int SortPageDict(PageDictionary cd1, PageDictionary cd2)
        {
            return cd1.HeadName.CompareTo(cd2.HeadName);
        }

        //protected void cmdDelete_Click(object sender, EventArgs e)
        //{
        //    PagesProvider.PagesDelete((sender as LinkButton).CommandName);
        //    BindRepeater();
        //}

        

        //protected string GetPageEditLink(Pages page)
        //{
        //    return ActionHelper.GetEditPagePath(this.ResolveUrlLC("Default.aspx"), page.PageName);
        //}

        //protected string GetPageInfo(Pages page)
        //{
        //    if (page.UserID.Equals(Guid.Empty))
        //    {
        //        return string.Empty;
        //    }


        //    return ProcessVersionInfo(page.PageName, page.UserID, page.Date, page.Version, false);
        //}

        protected string GetAuthor(Page page)
        {
            return CoreContext.UserManager.GetUsers(page.UserID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID);
        }

        protected string GetDate(Page page)
        {
            return string.Format("<span class=\"wikiDateTime\">{0}</span> {1}", page.Date.ToString("t"), page.Date.ToString("d"));
        }
    }
}