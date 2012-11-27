using System;
using System.ServiceModel;
using ASC.Core;
using ASC.Core.Notify.Jabber;

namespace ASC.Xmpp.Common
{
    public class JabberServiceClient
    {
        private static readonly TimeSpan timeout = TimeSpan.FromMinutes(1);

        private static DateTime lastErrorTime;

        private static bool IsServiceProbablyNotAvailable()
        {
            return lastErrorTime != default(DateTime) && lastErrorTime + timeout > DateTime.Now;
        }


        public bool IsAvailable()
        {
            using (var service = GetService())
            {
                try
                {
                    service.GetClientConfiguration(0);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public int GetNewMessagesCount(string username, int tenantId)
        {
            var result = 0;
            if (IsServiceProbablyNotAvailable() || string.IsNullOrEmpty(username)) return result;

            using (var service = GetService())
            {
                try
                {
                    result = service.GetNewMessagesCount(username, tenantId);
                }
                catch (Exception error)
                {
                    ProcessError(error);
                }
            }
            return result;
        }

        public bool IsUserAvailable(string username, int tenantId)
        {
            var result = false;
            if (IsServiceProbablyNotAvailable() || string.IsNullOrEmpty(username)) return result;

            using (var service = GetService())
            {
                try
                {
                    result = service.IsUserAvailable(username, tenantId);
                }
                catch (Exception error)
                {
                    ProcessError(error);
                }
            }
            return result;
        }

        public string GetAuthToken(int tenantId)
        {
            var result = string.Empty;
            if (IsServiceProbablyNotAvailable()) return result;

            var username = GetCurrentUserName();
            if (string.IsNullOrEmpty(username)) return result;

            using (var service = GetService())
            {
                try
                {
                    result = service.GetUserToken(username, tenantId);
                }
                catch (Exception error)
                {
                    ProcessError(error);
                }
            }
            return result;
        }

        public JabberClientConfiguration GetClientConfiguration(int tenantId)
        {
            JabberClientConfiguration result = null;
            if (IsServiceProbablyNotAvailable()) return result;

            using (var service = GetService())
            {
                try
                {
                    var serverCfg = service.GetClientConfiguration(tenantId);
                    result = new JabberClientConfiguration();
                    if (serverCfg.ContainsKey("Domain")) result.Domain = serverCfg["Domain"];
                    if (serverCfg.ContainsKey("BoshUri")) result.BoshUri = new Uri(serverCfg["BoshUri"]);
                    if (serverCfg.ContainsKey("Port")) result.Port = int.Parse(serverCfg["Port"]);
                }
                catch (Exception error)
                {
                    ProcessError(error);
                }
            }
            return result;
        }

        public void SendCommand(string from, string to, string command, int tenantId)
        {
            if (IsServiceProbablyNotAvailable()) return;

            using (var service = GetService())
            {
                try
                {
                    service.SendCommand(from, to, command, tenantId);
                }
                catch (Exception error)
                {
                    ProcessError(error);
                }
            }
        }

        public string GetCurrentUserName()
        {
            var userName = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName;
            return !string.IsNullOrEmpty(userName) ? userName.ToLower() : null;
        }


        private JabberServiceClientWcf GetService()
        {
            return new JabberServiceClientWcf();
        }

        private void ProcessError(Exception error)
        {
            if (error is FaultException)
            {
                throw error;
            }
            if (error is CommunicationException || error is TimeoutException)
            {
                lastErrorTime = DateTime.Now;
            }
            else
            {
                throw error;
            }
        }
    }
}