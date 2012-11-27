using System;
using System.Runtime.Serialization;
using ASC.Api.Interfaces;

namespace ASC.Api.Exceptions
{
    [Serializable]
    public class ApiDuplicateRouteException : Exception
    {
        public ApiDuplicateRouteException(IApiMethodCall currentMethod, IApiMethodCall registeredMethod)
            : base(string.Format("route '{0}' is already registered to '{1}'", currentMethod, registeredMethod))
        {
        }

        public ApiDuplicateRouteException()
        {
        }


        public ApiDuplicateRouteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ApiDuplicateRouteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}