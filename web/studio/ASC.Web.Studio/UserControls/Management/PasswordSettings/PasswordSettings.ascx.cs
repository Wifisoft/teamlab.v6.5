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
    [AjaxNamespace("PasswordSettingsController")]
    public partial class PasswordSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/PasswordSettings/PasswordSettings.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "passwordsettings_ui_script", WebPath.GetPath("js/auto/jquery-ui.custom.min.js"));
            //Page.ClientScript.RegisterClientScriptInclude(typeof(string), "slider_ui_script", WebPath.GetPath("usercontrols/management/PasswordSettings/js/slider.js"));
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "passwordsettings_script", WebPath.GetPath("usercontrols/management/PasswordSettings/js/PasswordSettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "passwordsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/PasswordSettings/css/<theme_folder>/PasswordSettings.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "passwordsettings_style_slider", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/PasswordSettings/css/<theme_folder>/jquery-ui.custom.css") + "\">", false);

        }

        [AjaxMethod]
        public object SavePasswordSettings(string objData)
        {
            try
            {
                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var passwordSettingsObj = jsSerializer.Deserialize<StudioPasswordSettings>(objData);
                var resultStatus = SettingsManager.Instance.SaveSettings(passwordSettingsObj,
                                                                         TenantProvider.CurrentTenantID);

                return
                    new
                        {
                            Status = 1,
                            Message = Resources.Resource.SuccessfullySaveSettingsMessage
                        };
            }
            catch (Exception e)
            {
                return new {Status = 0, Message = e.Message.HtmlEncode()};
            }
        }

        [AjaxMethod]
        public string LoadPasswordSettings()
        {
            var json = String.Empty;
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var passwordSettingsObj =
                SettingsManager.Instance.LoadSettings<StudioPasswordSettings>(TenantProvider.CurrentTenantID);


            return serializer.Serialize(passwordSettingsObj);
        }
    }
}