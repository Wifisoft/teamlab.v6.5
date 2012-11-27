using System;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public class XmppStanzaHandler : IXmppStanzaHandler
	{
		public virtual IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			return null;
		}

		public virtual void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{

		}

		public virtual void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context)
		{

		}

		public virtual void OnRegister(IServiceProvider serviceProvider)
		{

		}

		public virtual void OnUnregister(IServiceProvider serviceProvider)
		{

		}
	}
}
