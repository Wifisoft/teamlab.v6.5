using System;
using System.Configuration;
using System.Threading;
using ASC.Notify.Channels;
using ASC.Notify.Messages;
using ASC.Threading;
using log4net;

namespace ASC.Notify.Engine
{
    public class DispatchEngine
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Notify");
        private static readonly ILog logMessages = LogManager.GetLogger("ASC.Notify.Messages");

        private static readonly TimeSpan sendAttemptInterval = TimeSpan.FromSeconds(10);
        private readonly Context context;
        private readonly bool logOnly;


        public DispatchEngine(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.context = context;
            logOnly = "log".Equals(ConfigurationManager.AppSettings["core.notify.postman"], StringComparison.InvariantCultureIgnoreCase);
            log.DebugFormat("LogOnly: {0}", logOnly);
        }

        public SendResponse Dispatch(INoticeMessage message, string senderName)
        {
            var response = new SendResponse(message, senderName, SendResult.OK);
            if (!logOnly)
            {
                if (context.SenderHolder.GetSender(senderName) != null)
                {
                    var request = new DispatchRequest(message, context.SenderHolder.GetSender(senderName), (n, r) => response = r)
                    {
                        SendAttemptInterval = sendAttemptInterval,
                    };
                    DispatchExecutor(request);
                }
                else
                {
                    response = new SendResponse(message, senderName, SendResult.Impossible);
                }
            }
            LogMessage(message, senderName);
            return response;
        }

        private bool DispatchExecutor(DispatchRequest request)
        {
            if (TimeSpan.Zero < request.SleepBeforeSend)
            {
                Thread.Sleep(request.SleepBeforeSend);
            }
            request.LastAttempt = DateTime.UtcNow;
            request.AttemptCount++;

            var response = request.SenderChannel != null ?
                request.SenderChannel.DirectSend(request.NoticeMessage) :
                new SendResponse(request.NoticeMessage, null, SendResult.Impossible);

            var logmsg = string.Format("#{4}: [{0}] sended to [{1}] over {2}, status:{3} ", request.NoticeMessage.Subject, request.NoticeMessage.Recipient,
                (request.SenderChannel != null ? request.SenderChannel.SenderName : ""), response.Result, request.DispatchNum);

            if (response.Result == SendResult.Inprogress)
            {
                request.DispatchCallback = null;
                log.Debug(logmsg, response.Exception);
                log.Debug(String.Format("attemt #{1}, try send after {0}", request.SendAttemptInterval, request.AttemptCount));
            }
            else if (response.Result == SendResult.Impossible)
            {
                log.Error(logmsg, response.Exception);
            }
            else
            {
                log.Debug(logmsg);
            }
            if (request.DispatchCallback != null)
            {
                request.DispatchCallback(request.NoticeMessage, response);
            }
            return response.Result != SendResult.Inprogress;
        }

        private void LogMessage(INoticeMessage message, string senderName)
        {
            try
            {
                if (logMessages.IsDebugEnabled)
                {
                    logMessages.DebugFormat("[{5}]->[{1}] by [{6}] to [{2}] at {0}\r\n\r\n[{3}]\r\n{4}\r\n{7}",
                        DateTime.Now,
                        message.Recipient.Name,
                        0 < message.Recipient.Addresses.Length ? message.Recipient.Addresses[0] : string.Empty,
                        message.Subject,
                        (message.Body ?? string.Empty).Replace(Environment.NewLine, Environment.NewLine + @"   "),
                        message.Action,
                        senderName,
                        new string('-', 80));
                }
            }
            catch { }
        }


        private class DispatchRequest
        {
            public readonly int DispatchNum;

            public readonly INoticeMessage NoticeMessage;

            public readonly ISenderChannel SenderChannel;

            public int AttemptCount;

            public Action<INoticeMessage, SendResponse> DispatchCallback;

            public DateTime LastAttempt = DateTime.MinValue;

            public TimeSpan SendAttemptInterval = TimeSpan.FromMilliseconds(500);

            public TimeSpan SleepBeforeSend = TimeSpan.FromMilliseconds(200);


            public DispatchRequest(INoticeMessage message, ISenderChannel senderChannel, Action<INoticeMessage, SendResponse> callback)
            {
                Interlocked.Increment(ref DispatchNum);
                NoticeMessage = message;
                SenderChannel = senderChannel;
                DispatchCallback = callback;
            }
        }
    }
}