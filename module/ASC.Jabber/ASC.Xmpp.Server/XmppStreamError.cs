using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server
{
	public static class XmppStreamError
	{
		public static Error BadFormat
		{
			get { return new Error(StreamErrorCondition.BadFormat) { Prefix = Uri.PREFIX }; }
		}

		public static Error Conflict
		{
			get { return new Error(StreamErrorCondition.Conflict) { Prefix = Uri.PREFIX }; }
		}

		public static Error HostUnknown
		{
			get { return new Error(StreamErrorCondition.HostUnknown) { Prefix = Uri.PREFIX }; }
		}

		public static Error BadNamespacePrefix
		{
			get { return new Error(StreamErrorCondition.BadNamespacePrefix) { Prefix = Uri.PREFIX }; }
		}

		public static Error NotAuthorized
		{
			get { return new Error(StreamErrorCondition.NotAuthorized) { Prefix = Uri.PREFIX }; }
		}

		public static Error InvalidFrom
		{
			get { return new Error(StreamErrorCondition.InvalidFrom) { Prefix = Uri.PREFIX }; }
		}

		public static Error ImproperAddressing
		{
			get { return new Error(StreamErrorCondition.ImproperAddressing) { Prefix = Uri.PREFIX }; }
		}

		public static Error InternalServerError
		{
			get { return new Error(StreamErrorCondition.InternalServerError) { Prefix = Uri.PREFIX }; }
		}
	}
}
