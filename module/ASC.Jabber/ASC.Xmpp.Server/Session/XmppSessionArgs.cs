using System;

namespace ASC.Xmpp.Server.Session
{
	public class XmppSessionArgs : EventArgs
	{
		public XmppSession Session
		{
			get;
			private set;
		}

		public XmppSessionArgs(XmppSession session)
		{
			if (session == null) throw new ArgumentNullException("session");
			Session = session;
		}
	}
}