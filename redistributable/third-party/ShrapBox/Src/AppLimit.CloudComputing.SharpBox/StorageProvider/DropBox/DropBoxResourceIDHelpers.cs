using System;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox
{
    internal static class DropBoxResourceIDHelpers
    {
        public static String GetResourcePath(ICloudFileSystemEntry resource)
        {
            return GetResourcePath(resource, null);
        }

        public static String GetResourcePath(ICloudFileSystemEntry parent, String nameOrId)
        {
            String path = parent != null ? parent.Id.Trim('/') : String.Empty;
            if (!String.IsNullOrEmpty(nameOrId) && !nameOrId.Equals("/"))
                path = String.Format("{0}/{1}", path, nameOrId.Trim('/'));
            return path.Trim('/');
        }

        public static String GetParentID(String path)
        {
            path = path.Trim('/');
            int index = path.LastIndexOf('/');
            return index != -1 ? path.Substring(0, index) : "/";
        }
    }
}
