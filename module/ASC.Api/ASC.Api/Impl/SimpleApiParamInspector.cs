using System.Collections.Generic;
using ASC.Api.Interfaces;

namespace ASC.Api.Impl
{
    class SimpleApiParamInspector : IApiParamInspector
    {
        public IEnumerable<object> InspectParams(IEnumerable<object> parameters)
        {
            return parameters;
        }
    }
}