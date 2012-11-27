using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
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
    public class TaskEngine
    {
        private readonly EngineFactory _factory;
        private readonly ITaskDao _taskDao;
        private readonly IMilestoneDao _milestoneDao;
        private readonly FileEngine _fileEngine;
        private readonly ISubtaskDao _subtaskDao;

        public TaskEngine(IDaoFactory daoFactory, EngineFactory factory)
        {
            _factory = factory;
            _taskDao = daoFactory.GetTaskDao();
            _milestoneDao = daoFactory.GetMilestoneDao();
            _fileEngine = factory.GetFileEngine();
            _subtaskDao = daoFactory.GetSubtaskDao();
        }


        public List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant)
        {
            var listTask = _taskDao.GetByProject(projectId, status, participant).Where(CanRead).ToList();
            _subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public List<Task> GetByFilter(TaskFilter filter)
        {
            var listTask = new List<Task>();
            var showAll = !(filter.Max > 0 && filter.Max < 150000);
            var taskStatus = filter.TaskStatuses.Any();

            if (!taskStatus)
            {
                filter.TaskStatuses.Add(TaskStatus.Open);
            }

            do
            {
                var tasks = _taskDao.GetByFilter(filter).FindAll(CanRead);
                var lastTaskIndex = tasks.FindIndex(r => r.ID == filter.LastId);

                if (lastTaskIndex >= 0)
                    tasks = tasks.SkipWhile((r, index) => index <= lastTaskIndex).ToList();


                listTask.AddRange(tasks);

                listTask = listTask.Take((int) filter.Max).ToList();

                if (listTask.Count != 0)
                    filter.LastId = listTask.Last().ID;

                if ((showAll || (listTask.Count < filter.Max || tasks.Count == 0)) && !taskStatus)
                {
                    filter.TaskStatuses.Clear();
                    taskStatus = true;
                    continue;
                }

                filter.Offset += filter.Max;

                if (tasks.Count == 0 || showAll) break;

            }
            while (listTask.Count < filter.Max);


            _subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public List<Task> GetByResponsible(Guid responsibleId)
        {
            var listTask = _taskDao.GetByResponsible(responsibleId, null).Where(CanRead).ToList();
            _subtaskDao.GetSubtasks(ref listTask);
            return listTask.ToList();
        }

        public List<Task> GetByResponsible(Guid responsibleId, params TaskStatus[] statuses)
        {
            var listTask = _taskDao.GetByResponsible(responsibleId, statuses).Where(CanRead).ToList();
            _subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public List<Task> GetLastTasks(Guid participant, int max)
        {
            var listTask = _taskDao.GetLastTasks(participant, max).Where(CanRead).ToList();
            _subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public List<Task> GetMilestoneTasks(int milestoneId)
        {
            var listTask = _taskDao.GetMilestoneTasks(milestoneId).Where(CanRead).ToList();
            _subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        /// <summary>
        /// Get tasks created or updated during period
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<Task> GetUpdates(DateTime from, DateTime to)
        {
            return _taskDao.GetUpdates(from, to).Where(CanRead).ToList();
        }

        /// <summary>
        /// Get subtasks created during time interval attached to parent task
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<Task> GetSubtaskUpdates(DateTime from, DateTime to)
        {
            var subtasks = _subtaskDao.GetUpdates(from, to);
            var ids = subtasks.Select(x => x.Task).Distinct().ToList();
            var tasks = _taskDao.GetById(ids).ToDictionary(x => x.ID, x => x);
            foreach (var subtask in subtasks)
            {
                Task task;
                tasks.TryGetValue(subtask.Task, out task);
                task.SubTasks.Add(subtask);
            }
            return tasks.Select(x => x.Value).Where(CanRead).ToList();
        }

        public Task GetByID(int id)
        {
            var task = _taskDao.GetById(id);

            if (task != null)
                task.SubTasks = _subtaskDao.GetSubtasks(task.ID);

            return CanRead(task) ? task : null;
        }

        public List<Task> GetByID(ICollection<int> ids)
        {
            var listTask = _taskDao.GetById(ids).Where(CanRead).ToList();
            _subtaskDao.GetSubtasks(ref listTask);
            return listTask;

        }

        public bool IsExists(int id)
        {
            return _taskDao.IsExists(id);
        }

        public int GetTaskCount(int milestoneId, params TaskStatus[] statuses)
        {
            return _taskDao.GetTaskCount(milestoneId, statuses);
        }

        public void SetTaskOrders(int? milestoneId, int taskID, int? prevTaskID, int? nextTaskID)
        {
            var task = _taskDao.GetById(taskID);

            if (task == null) return;

            ProjectSecurity.DemandEdit(task);
            _taskDao.SetTaskOrders(milestoneId, taskID, prevTaskID, nextTaskID);
        }

        public Task SaveOrUpdate(Task task, IEnumerable<int> attachedFileIds, bool notifyResponsible)
        {
            return SaveOrUpdate(task, attachedFileIds, notifyResponsible, false);
        }

        public Task SaveOrUpdate(Task task, IEnumerable<int> attachedFileIds, bool notifyResponsible, bool isImport)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("task.Project");

            var milestone = task.Milestone != 0 ? _milestoneDao.GetById(task.Milestone) : null;
            var milestoneResponsible = milestone != null ? milestone.Responsible : Guid.Empty;

            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();

            var isNew = task.ID == default(int); //Task is new

            if (isNew)
            {
                if (task.CreateBy == default(Guid)) task.CreateBy = SecurityContext.CurrentAccount.ID;
                if (task.CreateOn == default(DateTime)) task.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreateTask(task.Project);
                task = _taskDao.Save(task);

                TimeLinePublisher.Task(task, milestone,
                                       isImport ? EngineResource.ActionText_Imported : EngineResource.ActionText_Create,
                                       UserActivityConstants.ContentActionType, UserActivityConstants.NormalContent,
                                       true);
            }
            else
            {
                //changed task
                ProjectSecurity.DemandEdit(GetByID(new[] {task.ID}).FirstOrDefault());
                task = _taskDao.Save(task);
                TimeLinePublisher.Task(task, milestone, EngineResource.ActionText_Update, UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity);
            }

            if (attachedFileIds != null && attachedFileIds.Count() > 0)
            {
                foreach (var attachedFileId in attachedFileIds)
                {
                    _fileEngine.AttachFileToTask(task.ID, attachedFileId);
                }
            }

            //Object id for sender
            var objectID = task.UniqID + "_" + task.Project.ID;

            var senders = new HashSet<Guid>(task.Responsibles) { task.Project.Responsible, milestoneResponsible, task.CreateBy, task.Responsible };
            senders.Remove(Guid.Empty);

            var subscriptionProvider = NotifySource.Instance.GetSubscriptionProvider();
            var recipientsProvider = NotifySource.Instance.GetRecipientsProvider();

            var recipients = senders
                .Select(r => recipientsProvider.GetRecipient(r.ToString()))
                .Where(r => r!= null && !IsUnsubscribedToTask(task, r.ID))
                .ToList();

            foreach (var recipient in recipients)
            {
                subscriptionProvider.Subscribe(NotifyConstants.Event_NewCommentForTask, objectID, recipient);
            }

            recipients.RemoveAll(r => r.ID == SecurityContext.CurrentAccount.ID.ToString());

            if (isNew)
            {
                NotifyTaskCreated(task, recipients, milestoneResponsible, notifyResponsible);
            }
            else
            {
                NotifyTaskEdited(task, notifyResponsible);
            }


            return task;
        }

        public void NotifyTaskCreated(Task task, List<IRecipient> recipients, Guid milestoneResponsible, bool notifyResponsible)
        {
            if (_factory.DisableNotifications) return;

            var senders = recipients.FindAll(r =>r != null && (r.ID.Equals(task.Project.Responsible.ToString()) || r.ID.Equals(milestoneResponsible.ToString())));

            if (task.Responsibles != null)
                senders = senders.FindAll(r => !task.Responsibles.Contains(new Guid(r.ID)));

            if (senders.Any())
                NotifyClient.Instance.SendAboutTaskCreating(senders, task);

            if (notifyResponsible && task.Responsibles != null)
            {
                senders = recipients.FindAll(r => r != null && task.Responsibles.Contains(new Guid(r.ID))).ToList();
                if (senders.Any())
                    NotifyClient.Instance.SendAboutResponsibleByTask(senders, task);
            }
        }

        public void NotifyTaskEdited(Task task, bool notifyResponsible)
        {
            if (_factory.DisableNotifications) return;

            var objectID = String.Format("{0}_{1}", task.UniqID, task.Project.ID);
            var senders = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForTask, objectID).ToList();
            senders.RemoveAll(r => r.ID == SecurityContext.CurrentAccount.ID.ToString());

            if (notifyResponsible && senders.Any())
                NotifyClient.Instance.SendAboutTaskEditing(senders, task);
        }

        public void NotifyResponsible(Task task)
        {
            //Don't send anything if notifications are disabled
            if (_factory.DisableNotifications) return;

            if (task.Responsibles != null && !task.Responsible.Equals(Guid.Empty))
            {
                var recipientsProvider = NotifySource.Instance.GetRecipientsProvider();
                task.Responsibles.Remove(SecurityContext.CurrentAccount.ID);
                var senders = task.Responsibles
                    .Select(r => recipientsProvider.GetRecipient(r.ToString()))
                    .Where(r => r != null).ToList();

                if (senders.Count != 0)
                {
                    NotifyClient.Instance.SendReminderAboutTask(senders, task);
                }
            }
        }

        public void Delete(Task task)
        {
            if (task == null) throw new ArgumentNullException("task");

            ProjectSecurity.DemandEdit(task);
            task.SubTasks.ForEach(subTask => _subtaskDao.Delete(subTask.ID));
            _taskDao.Delete(task.ID);

            var milestone = task.Milestone != 0 ? _milestoneDao.GetById(task.Milestone) : null;
            TimeLinePublisher.Task(task, milestone, EngineResource.ActionText_Delete, UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity);

            var objectID = task.UniqID + "_" + task.Project.ID;
            var recipients = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForTask, objectID)
                            .Where(r=> r.ID != SecurityContext.CurrentAccount.ID.ToString()).ToList();

            if (recipients.Count != 0)
            {
                NotifyClient.Instance.SendAboutTaskDeleting(recipients, task);
            }
        }

        public Task ChangeStatus(Task task, TaskStatus newStatus)
        {
            ProjectSecurity.DemandEdit(task);

            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("Project can be null.");
            if (task.Status == newStatus) return task;

            var objectID = String.Format("{0}_{1}", task.UniqID, task.Project.ID);
            var milestone = task.Milestone != 0 ? _milestoneDao.GetById(task.Milestone) : null;

            switch (newStatus)
            {
                case TaskStatus.Closed:
                    TimeLinePublisher.Task(task, milestone, EngineResource.ActionText_Closed, UserActivityConstants.ActivityActionType, UserActivityConstants.ImportantActivity);
                    break;

                case TaskStatus.Open:
                    TimeLinePublisher.Task(task, milestone, task.Status == TaskStatus.Closed ? EngineResource.ActionText_Reopen : EngineResource.ActionText_Accept, UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity);
                    break;
            }

            var senders = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForTask, objectID).ToList();
            senders.RemoveAll(r => r.ID == SecurityContext.CurrentAccount.ID.ToString());

            if (newStatus == TaskStatus.Closed && !_factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutTaskClosing(senders, task);

            if (newStatus == TaskStatus.Open && !_factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutTaskResumed(senders, task);

            _taskDao.TaskTrace(task.ID, (Guid)CallContext.GetData("CURRENT_ACCOUNT"), TenantUtil.DateTimeNow(), newStatus);

            task.Status = newStatus;
            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();
            task.StatusChangedOn = TenantUtil.DateTimeNow();
            
            //subtask
            if(newStatus == TaskStatus.Closed)
            {
                if (task.Responsible.Equals(Guid.Empty))
                    task.Responsible = SecurityContext.CurrentAccount.ID;

                _subtaskDao.CloseAllSubtasks(task);
            }

            return _taskDao.Save(task);
        }

        public Comment SaveOrUpdateTaskComment(Task task, Comment comment)
        {
            ProjectSecurity.DemandRead(task);

            _factory.GetCommentEngine().SaveOrUpdate(comment);

            NotifyNewComment(comment, task);

            SubscribeToTask(task, SecurityContext.CurrentAccount.ID);

            TimeLinePublisher.Comment(task, EngineResource.ActionText_Add);

            return comment;
        }

        private void NotifyNewComment(Comment comment, Task task)
        {
            //Don't send anything if notifications are disabled
            if (_factory.DisableNotifications) return;

            NotifyClient.Instance.SendNewComment(task, comment);
        }

        public void SubscribeToTask(Task task, Guid recipientID)
        {
            var objectID = String.Format("{0}_{1}", task.UniqID, task.Project.ID);
            var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(recipientID.ToString());

            if (!IsUnsubscribedToTask(task, recipient.ID))
                NotifySource.Instance.GetSubscriptionProvider().Subscribe(NotifyConstants.Event_NewCommentForTask,objectID, recipient);
        }

        public void FollowTask(Task task)
        {
            var objectID = String.Format("{0}_{1}", task.UniqID, task.Project.ID);
            var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());

            if (recipient == null) return;

            if (!IsSubscribedToTask(task))
                NotifySource.Instance.GetSubscriptionProvider().Subscribe(NotifyConstants.Event_NewCommentForTask, objectID, recipient);
            else
                NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_NewCommentForTask, objectID, recipient);
        }

        public bool IsSubscribedToTask(Task task)
        {
            var objectID = String.Format("{0}_{1}", task.UniqID, task.Project.ID);

            var objects = new List<String>(NotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(
              NotifyConstants.Event_NewCommentForTask,
              NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString())
              ));

            return !String.IsNullOrEmpty(objects.Find(item => String.Compare(item, objectID, true) == 0));
        }

        public bool IsUnsubscribedToTask(Task task, string recipientID)
        {
            var objectID = String.Format("{0}_{1}", task.UniqID, task.Project.ID);
            var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(recipientID);

            if (recipient == null) return false;

            return NotifySource.Instance.GetSubscriptionProvider().IsUnsubscribe((IDirectRecipient) recipient, NotifyConstants.Event_NewCommentForTask, objectID);
        }

        private bool CanRead(Task task)
        {
            return ProjectSecurity.CanRead(task);
        }
    }
}