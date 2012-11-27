using System;
using System.Collections.Generic;
using System.Threading;
using ASC.Collections;

namespace ASC.Xmpp.Server.Streams
{
	public class XmppStreamManager
	{
        private IDictionary<string, XmppStream> streams = new SynchronizedDictionary<string, XmppStream>();

		private ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		public XmppStream GetStream(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
			try
			{
				locker.EnterReadLock();
				return streams.ContainsKey(connectionId) ? streams[connectionId] : null;
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		public XmppStream GetOrCreateNewStream(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
			try
			{
                locker.EnterWriteLock();
				var stream = new XmppStream(connectionId);
				if (streams.ContainsKey(connectionId))
				{
					var oldStream = streams[connectionId];
					if (oldStream.Authenticated) stream.Authenticate(oldStream.User);
				}
				streams[connectionId] = stream;
				return stream;
			}
			finally
			{
				locker.ExitWriteLock();
			}
		}

		public void RemoveStream(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
			try
			{
				locker.EnterWriteLock();
				streams.Remove(connectionId);
			}
			finally
			{
				locker.ExitWriteLock();
			}
		}
	}
}
