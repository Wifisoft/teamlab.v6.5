using System;
using System.Linq;
using ASC.Api.Interfaces;
using ASC.Api.Publisher;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PollAttribute : ApiCallFilter
    {
        private const string DefaultPoll = "poll";

        public string PollUrl { get; set; }

        public PollAttribute()
            : this(DefaultPoll)
        {
        }

        public PollAttribute(string pollUrl)
        {
            PollUrl = pollUrl;
        }

        public override void PostMethodCall(IApiMethodCall method, ASC.Api.Impl.ApiContext context, object methodResponce)
        {
            var pubSub = ServiceLocator.Current.GetInstance<IApiPubSub>();

            if (pubSub != null)
            {
                pubSub.PublishDataForKey(
                    method.RoutingPollUrl + ":" +
                    PubSubKeyHelper.GetKeyForRoute(context.RequestContext.RouteData.Route.GetRouteData(context.RequestContext.HttpContext)),
                    new ApiMethodCallData(){Method = method,Result = methodResponce});
            }
            base.PostMethodCall(method, context, methodResponce);
        }
    }
}