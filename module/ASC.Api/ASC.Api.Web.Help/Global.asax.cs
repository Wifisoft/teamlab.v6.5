using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Web.Help.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace ASC.Api.Web.Help
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static CacheManifest CacheManifest = new CacheManifest();
        private static object _loadLock = new object();

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Cache", "web.appcache", new
                                                         {
                                                             controller = "CacheManifest",
                                                             action = "GetCacheManifest"
                                                         });

            routes.MapRoute(
                "Docs", // Route name
                "docs/{section}/{type}/{*url}", // URL with parameters
                new
                    {
                        controller = "Documentation",
                        action = "Index",
                        section = UrlParameter.Optional,
                        url = UrlParameter.Optional,
                        type = UrlParameter.Optional
                    } // Parameter defaults
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Help", action = "Basic", id = UrlParameter.Optional } // Parameter defaults
                );

        }

        protected void Application_Start()
        {
            

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);



            ClassNamePluralizer.LoadAndWatch(HttpContext.Current.Server.MapPath("~/App_Data/class_descriptions.xml"));

        }

        private static bool _isInitialized = false;

        protected void Application_BeginRequest(Object sender,
    EventArgs e)
        {
            if (!_isInitialized)
            {
                lock (_loadLock)
                {
                    if (!_isInitialized)
                    {
                        _isInitialized = true;

                        //Register cache
                        CacheManifest.AddServerFolder(new HttpContextWrapper(HttpContext.Current), "~/Content/images", "*.*");
                        CacheManifest.AddServerFolder(new HttpContextWrapper(HttpContext.Current), "~/Content/img", "*.*");
                        CacheManifest.AddServerFolder(new HttpContextWrapper(HttpContext.Current), "~/Content/sprite", "*.*");
                        CacheManifest.AddServerFile(new HttpContextWrapper(HttpContext.Current), "~/Scripts/libs/modernizr-1.7.min.js");
                        CacheManifest.AddCached(new Uri("/", UriKind.Relative));
                        CacheManifest.AddCached(new Uri("/Help/Authentication", UriKind.Relative));
                        CacheManifest.AddCached(new Uri("/Help/Faq", UriKind.Relative));
                        CacheManifest.AddCached(new Uri("/Help/Filters", UriKind.Relative));
                        CacheManifest.AddCached(new Uri("/Help/Batch", UriKind.Relative));
                        CacheManifest.AddOnline(new Uri("/Documentation/Search", UriKind.Relative));
                        CacheManifest.AddFallback(new Uri("/Documentation/Search", UriKind.Relative), new Uri("/docs/notfound", UriKind.Relative));

                        Documentation.Load(AppDomain.CurrentDomain.RelativeSearchPath, HttpContext.Current.Server.MapPath("~/SearchIndex/"));
                        Documentation.GenerateRouteMap();
                    }
                }
           }
        }
    }
}