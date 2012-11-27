#region usings

using System;
using System.Runtime.Serialization;
using ASC.Api.Enums;
using ASC.Api.Impl;

#endregion

namespace ASC.Api.Interfaces
{
    public interface IApiStandartResponce
    {
        object Response { get; set; }
        ErrorWrapper Error { get; set; }
        ApiStatus Status { get; set; }
        long Code { get; set; }
        long Count { get; set; }
        long StartIndex { get; set; }
        long? NextPage { get; set; }
        long? TotalCount { get; set; }
        ApiContext ApiContext { get; set; }
    }

    [DataContract(Name = "error", Namespace = "")]
    public class ErrorWrapper
    {
        public ErrorWrapper()
        {
        }

        public ErrorWrapper(Exception exception)
        {
            //Unwrap
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            Message = exception.Message;
#if (DEBUG)
            Type = exception.GetType().ToString();
            Stack = exception.StackTrace;
#endif
        }

        [DataMember(Name = "message", EmitDefaultValue = false, Order = 2)]
        public string Message { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false, Order = 3)]
        public string Type { get; set; }

        [DataMember(Name = "stack", EmitDefaultValue = false, Order = 3)]
        public string Stack { get; set; }
    }
}