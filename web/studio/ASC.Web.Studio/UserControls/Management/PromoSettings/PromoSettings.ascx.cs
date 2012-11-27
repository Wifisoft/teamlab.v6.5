using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("PromoSettingsController")]
    public partial class PromoSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/PromoSettings/PromoSettings.ascx"; } }
        protected StudioNotifyBarSettings _studioNotifyBarSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "firsttime1_script", WebPath.GetPath("usercontrols/Management/PromoSettings/js/PromoSettings.js"));

            _studioNotifyBarSettings = SettingsManager.Instance.LoadSettings<StudioNotifyBarSettings>(TenantProvider.CurrentTenantID);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SaveNotifyBarSettings(bool showPromotions)
        {
            AjaxResponse resp = new AjaxResponse();
            _studioNotifyBarSettings = SettingsManager.Instance.LoadSettings<StudioNotifyBarSettings>(TenantProvider.CurrentTenantID);
            _studioNotifyBarSettings.ShowPromotions = showPromotions;
            if (SettingsManager.Instance.SaveSettings<StudioNotifyBarSettings>(_studioNotifyBarSettings, TenantProvider.CurrentTenantID))
            {
                resp.rs1 = "1";
                resp.rs2 = "<div class=\"okBox\">" + Resources.Resource.SuccessfullySaveSettingsMessage + "</div>";
            }
            else
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class=\"errorBox\">" + Resources.Resource.UnknownError + "</div>";
            }

            return resp;
        }
    }
}