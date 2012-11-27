using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Notify;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.Services.NotifyService
{
    public class NotifyClient
    {
        private static NotifyClient instance;
        private readonly INotifyClient client;
        private readonly INotifySource source;

        public static NotifyClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(NotifyClient))
                    {
                        if (instance == null) instance = new NotifyClient(WorkContext.NotifyContext.NotifyService.RegisterClient(NotifySource.Instance), NotifySource.Instance);
                    }
                }
                return instance;
            }
        }

        public INotifyClient Client
        {
            get { return client; }
        }


        private NotifyClient(INotifyClient client, INotifySource source)
        {
            this.client = client;
            this.source = source;
        }


        public void SendInvaiteToProjectTeam(Guid userId, Project project)
        {
            var recipient = ToRecipient(userId);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_InviteToProject,
                    project.UniqID,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, project.Title),
                    ReplyToTagProvider.Message(project.ID));
            }
        }

        public void SendRemovingFromProjectTeam(Guid userId, Project project)
        {
            var recipient = ToRecipient(userId);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_RemoveFromProject,
                    project.UniqID,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, project.Title),
                    ReplyToTagProvider.Message(project.ID));
            }
        }

        public void SendMilestoneDeadline(Guid userID, Milestone milestone)
        {
            var recipient = ToRecipient(userID);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_MilestoneDeadline,
                    milestone.NotifyId,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, milestone.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, milestone.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, milestone.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, milestone.ID),
                    ReplyToTagProvider.Comment("project.milestone", milestone.ID.ToString()));
            }
        }

        public void SendAboutResponsibleByProject(Guid responsible, Project project)
        {
            var recipient = ToRecipient(responsible);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_ResponsibleForProject,
                    project.UniqID,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, project.Title),
                    new TagValue(NotifyConstants.Tag_AdditionalData, project.Description),
                    ReplyToTagProvider.Message(project.ID));
            }
        }

        public void SendAboutResponsibleByMilestone(Milestone milestone)
        {
            var recipient = ToRecipient(milestone.Responsible);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_ResponsibleForMilestone,
                    milestone.NotifyId,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, milestone.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, milestone.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, milestone.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, milestone.ID),
                    new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "MilestoneDescription", HttpUtility.HtmlEncode(milestone.Description) } }),
                    ReplyToTagProvider.Comment("project.milestone", milestone.ID.ToString()));
            }
        }

        public void SendAboutResponsibleByTask(List<IRecipient> recipients, Task task)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_ResponsibleForTask,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "TaskDescription", HttpUtility.HtmlEncode(task.Description) } }),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
        }

        public void SendAboutResponsibleBySubTask(Subtask subtask, Task task)
        {
            var recipient = ToRecipient(subtask.Responsible);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_ResponsibleForSubTask,
                    task.NotifyId,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                    new TagValue(NotifyConstants.Tag_SubEntityTitle, subtask.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                    new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "TaskDescription", HttpUtility.HtmlEncode(task.Description) } }),
                    ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
            }
        }

        public void SendReminderAboutTask(List<IRecipient> recipients, Task task)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_ReminderAboutTask,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "TaskDescription", HttpUtility.HtmlEncode(task.Description) } }),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
        }

        public void SendReminderAboutTaskDeadline(List<IRecipient> recipients, Task task)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_ReminderAboutTaskDeadline,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData,
                             new Hashtable
                                 {
                                     {"TaskDescription", HttpUtility.HtmlEncode(task.Description)},
                                     {"TaskDeadline", task.Deadline.ToString()}
                                 }),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
        }

        public void SendNewComment(ProjectEntity entity, Comment comment)
        {
            INotifyAction action;
            if (entity.GetType() == typeof(Issue)) action = NotifyConstants.Event_NewCommentForIssue;
            else if (entity.GetType() == typeof(Message)) action = NotifyConstants.Event_NewCommentForMessage;
            else if (entity.GetType() == typeof(Milestone)) action = NotifyConstants.Event_NewCommentForMilestone;
            else if (entity.GetType() == typeof(Task)) action = NotifyConstants.Event_NewCommentForTask;
            else return;

            var interceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            try
            {
                client.AddInterceptor(interceptor);
                client.SendNoticeAsync(
                    action,
                    entity.NotifyId,
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, entity.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, entity.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, entity.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, entity.ID),
                    new TagValue(NotifyConstants.Tag_AdditionalData, comment.Content),
                    GetReplyToEntityTag(entity, comment));
            }
            finally
            {
                client.RemoveInterceptor(interceptor.Name);
            }
        }

        public void SendAboutMilestoneClosing(List<IRecipient> recipients, Milestone milestone)
        {
            client.BeginSingleRecipientEvent("milestone closed");
            var interceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            client.AddInterceptor(interceptor);
            try
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_MilestoneClosed,
                    milestone.NotifyId,
                    recipients.ToArray(),
                    GetDefaultSenders(recipients.FirstOrDefault()),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, milestone.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, milestone.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, milestone.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, milestone.ID),
                    new TagValue(NotifyConstants.Tag_AdditionalData, HttpUtility.HtmlEncode(milestone.Description)),
                    ReplyToTagProvider.Comment("project.milestone", milestone.ID.ToString()));
            }
            finally
            {
                client.RemoveInterceptor(interceptor.Name);
                client.EndSingleRecipientEvent("milestone closed");
            }
        }

        public void SendAboutTaskClosing(List<IRecipient> recipients, Task task)
        {
            client.BeginSingleRecipientEvent("task closed");
            var interceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            client.AddInterceptor(interceptor);
            try
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_TaskClosed,
                    task.NotifyId,
                    recipients.ToArray(),
                    GetDefaultSenders(recipients.FirstOrDefault()),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                    new TagValue(NotifyConstants.Tag_AdditionalData, HttpUtility.HtmlEncode(task.Description)),
                    ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
            }
            finally
            {
                client.RemoveInterceptor(interceptor.Name);
                client.EndSingleRecipientEvent("task closed");
            }
        }

        public void SendAboutSubTaskClosing(List<IRecipient> recipients, Task task, Subtask subtask)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_SubTaskClosed,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_SubEntityTitle, subtask.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
        }

        public void SendAboutTaskDeleting(List<IRecipient> recipients, Task task)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_TaskDeleted,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData, HttpUtility.HtmlEncode(task.Description)),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString())
                );
        }

        public void SendAboutSubTaskDeleting(List<IRecipient> recipients, Task task, Subtask subtask)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_SubTaskDeleted,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_SubEntityTitle, subtask.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "TaskDescription", HttpUtility.HtmlEncode(task.Description) } }),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));

        }

        public void SendAboutMilestoneCreating(List<IRecipient> recipients, Milestone milestone)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_MilestoneCreated,
                milestone.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, milestone.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, milestone.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, milestone.Title),
                new TagValue(NotifyConstants.Tag_EntityID, milestone.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "MilestoneDescription", HttpUtility.HtmlEncode(milestone.Description) } }),
                ReplyToTagProvider.Comment("project.milestone", milestone.ID.ToString())
                );
        }

        public void SendAboutTaskCreating(List<IRecipient> recipients, Task task)
        {
            var resp = "Nobody";

            if (task.Responsibles.Count != 0)
            {
                var recip = task.Responsibles.Select(r => ToRecipient(r)).Where(r => r != null);
                resp = recip.Select(r => r.Name).Aggregate((a, b) => a + ", " + b);
            }

            client.SendNoticeToAsync(
                NotifyConstants.Event_TaskCreated,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_Responsible, resp),
                new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "TaskDescription", HttpUtility.HtmlEncode(task.Description) } }),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString())
                );

        }
        public void SendAboutMilestoneEditing(Milestone milestone)
        {
            var recipient = ToRecipient(milestone.Responsible);

            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_MilestoneEdited,
                    milestone.NotifyId,
                    new[] { recipient },
                    GetDefaultSenders(recipient),
                    null,
                    new TagValue(NotifyConstants.Tag_ProjectID, milestone.Project.ID),
                    new TagValue(NotifyConstants.Tag_ProjectTitle, milestone.Project.Title),
                    new TagValue(NotifyConstants.Tag_EntityTitle, milestone.Title),
                    new TagValue(NotifyConstants.Tag_EntityID, milestone.ID),
                    ReplyToTagProvider.Comment("project.milestone", milestone.ID.ToString())
                    );
            }
        }

        public void SendAboutTaskEditing(List<IRecipient> recipients, Task task)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_TaskEdited,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString())
                );

        }

        public void SendAboutSubTaskCreating(List<IRecipient> recipients, Task task, Subtask subtask)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_SubTaskCreated,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_SubEntityTitle, subtask.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_Responsible, !subtask.Responsible.Equals(Guid.Empty) ? ToRecipient(subtask.Responsible).Name : "Nobody"),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString())
                );

        }

        public void SendAboutTaskResumed(List<IRecipient> recipients, Task task)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_TaskResumed,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                new TagValue(NotifyConstants.Tag_AdditionalData, HttpUtility.HtmlEncode(task.Description)),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
        }

        public void SendAboutSubTaskResumed(List<IRecipient> recipients, Task task, Subtask subtask)
        {
            client.SendNoticeToAsync(
                NotifyConstants.Event_SubTaskResumed,
                task.NotifyId,
                recipients.ToArray(),
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, task.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, task.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
                new TagValue(NotifyConstants.Tag_SubEntityTitle, subtask.Title),
                new TagValue(NotifyConstants.Tag_EntityID, task.ID),
                ReplyToTagProvider.Comment("project.task", task.ID.ToString()));
        }

        private static TagValue GetReplyToEntityTag(ProjectEntity entity, Comment comment)
        {
            string type = string.Empty;
            if (entity is Task)
            {
                type = "project.task";
            }
            if (entity is Message)
            {
                type = "project.message";
            }
            if (entity is Milestone)
            {
                type = "project.milestone";
            }
            if (!string.IsNullOrEmpty(type))
            {
                return ReplyToTagProvider.Comment(type, entity.ID.ToString(), comment != null ? comment.ID.ToString() : null);
            }
            return null;
        }

        public void SendAboutMessageAction(List<IRecipient> recipients, Message message, bool isNew, Hashtable fileListInfoHashtable)
        {
            client.SendNoticeToAsync(
                isNew ? NotifyConstants.Event_MessageCreated : NotifyConstants.Event_MessageEdited,
                message.NotifyId,
                recipients.ToArray(), 
                GetDefaultSenders(recipients.FirstOrDefault()),
                null,
                new TagValue(NotifyConstants.Tag_ProjectID, message.Project.ID),
                new TagValue(NotifyConstants.Tag_ProjectTitle, message.Project.Title),
                new TagValue(NotifyConstants.Tag_EntityTitle, message.Title),
                new TagValue(NotifyConstants.Tag_EntityID, message.ID),
                new TagValue(NotifyConstants.Tag_EventType, isNew ? "NewMessage" : "EditMessage"),
                new TagValue(NotifyConstants.Tag_AdditionalData, new Hashtable { { "MessagePreview", message.Content }, { "Files", fileListInfoHashtable } }),
                ReplyToTagProvider.Comment("project.message", message.ID.ToString()));
        }

        public void SendAboutImportComplite(Guid user)
        {
            var recipient = ToRecipient(user);
            if (recipient != null)
            {
                client.SendNoticeToAsync(
                    NotifyConstants.Event_ImportData, 
                    null,
                    new[] { recipient },
                    GetDefaultSenders(recipient), 
                    null, 
                    new ITagValue[0]);
            }
        }

        private string[] GetDefaultSenders(IRecipient recipient)
        {
            return source.GetSubscriptionProvider().GetSubscriptionMethod(
                NotifyConstants.Event_NewCommentForMessage,
                recipient);
        }

        private IRecipient ToRecipient(Guid userID)
        {
            return source.GetRecipientsProvider().GetRecipient(userID.ToString());
        }

        private string GetUserName(Guid id)
        {
            return UserFormatter.GetUserName(CoreContext.UserManager.GetUsers(id));
        }
    }
}