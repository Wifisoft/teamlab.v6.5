using System.Configuration;

namespace ASC.Data.Storage.Configuration
{
    public class FixerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Schema.NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this[Schema.NAME]; }
            set { this[Schema.NAME] = value; }
        }

        [ConfigurationProperty(Schema.EXT, IsRequired = false, IsKey = false)]
        public string Extension
        {
            get { return (string) this[Schema.EXT]; }
            set { this[Schema.EXT] = value; }
        }

        [ConfigurationProperty(Schema.BEFOREXT, IsRequired = false, IsKey = false)]
        public string AppendBeforeExt
        {
            get { return (string) this[Schema.BEFOREXT]; }
            set { this[Schema.BEFOREXT] = value; }
        }

        [ConfigurationProperty(Schema.ACCEPT_ENCODING, IsRequired = false, IsKey = false)]
        public string AcceptEncoding
        {
            get { return (string) this[Schema.ACCEPT_ENCODING]; }
            set { this[Schema.ACCEPT_ENCODING] = value; }
        }
    }
}