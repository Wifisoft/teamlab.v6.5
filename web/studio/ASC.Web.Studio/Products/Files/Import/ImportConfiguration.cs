using System;
using System.Linq;
using System.Web.Configuration;
using ASC.Thrdparty;
using ASC.Thrdparty.Configuration;
using ASC.Thrdparty.TokenManagers;

namespace ASC.Web.Files.Import
{
    public static class ImportConfiguration
    {
        public static bool SupportImport { get; private set; }
        public static bool SupportBoxNetImport { get; private set; }
        public static bool SupportGoogleImport { get; private set; }
        public static bool SupportZohoImport { get; private set; }

        public static bool SupportInclusion { get; private set; }
        public static bool SupportBoxNetInclusion { get; private set; }
        public static bool SupportDropboxInclusion { get; private set; }
        public static bool SupportGoogleInclusion { get; private set; }


        public static string BoxNetApiKey { get; private set; }

        public static IAssociatedTokenManager GoogleTokenManager { get; private set; }

        public static string ZohoApiKey { get; private set; }

        public static string BoxNetIFrameAddress { get; private set; }

        public static string DropboxAppKey { get; private set; }

        public static string DropboxAppSecret { get; private set; }

        static ImportConfiguration()
        {
            SupportBoxNetImport = !string.IsNullOrEmpty(KeyStorage.Get("box.net"));
            SupportGoogleImport = !string.IsNullOrEmpty(KeyStorage.Get("googleConsumerKey"));
            SupportZohoImport = !string.IsNullOrEmpty(KeyStorage.Get("zoho"));
            SupportImport = SupportBoxNetImport || SupportGoogleImport || SupportZohoImport;

            var providers = (WebConfigurationManager.AppSettings["files.thirdparty.enable"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);

            SupportBoxNetInclusion = providers.Contains("boxnet");
            SupportDropboxInclusion = providers.Contains("dropbox") && !string.IsNullOrEmpty(KeyStorage.Get("dropboxappkey")) && !string.IsNullOrEmpty(KeyStorage.Get("dropboxappsecret"));
            SupportGoogleInclusion = providers.Contains("google") && SupportGoogleImport;
            SupportInclusion = SupportBoxNetInclusion || SupportDropboxInclusion || SupportGoogleInclusion;

            if (SupportBoxNetImport)
            {
                BoxNetApiKey = KeyStorage.Get("box.net");
                BoxNetIFrameAddress = KeyStorage.Get("box.net.framehandler");
            }
            if (SupportGoogleImport)
            {
                GoogleTokenManager = TokenManagerHolder.Get("google", "googleConsumerKey", "googleConsumerSecret");
            }
            if (SupportZohoImport)
            {
                ZohoApiKey = KeyStorage.Get("zoho");
            }

            if (SupportDropboxInclusion)
            {
                DropboxAppKey = KeyStorage.Get("dropboxappkey");
                DropboxAppSecret = KeyStorage.Get("dropboxappsecret");
            }
        }
    }
}