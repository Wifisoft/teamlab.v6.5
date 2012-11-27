using System.Configuration;
using System.Web;
using System.Web.Caching;
using ASC.Thrdparty.Configuration;
using ASC.Thrdparty.TokenManagers;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Thrdparty
{
    public class TokenManagerHolder
    {
        public static IAssociatedTokenManager Get(string providerKey, string consumerKey, string consumerSecret)
        {
            var tokenManager = (IAssociatedTokenManager)HttpRuntime.Cache.Get(providerKey);
            if (tokenManager == null)
            {
                if (!string.IsNullOrEmpty(consumerKey))
                {
                    tokenManager = GetTokenManager(consumerKey, consumerSecret);
                    HttpRuntime.Cache.Add(providerKey, tokenManager, null, Cache.NoAbsoluteExpiration,
                                          Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                }
            }
            return tokenManager;
        }

        private static IAssociatedTokenManager GetTokenManager(string consumerKey, string consumerSecret)
        {
            IAssociatedTokenManager tokenManager = null;
            var section = ConsumerConfigurationSection.GetSection();
            if (section!=null && !string.IsNullOrEmpty(section.ConnectionString))
            {
                tokenManager = new DbTokenManager(KeyStorage.Get(consumerKey), KeyStorage.Get(consumerSecret),
                                                  "auth_tokens",
                                                  ConfigurationManager.ConnectionStrings[section.ConnectionString]);
            }
            else
            {
                //For testing return the inmemorytokenmanager
                tokenManager = new InMemoryTokenManager(KeyStorage.Get(consumerKey), KeyStorage.Get(consumerSecret));    
            }
            
            return tokenManager;
        }
    }
}