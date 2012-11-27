using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Web.Studio.Core.Notify;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Users
{
    [AjaxNamespace("InviteResender")]
    public partial class ResendInvitesControl : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/usercontrols/users/resendinvitescontrol/resendinvitescontrol.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            _invitesResenderContainer.Options.IsPopup = true;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object Resend()
        {
            try
            {
                foreach (var user in new List<UserInfo>(CoreContext.UserManager.GetUsers())
                                    .FindAll(u => u.ActivationStatus == EmployeeActivationStatus.Pending))
                {
                    StudioNotifyService.Instance.UserInfoActivation(user);
                }

                return new { status = 1, message = Resources.Resource.SuccessResendInvitesText };
            }
            catch (Exception e)
            {
                return new {status=0, message = e.Message.HtmlEncode()};
            }
        }

        public static string GetHrefAction()
        {
            return "javascript:InvitesResender.Show();";
        }
    }
}