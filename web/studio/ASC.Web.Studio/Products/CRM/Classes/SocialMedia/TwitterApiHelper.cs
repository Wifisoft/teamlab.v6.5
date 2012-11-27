using ASC.SocialMedia.Twitter;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public static class TwitterApiHelper
    {
        public static TwitterApiInfo GetTwitterApiInfoForCurrentUser()
        {
            TwitterApiInfo apiInfo = new TwitterApiInfo
            {
                ConsumerKey = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterConsumerKey),
                ConsumerSecret = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterConsumerSecretKey)
            };

            SetDefaultTokens(apiInfo);

            return apiInfo;
        }

        private static void SetDefaultTokens(TwitterApiInfo apiInfo)
        {
            apiInfo.AccessToken = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterDefaultAccessToken);
            apiInfo.AccessTokenSecret = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterDefaultAccessTokenSecret);
        }
    }
}
