using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("TimeAndLanguageSettingsController")]
    public partial class TimeAndLanguage : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/TimeAndLanguage/TimeAndLanguage.ascx"; } }

        protected Tenant _currentTenant;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "TimeAndLanguage_script", WebPath.GetPath("usercontrols/management/TimeAndLanguage/js/TimeAndLanguage.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TimeAndLanguage_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/TimeAndLanguage/css/<theme_folder>/TimeAndLanguage.css") + "\">", false);
            _currentTenant = CoreContext.TenantManager.GetCurrentTenant();
        }

        protected string RenderLanguageSelector()
        {
            var sb = new StringBuilder();
            sb.Append("<select id=\"studio_lng\" class=\"comboBox\">");
            foreach (var ci in SetupInfo.EnabledCultures)
            {
                sb.AppendFormat("<option " + (String.Equals(_currentTenant.GetCulture().Name, ci.Name) ? "selected" : "") + " value=\"{0}\">{1}</option>", ci.Name, ci.DisplayName);
            }
            sb.Append("</select>");

            return sb.ToString();
        }

        protected string RenderTimeZoneSelector()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"studio_timezone\" class=\"comboBox\">");

            var listTimeZones = TimeZoneInfo.GetSystemTimeZones();
            var listTimeZeroZones = new List<TimeZoneInfo>();
            var listTimePlusZones = new List<TimeZoneInfo>();
            var listTimeMinusZones = new List<TimeZoneInfo>();
            for (var i = 0; i < listTimeZones.Count; i++)
            {
                if (i < 4)
                {
                    listTimeZeroZones.Add(listTimeZones[i]);
                    continue;
                }
                var nameZone = listTimeZones[i].DisplayName;
                if (nameZone[4] == '+')
                {
                    listTimePlusZones.Add(listTimeZones[i]);
                }
                else
                {
                    listTimeMinusZones.Add(listTimeZones[i]);
                }
            }
            listTimeMinusZones.Reverse();

            foreach (TimeZoneInfo timeZone in listTimeMinusZones)
            {
                sb.AppendFormat("<option " + ((timeZone.Equals(_currentTenant.TimeZone) && timeZone.Id.Equals(_currentTenant.TimeZone.Id)) ? "selected" : "") + " value=\"{0}\">{1}</option>", timeZone.Id, timeZone.DisplayName);
            }
            foreach (TimeZoneInfo timeZone in listTimeZeroZones)
            {
                sb.AppendFormat("<option " + ((timeZone.Equals(_currentTenant.TimeZone)  && timeZone.Id.Equals(_currentTenant.TimeZone.Id))? "selected" : "") + " value=\"{0}\">{1}</option>", timeZone.Id, timeZone.DisplayName);
            }
            foreach (TimeZoneInfo timeZone in listTimePlusZones)
            {
                sb.AppendFormat("<option " + ((timeZone.Equals(_currentTenant.TimeZone) && timeZone.Id.Equals(_currentTenant.TimeZone.Id)) ? "selected" : "") + " value=\"{0}\">{1}</option>", timeZone.Id, timeZone.DisplayName);
            }

            sb.Append("</select>");

            return sb.ToString();


        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveLanguageTimeSettings(string lng, string timeZoneID)
        {
            var resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var tenant = CoreContext.TenantManager.GetCurrentTenant();
                var culture = CultureInfo.GetCultureInfo(lng);

                var changelng = false;
                if (SetupInfo.EnabledCultures.Find(c => String.Equals(c.Name, culture.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    if (!String.Equals(tenant.Language, culture.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tenant.Language = culture.Name;
                        changelng = true;
                    }
                }

                tenant.TimeZone = new List<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones()).Find(tz => String.Equals(tz.Id, timeZoneID));

                CoreContext.TenantManager.SaveTenant(tenant);
                if (changelng)
                {
                    return new { Status = 1, Message = String.Empty };
                }
                else
                {
                    return new { Status = 2, Message = Resources.Resource.SuccessfullySaveSettingsMessage };
                }
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }       
    }
}