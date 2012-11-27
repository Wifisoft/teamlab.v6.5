using System.Configuration;

namespace ASC.Notify.Config
{
    class NotifyServiceCfgSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
        }

        [ConfigurationProperty("process")]
        public NotifyServiceCfgProcessElement Process
        {
            get { return (NotifyServiceCfgProcessElement)base["process"]; }
        }

        [ConfigurationProperty("senders")]
        [ConfigurationCollection(typeof(NotifyServiceCfgSendersCollection), AddItemName = "sender")]
        public NotifyServiceCfgSendersCollection Senders
        {
            get { return (NotifyServiceCfgSendersCollection)base["senders"]; }
        }
    }
}
