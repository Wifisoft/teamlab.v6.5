using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class ModuleConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigurationElement();
        }

        internal ModuleConfigurationElement GetModuleElement(string name)
        {
            return (ModuleConfigurationElement) BaseGet(name);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigurationElement) element).Name;
        }

        public void Add(ModuleConfigurationElement element)
        {
            BaseAdd(element);
        }

        internal ModuleConfigurationElement GetModuleElement(int index)
        {
            return (ModuleConfigurationElement) BaseGet(index);
        }
    }
}