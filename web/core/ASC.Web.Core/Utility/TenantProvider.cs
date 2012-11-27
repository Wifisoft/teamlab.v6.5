using ASC.Core;

namespace ASC.Web.Studio.Utility
{
    public class TenantProvider
    {
        public static int CurrentTenantID
        {
            get
            {
                return CoreContext.TenantManager.GetCurrentTenant().TenantId;
            }
        }
    }
}
