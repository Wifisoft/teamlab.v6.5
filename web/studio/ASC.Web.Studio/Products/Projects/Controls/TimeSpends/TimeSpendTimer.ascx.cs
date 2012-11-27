using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Classes;
using ASC.Core;
using ASC.Projects.Core.Domain;
using System.Collections.Generic;
using System.Globalization;

namespace ASC.Web.Projects.Controls.TimeSpends
{
    public partial class TimeSpendTimer : BaseUserControl
    {
        #region Properties

        public ProjectFat ProjectFat { get; set; }

        public int Target { get; set; }

        protected List<Task> UserTasks { get; set; }

        protected List<Project> UserProjects { get; set; }

        protected string DecimalSeparator { get { return CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator; } }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = SecurityContext.CurrentAccount.ID;
            UserTasks = Global.EngineFactory.GetTaskEngine().GetByProject(ProjectFat.Project.ID, TaskStatus.Open, currentUser);
            var projects = Global.EngineFactory.GetProjectEngine().GetByParticipant(currentUser);
            UserProjects = projects.FindAll(p =>
                Global.EngineFactory.GetTaskEngine().GetByProject(p.ID, TaskStatus.Open, currentUser).Count>0
                );
            UserTasks.Sort((x, y) => String.Compare(x.Title, y.Title));
            UserProjects.Sort((x, y) => String.Compare(x.Title, y.Title));
        }

        #endregion

        #region Methods

        protected string GetPlayButtonImg()
        {
            return WebImageSupplier.GetAbsoluteWebPath("play.png", ProductEntryPoint.ID);
        }

        protected string GetPauseButtonImg()
        {
            return WebImageSupplier.GetAbsoluteWebPath("pause.png", ProductEntryPoint.ID);
        }

        protected string GetRefreshButtonImg()
        {
            return WebImageSupplier.GetAbsoluteWebPath("refresh.png", ProductEntryPoint.ID);
        }

        protected string GetResetButtonImg()
        {
            return WebImageSupplier.GetAbsoluteWebPath("reset.png", ProductEntryPoint.ID);
        }

        #endregion
    }
}