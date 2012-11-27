using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    [AjaxNamespace("CustomizationSettings")]
    public partial class Customization : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/Customization/Customization.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "cust_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/firsttime/customization/css/customization.css") + "\">", false);

            _dateandtimeHolder.Controls.Add(LoadControl(TimeAndLanguage.Location));

            _namingPeopleHolder.Controls.Add(LoadControl(NamingPeopleSettingsContent.Location));

            _schemaHolder.Controls.Add(LoadControl(SkinSettingsContent.Location));
        }
    }
}