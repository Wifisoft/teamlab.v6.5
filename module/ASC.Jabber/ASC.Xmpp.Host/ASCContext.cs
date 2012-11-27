using ASC.Core;
using ASC.Core.Configuration;
using ASC.Core.Tenants;

namespace ASC.Xmpp.Host
{
    static class ASCContext
    {
        public static IUserManagerClient UserManager
        {
            get
            {
                return CoreContext.UserManager;
            }
        }

        public static IAuthManagerClient Authentication
        {
            get
            {
                return CoreContext.Authentication;
            }
        }

        public static IGroupManagerClient GroupManager
        {
            get
            {
                return CoreContext.GroupManager;
            }
        }

        public static Tenant GetCurrentTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant(false);
        }

        public static void SetCurrentTenant(string domain)
        {
            SecurityContext.AuthenticateMe(Constants.CoreSystem);

            var current = CoreContext.TenantManager.GetCurrentTenant(false);
            if (current == null || string.Compare(current.TenantDomain, domain, true) != 0)
            {
                CoreContext.TenantManager.SetCurrentTenant(domain);
            }
        }
    }
}