using System;
using System.Runtime.Serialization;

namespace ASC.SocialMedia
{
    public class SocialMediaException : Exception
    {
        public SocialMediaException()
            : base()
        {
        }

        public SocialMediaException(string message)
            : base(message)
        {
        }

        public SocialMediaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SocialMediaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}