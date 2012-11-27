using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;

namespace ASC.Web.Projects.Controls.TimeSpends
{
    public partial class TimeSpendActionTimer : BaseUserControl
    {
        #region Properties

        public Project Project { get; set; }

        public int Target { get; set; }

        protected Guid CurrentUser { get; set; }

        protected List<Participant> Users { get; set; }      

        protected List<Project> UserProjects { get; set; }

        protected IEnumerable<Task> OpenUserTasks { get; set; }

        protected IEnumerable<Task> ClosedUserTasks { get; set; }

        protected string DecimalSeparator { get { return CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator; } }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = SecurityContext.CurrentAccount.ID;

            var filter = new TaskFilter
            {
                SortBy = "title",
                SortOrder = true,
                ProjectStatuses = new List<ProjectStatus> { ProjectStatus.Open }
            };

            if (!ProjectSecurity.IsAdministrator(CurrentUser))
                filter.ParticipantId = CurrentUser;

            UserProjects = Global.EngineFactory.GetProjectEngine().GetByFilter(filter);

            if (UserProjects.Any() && (Project == null || !UserProjects.Contains(Project)))
                Project = UserProjects[0];

            var tasks = Global.EngineFactory.GetTaskEngine().GetByProject(Project.ID, null, Guid.Empty);

            OpenUserTasks = tasks.Where(r => r.Status == TaskStatus.Open).OrderBy(r => r.Title);
            ClosedUserTasks = tasks.Where(r => r.Status == TaskStatus.Closed).OrderBy(r => r.Title);

            Users = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID).OrderBy(r => DisplayUserSettings.GetFullUserName(r.UserInfo)).ToList();

            if (!string.IsNullOrEmpty(Request.QueryString["taskId"]))
            {
                Target = int.Parse(Request.QueryString["taskId"]);
            }
        }

        #endregion

        protected string GetPlayButtonImg()
        {
            return WebImageSupplier.GetAbsoluteWebPath("play.png", ProductEntryPoint.ID);
        }

        protected string GetPauseButtonImg()
        {
            return WebImageSupplier.GetAbsoluteWebPath("pause.png", ProductEntryPoint.ID);
        }

    }
}