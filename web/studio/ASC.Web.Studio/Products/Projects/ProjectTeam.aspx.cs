#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Core.Users;
using ASC.Notify.Patterns;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Web.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Controls.Projects;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Projects.Controls;
using ASC.Core;
using ASC.Projects.Core;
using System.Text;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Web.Projects.Controls.Tasks;

#endregion

namespace ASC.Web.Projects
{
    [AjaxNamespace("AjaxPro.Project")]
    public partial class ProjectTeam : BasePage
    {

        #region Properties

        protected Project Project { get; set; }

        #endregion

        #region Methods

        protected void InitView()
        {

            int projectID;
            if (!int.TryParse(UrlParameters.ProjectID, out projectID))
            {
                Response.Redirect(ProjectsCommonResource.StartURL);
            }

            Project = Global.EngineFactory.GetProjectEngine().GetByID(projectID);
            if (Project == null)
            {
                Response.Redirect(ProjectsCommonResource.StartURL);
            }

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.Projects,
                NavigationUrl = "projects.aspx"
            });
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = Project.HtmlTitle,
                NavigationUrl = "projects.aspx?prjID=" + Project.ID
            });
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.ProjectTeam
            });

            Title = HeaderStringHelper.GetPageTitle(ProjectResource.ProjectTeam, Master.BreadCrumbs);

            Master.DisabledSidePanel = true;
        }

        public string GetReportUri(bool onlyOpenTasks, Guid userID)
        {
            var filter = new ReportFilter();
            filter.TimeInterval = ReportTimeInterval.Absolute;
            filter.UserId = userID;
            filter.MilestoneStatuses.Add(MilestoneStatus.Open);
            filter.MilestoneStatuses.Add(MilestoneStatus.Closed);
            filter.TaskStatuses.Add(onlyOpenTasks ? TaskStatus.Open : TaskStatus.Closed);
            return string.Format("reports.aspx?action=generate&reportType=10&{0}", filter.ToUri());
        }

        protected String RenderProjectTeam(IList<Participant> participants, IList<Participant> oldTeam)
        {

            Page page = new Page();

            var oClosedProjectTeamView = (ClosedProjectTeamView)page.LoadControl(PathProvider.GetControlVirtualPath("ClosedProjectTeamView.ascx"));
            oClosedProjectTeamView.Team = participants;
            oClosedProjectTeamView.OldTeam = new List<Participant>(oldTeam);
            oClosedProjectTeamView.TeamLeader = new Participant(Project.Responsible);
            oClosedProjectTeamView.Project = Project;

            page.Controls.Add(oClosedProjectTeamView);

            System.IO.StringWriter writer = new System.IO.StringWriter();

            HttpContext.Current.Server.Execute(page, writer, false);

            string output = writer.ToString();

            writer.Close();

            return output;

        }

        [AjaxMethod]
        public String UserManager(String value, bool notifyIsChecked, int projID)
        {
            ProjectSecurity.DemandAuthentication();

            Project = Global.EngineFactory.GetProjectEngine().GetByID(projID);

            var checkedParticipant = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var shapeTeam = new List<Participant>();

            if (Project != null)
                shapeTeam.Add(Global.EngineFactory.GetParticipantEngine().GetByID(Project.Responsible));

            for (int index = 0; index < checkedParticipant.Length; index++)
                shapeTeam.Add(Global.EngineFactory.GetParticipantEngine().GetByID(new Guid(checkedParticipant[index])));

            var oldTeam = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID);

            var removeFromTeam = new List<Participant>();
            var inviteToTeam = new List<Participant>();

            foreach (var participant in oldTeam)
                if (!shapeTeam.Contains(participant))
                    if (participant.ID != Project.Responsible)
                        removeFromTeam.Add(participant);

            foreach (var participant in shapeTeam)
                if (!oldTeam.Contains(participant))
                {
                    Global.EngineFactory.GetParticipantEngine().RemoveFromFollowingProjects(Project.ID, participant.ID);
                    inviteToTeam.Add(participant);

                }


            
            foreach (var participant in inviteToTeam)
            {
                Global.EngineFactory.GetProjectEngine().AddToTeam(Project, participant, notifyIsChecked);
                
            }

            foreach (var participant in removeFromTeam)
            {
                Global.EngineFactory.GetProjectEngine().RemoveFromTeam(Project, participant, notifyIsChecked);
            }


            return RenderProjectTeam(shapeTeam, oldTeam);

        }

        [AjaxMethod]
        public string GetActionContent(int prjID, Guid userID)
        {
            ProjectSecurity.DemandAuthentication();

            var project = Global.EngineFactory.GetProjectEngine().GetByID(prjID);
            var sb = new StringBuilder();
            var ui = CoreContext.UserManager.GetUsers(userID);
            
            if (ProjectSecurity.CanCreateTask(project))
                //sb.AppendFormat("<a class='pm-dropdown-item' onclick=\"ASC.Projects.TaskActionPage.init(-1, null, '{0}');ASC.Projects.TaskActionPage.show();\">{1}</a>", userID, TaskResource.AddNewTask);
                sb.AppendFormat("<a class='pm-dropdown-item' onclick=\"showNewTaskPopup('{0}','{1}')\">{2}</a>", userID, ui.DisplayUserName(), TaskResource.AddNewTask);
            sb.AppendFormat("<a class='pm-dropdown-item' onclick=\"ASC.Projects.Reports.generateReportByUrl('{0}')\">{1}</a>",
                GetReportUri(true,userID), ReportResource.ReportOpenTasks);
            sb.AppendFormat("<a class='pm-dropdown-item' onclick=\"ASC.Projects.Reports.generateReportByUrl('{0}')\">{1}</a>",
                GetReportUri(false,userID), ReportResource.ReportClosedTasks);
            sb.AppendFormat("<a class='pm-dropdown-item' onclick=\"location.replace('mailto:{0}')\">{1}</a>", ui.Email, ProjectResource.ClosedProjectTeamWriteMail);
            sb.AppendFormat("<a class='pm-dropdown-item' onclick=\"ASC.Controls.JabberClient.open('{0}')\">{1}</a>", ui.UserName, ProjectResource.ClosedProjectTeamWriteInMessenger);

            return sb.ToString();
        }

        [AjaxMethod]
        public void SetTeamSecurity(int prjID, Guid userID, int module, bool visible)
        {
            var project = Global.EngineFactory.GetProjectEngine().GetByID(prjID);
            var participant = Global.EngineFactory.GetParticipantEngine().GetByID(userID);
            var security = ProjectTeamSecurity.None;

            switch(module)
            {
                case 0:
                    security = ProjectTeamSecurity.Messages;
                    break;
                case 1:
                    security = ProjectTeamSecurity.Files;
                    break;
                case 2:
                    security = ProjectTeamSecurity.Tasks;
                    break;
                case 3:
                    security = ProjectTeamSecurity.Milestone;
                    break;
            }

            Global.EngineFactory.GetProjectEngine().SetTeamSecurity(project, participant, security, visible);
        }

        #endregion

        #region Events

        protected override void PageLoad()
        {

            AjaxPro.Utility.RegisterTypeForAjax(typeof(ProjectTeam));

            InitView();
            var userInfo = Global.EngineFactory.GetParticipantEngine().GetByID(Project.Responsible).UserInfo;
            _phProjectLeaderCard.Controls.Add(new EmployeeUserCard
                                                         {
                                                             EmployeeInfo = userInfo,
                                                             EmployeeUrl = userInfo.GetUserProfilePageURL(),
                                                             EmployeeDepartmentUrl = CommonLinkUtility.GetUserDepartment(Project.Responsible)
                                                         });

            List<Participant> team = new List<Participant>();
            foreach (Participant prt in Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID))
            {
                var u = CoreContext.UserManager.GetUsers(prt.ID);
                if (u.ID != ASC.Core.Users.Constants.LostUser.ID && u.Status != EmployeeStatus.Terminated)
                {
                    team.Add(prt);
                }
            }

            _ltlTeam.Text = RenderProjectTeam(team, team);


            ASC.Web.Studio.UserControls.Users.UserSelector userSelector = (ASC.Web.Studio.UserControls.Users.UserSelector)LoadControl(ASC.Web.Studio.UserControls.Users.UserSelector.Location);
            userSelector.BehaviorID = "UserSelector";
            userSelector.DisabledUsers.Add(Project.Responsible);
            userSelector.Title = ProjectResource.ManagmentTeam;
            userSelector.SelectedUserListTitle = ProjectResource.Team;
            userSelector.CustomBottomHtml = string.Format("<div style='padding-top:10px'><input id='notify' type='checkbox'/><label for='notify' style='padding-left:10px' >{0}</label></div>", ProjectResource.NotifyProjectTeam);

            var selectedUsers = new List<Guid>();

            foreach (Participant participant in Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID))
            {
                selectedUsers.Add(participant.ID);
            }

            userSelector.SelectedUsers = selectedUsers;

            _phUserSelector.Controls.Add(userSelector);

            if (ProjectSecurity.CanCreateTask(Project))
            {
                var taskAction = (TaskAction)LoadControl(PathProvider.GetControlVirtualPath("TaskAction.ascx"));
                phAddTaskPanel.Controls.Add(taskAction);
            }

        }

        #endregion
    }
}
