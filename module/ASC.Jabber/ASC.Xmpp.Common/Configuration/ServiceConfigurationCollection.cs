using System.Configuration;

namespace ASC.Xmpp.Common.Configuration
{
    public class ServiceConfigurationCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ServiceConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ServiceConfigurationElement)element).Jid;
		}

		public void Add(ServiceConfigurationElement element)
		{
			BaseAdd(element);
		}
	}
}