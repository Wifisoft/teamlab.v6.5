using System;

namespace ASC.Xmpp.Common
{
	[Serializable]
	public class JabberClientConfiguration
	{
		public string Domain
		{
			get;
			set;
		}

		public Uri BoshUri
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}
	}
}
