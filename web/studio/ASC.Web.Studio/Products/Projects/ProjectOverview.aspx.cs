#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Helpers;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    public partial class ProjectOverview : BasePage
    {
        public ProjectFat ProjectFat { get; set; }

        private int ProjectId
        {
            get { return ProjectFat.Project.ID; }
        }

        private static Guid CurrentUser
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        private static ProjectEngine ProjectEngine
        {
            get { return Global.EngineFactory.GetProjectEngine(); }
        }

        private List<Milestone> milestones;

        private List<Task> tasks;

        protected override void PageLoad()
        {
            RequestContext.EnsureCurrentProduct();

            ((IStudioMaster)Master).DisabledSidePanel = true;

            _hintPopup.Options.IsPopup = true;

            ProjectFat = RequestContext.GetCurrentProjectFat();

            Title = string.Format("{0} - {1}", ProjectFat.Project.Title, ProjectResource.Projects);

            if (string.Compare(UrlParameters.ActionType, "edit", StringComparison.OrdinalIgnoreCase) == 0)
            {
                Server.Transfer("projectAction.aspx", true);
            }

            var projectManager = new EmployeeUserCard
            {
                EmployeeInfo = ProjectFat.Responsible,
                EmployeeUrl = ProjectFat.Responsible.GetUserProfilePageURL(),
                EmployeeDepartmentUrl = CommonLinkUtility.GetUserDepartment(ProjectFat.Responsible.ID)
            };
            projectManagerPlaceHolder.Controls.Add(projectManager);

            var teamManagenent = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);

            teamManagenent.BehaviorID = "UserSelector";
            teamManagenent.DisabledUsers.Add(new Guid());
            teamManagenent.Title = ProjectResource.ManagmentTeam;
            teamManagenent.SelectedUserListTitle = ProjectResource.Team;
            teamManagenent.SelectedUsers = ProjectEngine.GetTeam(ProjectId).Select(participant => participant.ID).ToList();

            teamManagementPlaceHolder.Controls.Add(teamManagenent);

            milestones = ProjectFat.GetMilestones();
            tasks = ProjectFat.GetTasks();
        }

        protected string GetProjectStatus()
        {
            return ProjectFat.Project.Status.ToString().ToLowerInvariant();
        }

        protected bool IsInProjectTeam()
        {
            return ProjectEngine.IsInTeam(ProjectId, CurrentUser);
        }

        protected bool IsFollowProject()
        {
            var projects = ProjectEngine.GetByFilter(new TaskFilter { Follow = true });
            return projects.Contains(ProjectFat.Project);
        }

        protected bool CanEditProject()
        {
            return ProjectSecurity.CanEdit(ProjectFat.Project);
        }

        protected bool CanDeleteProject()
        {
            return ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);
        }

        protected bool IsProjectDescriptionNotEmpty()
        {
            return !string.IsNullOrEmpty(ProjectFat.Project.Description);
        }

        protected string GetProjectTeamLink()
        {
            return string.Format("projectteam.aspx?prjID={0}", ProjectId);
        }

        protected string GetUpdateProjectLink()
        {
            return string.Format("projects.aspx?prjID={0}&action=edit", ProjectId);
        }

        protected string GetHistoryProjectLink()
        {
            return string.Format("history.aspx?prjID={0}", ProjectId);
        }

        protected string GetGrammaticalHelperParticipantCount()
        {
            var count = ProjectFat.GetActiveTeam().Count;
            return count == 0 ? string.Empty : count + " " + GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.ParticipantNominative,
                GrammaticalResource.ParticipantGenitiveSingular, GrammaticalResource.ParticipantGenitivePlural);
        }

        protected string GetMilestones()
        {
            var link = string.Format("milestones.aspx?prjID={0}", ProjectId);
            var count = milestones.Count;

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        protected string GetOpenMilestones()
        {
            var link = string.Format("milestones.aspx?prjID={0}#sortBy=deadline&sortOrder=ascending&status=open", ProjectId);
            var count = ChooseOpenMilestoneNumeralCase(milestones.Count(m => m.Status == MilestoneStatus.Open));

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        protected string GetOverdueMilestones()
        {
            var link = string.Format("milestones.aspx?prjID={0}#sortBy=deadline&sortOrder=ascending&overdue=true", ProjectId);
            var count = ChooseOverdueMilestoneNumeralCase(milestones.Count(m => m.DeadLine <= DateTime.Today && m.Status == MilestoneStatus.Open));

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        protected string GetTasks()
        {
            var link = string.Format("tasks.aspx?prjID={0}", ProjectId);
            var count = tasks.Count;

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        protected string GetOpenTasks()
        {
            var link = string.Format("tasks.aspx?prjID={0}#sortBy=deadline&sortOrder=ascending&status=open", ProjectId);
            var count = ChooseOpenTaskNumeralCase(tasks.Count(t => t.Status == TaskStatus.Open));

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        protected string GetOverdueTasks()
        {
            var link = string.Format("tasks.aspx?prjID={0}#sortBy=deadline&sortOrder=ascending&overdue=true&status=open", ProjectId);
            var count = ChooseOverdueTaskNumeralCase(tasks.Count(t => t.Status == TaskStatus.Open && t.Deadline != DateTime.MinValue && t.Deadline <= DateTime.Today));

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }


        public string GetDiscussions()
        {
            var link = string.Format("messages.aspx?prjID={0}", ProjectId);
            var count = Global.EngineFactory.GetMessageEngine().GetByProject(ProjectId).Count;

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        public string GetFiles()
        {
            var folderId = Global.EngineFactory.GetFileEngine().GetRoot(ProjectId);
            var link = string.Format("tmdocs.aspx?prjID={0}", ProjectId);
            var count = FilesIntegration.GetFolderDao().GetItemsCount(folderId, true);

            return string.Format("<a href='{0}'>{1}</a>", link, count);
        }

        protected string ChooseOpenMilestoneNumeralCase(int count)
        {
            return count == 0 ? string.Empty : count + " " + GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.OpenMilestoneNominative,
                GrammaticalResource.OpenMilestoneGenitiveSingular, GrammaticalResource.OpenMilestoneGenitivePlural);
        }

        protected string ChooseOpenTaskNumeralCase(int count)
        {
            return count == 0 ? string.Empty : count + " " + GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.OpenTaskNominative,
                GrammaticalResource.OpenTaskGenitiveSingular, GrammaticalResource.OpenTaskGenitivePlural);
        }

        protected string ChooseOverdueMilestoneNumeralCase(int count)
        {
            return count == 0 ? string.Empty : count + " " + GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.OverdueMilestoneNominative,
                GrammaticalResource.OverdueMilestoneGenitiveSingular, GrammaticalResource.OverdueMilestoneGenitivePlural);
        }
        
        protected string ChooseOverdueTaskNumeralCase(int count)
        {
            return count == 0 ? string.Empty : count + " " + GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.OverdueTaskNominative,
                GrammaticalResource.OverdueTaskGenitiveSingular, GrammaticalResource.OverdueTaskGenitivePlural);
        }

        protected string GetProjectTitle()
        {
            return ProjectFat.Project.Title.HtmlEncode();
        }

        protected string GetProjectDescription()
        {
            return ProjectFat.Project.Description.HtmlEncode().Replace("\n", "<br/>");
        }
    }
}
