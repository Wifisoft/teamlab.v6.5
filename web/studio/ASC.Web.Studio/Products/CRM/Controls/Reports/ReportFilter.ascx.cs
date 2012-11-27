using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Text;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Dao;
using ASC.Web.Core.Users;
using ASC.Web.CRM;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core.Users;


namespace ASC.Web.CRM.Controls.Reports
{
    public partial class ReportFilter : BaseUserControl
    {
        public TextBox tbxFromDate;
        public TextBox tbxToDate;

      
        public ReportType ReportType
        {
            get;
            set;
        }
        public string ReportDesc
        {
            get
            {
                switch (ReportType)
                {
                    case ReportType.PipelineByForecast:
                        return String.Format(CRMReportResource.Report_PipelineByForecast_Description, "<ul>", "</ul>", "<li>", "</li>");

                    case ReportType.PipelineByMilestone:
                        return String.Format(CRMReportResource.Report_PipelineByMilestone_Description, "<ul>", "</ul>", "<li>", "</li>");
                }
                return String.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ReportType == ReportType.PipelineByMilestone)
            {
                usersActivityFromDate.Text = TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern);
                usersActivityToDate.Text = TenantUtil.DateTimeNow().AddDays(7).ToString(DateTimeExtension.DateFormatPattern);
            }

            if (ReportType == ReportType.PipelineByForecast)
            {
                usersActivityFromDate.Text = TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern);
                usersActivityToDate.Text = TenantUtil.DateTimeNow().AddDays(7).ToString(DateTimeExtension.DateFormatPattern);
            }


            //var reportTemplate = (ReportTemplateView)LoadControl(PathProvider.GetControlVirtualPath("ReportTemplateView.ascx"));
            //reportTemplate.Template = null;
            //reportTemplatePh.Controls.Add(reportTemplate);

            HiddenFieldViewReportFilters.Value = "1";
            HiddenFieldViewReportTemplate.Value = "1";

            RegisterClientScript();

        }


          public string InitManagersDdl()
            {
            var sb = new StringBuilder().AppendFormat("<option value='-1' id='ddlUser-1'>{0}</option>", CustomNamingPeople.Substitute<CRMCommonResource>("AllUsers"));

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


        public string GetReportTypeTitle()
        {
            //return ReportHelper.GetReportInfo(ReportType).Title.ReplaceSingleQuote();
            return String.Empty;
        }

       


        private void RegisterClientScript()
        {
            //Page.ClientScript.RegisterClientScriptBlock(typeof(ReportFilter), "C4BE269D-DD10-46f1-8023-F2DAFC7FE8F7", "CurrFilter = " +
            //                                                              JavaScriptSerializer.Serialize(Filter) +"; ", true);
            
            //var TaskStatusses = 0;
            //if (Filter.TaskStatuses.Count == 2 && Filter.TaskStatuses.FindAll(ts=>ts==TaskStatus.Unclassified).Count==0) TaskStatusses = 2;
            //if (Filter.TaskStatuses.Count == 2 && Filter.TaskStatuses.FindAll(ts => ts == TaskStatus.Closed).Count == 0) TaskStatusses = 3;
            //if (Filter.TaskStatuses.Count == 2 && Filter.TaskStatuses.FindAll(ts => ts == TaskStatus.Open).Count == 0) TaskStatusses = 4;
            //if (Filter.TaskStatuses.Count == 3) TaskStatusses = 5;
           // Page.ClientScript.RegisterClientScriptBlock(typeof(string), "A41B163D-BF19-41c5-98E3-6FE026149245", "CurrFilterDate = " +
           //                                                               JavaScriptSerializer.Serialize(new
           //                                                                   {
           //                                                                       ToDate = Filter.ToDate.ToString(DateTimeExtension.DateFormatPattern),
           //                                                                       ForomDate = Filter.FromDate.ToString(DateTimeExtension.DateFormatPattern),
           //                                                                       TStatus = TaskStatusses
           //                                                                   }), true);
        }
    }





}

