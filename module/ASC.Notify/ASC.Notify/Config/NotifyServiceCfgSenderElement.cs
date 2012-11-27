using System.Collections.Generic;
using System.Configuration;

namespace ASC.Notify.Config
{
    class NotifyServiceCfgSenderElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
        }


        public new IDictionary<string, string> Properties
        {
            get;
            private set;
        }


        public NotifyServiceCfgSenderElement()
        {
            Properties = new Dictionary<string, string>();
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            Properties[name] = value;
            return true;
        }
    }
}
