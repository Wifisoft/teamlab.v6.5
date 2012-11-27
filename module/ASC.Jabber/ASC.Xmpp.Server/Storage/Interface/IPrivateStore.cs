using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Storage.Interface
{

	public interface IPrivateStore
	{
		Element GetPrivate(Jid jid, Element element);

        void SetPrivate(Jid jid, Element element);
	}
}