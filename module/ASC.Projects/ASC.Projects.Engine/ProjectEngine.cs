using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;

namespace ASC.Projects.Engine
{
    public class ProjectEngine
    {
        private readonly EngineFactory _factory;
        private readonly IProjectDao projectDao;
        private readonly IParticipantDao participantDao;

        public ProjectEngine(IDaoFactory daoFactory, EngineFactory factory)
        {
            _factory = factory;
            projectDao = daoFactory.GetProjectDao();
            participantDao = daoFactory.GetParticipantDao();
        }

        public virtual List<Project> GetAll()
        {
            return projectDao.GetAll(null, 0)
                .Where(CanRead)
                .ToList();
        }

        public virtual List<Project> GetAll(ProjectStatus status, int max)
        {
            return projectDao.GetAll(status, max)
                .Where(CanRead)
                .ToList();
        }

        public virtual List<Project> GetLast(ProjectStatus status, int max)
        {
            var offset = 0;
            var lastProjects = new List<Project>();
            var projects = projectDao.GetLast(status, offset, max)
                .Where(CanRead)
                .ToList();

            lastProjects.AddRange(projects);

            if (lastProjects.Count < max)
            {
                do
                {
                    offset = offset + max;
                    projects = projectDao.GetLast(status, offset, max);

                    if (projects.Count == 0)
                        return lastProjects;
                    else projects = projects
                        .Where(CanRead)
                        .ToList();

                    lastProjects.AddRange(projects);
                }
                while (lastProjects.Count < max);
            }

            return lastProjects.Count == max ? lastProjects : lastProjects.GetRange(0, max);
        }

        public virtual List<Project> GetByParticipant(Guid participant)
        {
            return projectDao.GetByParticipiant(participant, ProjectStatus.Open)
                .Where(CanRead)
                .ToList();
        }

        public virtual List<Project> GetByFilter(TaskFilter filter)
        {
            var listProjects = new List<Project>();

            while (true)
            {
                var projects = projectDao.GetByFilter(filter).Where(CanRead).ToList();

                if (filter.LastId != 0)
                {
                    var lastProjectIndex = projects.FindIndex(r => r.ID == filter.LastId);

                    if (lastProjectIndex >= 0)
                    {
                        projects = projects.SkipWhile((r, index) => index <= lastProjectIndex).ToList();
                    }
                }

                listProjects.AddRange(projects);

                if (filter.Max <= 0 || filter.Max>150000) break;

                listProjects = listProjects.Take((int) filter.Max).ToList();

                if (listProjects.Count == filter.Max || projects.Count == 0) break;


                if (listProjects.Count != 0)
                    filter.LastId = listProjects.Last().ID;

                filter.Offset += filter.Max;
            }

            return listProjects;
        }

        public virtual List<Project> GetFollowing(Guid participant)
        {
            return projectDao.GetFollowing(participant)
                .Where(CanRead)
                .ToList();
        }

        /// <summary>
        /// Get projects created or updated during period
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<Project> GetUpdates(DateTime from, DateTime to)
        {
            return projectDao.GetUpdates(from, to).Where(CanRead).ToList();
        }

        public Dictionary<int, DateTime> GetLastActivity(IEnumerable<string> projectIds)
        {
            return UserActivityManager.GetProjectsActivities(TenantProvider.CurrentTenantID, EngineFactory.ProductId, projectIds);
        }

        public virtual Project GetByID(int projectID)
        {
            var project = projectDao.GetById(projectID);
            return CanRead(project) ? project : null;
        }

        public virtual List<Project> GetByID(ICollection projectIDs)
        {
            return projectDao.GetById(projectIDs)
                .Where(CanRead)
                .ToList();
        }

        public virtual bool IsExists(int projectID)
        {
            return projectDao.IsExists(projectID);
        }

        private bool CanRead(Project project)
        {
            return ProjectSecurity.CanRead(project);
        }


        public virtual int Count()
        {
            return projectDao.Count();
        }

        public virtual int GetTaskCount(int projectId, params TaskStatus[] taskStatus)
        {
            return GetTaskCount(new List<int>() { projectId }, taskStatus)[0];
        }

        public virtual List<int> GetTaskCount(List<int> projectId, params TaskStatus[] taskStatus)
        {
            return projectDao.GetTaskCount(projectId, taskStatus);
        }

        public virtual int GetMilestoneCount(int projectId, params MilestoneStatus[] milestoneStatus)
        {
            return projectDao.GetMilestoneCount(projectId, milestoneStatus);
        }

        public virtual int GetMessageCount(int projectId)
        {
            return projectDao.GetMessageCount(projectId);
        }


        public Project SaveOrUpdate(Project project, bool notifyManager)
        {
            return SaveOrUpdate(project, notifyManager, false);
        }

