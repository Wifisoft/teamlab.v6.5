using System;
using System.Collections.Generic;
using ASC.Common.Module;
using ASC.Web.Host.Config;
using ASC.Web.Host.HttpRequestProcessor;
using log4net;

namespace ASC.Web.Host
{
    public class WebServerLauncher : IServiceController
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Host");
        private readonly List<IServer> webServers = new List<IServer>();


        public string ServiceName
        {
            get { return "Web server"; }
        }


        public void Start()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ServerConfiguration.Configure();
            foreach (SiteElement s in ServerConfiguration.Sites)
            {
                var webServer = new Server(s.Binding, s.Path);
                webServer.Start();

                log.InfoFormat("Web server start site {0} on {1}", s.Path, s.Binding);
                webServers.Add(webServer);
            }
        }

        public void Stop()
        {
            foreach (var webServer in webServers)
            {
                webServer.Stop();
            }
            webServers.Clear();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }
    }
}
