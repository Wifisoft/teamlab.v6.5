using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class FixerConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FixerConfigurationElement();
        }

        internal FixerConfigurationElement GetModuleElement(string name)
        {
            return (FixerConfigurationElement) BaseGet(name);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FixerConfigurationElement) element).Name;
        }

        public void Add(FixerConfigurationElement element)
        {
            BaseAdd(element);
        }
    }
}