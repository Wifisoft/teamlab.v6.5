using System;
using System.Collections.Generic;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Streams;


namespace ASC.Xmpp.Server.Handler
{
	public class XmppStreamHandler : IXmppStreamHandler
	{
		public virtual void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
		{
			
		}

		public virtual void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context)
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
