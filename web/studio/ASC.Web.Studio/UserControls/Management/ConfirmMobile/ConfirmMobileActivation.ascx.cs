using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.FederatedLogin;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Security.Ajax;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("MobileActivationController")]
    public partial class ConfirmMobileActivation : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/ConfirmMobile/ConfirmMobileActivation.ascx"; } }

        public bool Activate { get; set; }

        private string _mobilePhone;

        protected override void OnPreRender(EventArgs e)
        {
            var authComm = LoadControl(AuthCommunications.Location) as AuthCommunications;
            authComm.MaxHighAdmMess = true;
            _communitations.Controls.Add(authComm);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var smsAuthSettings = SettingsManager.Instance.LoadSettings<StudioSmsNotificationSettings>(TenantProvider.CurrentTenantID);
            
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "ConfirmMobile_script", WebPath.GetPath("usercontrols/Management/ConfirmMobile/js/ConfirmMobile.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ConfirmMobile_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/confirmmobile/css/css.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "watermark_script", WebPath.GetPath("js/jquery.watermarkinput.js"));

            if (Activate) return;

            var obj = (UserTransferData)Context.Session["UserTransferData"];
            if (obj == null)
                return;

            _mobilePhone = GetUser(obj).MobilePhone;
            PutAuthCode(obj, _mobilePhone, false);
        }

        [AjaxSecurityPassthroughAttribute]
        [AjaxMethod(HttpSessionStateRequirement.Read)]
        public string GetPhoneNoise(string number)
        {
            return BuildPhoneNoise(GetPhoneValueDigits(number));
        }

        [AjaxSecurityPassthroughAttribute]
        [AjaxMethod(HttpSessionStateRequirement.Read)]
        public object SendAuthCode(string phoneNumber)
        {
            if (phoneNumber == null || phoneNumber.Trim() == String.Empty)
                return new {Status = 0, Message = Resources.Resource.ActivateMobilePhoneEmptyPhoneNumber};
            try
            {
                var obj = (UserTransferData) Context.Session["UserTransferData"];
                PutAuthCode(obj, GetPhoneValueDigits(phoneNumber), false);
                return new {Status = 1, Noise = GetPhoneNoise(phoneNumber), Message = String.Empty};
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }
        [AjaxSecurityPassthroughAttribute]
        [AjaxMethod(HttpSessionStateRequirement.Read)]
        public object SendAuthCodeAgain()
        {
            try
            {
                var obj = (UserTransferData)Context.Session["UserTransferData"];

                if (obj == null)
                    return new {Status = 0, Message = ""};

                var user = GetUser(obj);

                PutAuthCode(obj, user.MobilePhone, true);
                return new { Status = 1 };
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }

        [AjaxSecurityPassthroughAttribute]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object ValidateAuthCode(string code, bool keepUser)
        {
            if (String.IsNullOrEmpty(code.Trim()))
                return new { Status = 0, Message = Resources.Resource.ActivateMobilePhoneEmptyCode };

            try
            {
                var obj = (UserTransferData)Context.Session["UserTransferData"];
                var user = GetUser(obj);
                var key = StudioSmsKeyStorage.Instance.GetKey(user.MobilePhone);

                if (key != code)
                    return new { Status = 0, Message = Resources.Resource.SmsAuthenticationMessageError };

                if (obj != null)
                {
                    var cookiesKey = Authenticate(obj);
                    CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);

                    if (keepUser)
                        CookiesManager.SetCookies(CookiesType.ConfKey, EmailValidationKeyProvider.GetEmailKey(user.Email.ToLower()));
                }

                if (user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
                {
                    user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.Activated;
                    CoreContext.UserManager.SaveUserInfo(user);
                }
                Context.Session["UserTransferData"] = null;
                StudioSmsKeyStorage.Instance.DeleteRecord(user.MobilePhone);

                return new { Status = 1 };

            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }

        void PutAuthCode(UserTransferData obj, string phoneNumber, bool again)
        {
            if (phoneNumber.StartsWith("+"))
                phoneNumber = phoneNumber.Substring(1, phoneNumber.Length - 1);

            if (obj == null || obj.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
            {
                var user = GetUser(obj);
                user.MobilePhone = phoneNumber;
                user.MobilePhoneActivationStatus = 0;

                if (obj != null)
                    Authenticate(obj);
                
                CoreContext.UserManager.SaveUserInfo(user);

                if (obj != null)
                    SecurityContext.Logout();
            }

            if (String.IsNullOrEmpty(StudioSmsKeyStorage.Instance.GetKey(phoneNumber)) || again)
            {
                SendMessageToLogon(phoneNumber);
            }
        }

        private string GetPhoneValueDigits(string mobilePhone)
        {
            var reg = new Regex(@"[^\d]");
            return reg.Replace(mobilePhone, String.Empty);
        }

        private string BuildPhoneNoise(string number)
        {
            if (String.IsNullOrEmpty(number))
                return String.Empty;

            if (number.Length < 6)
                return String.Format("****{0}", number.Substring(number.Length - 2, 2));

            var sb = new StringBuilder(15);
            sb.Append("+");
            sb.Append(number.Substring(0, 2));
            for (var i = 0; i < number.Length - 5; i++)
            {
                sb.Append("*");
            }
            sb.Append(number.Substring(number.Length - 3, 3));
            return sb.ToString();
        }

        public string GetPhoneNoise()
        {
            return BuildPhoneNoise(_mobilePhone);
        }

        private void SendMessageToLogon(string phoneNumber)
        {
            var smsSender = SmsSenderCreator.CreateSender(phoneNumber);
            var id = new Random().Next(100000, 1000000);
            StudioSmsKeyStorage.Instance.CreateRecord(phoneNumber, id.ToString());
            smsSender.Notify(String.Format(Resources.Resource.SmsAuthenticationMessageToUser, id));
        }

        private UserInfo GetUser(UserTransferData obj)
        {
            return CoreContext.UserManager.GetUsers(obj == null ? SecurityContext.CurrentAccount.ID : (!String.IsNullOrEmpty(obj.HashId) ? GetUserGuid(obj.HashId) : obj.UserId));
        }

        private string Authenticate(UserTransferData obj)
        {
            if (!String.IsNullOrEmpty(obj.HashId))
                return AuthenticateByHash(obj.HashId);

            return AuthenticateByPair(obj.Login, obj.Password);
        }

        private string AuthenticateByPair(string login, string password)
        {
            var cookiesKey = SecurityContext.AuthenticateMe(login, password);
            return cookiesKey;
        }

        private string AuthenticateByHash(string hashId)
        {
            var accounts = new AccountLinker(WebConfigurationManager.ConnectionStrings["webstudio"]).GetLinkedObjectsByHashId(hashId);

            var userId = GetUserGuid(hashId);
            if (userId.Equals(Guid.Empty))
                return String.Empty;

            var coreAcc = CoreContext.UserManager.GetUsers(userId);
            var cookiesKey = SecurityContext.AuthenticateMe(coreAcc.Email, CoreContext.Authentication.GetUserPasswordHash(coreAcc.ID));
            return cookiesKey;
        }

        private Guid GetUserGuid(string hashId)
        {
            var accounts = new AccountLinker(WebConfigurationManager.ConnectionStrings["webstudio"]).GetLinkedObjectsByHashId(hashId);

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
                    return account;
                }
            }
            return Guid.Empty;
        }
    }
}