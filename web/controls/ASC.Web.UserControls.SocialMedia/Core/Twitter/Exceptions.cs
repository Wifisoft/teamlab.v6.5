using System;

namespace ASC.SocialMedia.Twitter
{
    public class TwitterException : SocialMediaException
    {
        public TwitterException(string message)
            : base(message)
        {
        }
    }

    public class ConnectionFailureException : TwitterException
    {
        public ConnectionFailureException(string message)
            : base(message)
        {
        }
    }

    public class RateLimitException : TwitterException
    {
        public RateLimitException(string message)
            : base(message)
        {
        }
    }

    public class ResourceNotFoundException : TwitterException
    {
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }
    }

    public class UnauthorizedException : TwitterException
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
