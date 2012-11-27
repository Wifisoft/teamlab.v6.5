using System;
using System.Collections.Generic;
using ASC.Notify.Model;

namespace ASC.Forum
{
    public interface ISubscriberView
    {
        bool IsSubscribe { get; set; }

        event EventHandler<SubscribeEventArgs> Subscribe;

        event EventHandler<SubscribeEventArgs> UnSubscribe;

        event EventHandler<SubscribeEventArgs> UnSubscribeForSubscriptionType;        
        
        event EventHandler<SubscribeEventArgs> GetSubscriptionState;
    }

    public class SubscribeEventArgs : EventArgs
    {
        public INotifyAction NotifyAction { get; private set; }

        public string ObjectID { get; private set; }

        public Guid UserID { get; private set; }       
        
        public SubscribeEventArgs(INotifyAction notifyAction, string objectID, Guid userID)
        {
            this.NotifyAction = notifyAction;
            this.ObjectID = objectID;
            this.UserID = userID;            
        }
    }
}
