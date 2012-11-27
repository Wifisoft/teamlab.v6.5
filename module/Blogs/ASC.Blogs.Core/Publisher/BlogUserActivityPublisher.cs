using System;
using ASC.Blogs.Core.Domain;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users.Activity;

namespace ASC.Blogs.Core.Publisher
{
    public class BlogUserActivityPublisher: BaseUserActivityPublisher
    {

        internal static void PublishInternal(UserActivity activity)
        {
            UserActivityPublisher.Publish<BlogUserActivityPublisher>(activity);
        }

        internal static string GetContentID(Post post)
        {
            return String.Format("blog#{0}", post.ID);
        }

        internal static UserActivity ComposeActivityByPost(Post post)
        {
            UserActivity ua = new UserActivity();
            ua.ContentID = GetContentID(post);
            ua.Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            ua.ModuleID = BlogsSettings.ModuleID;
            ua.ProductID = ASC.Web.Community.Product.CommunityProduct.ID;
            ua.TenantID = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            ua.Title = post.Title;
            ua.URL = String.Format("{0}?blogID={1}", Constants.ViewBlogPageUrl, post.ID);

            return ua;
        }

        internal static UserActivity ApplyCustomeActivityParams(UserActivity ua, string actionText, Guid userID, int actionType, int businessValue)
        {
            ua.ActionText = actionText;
            ua.UserID = userID;
            ua.ActionType = actionType;
            ua.BusinessValue = businessValue;

            return ua;
        }
        
        #region posts activity

        public static void AddPost(Post post)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(post),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_AddPost,
                    post.UserID,
                    UserActivityConstants.ContentActionType,
                    Constants.AddPersonalPostBusinessValue 
                );
            ua.HtmlPreview = post.Content;
            PublishInternal(ua);
        }

        public static void EditPost(Post post, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(post),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_EditPost,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    Constants.EditPostBusinessValue);

            PublishInternal(ua);
        }

        public static void DeletePost(Post post, UserInfo author)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(post),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_DeletePost,
                    author.ID,
                    UserActivityConstants.ActivityActionType,
                    Constants.DeletePostBusinessValue);

            PublishInternal(ua);
        }

        public static void Voted(Post blog, Guid authorID)
        {
            var ua = 
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(blog),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_Vote,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    Constants.VoteBusinessValue);

            PublishInternal(ua);
        }

        #endregion


        #region comments activity

        public static void RemoveComment(Comment comment, Post post, Guid authorID)
        {
            var ua = 
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(post),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_DeleteComment,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    Constants.DeleteCommentBusinessValue);

            ua.URL += "#" + comment.ID;

            PublishInternal(ua);
        }

        public static void AddComment(Comment newComment,Post post)
        {
            var ua = 
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(post),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_AddComment,
                    newComment.UserID,
                    UserActivityConstants.ActivityActionType,
                    Constants.AddCommentBusinessValue);
            ua.URL += "#" + newComment.ID;
            ua.HtmlPreview = newComment.Content;

            PublishInternal(ua);
        }

        public static void UpdateComment(Comment comment, Post post, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPost(post),
                    ASC.Blogs.Core.Resources.BlogsResource.UserActivity_EditComment,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    Constants.EditCommentBusinessValue);
            ua.URL += "#" + comment.ID;

            PublishInternal(ua);
        }
        #endregion

    }
}
