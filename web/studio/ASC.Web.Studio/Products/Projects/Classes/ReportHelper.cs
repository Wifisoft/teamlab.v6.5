using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Cron;
using ASC.Notify.Messages;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Users;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Projects.Classes
{
    static class ReportHelper
    {
        public static ReportInfo GetReportInfo(ReportType reportType)
        {
            var virtualRoot = CommonLinkUtility.VirtualRoot != "/" ? CommonLinkUtility.VirtualRoot : string.Empty;
            var virtulaRootPath = CommonLinkUtility.ServerRootPath + virtualRoot;

            var mileColumns = new[] {
                ProjectResource.Project,
                MilestoneResource.Milestone,
                ReportResource.DeadLine,
                virtulaRootPath };

            var projColumns = new[] {
                ProjectsCommonResource.Title,
                ProjectResource.ProjectLeader,
                ProjectsCommonResource.Status, 
                GrammaticalResource.MilestoneGenitivePlural,
                TaskResource.Tasks,
                ReportResource.Participiants,
                ReportResource.ClickToSortByThisColumn,
                CommonLinkUtility.ServerRootPath,
                virtualRoot };

            var userColumns = new[] {
                ReportResource.User,
                "Not Accept",                
                ResourceEnumConverter.ConvertToString(TaskStatus.Open),
                ReportResource.ActiveTasks, 
                ResourceEnumConverter.ConvertToString(TaskStatus.Closed),
                ProjectsCommonResource.Total,
                ReportResource.ClickToSortByThisColumn,
                CommonLinkUtility.ServerRootPath };

            var taskColumns = new[] {
                ProjectResource.Project,
                MilestoneResource.Milestone,
                TaskResource.Task, 
                TaskResource.TaskResponsible,
                ProjectsCommonResource.Status,
                TaskResource.UnsortedTask,
                ReportResource.DeadLine,
                ReportResource.NoMilestonesAndTasks,
                CommonLinkUtility.ServerRootPath,
                virtualRoot };

            var taskExpiredColumns = new[] {
                ProjectResource.Project,
                MilestoneResource.Milestone,
                TaskResource.Task, 
                TaskResource.TaskResponsible,
                ProjectsCommonResource.Status,
                TaskResource.UnsortedTask,
                TaskResource.DeadLine,
                ReportResource.NoMilestonesAndTasks,
                CommonLinkUtility.ServerRootPath,
                virtualRoot };

            switch (reportType)
            {
                case ReportType.MilestonesExpired:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportLateMilestones_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportLateMilestones_Title,
                        mileColumns);

                case ReportType.MilestonesNearest:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportUpcomingMilestones_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportUpcomingMilestones_Title,
                        mileColumns);

                case ReportType.UsersWithoutActiveTasks:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportEmployeesWithoutActiveTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        CustomNamingPeople.Substitute<ReportResource>("ReportEmployeesWithoutActiveTasks_Title").HtmlEncode(),
                        userColumns);

                case ReportType.ProjectsWithoutActiveMilestones:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportProjectsWithoutActiveMilestones_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportProjectsWithoutActiveMilestones_Title,
                        projColumns);

                case ReportType.ProjectsWithoutActiveTasks:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportProjectsWithoutActiveTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportProjectsWithoutActiveTasks_Title,
                        projColumns);

                case ReportType.UsersActivity:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportUserActivity_Descripton, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportUserActivity_Title,
                        new[] { ReportResource.User, TaskResource.Tasks, MilestoneResource.Milestones, MessageResource.Messages, ProjectsFileResource.Files, ProjectsCommonResource.Total, ReportResource.ClickToSortByThisColumn, CommonLinkUtility.ServerRootPath });

                case ReportType.UsersWorkload:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportEmployment_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportEmployment_Title,
                        userColumns);

                case ReportType.ProjectsList:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportProjectList_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportProjectList_Title,
                        projColumns);

                case ReportType.TimeSpend:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportTimeSpendSummary_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportTimeSpendSummary_Title,
                        new[] { ReportResource.User, ProjectsCommonResource.SpentTotally, ProjectsCommonResource.Title, ReportResource.ClickToSortByThisColumn, CommonLinkUtility.ServerRootPath, virtualRoot });

                case ReportType.TasksByProjects:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportTaskList_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportTaskList_Title,
                        taskColumns);

                case ReportType.TasksByUsers:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportUserTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportUserTasks_Title,
                        taskColumns);

                case ReportType.TasksExpired:
                    return new ReportInfo(
                        String.Format(ReportResource.ReportLateTasks_Description, "<ul>", "</ul>", "<li>", "</li>"),
                        ReportResource.ReportLateTasks_Title,
                        taskExpiredColumns);
            };
            return ReportInfo.Empty;
        }

        public static string GetReportFileName(ReportType type)
        {
            var culture = CultureInfo.InvariantCulture;
            switch (type)
            {
                case ReportType.MilestonesExpired: return ReportResource.ResourceManager.GetString("ReportLateMilestones_Title", culture);
                case ReportType.MilestonesNearest: return ReportResource.ResourceManager.GetString("ReportUpcomingMilestones_Title", culture);
                case ReportType.UsersWithoutActiveTasks: return CustomNamingPeople.Substitute<ReportResource>("ReportEmployeesWithoutActiveTasks_Title").HtmlEncode();
                case ReportType.ProjectsWithoutActiveMilestones: return ReportResource.ResourceManager.GetString("ReportProjectsWithoutActiveMilestones_Title", culture);
                case ReportType.ProjectsWithoutActiveTasks: return ReportResource.ResourceManager.GetString("ReportProjectsWithoutActiveTasks_Title", culture);
                case ReportType.UsersActivity: return ReportResource.ResourceManager.GetString("ReportUserActivity_Title", culture);
                case ReportType.UsersWorkload: return ReportResource.ReportEmployment_Title;
                case ReportType.ProjectsList: return ReportResource.ResourceManager.GetString("ReportProjectList_Title", culture);
                case ReportType.TimeSpend: return ReportResource.ResourceManager.GetString("ReportTimeSpendSummary_Title", culture);
                case ReportType.TasksByProjects: return ReportResource.ResourceManager.GetString("ReportTaskList_Title", culture);
                case ReportType.TasksByUsers: return ReportResource.ResourceManager.GetString("ReportUserTasks_Title", culture);
                case ReportType.TasksExpired: return ReportResource.ResourceManager.GetString("ReportLateTasks_Title", culture);
                default: return "report";
            };
        }


        public static string BuildReport(ReportType reportType, ReportFilter filter, ReportViewType viewType)
        {
            //prepare filter
            if (reportType == ReportType.MilestonesExpired)
            {
                filter.ToDate = TenantUtil.DateTimeNow();
            }
            if (reportType == ReportType.TasksExpired)
            {
                filter.ToDate = TenantUtil.DateTimeNow();
                filter.TaskStatuses.Add(TaskStatus.Open);
            }

            //exec
            IList<object[]> result = null;

            if (reportType == ReportType.UsersActivity)
            {
                result = BuildUserActivityReport(filter);
            }
            else
            {
                result = Global.EngineFactory.GetReportEngine().BuildReport(reportType, filter);
                if (reportType == ReportType.TasksExpired)
                {
                    var tmp = new List<object[]>();
                    foreach (var row in result)
                        if (row[10] != null)//task has due date
                            tmp.Add(row);
                    result = tmp;
                }
            }

            if (result == null || result.Count == 0)
            {
                return null;
            }

            //add user info
            switch (reportType)
            {
                case ReportType.UsersWithoutActiveTasks:
                case ReportType.UsersWorkload:
                case ReportType.TimeSpend:
                case ReportType.UsersActivity:
                    result = AddUserInfo(result, 0);
                    result = result
                        .OrderBy(r => CoreContext.UserManager.GetUsers((Guid)r[0]), UserInfoComparer.Default)
                        .ToList();
                    break;

                case ReportType.ProjectsWithoutActiveMilestones:
                case ReportType.ProjectsWithoutActiveTasks:
                case ReportType.ProjectsList:
                    result = AddUserInfo(result, 2);
                    result = result
                        .OrderBy(r => (string)r[1])
                        .ToList();
                    break;

                case ReportType.TasksByProjects:
                case ReportType.TasksByUsers:
                case ReportType.TasksExpired:
                    result = AddUserInfo(result, 8);
                    result = AddStatusCssClass(result);
                    break;
            }

            return ReportTransformer.Transform(result, reportType, filter.ViewType, viewType);
        }


        private static IList<object[]> BuildUserActivityReport(ReportFilter filter)
        {
            var result = new List<object[]>();
            var users = new List<UserInfo>();
            if (filter.UserId != Guid.Empty)
            {
                users.Add(CoreContext.UserManager.GetUsers(filter.UserId));
            }
            else if (filter.DepartmentId != Guid.Empty)
            {
                users.AddRange(CoreContext.UserManager.GetUsersByGroup(filter.DepartmentId));
            }
            else
            {
                users.AddRange(CoreContext.UserManager.GetUsers());
            }

            foreach (var u in users.OrderBy(u => u, UserInfoComparer.Default))
            {
                var tasks = 0;
                var milestones = 0;
                var discussions = 0;
                var files = 0;
                var pid = ProductEntryPoint.ID;
                var fromDate = filter.GetFromDate(true);
                var toDate = filter.GetToDate(true);

                UserActivityManager.GetUserActivities(
                    TenantProvider.CurrentTenantID, u.ID, pid, new[] { pid }, UserActivityConstants.AllActionType, null, fromDate, toDate)
                    .ForEach(a =>
                    {
                        var data = a.AdditionalData.Split(new[] { '|' })[0];
                        if (a.ActionType == UserActivityConstants.ContentActionType && data == EntityType.Task.ToString()) tasks++;
                        if (a.ActionType == UserActivityConstants.ContentActionType && data == EntityType.Milestone.ToString()) milestones++;
                        if (a.ActionType == UserActivityConstants.ContentActionType && data == EntityType.Message.ToString()) discussions++;
                        if (a.ActionType == UserActivityConstants.ActivityActionType && data == EntityType.File.ToString()) files++;
                    });

                result.Add(new object[] { u.ID, tasks, milestones, discussions, files, tasks + milestones + discussions + files });
            }

            return result;
        }

        public static void SendAutoReports(DateTime datetime)
        {
            try
            {
                var now = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);
                foreach (var t in new DaoFactory(Global.DB_ID, -1).GetReportDao().GetAutoTemplates())
                {
                    try
                    {
                        var tenant = CoreContext.TenantManager.GetTenant(t.Tenant);
                        if (tenant != null && tenant.Status == TenantStatus.Active)
                        {
                            CoreContext.TenantManager.SetCurrentTenant(tenant);
                            var cron = new CronExpression(t.Cron) { TimeZone = CoreContext.TenantManager.GetCurrentTenant().TimeZone };
                            var date = cron.GetTimeAfter(now.AddTicks(-1));

                            log4net.LogManager.GetLogger("ASC.Web.Projects.Reports").DebugFormat("Find auto report: {0} - {1}, now: {2}, date: {3}", t.Name, t.Cron, now, date);
                            if (date == now)
                            {
                                var user = CoreContext.UserManager.GetUsers(new Guid(t.CreateBy));
                                if (user.ID != ASC.Core.Users.Constants.LostUser.ID /*&& user.Status*/)
                                {
                                    SecurityContext.AuthenticateMe(user.Email, CoreContext.Authentication.GetUserPasswordHash(user.ID));

                                    var result = BuildReport(t.ReportType, t.Filter, ReportViewType.EMail);
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        var message = new NoticeMessage(user, HtmlUtil.SanitizeFragment(HttpUtility.HtmlDecode(t.Name)), result, "html");
                                        log4net.LogManager.GetLogger("ASC.Web.Projects.Reports").DebugFormat("Send auto report: {0} to {1}, tenant: {2}", t.Name, user, CoreContext.TenantManager.GetCurrentTenant());
                                        WorkContext.NotifyContext.DispatchEngine.Dispatch(message, "email.sender");
                                    }
                                    else
                                    {
                                        SendEmptyAutoReports(t);
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Security.SecurityException se)
                    {
                        LogManager.GetLogger("ASC.Web.Projects.Reports").Info("SendAutoReports", se);
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger("ASC.Web.Projects.Reports").Error("SendAutoReports", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ASC.Web.Projects.Reports").Error("SendAutoReports", ex);
            }
        }

        public static void SendEmptyAutoReports(ReportTemplate template)
        {
            var result = string.Empty;
            var parameters = new XsltArgumentList();
            parameters.AddParam("p0", string.Empty, CommonLinkUtility.GetFullAbsolutePath("~/products/projects/templates.aspx"));
            parameters.AddParam("p1", string.Empty, ReportResource.ChangeSettings);
            var xml = string.Format("<div class='noContentBlock'>{0}</div>", ProjectsCommonResource.NoData);
            var fileName = Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("products\\projects\\templates\\{0}.xsl", ReportViewType.EMail)).ToLower();
            var xslt = new XslCompiledTransform();
            if (File.Exists(fileName))
            {
                xslt.Load(fileName);
                using (var reader = XmlReader.Create(new StringReader(xml)))
                using (var writer = new StringWriter())
                using (var xmlwriter = XmlWriter.Create(writer, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
                {
                    xslt.Transform(reader, parameters, writer);
                    result = writer.ToString();
                }
                var user = CoreContext.UserManager.GetUsers(new Guid(template.CreateBy));
                var message = new NoticeMessage(user, template.Name, result, "html");
                log4net.LogManager.GetLogger("ASC.Web.Projects.Reports").DebugFormat("Send auto report: {0}", template.Name);
                WorkContext.NotifyContext.DispatchEngine.Dispatch(message, "email.sender");
            }
        }

        private static IList<object[]> AddUserInfo(IList<object[]> rows, int userIdIndex)
        {
            var result = new List<object[]>();
            foreach (var row in rows)
            {
                if (row[userIdIndex] != null)
                {
                    var list = new List<object>(row);
                    var userId = (Guid)row[userIdIndex];
                    if (userId != Guid.Empty)
                    {
                        var user = CoreContext.UserManager.GetUsers(userId);
                        if (user.ID != Constants.LostUser.ID && user.Status != EmployeeStatus.Terminated)
                        {
                            list.Add(user.DisplayUserName(false));
                            list.Add(user.GetUserProfilePageURL(ProductEntryPoint.ID));
                            result.Add(list.ToArray());
                        }
                    }
                    else
                    {
                        list.Add(TaskResource.WithoutResponsible);
                        list.Add(string.Empty);
                        result.Add(list.ToArray());
                    }
                }
            }

            return result;
        }

        private static IList<object[]> AddStatusCssClass(IList<object[]> rows)
        {
            var result = new List<object[]>();
            foreach (var row in rows)
            {
                var list = new List<object>(row);
                if ((int)row[5] != -1)
                {
                    var milestoneStatus = (MilestoneStatus)row[5];
                    var milestoneCssClass = string.Empty;
                    if (milestoneStatus == MilestoneStatus.Open)
                    {
                        milestoneCssClass = "pm-blueText";
                        var milestoneDeadline = new DateTime();
                        if (DateTime.TryParse((string)row[4], out milestoneDeadline))
                        {
                            if (milestoneDeadline < TenantUtil.DateTimeNow())
                                milestoneCssClass = "pm-redText";
                        }
                    }
                    if (milestoneStatus == MilestoneStatus.Closed) milestoneCssClass = "pm-greenText";

                    list[5] = ResourceEnumConverter.ConvertToString(milestoneStatus);
                    list.Add(milestoneStatus);
                    list.Add(milestoneCssClass);
                }
                else
                {
                    row[5] = null;
                    list.Add(string.Empty);
                    list.Add(string.Empty);
                }

                var taskStatus = (TaskStatus)row[9];
                var taskCssClass = string.Empty;

                if (taskStatus == TaskStatus.Open)
                {
                    taskCssClass = "pm-blueText";
                    var taskDeadline = new DateTime();
                    if (DateTime.TryParse((string)row[10], out taskDeadline))
                    {
                        if (taskDeadline < TenantUtil.DateTimeNow())
                            taskCssClass = "pm-redText";
                    }
                }
                if (taskStatus == TaskStatus.Closed) taskCssClass = "pm-greenText";


                list[9] = ResourceEnumConverter.ConvertToString(taskStatus);
                list.Add(taskStatus);
                list.Add(taskCssClass);

                result.Add(list.ToArray());
            }
            return result;
        }

        public static string[] GetCsvColumnsName(ReportType reportType)
        {
            var mileColumns = new[] { 
                ReportResource.CsvColumnProjectjTitle,
                ReportResource.CsvColumnMilestoneTitle,
                ReportResource.CsvColumnMilestoneDeadline };

            var projColumns = new[] { 
                ReportResource.CsvColumnProjectjTitle,
                ProjectResource.ProjectLeader,
                ProjectsCommonResource.Status,
                GrammaticalResource.MilestoneGenitivePlural,
                TaskResource.Tasks,
                ReportResource.Participiants };

            var userColumns = new[] { 
                ReportResource.CsvColumnUserName,
                ResourceEnumConverter.ConvertToString(TaskStatus.Open),
                ResourceEnumConverter.ConvertToString(TaskStatus.Closed),
                ProjectsCommonResource.Total };

            var taskColumns = new[] { 
                ReportResource.CsvColumnProjectjTitle,
                ReportResource.CsvColumnMilestoneTitle,
                ReportResource.CsvColumnMilestoneDeadline, 
                ReportResource.CsvColumnMilestoneStatus,
                ReportResource.CsvColumnTaskTitle,
                ReportResource.CsvColumnTaskDueDate,
                ReportResource.CsvColumnTaskStatus,
                ReportResource.CsvColumnUserName,
                TaskResource.UnsortedTask};

            var activityColumns = new[] { 
                ReportResource.CsvColumnUserName,
                TaskResource.Tasks,
                MilestoneResource.Milestones,
                MessageResource.Messages,
                ProjectsFileResource.Files,
                ProjectsCommonResource.Total };

            var timeColumns = new[] { 
                ReportResource.CsvColumnUserName,
                ReportResource.CsvColumnTaskTitle,
                ProjectsCommonResource.SpentTotally };

            switch (reportType)
            {
                case ReportType.MilestonesExpired:
                    return mileColumns;

                case ReportType.MilestonesNearest:
                    return mileColumns;

                case ReportType.UsersWithoutActiveTasks:
                    return userColumns;

                case ReportType.ProjectsWithoutActiveMilestones:
                    return projColumns;

                case ReportType.ProjectsWithoutActiveTasks:
                    return projColumns;

                case ReportType.UsersActivity:
                    return activityColumns;

                case ReportType.UsersWorkload:
                    return userColumns;

                case ReportType.ProjectsList:
                    return projColumns;

                case ReportType.TimeSpend:
                    return timeColumns;

                case ReportType.TasksByProjects:
                    return taskColumns;

                case ReportType.TasksByUsers:
                    return taskColumns;

                case ReportType.TasksExpired:
                    return taskColumns;
            };

            return null;
        }
    }
}