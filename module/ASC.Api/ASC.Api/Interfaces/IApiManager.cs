#region usings

using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using ASC.Api.Impl;
using ASC.Api.Routing;
using ASC.Common.Web;

#endregion

namespace ASC.Api.Interfaces
{
    public interface IApiManager
    {
        object InvokeMethod(IApiMethodCall methodToCall, ApiContext apiContext);
        IApiMethodCall GetMethod(string routeUrl, string httpMethod);

    }
}