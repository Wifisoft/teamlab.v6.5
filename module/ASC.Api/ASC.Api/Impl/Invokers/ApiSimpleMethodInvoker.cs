using System.Collections.Generic;
using System.Linq;
using ASC.Api.Interfaces;

namespace ASC.Api.Impl.Invokers
{
    internal class ApiSimpleMethodInvoker : IApiMethodInvoker
    {
        #region IApiMethodInvoker Members

        public virtual object InvokeMethod(IApiMethodCall methodToCall, object instance, IEnumerable<object> callArg, ApiContext apicontext)
        {
            return methodToCall.Invoke(instance, callArg.ToArray());
        }

        #endregion
    }
}