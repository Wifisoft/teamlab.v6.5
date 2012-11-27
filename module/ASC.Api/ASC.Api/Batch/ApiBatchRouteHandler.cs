using System.Linq;
using System.Web;
using System.Web.Routing;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Batch
{
    public class ApiBatchRouteHandler : ApiRouteHandler
    {
        public override IHttpHandler GetHandler(IUnityContainer container, RequestContext requestContext)
        {
            return container.Resolve<ApiBatchHttpHandler>(new DependencyOverride(typeof(RouteData), requestContext.RouteData));
        }
    }
}