using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Employee;
using ASC.Api.Impl;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Core;
using ASC.Web.Studio.Core;

namespace ASC.Api.Settings
{
    ///<summary>
    /// Portal settings
    ///</summary>
    public class SettingsApi : Interfaces.IApiEntryPoint
    {
        private ApiContext _context;

        public string Name
        {
            get { return "settings"; }
        }

        public SettingsApi(ApiContext context)
        {
            _context = context;
        }

        ///<summary>
        /// Returns the list of all available portal settings with the current values for each one
        ///</summary>
        ///<short>
        /// Portal settings
        ///</short>
        ///<returns>Settings</returns>
        [Read("")]
        public SettingsWrapper GetSettings()
        {
            var settings = new SettingsWrapper();
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            settings.Timezone = tenant.TimeZone.ToSerializedString();
            settings.UtcOffset = tenant.TimeZone.GetUtcOffset(DateTime.UtcNow);
            settings.UtcHoursOffset = tenant.TimeZone.GetUtcOffset(DateTime.UtcNow).TotalHours;
            settings.TrustedDomains = tenant.TrustedDomains;
            settings.TrustedDomainsType = tenant.TrustedDomainsType;
            settings.Culture = tenant.GetCulture().ToString();
            return settings;
        }

        ///<summary>
        /// Returns space usage quota for portal with the space usage of each module
        ///</summary>
        ///<short>
        /// Space usage
        ///</short>
        ///<returns>Space usage and limits for upload</returns>
        [Read("quota")]
        public QuotaWrapper GetQuotaUsed()
        {
            var diskQuota = CoreContext.TenantManager.GetTenantQuota(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            return new QuotaWrapper(diskQuota, GetQuotaRows());
        }
        /// <summary>
        /// Get list of availibe portal versions including current version
        /// </summary>
        /// <returns>List of availibe portal versions including current version</returns>
        [Read("version")]
        public TenantVersionWrapper GetVersions()
        {
            return new TenantVersionWrapper(CoreContext.TenantManager.GetCurrentTenant().Version, CoreContext.TenantManager.GetTenantVersions());
        }

        /// <summary>
        /// Set current portal version to specified
        /// </summary>
        /// <param name="versionId">Version id</param>
        /// <returns>List of availibe portal versions including current version</returns>
        [Update("version")]
        public TenantVersionWrapper SetVersion(int versionId)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!CoreContext.TenantManager.GetTenantVersions().Any(x => x.Id == versionId)) throw new ArgumentException("Bad version id");
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant.Version != versionId)
            {
                tenant.Version = versionId;
                CoreContext.TenantManager.SaveTenant(tenant);
            }
            return GetVersions();
        }

        /// <summary>
        /// Returns security settings about product, module or addon
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Read("security")]
        public IEnumerable<SecurityWrapper> GetWebItemSecurityInfo(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                ids = WebItemManager.Instance.GetItemsAll().Select(i => i.ID.ToString());
            }

            var subItemList = WebItemManager.Instance.GetItemsAll().Where(item => item.IsSubItem()).Select(i => i.ID.ToString());

            return ids.Select(id => WebItemSecurity.GetSecurityInfo(id))
                .Select(i => new SecurityWrapper
                                 {
                                     WebItemId = i.WebItemId,
                                     Enabled = i.Enabled,
                                     Users = i.Users.Select(u => EmployeeWraper.Get(u)),
                                     Groups = i.Groups.Select(g => new GroupWrapperSummary(g)),
                                     IsSubItem = subItemList.Contains(i.WebItemId),
                                 }).ToList();
        }

        /// <summary>
        /// Set security settings for product, module or addon
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <param name="subjects"></param>
        [Update("security")]
        public IEnumerable<SecurityWrapper> SetWebItemSecurity(string id, bool enabled, IEnumerable<Guid> subjects)
        {
            WebItemSecurity.SetSecurity(id, enabled, subjects != null ? subjects.ToArray() : null);

            return GetWebItemSecurityInfo(new List<string> { id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        [Read("security/administrator/{productid}")]
        public IEnumerable<EmployeeWraper> GetProductAdministrators(Guid productid)
        {
            return WebItemSecurity.GetProductAdministrators(productid)
                .Select(u => EmployeeWraper.Get(u))
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Read("security/administrator")]
        public object IsProductAdministrator(Guid productid, Guid userid)
        {
            var result = WebItemSecurity.IsProductAdministrator(productid, userid);
            return new { ProductId = productid, UserId = userid, Administrator = result, };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="userid"></param>
        /// <param name="administrator"></param>
        /// <returns></returns>
        [Update("security/administrator")]
        public object SetProductAdministrator(Guid productid, Guid userid, bool administrator)
        {
            WebItemSecurity.SetProductAdministrator(productid, userid, administrator);
            return new { ProductId = productid, UserId = userid, Administrator = administrator, };
        }


        private static IList<TenantQuotaRow> GetQuotaRows()
        {
            return CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(CoreContext.TenantManager.GetCurrentTenant().TenantId))
                .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty).ToList();
        }
    }
}
