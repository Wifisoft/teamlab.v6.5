using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core.Tenants;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Resources;


namespace ASC.Web.UserControls.Wiki.UC
{
    public enum SaveResult
    {
        Ok = 0,
        OkPageRename,
        PageNameIsEmpty,
        PageNameIsIncorrect,
        SamePageExists,
        FileEmpty,
        OldVersion,
        UserIdIsEmpty,
        NoChanges,
        SectionUpdate,
        FileSizeExceeded,
        Error
    }


    public partial class EditPage : BaseUserControl
    {
        public delegate void SaveNewCategoriesAddedHandler(object sender, List<string> categories, string pageName);
        public event SaveNewCategoriesAddedHandler SaveNewCategoriesAdded;

        public delegate void SetNewFCKModeHandler(bool isWysiwygDefault);
        public event SetNewFCKModeHandler SetNewFCKMode;

        public delegate string GetUserFriendlySizeFormatHandler(long size);
        public event GetUserFriendlySizeFormatHandler GetUserFriendlySizeFormat;

        public Guid CurrentUserId { get; set; }

        private int pageVersion
        {
            get
            {
                if (ViewState["pageVersion"] == null)
                    return 0;
                return Convert.ToInt32(ViewState["pageVersion"]);
            }
            set { ViewState["pageVersion"] = value; }
        }

        public string AlaxUploaderPath
        {
            get
            {
                if (this.ViewState["AlaxUploaderPath"] == null)
                    return string.Empty;
                return this.ViewState["AlaxUploaderPath"].ToString().ToLower();
            }

            set { this.ViewState["AlaxUploaderPath"] = value; }
        }

        public string PleaseWaitMessage
        {
            get
            {
                if (this.ViewState["PleaseWaitMessage"] == null)
                    return string.Empty;
                return this.ViewState["PleaseWaitMessage"].ToString();
            }

            set { this.ViewState["PleaseWaitMessage"] = value; }
        }

        public string AjaxProgressLoaderGif
        {
            get
            {
                if (this.ViewState["AjaxProgressLoaderGif"] == null)
                    return string.Empty;
                return this.ViewState["AjaxProgressLoaderGif"].ToString();
            }

            set { this.ViewState["AjaxProgressLoaderGif"] = value; }
        }

        public string JQPath
        {
            get
            {
                if (this.ViewState["JQPath"] == null)
                    return string.Empty;
                return this.ViewState["JQPath"].ToString().ToLower();
            }

            set { this.ViewState["JQPath"] = value; }
        }


        private int pageSection
        {
            get
            {
                int result;
                if (!int.TryParse(Request["section"], out result))
                {
                    result = -1;
                }
                return result;
            }
        }

        /// <summary>
        /// The Main Container of the Preview. The Container display style will be setted to '' when the preview result will be ready.
        /// </summary>
        public string PreviewContainer
        {
            get
            {
                if (ViewState["PreviewContainer"] == null)
                    return string.Format("{0}_PrevContainer", this.ClientID);
                return ViewState["PreviewContainer"].ToString();
            }
            set { ViewState["PreviewContainer"] = value; }
        }

        public string OnPreviewReadyHandler
        {
            get
            {
                if (ViewState["OnPreviewReadyHandler"] == null)
                    return string.Format("{0}_OnPreviewReadyHandler", this.ClientID);
                return ViewState["OnPreviewReadyHandler"].ToString();
            }
            set { ViewState["OnPreviewReadyHandler"] = value; }
        }

        /// <summary>
        /// Div DOM Id where result of the preview will be setted.
        /// </summary>
        public string PreviewView
        {
            get
            {
                if (ViewState["PreviewView"] == null)
                    return string.Format("{0}_PrevValue", this.ClientID);
                return ViewState["PreviewView"].ToString();
            }
            set { ViewState["PreviewView"] = value; }
        }

        public bool IsWysiwygDefault
        {
            get
            {
                if (ViewState["IsWysiwygDefault"] == null)
                    return true;
                return Convert.ToBoolean(ViewState["IsWysiwygDefault"]);
            }
            set { ViewState["IsWysiwygDefault"] = value; }
        }
        public bool CanUploadFiles
        {
            get
            {
                if (ViewState["CanUploadFiles"] == null)
                    return false;
                return Convert.ToBoolean(ViewState["CanUploadFiles"]);
            }
            set { ViewState["CanUploadFiles"] = value; }
        }

