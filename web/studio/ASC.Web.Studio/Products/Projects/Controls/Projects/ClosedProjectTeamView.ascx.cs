#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Users;
using System.Text;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Classes;
using ASC.Projects.Engine;
using ASC.Web.Projects.Controls.Tasks;
using ASC.Web.Projects.Resources;
using ASC.Core;

#endregion

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ClosedProjectTeamView : BaseUserControl
    {
        #region Property

        public IList<Participant> Team { get; set; }
        public List<Participant> OldTeam { get; set; }
        public Participant TeamLeader { get; set; }
        public Project Project { get; set; }

        public bool TemplateMode { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Participant> prjTeam = new List<Participant>(OldTeam);
            bool IsNew = true;

            foreach (Participant newParticipant in Team)
            {
                IsNew = true;
                foreach (Participant oldParticipant in OldTeam)
                {
                    if (newParticipant.UserInfo.ID == oldParticipant.UserInfo.ID)
                    {
                        IsNew = false;
                    }
                }
                if (IsNew) prjTeam.Add(newParticipant);
            }

            prjTeam.Sort((x, y) => UserInfoComparer.Default.Compare(x.UserInfo, y.UserInfo));

            _rptContent.DataSource = prjTeam;
            _rptContent.DataBind();
        }

        public string IsNewUser(Guid id)
        {
            bool isNew = true;
            foreach (Participant oldParticipant in OldTeam)
            {
                if (id == oldParticipant.UserInfo.ID)
                {
                    isNew = false;
                    break;
                }

            }
            if (isNew) return "NewUserInTeam";
            else return string.Empty;
        }

        public string IsOldUser(Guid id)
        {
            bool isOld = true;
            foreach (Participant newParticipant in Team)
            {
                if (id == newParticipant.UserInfo.ID)
                {
                    isOld = false;
                    break;
                }
                if (id == TeamLeader.UserInfo.ID)
                {
                    isOld = false;
                    break;
                }

            }
            if (isOld) return "OldUserInTeam";
            else return string.Empty;
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

        public string GetCssClass(Guid userID, ProjectTeamSecurity security)
        {
            var participant = Global.EngineFactory.GetParticipantEngine().GetByID(userID);
            var havePermission = Global.EngineFactory.GetProjectEngine().GetTeamSecurity(Project, participant, security);

            if (ProjectSecurity.CanEditTeam(Project) && userID != TeamLeader.ID && !ProjectSecurity.IsAdministrator(userID) && Project.Private)
            {
                if (havePermission)
                    return "pm-projectTeam-modulePermissionOn";
                return "pm-projectTeam-modulePermissionOff";
            }
            else
            {
                if (havePermission)
                    return "pm-projectTeam-modulePermissionOn-disable";
                return "pm-projectTeam-modulePermissionOff-disable";
            }
        }

        public string GetOnClickEvent(Guid userID, ProjectTeamSecurity security)
        {
            if (ProjectSecurity.CanEditTeam(Project) 
                && userID != TeamLeader.ID
                && !ProjectSecurity.IsAdministrator(userID) 
                && Project.Private)
            {
                switch(security)
                {
                    case ProjectTeamSecurity.Messages:
                        return string.Format("onclick=\"javascript:changePermission(this,'{0}', 0);\" title='{1}'",
                        userID.ToString(), ProjectResource.ClosedProjectTeamMessageButtonTooltip);
                    case ProjectTeamSecurity.Files:
                        return string.Format("onclick=\"javascript:changePermission(this,'{0}', 1);\" title='{1}'",
                        userID.ToString(), ProjectResource.ClosedProjectTeamDocumentButtonTooltip);
                    case ProjectTeamSecurity.Tasks:
                        return string.Format("onclick=\"javascript:changePermission(this,'{0}', 2);\" title='{1}'",
                        userID.ToString(), ProjectResource.ClosedProjectTeamTaskButtonTooltip);
                    case ProjectTeamSecurity.Milestone:
                        return string.Format("onclick=\"javascript:changePermission(this,'{0}', 3);\" title='{1}'",
                        userID.ToString(), ProjectResource.ClosedProjectTeamMilestoneButtonTooltip);
                }
            }
            return string.Empty;
        }

        #endregion
    }
}