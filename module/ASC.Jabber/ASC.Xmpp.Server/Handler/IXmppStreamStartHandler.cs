using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public interface IXmppStreamStartHandler : IXmppHandler
	{
		string Namespace
		{
			get;
		}

		void StreamStartHandle(XmppStream xmppStream, Stream stream, XmppHandlerContext context);
	}
}
