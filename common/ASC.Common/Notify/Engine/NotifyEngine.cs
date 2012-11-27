using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ASC.Common.Notify.Patterns;
using ASC.Notify.Channels;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using log4net;

namespace ASC.Notify.Engine
{
    public class NotifyEngine : INotifyEngine
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Notify");

        private readonly Context context;

        private readonly List<SendMethodWrapper> sendMethods = new List<SendMethodWrapper>();

        private readonly Queue<NotifyRequest> requests = new Queue<NotifyRequest>(1000);

        private readonly Thread notifyScheduler;

        private readonly Thread notifySender;

        private readonly AutoResetEvent requestsEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent methodsEvent = new AutoResetEvent(false);

        private readonly Dictionary<string, IPatternStyler> stylers = new Dictionary<string, IPatternStyler>();

        private readonly IPatternFormatter sysTagFormatter = new ReplacePatternFormatter(@"_#(?<tagName>[A-Z0-9_\-.]+)#_", true);

        private readonly TimeSpan defaultSleep = TimeSpan.FromSeconds(10);



        public event Action<NotifyEngine, NotifyRequest> BeforeTransferRequest;

        public event Action<NotifyEngine, NotifyRequest> AfterTransferRequest;


        public bool Scheduling
        {
            get;
            set;
        }


        public NotifyEngine(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.context = context;
            notifyScheduler = new Thread(NotifyScheduler) { IsBackground = true, Name = "NotifyScheduler" };
            notifySender = new Thread(NotifySender) { IsBackground = true, Name = "NotifySender" };
        }


        public virtual void QueueRequest(NotifyRequest request)
        {
            if (BeforeTransferRequest != null)
            {
                BeforeTransferRequest(this, request);
            }
            lock (requests)
            {
                if (!notifySender.IsAlive)
                {
                    notifySender.Start();
                }

                requests.Enqueue(request);
            }
            requestsEvent.Set();
        }


        internal void RegisterSendMethod(INotifyClient client, Action<DateTime> method, Func<DateTime, DateTime?> nextDate)
        {
            if (!Scheduling) return;

            if (client == null) throw new ArgumentNullException("client");
            if (method == null) throw new ArgumentNullException("method");
            if (nextDate == null) throw new ArgumentNullException("nextDate");

            var w = new SendMethodWrapper(client, method, nextDate);
            lock (sendMethods)
            {
                if (!notifyScheduler.IsAlive)
                {
                    notifyScheduler.Start();
                }

                sendMethods.Remove(w);
                sendMethods.Add(w);
            }
            methodsEvent.Set();
        }

        internal void UnregisterSendMethod(INotifyClient client, Action<DateTime> method)
        {
            if (!Scheduling) return;

            if (client == null) throw new ArgumentNullException("client");
            if (method == null) throw new ArgumentNullException("method");

            lock (sendMethods)
            {
                sendMethods.Remove(new SendMethodWrapper(client, method, null));
            }
        }

