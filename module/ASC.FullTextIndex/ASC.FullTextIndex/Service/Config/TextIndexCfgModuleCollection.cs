using System.Configuration;

namespace ASC.FullTextIndex.Service.Config
{
	class TextIndexCfgModuleCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TextIndexCfgModuleElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TextIndexCfgModuleElement)element).Name;
		}
	}
}
