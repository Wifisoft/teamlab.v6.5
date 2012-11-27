using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Recipients;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Web.Core.Users.Activity;
using IDaoFactory = ASC.Projects.Core.DataInterfaces.IDaoFactory;

namespace ASC.Projects.Engine
{
    public class MessageEngine
    {
        private readonly EngineFactory _engineFactory;
        private readonly IMessageDao _messageDao;
        private readonly ICommentDao _commentDao;

        public MessageEngine(IDaoFactory daoFactory, EngineFactory engineFactory)
        {
            _engineFactory = engineFactory;
            _messageDao = daoFactory.GetMessageDao();
            _commentDao = daoFactory.GetCommentDao();
        }

        public Message GetByID(int id)
        {
            var message = _messageDao.GetById(id);

            if (message != null)
                message.CommentsCount = _commentDao.GetCommentsCount(new List<ProjectEntity> {message}).FirstOrDefault();

            return message;
        }

        public List<Message> GetByProject(int projectID)
        {
            var messages = _messageDao.GetByProject(projectID)
                .Where(CanRead)
                .ToList();
            var commentsCount = _commentDao.GetCommentsCount(messages.ConvertAll(r => (ProjectEntity)r));

            return messages.Select((message, index) =>
            { 
                                                    message.CommentsCount = commentsCount[index];
                                                    return message;
            }).ToList();

        }

        public List<Message> GetMessages(int startIndex,int maxResult)
        {
            var messages = _messageDao.GetMessages(startIndex, maxResult)
                .Where(CanRead)
                .ToList();
            var commentsCount = _commentDao.GetCommentsCount(messages.Select(r => (ProjectEntity)r).ToList());

            return messages.Select((message, index) =>
            {
                message.CommentsCount = commentsCount[index];
                return message;
            }).ToList();
            
        }

        public List<Message> GetRecentMessages(int maxResult)
        {
            int offset = 0;
            var recentMessages = new List<Message>();
            var messages = _messageDao.GetRecentMessages(offset, maxResult)
                .Where(CanRead)
                .ToList();

            recentMessages.AddRange(messages);

            if (recentMessages.Count < maxResult)
            {
                do
                {
                    offset = offset + maxResult;
                    messages = _messageDao.GetRecentMessages(offset, maxResult);

                    if (messages.Count == 0)
                        return recentMessages;
                    else messages = messages
                        .Where(CanRead)
                        .ToList();

                    recentMessages.AddRange(messages);
                }
                while (recentMessages.Count < maxResult);
            }

            var commentsCount = _commentDao.GetCommentsCount(recentMessages.Select(r => (ProjectEntity)r).ToList());

            recentMessages = recentMessages.Select((message, index) =>
            {
                message.CommentsCount = commentsCount[index];
                return message;
            }).ToList();

            return recentMessages.Count == maxResult ? recentMessages : recentMessages.GetRange(0, maxResult);
        }

        public List<Message> GetRecentMessages(int maxResult, params int[] projectID)
        {
            int offset = 0;
            var recentMessages = new List<Message>();
            var messages = _messageDao.GetRecentMessages(offset, maxResult, projectID)
                .Where(CanRead)
                .ToList();

            recentMessages.AddRange(messages);

            if (recentMessages.Count < maxResult)
            {
                do
                {
                    offset = offset + maxResult;
                    messages = _messageDao.GetRecentMessages(offset, maxResult, projectID);

                    if (messages.Count == 0)
                        return recentMessages;
                    else messages = messages
                        .Where(CanRead)
                        .ToList();

                    recentMessages.AddRange(messages);
                }
                while (recentMessages.Count < maxResult);
            }

            return recentMessages.Count == maxResult ? recentMessages : recentMessages.GetRange(0, maxResult);
        }

        public List<Message> GetByFilter(TaskFilter filter)
        {
            var listMessages = new List<Message>();

            while (true)
            {
                var messages = _messageDao.GetByFilter(filter).Where(CanRead).ToList();

                var lastMessageIndex = messages.FindIndex(r => r.ID == filter.LastId);

                if (lastMessageIndex >= 0)
                    messages = messages.SkipWhile((r, index) => index <= lastMessageIndex).ToList();

                listMessages.AddRange(messages);

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listMessages = listMessages.Take((int)filter.Max).ToList();

                if (listMessages.Count == filter.Max || messages.Count == 0) break;

                if (listMessages.Count != 0)
                    filter.LastId = listMessages.Last().ID;

                filter.Offset += filter.Max;
            }

            var commentsCount = _commentDao.GetCommentsCount(listMessages.Select(r => (ProjectEntity)r).ToList());

            return listMessages.Select((message, index) =>
            {
                message.CommentsCount = commentsCount[index];
                return message;
            }).ToList();
        }
        public List<Message> GetUpdates(DateTime from, DateTime to)
        {
            return _messageDao.GetUpdates(from, to).Where(CanRead).ToList();
        }

        public bool IsExists(int id)
        {
            return _messageDao.IsExists(id);
        }

        public Message SaveOrUpdate(Message message,bool notify, IEnumerable<Guid> participant, IEnumerable<int > fileIds)
        {
            return SaveOrUpdate(message, notify, participant, fileIds, false);
        }

