using System;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive
{
    internal static class SkyDriveHelpers
    {
        public static bool IsResourceID(String nameOrID)
        {
            return SkyDriveConstants.ResourceIDRegex.IsMatch(nameOrID);
        }

        public static void CopyProperties(ICloudFileSystemEntry src, ICloudFileSystemEntry dest)
        {
            if (!(dest is BaseFileEntry) || !(src is BaseFileEntry)) return;
            
            var destBase = dest as BaseFileEntry;
            var srcBase = src as BaseFileEntry;
            destBase.Name = srcBase.Name;
            destBase.Id = srcBase.Id;
            destBase.Modified = srcBase.Modified;
            destBase.Length = srcBase.Length;
            destBase[SkyDriveConstants.UploadLocationKey] = srcBase[SkyDriveConstants.UploadLocationKey];
            destBase.ParentID = srcBase.ParentID;
        }
    }
}
