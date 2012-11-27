using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class HandlersConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new HandlerConfigurationElement();
        }

        internal HandlerConfigurationElement GetHandler(string name)
        {
            return (HandlerConfigurationElement) BaseGet(name);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HandlerConfigurationElement) element).Name;
        }

        public void Add(HandlerConfigurationElement element)
        {
            BaseAdd(element);
        }
    }
}