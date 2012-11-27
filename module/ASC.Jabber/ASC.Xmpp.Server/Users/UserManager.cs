using System;
using System.Collections.Generic;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Users
{
	public class UserManager
	{
		private StorageManager storageManager;

		private IUserStore userStore;

		private IUserStore UserStore
		{
			get
			{
				if (userStore == null)
				{
					lock (this)
					{
						if (userStore == null) userStore = storageManager.UserStorage;
					}
				}
				return userStore;
			}
		}

		public UserManager(StorageManager storageManager)
		{
			if (storageManager == null) throw new ArgumentNullException("storageManager");
			this.storageManager = storageManager;
		}

		public bool IsUserExists(Jid jid)
		{
			return GetUser(jid) != null;
		}

		public User GetUser(Jid jid)
		{
			return UserStore.GetUser(jid);
		}

		public ICollection<User> GetUsers(string domain)
		{
			return UserStore.GetUsers(domain);
		}

		public void SaveUser(User user)
		{
			UserStore.SaveUser(user);
		}

		public void RemoveUser(Jid jid)
		{
			UserStore.RemoveUser(jid);
		}
	}
}