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

namespace ASC.Projects.Engine
{
    public class MilestoneEngine
    {
        private readonly EngineFactory _engineFactory;
        private readonly IMilestoneDao milestoneDao;
        private readonly ITaskDao taskDao;

        public MilestoneEngine(IDaoFactory daoFactory, EngineFactory engineFactory)
        {
            _engineFactory = engineFactory;
            milestoneDao = daoFactory.GetMilestoneDao();
            taskDao = daoFactory.GetTaskDao();
        }

        public List<Milestone> GetByFilter(TaskFilter filter)
        {
            var listMilestones = new List<Milestone>(); 

            while (true)
            {
                var milestones = milestoneDao.GetByFilter(filter).FindAll(CanRead);

                if (filter.LastId != 0)
                {
                    var lastMilestoneIndex = milestones.FindIndex(r => r.ID == filter.LastId);

                    if (lastMilestoneIndex >= 0)
                    {
                        milestones = milestones.SkipWhile((r, index) => index <= lastMilestoneIndex).ToList();
                    }
                }

                listMilestones.AddRange(milestones);

                if (filter.ParticipantId.HasValue)
                {
                    var tasks = taskDao.GetByResponsible(filter.ParticipantId.Value, new[] { TaskStatus.Open });
                    listMilestones = listMilestones.FindAll(milestone => tasks.Any(task => task.Milestone == milestone.ID));
                }

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listMilestones = listMilestones.Take((int) filter.Max).ToList();

                if (listMilestones.Count == filter.Max || milestones.Count == 0) break;

                if (listMilestones.Count != 0)
                    filter.LastId = listMilestones.Last().ID;

                filter.Offset += filter.Max;
            }

            return listMilestones;
        }

        public List<Milestone> GetByProject(int projectId)
        {
            var milestones = milestoneDao.GetByProject(projectId).Where(CanRead).ToList();
            milestones.Sort((x, y) =>
            {
                if (x.Status != y.Status) return x.Status.CompareTo(y.Status);
                if (x.Status == MilestoneStatus.Open) return x.DeadLine.CompareTo(y.DeadLine);
                return y.DeadLine.CompareTo(x.DeadLine);
            });
            return milestones;
        }

        public List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus)
        {
            var milestones = milestoneDao.GetByStatus(projectId, milestoneStatus).Where(CanRead).ToList();
            milestones.Sort((x, y) =>
            {
                if (x.Status != y.Status) return x.Status.CompareTo(y.Status);
                if (x.Status == MilestoneStatus.Open) return x.DeadLine.CompareTo(y.DeadLine);
                return y.DeadLine.CompareTo(x.DeadLine);
            });
            return milestones;
        }

        public List<Milestone> GetUpcomingMilestones(int max, params int[] projects)
        {
            var offset = 0;
            var milestones = new List<Milestone>();
            while (true)
            {
                var packet = milestoneDao.GetUpcomingMilestones(offset, 2 * max, projects);
                milestones.AddRange(packet.Where(CanRead));
                if (max <= milestones.Count || packet.Count() < 2 * max)
                {
                    break;
                }
                offset += 2 * max;
            }
            return milestones.Count <= max ? milestones : milestones.GetRange(0, max);
        }

        public List<Milestone> GetLateMilestones(int max, params int[] projects)
        {
            var offset = 0;
            var milestones = new List<Milestone>();
            while (true)
            {
                var packet = milestoneDao.GetLateMilestones(offset, 2 * max, projects);
                milestones.AddRange(packet.Where(CanRead));
                if (max <= milestones.Count || packet.Count() < 2 * max)
                {
                    break;
                }
                offset += 2 * max;
            }
            return milestones.Count <= max ? milestones : milestones.GetRange(0, max);
        }

        public List<Milestone> GetByDeadLine(DateTime deadline)
        {
            return milestoneDao.GetByDeadLine(deadline).Where(CanRead).ToList();
        }

        /// <summary>
        /// Get milestones created or updated during period
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<Milestone> GetUpdates(DateTime from, DateTime to)
        {
            return milestoneDao.GetUpdates(from, to).Where(CanRead).ToList();
        }

        public Milestone GetByID(int id)
        {
            var m = milestoneDao.GetById(id);
            return CanRead(m) ? m : null;
        }

        public bool IsExists(int id)
        {
            return GetByID(id) != null;
        }

        public Milestone SaveOrUpdate(Milestone milestone)
        {
            return SaveOrUpdate(milestone, false, false);
        }

        public Milestone SaveOrUpdate(Milestone milestone, bool notifyResponsible)
        {
            return SaveOrUpdate(milestone, notifyResponsible, false);
        }

