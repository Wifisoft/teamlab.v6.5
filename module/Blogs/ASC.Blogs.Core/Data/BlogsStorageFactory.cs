
namespace ASC.Blogs.Core.Data
{
    public class BlogsStorageFactory
    {
        public const string DbRegistryKey = "community";
 
        public static BlogsStorage GetStorage(int tenant)
        {
            return new BlogsStorage(DbRegistryKey, tenant);
        }
    }
}
