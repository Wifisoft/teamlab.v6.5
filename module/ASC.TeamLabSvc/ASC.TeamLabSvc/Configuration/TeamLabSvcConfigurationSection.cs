using System.Configuration;

namespace ASC.TeamLabSvc.Configuration
{
    class TeamLabSvcConfigurationSection : ConfigurationSection
    {
        public const string SECTION_NAME = "teamlab";

        public const string SERVICES = "services";

        public const string TYPE = "type";

        public const string DISABLE = "disable";


        [ConfigurationProperty(SERVICES)]
        public TeamLabSvcConfigurationCollection TeamlabServices
        {
            get { return (TeamLabSvcConfigurationCollection)this[SERVICES]; }
        }


        public static TeamLabSvcConfigurationSection GetSection()
        {
            return (TeamLabSvcConfigurationSection)ConfigurationManager.GetSection(TeamLabSvcConfigurationSection.SECTION_NAME);
        }
    }
}
