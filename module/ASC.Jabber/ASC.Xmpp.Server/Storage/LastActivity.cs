using System;

namespace ASC.Xmpp.Server.Storage
{
	public class LastActivity
	{
		public DateTime LogoutDateTime
		{
			get;
			private set;
		}

		public string Status
		{
			get;
			set;
		}

		public LastActivity(string status)
			: this(DateTime.UtcNow, status)
		{

		}

		public LastActivity(DateTime logout, string status)
		{
			LogoutDateTime = logout;
			if (!string.IsNullOrEmpty(status)) Status = status;
		}
	}
}