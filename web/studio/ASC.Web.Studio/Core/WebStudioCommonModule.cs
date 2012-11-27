using System;
using System.Globalization;
using System.Threading;
using System.Web;
using ASC.Core;
using ASC.Web.Core;

namespace ASC.Web.Studio.Core
{
    public class WebStudioCommonModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AcquireRequestState += AcquireRequestState;
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        public void Dispose()
        {
        }


        private void AcquireRequestState(object sender, EventArgs e)
        {
            Authenticate();
            ResolveUserCulture();
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && !RestrictRewriter(HttpContext.Current.Request.Url))
            {
                HttpContext.Current.PushRewritenUri();
            }
        }

        private void EndRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && !RestrictRewriter(HttpContext.Current.Request.Url))
            {
                HttpContext.Current.PopRewritenUri();
            }
        }


        private void Authenticate()
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null && !SecurityContext.IsAuthenticated)
            {
                var cookie = CookiesManager.GetCookies(CookiesType.AuthKey);
                if (!string.IsNullOrEmpty(cookie))
                {
                    SecurityContext.AuthenticateMe(cookie);
                }
            }
        }

        private void ResolveUserCulture()
        {
            CultureInfo culture = null;
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null)
            {
                culture = tenant.GetCulture();
                if (Thread.CurrentThread.CurrentCulture != culture)
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                }
                if (Thread.CurrentThread.CurrentUICulture != culture)
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
            if (SecurityContext.IsAuthenticated)
            {
                culture = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture();
                if (Thread.CurrentThread.CurrentCulture != culture)
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                }
                if (Thread.CurrentThread.CurrentUICulture != culture)
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
        }


        private bool RestrictRewriter(Uri uri)
        {
            return uri.AbsolutePath.IndexOf(".svc", StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}