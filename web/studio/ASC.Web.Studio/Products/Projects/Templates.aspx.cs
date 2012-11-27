using ASC.Web.Controls;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects
{
    public partial class Templates : BasePage
    {
        protected override void PageLoad()
        {
            ((IStudioMaster)Master).DisabledSidePanel = true;

            AjaxPro.Utility.RegisterTypeForAjax(typeof(Reports));
            ((Masters.BasicTemplate)(Master)).BreadCrumbs.Add(new BreadCrumb { Caption = ReportResource.Reports, NavigationUrl = "reports.aspx" });

            ((Masters.BasicTemplate)(Master)).BreadCrumbs.Add(new BreadCrumb { Caption = ReportResource.MyTemplates });

            Title = HeaderStringHelper.GetPageTitle(ReportResource.Reports, ((Masters.BasicTemplate)(Master)).BreadCrumbs);

            InitActionPanel();
        }

        public void InitActionPanel()
        {
            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Name = ReportResource.CreateNewReport,
                URL = "reports.aspx"
            });/*
            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Name = ReportResource.ReportHistory,
                URL = "reports.aspx"
            });*/
            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Name = ReportResource.MyTemplates,
                URL = "templates.aspx"
            });
        }

    }
}
