using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News
{
    [AjaxNamespace("Default")]
    public partial class Default : MainPage
    {
        //private ASC.Web.Controls.BBCodeParser.Parser postParser = new ASC.Web.Controls.BBCodeParser.Parser(CommonControlsConfigurer.SimpleTextConfig);

        private RequestInfo info;

        public RequestInfo Info
        {
            get
            {
                if (info == null) info = new RequestInfo(Request);
                return info;
            }
        }

        public int PageNumber
        {
            get { return ViewState["PageNumber"] != null ? Convert.ToInt32(ViewState["PageNumber"]) : 0; }
            set { ViewState["PageNumber"] = value; }
        }

        private List<BreadCrumb> Breadcrumb
        {
            get { return (Master as NewsMaster).BreadcrumbsControl; }
        }


        protected Uri FeedItemUrlWithParam
        {
            get { return new Uri("~/products/community/modules/news/editnews.aspx?docID=" + Info.UserIdAttribute, UriKind.Relative); }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof (Default), Page);
            commentList.Visible = CommunitySecurity.CheckPermissions(NewsConst.Action_Comment);

            pgNavigator.EntryCount = 1;
            pgNavigator.EntryCountOnPage = 1;
            Breadcrumb.Add(new BreadCrumb {Caption = NewsResource.NewsBreadCrumbs, NavigationUrl = FeedUrls.MainPageUrl});
            if (Info.HasUser)
            {
                Breadcrumb.Add(new BreadCrumb {Caption = Info.User.DisplayUserName(false), NavigationUrl = FeedUrls.GetFeedListUrl(Info.UserId)});
            }

            if (!IsPostBack)
            {
                var storage = FeedStorageFactory.Create();
                if (!string.IsNullOrEmpty(Request["docID"]))
                {
                    long docID;
                    if (long.TryParse(Request["docID"], out docID))
                    {
                        //Show panel
                        ContentView.Visible = false;
                        FeedView.Visible = true;

                        var feed = storage.GetFeed(docID);
                        if (feed != null)
                        {

                            if (!feed.Readed)
                            {
                                storage.ReadFeed(feed.Id, SecurityContext.CurrentAccount.ID.ToString());
                            }
                            FeedViewCtrl.Feed = feed;
                            hdnField.Value = feed.Id.ToString(CultureInfo.CurrentCulture);
                            Title += string.Format(CultureInfo.CurrentCulture, "-{0}", feed.Caption);
                            InitCommentsView(storage, feed);
                            FeedView.DataBind();
                            Breadcrumb.Add(new BreadCrumb {Caption = feed.Caption, NavigationUrl = FeedUrls.GetFeedUrl(docID, Info.UserId)});
                        }
                        else
                        {
                            ContentView.Visible = true;
                            FeedView.Visible = false;
                            FeedRepeater.Visible = true;
                        }
                    }
                }
                else
                {
                    PageNumber = string.IsNullOrEmpty(Request["page"]) ? 1 : Convert.ToInt32(Request["page"]);
                    LoadData();
                }
            }
            this.Title = HeaderStringHelper.GetPageTitle(Resources.NewsResource.AddonName, Breadcrumb);
        }

        private void InitCommentsView(IFeedStorage storage, Feed feed)
        {
            IList<CommentInfo> comments = new List<CommentInfo>();

            comments = BuildCommentsList(feed, storage.GetFeedComments(feed.Id));

            //AppendChildsComments(ref comments, storage.GetFeedComments(feed.Id));

            ConfigureComments(commentList, feed);
            commentList.Items = comments;
            commentList.CommentsCountTitle = GetCommentsCount(comments).ToString(CultureInfo.CurrentCulture);
            commentList.TotalCount = GetCommentsCount(comments);
        }

        private static void ConfigureComments(CommentsList commentList, Feed feed)
        {
            CommonControlsConfigurer.CommentsConfigure(commentList);
            commentList.Simple = false;
            commentList.BehaviorID = "_commentsObj";
            commentList.FckDomainName = "news_comments";

            commentList.JavaScriptAddCommentFunctionName = "Default.AddComment";
            commentList.JavaScriptPreviewCommentFunctionName = "Default.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "Default.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "Default.UpdateComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "Default.LoadCommentText";

            commentList.ObjectID = feed != null ? feed.Id.ToString(CultureInfo.CurrentCulture) : "";
        }

        private static int GetCommentsCount(ICollection<CommentInfo> comments)
        {
            var count = comments.Count;
            foreach (var info in comments)
            {
                count += GetCommentsCount(info.CommentList);
            }
            return count;
        }

        public static string GetHtmlImgUserAvatar(Guid userId)
        {
            var imgPath = UserPhotoManager.GetSmallPhotoURL(userId);
            if (imgPath != null) return "<img class=\"userMiniPhoto\"  src=\"" + imgPath + "\"/>";
            return string.Empty;
        }

        private CommentInfo GetCommentInfo(FeedComment comment)
        {
            var info = new CommentInfo
                           {
                               CommentID = comment.Id.ToString(CultureInfo.CurrentCulture),
                               UserID = new Guid(comment.Creator),
                               TimeStamp = comment.Date,
                               TimeStampStr = comment.Date.Ago(),
                               IsRead = true,
                               Inactive = comment.Inactive,
                               CommentBody = comment.Comment,
                               UserFullName = DisplayUserSettings.GetFullUserName(new Guid(comment.Creator)),
                               UserAvatar = GetHtmlImgUserAvatar(new Guid(comment.Creator)),
                               IsEditPermissions = CommunitySecurity.CheckPermissions(NewsConst.Action_Edit),
                               IsResponsePermissions = CommunitySecurity.CheckPermissions(NewsConst.Action_Comment),
                               UserPost = CoreContext.UserManager.GetUsers((new Guid(comment.Creator))).Title
                           };

            return info;
        }

        #region Ajax functions for comments management

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string RemoveComment(string commentId, string pid)
        {
            var storage = FeedStorageFactory.Create();
            var comment = storage.GetFeedComment(long.Parse(commentId, CultureInfo.CurrentCulture));
            comment.Inactive = true;
            storage.RemoveFeedComment(comment);
            return commentId;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse AddComment(string parentCommentId, string newsId, string text, string pid)
        {
            var resp = new AjaxResponse();
            resp.rs1 = parentCommentId;

            var comment = new FeedComment(long.Parse(newsId));
            comment.Comment = text;
            var storage = FeedStorageFactory.Create();
            if (!string.IsNullOrEmpty(parentCommentId))
                comment.ParentId = Convert.ToInt64(parentCommentId);

            var feed = storage.GetFeed(long.Parse(newsId, CultureInfo.CurrentCulture));
            comment = storage.SaveFeedComment(feed, comment);

            var info = GetCommentInfo(comment);
            var defComment = new CommentsList();
            ConfigureComments(defComment, feed);

            var visibleCommentsCount = 0;
            storage.GetFeedComments(feed.Id).ForEach((cmm) => { visibleCommentsCount += (cmm.Inactive ? 0 : 1); });

            resp.rs2 = CommentsHelper.GetOneCommentHtmlWithContainer(
                defComment, info, comment.IsRoot(), visibleCommentsCount%2 == 1);

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UpdateComment(string commentId, string text, string pid)
        {

            var resp = new AjaxResponse();
            resp.rs1 = commentId;
            if (text == null) return resp;
            var storage = FeedStorageFactory.Create();
            var comment = storage.GetFeedComment(long.Parse(commentId, CultureInfo.CurrentCulture));
            comment.Comment = text;
            storage.UpdateFeedComment(comment);


            resp.rs2 = text;

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string LoadCommentText(string commentId, string pid)
        {
            var storage = FeedStorageFactory.Create();
            var comment = storage.GetFeedComment(long.Parse(commentId, CultureInfo.CurrentCulture));
            return comment.Comment;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string GetPreview(string text, string commentID)
        {
            var storage = FeedStorageFactory.Create();

            var comment = new FeedComment(1)
                              {
                                  Date = TenantUtil.DateTimeNow(),
                                  Creator = SecurityContext.CurrentAccount.ID.ToString()
                              };

            if (!string.IsNullOrEmpty(commentID))
            {
                comment = storage.GetFeedComment(long.Parse(commentID, CultureInfo.CurrentCulture));
            }

            comment.Comment = text;

            var info = GetCommentInfo(comment);

            info.IsEditPermissions = false;
            info.IsResponsePermissions = false;

            var defComment = new CommentsList();
            ConfigureComments(defComment, null);

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                defComment, info, true, false);
        }

        #endregion

        private List<CommentInfo> BuildCommentsList(Feed feed, List<FeedComment> loaded)
        {
            return BuildCommentsList(feed, loaded, 0);
        }

        private List<CommentInfo> BuildCommentsList(Feed feed, List<FeedComment> loaded, long parentId)
        {
            var result = new List<CommentInfo>();
            foreach (var comment in FeedComment.SelectChildLevel(parentId, loaded))
            {
                var info = GetCommentInfo(comment);
                info.CommentList = BuildCommentsList(feed, loaded, comment.Id);

                result.Add(info);
            }
            return result;
        }

        private void LoadData()
        {
            var storage = FeedStorageFactory.Create();
            var feedType = FeedType.All;

            if (!string.IsNullOrEmpty(Request["type"]))
            {
                feedType = (FeedType) Enum.Parse(typeof (FeedType), Request["type"], true);
                var feedTypeInfo = FeedTypeInfo.FromFeedType(feedType);
                Breadcrumb.Add(new BreadCrumb
                                   {
                                       Caption = feedTypeInfo.TypeName,
                                       NavigationUrl = FeedUrls.GetFeedListUrl(feedType)
                                   });
                if (!string.IsNullOrEmpty(Request["search"]))
                {
                    Breadcrumb.Add(new BreadCrumb {Caption = HeaderStringHelper.GetHTMLSearchHeader(Request["search"]), NavigationUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") + "?type=" + Request["type"] + "&search=" + Request["search"] + Info.UserIdAttribute});
                }
            }
            else if (!string.IsNullOrEmpty(Request["search"]))
            {
                Breadcrumb.Add(new BreadCrumb {Caption = HeaderStringHelper.GetHTMLSearchHeader(Request["search"]), NavigationUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") + "?search=" + Request["search"] + Info.UserIdAttribute});
            }

            var feedsCount = !string.IsNullOrEmpty(Request["search"]) ? storage.SearchFeedsCount(Request["search"], feedType, Info.UserId) : storage.GetFeedsCount(feedType, Info.UserId);

            if (feedsCount == 0)
            {
                FeedRepeater.Visible = false;
                MessageShow.Visible = true;

                string buttonLink;
                string buttonName;
                var emptyScreenControl = new EmptyScreenControl {Describe = NewsResource.EmptyScreenText};

                switch (feedType)
                {
                    case FeedType.News:
                        emptyScreenControl.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_news.png", NewsConst.ModuleId);
                        emptyScreenControl.Header = NewsResource.EmptyScreenNewsCaption;
                        buttonLink = FeedUrls.EditNewsUrl;
                        buttonName = NewsResource.EmptyScreenNewsLink;
                        break;
                    case FeedType.Order:
                        emptyScreenControl.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_order.png", NewsConst.ModuleId);
                        emptyScreenControl.Header = NewsResource.EmptyScreenOrdersCaption;
                        buttonLink = FeedUrls.EditOrderUrl;
                        buttonName = NewsResource.EmptyScreenOrderLink;
                        break;
                    case FeedType.Advert:
                        emptyScreenControl.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_advert.png", NewsConst.ModuleId);
                        emptyScreenControl.Header = NewsResource.EmptyScreenAdvertsCaption;
                        buttonLink = FeedUrls.EditAdvertUrl;
                        buttonName = NewsResource.EmptyScreenAdvertLink;
                        break;
                    case FeedType.Poll:
                        emptyScreenControl.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_poll.png", NewsConst.ModuleId);
                        emptyScreenControl.Header = NewsResource.EmptyScreenPollsCaption;
                        buttonLink = FeedUrls.EditPollUrl;
                        buttonName = NewsResource.EmptyScreenPollLink;
                        break;
                    default:
                        emptyScreenControl.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_newslogo.png", NewsConst.ModuleId);
                        emptyScreenControl.Header = NewsResource.EmptyScreenEventsCaption;
                        buttonLink = FeedUrls.EditNewsUrl;
                        buttonName = NewsResource.EmptyScreenEventLink;
                        break;
                }

                if (CommunitySecurity.CheckPermissions(NewsConst.Action_Add) && String.IsNullOrEmpty(Request["uid"]) && String.IsNullOrEmpty(Request["search"]))
                    emptyScreenControl.ButtonHTML = String.Format("<a class='linkAddMediumText' href='{0}'>{1}</a>", buttonLink, buttonName);


                MessageShow.Controls.Add(emptyScreenControl);
            }
            else
            {
                const int pageSize = 20;
                var pageCount = (int) (feedsCount/pageSize + 1);
                if (pageCount < PageNumber) PageNumber = pageCount;

                var feeds = !string.IsNullOrEmpty(Request["search"]) ?
                                                                         storage.SearchFeeds(Request["search"], feedType, Info.UserId, pageSize, (PageNumber - 1)*pageSize) :
                                                                                                                                                                                storage.GetFeeds(feedType, Info.UserId, pageSize, (PageNumber - 1)*pageSize);

                pgNavigator.EntryCountOnPage = pageSize;
                pgNavigator.EntryCount = 0 < pageCount ? (int) feedsCount : pageSize;
                pgNavigator.CurrentPageNumber = PageNumber;

                pgNavigator.ParamName = "page";
                if (!string.IsNullOrEmpty(Request["search"]))
                {
                    pgNavigator.PageUrl = string.Format(CultureInfo.CurrentCulture, "{0}?search={1}", VirtualPathUtility.ToAbsolute("~/products/community/modules/news/"), Request["search"]);
                }
                else
                {
                    pgNavigator.PageUrl = string.IsNullOrEmpty(Request["type"]) ?
                                                                                    string.Format(CultureInfo.CurrentCulture, "{0}?{1}", VirtualPathUtility.ToAbsolute("~/products/community/modules/news/"), (string.IsNullOrEmpty(Info.UserIdAttribute) ? string.Empty : "?" + Info.UserIdAttribute.Substring(1))) :
                                                                                                                                                                                                                                                                                                                         string.Format(CultureInfo.CurrentCulture, "{0}?type={1}{2}", VirtualPathUtility.ToAbsolute("~/products/community/modules/news/"), Request["type"], Info.UserIdAttribute);
                }
                FeedRepeater.DataSource = feeds;
                FeedRepeater.DataBind();
            }
        }
    }
}