using System;
using System.Linq;
using System.Web;
using System.Collections;
using ASC.Projects.Core.Domain;
using ASC.Web.Projects.Resources;
using System.Collections.Generic;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Projects.Engine;

namespace ASC.Web.Projects.Classes
{
    public class RequestContext
    {
        static ProjectFat Projectctx { get { return Hash["_projectctx"] as ProjectFat; } set { Hash["_projectctx"] = value; } }
        static int? ProjectId { get { return Hash["_projectId"] as int?; } set { Hash["_projectId"] = value; } }
        static int? ProjectsCount { get { return Hash["_projectsCount"] as int?; } set { Hash["_projectsCount"] = value; } }

        static List<Project> UserProjects { get { return Hash["_userProjects"] as List<Project>; } set { Hash["_userProjects"] = value; } }
        static List<Project> UserFollowingProjects { get { return Hash["_userFollowingProjects"] as List<Project>; } set { Hash["_userFollowingProjects"] = value; } }

        #region Project

        static public bool IsInConcreteProject()
        {
            return !String.IsNullOrEmpty(UrlParameters.ProjectID);
        }

        static public Project EnsureCurrentProduct()
        {
            if (HttpContext.Current == null) throw new ApplicationException("Not in http request");

            var project = GetCurrentProject(false);

            if (project == null)
                HttpContext.Current.Response.Redirect(ProjectsCommonResource.StartURL, true);

            return project;
        }

        static public Project GetCurrentProject(bool isthrow)
        {
            if (Projectctx == null)
            {
                var project = Global.EngineFactory.GetProjectEngine().GetByID(GetCurrentProjectId(isthrow));

                if (project == null)
                {
                    if (isthrow) throw new ApplicationException("ProjectFat not finded");
                }
                else
                    Projectctx = new ProjectFat(project);
            }

            return Projectctx != null ? Projectctx.Project : null;
        }

        static public Project GetCurrentProject()
        {
            return GetCurrentProject(true);
        }

        static int GetCurrentProjectId(bool isthrow)
        {
            if (!ProjectId.HasValue)
            {
                int pid;
                if (!Int32.TryParse(UrlParameters.ProjectID, out pid))
                {
                    if (isthrow)
                        throw new ApplicationException("ProjectFat Id parameter invalid");
                }
                else
                    ProjectId = pid;
            }
            return ProjectId.HasValue ? ProjectId.Value : -1;
        }

        static public int GetCurrentProjectId()
        {
            return GetCurrentProjectId(true);
        }

        public static ProjectFat GetCurrentProjectFat()
        {
            if (Projectctx == null) EnsureCurrentProduct();
            return Projectctx;
        }

        #endregion

        #region Projects

        public static bool HasAnyProjects()
        {
            if (
                ProjectsCount.HasValue && ProjectsCount.Value > 0
                ||
                UserProjects != null && UserProjects.Count > 0
                ||
                UserFollowingProjects != null && UserFollowingProjects.Count > 0)
                return true;

            return GetProjectsCount() > 0;
        }

        public static bool HasCurrentUserAnyProjects()
        {
            return GetCurrentUserProjects().Count > 0 || GetCurrentUserFollowingProjects().Count > 0;
        }

        public static int GetProjectsCount()
        {
            if (!ProjectsCount.HasValue)
                ProjectsCount = Global.EngineFactory.GetProjectEngine().GetAll().Count();
            return ProjectsCount.Value;
        }

        public static List<Project> GetCurrentUserProjects()
        {
            return UserProjects ??
                   (UserProjects =
                    Global.EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID));
        }

        public static List<Project> GetCurrentUserFollowingProjects()
        {
            return UserFollowingProjects ??
                   (UserFollowingProjects =
                    Global.EngineFactory.GetProjectEngine().GetFollowing(SecurityContext.CurrentAccount.ID));
        }

        #endregion


        #region internal

        const string storageKey = "PROJECT_REQ_CTX";

        static Hashtable Hash
        {
            get
            {
                if (HttpContext.Current == null) throw new ApplicationException("Not in http request");

                var hash = (Hashtable)HttpContext.Current.Items[storageKey];
                if (hash == null)
                {
                    hash = new Hashtable();
                    HttpContext.Current.Items[storageKey] = hash;
                }
                return hash;
            }
        }

        #endregion
    }

    public class ProjectFat
    {
        readonly Project project;

        internal ProjectFat(Project project)
        {
            this.project = project;
            Responsible = Global.EngineFactory.GetParticipantEngine().GetByID(this.project.Responsible).UserInfo;
        }

        public Project Project { get { return project; } }

        List<Participant> team;
        public List<Participant> GetTeam()
        {
            return team ?? (team = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID)
                                       .OrderBy(p => p.UserInfo, UserInfoComparer.Default)
                                       .ToList());
        }

        public List<Participant> GetActiveTeam()
        {
            var projectTeam = GetTeam();

            if (ProjectSecurity.CanEditTeam(Project))
            {
                var engine = Global.EngineFactory.GetProjectEngine();
                var deleted = projectTeam.FindAll(u => u.UserInfo.Status != EmployeeStatus.Active || !CoreContext.UserManager.UserExists(u.ID));
                foreach (var d in deleted)
                {
                    engine.RemoveFromTeam(Project, d, true);
                }
            }

            var active = projectTeam.FindAll(u => u.UserInfo.Status != EmployeeStatus.Terminated && CoreContext.UserManager.UserExists(u.ID));
            return active.OrderBy(u => u.UserInfo, UserInfoComparer.Default).ToList();
        }

        public bool IsResponsible()
        {
            return Responsible.ID == SecurityContext.CurrentAccount.ID;
        }

        List<Milestone> milestones;
        public List<Milestone> GetMilestones()
        {
            return milestones ?? (milestones = Global.EngineFactory.GetMilestoneEngine().GetByProject(Project.ID));
        }

        public List<Milestone> GetMilestones(MilestoneStatus status)
        {
            return GetMilestones().FindAll(mil => mil.Status == status);
        }

        public Milestone GetMilestone(int id)
        {
            return GetMilestones().Find(mile => mile.ID == id);
        }

        public List<Milestone> GetOverdueMilestone()
        {
            return GetMilestones().Where(m => m.DeadLine < DateTime.Today && m.Status != MilestoneStatus.Closed).ToList();
        }

        private List<Task> tasks;
        public List<Task> GetTasks()
        {
            return tasks ?? (tasks = Global.EngineFactory.GetTaskEngine().GetByProject(Project.ID, null, Guid.Empty));
        }

        public UserInfo Responsible { get; private set; }
    }
}
