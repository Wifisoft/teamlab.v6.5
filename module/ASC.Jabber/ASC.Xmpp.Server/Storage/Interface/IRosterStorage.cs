using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.iq.roster;

namespace ASC.Xmpp.Server.storage.Interface
{
	public interface IRosterStore
	{

		List<UserRosterItem> GetRosterItems(string userName);

		List<UserRosterItem> GetRosterItems(string userName, SubscriptionType subscriptionType);

		UserRosterItem GetRosterItem(string userName, string jid);

		void SaveOrUpdateRosterItem(string userName, UserRosterItem item);

		void RemoveRosterItem(string userName, string jid);
	}
}
