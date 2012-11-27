#region usings

using System;
using System.Runtime.Serialization;

#endregion

namespace ASC.Api.Exceptions
{
    [Serializable]
    public class ApiArgumentMismatchException : FormatException
    {
        public ApiArgumentMismatchException(string key, Type failedType) : this(key, failedType, null)
        {
        }

        public ApiArgumentMismatchException(string key, Type failedType, Exception inner)
            : base(string.Format("Failed to convert parameter '{0}' to type '{1}'", key, failedType), inner)
        {
        }

        public ApiArgumentMismatchException()
        {
        }


        public ApiArgumentMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ApiArgumentMismatchException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}