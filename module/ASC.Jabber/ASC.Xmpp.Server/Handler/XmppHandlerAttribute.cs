using System;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Handler
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class XmppHandlerAttribute : Attribute
	{
		public Type XmppElementType
		{
			get;
			private set;
		}

		public XmppHandlerAttribute(Type xmppElementType) {
			if (xmppElementType == null) throw new ArgumentNullException("xmppElementType");

			if (!typeof(Element).IsAssignableFrom(xmppElementType)) throw new ArgumentException("xmppElementType not assigned from Element.");
			XmppElementType = xmppElementType;
		}
	}
}
