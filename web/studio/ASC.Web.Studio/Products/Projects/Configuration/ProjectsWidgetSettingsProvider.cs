#region Usings

using System;
using System.Collections.Generic;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Projects.Controls.Dashboard;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Dashboard.Settings;

#endregion

namespace ASC.Web.Projects.Configuration
{
    public class ProjectsWidgetSettingsProvider : IWidgetSettingsProvider
    {
        public bool Check(List<WidgetSettings> settings, Guid widgetId, Guid userId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (widgetId.Equals(ProjectsWidget.WidgetId))
            {
                if (settings == null || settings.Count != 5) return false;

                var myProjectsCount = settings[0].ConvertToNumber();
                var tasksCount = settings[1].ConvertToNumber();
                var followingProjectsCount = settings[4].ConvertToNumber();

                if (myProjectsCount.Value <= 0)
                {
                    errorMessage = ProjectsCommonResource.ProjectsWidget_MyProjectsCount_ErrorMessage;
                }
                if (followingProjectsCount.Value <= 0)
                {
                    errorMessage = ProjectsCommonResource.ProjectsWidget_FollowingProjectsCount_ErrorMessage;
                }
                if (tasksCount.Value <= 0)
                {
                    errorMessage = ProjectsCommonResource.ProjectsWidget_TasksCount_ErrorMessage;
                }

                return string.IsNullOrEmpty(errorMessage);
            }

            if (widgetId.Equals(MilestonesWidget.WidgetId))
            {
                if (settings == null || settings.Count != 1) return false;

                var milestonesCount = settings[0].ConvertToNumber();

                if (milestonesCount.Value <= 0)
                {
                    errorMessage = ProjectsCommonResource.MilestonesWidget_MilestonesCount_ErrorMessage;
                    return false;
                }

                return true;
            }

            if (widgetId.Equals(DiscussionsWidget.WidgetId))
            {
                if (settings == null || settings.Count != 1) return false;

                var messagesCount = settings[0].ConvertToNumber();
                if (messagesCount.Value <= 0)
                {
                    errorMessage = ProjectsCommonResource.MessagesWidget_ErrorMessage;
                    return false;
                }

                return true;
            }

            return false;
        }

        public void Save(List<WidgetSettings> settings, Guid widgetId, Guid userId)
        {
            if (widgetId.Equals(ProjectsWidget.WidgetId))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ProjectsWidgetSettings>(userId);

                var myProjectsCount = settings[0].ConvertToNumber();
                var tasksCount = settings[1].ConvertToNumber();
                var showOnlyMyTasks = settings[2].ConvertToBool();
                var showFollowingProjects = settings[3].ConvertToBool();
                var followingProjectsCount = settings[4].ConvertToNumber();

                widgetSettings.MyProjectsCount = myProjectsCount.Value;
                widgetSettings.ShowFollowingProjects = showFollowingProjects.Value;
                widgetSettings.FollowingProjectsCount = followingProjectsCount.Value;
                widgetSettings.TasksCount = tasksCount.Value;
                widgetSettings.ShowOnlyMyTasks = showOnlyMyTasks.Value;

                SettingsManager.Instance.SaveSettingsFor(widgetSettings, userId);
            }

            if (widgetId.Equals(MilestonesWidget.WidgetId))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<MilestonesWidgetSettings>(userId);

                var milestonesCount = settings[0].ConvertToNumber();

                widgetSettings.MilestonesCount = milestonesCount.Value;

                SettingsManager.Instance.SaveSettingsFor(widgetSettings, userId);
            }

            if (widgetId.Equals(DiscussionsWidget.WidgetId))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<DiscussionsWidgetSettings>(userId);

                var messagesCount = settings[0].ConvertToNumber();

                widgetSettings.DiscussionsCount = messagesCount.Value;

                SettingsManager.Instance.SaveSettingsFor(widgetSettings, userId);
            }
        }

        public List<WidgetSettings> Load(Guid widgetID, Guid userID)
        {
            var settings = new List<WidgetSettings>();

            if (widgetID.Equals(ProjectsWidget.WidgetId))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ProjectsWidgetSettings>(userID);
                settings.Add(new NumberWidgetSettings
                {
                    Title = ProjectsCommonResource.ProjectsWidget_PersonalProjectsCount_Title,
                    Value = widgetSettings.MyProjectsCount,
                    Description = ""
                });
                settings.Add(new NumberWidgetSettings
                {
                    Title = ProjectsCommonResource.ProjectsWidget_TasksCount_Title,
                    Value = widgetSettings.TasksCount,
                    Description = ""
                });
                settings.Add(new BoolWidgetSettings
                {
                    Title = ProjectsCommonResource.ProjectsWidget_ShowOnlyMyTasks_Title,
                    Value = widgetSettings.ShowOnlyMyTasks,
                    Description = ""
                });
                settings.Add(new BoolWidgetSettings
                {
                    Title = ProjectsCommonResource.ProjectsWidget_ShowFollowingProjects_Title,
                    Value = widgetSettings.ShowFollowingProjects,
                    Description = ""
                });
                settings.Add(new NumberWidgetSettings
                {
                    Title = ProjectsCommonResource.ProjectsWidget_FollowingProjectsCount_Title,
                    Value = widgetSettings.FollowingProjectsCount,
                    Description = ""
                });
            }

            if (widgetID.Equals(MilestonesWidget.WidgetId))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<MilestonesWidgetSettings>(userID);
                settings.Add(new NumberWidgetSettings
                {
                    Title = ProjectsCommonResource.MilestonesWidget_MilestonesCount_Title,
                    Value = widgetSettings.MilestonesCount,
                    Description = ""
                });
            }

            if (widgetID.Equals(DiscussionsWidget.WidgetId))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<DiscussionsWidgetSettings>(userID);
                settings.Add(new NumberWidgetSettings
                {
                    Title = ProjectsCommonResource.DiscussionsWidget_Title,
                    Value = widgetSettings.DiscussionsCount,
                    Description = ""
                });
            }

            return settings;
        }
    }
}
