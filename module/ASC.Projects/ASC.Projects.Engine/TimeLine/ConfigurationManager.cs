using System;

namespace ASC.Projects.Engine
{
    public static class ConfigurationManager
    {
        internal static Guid  ProductID { get; set; }
        internal static String BaseVirtualPath { get; set; }
        internal static String UserProfileLink { get; set; }
        internal static String FileStorageModule { get; set; }

        public static void Configure(Guid productID, String baseVirtualPath, String userProfileLink, String fileStorageModule)
        {
            ProductID = productID;
            BaseVirtualPath = baseVirtualPath;
            UserProfileLink = userProfileLink;
            FileStorageModule = fileStorageModule;
        }
    }
}
