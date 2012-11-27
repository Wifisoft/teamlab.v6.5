using System;
using System.Web;
using ASC.Common.Security.Authorizing;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.Core.Users.Activity;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Blogs.Core
{
	public sealed class Constants
	{
		public static readonly Guid ModuleId = new Guid("{6A598C74-91AE-437d-A5F4-AD339BD11BB2}");

        public static readonly Action Action_AddPost = new Action(
                                                        new Guid("{948AD738-434B-4a88-8E38-7569D332910A}"),
                                                        "Add post"
													);

        public static readonly Action Action_EditRemovePost = new Action(
                                                        new Guid("{00E7DFC5-AC49-4fd3-A1D6-98D84E877AC4}"),
                                                        "Edit post"
													);

		public static readonly Action Action_AddComment = new Action(
                                                        new Guid("{388C29D3-C662-4a61-BF47-FC2F7094224A}"),
                                                        "Add comment"
													);

        public static readonly Action Action_EditRemoveComment = new Action(
                                                        new Guid("{724CBB75-D1C9-451e-BAE0-4DE0DB96B1F7}"),
                                                        "Edit comment"
													);

        public static INotifyAction NewPost = new NotifyAction("new post", "new post");
        public static INotifyAction NewPostByAuthor = new NotifyAction("new personal post", "new personal post");
        public static INotifyAction NewComment = new NotifyAction("new comment", "new comment");

		public const string _NewBlogSubscribeCategory = "{9C8ED95F-07D2-42d0-B241-C0A51F7D26D5}";

		public static ITag TagPostSubject = new Tag("PostSubject");
		public static ITag TagURL = new Tag("URL");

		public static ITag TagUserName = new Tag("UserName");
		public static ITag TagUserURL = new Tag("UserURL");
		public static ITag TagDate = new Tag("Date");

		public static ITag TagPostPreview = new Tag("PostPreview");

		public static ITag TagCommentBody = new Tag("CommentBody");
		public static ITag TagCommentURL = new Tag("CommentURL");

        public static Guid ModuleID = ASC.Blogs.Core.BlogsSettings.ModuleID;
        public const string NHibernateConfigPath = "~/products/community/modules/blogs/config/NHibernate.cfg.xml";
        public static string FullNHibernateConfigPath = string.Empty;

        public const int MAX_TEXT_LENGTH = 255;

        public static readonly bool SHOW_CORPORATE_BLOGS = false;


        #region UserActivityConstants
        
        public static int AddPersonalPostBusinessValue = UserActivityConstants.NormalContent;
        public static int AddCorporatePostBusinessValue = UserActivityConstants.ImportantActivity;
        public static int EditPostBusinessValue = UserActivityConstants.SmallActivity;
        public static int DeletePostBusinessValue = UserActivityConstants.SmallActivity;

        public static int AddCommentBusinessValue = UserActivityConstants.NormalActivity;
        public static int EditCommentBusinessValue = UserActivityConstants.SmallActivity;
        public static int DeleteCommentBusinessValue = UserActivityConstants.SmallActivity;

        public static int VoteBusinessValue = UserActivityConstants.SmallActivity;


        #endregion

        #region Virtual Path
        
        public const string BaseVirtualPath = "~/products/community/modules/blogs/";
        public static string GetModuleAbsolutePath(string virualPath)
        {
            if (virualPath == "" || virualPath == "/")
                virualPath = "./";

            return VirtualPathUtility.ToAbsolute(VirtualPathUtility.Combine(BaseVirtualPath,virualPath));
        }
        
        public static string DefaultPageUrl { get { return GetModuleAbsolutePath(""); } }
        public static string ViewBlogPageUrl { get { return GetModuleAbsolutePath("viewblog.aspx"); } }
        public static string UserPostsPageUrl { get { return GetModuleAbsolutePath("/")+"?userid="; } }
        
        #endregion
	}
}