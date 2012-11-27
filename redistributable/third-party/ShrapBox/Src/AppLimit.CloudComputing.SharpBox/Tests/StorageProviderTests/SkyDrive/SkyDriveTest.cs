using System;
using NUnit.Framework;

namespace AppLimit.CloudComputing.SharpBox.Tests.StorageProviderTests.SkyDrive
{
    [TestFixture]
    public class SkyDriveTest : StorageProviderTestsBase
    {
        protected override CloudStorage CreateStorage()
        {
            var storage = new CloudStorage();
            var config = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.SkyDrive);
            storage.Open(config, GetAccessToken());
            return storage;
        }

        protected override ICloudStorageAccessToken GetAccessToken()
        {
            //NOTE: first obtain and serialize valid token to file on desktop
            var token = new CloudStorage().DeserializeSecurityTokenEx(
                Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\token_data"));
            return token;
        }
    }
}
