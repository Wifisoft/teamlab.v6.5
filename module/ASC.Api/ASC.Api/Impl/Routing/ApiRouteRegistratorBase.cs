using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Logging;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    internal abstract class ApiRouteRegistratorBase : IApiRouteRegistrator
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IApiConfiguration Config { get; set; }

        [Dependency]
        public ILog Log { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            var entryPoints = Container.Resolve<IEnumerable<IApiMethodCall>>();
            var extensions = new List<string>();
            foreach (IApiResponder apiSerializer in Container.ResolveAll<IApiResponder>())
            {
                extensions.AddRange(apiSerializer.GetSupportedExtensions().Select(x => x.StartsWith(".") ? x : "." + x));
            }
            RegisterEntryPoints(routes, entryPoints, extensions);
        }

        protected abstract void RegisterEntryPoints(RouteCollection routes, IEnumerable<IApiMethodCall> entryPoints, List<string> extensions);
    }
}