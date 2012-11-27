using System;
using System.Web;
using log4net;

namespace ASC.Api
{
    public class ApiSetupHttpModule : IHttpModule
    {
        private static bool initialized = false;


        public void Init(HttpApplication context)
        {
            if (initialized)
            {
                return;
            }

            try
            {
                ApiSetup.ConfigureEntryPoints();
                ApiSetup.RegisterRoutes();
                initialized = true;
            }
            catch (Exception err)
            {
                if (err is TypeInitializationException && err.InnerException != null)
                {
                    err = err.InnerException;
                }
                LogManager.GetLogger(GetType()).Error(err);
            }
        }

        public void Dispose()
        {
        }
    }
}
