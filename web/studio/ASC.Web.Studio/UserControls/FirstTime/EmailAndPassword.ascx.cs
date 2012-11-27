using System;
using System.Collections.Specialized;
using System.Net;
using System.Web.Configuration;
using System.Web.UI;
using AjaxPro;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    [AjaxNamespace("EmailAndPasswordController")]
    public partial class EmailAndPassword : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/EmailAndPassword.ascx"; } }

        protected Tenant _curTenant;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "firsttime1_script", WebPath.GetPath("usercontrols/firsttime/js/manager.js"));

            _curTenant = CoreContext.TenantManager.GetCurrentTenant();
        }

        [AjaxMethod]
        public object SaveData(string email, string pwd, string header)
        {
            try
            {
                var currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

                if (!currentUser.IsOwner())
                {
                    return new { Status = 0, Message = Resources.Resource.EmailAndPasswordNotOwner };
                }

                if (!UserManagerWrapper.ValidateEmail(email))
                {
                    return new { Status = 0, Message = Resources.Resource.EmailAndPasswordIncorrectEmail };
                }

                UserManagerWrapper.SetUserPassword(currentUser.ID, pwd);

                if (currentUser.Email != email)
                {
                    currentUser.Email = email;
                    currentUser.ActivationStatus = EmployeeActivationStatus.NotActivated;
                }
                CoreContext.UserManager.SaveUserInfo(currentUser);

                if (currentUser.ActivationStatus == EmployeeActivationStatus.NotActivated)
                    StudioNotifyService.Instance.SendEmailActivationInstructions(currentUser, email);

                var newCookie = SecurityContext.AuthenticateMe(email, CoreContext.Authentication.GetUserPasswordHash(currentUser.ID));
                CookiesManager.SetCookies(CookiesType.AuthKey, newCookie);

                var wizardSettings =
                    SettingsManager.Instance.LoadSettings<WizardSettings>(TenantProvider.CurrentTenantID);
                wizardSettings.Completed = true;
                SettingsManager.Instance.SaveSettings(wizardSettings, TenantProvider.CurrentTenantID);

                var currentTenant = CoreContext.TenantManager.GetCurrentTenant();
                currentTenant.Name = header;
                CoreContext.TenantManager.SaveTenant(currentTenant);

                SendInstallInfo(currentUser);

                return new { Status = 1, Message = Resources.Resource.EmailAndPasswordSaved };
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }

        public string GetEmail()
        {
            var currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            return currentUser.Email;
        }

        private void SendInstallInfo(UserInfo user)
        {
            try
            {
                StudioNotifyService.Instance.SendCongratulations(user);
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }
            try
            {
                var url = WebConfigurationManager.AppSettings["web.install-url"];
                if (!string.IsNullOrEmpty(url))
                {
                    var q = new MailQuery { Email = user.Email };
                    var index = url.IndexOf("?v=");
                    if (0 < index)
                    {
                        q.Version = url.Substring(index + 3);
                        url = url.Substring(0, index);
                    }
                    using (var webClient = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values.Add("query", Signature.Create<MailQuery>(q, "4be71393-0c90-41bf-b641-a8d9523fba5c"));
                        webClient.UploadValues(url, values);
                    }
                }
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }
        }

        private class MailQuery
        {
            public string Email { get; set; }
            public string Version { get; set; }
        }
    }
}