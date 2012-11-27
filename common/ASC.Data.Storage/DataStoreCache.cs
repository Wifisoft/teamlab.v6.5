using System.Web;
using System.Web.Caching;

namespace ASC.Data.Storage
{
    internal static class DataStoreCache
    {
        internal static void Put(IDataStore store, string tenantId, string module)
        {
            HttpRuntime.Cache.Add(MakeCacheKey(tenantId, module), store, null, Cache.NoAbsoluteExpiration,
                                  Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        internal static IDataStore Get(string tenantId, string module)
        {
            return HttpRuntime.Cache.Get(MakeCacheKey(tenantId, module)) as IDataStore;
        }


        private static string MakeCacheKey(string tennantId, string module)
        {
            return string.Format("{0}:\\{1}", tennantId, module);
        }
    }
}