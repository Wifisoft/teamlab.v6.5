#region Usings

using System;
using System.Linq;
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using System.Text;
using ASC.Core;
using AjaxPro;
using ASC.Web.Projects.Controls.TimeSpends;
using ASC.Projects.Engine;

#endregion

namespace ASC.Web.Projects
{
    [AjaxNamespace("AjaxPro.TimeTracking")]
    public partial class TimeTracking : BasePage
    {
        #region Properties

        public ProjectFat ProjectFat { get; set; }

        public static int TaskID
        {
            get
            {
                int id;
                if (Int32.TryParse(UrlParameters.EntityID, out id))
                {
                    return id;
                }
                return -1;
            }
        }

        protected bool IsTimer
        {
            get { return UrlParameters.ActionType == "timer"; }
        }

        protected float TotalTime { get; set; }

        #endregion

        #region Events

        protected override void PageLoad()
        {
            ((IStudioMaster)Master).DisabledSidePanel = true;

            if (!Global.ModuleManager.IsVisible(ModuleType.TimeTracking))
                Response.Redirect(ProjectsCommonResource.StartURL);

            var project = RequestContext.GetCurrentProject(false);

            if (RequestContext.IsInConcreteProject() && project == null)
            {
                Response.Redirect("default.aspx", true);
            }

            ProjectFat = new ProjectFat(project);

            if (!IsTimer)
            {
                if (TaskID <= 0)
                {
                    var advansedFilter = new Studio.Controls.Common.AdvansedFilter { BlockID = "AdvansedFilter" };
                    _filter.Controls.Add(advansedFilter);

                    var emptyScreenControlFilter = new Studio.Controls.Common.EmptyScreenControl
                    {
                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty-filter.png", ProductEntryPoint.ID),
                        Header = TimeTrackingResource.NoTimersFilter,
                        Describe = TimeTrackingResource.DescrEmptyListTimersFilter,
                        ButtonHTML = String.Format("<span class='baseLinkAction clearFilterButton'>{0}</span>", ProjectsFilterResource.ClearFilter)
                    };
                    emptyScreenFilter.Controls.Add(emptyScreenControlFilter);
                }

                InitPage(ProjectFat);

                var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_time_tracking.png", ProductEntryPoint.ID),
                    Header = TimeTrackingResource.NoTtimers,
                    Describe = String.Format(TimeTrackingResource.NoTimersNote)
                };

                if (CanCreateTime())
                    emptyScreenControl.ButtonHTML = String.Format("<span class='baseLinkAction addFirstElement'>{0}</span>", TimeTrackingResource.StartTimer);

                emptyScreen.Controls.Add(emptyScreenControl);

                var apiServer = new Api.ApiServer();

                if (TaskID > 0)
                {
                    var timesForFirstRequest = apiServer.GetApiResponse(String.Format("api/1.0/project/task/{0}/time.json", TaskID),"GET");
                    JsonPublisher(timesForFirstRequest, "timesForFirstRequest");
                }


                if (RequestContext.IsInConcreteProject())
                {
                    var projectTeam = apiServer.GetApiResponse(string.Format("api/1.0/project/{0}/team.json?fields={1}", RequestContext.GetCurrentProjectId(), "id,displayName"), "GET");
                    JsonPublisher(projectTeam, "projectTeam");
                }

                TotalTime = TaskID > 0
                                ? Global.EngineFactory.GetTimeTrackingEngine().GetByTask(TaskID).Sum(r => r.Hours)
                                : Global.EngineFactory.GetTimeTrackingEngine().GetByProject(ProjectFat.Project.ID).Sum(r => r.Hours);
            }
            else
            {   
                var taskId = TaskID;
                if (taskId > 0)
                {
                    var t = Global.EngineFactory.GetTaskEngine().GetByID(taskId);
                    if (t == null || t.Status == TaskStatus.Closed) taskId = -1;
                }
                Master.DisabledSidePanel = true;
                var cntrlTimer = (TimeSpendActionTimer)LoadControl(PathProvider.GetControlVirtualPath("TimeSpendActionTimer.ascx"));

                if (ProjectFat != null)
                    cntrlTimer.Project = ProjectFat.Project;

                cntrlTimer.Target = taskId;
                _phTimeSpendTimer.Controls.Add(cntrlTimer);
                Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.AutoTimer, Master.BreadCrumbs);
            }
        }

        #endregion

        #region Methods

        public void InitPage(ProjectFat projectFat)
        {
            if (RequestContext.GetCurrentProject(false) != null)
            {
                Master.BreadCrumbs.Add(new BreadCrumb(ProjectResource.Projects, "projects.aspx"));
                if (projectFat != null)
                {
                    Master.BreadCrumbs.Add(new BreadCrumb(projectFat.Project.HtmlTitle,
                                                          "projects.aspx?prjID=" + projectFat.Project.ID));
                }
            }
            if (TaskID > 0)
            {
                Master.BreadCrumbs.Add(new BreadCrumb(ProjectsCommonResource.TimeTracking,
                                                      "timetracking.aspx?prjID=" + projectFat.Project.ID));
                Master.BreadCrumbs.Add(new BreadCrumb(Global.EngineFactory.GetTaskEngine().GetByID(TaskID).Title));
            }
            else
            {
                Master.BreadCrumbs.Add(new BreadCrumb(ProjectsCommonResource.TimeTracking));
            }
            

            Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.TimeTracking, Master.BreadCrumbs);
        }

        public bool CanCreateTime()
        {
            return ProjectFat.GetTasks().Count(t => t.Status == TaskStatus.Open) > 0 && ProjectSecurity.CanCreateTimeSpend(ProjectFat.Project);
        }

        protected string GetTimes()
        {
            var hours = (int)TotalTime;
            var minutes = (int)((TotalTime - hours) * 60);
            return hours + ":" + minutes.ToString("D2");
        }

        #endregion

    }
}
