using System;
using System.Runtime.Serialization;
using System.Web;

namespace ASC.Api.Exceptions
{
    
    [Serializable]
    public class ApiBadHttpMethodException : HttpException
    {
        public ApiBadHttpMethodException(string currentMethod)
            : base(403, string.Format("{0} Not allowed", currentMethod))
        {
        }

        public ApiBadHttpMethodException()
        {
        }


        public ApiBadHttpMethodException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ApiBadHttpMethodException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}