using System;
using System.Collections.Generic;
using ASC.Api.Interfaces;

namespace ASC.Specific
{
    public class AntiXssParamInspector:IApiParamInspector
    {
        public IEnumerable<object> InspectParams(IEnumerable<object> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is string)
                {
                    var safeString = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(parameter as string);
                    yield return safeString;
                }
                else
                {
                    yield return parameter;    
                }
            }
        }
    }
}