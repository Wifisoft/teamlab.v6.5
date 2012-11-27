using System;
using ASC.SocialMedia.LinkedIn;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public static class LinkedInApiHelper
    {
        public static LinkedInDataProvider GetLinkedInDataProviderForCurrentUser()
        {
            IConsumerTokenManager tokenManager = null;
            string accessToken = null;

            string consumerKey = KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInConsumerKey);
            string consumerKeySecret = KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInConsumerSecretKey);

            accessToken = KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInDefaultAccessToken);
            if (String.IsNullOrEmpty(accessToken)) return null;
            tokenManager = new LinkedInDefaultAccountTokenManager(consumerKey, consumerKeySecret);
            return new LinkedInDataProvider(tokenManager, accessToken);
        }
    }
}
