using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using ASC.FederatedLogin.Profile;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.ApplicationBlock.Facebook;
using DotNetOpenAuth.OAuth2;

namespace ASC.FederatedLogin.LoginProviders
{
    public class ProviderManager
    {
        private static readonly Dictionary<string, ILoginProvider> Providers = new Dictionary<string, ILoginProvider>()
                                                                           {
                                                                               {
                                                                                   ProviderConstants.Facebook,
                                                                                   new FacebookLoginProvider()
                                                                                   },
                                                                               {
                                                                                   ProviderConstants.OpenId,
                                                                                   new OpenIdLoginProvider()
                                                                                   },
                                                                               {
                                                                                   ProviderConstants.Twitter,
                                                                                   new TwitterLoginProvider()
                                                                                },
                                                                                {
                                                                                   ProviderConstants.LinkedIn,
                                                                                   new LinkedInLoginProvider()
                                                                                },
                                                                           };



        public static LoginProfile Process(string providerType, HttpContext context, IDictionary<string,string> @params)
        {
            return Providers[providerType].ProcessAuthoriztion(context,@params);
        }
    }
}