        public virtual Project SaveOrUpdate(Project project, bool notifyManager, bool isImport)
        {
            if (project == null) throw new ArgumentNullException("project");

            project.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            project.LastModifiedOn = TenantUtil.DateTimeNow();

            if (project.ID == 0)
            {
                if (project.CreateBy == default(Guid)) project.CreateBy = SecurityContext.CurrentAccount.ID;
                if (project.CreateOn == default(DateTime)) project.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreateProject();
                projectDao.Save(project);

                TimeLinePublisher.Project(project,isImport ? EngineResource.ActionText_Imported: EngineResource.ActionText_Create, UserActivityConstants.ContentActionType, UserActivityConstants.Max);
            }
            else
            {
                ProjectSecurity.DemandEdit(project);
                projectDao.Save(project);

                if (!project.Private) ResetTeamSecurity(project);

                TimeLinePublisher.Project(project, EngineResource.ActionText_Update, UserActivityConstants.ActivityActionType, UserActivityConstants.Max);
            }

            if (notifyManager && !_factory.DisableNotifications && !project.Responsible.Equals(SecurityContext.CurrentAccount.ID))
                NotifyClient.Instance.SendAboutResponsibleByProject(project.Responsible, project);

            return project;
        }

        public virtual void Delete(int projectId)
        {
            var project = GetByID(projectId);
            if (project == null) return;

            ProjectSecurity.DemandEdit(project);
            projectDao.Delete(projectId);
            TimeLinePublisher.Project(project, EngineResource.ActionText_Delete, UserActivityConstants.ActivityActionType, UserActivityConstants.Max);
        }


        public virtual List<Participant> GetTeam(int project)
        {
            return projectDao.GetTeam(project);
        }

        public virtual bool IsInTeam(int project, Guid participant)
        {
            return projectDao.IsInTeam(project, participant);
        }

        public virtual void AddToTeam(Project project, Participant participant, bool sendNotification)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            ProjectSecurity.DemandEditTeam(project);
            projectDao.AddToTeam(project.ID, participant.ID);
            TimeLinePublisher.Team(project, participant.UserInfo, EngineResource.ActionText_AddToTeam);

            if (!_factory.DisableNotifications && sendNotification && !project.Responsible.Equals(participant.ID))
                NotifyClient.Instance.SendInvaiteToProjectTeam(participant.ID, project);
        }

        public virtual void RemoveFromTeam(Project project, Participant participant, bool sendNotification)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            ProjectSecurity.DemandEditTeam(project);
            projectDao.RemoveFromTeam(project.ID, participant.ID);
            TimeLinePublisher.Team(project, participant.UserInfo, EngineResource.ActionText_DeletedFromTeam);

            if (!_factory.DisableNotifications && sendNotification)
                NotifyClient.Instance.SendRemovingFromProjectTeam(participant.ID, project);
        }

        public virtual void UpdateTeam(Project project, IEnumerable<Guid> participants, bool notify)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participants == null) throw new ArgumentNullException("participants");

            ProjectSecurity.DemandEditTeam(project);

            var newTeam = participants.Select(p => new Participant(p)).ToList();
            var oldTeam = GetTeam(project.ID);

            var removeFromTeam = oldTeam.Where(p => !newTeam.Contains(p)).Where(p => p.ID != project.Responsible).ToList();
            var inviteToTeam = new List<Participant>();

            foreach (var participant in newTeam.Where(participant => !oldTeam.Contains(participant)))
            {
                participantDao.RemoveFromFollowingProjects(project.ID, participant.ID);
                inviteToTeam.Add(participant);
            }

            foreach (var participant in inviteToTeam)
            {
                AddToTeam(project, participant, notify);
            }

            foreach (var participant in removeFromTeam)
            {
                RemoveFromTeam(project, participant, notify);
            }
        }

        public virtual void SetTeamSecurity(Project project, Participant participant, ProjectTeamSecurity teamSecurity, bool visible)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            ProjectSecurity.DemandEditTeam(project);

            var security = projectDao.GetTeamSecurity(project.ID, participant.ID);
            if (visible)
            {
                if (security != ProjectTeamSecurity.None) security ^= teamSecurity;
            }
            else
            {
                security |= teamSecurity;
            }
            projectDao.SetTeamSecurity(project.ID, participant.ID, security);
            TimeLinePublisher.TeamSecurity(project,participant);
        }

        public virtual void ResetTeamSecurity(Project project)
        {
            if (project == null) throw new ArgumentNullException("project");

            ProjectSecurity.DemandEditTeam(project);

            var participant = GetTeam(project.ID);

            foreach (var part in participant)
            {
                projectDao.SetTeamSecurity(project.ID, part.ID, ProjectTeamSecurity.None);
            }

        }

        public virtual bool GetTeamSecurity(Project project, Participant participant, ProjectTeamSecurity teamSecurity)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (participant == null) throw new ArgumentNullException("participant");

            var security = projectDao.GetTeamSecurity(project.ID, participant.ID);
            return (security & teamSecurity) != teamSecurity;
        }

        public virtual List<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to)
        {
            return projectDao.GetTeamUpdates(from, to).Where(x => CanRead(x.Project)).ToList();
        }
    }
}
