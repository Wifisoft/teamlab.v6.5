using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using AjaxPro;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Common;
using ASC.Bookmarking.Pojo;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Common.Util;
using ASC.Web.UserControls.Bookmarking.Resources;
using ASC.Web.UserControls.Bookmarking.Util;
using HtmlAgilityPack;

namespace ASC.Web.UserControls.Bookmarking
{
    [AjaxNamespace("BookmarkPage")]
    public partial class BookmarkingUserControl : UserControl
    {
        #region Fields

        private BookmarkingServiceHelper ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

        public IList<Bookmark> Bookmarks { get; set; }

        #endregion

        #region Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof (BookmarkingUserControl));
            Utility.RegisterTypeForAjax(typeof (SingleBookmarkUserControl));
            Utility.RegisterTypeForAjax(typeof (CommentsUserControl));

            var createBookmark = LoadControl(BookmarkUserControlPath.CreateBookmarkUserControlPath) as CreateBookmarkUserControl;
            CreateBookmarkPanel.Controls.Add(createBookmark);

            var removePopup = LoadControl(BookmarkingRemoverFromFavouritePopup.Location) as BookmarkingRemoverFromFavouritePopup;
            BookmarkingRemoveFromFavouritePopupContainer.Controls.Add(removePopup);

            InitSettings();

            var SortControl = new ViewSwitcher {SortItemsHeader = BookmarkingUCResource.ShowLabel};

            ServiceHelper.InitServiceHelper(SortControl);
            BookmarkingSortPanel.Controls.Add(SortControl);

            if (Bookmarks == null)
            {
                Bookmarks = ServiceHelper.GetBookmarks();
            }

            if (Bookmarks == null || Bookmarks.Count == 0)
            {
                var hidePanelsFlag = false;
                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("bookmarks_icon.png", BookmarkingSettings.ModuleId),
                                                 Describe = BookmarkingUCResource.EmptyScreenText
                                             };

                if (ServiceHelper.DisplayMode.Equals(BookmarkingServiceHelper.BookmarkDisplayMode.SearchBookmarks))
                {
                    hidePanelsFlag = true;

                    emptyScreenControl.Header = BookmarkingUCResource.EmptyScreenSearchCaption;
                }
                else
                {
                    var sortBy = Request.QueryString[BookmarkingRequestConstants.SortByParam];
                    if (string.IsNullOrEmpty(sortBy)
                        || BookmarkingRequestConstants.MostRecentParam.Equals(sortBy)
                        || BookmarkingRequestConstants.PopularityParam.Equals(sortBy))
                    {
                        hidePanelsFlag = true;

                        emptyScreenControl.Header = BookmarkingUCResource.EmptyScreenCaption;
                        emptyScreenControl.ButtonHTML = String.Format("<a class='linkAddMediumText' href='CreateBookmark.aspx'>{0}</a>", BookmarkingUCResource.EmptyScreenLink);
                    }
                    else
                    {
                        emptyScreenControl.Header = BookmarkingUCResource.EmptyScreenSearchCaption;
                    }
                }

