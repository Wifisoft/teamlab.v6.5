#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ASC.Api.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

#endregion

namespace ASC.Api
{
    public static class ApiSetup
    {
        public enum ApiSetupState
        {
            UnityRegitrations,
            EntryPointConfigurations,
            BootInitializations,
            RouteRegistration,
            DocumentationBuilding,
            UnityAutoRegitrations
        }

        private static readonly UnityServiceLocator Locator;

        private static readonly IDictionary<ApiSetupState, IEnumerable<Exception>> _initialization = new Dictionary<ApiSetupState, IEnumerable<Exception>>();

        public static UnityContainer Container { get; private set; }

        public static IDictionary<ApiSetupState, IEnumerable<Exception>> Initialization
        {
            get { return _initialization; }
        }


        static ApiSetup()
        {
            try
            {

                //Create
                Container = new UnityContainer();
                Locator = new UnityServiceLocator(Container);
                //Add entlib extensions
                Container.AddNewExtension<EnterpriseLibraryCoreExtension>();
#pragma warning disable 612,618
                Container.AddNewExtension<CachingBlockExtension>();
#pragma warning restore 612,618
                ServiceLocator.SetLocatorProvider(() => Locator);
                Container.LoadConfiguration("api");
                ApiDefaultConfig.DoDefaultRegistrations(Container);
                //NOTE: disabled for now
                //try
                //{
                //    ApiDefaultConfig.DoAutomaticConfiguration(Container);
                //    Initialization.Add(ApiSetupState.UnityAutoRegitrations, null);
                //}
                //catch (Exception e)
                //{
                //    Initialization.Add(ApiSetupState.UnityAutoRegitrations, new[] { e });
                //}
            }
            catch (Exception e)
            {
                Initialization.Add(ApiSetupState.UnityRegitrations, new[] { e });
                throw;
            }
            Initialization.Add(ApiSetupState.UnityRegitrations, null);
        }



        public static void RegisterRoutes()
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            try
            {
                var registrators = Container.ResolveAll<IApiRouteRegistrator>();
                foreach (var registrator in registrators)
                {
                    registrator.RegisterRoutes(routes);
                }
            }
            catch (Exception e)
            {
                Initialization.Add(ApiSetupState.RouteRegistration, new[] { e });
                throw;
            }
            Initialization.Add(ApiSetupState.RouteRegistration, null);
        }

        public static IUnityContainer ConfigureEntryPoints()
        {
            try
            {
                //Do boot stuff
                var configurator = Container.Resolve<IApiRouteConfigurator>();
                configurator.RegisterEntryPoints();
            }
            catch (Exception e)
            {
                Initialization.Add(ApiSetupState.EntryPointConfigurations, new[] { e });
                throw;
            }
            Initialization.Add(ApiSetupState.EntryPointConfigurations, null);
            //Do boot auto search
            try
            {
                var boot = Container.ResolveAll<IApiBootstrapper>();
                foreach (var apiBootstrapper in boot)
                {
                    apiBootstrapper.Configure();
                }
            }
            catch (Exception e)
            {
                Initialization.Add(ApiSetupState.BootInitializations, new[] { e });
                throw;
            }
            Initialization.Add(ApiSetupState.BootInitializations, null);
            return Container;
        }


    }

}