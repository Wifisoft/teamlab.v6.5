using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("MailDomainSettingsController")]
    public partial class MailDomainSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/MailDomainSettings/MailDomainSettings.ascx"; } }
        protected Tenant _currentTenant = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "maildomainsettings_script", WebPath.GetPath("usercontrols/management/maildomainsettings/js/maildomainsettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "maildomainsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/maildomainsettings/css/<theme_folder>/maildomainsettings.css") + "\">", false);

            _currentTenant = CoreContext.TenantManager.GetCurrentTenant();            
        }
       
        private bool CheckTrustedDomain(string domain)
        {
            return !string.IsNullOrEmpty(domain) && new Regex("^[a-z0-9]([a-z0-9-.]){1,98}[a-z0-9]$").IsMatch(domain);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveMailDomainSettings(TenantTrustedDomainsType type, List<string> domains)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var tenant = CoreContext.TenantManager.GetCurrentTenant();

                if (type == TenantTrustedDomainsType.Custom)
                {
                    tenant.TrustedDomains.Clear();
                    foreach (var domain in domains)
                    {
                        var d = (domain ?? "").Trim().ToLower();
                        if (!CheckTrustedDomain(d))
                            return new { Status = 0, Message = Resources.Resource.ErrorNotCorrectTrustedDomain };                        

                        tenant.TrustedDomains.Add(d);
                    }
                }

                tenant.TrustedDomainsType = type;
                CoreContext.TenantManager.SaveTenant(tenant);

                return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }
    }
}