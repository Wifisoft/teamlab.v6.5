using System;
using System.Collections.Generic;
using System.Web;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Core;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;
namespace ASC.Web.Community.Blogs
{
    [AjaxNamespace("ViewBlog")]
    public partial class ViewBlog : BasePage
    {
        #region Members

        private string BlogId
        {
            get
            {
                return Request.QueryString["blogID"];
            }
        }

        #endregion

        #region Methods

        protected override void PageLoad()
        {
            if (String.IsNullOrEmpty(BlogId))
                Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);

            Utility.RegisterTypeForAjax(typeof(ViewBlog), this.Page);

            var engine = BasePage.GetEngine();
            ShowPost(engine);
			
			InitSidePanel(engine, TagCloud);
			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = ASC.Blogs.Core.Constants.ModuleID;

            this.Title = HeaderStringHelper.GetPageTitle(ASC.Blogs.Core.Resources.BlogsResource.AddonName, mainContainer.BreadCrumbs);
        }

        private void ShowPost(BlogsEngine engine)
        {
            //EditBlogPresenter presenter = new EditBlogPresenter(ctrlViewBlogView, DaoFactory.GetBlogDao());
            //ctrlViewBlogView.AttachPresenter(presenter);

            ctrlViewBlogView.UpdateCompleted += new EventHandler(HandleUpdateCompleted);
            ctrlViewBlogView.UpdateCancelled += new EventHandler(HandleUpdateCancelled);

            if (!IsPostBack)
            {

                Post post = null;

                try
                {
                    post = engine.GetPostById(new Guid(BlogId));

                    base.InitSubscribers(actions, post.UserID);
                }
                catch (Exception)
                {
                    post = null;
                }

                if (post != null)
                {
                    ctrlViewBlogView.post = post;

                    mainContainer.BreadCrumbs.Add(new BreadCrumb { Caption = ASC.Blogs.Core.Resources.BlogsResource.AddonName, NavigationUrl = VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath ) });
                    mainContainer.BreadCrumbs.Add(new BreadCrumb { Caption = DisplayUserSettings.GetFullUserName(post.UserID, false), NavigationUrl = VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath) + "?userid=" + post.UserID });
                    mainContainer.BreadCrumbs.Add(new BreadCrumb { Caption = post.Title });

                    var loadedComments = engine.GetPostComments(post.ID);

                    commentList.Items = BuildCommentsList(post, loadedComments);


                    ConfigureComments(commentList, loadedComments.Count, post);

                    if (!SecurityContext.DemoMode)
                        engine.SavePostReview(post,SecurityContext.CurrentAccount.ID);
                }
                else
                {
                    ctrlViewBlogView.Visible = false;
                    lblMessage.Visible = true;
                    mainContainer.BreadCrumbs.Add(new BreadCrumb { Caption = ASC.Blogs.Core.Resources.BlogsResource.AddonName, NavigationUrl = VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath) });
                    commentList.Visible = false;
                    ConfigureComments(commentList, 0, null);
                }
            }
        }
        
        List<CommentInfo> BuildCommentsList(Post post, List<Comment> loaded)
        {
            return BuildCommentsList(post, loaded, Guid.Empty);
        }
        List<CommentInfo> BuildCommentsList(Post post, List<Comment> loaded, Guid parentId)
        {
            var result = new List<CommentInfo>();
            foreach (var comment in Comment.SelectChildLevel(parentId, loaded))
            {
                var info = new CommentInfo
                               {
                                   CommentID = comment.ID.ToString(),
                                   UserID = comment.UserID,
                                   TimeStamp = comment.Datetime,
                                   TimeStampStr = comment.Datetime.Ago(),
                                   IsRead = true,
                                   Inactive = comment.Inactive,
                                   CommentBody = comment.Content,
                                   UserFullName = DisplayUserSettings.GetFullUserName(comment.UserID),
                                   UserAvatar = ImageHTMLHelper.GetHTMLUserAvatar(comment.UserID),
                                   UserPost = CoreContext.UserManager.GetUsers(comment.UserID).Title,
                                   IsEditPermissions = CommunitySecurity.CheckPermissions(comment, ASC.Blogs.Core.Constants.Action_EditRemoveComment),
                                   IsResponsePermissions = CommunitySecurity.CheckPermissions(post, ASC.Blogs.Core.Constants.Action_AddComment),
                                   CommentList = BuildCommentsList(post, loaded, comment.ID)
                               };

                result.Add(info);
            }
            return result;
        }

        private static void ConfigureComments(CommentsList commentList, int totalCount, Post postToUpdate)
        {
            CommonControlsConfigurer.CommentsConfigure(commentList);

            commentList.IsShowAddCommentBtn = CommunitySecurity.CheckPermissions(postToUpdate, ASC.Blogs.Core.Constants.Action_AddComment);
            commentList.CommentsCountTitle = totalCount > 0 ? totalCount.ToString() : "";
            commentList.FckDomainName = 

            commentList.ObjectID = postToUpdate != null ? postToUpdate.ID.ToString() : "";
            commentList.Simple = false;
            commentList.BehaviorID = "commentsObj";
            commentList.JavaScriptAddCommentFunctionName = "ViewBlog.AddComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "ViewBlog.LoadCommentBBCode";
            commentList.JavaScriptPreviewCommentFunctionName = "ViewBlog.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "ViewBlog.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "ViewBlog.UpdateComment";
            commentList.FckDomainName = "blogs_comments";

            commentList.TotalCount = totalCount;
        }

        private string GetBlogTypeName(Blog blog)
        {
            if (blog.GroupID == new Guid())
            {
                return ASC.Blogs.Core.Resources.BlogsResource.InPersonalBlogLabel;
            }
            else
            {
                var g = CoreContext.GroupManager.GetGroupInfo(blog.GroupID);
                var name = g.ID != ASC.Core.Users.Constants.LostGroupInfo.ID ? g.Name : string.Empty;
                return ASC.Blogs.Core.Resources.BlogsResource.InGroupBlogLabel + " \"" + name + "\"";
            }
        }

        #region Ajax functions for comments management

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse AddComment(string parrentCommentID, string blogID, string text, string pid)
        {
            var postId = Guid.Empty;
            Guid? parentId = null;
            try
            {
                postId = new Guid(blogID);

                if (!String.IsNullOrEmpty(parrentCommentID))
                    parentId = new Guid(parrentCommentID);
            }
            catch
            {
                return new AjaxResponse();
            }

            var engine = BasePage.GetEngine();

            var resp = new AjaxResponse();
            resp.rs1 = parrentCommentID;

            var post = engine.GetPostById(postId);

            if (post == null)
            {
                return new AjaxResponse() { rs10 = "postDeleted", rs11 = ASC.Blogs.Core.Constants.DefaultPageUrl };
            }

            CommunitySecurity.DemandPermissions(post, ASC.Blogs.Core.Constants.Action_AddComment);

            var newComment = new Comment();

            newComment.PostId = postId;
            newComment.Content = text;
            newComment.Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            newComment.UserID = SecurityContext.CurrentAccount.ID;

            if (parentId.HasValue)
                newComment.ParentId = parentId.Value;

            engine.SaveComment(newComment, post);

            //mark post as seen for the current user
            engine.SavePostReview(post, SecurityContext.CurrentAccount.ID);
            resp.rs2 = GetHTMLComment(newComment, false);

            return resp;
        }

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UpdateComment(string commentID, string text, string pid)
        {
            AjaxResponse resp = new AjaxResponse();
            resp.rs1 = commentID;

            Guid? id = null;
            try
            {
                if (!String.IsNullOrEmpty(commentID))
                    id = new Guid(commentID);
            }
            catch
            {
                return new AjaxResponse();
            }
            
            BlogsEngine engine = BasePage.GetEngine();

            var comment = engine.GetCommentById(id.Value);
            if (comment == null)
                throw new ApplicationException("Comment not found");

            CommunitySecurity.DemandPermissions(comment, ASC.Blogs.Core.Constants.Action_EditRemoveComment);

            comment.Content = text;

            var post = engine.GetPostById(comment.PostId);
            engine.UpdateComment(comment, post);

            resp.rs2 = text + Web.Controls.CodeHighlighter.GetJavaScriptLiveHighlight(true);

            return resp;
        }

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string RemoveComment(string commentID, string pid)
        {
            AjaxResponse resp = new AjaxResponse();
            resp.rs1 = commentID;

            Guid? id = null;
            try
            {
                if (!String.IsNullOrEmpty(commentID))
                    id = new Guid(commentID);
            }
            catch
            {
                return commentID;
            }

            BlogsEngine engine = BasePage.GetEngine();

            var comment = engine.GetCommentById(id.Value);
            if (comment == null)
                throw new ApplicationException("Comment not found");

            CommunitySecurity.DemandPermissions(comment, ASC.Blogs.Core.Constants.Action_EditRemoveComment);

            comment.Inactive = true;

            var post = engine.GetPostById(comment.PostId);
            engine.RemoveComment(comment, post);

            return commentID;
        }

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string GetPreview(string text, string commentID)
        {
            return GetHTMLComment(text, commentID);
        }

        #endregion

        private string GetHTMLComment(Comment comment, bool isPreview)
        {
            CommentInfo info = new CommentInfo();

            info.CommentID = comment.ID.ToString();
            info.UserID = comment.UserID;
            info.TimeStamp = comment.Datetime;
            info.TimeStampStr = comment.Datetime.Ago();

            info.IsRead = true;
            info.Inactive = comment.Inactive;
            info.CommentBody = comment.Content;
            info.UserFullName = DisplayUserSettings.GetFullUserName(comment.UserID);
            info.UserAvatar = ImageHTMLHelper.GetHTMLUserAvatar(comment.UserID);
            info.UserPost = CoreContext.UserManager.GetUsers(comment.UserID).Title;

            if (!isPreview)
            {
                info.IsEditPermissions = CommunitySecurity.CheckPermissions(comment, ASC.Blogs.Core.Constants.Action_EditRemoveComment);

                info.IsResponsePermissions = CommunitySecurity.CheckPermissions(comment.Post, ASC.Blogs.Core.Constants.Action_AddComment);
            }
            var defComment = new CommentsList();
            ConfigureComments(defComment, 0, null);

            return Web.Controls.CommentInfoHelper.CommentsHelper.GetOneCommentHtmlWithContainer(
                    defComment,
                    info,
                    comment.IsRoot(),
                    false);
        }

        private string GetHTMLComment(string text, string commentID)
        {

            var comment = new Comment
            {
                Content = text,
                Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                UserID = SecurityContext.CurrentAccount.ID
            };

            if (!String.IsNullOrEmpty(commentID))
            {
                comment = BasePage.GetEngine().GetCommentById(new Guid(commentID));
                comment.Content = text;
            }

            return GetHTMLComment(comment, true);
        }

        #endregion

        #region Events

        private void HandleUpdateCancelled(object sender, EventArgs e)
        {
            Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
        }

        private void HandleUpdateCompleted(object sender, EventArgs e)
        {
            Response.Redirect("viewblog.aspx?blogid=" + BlogId);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {

        }

        #endregion        
    }
}