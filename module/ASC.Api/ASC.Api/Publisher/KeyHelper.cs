using System.Linq;
using System.Web.Routing;

namespace ASC.Api.Publisher
{
    public class PubSubKeyHelper
    {
        public static string GetKeyForRoute(RouteData routeData)
        {
            return string.Join("|",routeData.Values.Select(x=>x.Key+":"+x.Value).ToArray());
        }
    }
}