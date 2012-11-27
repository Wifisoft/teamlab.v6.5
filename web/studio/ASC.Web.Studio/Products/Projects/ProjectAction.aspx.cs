#region Usings

using System;
using System.Linq;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    public partial class ProjectAction : BasePage
    {
        protected Project Project { get; set; }

        private ProjectFat ProjectFat { get; set; }

        protected bool HasTemplates { get; private set; }

        private static  TagEngine TagEngine
        {
            get { return Global.EngineFactory.GetTagEngine(); }
        }

        protected int ActiveTasksCount { get { return ProjectFat == null ? 0 : ProjectFat.GetTasks().Count(t => t.Status == TaskStatus.Open); } }
        protected int ActiveMilestonesCount { get { return ProjectFat == null ? 0 : ProjectFat.GetMilestones().Count(m => m.Status == MilestoneStatus.Open); } }

        protected bool SecurityEnable { get; private set; }

        protected override void PageLoad()
        {
            Bootstrap();
            
            LoadProjectManagerSelector();
            
            SetSecurityEnable();

            if (Project != null)
            {
                ProjectFat = new ProjectFat(Project);
                return;
            }

            LoadProjectTeamSelector();
        }

        private void Bootstrap()
        {
            ((IStudioMaster)Master).DisabledSidePanel = true;

            _hintPopupDeleteProject.Options.IsPopup = true;
            _hintPopupActiveTasks.Options.IsPopup = true;
            _hintPopupActiveMilestones.Options.IsPopup = true;

            if (!string.IsNullOrEmpty(UrlParameters.ProjectID))
                RequestContext.EnsureCurrentProduct();


            Project = RequestContext.GetCurrentProject(false);

            if (!ProjectSecurity.CanEdit(Project))
            {
                Response.Redirect(ProjectsCommonResource.StartURL, true);
            }

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.Projects,
                NavigationUrl = "projects.aspx"
            });

            if (Project == null)
            {
                Master.BreadCrumbs.Add(new BreadCrumb
                {
                    Caption = ProjectResource.CreateNewProject,
                    NavigationUrl = ""
                });
            }
            else
            {
                Master.BreadCrumbs.Add(new BreadCrumb
                {
                    Caption = Project.HtmlTitle,
                    NavigationUrl = "projects.aspx?prjID=" + Project.ID

                });
                Master.BreadCrumbs.Add(new BreadCrumb
                {
                    Caption = ProjectResource.EditProject,
                    NavigationUrl = ""

                });

                projectTitle.Text = Project.Title;
                projectDescription.Text = Project.Description;

                var tags = TagEngine.GetProjectTags(Project.ID).Select(r => r.Value).ToArray();
                projectTags.Text = string.Join(", ", tags);

                HasTemplates = false;
            }

            Title = HeaderStringHelper.GetPageTitle(ProjectResource.Projects, Master.BreadCrumbs);
        }

        private void LoadProjectManagerSelector()
        {
            var projectManagerSelector = new AdvancedUserSelector
            {
                ID = "projectManagerSelector"
            };
            if (Project != null)
            {
                projectManagerSelector.SelectedUserId = Project.Responsible;
            }
            projectManagerPlaceHolder.Controls.Add(projectManagerSelector);
        }
        
        private void LoadProjectTeamSelector()
        {
            var projectTeamSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            projectTeamSelector.BehaviorID = "projectTeamSelector";
            projectTeamSelector.DisabledUsers.Add(new Guid());
            projectTeamSelector.Title = ProjectResource.ManagmentTeam;
            projectTeamSelector.SelectedUserListTitle = ProjectResource.Team;

            projectTeamPlaceHolder.Controls.Add(projectTeamSelector);
        }

        private void SetSecurityEnable()
        {
            SecurityEnable = true;
            if (SecurityEnable) return;
            var stubControl = LoadControl(PremiumStub.Location) as PremiumStub;
            if (stubControl == null) return;
            stubControl.Type = PremiumFeatureType.PrivateProjects;
            premiumStubHolder.Controls.Add(stubControl);
        }

        protected string GetPageTitle()
        {
            return Project == null ? ProjectResource.CreateNewProject : ProjectResource.EditProject;
        }

        protected bool IsNotificationManagerAvailable()
        {
            return  Project == null && Global.IsAdmin;
        }

        protected bool IsEditingProjectAvailable()
        {
            return Project != null;
        }

        protected string GetProjectStatus()
        {
            return Project != null ? Project.Status.ToString().ToLowerInvariant() : string.Empty;
        }

        protected string GetProjectStatusTitle()
        {
            if (Project == null) return string.Empty;
            var status = Project.Status;
            return status == ProjectStatus.Open
                       ? ProjectResource.ActiveProject
                       : status == ProjectStatus.Paused
                             ? ProjectResource.PausedProject
                             : ProjectResource.ClosedProject;
        }

        protected string RenderProjectPrivacyCheckboxValue()
        {
            return Project != null && Project.Private
                        ? "checked"
                        : "";
        }

        protected string GetActiveTasksUrl()
        {
            return Project == null ? string.Empty : string.Format("tasks.aspx?prjID={0}#status=open", Project.ID);
        }

        protected string GetActiveMilestonesUrl()
        {
            return Project == null ? string.Empty : string.Format("milestones.aspx?prjID={0}#status=open", Project.ID);
        }

        protected string GetProjectActionButtonTitle()
        {
            return Project == null ? ProjectResource.AddNewProject : ProjectResource.SaveProject;
        }
    }
}
