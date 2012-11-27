#region Import

using System;
using ASC.CRM.Core.Dao;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;
using AjaxPro;
using TeamlabCreatorXlsx;
using EnumExtension = ASC.Web.CRM.Classes.EnumExtension;
using ASC.Web.CRM.Controls.Common;
using ASC.Data.Storage;

#endregion

namespace ASC.Web.CRM
{

    #region Types

    public struct CronFields
    {
        public int hour;
        public int period;
        public int periodItem;
    }

    #endregion

    [AjaxNamespace("AjaxPro.Reports")]
    public partial class Reports : BasePage
    {
        protected override void PageLoad()
        {
            RegisterClientScript();

            InitPageTitle();
        }

        protected void RegisterClientScript()
        {
            Page.ClientScript.RegisterClientScriptInclude(typeof(Reports), "flotlib_script",
                WebPath.GetPath("products/crm/controls/reports/jquery.flot.js"));
            Page.ClientScript.RegisterClientScriptBlock(typeof(Reports), "reports_full_screen",
                    @"<style type=""text/css"">
                    .mainPageLayout {
                        margin: 0px 20px;
                        text-align: left;  
                        width:auto;  
                    }
                    #studioPageContent{padding-bottom:0px;}
                    #studio_sidePanelUpHeight20{display:none;}                    
                    </style>", false);
        }

        protected void InitPageTitle()
        {
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = "Reports comming soon"
            });

            Master.DisabledSidePanel = true;

            //var privatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);

            //CommonContainerHolder.Controls.Add(privatePanel);

            Title = HeaderStringHelper.GetPageTitle("Reports", Master.BreadCrumbs);
        }

        protected void ToChtoBylo()
        {
        //    var reportTitle = String.Empty;

        //    ReportType reportType;
        //    ReportViewType reportViewType;

        //    if (!EnumExtension.TryParse(UrlParameters.ReportType, true, out reportType))
        //        return;

        //    if (!EnumExtension.TryParse(UrlParameters.View, true, out reportViewType))
        //        return;

        //    var fromDate = DateTime.MinValue;
        //    var toDate = DateTime.MaxValue;
        //    var responsibleID = Guid.Empty;

        //    if (!String.IsNullOrEmpty(Request["fromDate"]))
        //        fromDate = new DateTime(long.Parse(Request["fromDate"]));

        //    if (!String.IsNullOrEmpty(Request["toDate"]))
        //        toDate = new DateTime(long.Parse(Request["toDate"]));


        //    if (!String.IsNullOrEmpty(Request["responsibleID"]))
        //        responsibleID = new Guid(Request["responsibleID"]);

        //    var reportDao = Global.DaoFactory.GetReportDao();

        //    switch (reportType)
        //    {
        //        case ReportType.SalesByManager:
        //            reportContent.Text = reportDao.Transform(
        //                reportDao.BuildSalesByManagerReport(fromDate, toDate),
        //                reportType, reportViewType);

        //            reportTitle = CRMReportResource.Report_SalesByManager_Title;

        //            break;
        //        case ReportType.SalesByStage:
        //            reportContent.Text = reportDao.Transform(reportDao.BuildSalesByStageReport(
        //                                            responsibleID, fromDate,
        //                                            toDate),
        //                                    reportType,
        //                                    reportViewType);

        //            reportTitle = CRMReportResource.Report_SalesByStage_Title;
        //            break;
        //        case ReportType.SalesByContact:
        //            reportContent.Text = reportDao.Transform(
        //                reportDao.BuildSalesByContactReport(fromDate, toDate),
        //                reportType, reportViewType);

        //            reportTitle = CRMReportResource.Report_SalesByContact_Title;
        //            break;
        //        case ReportType.SalesByMonth:

        //            break;
        //        //case ReportType.SalesForecastByMonth:
        //        //        reportContent.Text = reportDao.Transform(
        //        //            reportDao.BuildSalesForecastByMonthReport(responsibleID, fromDate, toDate),
        //        //            reportType, reportViewType);

        //        //        reportTitle = CRMReportResource.Report_SalesForecastByMonth_Title;
        //        //    break;
        //        case ReportType.SalesForecastByManager:

        //            reportContent.Text = reportDao.Transform(
        //                reportDao.BuildSalesForecastByManagerReport(fromDate, toDate),
        //                reportType, reportViewType);

        //            reportTitle = CRMReportResource.Report_SalesForecastByManager_Title;
        //            break;
        //        case ReportType.SalesForecastByClient:

        //            reportContent.Text = reportDao.Transform(
        //                reportDao.BuildSalesForecastByClientReport(fromDate, toDate),
        //                reportType, reportViewType);

        //            reportTitle = CRMReportResource.Report_SalesForecastByClient_Title;

        //            break;
        //        default:
        //            break;
        //    }

        //    Master.DisabledSidePanel = true;

        //    Master.BreadCrumbs.Add(new BreadCrumb
        //    {
        //        Caption = reportTitle
        //    });

        //    Title = HeaderStringHelper.GetPageTitle(CRMCommonResource.ReportModuleName, Master.BreadCrumbs);
        }
    }
}