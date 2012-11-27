using System.Collections.Generic;
using ASC.Collections;
using ASC.Xmpp.Core.protocol.iq.disco;

namespace ASC.Xmpp.Server.Session
{
	public class ClientInfo
	{
		private const string DEFAULT_NODE = "DEFAULT_NODE";

        private IDictionary<string, DiscoInfo> discoCache = new SynchronizedDictionary<string, DiscoInfo>();

		public DiscoInfo GetDiscoInfo(string node)
		{
			if (string.IsNullOrEmpty(node)) node = DEFAULT_NODE;
			return discoCache.ContainsKey(node) ? discoCache[node] : null;
		}

		public void SetDiscoInfo(DiscoInfo discoInfo)
		{
			if (discoInfo == null) return;
			var node = !string.IsNullOrEmpty(discoInfo.Node) ? discoInfo.Node : DEFAULT_NODE;
			discoCache[node] = discoInfo;
		}
	}
}
