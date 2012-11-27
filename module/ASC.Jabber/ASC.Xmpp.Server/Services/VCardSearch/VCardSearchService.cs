using System.Collections.Generic;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Services.Jabber;

namespace ASC.Xmpp.Server.Services.VCardSearch
{
	class VCardSearchService : XmppServiceBase
	{
		public override void Configure(IDictionary<string, string> properties)
		{
			DiscoInfo.AddIdentity(new DiscoIdentity("service", Name, "jud"));

			Handlers.Add(new VCardSearchHandler());
			Handlers.Add(new VCardHandler());
			Handlers.Add(new ServiceDiscoHandler(Jid));
		}
	}
}