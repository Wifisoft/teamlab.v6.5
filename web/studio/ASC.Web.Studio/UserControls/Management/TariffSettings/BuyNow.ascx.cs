using System;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class BuyNow : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/TariffSettings/BuyNow.ascx"; } }
        protected string _monthUrl = "";
        protected string _yearUrl = "";
        protected bool _isExpired;
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "buynow_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/tariffsettings/css/<theme_folder>/buynow.css") + "\">", false);

            var tariff = CoreContext.TenantManager.GetTariff(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            _isExpired = tariff.State == TariffState.Frozen;

            var tid = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var uri = CoreContext.TenantManager.GetShoppingUri(tid, CoreContext.TenantManager.GetTenantQuota(tid).Id);
            _monthUrl = uri != null ? uri.ToString() : string.Empty;
            _yearUrl = string.Empty;
        }
    }
}