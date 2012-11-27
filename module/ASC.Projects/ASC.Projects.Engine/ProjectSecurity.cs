using System;
using System.Linq;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using ASC.Web.Core;

namespace ASC.Projects.Engine
{
    public class ProjectSecurity
    {
        #region Can Read

        public static bool CanReadMessages(Project project, Guid userId)
        {
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Messages);
        }

        public static bool CanReadMessages(Project project)
        {
            return CanReadMessages(project, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanReadFiles(Project project, Guid userId)
        {
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Files);
        }

        public static bool CanReadFiles(Project project)
        {
            return CanReadFiles(project, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanReadTasks(Project project, Guid userId)
        {
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Tasks);
        }

        public static bool CanReadTasks(Project project)
        {
            return CanReadTasks(project, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanReadMilestones(Project project, Guid userId)
        {
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Milestone);
        }

        public static bool CanReadMilestones(Project project)
        {
            return CanReadMilestones(project, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanRead(Project project, Guid userId)
        {
            if (project == null) return false;
            return !project.Private || IsInTeam(project, userId);
        }

        public static bool CanRead(Project project)
        {
            return CanRead(project, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanRead(Task task, Guid userId)
        {
            if (task == null || !CanRead(task.Project, userId)) return false;
            if (task.Responsible == userId) return true;

            if (task.Responsibles.Contains(userId)) return true;

            if (!GetTeamSecurity(task.Project, userId, ProjectTeamSecurity.Tasks)) return false;
            if (task.Milestone != 0 && !GetTeamSecurity(task.Project, userId, ProjectTeamSecurity.Milestone))
            {
                var m = GetFactory().GetMilestoneDao().GetById(task.Milestone);
                if (!CanRead(m, userId)) return false;
            }

            return true;
        }

        public static bool CanRead(Task task)
        {
            return CanRead(task, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanRead(Subtask subtask)
        {
            if (subtask == null) return false;
            return subtask.Responsible == SecurityContext.CurrentAccount.ID;
        }

        public static bool CanRead(Milestone milestone, Guid userId)
        {
            if (milestone == null || !CanRead(milestone.Project, userId)) return false;

            if (milestone.Responsible == userId) return true;

            return CanReadMilestones(milestone.Project, userId);
        }

        public static bool CanRead(Milestone milestone)
        {
            return CanRead(milestone, SecurityContext.CurrentAccount.ID);
        }

        public static bool CanRead(Message message, Guid userId)
        {
            if (message == null || !CanRead(message.Project, userId)) return false;

            return CanReadMessages(message.Project, userId);
        }

        public static bool CanRead(Message message)
        {
            return CanRead(message, SecurityContext.CurrentAccount.ID);
        }

        #endregion

        #region Can Create

        public static bool CanCreateProject()
        {
            return CurrentUserAdministrator;
        }

        public static bool CanCreateMilestone(Project project)
        {
            return IsProjectManager(project);
        }

        public static bool CanCreateMessage(Project project)
        {
            if (!SecurityContext.CurrentAccount.IsAuthenticated) return false;
            return CanReadMessages(project);
        }

        public static bool CanCreateTask(Project project)
        {
            if (IsProjectManager(project)) return true;
            return IsInTeam(project) && CanReadTasks(project);
        }

        public static bool CanCreateComment()
        {
            return SecurityContext.IsAuthenticated;
        }

        public static bool CanCreateTimeSpend(Project project)
        {
            return IsInTeam(project);
        }

        #endregion

        #region Can Edit

        public static bool CanEdit(Project project)
        {
            return IsProjectManager(project);
        }

        public static bool CanEdit(Milestone milestone)
        {
            if (milestone == null) return false;
            if (IsProjectManager(milestone.Project)) return true;

            if (!CanRead(milestone)) return false;

            return IsInTeam(milestone.Project) && 
                (milestone.CreateBy == SecurityContext.CurrentAccount.ID || 
                milestone.Responsible == SecurityContext.CurrentAccount.ID);
        }

        public static bool CanEdit(Message message)
        {
            if (message == null) return false;
            if (IsProjectManager(message.Project)) return true;

            if (!CanRead(message)) return false;

            return IsInTeam(message.Project) && message.CreateBy == SecurityContext.CurrentAccount.ID;
        }

        public static bool CanEdit(Task task)
        {
            if (task == null) return false;

            if (IsProjectManager(task.Project)) return true;

            return IsInTeam(task.Project) &&
                (task.CreateBy == SecurityContext.CurrentAccount.ID ||
                task.Responsible == SecurityContext.CurrentAccount.ID || 
                task.Responsible == Guid.Empty ||
                task.Responsibles.Contains(SecurityContext.CurrentAccount.ID) ||
                task.SubTasks.Select(r => r.Responsible).Contains(SecurityContext.CurrentAccount.ID));
        }

        public static bool CanEdit(Task task, Subtask subtask)
        {
            if (subtask == null) return false;

            if (CanEdit(task)) return true;

            return IsInTeam(task.Project) &&
                (subtask.CreateBy == SecurityContext.CurrentAccount.ID ||
                subtask.Responsible == SecurityContext.CurrentAccount.ID);
        }

        public static int CanWork(Task task)
        {
            if (task == null) return 0;

            if (IsProjectManager(task.Project) || (IsInTeam(task.Project) && task.CreateBy == SecurityContext.CurrentAccount.ID))
                return 3;

            if (IsInTeam(task.Project) && (task.Responsibles.Contains(SecurityContext.CurrentAccount.ID) ||
                task.SubTasks.Select(r => r.Responsible).Contains(SecurityContext.CurrentAccount.ID)))
                return 2;

            if (IsInTeam(task.Project) && task.Responsibles.Count == 0)
                return 1;

            return 0;
        }

        public static bool CanEditTeam(Project project)
        {
            return IsProjectManager(project);
        }

        public static bool CanEditComment(Project project, Comment comment)
        {
            if (project == null || comment == null) return false;
            return comment.CreateBy == SecurityContext.CurrentAccount.ID || CurrentUserAdministrator || project.Responsible == SecurityContext.CurrentAccount.ID;
        }

        public static bool CanEdit(TimeSpend timeSpend)
        {
            if (timeSpend == null) return false;
            return IsProjectManager(timeSpend.Task.Project) || timeSpend.Person == SecurityContext.CurrentAccount.ID;
        }

        #endregion

        #region Can Delete

        public static bool CanDelete(Task task)
        {
            if (task == null) return false;

            if (IsProjectManager(task.Project)) return true;

            return IsInTeam(task.Project) && task.CreateBy == SecurityContext.CurrentAccount.ID;
        }

        #endregion

        public static void DemandCreateProject()
        {
            if (!CanCreateProject()) throw CreateSecurityException();
        }

        public static void DemandCreateMessage(Project project)
        {
            if (!CanCreateMessage(project)) throw CreateSecurityException();
        }

        public static void DemandCreateMilestone(Project project)
        {
            if (!CanCreateMilestone(project)) throw CreateSecurityException();
        }

        public static void DemandCreateTask(Project project)
        {
            if (!CanCreateTask(project)) throw CreateSecurityException();
        }

        public static void DemandRead(Milestone milestone)
        {
            if (!CanRead(milestone != null ? milestone.Project : null)) throw CreateSecurityException();
        }

        public static void DemandRead(Message message)
        {
            if (!CanRead(message)) throw CreateSecurityException();
        }

        public static void DemandRead(Task task)
        {
            if (!CanRead(task)) throw CreateSecurityException();
        }

        public static void DemandEdit(Project project)
        {
            if (!CanEdit(project)) throw CreateSecurityException();
        }

        public static void DemandEdit(Message message)
        {
            if (!CanEdit(message)) throw CreateSecurityException();
        }

        public static void DemandEdit(Milestone milestone)
        {
            if (!CanEdit(milestone)) throw CreateSecurityException();
        }

        public static void DemandEdit(Task task)
        {
            if (!CanEdit(task)) throw CreateSecurityException();
        }

        public static void DemandWork(Task task)
        {
            if (CanWork(task) == 0) throw CreateSecurityException();
        }

        public static void DemandEdit(Task task, Subtask subtask)
        {
            if (!CanEdit(task, subtask)) throw CreateSecurityException();
        }

        public static void DemandEditTeam(Project project)
        {
            if (!CanEditTeam(project)) throw CreateSecurityException();
        }

        public static void DemandEditComment(Project project, Comment comment)
        {
            if (!CanEditComment(project, comment)) throw CreateSecurityException();
        }

        public static bool IsAdministrator(Guid userId)
        {
            return CoreContext.UserManager.IsUserInGroup(userId, ASC.Core.Users.Constants.GroupAdmin.ID) ||
                WebItemSecurity.IsProductAdministrator(EngineFactory.ProductId, userId);
        }


        private static bool CurrentUserAdministrator
        {
            get { return IsAdministrator(SecurityContext.CurrentAccount.ID); }
        }

        private static ASC.Projects.Core.DataInterfaces.IDaoFactory GetFactory()
        {
            return new DaoFactory("projects", CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        private static bool IsProjectManager(Project project)
        {
            return IsProjectManager(project, SecurityContext.CurrentAccount.ID);
        }

        private static bool IsProjectManager(Project project, Guid userId)
        {
            return IsAdministrator(userId) || (project != null && project.Responsible == userId);
        }

        private static bool IsInTeam(Project project)
        {
            return IsInTeam(project, SecurityContext.CurrentAccount.ID);
        }

        private static bool IsInTeam(Project project, Guid userId)
        {
            return IsAdministrator(userId) || (project != null && GetFactory().GetProjectDao().IsInTeam(project.ID, userId));
        }

        private static bool GetTeamSecurity(Project project, Guid userId, ProjectTeamSecurity security)
        {
            if (IsProjectManager(project, userId) || project == null || !project.Private) return true;
            var dao = GetFactory().GetProjectDao();
            var s = dao.GetTeamSecurity(project.ID, userId);
            return (s & security) != security && dao.IsInTeam(project.ID, userId);
        }

        public static void DemandAuthentication()
        {
            if (!CoreContext.TenantManager.GetCurrentTenant().Public && !SecurityContext.CurrentAccount.IsAuthenticated)
            {
                throw CreateSecurityException();
            } 
        }

        public static Exception CreateSecurityException()
        {
            throw new System.Security.SecurityException("Access denied.");
        }
    }
}
