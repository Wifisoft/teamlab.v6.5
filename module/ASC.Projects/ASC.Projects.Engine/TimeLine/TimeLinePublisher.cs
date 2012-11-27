using System;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using ASC.Collections;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Users.Activity;

namespace ASC.Projects.Engine
{
    public class TimeLinePublisher : BaseUserActivityPublisher
    {
        private const String AdditionalDataPattern = "{0}|{1}|{2}";
        private const String SecurityDataPattern = "{0}|{1}|{2}";

        private sealed class TimeLineUserActivity : UserActivity
        {
            int currentTenant = CallContext.GetData("CURRENT_TENANT_ID") != null ? (int)CallContext.GetData("CURRENT_TENANT_ID") : CoreContext.TenantManager.GetCurrentTenant().TenantId;

            public TimeLineUserActivity(String actionText, int actionType, int businessValue)
            {
                ProductID = ConfigurationManager.ProductID;
                ModuleID = ConfigurationManager.ProductID;
                Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
                UserID = SecurityContext.CurrentAccount.ID;
                ActionText = actionText;
                ActionType = actionType;
                BusinessValue = businessValue;
                TenantID = currentTenant;
            }
        }

        public static void Milestone(Milestone milestone, String actionText, int actionType, int businessValue)
        {
            //DropProjectActivitiesCache(milestone.Project);

            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = String.Empty,
                ContainerID = milestone.Project.ID.ToString(),
                Title = milestone.Title,
                URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "milestones.aspx"), String.Format("?prjID={0}&id={1}", milestone.Project.ID, milestone.ID)),
                AdditionalData = String.Format(AdditionalDataPattern, EntityType.Milestone, String.Empty, milestone.Project.Title),
                SecurityId = string.Format(SecurityDataPattern, EntityType.Milestone, milestone.ID, milestone.Project.ID)
            });
        }

        public static void TimeSpend(TimeSpend timeSpend, Project project, Task relativeTask, String actionText, int actionType, int businessValue)
        {
            UserActivityPublisher.Publish<TimeLinePublisher>(
             new TimeLineUserActivity(actionText, actionType, businessValue)
             {
                 ContentID = relativeTask != null ? relativeTask.ToString() : String.Empty,
                 ContainerID = timeSpend.Task.Project.ID.ToString(),
                 Title = relativeTask != null ? relativeTask.Title : timeSpend.Hours.ToString(),
                 URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "timeTracking.aspx"), String.Format("?prjID={0}", timeSpend.Task.Project.ID)),
                 AdditionalData = String.Format(AdditionalDataPattern, EntityType.TimeSpend, relativeTask != null ? relativeTask.Title : string.Empty, project.Title),
                 SecurityId = string.Format(SecurityDataPattern, EntityType.TimeSpend, relativeTask != null ? relativeTask.ID.ToString() : string.Empty, project.ID)
             });
        }

        public static void Project(Project project, String actionText, int actionType, int businessValue)
        {
            DropProjectActivitiesCache(project);
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = String.Empty,
                ContainerID = project.ID.ToString(),
                Title = project.Title,
                URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "projects.aspx"), String.Format("?prjID={0}", project.ID)),
                AdditionalData = String.Format(AdditionalDataPattern, EntityType.Project, String.Empty, project.Title),
                SecurityId = string.Format(SecurityDataPattern, EntityType.Project, string.Empty, project.ID)
            });
        }

        public static void Task(Task task, Milestone milestone, String actionText, int actionType, int businessValue)
        {
            Task(task, milestone, actionText, actionType, businessValue, false);
        }

        public static void Task(Task task, Milestone milestone, String actionText, int actionType, int businessValue, bool withPreview)
        {
            //DropProjectActivitiesCache(task.Project);

            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {

                ContentID = (milestone != null) ? milestone.ToString() : String.Empty,
                ContainerID = task.Project.ID.ToString(),
                Title = task.Title,
                URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "tasks.aspx").Replace("api/", ""), String.Format("?prjID={0}&id={1}", task.Project.ID, task.ID)),
                AdditionalData = String.Format(AdditionalDataPattern, EntityType.Task, (milestone != null) ? milestone.Title : String.Empty, task.Project.Title),
                SecurityId = string.Format(SecurityDataPattern, EntityType.Task, task.ID, task.Project.ID),
                HtmlPreview = withPreview ? task.Description.HtmlEncode() : null
            });
        }

        public static void Subtask(Subtask subtask, Task task, String actionText, int actionType, int businessValue)
        {
            //DropProjectActivitiesCache(task.Project);
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {

                ContentID = (task != null) ? task.ToString() : String.Empty,
                ContainerID = task.Project.ID.ToString(),
                Title = subtask.Title,
                URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "tasks.aspx").Replace("api/", ""), String.Format("?prjID={0}&id={1}", task.Project.ID, task.ID)),
                AdditionalData = String.Format(AdditionalDataPattern, EntityType.SubTask, (task != null) ? task.Title : String.Empty, task.Project.Title),
                SecurityId = string.Format(SecurityDataPattern, EntityType.SubTask, task.ID, task.Project.ID),
                HtmlPreview = null
            });
        }

        public static void Team(Project project, UserInfo userInfo, String actionText)
        {
            DropProjectActivitiesCache(project);

            var userProfileLink = VirtualPathUtility.ToAbsolute("~/userprofile.aspx") + String.Format("?uid={0}&pid={1}", userInfo.ID, ConfigurationManager.ProductID);

            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, UserActivityConstants.ActivityActionType, UserActivityConstants.ImportantActivity)
            {
                ContentID = String.Empty,
                ContainerID = project.ID.ToString(),
                Title = userInfo.DisplayUserName(false),
                URL = userProfileLink,
                AdditionalData = String.Format(AdditionalDataPattern, EntityType.Team, String.Empty, project.Title),
                SecurityId = string.Format(SecurityDataPattern, EntityType.Team, string.Empty, project.ID),
            });
        }

        public static void Message(Message message, String actionText, int actionType, int businessValue)
        {
            Message(message, actionText, actionType, businessValue, false);
        }

        public static void Message(Message message, String actionText, int actionType, int businessValue, bool withPreview)
        {
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = String.Empty,
                ContainerID = message.Project.ID.ToString(),
                Title = message.Title,
                URL = String.Format(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "messages.aspx") + "?prjID={0}&id={1}", message.Project.ID, message.ID),
                AdditionalData = String.Format(AdditionalDataPattern, EntityType.Message, String.Empty, message.Project.Title),
                SecurityId = string.Format(SecurityDataPattern, EntityType.Message, message.ID, message.Project.ID),
                HtmlPreview = withPreview ? message.Content : null
            });
        }

        public static void Comment(Object entity, String actionText)
        {
            if (entity is Task)
            {
                var task = entity as Task;
                UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, UserActivityConstants.ActivityActionType, UserActivityConstants.SmallActivity)
                {
                    ContentID = task.ID.ToString(),
                    ContainerID = task.Project.ID.ToString(),
                    Title = task.Title,
                    URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "tasks.aspx"), String.Format("?prjID={0}&id={1}", task.Project.ID, task.ID)),
                    AdditionalData = String.Format(AdditionalDataPattern, EntityType.Comment, task.Title, task.Project.Title),
                    SecurityId = string.Format(SecurityDataPattern, EntityType.Task, task.ID, task.Project.ID),
                });
            }

            if (entity is Milestone)
            {
                var milestone = entity as Milestone;
                UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, UserActivityConstants.ActivityActionType, UserActivityConstants.SmallActivity)
                {
                    ContentID = milestone.ID.ToString(),
                    ContainerID = milestone.Project.ID.ToString(),
                    Title = milestone.Title,
                    URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "milestones.aspx"), String.Format("?prjID={0}&id={1}", milestone.Project.ID, milestone.ID)),
                    AdditionalData = String.Format(AdditionalDataPattern, EntityType.Comment, milestone.Title, milestone.Project.Title),
                    SecurityId = string.Format(SecurityDataPattern, EntityType.Milestone, milestone.ID, milestone.Project.ID),
                });
            }

            if (entity is Message)
            {
                var message = entity as Message;
                UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, UserActivityConstants.ActivityActionType, UserActivityConstants.SmallActivity)
                {
                    ContentID = message.ID.ToString(),
                    ContainerID = message.Project.ID.ToString(),
                    Title = message.Title,
                    URL = String.Concat(VirtualPathUtility.ToAbsolute(ConfigurationManager.BaseVirtualPath + "messages.aspx"), String.Format("?prjID={0}&id={1}", message.Project.ID, message.ID)),
                    AdditionalData = String.Format(AdditionalDataPattern, EntityType.Comment, message.Title, message.Project.Title),
                    SecurityId = string.Format(SecurityDataPattern, EntityType.Message, message.ID, message.Project.ID),
                });
            }
        }

        private static readonly CachedDictionary<bool> ActivitySecurityCache =
            new CachedDictionary<bool>("activity-security-cache",
                Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(10),
                (x) => true);

        private static void DropProjectActivitiesCache(Project project)
        {
            ActivitySecurityCache.Clear(project.ID.ToString());
        }

        public static bool IsAllowedToView(UserActivity activity, EngineFactory engineFactory)
        {
            if (activity != null)
            {
                activity.SecurityFiltered = true;//Set to true to miss later checks
                //Root key is project id. will be used to drop cache
                return ActivitySecurityCache.Get(activity.ContainerID, SecurityContext.CurrentAccount.ID + activity.ID.ToString(), () => CheckAccess(activity, engineFactory));
            }
            //If empty - then don't care
            return true;
        }

        private static bool CheckAccess(UserActivity activity, EngineFactory engineFactory)
        {
            if (!string.IsNullOrEmpty(activity.SecurityId))
            {
                var data = activity.SecurityId.Split('|');
                if (data.Length == 3)
                {
                    try
                    {
                        var entityType = (EntityType)Enum.Parse(typeof(EntityType), data[0], true);
                        var entityId = string.IsNullOrEmpty(data[1]) ? -1 : int.Parse(data[1]);
                        var projectId = 0;
                        if (!int.TryParse(data[2], out projectId))
                        {
                            return false;
                        }
                        var project = engineFactory.GetProjectEngine().GetByID(projectId);
                        if (project.Private)
                        {
                            //Switch types
                            switch (entityType)
                            {
                                case EntityType.Team:
                                case EntityType.Project:
                                    return ProjectSecurity.CanRead(project);
                                case EntityType.Milestone:
                                    return ProjectSecurity.CanRead(engineFactory.GetMilestoneEngine().GetByID(entityId));
                                case EntityType.Task:
                                    return ProjectSecurity.CanRead(engineFactory.GetTaskEngine().GetByID(entityId));
                                case EntityType.Message:
                                    return ProjectSecurity.CanReadMessages(project);
                                case EntityType.File:
                                    return ProjectSecurity.CanReadFiles(project);
                                case EntityType.TimeSpend:
                                    {
                                        if (entityId < 0)
                                        {
                                            return ProjectSecurity.CanRead(project);
                                        }
                                        return ProjectSecurity.CanRead(engineFactory.GetTaskEngine().GetByID(entityId));
                                    }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(activity.ContainerID))
            {
                //Go long way. Parse old data
                int prjId;
                if (int.TryParse(activity.ContainerID, out prjId))
                {
                    var prj = engineFactory.GetProjectEngine().GetByID(prjId);
                    if (prj != null)
                    {
                        if (prj.Private)
                        {
                            try
                            {
                                return !string.IsNullOrEmpty(activity.AdditionalData) &&
                                       CheckPermission(prj, activity, engineFactory);
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static readonly Regex GetIdParam = new Regex(string.Format(@"(&|\?|\#){{1}}{0}=(?'value'[0-9]+)", Regex.Escape("id")), RegexOptions.Compiled);


        private static bool CheckPermission(Project project, UserActivity ua, EngineFactory engineFactory)
        {
            var additionalDataParts = ua.AdditionalData.Split('|');
            if (additionalDataParts.Length == 0) return false;

            var entityType = (EntityType)Enum.Parse(typeof(EntityType), additionalDataParts[0], true);

            if (entityType == EntityType.Message)
            {
                return ProjectSecurity.CanReadMessages(project);
            }
            if (entityType == EntityType.Task || entityType == EntityType.TimeSpend)
            {
                if (ProjectSecurity.CanReadTasks(project))
                {
                    return true;
                }
                if (entityType == EntityType.Task)
                {

                    var match = GetIdParam.Match(ua.URL);
                    int taskId;
                    if (match.Success && match.Groups["value"].Success && int.TryParse(match.Groups["value"].Value, out taskId))
                    {
                        var task = engineFactory.GetTaskEngine().GetByID(taskId);
                        if (ProjectSecurity.CanRead(task))
                            return true;
                    }
                    return false;
                }
                return SecurityContext.CurrentAccount.ID == ua.UserID;
            }
            if (entityType == EntityType.Milestone)
            {
                var match = GetIdParam.Match(ua.URL);
                int milestoneId;
                if (match.Success && match.Groups["value"].Success && int.TryParse(match.Groups["value"].Value, out milestoneId))
                {
                    var milestone = engineFactory.GetMilestoneEngine().GetByID(milestoneId);
                    return ProjectSecurity.CanRead(milestone);
                }
                return false;
            }
            return true;
        }

        public static void TeamSecurity(Project project, Participant participant)
        {
            DropProjectActivitiesCache(project);
        }
    }
}
