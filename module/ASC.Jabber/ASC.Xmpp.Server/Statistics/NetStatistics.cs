using System.Threading;

namespace ASC.Xmpp.Server.Statistics
{
	public static class NetStatistics
	{
		private static long readBytes;

		private static long writeBytes;


		public static bool Enabled
		{
			get;
			set;
		}

		public static void ReadBytes(int bytes)
		{
			if (!Enabled) return;
			Interlocked.Add(ref readBytes, bytes);
		}

		public static void WriteBytes(int bytes)
		{
			if (!Enabled) return;
			Interlocked.Add(ref writeBytes, bytes);
		}

		public static void Reset()
		{
			Interlocked.Exchange(ref readBytes, 0);
			Interlocked.Exchange(ref writeBytes, 0);
		}

		public static long GetReadBytes()
		{
			return readBytes;
		}

		public static long GetWriteBytes()
		{
			return writeBytes;
		}
	}
}
