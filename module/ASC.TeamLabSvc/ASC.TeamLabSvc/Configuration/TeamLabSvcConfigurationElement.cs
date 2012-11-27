using System.Configuration;

namespace ASC.TeamLabSvc.Configuration
{
    class TeamLabSvcConfigurationElement: ConfigurationElement
    {
        [ConfigurationProperty(TeamLabSvcConfigurationSection.TYPE, IsKey = true, IsRequired = true)]
		public string Type
		{
            get { return (string)this[TeamLabSvcConfigurationSection.TYPE]; }
            set { this[TeamLabSvcConfigurationSection.TYPE] = value; }
		}

        [ConfigurationProperty(TeamLabSvcConfigurationSection.DISABLE, DefaultValue = false)]
        public bool Disable
        {
            get { return (bool)this[TeamLabSvcConfigurationSection.DISABLE]; }
            set { this[TeamLabSvcConfigurationSection.DISABLE] = value; }
        }
    }
}
