using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using AjaxPro;

namespace ASC.Web.Studio.UserControls.Management.VersionSettings
{
    [AjaxNamespace("VersionSettingsController")]
    public partial class VersionSettings : System.Web.UI.UserControl
    {
        public const string Location = "~/UserControls/Management/VersionSettings/VersionSettings.ascx";

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "versionsettings_script", WebPath.GetPath("usercontrols/Management/VersionSettings/js/script.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "versionsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/Management/VersionSettings/css/<theme_folder>/style.css") + "\">", false);

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SwitchVersion(string version)
        {
            try
            {
                int tenantVersion = int.Parse(version);

                if (!CoreContext.TenantManager.GetTenantVersions().Any(x => x.Id == tenantVersion)) throw new ArgumentException(Resources.Resource.SettingsBadPortalVersion);

                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
                var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                if (tenant.Version != tenantVersion)
                {
                    tenant.Version = tenantVersion;
                    CoreContext.TenantManager.SaveTenant(tenant);
                }
                else
                {
                    throw new ArgumentException(Resources.Resource.SettingsAlreadyCurrentPortalVersion);
                }
                return new { Status = 1 };
            }
            catch (Exception e)
            {
                return new { Status = 0, e.Message };

            }
        }

        protected string GetLocalizedName(string name)
        {
            try
            {
                var localizedName = Resources.Resource.ResourceManager.GetString(("version_"+name.Replace(".","")).ToLowerInvariant());
                if (string.IsNullOrEmpty(localizedName))
                {
                    localizedName = name;
                }
                return localizedName;
            }
            catch (Exception)
            {
                return name;
            }
        }
    }
}