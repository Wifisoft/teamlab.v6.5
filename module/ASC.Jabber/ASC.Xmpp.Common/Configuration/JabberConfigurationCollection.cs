using System.Configuration;

namespace ASC.Xmpp.Common.Configuration
{
	public class JabberConfigurationCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new JabberConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((JabberConfigurationElement)element).Name;
		}

		public void Add(JabberConfigurationElement element)
		{
			BaseAdd(element);
		}

        public JabberConfigurationElement GetElement(int index)
        {
            return (JabberConfigurationElement)BaseGet(index);
        }
	}
}
