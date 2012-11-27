#region Usings

using System;
using ASC.Core;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls.History;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    public partial class History : BasePage
    {
        private static Guid CurrentUserId { get { return SecurityContext.CurrentAccount.ID; } }

        protected override void PageLoad()
        {
            ((IStudioMaster)Master).DisabledSidePanel = true;

            var project = RequestContext.GetCurrentProject(false);

            if (RequestContext.IsInConcreteProject() && project == null)
            {
                Response.Redirect("history.aspx", true);
            }

            LoadControls();

            GetApiData();

            Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.History, ProjectsCommonResource.History);
        }

        private void LoadControls()
        {
            var advansedFilter = new Studio.Controls.Common.AdvansedFilter { BlockID = "AdvansedFilter" };
            contentPlaceHolder.Controls.Add(advansedFilter);

            var historyList = (HistoryList)LoadControl(PathProvider.GetControlVirtualPath("HistoryList.ascx"));
            activitiesListPlaceHolder.Controls.Add(historyList);

            var emptyScreenControlFilter = new Studio.Controls.Common.EmptyScreenControl
                                               {
                                                   ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty-filter.png", ProductEntryPoint.ID),
                                                   Header = ProjectResource.FilterNoActivities,
                                                   Describe = ProjectResource.DescrEmptyListActivitiesFilter,
                                                   ButtonHTML = string.Format( "<a href='javascript:void(0)' class='baseLinkAction clearFilterButton'>{0}</a>", ProjectsFilterResource.ClearFilter)
                                               };
            activitiesListPlaceHolder.Controls.Add(emptyScreenControlFilter);

            var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
                                         {
                                             ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_history.png", ProductEntryPoint.ID),
                                             Header = ProjectResource.HistoryNotFound_Header,
                                             Describe = String.Format(ProjectResource.HistoryNotFound_Body)
                                         };

            activitiesListPlaceHolder.Controls.Add(emptyScreenControl);
        }


        private void GetApiData()
        {
            if (RequestContext.IsInConcreteProject()) return;

            var apiServer = new Api.ApiServer();
            var projects = apiServer.GetApiResponse(string.Format("api/1.0/project/filter.json?status=open&sortBy=title&sortOrder=ascending&fields={0}", "id,title"), "GET");
            JsonPublisher(projects, "projects");
        }
    }
}
