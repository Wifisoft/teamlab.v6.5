using System;
using System.Collections.Generic;

namespace ASC.Api.Interfaces
{
    public interface IApiParamInspector
    {
        IEnumerable<object> InspectParams(IEnumerable<object> parameters);
    }
}