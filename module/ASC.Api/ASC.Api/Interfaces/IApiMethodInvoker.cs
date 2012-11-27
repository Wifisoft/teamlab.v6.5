using System.Collections.Generic;
using ASC.Api.Impl;

namespace ASC.Api.Interfaces
{
    public interface IApiMethodInvoker
    {
        object InvokeMethod(IApiMethodCall methodToCall, object instance, IEnumerable<object> callArg, ApiContext apicontext);
    }
}