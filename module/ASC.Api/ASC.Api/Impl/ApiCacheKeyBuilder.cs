#region usings

using System.Collections.Generic;
using System.Linq;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.Cache;

#endregion

namespace ASC.Api.Impl
{
    public class ApiCacheKeyBuilder : IApiCacheMethodKeyBuilder
    {
        #region IApiCacheMethodKeyBuilder Members

        public virtual string BuildCacheKeyForMethodCall(IApiMethodCall apiMethodCall, IEnumerable<object> callArgs, ApiContext context)
        {
            return string.Format("{0}.{1}({2}),{3}:{4}",
                                 apiMethodCall.MethodCall.DeclaringType.FullName,
                                 apiMethodCall.MethodCall.Name,
                                 string.Join(",", callArgs.Select(x => x.GetHashCode().ToString()).ToArray()),
                                 apiMethodCall.MethodCall.DeclaringType.Assembly.FullName,context);
        }

        #endregion
    }
}