        public bool IsNew
        {
            get
            {
                if (ViewState["IsNew"] == null)
                    return false;
                return Convert.ToBoolean(ViewState["IsNew"]);
            }
            set { ViewState["IsNew"] = value; }
        }

        public string MainCssFile
        {
            get
            {
                if (ViewState["MainCssFile"] == null)
                    return string.Empty;
                return ViewState["MainCssFile"].ToString();
            }
            set { ViewState["MainCssFile"] = value; }
        }

        public bool IsSpecialName
        {
            get
            {
                if (ViewState["IsSpecialName"] == null)
                    return false;
                return Convert.ToBoolean(ViewState["IsSpecialName"]);
            }
            set { ViewState["IsSpecialName"] = value; }
        }


        public string PageName
        {
            get
            {
                if (ViewState["PageName"] == null)
                    return string.Empty;
                return ViewState["PageName"].ToString();
            }
            set { ViewState["PageName"] = value; }
        }

        public string WikiFckClientId
        {
            get { return Wiki_FCKEditor.ClientID; }
        }

        private bool IsNameReserved(string pageName)
        {
            if (!pageName.Contains(':'))
                return false;

            var exitReserv = (
                from rn in reservedPrefixes
                where rn.Equals(pageName.Split(':')[0], StringComparison.InvariantCultureIgnoreCase)
                select rn);

            return exitReserv.Count() > 0;
        }

        private string RenderPageContent(string pageName, string wiki)
        {
            return HtmlWikiUtil.WikiToHtml(pageName, wiki, Page.ResolveUrl(Request.AppRelativeCurrentExecutionFilePath), Wiki.GetPagesAndFiles(wiki),
                Page.ResolveUrl(ImageHandlerUrlFormat), TenantId, ConvertType.Wysiwyg);
        }

        public string mainPath
        {
            get
            {
                if (ViewState["mainPath"] == null)
                {
                    return string.Empty;
                }

                return ViewState["mainPath"].ToString().ToLower();
            }
            set { ViewState["mainPath"] = value; }
        }

        private void SetWikiFCKEditorValue(string pageName, string pageBody)
        {
            Wiki_FCKEditor.Value = IsWysiwygDefault ? RenderPageContent(pageName, pageBody) : pageBody;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                Wiki_FCKEditor.BasePath = VirtualPathUtility.ToAbsolute(BaseFCKRelPath);
                Wiki_FCKEditor.ToolbarSet = "WikiPanel";

                if (PageName != null && this.Visible)
                {
                    var page = Wiki.GetPage(PageNameUtil.Decode(PageName));
                    bool isPageNew = (page == null || IsNew);

                    RiseWikiPageLoaded(isPageNew, page);

                    if (!isPageNew)
                    {
                        txtPageName.Text = page.PageName;
                        if (pageSection < 0)
                        {
                            SetWikiFCKEditorValue(page.PageName, page.Body);
                        }
                        else
                        {
                            SetWikiFCKEditorValue(page.PageName, HtmlWikiUtil.GetWikiSectionBySectionNumber(page.Body, pageSection));
                            txtPageName.ReadOnly = true;
                            txtPageName.Text += string.Format(WikiUCResource.wikiPageEditSectionCaptionFormat, HtmlWikiUtil.GetWikiSectionNameBySectionNumber(page.Body, pageSection));
                        }

                        //Check for 'help' and 'main' page
                        if (IsStandartName(page))
                        {
                            txtPageName.ReadOnly = true;
                        }
                        pageVersion = page.Version;

                        //if (page.PageName.Equals(string.Empty))
                        //txtPageName.ReadOnly = true; //We need to disable any changes for the one time saved page.

                        RisePublishVersionInfo(page);
                    }
                    else if (!string.IsNullOrEmpty(PageName))
                    {
                        txtPageName.Text = PageNameUtil.Decode(PageName);
                        if (IsSpecialName)
                        {
                            txtPageName.ReadOnly = true;
                        }
                    }

                    phPageName.Visible = !txtPageName.ReadOnly;
                }

                hfFCKLastState.Value = (!IsWysiwygDefault).ToString().ToLower();
            }
        }

        private bool IsStandartName(Page page)
        {
            return page.PageName == WikiUCResource.HelpPageCaption || page.PageName == ""; //Main or help
        }

