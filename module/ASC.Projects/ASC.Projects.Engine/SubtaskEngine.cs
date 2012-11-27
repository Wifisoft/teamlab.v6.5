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
    public class SubtaskEngine
    {
        private readonly EngineFactory _factory;
        private readonly ISubtaskDao _subtaskDao;
        private readonly ITaskDao _taskDao;

        public SubtaskEngine(IDaoFactory daoFactory, EngineFactory factory)
        {
            _factory = factory;
            _subtaskDao = daoFactory.GetSubtaskDao();
            _taskDao = daoFactory.GetTaskDao();
        }

        #region get 

        public List<Task> GetByDate(DateTime from, DateTime to)
        {
            var subtasks = _subtaskDao.GetUpdates(from, to).ToDictionary(x => x.Task, x => x);
            var ids = subtasks.Select(x => x.Value.Task).Distinct().ToList();
            var tasks = _taskDao.GetById(ids);
            foreach (var task in tasks)
            {
                Subtask subtask;
                subtasks.TryGetValue(task.ID, out subtask);
                task.SubTasks.Add(subtask);
            }
            return tasks;
        }

        public int GetSubtaskCount(int taskid, params TaskStatus[] statuses)
        {
            return _subtaskDao.GetSubtaskCount(taskid, statuses);
        }

        public int GetSubtaskCount(int taskid)
        {
            return _subtaskDao.GetSubtaskCount(taskid, null);
        }

        public Subtask GetById(int id)
        {
            return _subtaskDao.GetById(id);
        }

        #endregion

        #region Actions 

        public Subtask ChangeStatus(Task task, Subtask subtask, TaskStatus newStatus)
        {
            if (subtask == null) throw new Exception("subtask.Task");
            if (task == null) throw new ArgumentNullException("task");
            if (task.Status == TaskStatus.Closed) throw new Exception("task can't be closed");

            if (subtask.Status == newStatus) return subtask;

            ProjectSecurity.DemandEdit(task, subtask);
           
            switch (newStatus)
            {
                case TaskStatus.Closed:
                    TimeLinePublisher.Subtask(subtask, task, EngineResource.ActionText_Closed, UserActivityConstants.ActivityActionType, UserActivityConstants.ImportantActivity);
                    break;

                case TaskStatus.Open:
                    TimeLinePublisher.Subtask(subtask, task, subtask.Status == TaskStatus.Closed ? EngineResource.ActionText_Reopen : EngineResource.ActionText_Accept, UserActivityConstants.ActivityActionType, UserActivityConstants.SmallActivity);
                    break;
            }

            subtask.Status = newStatus;
            subtask.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            subtask.LastModifiedOn = TenantUtil.DateTimeNow();
            subtask.StatusChangedOn = TenantUtil.DateTimeNow();

            if (subtask.Responsible.Equals(Guid.Empty))
                subtask.Responsible = SecurityContext.CurrentAccount.ID;

            var objectID = task.UniqID + "_" + task.Project.ID;
            var senders = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForTask, objectID).ToList();
            senders.RemoveAll(r => r.ID == SecurityContext.CurrentAccount.ID.ToString());

            if (task.Status != TaskStatus.Closed && newStatus == TaskStatus.Closed && !_factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutSubTaskClosing(senders, task, subtask);

            if (task.Status != TaskStatus.Closed && newStatus == TaskStatus.Open && !_factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutSubTaskResumed(senders, task, subtask);

            return _subtaskDao.Save(subtask);
        }

        public Subtask SaveOrUpdate(Subtask subtask, Task task)
        {
            if (subtask == null) throw new Exception("subtask.Task");
            if (task == null) throw new ArgumentNullException("task");
            if (task.Status == TaskStatus.Closed) throw new Exception("task can't be closed");

            var isNew = subtask.ID == default(int); //Task is new

            subtask.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            subtask.LastModifiedOn = TenantUtil.DateTimeNow();

            if (isNew)
            {
                if (subtask.CreateBy == default(Guid)) subtask.CreateBy = SecurityContext.CurrentAccount.ID;
                if (subtask.CreateOn == default(DateTime)) subtask.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandEdit(task);
                subtask = _subtaskDao.Save(subtask);
                TimeLinePublisher.Subtask(subtask, task, EngineResource.ActionText_Create,
                                          UserActivityConstants.ContentActionType, UserActivityConstants.NormalContent);
            }
            else
            {
                //changed task
                ProjectSecurity.DemandEdit(task, GetById(subtask.ID));
                subtask = _subtaskDao.Save(subtask);
                TimeLinePublisher.Subtask(subtask, task, EngineResource.ActionText_Update,
                                          UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity);
            }

            var objectID = task.UniqID + "_" + task.Project.ID;
            var recipients = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForTask, objectID)
                .Where(r => r.ID != SecurityContext.CurrentAccount.ID.ToString() && r.ID != subtask.Responsible.ToString() && r.ID != subtask.CreateBy.ToString())
                .ToList();

            NotifySubtask(task, subtask, recipients, isNew);

            var senders = new HashSet<Guid> { subtask.Responsible, subtask.CreateBy };
            senders.Remove(Guid.Empty);

            foreach (var sender in senders)
            {
                _factory.GetTaskEngine().SubscribeToTask(task, sender);
            }

            return subtask;
        }

        private void NotifySubtask(Task task, Subtask subtask, List<IRecipient> recipients, bool isNew)
        {
            //Don't send anything if notifications are disabled
            if (_factory.DisableNotifications) return;

            if (isNew && recipients.Any())
            {
                NotifyClient.Instance.SendAboutSubTaskCreating(recipients, task, subtask);
            }

            if (subtask.Responsible.Equals(Guid.Empty) || subtask.Responsible.Equals(SecurityContext.CurrentAccount.ID)) return;

            if (!_factory.GetTaskEngine().IsUnsubscribedToTask(task, subtask.Responsible.ToString()))
                NotifyClient.Instance.SendAboutResponsibleBySubTask(subtask, task);
        }

        public void Delete(Subtask subtask, Task task)
        {
            if (subtask == null) throw new ArgumentNullException("subtask");
            if (task == null) throw new ArgumentNullException("task");

            ProjectSecurity.DemandEdit(task, subtask);
            _subtaskDao.Delete(subtask.ID);

            TimeLinePublisher.Subtask(subtask, task, EngineResource.ActionText_Delete, UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity);

            var objectID = task.UniqID + "_" + task.Project.ID;
            var recipients = NotifySource.Instance.GetSubscriptionProvider().GetRecipients(NotifyConstants.Event_NewCommentForTask, objectID)
                            .Where(r => r.ID != SecurityContext.CurrentAccount.ID.ToString()).ToList();

            if (recipients.Any())
            {
                NotifyClient.Instance.SendAboutSubTaskDeleting(recipients, task, subtask);
            }
        }

        #endregion
    }
}