        public Message SaveOrUpdate(Message message, bool notify, IEnumerable<Guid> participant, IEnumerable<int> fileIds, bool isImport)
        {
            if (message == null) throw new ArgumentNullException("message");

            var isNew = message.ID == default(int);

            message.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            message.LastModifiedOn = TenantUtil.DateTimeNow();

            if (isNew)
            {
                if (message.CreateBy == default(Guid)) message.CreateBy = SecurityContext.CurrentAccount.ID;
                if (message.CreateOn == default(DateTime)) message.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreateMessage(message.Project);
                _messageDao.Save(message);
                TimeLinePublisher.Message(message, isImport ? EngineResource.ActionText_Imported : EngineResource.ActionText_Create, UserActivityConstants.ContentActionType, UserActivityConstants.NormalContent);
            }
            else
            {
                ProjectSecurity.DemandEdit(message);
                _messageDao.Save(message);
                TimeLinePublisher.Message(message, EngineResource.ActionText_Update, UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity, true);
            }

            var fileEngine = _engineFactory.GetFileEngine();
            if (fileIds != null)
            {
                foreach (var fileId in fileIds)
                {
                    fileEngine.AttachFileToMessage(message.ID,fileId);
                }
            }

            NotifyParticipiant(message, isNew, participant, fileEngine.GetMessageFiles(message), notify);

            return message;
        }

        public void Delete(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Project == null) throw new Exception("Project");

            ProjectSecurity.DemandEdit(message);
            TimeLinePublisher.Message(message, EngineResource.ActionText_Delete, UserActivityConstants.ActivityActionType, UserActivityConstants.SmallActivity);

            _messageDao.Delete(message.ID);

            String objectID = String.Format("{0}_{1}", message.UniqID, message.Project.ID);
            NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_NewCommentForMessage, objectID);
        }

        protected void NotifyParticipiant(Message message, bool isMessageNew, IEnumerable<Guid> participant, IEnumerable<Files.Core.File> uploadedFiles, bool sendNotify)
        {
            //Don't send anything if notifications are disabled
            if (_engineFactory.DisableNotifications) return;


            var subscriptionProvider = NotifySource.Instance.GetSubscriptionProvider();
            var recipientsProvider = NotifySource.Instance.GetRecipientsProvider();

            var objectId = message.NotifyId;
            var subscriptionRecipients = subscriptionProvider.GetRecipients(NotifyConstants.Event_NewCommentForMessage, objectId);
            var recipients = new HashSet<Guid>(participant) { SecurityContext.CurrentAccount.ID };

            foreach (var subscriptionRecipient in subscriptionRecipients)
            {
                var subscriptionRecipientId = new Guid(subscriptionRecipient.ID);
                if (!recipients.Contains(subscriptionRecipientId))
                {
                    subscriptionProvider.UnSubscribe(NotifyConstants.Event_NewCommentForMessage, objectId, subscriptionRecipient);
                }
            }
            var senders =  recipients.Select(r => recipientsProvider.GetRecipient(r.ToString()))
                .Where(r => r != null && !subscriptionProvider.IsUnsubscribe((IDirectRecipient)r, NotifyConstants.Event_NewCommentForMessage, objectId))
                .ToList();

            senders.ForEach(r => subscriptionProvider.Subscribe(NotifyConstants.Event_NewCommentForMessage, objectId, r));
            senders.RemoveAll(r => r.ID == SecurityContext.CurrentAccount.ID.ToString());

            if (sendNotify && senders.Any())
            {
                NotifyClient.Instance.SendAboutMessageAction(senders, message, isMessageNew, FileEngine.GetFileListInfoHashtable(uploadedFiles));
            }
        }

        public Comment SaveMessageComment(Message message, Comment comment)
        {
            ProjectSecurity.DemandRead(message);
            _engineFactory.GetCommentEngine().SaveOrUpdate(comment);
            NotifyComment(comment, message);
            TimeLinePublisher.Comment(message, EngineResource.ActionText_Add);
            return comment;
        }

        private void NotifyComment(Comment comment, Message message)
        {
            //Don't send anything if notifications are disabled
            if (_engineFactory.DisableNotifications) return;

            NotifyClient.Instance.SendNewComment(message, comment);

            var subscriptionProvider = NotifySource.Instance.GetSubscriptionProvider();
            var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());
            if (subscriptionProvider.IsUnsubscribe((IDirectRecipient)recipient, NotifyConstants.Event_NewCommentForMessage, message.NotifyId))
                return;

            var objects = new List<String>(subscriptionProvider.GetSubscriptions(NotifyConstants.Event_NewCommentForMessage, recipient));
            var subscribed = !String.IsNullOrEmpty(objects.Find(item => String.Compare(item, message.NotifyId, true) == 0));
            if (!subscribed)
            {
                subscriptionProvider.Subscribe(NotifyConstants.Event_NewCommentForMessage, message.NotifyId, recipient);
            }
        }

        public bool CanRead(Message message)
        {
            return ProjectSecurity.CanRead(message);
        }
    }
}