using System.Configuration;

namespace ASC.FullTextIndex.Service.Config
{
	class TextIndexCfgSectionHandler : ConfigurationSection
	{
        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get { return (string)this["connectionStringName"]; }
        }

        [ConfigurationProperty("changedCron", IsRequired = true)]
		public string ChangedCron
		{
			get { return (string)base["changedCron"]; }
		}

		[ConfigurationProperty("removedCron", IsRequired = true)]
		public string RemovedCron
		{
			get { return (string)base["removedCron"]; }
		}

        [ConfigurationProperty("delay", DefaultValue = 0)]
        public int Delay
        {
            get { return (int)base["delay"]; }
        }
        
        [ConfigurationProperty("indexPath", DefaultValue = "")]
		public string IndexPath
		{
			get { return (string)base["indexPath"]; }
		}

		[ConfigurationProperty("modules")]
		public TextIndexCfgModuleCollection Modules
		{
			get { return (TextIndexCfgModuleCollection)base["modules"]; }
		}
	}
}
