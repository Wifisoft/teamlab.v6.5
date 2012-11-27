#region Import

using System;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.Controls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Cases;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;
using ASC.CRM.Core;

#endregion

namespace ASC.Web.CRM
{
    public partial class Cases : BasePage
    {
        #region Properies

        #endregion

        #region Events

        protected override void PageLoad()
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof (Tasks));

            int caseID;

            if (int.TryParse(UrlParameters.ID, out caseID))
            {

                ASC.CRM.Core.Entities.Cases targetCase = Global.DaoFactory.GetCasesDao().GetByID(caseID);

                if (targetCase == null || !CRMSecurity.CanAccessTo(targetCase))
                    Response.Redirect(PathProvider.StartURL());

                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                {
                    ExecCasesActionView(targetCase);
                }
                else if (String.Compare(UrlParameters.Action, "delete", true) == 0)
                {
                    DeleteCase(caseID);
                    Response.Redirect("cases.aspx");
                }
                else
                {
                    ExecCasesDetailsView(targetCase);
                }
            }
            else
            {
                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                    ExecCasesActionView(null);
                else if (String.Compare(UrlParameters.Action, "import", true) == 0)
                    ExecImportView();
                else
                    ExecListCasesView();
            }
        }

        #endregion

        #region Methods

        protected void ExecImportView()
        {
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = CRMCasesResource.ImportCases
            });

            Master.DisabledSidePanel = true;

            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);

            importViewControl.EntityType = EntityType.Case;

            CommonContainerHolder.Controls.Add(importViewControl);

            Title = HeaderStringHelper.GetPageTitle(CRMCasesResource.ImportCases, Master.BreadCrumbs);
        }

        protected void ExecListCasesView()
        {
            Master.DisabledSidePanel = true;
            Master.BreadCrumbs.Add(new BreadCrumb {Caption = CRMCasesResource.AllCases});
            Title = HeaderStringHelper.GetPageTitle(CRMCasesResource.AllCases, Master.BreadCrumbs);
            CommonContainerHolder.Controls.Add(LoadControl(ListCasesView.Location));
        }

        protected void ExecCasesDetailsView(ASC.CRM.Core.Entities.Cases targetCase)
        {
            Master.DisabledSidePanel = true;
            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = CRMCasesResource.AllCases,
                                           NavigationUrl = "cases.aspx"
                                       });

            var casesDetailsViewControl = (CasesDetailsView) LoadControl(CasesDetailsView.Location);
            casesDetailsViewControl.TargetCase = targetCase;

            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = targetCase.Title
                                       });

            var title = targetCase.Title.HtmlEncode();

            if (CRMSecurity.IsPrivate(targetCase))
                Master.CommonContainerHeader = Global.RenderPrivateItemHeader(title, EntityType.Case, targetCase.ID);

            CommonContainerHolder.Controls.Add(casesDetailsViewControl);

            Title = HeaderStringHelper.GetPageTitle(targetCase.Title.HtmlEncode(), Master.BreadCrumbs);
        }

        protected void ExecCasesActionView(ASC.CRM.Core.Entities.Cases targetCase)
        {
            Master.DisabledSidePanel = true;
            
            var casesActionViewControl = (CasesActionView) LoadControl(CasesActionView.Location);

            casesActionViewControl.TargetCase = targetCase;

            CommonContainerHolder.Controls.Add(casesActionViewControl);

            var headerTitle = targetCase == null ? CRMCasesResource.AddCase : String.Format(CRMCasesResource.EditCaseHeader, targetCase.Title);

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = headerTitle
            });

            SideNavigatorPanel.Visible = false;

            //if (CRMSecurity.IsAdmin)
            //{
            //    SideActionsPanel.Controls.Add(new NavigationItem
            //    {
            //        Name = CRMDealResource.SettingCustomFields,
            //        URL = "settings.aspx?type=custom_field&view=case"
            //    });
            //}

            //if (targetCase != null)
            //    SideActionsPanel.Controls.Add(new NavigationItem
            //    {
            //        Name = CRMCasesResource.DeleteThisCase,
            //        URL = String.Format("javascript:deleteCase({0})", targetCase.ID)
            //    });

            Title = HeaderStringHelper.GetPageTitle(headerTitle.HtmlEncode(), Master.BreadCrumbs);
        }

        protected void DeleteCase(int id)
        {
            Global.DaoFactory.GetCasesDao().DeleteCases(id);
        }

        #endregion

    }
}