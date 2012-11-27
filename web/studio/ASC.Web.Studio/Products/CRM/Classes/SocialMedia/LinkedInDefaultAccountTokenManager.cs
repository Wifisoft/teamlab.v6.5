using System;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public class LinkedInDefaultAccountTokenManager : IConsumerTokenManager
    {
        public LinkedInDefaultAccountTokenManager(string consumerKey, string consumerSecret)
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
        }

        #region IConsumerTokenManager Members

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }

        #endregion

        #region ITokenManager Members

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            throw new NotImplementedException();
        }

        public string GetTokenSecret(string token)
        {
            if (String.IsNullOrEmpty(token))
                return null;

            if (String.Equals(token, KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInDefaultAccessToken)))
                return KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInDefaultAccessTokenSecret);

            return null;
        }

        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        public void StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest request, DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage response)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
