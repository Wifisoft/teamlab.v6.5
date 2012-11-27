using System.Configuration;

namespace ASC.Xmpp.Common.Configuration
{
    public class JabberConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty(Schema.LISTENERS, IsDefaultCollection = false)]
		public JabberConfigurationCollection Listeners
		{
			get { return (JabberConfigurationCollection)base[Schema.LISTENERS]; }
		}

		[ConfigurationProperty(Schema.STORAGES, IsDefaultCollection = false)]
		public JabberConfigurationCollection Storages
		{
			get { return (JabberConfigurationCollection)base[Schema.STORAGES]; }
		}

		[ConfigurationProperty(Schema.SERVICES, IsDefaultCollection = false)]
		public ServiceConfigurationCollection Services
		{
			get { return (ServiceConfigurationCollection)base[Schema.SERVICES]; }
		}
	}
}