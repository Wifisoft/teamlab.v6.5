using System.Collections.Generic;
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.protocol.iq.disco;

namespace ASC.Xmpp.Server.Jabber
{
	class TestService : XmppServiceBase
	{
		public TestService()
		{
			Name = "Multi User Chat";
			DiscoInfo.AddIdentity(new DiscoIdentity("muc", Name, "im"));
		}
	}
}
