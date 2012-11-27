#region Import

using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using ASC.Common.Utils;
using ASC.Projects.Core.Domain;
using ASC.Core;
using ASC.Web.Core.Helpers;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Projects.Resources;
using System.Text;
using ASC.Web.Studio.Utility;
using ASC.Projects.Core.Domain.Reports;

#endregion

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ProjectActivity : BaseUserControl, IUserActivityControlLoader
    {
        #region Members

        public Guid UserID { get; set; }
        public string HistoryRangeParams { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserID == Guid.Empty)
                UserID = SecurityContext.CurrentAccount.ID;


            var filter = new TaskFilter { ParticipantId = UserID, MyProjects = true };
            var tasks = Global.EngineFactory.GetTaskEngine().GetByFilter(filter);
            var projects = Global.EngineFactory.GetProjectEngine().GetByParticipant(UserID).Select(prj => new ProjectVm(prj)).OrderBy(r=> r.ProjectTitle).ToList();

            foreach (var project in projects)
            {
                project.OpenedTasksCount = tasks.Count(t => t.Project.ID == project.ProjectId && t.Status == TaskStatus.Open);
                project.ClosedTasksCount = tasks.Count(t => t.Project.ID == project.ProjectId && t.Status == TaskStatus.Closed);
            }

            ProjectsRepeater.DataSource = projects;
            ProjectsRepeater.DataBind();

            //load user activity
            //
            var activities = UserActivityManager.GetUserActivities(
                TenantProvider.CurrentTenantID,
                UserID,
                ProductEntryPoint.ID,
                new[] { ProductEntryPoint.ID },
                UserActivityConstants.AllActionType,
                null,
                0, 10);

            LastActivityRepeater.DataSource = activities.ConvertAll(a => new ActivityVm(a));
            LastActivityRepeater.DataBind();
        }

        #endregion

        #region Methods

        public string GetOpenedTasksString(int count)
        {
            return GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.ActiveTaskNominative, GrammaticalResource.ActiveTaskGenitiveSingular, GrammaticalResource.ActiveTaskGenitivePlural);
        }

        public string GetClosedTasksString(int count)
        {
            return GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.ClosedTaskNominative, GrammaticalResource.ClosedTaskGenitiveSingular, GrammaticalResource.ClosedTaskGenitivePlural);
        }

        public string GetActivitiesString(int count)
        {
            return GrammaticalHelper.ChooseNumeralCase(count, GrammaticalResource.ActionNominative, GrammaticalResource.ActionGenitiveSingular, GrammaticalResource.ActionGenitivePlural);
        }


        public string GetActivityReportUri()
        {
            var filter = new ReportFilter {TimeInterval = ReportTimeInterval.Absolute, UserId = UserID};
            return string.Format("{0}?action=generate&reportType=5&{1}",
                                 VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "reports.aspx"),
                                 filter.ToUri());
        }

        public string GetTasksReportUri()
        {
            var filter = new ReportFilter {TimeInterval = ReportTimeInterval.Absolute, UserId = UserID};
            filter.TaskStatuses.Add(TaskStatus.Open);
            filter.TaskStatuses.Add(TaskStatus.Closed);
            return string.Format("{0}?action=generate&reportType=10&{1}",
                                 VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "reports.aspx"),
                                 filter.ToUri());
        }

        #endregion

        #region IUserActivityControlLoader Members

        public Control LoadControl(Guid userID)
        {
            var cntrl = (ProjectActivity)LoadControl(PathProvider.GetControlVirtualPath("ProjectActivity.ascx"));
            cntrl.UserID = userID;
            return cntrl;
        }

        #endregion

        #region VM

        public class ProjectVm
        {
            public ProjectVm(Project project)
            {
                Project = project;
            }

            public Project Project;

            public int ProjectId
            {
                get { return Project.ID; }
            }

            public string ProjectTitle
            {
                get { return Project.Title; }
            }

            public int OpenedTasksCount { get; set; }
            public int ClosedTasksCount { get; set; }
            public int ActivityCount { get; set; }
        }

        public class ActivityVm
        {
            public ActivityVm(UserActivity activity)
            {
                Activity = activity;
            }

            public UserActivity Activity;

            public string DateString
            {
                get { return Activity.Date.ToString(DateTimeExtension.DateFormatPattern); }
            }

            public string TimeString
            {
                get { return Activity.Date.ToString("HH:mm"); }
            }

            public string EntityPlate
            {
                get
                {
                    var additionalDataParts = Activity.AdditionalData.Split(new[] { '|' });
                    var timeLineType = (EntityType)Enum.Parse(typeof(EntityType), additionalDataParts[0]);
                    return Global.RenderEntityPlate(timeLineType, true);
                }
            }

            public string EntityType
            {
                get
                {
                    var sb = new StringBuilder();

                    sb.AppendFormat("<a style='padding-right:10px' href='{0}'>{1}</a>",
                                    Activity.URL,
                                    HttpUtility.HtmlEncode(HtmlUtil.GetText(Activity.Title, 70)));

                    sb.AppendFormat("<span>{0}</span>", Activity.ActionText.ToLower());

                    return sb.ToString();
                }
            }

            public string EntityParentContainers
            {
                get
                {
                    var innerHTML = new StringBuilder();
                    var additionalDataParts = Activity.AdditionalData.Split(new[] { '|' });
                    if (additionalDataParts.Length < 3)
                    {
                        return string.Empty;
                    }

                    var timeLineType = (EntityType)Enum.Parse(typeof(EntityType), additionalDataParts[0]);

                    var parent = string.Empty;

                    switch (timeLineType)
                    {
                        case ASC.Projects.Core.Domain.EntityType.Comment:
                            if (Activity.URL.IndexOf("tasks") != -1)
                            {
                                if (additionalDataParts[1] != string.Empty)
                                    parent = string.Format("<span class='pm-grayText'>{0}</span><a style='padding-left:10px' href='{1}'>{2}</a>",
                                                           TaskResource.Task,
                                                           Activity.URL,
                                                           HtmlUtil.GetText(additionalDataParts[1], 50));
                            }
                            if (Activity.URL.IndexOf("milestones") != -1)
                            {
                                if (additionalDataParts[1] != string.Empty)
                                    parent = string.Format("<span class='pm-grayText'>{0}</span><a style='padding-left:10px' href='{1}'>{2}</a>",
                                                           MilestoneResource.Milestone,
                                                           Activity.URL,
                                                           HtmlUtil.GetText(additionalDataParts[1], 50));
                            }
                            if (Activity.URL.IndexOf("messages") != -1)
                            {
                                if (additionalDataParts[1] != string.Empty)
                                    parent = string.Format("<span class='pm-grayText'>{0}</span><a style='padding-left:10px' href='{1}'>{2}</a>",
                                                           MessageResource.Message,
                                                           Activity.URL,
                                                           HtmlUtil.GetText(additionalDataParts[1], 50));
                            }
                            break;
                        case ASC.Projects.Core.Domain.EntityType.Project:
                            break;
                        default:
                            if (additionalDataParts.Length > 2 && additionalDataParts[2] != string.Empty)
                                parent = string.Format("<span class='pm-grayText'>{0}</span><a style='padding-left:10px' href='{3}?prjID={1}'>{2}</a>",
                                                       ProjectResource.Project, Activity.ContainerID,
                                                       HtmlUtil.GetText(additionalDataParts[2], 50),
                                                       VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "projects.aspx"));
                            if (additionalDataParts.Length > 1 && additionalDataParts[1] != string.Empty)
                                parent = string.Format("<span class='pm-grayText'>{0}</span><a style='padding-left:10px' href='{4}?prjID={1}&id={2}'>{3}</a>",
                                                       MilestoneResource.Milestone, Activity.ContainerID, Activity.ContentID,
                                                       HtmlUtil.GetText(additionalDataParts[1], 50),
                                                       VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "milestones.aspx"));
                            break;
                    }
                    innerHTML.AppendLine(parent);

                    return innerHTML.ToString();
                }
            }
        }

        #endregion
    }

}