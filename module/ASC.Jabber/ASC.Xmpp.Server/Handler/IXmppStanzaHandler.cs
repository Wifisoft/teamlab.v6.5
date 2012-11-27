using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public interface IXmppStanzaHandler : IXmppHandler
	{
		IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context);

		void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context);

		void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context);
	}
}