        public SaveResult Save(Guid userId)
        {
            string pageName;
            return Save(userId, out pageName);
        }

        public SaveResult Save(Guid userId, out string pageName)
        {
            try
            {

                if (SetNewFCKMode != null)
                {
                    bool isSource;
                    bool.TryParse(hfFCKLastState.Value, out isSource);
                    SetNewFCKMode(!isSource);

                    IsWysiwygDefault = !isSource; //We need to update the variable if SetNewFCKMode is used only!!!
                }

                Page page;
                string currentPageName = txtPageName.Text.Trim();
                if (currentPageName.Length > 240) currentPageName = currentPageName.Substring(0, 240).Trim();
                currentPageName = PageNameUtil.Clean(currentPageName);
                if (pageSection >= 0)
                {
                    currentPageName = PageNameUtil.Decode(PageName);
                }

                pageName = currentPageName;
                bool isPageRename = pageName != PageNameUtil.Decode(PageName) && !string.IsNullOrEmpty(PageName);
                string oldPageName = PageName;

                if (currentPageName.Equals(string.Empty) && !txtPageName.ReadOnly)
                {
                    SetWikiFCKEditorValue(currentPageName, Wiki_FCKEditor.Value);
                    return SaveResult.PageNameIsEmpty;
                }

                if (!IsSpecialName && IsNameReserved(currentPageName))
                {
                    SetWikiFCKEditorValue(currentPageName, Wiki_FCKEditor.Value);
                    return SaveResult.PageNameIsIncorrect;
                }

                if (userId.Equals(Guid.Empty))
                {
                    SetWikiFCKEditorValue(currentPageName, Wiki_FCKEditor.Value);
                    return SaveResult.UserIdIsEmpty;
                }

                if (!PageNameUtil.Decode(PageName).Equals(currentPageName, StringComparison.InvariantCulture))
                {
                    page = Wiki.GetPage(currentPageName);
                    if (page != null)
                    {
                        SetWikiFCKEditorValue(currentPageName, Wiki_FCKEditor.Value);
                        return SaveResult.SamePageExists;
                    }
                    page = new Page();
                }
                else
                {
                    page = Wiki.GetPage(currentPageName);
                    if (page == null)
                    {
                        page = new Page();
                    }
                }

                page.PageName = currentPageName;
                string PageResult;
                if (pageSection < 0)
                {
                    PageResult = Wiki_FCKEditor.Value;
                }
                else
                {
                    PageResult = HtmlWikiUtil.SetWikiSectionBySectionNumber(page.Body, pageSection, Wiki_FCKEditor.Value);
                }

                if (PageResult.Equals(page.Body))
                {
                    SetWikiFCKEditorValue(page.PageName, Wiki_FCKEditor.Value);
                    return SaveResult.NoChanges;
                }

                page.Body = PageResult;

                if (pageVersion > 0 && pageVersion < Wiki.GetPageMaxVersion(page.PageName))
                {
                    SetWikiFCKEditorValue(page.PageName, Wiki_FCKEditor.Value);
                    return SaveResult.OldVersion;
                }
                page.Date = TenantUtil.DateTimeNow();
                page.UserID = userId;

                page.Version++;
                Wiki.SavePage(page);

                var newCats = Wiki.UpdateCategoriesByPageContent(page);
                if (newCats.Count > 0 && SaveNewCategoriesAdded != null)
                {
                    SaveNewCategoriesAdded(this, newCats, page.PageName);
                }

                pageVersion = page.Version;
                RisePublishVersionInfo(page);

                SetWikiFCKEditorValue(page.PageName, Wiki_FCKEditor.Value);

                if (pageSection >= 0)
                {
                    return SaveResult.SectionUpdate;
                }
                if (isPageRename)
                {
                    //create dumb page
                    var oldpage = Wiki.GetPage(PageNameUtil.Decode(oldPageName));
                    if (oldpage != null)
                    {
                        oldpage.Date = TenantUtil.DateTimeNow();
                        oldpage.UserID = userId;
                        oldpage.Body = string.Format(WikiUCResource.wikiPageMoved, pageName); //Dummy
                        oldpage.Version++;
                        Wiki.SavePage(oldpage);

                        return SaveResult.OkPageRename;
                    }
                    else
                    {
                        return SaveResult.SamePageExists;
                    }
                }
                return SaveResult.Ok;
            }
            catch (Exception)
            {
                pageName = txtPageName.Text.Trim();
                return SaveResult.Error;
            }
        }

