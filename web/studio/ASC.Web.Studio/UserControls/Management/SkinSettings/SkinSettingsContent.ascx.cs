using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Core;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("SkinSettingsController")]
    public partial class SkinSettingsContent : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/SkinSettings/SkinSettingsContent.ascx"; } }

        protected WebSkin _currentSkin;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "skinsettingscontent_script", WebPath.GetPath("usercontrols/management/skinsettings/js/skinsettingscontent.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "skinsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/skinsettings/css/<theme_folder>/skinsettings.css") + "\">", false);

            //Skin Settings
            var type = Request["type"] ?? "";

            if (type != "customization")
            {
                var settings = SettingsManager.Instance.LoadSettings<WebSkinSettings>(TenantProvider.CurrentTenantID);
                _currentSkin = settings.WebSkin;
            }
            else
            {
                _currentSkin = WebSkin.GetUserSkin();
            }

            var items = new List<object>();
            foreach (var s in WebSkin.Skins)
            {
                items.Add(new
                {
                    Id = s.ID,
                    Name = s.DisplayName,
                    Checked = string.Equals(s.ID, _currentSkin.ID, StringComparison.InvariantCultureIgnoreCase),
                    Folder = s.FolderName,
                    Path = WebImageSupplier.GetAbsoluteWebPath("skins/" + s.ID + ".png")
                });
            }

            skinRepeater.DataSource = items;
            skinRepeater.DataBind();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveSkinSettings(string skinID)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var skin = WebSkin.Skins.Find(s => string.Equals(s.ID, skinID, StringComparison.InvariantCultureIgnoreCase));

                if (skin != null)
                {
                    var settings = SettingsManager.Instance.LoadSettings<WebSkinSettings>(TenantProvider.CurrentTenantID);
                    settings.WebSkin = skin;
                    settings.IsDefault = false;
                    SettingsManager.Instance.SaveSettings<WebSkinSettings>(settings, TenantProvider.CurrentTenantID);
                }
                return new { Status = 1,Message="Saved" };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }
    }
}