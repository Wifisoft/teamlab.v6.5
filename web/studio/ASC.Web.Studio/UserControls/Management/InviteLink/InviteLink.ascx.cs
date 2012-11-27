using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Security.Cryptography;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class InviteLink : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/InviteLink/InviteLink.ascx"; } }
        protected string _generatedLink;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "invite_link_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/InviteLink/css/InviteLink.css") + "\">", false);
            var validationKey = EmailValidationKeyProvider.GetEmailKey(
                    ConfirmType.LinkInvite.ToString().ToLower());

            _generatedLink = CommonLinkUtility.GetFullAbsolutePath(String.Format("~/confirm.aspx?type={0}&key={1}&uid={2}", ConfirmType.LinkInvite, validationKey, SecurityContext.CurrentAccount.ID));
        }
    }
}