using System.Web.Routing;

namespace ASC.Api.Interfaces
{
    public interface IApiRouteRegistrator
    {
        void RegisterRoutes(RouteCollection routes);
    }
}