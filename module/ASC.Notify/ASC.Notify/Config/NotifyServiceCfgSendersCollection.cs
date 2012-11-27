using System.Configuration;

namespace ASC.Notify.Config
{
    public class NotifyServiceCfgSendersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new NotifyServiceCfgSenderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NotifyServiceCfgSenderElement)element).Name;
        }
    }
}
