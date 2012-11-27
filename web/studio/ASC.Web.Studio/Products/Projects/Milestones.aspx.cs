#region Usings

using System;
using System.Linq;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    public partial class Milestones : BasePage
    {
        private static Guid CurrentUserId { get { return SecurityContext.CurrentAccount.ID; } }

        protected override void PageLoad()
        {
            if (RequestContext.IsInConcreteProject())
            {
                var project = RequestContext.GetCurrentProject(false);

                if (project == null)
                {
                    Response.Redirect("milestones.aspx", true);
                }
            }

            Bootstrap();

            LoadControls();

            GetApiData();
        }

        private void Bootstrap()
        {
            ((IStudioMaster)Master).DisabledSidePanel = true;

            _hintPopupTasks.Options.IsPopup = true;
            _hintPopupTaskRemove.Options.IsPopup = true;

            if (RequestContext.IsInConcreteProject())
            {
                Master.BreadCrumbs.Add(new BreadCrumb(ProjectResource.Projects, "projects.aspx"));
                Master.BreadCrumbs.Add(new BreadCrumb(RequestContext.GetCurrentProject().HtmlTitle,
                    "projects.aspx?prjID=" + RequestContext.GetCurrentProjectId()));
            }

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = MilestoneResource.Milestones
            });

            Title = HeaderStringHelper.GetPageTitle(MilestoneResource.Milestones, Master.BreadCrumbs);
        }

        private void LoadControls()
        {
            var advansedFilter = new Studio.Controls.Common.AdvansedFilter { BlockID = "AdvansedFilter" };
            contentPlaceHolder.Controls.Add(advansedFilter);

            var milestoneList = (Controls.Milestones.MilestoneList)LoadControl(PathProvider.GetControlVirtualPath("MilestoneList.ascx"));
            milestoneListPlaceHolder.Controls.Add(milestoneList);

            var milestoneAction = (Controls.Milestones.MilestoneAction)LoadControl(PathProvider.GetControlVirtualPath("MilestoneAction.ascx"));
            milestoneActionPlaceHolder.Controls.Add(milestoneAction);

            var taskAction = (Controls.Tasks.TaskAction)LoadControl(PathProvider.GetControlVirtualPath("TaskAction.ascx"));
            taskActionPlaceHolder.Controls.Add(taskAction);

            var emptyScreenControlFilter = new Studio.Controls.Common.EmptyScreenControl
            {
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty-filter.png", ProductEntryPoint.ID),
                Header = MilestoneResource.FilterNoMilestones,
                Describe = MilestoneResource.DescrEmptyListMilFilter,
                ButtonHTML = String.Format("<a href='javascript:void(0)' class='baseLinkAction'>{0}</a>", ProjectsFilterResource.ClearFilter)
            };
            milestoneListPlaceHolder.Controls.Add(emptyScreenControlFilter);

            var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
            {
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_milestones.png", ProductEntryPoint.ID),
                Header = MilestoneResource.MilestoneNotFound_Header,
                Describe = String.Format(MilestoneResource.MilestonesMarkMajorTimestamps)
            };

            if (CanCreateMilestone())
                emptyScreenControl.ButtonHTML = String.Format("<a class='baseLinkAction'>{0}</a>", MilestoneResource.PlanFirstMilestone);

            milestoneListPlaceHolder.Controls.Add(emptyScreenControl);
        }

        private void GetApiData()
        {
            var apiServer = new Api.ApiServer();

            if (RequestContext.IsInConcreteProject())
            {
                var projectParticipants = apiServer.GetApiResponse(string.Format("api/1.0/project/{0}/team.json?fields={1}", RequestContext.GetCurrentProjectId(), "id,displayName"), "GET");
                JsonPublisher(projectParticipants, "projectParticipants");
                return;
            }

            var tags = apiServer.GetApiResponse(string.Format("api/1.0/project/tag.json"), "GET");
            JsonPublisher(tags, "tags");

            var projects = apiServer.GetApiResponse(ProjectSecurity.IsAdministrator(CurrentUserId) ?
                string.Format("api/1.0/project/filter.json?status=open&sortBy=title&sortOrder=ascending&fields={0}", "id,title") :
                string.Format("api/1.0/project/filter.json?status=open&sortBy=title&sortOrder=ascending&participant={0}&fields={1}", CurrentUserId, "id,title,responsible,status"),
            "GET");
            JsonPublisher(projects, "projects");
        }

        public bool CanCreateMilestone()
        {
            if (RequestContext.IsInConcreteProject())
                return ProjectSecurity.CanCreateMilestone(RequestContext.GetCurrentProject());

            return ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID)
                ? RequestContext.HasAnyProjects()
                : Global.EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID).Where(ProjectSecurity.CanCreateMilestone).Any();
        }
    }
}
