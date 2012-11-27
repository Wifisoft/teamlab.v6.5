using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace ASC.Thrdparty.Configuration
{
    public class ConsumerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("keys")]
        public KeysElementCollection Keys
        {
            get { return (KeysElementCollection)base["keys"]; }
            set { base["keys"] = value; }
        }

        [ConfigurationProperty("connectionstring")]
        public string ConnectionString
        {
            get { return (string)base["connectionstring"]; }
            set { base["connectionstring"] = value; }
        }

        private const string SectionName = "consumers";

        internal static ConsumerConfigurationSection GetSection()
        {
            return GetSection(string.Empty);
        }

        internal static ConsumerConfigurationSection GetSection(string configpath)
        {
            ConsumerConfigurationSection section;
            if (!string.IsNullOrEmpty(configpath))
            {
                if (configpath.Contains("\\") && !Uri.IsWellFormedUriString(configpath, UriKind.Relative))//Not mapped path
                {
                    var configMap = new ExeConfigurationFileMap
                    {
                        ExeConfigFilename =
                            string.Compare(Path.GetExtension(configpath), ".config", false) == 0
                                ? configpath
                                : Path.Combine(configpath, "web.config")
                    };
                    section = (ConsumerConfigurationSection)ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None).GetSection(SectionName);
                }
                else
                {
                    section = (ConsumerConfigurationSection)
                              WebConfigurationManager.OpenWebConfiguration(configpath).GetSection(SectionName);
                }
            }
            else
            {
                //Nothing worked, try local
                section = (ConsumerConfigurationSection)ConfigurationManager.GetSection(SectionName);
            }
            return section;
        }
    }

    public class KeysElementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyElement)element).Name;
        }

        public string GetKeyValue(string keyName)
        {
            var obj = BaseGet(keyName) as KeyElement;
            if (obj!=null)
            {
                return obj.Value;
            }
            return string.Empty;
        }
    }

    class KeyElement : ConfigurationElement
    {

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }


}