using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Web.Studio.Masters;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Controls;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Reports;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Core.Tenants;
using ASC.Web.Core.Helpers;
using ASC.Web.Studio.Core.Users;
using ASC.Projects.Engine;

namespace ASC.Web.Projects
{

    #region Types

    public struct CronFields
    {
        public int period;
        public int periodItem;
        public int hour;
    }

    #endregion

    [AjaxNamespace("AjaxPro.Reports")]
    public partial class Reports : BasePage
    {
        protected int reportType;

        protected bool IsGenerate
        {
            get { return UrlParameters.ActionType == "generate"; }
        }

        protected bool HasTemplates
        {
            get { return 0 < Global.EngineFactory.GetReportEngine().GetTemplates(SecurityContext.CurrentAccount.ID).Count; }
        }

        protected bool TemplateNotFound { get; set; }

        protected bool HasData { get; set; }

        protected ReportFilter Filter { get; set; }

        protected override void PageLoad()
        {
            ((IStudioMaster) Master).DisabledSidePanel = true;

            AjaxPro.Utility.RegisterTypeForAjax(typeof(Reports));

            TemplateNotFound = false;
            HasData = true;

            InitActionPanel();

            int.TryParse(UrlParameters.ReportType, out reportType);
            var repType = (ReportType)reportType;

            if (IsGenerate)
            {
                Master.DisabledSidePanel = true;

                var filter = new ReportFilter();
                var templateID = 0;
                if (int.TryParse(UrlParameters.EntityID, out templateID))
                {
                    var template = Global.EngineFactory.GetReportEngine().GetTemplate(templateID);
                    if (template != null)
                    {
                        filter = template.Filter;
                        Filter = template.Filter;
                        repType = template.ReportType;
                        reportType = (int)template.ReportType;
                        Master.BreadCrumbs.Add(new BreadCrumb { Caption = string.Format(ReportResource.ReportPageTitle, HttpUtility.HtmlDecode(template.Name)) });
                        Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, HttpUtility.HtmlDecode(template.Name)), Master.BreadCrumbs);
                    }
                    else
                    {
                        TemplateNotFound = true;
                        HasData = false;
                        emptyScreenControlPh.Controls.Add(new ElementNotFoundControl
                        {
                            Header = ReportResource.TemplateNotFound_Header,
                            Body = ReportResource.TemplateNotFound_Body,
                            RedirectURL = "templates.aspx",
                            RedirectTitle = ReportResource.TemplateNotFound_RedirectTitle
                        });

                        Master.DisabledSidePanel = false;
                    }
                }
                else
                {
                    filter = ReportFilter.FromUri(HttpContext.Current.Request.GetUrlRewriter());
                    Master.BreadCrumbs.Add(new BreadCrumb { Caption = string.Format(ReportResource.ReportPageTitle, ReportHelper.GetReportInfo(repType).Title) });
                    Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, ReportHelper.GetReportInfo(repType).Title), Master.BreadCrumbs);
                }

                var outputFormat = ReportViewType.Html;
                switch (HttpContext.Current.Request["format"])
                {
                    case "csv": outputFormat = ReportViewType.Csv; break;
                    case "xml": outputFormat = ReportViewType.Xml; break;
                    case "email": outputFormat = ReportViewType.EMail; break;
                    case "html": outputFormat = ReportViewType.Html; break;
                }

                var result = ReportHelper.BuildReport(repType, filter, outputFormat);

                switch (outputFormat)
                {
                    case ReportViewType.Html:
                        if (result != null)
                        {
                            reportResult.Text = result;
                        }
                        else
                        {
                            HasData = false;
                            emptyScreenControlPh.Controls.Add(new ASC.Web.Projects.Controls.EmptyScreenControl(ProjectsCommonResource.NoData));
                        }
                        reportFilter.Text = ParseFilter(repType, filter);
                        Filter = filter;
                        break;

                    case ReportViewType.Csv:
                    case ReportViewType.Xml:
                    case ReportViewType.EMail:
                        if (result != null)
                        {
                            var ext = outputFormat.ToString().ToLower();
                            Response.Clear();
                            Response.ContentType = "text/" + ext + "; charset=utf-8";
                            Response.ContentEncoding = Encoding.UTF8;
                            Response.Charset = Encoding.UTF8.WebName;
                            Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.{1}", ReportHelper.GetReportFileName(repType).Replace(' ', '_'), ext));
                            Response.Write(result);
                            Response.End();
                        }
                        break;
                }
            }
            else
            {
                Master.BreadCrumbs.Add(new BreadCrumb { Caption = ReportResource.Reports });
                Title = HeaderStringHelper.GetPageTitle(ReportResource.Reports, Master.BreadCrumbs);

                var filters = (ReportFilters)LoadControl(PathProvider.GetControlVirtualPath("ReportFilters.ascx"));
                filters.ReportType = repType;
                filters.Filter = string.IsNullOrEmpty(HttpContext.Current.Request.Url.Query) ? new ReportFilter() : ReportFilter.FromUri(HttpContext.Current.Request.Url);
                reportFiltersPh.Controls.Add(filters);
            } 
        }

        [AjaxMethod]
        public string InitUsersDdlByDepartment(string department)
        {
            ProjectSecurity.DemandAuthentication();

            var departmentID = !string.IsNullOrEmpty(department) && department != "-1" ? new Guid(department) : Guid.Empty;

            if (departmentID == Guid.Empty)
            {
                return GetAllUsersDropDownList();
            }
            else
            {
                var sb = new StringBuilder().AppendFormat("<option value='-1' id='ddlUser-1'>{0}</option>", CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                CoreContext.UserManager
                    .GetUsersByGroup(departmentID)
                    .OrderBy(u => u, UserInfoComparer.Default)
                    .ToList()
                    .ForEach(u => sb.AppendFormat("<option value='{0}' id='ddlUser{0}'>{1}</option>", u.ID, u.DisplayUserName()));
                return sb.ToString();
            }
        }

        [AjaxMethod]
        public string InitUsersDdlByProject(string project)
        {
            ProjectSecurity.DemandAuthentication();

            var projectID = Convert.ToInt32(project);

            if (projectID == -1)
            {
                return GetAllUsersDropDownList();
            }
            else
            {
                var sb = new StringBuilder().AppendFormat("<option value='-1' id='ddlUser-1'>{0}</option>", CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());

                var users = Global.EngineFactory.GetProjectEngine()
                    .GetTeam(projectID)
                    .Select(p => p.UserInfo)
                    .OrderBy(u => u, UserInfoComparer.Default)
                    .ToList();

                foreach (var u in users)
                {
                    if (u.Status != EmployeeStatus.Terminated)
                        sb.AppendFormat("<option value='{0}' id='ddlUser{0}'>{1}</option>", u.ID, u.DisplayUserName());
                }

                return sb.ToString();
            }
        }

        [AjaxMethod]
        public string InitPeriodItemsDdlByPeriod(int period)
        {
            ProjectSecurity.DemandAuthentication();

            var sb = new StringBuilder();

            switch (period)
            {
                case 0:
                    string value = string.Empty;
                    for (int i = 0; i < 24; i++)
                    {
                        if (i < 10)
                            value = string.Format("0{0}:00", i.ToString());
                        else value = string.Format("{0}:00", i.ToString());

                        sb.AppendFormat("<option value='{0:00}' id='ddlPeriodItems{0}' {1}>{2}</option>", i, i == 12 ? "selected='selected'" : string.Empty, value);
                    }
                    break;
                case 1:
                    var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                    //in cron expression week day 1-7 (not 0-6)
                    var firstday = (int)format.FirstDayOfWeek;
                    for (int i = firstday; i < firstday + 7; i++)
                    {
                        sb.AppendFormat("<option value='{0}' id='ddlPeriodItems{0}'>{1}</option>", i % 7 + 1, format.GetDayName((DayOfWeek)(i % 7)));
                    }
                    break;
                case 2:
                    for (int i = 1; i < 32; i++)
                    {
                        sb.AppendFormat("<option value='{0}' id='ddlPeriodItems{0}'>{0}</option>", i);
                    }
                    break;
            }

            return sb.ToString();
        }

        [AjaxMethod]
        public string InitProjectsDdlByTag(string tag)
        {
            ProjectSecurity.DemandAuthentication();

            if (string.IsNullOrEmpty(tag))
            {
                return GetAllProjectsDropDownList();
            }
            else
            {
                var sb = new StringBuilder()
                    .AppendFormat("<option value='-1' id='ddlProject-1'>{0}</option>", ProjectResource.AllProjects);

                foreach (var id in Global.EngineFactory.GetTagEngine().GetTagProjects(tag))
                {
                    var p = Global.EngineFactory.GetProjectEngine().GetByID(id);
                    if (p != null)
                    {
                        sb.AppendFormat("<option value='{0}' id='ddlProject{0}'>{1}</option>", id, p.HtmlTitle.HtmlEncode());
                    }
                }

                return sb.ToString();
            }
        }

        [AjaxMethod]
        public string InitUsersDdlByTag(string tag)
        {
            ProjectSecurity.DemandAuthentication();

            if (string.IsNullOrEmpty(tag))
            {
                return GetAllUsersDropDownList();
            }
            else
            {
                var sb = new StringBuilder()
                    .AppendFormat("<option value='-1' id='ddlUser-1'>{0}</option>", CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());

                foreach (var id in Global.EngineFactory.GetTagEngine().GetTagProjects(tag))
                {
                    var p = Global.EngineFactory.GetProjectEngine().GetByID(id);
                    if (p != null)
                    {
                        var prjTeam = Global.EngineFactory.GetProjectEngine()
                                        .GetTeam(id)
                                        .Select(u => u.UserInfo)
                                        .OrderBy(u => u, UserInfoComparer.Default)
                                        .ToList();

                        sb.AppendFormat("<optgroup label='{0}'>", Global.EngineFactory.GetProjectEngine().GetByID(id).HtmlTitle.HtmlEncode());

                        foreach (var u in prjTeam)
                        {
                            if (u.Status != EmployeeStatus.Terminated)
                                sb.AppendFormat("<option value='{0}' id='ddlUser{0}'>{1}</option>", u.ID, u.DisplayUserName());
                        }
                    }
                }

                return sb.ToString();
            }
        }

        [AjaxMethod]
        public string GetReportUrl(string reportParams)
        {
            ProjectSecurity.DemandAuthentication();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var rp = serializer.Deserialize<ReportParams>(reportParams);

            var reptype = 0;
            int.TryParse(rp.ReportType, out reptype);

            var filter = ParseJSONFilter(rp);
            return string.Format("reports.aspx?action=generate&reportType={0}&{1}", reptype, filter.ToUri());
        }

        [AjaxMethod]
        public void DeleteAllTemplates()
        {
            ProjectSecurity.DemandAuthentication();

            var myTemplates = Global.EngineFactory.GetReportEngine().GetTemplates(SecurityContext.CurrentAccount.ID);

            foreach (var template in myTemplates)
            {
                Global.EngineFactory.GetReportEngine().DeleteTemplate(template.Id);
            }
        }

        [AjaxMethod]
        public void DeleteAutoTemplates()
        {
            ProjectSecurity.DemandAuthentication();

            var autoTemplates = Global.EngineFactory.GetReportEngine().GetTemplates(SecurityContext.CurrentAccount.ID).FindAll(t => t.AutoGenerated);

            foreach (var template in autoTemplates)
            {
                Global.EngineFactory.GetReportEngine().DeleteTemplate(template.Id);
            }
        }

        public string GetInfoOffImageUrl()
        {
            return WebImageSupplier.GetAbsoluteWebPath("info_off.png", ProductEntryPoint.ID);
        }

        public string GetInfoOnImageUrl()
        {
            return WebImageSupplier.GetAbsoluteWebPath("info_on.png", ProductEntryPoint.ID);
        }

        public string GetCsvImageUrl()
        {
            return WebImageSupplier.GetAbsoluteWebPath("csv_16.png", ProductEntryPoint.ID);
        }

        public string GetPrintImageUrl()
        {
            return WebImageSupplier.GetAbsoluteWebPath("printer.gif", ProductEntryPoint.ID);
        }

        public string GetReportDescription(int reportType)
        {
            return ReportHelper.GetReportInfo((ReportType)reportType).Description.ReplaceSingleQuote().Replace("\n", "<br/>");
        }

        public string InitTemplatesDdl()
        {
            var templates = Global.EngineFactory.GetReportEngine().GetTemplates(SecurityContext.CurrentAccount.ID);
            var sb = new StringBuilder()
                .AppendFormat("<option value='-1' id='ddlTemplate-1'>{0}</option>", ReportResource.ChooseTemplate);
            templates.ForEach(p => sb.AppendFormat("<option value='{0}' id='ddlTemplate{0}'>{1}</option>", p.Id, p.Name));
            return sb.ToString();
        }

        public void InitActionPanel()
        {
            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Name = ReportResource.CreateNewReport,
                URL = "reports.aspx"
            });
            if (SecurityContext.IsAuthenticated)
            {
                SideNavigatorPanel.Controls.Add(new NavigationItem(ReportResource.MyTemplates, "templates.aspx"));
            }
        }

        private string GetAllUsersDropDownList()
        {
            var sb = new StringBuilder().AppendFormat("<option value='-1' id='ddlUser-1'>{0}</option>", CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());

            CoreContext.UserManager.GetUsers()
                .Where(u => string.IsNullOrEmpty(u.Department))
                .OrderBy(u => u, UserInfoComparer.Default)
                .ToList()
                .ForEach(u => sb.AppendFormat("<option value='{0}' id='ddlUser{0}'>{1}</option>", u.ID, u.DisplayUserName()));

            foreach (var g in CoreContext.GroupManager.GetGroups().OrderBy(g => g.Name))
            {
                sb.AppendFormat("<optgroup label=\"{0}\">", g.Name.HtmlEncode());
                foreach (var u in CoreContext.UserManager.GetUsersByGroup(g.ID).OrderBy(u => u.DisplayUserName()))
                {
                    sb.AppendFormat("<option value='{0}' id='ddlUser{0}'>{1}</option>", u.ID, u.DisplayUserName());
                }
            }
            return sb.ToString();
        }

        private string GetAllProjectsDropDownList()
        {
            var mine = Global.EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID).ToList();
            var projects = Global.EngineFactory.GetProjectEngine().GetAll();

            var active = projects.Where(p => p.Status == ProjectStatus.Open && !mine.Contains(p)).ToList();
            var archive = projects.Where(p => p.Status == ProjectStatus.Closed && !mine.Contains(p)).ToList();

            var sb = new StringBuilder()
                .AppendFormat("<option value='-1' id='ddlProject-1'>{0}</option>", ProjectResource.AllProjects)
                .AppendFormat("<optgroup label='{0}'>", ProjectResource.MyProjects);

            mine.ForEach(p => sb.AppendFormat("<option value='{0}' id='ddlProject{0}'>{1}</option>", p.ID, p.HtmlTitle.HtmlEncode()));
            sb.Append("</optgroup>");

            sb.AppendFormat("<optgroup label='{0}'>", ProjectResource.ActiveProjects);
            active.ForEach(p => sb.AppendFormat("<option value='{0}' id='ddlProject{0}'>{1}</option>", p.ID, p.HtmlTitle.HtmlEncode()));
            sb.Append("</optgroup>");

            sb.AppendFormat("<optgroup label='{0}'>", ProjectResource.ArchiveProjects);
            archive.ForEach(p => sb.AppendFormat("<option value='{0}' id='ddlProject{0}'>{1}</option>", p.ID, p.HtmlTitle.HtmlEncode()));
            sb.Append("</optgroup>");

            return sb.ToString();
        }

        private ReportFilter ParseJSONFilter(ReportParams rp)
        {
            var filter = new ReportFilter();

            var reportType = 0;
            int.TryParse(rp.ReportType, out reportType);

            if (HasValue(rp.Project))
            {
                filter.ProjectIds.Add(int.Parse(rp.Project));
            }
            if (HasValue(rp.User))
            {
                filter.UserId = new Guid(rp.User);
            }
            if (HasValue(rp.Tag))
            {
                filter.ProjectTag = rp.Tag;
            }
            else if (HasValue(rp.Department))
            {
                filter.DepartmentId = new Guid(rp.Department);
            }
            if (HasValue(rp.FromDate))
            {
                filter.FromDate = TenantUtil.DateTimeFromUtc(DateTime.Parse(rp.FromDate).ToUniversalTime()).Date;
            }
            if (HasValue(rp.ToDate))
            {
                var date = default(DateTime);
                if (DateTime.TryParse(rp.ToDate, out date))
                {
                    filter.ToDate = date;
                }
                else
                {
                    var days = 0;
                    if (int.TryParse(rp.ToDate, out days))
                    {
                        if (days == -1)
                            filter.ToDate = DateTime.MaxValue;
                        else
                            filter.ToDate = DateTime.Now.Date.AddDays(days);
                    }
                }
            }
            if (HasValue(rp.ProjectStatuses))
            {
                if (!Convert.ToBoolean(rp.ProjectStatuses)) filter.ProjectStatuses.Add(ProjectStatus.Open);
            }
            if (HasValue(rp.MilestoneStatuses))
            {
                var statuses = rp.MilestoneStatuses
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Convert.ToBoolean(s))
                    .ToArray();
                if (statuses[0]) filter.MilestoneStatuses.Add(MilestoneStatus.Open);
                if (statuses[1]) filter.MilestoneStatuses.Add(MilestoneStatus.Closed);
            }
            if (HasValue(rp.TaskStatuses))
            {
                var statuses = rp.TaskStatuses
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Convert.ToBoolean(s))
                    .ToArray();
                if (statuses[0]) filter.TaskStatuses.Add(TaskStatus.Open);
                if (statuses[1]) filter.TaskStatuses.Add(TaskStatus.Closed);
            }
            if (HasValue(rp.ViewType))
            {
                filter.ViewType = Convert.ToBoolean(rp.ViewType) ? 0 : 1;
            }
            if (HasValue(rp.TimeInterval))
            {
                var interval = 0;
                int.TryParse(rp.TimeInterval, out interval);
                filter.TimeInterval = (ReportTimeInterval)interval;
            }

            if(HasValue(rp.NoRes))
            {
                filter.NoResponsible = Convert.ToBoolean(rp.NoRes);
            }

            return filter;
        }

        private String ParseFilter(ReportType repType, ReportFilter filter)
        {
            var sb = new StringBuilder();
            var filterData = new StringBuilder();
            var row = "<tr><td class='filterRowTitle'>{0}:</td><td>{1}</td></tr>";
            var desc = string.Empty;
            var statuses = string.Empty;

            filterData.Append("<table cellspacing='0' cellpadding='0' class='filterTable'>");

            switch (repType)
            {
                case ReportType.MilestonesExpired:
                    desc = String.Format(ReportResource.ReportLateMilestones_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, ProjectResource.Tags, (filter.ProjectTag != string.Empty && filter.ProjectTag != null) ? filter.ProjectTag.HtmlEncode() : ProjectsCommonResource.All);
                    filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                    break;

                case ReportType.MilestonesNearest:
                    desc = String.Format(ReportResource.ReportUpcomingMilestones_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, ProjectResource.Tags, (filter.ProjectTag != string.Empty && filter.ProjectTag != null) ? filter.ProjectTag.HtmlEncode() : ProjectsCommonResource.All);
                    filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                    filterData.AppendFormat(row, ReportResource.ChooseTimeInterval, filter.GetFromDate(false).ToString(DateTimeExtension.DateFormatPattern) + " - " + filter.GetToDate(false).ToString(DateTimeExtension.DateFormatPattern));
                    break;

                case ReportType.ProjectsList:
                    desc = String.Format(ReportResource.ReportProjectList_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    if (filter.ProjectStatuses.Count > 0)
                    {
                        for (int i = 0; i < filter.ProjectStatuses.Count; i++)
                        {
                            statuses += ResourceEnumConverter.ConvertToString(filter.ProjectStatuses[i]);
                            statuses += i < filter.TaskStatuses.Count - 1 ? ", " : string.Empty;
                        }
                    }
                    else
                    {
                        statuses += ResourceEnumConverter.ConvertToString(ProjectStatus.Open) + ", ";
                        statuses += ResourceEnumConverter.ConvertToString(ProjectStatus.Closed);
                    }
                    filterData.AppendFormat(row, ReportResource.ProjectsStatus, statuses);
                    break;

                case ReportType.ProjectsWithoutActiveMilestones:
                    desc = String.Format(ReportResource.ReportProjectsWithoutActiveMilestones_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    break;

                case ReportType.ProjectsWithoutActiveTasks:
                    desc = String.Format(ReportResource.ReportProjectsWithoutActiveTasks_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    break;

                case ReportType.TasksByProjects:
                    desc = String.Format(ReportResource.ReportTaskList_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, ProjectResource.Tags, (filter.ProjectTag != string.Empty && filter.ProjectTag != null) ? filter.ProjectTag.HtmlEncode() : ProjectsCommonResource.All);
                    filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    for (int i = 0; i < filter.TaskStatuses.Count; i++)
                    {
                        statuses += ResourceEnumConverter.ConvertToString(filter.TaskStatuses[i]);
                        statuses += i < filter.TaskStatuses.Count - 1 ? ", " : string.Empty;
                    }
                    filterData.AppendFormat(row, ReportResource.TasksStatus, statuses);
                    if (filter.ToDate != DateTime.MinValue && filter.ToDate != DateTime.MaxValue)
                        filterData.AppendFormat(row, TaskResource.DeadLine, filter.ToDate.ToString(DateTimeExtension.DateFormatPattern));
                    break;

                case ReportType.TasksByUsers:
                    desc = String.Format(ReportResource.ReportUserTasks_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    for (int i = 0; i < filter.TaskStatuses.Count; i++)
                    {
                        statuses += ResourceEnumConverter.ConvertToString(filter.TaskStatuses[i]);
                        statuses += i < filter.TaskStatuses.Count - 1 ? ", " : string.Empty;
                    }
                    filterData.AppendFormat(row, ReportResource.TasksStatus, statuses);
                    if (filter.ToDate != DateTime.MinValue && filter.ToDate != DateTime.MaxValue)
                        filterData.AppendFormat(row, TaskResource.DeadLine, filter.ToDate.ToString(DateTimeExtension.DateFormatPattern));
                    break;

                case ReportType.TasksExpired:
                    desc = String.Format(ReportResource.ReportLateTasks_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, ProjectResource.Tags, (filter.ProjectTag != string.Empty && filter.ProjectTag != null) ? filter.ProjectTag.HtmlEncode() : ProjectsCommonResource.All);
                    filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                    break;

                case ReportType.TimeSpend:
                    desc = String.Format(ReportResource.ReportTimeSpendSummary_Description, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    if (filter.TimeInterval != ReportTimeInterval.Absolute || (filter.TimeInterval != ReportTimeInterval.Absolute && filter.ToDate != DateTime.MaxValue))
                        filterData.AppendFormat(row, ReportResource.ChooseTimeInterval, filter.GetFromDate(false).ToString(DateTimeExtension.DateFormatPattern) + " - " + filter.GetToDate(false).ToString(DateTimeExtension.DateFormatPattern));
                    filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                    break;

                case ReportType.UsersActivity:
                    desc = String.Format(ReportResource.ReportUserActivity_Descripton, "<ul>", "</ul>", "<li>", "</li>");
                    filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                    filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    if (filter.TimeInterval != ReportTimeInterval.Absolute || (filter.TimeInterval != ReportTimeInterval.Absolute && filter.ToDate != DateTime.MaxValue))
                        filterData.AppendFormat(row, ReportResource.ChooseTimeInterval, filter.GetFromDate(false).ToString(DateTimeExtension.DateFormatPattern) + " - " + filter.GetToDate(false).ToString(DateTimeExtension.DateFormatPattern));
                    break;

                case ReportType.UsersWithoutActiveTasks:
                    desc = String.Format(ReportResource.ReportEmployeesWithoutActiveTasks_Description, "<ul>", "</ul>", "<li>", "</li>");
                    if (filter.ViewType == 0)
                    {
                        filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                        filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    }
                    if (filter.ViewType == 1)
                    {
                        filterData.AppendFormat(row, ProjectResource.Tags, (filter.ProjectTag != string.Empty && filter.ProjectTag != null) ? filter.ProjectTag.HtmlEncode() : ProjectsCommonResource.All);
                        filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                        filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    }
                    break;

                case ReportType.UsersWorkload:
                    desc = String.Format(ReportResource.ReportEmployment_Description, "<ul>", "</ul>", "<li>", "</li>");
                    if (filter.ViewType == 0)
                    {
                        filterData.AppendFormat(row, CustomNamingPeople.Substitute<ReportResource>("Department").HtmlEncode(), filter.DepartmentId != Guid.Empty ? CoreContext.GroupManager.GetGroupInfo(filter.DepartmentId).Name.HtmlEncode() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments"));
                        filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    }
                    if (filter.ViewType == 1)
                    {
                        filterData.AppendFormat(row, ProjectResource.Tags, (filter.ProjectTag != string.Empty && filter.ProjectTag != null) ? filter.ProjectTag.HtmlEncode() : ProjectsCommonResource.All);
                        filterData.AppendFormat(row, ProjectResource.Project, (filter.ProjectIds != null && filter.ProjectIds.Count == 1) ? Global.EngineFactory.GetProjectEngine().GetByID(filter.ProjectIds[0]).Title.HtmlEncode() : ProjectResource.AllProjects);
                        filterData.AppendFormat(row, ReportResource.User, filter.UserId != Guid.Empty ? CoreContext.UserManager.GetUsers(filter.UserId).DisplayUserName() : CustomNamingPeople.Substitute<ProjectsCommonResource>("AllUsers").HtmlEncode());
                    }
                    break;
            }

            filterData.AppendFormat(row, ReportResource.GenerationDate, TenantUtil.DateTimeNow().ToString(DateTimeExtension.ShortDatePattern));

            filterData.Append("</table>");

            sb.AppendFormat("<div id='faux'><div id='leftcolumn'>{0}</div><div id='rightcolumn'><table cellspacing='0' cellpadding='0'><tr><td valign='top'><b>{1}:</b></td><td>{2}</td></tr></table></div><div class='clear'></div></div>",
                filterData.ToString(), ProjectResource.ProjectDescription, desc);

            return sb.ToString();
        }

        private bool HasValue(string value)
        {
            return !string.IsNullOrEmpty(value) && value != "-1";
        }

        [AjaxMethod]
        public string SaveTemplate(string reportParams, string title, int period, int periodItem, int hour, bool sendEmail)
        {
            ProjectSecurity.DemandAuthentication();

            if (title == null || title.Trim().Length == 0) throw new ArgumentNullException("title");

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var rp = serializer.Deserialize<ReportParams>(reportParams);
            var filter = ParseJSONFilter(rp);

            var reptype = 0;
            int.TryParse(rp.ReportType, out reptype);

            var template = new ReportTemplate((ReportType)reptype);
            template.Filter = filter;

            SaveOrUpdateTemplate(template, title, period, periodItem, hour, sendEmail);

            return string.Empty;
        }

        [AjaxMethod]
        public object UpdateTemplate(int Id, string Name, int period, int periodItem, int hour, bool AutoGenerated)
        {
            ProjectSecurity.DemandAuthentication();

            SaveOrUpdateTemplate(Global.EngineFactory.GetReportEngine().GetTemplate(Id), Name, period, periodItem, hour, AutoGenerated);
            return new { Message = "success", Name = Name, Cron = Global.EngineFactory.GetReportEngine().GetTemplate(Id).Cron };
        }

        private ReportTemplate SaveOrUpdateTemplate(ReportTemplate template, string title, int period, int periodItem, int hour, bool autoGenerated)
        {
            template.Name = HttpUtility.HtmlEncode(title);
            switch (period)
            {
                case 0:
                    template.Cron = string.Format("0 0 {0} * * ?", periodItem);
                    break;
                case 1:
                    template.Cron = string.Format("0 0 {0} ? * {1}", hour, periodItem);
                    break;
                case 2:
                    template.Cron = string.Format("0 0 {0} {1} * ?", hour, periodItem);
                    break;
            }
            template.AutoGenerated = autoGenerated;
            return Global.EngineFactory.GetReportEngine().SaveTemplate(template);
        }

        [Serializable]
        class ReportParams
        {
            public string ReportType = null;
            public string Project = null;
            public string Department = null;
            public string User = null;
            public string Tag = null;
            public string FromDate = null;
            public string ToDate = null;
            public string ProjectStatuses = null;
            public string MilestoneStatuses = null;
            public string TaskStatuses = null;
            public string ViewType = null;
            public string TimeInterval = null;
            public string NoRes = null;
        }
    }
}
