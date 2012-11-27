#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ASC.Projects.Core.Domain;
using ASC.Core;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Core.Users;
using ASC.Web.Projects.Controls.Common;
using ASC.Web.Projects.Resources;

using System.Linq;
using ASC.Web.Controls;
using ASC.Projects.Core.Domain.Reports;
#endregion

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ListProjectView : BaseUserControl
    {
        public enum ProjectFilter
        {
            Default = 0,
            My = 4,
            All = 1,
            Following = 2,
            ByLabel = 3

        }

        #region Members

        public bool ShowClosedProjects { get; set; }
        public string Tag { get; set; }
        public ProjectFilter CurrentFilter { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitView();
            InitFilterBlock();
        }

        #endregion

        #region Methods

        public List<WrapperListProjectView> GetRepeaterDataSourceList(IList<object[]> projectCollection)
        {

            List<WrapperListProjectView> list = new List<WrapperListProjectView>();

            foreach (var collectionItem in projectCollection)
            {
                var columns = (object[])collectionItem;

                list.Add(new WrapperListProjectView
                {
                    project_id = columns[0],
                    project_title = columns[1],
                    project_leader_id = columns[2],
                    project_status = columns[3],
                    milestones_count = columns[4],
                    tasks_count = columns[5],
                    participants_count = columns[6],
                });
            }

            return list;
        }

        protected void InitFilterBlock()
        {
            var SortControl = new ViewSwitcher();

            var combobox = new ViewSwitcherCombobox();
            combobox.SortLabel = ProjectResource.ProjectsByLabel;
            combobox.HintText = String.IsNullOrEmpty(Tag) ? String.Empty : Tag;

            combobox.ComboboxItems = new List<ViewSwitcherComboboxItem>();

            foreach (var label in Global.EngineFactory.GetTagEngine().GetTags())
                combobox.ComboboxItems.Add(new ViewSwitcherComboboxItem
                {
                    SortLabel = label.Value,
                    SortUrl = string.Format("projects.aspx?{0}={1}&{4}={2}{3}", UrlConstant.ProjectsFilter, ProjectFilter.ByLabel.ToString().ToLower(), HttpUtility.UrlEncode(label.Value), ShowClosedProjects ? "&view=all" : String.Empty, UrlConstant.ProjectsTag)

                });

            SortControl.SortItems.Add(new ViewSwitcherLinkItem
            {
                IsSelected = ProjectFilter.My == CurrentFilter || ProjectFilter.Default == CurrentFilter ? true : false,
                SortUrl = string.Format("projects.aspx?{0}={1}{2}", UrlConstant.ProjectsFilter, ProjectFilter.My.ToString().ToLower(), ShowClosedProjects ? "&view=all" : String.Empty),
                SortLabel = ProjectResource.MyProjects
            });



            SortControl.SortItems.Add(new ViewSwitcherLinkItem()
            {
                IsSelected = ProjectFilter.Following == CurrentFilter ? true : false,
                SortUrl = string.Format("projects.aspx?{0}={1}{2}", UrlConstant.ProjectsFilter, ProjectFilter.Following.ToString().ToLower(), ShowClosedProjects ? "&view=all" : String.Empty),
                SortLabel = ProjectResource.ProjectsKeepTrack,

            });

            if (combobox.ComboboxItems.Count > 0)
                SortControl.SortItems.Add(combobox);

            SortControl.SortItems.Add(new ViewSwitcherLinkItem()
            {
                IsSelected = ProjectFilter.All == CurrentFilter ? true : false,
                SortUrl = string.Format("projects.aspx?{0}={1}{2}", UrlConstant.ProjectsFilter, ProjectFilter.All.ToString().ToLower(), ShowClosedProjects ? "&view=all" : String.Empty),
                SortLabel = ProjectResource.AllProjects
            });

            //SortControl.SortItemsHeader = ProjectsCommonResource.Show + ":";

            _tabsContainer.Controls.Add(SortControl);

        }

        protected void InitControls()
        {

            EmptyScreenControl emptyScreenControl = new EmptyScreenControl();

            switch (CurrentFilter)
            {

                case ProjectFilter.My:
                case ProjectFilter.Default:
                    emptyScreenControl.HeaderContent = ShowClosedProjects ? ProjectResource.EmptyScreenMyProjectsHeaderContent : ProjectResource.EmptyScreenMyActiveProjectsHeaderContent;
                    break;

                case ProjectFilter.All:
                    emptyScreenControl.HeaderContent = ShowClosedProjects ? ProjectResource.EmptyScreenAllProjectsHeaderContent : ProjectResource.EmptyScreenAllActiveProjectsHeaderContent;
                    break;

                case ProjectFilter.Following:
                    emptyScreenControl.HeaderContent = ShowClosedProjects ? ProjectResource.EmptyScreenKeepTrackProjectsHeaderContent : ProjectResource.EmptyScreenKeepTrackActiveProjectsHeaderContent;
                    break;

                case ProjectFilter.ByLabel:
                    emptyScreenControl.HeaderContent = ShowClosedProjects ? ProjectResource.EmptyScreenByLabelProjectsHeaderContent : ProjectResource.EmptyScreenByLabelActiveProjectsHeaderContent;
                    break;
            }

            _phEmptyScreen.Controls.Add(emptyScreenControl);

        }

        protected void InitView()
        {

            if (String.Compare(Request["view"], "all", true) == 0)
                ShowClosedProjects = true;

            var list = RecieveData();

            if (list.Count == 0 && CurrentFilter == ProjectFilter.Default)
            {
                CurrentFilter = ProjectFilter.All;
                list = RecieveData();
            }

            string caption = String.Empty;
            switch (CurrentFilter)
            {
                case ASC.Web.Projects.Controls.Projects.ListProjectView.ProjectFilter.My:
                case ASC.Web.Projects.Controls.Projects.ListProjectView.ProjectFilter.Default:
                    caption = ProjectResource.MyProjects;
                    break;
                case ASC.Web.Projects.Controls.Projects.ListProjectView.ProjectFilter.All:
                    caption = ProjectResource.AllProjects;
                    break;
                case ASC.Web.Projects.Controls.Projects.ListProjectView.ProjectFilter.Following:
                    caption = ProjectResource.ProjectsKeepTrack;
                    break;
                case ASC.Web.Projects.Controls.Projects.ListProjectView.ProjectFilter.ByLabel:
                    caption = UrlParameters.ProjectsTag;
                    break;
            }
            (Page.Master as ASC.Web.Projects.Masters.BasicTemplate).BreadCrumbs.Add(new BreadCrumb { Caption = caption });


            InitControls();

            _rptContent.DataSource = GetRepeaterDataSourceList(list);
            _rptContent.DataBind();

        }

        protected IList<object[]> RecieveData()
        {

            var projectCollection = new ArrayList();
            var filter = new ReportFilter();
            if (!ShowClosedProjects) filter.ProjectStatuses.Add(ProjectStatus.Open);

            switch (CurrentFilter)
            {
                case ProjectFilter.My:
                case ProjectFilter.Default:
                    filter.UserId = SecurityContext.CurrentAccount.ID;
                    break;

                case ProjectFilter.Following:
                    filter.SetProjectIds(Global.EngineFactory.GetParticipantEngine().GetFollowingProjects(SecurityContext.CurrentAccount.ID));
                    break;

                case ProjectFilter.ByLabel:
                    filter = new ReportFilter();
                    filter.SetProjectIds(Global.EngineFactory.GetTagEngine().GetTagProjects(Tag));
                    break;
            }

            return Global.EngineFactory.GetReportEngine().BuildProjectsListReport(filter);
        }

        protected string GetPrivateImg(int projectID)
        {
            if (Global.EngineFactory.GetProjectEngine().GetByID(projectID).Private)
                return string.Format("<img src='{0}' title='{1}' alt='{1}' />",
                    WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID),
                    ProjectResource.HiddenProject);
            return string.Empty;
        }

        #endregion
    }

    public class WrapperListProjectView
    {
        public object project_id;
        public object project_title;
        public object project_leader_id;
        public object project_status;
        public object milestones_count;
        public object tasks_count;
        public object participants_count;
    }
}