using System;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Core.Security.Ajax;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using System.Net.Mail;

namespace ASC.Web.Studio.UserControls
{
    [AjaxNamespace("AuthCommunicationsController")]
    public partial class AuthCommunications : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/AuthCommunications.ascx"; }
        }

        public bool MaxHighAdmMess { get; set; }

        protected bool EnabledJoin
        {
            get
            {
                var t = CoreContext.TenantManager.GetCurrentTenant();
                if ((t.TrustedDomainsType == TenantTrustedDomainsType.Custom && t.TrustedDomains.Count > 0) ||
                    t.TrustedDomainsType == TenantTrustedDomainsType.All)
                    return true;
                return false;
            }
        }

        protected bool EnableAdmMess
        {
            get
            {
                var setting = SettingsManager.Instance.LoadSettings<StudioAdminMessageSettings>(TenantProvider.CurrentTenantID);
                return setting.Enable;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MaxHighAdmMess)
            {
                _joinBlock.Visible = false;
                _sendAdmin.Visible = true;
            }
            else
            {
                _joinBlock.Visible = EnabledJoin;

                _sendAdmin.Visible = EnableAdmMess;
            }
            if (EnabledJoin || EnableAdmMess)
                AjaxPro.Utility.RegisterTypeForAjax(GetType());
        }

        [AjaxSecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SendAdmMail(string email, string message)
        {
            try
            {
                StudioNotifyService.Instance.SendMsgToAdminFromNotAuthUser(email, message);
                return new { Status = 1, Message = Resources.Resource.AdminMessageSent };
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message.HtmlEncode() };
            }
        }

        [AjaxSecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SendJoinInviteMail(string email)
        {

            email = (email ?? "").Trim();
            AjaxResponse resp = new AjaxResponse();
            resp.rs1 = "0";

            try
            {
                if (!email.TestEmailRegex())
                    resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;

                var user = CoreContext.UserManager.GetUserByEmail(email);
                if (!user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    resp.rs1 = "0";
                    resp.rs2 = CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists").HtmlEncode();
                    return resp;
                }

                var tenant = CoreContext.TenantManager.GetCurrentTenant();

                if (tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom)
                {
                    var address = new MailAddress(email);
                    foreach (var d in tenant.TrustedDomains)
                    {
                        if (address.Address.EndsWith("@" + d, StringComparison.InvariantCultureIgnoreCase))
                        {
                            StudioNotifyService.Instance.InviteUsers(email, "", true, false);
                            resp.rs1 = "1";
                            resp.rs2 = Resources.Resource.FinishInviteJoinEmailMessage;
                            return resp;
                        }
                    }
                }
                else if (tenant.TrustedDomainsType == TenantTrustedDomainsType.All)
                {
                    StudioNotifyService.Instance.InviteUsers(email, "", true, false);
                    resp.rs1 = "1";
                    resp.rs2 = Resources.Resource.FinishInviteJoinEmailMessage;
                    return resp;
                }

                resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;
            }
            catch (FormatException)
            {
                resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;
            }
            catch (Exception e)
            {
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        }

        public static string RenderTrustedDominTitle()
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            if (tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom)
            {
                var domains = String.Empty;
                var i = 0;
                foreach (var d in tenant.TrustedDomains)
                {
                    if (i != 0)
                        domains += ", ";

                    domains += d;
                    i++;
                }
                return String.Format(Resources.Resource.TrustedDomainsInviteTitle, domains);
            }
            else if (tenant.TrustedDomainsType == TenantTrustedDomainsType.All)
                return Resources.Resource.SignInFromAnyDomainInviteTitle;

            return "";
        }
    }
}