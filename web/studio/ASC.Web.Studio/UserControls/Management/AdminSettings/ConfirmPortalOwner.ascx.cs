using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class ConfirmPortalOwner : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/AdminSettings/ConfirmPortalOwner.ascx"; } }
        protected string _newOwnerName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ownerconfirm_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/adminsettings/css/<theme_folder>/ownerconfirm.css") + "\">", false);

            Guid uid = Guid.Empty;
            try
            {
                uid = new Guid(Request["uid"]);
            }
            catch { };
            _newOwnerName = CoreContext.Authentication.GetAccountByID(uid).Name;

            if (IsPostBack)
            {


                var curTenant = CoreContext.TenantManager.GetCurrentTenant();
                curTenant.OwnerId = uid;

                bool authed = false;
                try
                {
                    if (!SecurityContext.IsAuthenticated)
                    {
                        SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                        authed = true;
                    }

                    CoreContext.TenantManager.SaveTenant(curTenant);
                }
                finally
                {
                    if (authed) SecurityContext.Logout();
                }

                _messageHolder.Visible = true;
                _confirmContentHolder.Visible = false;
            }
            else
            {
               
                _messageHolder.Visible = false;
                _confirmContentHolder.Visible = true;
            }
        }
       
    }
}