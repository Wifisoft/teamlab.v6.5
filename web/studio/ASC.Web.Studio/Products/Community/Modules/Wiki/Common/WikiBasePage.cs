using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.Community.Wiki.Common
{
    public enum ActionOnPage
    {
        None = 0,
        AddNew,
        AddNewFile,
        Edit,
        View,
        CategoryView,
        CategoryEdit
    }


    public class WikiBasePage : MainPage
    {
        protected WikiEngine Wiki
        {
            get { return new WikiEngine(); }
        }


        private bool? _isFile = null;
        public bool IsFile
        {
            get
            {
                if (_isFile == null)
                {
                    string s = WikiPage; // Init isFile
                }
                return _isFile.Value;
            }
        }


        protected string ConfigLocation
        {
            get { return HostingEnvironment.ApplicationVirtualPath + WikiManager.WikiSectionConfig; }
        }

        protected WikiSection _wikiSection = null;
        protected WikiSection PageWikiSection
        {
            get
            {
                if (_wikiSection == null)
                {
                    _wikiSection = WikiSection.Section;
                }
                return _wikiSection;
            }
        }

        protected string _rootPath = null;
        protected string RootPath
        {
            get
            {
                if (_rootPath == null)
                {
                    _rootPath = MapPath("~");
                }
                return _rootPath;
            }
        }


        private ActionOnPage _action = ActionOnPage.None;
        public ActionOnPage Action
        {
            get
            {
                if (_action.Equals(ActionOnPage.None))
                {
                    if (Request["action"] == null)
                    {
                        _action = ActionOnPage.View;
                    }
                    else
                    {
                        if (Request["action"].Equals("edit", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _action = ActionOnPage.Edit;
                        }
                        else if (Request["action"].Equals("newfile", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _action = ActionOnPage.AddNewFile;
                        }
                        else
                        {
                            _action = ActionOnPage.AddNew;
                        }
                    }

                    if (_action == ActionOnPage.View || _action == ActionOnPage.Edit)
                    {
                        if (WikiPage.StartsWith(ASC.Web.UserControls.Wiki.Constants.WikiCategoryKeyCaption, StringComparison.InvariantCultureIgnoreCase) && PageNameUtil.Decode(WikiPage).Contains(":"))
                        {
                            if (_action == ActionOnPage.View)
                                _action = ActionOnPage.CategoryView;
                            else
                                _action = ActionOnPage.CategoryEdit;
                        }
                    }
                }

                return _action;
            }
        }

        private string _wikiPage = null;
        public string WikiPage
        {
            get
            {
                if (_wikiPage == null)
                {
                    _isFile = false;
                    if (string.IsNullOrEmpty(Request["page"]))
                    {
                        _wikiPage = string.Empty;
                        if (!string.IsNullOrEmpty(Request["file"]))
                        {
                            _isFile = true;
                            _wikiPage = Request["file"];
                        }
                    }
                    else
                    {
                        _wikiPage = Request["page"];
                    }
                }

                return _wikiPage;
            }
        }

        protected WikiMaster WikiMaster
        {
            get { return (WikiMaster)Master; }
        }


        protected int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Title = HeaderStringHelper.GetPageTitle(WikiResource.ModuleName, WikiMaster.Breadcrumbs);
        }

        protected string PageHeaderText
        {
            get
            {
                if (WikiMaster.Breadcrumbs.Count > 0)
                {
                    return WikiMaster.Breadcrumbs[WikiMaster.Breadcrumbs.Count - 1].Caption;
                }
                return string.Empty;
            }
            set
            {
                WikiMaster.Breadcrumbs.Clear();
                WikiMaster.Breadcrumbs.Add(new BreadCrumb() { Caption = value, NavigationUrl = string.Empty });
            }
        }

        protected List<BreadCrumb> BreadCrumb
        {
            get { return WikiMaster.Breadcrumbs; }
        }

        protected string GetPageInfo(string name, Guid userID, DateTime date)
        {
            return string.Format(WikiResource.wikiPageInfoFormat,
                string.Format("<span style=\"padding-right: 8px; padding-left: 8px;\">{0}</span>", CoreContext.UserManager.GetUsers(userID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID)),
                string.Format("<span style=\"padding-right: 8px; padding-left: 8px;\">{0} {1}</span>", date.ToString("t"), date.ToString("d")));
        }

        protected string ProcessVersionInfo(string name, Guid userID, DateTime date, int version, bool isFile, bool versionIsLink)
        {

            return string.Format(@"{0}&nbsp;{2}&nbsp;{1}",
                    CoreContext.UserManager.GetUsers(userID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID),
                    date.AgoSentence(),
                    isFile ? string.Empty :
                           versionIsLink ?
                                string.Format(@"<a href=""{0}?page={3}"">{2}{1}</a>&nbsp;",
                                    this.ResolveUrlLC("PageHistoryList.aspx"),
                                    version,
                                    WikiResource.wikiVersionCaption,
                                    name) :
                                string.Format(@"{1}{0}&nbsp;",
                                    version,
                                    WikiResource.wikiVersionCaption)
                    );
        }

        protected string GetPageName(Page page)
        {
            if (string.IsNullOrEmpty(page.PageName))
                return WikiResource.MainWikiCaption;
            return HttpUtility.HtmlEncode(page.PageName);
        }

        protected string GetPageViewLink(Page page)
        {
            return ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), HttpUtility.HtmlDecode(page.PageName));
        }

        protected string ProcessVersionInfo(string name, Guid userID, DateTime date, int version, bool isFile)
        {
            return ProcessVersionInfo(name, userID, date, version, isFile, true);
        }

        protected static string GetFileLengthToString(long length)
        {
            string result = string.Empty;

            decimal KB = 1024;
            decimal MB = KB * 1024;
            decimal GB = MB * 1024;
            decimal TB = GB * 1024;

            if (length > TB)
            {
                result = string.Format("{0} {1}", Math.Round(length / TB, 2), WikiResource.wikiSizeTB);
            }
            else if (length > GB)
            {
                result = string.Format("{0} {1}", Math.Round(length / GB, 2), WikiResource.wikiSizeGB);
            }
            else if (length > MB)
            {
                result = string.Format("{0} {1}", Math.Round(length / MB, 2), WikiResource.wikiSizeMB);
            }
            else if (length > KB)
            {
                result = string.Format("{0} {1}", Math.Round(length / KB, 2), WikiResource.wikiSizeKB);
            }
            else
            {
                result = string.Format("{0} {1}", length, WikiResource.wikiSizeB);
            }

            return result;
        }
    }
}
