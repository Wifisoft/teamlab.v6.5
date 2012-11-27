using System;
using System.Linq;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("TariffSettingsController")]
    public partial class TariffSettings : System.Web.UI.UserControl
    {
        protected UserInfo _owner = null;
        protected bool _canOwnerEdit;
        protected Tariff tariff;

        public static string Location { get { return "~/UserControls/Management/TariffSettings/TariffSettings.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "watermark_script", WebPath.GetPath("js/jquery.watermarkinput.js"));
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "tariffsettings_script", WebPath.GetPath("usercontrols/management/tariffsettings/js/tariffsettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "tariffsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/tariffsettings/css/<theme_folder>/tariffsettings.css") + "\">", false);

            var curTenant = CoreContext.TenantManager.GetCurrentTenant();

            tariff = CoreContext.TenantManager.GetTariff(curTenant.TenantId);

            var payments = CoreContext.TenantManager.GetTariffPayments(curTenant.TenantId);

            _paymentsRepeater.Visible = (payments.Count() > 0);
            _paymentsRepeater.DataSource = payments;
            _paymentsRepeater.DataBind();

            var buyNowControl = LoadControl(BuyNow.Location) as BuyNow;
            _buyNowHolder.Controls.Add(buyNowControl);
            _buyNowHolder.Visible = !CoreContext.Configuration.Standalone && tariff.State == TariffState.Trial;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object UseCoupon(string code)
        {
            try
            {
                CoreContext.TenantManager.SetTariffCoupon(CoreContext.TenantManager.GetCurrentTenant().TenantId, code);
                return new { status = 0, message = "" };
            }
            catch (Exception e)
            {
                return new { status = 1, message = e.Message.HtmlEncode() };
            }
        }
    }
}