                BookmarksHolder.Controls.Add(emptyScreenControl);
                if (hidePanelsFlag)
                {
                    BookmarkingSortPanel.Visible = false;
                    CreateBookmarkPanel.Visible = false;
                    BookmarksMainPanel.Visible = false;
                }
            }
            else
            {
                LoadBookmarks(Bookmarks);
            }
        }

        private void InitSettings()
        {
            BookmarkingSettings.ThumbnailAbsolutePath = Server.MapPath("~/");
            BookmarkingSettings.ThumbnailAbsoluteFilePath = BookmarkingSettings.ThumbnailAbsolutePath + BookmarkingSettings.ThumbnailRelativePath;
        }

        #endregion

        #region Load & Refresh Bookmarks

        private void LoadBookmarks(IList<Bookmark> bookmarks)
        {
            if (!BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark.Equals(ServiceHelper.DisplayMode))
            {
                AddBookmarksListToPlaceHolder(bookmarks, BookmarksHolder);
                SetBookmarkingPagination();
            }
        }

        private void AddBookmarksListToPlaceHolder(IList<Bookmark> bookmarks, HtmlGenericControl p)
        {
            foreach (var b in bookmarks)
            {
                var c = LoadControl(BookmarkUserControlPath.SingleBookmarkUserControlPath) as BookmarkInfoBase;
                c.Bookmark = b;
                //c.UserBookmark = ServiceHelper.GetCurrentUserBookmark(b);
                c.UserBookmark = ServiceHelper.GetCurrentUserBookmark(b.UserBookmarks);
                p.Controls.Add(c);
            }
        }

        #endregion

        #region Remove From Favourites

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object RemoveBookmarkFromFavouriteInFavouriteMode(int userBookmarkID)
        {
            ServiceHelper.RemoveBookmarkFromFavourite(userBookmarkID);
            return null;
        }

        /// <summary>
        /// Removes bookmark from favourite. If after removing user bookmark raiting of this bookmark is 0, the bookmark will be removed completely.
        /// </summary>
        /// <param name="userBookmarkID"></param>
        /// <param name="uniqueID"></param>
        /// <returns>
        /// 1. null, if the bookmark was removed completely on the BookmarkInfo page.
        /// 2. string.Empty, is the bookmark was removed completely on the FavouriteBookmarks page.
        /// 3. Original bookmark in html.
        /// </returns>
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object RemoveBookmarkFromFavourite(int userBookmarkID, string uniqueID)
        {
            var b = ServiceHelper.RemoveBookmarkFromFavourite(userBookmarkID);
            if (b == null)
            {
                return BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark == ServiceHelper.DisplayMode ? null : string.Empty;
            }
            var userBookmarks = ServiceHelper.GetUserBookmarks(b);
            if (userBookmarks == null || userBookmarks.Count == 0)
            {
                return BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark == ServiceHelper.DisplayMode ? null : string.Empty;
            }
            return new {Bookmark = GetBookmarkAsString(b, new Guid(uniqueID)), ID = ServiceHelper.GetCurrentUserID()};
        }

        private object GetBookmarkAsString(Bookmark b, Guid uniqueID)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var textWriter = new HtmlTextWriter(sw))
                {
                    try
                    {
                        var c = LoadControl(BookmarkUserControlPath.SingleBookmarkUserControlPath) as BookmarkInfoBase;
                        c.Bookmark = b;
                        c.UserBookmark = ServiceHelper.GetCurrentUserBookmark(b);
                        c.UniqueId = uniqueID;
                        c.InitUserControl();
                        c.RenderControl(textWriter);
                    }
                    catch
                    {
                    }
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Save Bookmark

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveBookmark(string BookmarkUrl, string BookmarkName, string BookmarkDescription, string BookmarkTags)
        {
            var url = UpdateURL(BookmarkUrl);
            var b = ServiceHelper.AddBookmark(url, BookmarkName, BookmarkDescription, BookmarkTags);
            return GetBookmarkAsString(b, Guid.NewGuid());
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveBookmarkAjax(string BookmarkUrl, string BookmarkName, string BookmarkDescription, string BookmarkTags, string uniqueID)
        {
            var url = UpdateURL(BookmarkUrl);
            var b = ServiceHelper.AddBookmark(url, BookmarkName, BookmarkDescription, BookmarkTags);

            b = ServiceHelper.GetBookmarkWithUserBookmarks(url);

            var bookmarkString = GetBookmarkAsString(b, new Guid(uniqueID));
            if (BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark == ServiceHelper.DisplayMode)
            {
                var userImage = BookmarkingServiceHelper.GetHTMLUserAvatar();
                var userPageLink = BookmarkingServiceHelper.GetUserPageLink();
                var ub = ServiceHelper.GetCurrentUserBookmark(b);

                var userBookmarkDescription = BookmarkingServiceHelper.GetUserBookmarkDescriptionIfChanged(ub);
                var dateAdded = BookmarkingConverter.GetDateAsString(ub.DateAdded);
                var divID = ub.UserID.ToString();
                var userBookmarks = ServiceHelper.GetUserBookmarks(b);
                var addedBy = new BookmarkAddedByUserContorl().GetAddedByTableItem(userBookmarks.Count%2 != 0, userImage, userPageLink, userBookmarkDescription, dateAdded, divID);
                return new {BookmarkString = bookmarkString, AddedBy = addedBy, DivID = divID};
            }
            return new {BookmarkString = bookmarkString, AddedBy = string.Empty};
        }

        private static string UpdateURL(string URL)
        {
            if (URL == null)
            {
                return BookmarkingRequestConstants.Default_URL;
            }
            if (URL.StartsWith(BookmarkingRequestConstants.URL_Prefix) || URL.StartsWith(BookmarkingRequestConstants.URL_HTTPS_Prefix))
            {
                return URL;
            }
            URL = BookmarkingRequestConstants.URL_Prefix + URL;
            return URL;
        }

        #endregion

        #region Pagination

        private void SetBookmarkingPagination()
        {
            var pageNavigator = new PageNavigator();

            ServiceHelper.InitPageNavigator(pageNavigator);

            BookmarkingPaginationContainer.Controls.Add(pageNavigator);
        }

        #endregion

        #region Tags Autocomplete Popup Box

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse GetSuggest(string text, string varName, int limit)
        {
            var resp = new AjaxResponse();

            var startSymbols = text;
            var ind = startSymbols.LastIndexOf(",");
            if (ind != -1)
                startSymbols = startSymbols.Substring(ind + 1);

            startSymbols = startSymbols.Trim();

            IList<Tag> tags = new List<Tag>();
            if (!string.IsNullOrEmpty(startSymbols))
            {
                tags = ServiceHelper.GetAllTags(startSymbols, limit);
            }

            var resNames = new StringBuilder();
            var resHelps = new StringBuilder();

            foreach (var tag in tags)
            {
                resNames.Append(tag.Name);
                resNames.Append("$");
                resHelps.Append(tag.TagID);
                resHelps.Append("$");
            }

            resp.rs1 = resNames.ToString().TrimEnd('$');
            resp.rs2 = resHelps.ToString().TrimEnd('$');
            resp.rs3 = text;
            resp.rs4 = varName;

            return resp;
        }

        #endregion

        #region Ajax Request: get bookmark info and create a thumbnail

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object GetBookmarkByUrl(string url)
        {
            //Create thumbnail if it doesn't exists
            ThumbnailHelper.Instance.MakeThumbnail(url, true, true, HttpContext.Current, TenantProvider.CurrentTenantID);

            var b = ServiceHelper.GetBookmarkByUrl(url);
            return GetBookmarkByUrl(b, url);
        }

        private static object GetBookmarkByUrl(Bookmark b, string url)
        {
            var tags = string.Empty;
            if (b != null)
            {
                tags = BookmarkingServiceHelper.ConvertBookmarkToTagsString(b);
                var raitingHtml = new BookmarkRaitingUserControl().GetBookmarkRaiting(b);
                return new {Name = b.Name, Description = b.Description, Tags = tags, IsNewBookmark = false, Raiting = raitingHtml};
            }
            var title = GetWebSiteTitleByUrl(url);
            return new {Name = title[0], Description = title[1], Tags = tags, IsNewBookmark = true, Raiting = string.Empty};
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object GetUserBookmarkByUrl(string url)
        {
            //Create bookmark thumbnail
            ThumbnailHelper.Instance.MakeThumbnail(url, true, true, HttpContext.Current, TenantProvider.CurrentTenantID);

            var b = ServiceHelper.GetBookmarkWithUserBookmarks(url);

            if (b == null)
            {
                var title = GetWebSiteTitleByUrl(url);
                return new {Name = title[0], Description = title[1], Tags = string.Empty, IsNewBookmark = true, Raiting = string.Empty};
            }

            var userBookmark = ServiceHelper.GetCurrentUserBookmark(b.UserBookmarks);
            if (userBookmark == null)
            {
                return GetBookmarkByUrl(b, url);
            }

            var tags = string.Empty;

            tags = BookmarkingServiceHelper.ConvertBookmarkToTagsString(b);

            var raitingHtml = new BookmarkRaitingUserControl().GetBookmarkRaiting(b);
            return new {Name = userBookmark.Name, Description = userBookmark.Description, Tags = tags, IsNewBookmark = false, Raiting = raitingHtml};
        }

        private static string[] GetWebSiteTitleByUrl(string url)
        {
            var emptyResult = new[] {string.Empty, string.Empty};

            try
            {
                var webRequest = WebRequest.Create(url);
                webRequest.Timeout = BookmarkingSettings.PingTimeout;
                var webResponse = webRequest.GetResponse();
                if (webResponse == null)
                {
                    return emptyResult;
                }

                #region Encoding

                var encodingString = ((HttpWebResponse) (webResponse)).CharacterSet;
                var responseStream = webResponse.GetResponseStream();

                StreamReader sr;
                string text;
                Encoding encoding;
                var e = Encoding.GetEncoding("ISO-8859-1").BodyName;
                if (!String.IsNullOrEmpty(encodingString) && !e.ToLower().Equals(encodingString.ToLower()))
                {
                    encoding = Encoding.GetEncoding(encodingString);
                    sr = new StreamReader(responseStream, encoding);
                    text = sr.ReadToEnd();
                }
                else
                {
                    encoding = BookmarkingSettings.PageTitleEncoding;

                    sr = new StreamReader(responseStream, encoding);
                    text = sr.ReadToEnd();

                    var htmlEncoding = new HtmlDocument().DetectEncodingHtml(text);
                    if (htmlEncoding != null)
                    {
                        encoding = htmlEncoding;
                        text = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream(), encoding).ReadToEnd();
                    }
                    else
                    {
                        var doc = new HtmlDocument();
                        doc.Load(WebRequest.Create(url).GetResponse().GetResponseStream());
                        var encodingNode = doc.DocumentNode.SelectSingleNode(string.Format("//meta[{0}]", GetXpathArgumentIgnoreCase("charset")));
                        if (encodingNode != null)
                        {
                            var encodingAttr = encodingNode.Attributes["charset"];
                            if (encodingAttr != null)
                            {
                                var encodingVal = encodingAttr.Value;
                                if (!string.IsNullOrEmpty(encodingVal))
                                {
                                    encoding = Encoding.GetEncoding(encodingVal);
                                    text = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream(), encoding).ReadToEnd();
                                }
                            }
                        }
                    }
                }

                #endregion

                var d = new HtmlDocument {OptionReadEncoding = false};
                d.LoadHtml(text);
                var titleNode = d.DocumentNode.SelectSingleNode("//title");
                var title = string.Empty;
                if (titleNode != null)
                {
                    title = titleNode.InnerText;
                }

                var description = string.Empty;
                var descriptionNode = d.DocumentNode.SelectSingleNode(string.Format("//meta[{0}]", GetXpathArgumentIgnoreCase("name", "description")));
                if (descriptionNode != null)
                {
                    var content = descriptionNode.Attributes["content"];
                    if (content != null)
                    {
                        description = content.Value;
                    }
                }

                title = BookmarkingServiceHelper.EncodeUserData(title).Replace("<br/>", " ");
                description = BookmarkingServiceHelper.EncodeUserData(description);

                return new[] {title, description};

            }
            catch
            {
            }
            return emptyResult;
        }

        private static string GetXpathArgumentIgnoreCase(string argName, string argValue)
        {
            return string.Format("translate(@{0},'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{1}'", argName, argValue.ToLower());
        }

        private static string GetXpathArgumentIgnoreCase(string argName)
        {
            return string.Format("translate(@{0},'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')", argName);
        }

        #endregion

        #region Update Thumbnail On The Client

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object UpdateThumbnailImageSrc()
        {
            try
            {
                if (ServiceHelper.DisplayMode != BookmarkingServiceHelper.BookmarkDisplayMode.AllBookmarks
                    && ServiceHelper.DisplayMode != BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark
                    && ServiceHelper.DisplayMode != BookmarkingServiceHelper.BookmarkDisplayMode.Favourites)
                {
                    return new {url = string.Empty, thumbnailUrl = string.Empty};
                }
                var bookmarkUrl = UpdateURL(ServiceHelper.BookmarkToAdd.URL);
                ThumbnailHelper.Instance.MakeThumbnail(bookmarkUrl, true, true, HttpContext.Current, TenantProvider.CurrentTenantID);

                var thumbnailUrlByDisplayMode = BookmarkingServiceHelper.GetThumbnailUrlForUpdate(bookmarkUrl);
                if (string.IsNullOrEmpty(thumbnailUrlByDisplayMode))
                {
                    return null;
                }

                var result = new {url = HttpUtility.HtmlDecode(bookmarkUrl), thumbnailUrl = thumbnailUrlByDisplayMode};
                return result;
            }
            catch
            {
                return null;
            }

        }

        #region Thumbnails

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void MakeThumbnail(string url)
        {
            ThumbnailHelper.Instance.MakeThumbnail(url, true, true, HttpContext.Current, TenantProvider.CurrentTenantID);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void GenerateAllThumbnails(bool overrideFlag)
        {
            BookmarkingServiceHelper.GenerateAllThumbnails(overrideFlag);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void UpdateBookmarkThumbnail(int bookmarkID)
        {
            BookmarkingServiceHelper.UpdateBookmarkThumbnail(bookmarkID);
        }

        #endregion


        #endregion

        #region Subscriptions

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool IsSubscribedOnRecentBookmarks()
        {
            return ServiceHelper.IsSubscribed(ServiceHelper.SubscriptionRecentBookmarkID, BookmarkingBusinessConstants.NotifyActionNewBookmark);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool IsSubscribedOnBookmarkComments()
        {
            return ServiceHelper.IsSubscribed(ServiceHelper.SubscriptionBookmarkCommentsID, BookmarkingBusinessConstants.NotifyActionNewComment);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void IsSubscribedToNewBookmarkWithTag()
        {

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void SubscribeOnRecentBookmarks()
        {
            ServiceHelper.Subscribe(ServiceHelper.SubscriptionRecentBookmarkID, BookmarkingBusinessConstants.NotifyActionNewBookmark);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void SubscribeOnBookmarkComments()
        {
            ServiceHelper.Subscribe(ServiceHelper.SubscriptionBookmarkCommentsID, BookmarkingBusinessConstants.NotifyActionNewComment);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void SubscribeToNewBookmarkWithTag()
        {

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void UnSubscribeOnRecentBookmarks()
        {
            ServiceHelper.UnSubscribe(ServiceHelper.SubscriptionRecentBookmarkID, BookmarkingBusinessConstants.NotifyActionNewBookmark);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void UnSubscribeOnBookmarkComments()
        {
            ServiceHelper.UnSubscribe(ServiceHelper.SubscriptionBookmarkCommentsID, BookmarkingBusinessConstants.NotifyActionNewComment);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void UnSubscribeToNewBookmarkWithTag()
        {

        }

        #endregion

        public string GetCreateBookmarkPageUrl()
        {
            return BookmarkingServiceHelper.GetCreateBookmarkPageUrl();
        }

        public bool ShowCreateBookmarkLink()
        {
            switch (ServiceHelper.DisplayMode)
            {
                case BookmarkingServiceHelper.BookmarkDisplayMode.AllBookmarks:
                    return true;
                case BookmarkingServiceHelper.BookmarkDisplayMode.Favourites:
                    return true;
                case BookmarkingServiceHelper.BookmarkDisplayMode.SearchByTag:
                    return true;
            }
            return false;
        }

        public bool PermissionCheckCreateBookmark()
        {
            return BookmarkingPermissionsCheck.PermissionCheckCreateBookmark();
        }

        public bool IsSelectedBookmarkDisplayMode()
        {
            return ServiceHelper.IsSelectedBookmarkDisplayMode();
        }
    }
}