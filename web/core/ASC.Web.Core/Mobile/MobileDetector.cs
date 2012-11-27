using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using ASC.Core;
using log4net;

namespace ASC.Web.Core.Mobile
{
    public class MobileDetector:IHttpModule
    {
        private const string TenantReplacePattern = "%tenant%";

        public static void RedirectToMobileVersionIfNeeded(string mobileAddress, HttpContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(mobileAddress) && UrlCheckRegex.IsMatch(context.Request.GetUrlRewriter().ToString()) && IsUserAgentMatchesMobile(context.Request.UserAgent, RedirectRegex) && !CookiesManager.IsMobileBlocked())
                {
                    //TODO: check user status to display desktop or mobile version
                    if (mobileAddress.StartsWith("~"))
                    {
                        //Resolve to current
                        mobileAddress = VirtualPathUtility.ToAbsolute(mobileAddress);
                    }
                    if (mobileAddress.Contains(TenantReplacePattern))
                    {
                        var tennant = CoreContext.TenantManager.GetCurrentTenant();
                        mobileAddress = mobileAddress.Replace(TenantReplacePattern, tennant.TenantAlias);
                    }

                    var redirectUri = Uri.IsWellFormedUriString(mobileAddress, UriKind.Absolute) ? new Uri(mobileAddress) : new Uri(context.Request.GetUrlRewriter(), mobileAddress);
                    if (!redirectUri.Equals(context.Request.GetUrlRewriter()))
                    {
                        UriBuilder builder = new UriBuilder(redirectUri);
                        builder.Path += context.Request.GetUrlRewriter().AbsolutePath;
                        builder.Query += context.Request.GetUrlRewriter().Query.TrimStart('?');
                        redirectUri = builder.Uri;
                        LogManager.GetLogger("ASC.Mobile.Redirect").DebugFormat("Redirecting url:'{1}' to mobile. UA={0}", context.Request.UserAgent, context.Request.GetUrlRewriter());
                        context.Response.Redirect(redirectUri.ToString(), true);
                    }
                }

            }
            catch(ThreadAbortException)
            {
                //Don't do nothing
            }
            catch (Exception e)
            {
                //If error happens it's not so bad as you may think. We won't redirect user to mobile version.
                LogManager.GetLogger("ASC.Mobile.Redirect").Error("failed to redirect user to mobile", e);
            }
        }

        private static volatile Regex _userAgentRegex;
        private static volatile Regex _urlCheckRegex;
        private static volatile Regex _redirectRegex;
        private static readonly object SyncLock = new object();

        private const string UserAgentRegexDefaultPattern =
            @"android|avantgo|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od|ad)|iris|kindle|lge |maemo|midp|mmp|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\\/|plucker|pocket|psp|symbian|treo|up\\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino";

        private const string UrlCheckPattern = @"^((?!((/api/)|(/mobile/))).)*$";//Note: don't touch api and mobile

        public static string DetectMobileRegex
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["mobile.regex"] ??
                       UserAgentRegexDefaultPattern;
            }
        }

        private static Regex UrlCheckRegex
        {
            get
            {
                if (_urlCheckRegex == null)
                {
                    lock (SyncLock)
                    {
                        if (_urlCheckRegex == null)
                        {
                            _urlCheckRegex = new Regex(
                                System.Configuration.ConfigurationManager.AppSettings["mobile.urlregex"] ?? UrlCheckPattern, 
                                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                        }
                    }
                }
                return _urlCheckRegex;
            }
        }

        private static Regex UserAgentRegex
        {
            get
            {
                if (_userAgentRegex==null)
                {
                    lock (SyncLock)
                    {
                        if (_userAgentRegex==null)
                        {
                            _userAgentRegex = new Regex(DetectMobileRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                        }
                    }
                }
                return _userAgentRegex;
            }
        }

        private static Regex RedirectRegex
        {
            get
            {
                if (_redirectRegex == null)
                {
                    lock (SyncLock)
                    {
                        if (_redirectRegex == null)
                        {
                            _redirectRegex = new Regex(System.Configuration.ConfigurationManager.AppSettings["mobile.redirect-regex"] ?? UserAgentRegexDefaultPattern,
                                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                        }
                    }
                }
                return _redirectRegex;
            }
        }

        public static bool IsRequestMatchesMobile(HttpContext context)
        {
            //Check user agent
            var u = context.Request.UserAgent;
            return IsUserAgentMatchesMobile(u);
        }

        public static bool IsUserAgentMatchesMobile(string userAgnet)
        {
            return IsUserAgentMatchesMobile(userAgnet, UserAgentRegex);
        }

        public static bool IsUserAgentMatchesMobile(string userAgnet, Regex regex)
        {
            return !string.IsNullOrEmpty(userAgnet) && (regex.IsMatch(userAgnet));
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest+=BeginRequest;
        }

        private static void BeginRequest(object sender, EventArgs e)
        {
            //Detect mobile support on begin request
            RedirectToMobileVersionIfNeeded(System.Configuration.ConfigurationManager.AppSettings["mobile.redirect-url"],((HttpApplication)sender).Context);
        }

        public void Dispose()
        {
            
        }
    }


}