using System.Collections.Generic;
using ASC.Common.Module;
using ASC.Notify;
using ASC.Notify.Messages;

namespace ASC.Core.Notify.Senders
{
    public class NotifyServiceSender : INotifySender
    {
        public void Init(IDictionary<string, string> properties)
        {
        }

        public NoticeSendResult Send(NotifyMessage m)
        {
            using (var notifyClient = new NotifySerivceClientWcf())
            {
                notifyClient.SendNotifyMessage(m);
            }
            return NoticeSendResult.OK;
        }


        private class NotifySerivceClientWcf : BaseWcfClient<INotifyService>, INotifyService
        {
            public void SendNotifyMessage(NotifyMessage m)
            {
                Channel.SendNotifyMessage(m);
            }
        }
    }
}
