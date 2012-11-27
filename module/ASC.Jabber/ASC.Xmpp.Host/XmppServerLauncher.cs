using System.ServiceModel;
using ASC.Common.Module;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Host
{
    public class XmppServerLauncher : IServiceController
    {
        private ServiceHost host;
        private XmppServer xmppServer;


        public string ServiceName
        {
            get { return "Jabber"; }
        }

        public void Start()
        {
            xmppServer = new XmppServer();
            JabberConfiguration.Configure(xmppServer);
            xmppServer.StartListen();

            var jabberService = new JabberService(xmppServer);

            host = new ServiceHost(jabberService);
            host.Open();
        }

        public void Stop()
        {
            if (xmppServer != null)
            {
                xmppServer.StopListen();
                xmppServer = null;
            }
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }
    }
}
