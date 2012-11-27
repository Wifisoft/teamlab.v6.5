using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using AjaxPro;

namespace ASC.Web.Projects
{
    [AjaxNamespace("AjaxPro.CreateProjectFromTemplate")]
    public partial class CreateProjectFromTemplate : BasePage
    {
        private int _projectTmplId;

        public Template Templ { get; set; }

        protected bool IsAdmin
        {
            get { return ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID); }
        }

        protected override void PageLoad()
        {
            if (!IsAdmin)
                HttpContext.Current.Response.Redirect(ProjectsCommonResource.StartURL, true);

            Utility.RegisterTypeForAjax(typeof(CreateProjectFromTemplate));
            ((IStudioMaster)Master).DisabledSidePanel = true;

            if (Int32.TryParse(UrlParameters.EntityID, out _projectTmplId))
            {
                Templ = Global.EngineFactory.GetTemplateEngine().GetTemplate(_projectTmplId);
                JsonPublisher(Templ, "template");
            }

            InitBreadCrumbs();
            LoadProjectManagerSelector();
            LoadProjectTeamSelector();

        }
        private void InitBreadCrumbs()
        {
            Master.BreadCrumbs.Add(new BreadCrumb(ProjectTemplatesResource.CreateProjFromTmpl));
            Title = HeaderStringHelper.GetPageTitle(ProjectTemplatesResource.CreateProjFromTmpl, Master.BreadCrumbs);
        }

        private void LoadProjectManagerSelector()
        {
            var projectManagerSelector = new AdvancedUserSelector{ID = "projectManagerSelector"};
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

        [AjaxMethod]
        public string CreateProject(Project newProject, Guid[] team, string milestones, string noAssignTasks, bool notifyManager, bool notifyResponsibles)
        {
            if (ProjectSecurity.CanCreateProject())
            {
                if (newProject != null)
                {
                    Global.EngineFactory.GetProjectEngine().SaveOrUpdate(newProject, notifyManager);
                    Global.EngineFactory.GetProjectEngine().AddToTeam(newProject,Global.EngineFactory.GetParticipantEngine().GetByID(newProject.Responsible), true);

                    //add team
                    foreach (var participant in team.Where(participant => participant != Guid.Empty))
                    {
                        Global.EngineFactory.GetProjectEngine().AddToTeam(newProject, Global.EngineFactory.GetParticipantEngine().GetByID(participant), true);
                    }

                    // add milestone
                    var listMilestones = MilestoneParser(milestones);

                    foreach (var milestone in listMilestones)
                    {
                        var milestoneTasks = TaskParser(milestone.Description);
                        milestone.Description = string.Empty;
                        milestone.Project = newProject;
                        Global.EngineFactory.GetMilestoneEngine().SaveOrUpdate(milestone, notifyResponsibles);

                        foreach (var task in milestoneTasks)
                        {
                            task.Status = TaskStatus.Open;
                            task.Milestone = milestone.ID;
                            task.Project = newProject;
                            Global.EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, notifyResponsibles);
                        }
                    }

                    //add no assign tasks
                    
                    var listTasks = TaskParser(noAssignTasks);

                    foreach (var task in listTasks)
                    {
                        task.Project = newProject;
                        task.Status = TaskStatus.Open;
                        Global.EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, notifyResponsibles);
                    }

                    return "?prjID=" + newProject.ID;
                }

            }

            return string.Empty;
        }

        private static IEnumerable<Task> TaskParser(string tasks)
        {
            return JavaScriptDeserializer.DeserializeFromJson<List<Task>>(tasks);
        }

        private static IEnumerable<Milestone> MilestoneParser(string milestones)
        {
            return JavaScriptDeserializer.DeserializeFromJson<List<Milestone>>(milestones);
        }
    }
}