        public Milestone SaveOrUpdate(Milestone milestone, bool notifyResponsible, bool import)
        {
            if (milestone == null) throw new ArgumentNullException("milestone");
            if (milestone.Project == null) throw new Exception("milestone.project is null");

            milestone.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            milestone.LastModifiedOn = TenantUtil.DateTimeNow();

            var isNew = milestone.ID == default(int);//Task is new

            if (isNew)
            {
                if (milestone.CreateBy == default(Guid)) milestone.CreateBy = SecurityContext.CurrentAccount.ID;
                if (milestone.CreateOn == default(DateTime)) milestone.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreateMilestone(milestone.Project);
                milestone = milestoneDao.Save(milestone);
                TimeLinePublisher.Milestone(milestone, import ? EngineResource.ActionText_Imported : EngineResource.ActionText_Create, UserActivityConstants.ContentActionType, UserActivityConstants.ImportantContent);
            }
            else
            {
                ProjectSecurity.DemandEdit(milestone);
                milestone = milestoneDao.Save(milestone);
                TimeLinePublisher.Milestone(milestone, EngineResource.ActionText_Update, UserActivityConstants.ActivityActionType, UserActivityConstants.ImportantActivity);
            }


            if (!milestone.Responsible.Equals(Guid.Empty))
                NotifyMilestone(milestone, notifyResponsible, isNew);


            var objectId = String.Format("{0}_{1}", milestone.UniqID, milestone.Project.ID);
            NotifySource.Instance.GetSubscriptionProvider().Subscribe(NotifyConstants.Event_NewCommentForMilestone, objectId, NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()));
            return milestone;
        }

        public Milestone ChangeStatus(Milestone milestone, MilestoneStatus newStatus)
        {
            ProjectSecurity.DemandEdit(milestone);

            if (milestone == null) throw new ArgumentNullException("milestone");
            if (milestone.Project == null) throw new Exception("Project can be null.");
            if (milestone.Status == newStatus) return milestone;

            switch (newStatus)
            {
                case MilestoneStatus.Closed:
                    TimeLinePublisher.Milestone(milestone, EngineResource.ActionText_Closed, UserActivityConstants.ActivityActionType, UserActivityConstants.ImportantActivity);
                    break;


                case MilestoneStatus.Open:
                    TimeLinePublisher.Milestone(milestone, EngineResource.ActionText_Reopen, UserActivityConstants.ActivityActionType, UserActivityConstants.NormalActivity);
                    break;
            }


            milestone.Status = newStatus;
            milestone.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            milestone.LastModifiedOn = TenantUtil.DateTimeNow();
            milestone.StatusChangedOn = TenantUtil.DateTimeNow();

            var senders = new HashSet<Guid> { milestone.Project.Responsible, milestone.CreateBy, milestone.Responsible };
            senders.Remove(SecurityContext.CurrentAccount.ID);

            var recipientsProvider = NotifySource.Instance.GetRecipientsProvider();
            var recipients = senders.Select(r => recipientsProvider.GetRecipient(r.ToString())).Where(r => r != null).ToList();

            if (newStatus == MilestoneStatus.Closed && !_engineFactory.DisableNotifications && recipients.Count != 0)
            {
                NotifyClient.Instance.SendAboutMilestoneClosing(recipients, milestone);
            }

            return milestoneDao.Save(milestone);
        }

        private void NotifyMilestone(Milestone milestone, bool notifyResponsible, bool isNew)
        {
            //Don't send anything if notifications are disabled
            if (_engineFactory.DisableNotifications) return;

            if (isNew && milestone.Project.Responsible != SecurityContext.CurrentAccount.ID)
            {
                var users = new List<Guid> { milestone.Project.Responsible };
                var recipients = users.Select(r => NotifySource.Instance.GetRecipientsProvider().GetRecipient(r.ToString())).Where(r => r != null).ToList();
                if (recipients.Any())
                    NotifyClient.Instance.SendAboutMilestoneCreating(recipients,milestone);
            }

            if (notifyResponsible && milestone.Responsible != SecurityContext.CurrentAccount.ID)
            {
                if (isNew)
                    NotifyClient.Instance.SendAboutResponsibleByMilestone(milestone);
                else
                    NotifyClient.Instance.SendAboutMilestoneEditing(milestone);
            }
        }

        public void Delete(Milestone milestone)
        {
            if (milestone == null) throw new ArgumentNullException("milestone");

            ProjectSecurity.DemandEdit(milestone);
            milestoneDao.Delete(milestone.ID);
        }

        public Comment SaveMilestoneComment(Milestone milestone, Comment comment)
        {
            _engineFactory.GetCommentEngine().SaveOrUpdate(comment);
            NotifyNewComment(comment, milestone);
            TimeLinePublisher.Comment(milestone, EngineResource.ActionText_Add);
            return comment;
        }

        private void NotifyNewComment(Comment comment, Milestone milestone)
        {
            //Don't send anything if notifications are disabled
            if (_engineFactory.DisableNotifications) return;

            NotifyClient.Instance.SendNewComment(milestone, comment);

            var subscriptionProvider = NotifySource.Instance.GetSubscriptionProvider();
            var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());
            if (subscriptionProvider.IsUnsubscribe((IDirectRecipient)recipient, NotifyConstants.Event_NewCommentForMilestone, milestone.NotifyId))
                return;

            var objects = new List<String>(subscriptionProvider.GetSubscriptions(NotifyConstants.Event_NewCommentForMilestone, recipient));
            var subscribed = !String.IsNullOrEmpty(objects.Find(item => String.Compare(item, milestone.NotifyId, true) == 0));
            if (!subscribed)
            {
                subscriptionProvider.Subscribe(NotifyConstants.Event_NewCommentForMilestone, milestone.NotifyId, recipient);
            }
        }

        private bool CanRead(Milestone m)
        {
            return ProjectSecurity.CanRead(m);
        }
    }
}