using System;
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Server.Services
{
	public interface IXmppService : IConfigurable
	{
		Jid Jid
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}
		
		DiscoInfo DiscoInfo
		{
			get;
		}

		DiscoItem DiscoItem
		{
			get;
		}

		Vcard Vcard
		{
			get;
		}

		IXmppService ParentService
		{
			get;
			set;
		}

		void OnRegister(IServiceProvider serviceProvider);

		void OnUnregister(IServiceProvider serviceProvider);
	}
}
