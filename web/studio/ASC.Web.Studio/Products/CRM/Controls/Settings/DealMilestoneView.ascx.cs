using System;
using ASC.Web.Core.Security.Ajax;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.DealMilestoneView")]
    public partial class DealMilestoneView : BaseUserControl
    {
        #region Members

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/DealMilestoneView.ascx"); } }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(this.GetType());

            _manageDealMilestonePopup.Options.IsPopup = true;

            var apiServer = new Api.ApiServer();
            var data = apiServer.GetApiResponse("api/1.0/crm/opportunity/stage.json", "GET");

            Page.JsonPublisher(data, "dealMilestoneList");
        }

        #endregion

        #region Ajax Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void ReorderDealMilestone(int[] itemsIDs)
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            Global.DaoFactory.GetDealMilestoneDao().Reorder(itemsIDs);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ChangeColor(int itemID, string color)
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            var item = Global.DaoFactory.GetDealMilestoneDao().GetByID(itemID);
            item.Color = color;
            Global.DaoFactory.GetDealMilestoneDao().ChangeColor(item.ID, color);
            return color;
        }

        #endregion

    }
}


