using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("SmsValidationSettingsController")]
    public partial class SmsValidationSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/SmsValidationSettings/SmsValidationSettings.ascx"; } }
        protected StudioSmsNotificationSettings _studioSmsNotifSettings;
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "smsvalid_script", WebPath.GetPath("usercontrols/Management/SmsValidationSettings/js/SmsValidation.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "smsvalid_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/SmsValidationSettings/css/SmsValidation.css") + "\">", false);

            _studioSmsNotifSettings = SettingsManager.Instance.LoadSettings<StudioSmsNotificationSettings>(TenantProvider.CurrentTenantID);
        }

        [AjaxMethod]
        public object SaveSettings(string objData)
        {
            try
            {
                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var passwordSettingsObj = jsSerializer.Deserialize<StudioSmsNotificationSettings>(objData);
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