using System.Collections.Generic;

namespace ASC.Xmpp.Common.Configuration
{
	public interface IConfigurable
	{
		void Configure(IDictionary<string, string> properties);
	}
}
