using System;
using System.ServiceModel;
using log4net;

namespace ASC.Core.Notify.Jabber
{
    public class JabberServiceClient
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JabberServiceClient));

        private static readonly TimeSpan timeout = TimeSpan.FromMinutes(2);

        private static DateTime lastErrorTime = default(DateTime);

        private static bool IsServiceProbablyNotAvailable()
        {
            return lastErrorTime != default(DateTime) && lastErrorTime + timeout > DateTime.Now;
        }


        public bool SendMessage(string to, string subject, string text, int tenantId)
        {
            if (IsServiceProbablyNotAvailable()) return false;

            using (var service = new JabberServiceClientWcf())
            {
                try
                {
                    service.SendMessage(to, subject, text, tenantId);
                    return true;
                }
                catch (FaultException e)
                {
                    log.Error(e);
                    throw;
                }
                catch (CommunicationException e)
                {
                    log.Error(e);
                    lastErrorTime = DateTime.Now;
                }
                catch (TimeoutException e)
                {
                    log.Error(e);
                    lastErrorTime = DateTime.Now;
                }
            }

            return false;
        }
    }
}
