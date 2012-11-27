using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.Profile;
using ASC.Thrdparty.Configuration;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth;
using ASC.Thrdparty.TokenManagers;

namespace ASC.FederatedLogin.LoginProviders
{
    class TwitterLoginProvider : ILoginProvider
    {
        private const string _twittertokenmanager = "TwitterSessionTokenManager";
        private static WebConsumer signInConsumer;
        private static readonly object SignInConsumerInitLock = new object();

        private static WebConsumer TwitterSignIn
        {
            get
            {
                if (signInConsumer == null)
                {
                    lock (SignInConsumerInitLock)
                    {
                        if (signInConsumer == null)
                        {
                            signInConsumer = new WebConsumer(TwitterConsumer.SignInWithTwitterServiceDescription, ShortTermUserSessionTokenManager);
                        }
                    }
                }

                return signInConsumer;
            }
        }

        private static InMemoryTokenManager ShortTermUserSessionTokenManager
        {
            get
            {
                var store = HttpContext.Current.Session;
                var tokenManager = (InMemoryTokenManager)store[_twittertokenmanager];
                if (tokenManager == null)
                {
                    string consumerKey = KeyStorage.Get("twitterKey");
                    string consumerSecret = KeyStorage.Get("twitterSecret");
                    tokenManager = new InMemoryTokenManager(consumerKey, consumerSecret);
                    store[_twittertokenmanager] = tokenManager;
                }
                else
                {
                    throw new InvalidOperationException(
                        "No Twitter OAuth consumer key and secret could be found in web.config AppSettings.");
                }


                return tokenManager;
            }
        }


        public LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params)
        {

            var token = context.Request["oauth_token"];
            if (string.IsNullOrEmpty(token))
            {
                var request = TwitterConsumer.StartSignInWithTwitter(TwitterSignIn, false);
                request.Send();
            }
            else
            {
                string screenName;
                int userId;
                string accessToken;
                if (TwitterConsumer.TryFinishSignInWithTwitter(TwitterSignIn, out screenName, out userId, out accessToken))
                {
                    //Sucess. Get information
                    var info = TwitterConsumer.GetUserInfo(TwitterSignIn,userId, accessToken);
                    var profile = ProfileFromTwitter(info);
                    return profile;
                }
                return LoginProfile.FromError(new Exception("Login failed"));
            }


            return null;
        }

        internal static LoginProfile ProfileFromTwitter(XDocument info)
        {
            XPathNavigator nav = info.CreateNavigator();
            var profile = new LoginProfile
                              {
                                  Name = nav.SelectNodeValue("//screen_name"),
                                  DisplayName = nav.SelectNodeValue("//name"),
                                  Avatar = nav.SelectNodeValue("//profile_image_url"),
                                  TimeZone = nav.SelectNodeValue("//time_zone"),
                                  Locale = nav.SelectNodeValue("//lang"),
                                  Id = nav.SelectNodeValue("//id"),
                                  Link = nav.SelectNodeValue("//url"),
                                  Provider = ProviderConstants.Twitter
                              };
            return profile;
        }
    }
}