using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class StorageConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty(Schema.MODULES, IsDefaultCollection = false)]
        public ModuleConfigurationCollection Modules
        {
            get { return (ModuleConfigurationCollection) base[Schema.MODULES]; }
        }

        [ConfigurationProperty(Schema.HANDLERS, IsDefaultCollection = false)]
        public HandlersConfigurationCollection Handlers
        {
            get { return (HandlersConfigurationCollection) base[Schema.HANDLERS]; }
        }

        [ConfigurationProperty(Schema.APPENDERS, IsDefaultCollection = false)]
        public AppenderConfigurationCollection Appenders
        {
            get { return (AppenderConfigurationCollection) base[Schema.APPENDERS]; }
        }

        [ConfigurationProperty(Schema.FIXERS, IsDefaultCollection = false)]
        public FixerConfigurationCollection Fixers
        {
            get { return (FixerConfigurationCollection) base[Schema.FIXERS]; }
        }
    }
}