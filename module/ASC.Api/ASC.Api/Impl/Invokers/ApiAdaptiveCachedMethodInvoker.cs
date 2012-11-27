using System;
using ASC.Api.Interfaces;

namespace ASC.Api.Impl.Invokers
{
    internal class ApiAdaptiveCachedMethodInvoker : ApiCachedMethodInvoker
    {
        private readonly int _maxuncahetime;
        private readonly int _cachetime;

        
        public ApiAdaptiveCachedMethodInvoker(int maxuncahetime, int cachetime)
        {
            _maxuncahetime = maxuncahetime;
            _cachetime = cachetime;
        }

        protected override void GetCachingPolicy(IApiMethodCall methodToCall, TimeSpan elapsed, out long cacheTime, out bool shouldCache)
        {
            if (elapsed.TotalMilliseconds>_maxuncahetime)
            {
                cacheTime = _cachetime;
                shouldCache = IsGetRequest(methodToCall);
            }
            else
            {
                base.GetCachingPolicy(methodToCall, elapsed, out cacheTime, out shouldCache);
            }
        }

        protected override bool ShouldCacheMethod(IApiMethodCall methodToCall)
        {
            return IsGetRequest(methodToCall);
        }

        private static bool IsGetRequest(IApiMethodCall methodToCall)
        {
            return "get".Equals(methodToCall.HttpMethod, StringComparison.OrdinalIgnoreCase);
        }
    }
}