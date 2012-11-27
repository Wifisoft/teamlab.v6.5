using System;
using System.Collections.Generic;
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Core;
using ASC.Web.Core;
using System.Runtime.Remoting.Messaging;

namespace ASC.Specific.GloabalFilters
{
    public class ProductSecurityFilter : ApiCallFilter
    {
        private static readonly IDictionary<string, Guid> products;


        static ProductSecurityFilter()
        {
            var blog = new Guid("6a598c74-91ae-437d-a5f4-ad339bd11bb2");
            var bookmark = new Guid("28b10049-dd20-4f54-b986-873bc14ccfc7");
            var forum = new Guid("853b6eb9-73ee-438d-9b09-8ffeedf36234");
            var news = new Guid("3cfd481b-46f2-4a4a-b55c-b8c0c9def02c");
            var wiki = new Guid("742cf945-cbbc-4a57-82d6-1600a12cf8ca");
            var photo = new Guid("9d51954f-db9b-4aed-94e3-ed70b914e101");
            var community = new Guid("ea942538-e68e-4907-9394-035336ee0ba8");
            var crm = new Guid("6743007c-6f95-4d20-8c88-a8601ce5e76d");
            var files = new Guid("e67be73d-f9ae-4ce1-8fec-1880cb518cb4");
            var project = new Guid("1e044602-43b5-4d79-82f3-fd6208a11960");
            var calendar = new Guid("32d24cb5-7ece-4606-9c94-19216ba42086");
            products = new Dictionary<string, Guid>
            {
                { "blog", blog },
                { "bookmark", bookmark },
                { "event", news },
                { "forum", forum },
                { "photo", photo },
                { "wiki", wiki },
                { "crm", crm },
                { "files", files },
                { "project", project },
                { "calendar", calendar },
            };
        }


        public override void PreMethodCall(IApiMethodCall method, ApiContext context, IEnumerable<object> arguments)
        {
            if (!SecurityContext.IsAuthenticated) return;

            var pid = FindProduct(method.Name);
            if (pid != Guid.Empty)
            {
                if (CallContext.GetData("asc.web.product_id") == null)
                {
                    CallContext.SetData("asc.web.product_id", pid);
                }
                if (!WebItemSecurity.IsAvailableForUser(pid.ToString(), SecurityContext.CurrentAccount.ID))
                {
                    context.RequestContext.HttpContext.Response.StatusCode = 403;
                    context.RequestContext.HttpContext.Response.StatusDescription = "Access denied.";
                    context.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    throw new System.Security.SecurityException(string.Format("Product {0} denied for user {1}", method.Name, SecurityContext.CurrentAccount));
                }
            }
        }


        private Guid FindProduct(string apiname)
        {
            return !string.IsNullOrEmpty(apiname) && products.ContainsKey(apiname) ? products[apiname] : Guid.Empty;
        }
    }
}
