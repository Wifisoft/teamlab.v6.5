using System;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive
{
    internal class SkyDriveConfiguration : ICloudStorageConfiguration
    {
        public Uri ServiceLocator
        {
            get { return new Uri(SkyDriveConstants.BaseAccessUrl);}
        }

        public bool TrustUnsecureSSLConnections
        {
            get { return false; }
        }

        public CloudStorageLimits Limits
        {
            get { return new CloudStorageLimits(); }
        }
    }
}
