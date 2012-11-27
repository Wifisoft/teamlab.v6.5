using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    class ApiRouteRegistrator : ApiRouteRegistratorBase
    {
        protected override void RegisterEntryPoints(RouteCollection routes,IEnumerable<IApiMethodCall> entryPoints, List<string> extensions)
        {
            foreach (IApiMethodCall apiMethodCall in entryPoints.OrderBy(x => x.RoutingUrl.IndexOf('{')).ThenBy(x => x.RoutingUrl.LastIndexOf('}')))
            {
                foreach (string extension in extensions)
                {
                    routes.Add(GetRoute(Container.Resolve<IApiRouteHandler>(), apiMethodCall, extension));
                }
                routes.Add(GetRoute(Container.Resolve<IApiRouteHandler>(), apiMethodCall));
            }
        }

        public Route GetRoute(IApiRouteHandler routeHandler, IApiMethodCall method)
        {
            return GetRoute(routeHandler, method, string.Empty);
        }

        public Route GetRoute(IApiRouteHandler routeHandler, IApiMethodCall method, string extension)
        {
            var dataTokens = new RouteValueDictionary
                                 {
                                     {DataTokenConstants.RequiresAuthorization,method.RequiresAuthorization}
                                 };
            return new Route(method.FullPath + extension, null, method.Constraints, dataTokens, routeHandler);
        }
    }
}