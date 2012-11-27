#region usings

using System.Collections.Generic;
using ASC.Api.Impl;

#endregion

namespace ASC.Api.Interfaces.Cache
{
    public interface IApiCacheMethodKeyBuilder
    {
        string BuildCacheKeyForMethodCall(IApiMethodCall apiMethodCall, IEnumerable<object> callArgs, ApiContext context);
    }
}