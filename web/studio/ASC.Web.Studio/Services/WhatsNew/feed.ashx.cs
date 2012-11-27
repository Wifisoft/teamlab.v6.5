using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Core;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Services.WhatsNew
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    public class feed : IHttpHandler
    {
        public const string HandlerBasePath = "~/services/whatsnew/";
        public const string HandlerPath = "~/services/whatsnew/feed.ashx";

        private const string TitleParam = "c";
        private const string ProductParam = "pid";
        private const string ModuleParam = "mid";
        private const string UserParam = "uid";
        private const string ActionTypeParam = "t";
        private const string ContainerParam = "cid";

        public static string RenderRssMetaForLocation(string title)
        {
            IProduct product;
            IModule module;
            CommonLinkUtility.GetLocationByRequest(out product, out module);
            return RenderRssMeta(title, product != null ? product.ID.ToString("D") : null, module != null ? module.ID.ToString("D") : null, null);
        }

        public static string RenderRssMeta(string title)
        {
            return RenderRssMeta(title, CommonLinkUtility.GetProductID().ToString("D"));
        }

        public static string RenderRssMeta(string title, string productId)
        {
            return RenderRssMeta(title, productId, null);
        }

        public static string RenderRssMeta(string title, string productId, int? actionType)
        {
            return RenderRssMeta(title, productId, null, actionType);
        }

        public static string RenderRssMeta(string title, string productId, string module, int? actionType)
        {
            return RenderRssMeta(title, productId, module, actionType, null);
        }

        public static string RenderRssMeta(string title, string productId, string module, int? actionType, string containerParam)
        {
            return
                string.Format(@"<link rel=""alternate"" type=""application/atom+xml"" title=""{0}"" href=""{1}"" />", title, GetUrl(productId, module, actionType, containerParam, title));
        }

        private static string GetUrl(string productId, string module, int? actionType, string containerParam, string title)
        {
            var urlparams = new Dictionary<string, string>();
            var queryString = new StringBuilder();
            queryString.Append("?");

            if (!String.IsNullOrEmpty(productId))
                urlparams.Add(ProductParam, productId);

            if (!String.IsNullOrEmpty(module))
                urlparams.Add(ModuleParam, module);

            if (!String.IsNullOrEmpty(containerParam))
                urlparams.Add(ContainerParam, containerParam);

            if (!String.IsNullOrEmpty(title))
                urlparams.Add(TitleParam, title);

            if (actionType.HasValue)
                urlparams.Add(ActionTypeParam, actionType.Value.ToString());

            //Build queryParams string
            //
            foreach (var urlparam in urlparams.Where(urlparam => urlparam.Value != null))
            {
                queryString.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(urlparam.Key),
                                         HttpUtility.UrlEncode(urlparam.Value));
            }
            return CommonLinkUtility.GetFullAbsolutePath(HandlerPath) + queryString.ToString().TrimEnd('&');
        }

        public void ProcessRequest(HttpContext context)
        {
            //Process authorization
            if (!ProcessAuthorization(context))
            {
                AccessDenied(context);
                return;
            }
            var productId = Helpers.ParseGuid(context.Request[ProductParam]);
            var moduleIds = Helpers.ParseGuids(context.Request[ModuleParam]);
            var actionType = Helpers.ParseInt(context.Request[ActionTypeParam]);
            var containerIds = Helpers.Tokenize(context.Request[ContainerParam]);
            var title = context.Request[TitleParam];
            if (string.IsNullOrEmpty(title)) title = Resources.Resource.RecentActivity;
            var userIds = Helpers.ParseGuid(context.Request[UserParam]);
            if (userIds.HasValue) throw new NotImplementedException();

            var userActivity = UserActivityManager.GetUserActivities(
                TenantProvider.CurrentTenantID,
                null,
                productId.GetValueOrDefault(),
                moduleIds,
                actionType.GetValueOrDefault(),
                containerIds,
                0, 100);

            var feedItems = userActivity.Select(x =>
            {
                var user = CoreContext.UserManager.GetUsers(x.UserID);
                var veloContext = Formatter.PrepareContext(x, user);
                var item = new SyndicationItem(
                    HttpUtility.HtmlDecode(Formatter.Format(context, "title.txt", veloContext)),
                    SyndicationContent.CreateHtmlContent(Formatter.Format(context, "pattern.html", veloContext)),
                    new Uri(CommonLinkUtility.GetFullAbsolutePath(x.URL)),
                    x.ID.ToString(),
                    new DateTimeOffset(TenantUtil.DateTimeToUtc(x.Date)))
                    {
                        PublishDate = x.Date,
                        Summary = SyndicationContent.CreatePlaintextContent(Formatter.Format(context, "pattern.txt", veloContext))
                    };
                var person = new SyndicationPerson(user.Email) { Name = user.UserName };
                if (productId.HasValue)
                {
                    person.Uri = CommonLinkUtility.GetUserProfile(user.ID, productId.Value);
                }
                item.Authors.Add(person);
                return item;
            });

            var lastUpdate = new DateTimeOffset(DateTime.UtcNow);
            if (userActivity.Count > 0)
            {
                lastUpdate = new DateTimeOffset(userActivity.Max(x => x.Date));
            }

            var feed = new SyndicationFeed(
                title,
                Resources.Resource.RecentActivity,
                new Uri(context.Request.GetUrlRewriter(), VirtualPathUtility.ToAbsolute(Studio.WhatsNew.PageUrl)),
                TenantProvider.CurrentTenantID.ToString(),
                lastUpdate, feedItems);

            var rssFormatter = new Atom10FeedFormatter(feed);
            
            //Set writer settings
            var settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;


            using (var writer = XmlWriter.Create(context.Response.Output,settings))
            {
                rssFormatter.WriteTo(writer);
            }
            context.Response.Charset = Encoding.UTF8.WebName;
            context.Response.ContentType = "application/atom+xml";
        }

        private static bool ProcessAuthorization(HttpContext context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                //Try studio auth
                try
                {
                    var cookiesKey = CookiesManager.GetCookies(CookiesType.AuthKey);
                    if (!SecurityContext.AuthenticateMe(cookiesKey))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                catch (Exception)
                {
                    return AuthorizationHelper.ProcessBasicAuthorization(context);
                }
            }
            return SecurityContext.IsAuthenticated;
        }

        private static void AccessDenied(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.StatusDescription = "Access Denied";
            string realm = String.Format("Basic Realm=\"{0}\"", context.Request.GetUrlRewriter().Host);
            context.Response.AppendHeader("WWW-Authenticate", realm);
            context.Response.Write("401 Access Denied");
        }


        public bool IsReusable
        {
            get { return false; }
        }
    }
}
