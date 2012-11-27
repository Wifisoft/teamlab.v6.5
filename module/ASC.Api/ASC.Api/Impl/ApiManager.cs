#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Routing;
using ASC.Api.Attributes;
using ASC.Api.Exceptions;
using ASC.Api.Interfaces;
using ASC.Api.Logging;
using ASC.Api.Publisher;
using ASC.Api.Routing;
using ASC.Api.Utils;
using ASC.Common.Web;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Microsoft.Practices.Unity;
using System.Globalization;

#endregion

namespace ASC.Api.Impl
{

    public class ApiManager : IApiManager
    {
        private readonly IUnityContainer _container;
        private readonly IApiMethodInvoker _invoker;
        private readonly List<IApiParamInspector> _paramInspectors;
        private readonly IEnumerable<IApiMethodCall> _methods;

        [Dependency]
        public IApiConfiguration Config { get; set; }

        [Dependency]
        public IApiArgumentBuilder ArgumentBuilder { get; set; }

        [Dependency]
        public ILog Log { get; set; }

        #region IApiManager Members

        public ApiManager(IUnityContainer container, IApiMethodInvoker invoker)
        {
            _container = container;
            _invoker = invoker;
            _paramInspectors = _container.ResolveAll<IApiParamInspector>().ToList();
            _methods = _container.Resolve<IEnumerable<IApiMethodCall>>();

        }

        public object InvokeMethod(IApiMethodCall methodToCall, ApiContext apicontext)
        {
            if (apicontext == null) throw new ArgumentNullException("apicontext");

            if (methodToCall != null)
            {
                var context = apicontext.RequestContext;

                Log.Debug("Method to call={0}", methodToCall);
                object instance = _container.Resolve(methodToCall.ApiClassType, new DependencyOverride(typeof(ApiContext), apicontext));

                //try convert params
                var callArg = ArgumentBuilder.BuildCallingArguments(context, methodToCall);
                if (_paramInspectors.Any())
                {
                    callArg = _paramInspectors.Aggregate(callArg,
                                                   (current, apiParamInspector) =>
                                                   apiParamInspector.InspectParams(current));
                }

                Log.Debug("Arguments count: {0}", callArg == null ? "empty" : callArg.Count().ToString());


                try
                {
                    //Pre call filter
                    methodToCall.Filters.ForEach(x =>x.PreMethodCall(methodToCall, apicontext, callArg));
                    object result = _invoker.InvokeMethod(methodToCall, instance, callArg,apicontext);
                    //Post call filter
                    methodToCall.Filters.ForEach(x => x.PostMethodCall(methodToCall, apicontext, result));
                    return result;
                }
                catch (Exception e)
                {
                    methodToCall.Filters.ForEach(x => x.ErrorMethodCall(methodToCall, apicontext, e));
                    throw;
                }
            }
            throw new ApiBadHttpMethodException();
        }

        public IApiMethodCall GetMethod(string routeUrl, string httpMethod)
        {
            var methodToCall =
                _methods.Where(
                    x =>
                    x.FullPath ==
                    StringUtils.TrimExtension(routeUrl, Config.GetBasePath().Length)).SingleOrDefault(x => x.HttpMethod.Equals(httpMethod,StringComparison.OrdinalIgnoreCase));
            return methodToCall;
        }


        #endregion

    }
}