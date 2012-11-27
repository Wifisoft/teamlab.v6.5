#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Users;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects.Controls.Messages
{
    [AjaxNamespace("AjaxPro.DiscussionDetails")]
    public partial class DiscussionDetails : BaseUserControl
    {
        public Message Discussion { get; set; }
        public Project Project { get; set; }
        public bool CanReadFiles { get; set; }
        public bool CanEdit { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(DiscussionDetails), Page);
            Global.EngineFactory.GetParticipantEngine().Read(Page.Participant.ID, Discussion.UniqID, TenantUtil.DateTimeNow());

            _hintPopup.Options.IsPopup = true;

            LoadEmptyCommentsControl();
            LoadCommentsControl();

            BindDiscussionParticipants();

            CanEdit = ProjectSecurity.CanEdit(Discussion);

            if (CanEdit)
                LoadDiscussionParticipantsSelector();

            CanReadFiles = ProjectSecurity.CanReadFiles(Discussion.Project) && Global.ModuleManager.IsVisible(ModuleType.TMDocs);

            if (CanReadFiles)
                LoadDiscussionFilesControl();
        }

        private void LoadEmptyCommentsControl()
        {
            var emptyCommentsControl = new Studio.Controls.Common.EmptyScreenControl
            {
                Header = ProjectResource.Comments,
                HeaderDescribe = ProjectResource.CommentsEmptyList,
                ImgSrc = VirtualPathUtility.ToAbsolute("~/products/projects/App_Themes/Default/Images/massages-logo.png"),
                Describe = MessageResource.CommentsEmptyListDescription,
                ButtonHTML = string.Format("<span id='addFirstCommentButton' class='baseLinkAction'>{0}<span>", ProjectResource.AddFirstComment)
            };
            emptyCommentsPlaceHolder.Controls.Add(emptyCommentsControl);
        }

        private void LoadCommentsControl()
        {
            discussionComments.Items = BuilderCommentInfo();
            ConfigureComments(discussionComments, Discussion);
        }

        private static void ConfigureComments(CommentsList commentList, Message messageToUpdate)
        {
            CommonControlsConfigurer.CommentsConfigure(commentList);

            commentList.IsShowAddCommentBtn = ProjectSecurity.CanCreateComment();
            commentList.CommentsCountTitle = messageToUpdate != null
                ? Global.EngineFactory.GetCommentEngine().Count(messageToUpdate).ToString(CultureInfo.InvariantCulture) : "";
            commentList.ObjectID = messageToUpdate != null
                ? messageToUpdate.ID.ToString(CultureInfo.InvariantCulture) : "";

            commentList.Simple = false;
            commentList.BehaviorID = "commentsObj";
            commentList.JavaScriptAddCommentFunctionName = "AjaxPro.DiscussionDetails.AddComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "AjaxPro.DiscussionDetails.LoadCommentBBCode";
            commentList.JavaScriptPreviewCommentFunctionName = "AjaxPro.DiscussionDetails.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "AjaxPro.DiscussionDetails.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "AjaxPro.DiscussionDetails.UpdateComment";
            commentList.FckDomainName = "projects_comments";
            commentList.TotalCount = messageToUpdate != null ? Global.EngineFactory.GetCommentEngine().Count(messageToUpdate) : 0;
        }

        private void BindDiscussionParticipants()
        {
            var participants = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForMessage,
                               string.Format("{0}_{1}", Discussion.UniqID, Discussion.Project.ID));
            discussionParticipantRepeater.DataSource = participants;
            discussionParticipantRepeater.DataBind();
        }

        private void LoadDiscussionParticipantsSelector()
        {
            var discussionParticipantsSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            discussionParticipantsSelector.BehaviorID = "discussionParticipantsSelector";
            discussionParticipantsSelector.DisabledUsers.Add(new Guid());
            discussionParticipantsSelector.Title = MessageResource.DiscussionParticipants;
            discussionParticipantsSelector.SelectedUserListTitle = MessageResource.DiscussionParticipants;

            discussionParticipantsSelectorHolder.Controls.Add(discussionParticipantsSelector);
        }

        private void LoadDiscussionFilesControl()
        {
            var discussionFilesControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            discussionFilesControl.EntityType = "message";
            discussionFilesControl.ModuleName = "projects";
            discussionFilesPlaceHolder.Controls.Add(discussionFilesControl);
        }

        protected string GetDiscussionTitle()
        {
            return Discussion.Title.HtmlEncode();
        }

        protected string GetUpdateDiscussionUrl()
        {
            return string.Format("messages.aspx?prjID={0}&id={1}&action=edit", Project.ID, Discussion.ID);
        }
        
        protected string GetNewDiscussionUrl()
        {
            return string.Format("messages.aspx?prjID={0}&action=add", Project.ID);
        }

        protected string GetDiscussionAuthorAvatarURL()
        {
            return Global.EngineFactory.GetParticipantEngine().GetByID(Discussion.CreateBy).UserInfo.GetBigPhotoURL();
        }

        protected string GetDiscussionAuthorName()
        {
            return CoreContext.UserManager.GetUsers(Discussion.CreateBy).DisplayUserName();
        }

        protected string GetDiscussionAuthorUrl()
        {
            return CoreContext.UserManager.GetUsers(Discussion.CreateBy).GetUserProfilePageURL(ProductEntryPoint.ID);
        }

        protected string GetDiscussionAuthorTitle()
        {
            return CoreContext.UserManager.GetUsers(Discussion.CreateBy).Title.HtmlEncode();
        }

        protected int GetDiscussionFilesCount()
        {
            var files = FileEngine2.GetMessageFiles(Discussion);
            return files.Count;
        }

        protected string GetDiscussionText()
        {
            return HtmlUtility.GetFull(Discussion.Content, ProductEntryPoint.ID);
        }
        
        protected string GetDiscussionParticipantLink(string id)
        {
            var projectParticipant = Global.EngineFactory.GetParticipantEngine().GetByID(new Guid(id));
            return projectParticipant.UserInfo.RenderProfileLink(ProductEntryPoint.ID);
        }

        protected string GetCurrentParticipantLink()
        {
            var projectParticipant = Global.EngineFactory.GetParticipantEngine().GetByID(SecurityContext.CurrentAccount.ID);
            return projectParticipant.UserInfo.RenderProfileLink(ProductEntryPoint.ID);
        }
        
        protected bool IsSubscribed()
        {
            var objectId = String.Format("{0}_{1}", Discussion.UniqID, Discussion.Project.ID);
            var objects = new List<String>(NotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(
              NotifyConstants.Event_NewCommentForMessage,
              NotifySource.Instance.GetRecipientsProvider().GetRecipient(
                  SecurityContext.CurrentAccount.ID.ToString())
              ));

            return !String.IsNullOrEmpty(objects.Find(item => String.Compare(item, objectId, StringComparison.OrdinalIgnoreCase) == 0));
        }

        protected string GetCommentsTabTitle()
        {
            return !discussionComments.CommentsCountTitle.Equals("0")
                       ? string.Format("{0}({1})", MessageResource.Comments, discussionComments.CommentsCountTitle)
                       : MessageResource.Comments;
        }

        protected string GetParticipantsTabTitle()
        {
            var participantsCount = discussionParticipantRepeater.Items.Count;
            return participantsCount > 0
                       ? string.Format("{0}({1})", MessageResource.DiscussionParticipants, participantsCount)
                       : MessageResource.DiscussionParticipants;
        }

        protected string GetFilesTabTitle()
        {
            var participantsCount = discussionParticipantRepeater.Items.Count;
            return participantsCount > 0
                       ? string.Format("{0}({1})", ProjectsCommonResource.PepleSubscribedToMessage, participantsCount)
                       : ProjectsCommonResource.PepleSubscribedToMessage;
        }


        [AjaxMethod]
        public String ChangeSubscribe(int discussionId)
        {
            ProjectSecurity.DemandAuthentication();

            var discussion = Global.EngineFactory.GetMessageEngine().GetByID(discussionId);

            var objectId = String.Format("{0}_{1}", discussion.UniqID, discussion.Project.ID);
            var senders = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForMessage, objectId).ToList();
            var subscribed = senders.Any(item => item.ID == SecurityContext.CurrentAccount.ID.ToString());

            if (subscribed)
            {
                NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_NewCommentForMessage, objectId, NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()));
                return ProjectsCommonResource.SubscribeOnNewComment;
            }

            NotifySource.Instance.GetSubscriptionProvider().Subscribe(NotifyConstants.Event_NewCommentForMessage, objectId, NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()));
            return ProjectsCommonResource.UnSubscribeOnNewComment;
        }

        [AjaxMethod]
        public AjaxResponse AddComment(string parrentCommentID, string messageID, string text, string pid)
        {
            if (!ProjectSecurity.CanCreateComment())
                throw ProjectSecurity.CreateSecurityException();

            var resp = new AjaxResponse();

            var comment = new Comment
            {
                Content = text,
                TargetUniqID = ProjectEntity.BuildUniqId<Message>(Convert.ToInt32(messageID))
            };

            resp.rs1 = parrentCommentID;

            if (!String.IsNullOrEmpty(parrentCommentID))
                comment.Parent = new Guid(parrentCommentID);

            Discussion = Global.EngineFactory.GetMessageEngine().GetByID(Convert.ToInt32(messageID));
            Global.EngineFactory.GetMessageEngine().SaveMessageComment(Discussion, comment);
            resp.rs2 = GetHTMLComment(comment);
            return resp;
        }

        private string GetHTMLComment(Comment comment)
        {
            var commentInfo = new CommentInfo
            {
                TimeStamp = comment.CreateOn,
                TimeStampStr = comment.CreateOn.Ago(),
                CommentBody = comment.Content,
                CommentID = comment.ID.ToString(),
                UserID = comment.CreateBy,
                UserFullName = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo.DisplayUserName(),
                Inactive = comment.Inactive,
                IsEditPermissions = ProjectSecurity.CanEditComment(Discussion != null ? Discussion.Project : null, comment),
                IsResponsePermissions = ProjectSecurity.CanCreateComment(),
                IsRead = true,
                UserAvatar = Global.GetHTMLUserAvatar(comment.CreateBy),
                UserPost = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo.Title
            };

            if (discussionComments == null)
            {
                discussionComments = new CommentsList();
                ConfigureComments(discussionComments, null);
            }

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                    discussionComments,
                    commentInfo,
                    comment.Parent == Guid.Empty,
                    false);
        }

        [AjaxMethod]
        public AjaxResponse UpdateComment(string commentID, string text, string pid)
        {
            var resp = new AjaxResponse { rs1 = commentID };

            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));

            comment.Content = text;

            var targetID = Convert.ToInt32(comment.TargetUniqID.Split('_')[1]);
            var target = Global.EngineFactory.GetMessageEngine().GetByID(targetID);
            var targetProject = target.Project;

            ProjectSecurity.DemandEditComment(targetProject, comment);

            Global.EngineFactory.GetMessageEngine().SaveMessageComment(target, comment);

            resp.rs2 = text + Web.Controls.CodeHighlighter.GetJavaScriptLiveHighlight(true);

            return resp;
        }

        [AjaxMethod]
        public string RemoveComment(string commentID, string pid)
        {
            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));

            comment.Inactive = true;

            var targetID = Convert.ToInt32(comment.TargetUniqID.Split('_')[1]);
            var target = Global.EngineFactory.GetMessageEngine().GetByID(targetID);
            var targetProject = target.Project;

            ProjectSecurity.DemandEditComment(targetProject, comment);
            ProjectSecurity.DemandRead(target);

            Global.EngineFactory.GetCommentEngine().SaveOrUpdate(comment);

            return commentID;
        }

        [AjaxMethod]
        public string GetPreview(string text, string commentID)
        {
            ProjectSecurity.DemandAuthentication();

            return GetHTMLComment(text, commentID);
        }

        [AjaxMethod]
        public string LoadCommentBBCode(string commentId)
        {
            ProjectSecurity.DemandAuthentication();

            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentId));
            return comment != null ? comment.Content : String.Empty;
        }


        private string GetHTMLComment(Comment comment, bool isPreview)
        {
            var commentInfo = new CommentInfo
            {
                CommentID = comment.ID.ToString(),
                UserID = comment.CreateBy,
                TimeStamp = comment.CreateOn,
                TimeStampStr = comment.CreateOn.Ago(),
                UserPost = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo.Title,
                IsRead = true,
                Inactive = comment.Inactive,
                CommentBody = comment.Content,
                UserFullName = DisplayUserSettings.GetFullUserName(
                       Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo),
                UserAvatar = Global.GetHTMLUserAvatar(comment.CreateBy)
            };

            var defComment = new CommentsList();
            ConfigureComments(defComment, null);

            if (!isPreview)
            {
                commentInfo.IsEditPermissions = ProjectSecurity.CanEditComment(Discussion.Project, comment);
                commentInfo.IsResponsePermissions = ProjectSecurity.CanCreateComment();

                var when = Global.EngineFactory.GetParticipantEngine().WhenReaded(Page.Participant.ID, Discussion.UniqID);
                commentInfo.IsRead = when.HasValue && when.Value > comment.CreateOn;
            }

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                    defComment,
                    commentInfo,
                    comment.Parent == Guid.Empty,
                    false);
        }

        private string GetHTMLComment(string text, string commentId)
        {
            Comment comment;
            if (!String.IsNullOrEmpty(commentId))
            {
                comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentId));
                comment.Content = text;
            }
            else
            {
                comment = new Comment
                {
                    Content = text,
                    CreateOn = TenantUtil.DateTimeNow(),
                    CreateBy = SecurityContext.CurrentAccount.ID
                };
            }
            return GetHTMLComment(comment, true);
        }

        private IList<CommentInfo> BuilderCommentInfo()
        {
            var comments = Global.EngineFactory.GetCommentEngine().GetComments(Discussion);
            comments.Sort((x, y) => DateTime.Compare(x.CreateOn, y.CreateOn));

            return (from comment in comments where comment.Parent == Guid.Empty select GetCommentInfo(comments, comment)).ToList();
        }

        private CommentInfo GetCommentInfo(IEnumerable<Comment> allComments, Comment parent)
        {
            var commentInfo = new CommentInfo
                                  {
                TimeStampStr = parent.CreateOn.Ago(),
                IsRead = true,
                Inactive = parent.Inactive,
                IsResponsePermissions = ProjectSecurity.CanCreateComment(),
                IsEditPermissions = ProjectSecurity.CanEditComment(Discussion.Project, parent),
                CommentID = parent.ID.ToString(),
                CommentBody = parent.Content,
                UserID = parent.CreateBy,
                UserFullName = Global.EngineFactory.GetParticipantEngine().GetByID(parent.CreateBy).UserInfo.DisplayUserName(),
                UserPost = Global.EngineFactory.GetParticipantEngine().GetByID(parent.CreateBy).UserInfo.Title,
                UserAvatar = Global.GetHTMLUserAvatar(parent.CreateBy),
                CommentList = new List<CommentInfo>(),
            };

            if (allComments != null)
            {
                foreach (var comment in allComments.Where(comment => comment.Parent == parent.ID))
                {
                    commentInfo.CommentList.Add(GetCommentInfo(allComments, comment));
                }
            }
            return commentInfo;
        }

        [AjaxMethod]
        public string ChangeDiscussionParticipants(string discussionId, string value)
        {
            ProjectSecurity.DemandAuthentication();

            if (string.IsNullOrEmpty(discussionId)) return string.Empty;
            var discussion = Global.EngineFactory.GetMessageEngine().GetByID(int.Parse(discussionId));
            var participants = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            Global.EngineFactory.GetMessageEngine().SaveOrUpdate(discussion, false, participants.Select(p => new Guid(p)), null);
            return ProjectsCommonResource.UnSubscribeOnNewComment;
        }
    }
}
