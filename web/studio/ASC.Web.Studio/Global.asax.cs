using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Security;
using AjaxPro.Security;
using ASC.Common.Data;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Notify.Patterns;
using ASC.Web.Core;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Security.Ajax;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SearchHandlers;
using ASC.Web.Studio.Utility;
using log4net.Config;
using TMResourceData;

namespace ASC.Web.Studio
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();
            DbRegistry.Configure();
            InitializeDbResources();
            AjaxSecurityChecker.Instance.CheckMethodPermissions += AjaxCheckMethodPermissions;
            NotifyConfiguration.Configure();
            WebItemManager.Instance.LoadItems();
            SearchHandlerManager.Registry(new StudioSearchHandler());
            SearchHandlerManager.Registry(new EmployeeSearchHendler());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var wizardSettings = SettingsManager.Instance.LoadSettings<WizardSettings>(TenantProvider.CurrentTenantID);
            if (!SecurityContext.IsAuthenticated && wizardSettings.Completed)
            {
                MainPage.AutoAuthByCookies();
            }
            else if (Request.Url.AbsoluteUri.IndexOf("ajaxpro") < 0 && SecurityContext.IsAuthenticated)
            {
                WebItemManager.Instance.ItemGlobalHandlers.Login(SecurityContext.CurrentAccount.ID);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            CommonControlsConfigurer.FCKClearTempStore(Session);
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var currentTenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (currentTenant == null)
            {
                Response.Redirect(SetupInfo.NoTenantRedirectURL, true);
            }
            else if (currentTenant.Status != ASC.Core.Tenants.TenantStatus.Active)
            {
                var ind = Request.Url.AbsoluteUri.IndexOf(VirtualPathUtility.ToAbsolute("~/confirm.aspx"), StringComparison.InvariantCultureIgnoreCase);
                if (currentTenant.Status == TenantStatus.RemovePending || !(ind >= 0 && currentTenant.Status == TenantStatus.Suspended))
                {
                    Response.Redirect(SetupInfo.NoTenantRedirectURL, true);
                }
            }

            CheckBasicAuth(((HttpApplication)sender).Context);
            FixFlashPlayerCookieBug();
        }

        private void InitializeDbResources()
        {
            if (ConfigurationManager.AppSettings["resources.from-db"] == "true")
            {
                AssemblyWork.UploadResourceData(AppDomain.CurrentDomain.GetAssemblies());
                XmlPatternProvider.CreateResourceManager = (key, assembly) => new DBResourceManager(key.Substring(key.LastIndexOf('.') + 1) + ".resx", new ResourceManager(key, assembly));
                AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => AssemblyWork.UploadResourceData(AppDomain.CurrentDomain.GetAssemblies());
            }
        }

        private bool AjaxCheckMethodPermissions(MethodInfo method)
        {
            var authorized = SecurityContext.IsAuthenticated;
            if (!authorized && HttpContext.Current != null)
            {
                authorized = method.GetCustomAttributes(typeof(AjaxSecurityAttribute), true)
                    .Cast<AjaxSecurityAttribute>()
                    .Any(a => a.CheckAuthorization(HttpContext.Current));
                if (!authorized)
                {
                    authorized = SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey));
                }
            }
            return authorized;
        }

        /// <summary>
        /// Fix for the Flash Player Cookie bug in Non-IE browsers.
        /// Since Flash Player always sends the IE cookies even in FireFox we have to bypass the cookies by sending
        /// the values as part of the POST or GET and overwrite the cookies with the passed in values.
        /// The theory is that at this point (BeginRequest) the cookies have not been read by
        /// the Session and Authentication logic and if we update the cookies here we'll get our
        /// Session and Authentication restored correctly.
        /// </summary>
        private void FixFlashPlayerCookieBug()
        {
            try
            {
                if (HttpContext.Current.Request.Form["ASPSESSID"] != null)
                {
                    UpdateCookie("ASP.NET_SESSIONID", HttpContext.Current.Request.Form["ASPSESSID"]);
                }
                else if (HttpContext.Current.Request.QueryString["ASPSESSID"] != null)
                {
                    UpdateCookie("ASP.NET_SESSIONID", HttpContext.Current.Request.QueryString["ASPSESSID"]);
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.Write("Error Initializing Session");
            }
            try
            {
                if (HttpContext.Current.Request.Form["AUTHID"] != null)
                {
                    UpdateCookie(FormsAuthentication.FormsCookieName, HttpContext.Current.Request.Form["AUTHID"]);
                }
                else if (HttpContext.Current.Request.QueryString["AUTHID"] != null)
                {
                    UpdateCookie(FormsAuthentication.FormsCookieName, HttpContext.Current.Request.QueryString["AUTHID"]);
                }

            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.Write("Error Initializing Forms Authentication");
            }
        }

        private void CheckBasicAuth(HttpContext context)
        {
            string authCookie;
            if (AuthorizationHelper.ProcessBasicAuthorization(context, out authCookie))
            {
                CookiesManager.SetCookies(CookiesType.AuthKey, authCookie);
            }
        }

        private void UpdateCookie(string name, string value)
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(name);
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }
            cookie.Value = value;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }
    }
}