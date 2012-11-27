using ASC.Common.Web;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News.Code.DAO
{
    public static class FeedStorageFactory
	{
        public static string Id = "community";

		private static string key = "FeedStorageKey";

		public static IFeedStorage Create()
		{
			var ctx = DisposableHttpContext.Current;
			
			var storage = ctx[key] as IFeedStorage;
			if (storage == null)
			{
				storage = new DbFeedStorage(TenantProvider.CurrentTenantID);
				ctx[key] = storage;
			}
			return storage;
		}
	}
}