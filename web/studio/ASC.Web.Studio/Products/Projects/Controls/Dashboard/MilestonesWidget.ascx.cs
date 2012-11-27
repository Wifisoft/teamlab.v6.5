#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;

#endregion

namespace ASC.Web.Projects.Controls.Dashboard
{
    [Serializable]
    [DataContract]
    public class MilestonesWidgetSettings : ISettings
    {
        [DataMember(Name = "MilestonesCount")]
        public int MilestonesCount { get; set; }

        public Guid ID
        {
            get { return new Guid("{F1244734-FE07-4845-8A58-5D4DE2B87760}"); }
        }

        public ISettings GetDefault()
        {
            return new MilestonesWidgetSettings
            {
                MilestonesCount = 2
            };
        }
    }

    public partial class MilestonesWidget : BaseUserControl
    {
        private static Guid CurrentUser
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        private static MilestonesWidgetSettings WidgetSettings { get; set; }

        protected bool IsExistOverdueMilestones
        {
            get { return OverdueMilestones.Any(); }
        }
        
        protected bool IsShouldRenderNextMilestones
        {
            get { return (WidgetSettings.MilestonesCount - OverdueMilestones.Count() > 0)  && NextMilestones.Any() ; }
        }

        public static Guid WidgetId { get { return new Guid("{790C36BD-1C4B-48FF-A2C9-1E5FBB13F5A9}"); } }

        
        private IEnumerable<Milestone> OverdueMilestones { get; set; }
        private IEnumerable<Milestone> NextMilestones { get; set; }
        public IEnumerable<Milestone> Milestones { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            WidgetSettings = SettingsManager.Instance.LoadSettingsFor<MilestonesWidgetSettings>(CurrentUser);

            OverdueMilestones = Milestones.Where(m => m.DeadLine < DateTime.Today).Take(WidgetSettings.MilestonesCount);
            OverdueMilestonesRepeater.DataSource = OverdueMilestones;
            OverdueMilestonesRepeater.DataBind();

            NextMilestones = Milestones.Where(m => m.DeadLine >= DateTime.Today);
            NewMilestonesRepeater.DataSource = NextMilestones.Take(WidgetSettings.MilestonesCount - OverdueMilestones.Count());
            NewMilestonesRepeater.DataBind();
        }   

        public string GetShortMilestoneDeadline(DateTime deadline)
        {
            var date = deadline;
            var day = (deadline.Day / 10 == 0) ? "0" + deadline.Day : deadline.Day.ToString(CultureInfo.InvariantCulture);
            var month = ProjectsCommonResource.MonthNames_short.Split(',')[date.Month - 1];
            return day + " " + month;
        }

        public string GetProjectLink(int prjId)
        {
            return "projects.aspx?prjID=" + prjId;
        }

        public int GetDaysAfterMilestoneDeadline(DateTime deadline)
        {
            return (DateTime.Today - deadline).Days;
        }

        public int GetDaysAfterCreateMilestone(DateTime createOn)
        {
            return (DateTime.Today - createOn).Days;
        }

        public string GetReportUri()
        {
            var filter = new ReportFilter
                             {
                                 TimeInterval = ReportTimeInterval.Absolute,
                                 UserId = SecurityContext.CurrentAccount.ID
                             };
            filter.ProjectStatuses.Add(ProjectStatus.Open);
            return string.Format("reports.aspx?action=generate&reportType=7&{0}", filter.ToUri());
        }

        public string IsKeyMilestone(bool isKey)
        {
            return isKey
                ? string.Format("<img class='keyMilestone' src={0} alt={1} title={2}/>",
                        PathProvider.GetFileStaticRelativePath("key.png"), MilestoneResource.RootMilestone, MilestoneResource.RootMilestone)
                : string.Empty;
        }

        public bool IsExist(IEnumerable<Milestone> milestones)
        {
            return milestones.Any();
        }

        public string ResponsibleProfileLink(Guid responsible)
        {
            var userInfo = Global.EngineFactory.GetParticipantEngine().GetByID(responsible).UserInfo;

            return userInfo.RenderProfileLink(ProductEntryPoint.ID);
        }
    }
}
