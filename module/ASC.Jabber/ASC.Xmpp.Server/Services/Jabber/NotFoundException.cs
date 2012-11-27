using ASC.Xmpp.protocol.client;
using ASC.Xmpp.Server.Handler;

namespace ASC.Xmpp.Server.Jabber
{
	public class NotFoundException : XmppStanzaException
	{
		public NotFoundException()
			: base(ErrorType.modify, ErrorCondition.ItemNotFound)
		{

		}
	}
}
