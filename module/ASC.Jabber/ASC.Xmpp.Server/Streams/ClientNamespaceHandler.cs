using System;
using System.Text;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.bind;
using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.protocol.stream;
using ASC.Xmpp.Core.protocol.stream.feature;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Server.Streams
{
	class ClientNamespaceHandler : IXmppStreamStartHandler
	{
		public string Namespace
		{
			get { return Uri.CLIENT; }
		}

		public void StreamStartHandle(XmppStream xmppStream, Stream stream, XmppHandlerContext context)
		{
			var streamHeader = new StringBuilder();
			streamHeader.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
			streamHeader.AppendFormat("<stream:{0} xmlns:{0}='{1}' xmlns='{2}' from='{3}' id='{4}' version='1.0'>",
				Uri.PREFIX, Uri.STREAM, Uri.CLIENT, stream.To, xmppStream.Id);
			context.Sender.SendTo(xmppStream, streamHeader.ToString());

			var features = new Features();
			features.Prefix = Uri.PREFIX;
			if (xmppStream.Authenticated)
			{
				features.AddChild(new Bind());
				features.AddChild(new Core.protocol.iq.session.Session());
			}
			else
			{
				features.Mechanisms = new Mechanisms();
				features.Mechanisms.AddChild(new Mechanism(MechanismType.DIGEST_MD5));
				features.Mechanisms.AddChild(new Element("required"));
				features.Register = new Register();
			}
			streamHeader.Append(features.ToString());
			context.Sender.SendTo(xmppStream, features);
		}

		public void OnRegister(IServiceProvider serviceProvider)
		{

		}

		public void OnUnregister(IServiceProvider serviceProvider)
		{

		}
	}
}