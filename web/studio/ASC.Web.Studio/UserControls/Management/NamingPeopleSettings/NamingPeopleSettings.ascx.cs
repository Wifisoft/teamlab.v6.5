using System;
using System.Collections.Generic;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Users;
using ASC.Core;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class NamingPeopleSettings : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/NamingPeopleSettings/NamingPeopleSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            Page.ClientScript.RegisterClientScriptInclude(typeof (string), "peoplename_script", WebPath.GetPath("usercontrols/management/namingpeoplesettings/js/namingpeople.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "namingpeople_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/namingpeoplesettings/css/<theme_folder>/namingpeople.css") + "\">", false);

            content.Controls.Add(LoadControl(NamingPeopleSettingsContent.Location));
        }
    }
}