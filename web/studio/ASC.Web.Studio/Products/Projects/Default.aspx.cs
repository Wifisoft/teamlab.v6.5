#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls.Dashboard;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Dashboard;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    public partial class Dashboard : BasePage
    {
        private WidgetTab widgetTabControl;

        private static Guid CurrentUser
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        private static bool CanCreateProject 
        {
            get { return ProjectSecurity.CanCreateProject(); }
        }

        private static bool HasCurrentUserAnyProjects
        {
            get { return RequestContext.HasCurrentUserAnyProjects(); }
        }

        private static MilestoneEngine MilestoneEngine
        {
            get { return Global.EngineFactory.GetMilestoneEngine(); }
        }

        private static ParticipantEngine ParticipantEngine
        {
            get { return Global.EngineFactory.GetParticipantEngine(); }
        }

        private static MessageEngine DiscussionEngine
        {
            get { return Global.EngineFactory.GetMessageEngine(); }
        }

        private static CommentEngine CommentEngine
        {
            get { return Global.EngineFactory.GetCommentEngine(); }
        }

        protected override void PageLoad()
        {
            (Master).DisabledSidePanel = true;

            Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.Dashboard, Master.BreadCrumbs);
            
            AjaxPro.Utility.RegisterTypeForAjax(typeof(WidgetTab), Page);

            if (!RequestContext.HasAnyProjects())
            {
                RenderDashboardEmptyScreen();
                return;
            }

            if (HasCurrentUserAnyProjects)
            {
                RenderWidgetPanel();
            }
            else
            {
                RenderGreetingPanel();
            }
        }

        protected void RenderDashboardEmptyScreen()
        {

            var dashboardEmptyScreen = (ASC.Web.Projects.Controls.Dashboard.DashboardEmptyScreen)Page.LoadControl(PathProvider.GetControlVirtualPath("DashboardEmptyScreen.ascx"));

            contentPlaceHolder.Controls.Add(dashboardEmptyScreen);
        }

        private void RenderWidgetPanel()
        {
            if (!SecurityContext.IsAuthenticated) return;

            RenderNavigationPanel();

            widgetTabControl = new WidgetTab(new Guid("{1ac13227-6ae4-4777-89c6-d921b6ff0e8f}"), ColumnSchemaType.Schema_65_35, "projectsDashboard")
            {
                Settings = true
            };

            RenderProjectsWidget();

            RenderMilestonesWidget();

            RenderDiscussionsWidget();

            widgetPanelPlaceHolder.Controls.Add(widgetTabControl);
            
            Master.ContentHolder.Visible = false;
        }

        private void RenderNavigationPanel()
        {
            var navigationPanel = (NavigationPanel)LoadControl(NavigationPanel.Location);
            if (CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
            {
                navigationPanel.addButton(SettingsResource.Settings,
                                          WebImageSupplier.GetAbsoluteWebPath("32-Projects-import.png", ProductEntryPoint.ID),
                                          "settings.aspx", 2);
            }
            navigationPanel.addButton(ProjectsCommonResource.CustomizeWidgets,
                                      WebImageSupplier.GetAbsoluteWebPath("btn_managewidgets.png"),
                                      "javascript:projectsDashboard.ShowSettings('400px', '-200px')", 4);

            navigationPanelPlaceHolder.Controls.Add(navigationPanel);
        }

        private void RenderProjectsWidget()
        {
            var projectsWidget = (ProjectsWidget)LoadControl(PathProvider.GetControlVirtualPath("ProjectsWidget.ascx"));

            widgetTabControl.WidgetCollection.Add(new Widget(ProjectsWidget.WidgetId,
                                                             projectsWidget,
                                                             ProjectResource.ProjectsAndTasks,
                                                             string.Empty)
                                                      {
                                                          ImageURL = WebImageSupplier.GetAbsoluteWebPath("my_projects.png", ProductEntryPoint.ID),
                                                          SettingsProviderType = typeof(ProjectsWidgetSettingsProvider),
                                                          Position = new Point(0, 0)
                                                      });

        }

        private void RenderMilestonesWidget()
        {
            var milestones = GetMilestones();

            var milestonesWidget = (MilestonesWidget)LoadControl(PathProvider.GetControlVirtualPath("MilestonesWidget.ascx"));
            milestonesWidget.Milestones = milestones;

            var widgetTab = new Widget(MilestonesWidget.WidgetId, milestonesWidget, MilestoneResource.Milestones, string.Empty)
                                {
                                    ImageURL =WebImageSupplier.GetAbsoluteWebPath("my_milestones.png", ProductEntryPoint.ID),
                                    SettingsProviderType = typeof (ProjectsWidgetSettingsProvider),
                                    Position = new Point(1, 0)
                                };

            if (milestones.Count <= 0) widgetTab.Visible = false;
            widgetTabControl.WidgetCollection.Add(widgetTab);
        }

        private void RenderDiscussionsWidget()
        {
            var discussions = GetDiscussions();

            var discussionsWidget = (DiscussionsWidget)LoadControl(PathProvider.GetControlVirtualPath("DiscussionsWidget.ascx"));
            discussionsWidget.Discussions = discussions;

            var widgetTab = new Widget(DiscussionsWidget.WidgetId, discussionsWidget, MessageResource.Messages, string.Empty)
            {
                ImageURL = WebImageSupplier.GetAbsoluteWebPath("discussions.png", ProductEntryPoint.ID),
                SettingsProviderType = typeof (ProjectsWidgetSettingsProvider),
                Position = new Point(1, 1)
            };

            if (discussions.Count <= 0) widgetTab.Visible = false;
            widgetTabControl.WidgetCollection.Add(widgetTab);
        }

        private void RenderGreetingPanel()
        {
            var emptyDashboard = (EmptyDashboard)LoadControl(PathProvider.GetControlVirtualPath("EmptyWidget.ascx"));
            contentPlaceHolder.Controls.Add(emptyDashboard);
        }

        private static List<Milestone> GetMilestones()
        {
            return MilestoneEngine.GetByFilter(new TaskFilter
            {
                MyProjects = true,
                MilestoneStatuses = new List<MilestoneStatus>{MilestoneStatus.Open},
                SortBy = "deadline",
                SortOrder = true
            });
        }

        private static List<DiscussionWrapper> GetDiscussions()
        {
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<DiscussionsWidgetSettings>(CurrentUser);

            var filter = new TaskFilter
            {
                MyProjects = true,
                SortBy = "create_on",
                SortOrder = false,
                Max = widgetSettings.DiscussionsCount
            };

            var discussions = DiscussionEngine.GetByFilter(filter);

            var wrappedDiscussions = new List<DiscussionWrapper>();
            if (discussions.Count > 0)
            {
                var isReaded = ParticipantEngine.IsReaded(CurrentUser, discussions.ConvertAll(d => (ProjectEntity)d));

                wrappedDiscussions.AddRange(discussions.Select((t, i) => new DiscussionWrapper(t)
                {
                    IsReaded = isReaded[i]
                }));
            }
            return wrappedDiscussions;
        }
    }
}
