using System.Configuration;

namespace ASC.TeamLabSvc.Configuration
{
    class TeamLabSvcConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TeamLabSvcConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TeamLabSvcConfigurationElement)element).Type;
        }
    }
}
