using System;
using System.Collections.Generic;
using ASC.Common.Module;

namespace ASC.Core.Notify.Jabber
{
    public class JabberServiceClientWcf : BaseWcfClient<IJabberService>, IJabberService, IDisposable
    {
        public IDictionary<string, string> GetClientConfiguration(int tenantId)
        {
            return Channel.GetClientConfiguration(tenantId);
        }

        public bool IsUserAvailable(string username, int tenantId)
        {
            return Channel.IsUserAvailable(username, tenantId);
        }

        public int GetNewMessagesCount(string userName, int tenantId)
        {
            return Channel.GetNewMessagesCount(userName, tenantId);
        }

        public string GetUserToken(string userName, int tenantId)
        {
            return Channel.GetUserToken(userName, tenantId);
        }

        public void SendCommand(string from, string to, string command, int tenantId)
        {
            Channel.SendCommand(from, to, command, tenantId);
        }

        public void SendMessage(string to, string subject, string text, int tenantId)
        {
            Channel.SendMessage(to, subject, text, tenantId);
        }
    }
}
