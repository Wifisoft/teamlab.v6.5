using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Core.Users;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Projects.Controls.Reports
{
    public partial class ReportFilters : BaseUserControl
    {
        public TextBox tbxFromDate;
        public TextBox tbxToDate;

        public ReportFilter Filter { get; set; }

        public ReportType ReportType
        {
            get;
            set;
        }

        public string ReportDesc
        {
            get { return ReportHelper.GetReportInfo(ReportType).Description; }
        }

        protected void cblTaskStatus_onDataBound(object sender, EventArgs e)
        {
            RadioButtonList rbl = (RadioButtonList)sender;
            foreach (ListItem li in rbl.Items)
            {
                li.Attributes.Add("onclick", "javascript:setDisableAttr('" + li.Value + "')");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ReportType == ReportType.UsersActivity)
            {
                usersActivityFromDate.Text = TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern);
                usersActivityToDate.Text = TenantUtil.DateTimeNow().AddDays(7).ToString(DateTimeExtension.DateFormatPattern);
            }
            
            if (ReportType == ReportType.TimeSpend)
            {
                timeSpendFromDate.Text = TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern);
                timeSpendToDate.Text = TenantUtil.DateTimeNow().AddDays(7).ToString(DateTimeExtension.DateFormatPattern);
            }

            if (ReportType == ReportType.TasksByProjects || ReportType == ReportType.TasksByUsers)
            {
                cblTaskStatus.Items[0].Text = ReportResource.ActiveTaskStatus;
                cblTaskStatus.Items[1].Text = ReportResource.ClosedTaskStatus;
                cblTaskStatus.Items[2].Text = TaskResource.All;
            }

            var reportTemplate = (ReportTemplateView)LoadControl(PathProvider.GetControlVirtualPath("ReportTemplateView.ascx"));
            reportTemplate.Template = null;
            reportTemplatePh.Controls.Add(reportTemplate);

            HiddenFieldViewReportFilters.Value = "1";
            HiddenFieldViewReportTemplate.Value = "1";

            RegisterClientScript();
        }

        public string InitUsersDdl()
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
                foreach (var u in CoreContext.UserManager.GetUsersByGroup(g.ID).OrderBy(u => u, UserInfoComparer.Default))
                {
                    sb.AppendFormat("<option value='{0}' id='ddlUser{0}'>{1}</option>", u.ID, u.DisplayUserName());
                }
            }

            return sb.ToString();
        }

        public string InitProjectsDdl()
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

        public string InitDepartmentsDdl()
        {
            var sb = new StringBuilder()
                .AppendFormat("<option value='-1' id='ddlDepartment-1'>{0}</option>", CustomNamingPeople.Substitute<ProjectsCommonResource>("AllDepartments").HtmlEncode());
            CoreContext.UserManager
                .GetDepartments()
                .OrderBy(g => g.Name)
                .ToList()
                .ForEach(g => sb.AppendFormat("<option value='{0}' id='ddlDepartment{0}'>{1}</option>", g.ID, g.Name.HtmlEncode()));
            return sb.ToString();
        }

        public string InitTagsDdl()
        {
            var sb = new StringBuilder()
                .AppendFormat("<option value='' id='ddlTag'>{0}</option>", ProjectsCommonResource.All);
            var tags = Global.EngineFactory.GetTagEngine().GetTags();

            foreach (var tag in tags)
            {
                sb.AppendFormat("<option value='{0}' id='ddlTag{1}'>{0}</option>",tag.Value, tag.Key);
            }

            return sb.ToString();
        }

        public string InitUpcomingIntervalsDdl(bool withAnyOption)
        {
            var sb = new StringBuilder();

            if(withAnyOption)
                sb.AppendFormat("<option value='-1' id='ddlUpcomingInterval-1'>{0}</option>", ReportResource.Any);

            sb.AppendFormat("<option value='7' id='ddlUpcomingInterval7'>1 {0}</option>", ReportResource.Week)
                .AppendFormat("<option value='14' id='ddlUpcomingInterval14'>2 {0}</option>", ReportResource.Weeks)
                .AppendFormat("<option value='21' id='ddlUpcomingInterval21'>3 {0}</option>", ReportResource.Weeks)
                .AppendFormat("<option value='28' id='ddlUpcomingInterval28'>4 {0}</option>", ReportResource.Weeks)
                .AppendFormat("<option value='35' id='ddlUpcomingInterval35'>5 {0}</option>", ReportResource.Weeks);

            return sb.ToString();
        }

        public string InitTimeIntervalsDdl()
        {
            var sb = new StringBuilder()
                .AppendFormat("<option value='2' id='ddlTimeInterval2'>{0}</option>", ReportResource.Today)
                .AppendFormat("<option value='3' id='ddlTimeInterval3'>{0}</option>", ReportResource.Yesterday)
                .AppendFormat("<option value='5' id='ddlTimeInterval5'>{0}</option>", ReportResource.ThisWeek)
                .AppendFormat("<option value='6' id='ddlTimeInterval6' selected='selected'>{0}</option>", ReportResource.LastWeek)
                .AppendFormat("<option value='8' id='ddlTimeInterval8'>{0}</option>", ReportResource.ThisMonth)
                .AppendFormat("<option value='9' id='ddlTimeInterval9'>{0}</option>", ReportResource.LastMonth)
                .AppendFormat("<option value='11' id='ddlTimeInterval11'>{0}</option>", ReportResource.ThisYear)
                .AppendFormat("<option value='12' id='ddlTimeInterval12'>{0}</option>", ReportResource.LastYear)
                .AppendFormat("<option value='0' id='ddlTimeInterval0'>{0}</option>", ReportResource.Other);

            return sb.ToString();
        }

        public string GetReportTypeTitle()
        {
            return ReportHelper.GetReportInfo(ReportType).Title.ReplaceSingleQuote();
        }

        private void RegisterClientScript()
        {
            Page.ClientScript.RegisterClientScriptBlock(typeof(ReportFilter), "C4BE269D-DD10-46f1-8023-F2DAFC7FE8F7", "CurrFilter = " +
                                                                          JavaScriptSerializer.Serialize(Filter) +"; ", true);
            
            var TaskStatusses = 0;
            if (Filter.TaskStatuses.Count == 1 && Filter.TaskStatuses[0] == TaskStatus.Open) TaskStatusses = 0;
            if (Filter.TaskStatuses.Count == 1 && Filter.TaskStatuses[0]==TaskStatus.Closed) TaskStatusses = 1;
            if (Filter.TaskStatuses.Count == 2 && Filter.TaskStatuses.FindAll(ts => ts == TaskStatus.Open).Count != 0) TaskStatusses = 2;
            if (Filter.TaskStatuses.Count == 2 && Filter.TaskStatuses.FindAll(ts => ts == TaskStatus.Closed).Count == 0) TaskStatusses = 3;
            if (Filter.TaskStatuses.Count == 2 && Filter.TaskStatuses.FindAll(ts => ts == TaskStatus.Open).Count == 0) TaskStatusses = 4;
            if (Filter.TaskStatuses.Count == 3) TaskStatusses = 5;
            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "A41B163D-BF19-41c5-98E3-6FE026149242", "CurrFilterDate = " +
                                                                          JavaScriptSerializer.Serialize(new
                                                                              {
                                                                                  ToDate = Filter.ToDate.ToString(DateTimeExtension.DateFormatPattern),
                                                                                  ForomDate = Filter.FromDate.ToString(DateTimeExtension.DateFormatPattern),
                                                                                  TStatus = TaskStatusses
                                                                              }), true);
        }
    }
}