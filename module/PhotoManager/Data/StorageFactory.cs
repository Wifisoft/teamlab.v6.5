using ASC.Core;
using ASC.PhotoManager.Model;

namespace ASC.PhotoManager.Data
{
	public static class StorageFactory
	{
		public static string Id = "community";

		public static IImageStorage GetStorage()
		{
            return new ImageStorage2(Id, CoreContext.TenantManager.GetCurrentTenant().TenantId);
		}
	}
}
