using System;
using System.Web;
using ASC.Web.Core.Security.Ajax;
using AjaxPro;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Users;
using ASC.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("MySettings")]
    public partial class PwdTool : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/PwdTool.ascx"; } }

        public Guid UserID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {   
            _pwdRemainderContainer.Options.IsPopup = true;
            _pwdRemainderContainer.Options.InfoMessageText = "";
            _pwdRemainderContainer.Options.InfoType = ASC.Web.Controls.InfoType.Info;

            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
        }

        [AjaxSecurityPassthroughAttribute]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse RemaindPwd(string email)
        {
            AjaxResponse responce = new AjaxResponse();
            responce.rs1 = "0";

            if (!email.TestEmailRegex())
            {
                responce.rs2 = "<div>" + Resources.Resource.ErrorNotCorrectEmail + "</div>";
                return responce;
            }

            try
            {
                UserManagerWrapper.SendUserPassword(email);

                responce.rs1 = "1";
                responce.rs2 = String.Format(Resources.Resource.MessageYourPasswordSuccessfullySendedToEmail, email);
            }
            catch (Exception exc)
            {
                responce.rs2 = "<div>" + HttpUtility.HtmlEncode(exc.Message) + "</div>";
            }

            return responce;
        }
    }
}