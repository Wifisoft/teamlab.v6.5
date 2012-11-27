using System;
using System.Web.UI;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("SkinSettingsController")]
    public partial class SkinSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/SkinSettings/SkinSettings.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "skinsettings_script", WebPath.GetPath("usercontrols/management/skinsettings/js/skinsettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "skinsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/skinsettings/css/<theme_folder>/skinsettings.css") + "\">", false);
  
            content.Controls.Add(LoadControl(SkinSettingsContent.Location));
        }
    }
}