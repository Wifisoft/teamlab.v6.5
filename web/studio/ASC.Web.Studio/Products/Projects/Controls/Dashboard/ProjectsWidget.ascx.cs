#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Web.Core.Utility.Settings;
using System.Runtime.Serialization;
using ASC.Web.Projects.Resources;

#endregion

namespace ASC.Web.Projects.Controls.Dashboard
{
    [Serializable]
    [DataContract]
    public class ProjectsWidgetSettings : ISettings
    {
        [DataMember(Name = "MyProjectsCount")]
        public int MyProjectsCount { get; set; }

        [DataMember(Name = "ShowFollowingProjects")]
        public bool ShowFollowingProjects { get; set; }

        [DataMember(Name = "FollowingProjectsCount")]
        public int FollowingProjectsCount { get; set; }

        [DataMember(Name = "TasksCount")]
        public int TasksCount { get; set; }

        [DataMember(Name = "ShowOnlyMyTasks")]
        public bool ShowOnlyMyTasks { get; set; }

        [DataMember(Name = "ShowFollowingTasks")]
        public bool ShowFollowingTasks { get; set; }

        public Guid ID
        {
            get { return new Guid("{CD67BA0A-D4C5-42E9-98AE-995CE36F134C}"); }
        }

        public ISettings GetDefault()
        {
            return new ProjectsWidgetSettings
            {
                MyProjectsCount = 5,
                ShowFollowingProjects = true,
                FollowingProjectsCount = 2,
                TasksCount = 5,
                ShowOnlyMyTasks = true,
                ShowFollowingTasks = true
            };
        }
    }

    public partial class ProjectsWidget : BaseUserControl
    {
        private int currentOverdueTasksCount;
        private int currentNextTasksCount;

        private List<Task> tasksInMyProjects;

        protected bool IsMyProjectsExist { get; set; }
        protected bool IsFollowingProjectsExist { get; set; }
        protected bool IsFollowingTasksExist { get; set; }

        private static TaskEngine TaskEngine
        {
            get { return Global.EngineFactory.GetTaskEngine(); }
        }

        private static ProjectsWidgetSettings WidgetSettings { get; set; }

        public bool ShowFollowingProjects { get; set; }
        public bool ShowFollowingTasks { get; set; }

        public static Guid WidgetId { get { return new Guid("{C732F249-96FC-4D27-AE5D-EFD9D256DA1E}"); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            WidgetSettings = SettingsManager.Instance.LoadSettingsFor<ProjectsWidgetSettings>(SecurityContext.CurrentAccount.ID);

            ShowFollowingProjects = WidgetSettings.ShowFollowingProjects;
            ShowFollowingTasks = WidgetSettings.ShowFollowingTasks;

            var taskFilter = new TaskFilter
            {
                 SortBy = "deadline",
                 SortOrder = true,
                 MyProjects = true,
                 TaskStatuses = new List<TaskStatus> { TaskStatus.Open }
            };

            if (WidgetSettings.ShowOnlyMyTasks)
            {
                taskFilter.ParticipantId = SecurityContext.CurrentAccount.ID;
            }

            tasksInMyProjects = TaskEngine.GetByFilter(taskFilter);

            var myProjects = new List<Project>(tasksInMyProjects.Select(r => r.Project).Distinct().Take(WidgetSettings.MyProjectsCount));

            if (!tasksInMyProjects.Any())
            {
                myProjects.AddRange(RequestContext.GetCurrentUserProjects().Take(WidgetSettings.MyProjectsCount));
            }

            IsMyProjectsExist = myProjects.Any();

            MyProjectsRepeater.DataSource = myProjects.OrderBy(r => r.Title);
            MyProjectsRepeater.DataBind();

            if (ShowFollowingProjects)
            {
                var followingProjects = new List<Project>(RequestContext.GetCurrentUserFollowingProjects().Take(WidgetSettings.FollowingProjectsCount));
                IsFollowingProjectsExist = followingProjects.Any();

                if (IsFollowingProjectsExist)
                {
                    FollowingProjectsRepeater.DataSource = followingProjects;
                    FollowingProjectsRepeater.DataBind();
                }
            }
        }

        public string GetProjectLink(int prjId)
        {
            return "projects.aspx?prjID=" + prjId;
        }

        public List<Task> GetMyOverdueTasks(int projectId)
        {
            var tasks = tasksInMyProjects.Where(t => t.Project.ID == projectId
                && t.Deadline != DateTime.MinValue && t.Deadline.Date <= DateTime.Today.Date)
            .Take(WidgetSettings.TasksCount).ToList();
            currentOverdueTasksCount = tasks.Count;
            return tasks;
        }

        public List<Task> GetMyNewTasks(int projectId)
        {
            var takeCount = WidgetSettings.TasksCount - currentOverdueTasksCount;
            var tasks = tasksInMyProjects.Where(t => t.Project.ID == projectId &&
                t.CreateOn.Date >= DateTime.UtcNow.AddDays(-3).Date && t.CreateOn.Date <= DateTime.UtcNow.Date &&
                (t.Deadline == DateTime.MinValue || t.Deadline.Date > DateTime.Today.Date))
            .Take(takeCount).ToList();
            currentNextTasksCount = tasks.Count;
            return tasks;
        }

        public List<Task> GetMyOtherTasks(int projectId)
        {
            var takeCount = WidgetSettings.TasksCount - currentOverdueTasksCount - currentNextTasksCount;
            var tasks = tasksInMyProjects.Where(t => t.Project.ID == projectId &&
                t.CreateOn.Date < DateTime.UtcNow.AddDays(-3).Date &&
                (t.Deadline == DateTime.MinValue || t.Deadline.Date > DateTime.Today.Date))
            .Take(takeCount).ToList();
            return tasks;
        }


        public string GetTaskLink(Task task)
        {
            return String.Format("tasks.aspx?prjID={0}&id={1}", task.Project.ID, task.ID);
        }

        public string GetShortTaskDeadline(Task task)
        {
            var date = task.Deadline;
            var day = (task.Deadline.Day / 10 == 0) ? "0" + task.Deadline.Day : task.Deadline.Day.ToString(CultureInfo.InvariantCulture);
            var month = ProjectsCommonResource.MonthNames_short.Split(',')[date.Month - 1];
            return day + " " + month;
        }
    }
}
