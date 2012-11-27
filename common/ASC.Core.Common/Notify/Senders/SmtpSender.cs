using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using ASC.Common.Utils;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using log4net;

namespace ASC.Core.Notify.Senders
{
    class SmtpSender : INotifySender
    {
        private const string htmlFormat =
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">
<html>
<head>
<meta content=""text/html;charset=UTF-8"" http-equiv=""Content-Type"">
</head>
<body>{0}</body>
</html>";

        protected ILog Log
        {
            get;
            private set;
        }

        private string host;
        private int port;
        private bool ssl;
        private ICredentialsByHost credentials;


        public SmtpSender()
        {
            Log = LogManager.GetLogger("ASC.Notify");
        }


        public virtual void Init(IDictionary<string, string> properties)
        {
            host = properties["host"];
            port = properties.ContainsKey("port") ? int.Parse(properties["port"]) : 25;
            ssl = properties.ContainsKey("enableSsl") ? bool.Parse(properties["enableSsl"]) : false;
            if (properties.ContainsKey("userName"))
            {
                credentials = new NetworkCredential(
                    properties["userName"],
                    properties["password"],
                    properties.ContainsKey("domain") ? properties["domain"] : string.Empty);
            }
        }

        public virtual NoticeSendResult Send(NotifyMessage m)
        {
            var smtpClient = new SmtpClient(host, port) { Credentials = credentials, EnableSsl = ssl, };
            var result = NoticeSendResult.TryOnceAgain;
            try
            {
                try
                {
                    var mail = BuildMailMessage(m);
                    smtpClient.Send(mail);
                    result = NoticeSendResult.OK;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    throw;
                }
            }
            catch (ArgumentException)
            {
                result = NoticeSendResult.MessageIncorrect;
            }
            catch (ObjectDisposedException)
            {
                result = NoticeSendResult.SendingImpossible;
            }
            catch (InvalidOperationException)
            {
                result = string.IsNullOrEmpty(smtpClient.Host) || smtpClient.Port == 0 ? NoticeSendResult.SendingImpossible : NoticeSendResult.TryOnceAgain;
            }
            catch (SmtpFailedRecipientException e)
            {
                if (e.StatusCode == SmtpStatusCode.MailboxBusy ||
                    e.StatusCode == SmtpStatusCode.MailboxUnavailable ||
                    e.StatusCode == SmtpStatusCode.ExceededStorageAllocation)
                {
                    result = NoticeSendResult.TryOnceAgain;
                }
                else if (e.StatusCode == SmtpStatusCode.MailboxNameNotAllowed ||
                    e.StatusCode == SmtpStatusCode.UserNotLocalWillForward ||
                    e.StatusCode == SmtpStatusCode.UserNotLocalTryAlternatePath)
                {
                    result = NoticeSendResult.MessageIncorrect;
                }
                else if (e.StatusCode != SmtpStatusCode.Ok)
                {
                    result = NoticeSendResult.TryOnceAgain;
                }
            }
            catch (SmtpException)
            {
                result = NoticeSendResult.SendingImpossible;
            }
            return result;
        }

        private MailMessage BuildMailMessage(NotifyMessage m)
        {
            var email = new MailMessage
            {
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(m.From),
                ReplyTo = !string.IsNullOrEmpty(m.ReplyTo) ? new MailAddress(m.ReplyTo) : null,
                Subject = m.Subject,
            };

            foreach (var to in m.To.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                email.To.Add(new MailAddress(to));
            }

            if (m.ContentType == Pattern.HTMLContentType)
            {
                email.Body = HtmlUtil.GetText(m.Content);
                email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(GetHtmlView(m.Content), Encoding.UTF8, "text/html"));
            }
            else
            {
                email.Body = m.Content;
            }
            return email;
        }

        protected string GetHtmlView(string body)
        {
            return string.Format(htmlFormat, body);
        }
    }
}