using System.Collections.Generic;
using System.Linq;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.Cache;

namespace ASC.Specific
{
    public class TenantCacheKeyBuilder : ApiCacheKeyBuilder
    {
        public override string BuildCacheKeyForMethodCall(IApiMethodCall apiMethodCall, IEnumerable<object> callArgs, ApiContext context)
        {
            return Core.CoreContext.TenantManager.GetCurrentTenant().TenantId + base.BuildCacheKeyForMethodCall(apiMethodCall,callArgs,context);
        }
    }
}