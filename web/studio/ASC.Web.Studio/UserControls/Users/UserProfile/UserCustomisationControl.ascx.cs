using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;
using AjaxPro;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    [AjaxNamespace("UserCustomisation")]
    public partial class UserCustomisationControl : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Users/UserProfile/UserCustomisationControl.ascx"; } }
        protected Tenant _currentTenant;
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "accountlink_script", WebPath.GetPath("usercontrols/users/userprofile/js/usersettings.js"));
            _userSkinSettings.Controls.Add(LoadControl(SkinSettings.Location));
            _currentTenant = CoreContext.TenantManager.GetCurrentTenant();

        }
        protected string RenderLanguageSelector()
        {
            var currentLanguage = CultureInfo.CurrentCulture.Name;
            var sb = new StringBuilder();
            sb.Append("<select id=\"studio_lng\" class=\"comboBox\">");
            foreach (var ci in SetupInfo.EnabledCultures)
            {
                sb.AppendFormat("<option " + (String.Equals(currentLanguage, ci.Name) ? "selected" : "") + " value=\"{0}\">{1}</option>", ci.Name, ci.DisplayName);
            }
            sb.Append("</select>");

            return sb.ToString();
        }
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveSkinSettings(string skinID)
        {
            try
            {
                var skin = WebSkin.Skins.Find(s => string.Equals(s.ID, skinID, StringComparison.InvariantCultureIgnoreCase));

                if (skin != null)
                {
                    var currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                    var settings = SettingsManager.Instance.LoadSettingsFor<WebSkinSettings>(currentUser.ID);
                    settings.WebSkin = skin;
                    settings.IsDefault = false;
                    SettingsManager.Instance.SaveSettingsFor<WebSkinSettings>(settings, currentUser.ID);
                }
                return new { Status = 1 };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SaveUserLanguageSettings(string lng)
        {
            var resp = new AjaxResponse();
            try
            {                
                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

                var changelng = false;
                if (SetupInfo.EnabledCultures.Find(c => String.Equals(c.Name, lng, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    if (user.CultureName != lng)
                    {
                        user.CultureName = lng;
                        changelng = true;
                    }
                }
                CoreContext.UserManager.SaveUserInfo(user);
                if (changelng)
                {
                    resp.rs1 = "1";
                }
                else
                {
                    resp.rs1 = "2";
                    resp.rs2 = "<div class=\"okBox\">" + Resources.Resource.SuccessfullySaveSettingsMessage + "</div>";
                }
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class=\"errorBox\">" + e.Message.HtmlEncode() + "</div>";
            }
            return resp;
        }

       
    }
}