        protected string InitVariables()
        {
            return string.Format(@"var pageName = '{0}';
                            var appRelativeCurrentExecutionFilePath = '{1}';
                            var imageHandlerUrl = '{2}';
                            var wikiInternalStart = '{3}';
                            var wikiInternalFile = '{4}';
                            var wikiMaxFileUploadSizeString = '{5}'
                            var wikiFileUploadProgress = '{6}';
                            var wikiDenyFileUpload = {7};",
                             PageNameUtil.Decode(PageName).EscapeString(),
                             Page.ResolveUrl(Request.AppRelativeCurrentExecutionFilePath).EscapeString().ToLower(),
                             Page.ResolveUrl(ImageHandlerUrlFormat).EscapeString().ToLower(),
                             string.Format("{0}?page=", mainPath).EscapeString(),
                             string.Format("{0}?file=", mainPath).EscapeString(),
                             GetMaxFileUploadString(),
                             GetFileUploadProgress(),
                             (!CanUploadFiles).ToString().ToLower()
                             );
        }

        protected string GetFileUploadProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"textMediumDescribe\">");
            sb.Append(PleaseWaitMessage);
            sb.AppendFormat("</div><img src=\"{0}\"/>", AjaxProgressLoaderGif);

            return sb.ToString();
        }

        protected string GetMaxFileUploadString()
        {
            string result = string.Empty;
            long lMaxFileSize = FileUploader.MaxUploadSize;
            if (lMaxFileSize == 0)
                return result;
            if (GetUserFriendlySizeFormat != null)
            {
                result = GetUserFriendlySizeFormat(lMaxFileSize);
            }
            else
            {
                result = lMaxFileSize.ToString();
            }

            result = string.Format(WikiUCResource.wikiMaxUploadSizeFormat, result);

            return result;
        }

        protected string GetPageClassName()
        {
            return Page.GetType().BaseType.Name;
        }

        protected string GetAllStyles()
        {
            string linkCssFormat = "<link rel='stylesheet' text='text/css' href='{0}' />";
            string scriptFormat = "<script language='javascript' type='text/javascript' src='{0}'>\" + \"</\" + \"script>";
            string script = "\"";
            if (!string.IsNullOrEmpty(MainCssFile))
            {
                script += string.Format(linkCssFormat,
                    MainCssFile);
            }

            script += string.Format(linkCssFormat,
                    ToLowerPath(Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), "ASC.Web.UserControls.Wiki.Css.main.css")));

            script += string.Format(scriptFormat,
                    ToLowerPath(Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), "ASC.Web.UserControls.Wiki.Js.EditPage.js")));

            script += "\";";

            return script;
        }

        protected string ToLowerPath(string url)
        {
            if (!url.Contains("?"))
                return url.ToLower();
            string _url = url.Split('?')[0];
            string _params = url.Split('?')[1];
            return string.Format("{0}?{1}", _url.ToLower(), _params);
        }


        public static string ConvertWikiToHtml(string pageName, string value, string appRelativeCurrentExecutionFilePath,
                string imageHandlerUrl, int tenantId)
        {

            return HtmlWikiUtil.WikiToHtml(pageName, value, appRelativeCurrentExecutionFilePath, new WikiEngine().GetPagesAndFiles(value),
                imageHandlerUrl, tenantId, ConvertType.NotEditable);
        }

        public static string ConvertWikiToHtmlWysiwyg(string pageName, string value, string appRelativeCurrentExecutionFilePath,
                string imageHandlerUrl, int tenantId)
        {

            return HtmlWikiUtil.WikiToHtml(pageName, value, appRelativeCurrentExecutionFilePath, new WikiEngine().GetPagesAndFiles(value),
                imageHandlerUrl, tenantId, ConvertType.Wysiwyg);
        }

        public static string CreateImageFromWiki(string pageName, string value, string appRelativeCurrentExecutionFilePath,
                string imageHandlerUrl, int tenantId)
        {
            return HtmlWikiUtil.CreateImageFromWiki(pageName, value, appRelativeCurrentExecutionFilePath, imageHandlerUrl, tenantId);
        }

        public string GetShowPrevFunctionName()
        {
            return string.Format("{0}_ShowPreview", this.ClientID);
        }
    }
}