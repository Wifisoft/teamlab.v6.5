using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    public partial class HowTo : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/HowTo/HowTo.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "howto_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/firsttime/howto/css/<theme_folder>/howto.css") + "\">", false);
        }
    }
}