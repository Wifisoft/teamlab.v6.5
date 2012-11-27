using System.Collections.Generic;
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;

namespace ASC.Core.Notify.Senders
{
    public class JabberSender : INotifySender
    {
        private JabberServiceClient service = new JabberServiceClient();


        public void Init(IDictionary<string, string> properties)
        {
        }

        public NoticeSendResult Send(NotifyMessage m)
        {
            service.SendMessage(m.To, m.Subject, m.Content, m.Tenant);
            return NoticeSendResult.OK;
        }
    }
}
