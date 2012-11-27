using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;
using ASC.Web.Controls;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxPro.AjaxNamespace("Backup")]
    public partial class Backup : System.Web.UI.UserControl
    {        
        public static string Location { get { return "~/UserControls/Management/Backup.ascx"; } }

        public string Url
        {
            get
            {
                var url = Request.GetUrlRewriter().OriginalString;
                var i = url.LastIndexOf("/");
                if (i > 0)
                    return url.Substring(0, i);
                else return "";
            }
        }

        protected bool EnableBackup
        {
            get
            {
                return (CoreContext.TenantManager.GetTenantQuota(CoreContext.TenantManager.GetCurrentTenant().TenantId).HasBackup);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(Backup), this.Page);     
       
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public string Deactivate(bool flag)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var t = CoreContext.TenantManager.GetCurrentTenant();
            if (t != null)
            {
                SendMailDeactivate(t);
                var u = CoreContext.UserManager.GetUsers(t.OwnerId);
                var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", u.Email);
                return ((string)Resources.Resource.AccountDeactivationMsg).Replace(":email", emailLink);
            }
            return string.Empty;
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public string Delete(bool flag)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var t = CoreContext.TenantManager.GetCurrentTenant();
            if (t != null)
            {
                SendMailDelete(t);
                var u = CoreContext.UserManager.GetUsers(t.OwnerId);
                var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", u.Email);
                return ((string)Resources.Resource.AccountDeletionMsg).Replace(":email", emailLink);
            }
            return string.Empty;
        }

        private void SendMailDeactivate(Tenant t)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            StudioNotifyService.Instance.SendMsgPortalDeactivation(t,
                GetConfirmLink(u.Email, ConfirmType.PortalSuspend),
                GetConfirmLink(u.Email, ConfirmType.PortalContinue));
        }

        private void SendMailDelete(Tenant t)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            StudioNotifyService.Instance.SendMsgPortalDeletion(t,
                GetConfirmLink(u.Email, ConfirmType.PortalRemove));
        }

        private string GetConfirmLink(string email, ConfirmType confirmType)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email.ToLower() + confirmType.ToString().ToLower());

            return CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx") +
                string.Format("?type={0}&email={1}&key={2}", confirmType.ToString().ToLower(), HttpUtility.UrlEncode(email), validationKey); ;
        }
    }
}