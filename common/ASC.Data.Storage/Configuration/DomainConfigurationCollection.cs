using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class DomainConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DomainConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainConfigurationElement) element).Name;
        }

        internal DomainConfigurationElement GetDomainElement(string name)
        {
            return (DomainConfigurationElement) BaseGet(name);
        }

        public void Add(DomainConfigurationElement element)
        {
            BaseAdd(element);
        }

        internal DomainConfigurationElement GetModuleElement(int index)
        {
            return (DomainConfigurationElement) BaseGet(index);
        }
    }
}