#if DEBUG
using System;
using ASC.SocialMedia.LinkedIn;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using NUnit.Framework;

namespace ASC.SocialMedia.Tests
{
    class LinkedInDBTokenManager : IConsumerTokenManager
    {
        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }


        public LinkedInDBTokenManager(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }


        public string GetTokenSecret(string token)
        {
            return "43db680e-6c6a-4069-9155-9c16ef815586";
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            throw new NotImplementedException();
        }

        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class LinkedInTest
    {
        [Test]
        public void GetUserInfoTest()
        {
            var tokenManager = new LinkedInDBTokenManager("qnwIL9_wRC4Ew3iLl5sdEKvEDaSTgFn-RRaedF0XfXLZov0jDCq577Ta6wDLZr_8", "gJCNJ4UsvfCgPGHQRQt0CJ82GZTN6njeT1XxhyUaSsYHBAtCf58EE0P0ocBcLLqp");
            var provider = new LinkedInDataProvider(tokenManager, "8a17d3b4-5e99-4f5f-8ad3-5c9f0b28d9d1");
            var userInfo = provider.GetUserInfo("A_lDUH3Vb3");
        }
    }
}
#endif