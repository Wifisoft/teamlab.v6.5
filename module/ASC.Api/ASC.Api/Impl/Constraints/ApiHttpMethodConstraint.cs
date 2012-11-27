using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace ASC.Api.Impl.Constraints
{
    public class ApiHttpMethodConstraint : HttpMethodConstraint
    {
        public ApiHttpMethodConstraint(params string[] allowedMethods):base(allowedMethods)
        {
            
        }

        protected override bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var baseMatch = base.Match(httpContext, route, parameterName, values, routeDirection);
            if (!baseMatch && routeDirection==RouteDirection.IncomingRequest)
            {
                baseMatch = AllowedMethods.Any(method => string.Equals(method, httpContext.Request.RequestType, StringComparison.OrdinalIgnoreCase));
            }
            return baseMatch;
        }
    }
}