using System.Collections.Generic;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Storage.Interface
{
	public interface IUserStore
	{
		ICollection<User> GetUsers(string domain);

		void SaveUser(User user);

		User GetUser(Jid jid);

		void RemoveUser(Jid jid);
	}
}