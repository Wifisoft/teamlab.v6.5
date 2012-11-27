using System;
using AjaxPro;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Pojo;
using ASC.Core;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Common.Util;
using System.Collections.Generic;

namespace ASC.Web.UserControls.Bookmarking
{
	[AjaxNamespace("CommentsUserControl")]
	public partial class CommentsUserControl : System.Web.UI.UserControl
	{

		#region Fields
		private BookmarkingServiceHelper ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

		public long BookmarkID { get; set; }

		#region Bookmark Comments
		private IList<Comment> _bookmarkComments;

		public IList<Comment> BookmarkComments
		{
			get
			{
				return _bookmarkComments ?? new List<Comment>();
			}
			set
			{
				_bookmarkComments = value;
			}
		} 
		#endregion

		public CommentsList Comments
		{
			get
			{
				CommentList = ServiceHelper.Comments;
				return ServiceHelper.Comments;
			}
			set
			{
				ServiceHelper.Comments = CommentList;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			InitComments();
		}

		public void InitComments()
		{
			ConfigureComments(CommentList);
			CommentList.Items = BookmarkingConverter.ConvertCommentList(BookmarkComments);
			Comments = CommentList;
		}

		private void ConfigureComments(CommentsList commentList)
		{
			CommonControlsConfigurer.CommentsConfigure(commentList);

			// add comments permission check		
			commentList.IsShowAddCommentBtn = BookmarkingPermissionsCheck.PermissionCheckCreateComment();
			commentList.CommentsCountTitle = BookmarkComments.Count.ToString();
			commentList.Simple = false;
			commentList.BehaviorID = "commentsObj";
			commentList.JavaScriptAddCommentFunctionName = "CommentsUserControl.AddComment";
			commentList.JavaScriptLoadBBcodeCommentFunctionName = "CommentsUserControl.LoadCommentBBCode";
			commentList.JavaScriptPreviewCommentFunctionName = "CommentsUserControl.GetPreview";
			commentList.JavaScriptRemoveCommentFunctionName = "CommentsUserControl.RemoveComment";
			commentList.JavaScriptUpdateCommentFunctionName = "CommentsUserControl.UpdateComment";
			commentList.FckDomainName = "bookmarking_comments";
			commentList.TotalCount = BookmarkComments.Count;
			commentList.ShowCaption = false;
			commentList.ObjectID = BookmarkID.ToString();
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse AddComment(string parrentCommentID, long bookmarkID, string text, string pid)
		{

			AjaxResponse resp = new AjaxResponse();
			resp.rs1 = parrentCommentID;


			Comment comment = new Comment();

			comment.Content = text;

			comment.Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow();
			comment.UserID = SecurityContext.CurrentAccount.ID;
			Guid parentID = Guid.Empty;
			try
			{
				if (!string.IsNullOrEmpty(parrentCommentID))
				{
					parentID = new Guid(parrentCommentID);
				}
			}
			catch
			{
				parentID = Guid.Empty;
			}
			comment.Parent = parentID.ToString();
			comment.BookmarkID = bookmarkID;
			comment.ID = Guid.NewGuid();

			ServiceHelper.AddComment(comment);

			resp.rs2 = GetOneCommentHtmlWithContainer(comment);

			return resp;
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public string LoadCommentBBCode(string commentID)
		{
			if (string.IsNullOrEmpty(commentID))
				return string.Empty;

			var comment = ServiceHelper.GetCommentById(commentID);
			return comment.Content;
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public string GetPreview(string text, string commentID)
		{
			return GetHTMLComment(text, commentID);
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public string RemoveComment(string commentID, string pid)
		{
			ServiceHelper.RemoveComment(commentID);

			return commentID;
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse UpdateComment(string commentID, string text, string pid)
		{
			AjaxResponse resp = new AjaxResponse();
			resp.rs1 = commentID;

			ServiceHelper.UpdateComment(commentID, text);


			resp.rs2 = text + ASC.Web.Controls.CodeHighlighter.GetJavaScriptLiveHighlight(true);

			return resp;
		}

		private string GetHTMLComment(string text, string commentID)
		{

			Comment comment = new Comment()
			{
				Content = text,
				Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
				UserID = SecurityContext.CurrentAccount.ID
			};

			if (!String.IsNullOrEmpty(commentID))
			{
				comment = ServiceHelper.GetCommentById(commentID);

				comment.Content = text;
				comment.Parent = string.Empty;
			}

			var defComment = new CommentsList();

			ConfigureComments(defComment);

			ASC.Web.Controls.CommentInfoHelper.CommentInfo ci = BookmarkingConverter.ConvertComment(comment, BookmarkingServiceHelper.GetCurrentInstanse().BookmarkToAdd.Comments);
			ci.IsEditPermissions = false;
			ci.IsResponsePermissions = false;

			bool isRoot = string.IsNullOrEmpty(comment.Parent) || comment.Parent.Equals(Guid.Empty.ToString(), StringComparison.CurrentCultureIgnoreCase);


			return ASC.Web.Controls.CommentInfoHelper.CommentsHelper.GetOneCommentHtmlWithContainer(
					defComment, ci, isRoot, false);

		}

		private string GetOneCommentHtmlWithContainer(Comment comment)
		{


			return ASC.Web.Controls.CommentInfoHelper.CommentsHelper.GetOneCommentHtmlWithContainer(
					Comments,
					BookmarkingConverter.ConvertComment(comment, BookmarkingServiceHelper.GetCurrentInstanse().BookmarkToAdd.Comments),
					comment.Parent.Equals(Guid.Empty.ToString(), StringComparison.CurrentCultureIgnoreCase),
					false);

		}
	}
}