#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Tasks;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using ASC.Web.Projects.Controls.Common;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.Projects
{
    public partial class Tasks : BasePage
    {
        protected Project Project { get; set; }

        protected override void PageLoad()
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(TaskDescriptionView), Page);
            ((IStudioMaster)Master).DisabledSidePanel = true;

            Project = RequestContext.GetCurrentProject(false);

            if (RequestContext.IsInConcreteProject() && Project == null)
            {
                Response.Redirect("tasks.aspx", true);
            }
           
            InitBreadCrumbs();

            int taskID;

            if (Int32.TryParse(UrlParameters.EntityID, out taskID))
            {
                if (Project == null) return;

                var apiServer = new Api.ApiServer();
                var task = apiServer.GetApiResponse(String.Format("api/1.0/project/task/{0}.json", taskID), "GET");
                JsonPublisher(task, "taskDescription");

                var jTask = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(task)));

                if (jTask["count"].ToObject<int>() > 0)
                {
                    var taskDescriptionView = (TaskDescriptionView)LoadControl(PathProvider.GetControlVirtualPath("TaskDescriptionView.ascx"));

                    var taskDescription = new Task
                                              {
                                                  ID = jTask["response"]["id"].ToObject<int>(),
                                                  Title = jTask["response"]["title"].ToString(),
                                                  Status = jTask["response"]["status"].ToObject<TaskStatus>(),
                                                  Project = Project
                                              };

                    taskDescriptionView.Task = taskDescription;
                    taskDescriptionView.CanEditTask = (bool)jTask["response"]["canEdit"];
                    taskDescriptionView.CanDeleteTask = (int)jTask["response"]["canWork"] == 3;

                    _content.Controls.Add(taskDescriptionView);
                    Master.BreadCrumbs.Add(new BreadCrumb());
                }
                else
                {
                    TaskNotFoundControlView(Project.ID);
                    Master.BreadCrumbs.Clear();
                }
            }
            else
            {
               var advansedFilter = new Studio.Controls.Common.AdvansedFilter { BlockID = "AdvansedFilter" };
                _filter.Controls.Add(advansedFilter);

                var taskList = (TaskList) LoadControl(PathProvider.GetControlVirtualPath("TaskList.ascx"));
                _content.Controls.Add(taskList);

                var emptyScreenControlFilter = new Studio.Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty-filter.png", ProductEntryPoint.ID),
                    Header = TaskResource.NoTasks,
                    Describe = TaskResource.DescrEmptyListTaskFilter,
                    ButtonHTML = String.Format("<span class='baseLinkAction clearFilterButton'>{0}</span>", ProjectsFilterResource.ClearFilter)
                };
                _content.Controls.Add(emptyScreenControlFilter);

                var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),
                                                 Header = TaskResource.NoTasksCreated,
                                                 Describe = String.Format(TaskResource.TasksHelpTheManage)
                                             };

                if (CanCreateTask())
                    emptyScreenControl.ButtonHTML = String.Format("<span class='baseLinkAction addFirstElement'>{0}</span>", TaskResource.AddFirstTask);

                _content.Controls.Add(emptyScreenControl);
            }

            GetApiData();

            Title = HeaderStringHelper.GetPageTitle(TaskResource.Tasks, Master.BreadCrumbs);    
        }

        protected void TaskNotFoundControlView(int projectId)
        {
            _content.Controls.Add(new ElementNotFoundControl
            {
                Header = TaskResource.TaskNotFound_Header,
                Body = TaskResource.TaskNotFound_Body,
                RedirectURL = String.Format("tasks.aspx?prjID=" + projectId),
                RedirectTitle = TaskResource.TaskNotFound_RedirectTitle
            });
            Title = HeaderStringHelper.GetPageTitle(TaskResource.TaskNotFound_Header, Master.BreadCrumbs);
        }

        public string GetProjectId()
        {
            return RequestContext.GetCurrentProjectId().ToString(CultureInfo.InvariantCulture);
        }

        public void InitBreadCrumbs()
        {
            if (Project == null)
            {
                Master.BreadCrumbs.Add(new BreadCrumb(TaskResource.Tasks));
                return;
            }

            Master.BreadCrumbs.Add(new BreadCrumb(ProjectResource.Projects, "projects.aspx"));
            Master.BreadCrumbs.Add(new BreadCrumb(Project.HtmlTitle, "projects.aspx?prjID=" + Project.ID));
            Master.BreadCrumbs.Add(new BreadCrumb(TaskResource.Tasks, "tasks.aspx?prjID=" + Project.ID));
        }

        private void GetApiData()
        {
            var apiServer = new Api.ApiServer();
            var currentUserId = SecurityContext.CurrentAccount.ID;

            if (RequestContext.IsInConcreteProject())
            {
                var projectParticipants = apiServer.GetApiResponse(string.Format("api/1.0/project/{0}/team.json?fields={1}", 
                    RequestContext.GetCurrentProjectId(), "id,displayName"), "GET");
                JsonPublisher(projectParticipants, "projectTeam");

                var milestones = apiServer.GetApiResponse(string.Format("api/1.0/project/milestone/filter.json?sortBy=deadline&sortOrder=descending&status=open&projectId={0}&fields={1}",
                            RequestContext.GetCurrentProjectId(), "id,title,deadline"), "GET");
                JsonPublisher(milestones, "milestones");
            }
            else
            {
                var milestones = apiServer.GetApiResponse(ProjectSecurity.IsAdministrator(currentUserId) 
                    ? string.Format("api/1.0/project/milestone/filter.json?sortBy=deadline&sortOrder=descending&status=open&fields={0}", "id,title,deadline")
                    : string.Format("api/1.0/project/milestone/filter.json?sortBy=deadline&sortOrder=descending&status=open&participant={0}&fields={1}", currentUserId, "id,title,deadline"), 
                    "GET");
                JsonPublisher(milestones, "milestones");

                var tags = apiServer.GetApiResponse(string.Format("api/1.0/project/tag.json"), "GET");
                JsonPublisher(tags, "tags");

                var projects = apiServer.GetApiResponse(ProjectSecurity.IsAdministrator(currentUserId)
                    ? string.Format("api/1.0/project/filter.json?status=open&sortBy=title&sortOrder=ascending&fields={0}", "id,title,security,isPrivate")
                    : string.Format("api/1.0/project/filter.json?participant={0}&status=open&sortBy=title&sortOrder=ascending&fields={1}", currentUserId, "id,title,security,isPrivate"),
                    "GET");
                JsonPublisher(projects, "projects");
            }
        }

        public bool CanCreateTask()
        {
            if (RequestContext.IsInConcreteProject())
                return ProjectSecurity.CanCreateTask(RequestContext.GetCurrentProject());

            return ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID)
                ? RequestContext.HasAnyProjects()
                : Global.EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID).Where(ProjectSecurity.CanCreateTask).Any();
        }
    }
}
