using System;

namespace ASC.Xmpp.Server.Handler
{
	public interface IXmppHandler
	{
		void OnRegister(IServiceProvider serviceProvider);

		void OnUnregister(IServiceProvider serviceProvider);
	}
}
