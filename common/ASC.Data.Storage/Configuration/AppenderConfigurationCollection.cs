using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class AppenderConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppenderConfigurationElement();
        }

        internal AppenderConfigurationElement GetModuleElement(string name)
        {
            return (AppenderConfigurationElement) BaseGet(name);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppenderConfigurationElement) element).Name;
        }

        public void Add(AppenderConfigurationElement element)
        {
            BaseAdd(element);
        }
    }
}