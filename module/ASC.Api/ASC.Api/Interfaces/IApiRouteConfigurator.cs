using System.Collections.Generic;
using System.Reflection;
using ASC.Common.Web;

namespace ASC.Api.Interfaces
{
    public interface IApiRouteConfigurator
    {
        void RegisterEntryPoints();
        RouteCallInfo ResolveRoute(MethodInfo apiCall, Dictionary<string, object> arguments);
    }
}