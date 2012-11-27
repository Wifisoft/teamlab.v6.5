using System;
using System.Collections.Generic;
using ASC.Notify.Engine;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Notify.Model
{
    class NotifyClientImpl : INotifyClient
    {
        private readonly Context ctx;
        private readonly InterceptorStorage interceptors = new InterceptorStorage();
        private readonly INotifySource notifySource;
        private readonly List<ITagValue> tags = new List<ITagValue>();


        public NotifyClientImpl(Context context, INotifySource notifySource)
        {
            if (notifySource == null) throw new ArgumentNullException("notifySource");
            if (context == null) throw new ArgumentNullException("context");

            this.notifySource = notifySource;
            ctx = context;
        }


        public void SetStaticTags(IEnumerable<ITagValue> tagValues)
        {
            lock (tags)
            {
                if (tagValues != null)
                {
                    tags.AddRange(tagValues);
                }
                else
                {
                    tags.Clear();
                }
            }
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, recipients, senderNames, sendCallback, false, args);
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, recipients, null, sendCallback, false, args);
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, recipients, null, null, checkSubscription, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, new[] { recipient }, null, sendCallback, false, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(notifySource);
            var recipients = subscriptionSource.GetRecipients(action, objectID);
            SendNoticeToAsync(action, objectID, recipients, null, sendCallback, false, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args)
        {
            SendNoticeToAsync(action, objectID, new[] { recipient }, null, null, checkSubscription, args);
        }



        public void RegisterSendMethod(Action<DateTime> method, TimeSpan period, DateTime startDate)
        {
            RegisterSendMethod(method, d => (d < startDate) ? startDate : d + period);
        }

        public void UnregisterSendMethod(Action<DateTime> method)
        {
            ctx.NotifyEngine.UnregisterSendMethod(this, method);
        }

        public void BeginSingleRecipientEvent(string name)
        {
            interceptors.Add(new SingleRecipientInterceptor(name));
        }

        public void EndSingleRecipientEvent(string name)
        {
            interceptors.Remove(name);
        }

        public void AddInterceptor(ISendInterceptor interceptor)
        {
            interceptors.Add(interceptor);
        }

        public void RemoveInterceptor(string name)
        {
            interceptors.Remove(name);
        }


        private void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, SendNoticeCallback sendCallback, bool checkSubsciption, params ITagValue[] args)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");

            BeginSingleRecipientEvent("__syspreventduplicateinterceptor");

            foreach (var recipient in recipients)
            {
                var r = CreateRequest(action, objectID, recipient, sendCallback, args, senderNames, checkSubsciption);
                SendAsync(r);
            }
        }

        private void RegisterSendMethod(Action<DateTime> method, Func<DateTime, DateTime?> nextDate)
        {
            ctx.NotifyEngine.RegisterSendMethod(this, method, nextDate);
        }

        private void SendAsync(NotifyRequest request)
        {
            request.Interceptors = interceptors.GetAll();
            AddStaticTags(request);
            ctx.NotifyEngine.QueueRequest(request);
        }

        private void AddStaticTags(NotifyRequest request)
        {
            lock (tags)
            {
                foreach (var t in tags)
                {
                    if (t != null && !request.Arguments.Exists(a => a.Tag.Name == t.Tag.Name))
                    {
                        request.Arguments.Add(t);
                    }
                }
            }
        }

        private NotifyRequest CreateRequest(INotifyAction action, string objectID, IRecipient recipient, SendNoticeCallback sendCallback, ITagValue[] args, string[] senders, bool checkSubsciption)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (sendCallback != null) throw new NotImplementedException("sendCallback");

            var request = new NotifyRequest(notifySource, action, objectID, recipient);
            request.SenderNames = senders;
            request.IsNeedCheckSubscriptions = checkSubsciption;
            if (args != null) request.Arguments.AddRange(args);
            return request;
        }
    }
}