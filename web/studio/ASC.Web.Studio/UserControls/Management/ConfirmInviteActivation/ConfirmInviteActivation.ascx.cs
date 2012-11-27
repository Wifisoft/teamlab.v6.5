using System;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class ConfirmInviteActivation : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/ConfirmInviteActivation/ConfirmInviteActivation.ascx"; } }

        protected string _errorMessage;

        protected TenantInfoSettings _tenantInfoSettings;
        protected string _userName;
        protected string _userPost;
        protected string _userAvatar;

        protected string _email { get { return (Request["email"] ?? String.Empty).Trim(); } }
        protected string _firstName { get { return (Request["firstname"] ?? String.Empty).Trim(); } }
        protected string _lastName { get { return (Request["lastname"] ?? String.Empty).Trim(); } }
        protected string _pwd { get { return (Request["pwd"] ?? "").Trim(); } }
        protected string _rePwd { get { return (Request["repwd"] ?? "").Trim(); } }

        protected string GetEmailAddress()
        {
            if (!String.IsNullOrEmpty(Request["emailInput"]))
                return Request["emailInput"];

            if (!String.IsNullOrEmpty(Request["email"]))
                return Request["email"];

            return String.Empty;
        }

        private string GetEmailAddress(LoginProfile account)
        {
            var value = GetEmailAddress();
            return String.IsNullOrEmpty(value) ? account.EMail : value;
        }

        protected string GetFirstName()
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(Request["firstnameInput"])) value = Request["firstnameInput"];
            if (!string.IsNullOrEmpty(Request["firstname"])) value = Request["firstname"];
            return HttpUtility.HtmlEncode(value);
        }

        private string GetFirstName(LoginProfile account)
        {
            var value = GetFirstName();
            return String.IsNullOrEmpty(value) ? account.FirstName : value;
        }

        protected string GetLastName()
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(Request["lastnameInput"])) value = Request["lastnameInput"];
            if (!string.IsNullOrEmpty(Request["lastname"])) value = Request["lastname"];
            return HttpUtility.HtmlEncode(value);
        }

        private string GetLastName(LoginProfile account)
        {
            var value = GetLastName();
            return String.IsNullOrEmpty(value) ? account.LastName : value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "confirm_invite_activation_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/confirminviteactivation/css/<theme_folder>/confirm_invite_activation.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "confirm_invite_activation_script", WebPath.GetPath("usercontrols/management/confirminviteactivation/js/confirm_invite_activation.js"));

            _tenantInfoSettings = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID);

            Guid uid = Guid.Empty;
            try
            {
                uid = new Guid(Request["uid"]);
            }
            catch { }

            var type = typeof(ConfirmType).TryParseEnum(Request["type"] ?? "", ConfirmType.EmpInvite);

            var email = GetEmailAddress();
            var key = Request["key"] ?? "";
            var fap = Request["fap"] ?? "";

            //if (!string.IsNullOrEmpty(_email))
            //{
            //var thrd = LoadControl(AccountLinkControl.Location) as AccountLinkControl;
            //thrd.InviteView = true;
            //thrd.ClientCallback = "loginJoinCallback";
            //thrdParty.Controls.Add(thrd);
            //}
            Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Authorization, null, null);

            UserInfo user;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

                user = CoreContext.UserManager.GetUserByEmail(email);
                var usr = CoreContext.UserManager.GetUsers(uid);
                if (usr.ID.Equals(ASC.Core.Users.Constants.LostUser.ID) || usr.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID))
                    usr = CoreContext.UserManager.GetUsers(CoreContext.TenantManager.GetCurrentTenant().OwnerId);

                _userAvatar = usr.GetMediumPhotoURL();
                _userName = usr.DisplayUserName(true);
                _userPost = (usr.Title ?? "").HtmlEncode();
            }
            finally
            {
                SecurityContext.Logout();
            }

            if (type == ConfirmType.LinkInvite || type == ConfirmType.EmpInvite)
            {
                if (!user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    ShowError(CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists"));
                    return;
                }
            }

            else if (type == ConfirmType.Activation)
            {
                if (user.IsActive)
                {
                    ShowError(Resources.Resource.ErrorConfirmURLError);
                    return;
                }

                if (user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    ShowError(string.Format(Resources.Resource.ErrorUserNotFoundByEmail, email));
                    return;
                }
            }

            if (!IsPostBack)
                return;

            var firstName = GetFirstName();
            var lastName = GetLastName();
            var pwd = (Request["pwdInput"] ?? "").Trim();
            var repwd = (Request["repwdInput"] ?? "").Trim();
            LoginProfile thirdPartyProfile;

            //thirdPartyLogin confirmInvite
            if (Request["__EVENTTARGET"] == "thirdPartyLogin")
            {
                var valueRequest = Request["__EVENTARGUMENT"];
                thirdPartyProfile = new LoginProfile(valueRequest);

                if (!string.IsNullOrEmpty(thirdPartyProfile.AuthorizationError))
                {
                    // ignore cancellation
                    if (thirdPartyProfile.AuthorizationError != "Canceled at provider")
                        ShowError(HttpUtility.HtmlEncode(thirdPartyProfile.AuthorizationError));
                    return;
                }

                if (string.IsNullOrEmpty(thirdPartyProfile.EMail))
                {
                    ShowError(HttpUtility.HtmlEncode(Resources.Resource.ErrorNotCorrectEmail));
                    return;
                }
            }

            if (Request["__EVENTTARGET"] == "confirmInvite")
            {
                if (String.IsNullOrEmpty(email))
                {
                    _errorMessage = Resources.Resource.ErrorEmptyUserEmail;
                    return;
                }

                if (!email.TestEmailRegex())
                {
                    _errorMessage = Resources.Resource.ErrorNotCorrectEmail;
                    return;
                }

                if (String.IsNullOrEmpty(firstName))
                {
                    _errorMessage = Resources.Resource.ErrorEmptyUserFirstName;
                    return;
                }

                if (String.IsNullOrEmpty(lastName))
                {
                    _errorMessage = Resources.Resource.ErrorEmptyUserLastName;
                    return;
                }

                var checkPassResult = CheckPassword(pwd, repwd);
                if (!String.IsNullOrEmpty(checkPassResult))
                {
                    _errorMessage = checkPassResult;
                    return;
                }
            }
            var userID = Guid.Empty;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                if (type == ConfirmType.EmpInvite || type == ConfirmType.LinkInvite)
                {
                    UserInfo newUser;
                    if (Request["__EVENTTARGET"] == "confirmInvite")
                    {
                        newUser = CreateNewUser(firstName, lastName, email, pwd);
                        userID = newUser.ID;
                    }

                    if (Request["__EVENTTARGET"] == "thirdPartyLogin")
                    {
                        if (!String.IsNullOrEmpty(CheckPassword(pwd, repwd)))
                            pwd = UserManagerWrapper.GeneratePassword();
                        var valueRequest = Request["__EVENTARGUMENT"];
                        thirdPartyProfile = new LoginProfile(valueRequest);
                        newUser = CreateNewUser(GetFirstName(thirdPartyProfile), GetLastName(thirdPartyProfile), GetEmailAddress(thirdPartyProfile), pwd);
                        userID = newUser.ID;

                        var linker = new AccountLinker(WebConfigurationManager.ConnectionStrings["webstudio"]);
                        linker.AddLink(userID.ToString(), thirdPartyProfile);
                    }

                    #region Department

                    try
                    {
                        var deptID = new Guid((Request["deptID"] ?? "").Trim());
                        CoreContext.UserManager.AddUserIntoGroup(userID, deptID);
                    }
                    catch
                    {
                    }

                    #endregion
                }
                else if (type == ConfirmType.Activation)
                {
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    CoreContext.UserManager.SaveUserInfo(user);
                    SecurityContext.SetUserPassword(user.ID, pwd);

                    userID = user.ID;

                    //notify
                    StudioNotifyService.Instance.UserInfoAddedAfterInvite(user, pwd);
                }

                if (String.Equals(fap, "1"))
                    CoreContext.UserManager.AddUserIntoGroup(userID, ASC.Core.Users.Constants.GroupAdmin.ID);

            }
            catch (Exception exception)
            {
                (Page as confirm).ErrorMessage = HttpUtility.HtmlEncode(exception.Message);
                return;
            }
            finally
            {
                SecurityContext.Logout();
            }

            try
            {
                var cookiesKey = SecurityContext.AuthenticateMe(userID.ToString(), pwd);
                CookiesManager.SetCookies(CookiesType.UserID, userID.ToString());
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                StudioNotifyService.Instance.UserHasJoin();
            }
            catch (Exception exception)
            {
                (Page as confirm).ErrorMessage = HttpUtility.HtmlEncode(exception.Message);
                return;
            }

            UserOnlineManager.Instance.RegistryOnlineUser(SecurityContext.CurrentAccount.ID);
            WebItemManager.Instance.ItemGlobalHandlers.Login(SecurityContext.CurrentAccount.ID);
            var smsAuthSettings = SettingsManager.Instance.LoadSettings<StudioSmsNotificationSettings>(TenantProvider.CurrentTenantID);
            if (smsAuthSettings.Enable)
            {
                var uData = new UserTransferData();
                var usr = CoreContext.UserManager.GetUsers(userID);
                uData.Login = usr.Email;
                uData.UserId = userID;
                Session["UserTransferData"] = uData;
            }

            Response.Redirect("~/");
        }

        private void ShowError(string message)
        {
            ((StudioTemplate)this.Page.Master).TopNavigationPanel.CustomTitle = null;

            (Page as confirm).ErrorMessage = message;
            (Page as confirm)._confirmHolder2.Visible = false;
            (Page as confirm)._confirmHolder.Visible = false;
            (Page as confirm)._contentWithControl.Visible = true;

            if (SecurityContext.IsAuthenticated == false)
                (this.Page as confirm).ErrorMessage += ". " + String.Format(Resources.Resource.ForSignInFollowMessage,
                    string.Format("<a href=\"{0}\">", VirtualPathUtility.ToAbsolute("~/auth.aspx")), "</a>");

            _confirmHolder.Visible = false;
        }

        private string CheckPassword(string pwd, string repwd)
        {
            if (String.IsNullOrEmpty(pwd))
                return Resources.Resource.ErrorPasswordEmpty;

            try
            {
                UserManagerWrapper.CheckPasswordPolicy(pwd);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (!String.Equals(pwd, repwd))
                return Resources.Resource.ErrorMissMatchPwd;

            return String.Empty;
        }

        private UserInfo CreateNewUser(string firstName, string lastName, string email, string pwd)
        {
            var newUser = UserManagerWrapper.AddUser(new UserInfo()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                WorkFromDate = TenantUtil.DateTimeNow(),
                ActivationStatus = EmployeeActivationStatus.Activated //NOTE: set user to activated here since it accept an invite link already
            }, pwd, true);

            return newUser;
        }
    }
}