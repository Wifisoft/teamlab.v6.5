using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ASC.Api.Batch;
using ASC.Api.Impl.Constraints;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    public class ApiBatchRouteRegitrator : IApiRouteRegistrator
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IApiConfiguration Config { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            var constrasints = new RouteValueDictionary {{"method", new ApiHttpMethodConstraint("POST", "GET")}};
            var basePath = Config.GetBasePath();
            foreach (var extension in Container.ResolveAll<IApiResponder>().SelectMany(apiSerializer => apiSerializer.GetSupportedExtensions().Select(x => x.StartsWith(".") ? x : "." + x)))
            {
                routes.Add(new Route(basePath + "batch" + extension, null, constrasints, null, new ApiBatchRouteHandler()));
            }
            routes.Add(new Route(basePath + "batch", null, constrasints, null, new ApiBatchRouteHandler()));
        }
    }
}