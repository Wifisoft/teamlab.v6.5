using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Resources;
using ASC.Blogs.Core.Security;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Blogs
{
    public partial class Default : BasePage
    {
        #region Properties

        private int SelectedPage
        {
            get
            {
                int result;
                Int32.TryParse(Request.QueryString["page"], out result);
                if (result <= 0)
                    result = 1;
                return result;

            }
        }

        private int BlogsPerPage
        {
            get { return 5; }
        }

        public string GroupID
        {
            get { return Request.QueryString["groupID"]; }
        }

        public string UserID
        {
            get { return Request.QueryString["userID"]; }
        }

        public string TagName
        {
            get { return Request.QueryString["tagName"]; }
        }

        public string Search
        {
            get { return Request.QueryString["search"]; }
        }

        #endregion

        #region Methods

        protected override void PageLoad()
        {
            Guid? userId = null;
            if (!String.IsNullOrEmpty(UserID))
            {
                userId = Guid.NewGuid();
                try
                {
                    userId = new Guid(UserID);
                }
                catch
                {
                }
            }

            var postsQuery = new PostsQuery();
            mainContainer.BreadCrumbs.Add(new BreadCrumb {Caption = BlogsResource.AddonName, NavigationUrl = VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath)});
            if (userId.HasValue)
            {
                mainContainer.BreadCrumbs.Add(new BreadCrumb {Caption = DisplayUserSettings.GetFullUserName(userId.Value, false)});
                postsQuery.SetUser(userId.Value);
            }
            else if (!String.IsNullOrEmpty(TagName))
            {
                mainContainer.BreadCrumbs.Add(new BreadCrumb { Caption = HeaderStringHelper.GetHTMLSearchHeader(TagName) });
                postsQuery.SetTag(TagName);
            }
            else if (!String.IsNullOrEmpty(Search))
            {
                mainContainer.BreadCrumbs.Add(new BreadCrumb {Caption = HeaderStringHelper.GetHTMLSearchHeader(Search)});
                postsQuery.SetSearch(Search);
            }

            if (!IsPostBack)
            {
                var engine = BasePage.GetEngine();
                FillPosts(postsQuery, engine);

                InitSidePanel(engine, TagCloud);
                sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
                sideRecentActivity.ProductId = Product.CommunityProduct.ID;
                sideRecentActivity.ModuleId = ASC.Blogs.Core.Constants.ModuleID;

                base.InitSubscribers(actions);
            }

            this.Title = HeaderStringHelper.GetPageTitle(BlogsResource.AddonName, mainContainer.BreadCrumbs);
        }

        protected string QueryString(string excludeParamList)
        {
            var queryString = "&" + Request.QueryString.ToString();

            foreach (var excludeParamName in excludeParamList.Split(','))
            {
                var startPos = queryString.IndexOf("&" + excludeParamName + "=");
                if (startPos != -1)
                {
                    var endPos = queryString.IndexOf("&", startPos + 1);

                    if (endPos == -1)
                    {
                        queryString = queryString.Remove(startPos, queryString.Length - startPos);
                    }
                    else
                    {
                        queryString = queryString.Remove(startPos, endPos - startPos);
                    }
                }
            }
            return queryString.Trim('&');
        }


        private void FillPosts(PostsQuery query, BlogsEngine engine)
        {
            query
                .SetOffset((SelectedPage - 1)*BlogsPerPage)
                .SetCount(BlogsPerPage);

            SetTotalPostsCount(engine.GetPostsCount(query));
            var posts = engine.SelectPosts(query);
            FillSelectedPage(posts, engine);
        }

        private void FillSelectedPage(List<Post> posts, BlogsEngine engine)
        {
            if (posts == null || posts.Count == 0)
            {
                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("blog_icon.png", ASC.Blogs.Core.Constants.ModuleId),
                                                 Header = BlogsResource.EmptyScreenBlogCaption,
                                                 Describe = BlogsResource.EmptyScreenBlogText
                                             };

                if (CommunitySecurity.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID)), ASC.Blogs.Core.Constants.Action_AddPost)
                    && string.IsNullOrEmpty(UserID) && string.IsNullOrEmpty(Search))
                {
                    emptyScreenControl.ButtonHTML = String.Format("<a class='linkAddMediumText' href='addblog.aspx'>{0}</a>", BlogsResource.EmptyScreenBlogLink);
                }

                placeContent.Controls.Add(emptyScreenControl);
                return;
            }

            placeContent.Controls.Add(new Literal {Text = "<div>"});

            var post_with_comments = engine.GetPostsCommentsCount(posts);

            for (var i = 0; i < post_with_comments.Count; i++)
            {

                var post = post_with_comments[i].Value1;
                var commentCount = post_with_comments[i].Value2;

                var sb = new StringBuilder();

                sb.Append("<div class=\"" + (i%2 == 1 ? "tintLight" : "tintMedium") + " borderBase\" style=\"border-width: " + (i == 0 ? "1px" : "0") + " 0 1px 0;padding: 10px 14px;\">");

                sb.Append("<table class='BlogsTable' cellspacing='0' cellpadding='0' border='0'><tr><td valign='top'>");
                sb.Append("<div style='padding-top:4px;'>" + ImageHTMLHelper.GetLinkUserAvatar(post.UserID) + "</div>");
                sb.Append("</td><td><div class='longWordsBreak MainInfoBlock'>");

                sb.Append("<a href=\"viewblog.aspx?blogid=" + post.ID.ToString() + "\" class=\"linkHeaderLight\">" + HttpUtility.HtmlEncode(post.Title) + "</a>");

                sb.Append("<div class='BlockCreater'>");

                sb.Append("<a class='linkHeaderSmall' href='" + VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath) + "?userid=" + post.UserID + "'><span style='font-weight:normal;'>" + BlogsResource.BlogOfTitle + ":&nbsp;&nbsp;</span>" + DisplayUserSettings.GetFullUserName(post.UserID) + "</a>");

                sb.Append("</div>");
                sb.Append("<div >");
                sb.Append("<span class='textMediumDescribe' style='margin-right:5px;'>" + BlogsResource.PostedTitle + ":</span> " + CoreContext.UserManager.GetUsers(post.UserID).RenderProfileLink(Product.CommunityProduct.ID));
                sb.Append("<span class='textMediumDescribe'>&nbsp;&nbsp;" + post.Datetime.Ago() + "</span>");
                sb.Append("</div>");

                sb.Append("</div>");
                placeContent.Controls.Add(new Literal {Text = sb.ToString()});


                sb = new StringBuilder();

                sb.Append("<div class='longWordsBreak ContentBlock'>");

                sb.Append(HtmlUtility.GetPreview(post.Content, "<div style='margin-top:15px;'><a style='text-decoration:none;' href=\"viewblog.aspx?blogid=" + post.ID.ToString() + "\"><font style='text-decoration:underline;'>" + ASC.Blogs.Core.Resources.BlogsResource.ReadMoreLink + "</font><font style='font-size:14px;text-decoration:none;'>&nbsp;&#8594</font></a></div>", Product.CommunityProduct.ID));

                sb.Append("</div>");


                sb.Append("<div class='clearFix CommentsBlock'>");
                if (post.TagList.Count > 0)
                {
                    sb.Append("<div class=\"textMediumDescribe TagsBlock\">");
                    sb.Append("<img class=\"TagsImgBlock\" src=\"" + WebImageSupplier.GetAbsoluteWebPath("tags.png", BlogsSettings.ModuleID) + "\">");

                    var j = 0;
                    foreach (var tag in post.TagList)
                    {
                        if (j != 0)
                            sb.Append(", ");
                        j++;
                        sb.Append("<a style='margin-left:5px;' class=\"linkDescribe\" href=\"./?tagname=" + HttpUtility.UrlEncode(tag.Content) + "\">" + HttpUtility.HtmlEncode(tag.Content) + "</a>");
                    }

                    sb.Append("</div>");
                }

                sb.Append("<div class='CommentsLinkBlock'>");
                sb.Append("<a href='viewblog.aspx?blogid=" + post.ID + "#comments'>" + BlogsResource.CommentsTitle + ": " + commentCount.ToString() + "</a>");
                sb.Append("</div>");

                sb.Append("</div></td></tr></table>");

                sb.Append("</div>");


                placeContent.Controls.Add(new Literal {Text = sb.ToString()});
            }

            placeContent.Controls.Add(new Literal {Text = "</div>"});

        }

        #endregion

        private void SetTotalPostsCount(int count)
        {
            var pageNavigator = new PageNavigator
                                    {
                                        PageUrl = "./" + "?" + QueryString("page"),
                                        CurrentPageNumber = SelectedPage,
                                        EntryCountOnPage = BlogsPerPage,
                                        VisiblePageCount = 5,
                                        ParamName = "page",
                                        EntryCount = count
                                    };

            pageNavigatorHolder.Controls.Add(pageNavigator);
        }
    }
}