using System;
using System.Web;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Studio
{
    //  emp-invite - confirm ivite by email
    //  portal-suspend - confirm portal suspending - Tenant.SetStatus(TenantStatus.Suspended)
    //  portal-continue - confirm portal continuation  - Tenant.SetStatus(TenantStatus.Active)
    //  portal-remove - confirm portal deletation - Tenant.SetStatus(TenantStatus.RemovePending)
    //  DnsChange - change Portal Address and/or Custom domain name
    public enum ConfirmType
    {
        EmpInvite,
        LinkInvite,
        PortalSuspend,
        PortalContinue,
        PortalRemove,
        DnsChange,
        PortalOwnerChange,
        Activation,
        EmailChange,
        EmailActivation,
        PasswordChange,
        ProfileRemove,
        PhoneActivation,
        PhoneAuth,
        PhoneChange
    }

    public partial class confirm : MainPage
    {
        protected TenantInfoSettings _tenantInfoSettings;

        public string ErrorMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((IStudioMaster)this.Master).DisabledSidePanel = true;
            UserInfo _user = null;

            this.Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.AccountControlPageTitle, null, null);

            var type = typeof(ConfirmType).TryParseEnum<ConfirmType>(Request["type"] ?? "", ConfirmType.EmpInvite);
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableProductNavigation = true;
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableUserInfo = true;
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableSearch = true;

            if (type == ConfirmType.Activation || type == ConfirmType.EmpInvite)
                ((StudioTemplate)this.Master).TopNavigationPanel.CustomTitle = Resources.Resource.JoinTitle;

            _tenantInfoSettings = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID);

            var email = Request["email"] ?? "";
            var key = Request["key"] ?? "";
            var fap = Request["fap"] ?? "";


            var validInterval = SetupInfo.ValidEamilKeyInterval;
            var checkKeyResult = EmailValidationKeyProvider.ValidationResult.Invalid;

            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            if (tenant.Status != TenantStatus.Active && type != ConfirmType.PortalContinue)
            {
                Response.Redirect(SetupInfo.NoTenantRedirectURL, true);
            }

            if (type == ConfirmType.DnsChange)
            {
                var dnsChangeKey = string.Join(string.Empty, new string[] { email.ToLower(), type.ToString().ToLower(), Request["dns"], Request["alias"] });
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(dnsChangeKey, key, validInterval);
            }
            else if (type == ConfirmType.PortalContinue)
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower(), key);
            }
            else if ((type == ConfirmType.EmpInvite || type == ConfirmType.Activation) && !String.IsNullOrEmpty(fap) && String.Equals(fap, "1"))
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower() + "allrights", key, validInterval);
            }
            else if (type == ConfirmType.PasswordChange)
            {
                //Check activation signature
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower(), key, validInterval);
            }
            else if (type == ConfirmType.PortalOwnerChange && !String.IsNullOrEmpty(Request["uid"]))
            {
                Guid uid = Guid.Empty;
                try
                {
                    uid = new Guid(Request["uid"]);
                }
                catch { }
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower() + uid.ToString(), key, validInterval);
            }
            else if (type == ConfirmType.ProfileRemove && !(String.IsNullOrEmpty(Request["email"]) || String.IsNullOrEmpty(Request["key"])))
            {
                _user = CoreContext.UserManager.GetUserByEmail(email);

                if (_user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID)) return;

                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower(), key, validInterval);
            }
            else if (type == ConfirmType.EmpInvite && String.IsNullOrEmpty(email))
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower(), key, TimeSpan.FromDays(3));
            }
            else if (type == ConfirmType.PhoneActivation || type == ConfirmType.PhoneAuth)
            {
                UserTransferData obj;
                if (Context.Session["UserTransferData"] != null)
                {
                    obj = (Context.Session["UserTransferData"] as UserTransferData);
                    key = obj.ValidationKey;
                }
                else
                {
                    obj = new UserTransferData { Login = email };
                }
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey((obj.HashId ?? obj.Login) + type.ToString().ToLower(), key, TimeSpan.FromDays(3));
            }
            else if (type == ConfirmType.PhoneChange)
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower(), key, TimeSpan.FromDays(3));
            }
            else
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString().ToLower(), key, validInterval);
            }

            if (((!email.TestEmailRegex() && !(type == ConfirmType.PhoneActivation || type == ConfirmType.PhoneAuth)) || checkKeyResult != EmailValidationKeyProvider.ValidationResult.Ok) && type != ConfirmType.LinkInvite)
            {
                ShowError(Resources.Resource.ErrorConfirmURLError);
                return;
            }

            if (!email.TestEmailRegex() && !(type == ConfirmType.LinkInvite || type == ConfirmType.PhoneActivation || type == ConfirmType.PhoneAuth))
            {
                ShowError(Resources.Resource.ErrorNotCorrectEmail);
                return;
            }

            if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Invalid)
            {
                //If check failed
                ShowError(Resources.Resource.ErrorInvalidActivationLink);
                return;
            }
            if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Expired)
            {
                //If link expired
                ShowError(Resources.Resource.ErrorExpiredActivationLink);
                return;
            }

            switch (type)
            {
                //Invite
                case ConfirmType.EmpInvite:
                case ConfirmType.LinkInvite:
                case ConfirmType.Activation:
                    _confirmHolder2.Controls.Add(LoadControl(ConfirmInviteActivation.Location));
                    _contentWithControl.Visible = false;
                    break;

                case ConfirmType.EmailChange:
                case ConfirmType.PasswordChange:
                    _confirmHolder.Controls.Add(LoadControl(ConfirmActivation.Location));
                    break;

                case ConfirmType.EmailActivation:
                    ProcessEmailActivation(email);
                    break;

                case ConfirmType.PortalRemove:
                case ConfirmType.PortalSuspend:
                case ConfirmType.PortalContinue:
                case ConfirmType.DnsChange:
                    _confirmHolder.Controls.Add(LoadControl(ConfirmPortalActivity.Location));
                    break;

                case ConfirmType.PortalOwnerChange:
                    _confirmHolder.Controls.Add(LoadControl(ConfirmPortalOwner.Location));
                    break;

                case ConfirmType.ProfileRemove:
                    var control = (ProfileOperation)LoadControl(ProfileOperation.Location);
                    control.Key = key;
                    control.Email = email;
                    control.User = _user;
                    _confirmHolder.Controls.Add(control);
                    break;

                case ConfirmType.PhoneActivation:
                case ConfirmType.PhoneChange:
                    var authControl = (ConfirmMobileActivation)LoadControl(ConfirmMobileActivation.Location);
                    authControl.Activate = true;
                    _confirmHolder.Controls.Add(authControl);
                    break;

                case ConfirmType.PhoneAuth:
                    var authControl1 = (ConfirmMobileActivation)LoadControl(ConfirmMobileActivation.Location);
                    authControl1.Activate = false;
                    _confirmHolder.Controls.Add(authControl1);
                    break;
            }
        }

        private void ProcessEmailActivation(string email)
        {
            UserInfo user = CoreContext.UserManager.GetUserByEmail(email);

            if (user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
            {
                ShowError(Resources.Resource.ErrorConfirmURLError);
            }
            else if (user.ActivationStatus == EmployeeActivationStatus.Activated)
            {
                Response.Redirect("~/");
            }
            else
            {
                try
                {
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    CoreContext.UserManager.SaveUserInfo(user);
                }
                finally
                {
                    SecurityContext.Logout();
                    CookiesManager.ClearCookies(CookiesType.AuthKey);
                }

                string redirectUrl = String.Format("~/auth.aspx?confirmed-email={0}", email);
                Response.Redirect(redirectUrl, true);
            }
        }

        private void ShowError(string error)
        {
            if (SecurityContext.IsAuthenticated)
            {
                ErrorMessage = error;
                _confirmHolder.Visible = false;
            }
            else
            {
                Response.Redirect(string.Format("~/auth.aspx?m={0}", HttpUtility.UrlEncode(error)));
            }
        }
    }
}
