using System;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Auth : MainPage
    {
        protected string _loginMessage;
        protected string _login;
        protected string _password;
        protected TenantInfoSettings _tenantInfoSettings;

        protected string _confirmedEmail;

        protected void Page_Load(object sender, EventArgs e)
        {
            _login = "";
            _password = "";

            //Account link control
            AccountLinkControl accountLink = null;
            if (SetupInfo.ThirdPartyAuthEnabled)
            {
                accountLink = (AccountLinkControl)LoadControl(AccountLinkControl.Location);
                associateAccount.Visible = true;
                associateAccount.Text = Resources.Resource.LoginWithAccount;
                accountLink.ClientCallback = "authCallback";
                accountLink.SettingsView = false;
                signInPlaceholder.Controls.Add(accountLink);
            }

            ((IStudioMaster)this.Master).DisabledSidePanel = true;

            //top panel
            if (this.Master is StudioTemplate)
            {
                ((StudioTemplate)this.Master).TopNavigationPanel.DisableProductNavigation = true;
                ((StudioTemplate)this.Master).TopNavigationPanel.DisableSearch = true;
            }

            _tenantInfoSettings = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID);

            this.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Authorization, null, null);

            pwdReminderHolder.Controls.Add(LoadControl(PwdTool.Location));
            pwdReminderHolder.Controls.Add(LoadControl(InviteEmployeeControl.Location));
            _communitations.Controls.Add(LoadControl(AuthCommunications.Location));

            var msg = Request["m"];
            if (!string.IsNullOrEmpty(msg))
            {
                _loginMessage = "<div class='errorBox'>" + HttpUtility.HtmlEncode(msg) + "</div>";
            }

            if (this.IsPostBack && !SecurityContext.IsAuthenticated)
            {
                var uData = new UserTransferData();

                if (!String.IsNullOrEmpty(Request["login"]))
                {
                    _login = Request["login"];
                    uData.Login = _login;
                }

                if (!String.IsNullOrEmpty(Request["pwd"]))
                {
                    _password = Request["pwd"];
                    uData.Password = _password;
                }

                bool isDemo = false;
                if (!String.IsNullOrEmpty(Request["authtype"]))
                    isDemo = Request["authtype"] == "demo";

                string hashId = string.Empty;
                if (!string.IsNullOrEmpty(Request["__EVENTARGUMENT"]) && Request["__EVENTTARGET"] == "signInLogin" && accountLink != null)
                {
                    //Login from open id
                    hashId = Request["__EVENTARGUMENT"];
                    uData.HashId = hashId;
                }

                if (isDemo)
                {
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.Demo);
                }
                else
                {
                    try
                    {
                        string cookiesKey = string.Empty;
                        if (!string.IsNullOrEmpty(hashId))
                        {
                            var accounts = accountLink.GetLinker().GetLinkedObjectsByHashId(hashId);

                            foreach (var account in accounts.Select(x =>
                            {
                                try
                                {
                                    return new Guid(x);
                                }
                                catch
                                {
                                    return Guid.Empty;
                                }
                            }))
                            {
                                if (CoreContext.UserManager.UserExists(account) && account != Guid.Empty)
                                {
                                    var coreAcc = CoreContext.UserManager.GetUsers(account);
                                    cookiesKey = SecurityContext.AuthenticateMe(coreAcc.Email, CoreContext.Authentication.GetUserPasswordHash(coreAcc.ID));
                                    uData.UserId = coreAcc.ID;
                                    ProcessSmsValidation(uData);
                                }
                            }
                            if (string.IsNullOrEmpty(cookiesKey))
                            {
                                _loginMessage = "<div class=\"errorBox\">" + HttpUtility.HtmlEncode(Resources.Resource.LoginWithAccountNotFound) + "</div>";
                                return;
                            }
                        }
                        else
                        {

                            cookiesKey = SecurityContext.AuthenticateMe(_login, _password);
                            uData.UserId = SecurityContext.CurrentAccount.ID;
                            ProcessSmsValidation(uData);
                        }

                        CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                    }
                    catch (System.Security.SecurityException)
                    {
                        ProcessLogout();
                        _loginMessage = "<div class=\"errorBox\">" + HttpUtility.HtmlEncode(Resources.Resource.InvalidUsernameOrPassword) + "</div>";
                        return;
                    }
                    catch (Exception exception)
                    {
                        ProcessLogout();
                        _loginMessage = "<div class=\"errorBox\">" + HttpUtility.HtmlEncode(exception.Message) + "</div>";
                        return;
                    }
                }

                UserOnlineManager.Instance.RegistryOnlineUser(SecurityContext.CurrentAccount.ID);

                WebItemManager.Instance.ItemGlobalHandlers.Login(SecurityContext.CurrentAccount.ID);

                string refererURL = (string)Session["refererURL"];
                if (String.IsNullOrEmpty(refererURL))
                    Response.Redirect("~/");
                else
                {
                    Session["refererURL"] = null;
                    Response.Redirect(refererURL);
                }

                return;

            }
            else if (SecurityContext.IsAuthenticated && base.IsLogout)
            {
                ProcessLogout();
                Response.Redirect("~/auth.aspx");
            }

            ProcessConfirmedEmailCondition();
        }

        void ProcessLogout()
        {
            try
            {
                WebItemManager.Instance.ItemGlobalHandlers.Logout(SecurityContext.CurrentAccount.ID);

            }
            finally
            {
                //logout
                UserOnlineManager.Instance.UnRegistryOnlineUser(SecurityContext.CurrentAccount.ID);

                if (!SecurityContext.DemoMode)
                    CookiesManager.ClearCookies(CookiesType.AuthKey);

                SecurityContext.Logout();
            }
        }

        private void ProcessSmsValidation(UserTransferData uData)
        {
            var smsAuthSettings = SettingsManager.Instance.LoadSettings<StudioSmsNotificationSettings>(TenantProvider.CurrentTenantID);
            if (smsAuthSettings.Enable && SetupInfo.IsVisibleSettings<StudioSmsNotificationSettings>())
            {
                var confKey = CookiesManager.GetCookies(CookiesType.ConfKey);
                var activated = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).MobilePhoneActivationStatus;

                if (!String.IsNullOrEmpty(confKey) && EmailValidationKeyProvider.ValidateEmailKey(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email, confKey, TimeSpan.FromDays(30)) == EmailValidationKeyProvider.ValidationResult.Ok)
                    return;

                uData.MobilePhoneActivationStatus = activated;
                uData.ValidationKey = EmailValidationKeyProvider.GetEmailKey(GetEmailKey(uData,activated));
                Session["UserTransferData"] = uData;
                ProcessLogout();
                Response.Redirect(String.Format("~/Confirm.aspx?type={0}", activated == MobilePhoneActivationStatus.Activated ? ConfirmType.PhoneAuth : ConfirmType.PhoneActivation));
            }
        }

        private string GetEmailKey(UserTransferData uData, MobilePhoneActivationStatus activated)
        {
            return (uData.HashId ?? uData.Login) + (activated == MobilePhoneActivationStatus.Activated ? ConfirmType.PhoneAuth : ConfirmType.PhoneActivation).ToString().ToLower();
        }


        private void ProcessConfirmedEmailCondition()
        {
            if (!IsPostBack)
            {
                var confirmedEmail = Request.QueryString["confirmed-email"];

                if (!String.IsNullOrEmpty(confirmedEmail) && confirmedEmail.TestEmailRegex())
                {
                    _login = confirmedEmail;
                    _loginMessage = String.Format("{0}<br />{1}", Resources.Resource.MessageEmailConfirmed, Resources.Resource.MessageAuthorize);
                }
            }
        }
    }
}
