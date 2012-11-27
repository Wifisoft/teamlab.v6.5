#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Users;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects.Controls.Tasks
{
    [AjaxNamespace("AjaxPro.TaskDescriptionView")]
    public partial class TaskDescriptionView : BaseUserControl
    {
        #region Properties

        public Task Task { get; set; }
        public string TaskTimeSpend { get; set; }
        public int AttachmentsCount { get; set; }
        public bool CanReadFiles { get; set; }

        public bool CanEditTask { get; set; }

        public bool CanDeleteTask { get; set; }

        #endregion

        public void InitAttachments()
        {
            var attachments = FileEngine2.GetTaskFiles(Task);
            AttachmentsCount = attachments.Count;

            var taskAttachments = (Studio.UserControls.Common.Attachments.Attachments) LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            taskAttachments.EntityType = "task";
            taskAttachments.ModuleName = "projects";
            phAttachmentsControl.Controls.Add(taskAttachments);
        }

        public void InitEmptySubtasksPanel()
        {
            var emptyParticipantScreenControl = new Studio.Controls.Common.EmptyScreenControl
                                                    {
                                                        Header = TaskResource.Subtasks,
                                                        HeaderDescribe = TaskResource.EmptyListSubtasks,
                                                        ImgSrc = VirtualPathUtility.ToAbsolute("~/products/projects/App_Themes/Default/Images/subtasks-logo.png"),
                                                        Describe = TaskResource.SubtasksDescriptionForEmptyList,
                                                        ButtonHTML = string.Format("<span class='subtasksEmpty baseLinkAction'>{0}<span>", TaskResource.CreateFirstSubtask)
                                                    };
            _phEmptySubtasksPanel.Controls.Add(emptyParticipantScreenControl);
        }

        public void InitEmptyCommentsPanel()
        {
            var emptyParticipantScreenControl = new Studio.Controls.Common.EmptyScreenControl
                                                    {
                                                        Header = ProjectResource.Comments,
                                                        HeaderDescribe = ProjectResource.CommentsEmptyList,
                                                        ImgSrc = VirtualPathUtility.ToAbsolute("~/products/projects/App_Themes/Default/Images/massages-logo.png"),
                                                        Describe = ProjectResource.CommentsDescriptionEmptyList,
                                                        ButtonHTML = string.Format("<span class='commentsEmpty baseLinkAction'>{0}<span>", ProjectResource.AddFirstComment)
                                                    };
            _phEmptyCommentsPanel.Controls.Add(emptyParticipantScreenControl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopup.Options.IsPopup = true;
            _hintPopupTaskRemove.Options.IsPopup = true;

            var subtasks = (Subtasks)LoadControl(PathProvider.GetControlVirtualPath("Subtasks.ascx"));
            _subtasksTemplates.Controls.Add(subtasks);

            CanReadFiles = ProjectSecurity.CanReadFiles(Task.Project) && Global.ModuleManager.IsVisible(ModuleType.TMDocs);

            if (CanReadFiles)
                InitAttachments();

            InitEmptySubtasksPanel();
            InitEmptyCommentsPanel();
            InitCommentBlock();

            var timeList = Global.EngineFactory.GetTimeTrackingEngine().GetByTask(Task.ID);
            TaskTimeSpend = timeList.Sum(timeSpend => timeSpend.Hours).ToString();
            TaskTimeSpend = TaskTimeSpend.Replace(',', '.');

            Global.EngineFactory.GetParticipantEngine().Read(Page.Participant.ID, Task.UniqID, TenantUtil.DateTimeNow());
        }

        public bool CanCreateTimeSpend()
        {
            return Global.ModuleManager.IsVisible(ModuleType.TimeTracking)
                   && ProjectSecurity.CanCreateTimeSpend(Task.Project);
        }
        public bool CanCreateTask()
        {
                return ProjectSecurity.CanCreateTask(RequestContext.GetCurrentProject());
        }

        #region Comment List Control Block

        private void InitCommentBlock()
        {
            commentList.Items = BuilderCommentInfo();
            ConfigureComments(commentList, Task);
        }

        private IList<CommentInfo> BuilderCommentInfo()
        {
            var comments = Global.EngineFactory.GetCommentEngine().GetComments(Task).ToList();
            comments.Sort((x, y) => DateTime.Compare(x.CreateOn, y.CreateOn));

            return comments.Where(r => r.Parent == Guid.Empty).Select(comment => GetCommentInfo(comments, comment)).ToList();
        }

        private CommentInfo GetCommentInfo(IEnumerable<Comment> allComments, Comment parent)
        {
            var when = Global.EngineFactory.GetParticipantEngine().WhenReaded(Page.Participant.ID, Task.UniqID);
            var commentInfo = new CommentInfo
                                  {
                                      TimeStampStr = parent.CreateOn.Ago(),
                                      Inactive = parent.Inactive,
                                      IsRead = when.HasValue && parent.CreateOn < when.Value,
                                      IsResponsePermissions = ProjectSecurity.CanCreateComment(),
                                      IsEditPermissions = ProjectSecurity.CanEditComment(Task.Project, parent),
                                      CommentID = parent.ID.ToString(),
                                      CommentBody = parent.Content,
                                      UserID = parent.CreateBy,
                                      UserFullName = Global.EngineFactory.GetParticipantEngine().GetByID(parent.CreateBy).UserInfo.DisplayUserName(),
                                      UserPost = Global.EngineFactory.GetParticipantEngine().GetByID(parent.CreateBy).UserInfo.Title,
                                      UserAvatar = Global.GetHTMLUserAvatar(parent.CreateBy),
                                      CommentList = new List<CommentInfo>(),
                                  };

            if (allComments != null)
                foreach (var comment in allComments.Where(comment => comment.Parent == parent.ID))
                {
                    commentInfo.CommentList.Add(GetCommentInfo(allComments, comment));
                }

            return commentInfo;
        }

        #endregion

        #region Comment Block Managment

        [AjaxMethod]
        public AjaxResponse AddComment(string parrentCommentID, int taskID, string text, string pid)
        {
            if (!ProjectSecurity.CanCreateComment())
                throw ProjectSecurity.CreateSecurityException();

            var comment = new Comment
                              {
                                  Content = text,
                                  TargetUniqID = ProjectEntity.BuildUniqId<Task>(taskID)
                              };

            if (!String.IsNullOrEmpty(parrentCommentID))
                comment.Parent = new Guid(parrentCommentID);

            Task = Global.EngineFactory.GetTaskEngine().GetByID(taskID);
            Global.EngineFactory.GetTaskEngine().SaveOrUpdateTaskComment(Task, comment);

            return new AjaxResponse {rs1 = parrentCommentID, rs2 = GetHTMLComment(comment)};
        }

        private string GetHTMLComment(Comment comment)
        {
            var oCommentInfo = new CommentInfo
                                   {
                                       TimeStamp = comment.CreateOn,
                                       TimeStampStr = comment.CreateOn.Ago(),
                                       CommentBody = comment.Content,
                                       CommentID = comment.ID.ToString(),
                                       UserID = comment.CreateBy,
                                       UserFullName = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo.DisplayUserName(),
                                       Inactive = comment.Inactive,
                                       IsEditPermissions = ProjectSecurity.CanEditComment(Task != null ? Task.Project : null, comment),
                                       IsResponsePermissions = ProjectSecurity.CanCreateComment(),
                                       IsRead = true,
                                       UserAvatar = Global.GetHTMLUserAvatar(comment.CreateBy),
                                       UserPost = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo.Title

                                   };

            if (commentList == null)
            {
                commentList = new CommentsList();
                ConfigureComments(commentList, null);

            }

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                commentList,
                oCommentInfo,
                comment.Parent == Guid.Empty,
                false);

        }

        [AjaxMethod]
        public AjaxResponse UpdateComment(string commentID, string text, string pid)
        {
            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));
            var targetID = Convert.ToInt32(comment.TargetUniqID.Split('_')[1]);
            var target = Global.EngineFactory.GetTaskEngine().GetByID(targetID);
            var targetProject = target.Project;

            comment.Content = text;

            ProjectSecurity.DemandEditComment(targetProject, comment);
            Global.EngineFactory.GetTaskEngine().SaveOrUpdateTaskComment(target, comment);

            return new AjaxResponse {rs1 = commentID, rs2 = text + Web.Controls.CodeHighlighter.GetJavaScriptLiveHighlight(true)};

        }

        [AjaxMethod]
        public string RemoveComment(string commentID, string pid)
        {

            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));
            var targetID = Convert.ToInt32(comment.TargetUniqID.Split('_')[1]);
            var target = Global.EngineFactory.GetTaskEngine().GetByID(targetID);
            var targetProject = target.Project;

            ProjectSecurity.DemandEditComment(targetProject, comment);
            ProjectSecurity.DemandRead(target);

            comment.Inactive = true;

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
        public string LoadCommentBBCode(string commentID)
        {
            ProjectSecurity.DemandAuthentication();

            var finded = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));

            return finded != null ? finded.Content : String.Empty;
        }

        private string GetHTMLComment(string text, string commentID)
        {

            var comment = new Comment
                              {
                                  Content = text,
                                  CreateOn = TenantUtil.DateTimeNow(),
                                  CreateBy = SecurityContext.CurrentAccount.ID

                              };

            if (!String.IsNullOrEmpty(commentID))
            {
                comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));
                comment.Content = text;
            }

            return GetHTMLComment(comment, true);

        }

        private string GetHTMLComment(Comment comment, bool isPreview)
        {

            var info = new CommentInfo
                           {
                               CommentID = comment.ID.ToString(),
                               UserID = comment.CreateBy,
                               TimeStamp = comment.CreateOn,
                               TimeStampStr = comment.CreateOn.Ago(),
                               UserPost = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo.Title,
                               Inactive = comment.Inactive,
                               CommentBody = comment.Content,
                               UserFullName = DisplayUserSettings.GetFullUserName(Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy).UserInfo),
                               UserAvatar = Global.GetHTMLUserAvatar(comment.CreateBy)
                           };

            var defComment = new CommentsList();
            ConfigureComments(defComment, null);

            if (!isPreview)
            {
                var when = Global.EngineFactory.GetParticipantEngine().WhenReaded(Page.Participant.ID, Task.UniqID);
                info.IsRead = when.HasValue && when.Value > comment.CreateOn;
                info.IsEditPermissions = ProjectSecurity.CanEditComment(Task.Project, comment);
                info.IsResponsePermissions = ProjectSecurity.CanCreateComment();
            }

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                defComment,
                info,
                comment.Parent == Guid.Empty,
                false);

        }

        private static void ConfigureComments(CommentsList commentList, Task taskToUpdate)
        {
            var commentsCount = Global.EngineFactory.GetCommentEngine().Count(taskToUpdate);

            CommonControlsConfigurer.CommentsConfigure(commentList);

            commentList.IsShowAddCommentBtn = ProjectSecurity.CanCreateComment();

            commentList.CommentsCountTitle = commentsCount != 0 ? commentsCount.ToString() : "";

            commentList.ObjectID = taskToUpdate != null ? taskToUpdate.ID.ToString() : "";
            commentList.Simple = false;
            commentList.BehaviorID = "commentsObj";
            commentList.JavaScriptAddCommentFunctionName = "AjaxPro.TaskDescriptionView.AddComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "AjaxPro.TaskDescriptionView.LoadCommentBBCode";
            commentList.JavaScriptPreviewCommentFunctionName = "AjaxPro.TaskDescriptionView.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "AjaxPro.TaskDescriptionView.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "AjaxPro.TaskDescriptionView.UpdateComment";
            commentList.FckDomainName = "projects_comments";

            commentList.TotalCount = commentsCount;

        }

        #endregion
    }
}