        private void NotifyScheduler(object state)
        {
            try
            {
                while (true)
                {
                    var min = DateTime.MaxValue;
                    var now = DateTime.UtcNow;
                    List<SendMethodWrapper> copy;
                    lock (sendMethods)
                    {
                        copy = sendMethods.ToList();
                    }

                    foreach (var w in copy)
                    {
                        if (!w.ScheduleDate.HasValue)
                        {
                            lock (sendMethods)
                            {
                                sendMethods.Remove(w);
                            }
                        }

                        if (w.ScheduleDate.Value <= now)
                        {
                            try
                            {
                                w.InvokeSendMethod(now);
                            }
                            catch (Exception error)
                            {
                                log.Error(error);
                            }
                            w.UpdateScheduleDate(now);
                        }

                        if (w.ScheduleDate.Value > now && w.ScheduleDate.Value < min)
                        {
                            min = w.ScheduleDate.Value;
                        }
                    }

                    var wait = min != DateTime.MaxValue ? min - DateTime.UtcNow : defaultSleep;
                    if (wait < defaultSleep)
                    {
                        wait = defaultSleep;
                    }
                    methodsEvent.WaitOne(wait, false);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }


        private void NotifySender(object state)
        {
            try
            {
                while (true)
                {
                    NotifyRequest request = null;
                    lock (requests)
                    {
                        if (requests.Any())
                        {
                            request = requests.Dequeue();
                        }
                    }
                    if (request != null)
                    {
                        if (AfterTransferRequest != null)
                        {
                            AfterTransferRequest(this, request);
                        }
                        try
                        {
                            SendNotify(request);
                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                    }
                    else
                    {
                        requestsEvent.WaitOne();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }


        private NotifyResult SendNotify(NotifyRequest request)
        {
            var sendResponces = new List<SendResponse>();

            var response = CheckPreventInterceptors(request, InterceptorPlace.Prepare, null);
            if (response != null)
            {
                sendResponces.Add(response);
            }
            else
            {
                sendResponces.AddRange(SendGroupNotify(request));
            }

            NotifyResult result = null;
            if (sendResponces == null || sendResponces.Count == 0)
            {
                result = new NotifyResult(SendResult.OK, sendResponces);
            }
            else
            {
                result = new NotifyResult(sendResponces.Aggregate((SendResult)0, (s, r) => s |= r.Result), sendResponces);
            }
            log.Debug(result);
            return result;
        }

        private SendResponse CheckPreventInterceptors(NotifyRequest request, InterceptorPlace place, string sender)
        {
            return request.Intercept(place) ? new SendResponse(request.NotifyAction, sender, request.Recipient, SendResult.Prevented) : null;
        }

        private List<SendResponse> SendGroupNotify(NotifyRequest request)
        {
            var responces = new List<SendResponse>();
            SendGroupNotify(request, responces);
            return responces;
        }

        private void SendGroupNotify(NotifyRequest request, List<SendResponse> responces)
        {
            if (request.Recipient is IDirectRecipient)
            {
                var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(request.NotifySource);
                if (!request.IsNeedCheckSubscriptions || !subscriptionSource.IsUnsubscribe(request.Recipient as IDirectRecipient, request.NotifyAction, request.ObjectID))
                {
                    var directresponses = new List<SendResponse>(1);
                    try
                    {
                        directresponses = SendDirectNotify(request);
                    }
                    catch (Exception exc)
                    {
                        directresponses.Add(new SendResponse(request.NotifyAction, request.Recipient, exc));
                    }
                    responces.AddRange(directresponses);
                }
            }
            else
            {
                if (request.Recipient is IRecipientsGroup)
                {
                    var checkresp = CheckPreventInterceptors(request, InterceptorPlace.GroupSend, null);
                    if (checkresp != null)
                    {
                        responces.Add(checkresp);
                    }
                    else
                    {
                        var recipientProvider = ProviderResolver.GetEnsure<IRecipientProvider>(request.NotifySource);

                        try
                        {
                            var recipients = recipientProvider.GetGroupEntries(request.Recipient as IRecipientsGroup, request.ObjectID) ?? new IRecipient[0];
                            foreach (var recipient in recipients)
                            {
                                try
                                {
                                    var newRequest = request.Split(recipient);
                                    SendGroupNotify(newRequest, responces);
                                }
                                catch (Exception exc)
                                {
                                    responces.Add(new SendResponse(request.NotifyAction, request.Recipient, exc));
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            responces.Add(new SendResponse(request.NotifyAction, request.Recipient, exc) { Result = SendResult.IncorrectRecipient });
                        }
                    }
                }
                else
                {
                    responces.Add(new SendResponse(request.NotifyAction, request.Recipient, null)
                    {
                        Result = SendResult.IncorrectRecipient,
                        Exception = new NotifyException("recipient may be IRecipientsGroup or IDirectRecipient")
                    });
                }
            }
        }

        private List<SendResponse> SendDirectNotify(NotifyRequest request)
        {
            if (!(request.Recipient is IDirectRecipient)) throw new ArgumentException("request.Recipient not IDirectRecipient", "request");

            var responses = new List<SendResponse>();
            var response = CheckPreventInterceptors(request, InterceptorPlace.DirectSend, null);
            if (response != null)
            {
                responses.Add(response);
                return responses;
            }

            try
            {
                PrepareRequestFillSenders(request);
                PrepareRequestFillPatterns(request);
                PrepareRequestFillTags(request);
            }
            catch (Exception)
            {
                responses.Add(new SendResponse(request.NotifyAction, null, request.Recipient, SendResult.Impossible));
            }

            if (request.SenderNames != null && request.SenderNames.Length > 0)
            {
                foreach (var sendertag in request.SenderNames)
                {
                    var channel = context.SenderHolder.GetSender(sendertag);
                    if (channel != null)
                    {
                        try
                        {
                            response = SendDirectNotify(request, channel);
                        }
                        catch (Exception exc)
                        {
                            response = new SendResponse(request.NotifyAction, channel.SenderName, request.Recipient, exc);
                        }
                    }
                    else
                    {
                        response = new SendResponse(request.NotifyAction, sendertag, request.Recipient, new NotifyException(String.Format("Not registered sender \"{0}\".", sendertag)));
                    }
                    responses.Add(response);
                }
            }
            else
            {
                response = new SendResponse(request.NotifyAction, request.Recipient, new NotifyException("Notice hasn't any senders."));
                responses.Add(response);
            }
            return responses;
        }

        private SendResponse SendDirectNotify(NotifyRequest request, ISenderChannel channel)
        {
            var recipient = request.Recipient as IDirectRecipient;
            if (recipient == null) throw new ArgumentException("request.Recipient not IDirectRecipient", "request");

            request.CurrentSender = channel.SenderName;

            NoticeMessage noticeMessage;
            var oops = CreateNoticeMessageFromNotifyRequest(request, channel.SenderName, out noticeMessage);
            if (oops != null) return oops;

            request.CurrentMessage = noticeMessage;
            var preventresponse = CheckPreventInterceptors(request, InterceptorPlace.MessageSend, channel.SenderName);
            if (preventresponse != null) return preventresponse;

            channel.SendAsync(noticeMessage);

            return new SendResponse(noticeMessage, channel.SenderName, SendResult.Inprogress);
        }

        private SendResponse CreateNoticeMessageFromNotifyRequest(NotifyRequest request, string sender, out NoticeMessage noticeMessage)
        {
            if (request == null) throw new ArgumentNullException("request");

            var recipientProvider = ProviderResolver.GetEnsure<IRecipientProvider>(request.NotifySource);
            var recipient = request.Recipient as IDirectRecipient;

            var addresses = recipient.Addresses;
            if (addresses == null || !addresses.Any())
            {
                addresses = recipientProvider.GetRecipientAddresses(request.Recipient as IDirectRecipient, sender, request.ObjectID);
                recipient = new DirectRecipient(request.Recipient.ID, request.Recipient.Name, addresses);
            }

            recipient = recipientProvider.FilterRecipientAddresses(recipient);
            noticeMessage = request.CreateMessage(recipient);

            addresses = recipient.Addresses;
            if (addresses == null || !addresses.Any(a => !string.IsNullOrEmpty(a)))
            {
                //checking addresses
                return new SendResponse(request.NotifyAction, sender, recipient, new NotifyException(string.Format("For recipient {0} by sender {1} no one addresses getted.", recipient, sender)));
            }

            if (request.IsNeedPatternFormatting)
            {
                var pattern = request.GetSenderPattern(sender);
                if (pattern == null)
                {
                    return new SendResponse(request.NotifyAction, sender, recipient, new NotifyException(String.Format("For action \"{0}\" by sender \"{1}\" no one patterns getted.", request.NotifyAction, sender)));
                }

                noticeMessage.Pattern = pattern;
                noticeMessage.ContentType = pattern.ContentType;

                if (request.RequaredTags.Count > 0)
                {
                    var dependencyProvider = ProviderResolver.Get<IDependencyProvider>(request.NotifySource);
                    if (dependencyProvider != null)
                    {
                        try
                        {
                            var values = dependencyProvider.GetDependencies(noticeMessage, request.ObjectID, request.RequaredTags.ToArray());
                            request.Arguments.AddRange(values ?? new ITagValue[0]);
                        }
                        catch (Exception exc)
                        {
                            return new SendResponse(noticeMessage, sender, exc);
                        }
                    }
                }

                noticeMessage.AddArgument(request.Arguments.ToArray());
                var patternProvider = ProviderResolver.GetEnsure<IPatternProvider>(request.NotifySource);

                var formatter = patternProvider.GetFormatter(pattern) ?? new NullPatternFormatter();
                try
                {
                    formatter.FormatMessage(noticeMessage, noticeMessage.Arguments);

                    sysTagFormatter.FormatMessage(
                        noticeMessage, new[]
                                           {
                                               new TagValue(Context._SYS_RECIPIENT_ID, request.Recipient.ID),
                                               new TagValue(Context._SYS_RECIPIENT_NAME, request.Recipient.Name),
                                               new TagValue(Context._SYS_RECIPIENT_ADDRESS, addresses != null && addresses.Length > 0 ? addresses[0] : null)
                                           }
                        );
                    //Do styling here
                    if (!string.IsNullOrEmpty(pattern.Styler))
                    {
                        //We need to run through styler before templating
                        StyleMessage(noticeMessage);
                    }
                }
                catch (Exception exc)
                {
                    return new SendResponse(request.NotifyAction, sender, recipient, exc);
                }
            }
            return null;
        }

        private void StyleMessage(NoticeMessage message)
        {
            try
            {
                if (!stylers.ContainsKey(message.Pattern.Styler))
                {
                    var styler = Activator.CreateInstance(Type.GetType(message.Pattern.Styler, true)) as IPatternStyler;
                    if (styler != null)
                    {
                        stylers.Add(message.Pattern.Styler, styler);
                    }
                }
                stylers[message.Pattern.Styler].ApplyFormating(message);
            }
            catch (Exception exc)
            {
                log.Warn("error styling message", exc);
            }
        }

        private void PrepareRequestFillSenders(NotifyRequest request)
        {
            if (!request.IsNeedRetriveSenders) return;

            var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(request.NotifySource);
            request.SenderNames = subscriptionSource.GetSubscriptionMethod(request.NotifyAction, request.Recipient) ?? new string[0];
        }

        private void PrepareRequestFillPatterns(NotifyRequest request)
        {
            if (!request.IsNeedRetrivePatterns) return;

            request.Patterns = new IPattern[request.SenderNames.Length];
            if (request.Patterns.Length == 0) return;

            var apProvider = ProviderResolver.GetEnsure<IActionPatternProvider>(request.NotifySource);
            for (var i = 0; i < request.SenderNames.Length; i++)
            {
                var senderName = request.SenderNames[i];
                var pattern = (apProvider.GetPatternMethod != null ? apProvider.GetPatternMethod(request.NotifyAction, senderName, request) : null) ??
                    apProvider.GetPattern(request.NotifyAction, senderName) ??
                    apProvider.GetPattern(request.NotifyAction);

                if (pattern == null)
                {
                    throw new NotifyException(string.Format("For action \"{0}\" by sender \"{1}\" no one patterns getted.", request.NotifyAction.Name, senderName));
                }
                request.Patterns[i] = pattern;
            }
        }

        private void PrepareRequestFillTags(NotifyRequest request)
        {
            if (!request.IsNeedRetriveTags) return;

            var patternProvider = ProviderResolver.GetEnsure<IPatternProvider>(request.NotifySource);
            foreach (var pattern in request.Patterns)
            {
                IPatternFormatter formatter;
                try
                {
                    formatter = patternProvider.GetFormatter(pattern) ?? new NullPatternFormatter();
                }
                catch (Exception exc)
                {
                    throw new NotifyException(string.Format("For pattern \"{0}\" formatter not instanced.", pattern), exc);
                }
                ITag[] tags;
                try
                {
                    tags = formatter.GetTags(pattern) ?? new ITag[0];
                }
                catch (Exception exc)
                {
                    throw new NotifyException(string.Format("Get tags from formatter of pattern \"{0}\" failed.", pattern), exc);
                }

                foreach (var tag in tags.Where(tag => !request.Arguments.Exists(tagValue => Equals(tagValue.Tag, tag)) && !request.RequaredTags.Exists(rtag => Equals(rtag, tag))))
                {
                    request.RequaredTags.Add(tag);
                }
            }
        }


        private class SendMethodWrapper
        {
            private readonly INotifyClient client;
            private readonly Func<DateTime, DateTime?> nextDateMethod;
            private readonly Action<DateTime> method;
            private bool invoke;

            public DateTime? ScheduleDate { get; private set; }

            public SendMethodWrapper(INotifyClient client, Action<DateTime> method, Func<DateTime, DateTime?> nextDateMethod)
            {
                this.client = client;
                this.method = method;
                this.nextDateMethod = nextDateMethod;
                UpdateScheduleDate(DateTime.UtcNow);
            }

            public void UpdateScheduleDate(DateTime d)
            {
                try
                {
                    if (nextDateMethod != null)
                    {
                        ScheduleDate = nextDateMethod(d);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }

            public void InvokeSendMethod(DateTime d)
            {
                if (!invoke)
                {
                    method.BeginInvoke(d, InvokeSendCallback, method);
                    invoke = true;
                }
                else
                {
                    log.WarnFormat("InvokeSendMethod: {0} at {1} send but previus call not finished.", method.Method.Name, d);
                }
            }

            private void InvokeSendCallback(IAsyncResult ar)
            {
                try
                {
                    var m = (Action<DateTime>)ar.AsyncState;
                    m.EndInvoke(ar);
                }
                catch (Exception error)
                {
                    log.Error(error);
                }
                finally
                {
                    invoke = false;
                }
            }

            public override bool Equals(object obj)
            {
                var w = obj as SendMethodWrapper;
                return w != null && client.Equals(w.client) && method.Equals(w.method);
            }

            public override int GetHashCode()
            {
                return client.GetHashCode() | method.GetHashCode();
            }
        }
    }
}
