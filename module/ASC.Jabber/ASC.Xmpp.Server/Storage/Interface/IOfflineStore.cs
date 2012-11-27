using System.Collections.Generic;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Storage.Interface
{
	public interface IOfflineStore
	{
		List<Message> GetOfflineMessages(Jid jid);

        int GetOfflineMessagesCount(Jid jid);

		void SaveOfflineMessages(params Message[] messages);

		void RemoveAllOfflineMessages(Jid jid);


		List<Presence> GetOfflinePresences(Jid jid);

		void SaveOfflinePresence(Presence presence);

		void RemoveAllOfflinePresences(Jid jid);


		void SaveLastActivity(Jid jid, LastActivity lastActivity);

		LastActivity GetLastActivity(Jid jid);
	}
}