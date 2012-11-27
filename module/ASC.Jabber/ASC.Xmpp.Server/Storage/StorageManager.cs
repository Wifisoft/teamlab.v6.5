using System.Collections.Generic;
using ASC.Collections;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Storage
{
	public class StorageManager
	{
        private IDictionary<string, object> storages = new SynchronizedDictionary<string, object>();

		public IOfflineStore OfflineStorage
		{
			get { return GetStorage<IOfflineStore>("offline"); }
		}

		public IRosterStore RosterStorage
		{
			get { return GetStorage<IRosterStore>("roster"); }
		}

		public IVCardStore VCardStorage
		{
			get { return GetStorage<IVCardStore>("vcard"); }
		}

		public IPrivateStore PrivateStorage
		{
			get { return GetStorage<IPrivateStore>("private"); }
		}

		public IMucStore MucStorage
		{
			get { return GetStorage<IMucStore>("muc"); }
		}

		public IUserStore UserStorage
		{
			get { return GetStorage<IUserStore>("users"); }
		}

		public object this[string storageName]
		{
			get { return storages.ContainsKey(storageName) ? storages[storageName] : null; }
			set { storages[storageName] = value; }
		}

		public T GetStorage<T>(string storageName)
		{
			return (T)this[storageName];
		}

		public void SetStorage(string storageName, object storage)
		{
			this[storageName] = storage;
		}
	}
}