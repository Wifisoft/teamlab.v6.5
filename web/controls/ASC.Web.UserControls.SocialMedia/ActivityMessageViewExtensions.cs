using System;
using ASC.SocialMedia;
using ASC.Web.Core.Utility.Skins;
using System.Web.UI;

namespace ASC.Web.UserControls.SocialMedia
{
    public static class ActivityMessageViewExtensions
    {
        public static string GetSocialNetworkUrl(this Message message)
        {
            switch (message.Source)
            {
                case SocialNetworks.Twitter:
                    return "http://www.twitter.com";
                case SocialNetworks.Facebook:
                    return "http://www.facebook.com";
                case SocialNetworks.LinkedIn:
                    return "http://www.linkedin.com";
                case SocialNetworks.Digg:
                    return "http://www.digg.com";
                case SocialNetworks.Quora:
                    return "http://www.quora.com";
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetSocialNetworkTitle(this Message message)
        {
            switch (message.Source)
            {
                case SocialNetworks.Twitter:
                    return "twitter.com";
                case SocialNetworks.Facebook:
                    return "facebook.com";
                case SocialNetworks.LinkedIn:
                    return "linkedin.com";
                case SocialNetworks.Digg:
                    return "digg.com";
                case SocialNetworks.Quora:
                    return "quora.com";
                default:
                    throw new NotImplementedException();
            }
        }
        
    }
}
