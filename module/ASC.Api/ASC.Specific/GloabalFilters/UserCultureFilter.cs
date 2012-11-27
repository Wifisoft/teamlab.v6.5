using System.Threading;
using System.Web.Routing;
using ASC.Api.Attributes;
using ASC.Core;

namespace ASC.Specific.GloabalFilters
{
    public class UserCultureFilter : ApiCallFilter
    {
        public override void PreMethodCall(ASC.Api.Interfaces.IApiMethodCall method, ASC.Api.Impl.ApiContext context, System.Collections.Generic.IEnumerable<object> arguments)
        {
            //Check culture and set
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null)
            {
                var culture = tenant.GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            if (SecurityContext.IsAuthenticated)
            {
                var culture = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            base.PreMethodCall(method, context, arguments);
        }
    }
}