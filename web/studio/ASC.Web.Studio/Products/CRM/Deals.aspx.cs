#region Import

using System;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.Controls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Deals;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.CRM.Core;

#endregion

namespace ASC.Web.CRM
{
    public partial class Deals : BasePage
    {
        protected override void PageLoad()
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof (Tasks));
            
            int dealID;

            if (int.TryParse(Request["id"], out dealID))
            {

                Deal targetDeal = Global.DaoFactory.GetDealDao().GetByID(dealID);

                if (targetDeal == null || !CRMSecurity.CanAccessTo(targetDeal))
                    Response.Redirect(PathProvider.StartURL());

                if (String.Compare(Request["action"], "manage", true) == 0)
                    ExecDealActionView(targetDeal);
                else
                    ExecDealDetailsView(targetDeal);

            }
            else
            {
                if (String.Compare(Request["action"], "manage", true) == 0)
                    ExecDealActionView(null);
                else if (String.Compare(UrlParameters.Action, "import", true) == 0)
                    ExecImportView();
                else
                    ExecListDealView();
            }

        }

        #region Events

        protected void ExecImportView()
        {
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = CRMDealResource.ImportDeals
            });

            Master.DisabledSidePanel = true;

            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);

            importViewControl.EntityType = EntityType.Opportunity;

            CommonContainerHolder.Controls.Add(importViewControl);

            Title = HeaderStringHelper.GetPageTitle(CRMDealResource.ImportDeals, Master.BreadCrumbs);
        }

        protected void ExecDealDetailsView(Deal targetDeal)
        {

            if (!CRMSecurity.CanAccessTo(targetDeal))
                Response.Redirect(PathProvider.StartURL());

            var dealActionViewControl = (DealDetailsView) LoadControl(DealDetailsView.Location);

            dealActionViewControl.TargetDeal = targetDeal;

            Master.DisabledSidePanel = true;

            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = CRMDealResource.AllDeals,
                                           NavigationUrl = "deals.aspx"
                                       });

            var currentPageTitle = targetDeal.Title;

            if (targetDeal.ContactID > 0)
            {
                var contact = Global.DaoFactory.GetContactDao().GetByID(targetDeal.ContactID);
                if (contact != null)
                    currentPageTitle = String.Format("{0}: {1}", contact.GetTitle(), targetDeal.Title);
            }

            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = currentPageTitle
                                       });

            if (CRMSecurity.IsPrivate(targetDeal))
                Master.CommonContainerHeader = Global.RenderPrivateItemHeader(currentPageTitle.HtmlEncode(), EntityType.Opportunity, targetDeal.ID);

            CommonContainerHolder.Controls.Add(dealActionViewControl);

            Title = HeaderStringHelper.GetPageTitle(String.Format("{0}: {1}", currentPageTitle.HtmlEncode(), targetDeal.Title.HtmlEncode()), Master.BreadCrumbs);
        }

        protected void ExecListDealView()
        {
            var headerTitle = CRMDealResource.AllDeals;

            if (!String.IsNullOrEmpty(Request["userID"])) headerTitle = CRMDealResource.MyDeals;

            var listDealViewControl = (ListDealView) LoadControl(ListDealView.Location);
            listDealViewControl.contactID = 0;

            CommonContainerHolder.Controls.Add(listDealViewControl);

            Master.DisabledSidePanel = true;
            Master.BreadCrumbs.Add(new BreadCrumb {Caption = headerTitle});
            Title = HeaderStringHelper.GetPageTitle(headerTitle, Master.BreadCrumbs);
        }

        protected void ExecDealActionView(Deal targetDeal)
        {
            var dealActionViewControl = (DealActionView) LoadControl(DealActionView.Location);

            dealActionViewControl.TargetDeal = targetDeal;

            CommonContainerHolder.Controls.Add(dealActionViewControl);

            var headerTitle = targetDeal == null ? CRMDealResource.CreateNewDeal : String.Format(CRMDealResource.EditDealLabel, targetDeal.Title);

            Master.DisabledSidePanel = true;
            Master.BreadCrumbs.Add(new BreadCrumb { Caption = headerTitle });
            Title = HeaderStringHelper.GetPageTitle(headerTitle.HtmlEncode(), Master.BreadCrumbs);
        }

        #endregion
    }
}