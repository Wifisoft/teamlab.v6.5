using System.Security.Principal;
using ASC.Web.Host.HttpRequestProcessor;

namespace ASC.Web.Host.HttpHandlers
{
	class HttpHandlerContext
	{
		public Server Server
		{
			get;
			private set;
		}

		public HttpRequestProcessor.Host Host
		{
			get;
			private set;
		}

		public Connection Connection
		{
			get;
			private set;
		}

		public IIdentity Identity
		{
			get;
			private set;
		}

		public HttpHandlerContext(Server server, HttpRequestProcessor.Host host, Connection connection, IIdentity identity)
		{
			Server = server;
			Host = host;
			Connection = connection;
			Identity = identity;
		}
	}
}