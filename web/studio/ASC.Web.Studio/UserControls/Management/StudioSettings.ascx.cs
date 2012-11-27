using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using System.Configuration;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("StudioSettings")]
    public partial class StudioSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/StudioSettings.ascx"; } }

        public Guid ProductID { get; set; }

        protected StudioViewSettings _studioViewSettings;

        protected Tenant _currentTenant;

        protected bool EnableDomain
        {
            get
            {
                return (CoreContext.TenantManager.GetTenantQuota(CoreContext.TenantManager.GetCurrentTenant().TenantId).HasDomain);
            }
        }

        protected static bool EnableDnsChange
        {
            get
            {
                return !string.IsNullOrEmpty(ASC.Core.CoreContext.TenantManager.GetCurrentTenant().MappedDomain);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            _studioViewSettings = SettingsManager.Instance.LoadSettings<StudioViewSettings>(TenantProvider.CurrentTenantID);
            _currentTenant = CoreContext.TenantManager.GetCurrentTenant();

            _studioViewSettingsHolder.Visible = SetupInfo.IsVisibleSettings("ViewSettings");

            //main domain settings
            _mailDomainSettings.Controls.Add(LoadControl(MailDomainSettings.Location));

            //Portal version
            if (SetupInfo.IsVisibleSettings<VersionSettings.VersionSettings>())
                _portalVersionSettings.Controls.Add(LoadControl(VersionSettings.VersionSettings.Location));

            _timelngHolder.Controls.Add(LoadControl(TimeAndLanguage.Location));

            //promo settings
            if (SetupInfo.IsVisibleSettings<PromoSettings>())
                _studioNotifyBarSettingsHolder.Controls.Add(LoadControl(PromoSettings.Location));

            //strong security password settings
            _strongPasswordSettings.Controls.Add(LoadControl(PasswordSettings.Location));

            if (SetupInfo.IsVisibleSettings<AccessSettings>())
                _restrictedAccessSettings.Controls.Add(LoadControl(AccessSettings.Location));

            invLink.Controls.Add(LoadControl(InviteLink.Location));

            //sms settings
            if (SetupInfo.IsVisibleSettings<StudioSmsNotificationSettings>())
                _smsValidationSettings.Controls.Add(LoadControl(SmsValidationSettings.Location));

            _admMessSettings.Controls.Add(LoadControl(AdminMessageSettings.Location));
        }



        private bool CheckTrustedDomain(string domain)
        {
            const string regexp2 = "(^[.\\-_a-z0-9]+\\.){1,}([a-z]){2,6}$";
            return !string.IsNullOrEmpty(domain) && (6 <= domain.Length) && new Regex(regexp2).IsMatch(domain);
        }

        #region Check custom domain name
        /// <summary>
        /// Custom domain name shouldn't ends with tenant base domain name.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private static bool CheckCustomDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return false;
            }
            if (string.IsNullOrEmpty(TenantBaseDomain))
            {
                return true;
            }
            if (domain.EndsWith(TenantBaseDomain))
            {
                return false;
            }
            if (domain.Equals(TenantBaseDomain.TrimStart('.')))
            {
                return false;
            }
            return true;
        }
        #endregion

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SaveDnsSettings(string dnsName, string alias, bool enableDns)
        {
            var resp = new AjaxResponse() { rs1 = "1" };
            try
            {
                if (!EnableDomain)
                    throw new Exception(Resources.Resource.ErrorNotAllowedOption);

                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                if (!enableDns || string.IsNullOrEmpty(dnsName))
                {
                    dnsName = null;
                }
                if (dnsName == null || CheckCustomDomain(dnsName))
                {
                    if (CoreContext.Configuration.Standalone)
                    {
                        var tenant = CoreContext.TenantManager.GetCurrentTenant();
                        tenant.MappedDomain = dnsName;
                        CoreContext.TenantManager.SaveTenant(tenant);

                        return resp;
                    }
                    else
                    {
                        if (!CoreContext.TenantManager.GetCurrentTenant().TenantAlias.Equals(alias))
                        {
                            CoreContext.TenantManager.CheckTenantAddress(alias);
                        }
                    }

                    var portalAddress = string.Format("http://{0}.{1}", alias ?? String.Empty, SetupInfo.BaseDomain);

                    var u = CoreContext.UserManager.GetUsers(CoreContext.TenantManager.GetCurrentTenant().OwnerId);
                    StudioNotifyService.Instance.SendMsgDnsChange(CoreContext.TenantManager.GetCurrentTenant(), GenerateDnsChangeConfirmUrl(u.Email, dnsName, alias, ConfirmType.DnsChange), portalAddress, dnsName);
                    resp.rs2 = string.Format(Resources.Resource.DnsChangeMsg, string.Format("<a href='mailto:{0}'>{0}</a>", u.Email));
                }
                else
                {
                    resp.rs1 = "0";
                    resp.rs2 = "<div class='errorBox'>" + Resources.Resource.ErrorNotCorrectTrustedDomain + "</div>";
                }
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class='errorBox'>" + e.Message.HtmlEncode() + "</div>";
            }
            return resp;
        }

        private string GenerateDnsChangeConfirmUrl(string email, string dnsName, string tenantAlias, ConfirmType confirmType)
        {
            var key = string.Join(string.Empty, new string[] { email.ToLower(), confirmType.ToString().ToLower(), dnsName, tenantAlias });
            var validationKey = EmailValidationKeyProvider.GetEmailKey(key);

            var sb = new StringBuilder();
            sb.Append(CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx"));
            sb.AppendFormat("?email={0}&key={1}&type={2}", HttpUtility.UrlEncode(email), validationKey, confirmType);
            if (!string.IsNullOrEmpty(dnsName))
            {
                sb.AppendFormat("&dns={0}", dnsName);
            }
            if (!string.IsNullOrEmpty(tenantAlias))
            {
                sb.AppendFormat("&alias={0}", tenantAlias);
            }
            return sb.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SaveStudioViewSettings(bool leftSidePanel)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            AjaxResponse resp = new AjaxResponse();
            _studioViewSettings = SettingsManager.Instance.LoadSettings<StudioViewSettings>(TenantProvider.CurrentTenantID);
            _studioViewSettings.LeftSidePanel = leftSidePanel;
            if (SettingsManager.Instance.SaveSettings<StudioViewSettings>(_studioViewSettings, TenantProvider.CurrentTenantID))
            {
                resp.rs1 = "1";
                resp.rs2 = "<div class=\"okBox\">" + Resources.Resource.SuccessfullySaveSettingsMessage + "</div>";
            }
            else
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class=\"errorBox\">" + Resources.Resource.UnknownError + "</div>";
            }

            return resp;
        }



        protected static string TenantBaseDomain
        {
            get
            {
                if (String.IsNullOrEmpty(SetupInfo.BaseDomain))
                    return String.Empty;
                else
                    return String.Format(".{0}", SetupInfo.BaseDomain);
            }
        }


        public static string ModifyHowToAdress(string adr)
        {
            var lang = CoreContext.TenantManager.GetCurrentTenant().Language;
            if (lang.Contains("-"))
            {
                lang = lang.Split('-')[0];
            }
            if (lang != "en") lang += "/";
            else lang = string.Empty;
            return string.Format("{0}/{1}{2}", "http://www.teamlab.com", lang, adr ?? string.Empty);
        }
    }
}