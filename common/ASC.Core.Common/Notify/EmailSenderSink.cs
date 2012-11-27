using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ASC.Common.Notify;
using ASC.Core.Notify.Senders;
using ASC.Core.Tenants;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;
using log4net;

namespace ASC.Core.Notify
{
    class EmailSenderSink : Sink
    {
        private static readonly string senderName = ASC.Core.Configuration.Constants.NotifyEMailSenderSysName;
        private readonly INotifySender sender;


        public EmailSenderSink(INotifySender sender)
        {
            if (sender == null) throw new ArgumentNullException("sender");

            this.sender = sender;
        }


        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            if (message.Recipient.Addresses == null || message.Recipient.Addresses.Length == 0)
            {
                return new SendResponse(message, senderName, SendResult.IncorrectRecipient);
            }

            var responce = new SendResponse(message, senderName, default(SendResult));
            try
            {
                var m = CreateNotifyMessage(message);
                var result = sender.Send(m);

                switch (result)
                {
                    case NoticeSendResult.TryOnceAgain:
                        responce.Result = SendResult.Inprogress;
                        break;
                    case NoticeSendResult.MessageIncorrect:
                        responce.Result = SendResult.IncorrectRecipient;
                        break;
                    case NoticeSendResult.SendingImpossible:
                        responce.Result = SendResult.Impossible;
                        break;
                    default:
                        responce.Result = SendResult.OK;
                        break;
                }
                return responce;
            }
            catch (Exception e)
            {
                return new SendResponse(message, senderName, e);
            }
        }


        private NotifyMessage CreateNotifyMessage(INoticeMessage message)
        {
            var m = new NotifyMessage
            {
                Subject = message.Subject.Trim(' ', '\t', '\n', '\r'),
                ContentType = message.ContentType,
                Content = message.Body,
                Sender = senderName,
                CreationDate = DateTime.UtcNow,
            };

            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            m.Tenant = tenant == null ? Tenant.DEFAULT_TENANT : tenant.TenantId;

            var from = new MailAddress(CoreContext.Configuration.SmtpSettings.SenderAddress, CoreContext.Configuration.SmtpSettings.SenderDisplayName);
            var fromTag = message.Arguments.FirstOrDefault(x => x.Tag.Name.Equals(NotifyArgumentConstants.MessageFrom));
            if (fromTag != null && fromTag.Value != null)
            {
                try
                {
                    from = new MailAddress(from.Address, fromTag.Value.ToString(), Encoding.UTF8);
                }
                catch { }
            }
            m.From = from.ToString();

            var to = new List<string>();
            var nameOne = false;
            foreach (var address in message.Recipient.Addresses)
            {
                var recipient = !nameOne ? new MailAddress(address, message.Recipient.Name) : new MailAddress(address);
                to.Add(recipient.ToString());
                nameOne = true;
            }
            m.To = string.Join("|", to.ToArray());

            var replyTag = message.Arguments.FirstOrDefault(x => x.Tag.Name == "replyto");
            if (replyTag != null && replyTag.Value is string)
            {
                try
                {
                    m.ReplyTo = new MailAddress((string)replyTag.Value).ToString();
                }
                catch (Exception e)
                {
                    LogManager.GetLogger("ASC.Notify").Error("Error creating reply to tag for: " + replyTag.Value, e);
                }
            }

            return m;
        }
    }
}