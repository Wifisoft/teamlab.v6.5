using System;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AdmMessController")]
    public partial class AdminMessageSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/AdminMessageSettings/AdminMessageSettings.ascx"; } }
        protected StudioAdminMessageSettings _studioAdmMessNotifSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "admmess_script", WebPath.GetPath("usercontrols/Management/AdminMessageSettings/js/admmess.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "admmess_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/AdminMessageSettings/css/admmess.css") + "\">", false);

            _studioAdmMessNotifSettings = SettingsManager.Instance.LoadSettings<StudioAdminMessageSettings>(TenantProvider.CurrentTenantID);
       
        }

        [AjaxMethod]
        public object SaveSettings(bool turnOn)
        {
            try
            {
                var passwordSettingsObj = new StudioAdminMessageSettings { Enable = turnOn};
                var resultStatus = SettingsManager.Instance.SaveSettings(passwordSettingsObj, TenantProvider.CurrentTenantID);
                return
                 new
                 {
                     Status = 1,
                     Message = Resources.Resource.SuccessfullySaveSettingsMessage
                 };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }
    }
}