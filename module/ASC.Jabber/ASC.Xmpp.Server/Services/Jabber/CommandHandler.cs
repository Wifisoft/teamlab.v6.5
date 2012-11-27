using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.commands;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Command))]
    class CommandHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (!iq.HasTo || !iq.To.HasUser) return XmppStanzaError.ToServiceUnavailable(iq);

			var session = context.SessionManager.GetSession(iq.To);
            if (session != null)
            {
                context.Sender.SendTo(session, iq);
                return null;
            }
            else
            {
                return XmppStanzaError.ToRecipientUnavailable(iq);
            }
		}
	}
}