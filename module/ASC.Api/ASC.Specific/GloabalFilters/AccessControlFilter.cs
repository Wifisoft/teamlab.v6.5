using System;
using System.Linq;
using ASC.Api.Attributes;

namespace ASC.Specific.GloabalFilters
{
    public class AccessControlFilter : ApiCallFilter
    {
        public override void PostMethodCall(ASC.Api.Interfaces.IApiMethodCall method, ASC.Api.Impl.ApiContext context, object methodResponce)
        {
            try
            {
                context.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            }
            catch (Exception)
            {
                
            }
            base.PostMethodCall(method, context, methodResponce);
        }
    }
}