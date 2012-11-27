using System;
using ASC.SocialMedia.Facebook;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public static class FacebookApiHelper
    {
        public static FacebookApiInfo GetFacebookApiInfoForCurrentUser()
        {
            FacebookApiInfo apiInfo = new FacebookApiInfo();

            SetDefaultTokens(apiInfo);

            if (String.IsNullOrEmpty(apiInfo.AccessToken))
                return null;
            else
                return apiInfo;
        }

        private static void SetDefaultTokens(FacebookApiInfo apiInfo)
        {
            apiInfo.AccessToken = KeyStorage.Get(SocialMediaConstants.ConfigKeyFacebookDefaultAccessToken);
        }
    }
}
