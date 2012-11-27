#region Usings

using System;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using System.Linq;

#endregion

namespace ASC.Web.Projects.Controls.Messages
{
    public partial class DiscussionsList : BaseUserControl
    {
        public int ProjectId { get; set; }

        private static Guid CurrentUserId { get { return SecurityContext.CurrentAccount.ID; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadControls();

            GetApiData();
        }

        private void LoadControls()
        {
            var advansedFilter = new Studio.Controls.Common.AdvansedFilter { BlockID = "AdvansedFilter" };
            contentPlaceHolder.Controls.Add(advansedFilter);

            var emptyScreenControlFilter = new Studio.Controls.Common.EmptyScreenControl
            {
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty-filter.png", ProductEntryPoint.ID),
                Header = MessageResource.FilterNoDiscussions,
                Describe = MessageResource.DescrEmptyListMilFilter,
                ButtonHTML = String.Format("<a href='javascript:void(0)' class='baseLinkAction'>{0}</a>", ProjectsFilterResource.ClearFilter)
            };
            emptyScreenHolder.Controls.Add(emptyScreenControlFilter);

            var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
            {
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_discussions.png", ProductEntryPoint.ID),
                Header = MessageResource.DiscussionNotFound_Header,
                Describe = MessageResource.DiscussionNotFound_Describe
            };

            if (CanCreateDiscussion())
                emptyScreenControl.ButtonHTML = String.Format("<a class='baseLinkAction'>{0}</a>", MessageResource.StartFirstDiscussion);

            emptyScreenHolder.Controls.Add(emptyScreenControl);
        }

        private void GetApiData()
        {
            var apiServer = new Api.ApiServer();
            const string fields = "id,title";

            if (!RequestContext.IsInConcreteProject())
            {
                var tags = apiServer.GetApiResponse(string.Format("api/1.0/project/tag.json"), "GET");
                Page.JsonPublisher(tags, "tags");

                var projects = apiServer.GetApiResponse(ProjectSecurity.IsAdministrator(CurrentUserId) ?
                    string.Format("api/1.0/project/filter.json?status=open&sortBy=title&sortOrder=ascending&fields={0}", fields) :
                    string.Format("api/1.0/project/filter.json?participant={0}&status=open&sortBy=title&sortOrder=ascending&fields={1}", CurrentUserId, fields),
                "GET");
                Page.JsonPublisher(projects, "projects");
            }
        }

        public bool CanCreateDiscussion()
        {
            if (RequestContext.IsInConcreteProject())
                return ProjectSecurity.CanCreateMessage(RequestContext.GetCurrentProject());

            return ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID) 
                ? RequestContext.HasAnyProjects()
                : Global.EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID).Where(ProjectSecurity.CanCreateMessage).Any();
        }
    }
}
