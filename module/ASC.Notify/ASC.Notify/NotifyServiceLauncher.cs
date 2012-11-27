using System.ServiceModel;
using ASC.Common.Module;
using log4net.Config;

namespace ASC.Notify
{
    public class NotifyServiceLauncher : IServiceController
    {
        private ServiceHost serviceHost;
        private NotifyService service;


        public string ServiceName
        {
            get { return typeof(NotifyService).Name; }
        }


        public void Start()
        {
            XmlConfigurator.Configure();

            serviceHost = new ServiceHost(typeof(NotifyService));
            serviceHost.Open();

            service = new NotifyService();
            service.StartSending();
        }

        public void Stop()
        {
            if (service != null)
            {
                service.StopSending();
            }
            if (serviceHost != null)
            {
                serviceHost.Close();
            }
        }
    }
}
