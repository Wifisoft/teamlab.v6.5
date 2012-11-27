using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ASC.Api.Impl;
using ASC.Api.Interfaces;

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class|AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public abstract class ApiCallFilter:Attribute
    {
        public virtual void PreMethodCall(IApiMethodCall method,ApiContext context, IEnumerable<object> arguments){}

        public virtual void PostMethodCall(IApiMethodCall method, ApiContext context, object methodResponce){}

        public virtual void ErrorMethodCall(IApiMethodCall method, ApiContext context, Exception e) { }
    }
}