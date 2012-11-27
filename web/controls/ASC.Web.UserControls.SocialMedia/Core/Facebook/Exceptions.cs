using System;

namespace ASC.SocialMedia.Facebook
{
    public class APILimitException : SocialMediaException
    {
        public APILimitException()
            : base()
        {
        }
    }

    public class OAuthException : SocialMediaException
    {
        public OAuthException()
            : base()
        {
        }
    }
}