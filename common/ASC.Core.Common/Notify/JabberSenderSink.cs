using System;
using ASC.Core.Notify.Senders;
using ASC.Core.Tenants;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;

namespace ASC.Core.Notify
{
    class JabberSenderSink : Sink
    {
        private static readonly string senderName = ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName;
        private readonly INotifySender sender;


        public JabberSenderSink(INotifySender sender)
        {
            if (sender == null) throw new ArgumentNullException("sender");

            this.sender = sender;
        }


        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            try
            {
                var result = SendResult.OK;
                var username = CoreContext.UserManager.GetUsers(new Guid(message.Recipient.ID)).UserName;
                if (string.IsNullOrEmpty(username))
                {
                    result = SendResult.IncorrectRecipient;
                }
                else
                {
                    var m = new NotifyMessage
                    {
                        To = username,
                        Subject = message.Subject,
                        ContentType = message.ContentType,
                        Content = message.Body,
                        Sender = senderName,
                        CreationDate = DateTime.UtcNow,
                    };

                    var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                    m.Tenant = tenant == null ? Tenant.DEFAULT_TENANT : tenant.TenantId;

                    sender.Send(m);
                }
                return new SendResponse(message, senderName, result);
            }
            catch (Exception ex)
            {
                return new SendResponse(message, senderName, ex);
            }
        }
    }
}
