using System;
using System.Collections.Generic;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Notify
{
    public delegate void SendNoticeCallback(INotifyAction action, string objectID, IRecipient recipient, NotifyResult result);

    public interface INotifyClient
    {
        void SetStaticTags(IEnumerable<ITagValue> tagValues);


        void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, SendNoticeCallback sendCallback, params ITagValue[] args);

        void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, SendNoticeCallback sendCallback, params ITagValue[] args);

        void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args);

        void SendNoticeAsync(INotifyAction action, string objectID, SendNoticeCallback sendCallback, params ITagValue[] args);

        void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, SendNoticeCallback sendCallback, params ITagValue[] args);

        void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args);


        void RegisterSendMethod(Action<DateTime> method, TimeSpan period, DateTime startDate);

        void UnregisterSendMethod(Action<DateTime> method);


        void BeginSingleRecipientEvent(string name);

        void EndSingleRecipientEvent(string name);

        void AddInterceptor(ISendInterceptor interceptor);

        void RemoveInterceptor(string name);
    }
}