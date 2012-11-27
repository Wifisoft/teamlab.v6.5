using System;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class GreetingSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/GreetingSettings/GreetingSettings.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "greetingsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/greetingsettings/css/<theme_folder>/greetingsettings.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "greetingsettings_script", WebPath.GetPath("usercontrols/management/greetingsettings/js/greetingsettings.js"));

            content.Controls.Add(LoadControl(GreetingSettingsContent.Location));
        }
    }
}