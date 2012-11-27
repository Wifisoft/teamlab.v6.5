using System.Collections.Generic;
using System.Web.Routing;

namespace ASC.Api.Interfaces
{
    public interface IApiArgumentBuilder
    {
        IEnumerable<object> BuildCallingArguments(RequestContext context, IApiMethodCall methodToCall);
    }
}